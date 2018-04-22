using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.IO;
//using PGA.OpenDWG.DgnPurger;
using ACADDB = Autodesk.AutoCAD.DatabaseServices;
using ACADRT = Autodesk.AutoCAD.Runtime;
namespace PGA.OpenDWG
{
    internal class PolylineFunctions
    {
    }

    public class Commands
    {
        private const string dgnLsDefName = "DGNLSDEF";
        private const string dgnLsDictName = "ACAD_DGNLINESTYLECOMP";

        public struct ads_name
        {
            public IntPtr a;
            public IntPtr b;
        };

        [CommandMethod("DGNPURGE")]
        public static void PurgeDgnLinetypes()
        {
            var doc =
                Application.DocumentManager.MdiActiveDocument;
            PurgeDgnLinetypesInDb(doc.Database, doc.Editor);
        }

        [CommandMethod("DGNPURGEEXT")]
        public static void PurgeDgnLinetypesExt()
        {
            var doc =
                Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            var pofo = new PromptOpenFileOptions("\nSelect file to purge");

            // Use the command-line version if FILEDIA is 0 or
            // CMDACTIVE indicates we're being called from a script
            // or from LISP

            short fd = (short) Application.GetSystemVariable("FILEDIA");
            short ca = (short) Application.GetSystemVariable("CMDACTIVE");

            pofo.PreferCommandLine = (fd == 0 || (ca & 36) > 0);
            pofo.Filter = "DWG (*.dwg)|*.dwg|All files (*.*)|*.*";

            // Ask the user to select a DWG file to purge

            var pfnr = ed.GetFileNameForOpen(pofo);
            if (pfnr.Status == PromptStatus.OK)
            {
                // Make sure the file exists
                // (it should unless entered via the command-line)

                if (!File.Exists(pfnr.StringResult))
                {
                    ed.WriteMessage(
                        "\nCould not find file: \"{0}\".",
                        pfnr.StringResult
                        );
                    return;
                }

                try
                {
                    // We'll just suffix the selected filename with "-purged"
                    // for the output location. This file will be overwritten
                    // if the command is called multiple times

                    var output =
                        Path.GetDirectoryName(pfnr.StringResult) + "\\" +
                        Path.GetFileNameWithoutExtension(pfnr.StringResult) +
                        "-purged" +
                        Path.GetExtension(pfnr.StringResult);

                    // Assume a post-R12 drawing

                    using (var db = new ACADDB.Database(false, true))
                    {
                        // Read the DWG file into our Database object

                        db.ReadDwgFile(
                            pfnr.StringResult,
                            FileOpenMode.OpenForReadAndReadShare,
                            false,
                            ""
                            );

                        // No graphical changes, so we can keep the preview
                        // bitmap

                        db.RetainOriginalThumbnailBitmap = true;

                        // We'll store the current working database, to reset
                        // after the purge operation

                        var wdb = HostApplicationServices.WorkingDatabase;
                        HostApplicationServices.WorkingDatabase = db;

                        // Purge unused DGN linestyles from the drawing
                        // (returns false if nothing is erased)

                        if (PurgeDgnLinetypesInDb(db, ed))
                        {
                            // Check the version of the drawing to save back to

                            var ver =
                                (db.LastSavedAsVersion == DwgVersion.MC0To0
                                    ? DwgVersion.Current
                                    : db.LastSavedAsVersion
                                    );

                            // Now we can save

                            db.SaveAs(output, ver);

                            ed.WriteMessage(
                                "\nSaved purged file to \"{0}\".",
                                output
                                );
                        }

                        // Still need to reset the working database

                        HostApplicationServices.WorkingDatabase = wdb;
                    }
                }
                catch (ACADRT.Exception ex)
                {
                    ed.WriteMessage("\nException: {0}", ex.Message);
                }
            }
        }

        // Helper function to be shared between our command
        // implementations

        private static bool PurgeDgnLinetypesInDb(ACADDB.Database db, Editor ed)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                // Start by getting all the "complex" DGN linetypes
                // from the linetype table

                var linetypes = CollectComplexLinetypeIds(db, tr);

                // Store a count before we start removing the ones
                // that are referenced

                var ltcnt = linetypes.Count;

                // Remove any from the "to remove" list that need to be
                // kept (as they have references from objects other
                // than anonymous blocks)

                var ltsToKeep =
                    PurgeLinetypesReferencedNotByAnonBlocks(db, tr, linetypes);

                // Now we collect the DGN stroke entries from the NOD

                var strokes = CollectStrokeIds(db, tr);

                // Store a count before we start removing the ones
                // that are referenced

                var strkcnt = strokes.Count;

                // Open up each of the "keeper" linetypes, and go through
                // their data, removing any NOD entries from the "to
                // remove" list that are referenced

                PurgeStrokesReferencedByLinetypes(tr, ltsToKeep, strokes);

                // Erase each of the NOD entries that are safe to remove

                int erasedStrokes = 0;

                foreach (ObjectId id in strokes)
                {
                    try
                    {
                        var obj = tr.GetObject(id, OpenMode.ForWrite);
                        obj.Erase();
                        if (
                            obj.GetRXClass().Name.Equals("AcDbLSSymbolComponent")
                            )
                        {
                            EraseReferencedAnonBlocks(tr, obj);
                        }
                        erasedStrokes++;
                    }
                    catch (System.Exception ex)
                    {
                        ed.WriteMessage(
                            "\nUnable to erase stroke ({0}): {1}",
                            id.ObjectClass.Name,
                            ex.Message
                            );
                    }
                }

                // And the same for the complex linetypes

                int erasedLinetypes = 0;

                foreach (ObjectId id in linetypes)
                {
                    try
                    {
                        var obj = tr.GetObject(id, OpenMode.ForWrite);
                        obj.Erase();
                        erasedLinetypes++;
                    }
                    catch (System.Exception ex)
                    {
                        ed.WriteMessage(
                            "\nUnable to erase linetype ({0}): {1}",
                            id.ObjectClass.Name,
                            ex.Message
                            );
                    }
                }

                // Remove the DGN stroke dictionary from the NOD if empty

                bool erasedDict = false;

                var nod =
                    (DBDictionary) tr.GetObject(
                        db.NamedObjectsDictionaryId, OpenMode.ForRead
                        );

                ed.WriteMessage(
                    "\nPurged {0} unreferenced complex linetype records" +
                    " (of {1}).",
                    erasedLinetypes, ltcnt
                    );

                ed.WriteMessage(
                    "\nPurged {0} unreferenced strokes (of {1}).",
                    erasedStrokes, strkcnt
                    );

                if (nod.Contains(dgnLsDictName))
                {
                    var dgnLsDict =
                        (DBDictionary) tr.GetObject(
                            (ObjectId) nod[dgnLsDictName],
                            OpenMode.ForRead
                            );

                    if (dgnLsDict.Count == 0)
                    {
                        dgnLsDict.UpgradeOpen();
                        dgnLsDict.Erase();

                        ed.WriteMessage(
                            "\nRemoved the empty DGN linetype stroke dictionary."
                            );

                        erasedDict = true;
                    }
                }

                tr.Commit();

                // Return whether we have actually found anything to erase

                return (
                    erasedLinetypes > 0 || erasedStrokes > 0 || erasedDict
                    );
            }
        }

        // Collect the complex DGN linetypes from the linetype table

        private static ObjectIdCollection CollectComplexLinetypeIds(
            ACADDB.Database db, Transaction tr
            )
        {
            var ids = new ObjectIdCollection();

            var lt =
                (LinetypeTable) tr.GetObject(
                    db.LinetypeTableId, OpenMode.ForRead
                    );
            foreach (var ltId in lt)
            {
                // Complex DGN linetypes have an extension dictionary
                // with a certain record inside

                var obj = tr.GetObject(ltId, OpenMode.ForRead);
                if (obj.ExtensionDictionary != ObjectId.Null)
                {
                    var exd =
                        (DBDictionary) tr.GetObject(
                            obj.ExtensionDictionary, OpenMode.ForRead
                            );
                    if (exd.Contains(dgnLsDefName))
                    {
                        ids.Add(ltId);
                    }
                }
            }
            return ids;
        }

        // Collect the DGN stroke entries from the NOD

        private static ObjectIdCollection CollectStrokeIds(
            ACADDB.Database db, Transaction tr
            )
        {
            var ids = new ObjectIdCollection();

            var nod =
                (DBDictionary) tr.GetObject(
                    db.NamedObjectsDictionaryId, OpenMode.ForRead
                    );

            // Strokes are stored in a particular dictionary

            if (nod.Contains(dgnLsDictName))
            {
                var dgnDict =
                    (DBDictionary) tr.GetObject(
                        (ObjectId) nod[dgnLsDictName],
                        OpenMode.ForRead
                        );

                foreach (var item in dgnDict)
                {
                    ids.Add(item.Value);
                }
            }

            return ids;
        }

        // Remove the linetype IDs that have references from objects
        // other than anonymous blocks from the list passed in,
        // returning the ones removed in a separate list

        private static ObjectIdCollection
            PurgeLinetypesReferencedNotByAnonBlocks(
            ACADDB.Database db, Transaction tr, ObjectIdCollection ids
            )
        {
            var keepers = new ObjectIdCollection();

            // Open the block table record

            var bt =
                (BlockTable) tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            foreach (var btrId in bt)
            {
                // Open each block definition in the drawing

                var btr =
                    (BlockTableRecord) tr.GetObject(btrId, OpenMode.ForRead);

                // And open each entity in each block

                foreach (var id in btr)
                {
                    // Open the object and check its linetype

                    var obj = tr.GetObject(id, OpenMode.ForRead, true);
                    var ent = obj as Entity;
                    if (ent != null && !ent.IsErased)
                    {
                        if (ids.Contains(ent.LinetypeId))
                        {
                            // If the owner does not belong to an anonymous
                            // block, then we take it seriously as a reference

                            var owner =
                                (BlockTableRecord) tr.GetObject(
                                    ent.OwnerId, OpenMode.ForRead
                                    );
                            if (
                                !owner.Name.StartsWith("*") ||
                                owner.Name.ToUpper() == BlockTableRecord.ModelSpace ||
                                owner.Name.ToUpper().StartsWith(
                                    BlockTableRecord.PaperSpace
                                    )
                                )
                            {
                                // Move the linetype ID from the "to remove" list
                                // to the "to keep" list

                                ids.Remove(ent.LinetypeId);
                                keepers.Add(ent.LinetypeId);
                            }
                        }
                    }
                }
            }
            return keepers;
        }

        // Remove the stroke objects that have references from
        // complex linetypes (or from other stroke objects, as we
        // recurse) from the list passed in

        private static void PurgeStrokesReferencedByLinetypes(
            Transaction tr,
            ObjectIdCollection tokeep,
            ObjectIdCollection nodtoremove
            )
        {
            foreach (ObjectId id in tokeep)
            {
                PurgeStrokesReferencedByObject(tr, nodtoremove, id);
            }
        }

        // Remove the stroke objects that have references from this
        // particular complex linetype or stroke object from the list
        // passed in

        private static void PurgeStrokesReferencedByObject(
            Transaction tr, ObjectIdCollection nodIds, ObjectId id
            )
        {
            var obj = tr.GetObject(id, OpenMode.ForRead);
            if (obj.ExtensionDictionary != ObjectId.Null)
            {
                // Get the extension dictionary

                var exd =
                    (DBDictionary) tr.GetObject(
                        obj.ExtensionDictionary, OpenMode.ForRead
                        );

                // And the "DGN Linestyle Definition" object

                if (exd.Contains(dgnLsDefName))
                {
                    var lsdef =
                        tr.GetObject(
                            exd.GetAt(dgnLsDefName), OpenMode.ForRead
                            );

                    // Use a DWG filer to extract the references

                    var refFiler = new ReferenceFiler();
                    lsdef.DwgOut(refFiler);

                    // Loop through the references and remove any from the
                    // list passed in

                    foreach (ObjectId refid in refFiler.HardPointerIds)
                    {
                        if (nodIds.Contains(refid))
                        {
                            nodIds.Remove(refid);
                        }

                        // We need to recurse, as linetype strokes can reference
                        // other linetype strokes

                        PurgeStrokesReferencedByObject(tr, nodIds, refid);
                    }
                }
            }
            else if (
                obj.GetRXClass().Name.Equals("AcDbLSCompoundComponent") ||
                obj.GetRXClass().Name.Equals("AcDbLSPointComponent")
                )
            {
                // We also need to consider compound components, which
                // don't use objects in their extension dictionaries to
                // manage references to strokes...

                // Use a DWG filer to extract the references from the
                // object itself

                var refFiler = new ReferenceFiler();
                obj.DwgOut(refFiler);

                // Loop through the references and remove any from the
                // list passed in

                foreach (ObjectId refid in refFiler.HardPointerIds)
                {
                    if (nodIds.Contains(refid))
                    {
                        nodIds.Remove(refid);
                    }

                    // We need to recurse, as linetype strokes can reference
                    // other linetype strokes

                    PurgeStrokesReferencedByObject(tr, nodIds, refid);
                }
            }
        }

        // Erase the anonymous blocks referenced by an object

        private static void EraseReferencedAnonBlocks(
            Transaction tr, DBObject obj
            )
        {
            var refFiler = new ReferenceFiler();
            obj.DwgOut(refFiler);

            // Loop through the references and erase any
            // anonymous block definitions
            //
            foreach (ObjectId refid in refFiler.HardPointerIds)
            {
                BlockTableRecord btr =
                    tr.GetObject(refid, OpenMode.ForRead) as BlockTableRecord;
                if (btr != null && btr.IsAnonymous)
                {
                    btr.UpgradeOpen();
                    btr.Erase();
                }
            }
        }
    }


        public class ReferenceFiler : DwgFiler
        {
            public ObjectIdCollection HardPointerIds;
            public ObjectIdCollection SoftPointerIds;
            public ObjectIdCollection HardOwnershipIds;
            public ObjectIdCollection SoftOwnershipIds;

            public ReferenceFiler()
            {
                HardPointerIds = new ObjectIdCollection();
                SoftPointerIds = new ObjectIdCollection();
                HardOwnershipIds = new ObjectIdCollection();
                SoftOwnershipIds = new ObjectIdCollection();
            }

            public override ErrorStatus FilerStatus
            {
                get { return ErrorStatus.OK; }

                set { }
            }

            public override FilerType FilerType
            {
                get { return FilerType.IdFiler; }
            }

            public override long Position
            {
                get { return 0; }
            }

            public override IntPtr ReadAddress() { return new IntPtr(); }
            public override byte[] ReadBinaryChunk() { return null; }
            public override bool ReadBoolean() { return true; }
            public override byte ReadByte() { return new byte(); }
            public override void ReadBytes(byte[] value) { }
            public override double ReadDouble() { return 0.0; }
            public override Handle ReadHandle() { return new Handle(); }
            public override ObjectId ReadHardOwnershipId()
            {
                return ObjectId.Null;
            }
            public override ObjectId ReadHardPointerId()
            {
                return ObjectId.Null;
            }
            public override short ReadInt16() { return 0; }
            public override int ReadInt32() { return 0; }
            public override long ReadInt64() { return 0; }
            public override Point2d ReadPoint2d() { return new Point2d(); }
            public override Point3d ReadPoint3d() { return new Point3d(); }
            public override Scale3d ReadScale3d() { return new Scale3d(); }
            public override ObjectId ReadSoftOwnershipId()
            {
                return ObjectId.Null;
            }
            public override ObjectId ReadSoftPointerId()
            {
                return ObjectId.Null;
            }
            public override string ReadString() { return null; }
            public override ushort ReadUInt16() { return 0; }
            public override uint ReadUInt32() { return 0; }
            public override ulong ReadUInt64() { return 0; }
            public override Vector2d ReadVector2d()
            {
                return new Vector2d();
            }
            public override Vector3d ReadVector3d()
            {
                return new Vector3d();
            }

            public override void ResetFilerStatus() { }
            public override void Seek(long offset, int method) { }

            public override void WriteAddress(IntPtr value) { }
            public override void WriteBinaryChunk(byte[] chunk) { }
            public override void WriteBoolean(bool value) { }
            public override void WriteByte(byte value) { }
            public override void WriteBytes(byte[] value) { }
            public override void WriteDouble(double value) { }
            public override void WriteHandle(Handle handle) { }
            public override void WriteInt16(short value) { }
            public override void WriteInt32(int value) { }
            public override void WriteInt64(long value) { }
            public override void WritePoint2d(Point2d value) { }
            public override void WritePoint3d(Point3d value) { }
            public override void WriteScale3d(Scale3d value) { }
            public override void WriteString(string value) { }
            public override void WriteUInt16(ushort value) { }
            public override void WriteUInt32(uint value) { }
            public override void WriteUInt64(ulong value) { }
            public override void WriteVector2d(Vector2d value) { }
            public override void WriteVector3d(Vector3d value) { }

            public override void WriteHardOwnershipId(ObjectId value)
            {
                HardOwnershipIds.Add(value);
            }

            public override void WriteHardPointerId(ObjectId value)
            {
                HardPointerIds.Add(value);
            }

            public override void WriteSoftOwnershipId(ObjectId value)
            {
                SoftOwnershipIds.Add(value);
            }

            public override void WriteSoftPointerId(ObjectId value)
            {
                SoftPointerIds.Add(value);
            }

            public void reset()
            {
                HardPointerIds.Clear();
                SoftPointerIds.Clear();
                HardOwnershipIds.Clear();
                SoftOwnershipIds.Clear();
            }
        }
    }

