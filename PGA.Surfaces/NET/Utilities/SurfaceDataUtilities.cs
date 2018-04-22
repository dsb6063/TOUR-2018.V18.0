using System;

using Autodesk.AutoCAD.Geometry;

namespace C3DSurfacesDemo
{
    internal class SurfaceDataUtilities 
    {
        public static SurfaceTriangle GetTriangleAtIndex(double[] data, int index)
        {
            Point3d vx1 = GetPointAtIndex(data, index);
            Point3d vx2 = GetPointAtIndex(data, index + 3);
            Point3d vx3 = GetPointAtIndex(data, index + 6);
            return new SurfaceTriangle(vx1, vx2, vx3);
        }

        public static Point3d GetPointAtIndex(double[] data, int index)
        {
            double x = data[index];
            double y = data[index + 1];
            double z = data[index + 2];
            return new Point3d(x, y, z);
        }

        public static double[] PointCollectionToDoubleArray(Point3dCollection points)
        {
            double[] pointsAsDoubles = new double[points.Count * POINT_OFFSET];
            int idx = 0;
            foreach (Point3d point in points)
            {
                pointsAsDoubles[idx] = point.X;
                pointsAsDoubles[idx + 1] = point.Y;
                pointsAsDoubles[idx + 2] = point.Z;
                idx += POINT_OFFSET;
            }
            return pointsAsDoubles;
        }

        public static readonly int POINT_OFFSET = 3;
        public static readonly int TRIANGLE_OFFSET = 9;
    }
}