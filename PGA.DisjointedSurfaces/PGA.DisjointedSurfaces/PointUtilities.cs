// ***********************************************************************
// Assembly         : PGA.DisjointedSurfaces
// Author           : Daryl Banks, PSM
// Created          : 01-13-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 01-13-2016
// ***********************************************************************
// <copyright file="PointUtilities.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace PGA.DisjointedSurfaces
{
    /// <summary>
    /// Class PointUtilities.
    /// </summary>
    class PointUtilities
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
        /// <summary>
        /// Pnpolies the specified points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="test">The test.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool PointinPolyline(List<Point3d> points, Point3d test)
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
        /// <summary>
        /// Points the in polyline.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="tests">The tests.</param>
        /// <param name="test">The test.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Points the in polyline.
        /// </summary>
        /// <param name="boundary points">The points.</param>
        /// <param name="test points">The tests.</param>
        /// <returns><c>true</c> if inside, <c>false</c> otherwise.</returns>
        public static bool PointInPolyline(Point2dCollection points, Point2dCollection tests)
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

                    if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                        (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                        c = !c;
                }

                if (c) return c; //break if point is inside poly
            }


            return c;
        }

        /// <summary>
        /// Points the in polyline.
        /// </summary>
        /// <param name="boundary points">The points.</param>
        /// <param name="test points">The tests.</param>
        /// <returns><c>true</c> if inside, <c>false</c> otherwise.</returns>
        public static bool PointInPolyline(Point2dCollection points, Point2d test)
        {
            int nvert = points.Count;
            int tvert = 1;

            int i, j, m, n;
            bool c = false;
            for (m = 0, j = tvert - 1; m < tvert; n = m++)
            {

                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {

                    if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                        (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                        c = !c;
                }

                if (c) return c; //break if point is inside poly
            }


            return c;
        }
    }
}
