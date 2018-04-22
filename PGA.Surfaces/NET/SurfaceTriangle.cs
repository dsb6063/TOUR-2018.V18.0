using System;

using Autodesk.AutoCAD.Geometry;

namespace C3DSurfacesDemo
{
    public class SurfaceTriangle
    {
        public SurfaceTriangle(Point3d vx1, Point3d vx2, Point3d vx3)
        {
            Vertex1 = vx1;
            Vertex2 = vx2;
            Vertex3 = vx3;
        }

        public Point3d Vertex1 { get; set; }
        public Point3d Vertex2 { get; set; }
        public Point3d Vertex3 { get; set; }
    }
}