using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace PGA.SimplifyPolylines
{
    public class DouglasPeuckerImplementation
    {
        public static int Counter = 0;
        //internal static void DouglasPeuckerReduction(Point2dCollection points, Point2d firstPoint, Point2d lastPoint,
        //    Double toleranceSquared, ref List<Point2d> pointIndexsToKeep)
        //{
        //    float maxDistanceSquared = 0, tmp = 0, areaSquared = 0;
        //    double X = 0;
        //    float Y = 0, bottomSquared = 0, distanceSquared = 0;
        //    int indexFarthest = 0;

        //    unsafe
        //    {
        //        foreach (var point in points)
        //            {
        //                //Perpendicular distance 
        //                tmp = 0.5f*
        //                      ((lastPoint.Y - i)*(firstPoint - i) +
        //                       (*(samples + lastPoint) - *(samples + i))*(*(samples + firstPoint) - *(samples + i)));
        //                //Abs
        //                areaSquared = tmp*tmp;
        //                X = (firstPoint.X - lastPoint.X);
        //                Y = (*(samples + firstPoint) - *(samples + lastPoint));
        //                bottomSquared = X*X + Y*Y;
        //                distanceSquared = areaSquared/bottomSquared;

        //                if (distanceSquared > maxDistanceSquared)
        //                {
        //                    maxDistanceSquared = distanceSquared;
        //                    indexFarthest = i;
        //                }
        //            }
        //    }

        //    if (maxDistanceSquared > toleranceSquared && indexFarthest != 0)
        //    {
        //        //Add the largest point that exceeds the tolerance
        //        DouglasPeuckerReduction(points, firstPoint, indexFarthest, toleranceSquared, ref pointIndexsToKeep);
        //        pointIndexsToKeep.Add(indexFarthest);
        //        DouglasPeuckerReduction(points, indexFarthest, lastPoint, toleranceSquared, ref pointIndexsToKeep);
        //    }
        //}


        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="Points">The points.</param>
        /// <param name="Tolerance">The tolerance.</param>
        /// <returns>Point2d[].</returns>
        public static Point2d[] DouglasPeuckerReduction
            (List<Point2d> Points, Double Tolerance)
        {
            if (Points == null || Points.Count < 3)
                return Points.ToArray();

            Point2d firstPoint = Points.First();
            Point2d lastPoint  = Points.Last();
            List<Point2d> pointIndexsToKeep = new List<Point2d>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            int i = 1;
            while (Equals(firstPoint, lastPoint))
            {
                lastPoint = Points[Points.Count - i++];
            }

                //The first and the last point cannot be the same
            if (!(Equals(firstPoint, lastPoint)))
            {
                DouglasPeuckerReduction(Points.ToArray(), firstPoint, lastPoint,
                    Tolerance, ref pointIndexsToKeep);

                Point2dCollection returnPoints = new Point2dCollection();
                //pointIndexsToKeep.Sort();
                foreach (var point in pointIndexsToKeep)
                {
                    returnPoints.Add(point);
                }

                return returnPoints.ToArray();
            }
            return Points.ToArray();
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point indexs to keep.</param>
        private static void DouglasPeuckerReduction(Point2d[]
            points, Point2d firstPoint, Point2d lastPoint, Double tolerance,
            ref List<Point2d> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Point2d point = new Point2d();
            int indexFarthest = 0;
            int first =  GetIndex(points,firstPoint);
            int last  = GetIndex(points, lastPoint);

            int UpperBound = points.Count() - 1;
            for (int index = first; index < last; index++)
                {
                    point = (Point2d) points.GetValue(index);
                    if (!(Equals(firstPoint, point)) || !(Equals(lastPoint, point) || !(Equals(lastPoint, firstPoint))))
                    {
                        Double distance = PerpendicularDistance
                            (firstPoint, lastPoint, point);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            indexFarthest = index;
                        }

                    }
                }
       
             
            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(points[indexFarthest]);

                DouglasPeuckerReduction(points, firstPoint, points[indexFarthest], tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, points[indexFarthest], lastPoint, tolerance, ref pointIndexsToKeep);
            }
       
        }

        public static int GetIndex(Point2d[] points, Point2d point)
        {
            int i = 0;
            foreach (var items in points)
            {
                if (Equals(items, point))
                    return i;

                i++;
            }
            return 0;
        }

        /// <summary>
        /// Perpendiculars the distance.
        /// </summary>
        /// <param name="Point1">The point1.</param>
        /// <param name="Point2">The point2.</param>
        /// <param name="Point">The point.</param>
        /// <returns>Double.</returns>
        public static Double PerpendicularDistance
            (Point2d Point1, Point2d Point2, Point2d Point)
        {
            Double area = 0, bottom=0, height=0;
            try
            {
                if ((Math.Abs(Point.X + Point.Y) < 0.0001) || (Math.Abs(Point1.X + Point1.Y) < 0.0001) || (Math.Abs(Point2.X + Point2.Y) < 0.0001))
                    return 0;
                //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
                //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
                //Area = .5*Base*H                                          *Solve for height
                //Height = Area/.5/Base

                    area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
                                           Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
                                           Point2.Y - Point1.X * Point.Y));

                if (area == 0) return 0.0;

                bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
                                          Math.Pow(Point1.Y - Point2.Y, 2));

                if (bottom == 0) return 0.0;

                height = area / bottom * 2;

                return height;


            }
            catch
            {
                Debug.WriteLine(string.Format("area={0}bottom={1}height={2}", area, bottom, height));
                height = 0.0;
            }

            return height;
        }


        public static bool Equals(Point2d first, Point2d second)
        {
            if ((Math.Abs(first.X - (second.X)) < .0001) && (Math.Abs(first.Y - (second.Y)) < .0001))
                return true;
            
            return false;
        }
    }
}