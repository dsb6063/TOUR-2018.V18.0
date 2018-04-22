// ***********************************************************************
// Assembly         : PGA.DeleteDupSurfaces
// Author           : Daryl Banks, PSM
// Created          : 03-13-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 03-13-2016
// ***********************************************************************
// <copyright file="DeleteDupSurfaces.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using PGA.Database;
using PGA.MessengerManager;
using COMS=PGA.MessengerManager.MessengerManager;
using ACADUTIL=PGA.AcadUtilities.AcadUtilities;

namespace PGA.DeleteDupSurfaces
{
    /// <summary>
    /// Class DeleteDupSurfaces.
    /// </summary>
    [Obfuscation(Feature = "renaming", Exclude = true)]
    public static class DeleteDupSurfaces
    {

        /// <summary>
        /// Removes the duplicate surfaces by using Layer Rank.
        /// </summary>
        public static void RemoveDuplicateSurfaces_V1()
        {
            try
            {
                COMS.AddLog("Entering Remove Duplicate Surfaces V1!");
                DatabaseCommands commands = new DatabaseCommands();
                var excludedpolys = new List<Handle>();
                ObjectIdCollection polylines = null;
                //var features = commands.GetAllFeatures();
                #region Testing for Opening DWGs
                //var filename = OpenDwg.file;
                //Document doc = Application.DocumentManager.Open(filename, true);
                //Application.DocumentManager.MdiActiveDocument = doc;
                //var db = doc.Database;
                //var ed = doc.Editor; 
                #endregion

                Document doc = Application.DocumentManager.MdiActiveDocument;
                global::Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                using (doc.LockDocument())
                {

                    using (var trans = db.TransactionManager.StartTransaction())
                    {
                        PGA.SimplifyPolylines.Commands.SimplifyPolylinesTest();

                        //polylines = SelectPolylines.GetAllPolylines();
                        polylines = ACADUTIL.GetAllObjectIdsInModel(db, trans, false);

                        foreach (ObjectId master in polylines)
                        {
                            var masterArea = SelectPolylines.GetPolyLineArea(master, db);
                            foreach (ObjectId comparer in polylines)
                            {
                                //cannot be identical oids
                                if (master.Equals(comparer))
                                    continue;

                                //get areas of polys
                                var comparerArea = SelectPolylines.GetPolyLineArea(comparer, db);

                                System.Diagnostics.Debug.WriteLine(String.Format
                                    ("M={0},C={1}", masterArea, comparerArea));

                                if (comparerArea == null || masterArea == null)
                                    continue;

                                //Precision is set to < 0.5 Sq-ft Missed Features
                                //Precision is set to = 1.0 Sq-ft Works(Not Perfect) 

                                if (IsEqual(masterArea, comparerArea, 1.0))
                                {
                                    System.Diagnostics.Debug.WriteLine(String.Format("Entering Equals--> M={0},C={1}",
                                        masterArea, comparerArea));
                                    var masterLayer = SelectPolylines.GetPolyLineObject(master, db).Layer;
                                    var comparerLayer = SelectPolylines.GetPolyLineObject(comparer, db).Layer;

                                    //Get Rank and exclude secondary poly
                                    if (commands.GetFeatureRankByCode(masterLayer) >
                                        commands.GetFeatureRankByCode(comparerLayer))
                                    {
                                        //exclude comparer layer
                                        excludedpolys.Add(master.Handle);
                                        COMS.AddLog(String.Format(
                                            "Exclude Master by Layer Rank = CL={0},CA={1}, MA={2} ML={3} ",
                                            comparerLayer, comparerArea, masterArea, masterLayer));
                                    }
                                }
                            }
                        }
                        trans.Commit();
                    }
                    commands.ClearExcludedFeatures();

                    foreach (var id in excludedpolys)
                        commands.InsertToExcludedFeatures(id.ToString());
                }
                COMS.AddLog("Exiting Remove Duplicate Surfaces V1!");
            }
            catch (Exception ex)
            {

                COMS.LogException(ex);

            }

        }

        public static ObjectId GetObjectId(string handle)
        {
            ObjectId id = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            global::Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Handle h = new Handle(Int64.Parse
                    (handle, NumberStyles.AllowHexSpecifier));
                db.TryGetObjectId(h, out id);
                tr.Commit(); 
            }
            return id;
        }
        public static bool IsEqual( double? d1, double? d2, double precisionFactor)
        {
            if (d1 == null) throw new ArgumentNullException(nameof(d1));
            if (d2 == null) throw new ArgumentNullException(nameof(d2));

            double D1 = (double) d1;
            double D2 = (double) d2;

            return Math.Abs(D1 - D2) < precisionFactor ;
        }

        public static bool IsEqual(double d1, double d2, int precisionFactor)
        {
            return Math.Abs(d1 - d2) < precisionFactor * double.Epsilon;
        }
        /// <summary>
        /// Removes the duplicate surfaces by using additional filtering.
        /// looking for isolated polylines with no internal objects
        /// on same layer.
        /// </summary>
        public static void RemoveDuplicateSurfaces_V2()
        {
            try
            {
                COMS.AddLog("Entering Remove Duplicate Surfaces V2!");
                DatabaseCommands commands = new DatabaseCommands();
                // var filename = OpenDwg.file;
                ObjectIdCollection polylines = null;
                var excludedpolys = new List<Handle>();
                //var features = commands.GetAllFeatures();

                #region For Testing Open File
                //Document doc = OpenDwg.OpenDwgForWork(filename);
                //Application.DocumentManager.MdiActiveDocument = doc;
                //var db = doc.Database;
                //var ed = doc.Editor;

                #endregion

                Document doc = Application.DocumentManager.MdiActiveDocument;
                global::Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                using (doc.LockDocument())
                {
                    using (var trans = db.TransactionManager.StartTransaction())
                    {
                        polylines = ACADUTIL.GetAllObjectIdsInModel(db, trans, false);
                        ObjectId MaxORO = GetLargestAreaOfORO(polylines, db);
                        ObjectId MinOCO = GetSmallestAreaOfOCO(polylines, db);
                        ObjectId MinOIR = GetSmallestAreaOfOIR(polylines, db);

                        foreach (ObjectId master in polylines)
                        {
                            var masterpolyline = SelectPolylines.GetPolyLineObject(master, db);
                            var masterArea = SelectPolylines.GetPolyLineArea(master, db);

                            foreach (ObjectId comparer in polylines)
                            {
                                //cannot be identical oids

                                if (master.Equals(comparer))
                                    continue;

                                //get areas of polys

                                var comparerpolyline = SelectPolylines.GetPolyLineObject(comparer, db);
                                var comparerArea = SelectPolylines.GetPolyLineArea(comparer, db);

                                System.Diagnostics.Debug.WriteLine(String.Format("M={0},C={1}", masterArea, comparerArea));

                                if (comparerArea == null || masterArea == null)
                                    continue;

                                //Get Internal Polylines to Interate

                                var IsInternal = PointInPolyline(ACADUTIL.GetPointsFromPolyline(masterpolyline),
                                                                 ACADUTIL.GetPointsFromPolyline(comparerpolyline));

                                if ((masterArea > comparerArea) && IsInternal)
                                {
                                    System.Diagnostics.Debug.WriteLine(String.Format
                                     ("We are Internal! M={0},C={1}", masterArea, comparerArea));

                                    // Does master have any internals on same layer
                                    // We are looking for isolated polylines with no internal objects
                                    // on same layer.

                                    var masterLayer = SelectPolylines.GetPolyLineObject(master, db).Layer;
                                    var comparerLayer = SelectPolylines.GetPolyLineObject(comparer, db).Layer;

                                    //filter out largest ORO polyline
                                    if (masterLayer == "ORO" && master.Equals(MaxORO))
                                        continue;
                                    if (masterLayer == "OCO" && master.Equals(MinOCO))
                                    {
                                        excludedpolys.Add(master.Handle);
                                        continue;
                                    }
                                    #region Removed-5.14.16
                                    //if (masterLayer == "OIR" && master.Equals(MinOIR))
                                    //{
                                    //    excludedpolys.Add(master.Handle);
                                    //    continue;
                                    //} 
                                    #endregion
                                    //continue searching if has internal with same layer
                                    #region Exclude matching layers

                                    if (masterLayer.Equals(comparerLayer))
                                    {
                                        continue;
                                    }

                                    #endregion
                                    #region Add Additional Rank Filtering
                                    //If Green and Fairway, then exclude it!
                                    int Green = 1;
                                    int Fairway = 3;
                                    int IntRough = 21;

                                    //Get Rank and exclude secondary poly
                                    int masterRank = commands.GetFeatureRankByCode(masterLayer);
                                    int comparRank = commands.GetFeatureRankByCode(comparerLayer);
                                    COMS.AddLog(String.Format(
                                          "Debugging = ML={0},CL={1}, MR={2} CR={3} ",
                                          masterLayer, comparerLayer, masterRank, comparRank));

                                    if (masterRank == IntRough)
                                        continue;

                                    if ((masterRank > comparRank) &&
                                                (masterRank > 10) &&
                                              (comparRank == Green ||
                                               comparRank == Fairway))

                                    {
                                        //exclude master layer
                                        excludedpolys.Add(master.Handle);
                                        COMS.AddLog(String.Format(
                                            "Exclude Comparer by Layer Filtering = {0},{1}, MA={2} ML={3} ",
                                            comparerLayer, comparerArea, masterArea, masterLayer));
                                    }

                                    #endregion
                                }
                            }
                        }
                        trans.Commit();
                    }
                    foreach (var id in excludedpolys)
                        commands.InsertToExcludedFeatures(id.ToString());
                }
                COMS.AddLog("ExitingRemove Duplicate Surfaces V2!");
            }
            catch (Exception ex)
            {
                COMS.LogException(ex);
            }

        }

        public static ObjectId GetLargestAreaOfORO(ObjectIdCollection polylines,global::Autodesk.AutoCAD.DatabaseServices.Database db)
        {
            double MaxArea = 0;
            ObjectId pid = ObjectId.Null;
            foreach (ObjectId master in polylines)
            {
                var masterLayer = SelectPolylines.GetPolyLineObject(master, db).Layer;
                var masterArea = SelectPolylines.GetPolyLineArea(master, db);

                if ((double) masterArea == 0) continue;
                 
                if (masterLayer.Equals("ORO"))
                {
                    if (masterArea > MaxArea)
                    {
                        MaxArea = (double) masterArea;
                        pid = master;
                    }
                }
            }
            return pid;
        }

        public static ObjectId GetSmallestAreaOfOIR(ObjectIdCollection polylines, global::Autodesk.AutoCAD.DatabaseServices.Database db)
        {
            double MaxArea = 100000;
            ObjectId pid = ObjectId.Null;
            foreach (ObjectId master in polylines)
            {
                var masterLayer = SelectPolylines.GetPolyLineObject(master, db).Layer;
                var masterArea = SelectPolylines.GetPolyLineArea(master, db);

                if ((double)masterArea == 0) continue;

                if (masterLayer.Equals("OIR"))
                {
                    if (masterArea < MaxArea)
                    {
                        MaxArea = (double)masterArea;
                        pid = master;
                    }
                }
            }
            return pid;
        }

        public static ObjectId GetSmallestAreaOfOCO(ObjectIdCollection polylines, global::Autodesk.AutoCAD.DatabaseServices.Database db)
        {
            double MaxArea = 1000000;
            ObjectId pid = ObjectId.Null;
            foreach (ObjectId master in polylines)
            {
                var masterLayer = SelectPolylines.GetPolyLineObject(master, db).Layer;
                var masterArea = SelectPolylines.GetPolyLineArea(master, db);

                if ((double)masterArea == 0) continue;

                if (masterLayer.Equals("OCO"))
                {
                    if (masterArea < MaxArea)
                    {
                        MaxArea = (double)masterArea;
                        pid = master;
                    }
                }
            }
            return pid;
        }

        public static bool PointInPolyline(Point2dCollection points, Point2dCollection tests)
        {
            int nvert = points.Count;
            int tvert = tests.Count;
            Point2d test = new Point2d();

            int i, j, m, n;
            bool c = false;
            for (m = 0, j = tvert - 1; m < tvert; n = m++)
            {
                test = tests[m];

                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {

                    if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                        (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                        c = !c;
                }

                if (c) return c; //break if point is inside poly
            }

            return c;
        }

    }
}
