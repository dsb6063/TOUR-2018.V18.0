// ***********************************************************************
// Assembly         : PGA.PlotManager
// Author           : Daryl Banks, PSM
// Created          : 05-14-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 05-14-2016
// ***********************************************************************
// <copyright file="Commands.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Publishing;
using Autodesk.AutoCAD.Runtime;
using PGA.AttributeRefManager;
using PGA.SelectionManager;
using Exception = System.Exception;
using PlotType = Autodesk.AutoCAD.DatabaseServices.PlotType;

namespace PGA.PlotManager
{

    public static class Extensions
    {
        /// <summary>
        /// Reverses the order of the X and Y properties of a Point2d.
        /// </summary>
        /// <param name="flip">Boolean indicating whether to reverse or not.</param>
        /// <returns>The original Point2d or the reversed version.</returns>

        public static Point2d Swap(this Point2d pt, bool flip = true)
        {
            return flip ? new Point2d(pt.Y, pt.X) : pt;
        }

        /// <summary>
        /// Pads a Point2d with a zero Z value, returning a Point3d.
        /// </summary>
        /// <param name="pt">The Point2d to pad.</param>
        /// <returns>The padded Point3d.</returns>

        public static Point3d Pad(this Point2d pt)
        {
            return new Point3d(pt.X, pt.Y, 0);
        }

        /// <summary>
        /// Strips a Point3d down to a Point2d by simply ignoring the Z ordinate.
        /// </summary>
        /// <param name="pt">The Point3d to strip.</param>
        /// <returns>The stripped Point2d.</returns>

        public static Point2d Strip(this Point3d pt)
        {
            return new Point2d(pt.X, pt.Y);
        }

        /// <summary>
        /// Creates a layout with the specified name and optionally makes it current.
        /// </summary>
        /// <param name="name">The name of the viewport.</param>
        /// <param name="select">Whether to select it.</param>
        /// <returns>The ObjectId of the newly created viewport.</returns>

        public static ObjectId CreateAndMakeLayoutCurrent(
          this LayoutManager lm, string name, bool select = true
        )
        {
            // First try to get the layout

            var id = lm.GetLayoutId(name);

            // If it doesn't exist, we create it

            if (!id.IsValid)
            {
                id = lm.CreateLayout(name);
            }

            // And finally we select it

            if (select)
            {
                lm.CurrentLayout = name;
            }

            return id;
        }

        /// <summary>
        /// Applies an action to the specified viewport from this layout.
        /// Creates a new viewport if none is found withthat number.
        /// </summary>
        /// <param name="tr">The transaction to use to open the viewports.</param>
        /// <param name="vpNum">The number of the target viewport.</param>
        /// <param name="f">The action to apply to each of the viewports.</param>

        public static void ApplyToViewport(this Layout lay, Transaction tr, int vpNum, Action<Viewport> f)
        {
            var vpIds = lay.GetViewports();
            Viewport viewport = null;

            foreach (ObjectId vpId in vpIds)
            {
                var vp = tr.GetObject(vpId, OpenMode.ForWrite) as Viewport;
                if (vp != null || vp.Number == vpNum)
                {
                    // We have found our viewport
                    if (Math.Abs(vp.Width - 15.03) < 0.1)
                    {
                        viewport = vp;
                        break;
                    }
                }
            }

            if (viewport == null)
            {
                var btr = (BlockTableRecord)tr.GetObject(lay.BlockTableRecordId, OpenMode.ForWrite);
                viewport = new Viewport
                {
                    On = true,
                    GridOn = true
                };

                // Add it to the database
                btr.AppendEntity(viewport);
                tr.AddNewlyCreatedDBObject(viewport, true);
            }

            // And finally call our function on it
            f(viewport);
        }

        /// <summary>
        /// Apply plot settings to the provided layout.
        /// </summary>
        /// <param name="pageSize">The canonical media name for our page size.</param>
        /// <param name="styleSheet">The pen settings file (ctb or stb).</param>
        /// <param name="devices">The name of the output device.</param>

        public static void SetPlotSettings(
          this Layout lay, string pageSize, string styleSheet, string device
        )
        {
            using (var ps = new PlotSettings(lay.ModelType))
            {
                ps.CopyFrom(lay);

                var psv = PlotSettingsValidator.Current;

                // Set the device

                var devs = psv.GetPlotDeviceList();
                if (devs.Contains(device))
                {
                    psv.SetPlotConfigurationName(ps, device, null);
                    psv.RefreshLists(ps);
                }

                // Set the media name/size

                var mns = psv.GetCanonicalMediaNameList(ps);
                if (mns.Contains(pageSize))
                {
                    psv.SetCanonicalMediaName(ps, pageSize);
                }

                // Set the pen settings

                var ssl = psv.GetPlotStyleSheetList();
                if (ssl.Contains(styleSheet))
                {
                    psv.SetCurrentStyleSheet(ps, styleSheet);
                }

                // Copy the PlotSettings data back to the Layout

                var upgraded = false;
                if (!lay.IsWriteEnabled)
                {
                    lay.UpgradeOpen();
                    upgraded = true;
                }

                lay.CopyFrom(ps);

                if (upgraded)
                {
                    lay.DowngradeOpen();
                }
            }
        }

        /// <summary>
        /// Determine the maximum possible size for this layout.
        /// </summary>
        /// <returns>The maximum extents of the viewport on this layout.</returns>

        public static Extents2d GetMaximumExtents(this Layout lay)
        {
            // If the drawing template is imperial, we need to divide by
            // 1" in mm (25.4)

            var div = lay.PlotPaperUnits == PlotPaperUnit.Inches ? 25.4 : 1.0;

            // We need to flip the axes if the plot is rotated by 90 or 270 deg

            var doIt =
              lay.PlotRotation == PlotRotation.Degrees090 ||
              lay.PlotRotation == PlotRotation.Degrees270;

            // Get the extents in the correct units and orientation

            var min = lay.PlotPaperMargins.MinPoint.Swap(doIt) / div;
            var max =
              (lay.PlotPaperSize.Swap(doIt) -
               lay.PlotPaperMargins.MaxPoint.Swap(doIt).GetAsVector()) / div;

            return new Extents2d(min, max);
        }

        /// <summary>
        /// Sets the size of the viewport according to the provided extents.
        /// </summary>
        /// <param name="ext">The extents of the viewport on the page.</param>
        /// <param name="fac">Optional factor to provide padding.</param>

        public static void ResizeViewport(
          this Viewport vp, Extents2d ext, double fac = 1.0
        )
        {
            vp.Width = (ext.MaxPoint.X - ext.MinPoint.X) * fac;
            vp.Height = (ext.MaxPoint.Y - ext.MinPoint.Y) * fac;
            vp.CenterPoint =
              (Point2d.Origin + (ext.MaxPoint - ext.MinPoint) * 0.5).Pad();
        }

        /// <summary>
        /// Sets the view in a viewport to contain the specified model extents.
        /// </summary>
        /// <param name="ext">The extents of the content to fit the viewport.</param>
        /// <param name="fac">Optional factor to provide padding.</param>

        public static void FitContentToViewport(
          this Viewport vp, Extents3d ext, double fac = 1.0
        )
        {
            // Let's zoom to just larger than the extents

            vp.ViewCenter =
              (ext.MinPoint + ((ext.MaxPoint - ext.MinPoint) * 0.5)).Strip();

            // Get the dimensions of our view from the database extents

            var hgt = ext.MaxPoint.Y - ext.MinPoint.Y;
            var wid = ext.MaxPoint.X - ext.MinPoint.X;

            // We'll compare with the aspect ratio of the viewport itself
            // (which is derived from the page size)

            var aspect = vp.Width / vp.Height;

            // If our content is wider than the aspect ratio, make sure we
            // set the proposed height to be larger to accommodate the
            // content

            if (wid / hgt > aspect)
            {
                hgt = wid / aspect;
            }

            // Set the height so we're exactly at the extents

            vp.ViewHeight = hgt;

            // Set a custom scale to zoom out slightly (could also
            // vp.ViewHeight *= 1.1, for instance)

            vp.CustomScale *= fac;
        }
    }

    public class PrintCommands
    {
        [CommandMethod("PGA-CreateLayout")]
        public void CreateLayout()

        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
                return;
            var db = doc.Database;
            var ed = doc.Editor;

            var ext = new Extents2d();

            using (var tr = db.TransactionManager.StartTransaction())
            {
                // Create and select a new layout tab

                //var id = LayoutManager.Current.CreateAndMakeLayoutCurrent("PGA-OUTPUT");
                var id = LayoutManager.Current.GetLayoutId("TOUR-PUBLISH");

                // Open the created layout

                var lay = (Layout)tr.GetObject(id, OpenMode.ForWrite);

                // Make some settings on the layout and get its extents

                lay.SetPlotSettings(
                  //"ISO_full_bleed_2A0_(1189.00_x_1682.00_MM)", // Try this big boy!
                  "ANSI_B_(11.00_x_17.00_Inches)",
                  "acad.ctb",
                  "DWF6 ePlot.pc3"
                );

                ext = lay.GetMaximumExtents();

                lay.ApplyToViewport(
                  tr, 2,
                  vp =>
                  {
              // Size the viewport according to the extents calculated when
              // we set the PlotSettings (device, page size, etc.)
              // Use the standard 10% margin around the viewport
              // (found by measuring pixels on screenshots of Layout1, etc.)

              //vp.ResizeViewport(ext, 0.8);

              // Adjust the view so that the model contents fit
              if (ValidDbExtents(db.Extmin, db.Extmax))
                      {
                          vp.FitContentToViewport(new Extents3d(db.Extmin, db.Extmax), 0.9);
                      }

              // Finally we lock the view to prevent meddling

              //vp.Locked = true;
                  }
                );

                // Commit the transaction

                tr.Commit();
            }

            // Zoom so that we can see our new layout, again with a little padding

            ed.Command("_.ZOOM", "_E");
            ed.Command("_.ZOOM", ".7X");
            ed.Regen();
        }

        [CommandMethod("PGA-CreateGreenLayout")]
        public void CreateGreenLayout()

        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
                return;
            var db = doc.Database;
            var ed = doc.Editor;

            var ext = new Extents2d();

            try
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {

                    var oid = Select.GetGreenPolylines();

                    ext = GetPLineExts(oid);

                    // Create and select a new layout tab

                    var id = LayoutManager.Current.CreateAndMakeLayoutCurrent("PGA-LAYOUT-GREEN");


                    // Open the created layout

                    var lay = (Layout)tr.GetObject(id, OpenMode.ForWrite);

                    // Make some settings on the layout and get its extents

                    lay.SetPlotSettings(
                      //"ISO_full_bleed_2A0_(1189.00_x_1682.00_MM)", // Try this big boy!
                      "ANSI_B_(11.00_x_17.00_Inches)",
                      "acad.ctb",
                      "DWF6 ePlot.pc3"
                    );

                    //ext = lay.GetMaximumExtents();

                    lay.ApplyToViewport(
                      tr, 2,
                      vp =>
                      {
                      // Size the viewport according to the extents calculated when
                      // we set the PlotSettings (device, page size, etc.)
                      // Use the standard 10% margin around the viewport
                      // (found by measuring pixels on screenshots of Layout1, etc.)

                      vp.ResizeViewport(ext, 0.8);

                      // Adjust the view so that the model contents fit
                      //if (ValidDbExtents(db.Extmin, db.Extmax))
                      //    {
                      //        vp.FitContentToViewport(new Extents3d(db.Extmin, db.Extmax), 0.9);
                      //    }

                      // Finally we lock the view to prevent meddling

                      //vp.Locked = true;
                  }
                    );

                    // Commit the transaction

                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            // Zoom so that we can see our new layout, again with a little padding

            ed.Command("_.ZOOM", "_E");
            ed.Command("_.ZOOM", ".7X");
            ed.Regen();
        }

        private Extents2d GetPLineExts(List<ObjectId> oid)
        {
            using (Transaction tr = Active.StartTransaction())
            {
                var polyline= (tr.GetObject(oid.FirstOrDefault(), OpenMode.ForRead) as Polyline);
                var maxpt = polyline.Bounds.Value.MaxPoint.Convert2d(new Plane(Point3d.Origin,Vector3d.ZAxis));
                var minpt = polyline.Bounds.Value.MinPoint.Convert2d(new Plane(Point3d.Origin, Vector3d.ZAxis));

                return new Extents2d(minpt,maxpt);
            }

        }

        // Returns whether the provided DB extents - retrieved from
        // Database.Extmin/max - are "valid" or whether they are the default
        // invalid values (where the min's coordinates are positive and the
        // max coordinates are negative)

        private bool ValidDbExtents(Point3d min, Point3d max)
        {
            return
              !(min.X > 0 && min.Y > 0 && min.Z > 0 &&
                max.X < 0 && max.Y < 0 && max.Z < 0);
        }

        // Publishes layouts to a PDF file
        [CommandMethod("PGA-PublishLayouts")]
        public static void PublishLayouts()
        {
            var filename =  Application.DocumentManager.MdiActiveDocument.Name;
            using (DsdEntryCollection dsdDwgFiles = new DsdEntryCollection())
            {
                // Define the first layout
                using (DsdEntry dsdDwgFile1 = new DsdEntry())
                {
                    // Set the file name and layout
                    //dsdDwgFile1.DwgName = filename;
                    dsdDwgFile1.DwgName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) 
                                          + @"\Autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\Contents\Template\PGA_TOUR_PRINT.dwt";
                    dsdDwgFile1.Layout = "PGA-TOUR";  //"PGA-OUTPUT";
                    dsdDwgFile1.Title = "PGA TOUR AUTOGENERATED SURFACE";

                    // Set the page setup override
                   
                    dsdDwgFile1.Nps = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) 
                                      + @"\Autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\Contents\Template\PGA_TOUR_PRINT.dwt"; 
                    dsdDwgFile1.NpsSourceDwg = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) 
                                      + @"\Autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\Contents\Template\PGA_TOUR_PRINT.dwt";

                    dsdDwgFiles.Add(dsdDwgFile1);
                }
                #region Define Second Layout

                //// Define the second layout
                //using (DsdEntry dsdDwgFile2 = new DsdEntry())
                //{
                //    // Set the file name and layout
                //    dsdDwgFile2.DwgName = "C:\\AutoCAD\\Samples\\Sheet Sets\\Architectural\\A-02.dwg";
                //    dsdDwgFile2.Layout = "ELEVATIONS";
                //    dsdDwgFile2.Title = "A-02 ELEVATIONS";

                //    // Set the page setup override
                //    dsdDwgFile2.Nps = "";
                //    dsdDwgFile2.NpsSourceDwg = "";

                //    dsdDwgFiles.Add(dsdDwgFile2);
                //} 
                #endregion

                // Set the properties for the DSD file and then write it out
                using (DsdData dsdFileData = new DsdData())
                {
                    // Set the target information for publishing
                    dsdFileData.DestinationName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\PGA-Publish.pdf";
                    dsdFileData.ProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\";
                    dsdFileData.SheetType = SheetType.MultiPdf;

                    // Must never have existing file or will prompt to overwrite.
                    dsdFileData.PromptForDwfName = false;
                    
                    // Set the drawings that should be added to the publication
                    dsdFileData.SetDsdEntryCollection(dsdDwgFiles);

                    // Set the general publishing properties
                    dsdFileData.LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)  + @"\myBatch.txt";

                    // Create the DSD file
                    dsdFileData.WriteDsd(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\batchdrawings2.dsd");

                    try
                    {
                        // Publish the specified drawing files in the DSD file, and
                        // honor the behavior of the BACKGROUNDPLOT system variable

                        using (DsdData dsdDataFile = new DsdData())
                        {
                            dsdDataFile.ReadDsd(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\batchdrawings2.dsd");

                            // Get the DWG to PDF.pc3 and use it as a 
                            // device override for all the layouts
                            PlotConfig acPlCfg = PlotConfigManager.SetCurrentConfig("DWG to PDF.PC3");
                            Application.Publisher.PublishExecute(dsdDataFile, acPlCfg);
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception es)
                    {
                        PGA.MessengerManager.MessengerManager.AddLog(es.Message);
                    }
                }
            }
        }
        public static void PublishLayouts(string path, string hole)
        {

            var completepath = Path.ChangeExtension(path, "pdf");
          
            var filename = Application.DocumentManager.MdiActiveDocument.Name;
            using (DsdEntryCollection dsdDwgFiles = new DsdEntryCollection())
            {
                // Define the first layout
                using (DsdEntry dsdDwgFile1 = new DsdEntry())
                {
                    // Set the file name and layout
                    dsdDwgFile1.DwgName = filename;
                    dsdDwgFile1.Layout = "PGA-OUTPUT";
                    dsdDwgFile1.Title  = "PGA TOUR AUTOGENERATED SURFACE";

                    // Set the page setup override
                    dsdDwgFile1.Nps = "";
                    dsdDwgFile1.NpsSourceDwg = "";

                    dsdDwgFiles.Add(dsdDwgFile1);
                }
                #region Define Second Layout

                //// Define the second layout
                //using (DsdEntry dsdDwgFile2 = new DsdEntry())
                //{
                //    // Set the file name and layout
                //    dsdDwgFile2.DwgName = "C:\\AutoCAD\\Samples\\Sheet Sets\\Architectural\\A-02.dwg";
                //    dsdDwgFile2.Layout = "ELEVATIONS";
                //    dsdDwgFile2.Title = "A-02 ELEVATIONS";

                //    // Set the page setup override
                //    dsdDwgFile2.Nps = "";
                //    dsdDwgFile2.NpsSourceDwg = "";

                //    dsdDwgFiles.Add(dsdDwgFile2);
                //} 
                #endregion


                #region Define third layout
                //// Define the second layout
                using (DsdEntry dsdDwgFile2 = new DsdEntry())
                {
                    // Set the file name and layout
                    dsdDwgFile2.DwgName = filename;
                    dsdDwgFile2.Layout  = "ELEVATIONS";
                    dsdDwgFile2.Title   = "A-02 ELEVATIONS";
                    
                    
                    // Set the page setup override
                    dsdDwgFile2.Nps = "";
                    dsdDwgFile2.NpsSourceDwg = @"C:\Users\m4800\AppData\Roaming\Autodesk\ApplicationPlugins\PGA-CivilTinSurf2016.bundle\Contents\Template\PGA_PRINT_TMPLT.dwg";
                    dsdDwgFiles.Add(dsdDwgFile2);
                }
                #endregion

                // Set the properties for the DSD file and then write it out
                using (DsdData dsdFileData = new DsdData())
                {
                    dsdFileData.PromptForDwfName = false;
                    // Set the target information for publishing
                    //dsdFileData.DestinationName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PGA-Publish.pdf";
                    //dsdFileData.ProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
                    dsdFileData.DestinationName = completepath;    //PGA-Publish.pdf;
                    dsdFileData.ProjectPath = Path.GetDirectoryName(completepath);

                    dsdFileData.SheetType = SheetType.MultiPdf;

                    // Set the drawings that should be added to the publication
                    dsdFileData.SetDsdEntryCollection(dsdDwgFiles);

                    // Set the general publishing properties
                    //dsdFileData.LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\myBatch.txt";
                    dsdFileData.LogFilePath = Path.ChangeExtension(path, "txt"); // "\\myBatch.txt";


                    // Create the DSD file
                    //dsdFileData.WriteDsd(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\batchdrawings2.dsd");
                    dsdFileData.WriteDsd(Path.ChangeExtension(path, "dsd")); //"\\batchdrawings2.dsd");
                    try
                    {
                        // Publish the specified drawing files in the DSD file, and
                        // honor the behavior of the BACKGROUNDPLOT system variable

                        using (DsdData dsdDataFile = new DsdData())
                        {
                            //dsdDataFile.ReadDsd(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\batchdrawings2.dsd");
                            dsdDataFile.ReadDsd(Path.ChangeExtension(path, "dsd")); // + "\\batchdrawings2.dsd");
                            // Get the DWG to PDF.pc3 and use it as a 
                            // device override for all the layouts
                            PlotConfig acPlCfg = PlotConfigManager.SetCurrentConfig("DWG to PDF.PC3");

                            Application.Publisher.PublishExecute(dsdDataFile, acPlCfg);
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception es)
                    {
                        MessengerManager.MessengerManager.LogException(es);
                    }
                }
            }
        }



        /// <summary>
        /// Publishes the views2 multi sheet.
        /// </summary>
        [CommandMethod("PGA-PublishViews2MultiSheet")]
        public static void PublishViews2MultiSheet()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var viewsToPlot = new StringCollection();
            viewsToPlot.Add("Test1");
            viewsToPlot.Add("Test2");
            viewsToPlot.Add("Current");
            // Create page setup based on the views
            using (var Tx = db.TransactionManager.StartTransaction())
            {
                var layoutId = LayoutManager.Current.GetLayoutId
                    (LayoutManager.Current.CurrentLayout);

                var layout
                    = Tx.GetObject(layoutId, OpenMode.ForWrite) as Layout;

                foreach (var viewName in viewsToPlot)
                {
                    var plotSettings
                        = new PlotSettings(layout.ModelType);
                    plotSettings.CopyFrom(layout);

                    var psv
                        = PlotSettingsValidator.Current;

                    psv.SetPlotConfigurationName
                    (plotSettings,
                        "DWF6 ePlot.pc3",
                        "ANSI_A_(8.50_x_11.00_Inches)"
                    );
                    psv.RefreshLists(plotSettings);
                    psv.SetPlotViewName(plotSettings, viewName);
                    psv.SetPlotType
                    (
                        plotSettings,
                        PlotType.View
                    );
                    psv.SetUseStandardScale(plotSettings, true);
                    psv.SetStdScaleType
                    (
                        plotSettings,
                        StdScaleType.ScaleToFit
                    );
                    psv.SetPlotCentered(plotSettings, true);
                    psv.SetPlotRotation
                    (
                        plotSettings,
                        PlotRotation.Degrees000
                    );
                    psv.SetPlotPaperUnits
                    (
                        plotSettings,
                        PlotPaperUnit.Inches
                    );

                    plotSettings.PlotSettingsName
                        = String.Format("{0}{1}", viewName, "PS");
                    plotSettings.PrintLineweights = true;
                    plotSettings.AddToPlotSettingsDictionary(db);

                    Tx.AddNewlyCreatedDBObject(plotSettings, true);
                    psv.RefreshLists(plotSettings);

                    layout.CopyFrom(plotSettings);
                }

                Tx.Commit();
            }

            //put the plot in foreground
            var bgPlot = (short)Application.GetSystemVariable("BACKGROUNDPLOT");
            Application.SetSystemVariable("BACKGROUNDPLOT", 0);

            var dwgFileName
                = Application.GetSystemVariable("DWGNAME") as string;
            var dwgPath
                = Application.GetSystemVariable("DWGPREFIX") as string;
            using (var Tx = db.TransactionManager.StartTransaction())
            {
                var collection = new DsdEntryCollection();
                var activeLayoutId
                    = LayoutManager.Current.GetLayoutId
                        (LayoutManager.Current.CurrentLayout);

                foreach (var viewName in viewsToPlot)
                {
                    var layout = Tx.GetObject(
                        activeLayoutId,
                        OpenMode.ForRead
                    ) as Layout;

                    var entry = new DsdEntry();

                    entry.DwgName = dwgPath + dwgFileName;
                    entry.Layout = layout.LayoutName;
                    entry.Title = viewName;
                    entry.NpsSourceDwg = entry.DwgName;
                    entry.Nps = String.Format("{0}{1}", viewName, "PS");

                    collection.Add(entry);
                }

                // remove the ".dwg" extension
                dwgFileName = dwgFileName.Substring(0, dwgFileName.Length - 4);

                var dsdData = new DsdData();

                dsdData.SheetType = SheetType.MultiDwf;
                dsdData.ProjectPath = dwgPath;
                dsdData.DestinationName
                    = dsdData.ProjectPath + dwgFileName + ".dwf";

                if (File.Exists(dsdData.DestinationName))
                    File.Delete(dsdData.DestinationName);

                dsdData.SetDsdEntryCollection(collection);

                var dsdFile
                    = dsdData.ProjectPath + dwgFileName + ".dsd";

                //Workaround to avoid promp for pdf file name
                //set PromptForDwfName=FALSE in dsdData using StreamReader/StreamWriter

                dsdData.WriteDsd(dsdFile);

                var sr = new StreamReader(dsdFile);
                var str = sr.ReadToEnd();
                sr.Close();

                // Replace PromptForDwfName
                str = str.Replace("PromptForDwfName=TRUE", "PromptForDwfName=FALSE");

                // Workaround to have the page setup names included in the DSD file
                // Replace Setup names based on the created page setups
                // May not be required if Nps is output to the DSD
                var occ = 0;
                var index = str.IndexOf("Setup=");
                var startIndex = 0;
                var dsdText = new StringBuilder();
                while (index != -1)
                {
                    // 6 for length of "Setup="
                    var str1 =
                        str.Substring(startIndex, index + 6 - startIndex);

                    dsdText.Append(str1);
                    dsdText.Append(
                        String.Format("{0}{1}", viewsToPlot[occ], "PS"));

                    startIndex = index + 6;
                    index = str.IndexOf("Setup=", index + 6);

                    if (index == -1)
                    {
                        dsdText.Append(
                            str.Substring(startIndex, str.Length - startIndex));
                    }
                    occ++;
                }

                // Write the DSD
                var sw
                    = new StreamWriter(dsdFile);
                sw.Write(dsdText.ToString());
                sw.Close();

                // Read the updated DSD file
                dsdData.ReadDsd(dsdFile);

                // Erase DSD as it is no longer needed
                File.Delete(dsdFile);

                var plotConfig
                    = PlotConfigManager.SetCurrentConfig("DWF6 ePlot.pc3");

                var publisher = Application.Publisher;

                // Publish it
                publisher.PublishExecute(dsdData, plotConfig);

                Tx.Commit();
            }

            //reset the background plot value
            Application.SetSystemVariable("BACKGROUNDPLOT", bgPlot);
        }

        [CommandMethod("PublisherDSD")]
        static public void PublisherDSD()
        {
            try
            {
                DsdEntryCollection collection = new DsdEntryCollection();
                DsdEntry entry;

                entry = new DsdEntry();
                entry.Layout = "Layout1";
                entry.DwgName = "c:\\Temp\\Drawing1.dwg";
                entry.Nps = "Setup1";
                entry.Title = "Sheet1";
                collection.Add(entry);

                entry = new DsdEntry();
                entry.Layout = "Layout1";
                entry.DwgName = "c:\\Temp\\Drawing2.dwg";
                entry.Nps = "Setup1";
                entry.Title = "Sheet2";
                collection.Add(entry);

                DsdData dsd = new DsdData();

                dsd.SetDsdEntryCollection(collection);

                dsd.ProjectPath = "c:\\Temp\\";
                dsd.LogFilePath = "c:\\Temp\\logdwf.log";
                dsd.SheetType = SheetType.MultiDwf;
                dsd.NoOfCopies = 1;
                dsd.DestinationName = "c:\\Temp\\PublisherTest.dwf";
                dsd.SheetSetName = "PublisherSet";
                dsd.WriteDsd("c:\\Temp\\publisher.dsd");


                int nbSheets = collection.Count;

                using (PlotProgressDialog progressDlg =
                    new PlotProgressDialog(false, nbSheets, true))
                {
                    progressDlg.set_PlotMsgString(
                        PlotMessageIndex.DialogTitle,
                        "Plot API Progress");
                    progressDlg.set_PlotMsgString(
                        PlotMessageIndex.CancelJobButtonMessage,
                        "Cancel Job");
                    progressDlg.set_PlotMsgString(
                        PlotMessageIndex.CancelSheetButtonMessage,
                        "Cancel Sheet");
                    progressDlg.set_PlotMsgString(
                        PlotMessageIndex.SheetSetProgressCaption,
                        "Job Progress");
                    progressDlg.set_PlotMsgString(
                        PlotMessageIndex.SheetProgressCaption,
                        "Sheet Progress");

                    progressDlg.UpperPlotProgressRange = 100;
                    progressDlg.LowerPlotProgressRange = 0;

                    progressDlg.UpperSheetProgressRange = 100;
                    progressDlg.LowerSheetProgressRange = 0;

                    progressDlg.IsVisible = true;

                    Autodesk.AutoCAD.Publishing.Publisher publisher =
                        Application.Publisher;

                    Autodesk.AutoCAD.PlottingServices.PlotConfigManager.
                        SetCurrentConfig("DWF6 ePlot.pc3");

                    publisher.PublishDsd(
                        "c:\\Temp\\publisher.dsd", progressDlg);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
            }
        }


        [CommandMethod("PublishExecute")]
        static public void PublishExecute()
        {
            try
            {
                string dsdFilePath = "c:\\Temp\\publisher.dsd";
                string dsdLogPath = "c:\\Temp\\logdwf.log";
                string dwfDestPath = "c:\\Temp\\PublisherTest.dwf";

                DsdEntryCollection collection = new DsdEntryCollection();
                DsdEntry entry;

                entry = new DsdEntry();
                entry.Layout = "Layout1";
                entry.DwgName = "c:\\Temp\\Drawing1.dwg";
                entry.NpsSourceDwg = entry.DwgName;
                entry.Title = "Sheet1";
                collection.Add(entry);

                entry = new DsdEntry();
                entry.Layout = "Layout1";
                entry.DwgName = "c:\\Temp\\Drawing2.dwg";
                entry.NpsSourceDwg = entry.DwgName;
                entry.Title = "Sheet2";
                collection.Add(entry);

                DsdData dsd = new DsdData();
                dsd.SetDsdEntryCollection(collection);
                dsd.IsSheetSet = true;
                dsd.LogFilePath = dsdLogPath;
                dsd.SheetType = SheetType.MultiDwf;
                dsd.NoOfCopies = 1;
                dsd.DestinationName = dwfDestPath;
                dsd.SheetSetName = "PublisherSet";
                dsd.WriteDsd(dsdFilePath);

                //Workaround to avoid promp for dwf file name
                //set PromptForDwfName=FALSE in dsdData using StreamReader/StreamWriter
                System.IO.StreamReader sr = new System.IO.StreamReader(dsdFilePath);
                string str = sr.ReadToEnd();
                sr.Close();

                str = str.Replace("PromptForDwfName=TRUE", "PromptForDwfName=FALSE");
                System.IO.StreamWriter sw = new System.IO.StreamWriter(dsdFilePath);
                sw.Write(str);
                sw.Close();

                dsd.ReadDsd(dsdFilePath);

                Autodesk.AutoCAD.PlottingServices.PlotConfigManager.SetCurrentConfig(
                    "DWF6 ePlot.pc3");

                AboutToBeginPublishingEventHandler Publisher_AboutToBeginPublishing = null;
                Application.Publisher.AboutToBeginPublishing +=
                    new Autodesk.AutoCAD.Publishing.AboutToBeginPublishingEventHandler(
                        Publisher_AboutToBeginPublishing);

                Application.Publisher.PublishExecute(
                    dsd,
                    Autodesk.AutoCAD.PlottingServices.PlotConfigManager.CurrentConfig);

                System.IO.File.Delete(dsdFilePath);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
            }
        }
    }
    public static class Commands

    {
        [CommandMethod("PGA-UpdateAttributes")]
        public static void UpdateAttributes()
        {
            AttributeManager.UpdateAttributesInDatabase(Active.Database, "Title_Block_2_ANSI_Expand_B", "Contact_Name", "test");
            AttributeManager.UpdateAttributesInDatabase(Active.Database, "Title_Block_2_ANSI_Expand_B", "Contact_Phone", "test");
            AttributeManager.UpdateAttributesInDatabase(Active.Database, "Title_Block_2_ANSI_Expand_B", "Contact_Email", "test");
            AttributeManager.UpdateAttributesInDatabase(Active.Database, "Title_Block_2_ANSI_Expand_B", "MAP_TITLE", "TOUR Mapping System");
            AttributeManager.UpdateAttributesInDatabase(Active.Database, "Title_Block_2_ANSI_Expand_B", "PAGENUM", "1");
            AttributeManager.UpdateAttributesInDatabase(Active.Database, "Title_Block_2_ANSI_Expand_B", "PLOT_DATE", DateTime.Now.ToShortDateString());
            AttributeManager.UpdateAttributesInDatabase(Active.Database, "Title_Block_2_ANSI_Expand_B", "COURSE_NAME", "SAWGRASS COUNTRY CLUB");

        }
    }

}
