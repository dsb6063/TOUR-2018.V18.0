using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using Microsoft.SqlServer.Types;
using OSGeo.MapGuide;
using SqlServerTypes;

namespace PGA.DisjointedSurfaces
{

    public class GeometryManager
    {
        public GeometryManager()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
        }

        public static void WellKnownText()
        {
            MgAgfReaderWriter agfReaderWriter;
            MgWktReaderWriter wktReaderWriter;
            MgPoint pt11FromText;
            MgPoint pt11;
            var pt11TextSpec = "POINT XY ( 1 1 )";
            MgByteReader byteReader;
            string geometryAgfText;


            agfReaderWriter = new MgAgfReaderWriter();
            wktReaderWriter = new MgWktReaderWriter();
            // In the string to geometry direction:
            pt11FromText = wktReaderWriter.Read(pt11TextSpec) as MgPoint;
            byteReader = agfReaderWriter.Write(pt11FromText);
            // In the geometry to string direction:
            pt11 = (MgPoint) agfReaderWriter.Read(byteReader);
            geometryAgfText = wktReaderWriter.Write(pt11);
            // geometryAgfText now contains the text representation of the geometry
        }

        public static SqlBytes SerializeSqlGeographyMultiPoint(Point3dCollection points3d)
        {    
           
            var result = "";
            foreach (Point3d point in points3d)
            {
                if (points3d.IndexOf(point)==0)
                   result = String.Format("MULTIPOINT (({0} {1} {2})", point.X, point.Y, point.Z);
                result+= String.Format(",({0} {1} {2})", point.X, point.Y, point.Z);

            }
            if (String.IsNullOrEmpty(result)) return null;
            result += ")";  //, null)";
            SqlGeometry multipoint = SqlGeometry.STMPointFromText(new SqlChars(result), 0);
         
            return multipoint.Serialize();
        }

        public static SqlBytes SerializeSqlGeographyMultiPoint(Point2dCollection points2d)
        {   

            var result = "";
            foreach (Point2d point in points2d)
            {
                if (points2d.IndexOf(point) == 0)
                    result = String.Format("MULTIPOINT (({0} {1} {2})", point.X, point.Y, 0);
                result += String.Format(",({0} {1} {2})", point.X, point.Y, 0);

            }
            result += ")";  //, null)";
            SqlGeometry multipoint = SqlGeometry.STMPointFromText(new SqlChars(result), 0);
    
            return multipoint.Serialize();
        }
        //public static SqlBytes SerializeSqlGeographyMultiPoint(Point2dCollection points2d)
        //{
        //    SqlGeometryBuilder buildpoint = new SqlGeometryBuilder();
        //    buildpoint.SetSrid(0);
        //    buildpoint.BeginGeometry(OpenGisGeometryType.MultiPoint);

        //    foreach (Point2d point in points2d)
        //    {
        //        buildpoint.AddLine(point.X, point.Y, 0, null);
        //    }
        //    buildpoint.EndFigure();
        //    buildpoint.EndGeometry();
        //    return buildpoint.ConstructedGeometry.Serialize();
        //}
        public  static Point3dCollection DerializeSqlGeographyMultiPoint(SqlBytes points)
        {
            SqlGeometry wktpoints = SqlGeometry.Deserialize(points);
            if (wktpoints == null) throw new ArgumentNullException(nameof(wktpoints));
            Point3dCollection point3D = new Point3dCollection();

            for (int n = 1; n < wktpoints.STNumPoints();n++)
            {
                SqlGeometry pnt = wktpoints.STPointN(n);
                point3D.Add(new Point3d((double) pnt.STX, (double) pnt.STY, (double) pnt.Z));
            }
            return point3D;
        }


        public static byte[] ToBytes(string hex)
        {
            var shb = SoapHexBinary.Parse(hex);
            return shb.Value;
        }

        internal static byte[] ToBytes(SqlBytes b)
        { 
           //var shb = SoapHexBinary.Parse(b.ToString());
           // return shb.Value;


            byte[] binaryString = b.Buffer;

            // if the original encoding was ASCII
            string x = Encoding.ASCII.GetString(binaryString);

            //// if the original encoding was UTF-8
            //string y = Encoding.UTF8.GetString(binaryString);

            //// if the original encoding was UTF-16
            //string z = Encoding.Unicode.GetString(binaryString);

            return binaryString;
        }

        internal static string BytesToString(SqlBytes b)
        {

            byte[] binaryString = b.Buffer;

            // if the original encoding was ASCII
            string x = Encoding.ASCII.GetString(binaryString);

            return x;
        }
    }
}
