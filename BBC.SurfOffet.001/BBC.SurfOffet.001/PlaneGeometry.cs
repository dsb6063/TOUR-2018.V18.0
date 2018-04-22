// ***********************************************************************
// Assembly         : BBC.SurfOffet.001
// Author           : Daryl Banks, PSM
// Created          : 03-25-2017
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 03-25-2017
// ***********************************************************************
// <copyright file="PlaneGeometry.cs" company="Banks & Banks Consulting">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace BBC.SurfOffet
{
    /// <summary>
    /// Class PlaneGeometry.
    /// </summary>
    public class PlaneGeometry
    {

        /// <summary>
        /// Creates the point.
        /// </summary>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="Z">The z.</param>
        /// <returns>Point3D.</returns>
        public Point3D CreatePoint(double X, double Y, double Z)
        {
           return new Point3D(X, Y, Z);
        }

        /// <summary>
        /// Creates the matrix.
        /// </summary>
        /// <returns>Matrix&lt;System.Double&gt;.</returns>
        public Matrix<double> CreateMatrix()
        {
    
            return Matrix<double>.Build.Random(3, 3);
        }


        #region MathNet Methods

        /// <summary>
        /// Populates the matrix.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        /// <returns>Matrix&lt;System.Double&gt;.</returns>
        public static Matrix<double> PopulateMatrix(Vector<double> v1, Vector<double> v2, Vector<double> v3)
        {
            return Matrix<double>.Build.DenseOfColumnVectors(v1, v2, v3);
        }

        /// <summary>
        /// Gets the vector.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>Vector&lt;System.Double&gt;.</returns>
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

        //using DLA = MathNet.Numerics.LinearAlgebra.Double;

        /// <summary>
        /// Crosses the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>Vector&lt;System.Double&gt;.</returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Distances from plane.
        /// Equation of Plane (a*(x-x0) + b*(y-y0) + c*(z-z0));
        /// x0, y0, z0 are the initial point on the plane
        /// Test Point to get distance from plane (x,y,z)
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="point">The point.</param>
        /// <param name="testpoint">The testpoint.</param>
        /// <returns>System.Double.</returns>
        public static double DistanceFromPlane(Vector<double> normal, Vector<double> point, Vector<double> testpoint)
        {
            var a = normal[0];
            var b = normal[1];
            var c = normal[2];

            var x0 = point[0];
            var y0 = point[1];
            var z0 = point[2];

            var x = testpoint[0];
            var y = testpoint[1];
            var z = testpoint[2];

            var DistanceFromPlane = (a * (x - x0) + b * (y - y0) + c * (z - z0));
            var PerpendicularDist = Math.Abs(DistanceFromPlane) / Math.Sqrt(a * a + b * b + c * c);
            return PerpendicularDist;
        }

        /// <summary>
        /// Finals the z coord.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="point">The point.</param>
        /// <param name="testpoint">The testpoint.</param>
        /// <returns>System.Double.</returns>
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

        /// <summary>
        /// z the coord corrected.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="point">The point.</param>
        /// <param name="testpoint">The testpoint.</param>
        /// <param name="Offset">The offset.</param>
        /// <returns>System.Double.</returns>
        /// Todo: Needs additional Work!
        public static double ZCoordCorrected(Vector<double> normal, Vector<double> point, Vector<double> testpoint, double Offset)
        {
            var a = normal[0];
            var b = normal[1];
            var c = normal[2];

            var x0 = point[0];
            var y0 = point[1];
            var z0 = point[2];

            var x = testpoint[0];
            var y = testpoint[1];

            var z = (-a * x + a * x0 - b * y + b * y0 + c * z0 + Offset * Math.Sqrt(a * a + b * b + c * c)) / c;

            return z;
        }

        /// <summary>
        /// Finds the triangle center.
        /// </summary>
        /// <param name="tri1">The tri1.</param>
        /// <param name="tri2">The tri2.</param>
        /// <param name="tri3">The tri3.</param>
        /// <param name="normal">The normal.</param>
        /// <returns>Vector&lt;System.Double&gt;.</returns>
        public static Vector<double> FindTriangleCenter(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3, Vector<double> normal)
        {
            var centerX = (tri1[0] + tri2[0] + tri3[0]) / 3;
            var centerY = (tri1[1] + tri2[1] + tri3[1]) / 3;
            var centerZ = FinalZCoord(normal, tri1, Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, 0 }));
            return Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, centerZ });
        }

        public static Vector<double> Find2DTriangleCenter(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3 )
        {
            var centerX = (tri1[0] + tri2[0] + tri3[0]) / 3;
            var centerY = (tri1[1] + tri2[1] + tri3[1]) / 3;
            var centerZ = 0;
            return Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, centerZ });
        }

        public static Vector<double> FindOffset(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3,double offset)
        {
            var normal = Cross(tri1, tri2);
            var centerX = (tri1[0] + tri2[0] + tri3[0]) / 3;
            var centerY = (tri1[1] + tri2[1] + tri3[1]) / 3;
            var centerZ = FinalZCoord(normal, tri1, Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, 0 }));
            return Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, centerZ + offset });
        }

        public static Vector<double> FindEdge1Offset(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3, double offset)
        {
            var normal = Cross(tri1, tri2);
            var centerX = (tri1[0]);
            var centerY = (tri1[1]);
            var centerZ = FinalZCoord(normal, tri1, Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, 0 }));
            return Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, centerZ + offset });
        }
        public static Vector<double> FindEdge2Offset(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3, double offset)
        {
            var normal = Cross(tri1, tri2);
            var centerX = (tri1[0]);
            var centerY = (tri1[1]);
            var centerZ = FinalZCoord(normal, tri1, Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, 0 }));
            return Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, centerZ + offset });
        }
        public static Vector<double> FindEdge3Offset(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3, double offset)
        {
            var normal = Cross(tri1, tri2);
            var centerX = (tri1[0]);
            var centerY = (tri1[1]);
            var centerZ = FinalZCoord(normal, tri1, Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, 0 }));
            return Vector<double>.Build.DenseOfArray(new[] { centerX, centerY, centerZ + offset });
        }
        /// <summary>
        /// Finds the offset.
        /// </summary>
        /// <param name="tri1">The tri1.</param>
        /// <param name="tri2">The tri2.</param>
        /// <param name="tri3">The tri3.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Vector&lt;System.Double&gt;.</returns>
        public static Vector<double> FindOffset(Vector<double> tri1, Vector<double> tri2, Vector<double> tri3, Vector<double> normal, double offset)
        {
            //Point on the Plane at Center
            Vector<double> centerpoint = FindTriangleCenter(tri1, tri2, tri3, normal);

            //Move the point along Projection Vector the Z offset
            return Vector<double>.Build.DenseOfArray(new[] { centerpoint[0], centerpoint[1], centerpoint[2] + offset });
        }
        #endregion

    }
}
