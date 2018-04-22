using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using BBC.Common.AutoCAD;
using PGA.Civil.Logging;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using ACAD = Autodesk.AutoCAD.DatabaseServices;
using System.Diagnostics;

namespace C3DSurfacesDemo
{
    public class PasteSurfaces
    {
        public PasteSurfaces()
        {
            m_polylinesurfaces = new Dictionary<ObjectId, string>();
            m_polylinesonly    = new Dictionary<ObjectId, string>();
        }
        public void AddPointCloudObjects()
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                string surfaceName = "";
                var m_TheSurfaceId =
                    Autodesk.Civil.DatabaseServices.TinSurface.Create(CivilApplicationManager.WorkingDatabase,
                        surfaceName);
                Autodesk.Civil.DatabaseServices.TinSurface surface =
                    m_TheSurfaceId.GetObject(OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.TinSurface;
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

        public void TestPasteSurface(Autodesk.Civil.DatabaseServices.Surface srcSurface, Autodesk.Civil.DatabaseServices.TinSurface destSurface)
        {
            if (srcSurface == null)  throw new ArgumentNullException("srcSurface");
            if (destSurface == null) throw new ArgumentNullException("destSurface");

            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                destSurface.PasteSurface(srcSurface.Id);
                tr.Commit();
            }
        }

        public void ExtractBorder(Transaction trans, ObjectId surfaceId)
        {
            TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

            // Extract Border from the TinSurface
            // The extracted entities can be Polyline, Polyline3d, or Face.

            ObjectIdCollection entityIds;
            entityIds = surface.ExtractBorder(Autodesk.Civil.SurfaceExtractionSettingsType.Plan);
            for (int i = 0; i < entityIds.Count; i++)
            {
                ObjectId entityId = entityIds[i];
                if (entityId.ObjectClass == RXClass.GetClass(typeof (Polyline3d)))
                {
                    Polyline3d border = entityId.GetObject(OpenMode.ForWrite) as Polyline3d;
                    // Do what you want with the extrated 3d-polyline
                }
            }
        }

        //        Public Sub DemoPasteSurfaceAPI2009()


        //    On Error GoTo ErrLabel
        //    If getCivilObjects = True Then

        //        ' Write your code here

        //        Dim oTinSelectSurf As AeccTinSurface
        //        Dim oTinSourceSurface As AeccTinSurface
        //        Dim objEnt As AcadObject
        //        Dim varPick As Variant
        //        ThisDrawing.Utility.GetEntity objEnt, varPick, vbCrLf & "Select the Surfce Which needs to be Pasted : "
        //        If TypeOf objEnt Is AeccTinSurface Then
        //            Set oTinSelectSurf = objEnt
        //        Else
        //        MsgBox "Selected Entity is NOT a TIN Surface"
        //        Exit Sub
        //        End If
        //        Set oTinSourceSurface = g_oAeccDoc.Surfaces.Item("EG")

        //        oTinSourceSurface.PasteSurface oTinSelectSurf
        //        MsgBox "Surface Pasting Completed "

        //    End If
        //    Exit Sub


        //ErrLabel:
        //    MsgBox Err.Description & " " & Err.Number


        //End Sub

        public class SurfaceMatrix
        {
            private string surfaceType = "";
            private string surfaceName = "";
            private string surfaceStyle = "";
            private string destTINStyle = "";
            private string polyLayer = "";


            public string SurfaceType
            {
                get { return surfaceType; }
                set { surfaceType = value; }
            }

            public string SurfaceName
            {
                get { return surfaceName; }
                set { surfaceName = value; }
            }

            public string SurfaceStyle
            {
                get { return surfaceStyle; }
                set { surfaceStyle = value; }
            }

            public string DestTinStyle
            {
                get { return destTINStyle; }
                set { destTINStyle = value; }
            }

            public string PolyLayer
            {
                get { return polyLayer; }
                set { polyLayer = value; }
            }
        }

        public IList<SurfaceMatrix> CreateSurfaceMatrices()
        {
            IList<SurfaceMatrix> surfaceMatrix = new List<SurfaceMatrix>();
            SurfaceMatrix item = new SurfaceMatrix();
            item.DestTinStyle = "S-BRIDGES";
            item.SurfaceName = "S-BRIDGES";
            item.SurfaceStyle = "BRIDGE";
            item.SurfaceType = "0";
            item.PolyLayer = "OBR";

            surfaceMatrix.Add(item);

            return null;
        }

        public partial class LayerStatesBLL
        {
            private string _bridge = "OBR:S-BRIDGES:-S-BRIDGES:BRIDGE:0";

            public virtual string Bridge
            {
                get { return this._bridge; }
                set { this._bridge = value; }
            }

            private string _building = "OBD:S-BUNKER:S-BUNKER:BUNKER:0";

            public virtual string Building
            {
                get { return this._building; }
                set { this._building = value; }
            }

            private string _bunker = "OST:S-BUNKER";

            public virtual string Bunker
            {
                get { return this._bunker; }
                set { this._bunker = value; }
            }

            private string _bushOutline = "OBO:BUSHOL";

            public virtual string BushOutline
            {
                get { return this._bushOutline; }
                set { this._bushOutline = value; }
            }

            private string _cartPath = "OCA:CARTPATH";

            public virtual string CartPath
            {
                get { return this._cartPath; }
                set { this._cartPath = value; }
            }

            private string _collar = "OCO:S-COLLAR";

            public virtual string Collar
            {
                get { return this._collar; }
                set { this._collar = value; }
            }

            private string _dirtOutline = "ODO:DIRTOL";

            public virtual string DirtOutline
            {
                get { return this._dirtOutline; }
                set { this._dirtOutline = value; }
            }

            private string _fairway = "OFW:FAIRWAY";

            public virtual string Fairway
            {
                get { return this._fairway; }
                set { this._fairway = value; }
            }

            private string _green = "OGR:GREEN";

            public virtual string Green
            {
                get { return this._green; }
                set { this._green = value; }
            }

            private string _greenSideBunker = "OGS:GRN";

            public virtual string GreenSideBunker
            {
                get { return this._greenSideBunker; }
                set { this._greenSideBunker = value; }
            }

            private string _intMedRough = "IOR:S-IMRO";

            public virtual string IntMedRough
            {
                get { return this._intMedRough; }
                set { this._intMedRough = value; }
            }

            private string _landScaping = "OLN:TREE-SYMBOLS";

            public virtual string LandScaping
            {
                get { return this._landScaping; }
                set { this._landScaping = value; }
            }

            private string _nativeArea = "ONA:S-NATIVE";

            public virtual string NativeArea
            {
                get { return this._nativeArea; }
                set { this._nativeArea = value; }
            }

            private string _other = "OTH:OTHER";

            public virtual string Other
            {
                get { return this._other; }
                set { this._other = value; }
            }

            private string _path = "OPT:S-PATH";

            public virtual string Path
            {
                get { return this._path; }
                set { this._path = value; }
            }

            private string _rockOutline = "ORK:S-ROCKS";

            public virtual string RockOutline
            {
                get { return this._rockOutline; }
                set { this._rockOutline = value; }
            }

            private string _roughOutline = "ORO:ROUGH_OUTLINES";

            public virtual string RoughOutline
            {
                get { return this._roughOutline; }
                set { this._roughOutline = value; }
            }

            private string _steps = "OSS:S-STEPS";

            public virtual string Steps
            {
                get { return this._steps; }
                set { this._steps = value; }
            }

            private string _teeBox = "OTB:S-TEEBOX";

            public virtual string TeeBox
            {
                get { return this._teeBox; }
                set { this._teeBox = value; }
            }

            private string _treeOutline = "OTO:TREEOL";

            public virtual string TreeOutline
            {
                get { return this._treeOutline; }
                set { this._treeOutline = value; }
            }

            private string _walkStrip = "OWS:WST";

            public virtual string WalkStrip
            {
                get { return this._walkStrip; }
                set { this._walkStrip = value; }
            }

            private string _wall = "OWL:WALL";

            public virtual string Wall
            {
                get { return this._wall; }
                set { this._wall = value; }
            }

            private string _water = "OWA:WATER";

            public virtual string Water
            {
                get { return this._water; }
                set { this._water = value; }
            }

            private string _waterDrop = "OWD:S-WATER";

            public virtual string WaterDrop
            {
                get { return this._waterDrop; }
                set { this._waterDrop = value; }
            }

            private uint _layerStateID;

            public virtual uint LayerStateID
            {
                get { return this._layerStateID; }
                set { this._layerStateID = value; }
            }
        }

        public bool CreateOverallSurface()
        {
            string surfaceName = EditorUtils.PromptForString("\nEnter surface name: ");
            if (String.IsNullOrEmpty(surfaceName)) surfaceName = "OVERALLSURFACE";
            // if (surfaceName == String.Empty) EditorUtils.Write("\nERROR: Invalid name for surface.");


            string surfaceStyleName = EditorUtils.PromptForString("\nEnter surface style name: ");
            // if (surfaceStyleName == String.Empty) EditorUtils.Write("\nERROR: Invalid style name for surface style.");


            if (!SurfaceStyleManager.Exists(surfaceStyleName))
            {
                string msg = String.Format("\nSurface Style '{0}' doesn't exist. Creating it with default values.",
                    surfaceStyleName);
                //  EditorUtils.Write(msg);
                SurfaceStyleManager.CreateDefault(surfaceStyleName);
            }
            CivilTinSurface surface = new CivilTinSurface(surfaceName, surfaceStyleName);
            surface.AddPoints(SurfaceDataProvider.GenerateRandomPoints(100, 100, 100, 10));
            return true;
        }

        public Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
        GetAllInternalPolyLinesToSelected(Autodesk.AutoCAD.DatabaseServices.ObjectId oid,
            Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids, double minpolyseparation)
        {
            var intid = new ObjectIdCollection();
            Polyline basePolyline = null;
            Polyline tempPolyline = null;
            double bArea = 0.0;
            double cArea = 0.0;

            try
            {
                //Always set larger area to Max0 and Min0
                using (Database db = CivilApplicationManager.WorkingDatabase)
                {
                    foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in oids)
                    {
                        try
                        {

                            if (GetPolyFromObjId(objectId) == null)
                                continue;

                            //Test for Complexity

                            if (PolylineUtils.ExcludePolyBasedOnComplexity(objectId))
                                continue;

                            basePolyline = GetPolyFromObjId(oid, db);
                            tempPolyline = GetPolyFromObjId(objectId, db);
                        }
                        catch (NullReferenceException ex)
                        {
                            PGA.Civil.Logging.ACADLogging.LogMyExceptions("GETALLINTERNALPOLYLINESTOSELECTED" +
                                                                           ex.Message);
                            continue; //continue iterating next obj
                        }

                        if (basePolyline != null || tempPolyline != null)
                        {
                            bArea = basePolyline.Area;
                            cArea = tempPolyline.Area;

                            if (Math.Abs(bArea - cArea) < TOLERANCE)
                                continue; //same line
                        }
                        else
                        {
                            PGA.Civil.Logging.ACADLogging.LogMyExceptions("TempPolyline is Null: " + tempPolyline + ", " + basePolyline);
                            continue;
                        }

                        Point2dCollection tpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(tempPolyline);
                        Point2dCollection bpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(basePolyline);

                        if (PnPoly.PointInPolyline(bpoints, tpoints, minpolyseparation))
                        {
                            {
                                if (objectId.IsValid)
                                    intid.Add(objectId);
                            }
                        }
                       
                    }
                      
                    if (intid.Count != 0)
                        return intid;
                }
            }
            catch (NullReferenceException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions
                    ("GETALLINTERNALPOLYLINESTOSELECTED: " + e.Message);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            return null;
        }
        public Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection GetAllSurfaces()
        {
            try
            {
                Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids =
                    new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
                SelectionSet selection = null;
                selection = SelectionManager.GetSelectionSet("Select Surfaces","*" , "AECC_TIN_SURFACE");

                foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId oid in selection.GetObjectIds())
                {
                    oids.Add((oid));
                }
                return oids;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
            }

            return null;
        }

        public Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection GetAllPolyLines()
        {
            try
            {
                Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids =
                    new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
                SelectionSet selection = null;
                selection = SelectionManager.GetSelectionSet("Select Polylines");

                foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId oid in selection.GetObjectIds())
                {
                    oids.Add((oid));
                }
                return oids;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
            }

            return null;
        }

        public void AddStandardBoundary(ObjectId polyId, string surfacename,
            Autodesk.Civil.DatabaseServices.TinSurface surface)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                //1. Create new surface if no existing surface passed in 
                // if (surface == null)
                //   surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.TinSurface;
                //2. Store boundary
                ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                boundaryEntities = GetObjectIdCollectionFromEntity(polyId);

                //3. Access the BoundariesDefinition object from the surface object
                Autodesk.Civil.DatabaseServices.SurfaceDefinitionBoundaries surfaceBoundaries =
                    surface.BoundariesDefinition;

                //4. now add the boundary to the surface
                surfaceBoundaries.AddBoundaries(boundaryEntities, 1.0, SurfaceBoundaryType.Hide, true);


                tr.Commit();


                //surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as C3DLandDb.TinSurface;

                //// Add the selected polyline's ObjectId to a collection
                //ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                //boundaryEntities.Add(plineId);
            }
        }

        //public bool AddAsInnerBoundary(ObjectId pLines, CivilTinSurface surface)
        //{
        //    AddStandardBoundary(pLines, "", surface);
        //    return true;
        //}

        //private void AddStandardBoundary(ObjectId pLines, string surfacename, CivilTinSurface surface)
        //{
        //    using (Transaction tr = CivilApplicationManager.StartTransaction())
        //    {
        //        //1. Create new surface if no existing surface passed in 
        //        // if (surface == null)
        //        //   surface = m_TheSurfaceId.GetObject(OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.TinSurface;
        //        //2. Store boundary
        //        ObjectIdCollection boundaryEntities = new ObjectIdCollection();
        //        boundaryEntities = GetObjectIdCollectionFromEntity(pLines);
        //        //TinSurface sur = surface. as Autodesk.Civil.DatabaseServices.TinSurface;
        //        //3. Access the BoundariesDefinition object from the surface object
        //        Autodesk.Civil.DatabaseServices.SurfaceDefinitionBoundaries surfaceBoundaries = surface.BoundariesDefinition;

        //        //4. now add the boundary to the surface
        //        surfaceBoundaries.AddBoundaries(boundaryEntities, 1.0, SurfaceBoundaryType.Hide, true);


        //        tr.Commit();


        //        //surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as C3DLandDb.TinSurface;

        //        //// Add the selected polyline's ObjectId to a collection
        //        //ObjectIdCollection boundaryEntities = new ObjectIdCollection();
        //        //boundaryEntities.Add(plineId);

        //    }
        //}

        public bool AddSelectedLineAsOuterBoundary(Autodesk.AutoCAD.DatabaseServices.ObjectId id)
        {
            throw new NotImplementedException();
        }

        public CivilTinSurface CreateSelectedSurface(Autodesk.AutoCAD.DatabaseServices.ObjectId oid)
        {
            try
            {
                ACADLogging.LogMyExceptions("Start CreateSelectedSurface Method");
                string surfaceName = GetPolyLayerFromOid((oid));
                string surfaceStyleName = GetPolyLayerFromOid((oid)); //layer and style name are the same name.
                // m_polylinesurfaces = new Dictionary<ObjectId, string>();

                int i = 0;
                //string tempSurface = "s-" + surfaceName.ToLower();
                string tempSurface = surfaceName.ToUpper();
                while (CivilTinSurface.FindCivilTinSurfaceByName(tempSurface, ""))
                {
                    tempSurface = string.Format("{0}-{1}", surfaceName, i++);
                }
                surfaceName = tempSurface;
                if (!SurfaceStyleManager.Exists(surfaceStyleName))
                {
                    string msg = String.Format("\nSurface Style '{0}' doesn't exist. Creating it with default values.",
                        surfaceStyleName);
                    //EditorUtils.Write(msg);
                    SurfaceStyleManager.CreateDefault(surfaceStyleName);
                }
                CivilTinSurface surface = new CivilTinSurface(surfaceName, surfaceStyleName);
                m_polylinesurfaces.Add(CivilTinSurface.FindCivilTinSurfaceByName(surface.Name), surfaceName);
                m_polylinesonly.Add(oid, surfaceName);
                return surface;
            }
            catch (System.Exception ex)
            {
                ACADLogging.LogMyExceptions("CreateSelectedSurface"+ ex.Message);
            }
            return null;
        }

        public Vector3d GetPLineMaxGeoExtents(Polyline polyline)
        {
            return polyline.GeometricExtents.MaxPoint.GetAsVector();
        }

        public Vector3d GetPLineMinGeoExtents(Polyline polyline)
        {
            return polyline.GeometricExtents.MinPoint.GetAsVector();
        }

        public bool IsContainedWithInObject(Vector3d max0, Vector3d min0, Vector3d max1, Vector3d min1)
        {
            if ((max0.X > max1.X) && (max0.Y > max1.Y) && (min0.X < min1.X) && (min0.Y < min1.Y))
                return true;
            return false;
        }

        public bool IsContainedWithInObject(Point3d max0, Point3d min0, Point3d max1, Point3d min1)
        {
            if ((max0.X > max1.X) && (max0.Y > max1.Y) && (min0.X < min1.X) && (min0.Y < min1.Y))
                return true;
            return false;
        }

        internal Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection GetAllInternalPolyLinesToSelected(ObjectId oid)
        {
            try
            {
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
            }
            return null;
        }

        private static Extents3d GetExtents3DPoly(ObjectId selectedObjectId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = CivilApplicationManager.WorkingDatabase;
            Transaction tr = db.TransactionManager.StartTransaction();

            // Get the current UCS

            CoordinateSystem3d ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (tr)
            {
                DBObject obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                Polyline lwp = obj as Polyline;

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (lwp.Closed)
                    {
                        return lwp.GeometricExtents;
                    }
                    // Use a for loop to get each vertex, one by one

                    int vn = lwp.NumberOfVertices;

                    for (int i = 0; i < vn; i++)
                    {
                        // Could also get the 3D point here

                        Point2d pt = lwp.GetPoint2dAt(i);

                        ed.WriteMessage("\n" + pt.ToString());
                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    Polyline2d p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        // Use foreach to get each contained vertex

                        foreach (ObjectId vId in p2d)
                        {
                            Vertex2d v2d =
                                (Vertex2d) tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                    );

                            ed.WriteMessage(
                                "\n" + v2d.Position.ToString()
                                );
                        }
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        Polyline3d p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            // Use foreach to get each contained vertex

                            foreach (ObjectId vId in p3d)
                            {
                                PolylineVertex3d v3d =
                                    (PolylineVertex3d) tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                        );

                                ed.WriteMessage(
                                    "\n" + v3d.Position.ToString()
                                    );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
            return new Extents3d(new Point3d(0, 0, 0), new Point3d(0, 0, 0));
        }

        private static Point3d GetMinVector3D(Extents3d enExtents3D)
        {
            return enExtents3D.MinPoint;
        }

        private static Point3d GetMaxVector3D(Extents3d enExtents3D)
        {
            return enExtents3D.MaxPoint;
        }

        public static string GetPolyLayerFromOid(Autodesk.AutoCAD.DatabaseServices.ObjectId selectedObjectId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = CivilApplicationManager.WorkingDatabase; ///Database;
            Transaction tr = db.TransactionManager.StartTransaction();

            // Get the current UCS

            CoordinateSystem3d ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (tr)
            {
                DBObject obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                Polyline lwp = obj as Polyline;

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (lwp.Closed)
                    {
                        return lwp.Layer;
                    }
                    // Use a for loop to get each vertex, one by one

                    int vn = lwp.NumberOfVertices;

                    for (int i = 0; i < vn; i++)
                    {
                        // Could also get the 3D point here

                        Point2d pt = lwp.GetPoint2dAt(i);

                        ed.WriteMessage("\n" + pt.ToString());
                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    Polyline2d p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        // Use foreach to get each contained vertex

                        foreach (ObjectId vId in p2d)
                        {
                            Vertex2d v2d =
                                (Vertex2d) tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                    );

                            ed.WriteMessage(
                                "\n" + v2d.Position.ToString()
                                );
                        }
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        Polyline3d p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            // Use foreach to get each contained vertex

                            foreach (ObjectId vId in p3d)
                            {
                                PolylineVertex3d v3d =
                                    (PolylineVertex3d) tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                        );

                                ed.WriteMessage(
                                    "\n" + v3d.Position.ToString()
                                    );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
            return String.Empty;
        }

        //public static ObjectId CastObjectReverseId(Database db, Autodesk.AutoCAD.DatabaseServices.ObjectId objectId)
        //{
        //    ObjectId oid = new ObjectId();

        //    db.TryGetObjectId(objectId.Id.Handle, out oid);

        //    if (!oid.IsValid)
        //        throw new System.Exception("CastObjectReverseId: Threw an exception!");

        //    return oid;
        //}

        //public static ObjectId CastObjectReverseId(Autodesk.AutoCAD.DatabaseServices.ObjectId objectId, Database db)
        //{
        //    ObjectId oid;
        //    using (Document doc = Application.DocumentManager.MdiActiveDocument)
        //    {
        //        if (db == null) db = CivilApplicationManager.WorkingDatabase;

        //        db.TryGetObjectId(objectId.Id.Handle, out oid);

        //        if (!oid.IsValid)
        //            throw new System.Exception("CastObjectReverseId: Threw an exception!");
        //    }
        //    return oid;
        //}

        public Polyline GetPolyFromObjId(Autodesk.AutoCAD.DatabaseServices.ObjectId oid, Database db)
        {
            //using (Document doc = Application.DocumentManager.MdiActiveDocument)
            //{
            //Editor ed = doc.Editor;
            //Database db = doc.Database;

            // Get the current UCS

            //CoordinateSystem3d ucs =
            //    ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    DBObject obj;

                    try
                    {
                        obj = tr.GetObject(oid, OpenMode.ForRead);
                    }
                    catch (NullReferenceException e)
                    {
                        PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
                        return null;
                    }

                    Polyline lwp = obj as Polyline;

                    if (lwp != null)
                    {
                        // Is Polyline Closed
                        if (lwp.Closed)
                        {
                            return lwp;
                        }
                    }

                    tr.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                PGA.Civil.Logging.ACADLogging.LogMyExceptions("GETPOLYFROMOBJID:" + e.Message);
            }
            //}
            return null;
        }
        public  Polyline GetPolyFromObjId(Autodesk.AutoCAD.DatabaseServices.ObjectId oid)
        {
            using (Database db = CivilApplicationManager.WorkingDatabase)
            {

                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        DBObject obj;

                        try
                        {
                            obj = tr.GetObject(oid, OpenMode.ForRead);
                        }
                        catch (NullReferenceException e)
                        {
                            PGA.Civil.Logging.ACADLogging.LogMyExceptions(e.Message);
                            return null;
                        }

                        Polyline lwp = obj as Polyline;

                        if (lwp != null)
                        {
                            // Is Polyline Closed
                            if (lwp.Closed)
                            {
                                return lwp;
                            }
                        }

                        tr.Commit();
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception e)
                {
                    PGA.Civil.Logging.ACADLogging.LogMyExceptions("GETPOLYFROMOBJID:" + e.Message);
                }
            }
             
            return null;
        }

        //public static Autodesk.AutoCAD.DatabaseServices.ObjectId CastObjectId(
        //    Autodesk.AutoCAD.DatabaseServices.ObjectId objectId)
        //{
        //    Autodesk.AutoCAD.DatabaseServices.ObjectId oid = new Autodesk.AutoCAD.DatabaseServices.ObjectId(objectId);
        //    return oid;
        //}

        public ObjectIdCollection GetObjectIdCollectionFromEntity(ObjectId polyId)
        {
            ObjectIdCollection entities = new ObjectIdCollection();
            entities.Add(polyId);
            return entities;
        }

        public ObjectIdCollection GetPoint3DCollectionFromEntity(ObjectId polyId)
        {
            Point3dCollection points = new Point3dCollection();

            ObjectIdCollection entities = new ObjectIdCollection();
            entities.Add(polyId);
            return entities;
        }

        public static Point3dCollection GetPolyPointsByObjectId(Autodesk.AutoCAD.DatabaseServices.ObjectId selectedObjectId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = CivilApplicationManager.WorkingDatabase;
            Transaction tr = db.TransactionManager.StartTransaction();
            Point3dCollection points = new Point3dCollection();

            // Get the current UCS

            CoordinateSystem3d ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (tr)
            {
                DBObject obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                Polyline lwp = obj as Polyline;

                if (lwp != null)
                {
                    // Use a for loop to get each vertex, one by one

                    int vn = lwp.NumberOfVertices;

                    for (int i = 0; i < vn; i++)
                    {
                        Point3d point3D = lwp.GetPoint3dAt(i);
                        points.Add(point3D);
                    }
                }

                #region Comments

                //else
                //{

                //    // If an old-style, 2D polyline

                //    Polyline2d p2d = obj as Polyline2d;

                //    if (p2d != null)
                //    {

                //        // Use foreach to get each contained vertex

                //        foreach (ObjectId vId in p2d)
                //        {

                //            Vertex2d v2d =

                //                (Vertex2d)tr.GetObject(

                //                    vId,

                //                    OpenMode.ForRead

                //                    );

                //            ed.WriteMessage(

                //                "\n" + v2d.Position.ToString()

                //                );

                //        }

                //    }

                //    else
                //    {

                //        // If an old-style, 3D polyline

                //        Polyline3d p3d = obj as Polyline3d;

                //        if (p3d != null)
                //        {

                //            // Use foreach to get each contained vertex

                //            foreach (ObjectId vId in p3d)
                //            {

                //                PolylineVertex3d v3d =

                //                    (PolylineVertex3d)tr.GetObject(

                //                        vId,

                //                        OpenMode.ForRead

                //                        );

                //                ed.WriteMessage(

                //                    "\n" + v3d.Position.ToString()

                //                    );

                //            }

                //        }

                // }

                //}

                #endregion

                // Committing is cheaper than aborting

                tr.Commit();
            }
            return points;
        }

        internal static void ListInternalPolylines(Dictionary<ObjectId, ObjectIdCollection> m_dict)
        {

            foreach (var obj in m_dict.Keys)
            {
                ACADLogging.LogMyExceptions(obj + "******Key ********" + obj);
                 

                foreach (ObjectIdCollection collection in m_dict.Values)
                {
                    ACADLogging.LogMyExceptions("******Collection********");
                    foreach (ObjectId polylines in collection)
                    {
                        ACADLogging.LogMyExceptions(polylines.Handle.ToString());
                    }
                    
                }
            }
         
        }

        public bool CreateSurfaceForPolylines(ObjectIdCollection polylines)
        {
            try
            {
                foreach (ObjectId polyline in polylines)
                {
                    try
                    {
                        ACADLogging.LogMyExceptions("Starting CreateSurfaceForPolylines");
                        CreateSelectedSurface(polyline);
                        ACADLogging.LogMyExceptions("End CreateSurfaceForPolylines");
                    }
                    catch (System.Exception e)
                    {
                        ACADLogging.LogMyExceptions(e.Source, e);
                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                ACADLogging.LogMyExceptions(e.Source,e);
            }
            return false;
        }

        private static Dictionary<ObjectId,string> m_polylinesurfaces;
        private static Dictionary<ObjectId, string> m_polylinesonly;

        public bool AddBoundariesForSurfaces(ObjectIdCollection polylines)
        {

            try
            {
                foreach (KeyValuePair<ObjectId, string> pair in m_polylinesonly)
                {
                    try
                    {
                        //Test for Complexity

                        if(PolylineUtils.ExcludePolyBasedOnComplexity(pair.Key))
                            continue;

                        #region Create Outer Boundary
                        PGA.Civil.Logging.ACADLogging.LogMyExceptions("Start AddBoundariesForSurfaces");
                        using (Transaction tr = CivilApplicationManager.StartTransaction())
                        {
                            ObjectId lObjectId = new ObjectId();

                            lObjectId = CivilTinSurface.FindCivilTinSurfaceByName(pair.Value);
                            //1. Create new surface if no existing surface passed in 
                            TinSurface surface =
                                lObjectId.GetObject(OpenMode.ForRead) as TinSurface;
                            //2. Store boundary
                            ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                            boundaryEntities = GetObjectIdCollectionFromEntity(pair.Key);

                            //3. Access the BoundariesDefinition object from the surface object
                            SurfaceDefinitionBoundaries surfaceBoundaries =
                                surface.BoundariesDefinition;

                            //4. now add the boundary to the surface (for non-destructive set true)
                            try
                            {
                                surfaceBoundaries.AddBoundaries(boundaryEntities, 1.0, SurfaceBoundaryType.Outer, true);

                            }
                            catch (System.Exception)
                            {
                                PGA.Civil.Logging.ACADLogging.LogMyExceptions("Error AddBoundariesForSurfaces");

                            }
                            PGA.Civil.Logging.ACADLogging.LogMyExceptions("End AddBoundariesForSurfaces");



                            tr.Commit();

                        }

                        #endregion
                    }
                    catch (System.Exception ex)
                    {
                        ACADLogging.LogMyExceptions("Error in loop AddBoundariesForSurfaces"+ex.Message);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                ACADLogging.LogMyExceptions("Error in AddBoundariesForSurfaces" + ex.Message);
            }
            return false;
        }
        public CivilTinSurface FindSurfaceIdForPolylineV1(ObjectId oid)
        {
            try
            {

                foreach (KeyValuePair<ObjectId, string> id in m_polylinesurfaces)
                {
                    if (id.Key == null) continue;

                    if (oid == id.Key)
                    {
                        Debug.WriteLine("Match iod" + oid + "and" + id.Key);
                        return CivilTinSurface.CreateFromExisting(id.Value);
                    }
                    Debug.WriteLine("NO match iod" + oid + "and" + id.Key);
                }

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
        public static CivilTinSurface FindSurfaceIdForPolylineV2(ObjectId oid)
        {
            try
            {
               
                foreach (KeyValuePair<ObjectId,string> id in m_polylinesonly)
                {
                    if (id.Key == null) continue;

                    if (oid == id.Key)
                    {
                        Debug.WriteLine("Match iod" + oid + "and" + id.Key);
                        return CivilTinSurface.CreateFromExisting(id.Value);
                    }
                    Debug.WriteLine("NO match iod" + oid + "and" + id.Key);
                }
              
            }
            catch (System.Exception e)
            {
                ACADLogging.LogMyExceptions("FindSurfaceIdForPolylineV2" + e.Message);
            }
            return null;
        }

        public double TOLERANCE
        {
            get { return 0.001; }
        }

        public bool PasteAllSurfaces(Autodesk.Civil.DatabaseServices.Surface sHole)
        {
          
            ObjectId objectId = ObjectId.Null;
            try
            {
                foreach (KeyValuePair<ObjectId, string> id in m_polylinesurfaces)
                {
                    objectId = id.Key;
                    try
                    {                    
                        TestPasteSurface(sHole, CivilTinSurface.FindSurfaceBySurfaceId(objectId));
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e);
                    }             
                }
                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

    }
}