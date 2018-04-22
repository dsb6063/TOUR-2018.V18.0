using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BBC.SurfOffet
{
    public class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        public Point3D(double d, double d1, double d2)
        {
            X = d;
            Y = d1;
            Z = d2;

        }


    }
}
