using System;
using System.Collections.Generic;
using System.Linq;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.EditorInput;
using COMS = PGA.MessengerManager;
using Acaddb=global::Autodesk.AutoCAD.DatabaseServices;

namespace PGA.SelectionManager
{
    public static class Select
    {
        public static bool AddToPickSet(ObjectIdCollection oids)
        {
            bool retVal = false;
            try
            {
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

                bool isPickSetModified = false;
                ObjectIdCollection pickedOids = new ObjectIdCollection();

                //Access the current PickSet
                PromptSelectionResult resBuf = ed.SelectImplied();
                if (resBuf.Status != PromptStatus.Error)
                {
                    //Load into a selection set object
                    SelectionSet ssPickSet = resBuf.Value;
                    //Get the object ID's
                    pickedOids = new ObjectIdCollection(ssPickSet.GetObjectIds());
                }
                foreach (ObjectId oid in oids)
                {
                    if (!pickedOids.Contains(oid))
                    {
                        //Adding object to pickSet selection
                        pickedOids.Add(oid);
                        isPickSetModified = true;
                    }
                }
                if (isPickSetModified == true)
                {
                    List<ObjectId> pickedOidList = new List<ObjectId>();
                    foreach (ObjectId oid in pickedOids)
                    {
                        pickedOidList.Add(oid);
                    }
                    ed.SetImpliedSelection(pickedOidList.ToArray());
                    retVal = true;
                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);

                throw;
            }
            //_logger.Debug("End AddToPickSet");
            return retVal;

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
            ObjectIdCollection selection = new ObjectIdCollection(promptSelectionResult.Value.GetObjectIds());
            return selection;
        }
        public static IList<ObjectId> GetAllPolylines()
        {
            IList<ObjectId> ids = GetPolylineEntityIDs();

            return ids;
        }
        public static double? GetPolyLineArea(ObjectId selectedObjectId, global::Autodesk.AutoCAD.DatabaseServices.Database db)
        {
 
            Transaction tr = db.TransactionManager.StartTransaction();

 

            using (tr)
            {
                DBObject obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                Polyline lwp = obj as Polyline;

                if (lwp != null)
                {
                    if (lwp.Closed)
                    {
                        return lwp.Area;
                    }

                }

                else
                {
                    // If an old-style, 2D polyline

                    Polyline2d p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        return p2d.Area;
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        Polyline3d p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            return p3d.Area;
                        }
                    }
                }

                tr.Commit();
            }

            return null;
        }
        public static Polyline GetPolyLineObject(ObjectId selectedObjectId, global::Autodesk.AutoCAD.DatabaseServices.Database db)
        {
 
            Transaction tr = db.TransactionManager.StartTransaction();

            // Get the current UCS

            using (tr)
            {
                DBObject obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);

                Polyline lwp = obj as Polyline;

                if (lwp != null)
                {
                    if (lwp.Closed)
                    {
                        return lwp;
                    }

                }

                tr.Commit();
            }

            return null;
        }
        public static List<ObjectId> GetPolylineEntityIDs()

        {
            List<ObjectId> ids = null;

            try
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                var db = doc.Database;

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (BlockTable)tran.GetObject(db.BlockTableId, OpenMode.ForRead);

                    var br =
                        (BlockTableRecord)tran.GetObject(tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                    var b = br.Cast<ObjectId>();


                    ids = (from id in b
                           where id.ObjectClass.DxfName.ToUpper() == "POLYLINE" &&
                                 id.ObjectClass.DxfName.ToUpper() == "POLYLINE2D"

                           select id).ToList();


                    tran.Commit();
                }

            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }
        public static List<Acaddb.ObjectId> GetPolyLines()
        {
            List<Acaddb.ObjectId> ids = null;

            try
            {
                var db = Active.Database;

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (Acaddb.BlockTable)tran.GetObject(db.BlockTableId, Acaddb.OpenMode.ForRead);

                    var br =
                        (Acaddb.BlockTableRecord)
                            tran.GetObject(tbl[Acaddb.BlockTableRecord.ModelSpace], Acaddb.OpenMode.ForRead);

                    var b = br.Cast<Acaddb.ObjectId>();
 

                    ids = (from id in b
                           where id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE"
                           select id).ToList();


                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }
        public static List<Acaddb.ObjectId> GetGreenPolylines()
        {
            List<Acaddb.ObjectId> ids = null;

            try
            {
                var db = Active.Database;

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (Acaddb.BlockTable)tran.GetObject(db.BlockTableId, Acaddb.OpenMode.ForRead);

                    var br =
                        (Acaddb.BlockTableRecord)
                            tran.GetObject(tbl[Acaddb.BlockTableRecord.ModelSpace], Acaddb.OpenMode.ForRead);

                    var b = br.Cast<Acaddb.ObjectId>();


                    var pids = (from id in b
                           where id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE"                 
                           select id).ToList();

                    ids = new List<ObjectId>();

                    foreach (var id in pids)
                    {
                        var pline = tran.GetObject(id, OpenMode.ForRead) as Polyline;

                        if (pline == null)
                            continue;
                        if (pline.Layer == "OGR" || pline.Layer.ToUpper() == "S-GREEN")
                            ids.Add(pline.ObjectId);
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }

        public static List<Acaddb.ObjectId> GetFeatureLines()
        {
            List<Acaddb.ObjectId> ids = null;

            try
            {
                var db = Active.Database;

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (Acaddb.BlockTable)tran.GetObject(db.BlockTableId, Acaddb.OpenMode.ForRead);

                    var br =
                        (Acaddb.BlockTableRecord)
                            tran.GetObject(tbl[Acaddb.BlockTableRecord.ModelSpace], Acaddb.OpenMode.ForRead);

                    var b = br.Cast<Acaddb.ObjectId>();

                    #region Other Types

                    //==============search certain entity========================//

                    //"LINE" for line

                    //"LWPOLYLINE" for polyline

                    //"CIRCLE" for circle

                    //"INSERT" for block reference

                    //...

                    //We can use "||" (or) to search for more then one entity types

                    //============================================================//


                    //Use lambda extension method

                    //ids = b.Where(id => id.ObjectClass.DxfName.ToUpper() == "LINE" ||

                    //    id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE").ToList<ObjectId>();

                    #endregion

                    ids = (from id in b
                           where id.ObjectClass.DxfName.ToUpper() == "AECC_FEATURE_LINE"
                           select id).ToList();


                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }
        public static List<Acaddb.ObjectId> GetPolyline3D()
        {
            List<Acaddb.ObjectId> ids = null;

            try
            {
                var db = Active.Database;

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (Acaddb.BlockTable)tran.GetObject(db.BlockTableId, Acaddb.OpenMode.ForRead);

                    var br =
                        (Acaddb.BlockTableRecord)
                            tran.GetObject(tbl[Acaddb.BlockTableRecord.ModelSpace], Acaddb.OpenMode.ForRead);

                    var b = br.Cast<Acaddb.ObjectId>();
 

                    ids = (from id in b
                           where id.ObjectClass.DxfName.ToUpper() == "Polyline3d"
                           select id).ToList();


                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }

    }
}
