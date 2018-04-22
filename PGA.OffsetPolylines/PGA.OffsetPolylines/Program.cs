// ***********************************************************************
// Assembly         : PGA.OffsetPolylines
// Author           : Daryl Banks, PSM
// Created          : 02-18-2017
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 03-10-2017
// ***********************************************************************
// <copyright file="Program.cs" company="Banks & Banks Consulting">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Exception = System.Exception;
using COMS = PGA.MessengerManager;
using Acaddb =global::Autodesk.AutoCAD.DatabaseServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace PGA.OffsetPolylines
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Corrects the Polylines by removing intersections.
        /// </summary>
        [CommandMethod("PGA-CorrectPolylines")]
        public static void OffsetGreen()
        {
            try
            {
                var pids = GetAllPolylines();
                //var OGR  = GetGreen(pids);

                //if (OGR == null)
                //    return;

                //var refcentroid = GetCentroidOfOuterBoundary(GetPolylinePoints(OGR.ObjectId));
                //var collars  = GetCollar(pids);

                // AdjustCollars(collars);

                // var roughouts = GetRoughOutline(pids);

                // AdjustRoughLayouts(roughouts);


                var offset = -0.0001;
                var posOffset = 0.0001;

                foreach (Acaddb.ObjectId oid in pids)
                {
                    offset = -0.0001;

                    Acaddb.Polyline polyline = GetPolyline((oid));

                    if (polyline == null)
                        return;

                    if (polyline.Layer == "OGR")
                        continue;
                    if (polyline.Layer == "OCO")
                        offset = posOffset;
                    else if (polyline.Layer == "OIR")
                        offset = posOffset;

                    var newpoly = polyline.GetOffsetCurves(offset).Cast<Acaddb.Polyline>();

                    newpoly.FirstOrDefault().Layer = polyline.Layer;

                    // Get the current document and database

                    var acDoc = Application.DocumentManager.MdiActiveDocument;
                    var acCurDb = acDoc.Database;

                    using (var acTrans = Application.DocumentManager.MdiActiveDocument
                        .TransactionManager.StartTransaction())
                    {
                        // Open the Block table for read
                        Acaddb.BlockTable acBlkTbl;
                        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                            Acaddb.OpenMode.ForRead) as Acaddb.BlockTable;

                        // Open the Block table record Model space for write
                        Acaddb.BlockTableRecord acBlkTblRec;
                        acBlkTblRec = acTrans.GetObject(acBlkTbl[Acaddb.BlockTableRecord.ModelSpace],
                            Acaddb.OpenMode.ForWrite) as Acaddb.BlockTableRecord;
                        // Add each offset object
                        acBlkTblRec.AppendEntity(newpoly.FirstOrDefault());
                        acTrans.AddNewlyCreatedDBObject(newpoly.FirstOrDefault(), true);
                        acTrans.Commit();
                    }
                }

                DeletePolys(pids);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }
        /// <summary>
        /// Scales the water object.
        /// </summary>
        [CommandMethod("PGA-ScaleWaterObjects")]
        public static void ScaleWaterObject()
        {

            // Get the current document and database

            try
            {
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Acaddb.Database acCurDb = acDoc.Database;

                var pids = GetAllPolylines();
                var pidsToDelete = new Acaddb.ObjectIdCollection();

                var offset = -0.001;

                foreach (Acaddb.ObjectId oid in pids)
                {
                    offset = -0.001;

                    using (acDoc.LockDocument())
                    {

                        using (var acTrans = Application.DocumentManager.MdiActiveDocument
                            .TransactionManager.StartTransaction())
                        {

                            Acaddb.DBObject obj =
                                acTrans.GetObject(oid, Acaddb.OpenMode.ForWrite);

                            Acaddb.Polyline polyline = obj as Acaddb.Polyline;

                            if (polyline != null)
                            {
                                if (polyline.Closed)
                                {

                                    if (polyline.Layer != "OWA")
                                        continue;
                                    var centroid = GetCentroidOfOuterBoundary(GetPolylinePoints(oid));
                                    var centroid3d = new Point3d(centroid.X, centroid.Y, 0.0);
                                    var closepnt = polyline.GetClosestPointTo(centroid3d, true);
                                    var radius = polyline.GetDistAtPoint(closepnt);
                                    var scaleFac = 1 + offset/Math.Abs(radius);

                                    polyline.TransformBy(Matrix3d.Scaling(scaleFac,
                                        new Point3d(centroid.X, centroid.Y, 0)));
                                }
                            }


                            acTrans.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessengerManager.MessengerManager.LogException(ex);
            }

        }


        public static Acaddb.ObjectId ScaleNewPolylineObject(Point2dCollection points)
        {
            var offset = -0.001;
            var newoid = Acaddb.ObjectId.Null;
            var doc = Active.Document;

            try
            {
                // Get the current document and database
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Acaddb.Database acCurDb = acDoc.Database;
                using (acDoc.LockDocument())
                {
                    // Start a transaction
                    using (Acaddb.Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        // Open the Block table for read
                        Acaddb.BlockTable acBlkTbl;
                        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                            Acaddb.OpenMode.ForRead) as Acaddb.BlockTable;

                        // Open the Block table record Model space for write
                        Acaddb.BlockTableRecord acBlkTblRec;
                        acBlkTblRec = acTrans.GetObject(acBlkTbl[Acaddb.BlockTableRecord.ModelSpace],
                            Acaddb.OpenMode.ForWrite) as Acaddb.BlockTableRecord;

                        // Create a lightweight polyline
                        using (Acaddb.Polyline acPoly = new Acaddb.Polyline())
                        {

                            int i = 0;
                            foreach (Point2d pnt in points)
                            {
                                acPoly.AddVertexAt(i++, pnt, 0, 0, 0);
                            }
                            // Close the polyline
                            acPoly.Closed = true;

                            SendPolylineMessage("Length Info Before", acPoly);

                            var centroid = GetCentroidOfOuterBoundary(points);
                            var centroid3d = new Point3d(centroid.X, centroid.Y, 0.0);
                            var closepnt = acPoly.GetClosestPointTo(centroid3d, true);
                            var radius = acPoly.GetDistAtPoint(closepnt);
                            var scaleFac = 1 + (offset/Math.Abs(radius));
                            acPoly.TransformBy(Matrix3d.Scaling(scaleFac, new Point3d(centroid.X, centroid.Y, 0)));


                            SendPolylineMessage("Length Info After", acPoly);

                            // Add the new object to the block table record and the transaction

                            newoid = acBlkTblRec.AppendEntity(acPoly);
                            acTrans.AddNewlyCreatedDBObject(acPoly, true);

                            // Save the new objects to the database
                            acTrans.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }

            return newoid;
        }

        /// <summary>
        /// Scales the object.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void ScaleObject(string name)
        {

            try
            {
                MessengerManager.MessengerManager.AddLog("Start Scaling");
 

                var pids = GetAllPolylines();
                var pidsToDelete = new Acaddb.ObjectIdCollection();
                var doc = Active.Document;

                var offset = -0.001;

                foreach (Acaddb.ObjectId oid in pids)
                {
                    offset = -0.001;


                    using (doc.LockDocument())
                    {
                        using (var acTrans = Application.DocumentManager.MdiActiveDocument
                            .TransactionManager.StartTransaction())
                        {

                            Acaddb.DBObject obj =
                                acTrans.GetObject(oid, Acaddb.OpenMode.ForWrite);

                            Acaddb.Polyline polyline = obj as Acaddb.Polyline;

                            if (polyline != null)
                            {
                                if (polyline.Closed)
                                {

                                    if (polyline.Layer != name.ToUpper())
                                        continue;

                                    SendPolylineMessage("Length Info Before", polyline);

                                    var centroid = GetCentroidOfOuterBoundary(GetPolylinePoints(oid));
                                    var centroid3d = new Point3d(centroid.X, centroid.Y, 0.0);
                                    var closepnt = polyline.GetClosestPointTo(centroid3d, true);
                                    var radius = polyline.GetDistAtPoint(closepnt);
                                    var scaleFac = 1 + (offset/Math.Abs(radius));
                                    polyline.TransformBy(Matrix3d.Scaling(scaleFac,
                                        new Point3d(centroid.X, centroid.Y, 0)));


                                    SendPolylineMessage("Length Info After", polyline);

                                    MessengerManager.MessengerManager.AddLog
                                    (String.Format("Scale Info {0},{1},{2},{3},{4}",
                                        centroid,
                                        centroid3d,
                                        closepnt,
                                        radius,
                                        scaleFac));

                                }
                                else
                                {
                                    SendPolylineMessage("Polyline is not closed! ", polyline);
                                }
                            }
                            else
                            {
                                SendPolylineMessage("Polyline is NULL! ", polyline);
                            }

                            acTrans.Commit();
                        }
                        MessengerManager.MessengerManager.AddLog("End Scaling");

                    }
                }
            }
            catch (Exception ex)
            {

                MessengerManager.MessengerManager.LogException(ex);
            }

        }
        /// <summary>
        /// Tests the scale object.
        /// </summary>
        [CommandMethod("TestPGA-ScaleObjects")]

        public static void TestScaleObject()
        {

            try
            {
                MessengerManager.MessengerManager.AddLog("Start Scaling");


                var pids = GetAllPolylines();
                var pidsToDelete = new Acaddb.ObjectIdCollection();

                var offset = -0.001;

                foreach (Acaddb.ObjectId oid in pids)
                {
                    offset = -0.001;



                    using (var acTrans = Application.DocumentManager.MdiActiveDocument
                        .TransactionManager.StartTransaction())
                    {

                        Acaddb.DBObject obj =
                            acTrans.GetObject(oid, Acaddb.OpenMode.ForWrite);

                        Acaddb.Polyline polyline = obj as Acaddb.Polyline;

                        if (polyline != null)
                        {
                            if (polyline.Closed)
                            {

                                if (polyline.Layer != "S-TEE-BOX")
                                    continue;

                                SendPolylineMessage("Length Info Before", polyline);

                                var centroid = GetCentroidOfOuterBoundary(GetPolylinePoints(oid));
                                var centroid3d = new Point3d(centroid.X, centroid.Y, 0.0);
                                var closepnt = polyline.GetClosestPointTo(centroid3d, true);
                                var radius = polyline.GetDistAtPoint(closepnt);
                                var scaleFac = 1 + (offset / Math.Abs(radius));
                                polyline.TransformBy(Matrix3d.Scaling(scaleFac, new Point3d(centroid.X, centroid.Y, 0)));


                                SendPolylineMessage("Length Info After", polyline);

                                MessengerManager.MessengerManager.AddLog
                                    (String.Format("Scale Info {0},{1},{2},{3},{4}",
                                    centroid,
                                    centroid3d,
                                    closepnt,
                                    radius,
                                    scaleFac));

                            }
                            else
                            {
                                SendPolylineMessage("Polyline is not closed! ", polyline);
                            }
                        }
                        else
                        {
                            SendPolylineMessage("Polyline is NULL! ", polyline);
                        }

                        acTrans.Commit();
                    }
                    MessengerManager.MessengerManager.AddLog("End Scaling");

                }
            }
            catch (Exception ex)
            {

                MessengerManager.MessengerManager.LogException(ex);
            }

        }



        /// <summary>
        /// Sends the polyline message.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="polyline">The polyline.</param>
        public static void SendPolylineMessage(string info, Acaddb.Polyline polyline)
        {
            MessengerManager.MessengerManager.AddLog
              (String.Format(info + " L={0},A={1}", polyline.Length, polyline.Area));
        }

        /// <summary>
        /// Offsets the owa features.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Polyline is Null</exception>
        public static void OffsetOWAFeatures()
        {
            try
            {
                MessengerManager.MessengerManager.AddLog("Start Offset Water Features");

                var pidsToDelete = new Acaddb.ObjectIdCollection();
                var pids = GetAllPolylines();


                foreach (Acaddb.ObjectId oid in pids)
                {
                   var  offset = -0.001;

                    Acaddb.Polyline polyline = GetPolyline((oid));

                    if (polyline == null)
                        throw new ArgumentNullException("Polyline is Null");
                    

                    if (polyline.Layer != "OWA")
                        continue;

                    var newpoly = polyline.GetOffsetCurves(offset).Cast<Acaddb.Polyline>();

                    newpoly.FirstOrDefault().Layer = polyline.Layer;

                    // Get the current document and database

                    var acDoc = Application.DocumentManager.MdiActiveDocument;
                    var acCurDb = acDoc.Database;

                    using (var acTrans = Application.DocumentManager.MdiActiveDocument
                        .TransactionManager.StartTransaction())
                    {
                        // Open the Block table for read
                        Acaddb.BlockTable acBlkTbl;
                        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                            Acaddb.OpenMode.ForRead) as Acaddb.BlockTable;

                        // Open the Block table record Model space for write
                        Acaddb.BlockTableRecord acBlkTblRec;
                        acBlkTblRec = acTrans.GetObject(acBlkTbl[Acaddb.BlockTableRecord.ModelSpace],
                            Acaddb.OpenMode.ForWrite) as Acaddb.BlockTableRecord;
                        // Add each offset object
                        acBlkTblRec.AppendEntity(newpoly.FirstOrDefault());
                        acTrans.AddNewlyCreatedDBObject(newpoly.FirstOrDefault(), true);
                        acTrans.Commit();
                    }
                    pidsToDelete.Add(oid);
                }

                DeletePolys(pidsToDelete);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
            finally
            {
                MessengerManager.MessengerManager.AddLog("End Offset Water Features");
            }
        }
        /// <summary>
        /// Offsets the features by -0.001 ft
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">layer name</exception>
        public static void OffsetFeatures(string name)
        {
            try
            {
                MessengerManager.MessengerManager.AddLog("Start Offset Features");

                if (String.IsNullOrEmpty(name))
                    throw new ArgumentNullException("Layer Name");

                var pidsToDelete = new Acaddb.ObjectIdCollection();
                var pids = GetAllPolylines();

                //var refcentroid = GetCentroidOfOuterBoundary(GetPolylinePoints(OGR.ObjectId));


                var offset = -0.0001; //inital start value

                foreach (Acaddb.ObjectId oid in pids)
                {
                    offset = -0.001;

                    Acaddb.Polyline polyline = GetPolyline((oid));

                    if (polyline == null)
                        throw new ArgumentNullException("Polyline is Null"); 

                    if (polyline.Layer != name.ToUpper())
                        continue;

                    MessengerManager.MessengerManager.AddLog("Found Offset Features on Layer " + name);

                    var newpoly = polyline.GetOffsetCurves(offset).Cast<Acaddb.Polyline>();

                    newpoly.FirstOrDefault().Layer = polyline.Layer;

                    // Get the current document and database

                    var acDoc = Application.DocumentManager.MdiActiveDocument;
                    var acCurDb = acDoc.Database;

                    using (var acTrans = Application.DocumentManager.MdiActiveDocument
                        .TransactionManager.StartTransaction())
                    {
                        // Open the Block table for read
                        Acaddb.BlockTable acBlkTbl;
                        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                            Acaddb.OpenMode.ForRead) as Acaddb.BlockTable;

                        // Open the Block table record Model space for write
                        Acaddb.BlockTableRecord acBlkTblRec;
                        acBlkTblRec = acTrans.GetObject(acBlkTbl[Acaddb.BlockTableRecord.ModelSpace],
                            Acaddb.OpenMode.ForWrite) as Acaddb.BlockTableRecord;
                        // Add each offset object
                        acBlkTblRec.AppendEntity(newpoly.FirstOrDefault());
                        acTrans.AddNewlyCreatedDBObject(newpoly.FirstOrDefault(), true);
                        acTrans.Commit();
                    }
                    pidsToDelete.Add(oid);
                }

                DeletePolys(pidsToDelete);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
            finally
            {
                MessengerManager.MessengerManager.AddLog("End Offset Features");
            }
        }


        /// <summary>
        /// Gets all polylines.
        /// </summary>
        /// <returns>ACADDB.ObjectIdCollection.</returns>
        private static Acaddb.ObjectIdCollection GetAllPolylines()
        {
            return GetIdsByTypeTypeValue(
                "POLYLINE",
                "LWPOLYLINE",
                "POLYLINE2D");
        }

        /// <summary>
        /// Gets the ids by type type value.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>Acaddb.ObjectIdCollection.</returns>
        public static Acaddb.ObjectIdCollection GetIdsByTypeTypeValue(params string[] types)
        {

            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;

            // Get the editor to make the selection
            Editor oEd = doc.Editor;

            // Add our or operators so we can grab multiple types.
            IList<Acaddb.TypedValue> typedValueSelection = new List<Acaddb.TypedValue>
            {
                new Acaddb.TypedValue(Convert.ToInt32(Acaddb.DxfCode.Operator), "<or"),
                new Acaddb.TypedValue(Convert.ToInt32(Acaddb.DxfCode.Operator), "or>")
            };

            // We will need to insert our requested types into the collection.
            // Since we knew we would have to insert they types inbetween the operators..
            // I used a Enumerable type which gave me that functionallity. (IListf<T>)
            foreach (var type in types)
                typedValueSelection.Insert(1, new Acaddb.TypedValue(Convert.ToInt32(Acaddb.DxfCode.Start), type));

            SelectionFilter selectionFilter = new SelectionFilter(typedValueSelection.ToArray());

            // because we have to.. Not really sure why, I assume this is our only access point
            // to grab the entities that we want. (I am open to being corrected)
            PromptSelectionResult promptSelectionResult = oEd.SelectAll(selectionFilter);

            // return our new ObjectIdCollection that is "Hopefully" full of the types that we want.
            return new Acaddb.ObjectIdCollection(promptSelectionResult.Value.GetObjectIds());
        }

        /// <summary>
        /// Gets the centroid of outer boundary.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Point2d.</returns>
        private static Point2d GetCentroidOfOuterBoundary(Point2dCollection points)
        {
            try
            {
                return BBC.Common.AutoCAD.AcadUtilities.GetCentriod(points);
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return new Point2d();

        }


        /// <summary>
        /// Gets the polyline points.
        /// </summary>
        /// <param name="poid">The poid.</param>
        /// <returns>Point2dCollection.</returns>
        private static Point2dCollection GetPolylinePoints(Acaddb.ObjectId poid)
        {
            Point2dCollection points;

            using (Acaddb.Transaction tr = Active.StartTransaction())
            {
                points = new Point2dCollection();

                Acaddb.DBObject obj =
                    tr.GetObject(poid, Acaddb.OpenMode.ForRead);

                Acaddb.Polyline lwp = obj as Acaddb.Polyline;

                if (lwp != null)
                {
                    if (lwp.Closed)
                    {
                        int vn = lwp.NumberOfVertices;

                        for (int i = 0; i < vn; i++)
                        {
                            Point2d pt = lwp.GetPoint2dAt(i);
                            points.Add(pt);
                        }
                    }            
                }
  
            }

            return points;
        }


        /// <summary>
        /// Gets the polyline.
        /// </summary>
        /// <param name="poid">The poid.</param>
        /// <returns>Acaddb.Polyline.</returns>
        private static Acaddb.Polyline GetPolyline(Acaddb.ObjectId poid)
        {

            using (Acaddb.Transaction tr = Active.StartTransaction())
            {
                Acaddb.DBObject obj =
                    tr.GetObject(poid, Acaddb.OpenMode.ForRead);

                Acaddb.Polyline lwp = obj as Acaddb.Polyline;

                if (lwp != null)
                {
                    if (lwp.Closed)
                    {
                        return lwp;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the green.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>ACADDB.Polyline.</returns>
        private static Acaddb.Polyline GetGreen(Acaddb.ObjectIdCollection collection)
        {
            try
            {
                var theClass = RXObject.GetClass(typeof(Acaddb.Polyline));

                foreach (Acaddb.ObjectId oid in collection)
                {
                    if (!oid.ObjectClass.IsDerivedFrom(theClass))
                        continue;

                    var polyline = GetObject(oid);
                    if (polyline == null)
                        continue;
                    if (polyline.Layer.Length < 8)
                        if (polyline.Layer.Equals("OGR") ||
                            polyline.Layer.Contains("S-GREEN"))
                            return polyline;
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }
        /// <summary>
        /// Gets the collar.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>Acaddb.Polyline.</returns>
        private static Acaddb.Polyline GetCollar(Acaddb.ObjectIdCollection collection)
        {
            try
            {
                var theClass = RXObject.GetClass(typeof(Acaddb.Polyline));

                foreach (Acaddb.ObjectId oid in collection)
                {
                    if (!oid.ObjectClass.IsDerivedFrom(theClass))
                        continue;

                    var polyline = GetObject(oid);
                    if (polyline == null)
                        continue;
                    if (polyline.Layer.Length < 8)
                        if (polyline.Layer.Equals("OCA") ||
                            polyline.Layer.Contains("COLLAR"))
                            return polyline;
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }
        /// <summary>
        /// Gets the rough outline.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>Acaddb.Polyline.</returns>
        private static Acaddb.Polyline GetRoughOutline(Acaddb.ObjectIdCollection collection)
        {
            try
            {
                var theClass = RXObject.GetClass(typeof(Acaddb.Polyline));

                foreach (Acaddb.ObjectId oid in collection)
                {
                    if (!oid.ObjectClass.IsDerivedFrom(theClass))
                        continue;

                    var polyline = GetObject(oid);
                    if (polyline == null)
                        continue;
                    if (polyline.Layer.Length < 8)
                        if (polyline.Layer.Equals("ORO") ||
                            polyline.Layer.Contains("ROUGH-OUTLINE"))
                            return polyline;
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }



        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="polyoid">The polyoid.</param>
        /// <returns>ACADDB.Polyline.</returns>
        private static Acaddb.Polyline GetObject(Acaddb.ObjectId polyoid)
        {
            using (var tr = Application.DocumentManager.MdiActiveDocument
                .TransactionManager.StartTransaction())
            {
                var obj =
                    tr.GetObject(polyoid, Acaddb.OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                var lwp = obj as Acaddb.Polyline;

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (lwp.Closed)
                    {
                        return lwp;
                    }
                    lwp.UpgradeOpen();
                    lwp.Closed = true;
                    lwp.DowngradeOpen();
                    return lwp;
                }
                return null;
            }
        }

        /// <summary>
        /// Deletes the polys.
        /// </summary>
        /// <param name="oids">The oids.</param>
        /// <exception cref="System.ArgumentNullException">p</exception>
        public static void DeletePolys(Acaddb.ObjectIdCollection oids)
        {
            using (var db = Active.WorkingDatabase)
            {
                foreach (Acaddb.ObjectId objectId in oids)
                {
                    try
                    {
                        using (var tr = Active.StartTransaction())
                        {
                            var p = tr.GetObject(objectId, Acaddb.OpenMode.ForRead) as Acaddb.Polyline;
                            if (p == null) throw new ArgumentNullException(nameof(p));
                            if (p.Layer == "OGR")
                                continue;
                            p.UpgradeOpen();
                            p.Erase();
                            tr.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        COMS.MessengerManager.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Scales the object.
        /// </summary>
        /// <param name="polyId">The poly identifier.</param>
        public static void ScaleObject(Acaddb.ObjectId polyId)
        {

            try
            {
                MessengerManager.MessengerManager.AddLog("Start Scaling");

                var offset = -0.01;

                    using (var acTrans = Application.DocumentManager.MdiActiveDocument
                        .TransactionManager.StartTransaction())
                    {

                        Acaddb.DBObject obj =
                            acTrans.GetObject(polyId, Acaddb.OpenMode.ForWrite);

                        Acaddb.Polyline polyline = obj as Acaddb.Polyline;

                        if (polyline != null)
                        {
                            if (polyline.Closed)
                            {

                                SendPolylineMessage("Length Info Before", polyline);

                                var centroid = GetCentroidOfOuterBoundary(GetPolylinePoints(polyId));
                                var centroid3d = new Point3d(centroid.X, centroid.Y, 0.0);
                                var closepnt = polyline.GetClosestPointTo(centroid3d, true);
                                var radius = polyline.GetDistAtPoint(closepnt);
                                var scaleFac = 1 + (offset / Math.Abs(radius));
                                polyline.TransformBy(Matrix3d.Scaling(scaleFac, new Point3d(centroid.X, centroid.Y, 0)));


                                SendPolylineMessage("Length Info After", polyline);

                                MessengerManager.MessengerManager.AddLog
                                    (String.Format("Scale Info {0},{1},{2},{3},{4}",
                                    centroid,
                                    centroid3d,
                                    closepnt,
                                    radius,
                                    scaleFac));

                            }
                            else
                            {
                                SendPolylineMessage("Polyline is not closed! ", polyline);
                            }
                        }
                        else
                        {
                            SendPolylineMessage("Polyline is NULL! ", polyline);
                        }

                        acTrans.Commit();
                    }
                    MessengerManager.MessengerManager.AddLog("End Scaling");
            }
            catch (Exception ex)
            {

                MessengerManager.MessengerManager.LogException(ex);
            }
        }
    }


}
