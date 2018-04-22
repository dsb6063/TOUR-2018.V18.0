// ***********************************************************************
// Assembly         : BBC.SkeletonSurf.001
// Author           : Daryl Banks, PSM
// Created          : 03-28-2017
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 04-01-2017
// ***********************************************************************
// <copyright file="SkeletonSurf.cs" company="Banks & Banks Consulting">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Aec.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;

namespace BBC.SkeletonSurf
{
    /// <summary>
    /// Interface ISkeletonSurf
    /// </summary>
    public interface ISkeletonSurf
    {
        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <param name="surfId">The surf identifier.</param>
        /// <returns>System.Nullable&lt;Extents3d&gt;.</returns>
        Extents3d? GetBoundingBox(ObjectId surfId);
        /// <summary>
        /// Creates the boundary.
        /// </summary>
        /// <param name="extents">The extents.</param>
        void CreateBoundary(Extents3d extents);
        /// <summary>
        /// Gets the polyline intersections.
        /// </summary>
        /// <param name="polyline">The polyline.</param>
        /// <param name="sampleline">The sampleline.</param>
        void GetPolylineIntersections(Polyline polyline, Polyline sampleline);
        /// <summary>
        /// Creates the feature line from surf.
        /// </summary>
        /// <param name="surfId">The surf identifier.</param>
        /// <param name="start">The start.</param>
        /// <param name="endpt">The endpt.</param>
        void CreateFeatureLineFromSurf(ObjectId surfId, Point3d start, Point3d endpt);
        /// <summary>
        /// Creates the feature line from surf.
        /// </summary>
        /// <param name="surfId">The surf identifier.</param>
        /// <param name="start">The start.</param>
        /// <param name="endpt">The endpt.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="spacing">The spacing.</param>
        void CreateFeatureLineFromSurf(ObjectId surfId, Point3d start, Point3d endpt, double offset, double spacing);
        /// <summary>
        /// Gets the direction.
        /// </summary>
        /// <param name="extents">The extents.</param>
        void GetDirection(Extents3d extents);
        /// <summary>
        /// Creates the grid.
        /// </summary>
        /// <param name="extents3d">The extents3d.</param>
        /// <param name="spacing">The spacing.</param>
        /// <param name="trim">if set to <c>true</c> [trim].</param>
        void CreateGrid(Extents3d extents3d, double spacing, bool trim);
        /// <summary>
        /// Creates the closed box by lines.
        /// </summary>
        /// <param name="ptCollection">The pt collection.</param>
        void CreateClosedBoxByLines(Point2dCollection ptCollection);
        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        void CreateLine(Point2d p1, Point2d p2);

        /// <summary>
        /// Gets the extents into collection.
        /// </summary>
        /// <param name="extents">The extents.</param>
        /// <returns>Point2dCollection.</returns>
        Point2dCollection GetExtentsIntoCollection(Extents3d extents);

    }

    /// <summary>
    /// Class SkeletonSurf.
    /// </summary>
    public static class SkeletonSurf //: ISkeletonSurf

    {
        // Direction Vectors of Bounding Box

        /// <summary>
        /// The longside
        /// </summary>
        private static Vector3d longside;
        /// <summary>
        /// The shortside
        /// </summary>
        private static Vector3d shortside;

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <param name="surfaceId">The surface identifier.</param>
        /// <returns>System.Nullable&lt;Extents3d&gt;.</returns>
        public static Extents3d? GetBoundingBox(ObjectId surfaceId)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;
                if (surface != null)
                {
                    if (surface.Bounds != null) return surface.Bounds.Value;
                }
                trans.Commit();
            }

            return null;
        }
        /// <summary>
        /// Creates the boundary.
        /// </summary>
        /// <param name="extents">The extents.</param>
        /// 
        public static List<ObjectId> CreateBoundary(Extents3d extents, double interval)
        {
            var lines = new List<ObjectId>();
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Editor   ed = Application.DocumentManager.CurrentDocument.Editor;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Build Bounding Box
                CreateClosedBoxByLines(GetExtentsIntoCollection(extents));

                //Get Direction to Offset Sample Lines
                shortside = GetShortDirection(extents);
                longside  = GetLongDirection (extents);

                //Create lines perpendicular to the Long Side
                //at a specific interval

                var start     = extents.MinPoint;
                //var distance  = start.DistanceTo(new Point3d(extents.MaxPoint.X, extents.MinPoint.Y,0));
                var distance  = start.DistanceTo(new Point3d(extents.MinPoint.X, extents.MaxPoint.Y, 0));
                var linterval = interval;
                int i = 0;
                while (distance - linterval >= 0)
                {
                    var shortdis = start.DistanceTo(new Point3d(extents.MinPoint.X, extents.MaxPoint.Y,0));

                    var atBase = new Point3d(start.X +   linterval, 
                                                 start.Y,
                                                 0);

                    var atTop  = new Point3d(extents.MinPoint.X + linterval,
                                                extents.MaxPoint.Y,
                                                 0);

                    lines.Add(CreateLine(atBase,atTop));
                    linterval = interval * ++i;                    
                }
                    
                trans.Commit();
            }
            return lines;
        }

        /// <summary>
        /// Gets the polyline intersections.
        /// </summary>
        /// <param name="polyline">The polyline.</param>
        /// <param name="sampleline">The sampleline.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void GetPolylineIntersections(Polyline polyline, Polyline sampleline)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the feature line from surf.
        /// </summary>
        /// <param name="surfId">The surf identifier.</param>
        /// <param name="start">The start.</param>
        /// <param name="endpt">The endpt.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void CreateFeatureLineFromSurf(ObjectId surfId, Point3d start, Point3d endpt)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the feature line from surf.
        /// </summary>
        /// <param name="surfId">The surf identifier.</param>
        /// <param name="start">The start.</param>
        /// <param name="endpt">The endpt.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="spacing">The spacing.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void CreateFeatureLineFromSurf(ObjectId surfId, Point3d start, Point3d endpt, double offset,
            double spacing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        /// <param name="extents">The extents.</param>
        public static Vector3d GetShortDirection(Extents3d extents)
        {
            var p10 = extents.MinPoint;
            var p22 = extents.MaxPoint;
            var p12 = new Point3d(p10.X, p22.Y, 0);
            var p21 = new Point3d(p22.X, p10.Y, 0);

            if (p10.DistanceTo(p21) <= p10.DistanceTo(p12))
                return p10.GetVectorTo(p12);
            return p10.GetVectorTo(p21);
        }

        public static Vector3d GetLongDirection(Extents3d extents)
        {
            var p10 = extents.MinPoint;
            var p22 = extents.MaxPoint;
            var p12 = new Point3d(p10.X, p22.Y, 0);
            var p21 = new Point3d(p22.X, p10.Y, 0);

            if (p10.DistanceTo(p21) > p10.DistanceTo(p12))
                return p10.GetVectorTo(p21);
            return p10.GetVectorTo(p21);
        }

        /// <summary>
        /// Creates the grid.
        /// </summary>
        /// <param name="extents3d">The extents3d.</param>
        /// <param name="spacing">The spacing.</param>
        /// <param name="trim">if set to <c>true</c> [trim].</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void CreateGrid(Extents3d extents3d, double spacing, bool trim)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the closed box by lines.
        /// </summary>
        /// <param name="ptCollection">The pt collection.</param>
        public static void CreateClosedBoxByLines(Point2dCollection ptCollection)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < ptCollection.Count-1;i++)
                {
                     CreateLine(ptCollection[i],ptCollection[i+1]);            
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        public static ObjectId CreateLine(Point3d p1, Point3d p2)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            var outLine = ObjectId.Null;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                    OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                // Create a line that starts at 5,5 and ends at 12,3
                using (Line acLine = new Line(new Point3d(p1.X, p1.Y, p1.Z),
                    new Point3d(p2.X, p2.Y, p2.Z)))
                {
                    acLine.Color= Color.FromRgb(23, 54, 232);

                    // Add the new object to the block table record and the transaction
                    outLine = acBlkTblRec.AppendEntity(acLine);
                    acTrans.AddNewlyCreatedDBObject(acLine, true);
                }

                // Save the new object to the database
                acTrans.Commit();
            }
            return outLine;
        }


        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        public static void CreateLine(Point2d p1, Point2d p2)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                    OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                // Create a line that starts at 5,5 and ends at 12,3
                using (Line acLine = new Line(new Point3d(p1.X, p1.Y, 0),
                    new Point3d(p2.X, p2.Y, 0)))
                {

                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acLine);
                    acTrans.AddNewlyCreatedDBObject(acLine, true);
                }

                // Save the new object to the database
                acTrans.Commit();
            }
        }

        /// <summary>
        /// Gets the extents into collection.
        /// </summary>
        /// <param name="extents">The extents.</param>
        /// <returns>Point2dCollection.</returns>
        public static Point2dCollection GetExtentsIntoCollection(Extents3d extents)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                var max = new Point2d(extents.MaxPoint.X, extents.MaxPoint.Y);
                var min = new Point2d(extents.MinPoint.X, extents.MinPoint.Y);
                var minY = new Point2d(min.X, max.Y);
                var maxX = new Point2d(max.X, min.Y);

                var pcoll = new Point2dCollection();
                pcoll.Add(min);
                pcoll.Add(maxX);
                pcoll.Add(max);
                pcoll.Add(minY);
                pcoll.Add(min);

                return pcoll;
            }
        }
    }
}

