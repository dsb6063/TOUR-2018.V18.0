// ***********************************************************************
// Assembly         : PGA.Breaklines
// Author           : Daryl Banks
// Created          : 04-12-2016
//
// Last Modified By : Daryl Banks
// Last Modified On : 03-23-2018
// ***********************************************************************
// <copyright file="Commands.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.Settings;
using PGA.Database;
using Application = global::Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = System.Exception;

namespace PGA.Breaklines
{
    /// <summary>
    /// Class Commands.
    /// </summary>
    /// <seealso cref="Autodesk.AutoCAD.Runtime.IExtensionApplication" />
    public partial class Commands : IExtensionApplication
    {
        /// <summary>
        /// The _sur object identifier
        /// </summary>
        public static ObjectId _surObjectId = ObjectId.Null;
        /// <summary>
        /// The _site identifier
        /// </summary>
        public static ObjectId _siteId = ObjectId.Null;
        /// <summary>
        /// The _poly collection
        /// </summary>
        public static ObjectIdCollection _polyCollection;
        /// <summary>
        /// The _drawing name
        /// </summary>
        public static string _drawingName;
        /// <summary>
        /// The _ listof drawings
        /// </summary>
        public static IList<string> _ListofDrawings;
        /// <summary>
        /// The _time
        /// </summary>
        public static DateTime _time;
        /// <summary>
        /// The _directory
        /// </summary>
        private string _directory;
        /// <summary>
        /// The breaklines
        /// </summary>
        private readonly Breaklines breaklines;
        /// <summary>
        /// The select polylines
        /// </summary>
        private readonly SelectPolylines selectPolylines;
        /// <summary>
        /// The used3d collection
        /// </summary>
        private ObjectIdCollection used3dCollection;
        /// <summary>
        /// The used feature lines
        /// </summary>
        private readonly ObjectIdCollection usedFeatureLines;
        /// <summary>
        /// The used drawings
        /// </summary>
        private static List<string> usedDrawings;


        /// <summary>
        /// Initializes a new instance of the <see cref="Commands" /> class.
        /// </summary>
        public Commands()
        {
            try
            {
                using (var commands = new DatabaseCommands())
                {
                    #region Initialize Fields

                    _ListofDrawings = new List<string>();
                    usedDrawings = new List<string>();

                    #endregion

                    #region Linq Select SportVision DWGS

                    var dwgs = commands.GetAllExportToCadRecords();

                    var svdwg = from d in dwgs
                        where d.CompletePath.Contains("SportVision")
                        select d;

                    #endregion

                    #region Set Global Fields

                    _time = svdwg.Select(p => p.DateStamp.Value).FirstOrDefault();

                    _drawingName = svdwg.Select(p => p.CompletePath).FirstOrDefault();

                    _directory = Path.GetDirectoryName(_drawingName);

                    _ListofDrawings = svdwg.Select(p => p.CompletePath).ToList();

                    #endregion

                    #region Initialize Classes and Objects

                    breaklines = new Breaklines(_time, _ListofDrawings);
                    selectPolylines = new SelectPolylines();
                    usedFeatureLines = new ObjectIdCollection();
                    used3dCollection = new ObjectIdCollection();
                    usedDrawings = new List<string>();

                    #endregion
                }
            }
            catch (Exception)
            {
                MessengerManager.MessengerManager.AddLog
                    ("Initialize Open Drawing for Processing!");
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        public void Terminate()
        {
        }

        /// <summary>
        /// Registers the idle event.
        /// </summary>
        [CommandMethod("PGA-RegEvent001", CommandFlags.Session)]
        public void RegisterIdleEvent()
        {
            Application.Idle += OnIdle;
        }

        /// <summary>
        /// Uns the register idle event.
        /// </summary>
        [CommandMethod("PGA-UnRegEvent001", CommandFlags.Session)]
        public void UnRegisterIdleEvent()
        {
            Application.Idle -= OnIdle;
        }

        /// <summary>
        /// Adds the breaklines.
        /// </summary>
        public void AddBreaklines()
        {
            try
            {
                _polyCollection = new ObjectIdCollection();

                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId = breaklines.GetSiteId(surfaceId);
                var polycollection = selectPolylines.GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D");

                _siteId = siteId;
                _surObjectId = surfaceId;
                _polyCollection = polycollection;

                UpdatePolylineInfo(polycollection);

                foreach (ObjectId poly in polycollection)
                {
                    breaklines.AddCivil2016Breakline(surfaceId, poly, _siteId, null);
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Adds the breaklines by command line.
        /// </summary>
        public void AddBreaklinesByCommandLine()
        {
            try
            {
                PGA.MessengerManager.MessengerManager.AddLog("Starting AddBreaklinesByCommandLine!");
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    HideAmbientSettingsShowEvntVwer();
                    var surfaceId = breaklines.FindCivilTinSurface("All");
                    var poly3dCollection = new List<Polyline3d>();

                    _surObjectId = surfaceId;

                    foreach (ObjectId poly in _polyCollection)
                    {
                        try
                        {
                            if (used3dCollection.Contains(poly))
                                continue;

                            used3dCollection.Add(poly);

                            PGA.MessengerManager.MessengerManager.
                                AddLog("Looping AddBreaklinesByCommandLine! " + _polyCollection.IndexOf(poly));

                            if (_polyCollection.IndexOf(poly) == 60)
                                PGA.MessengerManager.MessengerManager.AddLog("Hit 60");


                            _siteId = breaklines.GetNewSiteId();

                            if (_siteId == ObjectId.Null)
                                continue;

                            using (Transaction tr = CivilApplicationManager.StartTransaction())
                            {
                                var featurelineId = breaklines.AddCivil2016BreaklineByTrans
                                    (surfaceId, poly, _siteId, null);

                                PGA.MessengerManager.MessengerManager.
                                    AddLog("Added a Breakline!");

                                if (featurelineId == ObjectId.Null)
                                    continue;

                                breaklines.AddCivil2016ElevationsToFeature
                                    (_surObjectId, featurelineId, _siteId, null);
                                PGA.MessengerManager.MessengerManager.
                                    AddLog("Assigned Elevations!");
                                tr.Commit();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            PGA.MessengerManager.MessengerManager.LogException(ex);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// New the drawing.
        /// </summary>
        [CommandMethod("PGA-NewDrawing", CommandFlags.Session)]
        public static void NewDrawing()
        {
            // Specify the template to use, if the template is not found
            // the default settings are used.
            var strTemplatePath = "acad.dwt";

            var acDocMgr = Application.DocumentManager;
            var acDoc = acDocMgr.Add(strTemplatePath);

            acDocMgr.MdiActiveDocument = acDoc;
            //AcadUtilities.AcadUtilities.SendStringToExecute("._PGA-RegEvent001\n");
            AcadUtilities.AcadUtilities.SendStringToExecute("._PGA-OpenDWG\n");
            AcadUtilities.AcadUtilities.SendStringToExecute("._PGA-AddBreaklines\n");
        }

        /// <summary>
        /// Opens the drawing.
        /// </summary>
        [CommandMethod("PGA-OpenDWG", CommandFlags.Session)]
        public static void OpenDrawing()
        {
            //var name = @"C:\Civil 3D Projects\PGA-SPORTSVISION\TINSURF-20160429-174844\SportVision\HOLE01\" +
            //          "SV-TPC-Louisiana-Clipped-For-SV01.DWG";
            var strFileName = GetDrawingNameFromDB();
            var acDocMgr = Application.DocumentManager;

            if (File.Exists(strFileName))
            {
                acDocMgr.Open(strFileName, false);
            }
            else
            {
                acDocMgr.MdiActiveDocument.Editor.WriteMessage("File " + strFileName +
                                                               " does not exist.");
            }
            SetInitialized();
        }

        /// <summary>
        /// Gets the first drawing name from database.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetFirstDrawingNameFromDB()
        {
            foreach (var dwg in _ListofDrawings)
            {
                if (usedDrawings.Contains(dwg))
                    continue;
                usedDrawings.Add(dwg);
                return dwg;
            }
            return "";
        }

        /// <summary>
        /// Gets the drawing name from database.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetDrawingNameFromDB()
        {
            //debug only
            usedDrawings.Clear();
            foreach (var dwg in _ListofDrawings)
            {
                if (usedDrawings.Contains(dwg))
                    continue;
                usedDrawings.Add(dwg);
                return dwg;
            }
            return "";
        }

        /// <summary>
        /// Opens the drawing for processing.
        /// </summary>
        public void OpenDrawingForProcessing()
        {
            var name = @"C:\Civil 3D Projects\PGA-SPORTSVISION\TINSURF-20160429-174844\SportVision\HOLE01\" +
                       "SV-TPC-Louisiana-Clipped-For-SV01.DWG";
            OpenDrawingForProcessing(name);


            //if (!IsLocked())
            //{
            //    EnableLock();
            //    //debug only
            //    //usedDrawings.Clear();
            //    foreach (var dwg in _ListofDrawings)
            //    {
            //        //if (usedDrawings.Contains(dwg))
            //        //    continue;
            //        //usedDrawings.Add(dwg);
            //        OpenDrawingForProcessing(dwg);
            //        goto End;
            //    }
            //    End:
            //    DisableLock();
            //}
            //else
            //{
            //   PGA.MessengerManager.MessengerManager.
            //        AddLog("Drawing is Locked!");
            //}
        }

        /// <summary>
        /// Opens the drawing for processing.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public static void OpenDrawingForProcessing(string filename)
        {
            var docMgr = Application.DocumentManager;
            if (File.Exists(filename))
            {
                using (var doc = docMgr.Open(filename, false))
                {
                    //using (doc.LockDocument())
                    //{
                    //    Application.DocumentManager.MdiActiveDocument = doc;
                    //    var db = doc.Database;
                    //    var ed = doc.Editor;
                    //}
                }
            }
        }

        /// <summary>
        /// Adds the break lines synch.
        /// </summary>
        [CommandMethod("PGA-AddBreaklines", CommandFlags.Session)]
        public void AddBreakLinesSynch()
        {
            try
            {
                _polyCollection = new ObjectIdCollection();
                used3dCollection = new ObjectIdCollection();

                if (_polyCollection.Count == 0)
                    _polyCollection = selectPolylines.
                        GetIdsByTypeTypeValue("POLYLINE",
                            "LWPOLYLINE", "POLYLINE2D");


                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId = breaklines.GetSiteId(surfaceId); //Use New 

                breaklines._originalPolys = _polyCollection;


                _siteId = siteId;
                _surObjectId = surfaceId;
                TotalDWGs = _polyCollection.Count;
                AddBreaklinesByCommandLine();
                _bLinesComplete = true;
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Tests the polylines for compliance.
        /// </summary>
        [CommandMethod("PGA-TestPolylines", CommandFlags.Session)]
        public void TestPolylinesForCompliance()
        {
            try
            {
                _polyCollection = new ObjectIdCollection();
                used3dCollection = new ObjectIdCollection();

                    _polyCollection = selectPolylines.
                        GetIdsByTypeTypeValue("POLYLINE",
                            "LWPOLYLINE", "POLYLINE2D");


                var surfaceId = breaklines.FindCivilTinSurface("All");

                foreach (ObjectId polyid in _polyCollection)
                {
                          //Test and Simplify to 0.1
                    if (breaklines.AddStandardBoundary(polyid, "", surfaceId))
                        breaklines.RemoveBoundaryOperations(surfaceId);
                    else
                        SimplifyPolylines.Commands.SimplifyPolylinesInterative(polyid);
                }              
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Adds the breaklines by command line v2.
        /// </summary>
        public void AddBreaklinesByCommandLineV2()
        {
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    EnableLock();

                    var poly3dCollection = new List<Polyline3d>();

                    foreach (ObjectId poly in _polyCollection)
                    {
                        if (used3dCollection.Contains(poly))
                            goto End;

                        used3dCollection.Add(poly);

                        var siteId = breaklines.GetNewSiteId();

                        _siteId = siteId;

                        var featurelineId = breaklines.AddCivil2016BreaklineByTrans
                            (_surObjectId, poly, _siteId, null);


                        if (featurelineId == ObjectId.Null)
                            continue;

                        breaklines.AddCivil2016ElevationsToFeature
                            (_surObjectId, featurelineId, _siteId, null);
                        break;
                    }
                    DisableLock();

                    End:
                    {
                        MessengerManager.MessengerManager.AddLog("Breaklines Completed!");
                        _bLinesComplete = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Adds the break lines synch.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="outdwg">The outdwg.</param>
        public void AddBreakLinesSynch(DateTime value, string outdwg)
        {
            try
            {
                _polyCollection = new ObjectIdCollection();
                used3dCollection = new ObjectIdCollection();

                    _polyCollection = selectPolylines.
                        GetIdsByTypeTypeValue("POLYLINE",
                            "LWPOLYLINE", "POLYLINE2D");

                var surfaceId = breaklines.FindCivilTinSurface("All");

                breaklines._originalPolys = _polyCollection;

                _siteId = ObjectId.Null;

                _surObjectId = surfaceId;
                TotalDWGs = _polyCollection.Count;

                AddBreaklinesByCommandLine();

            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Gets the surface entity i ds.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <returns>List&lt;ObjectId&gt;.</returns>
        public static List<ObjectId> GetSurfaceEntityIDs(Autodesk.AutoCAD.DatabaseServices.Database db)

        {
            List<ObjectId> ids = null;

            try
            {
                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (BlockTable) tran.GetObject(db.BlockTableId, OpenMode.ForRead);

                    var br =
                        (BlockTableRecord) tran.GetObject(tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                    var b = br.Cast<ObjectId>();

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
                MessengerManager.MessengerManager.LogException(ex);
            }

            return ids;
        }

        /// <summary>
        /// Adds the feature lines to all surface.
        /// </summary>
        [CommandMethod("PGA-AddFeaLinesToALLSurf")]
        public void AddFeatureLinesToALLSurface()
        {
            try
            {
                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId = breaklines.GetSiteId(surfaceId);

                _siteId = siteId;
                _surObjectId = surfaceId;

                var exfeaturelines = GetSurfaceEntityIDs(CivilApplicationManager.WorkingDatabase);

                //var poly3DCollection = new List<Polyline3d>();

                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    foreach (var featurelineId in exfeaturelines)
                    {
                        //if (usedFeatureLines.Contains(featurelineId))
                        //    continue;

                        //usedFeatureLines.Add(featurelineId);
                        breaklines.AddStandardBreakline(surfaceId, featurelineId, "");
                        //var poly3d = breaklines.AddCivil2016ElevationsToFeature
                        //    (surfaceId, featurelineId, siteId, null);

                        //poly3DCollection.Add(poly3d);
                        //poly3d.Dispose();

                        // break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Sets the ambient settings show evnt vwer.
        /// </summary>
        public void SetAmbientSettingsShowEvntVwer()
        {
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (var doc = CivilDocument.GetCivilDocument(CivilApplicationManager.WorkingDatabase))
                    {
                        var ambientSettings = doc.Settings.DrawingSettings.AmbientSettings;
                        ambientSettings.General.ShowEventViewer.Value = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Hides the ambient settings show evnt vwer.
        /// </summary>
        public void HideAmbientSettingsShowEvntVwer()
        {
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (var doc = CivilDocument.GetCivilDocument(CivilApplicationManager.WorkingDatabase))
                    {
                        var ambientSettings = doc.Settings.DrawingSettings.AmbientSettings;
                        ambientSettings.General.ShowEventViewer.Value = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Adds the feature line elev from surface.
        /// </summary>
        [CommandMethod("PGA-AddFeatureLinesElevBySurf")]
        public void AddFeatureLineElevFromSurface()
        {
            try
            {
                // HideAmbientSettingsShowEvntVwer();

                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId = breaklines.GetSiteId(surfaceId);

                _siteId = siteId;
                _surObjectId = surfaceId;

                var exfeaturelines = GetSurfaceEntityIDs(CivilApplicationManager.WorkingDatabase);

                var poly3DCollection = new List<Polyline3d>();

                //using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                //{
                foreach (var featurelineId in exfeaturelines)
                {
                    //return;
                    // usedFeatureLines.Clear();
                    if (usedFeatureLines.Contains(featurelineId))
                        return;

                    usedFeatureLines.Add(featurelineId);

                    //var poly3d = breaklines.AddCivil2016ElevationsToFeature
                    //    (surfaceId, featurelineId, siteId, null);

                    //if (Math.Abs(poly3d.StartPoint.Z) < 0.01)
                    //{
                    //    MessengerManager.MessengerManager.AddLog("Z-Value not Extracted! " + featurelineId);
                    //}

                    //poly3DCollection.Add(poly3d);
                    //poly3d.Dispose();
                    break;
                }
                //}
                // SetAmbientSettingsShowEvntVwer();
                //AcadUtilities.AcadUtilities.SendStringToExecute
                //    ("._PGA-AddFeatureLinesElevBySurf\n");
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Creates the featureline from CMND.
        /// </summary>
        [CommandMethod("PGA-CreateFeaturelineFromCmnd")]
        public void CreateFeaturelineFromCmnd()
        {
            try
            {
                Application.SetSystemVariable("PICKFIRST", 1);

                var _polyCollection = selectPolylines.
                    GetIdsByTypeTypeValue("POLYLINE",
                        "LWPOLYLINE", "POLYLINE2D");
                // Get the document
                //var doc = Application.DocumentManager.MdiActiveDocument;

                //// Get the editor to make the selection
                //Editor oEd = doc.Editor;
                //oEd.Command();
                AcadUtilities.AcadUtilities.SendStringToExecute
                    ("._CreateFeatureLines\n");

                AcadUtilities.AcadUtilities.SendStringToExecute
                    ("._PGA-UnSetPickFirst\n");
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Sets the pick first.
        /// </summary>
        [CommandMethod("PGA-UnSetPickFirst")]
        public void SetPickFirst()
        {
            Application.SetSystemVariable("PICKFIRST", 0);
        }

        /// <summary>
        /// Recurses the elev from surface.
        /// </summary>
        public void RecurseElevFromSurface()
        {
            try
            {
                HideAmbientSettingsShowEvntVwer();

                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId = breaklines.GetSiteId(surfaceId);

                _siteId = siteId;
                _surObjectId = surfaceId;

                var exfeaturelines = GetSurfaceEntityIDs(CivilApplicationManager.WorkingDatabase);

                var poly3DCollection = new List<Polyline3d>();

                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    foreach (var featurelineId in exfeaturelines)
                    {
                        using (var tr = CivilApplicationManager.StartTransaction())

                        {
                            //var poly3d = breaklines.AddCivil2016ElevationsToFeature
                            //    (surfaceId, featurelineId, siteId, null);

                            //if (Math.Abs(poly3d.StartPoint.Z) < 0.01)
                            //{
                            //    MessengerManager.MessengerManager.AddLog("Z-Value not Extracted! " + featurelineId);
                            //}
                            tr.Commit();
                        }
                    }
                    // break;
                }
                SetAmbientSettingsShowEvntVwer();
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Adds the poly3 d to side database.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <exception cref="ArgumentNullException">FilePath! - AddPOly3DToSideDB</exception>
        public void AddPoly3DToSideDB(string filepath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filepath))
                {
                    var filename = Path.GetFileName(filepath);
                    var dir = Path.GetDirectoryName(filepath);
                    var file = "SV-3DP-" + filename;
                    var output = Path.Combine(dir, file);
                    //filepath = filepath.Replace("SV", "SV-3DP");
                    _drawingName = output;
                    AddPoly3DToSideDB();
                }
                else
                  throw new ArgumentNullException("FilePath!","AddPOly3DToSideDB");
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Adds the poly3 d to side database.
        /// </summary>
        /// <exception cref="ArgumentNullException">Feature Lines</exception>
        [CommandMethod("PGA-AddPoly3DToSideDB")]
        public void AddPoly3DToSideDB()
        {
            try
            {
                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId = breaklines.GetSiteId(surfaceId);

                _siteId = siteId;
                _surObjectId = surfaceId;

                var exfeaturelines = GetSurfaceEntityIDs(CivilApplicationManager.WorkingDatabase);

                if(exfeaturelines==null)
                   throw new ArgumentNullException("Feature Lines");

                foreach (var featurelineId in exfeaturelines)
                {
                    if (usedFeatureLines.Contains(featurelineId))
                        continue;

                    usedFeatureLines.Add(featurelineId);
                    breaklines.OpenFeatureLineForSave(featurelineId);
                }
                breaklines.SaveDrawing(null);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Adds the feature lines to all surface.
        /// </summary>
        [CommandMethod("PGA-AddBreaklinesToAllSurface")]

        public void AddFeatureLinesToAllSurface()
        {
            try
            {
                // HideAmbientSettingsShowEvntVwer();

                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId = breaklines.GetSiteId(surfaceId);

                _siteId = siteId;
                _surObjectId = surfaceId;

                var exfeaturelines = GetSurfaceEntityIDs(CivilApplicationManager.WorkingDatabase);

                var featureCollection =  new ObjectIdCollection(exfeaturelines.ToArray());

                breaklines.AddFeatureLinesToAllSurface(featureCollection,surfaceId); 
            }

            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        // [CommandMethod("PGA-OpenDrawingForProcessing",CommandFlags.Session)]
        /// <summary>
        /// Initializes the timer v2.
        /// </summary>
        public void InitializeTimerV2()
        {
            try
            {
                //_time = time;
                //_drawingName = name;
                //_directory = Path.GetDirectoryName(name);

                //_polyCollection = new ObjectIdCollection();
                // Breaklines breaklines = new Breaklines(_time,_ListofDrawings);
                //SelectPolylines selectPolylines = new SelectPolylines();
                //CreateTINSurface surface = new CreateTINSurface();
                OpenDrawingForProcessing();
                //using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                //{

                //var surfaceId = breaklines.FindCivilTinSurface("All");
                //surface.SetSurfaceStyleToStandard(surfaceId);
                //var siteId = breaklines.GetSiteId(surfaceId);
                //var polycollection = selectPolylines.GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D");

                //_siteId = siteId;
                //_surObjectId = surfaceId;
                //_polyCollection = polycollection;


                ////breaklines.GetDrawingName(time, name);
                //AddBreaklinesByCommandLine();

                //}
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Updates the polyline information.
        /// </summary>
        /// <param name="polyIdCollection">The poly identifier collection.</param>
        /// <exception cref="ArgumentNullException">Polyline Info</exception>
        /// <exception cref="System.ArgumentNullException">Polyline Info</exception>
        public void UpdatePolylineInfo(ObjectIdCollection polyIdCollection)
        {
            var color = string.Empty;
            var layer = string.Empty;
            var area = 0.0;
            var length = string.Empty;
            var handle = string.Empty;
            var nodes = 0;
            var time = DateTime.Now;
            var oid = string.Empty;
            var dwgName = string.Empty;

            using (var commands = new DatabaseCommands())
            {
                try
                {
                    foreach (ObjectId item in polyIdCollection)
                    {
                        var polyinfo = item.GetObject(OpenMode.ForRead) as Polyline;

                        if (polyinfo == null)
                            throw new ArgumentNullException("Polyline Info");

                        time = _time;
                        color = polyinfo.Color.ToString();
                        layer = polyinfo.Layer;
                        area = polyinfo.Area;
                        length = polyinfo.Length.ToString();
                        handle = polyinfo.Handle.ToString();
                        nodes = polyinfo.NumberOfVertices;
                        dwgName = _drawingName;
                        oid = polyinfo.ObjectId.ToString();

                        commands.InsertPolylineInformation(time, color, layer, area, length, handle, nodes, oid, dwgName);
                    }
                }
                catch (Exception ex)
                {
                    MessengerManager.MessengerManager.LogException(ex);
                }
            }
        }
    }

    /// <summary>
    /// Class ExtensionMethods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public static string ToString(this string val)
        {
            var sb = new StringBuilder();
            try
            {
                sb.Append(val);
            }
            catch (Exception)
            {
            }
            return sb.ToString();
        }
    }
}

 
