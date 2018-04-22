using System;

using Autodesk.AutoCAD.Geometry;

namespace C3DSurfacesDemo
{
    public class SurfaceDataProvider 
    {
        static SurfaceDataProvider()
        {
            m_Generator = new Random();
        }

        public static Point3dCollection GenerateRandomPoints(int count, int xMax, int yMax, int zMax)
        {
            Point3dCollection points = new Point3dCollection();
            for (int i = 0; i < count; i++)
            {
                points.Add(GenerateRandomPoint(xMax, yMax, zMax));
            }
            return points;
        }

        public static Point3d GenerateRandomPoint(int xMax, int yMax, int zMax)
        {
            double x = m_Generator.NextDouble() * xMax;
            double y = m_Generator.NextDouble() * yMax;
            double z = m_Generator.NextDouble() * zMax;
            return new Point3d(x, y, z);
        }

        public static Point3dCollection GenerateBreaklinePoints(int count, int xMax, int yMax)
        {
            Point3dCollection points = new Point3dCollection();
            double xDelta = xMax / count;
            double yDelta = yMax / count;
            for (int i = 0; i < count; i++)
            {
                double x = (m_Generator.NextDouble() * xDelta) + (xDelta * i);
                double y = (m_Generator.NextDouble() * yDelta) + (yDelta * i);
                points.Add(new Point3d(x, y, 1));
            }
            return points;
        }

        private static Random m_Generator;
    }
}