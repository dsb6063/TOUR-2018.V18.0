using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace PGA.AcadUtilities
{
    public class PolygonFunctions
    {

        #region PointInPolygon

        /// <summary>
        /// Determines if a point is inside a polygon
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="polyPoints"></param>
        /// <param name="outsidePoint"></param>
        /// <param name="includeTouching"></param>
        /// <returns></returns>
        public static bool IsPointInsidePolygon(Point2d testPoint, IList<Point2d> polyPoints, bool includeTouching)
        {
            Point2d outsidePoint = new Point2d();
            GetOutsidePoint(ref outsidePoint, polyPoints);

            long pointCount = 0;
            long intersectionCount = 0;
            double howClose = 0;
            bool retval = false;

            Point2d onePoint = new Point2d();
            Point2d startPoint = new Point2d();
            Point2d endPoint = new Point2d();
            Point2d nearPoint = new Point2d();
            pointCount = polyPoints.Count;
            for (int i = 0; i < polyPoints.Count - 1; i++)
            {
                onePoint = polyPoints[i];
                startPoint = new Point2d(onePoint.X, onePoint.Y);

                onePoint = polyPoints[i + 1];
                endPoint = new Point2d(onePoint.X, onePoint.Y);

                if ((includeTouching == true))
                {
                    howClose = DistanceToSegment(testPoint, startPoint, endPoint, ref nearPoint);
                    if (((howClose >= 0) && (howClose <= 1E-08)))
                    {
                        retval = true;
                        break;
                    }
                }
                if ((retval == false))
                {
                    if ((DoSegmentsIntersect(startPoint, endPoint, outsidePoint, testPoint) == true))
                    {
                        // Check that testpoint is not on the boundary line
                        if ((includeTouching == true))
                        {
                            if ((DistanceToSegment(testPoint, startPoint, endPoint, ref nearPoint) > 0))
                            {
                                intersectionCount = (intersectionCount + 1);
                            }
                        }
                    }
                }
            }
            if ((retval == false))
            {
                if (((intersectionCount%2) == 1))
                {
                    retval = true;
                }
            }
            return retval;
        }

        /// <summary>
        /// Gets the minimum bounding point from a list of points
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <returns></returns>
        public static Point2d GetMinFromPolygon(IList<Point2d> polyPoints)
        {
            Point2d minPoint = new Point2d();
            Point2d firstPoint = polyPoints[0];
            minPoint = new Point2d(firstPoint.X,  firstPoint.Y);

            foreach (Point2d onePoint in polyPoints)
            {
                Double x=0;
                Double y=0;

                if (onePoint.X < minPoint.X)
                {
                    x = onePoint.X;
                }
                if (onePoint.Y < minPoint.Y)
                {
                    y = onePoint.Y;
                }
                minPoint = new Point2d(x,y);
            }
            return minPoint;
        }

        /// <summary>
        /// Gets a point outside of a polygon
        /// </summary>
        /// <param name="outsidePoint"></param>
        /// <param name="polyPoints"></param>
        /// <returns></returns>
        private static bool GetOutsidePoint(ref Point2d outsidePoint, IList<Point2d> polyPoints)
        {
            Point2d minPoint = GetMinFromPolygon(polyPoints);
            outsidePoint = new Point2d((minPoint.X - 10),(minPoint.Y - 10));
            return true;
        }

        /// <summary>
        /// Determines if two vectors intersect
        /// </summary>
        /// <param name="startPoint1"></param>
        /// <param name="endPoint1"></param>
        /// <param name="startPoint2"></param>
        /// <param name="endPoint2"></param>
        /// <returns></returns>
        private static bool DoSegmentsIntersect(Point2d startPoint1, Point2d endPoint1, Point2d startPoint2,
            Point2d endPoint2)
        {
            bool retval = false;
            double deltaX1 = (endPoint1.X - startPoint1.X);
            double deltaY1 = (endPoint1.Y - startPoint1.Y);
            double deltaX2 = (endPoint2.X - startPoint2.X);
            double deltaY2 = (endPoint2.Y - startPoint2.Y);
            if ((((deltaX2*deltaY1) - (deltaY2*deltaX1)) == 0))
            {
                //  The segments are parallel.
                return false;
            }

            double s = (((deltaX1*(startPoint2.Y - startPoint1.Y)) + (deltaY1*(startPoint1.X - startPoint2.X)))/
                        ((deltaX2*deltaY1) - (deltaY2*deltaX1)));
            double t = (((deltaX2*(startPoint1.Y - startPoint2.Y)) + (deltaY2*(startPoint2.X - startPoint1.X)))/
                        ((deltaY2*deltaX1) - (deltaX2*deltaY1)));

            if ((s >= 0.0) && (s <= 1.0) && (t >= 0.0) && (t <= 1.0))
            {
                retval = true;
            }
            return retval;
        }

        /// <summary>
        /// Calculates the distance between a point and a segment.
        /// Note that nearpoint is returned as well as distance
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="nearPoint"></param>
        /// <returns></returns>
        public static double DistanceToSegment(Point2d testPoint, Point2d startPoint, Point2d endPoint,
            ref Point2d nearPoint)
        {
            double t;
            double deltaX = (endPoint.X - startPoint.X);
            double deltaY = (endPoint.Y - startPoint.Y);
            if (((deltaX == 0) && (deltaY == 0)))
            {
                //  It's a point not a line segment.
                deltaX = (testPoint.X - startPoint.X);
                deltaY = (testPoint.Y - startPoint.Y);
                nearPoint = new Point2d(startPoint.X,startPoint.Y);
                return Math.Sqrt(((deltaX*deltaX) + (deltaY*deltaY)));
            }
            //  Calculate the t that minimizes the distance.
            t = ((((testPoint.X - startPoint.X)*deltaX) + ((testPoint.Y - startPoint.Y)*deltaY))/
                 ((deltaX*deltaX) + (deltaY*deltaY)));
            if ((t < 0))
            {
                deltaX = (testPoint.X - startPoint.X);
                deltaY = (testPoint.Y - startPoint.Y);
                nearPoint = new Point2d(startPoint.X, startPoint.Y);
            }
            else if ((t > 1))
            {
                deltaX = (testPoint.X - endPoint.X);
                deltaY = (testPoint.Y - endPoint.Y);
                nearPoint = new Point2d(endPoint.X, endPoint.Y);
            }
            else
            {
                nearPoint = new Point2d((startPoint.X + (t*deltaX)), (startPoint.Y + (t*deltaY)));
                deltaX = (testPoint.X - nearPoint.X);
                deltaY = (testPoint.Y - nearPoint.Y);
            }
            return Math.Sqrt(((deltaX*deltaX) + (deltaY*deltaY)));
        }

        #endregion

        public static double DistanceBetweenPoints(Point2d one, Point2d two)
        {
            return Math.Sqrt((two.X - one.X) * (two.X - one.X) + (two.Y - one.Y) * (two.Y - one.Y));
        }
    }
}

