#region

using System;
using System.Collections.Generic;
using global::Autodesk.AutoCAD.DatabaseServices;

#endregion

//using Common.Logging;

namespace BBC.Common.AutoCAD
{
    /// <summary>
    ///     Contains helper methods for interacting with AutoCAD layouts
    /// </summary>
    public static class Layout_Manager
    {
        #region Private Members

        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(Layout_Manager));

        #endregion

        #region SetCurrentLayout - Uses LayoutManager

        /// <summary>
        ///     Sets the current layout.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns></returns>
        public static bool SetCurrentLayout(Database db, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetCurrentLayout");
            var retval = false;

            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layoutName))
                throw new ArgumentNullException("layoutName", "The argument 'layoutName' was null or empty.");

            using (var trans = db.TransactionManager.StartTransaction())
            {
                SetCurrentLayout(db, trans, layoutName);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End SetCurrentLayout");
            return retval;
        }

        /// <summary>
        ///     Sets the current layout.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns></returns>
        public static bool SetCurrentLayout(Database db, Transaction trans, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetCurrentLayout");
            var retval = false;

            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layoutName))
                throw new ArgumentNullException("layoutName", "The argument 'layoutName' was null or empty.");
            try
            {
                //TODO: Change this to not use the LayoutManager
                LayoutManager.Current.CurrentLayout = layoutName;
                retval = true;
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in SetCurrentLayout", ex);
                throw;
            }
            PGA.MessengerManager.MessengerManager.AddLog("End SetCurrentLayout");
            return retval;
        }

        #endregion

        #region DeleteLayout - Uses LayoutManager

        /// <summary>
        ///     Deletes the layout if defined.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layoutName">The layoutName.</param>
        public static void DeleteLayout(Database db, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayout");
            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layoutName))
                throw new ArgumentNullException("layoutName", "The argument 'layoutName' was null or empty.");

            using (var trans = db.TransactionManager.StartTransaction())
            {
                DeleteLayout(db, trans, layoutName);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayout");
        }

        /// <summary>
        ///     Deletes the layout if defined.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layoutName">The layoutName.</param>
        /// <returns></returns>
        public static bool DeleteLayout(Database db, Transaction trans, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayout");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            if (string.IsNullOrEmpty(layoutName))
                throw new ArgumentNullException("layoutName", "The argument 'layoutName' was null or empty.");

            var erased = false;
            try
            {
                //TODO: Change this to not use the LayoutManager
                var layoutId = GetLayoutId(db, trans, layoutName);
                if (layoutId != ObjectId.Null)
                {
                    LayoutManager.Current.DeleteLayout(layoutName);
                    erased = true;
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in DeleteLayout", ex);
                throw;
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayout");
            return erased;
        }

        #endregion

        #region GetLayoutId

        /// <summary>
        ///     Gets the layout id.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layoutName">The layoutName.</param>
        /// <returns>The object id the layout if it exists, otherwise ObjectId.Null</returns>
        public static ObjectId GetLayoutId(Database db, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayoutId");
            var layoutId = ObjectId.Null;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                layoutId = GetLayoutId(db, trans, layoutName);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayoutId");
            return layoutId;
        }

        /// <summary>
        ///     Gets the layout id.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layoutName">The layoutName.</param>
        /// <returns>The object id the layout if it exists, otherwise ObjectId.Null</returns>
        public static ObjectId GetLayoutId(Database db, Transaction trans, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayoutId");
            var layoutId = ObjectId.Null;
            layoutName = layoutName.ToUpper();
            using (var layoutDictionary = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary)
            {
                Layout oneLayout = null;
                foreach (var dictEntry in layoutDictionary)
                {
                    var oneLayoutName = dictEntry.Key;
                    oneLayoutName = oneLayoutName.ToUpper();
                    if (oneLayoutName == layoutName)
                    {
                        layoutId = dictEntry.Value;
                        oneLayout = trans.GetObject(layoutId, OpenMode.ForRead) as Layout;
                        if (oneLayout != null)
                        {
                            break;
                        }
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayoutId");
            return layoutId;
        }

        #endregion

        #region IsLayoutDefined

        /// <summary>
        ///     Determines whether [is layout defined] [the specified db].
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns>
        ///     <c>true</c> if [is layout defined] [the specified db]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLayoutDefined(Database db, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLayoutDefined");
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = IsLayoutDefined(db, trans, layoutName);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLayoutDefined");
            return retval;
        }

        /// <summary>
        ///     Determines whether [is layout defined] [the specified db].
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns>
        ///     <c>true</c> if [is layout defined] [the specified db]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLayoutDefined(Database db, Transaction trans, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLayoutDefined");
            var retval = false;
            layoutName = layoutName.ToUpper();

            using (
                var layoutDictionary = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary)
            {
                foreach (var dictEntry in layoutDictionary)
                {
                    var oneLayoutName = dictEntry.Key;
                    oneLayoutName = oneLayoutName.ToUpper();
                    if (oneLayoutName == layoutName)
                    {
                        retval = true;
                        break;
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLayoutDefined");
            return retval;
        }

        #endregion

        #region GetLayoutNames

        /// <summary>
        ///     Gets a list of layout names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <returns></returns>
        public static IList<string> GetLayoutNames(Database db)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayoutNames");
            IList<string> layoutNames = new List<string>();
            using (var trans = db.TransactionManager.StartTransaction())
            {
                layoutNames = GetLayoutNames(db, trans);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayoutNames");
            return layoutNames;
        }

        /// <summary>
        ///     Gets a list of layout names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of layout names</returns>
        public static IList<string> GetLayoutNames(Database db, Transaction trans)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayoutNames");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            IList<string> layoutNames = new List<string>();
            using (
                var layoutDictionary = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary)
            {
                foreach (var dictEntry in layoutDictionary)
                {
                    var layoutName = dictEntry.Key;
                    layoutNames.Add(layoutName);
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayoutNames");
            return layoutNames;
        }

        /// <summary>
        ///     Gets a list of layout names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>A list of layout names</returns>
        public static IList<string> GetLayoutNames(Database db, Transaction trans, string filter)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayoutNames");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            IList<string> layoutNames = new List<string>();
            using (
                var layoutDictionary = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary)
            {
                foreach (var dictEntry in layoutDictionary)
                {
                    var layoutName = dictEntry.Key;
                    if (layoutName.ToUpper().StartsWith(filter.ToUpper().Replace("*", string.Empty)))
                    {
                        //Adding LayoutName to List";
                        layoutNames.Add(layoutName);
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayoutNames");
            return layoutNames;
        }

        #endregion

        #region WBlockCloneLayout

        /// <summary>
        ///     Copies the named layout from the source drawing file into the db
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="targetLayoutName">Name of the target layout.</param>
        /// <param name="sourceLayoutName">Name of the source layout.</param>
        /// <param name="sourceLayoutDrawingName">Name of the source layout drawing.</param>
        /// <returns></returns>
        public static bool WBlockCloneLayout(Database db, string targetLayoutName, string sourceLayoutName,
            string sourceLayoutDrawingName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start WBlockCloneLayout");
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = WBlockCloneLayout(db, trans, targetLayoutName, sourceLayoutName, sourceLayoutDrawingName);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End WBlockCloneLayout");
            return retval;
        }

        /// <summary>
        ///     Copies the named layout from the source drawing file into the db
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="targetLayoutName">Name of the target layout.</param>
        /// <param name="sourceLayoutName">Name of the source layout.</param>
        /// <param name="sourceLayoutDrawingName">Name of the source layout drawing.</param>
        /// <returns></returns>
        public static bool WBlockCloneLayout(Database db, Transaction trans, string targetLayoutName,
            string sourceLayoutName, string sourceLayoutDrawingName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start WBlockCloneLayout");
            var retval = false;

            //Verify layout is not already defined in target database (can't copy in a duplicate name)
            if (IsLayoutDefined(db, trans, targetLayoutName))
            {
                return false;
            }

            // Load the source database as side database
            using (var sourceDb = AcadUtilities.LoadSideDatabase(sourceLayoutDrawingName, true))
                //if (sourceDb != null)
            {
                var sourceObjectIdCollection = new ObjectIdCollection();

                // Start transaction on source database
                using (var wblockTrans = sourceDb.TransactionManager.StartTransaction())
                {
                    // get the layout object from source db
                    Layout sourceLayout = null;
                    var layoutId = GetLayoutId(sourceDb, wblockTrans, sourceLayoutName);
                    if (layoutId != null)
                    {
                        sourceLayout = wblockTrans.GetObject(layoutId, OpenMode.ForRead) as Layout;

                        // Get id collection of all objects on layout's block table record
                        using (
                            var sourceLayoutBtr =
                                wblockTrans.GetObject(sourceLayout.BlockTableRecordId, OpenMode.ForRead) as
                                    BlockTableRecord)
                        {
                            foreach (var oid in sourceLayoutBtr)
                            {
                                sourceObjectIdCollection.Add(oid);
                            }
                        }

                        // Get the block table from the target database
                        var targetBt = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;

                        // Derive the paper space name for the next layout
                        var layoutPaperSpacePrefix = "*Paper_Space";
                        var layoutCount = 1;
                        while (targetBt.Has(layoutPaperSpacePrefix + layoutCount))
                        {
                            layoutCount++;
                        }
                        var nextPaperSpaceLayoutName = layoutPaperSpacePrefix + layoutCount;

                        // Create a new layout as a copy of the source layout
                        var targetLayout = new Layout();
                        targetLayout.LayoutName = targetLayoutName;
                        targetLayout.CopyFrom(sourceLayout);

                        // Add a new block table record to the target block table with the next paper space name
                        var targetBtr = new BlockTableRecord();
                        targetBtr.Name = nextPaperSpaceLayoutName;

                        // Note that this does not work as expected - 
                        // the name is redefined at .Add. No error is thrown and the layout name sorts itself out on save / close / open?
                        targetBt.Add(targetBtr);
                        trans.AddNewlyCreatedDBObject(targetBtr, true);

                        // Wblock Clone all the objects found in the source layout to the target database
                        sourceDb.WblockCloneObjects(sourceObjectIdCollection, targetBtr.ObjectId, new IdMapping(),
                            DuplicateRecordCloning.Ignore, false);

                        // Add the layout to the target database layout dictionary
                        targetLayout.AddToLayoutDictionary(db, targetBtr.ObjectId);

                        // Add the layout to the target database
                        trans.AddNewlyCreatedDBObject(targetLayout, true);

                        retval = true;
                    }
                }
                sourceDb.Dispose();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End WBlockCloneLayout");
            return retval;
        }

        #endregion

        #region DeleteViewportsFromLayout

        /// <summary>
        ///     Deletes the viewports from layout.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns></returns>
        public static bool DeleteViewportsFromLayout(Database db, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteViewportsFromLayout");
            var retval = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retval = DeleteViewportsFromLayout(db, trans, layoutName);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteViewportsFromLayout");
            return retval;
        }

        /// <summary>
        ///     Deletes the viewports from layout.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns></returns>
        public static bool DeleteViewportsFromLayout(Database db, Transaction trans, string layoutName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteViewportsFromLayout");
            var retval = false;

            layoutName = layoutName.ToUpper();
            if (IsLayoutDefined(db, trans, layoutName))
            {
                var layoutId = GetLayoutId(db, trans, layoutName);
                Layout oneLayout = null;
                oneLayout = trans.GetObject(layoutId, OpenMode.ForRead) as Layout;
                if (oneLayout != null)
                {
                    using (
                        var layoutBtr =
                            trans.GetObject(oneLayout.BlockTableRecordId, OpenMode.ForWrite, false) as BlockTableRecord)
                    {
                        var oids = new ObjectIdCollection();
                        var isFirstViewport = true;
                        foreach (var oid in layoutBtr)
                        {
                            if (oid.ObjectClass.Name == "AcDbViewport")
                            {
                                if (isFirstViewport)
                                {
                                    isFirstViewport = false;
                                }
                                else
                                {
                                    oids.Add(oid);
                                }
                            }
                        }
                        if (oids.Count > 0)
                        {
                            AcadUtilities.DeleteObjects(db, trans, oids);
                        }
                        retval = true;
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteViewportsFromLayout");
            return retval;
        }

        #endregion

        #region CreateViewportFromPointList

        /// <summary>
        /// Creates a new paper space viewport using the point list to define a clip entity
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="pointList">The point list.</param>
        /// <param name="viewportObjectId">The viewport object id.</param>
        /// <returns></returns>
        //public static bool CreateViewportFromPointList(Database db, string layoutName, string layerName, IList<GemsPoint2d> pointList, ref ObjectId viewportObjectId)
        //{
        //    PGA.MessengerManager.MessengerManager.AddLog("Start CreateViewportFromPointList");
        //    bool retval = false;
        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        retval = CreateViewportFromPointList(db, trans, layoutName, layerName, pointList, ref viewportObjectId);
        //        trans.Commit();
        //    }
        //    PGA.MessengerManager.MessengerManager.AddLog("End CreateViewportFromPointList");
        //    return retval;
        //}

        /// <summary>
        /// Creates a new paper space viewport using the point list to define a clip entity
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layoutName">Name of the layout.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="pointList">The point list.</param>
        /// <param name="viewportObjectId">The viewport object id.</param>
        /// <returns></returns>
        //public static bool CreateViewportFromPointList(Database db, Transaction trans, string layoutName, string layerName, IList<GemsPoint2d> pointList, ref ObjectId viewportObjectId)
        //{
        //    PGA.MessengerManager.MessengerManager.AddLog("Start CreateViewportFromPointList");
        //    bool retval = false;

        //    layoutName = layoutName.ToUpper();
        //    if(IsLayoutDefined(db, trans, layoutName) == true)
        //    {
        //        ObjectId layoutId = GetLayoutId(db, trans, layoutName);
        //        Layout oneLayout = null;
        //        oneLayout = trans.GetObject(layoutId, OpenMode.ForRead) as Layout;
        //        if (oneLayout != null)
        //        {
        //            using (BlockTableRecord layoutBtr = trans.GetObject(oneLayout.BlockTableRecordId, OpenMode.ForWrite, false) as BlockTableRecord)
        //            {
        //                // Create target layer
        //                LayerManager.CreateLayer(db, trans, layerName);

        //                int pointIndex = 0;
        //                int pointCount = pointList.Count;
        //                Polyline plineObject = new Polyline(pointCount);
        //                foreach (GemsPoint2d onePoint in pointList)
        //                {
        //                    plineObject.AddVertexAt(pointIndex, new Point2d(onePoint.X, onePoint.Y), 0, 0, 0);
        //                    pointIndex++;
        //                }
        //                plineObject.Closed = true;
        //                plineObject.Layer = layerName;

        //                Entity ent = plineObject as Entity;
        //                if (ent != null)
        //                {
        //                    // Create clipping pline
        //                    ObjectId plineObjectId = layoutBtr.AppendEntity(plineObject); // layoutBtr.AppendEntity(plineObject);
        //                    trans.AddNewlyCreatedDBObject(plineObject, true);

        //                    // Create viewport
        //                    Viewport vp = new Viewport();
        //                    layoutBtr.AppendEntity(vp);
        //                    trans.AddNewlyCreatedDBObject(vp, true);

        //                    // Set the vp boundary entity and turn the viewport clipping on
        //                    vp.NonRectClipEntityId = plineObjectId;
        //                    vp.NonRectClipOn = true;
        //                    vp.Layer = layerName;
        //                    //vp.On = true;

        //                    viewportObjectId = vp.ObjectId;
        //                    retval = true;
        //                }
        //            }
        //        }
        //    }
        //    PGA.MessengerManager.MessengerManager.AddLog("End CreateViewportFromPointList");
        //    return retval;
        //}

        /// <summary>
        /// Sets the viewport view.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="vpOid">The vp oid.</param>
        /// <param name="pointList">The point list.</param>
        /// <returns></returns>
        //public static bool SetViewportView(Database db, Transaction trans, ObjectId vpOid, IList<GemsPoint2d> pointList)
        //{
        //    PGA.MessengerManager.MessengerManager.AddLog("Start SetViewportView");
        //    using (Viewport vp = trans.GetObject(vpOid, OpenMode.ForWrite, false) as Viewport)
        //    {
        //        GemsPoint2d centerPoint = CoordinateService.GetCentroidFromPolygon(pointList);
        //        GemsPoint2d minPoint = CoordinateService.GetMinFromPolygon(pointList);
        //        GemsPoint2d maxPoint = CoordinateService.GetMaxFromPolygon(pointList);
        //        double width = maxPoint.X - minPoint.X;
        //        double height = maxPoint.Y - minPoint.Y;
        //        vp.ViewCenter = new Point2d(centerPoint.X, centerPoint.Y);
        //        vp.ViewHeight = height;
        //    }
        //    PGA.MessengerManager.MessengerManager.AddLog("End SetViewportView");
        //    return true;
        //}

        #endregion
    }
}