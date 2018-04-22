// ***********************************************************************
// Assembly         : PGA.SimplifyPolylines
// Author           : Daryl Banks, PSM
// Created          : 02-17-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 05-05-2016
// ***********************************************************************
// <copyright file="Commands.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.Colors;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.Runtime;
using PGA.Database;
using ObjectId = global::Autodesk.AutoCAD.DatabaseServices.ObjectId;
using ObjectIdCollection = global::Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;

namespace PGA.SimplifyPolylines
{
    /// <summary>
    /// Class Commands.
    /// </summary>
    public static class Commands
    {

        // [CommandMethod("PGA-SimplifyPolylines")]
        /// <summary>
        /// Simplifies the polylines.
        /// </summary>
        public static void SimplifyPolylines()
        {
            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;
            IList<Point2dCollection> point2DCollections = new List<Point2dCollection>();
            IList<Point2dCollection> reducedCollection = new List<Point2dCollection>();
            Point2dCollection sortedCollection = new Point2dCollection();
            double tolerance = 0.001;

            try
            {
                //Select Polyline Objects
                ObjectIdCollection collection = GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3D");

                if (collection == null || collection.Count == 0) return;

                using (DocumentLock doclock = doc.LockDocument())
                {
                    IList<string> layerlist = new List<string>();

                    //Get Point Collection of Polylines
                    foreach (ObjectId obj in collection)
                    {
                        string layername = GetPolylineName(obj);
                        layerlist.Add(layername); 
                            
                        point2DCollections.Add(GetPoint2dFromPolylines(obj));

                        //Log Original Information
                        GetPolylineName(obj, collection.IndexOf(obj));
                    }

                    int counter = 0;
                    foreach (Point2dCollection pnt2DCol in point2DCollections)
                    {
                        int index = point2DCollections.IndexOf(pnt2DCol);
                        string layername = layerlist[counter];
                        Point2d[] reducedPoints;
                        List<Point2d> list = new List<Point2d>(pnt2DCol.ToArray());

                        //Reduce Polylines
                        reducedPoints = DouglasPeuckerImplementation.DouglasPeuckerReduction(list, tolerance);
                        reducedCollection.Add(ConvertCollections(reducedPoints));


                        sortedCollection = SortModified(list, ConvertCollections(reducedPoints));
                        // Add Reduced Polylines to Database
                        //CreateSimplifiedPolylines(layername, counter, sortedCollection);
                        ModifyPolylineVertices(sortedCollection, collection[index]);
                        //Store Polyline Information

                        //Log Polyline Information
                        GetPolylineName(sortedCollection, point2DCollections.IndexOf(pnt2DCol));
                        counter++;
                    }

                    IList<Point2dCollection> OriginalPointCollection = point2DCollections.ToList();

                     
                    //Get Point Collection of Polylines
                    //foreach (ObjectId obj in collection)
                    //{
                    //    int index  = collection.IndexOf(obj);
                    //    string layername = GetPolylineName(obj);

                    //    //Point2dCollection points = OriginalPointCollection.ElementAt(index);
                    //    CreateSimplifiedPolylines(layername,index,sortedCollection);
                    //}
                }
                //DeleteOldPolylines(collection);
            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs("SimplifyPolylines: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes the old polylines.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private static void DeleteOldPolylines(ObjectIdCollection collection)
        {
            // Get the current document and database 
            var acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var acCurDb = acDoc.Database;
            var ed = acDoc.Editor;

            using (acDoc.LockDocument())
            {
                using (var acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;

                    PGA.AcadUtilities.AcadUtilities.DeleteObjects(collection);
                    //foreach (ObjectId selectedObjectId in collection)
                    //{
                    //    var obj = acTrans.GetObject(selectedObjectId, OpenMode.ForWrite);

                    //    var p2d = obj as Polyline2d;

                    //    if (p2d != null && p2d.Color == Color.FromColorIndex(ColorMethod.ByAci, 0))
                    //    {
                    //        p2d.Erase(true);
                    //    }
                    //    else
                    //    {
                    //        var lwp = obj as Polyline;

                    //        if (lwp != null && lwp.Color == Color.FromColorIndex(ColorMethod.ByAci, 0))
                    //        {
                    //            lwp.Erase(true);
                    //        }
                    //    }
                    //}
                    acTrans.Commit();
                }
            }
        }

        /// <summary>
        /// Creates the simplified polylines.
        /// </summary>
        /// <param name="layername">The layername.</param>
        /// <param name="index">The index.</param>
        /// <param name="points">The points.</param>
        public static void CreateSimplifiedPolylines(string layername, int index, Point2dCollection points)
        {
            Point3dCollection points3D = PGA.AcadUtilities.AcadUtilities.ConvertTo3dCollection(points);

            // Get the current document and database 
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database acCurDb = acDoc.Database;
            Editor ed = acDoc.Editor;
            // Lock the current document 
            PlanarEntity plane;

            CoordinateSystem3d ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            plane = new Plane(ucs.Origin, ucs.Xaxis);

            using (DocumentLock acLckDocCur = acDoc.LockDocument())
            {
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    #region MyRegion

                    // Open the Block table record for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;


                    //DoubleCollection bulges = new DoubleCollection();
                    //double[] bulgelist = new double[] { };
                    //Point3dCollection p3DCollection = new Point3dCollection();
                    //int i = 0;
                    //foreach (var point in points)
                    //{
                    //    Point3d p3D = new Point3d(point.X, point.Y, 0.0);
                    //    p3DCollection.Add(p3D);

                    //    bulges.Add(0);
                    //}

                    Polyline polyline = new Polyline();

                    for (int i = 0; i < points.Count; i++)
                    {
                        polyline.AddVertexAt(i,points[i],0.0,0.0,0.0);
                    }
                    polyline.Closed = true;
                    polyline.Layer = layername;
                    polyline.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    acBlkTblRec.AppendEntity(polyline);
                    acTrans.AddNewlyCreatedDBObject(polyline, true);


                    #region Polyline2d

                    //Polyline2d polyline2D = new Polyline2d(Poly2dType.SimplePoly, points3D, 0, true, 0, 0, null);
                    //polyline2D.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    //polyline2D.Layer = layername;
                    //acBlkTblRec.AppendEntity(polyline2D);
                    //acTrans.AddNewlyCreatedDBObject(polyline2D, true);



                    #endregion

                    #endregion         

                    acTrans.Commit();
                }

            }
        }

        /// <summary>
        /// Simplifies the polylines interative.
        /// </summary>
        public static void SimplifyPolylinesInterative(double tolerance)
        {
            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;
            IList<Point2dCollection> point2DCollections = new List<Point2dCollection>();
            IList<Point2dCollection> reducedCollection = new List<Point2dCollection>();
            Point2dCollection sortedCollection = new Point2dCollection();

            try
            {
                //Select Polyline Objects
                ObjectIdCollection collection = GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3D");

                if (collection == null || collection.Count == 0) return;

                using (DocumentLock doclock = doc.LockDocument())
                {


                    //Get Point Collection of Polylines
                    foreach (ObjectId obj in collection)
                    {
                        point2DCollections.Add(GetPoint2dFromPolylines(obj));

                        //Log Original Information
                        GetPolylineName(obj, collection.IndexOf(obj));
                    }

                    foreach (Point2dCollection pnt2DCol in point2DCollections)
                    {
                        int index = point2DCollections.IndexOf(pnt2DCol);
                        Point2d[] reducedPoints;
                        List<Point2d> list = new List<Point2d>(pnt2DCol.ToArray());

                        //Reduce Polylines
                        reducedPoints = DouglasPeuckerImplementation.DouglasPeuckerReduction(list, tolerance);
                        reducedCollection.Add(ConvertCollections(reducedPoints));


                        sortedCollection = SortModified(list, ConvertCollections(reducedPoints));
                        // Add Reduced Polylines to Database
                        ModifyPolylineVertices(sortedCollection, collection[index]);
                        //Log Polyline Information
                        GetPolylineName(sortedCollection, point2DCollections.IndexOf(pnt2DCol));
                        index++;
                    }
                }

            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs("SimplifyPolylines: " + ex.Message);
            }
        }

        /// <summary>
        /// Simplifies the polylines test.
        /// </summary>
        [CommandMethod("PGA-SimplifyPolylinesTest")]
        public static void SimplifyPolylinesTest()
        {
            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;
            IList<Point2dCollection> point2DCollections = new List<Point2dCollection>();
            IList<Point2dCollection> reducedCollection = new List<Point2dCollection>();
            Point2dCollection sortedCollection = new Point2dCollection();
            double tolerance = 0.001;

            try
            {
                //Select Polyline Objects
                ObjectIdCollection collection = GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3D");

                if (collection == null || collection.Count == 0) return;

                using (DocumentLock doclock = doc.LockDocument())
                {


                    //Get Point Collection of Polylines
                    foreach (ObjectId obj in collection)
                    {
                        point2DCollections.Add(GetPoint2dFromPolylines(obj));

                        //Log Original Information
                        GetPolylineName(obj, collection.IndexOf(obj));
                    }

                    foreach (Point2dCollection pnt2DCol in point2DCollections)
                    {
                        int index = point2DCollections.IndexOf(pnt2DCol);
                        Point2d[] reducedPoints;
                        List<Point2d> list = new List<Point2d>(pnt2DCol.ToArray());

                        //Reduce Polylines
                        reducedPoints = DouglasPeuckerImplementation.DouglasPeuckerReduction(list, tolerance);
                        reducedCollection.Add(ConvertCollections(reducedPoints));


                        sortedCollection = SortModified(list, ConvertCollections(reducedPoints));
                        // Add Reduced Polylines to Database
                        //CreateNewPolyliness(null, sortedCollection);
                        ModifyPolylineVertices(sortedCollection, collection[index]);
                        //Store Polyline Information

                        //Log Polyline Information
                        GetPolylineName(sortedCollection, point2DCollections.IndexOf(pnt2DCol));
                        index++;
                    }
                }

            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs("SimplifyPolylines: " + ex.Message);
            }
        }

        public static void SimplifyPolylinesInterative(ObjectId polyid)
        {
            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;
            IList<Point2dCollection> point2DCollections = new List<Point2dCollection>();
            IList<Point2dCollection> reducedCollection = new List<Point2dCollection>();
            Point2dCollection sortedCollection = new Point2dCollection();
            double tolerance = 0.001;

            try
            {
                //Select Polyline Objects
                ObjectIdCollection collection = new ObjectIdCollection();
                collection.Add(polyid);

                if (collection == null || collection.Count == 0) return;

                using (DocumentLock doclock = doc.LockDocument())
                {


                    //Get Point Collection of Polylines
                    foreach (ObjectId obj in collection)
                    {
                        point2DCollections.Add(GetPoint2dFromPolylines(obj));

                        //Log Original Information
                        GetPolylineName(obj, collection.IndexOf(obj));
                    }

                    foreach (Point2dCollection pnt2DCol in point2DCollections)
                    {
                        int index = point2DCollections.IndexOf(pnt2DCol);
                        Point2d[] reducedPoints;
                        List<Point2d> list = new List<Point2d>(pnt2DCol.ToArray());

                        //Reduce Polylines
                        reducedPoints = DouglasPeuckerImplementation.DouglasPeuckerReduction(list, tolerance);
                        reducedCollection.Add(ConvertCollections(reducedPoints));

                        sortedCollection = SortModified(list, ConvertCollections(reducedPoints));

                        // Add Reduced Polylines to Database
                        ModifyPolylineVertices(sortedCollection, collection[index]);

                        GetPolylineName(sortedCollection, point2DCollections.IndexOf(pnt2DCol));
                        index++;
                    }
                }

            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs("SimplifyPolylines: " + ex.Message);
            }
        }

        /// <summary>
        /// Modifies the polyline vertices.
        /// </summary>
        /// <param name="sortedpoints">The sortedpoints.</param>
        /// <param name="polylineoId">The polylineo identifier.</param>
        private static void ModifyPolylineVertices(Point2dCollection sortedpoints, ObjectId polylineoId)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;


            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                var obj = tr.GetObject(polylineoId, OpenMode.ForRead);
                var pl = obj as Polyline;
                if (pl != null)
                {                    
                    pl.UpgradeOpen();
                    pl.Reset(true,sortedpoints.Count);
                    for (int i = 0; i < sortedpoints.Count; i++)
                    {
                        pl.AddVertexAt(i, sortedpoints[i],0,0,0);
                    }
                    pl.Closed = true;
                }
                tr.Commit();
            }
        }
        /// <summary>
        /// Modifies the polyline vertices.
        /// </summary>
        /// <param name="originalpoints">The originalpoints.</param>
        /// <param name="sortedpoints">The sortedpoints.</param>
        /// <param name="polylineoId">The polylineo identifier.</param>
        private static void ModifyPolylineVertices(Point2dCollection originalpoints, Point2dCollection sortedpoints, ObjectId polylineoId)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;

          
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    var obj = tr.GetObject(polylineoId, OpenMode.ForRead);
                    var pl = obj as Polyline;
                    var flag = false;

                    if (pl != null)
                    {
                        pl.UpgradeOpen();
                        for (int i = 1; i < pl.NumberOfVertices - 1; i++)
                        {
                            flag = false;
                            for (int j = 0; j < sortedpoints.Count; j++)
                            {
                                if ((pl.GetPoint2dAt(i).Equals(sortedpoints[j])))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag == false) pl.RemoveVertexAt(i);
                        }

                    }
                    tr.Commit();
                }
       
        }


        /// <summary>
        /// Creates the new polyliness.
        /// </summary>
        /// <param name="_sourcedb">The _sourcedb.</param>
        /// <param name="_collection">The _collection.</param>
        public static void CreateNewPolyliness(Autodesk.AutoCAD.DatabaseServices.Database _sourcedb, Point2dCollection _collection)
        {
       
            // Get the current document and database 
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database acCurDb =  acDoc.Database;
            Editor ed = acDoc.Editor;
            // Lock the current document 
            PlanarEntity plane;

            CoordinateSystem3d ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            plane = new Plane(ucs.Origin, ucs.Xaxis);

            using (DocumentLock acLckDocCur = acDoc.LockDocument())
            {
                // Start a transaction 
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    #region MyRegion

                    // Open the Block table record for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;


                    DoubleCollection bulges = new DoubleCollection();
                    double[] bulgelist = new double[] {};
                    Point3dCollection p3DCollection = new Point3dCollection();
                    foreach (var point in _collection)
                    {
                        Point3d p3D = new Point3d(point.X, point.Y, 0.0);
                        p3DCollection.Add(p3D);

                        bulges.Add(0);
                    }
                
                    Polyline2d polyline2D = new Polyline2d(Poly2dType.SimplePoly, p3DCollection,0,true,0,0,bulges);
                    polyline2D.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                  
                    acBlkTblRec.AppendEntity(polyline2D);
                    acTrans.AddNewlyCreatedDBObject(polyline2D, true); 

                    #endregion

                    acTrans.Commit();
                }
                //    // Unlock the document 
            }
        }

        /// <summary>
        /// Gets the ids by type type value.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>ObjectIdCollection.</returns>
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

        /// <summary>
        /// Gets the point2d from polylines.
        /// </summary>
        /// <param name="selectedObjectId">The selected object identifier.</param>
        /// <returns>Point2dCollection.</returns>
        public static Point2dCollection GetPoint2dFromPolylines(ObjectId selectedObjectId)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var Points = new Point2dCollection();
            PlanarEntity plane;

            //Get the current UCS

                CoordinateSystem3d ucs =
                    ed.CurrentUserCoordinateSystem.CoordinateSystem3d;
            
            plane = new Plane(ucs.Origin,ucs.Xaxis);

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                //"lightweight" (or optimized) polyline

                var lwp = obj as Polyline;

                if (lwp != null)
                {
                    var vn = lwp.NumberOfVertices;

                    for (var i = 0; i < vn; i++)
                    {
                        var pt = lwp.GetPoint2dAt(i);
                        Points.Add(pt);
                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    var p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                      
                        foreach (ObjectId vId in p2d)
                        {
                            var v2d =
                                (Vertex2d) tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                    );

                           // Points.Add(v2d.Position.Convert2d(plane));
                            Points.Add((new Point2d(v2d.Position.X, v2d.Position.Y)));
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

                                //ed.WriteMessage(
                                //    "\n" + v3d.Position.ToString()
                                //    );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
            return Points;
        }

        /// <summary>
        /// Converts the collections.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Point2dCollection.</returns>
        private static Point2dCollection ConvertCollections(Point2d[] points)
        {
            Point2dCollection _collection = new Point2dCollection();

            foreach (var pnts in points)
            {
                _collection.Add(pnts);
            }

            return _collection;
        }

        /// <summary>
        /// Gets the name of the polyline.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool GetPolylineName(Point2dCollection points,int index)
        {
            DatabaseLogs.FormatLogs(String.Format("Polyline Position= {0} - Vertices= {1}",
              index , points.Count));

            return true;
        }

        /// <summary>
        /// Gets the name of the polyline.
        /// </summary>
        /// <param name="selectedObjectId">The selected object identifier.</param>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool GetPolylineName(ObjectId selectedObjectId, int index)
        {

            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var counter = 0;
            var position = 0;
            var name    = "";
            PlanarEntity plane;

            //Get the current UCS

            CoordinateSystem3d ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            plane = new Plane(ucs.Origin, ucs.Xaxis);

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                //"lightweight" (or optimized) polyline

                var lwp = obj as Polyline;

                if (lwp != null)
                {
                    var vn = lwp.NumberOfVertices;

                    DatabaseLogs.FormatLogs( String.Format("Polyline Name= {2}-{0} - Vertices= {1}", 
                        lwp.Layer, lwp.NumberOfVertices,index));
                }

                else
                {
                    // If an old-style, 2D polyline

                    var p2d = obj as Polyline2d;
                   

                    if (p2d != null)
                    {           

                        foreach (ObjectId vId in p2d)
                        {
                            var v2d =
                                (Vertex2d)tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                    );

                            name = v2d.BlockName;
                            position++;
                        }

                      DatabaseLogs.FormatLogs(  String.Format("Polyline Name= {2}-{0} - Vertices= {1}",
                           p2d.Layer,counter,index));
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
                                    (PolylineVertex3d)tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                        );
                                name = v3d.Layer;
                            }

                            DatabaseLogs.FormatLogs(String.Format("Polyline Name= {2}-{0} - Vertices= {1}",
                               name, counter,index));
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
            return true;
        }

        /// <summary>
        /// Gets the name of the polyline.
        /// </summary>
        /// <param name="selectedObjectId">The selected object identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetPolylineName(ObjectId selectedObjectId)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            PlanarEntity plane;

            //Get the current UCS

            var ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            plane = new Plane(ucs.Origin, ucs.Xaxis);

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                //"lightweight" (or optimized) polyline

                var lwp = obj as Polyline;

                if (lwp != null)
                {
                    return lwp.Layer;
                }

                // If an old-style, 2D polyline

                var p2d = obj as Polyline2d;


                if (p2d != null)
                {
                    return p2d.Layer;
                }

                // If an old-style, 3D polyline

                var p3d = obj as Polyline3d;

                if (p3d != null)
                {
                    return p3d.Layer;
                }


                // Committing is cheaper than aborting

                tr.Commit();
            }
            return null;
        }


        /// <summary>
        /// Sorts the modified.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="reduced">The reduced.</param>
        /// <returns>Point2dCollection.</returns>
        public static Point2dCollection SortModified(List<Point2d> original, Point2dCollection reduced )
        {
            IList<Point2d> sorted = new List<Point2d>();
            //original.CopyTo(sorted.ToArray(),original.Count);
            sorted = original.ToList();
            if (original.Count < 2) return null;

            foreach (Point2d item in sorted)
            {
                if (!reduced.Contains(item))
                    original.Remove(item);
            }
        
            return ConvertCollections(original);
        }

        /// <summary>
        /// Converts the collections.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns>Point2dCollection.</returns>
        private static Point2dCollection ConvertCollections(List<Point2d> original)
        {
            Point2dCollection _collection = new Point2dCollection();

            foreach (var pnts in original)
            {
                _collection.Add(pnts);
            }

            return _collection;
        }

        /// <summary>
        /// Sorts the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>Point2dCollection.</returns>
        public static Point2dCollection Sort(Point2dCollection collection)
        {


            IList<Point2d> usedList = new List<Point2d>();
            Point2dCollection pointsout = new Point2dCollection();
            Point2dCollection sorted    = new Point2dCollection();
            Point2d last  = new Point2d();
            Point2d first = new Point2d();
            if (collection.Count < 2) return null;

            if (collection.Count > 1)
            {
                first = collection[0];
                last  = collection[1];
            }
            
            //last item resorted
            foreach (var item in collection)
            {
                if (!item.Equals(last))
                    pointsout.Add(item);
            }
            pointsout.Add(last);
            //final sort nearestneighbor
            
            int count = 0;
            foreach (var item in pointsout)
            {

                if (count == 0)
                {
                    sorted.Add(item);
                    usedList.Add(item);
                }
                else
                {
                    sorted.Add(NearestNeighbor(sorted[count - 1], pointsout,usedList));
                    usedList.Add(sorted[count]);
                }
                count++;
            }
            sorted.Add(last);
            return sorted;
        }


        /// <summary>
        /// Closests the point.
        /// </summary>
        /// <param name="point3D">The point3 d.</param>
        /// <param name="collection">The collection.</param>
        public static void ClosestPoint(Point3d point3D,Point2dCollection collection)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Editor ed = doc.Editor;
            //PromptPointResult ppr = ed.GetPoint("\nSpecify a point: ");
            //if (ppr.Status != PromptStatus.OK) return;
            //Point3d pt = ppr.Value.TransformBy(ed.CurrentUserCoordinateSystem);
            Point3d pt = point3D;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                var closest = btr
                    .Cast<ObjectId>()
                    .Select(id => tr.GetObject(id, OpenMode.ForRead) as Polyline)
                    .Where(ent => ent != null)
                    .Select(pline => new { pLine = pline, Dist = pt.DistanceTo(pline.GetClosestPointTo(pt, false)) })
                    .Aggregate((l1, l2) => l1.Dist < l2.Dist ? l1 : l2);
                if (closest != null)
                {
                    closest.pLine.Highlight();
                    ed.WriteMessage("\nDistance = {0}", closest.Dist);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Nearests the neighbor.
        /// </summary>
        /// <param name="testpoint2D">The testpoint2 d.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="usedList">The used list.</param>
        /// <returns>Point2d.</returns>
        public static Point2d NearestNeighbor(Point2d testpoint2D, Point2dCollection collection,IList<Point2d> usedList)
        {
            Point2d nearPoint2D = new Point2d();
            double MaxDistance = 0;
            double MinDistance = 100000;
            double _Distance = 0;

            foreach (var point in collection)
            {
                if (!point.Equals(testpoint2D) && !(usedList.Contains(point)))
                {
                    _Distance = Distance(testpoint2D, point);
                    if (_Distance > MaxDistance)
                        MaxDistance = _Distance;
                    if (_Distance < MinDistance && Math.Abs(MinDistance) > 0.0001)
                    {
                        MinDistance = _Distance;
                        nearPoint2D = point;
                    }
                }
            }
            return nearPoint2D;
        }

        /// <summary>
        /// Nearests the neighbor.
        /// </summary>
        /// <param name="testpoint2D">The testpoint2 d.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>Point2d.</returns>
        public static Point2d NearestNeighbor(Point2d testpoint2D, Point2dCollection collection)
        {
            Point2d nearPoint2D = new Point2d();
            double MaxDistance = 0;
            double MinDistance = 100000;
            double _Distance = 0;

            foreach (var point in collection)
            {
                if (!point.Equals(testpoint2D))
                {
                    _Distance = Distance(testpoint2D, point);
                    if (_Distance > MaxDistance)
                        MaxDistance = _Distance;
                    if (_Distance < MinDistance && Math.Abs(MinDistance) > 0.0001)
                    {
                        MinDistance = _Distance;
                        nearPoint2D = point;
                    }
                }
            }
            return nearPoint2D;
        }
        /// <summary>
        /// Calculates the distance between a point and a segment.
        /// Note that nearpoint is returned as well as distance
        /// </summary>
        /// <param name="testPoint">The test point.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="nearPoint">The near point.</param>
        /// <returns>System.Double.</returns>
        public static double DistanceToSegment(Point2d testPoint, Point2d startPoint, Point2d endPoint,  Point2d nearPoint)
        {
            double t;
            double deltaX = (endPoint.X - startPoint.X);
            double deltaY = (endPoint.Y - startPoint.Y);
            if (((deltaX == 0) && (deltaY == 0)))
            {
                //  It's a point not a line segment.
                deltaX = (testPoint.X - startPoint.X);
                deltaY = (testPoint.Y - startPoint.Y);
                nearPoint= (new Point2d(startPoint.X, startPoint.Y));
                return Math.Sqrt(((deltaX * deltaX) + (deltaY * deltaY)));
            }
            //  Calculate the t that minimizes the distance.
            t = ((((testPoint.X - startPoint.X) * deltaX) + ((testPoint.Y - startPoint.Y) * deltaY)) / ((deltaX * deltaX) + (deltaY * deltaY)));
            if ((t < 0))
            {
                deltaX = (testPoint.X - startPoint.X);
                deltaY = (testPoint.Y - startPoint.Y);
                nearPoint = (new Point2d(startPoint.X, startPoint.Y));
            }
            else if ((t > 1))
            {
                deltaX = (testPoint.X - endPoint.X);
                deltaY = (testPoint.Y - endPoint.Y);
                nearPoint = (new Point2d(endPoint.X, endPoint.Y));
            }
            else
            {
                deltaX = (testPoint.X - nearPoint.X);
                deltaY = (testPoint.Y - nearPoint.Y);
                nearPoint = (new Point2d(startPoint.X + (t * deltaX), startPoint.Y + (t * deltaX)));

            }
            return Math.Sqrt(((deltaX * deltaX) + (deltaY * deltaY)));
        }

        /// <summary>
        /// Distances the specified p1.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>System.Double.</returns>
        public static double Distance(Point2d p1, Point2d p2)
        {
            double dx = 0;
            double dy = 0;

            dx = Convert.ToDouble(Math.Abs(p1.X - p2.X));
            dy = Convert.ToDouble(Math.Abs(p1.Y - p2.Y));

            double distance = Math.Sqrt(dx * dx + dy * dy);

            return distance;
            //return (double)Math.Round(distance, MidpointRounding.AwayFromZero);
        }
    }
}
