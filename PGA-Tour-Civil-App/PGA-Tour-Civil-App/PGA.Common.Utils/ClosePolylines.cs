#region

using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using BBC.Common.AutoCAD;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = System.Exception;

#endregion

namespace PGA.Autodesk.Utils
{
    public static class ClosePolylines
    {
        public static void TurnOffSurfaceLayers()
        {
            #region Database

            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            #endregion

            var dl =
                doc.LockDocument(
                    DocumentLockMode.ProtectedAutoWrite,
                    null, null, true
                );

            try
            {
                using (dl)
                {
                    var tr =
                        doc.TransactionManager.StartTransaction();

                    using (tr)
                    {
                        IList<string> layers = new List<string>();
                        layers = LayerManager.GetLayerNames(doc.Database);
                        foreach (var layer in layers)
                        {
                            if (layer.Contains("S-"))
                                LayerManager.OffLayer
                                    (doc.Database, layer, true);
                        }

                        tr.Commit();
                    }
                }
            }
            catch (Exception)
            {
                MessengerManager.MessengerManager.AddLog(
                    "\nError turning on layers."
                );
            }
        }

        public static void TurnOnSurfaceLayers()
        {
            #region Database

            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            #endregion

            var dl =
                doc.LockDocument(
                    DocumentLockMode.ProtectedAutoWrite,
                    null, null, true
                );

            try
            {
                using (dl)
                {
                    var tr =
                        doc.TransactionManager.StartTransaction();

                    using (tr)
                    {
                        IList<string> layers = new List<string>();
                        layers = LayerManager.GetLayerNames(doc.Database);
                        foreach (var layer in layers)
                        {
                            if (layer.Contains("S-"))
                                LayerManager.OffLayer
                                    (doc.Database, layer, false);
                        }

                        tr.Commit();
                    }
                }
            }
            catch (Exception)
            {
                MessengerManager.MessengerManager.AddLog(
                    "\nError turning on layers."
                );
            }
        }

        public static void TurnOnAllLayers()
        {

            #region Database

            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            #endregion

            var dl =
                doc.LockDocument(
                    DocumentLockMode.ProtectedAutoWrite,
                    null, null, true
                );

            try
            {
                using (dl)
                {
                    var tr =
                        doc.TransactionManager.StartTransaction();

                    using (tr)
                    {
                        LayerManager.OffAllLayers(doc.Database, false);
                        LayerManager.OffLayer(doc.Database, "EXCLUDED-OBJECTS", true);
                        tr.Commit();
                    }
                }
            }
            catch (Exception)
            {
                MessengerManager.MessengerManager.AddLog(
                    "\nError turning on layers."
                );
            }
        }

        public static void OROIsoSelectedPolylines()
        {
            SelectionSet selection = null;


            #region Database

            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            #endregion

            var dl =
                doc.LockDocument(
                    DocumentLockMode.ProtectedAutoWrite,
                    null, null, true
                );

            using (dl)
            {
                var tr =
                    doc.TransactionManager.StartTransaction();

                using (tr)
                {
                    LayerManager.OffAllLayers(doc.Database, true);
                    LayerManager.OffLayer(doc.Database, "ORO", false);
                    tr.Commit();
                }
            }

            try
            {
                selection = SelectionManager.GetSelectionSet("");
                foreach (var obj in selection.GetObjectIds())
                {
                    if (obj.IsErased)
                        continue;

                    if (MoveDuplicatePolylines(obj))
                        ed.WriteMessage("\nDeleted Polyline Object: " +
                                        obj.Handle);
                }
            }
            catch (Exception)
            {
                MessengerManager.MessengerManager.AddLog(
                    "\nError deleting polylines."
                );
            }
 
        }

        private static bool MoveDuplicatePolylines(ObjectId pobj)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();

            doc.LockDocument(
                DocumentLockMode.ProtectedAutoWrite,
                null, null, true
            );

            using (tr)
            {
                try
                {
                    var obj =
                        tr.GetObject(pobj, OpenMode.ForWrite);

                    var lwp = obj as Polyline;

                    /*Add layer*/

                    if (!LayerManager.IsDefined(db, "EXCLUDED-OBJECTS"))
                    {
                        LayerManager.CreateLayer(db, "EXCLUDED-OBJECTS");
                        LayerManager.OffLayer(db, "EXCLUDED-OBJECTS", true);
                    }
                    lwp.Layer = "EXCLUDED-OBJECTS";
                    tr.Commit();
                    return true;
                }
                catch (Exception)
                {
                    MessengerManager.MessengerManager.AddLog(
                        "\nError moving polylines."
                    );
                }
                return false;
            }
        }

        public static void CloseSelectedPolylines()
        {
            ObjectId[] selection = null;

            #region Database

            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            #endregion

            try

            {
                var selectionRes =
                    ed.SelectImplied();

                // If there's no pickfirst set available...

                if (selectionRes.Status == PromptStatus.Error)

                {
                    // ... ask the user to select entities

                    var selectionOpts =
                        new PromptSelectionOptions();

                    selectionOpts.MessageForAdding =
                        "\nSelect polylines: ";

                    selectionRes =
                        ed.GetSelection(selectionOpts);
                }

                else

                {
                    // If there was a pickfirst set, clear it

                    ed.SetImpliedSelection(new ObjectId[0]);
                }

                // If the user has not cancelled...

                if (selectionRes.Status == PromptStatus.OK)
                {
                    /*Remove Duplicate Polylines*/
                    try
                    {
                        selection = selectionRes.Value.GetObjectIds().Where
                        (id =>
                        {
                            return (id.ObjectClass == RXObject.GetClass(typeof(Polyline))) ||
                                   (id.ObjectClass == RXObject.GetClass(typeof(Polyline2d))) ||
                                   (id.ObjectClass == RXObject.GetClass(typeof(Polyline3d)));
                        }).ToArray();


                        foreach (var obj in selection)
                        {
                            if (obj.IsErased)
                                continue;

                            if (!DeleteDuplicatePolylines(obj, selection))
                                ed.WriteMessage("\nDeleted Polyline Object: " + obj.Handle);
                        }
                    }
                    catch (Exception)
                    {
                        MessengerManager.MessengerManager.AddLog(
                            "\nError deleting polylines."
                        );
                    }
                    /*Remove Polylines not on 3 letter layers*/

                    try
                    {
                        foreach (var obj in selection)
                        {
                            if (obj.IsErased)
                                continue;

                            if (!DeleteDuplicatePolylines(obj))
                                InteratePolyLines(obj);

                            ChangeLayerColorFor3LetterName(obj);
                        }
                    }
                    catch (Exception)
                    {
                        MessengerManager.MessengerManager.AddLog(
                            "\nError deleting polylines on 3 letters."
                        );
                    }
                }
            }
            catch
            {
            }
        }

        private static void ChangeLayerColorFor3LetterName(ObjectId pobj)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();


            using (tr)
            {
                var obj =
                    tr.GetObject(pobj, OpenMode.ForWrite);

                var lwp = obj as Polyline;

                /*Change the Layer Color*/
                if (lwp != null)
                {
                    LayerManager.ChangeLayerColor
                        (db, lwp.Layer, PLineToLayers.AssignLayerColor(lwp));
                }


                tr.Commit();
            }
        }

        private static bool DeleteDuplicatePolylines(ObjectId obase, ObjectId[] selection)
        {
            foreach (var obj in selection)
            {
                if (obj.IsErased)
                    continue;

                if (obase.Equals(obj))
                    continue;
                if ((GetPolylineArea(obj) == GetPolylineArea(obase)) && GetMatchingCentroids(obj, obase))
                {
                    if (!IsProtectedLayer(obase)) RemoveObjectFromDatabase(obase);
                    return true;
                }
            }
            return false;
        }

        private static bool DeleteDuplicatePolylines(ObjectId obase, SelectionSet selection)
        {
            foreach (var obj in selection.GetObjectIds())
            {
                if (obj.IsErased)
                    continue;

                if (obase.Equals(obj))
                    continue;
                if ((GetPolylineArea(obj) == GetPolylineArea(obase)) && GetMatchingCentroids(obj, obase))
                {
                    if (!IsProtectedLayer(obase)) RemoveObjectFromDatabase(obase);
                    return true;
                }
            }
            return false;
        }

        private static bool IsProtectedLayer(ObjectId obase)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();


            using (tr)
            {
                var obj =
                    tr.GetObject(obase, OpenMode.ForRead);

                var lwp = obj as Polyline;

                /*Change the Layer Color*/

                LayerManager.ChangeLayerColor
                    (db, lwp.Layer, PLineToLayers.AssignLayerColor(lwp));

                if (!(lwp.Layer.ToUpper() == "ORO"))
                {
                    tr.Commit();
                    return true;
                }
                tr.Commit();
            }

            return false;
        }

        private static bool GetMatchingCentroids(ObjectId obj, ObjectId obase)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();


            using (tr)
            {
                var obj1 =
                    tr.GetObject(obj, OpenMode.ForWrite);

                var obj2 =
                    tr.GetObject(obase, OpenMode.ForWrite);


                var lwp1 = obj1 as Polyline;
                var lwp2 = obj2 as Polyline;

                if ((lwp1 != null) && (lwp2 != null))
                {
                    if ((lwp2.EndPoint.X == lwp1.EndPoint.X) &&
                        (lwp2.EndPoint.Y == lwp1.EndPoint.Y))
                    {
                        // lwp2.Erase();
                        return true;
                    }
                }
                tr.Commit();
            }


            return false;
        }


        private static void RemoveObjectFromDatabase(ObjectId obase)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();


            using (tr)
            {
                var obj =
                    tr.GetObject(obase, OpenMode.ForWrite);


                var lwp = obj as Polyline;

                if (lwp != null)
                {
                    lwp.Erase();
                }

                tr.Commit();
            }
        }


        public static void InteratePolyLines(ObjectId selectedObjectId)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();

            // Get the current UCS

            var ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (tr)
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                var lwp = obj as Polyline;

                /*Change the Layer Color*/
                if (lwp != null)
                {
                    LayerManager.ChangeLayerColor
                        (db, lwp.Layer, PLineToLayers.AssignLayerColor(lwp));
                }

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (!lwp.Closed)
                    {
                        lwp.UpgradeOpen();
                        lwp.Color = Color.FromColorIndex(ColorMethod.ByColor, 1);
                        CreateDrawingErrorReferences(lwp.StartPoint, lwp.EndPoint, db, ucs);
                        LayerManager.ChangeLayerColor(db, lwp.Layer, PLineToLayers.AssignLayerColor(lwp));
                        lwp.DowngradeOpen();
                    }
                    // Use a for loop to get each vertex, one by one

                    var vn = lwp.NumberOfVertices;

                    for (var i = 0; i < vn; i++)
                    {
                        // Could also get the 3D point here

                        var pt = lwp.GetPoint2dAt(i);

                        ed.WriteMessage("\n" + pt);
                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    var p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        // Use foreach to get each contained vertex

                        foreach (ObjectId vId in p2d)
                        {
                            var v2d =
                                (Vertex2d) tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                );

                            ed.WriteMessage(
                                "\n" + v2d.Position
                            );
                        }
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        var p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            // Use foreach to get each contained vertex

                            foreach (ObjectId vId in p3d)
                            {
                                var v3d =
                                    (PolylineVertex3d) tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                    );

                                ed.WriteMessage(
                                    "\n" + v3d.Position
                                );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
        }

        private static void CreateDrawingErrorReferences(Point3d startPoint, Point3d endPoint,
            global::Autodesk.AutoCAD.DatabaseServices.Database db,
            CoordinateSystem3d ucs)
        {
            try
            {
                var tr = db.TransactionManager.StartTransaction();

                using (tr)
                {
                    var btr =
                        (BlockTableRecord) tr.GetObject(
                            db.CurrentSpaceId,
                            OpenMode.ForWrite
                        );

                    var cir = new Circle(startPoint, ucs.Zaxis, 5);
                    cir.Color = Color.FromColorIndex(ColorMethod.ByColor, 1);
                    btr.AppendEntity(cir);

                    tr.AddNewlyCreatedDBObject(cir, true);
                    tr.Commit();
                }
            }
            catch
            {
                MessengerManager.MessengerManager.AddLog(
                    "\nError calculating enclosing circle."
                );
            }
        }

        public static bool DeleteDuplicatePolylines(ObjectId Pobj)
        {
            try
            {
                var db = HostApplicationServices.WorkingDatabase;

                var tr = db.TransactionManager.StartTransaction();

                using (tr)
                {
                    var obj =
                        tr.GetObject(Pobj, OpenMode.ForWrite);

                    var lwp = obj as Polyline;
                    if ((lwp != null) && !lwp.IsErased)
                    {
                        if (!(lwp.Layer.Length.Equals(3) && Matches(lwp.Layer)))
                            if (!IsProtectedLayer(Pobj)) lwp.Erase();

                        tr.Commit();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                MessengerManager.MessengerManager.AddLog(
                    "\nError removing polylines: DeleteDuplicatePolylines(ObjectId Pobj)"
                );
            }
            return false;
        }

        private static bool Matches(string p)
        {
            var str = "OBR,OBD,OST,OBO,OCA,OCO,ODO,OFW,OGR,OGS,IOR,OLN,ONA,OTH,OPT,ORK,ORO,OSS,OTB,OTO,OWS,OWL,OWA,OWD";
            var states = str.Split(',');
            foreach (var val in states)
            {
                if (val.Equals(p))
                    return true;
            }
            return false;
        }

        private static double GetPolylineArea(ObjectId selectedObjectId)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);

                var lwp = obj as Polyline;

                if (lwp != null)
                {
                    return lwp.Area;
                }
                tr.Commit();
                return 0; /*return value could be an issue*/
            }
        }
    }
}