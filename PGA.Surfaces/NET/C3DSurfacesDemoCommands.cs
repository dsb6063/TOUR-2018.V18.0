// ***********************************************************************
// Assembly         : PGA.Surfaces
// Author           : Daryl Banks, PSM
// Created          : 03-15-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 03-15-2016
// ***********************************************************************
// <copyright file="C3DSurfacesDemoCommands.cs" company="Banks & Banks Consulting">
//     Copyright (c) Banks & Banks Consulting. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.Runtime;
using global::Autodesk.AutoCAD.Windows.Data;
using global::Autodesk.Civil.ApplicationServices;
using global::Autodesk.Civil.DatabaseServices;
using C3DSurfacesDemo;
using PGA.Database;
using PGA.DataContext;
using PGA.OffsetPolylines;
using PGA.Surfaces;
using Application = global::Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = System.Exception;
using ObjectId = global::Autodesk.AutoCAD.DatabaseServices.ObjectId;
using ObjectIdCollection = global::Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;
using Polyline = global::Autodesk.AutoCAD.DatabaseServices.Polyline;

[assembly: CommandClass(typeof (C3DSurfacesDemoCommands))                                ]

namespace PGA.Surfaces
{
    /// <summary>
    /// Class C3DSurfacesDemoCommands.
    /// </summary>

    #region New Snippet Insert



    public class C3DSurfacesDemoCommands
        {

            public static ObjectIdCollection GetIdsByTypeTypeValue(params string[] types)
            {

                // Get the document
                var doc = Application.DocumentManager.MdiActiveDocument;

                // Get the editor to make the selection
                Editor oEd = doc.Editor;

                // Add our or operators so we can grab multiple types.
                IList<TypedValue> typedValueSelection = new List<TypedValue>
                {
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>")
                };

                // We will need to insert our requested types into the collection.
                // Since we knew we would have to insert they types inbetween the operators..
                // I used a Enumerable type which gave me that functionallity. (IListf<T>)
                foreach (var type in types)
                    typedValueSelection.Insert(1, new TypedValue(Convert.ToInt32(DxfCode.Start), type));

                SelectionFilter selectionFilter = new SelectionFilter(typedValueSelection.ToArray());

                // because we have to.. Not really sure why, I assume this is our only access point
                // to grab the entities that we want. (I am open to being corrected)
                PromptSelectionResult promptSelectionResult = oEd.SelectAll(selectionFilter);

                // return our new ObjectIdCollection that is "Hopefully" full of the types that we want.
                return new ObjectIdCollection(promptSelectionResult.Value.GetObjectIds());
            }
        //    /// <summary>
        //    /// Gets the object identifier.
        //    /// </summary>
        //    /// <param name="handle">The handle.</param>
        //    /// <returns>ObjectId.</returns>
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

        //    /// <summary>
        //    /// Determines whether [is tin excluded] [the specified objectid].
        //    /// </summary>
        //    /// <param name="objectid">The objectid.</param>
        //    /// <returns><c>true</c> if [is tin excluded] [the specified objectid]; otherwise, <c>false</c>.</returns>
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
        public void CreateAllSurfaces()
            {

                try
                {
                    var polylines      = new ObjectIdCollection();
                    var surfaceManager = new PasteSurfaces();
                    var m_dict = new Dictionary<ObjectId, ObjectIdCollection>();
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


                    #endregion

                    #region get polylines and generate surfaces and boundaries

                        //polylines = surfaceManager.GetAllPolyLines();
                     polylines = GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3D");
                    
                    //polylines  = surfaceManager.GetIdsByTypeCollection();

                    //Create a surface hive to generate a TIN surface
                    if (!surfaceManager.CreateSurfaceForPolylines(polylines))
                            throw new SystemException("Create surface for polylines failed!");
                        if (!surfaceManager.PasteAllSurfaces(CivilTinSurface.GetCivilSurfaceBySurfaceName("All")))
                            throw new SystemException("Pasting Surfaces failed!");
                        if (!surfaceManager.AddBoundariesForSurfaces(polylines))
                            throw new SystemException("Add overall Boundaries for surfaces failed!");

                    #endregion
                    PGA.MessengerManager.MessengerManager.AddLog 
                       ("Store boundaries to Object Dictionary");
                        ObjectIdCollection internalPLines = null;
                        foreach (ObjectId baseObj in polylines)
                        {
                            #region store the internal boundaries for the selected base object in dict
                            if (GetPolyFromObjId(baseObj) == null)
                                continue;

                        if (IsTINExcluded(baseObj))
                            continue;

                        internalPLines = surfaceManager.GetAllInternalPolyLinesToSelected(baseObj, polylines);
            

                            m_dict.Add(baseObj, internalPLines);


                            #endregion
                        }

                        #region Iterate through inner boundaries
                                            PGA.MessengerManager.MessengerManager.AddLog ("Start Iterate through inner boundaries");
                        CivilTinSurface lsurface = null;
                        foreach (KeyValuePair<ObjectId, ObjectIdCollection> innerbdy in m_dict)
                        {

                            if (innerbdy.Value == null) continue;

                            ObjectId outerPline = innerbdy.Key;
                            ObjectIdCollection innerIdCollection = innerbdy.Value;


                            lsurface = PasteSurfaces.FindSurfaceIdForPolylineV2(outerPline);

                            if (lsurface == null)
                                continue;

                        lsurface.SetDefaultBuildOptions();

                        #region Interate Internal Polylines to add breaklines and boundaries
                        PGA.MessengerManager.MessengerManager.AddLog ("Start AddStandardInnerBoundary");
                            if (innerIdCollection != null)
                                foreach (ObjectId pLines in innerIdCollection)
                                {
                                    if (pLines == null) continue;
                                    try
                                    {
                                        var layer =  PGA.SurfaceManager.SurfaceManager.GetPolylineLayer(pLines);

                                        lsurface.AddStandardInnerBoundary(pLines, "Boundary-" + layer + DateTime.Now.Millisecond);
                                    }
                                    catch (NullReferenceException e)
                                    {
                                         PGA.MessengerManager.MessengerManager.AddLog ("AddAsInnerBoundary Failed: " + e.Message);
                                    }
                                    catch (Exception e)
                                    {
                                         PGA.MessengerManager.MessengerManager.AddLog ("AddAsInnerBoundary Failed: " + e.Message);
                                    }
                                    internalPLines = null;
                                }
                                PGA.MessengerManager.MessengerManager.AddLog ("End AddStandardInnerBoundary");

                            #endregion
                            #region Rebuild Surfaces
                                                PGA.MessengerManager.MessengerManager.AddLog ("Start RebuildSurfaceBySurfaceName");
                            if (lsurface != null)
                                CivilTinSurface.RebuildSurfaceBySurfaceName(lsurface.Name);
                            #endregion
                        }

                        #endregion


                        internalPLines = null;
                    PGA.MessengerManager.MessengerManager.AddLog
                      ("End Iterate through inner boundaries");
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
                PGA.MessengerManager.MessengerManager.LogException(e);
                }
                catch (global::Autodesk.AutoCAD.Runtime.Exception e)
                {
                PGA.MessengerManager.MessengerManager.LogException(e);
                }
                catch (Exception e)
                {
                PGA.MessengerManager.MessengerManager.LogException(e);
                }
            }

         
            [CommandMethod("PGA-CreateAllSurfaces", CommandFlags.UsePickSet)]
            public void CreateSingleSurface()
            {
                try
                {
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

                        //Get Selected Polylines
                        Editor ed = Active.Editor;

                        try

                        {

                            PromptSelectionResult selectionRes =

                                ed.SelectImplied();

                            // If there's no pickfirst set available...

                            if (selectionRes.Status == PromptStatus.Error)

                            {

                                // ... ask the user to select entities

                                PromptSelectionOptions selectionOpts =

                                    new PromptSelectionOptions();

                                selectionOpts.MessageForAdding =

                                    "\nSelect polylines to create a surface: ";

                                selectionRes =

                                    ed.GetSelection(selectionOpts);

                            }

                            else

                            {

                                // If there was a pickfirst set, clear it

                                ed.SetImpliedSelection(new ObjectId[0]);

                            }

                            // If the user has not cancelled...

                            if (selectionRes.Status == PromptStatus.OK)

                            {
                                var selectedObjects = selectionRes.Value.GetObjectIds();
                                var selectedpolyline = new ObjectIdCollection(selectedObjects);
                                var polylines = surfaceManager.GetIdsByTypeCollection();

                            if (!surfaceManager.CreateSurfaceForPolylines(selectedpolyline))
                                throw new SystemException("Create surface for polylines failed!");
                            if (!surfaceManager.PasteAllSurfaces(CivilTinSurface.GetCivilSurfaceBySurfaceName("All")))
                                    throw new SystemException("Pasting Surfaces failed!");                     
                            if (!surfaceManager.AddBoundariesForSurfaces(selectedpolyline))
                                    throw new SystemException("Add overall Boundaries for surfaces failed!");

                                #endregion

                                ObjectIdCollection internalPLines = null;

                                foreach (ObjectId selected in selectedpolyline)
                                {

                                //Create a surface hive to generate a TIN surface
                                //Disabled min poly separation distance in backend 2/3/2018

                                #region store the internal boundaries for the selected base object in dict

                                if (GetPolyFromObjId(selected) == null)
                                            continue;

                                        internalPLines = surfaceManager.GetAllInternalPolyLinesToSelected(selected,polylines);

                                        m_dict.Add(selected, internalPLines);


                                        #endregion
                                }

                            #region Iterate through inner boundaries

                            CivilTinSurface lsurface = null;

                            foreach (KeyValuePair<ObjectId, ObjectIdCollection> boundary in m_dict)
                                {
                                #region Removed

                                //#region Create Surface for that Base Polyline and Add outer boundary

                                //if ((lsurface = (surfaceManager.CreateSelectedSurface(baseObj))) == null)
                                //    throw new Exception("CreateSelectedSurface Failed!");

                                //lsurface.AddStandardBoundary(baseObj, "OuterBoundary");
                                //#endregion

                                #endregion

                                lsurface = PasteSurfaces.FindSurfaceIdForPolylineV2(boundary.Key);

                                if (boundary.Value == null) continue;


                                #region Interate Internal Polylines to add breaklines and boundaries

                                    if (boundary.Value != null)
                                        foreach (ObjectId innnerId in boundary.Value)
                                        {
                                            try
                                            {
                                                #region Breaklines Deprecated

                                                //Breaklines removed due to overlapping
                                                //lsurface.AddStandardBreakline
                                                //    (PasteSurfaces.GetPolyPointsByObjectId(pLines), "Breakline-"); 

                                                #endregion

                                                var currentPoly  = GetPolyFromObjId(innnerId);
                                                var selectedPoly = GetPolyFromObjId(boundary.Key);
                                                if (currentPoly != null && currentPoly.Layer == selectedPoly.Layer)
                                                {

                                                if (lsurface != null)
                                                        lsurface.AddStandardInnerBoundary(innnerId, "Boundary-" + currentPoly.Layer);

                                                }

                                            }
                                        

                                        
                                            catch (NullReferenceException e)
                                            {
                                                PGA.MessengerManager.MessengerManager.AddLog(
                                                    "AddAsInnerBoundary Failed: " +
                                                    e.Message);
                                            }
                                            catch (Exception e)
                                            {
                                                PGA.MessengerManager.MessengerManager.AddLog(
                                                    "AddAsInnerBoundary Failed: " +
                                                    e.Message);
                                            }
                                        }
                                    internalPLines = null;

                                    #endregion

                                }

                            #region Rebuild Surfaces

                            if (lsurface != null)
                                CivilTinSurface.RebuildSurfaceBySurfaceName(lsurface.Name);

                            #endregion

                            ed.Regen();
                            #endregion


                            internalPLines = null;
                            }
                        }
                        catch (System.Exception ex)
                        {
                        PGA.MessengerManager.MessengerManager.LogException(ex);
                            return;

                        }
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
                PGA.MessengerManager.MessengerManager.LogException(e);
                }
                catch (global::Autodesk.AutoCAD.Runtime.Exception e)
                {
                PGA.MessengerManager.MessengerManager.LogException(e);
                }
                catch (Exception e)
                {
                PGA.MessengerManager.MessengerManager.LogException(e);
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

                            internalPLines = surfaceManager.GetAllInternalPolyLinesToSelected(baseObj, polylines);

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
                                    var layer = PGA.SurfaceManager.SurfaceManager.GetPolylineLayer(pLines);

                                    lsurface.AddStandardInnerBoundary(pLines, "Boundary-" + layer);
                                    }
                                    catch (NullReferenceException e)
                                    {
                                                            PGA.MessengerManager.MessengerManager.AddLog ("AddAsInnerBoundary Failed: " + e.Message);
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
                PGA.MessengerManager.MessengerManager.LogException(e);
                }
                catch (global::Autodesk.AutoCAD.Runtime.Exception e)
                {
                PGA.MessengerManager.MessengerManager.LogException(e);
                }
                catch (Exception e)
                {
                PGA.MessengerManager.MessengerManager.LogException(e);
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
            global::Autodesk.AutoCAD.DatabaseServices.Database db = Application.DocumentManager.MdiActiveDocument.Database;
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

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
                PGA.MessengerManager.MessengerManager.LogException(e);
                }

            }
 
            public static global::Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection CloseSelectedPolylines()
            {

            global::Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection oids;
                oids = new global::Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();

                SelectionSet selection;
                selection = BBC.Common.AutoCAD.SelectionManager.GetSelectionSet("");

                foreach (ObjectId obj in selection.GetObjectIds())
                {
                    oids.Add(obj);
                }
                return oids;
            }
            public Polyline GetPolyFromObjId(global::Autodesk.AutoCAD.DatabaseServices.ObjectId oid)
            {
                using (global::Autodesk.AutoCAD.DatabaseServices.Database db = CivilApplicationManager.WorkingDatabase)
                {

                    try
                    {
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                        global::Autodesk.AutoCAD.DatabaseServices.DBObject obj;

                            try
                            {
                                obj = tr.GetObject(oid, OpenMode.ForRead);
                            }
                            catch (NullReferenceException e)
                            {
                            PGA.MessengerManager.MessengerManager.LogException(e);
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
                    catch (global::Autodesk.AutoCAD.Runtime.Exception e)
                    {
                    PGA.MessengerManager.MessengerManager.LogException(e);
                }
            }

                return null;
            }

 
        }
    
    #endregion
}