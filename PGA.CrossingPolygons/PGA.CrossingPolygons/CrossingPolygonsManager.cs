using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ACAD = Autodesk.AutoCAD.ApplicationServices;
using ACADRT = Autodesk.AutoCAD.Runtime;
using ACADDB = Autodesk.AutoCAD.DatabaseServices;


namespace PGA.CrossingPolygons
{



    /// <exclude />
    public static class CrossingPolygonsManager
    {
        public static ACADDB.Region AddRegion(ACADDB.ObjectId acadObjectId)
        {
            ACADDB.Region returnvalue = null;
            // Get the current document and database
            ACAD.Document acDoc = ACAD.Application.DocumentManager.MdiActiveDocument;
            ACADDB.Database acCurDb = acDoc.Database;

            // Start a transaction
            using (ACADDB.Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                ACADDB.BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                    ACADDB.OpenMode.ForRead) as ACADDB.BlockTable;

                // Open the Block table record Model space for write
                ACADDB.BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[ACADDB.BlockTableRecord.ModelSpace],
                    ACADDB.OpenMode.ForWrite) as ACADDB.BlockTableRecord;

                ACADDB.Polyline polyline = acTrans.GetObject(acadObjectId,
                    ACADDB.OpenMode.ForRead) as ACADDB.Polyline;
                if (polyline != null)

                {
                    ACADDB.DBObjectCollection acDBObjColl = new ACADDB.DBObjectCollection();
                    //acDBObjColl.Add((ACADDB.DBObject) polyline.AcadObject);
                    polyline.Explode(acDBObjColl);
                    // Calculate the regions based on each closed loop
                    ACADDB.DBObjectCollection myRegionColl = new ACADDB.DBObjectCollection();
                    myRegionColl = ACADDB.Region.CreateFromCurves(acDBObjColl);
                    ACADDB.Region acRegion = myRegionColl[0] as ACADDB.Region;
                    returnvalue = acRegion;
                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acRegion);
                    acTrans.AddNewlyCreatedDBObject(acRegion, true);

                    // Dispose 
                }

                // Save the new object to the database
                acTrans.Commit();
            }
            return returnvalue;
        }

        public static ACADDB.Region CompareResultsofRegion(ACADDB.Region region1, ACADDB.Region region2)
        {
            var outterRegion = region1;
            var innerRegion = region2;

            if (outterRegion.Area > innerRegion.Area)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);

            }
            return outterRegion;
        }

        public static ACADDB.Region SubtractRegions(ACADDB.ObjectId outter, ACADDB.ObjectId inner)
        {
            var outterRegion = AddRegion(outter);
            var innerRegion = AddRegion(inner);

            if (outterRegion.Area > innerRegion.Area)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, innerRegion);
                innerRegion.Dispose();
                return outterRegion;

            }
            else
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, outterRegion);
                outterRegion.Dispose();
                return innerRegion;
            }

        }

        public static ACADDB.Region UniteRegions(ACADDB.ObjectId outter, ACADDB.ObjectId inner)
        {
            var outterRegion = AddRegion(outter);
            var innerRegion = AddRegion(inner);

            if (outterRegion.Area > innerRegion.Area)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, innerRegion);
                innerRegion.Dispose();
                return outterRegion;

            }
            else
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, outterRegion);
                outterRegion.Dispose();
                return innerRegion;
            }
        }

        public static ACADDB.Region UniteRegions(ACADDB.ObjectId outter, ACADDB.ObjectId inner, bool firstchoice)
        {
            var outterRegion = AddRegion(outter);
            var innerRegion = AddRegion(inner);

            if (firstchoice)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, innerRegion);
                innerRegion.Dispose();
                return outterRegion;

            }
            else
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, outterRegion);
                outterRegion.Dispose();
                return innerRegion;
            }
        }
        public static void MatchOverlappingRegions(ACADDB.Region region1, ACADDB.Region region2,
            ACADDB.Polyline polyline)
        {
            var outterRegion = region1;
            var innerRegion = region2;
            Point2dCollection polylinepoints = null;

            if (polyline != null)
            {
                polylinepoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(polyline);
            }
            if (outterRegion.Area > innerRegion.Area)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);

            }


            if (outterRegion != null)
            {
                ACADDB.DBObjectCollection objs =
                    PolylineFromRegion(outterRegion);

                foreach (ACADDB.Entity ent in objs)
                {
                    var extents = ent.Bounds;
                    var upper = extents.Value.MaxPoint;
                    var lower = extents.Value.MinPoint;
                    var poly = ent as ACADDB.Polyline;
                    Point2dCollection tempcol = null;
                    if (poly != null && polylinepoints != null)
                    {
                        tempcol = new Point2dCollection();
                        var intpoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(poly);
                        //search
                        for (int i = 0; i < intpoints.Count; i++)
                        {
                            var result = polylinepoints.IndexOf(intpoints[i]);
                            if (result == -1)
                            {
                                tempcol.Add(intpoints[i]);
                            }
                        }

                        //replace
                        for (int i = 0; i < intpoints.Count; i++)
                        {
                            if (!tempcol.Contains(intpoints[i]))
                            {
                                var result = polylinepoints.IndexOf(intpoints[i]);
                                if (result != -1)
                                {
                                    polylinepoints.RemoveAt(result);
                                    polylinepoints.Add(PGA.SimplifyPolylines.Commands.NearestNeighbor(intpoints[i],
                                        tempcol));
                                }
                            }
                        }
                    }
                }

                PGA.SimplifyPolylines.Commands.CreateSimplifiedPolylines
                    (polyline.Layer, 0, polylinepoints);

            }

        }


        public static void MatchOverlappingRegionsByIntersect(ACADDB.Polyline region1, ACADDB.Polyline region2)
        {
            int startindex1 = 0;
            int startindex2 = 0;
            int secindex1 = 0;
            int secindex2 = 0;
            bool btemp1 = false;
            var outterRegion = AddRegion(region1.ObjectId);
            var innerRegion  = AddRegion(region2.ObjectId);
            var isYIncreasing2 = false;
            var isYIncreasing1 = false;
            ACADDB.Region intRegion;
            Point2dCollection temp1        = null;
            Point2dCollection temp2        = null;
            Point2dCollection plinepoints1 = null;
            Point2dCollection plinepoints2 = null;
            Point2dCollection intpoints    = null;


             if (outterRegion.Area > 0 && innerRegion.Area > 0)

             { 
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intRegion = outterRegion; //Assign X-Region

                 if (intRegion.Area > 0)
                 {
                     //Collection of Polyline X-Sections

                     ACADDB.DBObjectCollection objs =
                         PolylineFromRegion(outterRegion);

                     //Looping Polyline X-Sections

                     foreach (ACADDB.Entity ent in objs)
                     {
                         do
                         {
                             startindex1 = 0;
                             startindex2 = 0;
                             secindex1 = 0;
                             secindex2 = 0;
                             btemp1 = false;
                             isYIncreasing2 = false;
                             isYIncreasing1 = false;                   
                             temp1 = null;
                             temp2 = null;
                             plinepoints1 = null;
                             plinepoints2 = null;
                             intpoints = null;

                            //Get the Polyline Points
                            plinepoints1 = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(region1);
                             plinepoints2 = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(region2);

                             //Get Extents of Polyline Loop
                             ACADDB.Extents3d extents = ent.Bounds.Value;
                             var upper = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(extents.MaxPoint);
                             var lower = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(extents.MinPoint);

                             //Find Direction of Pline
                             var polyDirIncreasing = false;


                             //X-Section Pline to Points
                             var poly = ent as ACADDB.Polyline;

                             if (poly != null && plinepoints2 != null)
                             {
                                 temp1 = new Point2dCollection();
                                 temp2 = new Point2dCollection();
                                 var first = new Point2d();
                                 var last = new Point2d();
                                 //X-Section Points
                                 intpoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(poly);

                                 polyDirIncreasing = IsYCoordinateIncreasing(intpoints[0], intpoints[1]);

                                 //search polyline for first and last -1 points
                                 // first and last are same to   close    pline

                                 for (int i = 0; i < intpoints.Count; i++)
                                 {
                                     if (intpoints.Count <= 2)
                                         return;

                                     first = intpoints[0];
                                     last = intpoints[intpoints.Count - 2];


                                     //Find matching points with X-Section

                                     var result1 = plinepoints1.IndexOf(intpoints[i]);
                                     var result2 = plinepoints2.IndexOf(intpoints[i]);
                                     if (result1 != -1)
                                     {
                                         if (!temp1.Contains(intpoints[i]))
                                             temp1.Add(intpoints[i]);
                                     }

                                     if (result2 != -1)
                                     {
                                         if (!temp2.Contains(intpoints[i]))
                                             temp2.Add(intpoints[i]);
                                     }
                                 }
                                 if (plinepoints1.Contains(first) && plinepoints2.Contains(first))
                                 {

                                     if (temp1.Count > temp2.Count)
                                     {
                                         btemp1 = true;
                                     }

                                     startindex1 = plinepoints1.IndexOf(first);
                                     startindex2 = plinepoints2.IndexOf(first);
                                     secindex1 = 0;
                                     secindex2 = 0;

                                     if (temp1.Contains(last))
                                         secindex1 = plinepoints1.IndexOf(last);

                                     secindex2 = startindex2 + 1;
                                     secindex1 = startindex1 + 1;
                                     if( plinepoints1.Count < startindex1 + 1)
                                     {
                                         isYIncreasing1 = IsYCoordinateIncreasing(plinepoints1[startindex1],
                                             plinepoints1[startindex1 + 1]);
                                     }
                                     if (plinepoints2.Count < startindex2 + 1)
                                     {
                                         isYIncreasing2 = IsYCoordinateIncreasing(plinepoints2[startindex2],
                                             plinepoints2[startindex2 + 1]);
                                     }
                                     if (isYIncreasing1 != isYIncreasing2)
                                         ReversePolylineDirection(region2);
                                 }
                             }

                             Debug.WriteLine("Now Greater Than Search Value");

                         } while (isYIncreasing2 != isYIncreasing1);
                     
                             for (int i = 1; i < intpoints.Count-2; i++)
                             {
                            if (plinepoints1.Contains(intpoints[i]))
                                plinepoints1.Remove(intpoints[i]);

                            if (plinepoints2.Contains(intpoints[i]))
                                plinepoints2.Remove(intpoints[i]);
                            //if (!temp2.Contains(intpoints[i]))
                            //{
                            //    if (!plinepoints2.Contains(intpoints[i]))
                            //        plinepoints2.Insert
                            //            (startindex2, intpoints[i]);
                            //}
                            //if (!plinepoints1.Contains(intpoints[i]))
                            //   plinepoints1.Insert
                            //   (startindex1, intpoints[i]);
                        }



                    }
        
                    PGA.SimplifyPolylines.Commands.CreateSimplifiedPolylines
                        (region1.Layer, 0, plinepoints2);
                }
            }
        }


        public static void MatchOverlappingRegionsByTracing(ACADDB.Polyline region1, ACADDB.Polyline region2)
        {
            var outPoints = new Point2dCollection();
            var outterRegion = AddRegion(region1.ObjectId);
            var innerRegion = AddRegion(region2.ObjectId);
            var polyline = region2;
            ACADDB.Region intRegion;
            var MasterCount = 0;
            Point2dCollection polylinepoints = null;

            if (polyline != null)
            {
                polylinepoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(polyline);
            }
            if (outterRegion.Area > innerRegion.Area && outterRegion.Area > 0 && innerRegion.Area > 0)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intRegion = outterRegion;

                if (intRegion.Area > 0)
                {
                    ACADDB.DBObjectCollection objs =
                        PolylineFromRegion(outterRegion);

                    foreach (ACADDB.Entity ent in objs)
                    {
                        ACADDB.Extents3d extents = ent.Bounds.Value;
                        var upper = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(extents.MaxPoint);
                        var lower = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(extents.MinPoint);
                        var polyDirIncreasing = false;
                        var poly = ent as ACADDB.Polyline;
                        Point2dCollection tempcol = null;
                        if (poly != null && polylinepoints != null)
                        {
                            tempcol = new Point2dCollection();
                            var intpoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(poly);
                            polyDirIncreasing = IsYCoordinateIncreasing(intpoints[0], intpoints[1]);
                            //search
                            for (int i = 0; i < intpoints.Count; i++)
                            {
                                var result = polylinepoints.IndexOf(intpoints[i]);
                                if (result == -1)
                                {
                                    tempcol.Add(intpoints[i]);
                                }
                            }

                            //replace


                            for (int j = 0; j < polylinepoints.Count - 1; j++)
                            {
                                //if (!outPoints.Contains(polylinepoints[j]))
                                //    outPoints.Add(polylinepoints[j]);


                                var firstpt = polylinepoints[j];
                                var secondpt = polylinepoints[j + 1];
                                var isXIncreasing = IsXCoordinateIncreasing(firstpt, secondpt);
                                var isYIncreasing = IsYCoordinateIncreasing(firstpt, secondpt);
                                var IsLowerExtents = CalcDistanceToExtents(firstpt, lower, upper);
                                var reversed = new Point2dCollection();

                                Point2d searchpt = new Point2d();

                                if (IsLowerExtents) searchpt = lower;
                                else searchpt = upper;
                                var passSearchPoint = IsGreaterThanPoint
                                    (firstpt, secondpt,
                                        isYIncreasing,
                                        isXIncreasing, searchpt);

                                if (!passSearchPoint)
                                {
                                    Debug.WriteLine("Entered GreaterThan");
                                    // if (firstpt.Y == searchpt.Y)
                                    if (intpoints.Contains(firstpt))

                                    {

                                        if (isYIncreasing != polyDirIncreasing)
                                        {
                                            reversed = ReversePolyline(intpoints);
                                            
                                        }

                                        if (reversed.Count == 0)
                                        {
                                            var lindex = polylinepoints.IndexOf(firstpt);
                                            for (int i=0 ;i<intpoints.Count;i++)
                                            {
                                                var count = intpoints.Count - 2;
                                                var max =
                                                    AcadUtilities.PolygonFunctions.DistanceBetweenPoints(
                                                        polylinepoints[lindex], intpoints[count]);
                                                if (AcadUtilities.PolygonFunctions.DistanceBetweenPoints(
                                                         polylinepoints[lindex+1], intpoints[i]) < max)
                                                    polylinepoints.RemoveAt(lindex + 1);
                                                lindex++;
                                                if (!polylinepoints.Contains(intpoints[i]))
                                                    polylinepoints.Insert(lindex, intpoints[i]);
                                            }
                                        }
                                        //we need direction
                                        //check if we have passed extents
                                        Debug.WriteLine("Entered Equal");
                                        //var DistRank = new List<PointIndex>();
                                        //var ordered = new List<Point2d>();
                                        ////DistRank = GetListofDistances(tempcol,lower,upper);
                                        //ordered = GetListofDistances(tempcol, firstpt, isYIncreasing);
                                        //var index = polylinepoints.IndexOf(firstpt);
                                        //var result = false;
                                        //foreach (Point2d item in ordered)
                                        //{
                                        //    if (WithinExtents(item, extents) && !polylinepoints.Contains(item))
                                        //    {
                                        //        if (isYIncreasing)
                                        //        {
                                        //            if (polylinepoints[index + 1].Y > item.Y)
                                        //                result = true;

                                        //        }
                                        //        else
                                        //        {
                                        //            if (polylinepoints[index + 1].Y < item.Y)
                                        //                result = true;
                                        //        }
                                        //        if (result)
                                        //        {
                                        //            polylinepoints.Insert(++index, item);
                                        //            Debug.WriteLine(String.Format("Added Point-{0}-{1},{2}",
                                        //                polylinepoints.Count,
                                        //                item.X, item.Y));
                                        //        }
                                        //    }

                                        //}
                                    }

                                }
                                else
                                {
                                    Debug.WriteLine("NOT Equal");
                                }

                            }
                            Debug.WriteLine("Now Greater Than Search Value");

                        }

                    }

                    PGA.SimplifyPolylines.Commands.CreateSimplifiedPolylines
                        (polyline.Layer, 0, polylinepoints);
                }
            }
        }


        private static Point2dCollection ReversePolyline(Point2dCollection poly)
        {
            var newlist = new Point2dCollection();
            var UPPER = poly.Count;

            for (int i = UPPER-1; i >= 0; i--)
            {
                newlist.Add(poly[i]);
            }

            return newlist;
        }

        private static bool WithinExtents(Point2d item,ACADDB.Extents3d ext)
        {
            var extLL = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(ext.MinPoint);
            var extUR = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(ext.MaxPoint);
            var extLR = new Point2d(extLL.Y, extUR.X);
            var extUL = new Point2d(extLL.X,extUR.Y);
            var list  = new List<Point2d>(4);
            list.Add(extLL);
            list.Add(extLR);
            list.Add(extUR);
            list.Add(extUL);
            return PGA.AcadUtilities.PolygonFunctions.IsPointInsidePolygon(item, list, true);
        }

        private static List<Point2d> GetListofDistances(Point2dCollection tempcol, Point2d firstpt,bool isYCoordIncreasing)
        {
            var DistRank = new List<Point2d>();
            var distL = 0.0;
            var distH = 0.0;
            //var UPPER = tempcol.Count;
            ////var rankings = new List<KeyValuePair<int, double>>();

         
            //for (int i = 0; i < UPPER; i++)
            //{
            //    distL = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(tempcol[i], lower);
            //    distH = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(tempcol[i], upper);
            //    if (distH > distL) rankings.Add(new KeyValuePair<int, double>(i, distL));
            //    else rankings.Add(new KeyValuePair<int, double>(i, distH));
            //}
            if (isYCoordIncreasing)
            {
                var sorted = from p in tempcol.ToArray()
                    orderby p.Y ascending 
                    select p;

                foreach (Point2d point in sorted)
                {
                    DistRank.Add(point);
                }
            }
            else
            {
                var sorted = from p in tempcol.ToArray()
                             orderby p.Y descending 
                             select p;

                foreach (Point2d point in sorted)
                {
                    DistRank.Add(point);
                }
            }

            return DistRank;
        }

        private static List<PointIndex> GetListofDistances(Point2dCollection tempcol, Point2d lower, Point2d upper)
        {
            var DistRank = new List<PointIndex>();
            var distL = 0.0;
            var distH = 0.0;
            var UPPER = tempcol.Count;
            var rankings = new List<KeyValuePair<int, double>>();

            //var sorted = from p in rankings
            //             orderby p.Value ascending
            //             select p;

            for (int i = 0; i < UPPER; i++)
            {
                distL = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(tempcol[i], lower);
                distH = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(tempcol[i], upper);
                if (distH > distL) rankings.Add(new KeyValuePair<int, double>(i, distL));
                else rankings.Add(new KeyValuePair<int, double>(i, distH));
            }

            //list of ascending distances 
            var sorted = from p in rankings
                orderby p.Value ascending
                select p;


            foreach (KeyValuePair<int, double> point in sorted)
            {
                DistRank.Add(new PointIndex(point.Key, tempcol[point.Key]));
            }

            return DistRank;
        }

        private static bool IsGreaterThanPoint(Point2d firstpt, Point2d secondpt, bool Yincrease, bool Xincrease, Point2d searchpt)
        {
            var distL = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(firstpt, searchpt);
            var distH = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(secondpt, searchpt);
            if (distH > distL) return false;
            return true;
        }

        private static bool CalcDistanceToExtents(Point2d firstpt, Point2d extLow, Point2d extHigh)
        {
            var distL = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(firstpt, extLow);
            var distH = PGA.AcadUtilities.PolygonFunctions.DistanceBetweenPoints(firstpt, extHigh);
            if (distH > distL) return true;
            return false;

        }

        public static void MatchOverlappingRegionsByPoints(ACADDB.Polyline region1, ACADDB.Polyline region2)
        {
            var outterRegion = AddRegion(region1.ObjectId);
            var innerRegion = AddRegion(region2.ObjectId);
            var polyline = region2;
            ACADDB.Region intRegion;

            Point2dCollection polylinepoints = null;

            if (polyline != null)
            {
                polylinepoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(polyline);
            }
            if (outterRegion.Area > innerRegion.Area && outterRegion.Area > 0 && innerRegion.Area > 0)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intRegion = outterRegion;

                if (outterRegion != null || innerRegion != null)
                {
                    ACADDB.DBObjectCollection objs =
                        PolylineFromRegion(outterRegion);

                    foreach (ACADDB.Entity ent in objs)
                    {
                        ACADDB.Extents3d extents = ent.Bounds.Value;
                        var upper = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(extents.MaxPoint);
                        var lower = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(extents.MinPoint);
                        var poly = ent as ACADDB.Polyline;
                        Point2dCollection tempcol = null;
                        if (poly != null && polylinepoints != null)
                        {
                            tempcol = new Point2dCollection();
                            var intpoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(poly);
                            
                            var extpoint  = new Point2dCollection();

                            for (int i = 0; i < intpoints.Count; i++)
                            {
                                var result = polylinepoints.IndexOf(intpoints[i]);
                                if (result == -1)
                                {
                                    if (!tempcol.Contains(intpoints[i]))
                                        tempcol.Add(intpoints[i]);
                                }
                                else
                                {
                                    if (!extpoint.Contains(intpoints[i]))
                                    extpoint.Add(intpoints[i]);
                                }
                            }


                            for (int i = 0; i < extpoint.Count; i++)
                            {
                                var intpoint = PGA.SimplifyPolylines.Commands.NearestNeighbor(extpoint[i], tempcol);
                                var index = polylinepoints.IndexOf(extpoint[i]);
                                Point2d[] sortedarray = new Point2d[tempcol.Count];
                                var IsYincreasing = false;
                                foreach (var item in tempcol)
                                {
                                    //determine if Y is decreasing and sort tempcol
                                    if (IsYCoordinateIncreasing(polylinepoints[index - 1], polylinepoints[index]))
                                        IsYincreasing = true;

                                    if (IsYincreasing)
                                    {
                                        var sorted = from p in tempcol.ToArray()
                                                     orderby p.Y ascending
                                                     select p;
                                        sortedarray = sorted.ToArray();
                                    }
                                    else
                                    {
                                        var sorted = from p in tempcol.ToArray()
                                                     orderby p.Y descending
                                                     select p;

                                        sortedarray = sorted.ToArray();
                                    }

                                   
                                    //Also value must be greater/lesser than prev.
                                    if (!polylinepoints.Contains(sortedarray[i]))
                                    polylinepoints.Insert(++index, sortedarray[i]);
                                }

                                break;
                            }
                        }
                    }

                    PGA.SimplifyPolylines.Commands.CreateSimplifiedPolylines
                        (polyline.Layer, 0, polylinepoints);

                }
            }

        }

        private static double AverageCoordinate(Point2dCollection tempcol)
        {
            var totX = 0.0;
            var avgX = 0.0;
            var count = 0;
            foreach (var item in tempcol)
            {
                totX += item.X;
                count++;
            }
            avgX = totX/count;
            return avgX;
        }

        public  struct PointIndex
        {

            public PointIndex(int index,Point2d point)
            {
                Index = index;
                Point = point;
            }

            public  int Index { get; set; }
            public  Point2d Point { get; set; }
        }

        private static IList<PointIndex> GetIndicesBelow(int index, Point2dCollection points)
        {
            IList<PointIndex> list = new List<PointIndex>();
            if (index - 3 >= 0 )
            {
                {
                    list.Add(new PointIndex(index - 1, points[index - 1]));
                    list.Add(new PointIndex(index - 2, points[index - 2]));
                    list.Add(new PointIndex(index - 3, points[index - 3]));
                }
            }
            else if (index == 0)
            {
                 list.Add(new PointIndex(0, points[0]));
            }
            return list;
        }

        private static IList<PointIndex> GetIndicesAbove(int index, Point2dCollection points)
        {
            IList<PointIndex> list = new List<PointIndex>();
            if (index + 3 < points.Count)
            {
                {
                    list.Add(new PointIndex(index + 1, points[index + 1]));
                    list.Add(new PointIndex(index + 2, points[index + 2]));
                    list.Add(new PointIndex(index + 3, points[index + 3]));
                }
            }
            else if (index == points.Count)
            {
                list.Add(new PointIndex(points.Count, points[points.Count]));
            }
            return list;
        }
        private static Point2d? GetNearestNode(Point2dCollection testvals, Point2d testpt, Point2dCollection polylinepoints)
        {
            ////returns a starting point for comparison with master point list
            //var test  = PGA.SimplifyPolylines.Commands.NearestNeighbor(testpt, polylinepoints);
            
            ////returns the index to the location of the point
            //var index = polylinepoints.IndexOf(test);

            ////Find the point trend of the near points by index
            //var upper3 = GetIndicesAbove(index, polylinepoints);
            //var lower3 = GetIndicesBelow(index, polylinepoints);
            //if (FindInsertionPoint(upper3, lower3,testpt) != -1);

           

            //var sorted = SortMyValues(index, testvals, polylinepoints);
            //for (int j = 0; j < sorted.Count; j++)
            //{
            //    polylinepoints.Insert(index, sorted[j]);
            //    index++;
            //}


            return null;
        }
        private static bool IsXCoordinateIncreasing(Point2d first, Point2d second)
        {
            var diffy = (second.Y - first.Y);
            var diffx = (second.X - first.X);
            if (diffx < 0) return false;
            return true;
        }

        private static bool IsYCoordinateIncreasing(Point2d first, Point2d second)
        {
            var diffy = (second.Y - first.Y);
            var diffx = (second.X - first.X);
            if (diffy < 0) return false;
            return true;
        }

        private static int FindInsertionPoint(IList<PointIndex> upper3, IList<PointIndex> lower3, Point2d intpnts)
        {
            var master    = new List<PointIndex>();
 
            foreach (PointIndex items in upper3)
                if(items.Index != -1)  master.Add(items);
            foreach (PointIndex items in lower3)
                if (items.Index != -1) master.Add(items);

            master.Add(new PointIndex(0,intpnts));

            var sort = (from p in master.ToArray()
                        orderby p.Point.Y descending
                        select p);         

            Point2d? previouspoint = null;
            var sorted = sort.ToArray();
            for (int i = 0; i < sorted.Count(); i++)
            {
                if (sorted[i].Point == intpnts)
                    break;
                if (sorted[i].Point != intpnts)
                    previouspoint = sorted[i].Point;
            }

            //retreive the previous point location
            var matchpoint = (from p in master.ToArray()
                              where (p.Point.Y == previouspoint.Value.Y &&
                                     p.Point.X == previouspoint.Value.X)
                              select p.Index);

            return Convert.ToInt32(matchpoint.FirstOrDefault());

        }

        private static Point2dCollection SortMyValues(int index, Point2dCollection values, Point2dCollection masterList)
        {
            var sorted = new Point2dCollection();
            var nextpoint = masterList[index - 1];
            var currentpoint = masterList[index];

            var mdiffy = (nextpoint.Y - currentpoint.Y);
            var mdiffx = (nextpoint.X - currentpoint.X);

            for (int i = 0; i < 1; i++)
            {
                if (values.Count < 2)
                {
                    sorted.Add(values[i]);
                    return sorted;
                }
                var _nextpoint = values[i + 1];
                var _currentpoint = values[i];

                {
                    var diffy = ( _nextpoint.Y-  _currentpoint.Y);
                    var diffx = ( _nextpoint.X-  _currentpoint.X);
                    if (mdiffy < 0 && diffy < 0)
                    {
                        var sort = (from p in values.ToArray()
                            orderby p.Y descending
                            select p);

                        foreach (var item in sort)
                        {
                            sorted.Add(item);
                        }
                    }
                    else
                    {
                        var ascsort = (from p in values.ToArray()
                            orderby p.Y ascending
                            select p);


                        foreach (var item in ascsort)
                        {
                            sorted.Add(item);
                        }
                    }
                }

            }
            return sorted;
        }

        public static void MatchOverlappingRegions(ACADDB.Polyline region1, ACADDB.Polyline region2)
        {
            var outterRegion = AddRegion(region1.ObjectId);
            var innerRegion = AddRegion(region2.ObjectId);
            var polyline = region2;
            var polyline1 = region1;
            Point2dCollection polylinepoints = null;
            ACADDB.Region intersectRegion = null;
            ACADDB.Region MasterintRegion = null;
            if (polyline != null)
            {
                polylinepoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(polyline);
            }


            outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
            intersectRegion = outterRegion;
            MasterintRegion = intersectRegion;
            innerRegion = AddRegion(region2.ObjectId);

            if (intersectRegion.Area > 0)
            {
                outterRegion = AddRegion(region1.ObjectId);
                innerRegion = AddRegion(region2.ObjectId);
            }
            if (intersectRegion.Area > 0)
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intersectRegion = outterRegion;

                if (intersectRegion.Area > 0)
                {
                    outterRegion = AddRegion(region1.ObjectId);
                    innerRegion = AddRegion(region2.ObjectId);
                }
            }

            if (intersectRegion.Area > 0)
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intersectRegion = outterRegion;
                if (intersectRegion.Area > 0)
                {
                    outterRegion = AddRegion(region1.ObjectId);
                    innerRegion = AddRegion(region2.ObjectId);
                }
            }

            if (intersectRegion.Area > 0)
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intersectRegion = outterRegion;
                if (intersectRegion.Area > 0)
                {
                    outterRegion = AddRegion(region1.ObjectId);
                    innerRegion = AddRegion(region2.ObjectId);
                }
            }

            if (intersectRegion.Area > 0)
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intersectRegion = outterRegion;

                if (intersectRegion.Area > 0)
                {
                    outterRegion = AddRegion(region1.ObjectId);
                    innerRegion = AddRegion(region2.ObjectId);
                }
            }

            if (intersectRegion.Area > 0)
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, MasterintRegion);
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, innerRegion);
                intersectRegion = outterRegion;

                if (intersectRegion.Area > 0)
                {
                    outterRegion = AddRegion(region1.ObjectId);
                    innerRegion = AddRegion(region2.ObjectId);
                }
            }
            RegionToPolyline(outterRegion);
            RegionToPolyline(innerRegion);



        }

        private static Point2d GetNearestNeighbor(Point2dCollection tempcol)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Get the centroid of a Region.
        ///</summary>
        ///<param name="cur">An optional curve used to define the region.</param>
        ///<returns>A nullable Point3d containing the centroid of the Region.</returns>

        public static Point3d? GetCentroid(this ACADDB.Region reg, ACADDB.Curve cur = null)
        {
            if (cur == null)
            {
                var idc = new ACADDB.DBObjectCollection();
                reg.Explode(idc);
                if (idc.Count == 0)
                    return null;

                cur = idc[0] as ACADDB.Curve;
            }

            if (cur == null)
                return null;

            var cs = cur.GetPlane().GetCoordinateSystem();
            var o = cs.Origin;
            var x = cs.Xaxis;
            var y = cs.Yaxis;

            var a = reg.AreaProperties(ref o, ref x, ref y);
            var pl = new Plane(o, x, y);
            return pl.EvaluatePoint(a.Centroid);
        }

        public static void CentroidOfRegion()
        {
            var doc = ACAD.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;

            var peo = new PromptEntityOptions("\nSelect a region");
            peo.SetRejectMessage("\nMust be a region.");
            peo.AddAllowedClass(typeof(ACADDB.Region), false);
            var per = ed.GetEntity(peo);
            if (per.Status != PromptStatus.OK)
                return;

            using (var tr = doc.TransactionManager.StartTransaction())
            {
                var reg = tr.GetObject(per.ObjectId, ACADDB.OpenMode.ForRead) as ACADDB.Region;
                if (reg != null)
                {
                    var pt = reg.GetCentroid();
                    ed.WriteMessage("\nCentroid is {0}", pt);
                }
                tr.Commit();
            }
        }

        static public void RegionToPolylineTest()
        {
            ACAD.Document doc =
              ACAD.Application.DocumentManager.MdiActiveDocument;
            ACADDB.Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityOptions peo =
              new PromptEntityOptions("\nSelect a region:");
            peo.SetRejectMessage("\nMust be a region.");
            peo.AddAllowedClass(typeof(ACADDB.Polyline), true);

            PromptEntityResult per =
              ed.GetEntity(peo);


            PromptEntityOptions peo1 =
           new PromptEntityOptions("\nSelect a region:");
            peo1.SetRejectMessage("\nMust be a region.");
            peo1.AddAllowedClass(typeof(ACADDB.Polyline), true);

            PromptEntityResult per1 =
              ed.GetEntity(peo1);

            if (per.Status != PromptStatus.OK)
                return;

            ACADDB.Transaction tr =
              doc.TransactionManager.StartTransaction();
            using (tr)
            {
                ACADDB.BlockTable bt =
                  (ACADDB.BlockTable)tr.GetObject(
                    db.BlockTableId,
                    ACADDB.OpenMode.ForRead);
                ACADDB.BlockTableRecord btr =
                  (ACADDB.BlockTableRecord)tr.GetObject(
                    bt[ACADDB.BlockTableRecord.ModelSpace],
                    ACADDB.OpenMode.ForRead);

                ACADDB.Polyline reg =
                  tr.GetObject(
                    per.ObjectId,
                    ACADDB.OpenMode.ForRead) as ACADDB.Polyline;

                ACADDB.Polyline reg1 =
                    tr.GetObject(
                        per1.ObjectId,
                        ACADDB.OpenMode.ForRead) as ACADDB.Polyline;

                if (reg != null)
                {
                    MatchOverlappingRegionsByIntersect(reg,reg1);
                }
                tr.Commit();
            }
        }

        static public void RegionToPolyline(ACADDB.Region reg)
        {
            ACAD.Document doc =
              ACAD.Application.DocumentManager.MdiActiveDocument;
            ACADDB.Database db = doc.Database;
            Editor ed = doc.Editor;

          

            ACADDB.Transaction tr =
              doc.TransactionManager.StartTransaction();
            using (tr)
            {
                ACADDB.BlockTable bt =
                  (ACADDB.BlockTable)tr.GetObject(
                    db.BlockTableId,
                    ACADDB.OpenMode.ForRead);
                ACADDB.BlockTableRecord btr =
                  (ACADDB.BlockTableRecord)tr.GetObject(
                    bt[ACADDB.BlockTableRecord.ModelSpace],
                    ACADDB.OpenMode.ForRead);



                if (reg != null)
                {
                    ACADDB.DBObjectCollection objs =
                      PolylineFromRegion(reg);

                    // Append our new entities to the database

                    btr.UpgradeOpen();

                    foreach (ACADDB.Entity ent in objs)
                    {
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                    }

                    // Finally we erase the original region

                    reg.UpgradeOpen();
                    reg.Erase();
                }
                tr.Commit();
            }
        }
        static public void RegionToPolyline()
            {
                ACAD.Document doc =
                  ACAD.Application.DocumentManager.MdiActiveDocument;
                ACADDB.Database db = doc.Database;
                Editor ed = doc.Editor;

                PromptEntityOptions peo =
                  new PromptEntityOptions("\nSelect a region:");
                peo.SetRejectMessage("\nMust be a region.");
                peo.AddAllowedClass(typeof(ACADDB.Region), true);

                PromptEntityResult per =
                  ed.GetEntity(peo);

                if (per.Status != PromptStatus.OK)
                    return;

                ACADDB.Transaction tr =
                  doc.TransactionManager.StartTransaction();
                using (tr)
                {
                    ACADDB.BlockTable bt =
                      (ACADDB.BlockTable)tr.GetObject(
                        db.BlockTableId,
                        ACADDB.OpenMode.ForRead);
                    ACADDB.BlockTableRecord btr =
                      (ACADDB.BlockTableRecord)tr.GetObject(
                        bt[ACADDB.BlockTableRecord.ModelSpace],
                        ACADDB.OpenMode.ForRead);

                    ACADDB.Region reg =
                      tr.GetObject(
                        per.ObjectId,
                        ACADDB.OpenMode.ForRead) as ACADDB.Region;


                    if (reg != null)
                    {
                        ACADDB.DBObjectCollection objs =
                          PolylineFromRegion(reg);

                        // Append our new entities to the database

                        btr.UpgradeOpen();

                        foreach (ACADDB.Entity ent in objs)
                        {
                            btr.AppendEntity(ent);
                            tr.AddNewlyCreatedDBObject(ent, true);
                        }

                        // Finally we erase the original region

                        reg.UpgradeOpen();
                        reg.Erase();
                    }
                    tr.Commit();
                }
            }

        private static ACADDB.DBObjectCollection PolylineFromRegion(
              ACADDB.Region reg
            )
            {
                // We will return a collection of entities
                // (should include closed Polylines and other
                // closed curves, such as Circles)

                ACADDB.DBObjectCollection res =
                  new ACADDB.DBObjectCollection();

                // Explode Region -> collection of Curves / Regions

                ACADDB.DBObjectCollection cvs =
                  new ACADDB.DBObjectCollection();
                reg.Explode(cvs);

                // Create a plane to convert 3D coords
                // into Region coord system

                Plane pl =
                  new Plane(new Point3d(0, 0, 0), reg.Normal);

                using (pl)
                {
                    bool finished = false;

                    while (!finished && cvs.Count > 0)
                    {
                        // Count the Curves and the non-Curves, and find
                        // the index of the first Curve in the collection

                        int cvCnt = 0, nonCvCnt = 0, fstCvIdx = -1;

                        for (int i = 0; i < cvs.Count; i++)
                        {
                            ACADDB.Curve tmpCv = cvs[i] as ACADDB.Curve;
                            if (tmpCv == null)
                                nonCvCnt++;
                            else
                            {
                                // Closed curves can go straight into the
                                // results collection, and aren't added
                                // to the Curve count

                                if (tmpCv.Closed)
                                {
                                    res.Add(tmpCv);
                                    cvs.Remove(tmpCv);
                                    // Decrement, so we don't miss an item
                                    i--;
                                }
                                else
                                {
                                    cvCnt++;
                                    if (fstCvIdx == -1)
                                        fstCvIdx = i;
                                }
                            }
                        }

                        if (fstCvIdx >= 0)
                        {
                        // For the initial segment take the first
                        // Curve in the collection

                        ACADDB.Curve fstCv = (ACADDB.Curve)cvs[fstCvIdx];

                            // The resulting Polyline

                            ACADDB.Polyline p = new ACADDB.Polyline();

                            // Set common entity properties from the Region

                            p.SetPropertiesFrom(reg);

                            // Add the first two vertices, but only set the
                            // bulge on the first (the second will be set
                            // retroactively from the second segment)

                            // We also assume the first segment is counter-
                            // clockwise (the default for arcs), as we're
                            // not swapping the order of the vertices to
                            // make them fit the Polyline's order

                            p.AddVertexAt(
                              p.NumberOfVertices,
                              fstCv.StartPoint.Convert2d(pl),
                              BulgeFromCurve(fstCv, false), 0, 0
                            );

                            p.AddVertexAt(
                              p.NumberOfVertices,
                              fstCv.EndPoint.Convert2d(pl),
                              0, 0, 0
                            );

                            cvs.Remove(fstCv);

                            // The next point to look for

                            Point3d nextPt = fstCv.EndPoint;

                            // We no longer need the curve

                            fstCv.Dispose();

                            // Find the line that is connected to
                            // the next point

                            // If for some reason the lines returned were not
                            // connected, we could loop endlessly.
                            // So we store the previous curve count and assume
                            // that if this count has not been decreased by
                            // looping completely through the segments once,
                            // then we should not continue to loop.
                            // Hopefully this will never happen, as the curves
                            // should form a closed loop, but anyway...

                            // Set the previous count as artificially high,
                            // so that we loop once, at least.

                            int prevCnt = cvs.Count + 1;
                            while (cvs.Count > nonCvCnt && cvs.Count < prevCnt)
                            {
                                prevCnt = cvs.Count;
                                foreach (ACADDB.DBObject obj in cvs)
                                {
                                ACADDB.Curve cv = obj as ACADDB.Curve;

                                    if (cv != null)
                                    {
                                        // If one end of the curve connects with the
                                        // point we're looking for...

                                        if (cv.StartPoint == nextPt ||
                                            cv.EndPoint == nextPt)
                                        {
                                            // Calculate the bulge for the curve and
                                            // set it on the previous vertex

                                            double bulge =
                                              BulgeFromCurve(cv, cv.EndPoint == nextPt);
                                            if (bulge != 0.0)
                                                p.SetBulgeAt(p.NumberOfVertices - 1, bulge);

                                            // Reverse the points, if needed

                                            if (cv.StartPoint == nextPt)
                                                nextPt = cv.EndPoint;
                                            else
                                                // cv.EndPoint == nextPt
                                                nextPt = cv.StartPoint;

                                            // Add out new vertex (bulge will be set next
                                            // time through, as needed)

                                            p.AddVertexAt(
                                              p.NumberOfVertices,
                                              nextPt.Convert2d(pl),
                                              0, 0, 0
                                            );

                                            // Remove our curve from the list, which
                                            // decrements the count, of course

                                            cvs.Remove(cv);
                                            cv.Dispose();

                                            break;
                                        }
                                    }
                                }
                            }

                            // Once we have added all the Polyline's vertices,
                            // transform it to the original region's plane

                            p.TransformBy(Matrix3d.PlaneToWorld(pl));
                            res.Add(p);

                            if (cvs.Count == nonCvCnt)
                                finished = true;
                        }

                        // If there are any Regions in the collection,
                        // recurse to explode and add their geometry

                        if (nonCvCnt > 0 && cvs.Count > 0)
                        {
                            foreach (ACADDB.DBObject obj in cvs)
                            {
                                ACADDB.Region subReg = obj as ACADDB.Region;
                                if (subReg != null)
                                {
                                    ACADDB.DBObjectCollection subRes =
                                      PolylineFromRegion(subReg);

                                    foreach (ACADDB.DBObject o in subRes)
                                        res.Add(o);

                                    cvs.Remove(subReg);
                                    subReg.Dispose();
                                }
                            }
                        }
                        if (cvs.Count == 0)
                            finished = true;
                    }
                }
                return res;
            }

            // Helper function to calculate the bulge for arcs
            private static double BulgeFromCurve(
              ACADDB.Curve cv,
              bool clockwise
            )
            {
                double bulge = 0.0;

                ACADDB.Arc a = cv as ACADDB.Arc;
                if (a != null)
                {
                    double newStart;

                    // The start angle is usually greater than the end,
                    // as arcs are all counter-clockwise.
                    // (If it isn't it's because the arc crosses the
                    // 0-degree line, and we can subtract 2PI from the
                    // start angle.)

                    if (a.StartAngle > a.EndAngle)
                        newStart = a.StartAngle - 8 * Math.Atan(1);
                    else
                        newStart = a.StartAngle;

                    // Bulge is defined as the tan of
                    // one fourth of the included angle

                    bulge = Math.Tan((a.EndAngle - newStart) / 4);

                    // If the curve is clockwise, we negate the bulge

                    if (clockwise)
                        bulge = -bulge;
                }
                return bulge;
            }


        public static void ReversePolylineDirection(ACADDB.Polyline poly)
        {
            try
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                var db = doc.Database;
                using (ACADDB.Transaction trans = db.TransactionManager.StartTransaction())
                {
                    using (doc.LockDocument())
                    {
                        poly.UpgradeOpen();
                        poly.ReverseCurve();
                        poly.DowngradeOpen();
                    }
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }


        public static double? plIntersect(ACADDB.ObjectId oid0, ACADDB.ObjectId oid1)
        {
            ACAD.Document doc  = ACAD.Application.DocumentManager.MdiActiveDocument;
            ACADDB.Database db = doc.Database;

            double? area = null;
            using (ACADDB.Transaction tr = db.TransactionManager.StartTransaction())
            using (ACADDB.DBObjectCollection csc0 = new ACADDB.DBObjectCollection())
            using (ACADDB.DBObjectCollection csc1 = new ACADDB.DBObjectCollection())
            {
                ACADDB.Polyline pl0 = (ACADDB.Polyline)tr.GetObject(oid0, ACADDB.OpenMode.ForRead);
                ACADDB.Polyline pl1 = (ACADDB.Polyline)tr.GetObject(oid1, ACADDB.OpenMode.ForRead);
                try
                {
                    pl0.Explode(csc0);
                    pl1.Explode(csc1);
                    using (ACADDB.DBObjectCollection rec0 = ACADDB.Region.CreateFromCurves(csc0))
                    using (ACADDB.DBObjectCollection rec1 = ACADDB.Region.CreateFromCurves(csc1))
                    {
                        if (rec0.Count == 1 && rec1.Count == 1)
                        {
                            ACADDB.Region re0 = (ACADDB.Region)rec0[0];
                            re0.BooleanOperation(ACADDB.BooleanOperationType.BoolIntersect, (ACADDB.Region)rec1[0]);
                            area = new double?(re0.Area);
                        }
                    }
                }
                catch
                {
                }
                tr.Commit();
            }
            return area;
        }

    }
}
