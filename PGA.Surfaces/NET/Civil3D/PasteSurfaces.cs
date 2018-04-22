using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.Runtime;
using global::Autodesk.Civil;
using global::Autodesk.Civil.ApplicationServices;
using global::Autodesk.Civil.DatabaseServices;
using BBC.Common.AutoCAD;
using Application = global::Autodesk.AutoCAD.ApplicationServices.Core.Application;
using DBObject = global::Autodesk.AutoCAD.DatabaseServices.DBObject;
using ACAD = global::Autodesk.AutoCAD.DatabaseServices;
using System.Diagnostics;
using System.Globalization;
using PGA.Database;
using PGA.DataContext;
using PGA.MessengerManager;
using Exception = System.Exception;

namespace C3DSurfacesDemo
{
    public class PasteSurfaces
    {
        public PasteSurfaces()
        {
            m_polylinesurfaces = new Dictionary<ObjectId, string>();
            m_polylinesonly = new Dictionary<ObjectId, string>();
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
                var styleId = SurfaceStyleManager.GetStyleId(surfaceStyleName);
                if (styleId == ObjectId.Null)
                {
                    throw new CreateSurfaceException("Unable to create Surface object.");
                }
                surface.StyleId = styleId;
                tr.Commit();
            }
        }

        public void TestPasteSurface(Autodesk.Civil.DatabaseServices.Surface srcSurface,
            Autodesk.Civil.DatabaseServices.TinSurface destSurface)
        {
            if (srcSurface == null) throw new ArgumentNullException("srcSurface");
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

            private string _rockOutline = "ORK:S-ROCK-OUTLINE";

            public virtual string RockOutline
            {
                get { return this._rockOutline; }
                set { this._rockOutline = value; }
            }

            private string _roughOutline = "ORO:ROUGH-OUTLINES";

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
                Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids)
        {
           

            try
            {
                //Always set larger area to Max0 and Min0
                using (Database db = CivilApplicationManager.WorkingDatabase)
                {
                    var intid = new ObjectIdCollection();


                    foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in oids)
                    {
                        ACAD.Polyline basePolyline = null;
                        //ACAD.Polyline compPolyline = null;
                        ACAD.Polyline tempPolyline = null;
                        double bArea = 0.0;
                        double cArea = 0.0;

                        try
                        {

                            if (GetPolyFromObjId(objectId) == null)
                                continue;

                            basePolyline = GetPolyFromObjId(oid, db);
                            tempPolyline = GetPolyFromObjId(objectId, db);
                        }
                        catch (NullReferenceException ex)
                        {
                            PGA.MessengerManager.MessengerManager.LogException(ex);

                            PGA.MessengerManager.MessengerManager.AddLog
                            (string.Format("GETALLINTERNALPOLYLINESTOSELECTED{0}", ex.Message));
                            continue;  
                        }

                        if (basePolyline != null || tempPolyline != null)
                        {
                            //must be on same Layer
                            if (basePolyline.Layer.ToUpperInvariant() !=
                                tempPolyline.Layer.ToUpperInvariant())
                                continue;
                            //must not have equal areas
                            bArea = basePolyline.Area;
                            cArea = tempPolyline.Area;

                            if (Math.Abs(bArea - cArea) < TOLERANCE)
                                continue; //not same line
                        }
                        else
                        {

                            PGA.MessengerManager.MessengerManager.AddLog
                           (string.Format("TempPolyline is Null: {0}, {1}", tempPolyline, basePolyline));
                            continue;
                        }

                        Point2dCollection tpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(tempPolyline);
                        Point2dCollection bpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(basePolyline);

                        if (PnPoly.PointInPolyline(bpoints, tpoints))
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
                PGA.MessengerManager.MessengerManager.AddLog
                    (string.Format("GETALLINTERNALPOLYLINESTOSELECTED: {0}", e.Message));
            }
            return null;
        }

        public Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
          GetAllInternalPolyLinesToSelectedOriginal(Autodesk.AutoCAD.DatabaseServices.ObjectId oid,
              Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids)
        {
            var intid = new ObjectIdCollection();
            ACAD.Polyline basePolyline = null;
            ACAD.Polyline tempPolyline = null;
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

                            basePolyline = GetPolyFromObjId(oid, db);
                            tempPolyline = GetPolyFromObjId(objectId, db);
                        }
                        catch (NullReferenceException ex)
                        {
                            PGA.MessengerManager.MessengerManager.LogException(ex);

                            PGA.MessengerManager.MessengerManager.AddLog
                            (string.Format("GETALLINTERNALPOLYLINESTOSELECTED{0}", ex.Message));
                            continue;
                        }

                        if (basePolyline != null || tempPolyline != null)
                        {
                            bArea = basePolyline.Area;
                            cArea = tempPolyline.Area;

                            if (Math.Abs(bArea - cArea) < TOLERANCE)
                                continue; //not same line
                        }
                        else
                        {

                            PGA.MessengerManager.MessengerManager.AddLog
                           (string.Format("TempPolyline is Null: {0}, {1}", tempPolyline, basePolyline));
                            continue;
                        }

                        Point2dCollection tpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(tempPolyline);
                        Point2dCollection bpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(basePolyline);

                        if (PnPoly.PointInPolyline(bpoints, tpoints))
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
                PGA.MessengerManager.MessengerManager.AddLog
                    (string.Format("GETALLINTERNALPOLYLINESTOSELECTED: {0}", e.Message));
            }
            return null;
        }


        private static List<ObjectId> GetSurfaceEntityIDs(Database db)

        {
            List<ObjectId> ids = null;

            try
            {

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (BlockTable)tran.GetObject(db.BlockTableId, OpenMode.ForRead);

                    var br =
                        (BlockTableRecord)tran.GetObject(tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead);

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
                           where id.ObjectClass.DxfName.ToUpper() == "AECC_TIN_SURFACE"
                           select id).ToList();


                    tran.Commit();
                }

            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }

            return ids;
        }


        public static Autodesk.AutoCAD.DatabaseServices.ObjectId GetSurfaceByName(String surfaceName)
        {
            try
            {
                Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids =
                    new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();

                ObjectId surfaceId = ObjectId.Null;

                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    var doc = CivilApplicationManager.ActiveCivilDocument;
                    var surfacecollection = GetSurfaceEntityIDs(CivilApplicationManager.WorkingDatabase);

                    foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId oid in surfacecollection)
                    {
                        TinSurface surface = oid.GetObject(OpenMode.ForRead, false, true) as TinSurface;

                        if (surface != null)
                        {
                            if (surface.Name == surfaceName.ToUpper() ||
                                surface.Name == surfaceName ||
                                surface.Name.Contains("All"))

                                return surfaceId = oid;
                        }
                    }
                    tr.Commit();
                }
                return surfaceId;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }

            return ObjectId.Null;
        }

        public Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection GetAllSurfaces()
        {
            try
            {
                Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids =
                    new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
                SelectionSet selection = null;
                selection = SelectionManager.GetSelectionSet("Select Surfaces", "*", "AECC_TIN_SURFACE");

                foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId oid in selection.GetObjectIds())
                {
                    oids.Add((oid));
                }
                return oids;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }

            return null;
        }

        public ObjectIdCollection GetIdsByTypeCollection()
        {
            ObjectIdCollection collection = new ObjectIdCollection();

            Func<Type, RXClass> getClass = RXObject.GetClass;

            // You can set this anywhere
            var acceptableTypes = new HashSet<RXClass>
            {
                getClass(typeof(ACAD.Polyline)),
                getClass(typeof(Polyline2d)),
                getClass(typeof(Polyline3d))
            };

            var doc = Application.DocumentManager.MdiActiveDocument;
            using (var trans = doc.TransactionManager.StartOpenCloseTransaction())
            {
                var modelspace = (BlockTableRecord)
                    trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(doc.Database), OpenMode.ForRead);

                var polylineIds = (from id in modelspace.Cast<ObjectId>()
                    where acceptableTypes.Contains(id.ObjectClass)
                    select id).ToList();

                foreach (var id in polylineIds)
                {
                    collection.Add(id);
                }


                trans.Commit();
                return collection;
            }
        }

        //public Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection GetAllPolyLinesInWorkSpace()
        //{
        //    try
        //    {
        //        Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids =
        //            new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
        //        SelectionSet selection = null;
        //        selection = SelectionManager.("Select Polylines");
        //        GetIdsByTypeCollection();
        //        foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId oid in selection.GetObjectIds())
        //        {
        //            oids.Add((oid));
        //        }
        //        return oids;
        //    }
        //    catch (Autodesk.AutoCAD.Runtime.Exception e)
        //    {
        //        PGA.MessengerManager.MessengerManager.LogException(e);
        //    }

        //    return null;
        //}

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
                PGA.MessengerManager.MessengerManager.LogException(e);
            }

            return null;
        }

        /// <exclude />
        public void AddStandardBoundary(ObjectId polyId, string surfacename,
            Autodesk.Civil.DatabaseServices.TinSurface surface)
        {
            ACAD.Polyline poly = null;

            try
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                     
                    //Store boundary
               
                    poly = polyId.GetObject(OpenMode.ForRead) as ACAD.Polyline;
                    var points = AcadUtilities.GetPointsFromPolyline(poly);

                    //Access the BoundariesDefinition object from the surface object

                    Autodesk.Civil.DatabaseServices.SurfaceDefinitionBoundaries surfaceBoundaries =
                        surface.BoundariesDefinition;
                    if (points.Count == 0)
                    {
                        DatabaseLogs.FormatLogs("AddStandardBoundary: Add Boundary Failed: Area = 0");
                        throw new ArgumentException(
                            String.Format("Len={0} Nodes={1} Layer={2} Oid={3}",
                            poly.Length.ToString(),
                            poly.NumberOfVertices.ToString(),
                            poly.Layer.ToString(),
                            poly.ObjectId.ToString()),"AddStandardBoundary");
                    }
                    else
                    {
                        //Add the boundary to the surface
                        surfaceBoundaries.AddBoundaries(points, 0.1, SurfaceBoundaryType.Hide, true);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {

                DatabaseLogs.FormatLogs("Failed AddStandardBoundary:  SurfaceBoundaryType.Hide " + (ex.Message));
                //throw new ArgumentException(
                //           String.Format("Len={0} Nodes={1} Layer={2} Oid={3}",
                //           poly.Length.ToString(),
                //           poly.NumberOfVertices.ToString(),
                //           poly.Layer.ToString(),
                //           poly.ObjectId.ToString()), "AddStandardBoundary");

                ///Todo: Added Support for Boundary Failure 3.10.2017 ///
                ProcessFailedHideBoundarys(polyId, surfacename, surface);
            }
            finally 
            {
                poly.Dispose();
            }

        }
        private void ProcessFailedOuterBoundary(ObjectId polyId, string surfacename,
    Autodesk.Civil.DatabaseServices.TinSurface surface)
        {
            try
            {
                PGA.OffsetPolylines.Program.ScaleObject(polyId);

                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    DatabaseLogs.FormatLogs("Starting Correction For AddStandardBoundary: ");

                    //Store boundary
                    var poly = polyId.GetObject(OpenMode.ForRead) as ACAD.Polyline;
                    var points = AcadUtilities.GetPointsFromPolyline(poly);

                    //Access the BoundariesDefinition object from the surface object

                    Autodesk.Civil.DatabaseServices.SurfaceDefinitionBoundaries surfaceBoundaries =
                        surface.BoundariesDefinition;

                    if (points.Count == 0)
                    {
                        //Throw new Exception and Log

                        DatabaseLogs.FormatLogs("AddStandardBoundary: Add Boundary Failed: Area = 0");
                        throw new ArgumentException(
                            String.Format("Len={0} Nodes={1} Layer={2} Oid={3}",
                            poly.Length.ToString(),
                            poly.NumberOfVertices.ToString(),
                            poly.Layer.ToString(),
                            poly.ObjectId.ToString()), "AddStandardBoundary");
                    }
                    else
                    {
                        //Add the boundary to the surface

                        surfaceBoundaries.AddBoundaries(points, 1.0, SurfaceBoundaryType.Outer, true);
                    }
                    tr.Commit();
                }
                DatabaseLogs.FormatLogs("Ending Correction For AddStandardBoundary: ");

            }
            catch (Exception ex)
            {
                DatabaseLogs.FormatLogs("Failed Correction For AddStandardBoundary: " + (ex.Message));
            }
        }
        private void ProcessFailedOuterBoundary(Point2dCollection inptscol, string surfacename,
Autodesk.Civil.DatabaseServices.TinSurface surface)
        {
            try
            {

                var polyId = PGA.OffsetPolylines.Program.ScaleNewPolylineObject(inptscol);

                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    DatabaseLogs.FormatLogs("Starting Correction For AddStandardBoundary: ");

                    //Store boundary
                    var poly = polyId.GetObject(OpenMode.ForRead) as ACAD.Polyline;
                    var points = AcadUtilities.GetPointsFromPolyline(poly);

                    //Access the BoundariesDefinition object from the surface object

                    Autodesk.Civil.DatabaseServices.SurfaceDefinitionBoundaries surfaceBoundaries =
                        surface.BoundariesDefinition;

                    if (points.Count == 0)
                    {
                        //Throw new Exception and Log

                        DatabaseLogs.FormatLogs("AddStandardBoundary: Add Boundary Failed: Area = 0");
                        throw new ArgumentException(
                            String.Format("Len={0} Nodes={1} Layer={2} Oid={3}",
                            poly.Length.ToString(),
                            poly.NumberOfVertices.ToString(),
                            poly.Layer.ToString(),
                            poly.ObjectId.ToString()), "AddStandardBoundary");
                    }
                    else
                    {
                        //Add the boundary to the surface

                        surfaceBoundaries.AddBoundaries(points, 1.0, SurfaceBoundaryType.Outer, true);
                    }
                    tr.Commit();
                }
                DatabaseLogs.FormatLogs("Ending Correction For AddStandardBoundary: ");

            }
            catch (Exception ex)
            {
                DatabaseLogs.FormatLogs("Failed Correction For AddStandardBoundary: " + (ex.Message));
            }
        }
        private void ProcessFailedHideBoundarys(ObjectId polyId, string surfacename,
            Autodesk.Civil.DatabaseServices.TinSurface surface)
        {
            try
            {
                PGA.OffsetPolylines.Program.ScaleObject(polyId);

                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    DatabaseLogs.FormatLogs("Starting Correction For AddStandardBoundary: ");

                    //Store boundary
                    var poly = polyId.GetObject(OpenMode.ForRead) as ACAD.Polyline;
                    var points = AcadUtilities.GetPointsFromPolyline(poly);

                    //Access the BoundariesDefinition object from the surface object

                    Autodesk.Civil.DatabaseServices.SurfaceDefinitionBoundaries surfaceBoundaries =
                        surface.BoundariesDefinition;

                    if (points.Count == 0)
                    {
                        //Throw new Exception and Log

                        DatabaseLogs.FormatLogs("AddStandardBoundary: Add Boundary Failed: Area = 0");
                        throw new ArgumentException(
                            String.Format("Len={0} Nodes={1} Layer={2} Oid={3}",
                            poly.Length.ToString(),
                            poly.NumberOfVertices.ToString(),
                            poly.Layer.ToString(),
                            poly.ObjectId.ToString()), "AddStandardBoundary");
                    }
                    else
                    {
                        //Add the boundary to the surface

                        surfaceBoundaries.AddBoundaries(points, 1.0, SurfaceBoundaryType.Hide, true);
                    }
                    tr.Commit();
                }
                DatabaseLogs.FormatLogs("Ending Correction For AddStandardBoundary: ");

            }
            catch (Exception ex)
            {
                DatabaseLogs.FormatLogs("Failed Correction For AddStandardBoundary: " + (ex.Message));
            }
        }

        /// <exclude />
        public void RemoveStandardBoundary(ObjectId polyId, string surfacename,
          Autodesk.Civil.DatabaseServices.TinSurface surface)
        {
            try
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
                }
            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs("AddStandardBoundary: " + (ex.Message));
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
            string surfaceName = GetPolyLayerFromOid((oid));

            if (surfaceName==String.Empty)
                return null;

            string surfaceStyleName = GetPolyLayerFromOid((oid)); //layer and style name are the same name.

            int i = 0;
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
                SurfaceStyleManager.CreateDefault(surfaceStyleName);
            }
            CivilTinSurface surface = new CivilTinSurface(surfaceName, surfaceStyleName);
            surface.SetDefaultBuildOptions(); ///Todo: Added 2.20/2017
            m_polylinesurfaces.Add(CivilTinSurface.FindCivilTinSurfaceByName(surface.Name), surfaceName);
            m_polylinesonly.Add(oid, surfaceName);
            return surface;
        }


        public Vector3d GetPLineMaxGeoExtents(ACAD.Polyline polyline)
        {
            return polyline.GeometricExtents.MaxPoint.GetAsVector();
        }

        internal ObjectIdCollection GetAllInternalPolyLinesToSelected(ObjectId oid, ObjectIdCollection oids, double minpolyseparation)
        {
            var intid = new ObjectIdCollection();
            ACAD.Polyline basePolyline = null;
            ACAD.Polyline tempPolyline = null;
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

                            #region Disable Complexity 2/2/2018 
                            //if (PolylineUtils.ExcludePolyBasedOnComplexity(objectId))
                            //    continue; 
                            #endregion

                            basePolyline = GetPolyFromObjId(oid, db);
                            tempPolyline = GetPolyFromObjId(objectId, db);
                        }
                        catch (NullReferenceException ex)
                        {
                            PGA.MessengerManager.MessengerManager.AddLog("GETALLINTERNALPOLYLINESTOSELECTED" +
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
                            PGA.MessengerManager.MessengerManager.AddLog("TempPolyline is Null: " + tempPolyline + ", " + basePolyline);
                            continue;
                        }

                        Point2dCollection tpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(tempPolyline);
                        Point2dCollection bpoints =
                            BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(basePolyline);

                        if (PnPoly.PointInPolyline(bpoints, tpoints))
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
                PGA.MessengerManager.MessengerManager.AddLog
                    ("GETALLINTERNALPOLYLINESTOSELECTED: " + e.Message);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            return null;
        }

        public Vector3d GetPLineMinGeoExtents(ACAD.Polyline polyline)
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
                PGA.MessengerManager.MessengerManager.LogException(e);
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

                ACAD.Polyline lwp = obj as ACAD.Polyline;

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

                ACAD.Polyline lwp = obj as ACAD.Polyline;

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (!lwp.Closed)
                    {
                        PGA.MessengerManager.MessengerManager.AddLog("Polyline is not Closed!");
                    }

                    return lwp.Layer;


                }

                else
                {
                    // If an old-style, 2D polyline

                    Polyline2d p2d = obj as Polyline2d;

                 

                    if (p2d != null)
                    {
                        if (!p2d.Closed)
                        {
                            PGA.MessengerManager.MessengerManager.AddLog("Polyline is not Closed!");
                        }

                        return p2d.Layer;
                         
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        Polyline3d p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            if (!p3d.Closed)
                            {
                                PGA.MessengerManager.MessengerManager.AddLog("Polyline is not Closed!");
                            }
                            return p3d.Layer;
                            
                        }
                    }
                }

                tr.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("Polyline Error!");

            return String.Empty;
        }


        public ACAD.Polyline GetPolyFromObjId(Autodesk.AutoCAD.DatabaseServices.ObjectId oid, Database db)
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
                        PGA.MessengerManager.MessengerManager.LogException(e);
                        return null;
                    }

                    ACAD.Polyline lwp = obj as ACAD.Polyline;

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
                PGA.MessengerManager.MessengerManager.AddLog
                ("GETPOLYFROMOBJID:" + e.Message);
            }
            return null;
        }

        public ACAD.Polyline GetPolyFromObjId(Autodesk.AutoCAD.DatabaseServices.ObjectId oid)
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
                            PGA.MessengerManager.MessengerManager.LogException(e);
                            return null;
                        }

                        ACAD.Polyline lwp = obj as ACAD.Polyline;

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
                    PGA.MessengerManager.MessengerManager.AddLog
                    ("GETPOLYFROMOBJID:" + e.Message);
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

        public static Point3dCollection GetPolyPointsByObjectId(
            Autodesk.AutoCAD.DatabaseServices.ObjectId selectedObjectId)
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

                ACAD.Polyline lwp = obj as ACAD.Polyline;

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

                tr.Commit();
            }
            return points;
        }

        internal static void ListInternalPolylines(Dictionary<ObjectId, ObjectIdCollection> m_dict)
        {
            //ObjectIdCollection existingList;
            //ObjectId key;


            foreach (var obj in m_dict.Keys)
            {
                PGA.MessengerManager.MessengerManager.AddLog
                (obj + "******Key ********" + obj);


                foreach (ObjectIdCollection collection in m_dict.Values)
                {
                    PGA.MessengerManager.MessengerManager.AddLog
                       ("******Collection********");
                    foreach (ObjectId polylines in collection)
                    {
                        PGA.MessengerManager.MessengerManager.AddLog
                              (polylines.Handle.ToString());
                    }

                }
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
        public static bool IsTINExcluded(ObjectId objectid)
        {

            using (DatabaseCommands commands = new DatabaseCommands())
            {
                IList<ExcludedFeatures> excludedfeatures;
                excludedfeatures = commands.GetAllExcludedFeatures_V2();

                foreach (var str in excludedfeatures)
                {
                    ObjectId compare = GetObjectId(str.Handle.Trim());
                    if (objectid.Equals(compare))
                    {
                        PGA.Database.DatabaseLogs.FormatLogs("Filtered Polylines");
                        return true;
                    }
                }
            }

            return false;
        }
        public bool CreateSurfaceForPolylines(ObjectIdCollection polylines)
        {
            try
            {
               

                foreach (ObjectId polyline in polylines)
                {
                    try
                    {
                           if (!IsTINExcluded(polyline))
                            CreateSelectedSurface(polyline);
                    }
                    catch (System.Exception e)
                    {
                        PGA.MessengerManager.MessengerManager.LogException(e);

                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                PGA.MessengerManager.MessengerManager.LogException(e);
            }
            return false;
        }

        private static Dictionary<ObjectId, string> m_polylinesurfaces;
        private static Dictionary<ObjectId, string> m_polylinesonly;

        public bool AddBoundariesForSurfaces(ObjectIdCollection polylines)
        {

            try
            {
                foreach (KeyValuePair<ObjectId, string> pair in m_polylinesonly)
                {
                    TinSurface surface = null;  ///Todo: Modified 3/10/17///
                    var name = "";
                    try
                    {

                        #region Create Outer Boundary

                        using (Transaction tr = CivilApplicationManager.StartTransaction())
                        {
                            ObjectId lObjectId = new ObjectId();

                            lObjectId = CivilTinSurface.FindCivilTinSurfaceByName(pair.Value);
                            //1. Create new surface if no existing surface passed in 
                            surface = lObjectId.GetObject(OpenMode.ForRead) as TinSurface;
                            name = surface.Name;
                            //2. Store boundary
                            ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                            boundaryEntities = GetObjectIdCollectionFromEntity(pair.Key);

                            //3. Access the BoundariesDefinition object from the surface object
                            SurfaceDefinitionBoundaries surfaceBoundaries =
                                surface.BoundariesDefinition;

                            //4. now add the boundary to the surface
                            var sbo = surfaceBoundaries.AddBoundaries(boundaryEntities, 0.1, SurfaceBoundaryType.Outer, true);
                            sbo.Name = "Boundary-" +  name + "-" + DateTime.Now.Millisecond;

                            tr.Commit();
 
                        }

                        #endregion

                    }
                    catch (System.Exception ex)
                    {
                        DatabaseLogs.FormatLogs(String.Format("Error Add Boundaries for Surface! {0}", ex.Message));

                        try
                        {
                            ProcessFailedOuterBoundary(pair.Key, name, surface);
                        }
                        catch (Exception)
                        {
                            ProcessOverlapOuterBoundary(name, surface);  //Disable Existing Outer Polyline
                        }
                    }

                }
                return true;
            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs( (ex.Message));
            }
            return false;
        }

        public CivilTinSurface FindSurfaceIdForPolylineV1(ObjectId oid)
        {
            try
            {

                foreach (KeyValuePair<ObjectId, string> id in m_polylinesurfaces)
                {
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

                foreach (KeyValuePair<ObjectId, string> id in m_polylinesonly)
                {
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

        public void AddBoundariesForSurfaces(Point2dCollection polylinepnts, ObjectId surfaceid)
        {
            var name = "";
            TinSurface surface = null;
            try
            {
                #region Create Outer Boundary

                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    surface = CivilTinSurface.FindSurfaceBySurfaceId(surfaceid);
                    if (surface == null)
                        return;
                    name = surface.Name;
                   // surface.UpgradeOpen();
                    ////2. Store boundary
                    //ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                    // boundaryEntities = GetObjectIdCollectionFromEntity(polylines);
                    
                    //3. Access the BoundariesDefinition object from the surface object
                    SurfaceDefinitionBoundaries surfaceBoundaries =
                        surface.BoundariesDefinition;

                    //4. now add the boundary to the surface
                    surfaceBoundaries.AddBoundaries(polylinepnts, 0.1, SurfaceBoundaryType.Outer, true);
                   // surface.DowngradeOpen();

                    tr.Commit();

                }

                #endregion

            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs(String.Format("Error Add Boundaries for Surface! {0}", ex.Message));

                try
                {
                    ProcessFailedOuterBoundary(polylinepnts, name, surface);  //Inserts Outer Polyline
                }
                catch (Exception)
                {
                    ProcessOverlapOuterBoundary(name, surface);  //Disable Existing Outer Polyline
                }
            }
            
        }

        private void ProcessOverlapOuterBoundary( string name, TinSurface surface)
        {
            try
            {
                var outbound = PGA.SurfaceManager.SurfaceManager.GetOuterBoundaryCollection(surface);
                var polyId   = PGA.OffsetPolylines.Program.ScaleNewPolylineObject(outbound);

                PGA.SurfaceManager.SurfaceManager.DisableOuterBoundary(surface);

                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    DatabaseLogs.FormatLogs("Starting Correction For Outer Boundary: ");

                    var poly   = polyId.GetObject(OpenMode.ForRead) as ACAD.Polyline;
                    var points = AcadUtilities.GetPointsFromPolyline(poly);

                    ProcessFailedOuterBoundary(points, name, surface);  //Inserts Outer Polyline
             
                    tr.Commit();
                }

            }
            catch (Exception ex)
            {
                DatabaseLogs.FormatLogs("Ending Correction For Outer Boundary: " + (ex.Message));
            }
        }

        public void AddBoundariesForSurfaces(ObjectId polylines, ObjectId surfaceid)
        {
            var name = "";
            TinSurface surface = null;
            try
            {

                #region Create Outer Boundary

                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    surface = CivilTinSurface.FindSurfaceBySurfaceId(surfaceid);
                    if (surface == null)
                        return;

                    // surface.UpgradeOpen();
                    ////2. Store boundary
                    ObjectIdCollection boundaryEntities = new ObjectIdCollection();
                    boundaryEntities = GetObjectIdCollectionFromEntity(polylines);

                    //3. Access the BoundariesDefinition object from the surface object
                    SurfaceDefinitionBoundaries surfaceBoundaries =
                        surface.BoundariesDefinition;

                    //4. now add the boundary to the surface
                    surfaceBoundaries.AddBoundaries(boundaryEntities, 0.1, SurfaceBoundaryType.Outer, true);
                    // surface.DowngradeOpen();

                    tr.Commit();

                }

                #endregion

                AddHandletoDescription(surface, polylines);
            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs(String.Format("Error Add Boundaries for Surface! {0}", ex.Message));

                try
                {
                    ProcessFailedOuterBoundary(polylines, name, surface);
                }
                catch (Exception)
                {
                    ProcessOverlapOuterBoundary(name, surface);  //Disable Existing Outer Polyline
                }
            }

        }

        private void AddHandletoDescription(TinSurface surface, ObjectId polylines)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                if (surface == null)
                    return;

                surface.UpgradeOpen();
                surface.Description = polylines.Handle.ToString();
                surface.DowngradeOpen();

                tr.Commit();

            }
        }

        public static string GetPolylineName(ObjectId selectedObjectId)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            // var counter = 0;
            PlanarEntity plane;

            //Get the current UCS

            var ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            plane = new Plane(ucs.Origin, ucs.Xaxis);

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                //"lightweight" (or optimized) polyline

                var lwp = obj as ACAD.Polyline;

                if (lwp != null)
                {
                    return lwp.Layer;
                }

                // If an old-style, 2D polyline

                var p2d = obj as Polyline2d;


                if (p2d != null)
                {
                    return p2d.Layer;
                }

                // If an old-style, 3D polyline

                var p3d = obj as Polyline3d;

                if (p3d != null)
                {
                    return p3d.Layer;
                }


                // Committing is cheaper than aborting

                tr.Commit();
            }
            return null;
        }

    }
}
