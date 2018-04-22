using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using global::Autodesk.AutoCAD.Runtime;
using global::Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using COMS = PGA.MessengerManager;
using Acaddb =global::Autodesk.AutoCAD.DatabaseServices;
using Application = global::Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = System.Exception;

namespace PGA.FeaturelineManager
{
    public static class FeatureLineManager
    {
        const double MaxDiff = 100.0;
        const double MaxWater = 0.5;

        public static List<Acaddb.ObjectId> GetFeatureLines()
        {
            List<Acaddb.ObjectId> ids = null;

            try
            {
                var db = Active.Database;

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (Acaddb.BlockTable) tran.GetObject(db.BlockTableId, Acaddb.OpenMode.ForRead);

                    var br =
                        (Acaddb.BlockTableRecord)
                        tran.GetObject(tbl[Acaddb.BlockTableRecord.ModelSpace], Acaddb.OpenMode.ForRead);

                    var b = br.Cast<Acaddb.ObjectId>();

                    #region Other Types

                    //==============search certain entity========================//

                    //"LINE" for line

                    //"LWPOLYLINE" for polyline

                    //"CIRCLE" for circle

                    //"INSERT" for block reference

                    //...

                    //We can use "||" (or) to search for more then one entity types

                    //============================================================//


                    //Use lambda extension method

                    //ids = b.Where(id => id.ObjectClass.DxfName.ToUpper() == "LINE" ||

                    //    id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE").ToList<ObjectId>();

                    #endregion

                    ids = (from id in b
                        where id.ObjectClass.DxfName.ToUpper() == "AECC_FEATURE_LINE"
                        select id).ToList();


                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }

        [CommandMethod("PGA-FEATURELINECLEANUP")]
        public static void RecurseElevFromSurface()
        {
            try
            {

                var surfaceId = Acaddb.ObjectId.Null;
                var siteId = Acaddb.ObjectId.Null;
                //var ElevDiff = 0.0;
                var mean = 0.0;

                var featurelines = GetFeatureLines();

                COMS.MessengerManager.AddLog("Start RecurseElevFromSurface");


                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    foreach (var Id in featurelines)
                    {
                        using (var tr = Active.StartTransaction())

                        {
                            var fId = Id.GetObject(Acaddb.OpenMode.ForRead) as FeatureLine;

                            if (fId != null)
                            {
                                try
                                {
                                    mean = GetAverageElev(fId);
                                }
                                catch
                                {
                                }

                                if (mean <= 0)
                                    goto Skip;

                                if (ZeroElevation(fId))
                                {
                                    ApplyElevationCorrection(fId, mean, fId.MaxElevation);
                                }

                                if (fId.Layer.Contains("S-WATER"))
                                {
                                    //CorrectWaterFeaturelines(fId, mean);
                                }

                                SendMessage(fId);

                            }

                            Skip:

                            tr.Commit();
                        }
                    }

                    COMS.MessengerManager.AddLog(" End RecurseElevFromSurface");

                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public static void CheckElevFromFeatureLine(FeatureLine fl)
        {
            try
            {

                var mean = 0.0;

                if (fl == null) throw new ArgumentNullException(nameof(fl));


                COMS.MessengerManager.AddLog("Start CheckElevFromFeatureLine");

                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {

                    using (var tr = Active.StartTransaction())

                    {

                        var feature = (FeatureLine) fl.ObjectId.GetObject(Acaddb.OpenMode.ForRead);

                        if (!feature.IsWriteEnabled)
                            feature.UpgradeOpen();


                        if (feature != null)
                        {
                            SendMessage(feature);

                            try
                            {
                                mean = GetAverageElev(feature);
                            }
                            catch
                            {
                            }

                            if (Math.Abs(mean) <= 0.01)
                                goto Skip;

                            if (ZeroElevation(feature))
                            {
                                ApplyElevationCorrection(feature, mean, feature.MaxElevation);
                            }

                            if (feature.Layer.Contains("S-WATER"))
                            {
                                // CorrectWaterFeaturelines(fl, mean);
                            }

                            SendMessage(feature);

                        }

                        Skip:

                        tr.Commit();
                    }

                    COMS.MessengerManager.AddLog("End CheckElevFromFeatureLine");

                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }


        private static bool ZeroElevation(FeatureLine fl)
        {
            try
            {
                using (var tr = Active.StartTransaction())
                {

                    Point3dCollection pointsOnFL =
                        fl.GetPoints(FeatureLinePointType.AllPoints);

                    for (int i = 0; i < pointsOnFL.Count; i++)
                    {
                        if (Math.Abs(pointsOnFL[i].Z) < 0.01)
                            return true;
                    }

                    tr.Commit();
                }
            }
            catch (Exception ex)

            {
                MessengerManager.MessengerManager.LogException(ex);
            }

            return false;
        }

        private static void ApplyElevationCorrection(FeatureLine fl, double mean, double flMaxElevation)
        {

            try
            {
                if (mean <= 0) throw new ArgumentOutOfRangeException(nameof(mean));


                fl.UpgradeOpen();
                COMS.MessengerManager.AddLog("Start ApplyElevationCorrection");

                using (var tr = Active.StartTransaction())
                {
                    Point3dCollection pointsOnFL =
                        fl.GetPoints(FeatureLinePointType.AllPoints);

                    for (int i = 0; i < pointsOnFL.Count; i++)
                    {
                        Point3d p = pointsOnFL[i];


                        var diff = Math.Abs(flMaxElevation - p.Z);
                        if (diff > MaxDiff)
                        {
                            try
                            {
                                fl.SetPointElevation(i, mean);
                                COMS.MessengerManager.AddLog
                                ("Changed Feature Line: " +
                                 fl.Name + " Old Elevation=" +
                                 p.Z + " New Elevation=" +
                                 mean);
                            }
                            catch (Exception ex)
                            {
                                COMS.MessengerManager.LogException(ex);
                            }
                        }
                    }

                    tr.Commit();
                }

                COMS.MessengerManager.AddLog("End ApplyElevationCorrection");

            }
            catch (Exception ex)

            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public static void SendMessage(FeatureLine fl)
        {
            try
            {
                var _site = fl.SiteId;
                var _lay = fl.Layer;
                var _name = fl.Name;
                var max = fl.MaxElevation;
                var min = fl.MinElevation;
                var maxg = fl.MaxGrade;
                var sitename = "";

                var site = _site.GetObject(Acaddb.OpenMode.ForRead) as Site;
                if (site != null)
                    sitename = site.Name;
                else
                    sitename = "No-Id";

                WriteOutput(_lay, _name, max, min, maxg, sitename);
            }
            catch (Exception ex)

            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private static void WriteOutput(string _lay, string _name, double max, double min, double maxg, string sitename)
        {
            COMS.MessengerManager.AddLog(string.Format
            ("Feature Line Details: Site= " +
             "{0}, Layer= " +
             "{1}, Name= " +
             "{2}, Ele-Max=" +
             "{3}, Ele-Min=" +
             "{4}, MaxGrade=" +
             "{5}",
                sitename, _lay, _name, max, min, maxg));
        }

        private static double ElevationDifference(FeatureLine fl)
        {

            return (Math.Abs(fl.MaxElevation - fl.MinElevation));
        }

        public static double GetAverageElev(FeatureLine fl)
        {

            var nodes = 0;
            var mean = 0.0;
            //var total = 0.0;
            var median = 0.0;
            var max = 0.0;
            var min = 0.0;
            try
            {

                var vals = GetZValues(fl);

                if (vals == null)
                    return 0.0;

                nodes = GetCount(vals);

                if (nodes == 0)
                    return 0.0;
                try
                {
                    MessengerManager.MessengerManager.AddLog("Mean Before: " + mean);

                    mean = MathNet.Numerics.Statistics.Statistics.GeometricMean(vals);

                    MessengerManager.MessengerManager.AddLog("Mean After: " + mean);


                    if (Math.Abs(mean) < 0.01)
                    {
                        median = MathNet.Numerics.Statistics.Statistics.Median(vals);
                        min = MathNet.Numerics.Statistics.Statistics.Minimum(vals);
                        max = MathNet.Numerics.Statistics.Statistics.Maximum(vals);

                        if (Math.Abs(median) > 0.01)
                            return median;

                    }
                    else
                        return mean;
                }
                catch
                {
                }
            }
            catch (Exception ex)

            {
                MessengerManager.MessengerManager.LogException(ex);
            }

            return 0.0;
        }

        public static double[] GetZValues(FeatureLine fl)
        {
            try
            {
                Point3dCollection pointsOnFL =
                    fl.GetPoints(FeatureLinePointType.AllPoints);

                var mypoints = new List<double>();


                foreach (Point3d item in pointsOnFL)
                {
                    if (Math.Abs(item.Z) > 0.01)
                        mypoints.Add(item.Z);
                }


                return mypoints.ToArray();
            }
            catch (Exception ex)

            {
                MessengerManager.MessengerManager.LogException(ex);
            }

            return null;
        }

        public static int GetCount(double[] pnts)
        {
            try
            {
                return pnts.Length;
            }
            catch (Exception ex)

            {
                MessengerManager.MessengerManager.LogException(ex);
            }

            return 0;
        }

        /// <summary>
        /// Corrects the water featurelines.
        /// Has been omitted until further notice due to 25 swings in elevation for wier and structures
        /// </summary>
        /// <param name="fl">The fl.</param>
        /// <param name="mean">The mean.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static void CorrectWaterFeaturelines(FeatureLine fl, double mean)
        {

            try
            {
                if (mean <= 0) throw new ArgumentOutOfRangeException(nameof(mean));

                fl.UpgradeOpen();
                if (fl.Layer.Contains("S-WATER"))
                {
                    using (var tr = Active.StartTransaction())
                    {
                        Point3dCollection pointsOnFL =
                            fl.GetPoints(FeatureLinePointType.AllPoints);
                        COMS.MessengerManager.AddLog("Start CorrectWaterFeaturelines");

                        for (int i = 0; i < pointsOnFL.Count; i++)
                        {
                            Point3d p = pointsOnFL[i];
                            var last = pointsOnFL.Count - 1;
                            var diff = Math.Abs(p.Z - mean);

                            if ((diff > MaxWater) && (Math.Abs(mean) > 0.01))
                            {
                                COMS.MessengerManager.AddLog("S-Changed to Mean = " + mean);
                                try
                                {
                                    if (i != last)
                                        fl.SetPointElevation(i, mean);
                                }
                                catch (System.ArgumentOutOfRangeException)
                                {
                                }

                                catch (Exception ex)
                                {
                                    MessengerManager.MessengerManager.LogException(ex);
                                    MessengerManager.MessengerManager.AddLog("Failure = " + i + "and Mean = " + mean);

                                }

                                COMS.MessengerManager.AddLog("E-Changed to Mean = " + mean);

                            }
                        }

                        tr.Commit();
                    }
                }

                COMS.MessengerManager.AddLog("End CorrectWaterFeaturelines");
            }

            catch (Exception ex)

            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public static void SetNewPointElevation(FeatureLine fl,
            Point3d point, double elevationDelta)
        {
            // get all points on the Feature Line
            Point3dCollection pointsOnFL =
                fl.GetPoints(FeatureLinePointType.AllPoints);

            // find the closest point and index
            double distance = double.MaxValue;
            int index = 0;
            Point3d closestPointOnCurve = Point3d.Origin;
            for (int i = 0; i < pointsOnFL.Count; i++)
            {
                Point3d p = pointsOnFL[i];
                if (p.DistanceTo(point) < distance)
                {
                    distance = p.DistanceTo(point);
                    closestPointOnCurve = p;
                    index = i;
                }
            }

            // apply the delta
            fl.SetPointElevation(index,
                closestPointOnCurve.Z + elevationDelta);
        }

        public static void CheckElevFromFeatureLine(Acaddb.ObjectId featureId)
        {
            var mean = 0.0;

            using (Acaddb.Transaction tr = Active.StartTransaction())
            {
                var feature =
                    (Autodesk.Civil.DatabaseServices.FeatureLine) featureId.GetObject(Acaddb.OpenMode.ForRead);

                if (feature == null)
                    return;
                if (feature != null)
                {
                    SendMessage(feature);

                    try
                    {
                        mean = GetAverageElev(feature);
                    }
                    catch
                    {
                    }

                    if (Math.Abs(mean) <= 0.01)
                        goto Skip;

                    if (ZeroElevation(feature))
                    {
                        COMS.MessengerManager.AddLog("Starting Checking Zero Elevation > 20 Points");
                        if (feature.GetPoints(FeatureLinePointType.AllPoints).Cast<Point3d>().Count(p => Math.Abs(p.Z) < 0.001) < 20)
                        {
                            COMS.MessengerManager.AddLog("Starting Correcting Zero Elevation > 20 Points");
                            ApplyElevationCorrection(feature, mean, feature.MaxElevation);
                            COMS.MessengerManager.AddLog("Ending Correcting Zero Elevation > 20 Points");

                        }
                        COMS.MessengerManager.AddLog("Ending Checking Zero Elevation > 20 Points");

                    }

                    SendMessage(feature);

                }

                Skip:

                tr.Commit();
            }

            COMS.MessengerManager.AddLog("End CheckElevFromFeatureLine");

        }
    }  
}
