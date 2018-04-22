 
using System.Collections.Generic;
//using Teigha.Core;
//using Teigha.TD;
using System.Diagnostics;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
namespace C3DSurfacesDemo
{
    public class PnPoly
    {
        #region Teigha Implementation
        //public static bool pnpoly(List<OdGePoint3d> points, OdGePoint3d test)
        //{
        //    int nvert = points.Count;

        //    int i, j;
        //    bool c = false;

        //    for (i = 0, j = nvert - 1; i < nvert; j = i++)
        //    {

        //        if (((points[i].y > test.y) != (points[j].y > test.y)) &&
        //            (test.x < (points[j].x - points[i].x)*(test.y - points[i].y)/(points[j].y - points[i].y) + points[i].x))
        //            c = !c;
        //    }

        //    return c;
        //} 
        #endregion

        #region Autodesk Implementation
        public static bool pnpoly(List<Point3d> points, Point3d test)
        {
            int nvert = points.Count;

            int i, j;
            bool c = false;

            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {

                if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                    (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                    c = !c;
            }

            return c;
        }
        public static bool PointInPolyline(List<Point3d> points, List<Point3d> tests, Point3d test)
        {
            int nvert = points.Count;

            int i, j;
            bool c = false;

            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {

                if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                    (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                    c = !c;
            }

            return c;
        }
        #endregion

        internal static bool PointInPolyline(Point2dCollection points, Point2dCollection tests)
        {
            int nvert = points.Count;
            int tvert = tests.Count;
            Point2d test = new Point2d();

            int i, j, m, n;
            bool Inside = false;
            for (m = 0, j = tvert - 1; m < tvert; n = m++)
            {
                test = tests[m];

                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {

                    if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                        (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                    {
                        Inside = !Inside;
                    }
                }
                Debug.WriteLine(System.DateTime.Now + " Inside =" + Inside);
                if (Inside) return Inside;
            }


            return Inside;
        }

        internal static bool PointInPolyline(Point2dCollection points, Point2dCollection tests, double minpolyseparation)
        {
            int nvert = points.Count;
            int tvert = tests.Count;
            Point2d test = new Point2d();

            int i, j, m, n;
            bool c = false;
            for (m = 0, j = tvert - 1; m < tvert; n = m++)
            {
                test = tests[m];

                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {
                    if (!((System.Math.Abs(points[i].Y - test.Y) >= minpolyseparation) && (System.Math.Abs(points[j].Y - test.Y) >= minpolyseparation) &&
                        (System.Math.Abs(points[i].X - test.X) >= minpolyseparation) && (System.Math.Abs(points[j].X - test.X) >= minpolyseparation)))
                        return false; // line too close

                    if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                        (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                        c = !c;
                }

                if (c) return c;
            }


            return c;
        }
    }
}