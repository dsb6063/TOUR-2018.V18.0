using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using C3DSurfacesDemo;
using PGA.Civil.Logging;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = System.Exception;


[assembly: CommandClass(typeof (C3DSurfacesDemoCommands))                                ]

namespace C3DSurfacesDemo
{
    public class C3DSurfacesDemoCommands
    {
        public static double minpolyseparation;
        public static string sminpolyseparation;

        [CommandMethod("PGA-CreateAllSurfaces")]
        public void CreateAllSurfaces()
        {
            
            try
            {
                var polylines =
                    new ObjectIdCollection();
                Dictionary<ObjectId, ObjectIdCollection> m_dict;
                var surfaceManager = new PasteSurfaces();
                m_dict = new Dictionary<ObjectId, ObjectIdCollection>();
                #region Steps
                //1 Create Overall Surface
                //2 Create new Surface with Defaults
                //3 Open new Surface to Paste Overall Surface
                //4 Select Surface to Paste Overall Surface Into
                //5 Add Boundary to New Surface
                //1 Select All Polylines
                //2 Determine if boundaries are needed to be type inner or outer
                //3 Filter Polylines based on layer name. May be able to know the type. 
                #endregion

                try
                {
                    sminpolyseparation = EditorUtils.PromptForString("\nEnter polyline separation distance (0.05'): ");

                    if (String.IsNullOrEmpty(sminpolyseparation)) { minpolyseparation = 0.05; }
                    else { minpolyseparation = Convert.ToDouble(sminpolyseparation); }
                }
                catch (Exception)
                {
                    minpolyseparation = 0.05; //set default
                }

                if (CivilTinSurface.FindCivilTinSurface("All"))
                {

                    #region Insert Point Cloud into Overall Surface


                    #endregion

                    #region get polylines and generate surfaces and boundaries

                    polylines = surfaceManager.GetAllPolyLines();
                    //Create a surface hive to generate a TIN surface
                    if (!surfaceManager.CreateSurfaceForPolylines(polylines))
                        throw new SystemException("Create surface for polylines failed!");
                    if (!surfaceManager.PasteAllSurfaces(CivilTinSurface.GetCivilSurfaceBySurfaceName("All")))
                        throw new SystemException("Pasting Surfaces failed!");
                    if (!surfaceManager.AddBoundariesForSurfaces(polylines))
                        throw new SystemException("Add overall Boundaries for surfaces failed!");

                    #endregion
                    ACADLogging.LogMyExceptions("Store boundaries to Object Dictionary");
                    ObjectIdCollection internalPLines = null;
                    foreach (ObjectId baseObj in polylines)
                    {
                        #region store the internal boundaries for the selected base object in dict
                        if (GetPolyFromObjId(baseObj) == null)
                            continue;

                        internalPLines = surfaceManager.GetAllInternalPolyLinesToSelected(baseObj, polylines, minpolyseparation);

                        m_dict.Add(baseObj, internalPLines);

                        //if (internalPLines == null) continue;

                        #endregion
                    }

                    #region Iterate through inner boundaries
                    ACADLogging.LogMyExceptions("Start Iterate through inner boundaries");
                    CivilTinSurface lsurface=null;
                    foreach (KeyValuePair<ObjectId, ObjectIdCollection> innerbdy in m_dict)
                    {
                        #region Removed

                        //#region Create Surface for that Base Polyline and Add outer boundary

                        //if ((lsurface = (surfaceManager.CreateSelectedSurface(baseObj))) == null)
                        //    throw new Exception("CreateSelectedSurface Failed!");

                        //lsurface.AddStandardBoundary(baseObj, "OuterBoundary");
                        //#endregion

                        #endregion

                        if (innerbdy.Value == null) continue;

                        ObjectId outerPline = innerbdy.Key;
                        ObjectIdCollection innerIdCollection = innerbdy.Value;


                        lsurface = PasteSurfaces.FindSurfaceIdForPolylineV2(outerPline);

                        if (lsurface == null)
                            continue;

                        #region Interate Internal Polylines to add breaklines and boundaries
                        ACADLogging.LogMyExceptions("Start AddStandardInnerBoundary");
                        if (innerIdCollection != null)
                            foreach (ObjectId pLines in innerIdCollection)
                            {
                                if (pLines == null) continue;
                                try
                                {
                                    #region Breaklines Deprecated
                                    //Breaklines removed due to overlapping
                                    //lsurface.AddStandardBreakline
                                    //    (PasteSurfaces.GetPolyPointsByObjectId(pLines), "Breakline-"); 
                                    #endregion
                                    lsurface.AddStandardInnerBoundary(pLines, "Boundary-");
                                }
                                catch (NullReferenceException e)
                                {
                                    ACADLogging.LogMyExceptions("AddAsInnerBoundary Failed: " + e.Message);
                                }
                                catch (Exception e)
                                {
                                    ACADLogging.LogMyExceptions("AddAsInnerBoundary Failed: " + e.Message);
                                }
                                internalPLines = null;
                            }
                        ACADLogging.LogMyExceptions("End AddStandardInnerBoundary");

                        #endregion
                        #region Rebuild Surfaces
                        ACADLogging.LogMyExceptions("Start RebuildSurfaceBySurfaceName");
                        if (lsurface != null)
                            CivilTinSurface.RebuildSurfaceBySurfaceName(lsurface.Name); 
                        #endregion
                    }

                    #endregion

                                    
                    internalPLines = null;
                    ACADLogging.LogMyExceptions("End Iterate through inner boundaries");
                }
                else
                {
                    EditorUtils.Write("Missing 'All' Surface. Create Surfaces Incomplete!");
                    throw new FileNotFoundException(
                     "Did not find the surface ALL!");
                }

                EditorUtils.Write("Create Surfaces Complete!\n");
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                ACADLogging.LogMyExceptions(e.Message);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ACADLogging.LogMyExceptions(e.Message);
            }
            catch (Exception e)
            {
                ACADLogging.LogMyExceptions(e.Message);
            }
        }

        //[CommandMethod("PGA-CreateSingleSurface")]
        public void CreateSingleSurface()
        {
            try
            {
                var polylines =
                    new ObjectIdCollection();
                Dictionary<ObjectId, ObjectIdCollection> m_dict;
                var surfaceManager = new PasteSurfaces();
                m_dict = new Dictionary<ObjectId, ObjectIdCollection>();
                #region Steps
                //1 Create Overall Surface
                //2 Create new Surface with Defaults
                //3 Open new Surface to Paste Overall Surface
                //4 Select Surface to Paste Overall Surface Into
                //5 Add Boundary to New Surface
                //1 Select All Polylines
                //2 Determine if boundaries are needed to be type inner or outer
                //3 Filter Polylines based on layer name. May be able to know the type. 
                #endregion


                if (CivilTinSurface.FindCivilTinSurface("All"))
                {

                    #region Insert Point Cloud into Overall Surface

                    ///TODO///
                    #endregion

                    #region get polylines and generate surfaces and boundaries

                    polylines = surfaceManager.GetAllPolyLines();
                    //Create a surface hive to generate a TIN surface
                    if (!surfaceManager.CreateSurfaceForPolylines(polylines))
                        throw new SystemException("Create surface for polylines failed!");
                    if (!surfaceManager.PasteAllSurfaces(CivilTinSurface.GetCivilSurfaceBySurfaceName("All")))
                        throw new SystemException("Pasting Surfaces failed!");
                    if (!surfaceManager.AddBoundariesForSurfaces(polylines))
                        throw new SystemException("Add overall Boundaries for surfaces failed!");

                    #endregion

                    ObjectIdCollection internalPLines = null;
                    foreach (ObjectId baseObj in polylines)
                    {
                        #region store the internal boundaries for the selected base object in dict
                        if (GetPolyFromObjId(baseObj) == null)
                            continue;

                        internalPLines = surfaceManager.GetAllInternalPolyLinesToSelected(baseObj, polylines, minpolyseparation);

                        m_dict.Add(baseObj, internalPLines);

                        //if (internalPLines == null) continue;

                        #endregion
                    }

                    #region Iterate through inner boundaries

                    CivilTinSurface lsurface = null;
                    foreach (KeyValuePair<ObjectId, ObjectIdCollection> innerbdy in m_dict)
                    {
                        #region Removed

                        //#region Create Surface for that Base Polyline and Add outer boundary

                        //if ((lsurface = (surfaceManager.CreateSelectedSurface(baseObj))) == null)
                        //    throw new Exception("CreateSelectedSurface Failed!");

                        //lsurface.AddStandardBoundary(baseObj, "OuterBoundary");
                        //#endregion

                        #endregion

                        if (innerbdy.Value == null) continue;

                        ObjectId outerPline = innerbdy.Key;
                        ObjectIdCollection innerIdCollection = innerbdy.Value;

                        if (outerPline == null) continue;

                        lsurface = PasteSurfaces.FindSurfaceIdForPolylineV2(outerPline);

                        if (lsurface == null)
                            continue;

                        #region Interate Internal Polylines to add breaklines and boundaries

                        if (innerIdCollection != null)
                            foreach (ObjectId pLines in innerIdCollection)
                            {
                                try
                                {
                                    #region Breaklines Deprecated
                                    //Breaklines removed due to overlapping
                                    //lsurface.AddStandardBreakline
                                    //    (PasteSurfaces.GetPolyPointsByObjectId(pLines), "Breakline-"); 
                                    #endregion
                                    lsurface.AddStandardInnerBoundary(pLines, "Boundary-");
                                }
                                catch (NullReferenceException e)
                                {
                                    ACADLogging.LogMyExceptions("AddAsInnerBoundary Failed: " + e.Message);
                                }
                                catch (Exception e)
                                {
                                    ACADLogging.LogMyExceptions("AddAsInnerBoundary Failed: " + e.Message);
                                }
                            }
                        internalPLines = null;

                        #endregion
                        #region Rebuild Surfaces
                        if (lsurface != null)
                            CivilTinSurface.RebuildSurfaceBySurfaceName(lsurface.Name);
                        #endregion
                    }

                    #endregion


                    internalPLines = null;

                }
                else
                {
                    throw new FileNotFoundException(
                     "Did not find the surface ALL!");
                }

                EditorUtils.Write("Create Surfaces Complete!");

            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                ACADLogging.LogMyExceptions(e.Message);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ACADLogging.LogMyExceptions(e.Message);
            }
            catch (Exception e)
            {
                ACADLogging.LogMyExceptions(e.Message);
            }
        }

        [CommandMethod("PGA-RebuildAllSurfaces")]
        public void RebuildAllSurfaces()
        {
            try
            {
                var polylines =
                    new ObjectIdCollection();
                Dictionary<ObjectId, ObjectIdCollection> m_dict;
                 
                var surfaceManager = new PasteSurfaces();
                m_dict = new Dictionary<ObjectId, ObjectIdCollection>();

                if (CivilTinSurface.FindCivilTinSurface("All"))
                {

                    #region get polylines and generate surfaces and boundaries

                    polylines = surfaceManager.GetAllPolyLines();
         
                    #endregion

                    ObjectIdCollection internalPLines = null;
                    foreach (ObjectId baseObj in polylines)
                    {
                        #region store the internal boundaries for the selected base object in dict
                        if (GetPolyFromObjId(baseObj) == null)
                            continue;

                        internalPLines = surfaceManager.GetAllInternalPolyLinesToSelected(baseObj, polylines, minpolyseparation);

                        m_dict.Add(baseObj, internalPLines);

                        #endregion
                    }

                    #region Iterate through inner boundaries

                    CivilTinSurface lsurface = null;
                    foreach (KeyValuePair<ObjectId, ObjectIdCollection> innerbdy in m_dict)
                    {
                        #region Removed

                        //#region Create Surface for that Base Polyline and Add outer boundary

                        //if ((lsurface = (surfaceManager.CreateSelectedSurface(baseObj))) == null)
                        //    throw new Exception("CreateSelectedSurface Failed!");

                        //lsurface.AddStandardBoundary(baseObj, "OuterBoundary");
                        //#endregion

                        #endregion

                        if (innerbdy.Value == null) continue;

                        ObjectId outerPline = innerbdy.Key;
                        ObjectIdCollection innerIdCollection = innerbdy.Value;


                        lsurface = PasteSurfaces.FindSurfaceIdForPolylineV2(outerPline);

                        if (lsurface == null)
                            continue;

                        #region Interate Internal Polylines to add breaklines and boundaries

                        if (innerIdCollection != null)
                            foreach (ObjectId pLines in innerIdCollection)
                            {
                                try
                                {
                                    #region Breaklines Deprecated
                                    //Breaklines removed due to overlapping
                                    //lsurface.AddStandardBreakline
                                    //    (PasteSurfaces.GetPolyPointsByObjectId(pLines), "Breakline-"); 
                                    #endregion
                                    lsurface.AddStandardInnerBoundary(pLines, "Boundary-");
                                }
                                catch (NullReferenceException e)
                                {
                                    ACADLogging.LogMyExceptions("AddAsInnerBoundary Failed: " + e.Message);
                                    //throw new Exception(e.Message);
                                }
                                internalPLines = null;
                            }

                        #endregion
                        #region Rebuild Surfaces
                        if (lsurface != null)
                            CivilTinSurface.RebuildSurfaceBySurfaceName(lsurface.Name); 
                        #endregion
                        #region Regenerate Graphics
                        EditorUtils.Regen(); 
                        #endregion
                    }

                    #endregion


                    internalPLines = null;

                }
                else
                {
                    throw new FileNotFoundException(
                     "Did not find the surface ALL!");
                }

                EditorUtils.Write("\nRebuild Surfaces Complete!\n");

            }
            catch (NullReferenceException e)
            {
                EditorUtils.Write(e.StackTrace);
                ACADLogging.LogMyExceptions(e.Message);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ACADLogging.LogMyExceptions(e.Message);
            }
            catch (Exception e)
            {
                ACADLogging.LogMyExceptions(e.Message);
            }
        }
        private static void ZoomAll()
        {
            //Zoom All Fails
            BBC.Common.AutoCAD.AcadUtilities.ZoomAll();
        }

        [CommandMethod("PGA-ExportSurfaceObjects")]
        public static void ExportSurfaceObjects()
        {
            PasteSurfaces surfaceManager = new PasteSurfaces();
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Editor   ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                ObjectIdCollection surfaces = surfaceManager.GetAllSurfaces();

                foreach (ObjectId surfaceId in surfaces)
                {
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;

                        surface.GetTriangles(true);


                        trans.Commit();
                    }

                }
            }
            catch (Exception e)
            {
                ACADLogging.LogMyExceptions(e.Message);
            }

        }
        //[CommandMethod("PGA-ExportTo3DSMax")]
        //public static void Export3DSMax()
        //{
        //    PasteSurfaces surfaceManager = new PasteSurfaces();
        //    Database db = Application.DocumentManager.MdiActiveDocument.Database;
        //    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

        //    try
        //    {
        //        ObjectIdCollection surfaces = surfaceManager.GetAllSurfaces();

        //        foreach (ObjectId surfaceId in surfaces)
        //        {
        //            using (Transaction trans = db.TransactionManager.StartTransaction())
        //            {
        //                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;

        //                surface.GetTriangles(true);


        //                trans.Commit();
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ACADLogging.LogMyExceptions(e.Message);
        //    }

        //}
        public static Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection CloseSelectedPolylines()
        {
             
            Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids;
            oids = new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
            
            SelectionSet selection;
            selection = BBC.Common.AutoCAD.SelectionManager.GetSelectionSet("");

            foreach (ObjectId obj in selection.GetObjectIds())
            {
                oids.Add(obj);
            }
            return oids;
        }
        public Polyline GetPolyFromObjId(Autodesk.AutoCAD.DatabaseServices.ObjectId oid)
        {
            using (Database db = CivilApplicationManager.WorkingDatabase)
            {

                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        Autodesk.AutoCAD.DatabaseServices.DBObject obj;

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

        [CommandMethod("PGA-WeedPolylines")]
        public static void MapClean()
        {
            try
            {
                C3DSurfacesDemo.MapClean.ProcessLineWork();
            }
            catch (Exception ex)
            {
                ACADLogging.LogMyExceptions("MapClean" + ex.Message);
            }
        }
    }
}