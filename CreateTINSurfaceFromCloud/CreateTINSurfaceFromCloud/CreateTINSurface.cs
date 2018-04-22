// ***********************************************************************
// Assembly         : CreateTINSurfaceFromCloud
// Author           : Daryl Banks, PSM
// Created          : 01-16-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 09-20-2016
// ***********************************************************************
// <copyright file="CreateTINSurface.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Diagnostics;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices.Core;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using C3DApplicationDocuments;
using CreateTINSurfaceFromCloud.ACAD;
using PGA.MessengerManager;
using civildoc = Autodesk.Civil.DatabaseServices;
using System;
using PGA.SurfaceManager;

namespace CreateTINSurfaceFromCloud
{
    /// <summary>
    /// Class CreateTINSurface.
    /// </summary>
    public class CreateTINSurface
    {

        //[CommandMethod("CreateFromTIN")]
        //public void CreateFromTin()
        //{
        //    var civildoc = AcadApplictionDocument.GetMdiDocument();
        //    var editor = civildoc.Editor;

        //    using (
        //        Transaction ts =
        //            Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
        //    {
        //        // Example TIN surface from Civil Tutorials:
        //        string tinFile =
        //            @"C:\Program Files\Autodesk\AutoCAD Civil 3D 2013\Help\Civil Tutorials\Corridor surface.tin";
        //        try
        //        {
        //            Database db = Application.DocumentManager.MdiActiveDocument.Database;
        //            ObjectId tinSurfaceId = TinSurface.CreateFromTin(db, tinFile);
        //            editor.WriteMessage("Import succeeded: {0} \n {1}", tinSurfaceId.ToString(), db.Filename);
        //        }
        //        catch (System.Exception e)
        //        {
        //            // handle bad file path 
        //            editor.WriteMessage("Import failed: {0}", e.Message);
        //        }

        //        // commit the transaction
        //        ts.Commit();
        //    }
        //}

        //[CommandMethod("CreateTINSurface")]
        //public void CreateTINSurfaceByPointCollection()
        //{

        //    using (Transaction ts = AcadApplictionDocument.GetTransaction())
        //    {
        //        var doc = CivilApplicationManager.ActiveCivilDocument;

        //        string surfaceName = "All";
        //        // Select a style to use 

        //        if (GetSurfaceStyle("pga-tour-style") == null)
        //            SurfaceStyle();

        //        ObjectId surfaceStyleId = GetSurfaceStyle("pga-tour-style").ObjectId;

        //        if (surfaceStyleId == null) return;
        //        // Create the surface
        //        ObjectId surfaceId = TinSurface.Create(surfaceName, surfaceStyleId);

        //        TinSurface surface = surfaceId.GetObject(OpenMode.ForWrite) as TinSurface;

        //        // Add some random points
        //        Point3dCollection points = new Point3dCollection();
        //        points = ReadPointCloudFile.ReadFile(
        //            @"C:\Civil 3D Projects\PGA-8.23.2016\SV", "dtm06.txt");

        //        surface.AddVertices(points);

        //        // commit the create action
        //        ts.Commit();
        //    }


        //}

        /// <summary>
        /// Creates the tin surface by point collection.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="polylinelayer">The polylinelayer.</param>
        /// <returns>TinSurface.</returns>
        //public TinSurface CreateTINSurfaceByPointCollection(Point3dCollection points, string polylinelayer)
        //{
        //    TinSurface surface;

        //    using (Transaction ts = AcadApplictionDocument.GetTransaction())
        //    {
        //        var doc = CivilApplicationManager.ActiveCivilDocument;

        //        string surfaceName = polylinelayer;
        //        // Select a style to use 

        //        if (GetSurfaceStyle(polylinelayer) == null)
        //            SurfaceStyle();

        //        ObjectId surfaceStyleId = GetSurfaceStyle(polylinelayer).ObjectId;

        //        // Create the surface
        //        ObjectId surfaceId = TinSurface.Create(surfaceName, surfaceStyleId);

        //        surface = surfaceId.GetObject(OpenMode.ForWrite) as TinSurface;
        //        //surface.BuildOptions.MaximumTriangleLength = 20;
        //        //surface.BuildOptions.UseMaximumTriangleLength = true;
        //        SetDefaultBuildOptions(surface); ///Todo: 2.20.2017 
        //        // Add some random points
        //        //Point3dCollection points = new Point3dCollection();
        //        //points = ReadPointCloudFile.ReadFile(path, filename);


        //        surface.AddVertices(points);

        //        // commit the create action
        //        ts.Commit();
        //    }

        //    return surface;
        //}

        /// <summary>
        /// Creates the tin surface by point collection.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        public void CreateTINSurfaceByPointCollection(string path, string filename)
        {

            using (Transaction ts = AcadApplictionDocument.GetTransaction())
            {
                var doc = CivilApplicationManager.ActiveCivilDocument;

                string surfaceName = "All";

                // Select a style to use 
                ObjectId surfaceStyleId = ObjectId.Null;
                //NO-DISPLAY
                //if (GetSurfaceStyle("NO-DISPLAY") != null)
                //{
                //    if (GetSurfaceStyle("NO-DISPLAY") == null)
                //        SurfaceStyle();

                //    surfaceStyleId = GetSurfaceStyle("NO-DISPLAY").ObjectId;
                //}
                if (GetSurfaceStyle("Standard") != null)
                {
                    if (GetSurfaceStyle("Standard") == null)
                        SurfaceStyle();

                    surfaceStyleId = GetSurfaceStyle("Standard").ObjectId;
                }
                // Create the surface
                ObjectId surfaceId = TinSurface.Create(surfaceName, surfaceStyleId);

                TinSurface surface = surfaceId.GetObject(OpenMode.ForWrite) as TinSurface;

                surface.DowngradeOpen();
                SetDefaultBuildOptions(surface); ///Todo: 2.20.2017 
                surface.UpgradeOpen();
                
                //surface.BuildOptions.MaximumTriangleLength = 200;
                //surface.BuildOptions.UseMaximumTriangleLength = true;
                //// Add some LiDAR points
                Point3dCollection points = new Point3dCollection();
                points = ReadPointCloudFile.ReadFile(path, filename);

                surface.AddVertices(points);

                SetSmoothing(surface, points);

                // commit the create action
                ts.Commit();
            }


        }

        /// <summary>
        /// Smoothes the contours.
        /// </summary>
        /// <param name="surfaceStyleId">The surface style identifier.</param>
        /// <returns>SurfaceStyle.</returns>
        public SurfaceStyle SmoothContours(ObjectId surfaceStyleId )
        {
            try
            {
                SurfaceStyle surfaceStyle = null;
                using (Transaction ts = CivilApplicationManager.StartTransaction())
                {
                    surfaceStyle = surfaceStyleId.GetObject(
                        OpenMode.ForWrite) as SurfaceStyle;

                    surfaceStyle.ContourStyle.SmoothContours = true;
                    surfaceStyle.ContourStyle.SmoothingType
                        = ContourSmoothingType.AddVertices;
                    surfaceStyle.ContourStyle.SmoothingFactor = 10;

                    ts.Commit();
                }
                return surfaceStyle;

            }
            catch (System.Exception ex)
            {

                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return null;
        }

        public void SetSmallTrianglesAllSurfaces()
        {

            var surfaces = SurfaceManager.GetSurfacesList();
            foreach (var surface in surfaces)
            {
                try
                {
                    // Filter List

                    if (surface.Name.Contains("GREEN") ||
                        surface.Name.Contains("COLLAR") ||
                        surface.Name.Contains("BUNKER") ||
                        surface.Name.Contains("BRIDGE") ||
                        surface.Name.Contains("INTERMEDIATE-ROUGH"))
                    {
                        surface.BuildOptions.MaximumTriangleLength = Math.Sqrt((Math.Pow(0.5, 2)) + Math.Pow(0.5, 2));
                        surface.BuildOptions.UseMaximumTriangleLength = true;
                    }
                }
                catch (System.Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.AddLog(
                        String.Format("SetSmallTrianglesAllSurfaces All Failed {0}", ex));
                }
            }
        }

        public void SmoothAllSurfaces()
        {

            var surfaces = SurfaceManager.GetSurfacesList();
            foreach (var surface in surfaces)
            {
                try
                {
                    // Filter List

                    if (surface.Name.Contains("GREEN") ||
                               surface.Name.Contains("COLLAR") ||
                               surface.Name.Contains("BUNKER") ||
                               surface.Name.Contains("BRIDGE"))
                    {
                        SetSmoothing(surface);
                    }
                }
                catch (System.Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.AddLog(String.Format("Smoothing All Region Failed {0}", ex));
                }
            }
        }

        public TinSurface SetSmoothing(TinSurface surface)
        {
            try
            {
                var points   = new Point3dCollection();
                var polyline = PGA.SurfaceManager.SurfaceManager.GetOuterBoundary(surface);

                if (polyline != null)
                {
                    using (Transaction tr = CivilApplicationManager.StartTransaction())
                    {
                        foreach (PolylineVertex3d v3d in polyline)
                        {
                            points.Add(v3d.Position);
                        }
                        tr.Commit();
                    }
                    SetSmoothing(surface, points);
                }
                else
                {
                    PGA.MessengerManager.MessengerManager.AddLog(String.Format("Smoothing Region Failed to get Points!"));
                }

            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog(String.Format("Smoothing Region Failed {0}", ex));
            }


            return null;
        }


        /// <summary>
        /// Sets the smoothing to 0.5 ft for select surfaces
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="points">The points.</param>
        /// <returns>TinSurface.</returns>
        public TinSurface SetSmoothing(TinSurface surface, Point3dCollection points)
        {
            try
            {

                PGA.MessengerManager.MessengerManager.AddLog("Start Smoothing for " + surface.Name);


                //Smooth Critical Surfaces//

                if (surface.Name.Contains("GREEN") ||
                     surface.Name.Contains("COLLAR") ||
                     surface.Name.Contains("BUNKER") ||
                     surface.Name.Contains("BRIDGE"))
                {

                    using (Transaction ts = CivilApplicationManager.StartTransaction())
                    {
                        //try
                        //{
                        //    if (!surface.IsWriteEnabled)
                        //        surface.UpgradeOpen();
                        //}
                        //catch (System.Exception ex)
                        //{
                        //    MessengerManager.AddLog("Open for Write Failed: " + surface.Name);

                        //}

                        SurfacePointOutputOptions pointOutputOptions =
                            new SurfacePointOutputOptions();
                        pointOutputOptions.GridSpacingX = 0.5;
                        pointOutputOptions.GridSpacingY = 0.5;
                        pointOutputOptions.OutputLocations =
                            SurfacePointOutputLocationsType.GridBased;
                        pointOutputOptions.OutputRegions = new Point3dCollection[]
                        {points};

                        SurfaceOperationSmooth op =
                            surface.SmoothSurfaceByNNI(pointOutputOptions);

                        ts.Commit();
                    }
                    PGA.MessengerManager.MessengerManager.AddLog("End Smoothing for " + surface.Name);

                }
                return surface;


            }
            catch (System.Exception ex)
            {

                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the surface style.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>SurfaceStyle.</returns>
        public SurfaceStyle GetSurfaceStyle(string name)
        {
            using (Transaction ts = CivilApplicationManager.StartTransaction())
            {
                var doc = CivilApplicationManager.ActiveCivilDocument;

                int total = doc.Styles.SurfaceStyles.Count();
                for (int i = 0; i < total; i++)
                {
                    ObjectId surfaceStyleId = doc.Styles.SurfaceStyles[i]; // ("pga-tour-style");
                    SurfaceStyle s = surfaceStyleId.GetObject(OpenMode.ForRead) as SurfaceStyle;
                    var result = s.Name == name;
                    if (result)
                    {
                        return s;
                    }
                    Debug.WriteLine(s.Name);
                }
            }
            return null;
        }

        /// <summary>
        /// Surfaces the style.
        /// </summary>
        [CommandMethod("SurfaceStyle")]
        public void SurfaceStyle()
        {
            using (
                Transaction ts =
                    Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                var doc = CivilApplicationManager.ActiveCivilDocument;

                // create a new style called 'example style':
                ObjectId styleId = doc.Styles.SurfaceStyles.Add("pga-tour-style");

                // modify the style:
                SurfaceStyle surfaceStyle = styleId.GetObject(OpenMode.ForWrite) as SurfaceStyle;

                // display surface triangles
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Triangles).Visible = true;
                surfaceStyle.GetDisplayStyleModel(SurfaceDisplayStyleType.Triangles).Visible = true;

                // display boundaries, exterior only:
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Boundary).Visible = true;
                surfaceStyle.BoundaryStyle.DisplayExteriorBoundaries = true;
                surfaceStyle.BoundaryStyle.DisplayInteriorBoundaries = false;

                // display major contours:
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.MajorContour).Visible = true;

                // turn off display of other items:
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.MinorContour).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.UserContours).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Directions).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Elevations).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Slopes).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.SlopeArrows).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Watersheds).Visible = false;

                // do the same for all model display settings as well


                // commit the transaction
                ts.Commit();
            }
        }

        /// <summary>
        /// Surfaces the style and attach.
        /// </summary>
        public void SurfaceStyleAndAttach()
        {
            using (
                Transaction ts =
                    Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                var doc = CivilApplicationManager.ActiveCivilDocument;

                // create a new style called 'example style':
                ObjectId styleId = doc.Styles.SurfaceStyles.Add("pga-tour-style");

                // modify the style:
                SurfaceStyle surfaceStyle = styleId.GetObject(OpenMode.ForWrite) as SurfaceStyle;

                // display surface triangles
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Triangles).Visible = true;
                surfaceStyle.GetDisplayStyleModel(SurfaceDisplayStyleType.Triangles).Visible = true;

                // display boundaries, exterior only:
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Boundary).Visible = true;
                surfaceStyle.BoundaryStyle.DisplayExteriorBoundaries = true;
                surfaceStyle.BoundaryStyle.DisplayInteriorBoundaries = false;

                // display major contours:
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.MajorContour).Visible = true;

                // turn off display of other items:
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.MinorContour).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.UserContours).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Directions).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Elevations).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Slopes).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.SlopeArrows).Visible = false;
                surfaceStyle.GetDisplayStylePlan(SurfaceDisplayStyleType.Watersheds).Visible = false;

                // do the same for all model display settings as well


                // assign the style to the first surface in the document:
                TinSurface surf = doc.GetSurfaceIds()[0].GetObject(OpenMode.ForWrite) as TinSurface;
                surf.StyleId = styleId;

                // commit the transaction
                ts.Commit();
            }
        }

        /// <summary>
        /// Sets the surface style to hidden.
        /// </summary>
        /// <param name="surfaceId">The surface identifier.</param>
        public void SetSurfaceStyleToHidden(ObjectId surfaceId)
        {
            using (Transaction ts = AcadApplictionDocument.GetTransaction())
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {

                    var doc = CivilApplicationManager.ActiveCivilDocument;


                    // Select a style to use 
                    ObjectId surfaceStyleId = ObjectId.Null;
                    //NO-DISPLAY
                    if (GetSurfaceStyle("NO-DISPLAY") != null)
                    {
                        if (GetSurfaceStyle("NO-DISPLAY") == null)
                            SurfaceStyle();

                        surfaceStyleId = GetSurfaceStyle("NO-DISPLAY").ObjectId;
                    }
                    else
                    {
                        if (GetSurfaceStyle("Standard") == null)
                            SurfaceStyle();

                        surfaceStyleId = GetSurfaceStyle("Standard").ObjectId;
                    }

                    TinSurface surface = surfaceId.GetObject(OpenMode.ForWrite) as TinSurface;

                    if (surface != null) surface.StyleId = surfaceStyleId;

                    // commit the create action
                    ts.Commit();
                }
            }
        }
        /// <summary>
        /// Sets the surface style to standard.
        /// </summary>
        /// <param name="surfaceId">The surface identifier.</param>
        public void SetSurfaceStyleToStandard(ObjectId surfaceId)
        {
            using (Transaction ts = AcadApplictionDocument.GetTransaction())
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {

                    var doc = CivilApplicationManager.ActiveCivilDocument;


                    // Select a style to use 
                    ObjectId surfaceStyleId = ObjectId.Null;

                    if (GetSurfaceStyle("Standard") != null)
                    {
                        if (GetSurfaceStyle("Standard") == null)
                            SurfaceStyle();

                        surfaceStyleId = GetSurfaceStyle("Standard").ObjectId;
                    }


                    TinSurface surface = surfaceId.GetObject(OpenMode.ForWrite) as TinSurface;

                    if (surface != null) surface.StyleId = surfaceStyleId;

                    // commit the create action
                    ts.Commit();
                }
            }
        }

        /// <summary>
        /// Elevations the in surface range.
        /// </summary>
        /// <param name="elevation">The elevation.</param>
        /// <param name="surface">The surface.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool elevationInSurfaceRange(double elevation, TinSurface surface)
        {
            GeneralSurfaceProperties properties =
                surface.GetGeneralProperties();
            if (elevation < properties.MinimumElevation ||
                elevation > properties.MaximumElevation)
            {
                ////_editor.WriteMessage(
                //"\nSpecified elevation not in surface range.")
                //;
                return false;
            }
            return true;

        }

        /// <summary>
        /// Triangles the distance in surface range.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="surface">The surface.</param>
        private void TriangleDistanceInSurfaceRange(double distance, TinSurface surface)
        {
            surface.BuildOptions.MaximumTriangleLength = distance;
            ////_editor.WriteMessage(
            //"\nSpecified elevation not in surface range.")
            //;
        }

        /// <summary>
        /// Sets the default build options.
        /// Date: 2/19/2017
        /// </summary>
        /// <param name="surface">The surface.</param>
        public void SetDefaultBuildOptions(TinSurface surface)
        {
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (var tr = CivilApplicationManager.StartTransaction())
                    {
                        MessengerManager.AddLog("Start Setting Build Options");
 
                        if (surface == null)
                            return;

                        surface.UpgradeOpen();
                        ///Todo:Handle Breaklines for Water and Bulkhead-Added 3.7.17
                        /// 
                        if (surface.Name.Contains("WATER"))
                            surface.BuildOptions.MaximumTriangleLength = 100;
                        else if (surface.Name.Contains("All"))
                            surface.BuildOptions.MaximumTriangleLength = 200;
                        else
                            surface.BuildOptions.MaximumTriangleLength = 20;

                        surface.BuildOptions.UseMaximumTriangleLength = true;
                        surface.BuildOptions.NeedConvertBreaklines = true;
                        surface.BuildOptions.ExecludeMinimumElevation = true;
                        surface.BuildOptions.MinimumElevation = 0.1; // Possible negative elevation
                        surface.BuildOptions.CrossingBreaklinesElevationOption =
                            CrossingBreaklinesElevationType.UseLast; ///Todo: Changed 2/19/17 from None.
                        surface.DowngradeOpen();

                        tr.Commit();
                        MessengerManager.AddLog("End Setting Build Options");
                        MessengerManager.AddLog("Surface Settings: " + surface.Name);

                    }
                }
            }
            catch (System.Exception e)
            {
                MessengerManager.LogException(e);
            }
        }

    }
}


