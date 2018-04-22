//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Windows.Media;
//using System.Windows.Shapes;
//using Autodesk.AutoCAD.Geometry;

////using ESRI.ArcGIS.Client.Geometry;


//namespace Geometry.Model
//    {
//        /// <summary>  
//        /// Converted from a php class by me to work in .NET c#.  The class was taken from this link: http://trac.mapfish.org/trac/mapfish/browser/sandbox/aabt/MapFish/server/php/GeoJSON/WKT/WKT.class.php?rev=2399  
//        /// </summary>  
//        public class WKTModel
//        {
//            static string _regExpTypeStr = @"^\s*(\w+)\s*\(\s*(.*)\s*\)\s*$";
//            static string _regExpSpaces = @"\s+";
//            static string _regExpParenComma = @"\)\s*,\s*\(";
//            static string _regExpDoubleParenComma = @"\)\s*\)\s*,\s*\(\s*\(";
//            static string _regExpTrimParens = @"^\s*\(?(.*?)\)?\s*$";

//            public enum GeometryType
//            {
//                UNKNOWN,
//                POINT,
//                MULTIPOINT,
//                LINESTRING,
//                MULTILINESTRING,
//                POLYGON,
//                MULTIPOLYGON,
//                GEOMETRYCOLLECTION
//            }

//            private WKTModel() { }


//            /// <summary>  
//            /// Converts a geometry object to WKT.  
//            /// </summary>  
//            public static string ToWKT(System.Windows.Media.Geometry g)
//            {
//                StringBuilder sb = new StringBuilder();

//                if (g is Polyline)
//                {
//                    var l = g as Polyline;
//                    if (l.Paths.Count == 1)
//                        sb.AppendFormat("LINESTRING  ({0})", ConvertPointCollection(l.Paths[0]));
//                    else if (l.Paths.Count > 1)
//                        sb.AppendFormat("MULTILINESTRING  ({0})", string.Join(",", l.Paths.Select(x => ConvertPointCollection(x, true))));
//                }
//                return sb.ToString();
//            }

//            public static GeometryType GetGeometryType(string WKT)
//            {
//                if (string.IsNullOrEmpty(WKT))
//                    return GeometryType.UNKNOWN;

//                var matches = Regex.Matches(WKT, _regExpTypeStr);

//                try
//                {
//                    var mytype = matches.Count == 0 ? GeometryType.UNKNOWN : (GeometryType)Enum.Parse(typeof(GeometryType), matches[0].Groups[1].Value, true);
//                    return mytype;
//                }
//                catch (Exception ex)
//                {
//                    return GeometryType.UNKNOWN;
//                }
//            }

//            /// <summary>  
//            /// Parses the WKT and truns it into and ESRI Geometry object  
//            /// </summary>  
//            public static System.Windows.Media.Geometry ToGeometry(string WKT, int WKID)
//            {
//                var geometry = new WKTModel().Read(WKT);
//                geometry.SpatialReference = new SpatialReference(WKID);
//                return geometry;
//            }


//            /// <summary>  
//            /// Parses the WKT and truns it into and ESRI geometry object  
//            /// </summary>  
//            private System.Windows.Media.Geometry Read(string WKT)
//            {
//                if (!Regex.IsMatch(WKT, _regExpTypeStr))
//                    return null;

//                var matches = Regex.Matches(WKT, _regExpTypeStr);
//                return Parse(ParseToGeometryType(matches[0].Groups[1].Value), matches[0].Groups[2].Value);
//            }

//            private GeometryType ParseToGeometryType(string type)
//            {
//                return (GeometryType)Enum.Parse(typeof(GeometryType), type, true);
//            }

//            /// <summary>  
//            /// Parses a section of the WKT into a geometry object  
//            /// </summary>  
//            private Point2d? Parse(GeometryType type, string str)
//            {
//                var points = new PointCollection();

//                switch (type)
//                {
//                    case GeometryType.POINT:
//                        var coords = RegExplode(_regExpSpaces, str);
//                        double x;
//                        double y;
//                        var isXValid = double.TryParse(coords[0], out x);
//                        var isYValid = double.TryParse(coords[1], out y);
//                        if (isXValid && isYValid)
//                            return new Point2d(x, y);
//                        else
//                            return null;
//                    case GeometryType.MULTIPOINT:
//                        foreach (var p in str.Trim().Split(','))
//                            points.Add(Parse(GeometryType.POINT, p) as Point2d);

//                        return new MultiPoint(points);

//                    case GeometryType.LINESTRING:
//                        foreach (var p in str.Trim().Split(','))
//                            points.Add(Parse(GeometryType.POINT, p) as MapPoint);

//                        var line = new ESRI.ArcGIS.Client.Geometry.Polyline();
//                        line.Paths.Add(points);
//                        return line;

//                    //case GeometryType.MULTILINESTRING:
//                    //    var lines = RegExplode(_regExpParenComma, str);
//                    //    var nestedPoints = new List<PointCollection>();
//                    //    foreach (var l in lines)
//                    //    {
//                    //        var myline = Regex.Replace(l, _regExpTrimParens, "$1");
//                    //        nestedPoints.Add(((Polyline)Parse(GeometryType.LINESTRING, myline)).Paths[0]);
//                    //    }
//                    //    return new Polyline() { Paths = new ObservableCollection<PointCollection>(nestedPoints) };

//                    //case GeometryType.POLYGON:
//                    //    var rings = RegExplode(_regExpParenComma, str);
//                    //    var ringPoints = new List<PointCollection>();
//                    //    foreach (var r in rings)
//                    //    {
//                    //        var ring = Regex.Replace(r, _regExpTrimParens, "$1");
//                    //        var pline = Parse(GeometryType.LINESTRING, ring) as Polyline;
//                    //        ringPoints.Add(pline.Paths[0]);
//                    //    }
//                    //    return new Polygon() { Rings = new ObservableCollection<PointCollection>(ringPoints) };

//                    //case GeometryType.MULTIPOLYGON:
//                    //    var polygons = RegExplode(_regExpDoubleParenComma, str);
//                    //    var multiPolyRings = new List<PointCollection>();
//                    //    foreach (var p in polygons)
//                    //    {
//                    //        var polygon = Regex.Replace(p, _regExpTrimParens, "$1");
//                    //        var poly = Parse(GeometryType.POLYGON, polygon) as Polygon;
//                    //        multiPolyRings.AddRange(poly.Rings);
//                    //    }
//                    //    return new Polygon() { Rings = new ObservableCollection<PointCollection>(multiPolyRings) };

//                    //case GeometryType.GEOMETRYCOLLECTION:
//                    //    throw new NotImplementedException();

//                    //  str = preg_replace('/,\s*([A-Za-z])/', '|$1', str);   
//                    //  $wktArray = explode('|', trim(str));   
//                    //  foreach ($wktArray as $wkt)   
//                    //  {   
//                    //    components[] = $this->read($wkt);   
//                    //  }   
//                    //  return new GeometryCollection(components);   

//                    default:
//                        return null;
//                }
//            }


//            /// <summary>  
//            /// Split string according to first match of passed regEx index of regExes   
//            /// </summary>  
//            /// <param name="regEx"></param>  
//            /// <param name="str"></param>  
//            /// <returns></returns>  
//            protected List<string> RegExplode(string regEx, string str)
//            {
//                var matches = Regex.Matches(str, regEx);
//                return matches.Count == 0 ? new List<string> { str.Trim() } : str.Trim().Split(new string[] { matches[0].Value }, StringSplitOptions.None).ToList();
//            }


//            private static string ConvertPointCollection(PointCollection pc, bool wrapInParens = false)
//            {
//                StringBuilder sb = new StringBuilder();
//                if (pc != null)
//                {
//                    foreach (var p in pc)
//                        sb.AppendFormat(ConvertPoint(p));
//                }

//                //trim the trailing comma  
//                if (sb.Length > 0)
//                {
//                    sb = sb.Remove(sb.Length - 1, 1);
//                    if (wrapInParens)
//                        sb = sb.Insert(0, "(").Append(")");
//                }

//                return sb.ToString();
//            }

//            private static string ConvertPoint(MapPoint p)
//            {
//                return string.Format("{0} {1},", p.X, p.Y);
//            }

//        }
//    }
//}
//}
