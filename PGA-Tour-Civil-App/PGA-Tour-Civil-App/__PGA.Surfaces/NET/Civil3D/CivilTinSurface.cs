using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using C3DLandDb = Autodesk.Civil.DatabaseServices;

namespace C3DSurfacesDemo
{
    public class CivilTinSurface
    {
        public CivilTinSurface(string surfaceName, string surfaceStyleName)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                // SetBaseLayerCivilTinSurface(surfaceStyleName);  //not needed
                m_TheSurfaceId = C3DLandDb.TinSurface.Create(CivilApplicationManager.WorkingDatabase, surfaceName);
                C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForWrite) as C3DLandDb.TinSurface;
                surface.Layer = surfaceStyleName;
                // surface.Material = MaterialManager.GetMaterialForSurfaceName(surfaceName);
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

        public static bool FindCivilTinSurfaceByName(string surfaceName, string value)
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
                using (Transaction tr = CivilApplicationManager.StartTransaction())
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
            get
            {
                return new Point3dEnumerable(m_TheSurfaceId);
            }
        }

        public IEnumerable<SurfaceTriangle> Triangles
        {
            get
            {
                return new TriangleEnumerable(m_TheSurfaceId);
            }
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
            using (Transaction tr = CivilApplicationManager.StartTransaction())
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
                PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
            }
            return null;
        }

        public static C3DLandDb.TinSurface GetCivilSurfaceBySurfaceName(string surf)
        {
            C3DLandDb.TinSurface surface;
            try
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions("Starting GetCivilSurfaceBySurfaceName");
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
                PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
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
                    if (surface != null)
                        surface.Rebuild();
                    tr.Commit();
                }
                return true;
            }
            catch (System.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
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
                    try
                    {
                        surfaceBoundaries.AddBoundaries(boundaryEntities, 1.0, SurfaceBoundaryType.Outer, true);

                    }
                    catch (Exception e)
                    {
                        PGA.Civil.Logging.ACADLogging.LogMyExceptions("Error AddStandardBoundary" + e.Message);
                    }                   
                    tr.Commit();
                   
                }
            }
            catch (System.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions("AddStandardBoundary"+e.Message);
            }
        }

        public void AddStandardInnerBoundary(ObjectId polyId, string surfacename)
        {
            try
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    try
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
                        try
                        {
                            surfaceBoundaries.AddBoundaries(boundaryEntities, 1.0, SurfaceBoundaryType.Hide, true);
                        }
                        catch (Exception ex)
                        {
                            PGA.Civil.Logging.ACADLogging.LogMyExceptions(string.Format
                                ("AddStandardInnerBoundary: {0}:{1}", polyId, ex.Message));

                        }
                    }
                    catch (Exception ex)
                    {
                        PGA.Civil.Logging.ACADLogging.LogMyExceptions(string.Format("AddStandardInnerBoundary: {0}:{1}", surfacename, ex.Message));
                    }
                  
                    tr.Commit();
                }
            }
            catch (System.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions("AddStandardInnerBoundary: " + e.Message);
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
            using (Transaction tr = CivilApplicationManager.StartTransaction())
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

        internal static C3DLandDb.Surface GetCivilTinSurface(string p)
        {
            throw new NotImplementedException();
        }
        //internal static bool RebuildSurfaceBySurfaceName(Dictionary<ObjectId, ObjectIdCollection> m_dict)
        //{
        //    CivilTinSurface.RebuildSurfaceBySurfaceName(lsurface.Name);
        //}
    }

    public class Point3dEnumerable : IEnumerable<Point3d>
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

    public class Point3dEnumerator : IEnumerator<Point3d>
    {
        public Point3dEnumerator(C3DLandDb.TinSurface surface)
        {
            m_VertexEnumerator = surface.Vertices.GetEnumerator();
        }

        public Point3d Current
        {
            get
            {
                return currentPoint();
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return currentPoint();
            }
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
            get
            {
                return currentTriangle();
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return currentTriangle();
            }
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
                string surfaceName = "";
                var m_TheSurfaceId = C3DLandDb.TinSurface.Create(CivilApplicationManager.WorkingDatabase, surfaceName);
                C3DLandDb.TinSurface surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                string surfaceStyleName = "";
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