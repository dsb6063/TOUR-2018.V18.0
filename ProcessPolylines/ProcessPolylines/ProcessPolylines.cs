using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.Runtime;
using PGA.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pge.Common.Framework;
using global::Autodesk.AutoCAD.DatabaseServices;
using PGA.Autodesk.Utils;
using PGA.DataContext;
using BBC.Common.AutoCAD;
using Application = global::Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Polyline = global::Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace ProcessPolylines
{



    public class ProcessPolylines
    {
        public static IList<DrawingStack> DwDrawingStacks { get; set; } = new List<DrawingStack>();
        public static string GDwgPath { get; set; }



        /// <summary>
        /// Gets the type of the ids by.
        /// </summary>
        /// <returns>IList&lt;ObjectId&gt;.</returns>
        public IList<ObjectId> GetIdsByType()
        {
            Func<Type, RXClass> getClass = RXObject.GetClass;

            // You can set this anywhere
            var acceptableTypes = new HashSet<RXClass>
            {
                getClass(typeof (Polyline)),
                getClass(typeof (Polyline2d)),
                getClass(typeof (Polyline3d))
            };

            var doc = Application.DocumentManager.MdiActiveDocument;
            using (var trans = doc.TransactionManager.StartOpenCloseTransaction())
            {
                var modelspace = (BlockTableRecord)
                    trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(doc.Database), OpenMode.ForRead);

                var polylineIds = (from id in modelspace.Cast<ObjectId>()
                    where acceptableTypes.Contains(id.ObjectClass)
                    select id).ToList();

                trans.Commit();
                return polylineIds;
            }
        }

        public static ObjectIdCollection GetIdsByTypeTypeValue(params string[] types)
        {
            
            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;

            // Get the editor to make the selection
            Editor oEd = doc.Editor;

            // Add our or operators so we can grab multiple types.
            IList<TypedValue> typedValueSelection = new List<TypedValue> {
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>")
                };

            // We will need to insert our requested types into the collection.
            // Since we knew we would have to insert they types inbetween the operators..
            // I used a Enumerable type which gave me that functionallity. (IListf<T>)
            foreach (var type in types)
                typedValueSelection.Insert(1, new TypedValue(Convert.ToInt32(DxfCode.Start), type));

            SelectionFilter selectionFilter = new SelectionFilter(typedValueSelection.ToArray());

            // because we have to.. Not really sure why, I assume this is our only access point
            // to grab the entities that we want. (I am open to being corrected)
            PromptSelectionResult promptSelectionResult = oEd.SelectAll(selectionFilter);

            // return our new ObjectIdCollection that is "Hopefully" full of the types that we want.
            return new ObjectIdCollection(promptSelectionResult.Value.GetObjectIds());
        }

        public static ObjectIdCollection GetIdsByTypeCollection()
        {
            ObjectIdCollection collection = new ObjectIdCollection();

            Func<Type, RXClass> getClass = RXObject.GetClass;

            // You can set this anywhere
            var acceptableTypes = new HashSet<RXClass>
            {
                getClass(typeof (Polyline)),
                getClass(typeof (Polyline2d)),
                getClass(typeof (Polyline3d))
            };

            var doc = Application.DocumentManager.MdiActiveDocument;
            using (var trans = doc.TransactionManager.StartOpenCloseTransaction())
            {
                var modelspace = (BlockTableRecord)
                    trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(doc.Database), OpenMode.ForRead);

                var polylineIds = (from id in modelspace.Cast<ObjectId>()
                                   where acceptableTypes.Contains(id.ObjectClass)
                                   select id).ToList();

                foreach (var id in polylineIds)
                {
                    collection.Add(id);
                }


                trans.Commit();
                return collection;
            }
        }


        [CommandMethod("PGA-ProcessPolylines")]
        public static void LoadandProcessPolys()
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                #region Get Dwgs to Process

                var dwgs = commands.LoadandProcessPolys();

                foreach (var dwg in dwgs)
                {
                    DwDrawingStacks.Add(dwg);
                }

                #endregion

                GDwgPath = commands.GetGlobalDWGPath();

                var doc =
                Application.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;

                foreach (var dwg in DwDrawingStacks)
                {
                    string gpath = Convert.ToString(GDwgPath);

                    if (gpath != null)
                    {
                        string path = Path.Combine(gpath, dwg.PolylineDwgName);

                        if (path == null) throw new ArgumentNullException(nameof(path));


                        if (!File.Exists(path))
                        {
                            DatabaseLogs.FormatLogs("File Not Found", path);
                            return;

                        }

                        try
                        {
                            // We'll just suffix the selected filename with "-purged"
                            // for the output location. This file will be overwritten
                            // if the command is called multiple times

                            //var output =
                            //    Path.GetDirectoryName(pfnr.StringResult) + "\\" +
                            //    Path.GetFileNameWithoutExtension(pfnr.StringResult) +
                            //    "-purged" +
                            //    Path.GetExtension(pfnr.StringResult);

                            // Assume a post-R12 drawing

                            using (var db = new Autodesk.AutoCAD.DatabaseServices.Database(false, true))
                            {
                                // Read the DWG file into our Database object

                                db.ReadDwgFile(
                                    path,
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
                                ObjectIdCollection collection = GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");

                                CopyPolylinesBetweenDatabases(db,collection);
                                
                                // Still need to reset the working database

                                HostApplicationServices.WorkingDatabase = wdb;
                                wdb.Save();
                                string date = dwg.DateStamp.Value.ToFileTime().ToString();
                                string output = Path.Combine(commands.GetGlobalDestinationPath(dwg.DateStamp.Value), String.Format("HOLES-{0}", date));
                                FileUtilities.CreateDirectory(output);
                                PLineToLayers.ProcessLayers(collection,wdb);
                                output = Path.Combine(output, dwg.PolylineDwgName);
                                doc.Database.SaveAs(output,DwgVersion.Current);

                                //    if (PurgeDgnLinetypesInDb(db, ed))
                                //    {
                                //        // Check the version of the drawing to save back to

                                //        var ver =
                                //            (db.LastSavedAsVersion == DwgVersion.MC0To0
                                //                ? DwgVersion.Current
                                //                : db.LastSavedAsVersion
                                //                );

                                //        // Now we can save

                                //        string output = null;
                                //        db.SaveAs(output, ver);

                                //        ed.WriteMessage(
                                //            "\nSaved purged file to \"{0}\".",
                                //            output
                                //            );
                                //    }

                                //    // Still need to reset the working database

                                //    HostApplicationServices.WorkingDatabase = wdb;
                            }

                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            ed.WriteMessage("\nException: {0}", ex.Message);
                        }

                    }
                    break;
                }
            }
        }


        [CommandMethod("PGA-ChangeLayers")]
        public static void ChangePolylineLayers()
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                #region Get Dwgs to Process

                var dwgs = commands.LoadandProcessPolys();

                foreach (var dwg in dwgs)
                {
                    DwDrawingStacks.Add(dwg);
                }

                #endregion

                GDwgPath = commands.GetGlobalDWGPath();

                var doc =
                Application.DocumentManager.MdiActiveDocument;
                var ed = doc.Editor;

                foreach (var dwg in DwDrawingStacks)
                {
                    string gpath = Convert.ToString(GDwgPath);

                    if (gpath != null)
                    {
                        string path = Path.Combine(gpath, dwg.PolylineDwgName);

                        if (path == null) throw new ArgumentNullException(nameof(path));


                        if (!File.Exists(path))
                        {
                            DatabaseLogs.FormatLogs("File Not Found", path);
                            return;

                        }

                        try
                        {
                            // We'll just suffix the selected filename with "-purged"
                            // for the output location. This file will be overwritten
                            // if the command is called multiple times

                            //var output =
                            //    Path.GetDirectoryName(pfnr.StringResult) + "\\" +
                            //    Path.GetFileNameWithoutExtension(pfnr.StringResult) +
                            //    "-purged" +
                            //    Path.GetExtension(pfnr.StringResult);

                            // Assume a post-R12 drawing

                            using (var db = doc.Database)
                            {
                                                
                                ObjectIdCollection collection = GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");
 
                                //db.Save();
                                string date = dwg.DateStamp.Value.ToFileTime().ToString();
                                string output = Path.Combine(commands.GetGlobalDestinationPath(dwg.DateStamp.Value), String.Format("HOLES-{0}", date));
                                Pge.Common.Framework.FileUtilities.CreateDirectory(output);
                                PLineToLayers.ProcessLayers(collection, db);
                                output = Path.Combine(output, dwg.PolylineDwgName);
                                doc.Database.SaveAs(output, DwgVersion.Current);
 
                            }

                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            ed.WriteMessage("\nException: {0}", ex.Message);
                        }

                    }
                    break;
                }
            }
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

            short fd = (short)Application.GetSystemVariable("FILEDIA");
            short ca = (short)Application.GetSystemVariable("CMDACTIVE");

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

                    using (var db = new Autodesk.AutoCAD.DatabaseServices.Database(false, true))
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
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    ed.WriteMessage("\nException: {0}", ex.Message);
                }
            }
        }

        private static bool PurgeDgnLinetypesInDb(Database db, Editor ed)
        {
            throw new NotImplementedException();
        }


       // [CommandMethod("CopyPolylinesBetweenDatabases", CommandFlags.Session)]
        public static void CopyPolylinesBetweenDatabases(Database _sourcedb, ObjectIdCollection _collection)
        {
            ObjectIdCollection acObjIdColl = new ObjectIdCollection();
            acObjIdColl = _collection;

            // Get the current document and database 
            //Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = _sourcedb;// acDoc.Database;
            // Lock the current document 
            //using (DocumentLock acLckDocCur = acDoc.LockDocument())
            //{
            //    // Start a transaction 
            //    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            //    {
            //        #region MyRegion
            //        //// Open the Block table record for read
            //        //BlockTable acBlkTbl;
            //        //acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
            //        //    OpenMode.ForRead) as BlockTable;
            //        //// Open the Block table record Model space for write
            //        //BlockTableRecord acBlkTblRec;
            //        //acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
            //        //    OpenMode.ForWrite) as BlockTableRecord;
            //        //// Create a circle that is at (0,0,0) with a radius of 5
            //        //Circle acCirc1 = new Circle();
            //        //acCirc1.Center = new Point3d(0, 0, 0);
            //        //acCirc1.Radius = 5;
            //        //// Add the new object to the block table record and the transaction
            //        //acBlkTblRec.AppendEntity(acCirc1);
            //        //acTrans.AddNewlyCreatedDBObject(acCirc1, true);
            //        //// Create a circle that is at (0,0,0) with a radius of 7
            //        //Circle acCirc2 = new Circle();
            //        //acCirc2.Center = new Point3d(0, 0, 0);
            //        //acCirc2.Radius = 7;
            //        //// Add the new object to the block table record and the transaction
            //        //acBlkTblRec.AppendEntity(acCirc2);
            //        //acTrans.AddNewlyCreatedDBObject(acCirc2, true);
            //        //// Add all the objects to copy to the new document
            //        //acObjIdColl = new ObjectIdCollection();
            //        //acObjIdColl.Add(acCirc1.ObjectId);
            //        //acObjIdColl.Add(acCirc2.ObjectId);
            //        //// Save the new objects to the database 
            //        #endregion   




            //        acTrans.Commit();
            //    }
            //    // Unlock the document 
            //}
            // Change the file and path to match a drawing template on your workstation 
            //string sLocalRoot = Application.GetSystemVariable("LOCALROOTPREFIX") as string;
            //string sTemplatePath = sLocalRoot + "Template\\acad.dwt";
            // Create a new drawing to copy the objects to DocumentCollection 
            var acDocMgr = Application.DocumentManager;
            Document acNewDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDbNewDoc = acNewDoc.Database;

            // Lock the new document 
            using (DocumentLock acLckDoc = acNewDoc.LockDocument())
            {
                // Start a transaction in the new database
                using (Transaction acTrans = acDbNewDoc.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTblNewDoc;
                    acBlkTblNewDoc = acTrans.GetObject(acDbNewDoc.BlockTableId,
                        OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for read
                    BlockTableRecord acBlkTblRecNewDoc;
                    acBlkTblRecNewDoc = acTrans.GetObject(acBlkTblNewDoc[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead) as BlockTableRecord;
                    // Clone the objects to the new database
                    IdMapping acIdMap = new IdMapping();
                    acCurDb.WblockCloneObjects(acObjIdColl, acBlkTblRecNewDoc.ObjectId, acIdMap,
                        DuplicateRecordCloning.Ignore, false);
                    // Save the copied objects to the database
                    acTrans.Commit();
                }
                // Unlock the document 
            }
            // Set the new document current 

            acDocMgr.MdiActiveDocument = acNewDoc;
        }


        [CommandMethod("CopyObjectsBetweenDatabases", CommandFlags.Session)]
        public static void CopyObjectsBetweenDatabases()
        {
            ObjectIdCollection acObjIdColl = new ObjectIdCollection();
            // Get the current document and database 
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            // Lock the current document 
            using (DocumentLock acLckDocCur = acDoc.LockDocument())
            {
                // Start a transaction 
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table record for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;
                    // Create a circle that is at (0,0,0) with a radius of 5
                    Circle acCirc1 = new Circle();
                    acCirc1.Center = new Point3d(0, 0, 0);
                    acCirc1.Radius = 5;
                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acCirc1);
                    acTrans.AddNewlyCreatedDBObject(acCirc1, true);
                    // Create a circle that is at (0,0,0) with a radius of 7
                    Circle acCirc2 = new Circle();
                    acCirc2.Center = new Point3d(0, 0, 0);
                    acCirc2.Radius = 7;
                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acCirc2);
                    acTrans.AddNewlyCreatedDBObject(acCirc2, true);
                    // Add all the objects to copy to the new document
                    acObjIdColl = new ObjectIdCollection();
                    acObjIdColl.Add(acCirc1.ObjectId);
                    acObjIdColl.Add(acCirc2.ObjectId);
                    // Save the new objects to the database
                    acTrans.Commit();
                }
                // Unlock the document 
            }
            // Change the file and path to match a drawing template on your workstation 
            string sLocalRoot = Application.GetSystemVariable("LOCALROOTPREFIX") as string;
            string sTemplatePath = sLocalRoot + "Template\\acad.dwt";
            // Create a new drawing to copy the objects to DocumentCollection 
            var acDocMgr = Application.DocumentManager;
            Document acNewDoc = acDocMgr.Add(sTemplatePath);
            Database acDbNewDoc = acNewDoc.Database;
            // Lock the new document 
            using (DocumentLock acLckDoc = acNewDoc.LockDocument())
            {
                // Start a transaction in the new database
                using (Transaction acTrans = acDbNewDoc.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTblNewDoc;
                    acBlkTblNewDoc = acTrans.GetObject(acDbNewDoc.BlockTableId,
                        OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for read
                    BlockTableRecord acBlkTblRecNewDoc;
                    acBlkTblRecNewDoc = acTrans.GetObject(acBlkTblNewDoc[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead) as BlockTableRecord;
                    // Clone the objects to the new database
                    IdMapping acIdMap = new IdMapping();
                    acCurDb.WblockCloneObjects(acObjIdColl, acBlkTblRecNewDoc.ObjectId, acIdMap,
                        DuplicateRecordCloning.Ignore, false);
                    // Save the copied objects to the database
                    acTrans.Commit();
                }
                // Unlock the document 
            }
            // Set the new document current 

            acDocMgr.MdiActiveDocument = acNewDoc;
        }






        // Helper function to be shared between our command
        // implementations

        //private static bool PurgeDgnLinetypesInDb(Autodesk.AutoCAD.DatabaseServices.Database db, Editor ed)
        //{
        //    using (var tr = db.TransactionManager.StartTransaction())
        //    {
        //        // Start by getting all the "complex" DGN linetypes
        //        // from the linetype table

        //        var linetypes = CollectComplexLinetypeIds(db, tr);

        //        // Store a count before we start removing the ones
        //        // that are referenced

        //        var ltcnt = linetypes.Count;

        //        // Remove any from the "to remove" list that need to be
        //        // kept (as they have references from objects other
        //        // than anonymous blocks)

        //        var ltsToKeep =
        //            PurgeLinetypesReferencedNotByAnonBlocks(db, tr, linetypes);

        //        // Now we collect the DGN stroke entries from the NOD

        //        var strokes = CollectStrokeIds(db, tr);

        //        // Store a count before we start removing the ones
        //        // that are referenced

        //        var strkcnt = strokes.Count;

        //        // Open up each of the "keeper" linetypes, and go through
        //        // their data, removing any NOD entries from the "to
        //        // remove" list that are referenced

        //        PurgeStrokesReferencedByLinetypes(tr, ltsToKeep, strokes);

        //        // Erase each of the NOD entries that are safe to remove

        //        int erasedStrokes = 0;

        //        foreach (ObjectId id in strokes)
        //        {
        //            try
        //            {
        //                var obj = tr.GetObject(id, OpenMode.ForWrite);
        //                obj.Erase();
        //                if (
        //                    obj.GetRXClass().Name.Equals("AcDbLSSymbolComponent")
        //                    )
        //                {
        //                    EraseReferencedAnonBlocks(tr, obj);
        //                }
        //                erasedStrokes++;
        //            }
        //            catch (System.Exception ex)
        //            {
        //                ed.WriteMessage(
        //                    "\nUnable to erase stroke ({0}): {1}",
        //                    id.ObjectClass.Name,
        //                    ex.Message
        //                    );
        //            }
        //        }

        //        // And the same for the complex linetypes

        //        int erasedLinetypes = 0;

        //        foreach (ObjectId id in linetypes)
        //        {
        //            try
        //            {
        //                var obj = tr.GetObject(id, OpenMode.ForWrite);
        //                obj.Erase();
        //                erasedLinetypes++;
        //            }
        //            catch (System.Exception ex)
        //            {
        //                ed.WriteMessage(
        //                    "\nUnable to erase linetype ({0}): {1}",
        //                    id.ObjectClass.Name,
        //                    ex.Message
        //                    );
        //            }
        //        }

        //        // Remove the DGN stroke dictionary from the NOD if empty

        //        bool erasedDict = false;

        //        var nod =
        //            (DBDictionary)tr.GetObject(
        //                db.NamedObjectsDictionaryId, OpenMode.ForRead
        //                );

        //        ed.WriteMessage(
        //            "\nPurged {0} unreferenced complex linetype records" +
        //            " (of {1}).",
        //            erasedLinetypes, ltcnt
        //            );

        //        ed.WriteMessage(
        //            "\nPurged {0} unreferenced strokes (of {1}).",
        //            erasedStrokes, strkcnt
        //            );

        //        if (nod.Contains(dgnLsDictName))
        //        {
        //            var dgnLsDict =
        //                (DBDictionary)tr.GetObject(
        //                    (ObjectId)nod[dgnLsDictName],
        //                    OpenMode.ForRead
        //                    );

        //            if (dgnLsDict.Count == 0)
        //            {
        //                dgnLsDict.UpgradeOpen();
        //                dgnLsDict.Erase();

        //                ed.WriteMessage(
        //                    "\nRemoved the empty DGN linetype stroke dictionary."
        //                    );

        //                erasedDict = true;
        //            }
        //        }

        //        tr.Commit();

        //        // Return whether we have actually found anything to erase

        //        return (
        //            erasedLinetypes > 0 || erasedStrokes > 0 || erasedDict
        //            );
        //    }
        //}
    }
}
