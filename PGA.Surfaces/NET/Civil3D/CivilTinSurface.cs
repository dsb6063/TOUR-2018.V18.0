using System;
using System.Collections.Generic;
using System.Globalization;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using PGA.MessengerManager;
using PGA.SurfaceManager;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using C3DLandDb = Autodesk.Civil.DatabaseServices;
//using PGA.SurfaceManager;

namespace C3DSurfacesDemo
{
    public class CivilTinSurface
    { 
        public CivilTinSurface(string surfaceName, string surfaceStyleName)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                m_TheSurfaceId = C3DLandDb.TinSurface.Create(CivilApplicationManager.WorkingDatabase, surfaceName);
                C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForWrite) as C3DLandDb.TinSurface;
                surface.Layer = surfaceStyleName;
                ObjectId styleId = SurfaceStyleManager.GetStyleId(surfaceStyleName);
                if (styleId == ObjectId.Null)
                {
                    throw new CreateSurfaceException("Unable to create Surface object.");
                }
                surface.StyleId = styleId;
                tr.Commit();
            }
        }
        public static void SetBaseLayerCivilTinSurface(string surfaceStyleName)
        { 
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);

            DocumentLock loc = doc.LockDocument();

            using (loc)
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    CivilApplicationManager.ActiveCivilDocument.Settings.DrawingSettings.ObjectLayerSettings.GetObjectLayerSetting(Autodesk.Civil.Settings.SettingsObjectLayerType.TinSurface).LayerName = surfaceStyleName;
                    tr.Commit();
                }
            }
        }
        public static CivilTinSurface CreateFromExisting(string surfaceName)
        {
            ObjectId requested = findSurface(surfaceName);
            if (requested == ObjectId.Null)
            {
                string msg = String.Format(CultureInfo.CurrentCulture, "The specified surface '{0}' does not exists.", surfaceName);
                throw new CreateSurfaceException(msg);
            }

            return new CivilTinSurface(requested);
        }
        public static bool FindCivilTinSurface(string surfaceName)
        {
            ObjectId requested = findSurface(surfaceName);
            if (requested == ObjectId.Null)
            {
                return false;
            }

            return true;
        }
        public static ObjectId FindCivilTinSurfaceByName(string surfaceName)
        {
            ObjectId requested = findSurface(surfaceName);
            if (requested == ObjectId.Null)
            {
                return requested;
            }

            return requested;
        }
        public static bool FindCivilTinSurfaceByName(string surfaceName,string value)
        {
            ObjectId requested = findSurface(surfaceName);
            if (requested == ObjectId.Null)
            {
                return false;
            }

            return true;
        }
        public string Name 
        {
            get 
            { 
                using(Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    return surface.Name; 
                }
                
            }
            set 
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForWrite) as C3DLandDb.TinSurface;
                    surface.Name = value;
                    tr.Commit();
                }
            }
        }

        public IEnumerable<Point3d> Points 
        {
            get { return new Point3dEnumerable(m_TheSurfaceId); }
        }

        public IEnumerable<SurfaceTriangle> Triangles
        {
            get { return new TriangleEnumerable(m_TheSurfaceId); }
        }

        public void AddPoints(Point3dCollection points)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForWrite) as C3DLandDb.TinSurface;
                surface.AddVertices(points);
                tr.Commit();
            }
        }

        public void AddStandardBreakline(Point3dCollection points, string name)
        {
            ObjectId polyId = createPolylineFromPoints(points);
            AddStandardBreakline(polyId, name);
        }

        public void AddStandardBreakline(ObjectId polyId, string name)
        {
            using(Transaction tr = CivilApplicationManager.StartTransaction())
            {
                C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                ObjectIdCollection entities = new ObjectIdCollection();
                entities.Add(polyId);
                surface.BreaklinesDefinition.AddStandardBreaklines(entities, 1.0, 1.0, 1.0, 0.0);
                tr.Commit();
            }
        }

        public ObjectIdCollection GetObjectIdCollectionFromEntity(ObjectId polyId)
        {
            ObjectIdCollection entities = new ObjectIdCollection();
            entities.Add(polyId);
            return entities;
        }
        public C3DLandDb.TinSurface GetCivilSurfaceBySurfaceId(ObjectId surIdId)
        {
            C3DLandDb.TinSurface surface;
            try
            {
               
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {               
                    surface = surIdId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;                  
                    tr.Commit();
                }
                return surface;
               
            }
            catch (System.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }
            return null;
        }
        public static C3DLandDb.TinSurface GetCivilSurfaceBySurfaceName(string surf)
        {
            C3DLandDb.TinSurface surface;
            try
            {
                ObjectId surIdId = FindCivilTinSurfaceByName(surf);
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    surface = surIdId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    tr.Commit();
                }
                return surface;

            }
            catch (System.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }
            return null;
        }
        public static bool RebuildSurfaceBySurfaceName(string surf)
        {
            C3DLandDb.TinSurface surface;
            try
            {
                ObjectId surIdId = FindCivilTinSurfaceByName(surf);
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    surface = surIdId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    if (surface != null) surface.Rebuild();
                    tr.Commit();
                }
                return true;

            }
            catch (System.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }
            return false;
        }

        public void AddStandardBoundary(ObjectId polyId, string surfacename)
        {
            try
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    //1. Create new surface if no existing surface passed in 
                    C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    if (surface == null)
                        surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    //2. Store boundary
                    ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                    boundaryEntities = GetObjectIdCollectionFromEntity(polyId);

                    //3. Access the BoundariesDefinition object from the surface object
                    C3DLandDb.SurfaceDefinitionBoundaries surfaceBoundaries = surface.BoundariesDefinition;

                    //4. now add the boundary to the surface
                    surfaceBoundaries.AddBoundaries(boundaryEntities, 1.0, SurfaceBoundaryType.Outer, true);
                
                   
                    tr.Commit();

               

                    //surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as C3DLandDb.TinSurface;

                    //// Add the selected polyline's ObjectId to a collection
                    //ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                    //boundaryEntities.Add(plineId);

                }
            }
            catch (System.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }
        }
        public void AddStandardInnerBoundary(ObjectId polyId, string layername)
        {
            try
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    //1. Create new surface if no existing surface passed in 
                    C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    if (surface == null)
                        surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    //2. Store boundary
                    ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                    boundaryEntities = GetObjectIdCollectionFromEntity(polyId);

                    //3. Access the BoundariesDefinition object from the surface object
                    C3DLandDb.SurfaceDefinitionBoundaries surfaceBoundaries = surface.BoundariesDefinition;
                   
                    //4. now add the boundary to the surface
                    var sbo = surfaceBoundaries.AddBoundaries(boundaryEntities, 1.0, SurfaceBoundaryType.Hide, true);
                    sbo.Name = layername + "-" + DateTime.Now.Millisecond;
                    
                    tr.Commit();

                }
            }
            catch (System.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }
        }
        private CivilTinSurface(ObjectId sourceId)
        {
            m_TheSurfaceId = sourceId;
        }

        private static ObjectId findSurface(string surfaceName)
        {
            CivilDocument doc = CivilApplicationManager.ActiveCivilDocument;
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                foreach (ObjectId surfaceId in doc.GetSurfaceIds())
                {
                    C3DLandDb.Surface surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.Surface;
                    if (surface.Name == surfaceName)
                    {
                        return surfaceId;
                    }
                }
            }
            
            return ObjectId.Null;
        }

        private ObjectId createPolylineFromPoints(Point3dCollection points)
        {
            ObjectId polyId = ObjectId.Null;    
            Polyline3d polyline = new Polyline3d(Poly3dType.SimplePoly, points, false);
            polyId = addEntityToCurrentDatabase(polyline);
            return polyId;           
        }

        private ObjectId addEntityToCurrentDatabase(Entity entity)
        {
            ObjectId entityId = ObjectId.Null;
            using(Transaction tr = CivilApplicationManager.StartTransaction())
            {
                Database curDb = CivilApplicationManager.WorkingDatabase;
                BlockTable bt = tr.GetObject(curDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                entityId = btr.AppendEntity(entity);
                tr.AddNewlyCreatedDBObject(entity, true);
                tr.Commit();
            }
            return entityId;
        }

        private ObjectId m_TheSurfaceId;

        public static C3DLandDb.TinSurface FindSurfaceBySurfaceId(ObjectId surfaceId)
        {
            CivilDocument doc = CivilApplicationManager.ActiveCivilDocument;
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {

                C3DLandDb.TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;

                return surface;

            }
        }

        public static string FindSurfaceNameById(ObjectId surfaceId)
        {
            CivilDocument doc = CivilApplicationManager.ActiveCivilDocument;
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {

                C3DLandDb.Surface surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.Surface;

                return surface.Name;

            }
        }


        /// <summary>
        ///     Sets the default build options.
        ///     Date: 2/19/2017
        /// </summary>
        /// <param name="surface">The surface.</param>
        public void SetDefaultBuildOptions()
        {
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (var tr = CivilApplicationManager.StartTransaction())
                    {
                        MessengerManager.AddLog("Start Setting Build Options");

                        var surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;

                        if (surface == null)
                            surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;

                        if (surface == null)
                            return;

                        surface.UpgradeOpen();
                        ///Todo:Handle Breaklines for Water and Bulkhead
                        /// 
                        if (surface.Name.Contains("WATER"))
                            surface.BuildOptions.MaximumTriangleLength = 100;
                        else if (surface.Name.Contains("All"))
                            surface.BuildOptions.MaximumTriangleLength = 60;
                        else
                            surface.BuildOptions.MaximumTriangleLength = 20;

                        surface.BuildOptions.MaximumTriangleLength = 20;
                        surface.BuildOptions.UseMaximumTriangleLength = true;
                        surface.BuildOptions.NeedConvertBreaklines = true;
                        surface.BuildOptions.ExecludeMinimumElevation = true;
                        surface.BuildOptions.MinimumElevation = 0.1;
                        surface.BuildOptions.CrossingBreaklinesElevationOption =
                            CrossingBreaklinesElevationType.UseLast; ///Todo: Changed 2/19/17 from None.
                       
                        surface.DowngradeOpen();

                        tr.Commit();
                        MessengerManager.AddLog("End Setting Build Options " + surface.Name);
                    }
                }
            }
            catch (Exception e)
            {
                MessengerManager.LogException(e);
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
                               surface.Name.Contains("BRIDGE") ||
                               surface.Name.Contains("INTERMEDIATE-ROUGH"))
                    {
                        SetSmoothing(surface);
                    }
                }
                catch (Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.AddLog(String.Format("Smoothing All Region Failed {0}", ex));
                }
            }
        }

        public  C3DLandDb.TinSurface SetSmoothing(C3DLandDb.TinSurface surface)
        {
            try
            {
                var points = new Point3dCollection();
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
            catch (Exception ex)
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
        public C3DLandDb.TinSurface SetSmoothing(C3DLandDb.TinSurface surface, Point3dCollection points)
        {
            try
            {
                MessengerManager.AddLog("Start Surface Smoothing: " + surface.Name);

                //Smooth Critical Surfaces//

                if (surface.Name.Contains("GREEN") ||
                     surface.Name.Contains("COLLAR") ||
                     surface.Name.Contains("BUNKER") ||
                     surface.Name.Contains("BRIDGE") )
                {

                    using (Transaction ts = CivilApplicationManager.StartTransaction())
                    {
                  
                        C3DLandDb.SurfacePointOutputOptions pointOutputOptions =
                            new C3DLandDb.SurfacePointOutputOptions();
                        pointOutputOptions.GridSpacingX = 0.5;
                        pointOutputOptions.GridSpacingY = 0.5;
                        pointOutputOptions.OutputLocations =
                            SurfacePointOutputLocationsType.GridBased;
                        pointOutputOptions.OutputRegions = new Point3dCollection[]
                        {points};

                        C3DLandDb.SurfaceOperationSmooth op =
                            surface.SmoothSurfaceByNNI(pointOutputOptions);

                        ts.Commit();
                    }
                    MessengerManager.AddLog("End Surface Smoothing: " + surface.Name);

                }
                return surface;


            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return null;
        }

    }

    /// <summary>
    /// Illustrates Smoothing a TIN Surface
    /// </summary>
    //public void SmoothTinSurface()
    //{
    //    using (Transaction ts = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
    //    {
    //        try
    //        {
    //            // Select a TIN Surface:     
    //            ObjectId surfaceId = promptForEntity("Select a TIN surface to smooth\n", typeof(TinSurface));
    //            TinSurface oSurface = surfaceId.GetObject(OpenMode.ForWrite) as TinSurface;

    //            // Select a polyline to define the output region:
    //            ObjectId polylineId = promptForEntity("Select a polyline to define the output region\n", typeof(Polyline));
    //            Point3dCollection points = new Point3dCollection();
    //            Polyline polyline = polylineId.GetObject(OpenMode.ForRead) as Polyline;

    //            // Convert the polyline into a collection of points:
    //            int count = polyline.NumberOfVertices;
    //            for (int i = 0; i < count; i++)
    //            {
    //                points.Add(polyline.GetPoint3dAt(i));
    //            }

    //            // Set the options:
    //            SurfacePointOutputOptions output = new SurfacePointOutputOptions();
    //            output.OutputLocations = SurfacePointOutputLocationsType.Centroids;
    //            output.OutputRegions = new Point3dCollection[] { points };

    //            SurfaceOperationSmooth op = oSurface.SmoothSurfaceByNNI(output);

    //            editor.WriteMessage("Output Points: {0}\n", op.OutPutPoints.Count);

    //            // Commit the transaction
    //            ts.Commit();
    //        }
    //        catch (System.Exception e) { editor.WriteMessage(e.Message); }
    //    }
    //}


    public class Point3dEnumerable  : IEnumerable<Point3d>
    {
        public Point3dEnumerable(ObjectId surfaceId)
        {
            m_Surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
        }

        public IEnumerator<Point3d> GetEnumerator()
        {
            return new Point3dEnumerator(m_Surface);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Point3dEnumerator(m_Surface);
        }

        private C3DLandDb.TinSurface m_Surface;
    }

    public class Point3dEnumerator  : IEnumerator<Point3d>
    {
        public Point3dEnumerator(C3DLandDb.TinSurface surface)
        {
            m_VertexEnumerator = surface.Vertices.GetEnumerator();
        }

        public Point3d Current
        {
            get { return currentPoint(); }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return currentPoint(); }
        }

        public void Dispose()
        {
            m_VertexEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return m_VertexEnumerator.MoveNext();
        }

        public void Reset()
        {
            m_VertexEnumerator.Reset();
        }

        private Point3d currentPoint()
        {
            return m_VertexEnumerator.Current.Location;
        }

        private IEnumerator<C3DLandDb.TinSurfaceVertex> m_VertexEnumerator;
    }

    public class TriangleEnumerable : IEnumerable<SurfaceTriangle>
    {
        public TriangleEnumerable(ObjectId surfaceId)
        {
            m_Surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
        }

        public IEnumerator<SurfaceTriangle> GetEnumerator()
        {
            return new TriangleEnumerator(m_Surface);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new TriangleEnumerator(m_Surface);
        }

        private C3DLandDb.TinSurface m_Surface;
    }

    public class TriangleEnumerator : IEnumerator<SurfaceTriangle>
    {
        public TriangleEnumerator(C3DLandDb.TinSurface surface)
        {
            m_TriangleEnumerator = surface.Triangles.GetEnumerator();
            
        }

        public SurfaceTriangle Current
        {
            get { return currentTriangle(); }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return currentTriangle(); }
        }

        public void Dispose()
        {
            m_TriangleEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return m_TriangleEnumerator.MoveNext();
        }

        public void Reset()
        {
            m_TriangleEnumerator.Reset();
        }

        private SurfaceTriangle currentTriangle()
        {
            C3DLandDb.TinSurfaceTriangle triangle = m_TriangleEnumerator.Current;
            Point3d vx1 = triangle.Vertex1.Location;
            Point3d vx2 = triangle.Vertex2.Location;
            Point3d vx3 = triangle.Vertex3.Location;
            return new SurfaceTriangle(vx1, vx2, vx3);
        }

        private IEnumerator<C3DLandDb.TinSurfaceTriangle> m_TriangleEnumerator;

        public void AddPointCloudObjects()
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                string surfaceName ="";
                var m_TheSurfaceId = C3DLandDb.TinSurface.Create(CivilApplicationManager.WorkingDatabase, surfaceName);
                C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                string surfaceStyleName="";
                ObjectId styleId = SurfaceStyleManager.GetStyleId(surfaceStyleName);
                if (styleId == ObjectId.Null)
                {
                    throw new CreateSurfaceException("Unable to create Surface object.");
                }
                surface.StyleId = styleId;
                tr.Commit();
            }
        }

   
    }
}