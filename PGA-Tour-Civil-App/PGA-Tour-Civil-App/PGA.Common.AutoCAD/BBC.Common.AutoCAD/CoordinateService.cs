using System;
using System.Collections.Generic;
using System.Text;

using Common.Logging;

using Pge.Common.Framework;
using Pge.Common.DataManager;
using Pge.Common.DataTransfer;

namespace Pge.Common.AutoCAD
{
    public class CoordinateService
    {

        #region Private Members

        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(CoordinateService));

        #endregion

        /// <summary>
        /// Gets the state planes.
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetStatePlanes()
        {
            //_logger.Debug("Start GetStatePlanes");
            IList<string> statePlanes = new List<string>();
            statePlanes.Add("CA-I");
            statePlanes.Add("CA-II");
            statePlanes.Add("CA-III");
            statePlanes.Add("CA-IV");
            statePlanes.Add("CA-V");
            //_logger.Debug("End GetStatePlanes");
            return statePlanes;
        }

        /// <summary>
        /// Gets the centroid (mathematical) from a list of points
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <returns></returns>
        public static GemsPoint2d GetCentroidFromPolygon(IList<GemsPoint2d> polyPoints)
        {
            //_logger.Debug("Start GetCentroidFromPolygon");
            GemsPoint2d minPoint = GetMinFromPolygon(polyPoints);
            GemsPoint2d maxPoint = GetMaxFromPolygon(polyPoints);
            GemsPoint2d centroidPoint = new GemsPoint2d();
            centroidPoint.X = minPoint.X + ((maxPoint.X - minPoint.X) / 2.0);
            centroidPoint.Y = minPoint.Y + ((maxPoint.Y - minPoint.Y) / 2.0);
            //_logger.Debug("End GetCentroidFromPolygon");
            return centroidPoint;
        }

        /// <summary>
        /// Gets the maximum bounding point from a list of points
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <returns></returns>
        public static GemsPoint2d GetMaxFromPolygon(IList<GemsPoint2d> polyPoints)
        {
            //_logger.Debug("Start GetMaxFromPolygon");
            GemsPoint2d maxPoint = new GemsPoint2d();
            GemsPoint2d firstPoint = polyPoints[0];
            maxPoint.X = firstPoint.X;
            maxPoint.Y = firstPoint.Y;

            foreach (GemsPoint2d onePoint in polyPoints)
            {
                if (onePoint.X > maxPoint.X)
                {
                    maxPoint.X = onePoint.X;
                }
                if (onePoint.Y > maxPoint.Y)
                {
                    maxPoint.Y = onePoint.Y;
                }
            }
            //_logger.Debug("End GetMaxFromPolygon");
            return maxPoint;
        }

        /// <summary>
        /// Gets the minimum bounding point from a list of points
        /// </summary>
        /// <param name="polyPoints">The poly points.</param>
        /// <returns></returns>
        public static GemsPoint2d GetMinFromPolygon(IList<GemsPoint2d> polyPoints)
        {
            //_logger.Debug("Start GetMinFromPolygon");
            GemsPoint2d minPoint = new GemsPoint2d();
            GemsPoint2d firstPoint = polyPoints[0];
            minPoint.X = firstPoint.X;
            minPoint.Y = firstPoint.Y;

            foreach (GemsPoint2d onePoint in polyPoints)
            {
                if (onePoint.X < minPoint.X)
                {
                    minPoint.X = onePoint.X;
                }
                if (onePoint.Y < minPoint.Y)
                {
                    minPoint.Y = onePoint.Y;
                }
            }
            //_logger.Debug("End GetMinFromPolygon");
            return minPoint;
        }

        /// <summary>
        /// Builds the polygon from min max.
        /// </summary>
        /// <param name="lowerLeft">The lower left.</param>
        /// <param name="upperRight">The upper right.</param>
        /// <returns></returns>
        public static IList<GemsPoint2d> BuildPolygonFromMinMax(GemsPoint2d lowerLeft, GemsPoint2d upperRight)
        {
            //_logger.Debug("Start BuildPolygonFromMinMax");
            IList<GemsPoint2d> polyPoints = new List<GemsPoint2d>();
            
            polyPoints.Add(new GemsPoint2d(lowerLeft.X, lowerLeft.Y));
            polyPoints.Add(new GemsPoint2d(upperRight.X, lowerLeft.Y));
            polyPoints.Add(new GemsPoint2d(upperRight.X, upperRight.Y));
            polyPoints.Add(new GemsPoint2d(lowerLeft.X, upperRight.Y));
            polyPoints.Add(new GemsPoint2d(lowerLeft.X, lowerLeft.Y));

            //_logger.Debug("End BuildPolygonFromMinMax");
            return polyPoints;
        }

        /// <summary>
        /// Translates the WG S84 to state plane.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="statePlaneMapCsCode">The state plane map cs code.</param>
        /// <param name="inputCoord">The input coord.</param>
        /// <returns></returns>
        public static GemsPoint2d TranslateWGS84ToStatePlane(IDataManager manager, string statePlaneMapCsCode, GemsPoint2d inputCoord)
        {
            //_logger.Debug("Start TranslateWGS84ToStatePlane");
            bool retval = false;

            string wgs84mapcscode = "LL84";

            string wgs84wktext = manager.GetWKTextFromMapCsCode(wgs84mapcscode);
            string stateplanewktext = manager.GetWKTextFromMapCsCode(statePlaneMapCsCode);
            GemsPoint2d transformedCoord = new GemsPoint2d();
            retval = MapUtilities.TransformPoint2d(wgs84wktext, stateplanewktext, inputCoord, ref transformedCoord);

            //_logger.Debug("End TranslateWGS84ToStatePlane");
            return transformedCoord;
        }

        /// <summary>
        /// Translates the state plane to WG S84.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="statePlaneMapCsCode">The state plane map cs code.</param>
        /// <param name="inputCoord">The input coord.</param>
        /// <returns></returns>
        public static GemsPoint2d TranslateStatePlaneToWGS84(IDataManager manager, string statePlaneMapCsCode, GemsPoint2d inputCoord)
        {
            //_logger.Debug("Start TranslateStatePlaneToWGS84");
            bool retval = false;

            string wgs84mapcscode = "LL84";

            string wgs84wktext = manager.GetWKTextFromMapCsCode(wgs84mapcscode);
            string stateplanewktext = manager.GetWKTextFromMapCsCode(statePlaneMapCsCode);
            GemsPoint2d transformedCoord = new GemsPoint2d();
            retval = MapUtilities.TransformPoint2d(stateplanewktext, wgs84wktext , inputCoord, ref transformedCoord);

            //_logger.Debug("End TranslateStatePlaneToWGS84");
            return transformedCoord;
        }

        /// <summary>
        /// Gets the state plane from lat long.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        /// <param name="statePlane">The state plane.</param>
        /// <returns></returns>
        public static bool GetStatePlaneFromLatLong(IDataManager manager, GemsPoint2d source, ref GemsPoint2d dest, ref string statePlane)
        {
            //_logger.Debug("Start GetStatePlaneFromLatLong");
            bool result = false;
            
            string statePlaneUsed = String.Empty;
            foreach (string sp in CoordinateService.GetStatePlanes())
            {
                try
                {
                    dest = CoordinateService.TranslateWGS84ToStatePlane(manager, sp, source);
                    result = true;
                    statePlane = sp;
                    break;
                }
                catch(System.Exception ex) 
                {
                    //_logger.Error("Error in GetStatePlaneFromLatLong", ex);
                    throw;
                }
            }
            //_logger.Debug("End GetStatePlaneFromLatLong");
            return result;
        }

        /// <summary>
        /// Gets the lat long to state plane.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="source">The source.</param>
        /// <param name="statePlane">The state plane.</param>
        /// <param name="dest">The dest.</param>
        /// <returns></returns>
        public static bool GetLatLongFromStatePlane(IDataManager manager, GemsPoint2d source, string statePlane, ref GemsPoint2d dest)
        {
            //_logger.Debug("Start GetLatLongFromStatePlane");
            bool result = false;

            try
            {
                dest = CoordinateService.TranslateStatePlaneToWGS84(manager, statePlane, source);
                result = true;                    
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in GetLatLongFromStatePlane", ex);
                throw;
            }
            //_logger.Debug("Start GetLatLongFromStatePlane");
            return result;
        }

        /// <summary>
        /// Converts to lat long.
        /// </summary>
        /// <param name="dm">The dm.</param>
        /// <param name="sourcePoints">The source points.</param>
        /// <param name="acMapCsCode">The ac map cs code.</param>
        /// <returns></returns>
        public static IList<GemsPoint2d> ConvertToLatLong(IDataManager dm, IList<GemsPoint2d> sourcePoints, string acMapCsCode)
        {
            //_logger.Debug("Start ConvertToLatLong");
            IList<GemsPoint2d> list = new List<GemsPoint2d>();

            foreach (GemsPoint2d ptSource in sourcePoints)
            {
                GemsPoint2d pt = CoordinateService.TranslateStatePlaneToWGS84(dm, acMapCsCode, ptSource);
                list.Add(pt);
            }
            //_logger.Debug("Start ConvertToLatLong");
            return list;
        }

        /// <summary>
        /// Move a list of points by the offset
        /// </summary>
        /// <param name="offsetX">The offset X.</param>
        /// <param name="offsetY">The offset Y.</param>
        /// <param name="sourcePoints">The source points.</param>
        /// <returns></returns>
        public static IList<GemsPoint2d> MovePolygon(double offsetX, double offsetY, IList<GemsPoint2d> sourcePoints)
        {
            //_logger.Debug("Start MovePolygon");
            IList<GemsPoint2d> movedPoints = new List<GemsPoint2d>();
            try
            {
                foreach (GemsPoint2d ptSource in sourcePoints)
                {
                    GemsPoint2d pt = new GemsPoint2d(ptSource.X + offsetX, ptSource.Y + offsetY);
                    movedPoints.Add(pt);
                }
            }
            catch(System.Exception ex)
            {
                //_logger.Error("Error in MovePolygon", ex);
                throw;
            }
            //_logger.Debug("Start MovePolygon");
            return movedPoints;
        }

        /// <summary>
        /// Scale a list of points
        /// </summary>
        /// <param name="basePoint">The base point.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="sourcePoints">The source points.</param>
        /// <returns></returns>
        public static IList<GemsPoint2d> ScalePolygon(GemsPoint2d basePoint, double scaleFactor, IList<GemsPoint2d> sourcePoints)
        {
            //_logger.Debug("Start ScalePolygon");
            IList<GemsPoint2d> scaledPoints = new List<GemsPoint2d>();
            try
            {
                foreach (GemsPoint2d ptSource in sourcePoints)
                {
                    double deltaX = ptSource.X - basePoint.X;
                    double deltaY = ptSource.Y - basePoint.Y;
                    deltaX *= scaleFactor;
                    deltaY *= scaleFactor;
                    GemsPoint2d pt = new GemsPoint2d(basePoint.X + deltaX, basePoint.Y + deltaY);
                    scaledPoints.Add(pt);
                }
            }
            catch (System.Exception ex)
            {
                //_logger.Error("Error in ScalePolygon", ex);
                throw;
            }
            //_logger.Debug("Start ScalePolygon");
            return scaledPoints;
        }
    
    }
}
