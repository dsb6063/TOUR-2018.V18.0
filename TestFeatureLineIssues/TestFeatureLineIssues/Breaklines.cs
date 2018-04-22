#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.Settings;
using BBC.Common.AutoCAD;
using C3DLandDb = Autodesk.Civil.DatabaseServices;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Exception = System.Exception;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using ObjectIdCollection = Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;

#endregion

namespace TestFeatureLineIssues
{
    public interface IBreaklines
    {
        ObjectId GetSiteId(ObjectId surfaceId);
        ObjectId FindCivilTinSurface(string surfaceName);
        Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection GetAllSurfaces();
    }

    public class Breaklines : IBreaklines
    {
        #region Global Fields Declaration

        private DateTime _time;
        private string _drawingName;
        private IList<string> _listDwgs;
        private List<Polyline3d> _polylines;
        public ObjectIdCollection _originalPolys;

        #endregion

        public Breaklines(DateTime time, IList<string> listDwgs)
        {
            #region Assign Fields

            _time = time;
            _drawingName = "";
            //_listDwgs = new List<string>(listDwgs);
            _polylines = new List<Polyline3d>();
            _originalPolys = new ObjectIdCollection();

            #endregion
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
            }

            return null;
        }

        public ObjectId FindCivilTinSurface(string surfaceName)
        {
            ObjectId requested = findSurface(surfaceName);
            if (requested == ObjectId.Null)
            {
                return ObjectId.Null;
            }

            return requested;
        }

        public ObjectId findSurface(string surfaceName)
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

        public void AddStandardBreakline(ObjectId SurfaceId, ObjectId polyId, string name)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                C3DLandDb.TinSurface surface = SurfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                ObjectIdCollection entities = new ObjectIdCollection();
                entities.Add(polyId);
                surface.BreaklinesDefinition.AddStandardBreaklines(entities, 1.0, 1.0, 1.0, 0.0);
                tr.Commit();
            }
        }

        public ObjectId AddCivil2016BreaklineByTrans(ObjectId SurfaceId, ObjectId polyId, ObjectId siteId, string name)
        {
            ObjectId featureId = ObjectId.Null;
            C3DLandDb.FeatureLine feature = null;
            Polyline3d poly = null;
            var layername = "";
            Debug.WriteLine("Starting AddCivil2016BreaklineByTrans");

            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (Transaction tr = CivilApplicationManager.StartTransaction())
                    {
                        var checkPoly = polyId.GetObject
                            (OpenMode.ForRead, false, true) as Polyline;

                        if (checkPoly != null)
                        {
                            featureId = C3DLandDb.FeatureLine.Create
                                (GetFeatureLineName(), polyId, siteId);

                            layername = checkPoly.Layer;
                            var color = checkPoly.Color;

                            feature = featureId.GetObject(OpenMode.ForWrite) as C3DLandDb.FeatureLine;

                            feature.Layer = layername;
                            feature.Color = color;
                        }
                        else
                        {
                            tr.Abort();
                            Debug.WriteLine("Aborted AddCivil2016BreaklineByTrans " +
                                            layername + " Count: " + checkPoly.NumberOfVertices);
                        }

                        tr.Commit();

                        Debug.WriteLine("Ending AddCivil2016BreaklineByTrans" +
                                        layername + " Count: " + checkPoly.NumberOfVertices);

                        return featureId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetDrawingName(DateTime time, string name)
        {
            _time = time;
            _drawingName = name;

            var directory = Path.GetDirectoryName(name);
            var filename = Path.GetFileName(name);
            name = Path.Combine(directory, "BL_" + filename);

            //PGA.Database.DatabaseCommands commands = new PGA.Database.DatabaseCommands();
            //var result = commands.GetSettingsByDate(time);
            //var drawing = Path.Combine(result.DestinationFolder, "Breaklines");
            //if (!Directory.Exists(drawing))
            //    Directory.CreateDirectory(drawing);

            //return Path.Combine(drawing, "BL_" + name);
            return name;
        }


        /// <exclude />
        public bool AddStandardBoundary(ObjectId polyId, string name, ObjectId surfaceId)
        {
            try
            {
                var db = CivilApplicationManager.WorkingDatabase;
                var doc = CivilDocument.GetCivilDocument(db);
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    C3DLandDb.TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    //surface.UpgradeOpen();
                    FeatureLineIssues.AddStandardBoundary(polyId, name, surface);
                    tr.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public void AddCivil2016Breakline(ObjectId SurfaceId, ObjectId polyId, ObjectId siteId, string name)
        {
            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                ObjectId featureId = ObjectId.Null;
                C3DLandDb.FeatureLine feature = null;

                //ObjectId featureId = C3DLandDb.FeatureLine.Create(GetFeatureLineName(), polyId, GetSiteId(ObjectId.Null));
                var checkPoly = polyId.GetObject(OpenMode.ForRead, false, true) as Polyline;

                if (checkPoly != null)
                    featureId = C3DLandDb.FeatureLine.Create(GetFeatureLineName(), polyId, siteId);

                feature = (C3DLandDb.FeatureLine) featureId.GetObject(OpenMode.ForRead);
                feature.AssignElevationsFromSurface(SurfaceId, true);
                var p3Dcollection = feature.GetPoints(FeatureLinePointType.AllPoints);

                //Polyline3d poly = new Polyline3d(Poly3dType.SimplePoly, p3Dcollection, true);
                //tr.AddNewlyCreatedDBObject(poly,true);

                C3DLandDb.TinSurface surface = SurfaceId.GetObject(OpenMode.ForWrite) as C3DLandDb.TinSurface;

                //ObjectIdCollection entities = new ObjectIdCollection();
                //entities.Add(poly.ObjectId);
                surface.BreaklinesDefinition.AddStandardBreaklines(p3Dcollection, 1.0, 2.0, 1.0, 0.0);
                tr.Commit();
            }
        }

        public ObjectId GetSiteId(ObjectId surfaceId)
        {
            var site = ObjectId.Null;

            using (Transaction tr = CivilApplicationManager.StartTransaction())

            {
                Autodesk.AutoCAD.DatabaseServices.Database db = CivilApplicationManager.WorkingDatabase;
                var doc = CivilDocument.GetCivilDocument(db);
                ObjectIdCollection siteIds = doc.GetSiteIds();
                if (siteIds != null && siteIds.Count != 0)
                {
                    site = (from s in siteIds.Cast<ObjectId>()
                        select s).FirstOrDefault();
                }

                if (site == ObjectId.Null)
                {
                    site = C3DLandDb.Site.Create(doc, "Site-ALL");
                }

                tr.Commit();
            }
            return site;
        }

        public void SetSurfaceBuildOptions()
        {
            CrossingBreaklinesElevationType crossing = CrossingBreaklinesElevationType.UseNone;
        }

   

        private string GetFeatureLineName()
        {
            Random random = new Random();
            return string.Format("FL-{0}{1}", DateTime.Now.Millisecond, random.Next(100));
        }

        public void AddFeatureLinesToAllSurface(ObjectIdCollection featureIds, ObjectId surfaceId)
        {
            Debug.WriteLine("Starting AddFeatureLinesToAllSurface");
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {
                    C3DLandDb.TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                    surface.UpgradeOpen();
                    surface.BreaklinesDefinition.AddStandardBreaklines(featureIds, 1.0, 2.0, 2.0, 0.0);
                    surface.DowngradeOpen();

                    tr.Commit();
                    Debug.WriteLine("Ending AddFeatureLinesToAllSurface");
                }
            }
        }

        public void AddCivil2016ElevationsToFeature(ObjectId surfaceId, ObjectId featureId, ObjectId siteId,
            object o)
        {
            try
            {
                Debug.WriteLine("Starting AddCivil2016ElevationsToFeature");
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (Transaction tr = CivilApplicationManager.StartTransaction())
                    {
                        var feature = (C3DLandDb.FeatureLine) featureId.GetObject(OpenMode.ForRead);
                        feature.UpgradeOpen();
                        feature.AssignElevationsFromSurface(surfaceId, false);
                        feature.DowngradeOpen();

                        //Check and Refine Breakline
                        //FeatureLineManager.RecurseElevFromSurface();

                        var p3Dcollection = feature.GetPoints(FeatureLinePointType.AllPoints);
                        //var poly3d = new Polyline3d(Poly3dType.SimplePoly, p3Dcollection, true);

                        C3DLandDb.TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) as C3DLandDb.TinSurface;
                        surface.UpgradeOpen();
                        ///Todo:Handle Breaklines for Water and Bulkhead
                        surface.BuildOptions.ExecludeMinimumElevation = true;
                        surface.BuildOptions.MinimumElevation = 0.1;
                        surface.BuildOptions.CrossingBreaklinesElevationOption =
                            CrossingBreaklinesElevationType.UseNone;
                        surface.BreaklinesDefinition.AddStandardBreaklines(p3Dcollection, 1.0, 2.0, 2.0, 0.0);
                        surface.DowngradeOpen();

                        tr.Commit();
                        Debug.WriteLine("Ending AddCivil2016ElevationsToFeature");
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
            }
            catch (System.AccessViolationException ex)
            {
            }
        }

        [CommandMethod("RemovePaste")]
        public void RemovePastecommandByCmd()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            PromptEntityOptions entOpts = new PromptEntityOptions("\nSelect Surface:");
            entOpts.SetRejectMessage("...not a Surface, try again.");
            entOpts.AddAllowedClass(typeof (C3DLandDb.TinSurface), true);
            PromptEntityResult entRes = ed.GetEntity(entOpts);
            if (entRes.Status != PromptStatus.OK)
                return;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                C3DLandDb.TinSurface surf = (C3DLandDb.TinSurface) entRes.ObjectId.GetObject(OpenMode.ForRead);
                C3DLandDb.SurfaceOperationCollection ops = surf.Operations;
                entOpts.Message = "Select pasted surface to remove: ";
                entRes = ed.GetEntity(entOpts);
                if (entRes.Status != PromptStatus.OK)
                    return;
                for (int i = 0; i < ops.Count; i++)
                {
                    C3DLandDb.SurfaceOperationPasteSurface op = ops[i] as C3DLandDb.SurfaceOperationPasteSurface;
                    if (op == null)
                        continue;
                    if (op.SurfaceId == entRes.ObjectId)
                    {
                        ops.Remove(op);
                        break;
                    }
                }
                surf.Rebuild();
                tr.Commit();
            }
        }

        /// <summary>
        /// Removes the pasted objects.
        /// </summary>
        /// <param name="entRes">The surface id resource.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void RemoveBoundaryOperations(ObjectId entRes)
        {
            #region SDI Non-Session

            // Document doc = Application.DocumentManager.MdiActiveDocument;
            //Editor ed = doc.Editor; 

            #endregion

            CivilDocument doc = CivilDocument.GetCivilDocument(CivilApplicationManager.WorkingDatabase);

            Autodesk.AutoCAD.DatabaseServices.Database db = CivilApplicationManager.WorkingDatabase;

            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    C3DLandDb.TinSurface surf = (C3DLandDb.TinSurface) entRes.GetObject(OpenMode.ForRead);
                    if (surf == null) throw new ArgumentNullException(nameof(surf));

                    C3DLandDb.SurfaceOperationCollection ops = surf.Operations;
                    C3DLandDb.SurfaceDefinitionBoundaries ops1 = surf.BoundariesDefinition;

                    #region Surface Add Boundary Operation

                    for (int i = 0; i < ops.Count; i++)

                    {
                        C3DLandDb.SurfaceOperationAddBoundary op = ops[i] as C3DLandDb.SurfaceOperationAddBoundary;

                        if (op == null)
                            continue;
                        if (op.Count > 0)
                        {
                            ops.Remove(op);
                            break;
                        }
                    }

                    #endregion

                    #region Surface Existing Boundary Definitions

                    for (int i = 0; i < ops1.Count; i++)

                    {
                        if (ops1.Count == 0)
                            continue;
                        if (ops1.Count > 0)
                        {
                            ops1.RemoveAt(i);
                            break;
                        }
                    }

                    #endregion

                    surf.Rebuild();
                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
            }
        }


        public object OpenFeatureLineForSave(ObjectId featureId)
        {
            Debug.WriteLine("Starting OpenFeatureLineForSave");

            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                var feature = (C3DLandDb.FeatureLine) featureId.GetObject(OpenMode.ForWrite, false, true);
                var p3Dcollection = feature.GetPoints(FeatureLinePointType.AllPoints);

                var poly3d = new Polyline3d(Poly3dType.SimplePoly, p3Dcollection, true);
                poly3d.Layer = feature.Layer;
                poly3d.Color = feature.Color;
                _polylines.Add(poly3d);

                //C3DLandDb.TinSurface surface = surfaceId.GetObject(OpenMode.ForWrite) as C3DLandDb.TinSurface;
                //surface.BreaklinesDefinition.AddStandardBreaklines(p3Dcollection, 1.0, 2.0, 5.0, 0.0);

                tr.Commit();
                Debug.WriteLine("Ending OpenFeatureLineForSave");
                return poly3d;
            }
        }

        public ObjectId GetFeatureLineSiteId(ObjectId featureId)
        {
            Debug.WriteLine("Starting GetFeatureLineSiteId");

            using (Transaction tr = CivilApplicationManager.StartTransaction())
            {
                var feature = (C3DLandDb.FeatureLine) featureId.GetObject(OpenMode.ForRead, false, true);
                var siteid = feature.SiteId;

                Debug.WriteLine("Ending GetFeatureLineSiteId");
                return siteid;
            }
        }

        public ObjectId GetNewSiteId()
        {
            var site = ObjectId.Null;
            Random rand = new Random();
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (Transaction tr = CivilApplicationManager.StartTransaction())
                    {
                        var num = rand.Next(1000, 9999).ToString();

                        Autodesk.AutoCAD.DatabaseServices.Database db = CivilApplicationManager.WorkingDatabase;
                        var doc = CivilDocument.GetCivilDocument(db);

                        site = C3DLandDb.Site.Create(doc, "Site-" + num);

                        tr.Commit();
                    }
                }
            }
            catch (System.ArgumentException)
            {
                GetNewSiteId();
            }

            return site;
        }
    }
}
