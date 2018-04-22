// ***********************************************************************
// Assembly         : PGA.SurfaceManager
// Author           : Daryl Banks, PSM
// Created          : 08-09-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 02-11-2018
// ***********************************************************************
// <copyright file="SurfaceManager.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices.Core;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using COMS = PGA.MessengerManager;
using Exception = System.Exception;
using BBC.Common.AutoCAD;
using PGA.OffsetPolylines;

namespace PGA.SurfaceManager
{
    /// <summary>
    /// Class SurfaceManager.
    /// </summary>
    public static class SurfaceManager
    {
        /// <summary>
        /// The surface collection
        /// </summary>
        private static ObjectIdCollection surfaceCollection;
        /// <summary>
        /// The polyline collection
        /// </summary>
        private static ObjectIdCollection polylineCollection;
        /// <summary>
        /// The tin surfaces
        /// </summary>
        private static List<TinSurface> tinSurfaces;
        /// <summary>
        /// The polylines
        /// </summary>
        private static List<Polyline> thePolylines;

        /// <summary>
        /// The g count
        /// </summary>
        public static int gCount;
        /// <summary>
        /// The g error
        /// </summary>
        public static int gError;
        /// <summary>
        /// The g outer area
        /// </summary>
        public static double gOuterArea;

        /// <summary>
        /// The iterations
        /// </summary>
        public static int iterations = 0;


        /// <summary>
        /// Gets the surfaces.
        /// </summary>
        [CommandMethod("PGA-GetSurfaces")]
        public static void GetSurfaces()
        {
            surfaceCollection = new ObjectIdCollection();
            tinSurfaces = new List<TinSurface>();
            gCount = 0; //index for global access
            gError = 0; //index of error surface

            // Get the various active objects
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = Active.Database;
            // Create a new transaction
            using (var tr = Active.StartTransaction())
            {
                // Get the block table for the curr
                var blockTable =
                    (BlockTable) tr.GetObject(
                        database.BlockTableId, OpenMode.ForRead);
                // Get the model space block table record
                var modelSpace =
                    (BlockTableRecord) tr.GetObject(
                        blockTable[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead);

                var SurfaceClass = RXObject.GetClass(typeof (TinSurface));

                // Loop through the entities in model space
                foreach (var objectId in modelSpace)
                {
                    // Look for circles
                    if (objectId.ObjectClass.IsDerivedFrom(SurfaceClass))
                    {
                        surfaceCollection.Add(objectId);

                        var surface =
                            (TinSurface) tr.GetObject(
                                objectId, OpenMode.ForRead);
                        if (surface != null)
                            tinSurfaces.Add(surface);
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Gets the surfaces list.
        /// </summary>
        /// <returns>List&lt;TinSurface&gt;.</returns>
        public static List<TinSurface> GetSurfacesList()
        {
            surfaceCollection = new ObjectIdCollection();
            tinSurfaces = new List<TinSurface>();
            gCount = 0; //index for global access
            gError = 0; //index of error surface

            // Get the various active objects
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = Active.Database;
            // Create a new transaction
            using (var tr = Active.StartTransaction())
            {
                // Get the block table for the curr
                var blockTable =
                    (BlockTable) tr.GetObject(
                        database.BlockTableId, OpenMode.ForRead);
                // Get the model space block table record
                var modelSpace =
                    (BlockTableRecord) tr.GetObject(
                        blockTable[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead);

                var SurfaceClass = RXObject.GetClass(typeof (TinSurface));

                // Loop through the entities in model space
                foreach (var objectId in modelSpace)
                {
                    // Look for surfaces
                    if (objectId.ObjectClass.IsDerivedFrom(SurfaceClass))
                    {
                        surfaceCollection.Add(objectId);

                        var surface =
                            (TinSurface) tr.GetObject(
                                objectId, OpenMode.ForRead);
                        if (surface != null)
                            tinSurfaces.Add(surface);
                    }
                }

                tr.Commit();
            }
            return tinSurfaces;
        }

        /// <summary>
        /// Gets the polyline list.
        /// </summary>
        /// <returns>List&lt;Polyline&gt;.</returns>
        public static List<Polyline> GetPolylineList()
        {
            polylineCollection = new ObjectIdCollection();
            thePolylines = new List<Polyline>();

            // Get the various active objects
            var document = Application.DocumentManager.MdiActiveDocument;
            var database = Active.Database;
            // Create a new transaction
            using (var tr = Active.StartTransaction())
            {
                // Get the block table for the curr
                var blockTable =
                    (BlockTable)tr.GetObject(
                        database.BlockTableId, OpenMode.ForRead);
                // Get the model space block table record
                var modelSpace =
                    (BlockTableRecord)tr.GetObject(
                        blockTable[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead);

                var PolyClass = RXObject.GetClass(typeof(Polyline));

                // Loop through the entities in model space
                foreach (var objectId in modelSpace)
                {
                    // Look for circles
                    if (objectId.ObjectClass.IsDerivedFrom(PolyClass))
                    {
                        polylineCollection.Add(objectId);

                        var pline =
                            (Polyline)tr.GetObject(
                                objectId, OpenMode.ForRead);
                        if (pline != null)
                            thePolylines.Add(pline);
                    }
                }

                tr.Commit();
            }
            return thePolylines;
        }


        /// <summary>
        /// Gets the terrain surface properties.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns>TerrainSurfaceProperties.</returns>
        public static TerrainSurfaceProperties GetTerrainSurfaceProperties(TinSurface theSurface)
        {
            try
            {
                var p = theSurface.GetTerrainProperties();

                return p;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("triangle"))
                {
                    PGA.MessengerManager.MessengerManager.AddLog("*************INSPECT SURFACE*************");
                    PGA.MessengerManager.MessengerManager.AddLog("*************"+ theSurface.Name +"*************");
                    PGA.MessengerManager.MessengerManager.AddLog("*************INSPECT SURFACE*************");
                }
                PGA.MessengerManager.MessengerManager.LogException(ex.Message, ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the terrain surface properties.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>TerrainSurfaceProperties.</returns>
        public static TerrainSurfaceProperties GetTerrainSurfaceProperties(string name)
        {
            try
            {

                var surface = SurfaceManager.tinSurfaces.Find(s=>s.Name == name);

                var p = surface.GetTerrainProperties();

                return p;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("triangle"))
                {
                    PGA.MessengerManager.MessengerManager.AddLog("*************INSPECT SURFACE*************");
                    PGA.MessengerManager.MessengerManager.AddLog("*************" + name + "*************");
                    PGA.MessengerManager.MessengerManager.AddLog("*************INSPECT SURFACE*************");
                }
                PGA.MessengerManager.MessengerManager.LogException(ex.Message, ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the tin surface properties.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns>TinSurfaceProperties.</returns>
        public static TinSurfaceProperties GetTinSurfaceProperties(TinSurface theSurface)
        {
            try
            {
                var tin = theSurface.GetTinProperties();

                return tin;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("triangle"))
                {
                    PGA.MessengerManager.MessengerManager.AddLog("*************INSPECT SURFACE*************");
                    PGA.MessengerManager.MessengerManager.AddLog("*************" + theSurface.Name + "*************");
                    PGA.MessengerManager.MessengerManager.AddLog("*************INSPECT SURFACE*************");
                }
                PGA.MessengerManager.MessengerManager.LogException(ex.Message, ex);
            }
            return null;
        }

        /// <summary>
        /// Checks the statistics.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckStatistics(TinSurface theSurface)
        {
            try
            {
                
                var p = GetTerrainSurfaceProperties(theSurface);
                var tin = GetTinSurfaceProperties(theSurface);

                if (p == null || tin == null )
                {
                    COMS.MessengerManager.AddLog(BuildSurfaceString(theSurface));
                    return true;
                }
                COMS.MessengerManager.AddLog("Audit: No Issues for Surface " + theSurface.Name);
                return false;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return false;
        }

        /// <summary>
        /// Builds the surface string.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns>System.String.</returns>
        private static string BuildSurfaceString(TinSurface theSurface)
        {
            try
            {
                var n = theSurface.Name;
                var l = theSurface.Layer;
                var a = theSurface.Area;

                return string.Format("Surface Failure Information: Name={0}, Layer={1}, Area={2}", n, l, a);
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return "Surface Information Failed";
        }

        /// <summary>
        /// Checks the elevation statistics.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void CheckElevationStatistics(TinSurface theSurface)
        {
            try
            {
                var p = theSurface.GetGeneralProperties();

                if (p != null)
                {
                    var MaxElev = p.MaximumElevation;
                    var MinElev = p.MinimumElevation;


                    if (Math.Abs(MaxElev - MinElev) > 50.0)
                    {
                        COMS.MessengerManager.AddLog("Elev-Exceeded-"
                                                     + BuildSurfaceString(theSurface));
                    }
                }
                else
                {
                    COMS.MessengerManager.AddLog("Elev-Stats-"
                                                 + BuildSurfaceString(theSurface));
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Gets the surface operation defs.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns>SurfaceOperationCollection.</returns>
        public static SurfaceOperationCollection GetSurfaceOperationDefs(TinSurface theSurface)
        {
            var sbd = theSurface.BoundariesDefinition;
            sbd[0].Enabled = false;
            var soc = theSurface.Operations;

            return soc;
        }

        /// <summary>
        /// Processes the surface ops.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void ProcessSurfaceOps(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    var sbd = theSurface.BoundariesDefinition;

                    for (var i = gCount; i < sbd.Count; i++)
                    {
                        if (CheckStatistics(theSurface))
                        {
                            COMS.MessengerManager.AddLog("Checking..." + theSurface.Name);
                            DisableSurfaceBoundaries(theSurface, gCount);
                        }
                       
                        gCount++;
                    }
                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Disables the surface boundaries.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void DisableSurfaceBoundaries(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    var num = 0;
                    var sbd = theSurface.BoundariesDefinition;

                    num = sbd.Count;


                    for (var i = gCount; i < num; i++)
                    {
                        if (sbd[i].BoundaryType != SurfaceBoundaryType.Outer)
                        {
                            sbd[i].Enabled = false;
                            gCount = i;

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Disables the surface boundaries.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <param name="index">The index.</param>
        public static void DisableSurfaceBoundaries(TinSurface theSurface, int index)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    var sbd = theSurface.BoundariesDefinition;
                    COMS.MessengerManager.AddLog("Boundary Count..." + sbd.Count);

                    if (gOuterArea == 0.0)
                        CreateOuterArea(sbd[index]
                            .Cast<SurfaceBoundary>().FirstOrDefault());
                


                    if (sbd[index].BoundaryType != SurfaceBoundaryType.Outer)
                    {
                        PGA.MessengerManager.MessengerManager.AddLog
                            ("Starting Disable Inner Surface for " + theSurface.Name);

                        var innerArea = CreateInnerArea(sbd[index]
                            .Cast<SurfaceBoundary>().FirstOrDefault());

                        if (innerArea >= gOuterArea || innerArea > 0.75*gOuterArea)
                        {
                            PGA.MessengerManager.MessengerManager.
                                AddLog("Disabled!");

                            sbd[index].Enabled = false;
                        }
                        else
                        {
                            sbd[index].Enabled = true;
                            PGA.MessengerManager.MessengerManager.
                                AddLog("Enabled!");

                        }
                    }
                    else
                        CreateOuterArea(sbd[index]
                            .Cast<SurfaceBoundary>().FirstOrDefault());


                    PGA.MessengerManager.MessengerManager.AddLog
                        ("Ending Disable Inner Surface for " + theSurface.Name);
                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Disables the surface. Case # 6
        /// Hide Bndy = Total Surface Bndy
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void DisableSurfaceByArea(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                var OuterArea = gOuterArea;
                var InnerArea = 0.0;
                COMS.MessengerManager.AddLog("Surface Name =" + theSurface.Name);
                try
                {
                    var sbd = theSurface.BoundariesDefinition;

                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Hide)
                        {
                            if (sbd[i].Enabled == true)
                            {
                                var innerArea = CreateInnerArea(sbd[i]
                                    .Cast<SurfaceBoundary>().FirstOrDefault());

                                InnerArea = innerArea;
                                COMS.MessengerManager.AddLog("Inner Area Value =" + InnerArea);

                                if (Math.Abs(OuterArea - InnerArea) < 0.1)
                                {
                                    sbd[i].Enabled = false;

                                }

                            }

                        } 
                                                                                    
                    }
                   

                    COMS.MessengerManager.AddLog("Inner Area Check =" + InnerArea);
                    COMS.MessengerManager.AddLog("Outer Area Check =" + OuterArea);


                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
        }


        /// <summary>
        /// Disables the surface.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void DisableSurface(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                var OuterArea = gOuterArea;
                var InnerArea = 0.0;

                var OuterLayer = theSurface.Layer;
                try
                {
                   
                    var sbd = theSurface.BoundariesDefinition;

                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Outer)
                        {
                            OuterArea = CreateOuterBoundary(sbd[i]
                                .Cast<SurfaceBoundary>()
                                .FirstOrDefault()).Area;
                        }
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Hide)
                        {
                            if (sbd[i].Enabled == true)
                            {
                                var innerArea = CreateInnerArea(sbd[i]
                                    .Cast<SurfaceBoundary>().FirstOrDefault());

                                COMS.MessengerManager.AddLog("Inner Area Value =" + innerArea);

                                InnerArea =  InnerArea + innerArea;
                            }

                        }

                    }
                    if (Math.Abs(OuterArea - InnerArea) < 0.1 || InnerArea > OuterArea)
                    {
                        //change to No-Display
                        UpdateSurfaceStyle(theSurface);

                    }

                    COMS.MessengerManager.AddLog("Inner Area Check =" + InnerArea);
                    COMS.MessengerManager.AddLog("Outer Area Check =" + OuterArea);


                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
        }


        /// <summary>
        /// Creates the inner area.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.Double.</returns>
        public static double CreateInnerArea(SurfaceBoundary s)
        {
            try
            {
                return new Polyline3d(Poly3dType.SimplePoly, s.Vertices, true).Area;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return 0.0;
        }


        /// <summary>
        /// Creates the outer area.
        /// </summary>
        /// <param name="s">The s.</param>
        private static void CreateOuterArea(SurfaceBoundary s)
        {
            try
            {
                gOuterArea = new Polyline3d(Poly3dType.SimplePoly, s.Vertices, true).Area;
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Enables the surface boundaries.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void EnableSurfaceBoundaries(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    var num = 0;
                    var sbd = theSurface.BoundariesDefinition;

                    num = sbd.Count;

                    for (var i = gCount; i < num; i++)
                    {
                        sbd[i].Enabled = true;
                        gCount = i;
                    }
                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Enables the surface boundaries.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <param name="index">The index.</param>
        public static void EnableSurfaceBoundaries(TinSurface theSurface, int index)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    var sbd = theSurface.BoundariesDefinition;

                    sbd[index].Enabled = true;
                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Tests the isolated parent.
        /// </summary>
        [CommandMethod("PGA-TestIsolatedParent")]
        public static void TestIsolatedParent()
        {
            foreach (var item in GetSurfacesList())
            {

                var t = GetTerrainSurfaceProperties(item);
                var tin = GetTinSurfaceProperties(item);

                if (t == null || tin == null)
                {
                    COMS.MessengerManager.AddLog("TIN Surface Yielded Null!");
                    continue;
                }

                if (t.SurfaceArea2D == 0.0)
                    continue;

                COMS.MessengerManager.AddLog("*******Testing Surface*********");
                COMS.MessengerManager.AddLog("Start Testing Surface= " + item.Name);
                COMS.MessengerManager.AddLog("Surface Area = "  + t.SurfaceArea2D);
                COMS.MessengerManager.AddLog("Surface Style= " + item.StyleName);

                if (item.StyleName == "NO-DISPLAY")
                    continue;

                if (!IsIsolatedParent(item))
                {
                    COMS.MessengerManager.AddLog("ISOLATED PARENT==> YES: " + item.Name);
                    UpdateSurfaceStyle(item);

                }
                else
                    COMS.MessengerManager.AddLog("ISOLATED PARENT==> NO: " + item.Name);
  


                COMS.MessengerManager.AddLog("*******End Testing*********");

            }
        }

        /// <summary>
        /// Gets the style identifier.
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <returns>ObjectId.</returns>
        public static ObjectId GetStyleId(string styleName)
        {
            using (Transaction tr = Active.StartTransaction())
            {
                CivilDocument doc = Active.ActiveCivilDocument;
                SurfaceStyleCollection styles = doc.Styles.SurfaceStyles;
                foreach (ObjectId styleId in styles)
                {
                    SurfaceStyle style = styleId.GetObject(OpenMode.ForRead) as SurfaceStyle;
                    if (styleName == style.Name)
                    {
                        tr.Commit();
                        return styleId;
                    }
                }
                tr.Abort();
            }

            return ObjectId.Null;
        }

        /// <summary>
        /// Updates the surface style.
        /// </summary>
        /// <param name="surface">The surface.</param>
        private static void UpdateSurfaceStyle(TinSurface surface)
        {
            try
            {
                using (var tr = Active.StartTransaction())
                {
                    //if (!surface.IsWriteEnabled)
                    //    surface.UpgradeOpen();

                    {
                        var styleId = GetStyleId("NO-DISPLAY");

                        surface.StyleId = styleId;
                    }
                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Determines whether [is over lapping parent] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [is over lapping parent] [the specified item]; otherwise, <c>false</c>.</returns>
        private static bool IsOverLappingParent(TinSurface item)
        {
            return false;
        }

        /// <summary>
        /// Determines whether [is isolated parent] [the specified the surface].
        /// Iso Parent not have sub surfaces on same layer within boundary
        /// Iso Parent has sub surfaces on other layers within boundary
        /// We are looking for surfaces with NO inner boundaries
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns><c>true</c> if [is isolated parent] [the specified the surface]; otherwise, <c>false</c>.</returns>
        public static bool IsIsolatedParent(TinSurface theSurface)
        {
            try
            {
                thePolylines = GetPolylineList();


                var layer = theSurface.Layer;
                var outerboundary = GetOuterBoundaryCollection(theSurface);

                if (outerboundary == null)
                    return false;

                var co = AcadUtilities.GetCentriod(outerboundary);

                if (co == null)
                    return false;

                if (!HasInnerBoundary(theSurface))
                {
                    var t = GetTerrainSurfaceProperties(theSurface);

                    foreach (Polyline item in thePolylines)
                    {
                        // Check for Equal Areas

                        COMS.MessengerManager.AddLog("Surface Area= " + t.SurfaceArea2D);
                        COMS.MessengerManager.AddLog("Polyline Area= " + item.Area);

                        if (Math.Abs(t.SurfaceArea2D - item.Area) < 2.0)
                            continue;
                        if ((t.SurfaceArea2D < item.Area))
                            continue;

                        var p = AcadUtilities.GetPointsFromPolyline(item);
                        var ci = AcadUtilities.GetCentriod(p);

                        var boundary = outerboundary.ToArray().ToList();

                        if (PointInPolyline(outerboundary,p))
                        {
                            COMS.MessengerManager.AddLog
                                ("We Do NOT have Isolated Parent! " 
                                + theSurface.Name);

                            COMS.MessengerManager.AddLog
                              ("Polyline Layer! "
                              + item.Layer);
                            return false;
                        }

                    }
                    COMS.MessengerManager.AddLog
                        ("We DO have Isolated Parent! Running Correction " 
                        + theSurface.Name);

                    CorrectIsolatedParent(theSurface)
                    ;
                    return true;
                }

            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return true;
        }

        /// <summary>
        /// Corrects the isolated parent.
        /// </summary>
        [CommandMethod("PGA-TestCorrectIsolatedParent")]

        public static void CorrectIsolatedParent()
        {

            try
            {
                var theSurfaces = GetSurfacesList();

                foreach (var theSurface in theSurfaces)
                {
                    COMS.MessengerManager.AddLog("Starting CorrectIsolatedParent" + theSurface.Name);


                    using (var tr = Active.StartTransaction())
                    {
                        thePolylines = GetPolylineList();


                        var sbd = theSurface.BoundariesDefinition;
                        var allSurface = GetTerrainSurfaceProperties("All");
                        var allArea = allSurface.SurfaceArea2D;
                        var curArea =  GetTerrainSurfaceProperties(theSurface.Name).SurfaceArea2D; 
                        
                        Point2dCollection pnts = new Point2dCollection();


                        if (theSurface.Name == "All")
                            continue;


                        var roughcal = Math.Abs((allArea - curArea) / allArea) * 100;
                        var result   = roughcal < 0.10? true: false;


                        if (result)
                        {
                            COMS.MessengerManager.AddLog("Entering CorrectIsolatedParent = " + roughcal);

                            var layer = theSurface.Layer;
                            var outerboundary = GetOuterBoundaryCollection(theSurface);

                            if (outerboundary == null)
                                return;


                            RemoveOuterBoundary(theSurface);

                            SurfaceDefinitionBoundaries surfaceBoundaries =
                                theSurface.BoundariesDefinition;

                            var newoid = PGA.OffsetPolylines.Program.ScaleNewPolylineObject(outerboundary);

                            var polyline = tr.GetObject(newoid, OpenMode.ForWrite) as Polyline;
                            polyline.Layer = layer;
                            polyline.Closed = true;

                            for (int i = 0; i < polyline.NumberOfVertices; i++)
                            {
                                pnts.Add(polyline.GetPoint2dAt(i));
                            }

                            surfaceBoundaries.AddBoundaries(pnts, 0.5, SurfaceBoundaryType.Outer, true);
                        }
                        else
                        {
                            COMS.MessengerManager.AddLog("Not Excessive Result For CorrectIsolatedParent = " + roughcal);
                        }
                        tr.Commit();

                        theSurface.Rebuild();
                    }
                }
                COMS.MessengerManager.AddLog("Ending CorrectIsolatedParent");

            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

        }



        /// <summary>
        /// Corrects the isolated parent by scaling.
        /// </summary>
        public static void CorrectIsolatedParentByScaling()
        {
            var flError = false;
            var currentSurface = string.Empty;
            try
            {
                var theSurfaces = GetSurfacesList();

                foreach (var theSurface in theSurfaces)
                {
                    COMS.MessengerManager.AddLog("Starting CorrectIsolatedParentByScaling-Name " + theSurface.Name);
                    COMS.MessengerManager.AddLog("Starting CorrectIsolatedParentByScaling-Triangles " + theSurface.Triangles.Count);
                    COMS.MessengerManager.AddLog("Starting CorrectIsolatedParentByScaling-BoundaryDefinitions " + theSurface.BoundariesDefinition.Count);



                    using (var tr = Active.StartTransaction())
                    {
                        thePolylines = GetPolylineList();
                        currentSurface = theSurface.Name;

                        var sbd = theSurface.BoundariesDefinition;
                        var allSurface = GetTerrainSurfaceProperties("All");
                        var allArea = allSurface.SurfaceArea2D;
                        var curArea = 0.0;
                        if (GetTerrainSurfaceProperties(theSurface.Name) != null)
                        {
                            curArea = GetTerrainSurfaceProperties(theSurface.Name).SurfaceArea2D;
                        }
                        

                        Point2dCollection pnts = new Point2dCollection();


                        if (theSurface.Name == "All")
                            continue;


                        var roughcal = Math.Abs((allArea - curArea) / allArea) * 100;
                        var result = roughcal < 10 ? true : false;


                        if (result)
                        {
                            flError = true;

                            iterations++;

                            COMS.MessengerManager.AddLog("Entering CorrectIsolatedParentByScaling = " + roughcal);

                            var layer = theSurface.Layer;
                            var outerboundary = GetOuterBoundaryCollection(theSurface);

                            if (outerboundary == null)
                                return;

                            var bndyarea = GetOuterBoundaryArea(theSurface);

                            Polyline poly = thePolylines.Find(p => p.Layer == theSurface.Layer && Math.Abs(p.Area - bndyarea) < 0.1);

                            PGA.OffsetPolylines.Program.ScaleObject(poly.ObjectId);                     
                        }
                        else
                        {
                            COMS.MessengerManager.AddLog("No changes for CorrectIsolatedParentByScaling = " + roughcal);
                        }
                        tr.Commit();
                    }
                    theSurface.Rebuild();

                }
                COMS.MessengerManager.AddLog("Ending CorrectIsolatedParentByScaling");

                if (flError && iterations < 10)
                    CorrectIsolatedParentByScaling();
                else
                {
                    iterations = 0;
                }

            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(currentSurface,ex);
            }

        }


        /// <summary>
        /// Corrects the isolated parent when no error thrown.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void CorrectIsolatedParent(TinSurface theSurface)
        {

            try
            {
                COMS.MessengerManager.AddLog("Starting CorrectIsolatedParent" + theSurface.Name);
                

                using (var tr = Active.StartTransaction())
                {
                    thePolylines = GetPolylineList();


                    var sbd = theSurface.BoundariesDefinition;
                    var allSurface = GetTerrainSurfaceProperties("All");
                    var allArea    = allSurface.SurfaceArea2D;
                    var curArea = GetTerrainSurfaceProperties(theSurface.Name).SurfaceArea2D;

                    Point2dCollection pnts = new Point2dCollection();


                    if (theSurface.Name == "All")
                        return;


                    var result = Math.Abs((allArea - curArea)/allArea)*100 < 0.10;


                    if (result)
                    {
                        COMS.MessengerManager.AddLog("Entering CorrectIsolatedParent = " + result);

                        var layer = theSurface.Layer;
                        var outerboundary = GetOuterBoundaryCollection(theSurface);

                        if (outerboundary == null)
                            return;

                        RemoveOuterBoundary(theSurface);

                        SurfaceDefinitionBoundaries surfaceBoundaries =
                            theSurface.BoundariesDefinition;

                        var newoid = PGA.OffsetPolylines.Program.ScaleNewPolylineObject(outerboundary);

                        var polyline = tr.GetObject(newoid, OpenMode.ForWrite) as Polyline;
                        polyline.Layer  = layer;
                        polyline.Closed = true;

                        for (int i = 0; i < polyline.NumberOfVertices; i++)
                        {
                            pnts.Add(polyline.GetPoint2dAt(i));
                        }

                        surfaceBoundaries.AddBoundaries(pnts, 1.0, SurfaceBoundaryType.Outer, true);
                    }
                    else
                    {
                        COMS.MessengerManager.AddLog("Not Excessive Result For CorrectIsolatedParent = "  + result);
                    }
                    tr.Commit();

                    theSurface.Rebuild();

                }
                COMS.MessengerManager.AddLog("Ending CorrectIsolatedParent" + theSurface.Name);

            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

        }

        /// <summary>
        /// Gets the outer boundary.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns>Polyline3d.</returns>
        public static Polyline3d GetOuterBoundary(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    COMS.MessengerManager.AddLog("Starting Checking For Outer Boundary" + theSurface.Name);

                    var sbd = theSurface.BoundariesDefinition;

                    if (sbd == null)
                        return null;

                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Outer)
                        {
                            COMS.MessengerManager.AddLog("Found Outer Boundary");
                            return CreateOuterBoundary(sbd[i]
                                   .Cast<SurfaceBoundary>()
                                   .FirstOrDefault());
                        }
                    }

                    COMS.MessengerManager.AddLog("End Checking For Outer Boundary" + theSurface.Name);

                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }

            return null;
        }

        /// <summary>
        /// Gets the outer boundary area.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.NullReferenceException">GetOuterBoundaryArea
        /// or
        /// GetOuterBoundaryArea</exception>
        public static double GetOuterBoundaryArea(TinSurface theSurface)
        {
           // var area = 0.0;
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    COMS.MessengerManager.AddLog("Starting Checking For Outer Boundary" + theSurface.Name);

                    var sbd = theSurface.BoundariesDefinition;

                    if (sbd == null)
                        throw new NullReferenceException("GetOuterBoundaryArea");

                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Outer)
                        {
                            COMS.MessengerManager.AddLog("Found Outer Boundary");
                            return CreateOuterBoundary(sbd[i]
                                   .Cast<SurfaceBoundary>()
                                   .FirstOrDefault()).Area;
                        }
                    }

                    COMS.MessengerManager.AddLog("End Checking For Outer Boundary" + theSurface.Name);

                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
            throw new NullReferenceException("GetOuterBoundaryArea");
        }


        /// <summary>
        /// Removes the outer boundary.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        public static void RemoveOuterBoundary(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    COMS.MessengerManager.AddLog("Starting Checking For Outer Boundary" + theSurface.Name);

                    var sbd = theSurface.BoundariesDefinition;

                    if (sbd == null)
                        return;

                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Outer)
                        {
                            COMS.MessengerManager.AddLog("Removed Outer Boundary");
                            sbd.RemoveAt(i); 
                        }
                    }

                    COMS.MessengerManager.AddLog("End Checking For Outer Boundary" + theSurface.Name);

                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();

               // theSurface.Rebuild();

            }

        }


        /// <summary>
        /// Gets the outer boundary collection.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns>Point2dCollection.</returns>
        public static Point2dCollection GetOuterBoundaryCollection(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    COMS.MessengerManager.AddLog("Starting Checking For Outer Boundary" + theSurface.Name);

                    var sbd = theSurface.BoundariesDefinition;

                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Outer)
                        {
                            COMS.MessengerManager.AddLog("Found Outer Boundary");
                            var p3d= sbd[i]
                                .Cast<SurfaceBoundary>()
                                .FirstOrDefault().Vertices;

                            return Cvt3DPCol(p3d);
                        }
                    }

                    COMS.MessengerManager.AddLog("End Checking For Outer Boundary" + theSurface.Name);

                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }

            return null;
        }

        /// <summary>
        /// Creates the outer boundary.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>Polyline3d.</returns>
        private static Polyline3d CreateOuterBoundary(SurfaceBoundary s)
        {
            try
            {
                return new Polyline3d(Poly3dType.SimplePoly, s.Vertices, true);
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }

        /// <summary>
        /// Determines whether [has inner boundary] [the specified the surface].
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns><c>true</c> if [has inner boundary] [the specified the surface]; otherwise, <c>false</c>.</returns>
        public static bool HasInnerBoundary(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    COMS.MessengerManager.AddLog("Starting Checking For Hide Boundary" + theSurface.Name);

                    var sbd = theSurface.BoundariesDefinition;
 
                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Hide)
                        {
                            COMS.MessengerManager.AddLog("Found Hide Boundary");
                            return true;
                        }
                    }

                    COMS.MessengerManager.AddLog("End Checking For Hide Boundary" + theSurface.Name);

                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
            return false;
        }

        /// <summary>
        /// Disables the outer boundary.
        /// </summary>
        /// <param name="theSurface">The surface.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DisableOuterBoundary(TinSurface theSurface)
        {
            using (var tr = Active.StartTransaction())
            {
                try
                {
                    COMS.MessengerManager.AddLog("Starting Checking For Outer Boundary" + theSurface.Name);

                    var sbd = theSurface.BoundariesDefinition;

                    for (var i = 0; i < sbd.Count; i++)
                    {
                        if (sbd[i].BoundaryType == SurfaceBoundaryType.Outer)
                        {
                            COMS.MessengerManager.AddLog("Found Outer Boundary");

                            sbd[i].Enabled = false;
                            
                            tr.Commit();

                            return true;
                        }
                    }

                    COMS.MessengerManager.AddLog("End Checking For Outer Boundary" + theSurface.Name);

                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
                tr.Commit();
            }
            return false;
        }


        /// <summary>
        /// Gets the centroid of outer boundary.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>Point2d.</returns>
        private static Point2d GetCentroidOfOuterBoundary(SurfaceBoundary s)
        {
            try
            {
               return BBC.Common.AutoCAD.AcadUtilities.GetCentriod(Cvt3DPCol(s.Vertices));
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return new Point2d();
        }

        /// <summary>
        /// Gets the centroid of outer boundary.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>Point2d.</returns>
        private static Point2d GetCentroidOfOuterBoundary(Point3dCollection s)
        {
            try
            {
                return BBC.Common.AutoCAD.AcadUtilities.GetCentriod(Cvt3DPCol(s));
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return new Point2d();
        }

        /// <summary>
        /// Convert 3d to 2d Collection.
        /// </summary>
        /// <param name="p3DCollection">The p3d collection.</param>
        /// <returns>Point2dCollection.</returns>
        private static Point2dCollection Cvt3DPCol(Point3dCollection p3DCollection)
        {
            var p2dc = new Point2dCollection();

            foreach (Point3d item in p3DCollection)
            {
               p2dc.Add(BBC.Common.AutoCAD.AcadUtilities.ConvertTo2d(item));

            }
            return p2dc;
        }


        /// <summary>
        /// Points the in poly.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="test">The test.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool PointInPoly(List<Point2d> points, Point2d test)
        {
            int nvert = points.Count;

            int i, j;
            bool c = false;

            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {

                if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                    (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                    c = !c;
            }

            return c;
        }

        /// <summary>
        /// Points the in polyline.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="tests">The tests.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool PointInPolyline(Point2dCollection points, Point2dCollection tests)
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

        /// <summary>
        /// Gets the polyline layer.
        /// </summary>
        /// <param name="selectedObjectId">The selected object identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetPolylineLayer(ObjectId selectedObjectId)
        {

            var db = Active.Database;
            var doc = Active.Document;
            var ed = Active.Editor;



            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Autodesk.AutoCAD.DatabaseServices.DBObject obj =
                        tr.GetObject(selectedObjectId, OpenMode.ForRead);

                    Polyline lwp = obj as Polyline;

                    if (lwp != null)
                    {
                        if (lwp.Closed)
                        {
                            return lwp.Layer;
                        }

                    }

                    else
                    {

                        Polyline2d p2d = obj as Polyline2d;

                        if (p2d != null)
                        {
                            return p2d.Layer;
                        }

                        else
                        {

                            Polyline3d p3d = obj as Polyline3d;

                            if (p3d != null)
                            {
                                return p3d.Layer;
                            }
                        }
                    }

                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Polyline Layer Name", ex);
            }

            return String.Empty;
        }

    }
}



