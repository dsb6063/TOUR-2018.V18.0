// ***********************************************************************
// Assembly         : PGA.CivTinSurf
// Author           : Daryl Banks, PSM
// Created          : 05-19-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 03-13-2017
// ***********************************************************************
// <copyright file="Coalesce.cs" company="Banks and Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Interop;
using BBC.Common.Active;
using global::Autodesk.AutoCAD.ApplicationServices;
using C3DSurfacesDemo;
using ACAD    = global::Autodesk.AutoCAD.ApplicationServices;
using ACADRT  = global::Autodesk.AutoCAD.Runtime;
using ACADDB  = global::Autodesk.AutoCAD.DatabaseServices;
using PGA.Database;
using PGA.DataContext;
using Process = ProcessPolylines.ProcessPolylines;
using AssignNames = AssignPolylineLayers.Commands;
using CreateAllSurface = CreateTINSurfaceFromCloud;
using DUPS = PGA.DeleteDupSurfaces.DeleteDupSurfaces;
using PGA.Autodesk.Utils;
using PGA.CourseCoaleseProject;
using PGA.Surfaces;
using PGA.DrawingManager;
using static PGA.PlotManager.PlotCommands;
using Utils = Pge.Common.Framework;
using COMS = PGA.MessengerManager;
[assembly: Obfuscation(Feature = "encryptmethod", Exclude = true)]
namespace PGA.CourseCoalesceProject
{
    /// <summary>
    /// Class Coalesce.
    /// </summary>
    [Obfuscation(Feature="encryptmethod", Exclude=true)]
    public static class Coalesce
    {
        /// <summary>
        /// The numberof polylines
        /// </summary>
        public static int NumberofPolylines ;
        /// <summary>
        /// The flag automatic error reports
        /// </summary>
        private static bool _flagAutoErrorReports;
        /// <summary>
        /// Gets or sets the drawingpath.
        /// </summary>
        /// <value>The drawingpath.</value>
        public static string Drawingpath { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Coalesce"/> is flag.
        /// </summary>
        /// <value><c>true</c> if flag; otherwise, <c>false</c>.</value>
        public static bool Flag { get; set; }
        /// <summary>
        /// Gets or sets the hole.
        /// </summary>
        /// <value>The hole.</value>
        public static int Hole { get; set; }
        /// <summary>
        /// Gets or sets the date stamp.
        /// </summary>
        /// <value>The date stamp.</value>
        public static DateTime DateStamp { set; get; }
        /// <summary>
        /// Gets or sets the total drawings.
        /// </summary>
        /// <value>The total drawings.</value>
        public static int TotalDrawings { set; get; }

        /// <summary>
        /// Loadands the process polys. See PGA-TOUR-Assembly for Command

        [ACADRT.CommandMethod("PGA-STARTCOALESCING", ACADRT.CommandFlags.Session)]
        public static void LoadandProcessPolys()
        {
           try
           {
               SetQNewTemplate();

               COMS.MessengerManager.AddLog(GetOsVersion());

               using (DatabaseCommands commands = new DatabaseCommands())
               {
                    #region Initialize Progress Bar
                    try
                    {
                        TotalDrawings = 0;

                        commands.DeleteTimerInfo();
                        commands.CheckForTimerRecs();
                        commands.ClearTimerInfo();
                    }
                    catch { } 
                    #endregion

                    #region Template Path and Settings

                    var TemplatePath = commands.GetTemplatePath();

                   OutputDllVersions(commands.GetTemplatePath());
                   #endregion

                   #region Get Dwgs to Process

                   var dwgs = commands.LoadandProcessPolys();

                   if (dwgs == null || dwgs.Count == 0)
                   {

                        COMS.MessengerManager.ShowMessageAlert
                            ("Error: Unable to Retrieve Drawings!");

                        throw new ArgumentNullException
                            ("Unable to Retrieve Drawings! Clear Database!");
                   }

                   IList<DrawingStack> DwDrawingStacks = new List<DrawingStack>();
                   foreach (var dwg in dwgs)
                   {
                       if (DwDrawingStacks != null) DwDrawingStacks.Add(dwg);
                   }

                   #endregion

                   foreach (DrawingStack dwg in DwDrawingStacks)
                   {
                        COMS.MessengerManager.AddLog("Start Checking DateStamp!");

                        #region Check DateStamp

                       DateStamp = (DateTime) CheckDateStamp(dwg.DateStamp);
                      
                       COMS.MessengerManager.AddLog("End Checking DateStamp!");

                        #endregion

                        #region Initialize Fields

                        ProjectSettings s = new ProjectSettings(DateStamp);
                       GDwgPath = commands.GetGlobalDWGPath(DateStamp);
                       GCloudPath = commands.GetGlobalPointCloudPath(DateStamp);
                       ACAD.Application.SetSystemVariable("FILEDIA", 0);

                       #endregion

                       #region Check and Set DWG Start Flag DWG Stack

                       if (commands.IsDrawingStackStartedFlagSet(dwg))
                       {
                           continue;
                       }
                       else
                           commands.SetDrawingStackStartedFlag(dwg);

                       #endregion

                       #region Drawing Check Max

                       if (++TotalDrawings > 18)
                           throw new System.Exception
                               ("Clear Database! Drawing Stack Not Synchronized! Drawings exceed number of 18 Max!");

                        #endregion

                        #region Notifications
                        COMS.MessengerManager.AddLog("Starting Notifications!");

                        if (TotalDrawings == 1)
                           commands.UpdateTotalDWGSTimerInfo(DwDrawingStacks.Count());

                        commands.SetS1TimerInfo(TotalDrawings);

                        commands.InsertNotifications
                           (String.Format("{0},{1},{2}",
                               1, TotalDrawings, DwDrawingStacks.Count
                               ));
                        COMS.MessengerManager.AddLog("Ending Notifications!");

                        #endregion

                        #region Document Manager Work

                        var acDocMgr = ACAD.Application.DocumentManager;
                       Document acNewDoc = null;
                       Document acDoc = ACAD.Application.DocumentManager.MdiActiveDocument;

                       if (acDocMgr.Count == 1)
                       {

                           acNewDoc = acDocMgr.Add(TemplatePath);
                           using (acDocMgr.MdiActiveDocument.LockDocument())
                           {
                               acDocMgr.MdiActiveDocument = acDoc;
                               acDocMgr.CurrentDocument.CloseAndDiscard();
                               acDocMgr.MdiActiveDocument = acNewDoc;

                           }
                       }
                       COMS.MessengerManager.AddLog
                           ("Begin Process Drawing: " + dwg.PolylineDwgName);

                       #endregion

                       using (acDocMgr.MdiActiveDocument.LockDocument())
                       {
                           if (acDoc != null)

                           {
                               #region NewDocDb

                               ACADDB.Database acDbNewDoc = acNewDoc.Database;
                               var ed = acNewDoc.Editor;

                               #endregion

                               #region Check Global Path

                               string gpath = Convert.ToString(GDwgPath);

                               #endregion

                               if (gpath != null)
                               {
                                   #region Path Checks

                                   string path = Path.Combine(gpath, dwg.PolylineDwgName);

                                   if (path == null) throw new ArgumentNullException(nameof(path));


                                   if (!File.Exists(path))
                                   {
                                       COMS.MessengerManager.ShowMessageAlert
                                           ((String.Format("File Not Found: {0}", path)));

                                       return;
                                   }

                                   #endregion

                                   try
                                   {
                                       using (ACADDB.Database db = new ACADDB.Database(false, true))
                                       {
                                           #region Read Dwg File

                                           // Read the DWG file into our Database object

                                           db.ReadDwgFile(
                                               path,
                                               ACADDB.FileOpenMode.OpenForReadAndReadShare,
                                               false,
                                               ""
                                               );

                                           #endregion

                                           #region Set DWG Running Flag

                                           commands.SetDrawingStackRunningFlag(dwg);

                                            #endregion

                                           #region Copy Polylines to Current dwg
                                            COMS.MessengerManager.AddLog("Starting Polyline Copy!");

                                            db.RetainOriginalThumbnailBitmap = true;

                                           // We'll store the current working database, to reset
                                           // after the purge operation

                                           var wdb = ACADDB.HostApplicationServices.WorkingDatabase;
                                           ACADDB.HostApplicationServices.WorkingDatabase = db;

                                           // Purge unused DGN linestyles from the drawing
                                           // (returns false if nothing is erased)
                                           ACADDB.ObjectIdCollection collection = Process.GetIdsByTypeTypeValue(
                                               "POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");
                                           NumberofPolylines = collection.Count;
                                           Process.CopyPolylinesBetweenDatabases(db, collection);


                                           // Still need to reset the working database

                                            ACADDB.HostApplicationServices.WorkingDatabase = wdb;
                                            COMS.MessengerManager.AddLog("Ending Polyline Copy!");

                                            #endregion

                                            #region Close All Polylines

                                           ClosePolylines();
                                            #endregion

                                            #region Cases to Remove Duplicate Surfaces

                                           if (s.UseRemoveDups)
                                           {
                                               DUPS.RemoveDuplicateSurfaces_V1();
                                               DUPS.RemoveDuplicateSurfaces_V2();
                                           }

                                           #endregion

                                           #region Messaging and System Variables
                                            ACAD.Application.SetSystemVariable("PROXYSHOW", 0);
                                            ACAD.Application.SetSystemVariable("PROXYNOTICE",   0);
                                            ACAD.Application.SetSystemVariable("PROXYGRAPHICS", 0);

                                           COMS.MessengerManager.AddLog("Processing Duplicates!");

                                           COMS.MessengerManager.AddLog(String.Format
                                               ("Starting Drawing: {0}", dwg.PolylineDwgName));

                                           COMS.MessengerManager.AddLog(String.Format
                                               ("Set Date Started in Task: {0}", dwg.PolylineDwgName));

                                            #endregion

                                            #region Selection of Polylines
                                            COMS.MessengerManager.AddLog("Starting Polyline Selection!");

                                            collection = null;
                                           // Reselect Polylines in the current database
                                           collection = Process.GetIdsByTypeTypeValue(
                                               "POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");
                                            COMS.MessengerManager.AddLog("Ending Polyline  Selection!");

                                            #endregion

                                            #region Process Layers & Handle Output Folders
                                            COMS.MessengerManager.AddLog("Starting Output Folders!");

                                            string output = "";
                                           if (s.UseBreaklines && !s.UseSportVision)
                                               output = SetOutPutFolder(dwg, commands);
                                           else if (s.UseBreaklines && s.UseSportVision)
                                               output = SetSportVisionFolder(dwg, commands);
                                           else
                                               output = SetOutPutFolder(dwg, commands);
                                           string fileName =
                                               System.IO.Path.GetFileNameWithoutExtension(dwg.PolylineDwgName);
                                           string outdxf = Path.Combine(output,
                                               String.Format("{0}{1}", fileName, ".dxf"));
                                           string outdwg = Path.Combine(output, dwg.PolylineDwgName);
                                           PLineToLayers.ProcessLayers(collection, wdb);
                                           Hole = (int) dwg.Hole;
                                           Drawingpath = outdwg;
                                            COMS.MessengerManager.AddLog("Ending Output Folders!");


                                            #endregion

                                            #region Zoom Extents
                                            ZoomExtents();

                                            #endregion


                                            #region Simplify Polylines

                                            if (s.UseSimplify)
                                           {
                                               COMS.MessengerManager.AddLog("Started: Simplify Polylines => Custom Value = " + s.SimplifyTolerance);
                                               PGA.SimplifyPolylines.Commands.SimplifyPolylinesInterative(s.SimplifyTolerance);
                                               COMS.MessengerManager.AddLog("Finished: Simplify Polylines => Custom Value = " + s.SimplifyTolerance);

                                           }

                                            #endregion

                                            #region Offset Water Features
                                            PGA.OffsetPolylines.Program.ScaleObject("OWA");
                                            PGA.OffsetPolylines.Program.ScaleObject("S-WATER");
                                            #endregion
                                            #region Offset Selected Features
                                            PGA.OffsetPolylines.Program.ScaleObject("ODO");
                                            PGA.OffsetPolylines.Program.ScaleObject("S-DIRT-OUTLINE");
                                            PGA.OffsetPolylines.Program.ScaleObject("OTB");
                                            PGA.OffsetPolylines.Program.ScaleObject("S-TEE-BOX");
                                            #endregion

                                            #region Change Layer Names

                                            AssignNames.ChangeLayers();

                                            #endregion

                                            #region Selection of Polylines
                                            COMS.MessengerManager.AddLog("Starting Polyline Selection!");

                                            collection = null;
                                            // Reselect Polylines in the current database
                                            collection = Process.GetIdsByTypeTypeValue(
                                                "POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3D");
                                            COMS.MessengerManager.AddLog("Ending Polyline  Selection!");

                                            #endregion

                                            #region Create ALL Surface

                                            //Create Point Cloud Surface "All"
                                            CreateAllSurface.CreateTINSurface pTinSurface =
                                               new CreateAllSurface.CreateTINSurface();                                         

                                           pTinSurface.CreateTINSurfaceByPointCollection(GCloudPath,
                                               dwg.PointCloudDwgName);
                                           
                                          
                                           SavezCADFile(outdwg, acDbNewDoc, dwg);


                                           #endregion

                                           #region Init Class and Start Probing

                                           PGA.Breaklines.Commands _commands = new
                                               PGA.Breaklines.Commands(dwg.DateStamp.Value, outdwg);

                                            _commands.HideAmbientSettingsShowEvntVwer();

                                           if (s.UseProbing)
                                           {
                                               _commands.TestPolylinesForCompliance();
                                           }

                                           #endregion

                                           #region Add Breaklines and Refine

                                           if (s.UseBreaklines)
                                           {

                                             
                                                //Add Breaklines to "All"
                                                _commands.AddBreakLinesSynch
                                                   (dwg.DateStamp.Value, outdwg);

                                                
                                            }

                                           #endregion

                                           #region Create Sub-Surfaces

                                           //Create all minor surfaces

                                           PGA.Surfaces.C3DSurfacesDemoCommands civilcommand =
                                               new C3DSurfacesDemoCommands();
                                           civilcommand.CreateAllSurfaces();

                                           #region Hide "All" Surface

                                           try
                                           {
                                               ACADDB.ObjectId surfaceId = PasteSurfaces.GetSurfaceByName("All");
                                               pTinSurface.SetSurfaceStyleToHidden(surfaceId);
                                           }
                                           catch (ACADRT.Exception ex)
                                           {
                                               #region Messaging

                                               COMS.MessengerManager.AddLog(String.Format
                                                   ("Hide Surface Style: {0}", ex.Message));
                                               COMS.MessengerManager.LogException(ex);

                                               #endregion
                                           }

                                            #endregion
                                            #region Smoothing Operations
                                            pTinSurface.SmoothAllSurfaces();
                                            #endregion
                                            #region Set Small Triangle Build Operations
                                            pTinSurface.SetSmallTrianglesAllSurfaces();
                                            #endregion
                                            #region Check for Zero Elevations and Isolated Parents
                                            PGA.SurfaceManager.Commands.CheckSurfaceForErrors();
                                            //PGA.SurfaceManager.SurfaceManager.TestIsolatedParent();
                                            //PGA.SurfaceManager.SurfaceManager.CorrectIsolatedParent();
                                            PGA.SurfaceManager.SurfaceManager.CorrectIsolatedParentByScaling();
                                            #endregion

                                            SavesCADFile(outdwg, acDbNewDoc, dwg);

                                           if (s.UseReports)
                                           {
                                               GenerateReport(outdwg, acDbNewDoc, dwg);
                                               GenerateXMLReport(outdwg, acDbNewDoc, dwg);

                                               COMS.MessengerManager.AddLog(String.Format
                                                   ("Generated Report: {0}", dwg.PolylineDwgName));
                                               COMS.MessengerManager.AddLog(String.Format
                                                   ("Completed Drawing: {0}", dwg.PolylineDwgName));
                                           }
                                           if (s.UseGenErrorReport)
                                               _flagAutoErrorReports = true;
                                            #endregion
                                           #region Show "All" Surface

                                            try
                                            {
                                               //ACADDB.ObjectId surfaceId = PasteSurfaces.GetSurfaceByName("All");
                                               //pTinSurface.SetSurfaceStyleToStandard(surfaceId);
                                           }
                                           catch (ACADRT.Exception ex)
                                           {
                                               #region Messaging

                                               COMS.MessengerManager.AddLog(String.Format
                                                   ("Show Surface Style: {0}", ex.Message));
                                               COMS.MessengerManager.LogException(ex);

                                               #endregion
                                           }

                                            #endregion
                                            #region Create BreakLines Output

                                            COMS.MessengerManager.AddLog("Start Breakline Outputs!");

                                           #region Hide "All" Surface

                                           try
                                           {
                                               ACADDB.ObjectId surfaceId = PasteSurfaces.GetSurfaceByName("All");
                                               pTinSurface.SetSurfaceStyleToHidden(surfaceId);
                                           }
                                           catch (ACADRT.Exception ex)
                                           {
                                               #region Messaging

                                               COMS.MessengerManager.AddLog(String.Format
                                                   ("Hide Surface Style: {0}", ex.Message));
                                               COMS.MessengerManager.LogException(ex);

                                               #endregion
                                           }

                                           #endregion

                                           if (s.UseSportVision && s.UseBreaklines)
                                           {
                                               COMS.MessengerManager.AddLog("Saving Sport Vision Outputs!");
                                               //SavesSVCADFile(svPath, acDbNewDoc, dwg);

                                               if (s.UseOutput3DPolys)
                                               {
                                                   try
                                                   {
                                                       COMS.MessengerManager.AddLog("Saving 3D Polyline Outputs!");

                                                       _commands.AddPoly3DToSideDB
                                                           (Poly3DPath(output, acDbNewDoc, dwg));
                                                   }
                                                   catch (Exception ex)
                                                   {
                                                       COMS.MessengerManager.LogException("Saving 3D Polyline Outputs", ex);
                                                   }

                                               }
                                           }

                                           COMS.MessengerManager.AddLog("End Breakline Outputs!");

                                            #endregion
                                            #region Create PDF Report
                                            //if (s.UsePDFReport)
                                            //{
                                            //    SavesPDFReport(outdwg, dwg);
                                            //} 
                                            #endregion
                                            #region Set DWG Completed Flag

                                            commands.SetDrawingStackCompletedFlag(dwg);

                                           #endregion
                                       }
                                   }
                                   catch (ACADRT.Exception ex)
                                   {
                                        #region Messaging
                                       COMS.MessengerManager.LogException(ex);

                                       #endregion
                                   }
                               }
                           }
                       }
                        #region Snapshot
                        //Active.Editor.UpdateScreen();
                        //System.Threading.Thread.SpinWait(1000);
                        //SnapshotToFile(Drawingpath.Replace("dwg", "png"), VisualStyleType.Wireframe2D); 
                        #endregion


                        #region Messaging

                        COMS.MessengerManager.AddLog
                           ("End Process Drawing: " + dwg.PolylineDwgName);

                        #endregion


                    }

                    #region Update Task in DB

                    commands.UpdateTaskCompleteDate(DateStamp);
                   commands.UpdateTaskManagerCompleteDate(DateStamp);

                   #endregion
               }
               try
               {
                    #region Reporting
                    COMS.MessengerManager.AddLog("Started Sending Automatic Reports!");
                    if (_flagAutoErrorReports)
                        StartProcess(DateStamp.Ticks.ToString());

                    COMS.MessengerManager.AddLog("Ended Sending Automatic Reports!"); 
                    #endregion

                }
               catch
               {
                   COMS.MessengerManager.AddLog("Failed Sending Automatic Reports!");
               }



               #region SendStringToExecute Commands for Watcher

               ACAD.Application.SetSystemVariable("NOMUTT", 1);
               ACAD.Application.SetSystemVariable("CMDECHO", 0);
               AcadUtilities.AcadUtilities.WriteMessage("\nPlease Wait! Starting Drawing Watcher!\n");
               AcadUtilities.AcadUtilities.WriteMessage("Please Wait! It may take 2 Minutes to Start Processing!\n");
               AcadUtilities.AcadUtilities.SendStringToExecute("._PGA-StartWatcher\n");

               #endregion

           }
           catch (System.Exception ex)
           {
               #region Messaging

               COMS.MessengerManager.AddLog(String.Format
                   ("Coalesce Drawing: {0}", ex.Message));
               COMS.MessengerManager.LogException(ex);

               #endregion
           }
           finally
           {
                ACAD.Application.SetSystemVariable("PROXYSHOW", 0);
                ACAD.Application.SetSystemVariable("PROXYNOTICE", 0);
                ACAD.Application.SetSystemVariable("PROXYGRAPHICS", 0);
                ACAD.Application.SetSystemVariable("FILEDIA", 0);
            }
        }

        public static void SetQNewTemplate()
        {
            try
            {
               COMS.MessengerManager.AddLog("Set QNew Template.");

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    AcadPreferences acPrefComObj = (AcadPreferences)global::Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
    
                    if (commands.GetTemplatePath() != acPrefComObj.Files.QNewTemplateFile)
                        acPrefComObj.Files.QNewTemplateFile = commands.GetTemplatePath();
                    
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        private static void OutputDllVersions(string getTemplatePath)
        {
            try
            {
                COMS.MessengerManager.AddLog("Starting DLL Info!");

                var template = Path.GetDirectoryName(getTemplatePath);

                DirectoryInfo dirs = new DirectoryInfo((template.Replace("template", "windows")));

                var files = dirs.GetFiles();
                for (int i = 0; i < files.Count(); i++)
                {
                    COMS.MessengerManager.AddLog(GetFileVersion(files[i].FullName.ToString()));
                    COMS.MessengerManager.AddLog(GetFileName(files[i].FullName.ToString()));
                }
                COMS.MessengerManager.AddLog("Ending DLL Info!");

            }
            catch (Exception )
            {
                #region Messaging

                COMS.MessengerManager.AddLog("Get Dll File Info Failed!");

                #endregion

            }
        }

        /// <summary>
        /// Checks the date stamp.
        /// </summary>
        /// <param name="dateStamp">The date stamp.</param>
        /// <returns>System.Nullable&lt;DateTime&gt;.</returns>
        private static DateTime? CheckDateStamp(DateTime? dateStamp)
        {
            COMS.MessengerManager.AddLog("Starting DateTime!");
            var _dateStamp = dateStamp.GetValueOrDefault();
            COMS.MessengerManager.AddLog("DateTime = " + _dateStamp);

            if (_dateStamp != null)
            {
                return _dateStamp;
            }
            return null;
        }

        /// <summary>
        /// Zooms the extents.
        /// </summary>
        public static void ZoomExtents()
        {
            try
            {
                var acad = global::Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
                acad.GetType().InvokeMember("ZoomExtents", BindingFlags.Public |
                                                           BindingFlags.InvokeMethod, null, acad, null);
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }


        /// <summary>
        /// Closes the polylines.
        /// </summary>
        public static void ClosePolylines()
        {
            try
            {
                var pId = GetAllPolylines();

                foreach (ACADDB.ObjectId oid in pId)
                {
                    try
                    {
                        using (var tr = CivilApplicationManager.StartTransaction())
                        {
                            var p = tr.GetObject(oid, ACADDB.OpenMode.ForWrite) as ACADDB.Polyline;
                            if (p != null)
                                p.Closed = true;
                            tr.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        COMS.MessengerManager.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }
        /// <summary>
        /// Gets all polylines.
        /// </summary>
        /// <returns>ACADDB.ObjectIdCollection.</returns>
        private static ACADDB.ObjectIdCollection GetAllPolylines()
        {
            return Process.GetIdsByTypeTypeValue(
                "POLYLINE",
                "LWPOLYLINE",
                "POLYLINE2D");
        }

        /// <summary>
        /// Sets the sport vision folder.
        /// </summary>
        /// <param name="dwg">The DWG.</param>
        /// <param name="commands">The commands.</param>
        /// <returns>System.String.</returns>
        private static string SetSportVisionFolder(DrawingStack dwg, DatabaseCommands commands)
        {
            try
            {
                if (commands == null)
                    commands = new DatabaseCommands();

                var date = DateConverts.DateTimeToStringFileSafe
                    (dwg.DateStamp.Value);

                var output = Path.Combine(commands.GetGlobalDestinationPath(dwg.DateStamp.Value),
                    String.Format("SPORTVISION-{0}", date));

                Utils.FileUtilities.CreateDirectory(output);
                return output;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return "";
        }

        /// <summary>
        /// Dumps the drawing stack.
        /// </summary>
        private static void DumpDrawingStack()
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                COMS.MessengerManager.AddLog("Date is NULL!");
                commands.DumpDrawingStack();
            }
        }


        public static string GetFileVersion(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }
        public static string GetProdVersion(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).ProductVersion;
        }
        public static string GetFileName(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).FileName;
        }

        /// <summary>
        /// Gets the os version.
        /// </summary>
        /// <returns>System.String.</returns>
        /// 
        public static string GetOsVersion()
        {
            try
            {
                var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                            select x.GetPropertyValue("Caption")).FirstOrDefault();
                return name != null ? name.ToString() : "Unknown OS Version";
            }
            catch (ACADRT.Exception ex)
            {           
                COMS.MessengerManager.LogException(ex);
            }
            return "Unknown OS Version";
        }
        /// <summary>
        /// Generates the XML report.
        /// </summary>
        /// <param name="outdwg">The outdwg.</param>
        /// <param name="acDbNewDoc">The ac database new document.</param>
        /// <param name="dwg">The DWG.</param>
        private static void GenerateXMLReport(string outdwg, ACADDB.Database acDbNewDoc, DrawingStack dwg)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    ReportWriter.ReportWriter writer = new ReportWriter.ReportWriter();

                    var hole = GetAndCheckHole(dwg);
                    var filepath = Path.GetDirectoryName(outdwg);
                    var savepath = GetAndCheckPath(hole, filepath);


                    var drawingname = String.Format("s-{0}", dwg.PolylineDwgName);
                    var completepath = Path.Combine(savepath, drawingname);
                    writer.InitializeXMLReportWriter(completepath,DateStamp,hole,dwg.PolylineDwgName);
                }
            }
            catch (System.Exception ex1)
            {
                COMS.MessengerManager.LogException(ex1);
            }
        }

        /// <summary>
        /// Starts the process.
        /// </summary>
        /// <param name="dt">The dt.</param>
        private static void StartProcess(string dt)
        {
            try
            {
                System.Diagnostics.Process ps = new System.Diagnostics.Process();
                ConfigProperties(ps, dt);
            }
            catch (System.Exception ex1)
            {
                COMS.MessengerManager.LogException(ex1);
            }
        }
        /// <summary>
        /// Configurations the properties.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="args">The arguments.</param>
        private static void ConfigProperties(System.Diagnostics.Process process, string args)
        {
            try
            {
                string name = GetReportsProcessPath();
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = args;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        /// <summary>
        /// Gets the reports process path.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        private static string GetReportsProcessPath()
        {
            string filename = @"Autodesk\ApplicationPlugins\PGA-CivilTinSurf2016.bundle\Contents\Reports\Bin\PGA.TransFormReports.exe";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            PGA.MessengerManager.MessengerManager.AddLog(string.Concat("Set Path: ", Path.Combine(path, filename)));
            if (!File.Exists(Path.Combine(path, filename)))
            {
                PGA.MessengerManager.MessengerManager.AddLog(string.Concat("FileNotFound: ", Path.Combine(path, filename)));
                throw new FileNotFoundException(Path.Combine(path, filename));
            }
            PGA.MessengerManager.MessengerManager.AddLog(string.Concat("File Found: ", Path.Combine(path, filename)));
            return Path.Combine(path, filename);
        }

        /// <summary>
        /// Gets the and check hole.
        /// </summary>
        /// <param name="dwg">The DWG.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static string GetAndCheckHole(DrawingStack dwg)
        {
            string hole = null;
            try
            {
                hole = GolfHoles.GetHoles((int)dwg.Hole);
                if (String.IsNullOrEmpty(hole))
                    throw new ArgumentNullException();
            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return hole;
        }
        /// <summary>
        /// Gets the and check path.
        /// </summary>
        /// <param name="_hole">The hole.</param>
        /// <param name="_path">The path.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static string GetAndCheckPath(string _hole, string _path)
        {
            string savepath = null;
            try
            {
                savepath = Path.Combine(_path, "HOLE" + _hole);

                if (String.IsNullOrEmpty(savepath))
                    throw new ArgumentNullException();

                Utils.FileUtilities.CreateDirectory(savepath);

            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return savepath;
        }
        /// <summary>
        /// Handles the CommandEnded event of the AcNewDoc control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CommandEventArgs"/> instance containing the event data.</param>
        private static void AcNewDoc_CommandEnded(object sender, CommandEventArgs e)
        {

        }
        /// <summary>
        /// Savezs the cad file.
        /// </summary>
        /// <param name="outdwg">The outdwg.</param>
        /// <param name="acDbNewDoc">The ac database new document.</param>
        /// <param name="dwg">The DWG.</param>
        private static void SavezCADFile(string outdwg, ACADDB.Database acDbNewDoc, DrawingStack dwg)
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                var hole = GolfHoles.GetHoles((int) dwg.Hole);
                var filepath = Path.GetDirectoryName(outdwg);
                var savepath = Path.Combine(filepath, "HOLE" + hole);

                Utils.FileUtilities.CreateDirectory(savepath);

                var drawingname  = String.Format("z-{0}", dwg.PolylineDwgName);
                var completepath = Path.Combine(savepath,drawingname);

                commands.AddExportToCADRecord
                        (dwg, completepath, Convert.ToInt64(hole));
             
                acDbNewDoc.SaveAs(completepath, ACADDB.DwgVersion.Current);

            }
        }
        /// <summary>
        /// Generates the report.
        /// </summary>
        /// <param name="outdwg">The outdwg.</param>
        /// <param name="acDbNewDoc">The ac database new document.</param>
        /// <param name="dwg">The DWG.</param>
        private static void GenerateReport(string outdwg, ACADDB.Database acDbNewDoc, DrawingStack dwg)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    ReportWriter.ReportWriter writer = new ReportWriter.ReportWriter();

                    var hole = GolfHoles.GetHoles((int)dwg.Hole);
                    var filepath = Path.GetDirectoryName(outdwg);
                    var savepath = Path.Combine(filepath, "HOLE" + hole);

                    Utils.FileUtilities.CreateDirectory(savepath);

                    var drawingname = String.Format("s-{0}", dwg.PolylineDwgName);
                    var completepath = Path.Combine(savepath, drawingname);
                    writer.InitializeReportWriter(completepath);
                }
            }
            catch (System.Exception ex1)
            {
                COMS.MessengerManager.LogException(ex1);
            }
        }
        /// <summary>
        /// Saveses the cad file.
        /// </summary>
        /// <param name="outdwg">The outdwg.</param>
        /// <param name="acDbNewDoc">The ac database new document.</param>
        /// <param name="dwg">The DWG.</param>
        private static void SavesCADFile(string outdwg, ACADDB.Database acDbNewDoc, DrawingStack dwg)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var hole = GolfHoles.GetHoles((int)dwg.Hole);
                    var filepath = Path.GetDirectoryName(outdwg);
                    var savepath = Path.Combine(filepath, "HOLE" + hole);

                    Utils.FileUtilities.CreateDirectory(savepath);

                    var drawingname = String.Format("s-{0}", dwg.PolylineDwgName);
                    var completepath = Path.Combine(savepath, drawingname);

                    commands.AddExportToCADRecord
                            (dwg, completepath, Convert.ToInt64(hole));

                    acDbNewDoc.SaveAs(completepath, ACADDB.DwgVersion.Current);
                }
            }
            catch (ACADRT.Exception ex)
            {               
                COMS.MessengerManager.LogException(ex);
            }
        }
        /// <summary>
        /// Saveses the PDF report.
        /// </summary>
        /// <param name="outdwg">The outdwg.</param>
        /// <param name="dwg">The DWG.</param>
        private static void SavesPDFReport(string outdwg, DrawingStack dwg)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var hole = GolfHoles.GetHoles((int)dwg.Hole);
                    var filepath = Path.GetDirectoryName(outdwg);
                    var savepath = Path.Combine(filepath, "HOLE" + hole);

                    Utils.FileUtilities.CreateDirectory(savepath);

                    var drawingname = String.Format("s-{0}", dwg.PolylineDwgName);
                    var completepath = Path.Combine(savepath, drawingname);

                    commands.AddExportToCADRecord
                            (dwg, completepath, Convert.ToInt64(hole));

                    COMS.MessengerManager.AddLog("completepath = " + completepath);
                   // PGA.PlotManager.PrintCommands.PublishLayouts(completepath,hole);

                }
            }
            catch (ACADRT.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }
        /// <summary>
        /// Saveses the svcad file.
        /// </summary>
        /// <param name="outdwg">The outdwg.</param>
        /// <param name="acDbNewDoc">The ac database new document.</param>
        /// <param name="dwg">The DWG.</param>
        private static void SavesSVCADFile(string outdwg, ACADDB.Database acDbNewDoc, DrawingStack dwg)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var hole = GolfHoles.GetHoles((int)dwg.Hole);
                    var filepath = Path.GetDirectoryName(outdwg);
                    var savepath = Path.Combine(filepath, "HOLE" + hole);

                    Utils.FileUtilities.CreateDirectory(savepath);

                    var drawingname = String.Format("SV-{0}", dwg.PolylineDwgName);
                    var completepath = Path.Combine(savepath, drawingname);

                    commands.AddExportToCADRecord
                            (dwg, completepath, Convert.ToInt64(hole));

                    acDbNewDoc.SaveAs(completepath, ACADDB.DwgVersion.Current);
                }
            }
            catch (ACADRT.Exception ex)
            {
                COMS.MessengerManager.AddLog(String.Format
                    ("SavesSVCADFile: {0}", ex.Message));
                COMS.MessengerManager.LogException(ex);
            }
        }
        /// <summary>
        /// Poly3s the d path.
        /// </summary>
        /// <param name="outdwg">The outdwg.</param>
        /// <param name="acDbNewDoc">The ac database new document.</param>
        /// <param name="dwg">The DWG.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// hole
        /// or
        /// FullName
        /// </exception>
        private static string Poly3DPath(string outdwg, ACADDB.Database acDbNewDoc, DrawingStack dwg)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var hole = GolfHoles.GetHoles((int)dwg.Hole);
                    if (hole == null) throw new ArgumentNullException(nameof(hole));
                    //var filepath = Path.GetDirectoryName(outdwg);
                    //if (filepath == null) throw new ArgumentNullException(nameof(filepath));
                    var savepath = Path.Combine(outdwg, "HOLE" + hole);

                    var dirinfo = Utils.FileUtilities.CreateDirectory(savepath);

                    if (!dirinfo.Exists)
                        throw new ArgumentNullException(nameof(dirinfo.FullName));

                    var drawingname = String.Format("SV-{0}", dwg.PolylineDwgName);
                    var completepath = Path.Combine(savepath, drawingname);

                    return completepath;
                }
            }
            catch (ACADRT.Exception ex)
            {               
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }
        /// <summary>
        /// Sets the sport vision folder.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        private static string SetSportVisionFolder(string name)
        {
            try
            {
                var dir = Path.GetDirectoryName(name);
                var file = Path.GetFileName(name);

                var output = Path.Combine(dir, "SportVision");
                Utils.FileUtilities.CreateDirectory(output);

                return Path.Combine(output, file);
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return "";
        }
        /// <summary>
        /// Sets the out put folder.
        /// </summary>
        /// <param name="dwg">The DWG.</param>
        /// <param name="commands">The commands.</param>
        /// <returns>System.String.</returns>
        private static string SetOutPutFolder(DrawingStack dwg, DatabaseCommands commands)
        {
            try
            {
                if (commands == null)
                    commands = new DatabaseCommands();

                var date = DateConverts.DateTimeToStringFileSafe
                           (dwg.DateStamp.Value);

                var output = Path.Combine(commands.GetGlobalDestinationPath(dwg.DateStamp.Value),
                             String.Format("TINSURF-{0}", date));

                Utils.FileUtilities.CreateDirectory(output);
                return output;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return "";
        }
        /// <summary>
        /// Gets or sets the g cloud path.
        /// </summary>
        /// <value>The g cloud path.</value>
        public static string GCloudPath { get; set; }
        /// <summary>
        /// Gets or sets the dw drawing stacks.
        /// </summary>
        /// <value>The dw drawing stacks.</value>
        public static IList<DrawingStack> DwDrawingStacks { get; set; }
        /// <summary>
        /// Gets or sets the g DWG path.
        /// </summary>
        /// <value>The g DWG path.</value>
        public static string GDwgPath { get; set; }
        /// <summary>
        /// Loadands the process polys synch.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// dwgs
        /// or
        /// path
        /// </exception>
        /// <exception cref="System.Exception">Clear Database! Drawing Stack Not Synchronized!</exception>
        public static void LoadandProcessPolysSynch()
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var TemplatePath = commands.GetTemplatePath();

                    #region Get Dwgs to Process

                    var dwgs = commands.LoadandProcessPolys();

                    if (dwgs == null) throw new ArgumentNullException(nameof(dwgs));
                    IList<DrawingStack> DwDrawingStacks = new List<DrawingStack>();
                    foreach (var dwg in dwgs)
                    {
                        if (DwDrawingStacks != null) DwDrawingStacks.Add(dwg);
                    }

                    #endregion

                   

                    var TotalDrawings = 0;
                    foreach (DrawingStack dwg in DwDrawingStacks)
                    {

                        GDwgPath = commands.GetGlobalDWGPath(dwg.DateStamp.Value);
                        GCloudPath = commands.GetGlobalPointCloudPath(dwg.DateStamp.Value);

                        if (++TotalDrawings > 18)
                            throw new System.Exception
                                ("Clear Database! Drawing Stack Not Synchronized!");


                        var acDocMgr = ACAD.Application.DocumentManager;
                        Document acNewDoc = null;
                        Document acDoc = ACAD.Application.DocumentManager.MdiActiveDocument;

                        if (acDocMgr.Count == 1)
                        {

                            acNewDoc = acDocMgr.Add(TemplatePath);
                            using (acDocMgr.MdiActiveDocument.LockDocument())
                            {
                                acDocMgr.MdiActiveDocument = acDoc;
                                acDocMgr.CurrentDocument.CloseAndDiscard();
                                acDocMgr.MdiActiveDocument = acNewDoc;

                            }

                        }
                        COMS.MessengerManager.AddLog
                        ("Begin Process Drawing: " + dwg.PolylineDwgName);

                        ACAD.Application.SetSystemVariable("FILEDIA", 0);

                        using (acDocMgr.MdiActiveDocument.LockDocument())
                        {

                            if (acDoc != null)

                            {
                                ACADDB.Database acDbNewDoc = acNewDoc.Database;
                                var ed = acNewDoc.Editor;
                                //var ed = doc.Editor;

                                string gpath = Convert.ToString(GDwgPath);

                                if (gpath != null)
                                {
                                    string path = Path.Combine(gpath, dwg.PolylineDwgName);

                                    if (path == null) throw new ArgumentNullException(nameof(path));


                                    if (!File.Exists(path))
                                    {
                                        DatabaseLogs.FormatLogs("File Not Found", path);
                                        return;
                                    }

                                    try
                                    {
                                        if (dwg.DateStamp != null)
                                            DateStamp = (DateTime)dwg.DateStamp;

                                        using (ACADDB.Database db = new ACADDB.Database(false, true))
                                        {
                                            // Read the DWG file into our Database object

                                            db.ReadDwgFile(
                                                path,
                                                ACADDB.FileOpenMode.OpenForReadAndReadShare,
                                                false,
                                                ""
                                                );

                                            db.RetainOriginalThumbnailBitmap = true;

                                            // We'll store the current working database, to reset
                                            // after the purge operation

                                            var wdb = ACADDB.HostApplicationServices.WorkingDatabase;
                                            ACADDB.HostApplicationServices.WorkingDatabase = db;

                                            // Purge unused DGN linestyles from the drawing
                                            // (returns false if nothing is erased)
                                            ACADDB.ObjectIdCollection collection = Process.GetIdsByTypeTypeValue(
                                                "POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");
                                            NumberofPolylines = collection.Count;
                                            Process.CopyPolylinesBetweenDatabases(db, collection);

                                            // Still need to reset the working database

                                            ACADDB.HostApplicationServices.WorkingDatabase = wdb;

                                            acNewDoc.CommandEnded += AcNewDoc_CommandEnded;

                                            COMS.MessengerManager.AddLog(String.Format
                                                ("Starting Drawing: {0}", dwg.PolylineDwgName));

                                            COMS.MessengerManager.AddLog(String.Format
                                                 ("Set Date Started in Task: {0}", dwg.PolylineDwgName));

                                            collection = null;
                                            // Reselect Polylines in the current database
                                            collection = Process.GetIdsByTypeTypeValue(
                                                    "POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");

                                            string date = dwg.DateStamp.Value.ToFileTime().ToString();

                                            string output = SetOutPutFolder(dwg, commands);

                                            string fileName =
                                                System.IO.Path.GetFileNameWithoutExtension(dwg.PolylineDwgName);
                                            string outdxf = Path.Combine(output,
                                                String.Format("{0}{1}", fileName, ".dxf"));
                                            string outdwg = Path.Combine(output, dwg.PolylineDwgName);
                                            PLineToLayers.ProcessLayers(collection, wdb);
                                            Hole = (int)dwg.Hole;
                                            Drawingpath = outdwg;
                                            //Change Layer Names
                                            AssignNames.ChangeLayers();
                                            //Create Point Cloud Surface "All"
                                            CreateAllSurface.CreateTINSurface pTinSurface =
                                                new CreateAllSurface.CreateTINSurface();

                                            pTinSurface.CreateTINSurfaceByPointCollection(GCloudPath,
                                                dwg.PointCloudDwgName);

                                            SavezCADFile(outdwg, acDbNewDoc, dwg);
                                            //Create all minor surfaces
                                            PGA.Surfaces.C3DSurfacesDemoCommands civilcommand =
                                                new C3DSurfacesDemoCommands();
                                            civilcommand.CreateAllSurfaces();

                                            SavesCADFile(outdwg, acDbNewDoc, dwg);
                                            COMS.MessengerManager.AddLog(String.Format
                                                ("Completed Drawing: {0}", dwg.PolylineDwgName));

                                        }
                                    }
                                    catch (ACADRT.Exception ex)
                                    {
                                        COMS.MessengerManager.AddLog(String.Format
                                            ("Coalesce Drawing: {0}", ex.Message));
                                        COMS.MessengerManager.LogException(ex);
                                    }

                                }

                            }
                        }
                        COMS.MessengerManager.AddLog
                        ("End Process Drawing: " + dwg.PolylineDwgName);
                        //break;//only testing
                    }
                    PGA.ExportToAutoCAD.ExportToCad.SynchExportDWGs();
                    ACAD.Application.SetSystemVariable("FILEDIA", 1);
                    commands.UpdateTaskCompleteDate(DateStamp);
                    commands.UpdateTaskManagerCompleteDate(DateStamp);
                    OpenandCloseDwgs.CloseDocuments();
                }
            }
            catch (System.Exception ex1)
            {
                COMS.MessengerManager.AddLog(String.Format
                    ("Coalesce Drawing: {0}", ex1.Message));
                COMS.MessengerManager.LogException(ex1);
            }
            //Reset DWG State Here 
            ACAD.Application.SetSystemVariable("FILEDIA", 1);
        }

    }
}
