using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
 
namespace BBC.SurfOffet
{
    public class Tests
    {

        public void Test()
        {
            //	Matrix<double> m = Matrix<double>.Build.DenseOfColumnArrays(new[] {2.0, 3.0,3.0}, new[] {4.0, 5.0,3.0},new[] {4.0, 5.0,3.0});

            //Points only stored as Vector
            Vector<double> p1 = Vector<double>.Build.DenseOfArray(new[] {1.0, 0, 2});
            Vector<double> p2 = Vector<double>.Build.DenseOfArray(new[] {-1.0, 1, 2});
            Vector<double> p3 = Vector<double>.Build.DenseOfArray(new[] {5.0, 0, 3});

            //	Vector<double> p1 = Vector<double>.Build.DenseOfArray(new[] { 1, 3.0, 0 });
            //	Vector<double> p2 = Vector<double>.Build.DenseOfArray(new[] { 2.0, 2, 0 });
            //	Vector<double> p3 = Vector<double>.Build.DenseOfArray(new[] { 1.0, 1, 0 });
            //	Vector<double> p4 = Vector<double>.Build.DenseOfArray(new[] { 1.0, 1, 1 });

            //Build the Actual Vectors
            Vector<double> v1 = PlaneGeometry.GetVector(p1, p2);
            Vector<double> v2 = PlaneGeometry.GetVector(p1, p3);
            Vector<double> v3 = PlaneGeometry.GetVector(p2, p3);

            //Populate Matrix
            var p = PlaneGeometry.PopulateMatrix(v1, v2, v3);

            //normal vector to plane
            var cross = PlaneGeometry.Cross(v1, v2);

            //Get Distance from Plane
            var distance   = PlaneGeometry.DistanceFromPlane(cross, p1, p2);
            var computeZ   = PlaneGeometry.FinalZCoord(cross, p1, p2);
            var Zcorrected = PlaneGeometry.ZCoordCorrected(cross, p1, p2, 1.0);
            var triangleCenter  = PlaneGeometry.FindTriangleCenter(p1, p2, p3, cross);
        }


    }
}
