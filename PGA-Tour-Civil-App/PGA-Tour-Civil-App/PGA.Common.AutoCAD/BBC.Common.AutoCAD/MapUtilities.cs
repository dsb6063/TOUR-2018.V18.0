using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.Platform;

using OSGeo.MapGuide;

using Pge.Common.DataTransfer;

namespace Pge.Common.AutoCAD
{
    public class MapUtilities
    {
        #region Private Memebers

        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(MapUtilities));

        #endregion
        
        #region MapBreak

        /// <summary>
        /// Maps the break.
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <param name="layerFilter">The layer filter.</param>
        /// <returns></returns>
        public static bool MapBreak(IList<GemsPoint2d> polyPoints, string layerFilter)
        {
            bool retval = false;

            //_logger.Debug("Start MapBreak");
            GemsPoint2d minPoint = CoordinateService.GetMinFromPolygon(polyPoints);
            GemsPoint2d maxPoint = CoordinateService.GetMaxFromPolygon(polyPoints);

            // This should return a selection set and check that it contains one or more objects
            // I would need to add a using to get the SelectionSet object in this class...?
            SelectionManager.GetSelectionSet(minPoint.X, minPoint.Y, maxPoint.X, maxPoint.Y, layerFilter, true);

            // The CMDDIA calls should reference SetSystemVariable
            StringBuilder acadCmd = new StringBuilder();
            acadCmd.Append("CMDDIA 0 ");
            acadCmd.Append("_MAPBREAK DEFINE ");
            foreach (GemsPoint2d onePoint in polyPoints)
            {
                acadCmd.Append(onePoint.X.ToString());
                acadCmd.Append(",");
                acadCmd.Append(onePoint.Y.ToString());
                acadCmd.Append("\r");
            }
            acadCmd.Append("\r");
            acadCmd.Append("N Y P  Y Y ");
            acadCmd.Append("CMDDIA 1 ");
            AcadUtilities.SendStringToExecute(acadCmd.ToString());
            retval = true;

            //_logger.Debug("End MapBreak");
            return retval;
        }

        #endregion

        #region AutoCAD Map Project interactions for Coordinate Systems

        /// <summary>
        /// References the current AutoCAD Map Project drawing and returns the AutoCAD Map coordinate system code (if set)
        /// </summary>
        /// <returns></returns>
        public static string GetMapCSCode()
        {
            string mapcscode = String.Empty;

            //_logger.Debug("Start GetMapCSCode");
            try
            {
                MapApplication AcMap;
                Autodesk.Gis.Map.Project.ProjectModel AcMapProject;
                AcMap = HostMapApplicationServices.Application;
                AcMapProject = AcMap.ActiveProject;
                mapcscode = AcMapProject.Projection;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in GetMapCSCode", ex);
                throw;
            }
            //_logger.Debug("End GetMapCSCode");
            return mapcscode;
        }

        /// <summary>
        /// References the current AutoCAD Map Project drawing and returns the WKText for the current coordinate system (if set)
        /// </summary>
        /// <returns></returns>
        public static string GetWKTextCS()
        {
            string wktextcs = String.Empty;

            //_logger.Debug("Start GetWKTextCS");
            try
            {
                string mapcscode = GetMapCSCode();
                if (!String.IsNullOrEmpty(mapcscode))
                {
                    AcMapMap MapProject = AcMapMap.GetCurrentMap();
                    wktextcs = MapProject.GetMapSRS();
                }
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in GetWKTextCS", ex);
                throw;
            }
            //_logger.Debug("End GetWKTextCS");
            return wktextcs;
        }

        /// <summary>
        /// References the current AutoCAD Map Project drawing and sets the AutoCAD Map coordinate system code
        /// Note that this must not be called if a query has been executed.
        /// Behavior is unknown if there are even drawings attached to the project
        /// Best to call this on a new drawing with no attached drawings or queried objects
        /// </summary>
        /// <param name="mapCsCode">The map cs code.</param>
        /// <returns></returns>
        public static bool SetMapCSCode(string mapCsCode)
        {
            bool retVal = false;

            //_logger.Debug("Start SetMapCSCode");
            try
            {
                // Note that setting the coordinate system code causes a zoom window command to be piped to the command line
                // This is getting sent in a manner that causes AutoCAD to get confused
                // Replacing with SendStringToExecute is a work around
                AcadUtilities.SendStringToExecute("ADESETCRDSYS S " + mapCsCode + " X ");
                //MapApplication AcMap;
                //Autodesk.Gis.Map.Project.ProjectModel AcMapProject;
                //AcMap = HostMapApplicationServices.Application;
                //AcMapProject = AcMap.ActiveProject;
                //AcMapProject.Projection = mapCsCode;
                retVal = true;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in SetMapCSCode", ex);
                throw;
            }
            //_logger.Debug("End SetMapCSCode");
            return retVal;
        }

        /// <summary>
        /// References the current AutoCAD Map Project drawing and sets the AutoCAD Map coordinate system code
        /// Note that this must not be called if a query has been executed.
        /// Behavior is unknown if there are even drawings attached to the project
        /// Best to call this on a new drawing with no attached drawings or queried objects
        /// </summary>
        /// <param name="mapCsCode">The map cs code.</param>
        /// <returns></returns>
        public static bool SetMapCSCode_API(string mapCsCode)
        {
            bool retVal = false;

            //_logger.Debug("Start SetMapCSCode_API");
            try
            {
                MapApplication AcMap;
                Autodesk.Gis.Map.Project.ProjectModel AcMapProject;
                AcMap = HostMapApplicationServices.Application;
                AcMapProject = AcMap.ActiveProject;
                AcMapProject.Projection = mapCsCode;
                retVal = true;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in SetMapCSCode_API", ex);
                throw;
            }
            //_logger.Debug("End SetMapCSCode_API");
            return retVal;
        }

        /// <summary>
        /// Clears the AutoCAD Map Error Stack
        /// </summary>
        /// <returns></returns>
        public static bool ClearErrorStack()
        {
            bool retVal = false;

            //_logger.Debug("Start ClearErrorStack");
            try
            {
                MapApplication AcMap = HostMapApplicationServices.Application;
                AcMap.ErrorStack.Clear();
                retVal = true;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in ClearErrorStack", ex);
                throw;
            }
            //_logger.Debug("End ClearErrorStack");
            return retVal;
        }

        #endregion

        #region Coordinate Transformation

        // Note: OSGeo references WKText strings for coordinate system definitions.
        // A database lookup table has been populated with AutoCAD Map Coordinate System codes and WKText strings
        // Use the Data Manager method GetWKTextFromMapCsCode to translate MapCS to WKText

        /// <summary>
        /// Coordinate transformation between two world coordinate systems
        /// This Version works with coordinates as AutoCAD Point2d objects
        /// </summary>
        /// <param name="wkTextSource">The wk text source.</param>
        /// <param name="wkTextTarget">The wk text target.</param>
        /// <param name="inputPoint">The input point.</param>
        /// <param name="transformedPoint">The transformed point.</param>
        /// <returns></returns>
        public static bool TransformPoint2d(string wkTextSource, string wkTextTarget, GemsPoint2d inputPoint, ref GemsPoint2d transformedPoint)
        {
            bool RetVal = false;

            //_logger.Debug("Start TransformPoint2d");
            try
            {
                //Creating coordinate system factory
                MgCoordinateSystemFactory CSFactory = new MgCoordinateSystemFactory();
                //Creating coordinate system objects
                MgCoordinateSystem SourceCS = CSFactory.Create(wkTextSource);
                MgCoordinateSystem TargetCS = CSFactory.Create(wkTextTarget);
                //Creating geometry factory
                MgGeometryFactory GeomFactory = new MgGeometryFactory();
                //Populating geometry factory CreateCoordinateXY
                MgCoordinate SourceCoord = GeomFactory.CreateCoordinateXY(inputPoint.X, inputPoint.Y);
                //Getting transformation definition
                MgCoordinateSystemTransform CSTransform = CSFactory.GetTransform(SourceCS, TargetCS);
                //Transforming coordinate
                MgCoordinate TargetCoord = CSTransform.Transform(SourceCoord);
                //Populating return coordinate object
                transformedPoint = new GemsPoint2d();
                transformedPoint.X = TargetCoord.X;
                transformedPoint.Y = TargetCoord.Y;

                RetVal = true;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in TransformPoint2d", ex);
                throw;
            }
            //_logger.Debug("End TransformPoint2d");
            return RetVal;
        }

        /// <summary>
        /// Coordinate transformation between two world coordinate systems
        /// This Version works with coordinates as X, Y doubles
        /// </summary>
        /// <param name="wkTextSource">The wk text source.</param>
        /// <param name="wkTextTarget">The wk text target.</param>
        /// <param name="inputPointX">The input point X.</param>
        /// <param name="inputPointY">The input point Y.</param>
        /// <param name="transformedPointX">The transformed point X.</param>
        /// <param name="transformedPointY">The transformed point Y.</param>
        /// <returns></returns>
        public static bool TransformPoint2d(string wkTextSource, string wkTextTarget, double inputPointX, double inputPointY, ref double transformedPointX, ref double transformedPointY)
        {
            bool RetVal = false;

            //_logger.Debug("Start TransformPoint2d");
            try
            {
                //Creating coordinate system factory
                MgCoordinateSystemFactory CSFactory = new MgCoordinateSystemFactory();
                //Creating coordinate system objects
                MgCoordinateSystem SourceCS = CSFactory.Create(wkTextSource);
                MgCoordinateSystem TargetCS = CSFactory.Create(wkTextTarget);
                //Creating geometry factory
                MgGeometryFactory GeomFactory = new MgGeometryFactory();
                //Populating geometry factory CreateCoordinateXY
                MgCoordinate SourceCoord = GeomFactory.CreateCoordinateXY(inputPointX, inputPointY);
                //Getting transformation definition
                MgCoordinateSystemTransform CSTransform = CSFactory.GetTransform(SourceCS, TargetCS);
                //Transforming coordinate
                MgCoordinate TargetCoord = CSTransform.Transform(SourceCoord);
                //Populating return coordinate objects
                transformedPointX = TargetCoord.X;
                transformedPointY = TargetCoord.Y;
                RetVal = true;
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in TransformPoint2d", ex);
                throw;
            }
            //_logger.Debug("End TransformPoint2d");
            return RetVal;
        }

        #endregion

        /// <summary>
        /// Formats an IList of GemsPoint2d objects into an SDO Polygon string
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <param name="oracleSrid">The oracle srid.</param>
        /// <returns></returns>
        public static string FormatPolygonToSDO(IList<GemsPoint2d> polyPoints, string oracleSrid)
        {
            string sdoString = String.Empty;

            //_logger.Debug("Start FormatPolygonToSDO");
            try
            {
                //SDO_GTYPE is formatted as: DLTT - where D:2 Dimensional, L:0 No 3rd Dimensional content TT: 03 Polygon
                string SDO_GTYPE = "2003";
                //Spatial Resource Identifier (Oracle's Coordinate System Number)
                string SDO_SRID = oracleSrid;
                //This is a polygon so SDO_POINT is NULL
                string SDO_POINT = "NULL";
                //Starting Offset 1, Exterior Polygon, Straight Line Segments (not arcs)
                string SDO_ELEM_INFO = "SDO_ELEM_INFO_ARRAY(1,1003,1)";
                //Stepping through points
                string SDO_ORDINATES = "";
                SDO_ORDINATES = "SDO_ORDINATE_ARRAY(";
                string pointString = FormatPolygonToPointString(polyPoints);
                SDO_ORDINATES += pointString;
                //foreach (GemsPoint2d onePoint in polyPoints)
                //{
                //    SDO_ORDINATES += onePoint.X.ToString() + "," + onePoint.Y.ToString();
                //    SDO_ORDINATES += ",";
                //}
                ////Verifying polygon is closed (first point == last point)
                //GemsPoint2d firstPoint = polyPoints[0];
                //GemsPoint2d lastPoint = polyPoints[polyPoints.Count - 1];
                //if ((firstPoint.X == lastPoint.X) && (firstPoint.Y == lastPoint.Y))
                //{
                //    //Drop the comma
                //    SDO_ORDINATES = SDO_ORDINATES.Substring(0, SDO_ORDINATES.Length - 1);
                //}
                //else
                //{
                //    //Add first point as closing point
                //    SDO_ORDINATES += firstPoint.X.ToString() + "," + firstPoint.Y.ToString();
                //}
                SDO_ORDINATES += ")";

                //Concatenating SDO object strings to SDO Geometry string"
                sdoString += "SDO_GEOMETRY";
                sdoString += "(";
                sdoString += SDO_GTYPE + ",";
                sdoString += SDO_SRID + ",";
                sdoString += SDO_POINT + ",";
                sdoString += SDO_ELEM_INFO + ",";
                sdoString += SDO_ORDINATES;
                sdoString += ")";
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in FormatPolygonToSDO", ex);
                throw;
            }
            //_logger.Debug("End FormatPolygonToSDO");
            return sdoString;
        }

        /// <summary>
        /// Formats an IList of GemsPoint2d objects into a comma separated string of x1,y1, x2,y2... xn,yn
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <returns></returns>
        public static string FormatPolygonToPointString(IList<GemsPoint2d> polyPoints)
        {
            string pointString = String.Empty;

            //_logger.Debug("Start FormatPolygonToPointString");
            try
            {
                foreach (GemsPoint2d onePoint in polyPoints)
                {
                    pointString += onePoint.X.ToString() + "," + onePoint.Y.ToString();
                    pointString += ",";
                }
                //Verifying polygon is closed (first point == last point)
                GemsPoint2d firstPoint = polyPoints[0];
                GemsPoint2d lastPoint = polyPoints[polyPoints.Count - 1];
                if ((firstPoint.X == lastPoint.X) && (firstPoint.Y == lastPoint.Y))
                {
                    //Drop the comma
                    pointString = pointString.Substring(0, pointString.Length - 1);
                }
                else
                {
                    //Add first point as closing point
                    pointString += firstPoint.X.ToString() + "," + firstPoint.Y.ToString();
                }
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in FormatPolygonToPointString", ex);
                throw;
            }
            //_logger.Debug("End FormatPolygonToPointString");
            return pointString;
        }

        /// <summary>
        /// Extracts the points from string.
        /// </summary>
        /// <param name="pointString">The point string.</param>
        /// <returns></returns>
        public static IList<GemsPoint2d> ExtractPointsFromString(string pointString)
        {
            IList<GemsPoint2d> polyPoints = new List<GemsPoint2d>();

            //_logger.Debug("Start ExtractPointsFromString");
            try
            {
                string[] coordinates = pointString.Split(',');
                bool isCoordX = true;
                GemsPoint2d onePoint = null;
                foreach (string coordinate in coordinates)
                {
                    if (isCoordX == true)
                    {
                        onePoint = new GemsPoint2d();
                        onePoint.X = Convert.ToDouble(coordinate);
                        isCoordX = false;
                    }
                    else
                    {
                        onePoint.Y = Convert.ToDouble(coordinate);
                        polyPoints.Add(onePoint);
                        isCoordX = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in ExtractPointsFromString", ex);
                throw;
            }
            //_logger.Debug("End ExtractPointsFromString");
            return polyPoints;
        }

    }
}




