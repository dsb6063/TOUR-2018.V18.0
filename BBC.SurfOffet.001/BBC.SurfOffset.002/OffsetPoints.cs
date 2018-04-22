using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using MathNet.Numerics.LinearAlgebra;
using Autodesk.Civil.DatabaseServices;
using MathNet.Numerics.LinearAlgebra.Double;
using Acad = Autodesk.AutoCAD.Geometry;


namespace BBC.SurfOffset002
{
    public class OffsetPoints
    {
        public static IEnumerable<Acad.Vector3d> Where(TinSurfaceEdgeCollection edges, Func <Autodesk.Civil.DatabaseServices.TinSurfaceEdgeCollection, Acad.Vector3d> action)
        {

            return null;
        }
        public static Point3dCollection GetOffsetTrianglesEdges(TinSurfaceTriangle tri, double offset)
        {
            var points = new Point3dCollection();

            try
            {
                Acad.Vector3d cenpointp3d = new Acad.Vector3d();
                Acad.Vector3d centerv1    = new Acad.Vector3d();
                Acad.Vector3d centerv2    = new Acad.Vector3d();
                Acad.Vector3d centerv3    = new Acad.Vector3d();


                var edge1 = tri.Edge1;
                var edge2 = tri.Edge2;
                var edge3 = tri.Edge3;

                //compute the contributions from the edges

                var tv111 = edge1.Triangle1.Vertex1.Location;
                var tv112 = edge1.Triangle1.Vertex2.Location;
                var tv113 = edge1.Triangle1.Vertex3.Location;

                var tv121 = edge1.Triangle2.Vertex1.Location;
                var tv122 = edge1.Triangle2.Vertex2.Location;
                var tv123 = edge1.Triangle2.Vertex3.Location;

                BBC.SkeletonSurf.SkeletonSurf.CreateLine(tv111, tv112);
                BBC.SkeletonSurf.SkeletonSurf.CreateLine(tv111, tv113);
                BBC.SkeletonSurf.SkeletonSurf.CreateLine(tv113, tv112);


                // vectors

                var v22 = tv111.GetVectorTo(tv113);
                var v23 = tv121.GetVectorTo(tv123);
                var n22 = v22.GetNormal(Tolerance.Global);
                var n23 = v23.GetNormal(Tolerance.Global);
                var n = n22 + n23;
                var angle = v22.GetAngleTo(v23) * 180 / Math.PI;
                var ap  = v22.AngleOnPlane(new Plane(tv121, v23.CrossProduct(v22)));
                var ap2 = v23.AngleOnPlane(new Plane(tv121, v23.CrossProduct(v22)));

                //edge1.Triangle1

                var v1 = tri.Vertex1.Location;
                var v2 = tri.Vertex2.Location;
                var v3 = tri.Vertex3.Location;
                
                

                var vec1 = v1.GetVectorTo(v2);
                var vec2 = v1.GetVectorTo(v3);

                var norm = vec1.CrossProduct(vec2);
                var e1 = UnitVector3D(vec1);
                var e2 = UnitVector3D(vec2);

                var M = new Acad.Vector3d(norm.X, norm.Y, norm.Z + offset);

                cenpointp3d = FindTriangleCenter(v1, v2, v3, vec1.CrossProduct(vec2));

                centerv1 = new Acad.Vector3d(cenpointp3d.X + v1.X, cenpointp3d.Y + v1.Y, cenpointp3d.Z + v1.Z) / 2.0;
                centerv2 = new Acad.Vector3d(cenpointp3d.X + v2.X, cenpointp3d.Y + v2.Y, cenpointp3d.Z + v2.Z) / 2.0;
                centerv3 = new Acad.Vector3d(cenpointp3d.X + v3.X, cenpointp3d.Y + v3.Y, cenpointp3d.Z + v3.Z) / 2.0;

                var offsetcenterv1 = (centerv1.Add(M));
                var offsetcenterv2 = (centerv2.Add(M));
                var offsetcenterv3 = (centerv3.Add(M));

                var p1 = new Acad.Point3d(offsetcenterv1.X, offsetcenterv1.Y, offsetcenterv1.Z);
                var p2 = new Acad.Point3d(offsetcenterv2.X, offsetcenterv2.Y, offsetcenterv2.Z);
                var p3 = new Acad.Point3d(offsetcenterv3.X, offsetcenterv3.Y, offsetcenterv3.Z);


                points.Add(p1);
                points.Add(p2);
                points.Add(p3);

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }

            return points;
        }

        public static IEnumerable<Vector<double>> OldGetOffsetPoints(TinSurfaceVertex vert, double offset)
        {
            var points = new List<Vector<double>>();

            try
            {
                var sum = new Summations();
                var sumv = new Summations();

                Vector<double> centerOfPlane= null;
                Vector<double> centerv1 = null;
                Vector<double> centerv2 = null;
                Vector<double> centerv3 = null;
                // central connection point

                var location = vert.Location;

                var cenpoint = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(new [] {location.X,location.Y,location.Z});

                // all the connecting Triangles

                var triangles = vert.Triangles;

                // all the connecting vertices

                var convertices = vert.Vertices;

                foreach (var tri in triangles)
                {
                    var v1 = tri.Vertex1.Location;
                    var v2 = tri.Vertex2.Location;
                    var v3 = tri.Vertex3.Location;

                    var p1 = Vector<double>.Build.DenseOfArray(new[] {v1.X, v1.Y, v1.Z});
                    var p2 = Vector<double>.Build.DenseOfArray(new[] {v2.X, v2.Y, v2.Z});
                    var p3 = Vector<double>.Build.DenseOfArray(new[] {v3.X, v3.Y, v3.Z});

                    centerOfPlane = FindTriangleCenter(p1, p2, p3, Cross(p1, p2));
                    centerv1 =   (cenpoint + p1) /2.0;
                    centerv2 =   (cenpoint + p2) /2.0;
                    centerv3 =   (cenpoint + p3) /2.0;

                    //Convert Formatting 

                    var NodeTriangles1 = new List<Triangle>();


                    foreach (var triVertex in triangles)
                    {
                        NodeTriangles1.Add(new Triangle(triVertex.Vertex1, triVertex.Vertex2, triVertex.Vertex3));
                    }


                    //Triangle Center Points

                    sum = IterateAdjacentTriangles(NodeTriangles1, cenpoint);

                    //Triangle shared at vertex 1, 2, and 3   

                    //sumv = IterateVertexTriangles(NodeTriangles1, p1);
                    //sumv = IterateVertexTriangles(NodeTriangles1, p2);
                    //sumv = IterateVertexTriangles(NodeTriangles1, p3);

                }

                var M = ((sum.Top)/Magnitude(sum.Bot))*offset;

                var offsetvecCenter = (centerOfPlane + M);
                var offsetcenterv1 = (centerv1 + M);
                var offsetcenterv2 = (centerv2 + M);
                var offsetcenterv3 = (centerv3 + M);
     
                points.Add(offsetvecCenter);
                points.Add(offsetcenterv1);
                points.Add(offsetcenterv2);
                points.Add(offsetcenterv3);


            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }

            return points;
        }
        public static List<Acad.Vector3d> GetOffsetPoints(TinSurfaceVertex vert, double offset)
        {
            var points = new List<Acad.Vector3d>();

            try
            {
                var sum  = new SummationsAcad();
                var sumv = new SummationsAcad();

                Acad.Vector3d centerOfPlane = new Acad.Vector3d();
                Acad.Vector3d centerv1 = new Acad.Vector3d();
                Acad.Vector3d centerv2 = new Acad.Vector3d();
                Acad.Vector3d centerv3 = new Acad.Vector3d();

                // central connection point

                var location = vert.Location;

                var cenpoint = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(new[] { location.X, location.Y, location.Z });
                var cenpointp3d = new Acad.Point3d(cenpoint[0],cenpoint[1],cenpoint[2]);

                // all the connecting Triangles

                var triangles = vert.Triangles;
             
                
                // all the connecting vertices's

                var connectedvertices  = vert.Vertices;
                var connectedTriangles = vert.Triangles;

                foreach (var tri in connectedTriangles)
                {
                    var v1 = tri.Vertex1.Location;
                    var v2 = tri.Vertex2.Location;
                    var v3 = tri.Vertex3.Location;


                    var vec1 = v1.GetVectorTo(v2);
                    var vec2 = v1.GetVectorTo(v3);

                    var norm = vec1.CrossProduct(vec2);
                    var e1 = UnitVector3D(vec1);
                    var e2 = UnitVector3D(vec2);

                    centerOfPlane = FindTriangleCenter(v1, v2, v3, vec1.CrossProduct(vec2));

                    //create 3 more points on the plane

                    centerv1 = new Acad.Vector3d(cenpointp3d.X + v1.X, cenpointp3d.Y + v1.Y, cenpointp3d.Z + v1.Z) / 2.0;                                    
                    centerv2 = new Acad.Vector3d(cenpointp3d.X + v2.X, cenpointp3d.Y + v2.Y, cenpointp3d.Z + v2.Z) / 2.0;
                    centerv3 = new Acad.Vector3d(cenpointp3d.X + v3.X, cenpointp3d.Y + v3.Y, cenpointp3d.Z + v3.Z) / 2.0;


                    sum = IterateAdjacentTrianglesAcad(connectedTriangles, cenpointp3d);

                }

                //Triangle Center Points

                var M = (new Acad.Vector3d(sum.Top.X, sum.Top.Y, sum.Top.Z) / (Magnitude(new Acad.Vector3d(sum.Bot.X, sum.Bot.Y, sum.Bot.Z)))) * offset;

                var offsetvecCenter = (centerOfPlane.Add(M));
                var offsetcenterv1 = (centerv1.Add(M));
                var offsetcenterv2 = (centerv2.Add(M));
                var offsetcenterv3 = (centerv3.Add(M));

                points.Add(offsetvecCenter);
                points.Add(offsetcenterv1);
                points.Add(offsetcenterv2);
                points.Add(offsetcenterv3);


            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }

            return points.ToList();
        }
        public static Point3dCollection GetOffsetTriangles(TinSurfaceTriangle tri, double offset)
        {
            var points = new Point3dCollection();

            try
            {
                Acad.Vector3d cenpointp3d = new Acad.Vector3d();
                Acad.Vector3d centerv1 = new Acad.Vector3d();
                Acad.Vector3d centerv2 = new Acad.Vector3d();
                Acad.Vector3d centerv3 = new Acad.Vector3d();

                var v1 = tri.Vertex1.Location;
                var v2 = tri.Vertex2.Location;
                var v3 = tri.Vertex3.Location;


                var vec1 = v1.GetVectorTo(v2);
                var vec2 = v1.GetVectorTo(v3);

                var norm = vec1.CrossProduct(vec2);
                var e1 = UnitVector3D(vec1);
                var e2 = UnitVector3D(vec2);


                var M = new Acad.Vector3d(norm.X, norm.Y, norm.Z + offset);

                cenpointp3d = FindTriangleCenter(v1, v2, v3, vec1.CrossProduct(vec2));

                centerv1 = new Acad.Vector3d(cenpointp3d.X + v1.X, cenpointp3d.Y + v1.Y, cenpointp3d.Z + v1.Z)/2.0;
                centerv2 = new Acad.Vector3d(cenpointp3d.X + v2.X, cenpointp3d.Y + v2.Y, cenpointp3d.Z + v2.Z)/2.0;
                centerv3 = new Acad.Vector3d(cenpointp3d.X + v3.X, cenpointp3d.Y + v3.Y, cenpointp3d.Z + v3.Z)/2.0;

                var offsetcenterv1 = (centerv1.Add(M));
                var offsetcenterv2 = (centerv2.Add(M));
                var offsetcenterv3 = (centerv3.Add(M));

                var p1 = new Acad.Point3d(offsetcenterv1.X, offsetcenterv1.Y, offsetcenterv1.Z);
                var p2 = new Acad.Point3d(offsetcenterv2.X, offsetcenterv2.Y, offsetcenterv2.Z);
                var p3 = new Acad.Point3d(offsetcenterv3.X, offsetcenterv3.Y, offsetcenterv3.Z);


                points.Add(p1);
                points.Add(p2);
                points.Add(p3);

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }

            return points;
        }

        public static IEnumerable<Vector<double>> GetOffsetPoints(List<TinSurfaceVertex> tinSurfaceVertices, double offset)
        {
            var points = new List<Vector<double>>();

            foreach (var vertices in tinSurfaceVertices)
            {

                try
                {

                    // central connection point

                    var location = vertices.Location;

                    // all the connecting Triangles

                    var triangles = vertices.Triangles;

                    // all the connecting vertices

                    var convertices = vertices.Vertices;

                    foreach (var tri in triangles)
                    {
                        var v1 = tri.Vertex1.Location;
                        var v2 = tri.Vertex2.Location;
                        var v3 = tri.Vertex3.Location;

                        var p1 = Vector<double>.Build.DenseOfArray(new[] {v1.X, v1.Y, v1.Z});
                        var p2 = Vector<double>.Build.DenseOfArray(new[] {v2.X, v2.Y, v2.Z});
                        var p3 = Vector<double>.Build.DenseOfArray(new[] {v3.X, v3.Y, v3.Z});

                        var centerOfPlane = FindTriangleCenter(p1, p2, p3, Cross(p1, p2));

                        ////Get the neighboring shared node triangles

                        var nodes1 = tinSurfaceVertices.FirstOrDefault(v => v.Location == v1);
                        var nodes2 = tinSurfaceVertices.FirstOrDefault(v => v.Location == v2);
                        var nodes3 = tinSurfaceVertices.FirstOrDefault(v => v.Location == v3);

                        //Convert Formatting 

                        var NodeTriangles1 = new List<Triangle>();
                        var NodeTriangles2 = new List<Triangle>();
                        var NodeTriangles3 = new List<Triangle>();

                        foreach (var triVertex in nodes1.Triangles)
                        {
                            NodeTriangles1.Add(new Triangle(triVertex.Vertex1,triVertex.Vertex2,triVertex.Vertex3));
                        }
                        foreach (var triVertex in nodes2.Triangles)
                        {
                            NodeTriangles2.Add(new Triangle(triVertex.Vertex1, triVertex.Vertex2, triVertex.Vertex3));
                        }
                        foreach (var triVertex in nodes3.Triangles)
                        {
                            NodeTriangles3.Add(new Triangle(triVertex.Vertex1, triVertex.Vertex2, triVertex.Vertex3));
                        }

                        //Triangle shared at vertex 1, 2, and 3   
                        var sum1 = IterateAdjacentTriangles(NodeTriangles1, p1);
                        var sum2 = IterateAdjacentTriangles(NodeTriangles2, p2);
                        var sum3 = IterateAdjacentTriangles(NodeTriangles3, p3);

                        var M = ((sum1.Top + sum2.Top + sum3.Top)/Magnitude(sum1.Bot + sum2.Bot + sum3.Bot))*offset;
                        var offsetvecCenter = (centerOfPlane + M);

                        var sumv1 = IterateVertexTriangles(NodeTriangles1, p1);
                        var sumv2 = IterateVertexTriangles(NodeTriangles2, p2);
                        var sumv3 = IterateVertexTriangles(NodeTriangles3, p3);

                        var Mc = (((AddVectors(sumv1.Top, sumv2.Top, sumv3.Top))/
                                   (Magnitude(AddVectors(sumv1.Bot, sumv2.Bot, sumv3.Bot))))*offset);

                        var offsetvecVertex1 = (p1 + Mc);
                        var offsetvecVertex2 = (p2 + Mc);
                        var offsetvecVertex3 = (p3 + Mc);


                        // final points


                        //points.Add(offsetvecVertex1);
                        //points.Add(offsetvecVertex2);
                        //points.Add(offsetvecVertex3);
                        points.Add(offsetvecCenter);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                }

            }
            return points;
        }

        public static List<Vector<double>> GetOffsetPoints(List<Triangle> triangles, double offset )
        {
            var points = new List<Vector<double>>();

            foreach (var triangle in triangles)
            {

                try
                {
                    //3-Vertices for Triangle

                    var v1 = triangle;
                    var v2 = triangle;
                    var v3 = triangle;

                    //Formatting

                    var p1 = Vector<double>.Build.DenseOfArray(new[] { v1.V1.X, v1.V1.Y, v1.V1.Z });
                    var p2 = Vector<double>.Build.DenseOfArray(new[] { v2.V2.X, v2.V2.Y, v2.V2.Z });
                    var p3 = Vector<double>.Build.DenseOfArray(new[] { v3.V3.X, v3.V3.Y, v3.V3.Z });

                    var centerOfPlane = FindTriangleCenter(p1, p2, p3, Cross(p1, p2));

                    //Get the neighboring shared node triangles

                    List<Triangle> NodeTriangles1 = GetAllTrianglesConnected(p1, triangles);
                    List<Triangle> NodeTriangles2 = GetAllTrianglesConnected(p2, triangles);
                    List<Triangle> NodeTriangles3 = GetAllTrianglesConnected(p3, triangles);

                    //Triangle shared at vertex 1, 2, and 3   
                    var sum1 = IterateAdjacentTriangles(NodeTriangles1, p1);
                    var sum2 = IterateAdjacentTriangles(NodeTriangles2, p2);
                    var sum3 = IterateAdjacentTriangles(NodeTriangles3, p3);

                    var M = ((sum1.Top + sum2.Top + sum3.Top) / Magnitude(sum1.Bot + sum2.Bot + sum3.Bot)) * offset;
                    var offsetvecCenter = (centerOfPlane + M);

                    var sumv1 = IterateVertexTriangles(NodeTriangles1, p1);
                    var sumv2 = IterateVertexTriangles(NodeTriangles2, p2);
                    var sumv3 = IterateVertexTriangles(NodeTriangles3, p3);

                    var Mc = (((AddVectors(sumv1.Top, sumv2.Top, sumv3.Top)) /
                               (Magnitude(AddVectors(sumv1.Bot, sumv2.Bot, sumv3.Bot)))) * offset);

                    var offsetvecVertex1 = (p1 + Mc);
                    var offsetvecVertex2 = (p2 + Mc);
                    var offsetvecVertex3 = (p3 + Mc);


                    // final points


                    points.Add(offsetvecVertex1);
                    points.Add(offsetvecVertex2);
                    points.Add(offsetvecVertex3);
                    points.Add(offsetvecCenter);

                }
                catch (System.Exception ex)
                {
                   Debug.WriteLine("Error: " + ex.Message);
                }

            }
            return points;
        }
        public static Vector<double> AddVectors(Vector<double> v1, Vector<double> v2, Vector<double> v3)
        {
            return Vector<double>.Build.DenseOfArray(new[] { (v1[0] + v2[0] + v3[0]), (v1[1] + v2[1] + v3[1]), (v1[2] + v2[2] + v3[2]) });
        }
        public static void GetVerticesFromTriangle(Triangle triangle)
        {




        }
        public static Summations IterateAdjacentTriangles(List<Triangle> triangles, Vector<double> centernode)
        {
            Vector<double> top = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });
            Vector<double> bot = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });

            foreach (var triangle in triangles)
            {
                //3-Vertices for Triangle

                var v1 = triangle;
                var v2 = triangle;
                var v3 = triangle;

                //Formatting

                var p1 = Vector<double>.Build.DenseOfArray(new[] { v1.V1.X, v1.V1.Y, v1.V1.Z });
                var p2 = Vector<double>.Build.DenseOfArray(new[] { v2.V2.X, v2.V2.Y, v2.V2.Z });
                var p3 = Vector<double>.Build.DenseOfArray(new[] { v3.V3.X, v3.V3.Y, v3.V3.Z });

                var centerOfPlane = FindTriangleCenter(p1, p2, p3, Cross(p1, p2));
                var vector1 = UnitVector(GetVector(p1, p2));
                var vector2 = UnitVector(GetVector(p1, p3));

                var angle12 = Math.Acos(Dot(vector1, vector2));
                var normal12 = Cross(vector1, vector2);
                var magnit12 = Magnitude(vector1, vector2);

                if (NormalIsPerpedicular(normal12))
                {
                    top += normal12.Multiply(angle12);
                    bot += normal12.Multiply(angle12);
                }
            }

            return new Summations(top, bot);
        }
        public static SummationsAcad IterateAdjacentTrianglesAcad(TinSurfaceTriangleCollection connectedTriangles, Acad.Point3d centernode)
        {
            var top = new Acad.Vector3d(0, 0, 0);
            var bot = new Acad.Vector3d(0, 0, 0);

            foreach (var triangle in connectedTriangles)
            {
                //3-Vertices's for Triangle

                var v1 = triangle;
                var v2 = triangle;
                var v3 = triangle;

                //Formatting

                var p1 = triangle.Vertex1.Location;
                var p2 = triangle.Vertex2.Location;
                var p3 = triangle.Vertex3.Location;

                //Get Vectors

                var vec12 = p1.GetVectorTo(p2);
                var vec13 = p1.GetVectorTo(p3);
                var norm  = vec13.CrossProduct(vec12);

                var centerOfPlane = FindTriangleCenter(p1, p2, p3, norm);


                var e1  = UnitVector3D(vec12);
                var e2  = UnitVector3D(vec13);
                
                var angleradian= Math.Acos(e1.DotProduct(e2));
                var unitnormal = e1.CrossProduct(e2);


                top += unitnormal.MultiplyBy(angleradian);
                bot += unitnormal.MultiplyBy(angleradian);

            }

            return new SummationsAcad(top, bot);
        }


        private static double Magnitude(Acad.Vector3d vec)
        {
            return  (Math.Sqrt(Math.Pow(vec.X, 2) + Math.Pow(vec.Y, 2) + Math.Pow(vec.Z, 2)));

        }

        private static Acad.Vector3d FindTriangleCenter(Autodesk.AutoCAD.Geometry.Point3d p1, Autodesk.AutoCAD.Geometry.Point3d p2, Autodesk.AutoCAD.Geometry.Point3d p3, Autodesk.AutoCAD.Geometry.Vector3d norm)
        {
            var centerX = (p1.X + p2.X + p3.X) / 3;
            var centerY = (p1.Y + p2.Y + p3.Y) / 3;
            var centerZ = FinalZCoordAcad(norm,  p1, new Autodesk.AutoCAD.Geometry.Vector3d( centerX, centerY, 0 ));
            return new Acad.Vector3d(centerX, centerY, centerZ );
        }

        public static bool NormalIsPerpedicular(Vector<double> v)
        {
            if (v[0] == 0.0 && v[1] == 0.0 && v[2] == 0.0)
                return false;
            return true;
        }
        public static Summations OLDIterateVertexTriangles(List<Triangle> triangles)
        {
            Vector<double> top = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });
            Vector<double> bot = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });
            var C = 1.0; ;
            foreach (var triangle in triangles)
            {
                //3-Vertices for Triangle

                var v1 = triangle;
                var v2 = triangle;
                var v3 = triangle;

                //Formatting the input vectors

                var p1 = Vector<double>.Build.DenseOfArray(new[] { v1.V1.X, v1.V1.Y, v1.V1.Z });
                var p2 = Vector<double>.Build.DenseOfArray(new[] { v2.V2.X, v2.V2.Y, v2.V2.Z });
                var p3 = Vector<double>.Build.DenseOfArray(new[] { v3.V3.X, v3.V3.Y, v3.V3.Z });

                var centerOfPlane = FindTriangleCenter(p1, p2, p3, Cross(p1, p2));
                var vector1 = UnitVector(GetVector(p1, p2));
                var vector2 = UnitVector(GetVector(p1, p3));

                var angle12 = Math.Acos(Dot(vector1, vector2));
                var normal12 = Cross(vector1, vector2);
                var magnit12 = Magnitude(vector1, vector2);

                //Concave or Convex
                if (IsConcave(vector1, vector2))
                {
                    C = ComputeConcaveOffset(normal12, vector2);
                }
                else
                    C = ComputeConvexOffset(normal12, vector2);

                //Equation
                top += normal12.Multiply(angle12) * C;
                bot += normal12.Multiply(angle12);
            }

            return new Summations(top, bot);
        }
        public static Vector<double> GetAdjacentTriangleNormals(Triangle tri1, Vector<double> vertexofinterest)
        {
            //3-Vertices for Triangle

            var v1 = tri1;
            var v2 = tri1;
            var v3 = tri1;

            //Formatting the input vectors aroung connecting node

            var p1 = Vector<double>.Build.DenseOfArray(new[] { v1.V1.X, v1.V1.Y, v1.V1.Z });
            var p2 = Vector<double>.Build.DenseOfArray(new[] { v2.V2.X, v2.V2.Y, v2.V2.Z });
            var p3 = Vector<double>.Build.DenseOfArray(new[] { v3.V3.X, v3.V3.Y, v3.V3.Z });

            var pi = vertexofinterest;

            var pf1 = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });
            var pf2 = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });

            if (AreEqual(vertexofinterest, p1))
            {
                pf1 = p2;
                pf2 = p3;
            }

            else if (AreEqual(vertexofinterest, p2))
            {
                pf1 = p1;
                pf2 = p3;
            }

            else
            {
                pf1 = p1;
                pf2 = p2;
            }

            var vector1 = UnitVector(GetVector(pi, pf1));
            var vector2 = UnitVector(GetVector(pi, pf2));

            var angle12 = Math.Acos(Dot(vector1, vector2));
            var normal12 = Cross(vector1, vector2);
            var magnit12 = Magnitude(vector1, vector2);

            return normal12;
        }
        public static Summations IterateVertexTriangles(List<Triangle> triangles, Vector<double> vertexofinterest)
        {
            Vector<double> top = Vector<double>.Build.DenseOfArray(new[] { 0.00001, 0.000001, 0.000001 });
            Vector<double> bot = Vector<double>.Build.DenseOfArray(new[] { 0.00001, 0.000001, 0.000001 });
            var C = 1.0; ;

            if (triangles.Count < 2)
                return new Summations(top, bot);

            for (int i = 0; i < triangles.Count; i++)
            {
                try
                {
                    var j = (i == triangles.Count - 1) ? 0 : i + 1; //Last in queue paired with first
                    var first  = triangles.Where(p => p == triangles[i]).Skip(0).Take(1).FirstOrDefault();
                    var second = triangles.Where(p => p == triangles[j]).Skip(0).Take(1).FirstOrDefault();

                    if (first == null || second == null)
                        continue;

                    //Normal Unit Vector

                    var normalTri1 = UnitVector(GetAdjacentTriangleNormals(first, vertexofinterest));
                    var normalTri2 = UnitVector(GetAdjacentTriangleNormals(second, vertexofinterest));

                    //Get the unit vectors of each triangle

                    var firstvectpair = GetProperVector(first,  vertexofinterest);
                    var seconvectpair = GetProperVector(second, vertexofinterest);


                    var first1  = (firstvectpair.v1);
                    var second1 = (seconvectpair.v1);
                    var first2  = (firstvectpair.v2);
                    var second2 = (seconvectpair.v2);

                    //Concave or Convex; Compute Scale Factor

                    if (IsConcave(first1, second1))
                    {
                        C = ComputeConcaveOffset(normalTri1, normalTri2);
                    }
                    else
                        C = ComputeConvexOffset(normalTri1, normalTri2);

                    //Equation

                    var angle  = Math.Acos(Dot(first1, first2));
                    var normal = Cross(first1, first2);
                    
                    if (NormalIsPerpedicular(normal))
                    {
                        if (C == 0.0 || double.IsNaN(C))
                            C = 1.0;

                        top += normal.Multiply(angle)*C;
                        bot += normal.Multiply(angle);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("IterateVertexTriangles " + ex.Message);
                }
            }

            return new Summations(top, bot);
        }
        public static VectorPair GetProperVector(Triangle tri, Vector<double> vertexofinterest)
        {
            //3-Vertices for Triangle

            var v1 = tri; //vertex 1
            var v2 = tri; //vertex 2
            var v3 = tri; //vertex 3

            //Formatting the input vectors aroung connecting node

            var p1 = Vector<double>.Build.DenseOfArray(new[] { v1.V1.X, v1.V1.Y, v1.V1.Z });
            var p2 = Vector<double>.Build.DenseOfArray(new[] { v2.V2.X, v2.V2.Y, v2.V2.Z });
            var p3 = Vector<double>.Build.DenseOfArray(new[] { v3.V3.X, v3.V3.Y, v3.V3.Z });

            var pi = vertexofinterest;

            var pf1 = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });
            var pf2 = Vector<double>.Build.DenseOfArray(new[] { 0.0, 0, 0 });

            if (AreEqual(vertexofinterest, p1))
            {
                pf1 = p2;
                pf2 = p3;
            }

            else if (AreEqual(vertexofinterest, p2))
            {
                pf1 = p1;
                pf2 = p3;
            }

            else
            {
                pf1 = p1;
                pf2 = p2;
            }

            var vector1 = UnitVector(GetVector(pi, pf1));
            var vector2 = UnitVector(GetVector(pi, pf2));

            var angle12 = Math.Acos(Dot(vector1, vector2));
            var normal12 = Cross(vector1, vector2);
            var magnit12 = Magnitude(vector1, vector2);

            return new VectorPair(vector1, vector2);
        }
        public static bool AreEqual(Vector<double> v1, Vector<double> v2)
        {
            if (v1[0] == v2[0] && v1[1] == v2[1] && v1[2] == v2[2])
                return true;
            return false;
        }
        public static Vector<double> GetVector(Vector<double> p1, Vector<double> p2)
        {
            var v = Vector<double>.Build.DenseOfArray(new[]
                {
            p2[0] - p1[0],
            p2[1] - p1[1],
            p2[2] - p1[2]
        }
            );
            return v;
        }
        public static bool IsConcave(Vector<double> v1, Vector<double> v2)
        {
            //angle = arcos(v1•v2/ |v1||v2|)

            var angle = Math.Acos(Dot(v1, v2) / Magnitude(v1, v2));

            if (angle >  Math.PI / 180)
                return false;
            else
                return true;
        }
        public static double ComputeConcaveOffset(Vector<double> vn, Vector<double> v)
        {
            // ((1.0 + ( r / off)) / (Math.Sin(((3.1415 / 2.0) - angle))) - (r / off)).Dump("ConcaveOffset");

            //	var r = 0.0;
            //	var off = 0.0;
            // var angle = Math.Acos(Dot(vn, v) / Magnitude(vn, v));
            var angle = Math.Acos(Dot(vn, v));
            if (Math.Abs(angle - (3.1415/2)) < 0.001)
                return 1.0;
            else
                return ((1.0)/(Math.Sin(((3.1415/2.0) - angle))));

        }
        public static double ComputeConvexOffset(Vector<double> vn, Vector<double> v)
        {

            var r   = 0.000000012;
            var off = 0.0000001;
            var fillet = r/off;
            var angle = Math.Acos(Dot(vn, v));


            if (Math.Abs(angle - (3.1415 / 2)) < 0.01)
                return 1.0;
            return ((1 - (r / off)) / (Math.Sin((3.1415 / 2 - angle))) + (r / off));

        }
        public static List<Triangle> GetAllTrianglesConnected(Vector<double> vertex, List<Triangle> triangles)
        {
             
            return triangles.Where(

                p => p.V1.X == vertex[0] && p.V1.Y == vertex[1] && p.V1.Z == vertex[2] ||
                     p.V2.X == vertex[0] && p.V2.Y == vertex[1] && p.V2.Z == vertex[2] ||
                     p.V3.X == vertex[0] && p.V3.Y == vertex[1] && p.V3.Z == vertex[2]

            ).ToList();
        }
        public static Vector<double> Cross(Vector<double> left, Vector<double> right)
        {
            if ((left.Count != 3 || right.Count != 3))
            {
                string message = "Vectors must have a length of 3.";
                throw new Exception(message);
            }
            Vector<double> result = Vector<double>.Build.Dense(3);
            result[0] = left[1] * right[2] - left[2] * right[1];
            result[1] = -left[0] * right[2] + left[2] * right[0];
            result[2] = left[0] * right[1] - left[1] * right[0];

            return result;
        }
        public static double FinalZCoord(Vector<double> normal, Vector<double> point, Vector<double> testpoint)
        {
            var a = normal[0];
            var b = normal[1];
            var c = normal[2];

            var x0 = point[0];
            var y0 = point[1];
            var z0 = point[2];

            var x = testpoint[0];
            var y = testpoint[1];

            var z = -(a * x - a * x0 + b * y - b * y0 - c * z0) / c;

            return z;
        }
        public static double FinalZCoordAcad(Autodesk.AutoCAD.Geometry.Vector3d normal, Autodesk.AutoCAD.Geometry.Point3d point, Autodesk.AutoCAD.Geometry.Vector3d testpoint)
        {
            var a = normal.X;
            var b = normal.Y;
            var c = normal.Z;

            var x0 = point.X;
            var y0 = point.Y;
            var z0 = point.Z;

            var x = testpoint.X;
            var y = testpoint.Y;

            var z = -(a * x - a * x0 + b * y - b * y0 - c * z0) / c;

            return z;
        }

        public static Vector<double> FindTriangleCenter(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3, Vector<double> normal)
        {
            var centerX = (tri1[0] + tri2[0] + tri3[0]) / 3;
            var centerY = (tri1[1] + tri2[1] + tri3[1]) / 3;
            var centerZ = FinalZCoord(normal, tri1, Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, 0 }));
            return Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, centerZ });
        }
        public static Vector<double> Cj_Plane()
        {
            return Vector<double>.Build.DenseOfArray(new[] { 1.0, 1.0, 1.0 });
        }
        public static double Angle(double dot, double mag)
        {
            return Math.Acos(dot / mag);
        }
        public static Vector<double> V_Angle(double dot, double mag)
        {
            var a = Math.Acos(dot / mag);
            return Vector<double>.Build.DenseOfArray(new[] { a, a, a });
        }
        public static double Dot(Vector<double> v1, Vector<double> v2)
        {
            return v1.DotProduct(v2);
        }

        public static Acad.Vector3d UnitVector3D(Acad.Vector3d vector)
        {
            return vector.DivideBy(Magnitude(vector));
        }
        public static Vector<double> UnitVector(double x, double y, double z)
        {
            var M = Math.Sqrt(x * x + y * y + z * z);
            return Vector<double>.Build.DenseOfArray(new[] { x / M, y / M, z / M });
        }
        public static Vector<double> UnitVector(Vector<double> vector)
        {
            double x = vector[0];
            double y = vector[1];
            double z = vector[2];

            var M = Math.Sqrt(x * x + y * y + z * z);
            return Vector<double>.Build.DenseOfArray(new[] {x/M, y/M, z/M});
        }
        public static double Magnitude(Vector<double> v1, Vector<double> v2)
        {
            var sum12 = Vector<double>.Build.DenseOfArray(new[] { v1[0] + v2[0], v1[1] + v2[1], v1[2] + v2[2] });
            return Magnitude(sum12);
        }
        public static double Magnitude(Vector<double> v1)
        {
            return Math.Sqrt(Math.Pow(v1[0], 2) + Math.Pow(v1[1], 2) + Math.Pow(v1[2], 2));
        }
        public class facet_normal
        {
            public double Nx;
            public double Ny;
            public double Nz;

            public facet_normal(double x, double y, double z)
            {
                Nx = x;
                Ny = y;
                Nz = z;
            }

        }
        public class Point3d
        {
            public double Nx;
            public double Ny;
            public double Nz;

            public Point3d(double x, double y, double z)
            {
                Nx = x;
                Ny = y;
                Nz = z;
            }

        }
        public class Vector3d
        {
            public double X;
            public double Y;
            public double Z;

            public Vector3d(Point3d pt)
            {
                X = pt.Nx;
                Y = pt.Ny;
                Z = pt.Nz;
            }

            public Vector3d(double v1, double v2, double v3)
            {
                X= v1;
                Y= v2;
                Z = v3;
            }
        }
        public class Triangle
        {
            public Vector3d V1;
            public Vector3d V2;
            public Vector3d V3;
  

            public Triangle(Vector3d x, Vector3d y, Vector3d z)
            {
                V1 = x;
                V2 = y;
                V3 = z;
            }
            public Triangle(Triangle x)
            {
                V1 = x.V1;
                V2 = x.V2;
                V3 = x.V3;
            }
            public Triangle(Vector<double> x, Vector<double> y, Vector<double> z)
            {
                V1 = new Vector3d(x[0], x[1], x[2]);
                V2 = new Vector3d(y[0], y[1], y[2]);
                V3 = new Vector3d(z[0], z[1], z[2]);
            }
            public Triangle(Point3d x, Point3d y, Point3d z)
            {
                V1 = new Vector3d(x.Nx, x.Ny, x.Nz);
                V2 = new Vector3d(x.Nx, x.Ny, x.Nz);
                V3 = new Vector3d(x.Nx, x.Ny, x.Nz);
            }
            public Triangle()
            {
                V1 = new Vector3d(0.0, 0.0, 0.0);
                V2 = new Vector3d(0.0, 0.0, 0.0);
                V3 = new Vector3d(0.0, 0.0, 0.0);
            }

            public Triangle(TinSurfaceVertex vertex1, TinSurfaceVertex vertex2, TinSurfaceVertex vertex3)
            {
 
                V1 = new Vector3d(vertex1.Location.X, vertex1.Location.Y, vertex1.Location.Z);
                V2 = new Vector3d(vertex2.Location.X, vertex2.Location.Y, vertex2.Location.Z);
                V3 = new Vector3d(vertex3.Location.X, vertex3.Location.Y, vertex3.Location.Z);

            }
        }
        public class Summations
        {
            public Vector<double> Top;
            public Vector<double> Bot;

            public Summations(Vector<double> _Top, Vector<double> _Bot)
            {
                Top = _Top;
                Bot = _Bot;
            }
            public Summations()
            {
                Top = Vector<double>.Build.DenseOfArray(new[] { 0.000001, 0.000001, 0.000001 });
                Bot = Vector<double>.Build.DenseOfArray(new[] { 0.000001, 0.000001, 0.000001 });
            }

        }

        public class SummationsAcad
        {
            public Acad.Vector3d Top;
            public Acad.Vector3d Bot;

            public SummationsAcad(Acad.Vector3d _Top, Acad.Vector3d _Bot)
            {
                Top = _Top;
                Bot = _Bot;
            }
            public SummationsAcad()
            {
                Top = new Acad.Vector3d(0, 0, 0);
                Bot = new Acad.Vector3d(0, 0, 0);
            }

        }
        public struct NormalPair
        {
            public Triangle x, y;

            public NormalPair(Triangle p1, Triangle p2)
            {
                x = p1;
                y = p2;
            }
        }
        public struct VectorPair
        {
            public Vector<double> v1, v2;

            public VectorPair(Vector<double> p1, Vector<double> p2)
            {
                v1 = p1;
                v2 = p2;
            }
        }


     
    }
}
