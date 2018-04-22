namespace AssignPolylineLayers
{
    using Autodesk.AutoCAD.ApplicationServices;
    using global::Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.Geometry;
    using Autodesk.AutoCAD.Runtime;
    using Autodesk.AutoCAD.EditorInput;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.AutoCAD.Colors;

    public class Layers
    {
  
        private static bool IsLayerDefined(Database db, string layername)
        {
            try
            {
                if( String.IsNullOrEmpty(layername))
                {
                    return false;
                }
                if (LayerManager.IsDefined(db, layername))
                    return LayerManager.IsDefined(db, layername);
            }
            catch (System.Exception )
            {
            }
            return false;
        }

        //private static void InteratePolyLines(ObjectId selectedObjectId)
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Editor ed = doc.Editor;
        //    Database db = doc.Database;
        //    Transaction tr = db.TransactionManager.StartTransaction();

        //    // Get the current UCS

        //    //CoordinateSystem3d ucs =
        //    //    ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

        //    using (tr)
        //    {
        //        DBObject obj =
        //            tr.GetObject(selectedObjectId, OpenMode.ForRead);


        //        // If a "lightweight" (or optimized) polyline

        //        Polyline lwp = obj as Polyline;

        //        if (lwp != null)
        //        {
        //            // Is Polyline Closed
        //            if (lwp.Closed)
        //            {
        //                lwp.UpgradeOpen();
        //                if (IsLayerDefined(db, AssignPolyLinesToLayers(lwp).ToUpper()))
        //                    lwp.Layer = AssignPolyLinesToLayers(lwp).ToUpper() ?? lwp.Layer;
        //                lwp.DowngradeOpen();
        //            }
        //            // Use a for loop to get each vertex, one by one

        //            int vn = lwp.NumberOfVertices;

        //            for (int i = 0; i < vn; i++)
        //            {
        //                // Could also get the 3D point here

        //                Point2d pt = lwp.GetPoint2dAt(i);

        //                //ed.WriteMessage("\n" + pt.ToString());
        //            }
        //        }

        //        else
        //        {
        //            // If an old-style, 2D polyline

        //            Polyline2d p2d = obj as Polyline2d;

        //            if (p2d != null)
        //            {
        //                p2d.UpgradeOpen();
        //                if (IsLayerDefined(db, AssignPolyLinesToLayers(p2d)))
        //                    p2d.Layer = AssignPolyLinesToLayers(p2d) ?? p2d.Layer;
        //                p2d.DowngradeOpen();
        //                // Use foreach to get each contained vertex

        //                foreach (ObjectId vId in p2d)
        //                {
        //                    Vertex2d v2d =
        //                        (Vertex2d)tr.GetObject(
        //                            vId,
        //                            OpenMode.ForRead
        //                            );

        //                    //ed.WriteMessage(
        //                    //    "\n" + v2d.Position.ToString()
        //                    //    );
        //                }
        //            }

        //            else
        //            {
        //                // If an old-style, 3D polyline

        //                Polyline3d p3d = obj as Polyline3d;

        //                if (p3d != null)
        //                {
        //                    // Use foreach to get each contained vertex

        //                    foreach (ObjectId vId in p3d)
        //                    {
        //                        PolylineVertex3d v3d =
        //                            (PolylineVertex3d)tr.GetObject(
        //                                vId,
        //                                OpenMode.ForRead
        //                                );

        //                        //ed.WriteMessage(
        //                        //    "\n" + v3d.Position.ToString()
        //                        //    );
        //                    }
        //                }
        //            }
        //        }

        //        // Committing is cheaper than aborting

        //        tr.Commit();
        //    }
        //}

        private static string AssignPolyLinesToLayers(Polyline2d p2d)
        {
            //Get the 3-digit layer name of the object and compare

            try
            {
                string name = p2d.Layer.ToUpper();

                switch (name)
                {
                    case "OBR":
                        return "S-Bridge".ToUpper();

                    case "OBD":
                        return "S-Building".ToUpper();

                    case "OST":
                        return "S-Bunker".ToUpper();

                    case "OBO":
                        return "S-Bush".ToUpper();

                    case "OCA":
                        return "S-CartPath".ToUpper();

                    case "OCO":
                        return "S-Collar".ToUpper();

                    case "ODO":
                        return "S-Dirt-Outline".ToUpper();

                    case "OFW":
                        return "S-Fairway".ToUpper();

                    case "OGR":
                        return "S-Green".ToUpper();

                    case "OGS":
                        return "S-GreenSide-Bunker".ToUpper();

                    case "OIR":
                        return "S-Intermediate-Rough".ToUpper();

                    case "OLN":
                        return "S-LandScaping".ToUpper();

                    case "ONA":
                        return "S-Native-Area".ToUpper();

                    case "ORK":
                        return "S-Rock-Outline".ToUpper();

                    case "OPT":
                        return "S-Path".ToUpper();

                    case "ORO":
                        return "S-Rough-Outlines".ToUpper();

                    case "OSS":
                        return "S-Steps".ToUpper();

                    case "OTB":
                        return "S-Tee-Box".ToUpper();

                    case "OTO":
                        return "S-Tree-Outline".ToUpper();

                    case "OWS":
                        return "S-Walk-Strip".ToUpper();

                    case "OWL":
                        return "S-Wall".ToUpper();

                    case "OWA":
                        return "S-Water".ToUpper();

                    case "OWD":
                        return "S-Water-Drop".ToUpper();
                    case "OTH":
                        return "S-Other".ToUpper();
                    default:
                        return name;
                }
            }
            catch (System.Exception )
            {
            }
            return null;
        }

        private static void InteratePolyLines(ObjectId selectedObjectId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            // Get the current UCS

            CoordinateSystem3d ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (Transaction tr = db.TransactionManager.StartTransaction())

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
                        string layername = AssignPolyLinesToLayers(lwp);
                        //Common.Logging.ACADLogging.LogMyExceptions(lwp.Layer);
                        lwp.UpgradeOpen();
                        if (IsLayerDefined(db,layername))
                            lwp.Layer = layername ?? lwp.Layer;
                        lwp.DowngradeOpen();
                        //Common.Logging.ACADLogging.LogMyExceptions(lwp.Layer);
                    }
                    // Use a for loop to get each vertex, one by one

                    int vn = lwp.NumberOfVertices;

                    for (int i = 0; i < vn; i++)
                    {
                        // Could also get the 3D point here

                        Point2d pt = lwp.GetPoint2dAt(i);

                        //ed.WriteMessage("\n" + pt.ToString());
                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    Polyline2d p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        // Use foreach to get each contained vertex
                        // Is Polyline Closed
                        if (p2d.Closed)
                        {
                            string layername = AssignPolyLinesToLayers(p2d);
                            //Common.Logging.ACADLogging.LogMyExceptions(lwp.Layer);
                            p2d.UpgradeOpen();
                            if (IsLayerDefined(db, layername))
                                p2d.Layer = layername ?? p2d.Layer;
                            p2d.DowngradeOpen();
                            //Common.Logging.ACADLogging.LogMyExceptions(lwp.Layer);
                        }


                        //foreach (ObjectId vId in p2d)
                        //{
                        //    Vertex2d v2d =
                        //        (Vertex2d)tr.GetObject(
                        //            vId,
                        //            OpenMode.ForRead
                        //            );

                            //ed.WriteMessage(
                            //    "\n" + v2d.Position.ToString()
                            //    );
                        //}
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
                                    (PolylineVertex3d)tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                        );

                                //ed.WriteMessage(
                                //    "\n" + v3d.Position.ToString()
                                //    );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
        }

        private static string AssignPolyLinesToLayers(Polyline lwp)
        {
            //Get the 3-digit layer name of the object and compare

            try
            {
                string name = lwp.Layer.ToUpper();

                switch (name)
                {
                    case "OBR":
                        return "S-Bridge".ToUpper();

                    case "OBD":
                        return "S-Building".ToUpper();

                    case "OST":
                        return "S-Bunker".ToUpper();

                    case "OBO":
                        return "S-Bush".ToUpper();

                    case "OCA":
                        return "S-CartPath".ToUpper();

                    case "OCO":
                        return "S-Collar".ToUpper();

                    case "ODO":
                        return "S-Dirt-Outline".ToUpper();

                    case "OFW":
                        return "S-Fairway".ToUpper();

                    case "OGR":
                        return "S-Green".ToUpper();

                    case "OGS":
                        return "S-GreenSide-Bunker".ToUpper();

                    case "OIR":
                        return "S-Intermediate-Rough".ToUpper();

                    case "OLN":
                        return "S-LandScaping".ToUpper();

                    case "ONA":
                        return "S-Native-Area".ToUpper();

                    case "ORK":
                        return "S-Rock-Outline".ToUpper();

                    case "OPT":
                        return "S-Path".ToUpper();

                    case "ORO":
                        return "S-Rough-Outlines".ToUpper();

                    case "OSS":
                        return "S-Steps".ToUpper();

                    case "OTB":
                        return "S-Tee-Box".ToUpper();

                    case "OTO":
                        return "S-Tree-Outline".ToUpper();

                    case "OWS":
                        return "S-Walk-Strip".ToUpper();

                    case "OWL":
                        return "S-Wall".ToUpper();

                    case "OWA":
                        return "S-Water".ToUpper();

                    case "OWD":
                        return "S-Water-Drop".ToUpper();
                    case "OTH":
                        return "S-Other".ToUpper();
                    default:
                        return name;
                }
            }
            catch (System.Exception )
            {
            }
            return null;
        }

        //public static void SelectPolylines()
        //{
        //    ObjectIdCollection oids = null;
        //    SelectionSet selection = null;
        //    selection = BBC.Common.AutoCAD.SelectionManager.GetSelectionSet("");

        //    foreach (ObjectId obj in selection.GetObjectIds())
        //    {
        //        InteratePolyLines(obj);
        //    }

        //}

        public static void ProcessLayers(ObjectIdCollection objs)
        {
            foreach (ObjectId obj in objs)
            {
                InteratePolyLines(obj);
            }
        }

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
            foreach (string type in types)
                typedValueSelection.Insert(1, new TypedValue(Convert.ToInt32(DxfCode.Start), type));

            SelectionFilter selectionFilter = new SelectionFilter(typedValueSelection.ToArray());

            // because we have to.. Not really sure why, I assume this is our only access point
            // to grab the entities that we want. (I am open to being corrected)
            PromptSelectionResult promptSelectionResult = oEd.SelectAll(selectionFilter);

            // return our new ObjectIdCollection that is "Hopefully" full of the types that we want.
            return new ObjectIdCollection(promptSelectionResult.Value.GetObjectIds());

        }


    }

  /// <summary>
        /// Contains helper methods for interacting with AutoCAD layers
        /// </summary>
        public class LayerManager
        {

            #region Private Members

            //private static readonly ILog //_logger = LogManager.GetLogger(typeof(LayerManager));

            #endregion

            #region CreateLayer

            /// <summary>
            /// Determines whether the specified db is defined.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <returns>
            ///   <c>true</c> if the specified db is defined; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsDefined(Database db, string layerName)
            {
                //_logger.Debug("Start IsDefined");
                bool defined = false;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    defined = IsDefined(db, trans, layerName);
                }
                //_logger.Debug("End IsDefined");
                return defined;
            }

            /// <summary>
            /// Determines whether the specified db is defined.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <returns>
            ///   <c>true</c> if the specified db is defined; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsDefined(Database db, Transaction trans, string layername)
            {
                //_logger.Debug("Start IsDefined");
                bool defined = false;
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (layerTable.Has(layername))
                    {
                        defined = true;
                    }
                }
                //_logger.Debug("End IsDefined");
                return defined;
            }

            /// <summary>
            /// Creates the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layername">The layername.</param>
            /// <returns></returns>
            public static ObjectId CreateLayer(Database db, string layername)// , string linetype, Color color, PlotStyleDescriptor poltStyle, bool plots, bool frewezeInViewPorts)
            {
                //_logger.Debug("Start CreateLayer");
                if (db == null)
                    throw new ArgumentNullException("db", "The argument 'db' was null.");

                if (string.IsNullOrEmpty(layername))
                    throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

                ObjectId id = ObjectId.Null;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    id = CreateLayer(db, trans, layername);
                    trans.Commit();
                }
                //_logger.Debug("End CreateLayer");
                return id;
            }

            /// <summary>
            /// Creates the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layername">The layername.</param>
            /// <param name="color">The color.</param>
            /// <returns></returns>
            public static ObjectId CreateLayer(Database db, string layername, Color color)
            {
                //_logger.Debug("Start CreateLayer");
                if (db == null)
                    throw new ArgumentNullException("db", "The argument 'db' was null.");

                if (string.IsNullOrEmpty(layername))
                    throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

                if (color == null)
                    throw new ArgumentNullException("color", "The argument 'color' was null.");

                ObjectId id = ObjectId.Null;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    id = CreateLayer(db, trans, layername, color);
                    trans.Commit();
                }
                //_logger.Debug("End CreateLayer");
                return id;
            }

            /// <summary>
            /// Creates the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <returns></returns>
            public static ObjectId CreateLayer(Database db, Transaction trans, string layername) //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
            {
                return CreateLayer(db, trans, layername, null, ObjectId.Null);
            }

            /// <summary>
            /// Creates the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <param name="color">The color.</param>
            /// <returns></returns>
            public static ObjectId CreateLayer(Database db, Transaction trans, string layername, Color color) //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
            {
                return CreateLayer(db, trans, layername, color, ObjectId.Null);
            }

            /// <summary>
            /// Creates the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <param name="color">The color.</param>
            /// <param name="linetypeId">The linetype id.</param>
            /// <returns></returns>
            public static ObjectId CreateLayer(Database db, Transaction trans, string layername, Color color, ObjectId linetypeId)
            {
                //_logger.Debug("Start CreateLayer");
                if (trans == null)
                    throw new ArgumentNullException("trans", "The argument 'trans' was null.");

                if (string.IsNullOrEmpty(layername))
                    throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

                ObjectId id = ObjectId.Null;

                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (!layerTable.Has(layername))
                    {
                        using (LayerTableRecord layerTableRecord = new LayerTableRecord())
                        {
                            layerTableRecord.Name = layername;
                            if (color != null)
                                layerTableRecord.Color = color;
                            if (linetypeId != ObjectId.Null)
                                layerTableRecord.LinetypeObjectId = linetypeId;
                            id = layerTable.Add(layerTableRecord);
                            trans.AddNewlyCreatedDBObject(layerTableRecord, true);
                        }
                    }
                    else
                    {
                        id = layerTable[layername];
                    }
                }

                //_logger.Debug("End CreateLayer");
                return id;
            }

            #endregion

            /// <summary>
            /// Deletes the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layername">The layername.</param>
            public static void DeleteLayer(Database db, string layername)
            {
                //_logger.Debug("Start DeleteLayer");
                if (db == null)
                    throw new ArgumentNullException("db", "The argument 'db' was null.");

                if (string.IsNullOrEmpty(layername))
                    throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    DeleteLayer(db, trans, layername);
                    trans.Commit();
                }
                //_logger.Debug("End DeleteLayer");
            }
            /// <summary>
            /// Deletes the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <returns></returns>
            public static bool DeleteLayer(Database db, Transaction trans, string layername) //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
            {
                //_logger.Debug("Start DeleteLayer");
                if (trans == null)
                    throw new ArgumentNullException("trans", "The argument 'trans' was null.");

                if (string.IsNullOrEmpty(layername))
                    throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

                bool erased = false;
                try
                {
                    using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                    {
                        if (layerTable.Has(layername))
                        {
                            ObjectId layerId = layerTable[layername];
                            using (LayerTableRecord layerTableRecord = trans.GetObject(layerId, OpenMode.ForWrite, false) as LayerTableRecord)
                            {
                                if (!layerTableRecord.IsErased)
                                {
                                    layerTableRecord.Erase();
                                    erased = true;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception )
                {
                    //_logger.Error("Error in DeleteLayer", ex);
                    throw;
                }

                //_logger.Debug("End DeleteLayer");
                return erased;
            }
            /// <summary>
            /// Returns the layer's color.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <returns></returns>
            /// <summary>
            /// Deletes the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layername">The layername.</param>
            /// <returns></returns>
            public static void GetLayerColor(Database db, string layername)
            {
                //_logger.Debug("Start DeleteLayer");
                if (db == null)
                    throw new ArgumentNullException("db", "The argument 'db' was null.");

                if (string.IsNullOrEmpty(layername))
                    throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    GetLayerColor(db, trans, layername);
                    trans.Commit();
                }
                //_logger.Debug("End DeleteLayer");
            }
            public static Color GetLayerColor(Database db, Transaction trans, string layername) //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
            {
                //_logger.Debug("Start DeleteLayer");
                if (trans == null)
                    throw new ArgumentNullException("trans", "The argument 'trans' was null.");

                if (string.IsNullOrEmpty(layername))
                    throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

                try
                {
                    using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                    {
                        if (layerTable.Has(layername))
                        {
                            ObjectId layerId = layerTable[layername];
                            using (LayerTableRecord layerTableRecord = trans.GetObject(layerId, OpenMode.ForWrite, false) as LayerTableRecord)
                            {
                                if (!layerTableRecord.IsErased)
                                {
                                    return layerTableRecord.Color;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception )
                {
                    //_logger.Error("Error in DeleteLayer", ex);
                    throw;
                }

                //_logger.Debug("End DeleteLayer");
                return null;
            }

            /// <summary>
            /// Gets the layer id.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layername">The layername.</param>
            /// <returns>The object id the layer if it exists, otherwise ObjectId.Null</returns>        
            public static ObjectId GetLayerId(Database db, string layername)
            {
                //_logger.Debug("Start GetLayerId");
                ObjectId layerId = ObjectId.Null;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    layerId = GetLayerId(db, trans, layername);
                    trans.Commit();
                }
                //_logger.Debug("End GetLayerId");
                return layerId;
            }

            /// <summary>
            /// Gets the layer id.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <returns>The object id the layer if it exists, otherwise ObjectId.Null</returns>
            public static ObjectId GetLayerId(Database db, Transaction trans, string layername)
            {
                //_logger.Debug("Start GetLayerId");
                ObjectId layerId = ObjectId.Null;
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (layerTable.Has(layername))
                    {
                        layerId = layerTable[layername];
                    }
                }
                //_logger.Debug("End GetLayerId");
                return layerId;
            }

            /// <summary>
            /// Gets the Object Ids of all layers matching the filter
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layerFilter">The layer filter.</param>
            /// <param name="useWildCard">if set to <c>true</c> [use wild card].</param>
            /// <returns></returns>
            public static ObjectIdCollection GetLayerIds(Database db, Transaction trans, string layerFilter, bool useWildCard)
            {
                //_logger.Debug("Start GetLayerIds");
                if (trans == null)
                    throw new ArgumentNullException("trans", "The argument 'trans' was null.");

                ObjectIdCollection layerIds = new ObjectIdCollection();
                ObjectId layerId = ObjectId.Null;

                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead, false))
                {
                    foreach (ObjectId id in layerTable)
                    {
                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForRead) as LayerTableRecord)
                        {
                            if (useWildCard == true)
                            {
                                if (record.Name.ToUpper().StartsWith(layerFilter.ToUpper().Replace("*", string.Empty)))
                                {
                                    layerIds.Add(record.ObjectId);
                                }
                            }
                            else
                            {
                                if (record.Name.ToUpper() == layerFilter.ToUpper())
                                {
                                    layerIds.Add(record.ObjectId);
                                }
                            }
                        }
                    }
                }
                //_logger.Debug("End GetLayerIds");
                return layerIds;
            }

            /// <summary>
            /// Gets a list of layer names.
            /// </summary>
            /// <param name="db">The database to read.</param>
            /// <returns></returns>
            public static IList<string> GetLayerNames(Database db)
            {
                //_logger.Debug("Start GetLayerNames");
                IList<string> layerNames = new List<string>();
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    layerNames = GetLayerNames(db, trans);
                    trans.Commit();
                }
                //_logger.Debug("End GetLayerNames");
                return layerNames;
            }

            /// <summary>
            /// Gets a list of layer names.
            /// </summary>
            /// <param name="db">The database to read.</param>
            /// <param name="trans">The transaction to use.</param>
            /// <returns>A list of layer names</returns>
            public static IList<string> GetLayerNames(Database db, Transaction trans)
            {
                //_logger.Debug("Start GetLayerNames");
                IList<string> layerNames = new List<string>();
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    foreach (ObjectId id in layerTable)
                    {
                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForRead) as LayerTableRecord)
                        {
                            layerNames.Add(record.Name);
                        }
                    }
                }
                //_logger.Debug("End GetLayerNames");
                return layerNames;
            }

            /// <summary>
            /// Gets a list of layer names.
            /// </summary>
            /// <param name="db">The database to read.</param>
            /// <param name="trans">The transaction to use.</param>
            /// <returns>A list of layer names</returns>
            public static IList<string> GetLayerNames(Database db, Transaction trans, string filter)
            {
                //_logger.Debug("Start GetLayerNames");
                if (trans == null)
                    throw new ArgumentNullException("trans", "The argument 'trans' was null.");

                IList<string> layerNames = new List<string>();

                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead, false))
                {
                    foreach (ObjectId id in layerTable)
                    {
                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForRead) as LayerTableRecord)
                        {
                            if (record.Name.ToUpper().StartsWith(filter.ToUpper().Replace("*", string.Empty)))
                            {
                                layerNames.Add(record.Name);
                            }
                        }
                    }
                }
                //_logger.Debug("End GetLayerNames");
                return layerNames;
            }

            #region Lock

            /// <summary>
            /// Gets a list of layer names.
            /// </summary>
            /// <param name="db">The database to read.</param>
            /// <returns></returns>
            public static bool IsLocked(Database db, string layerName)
            {
                //_logger.Debug("Start IsLocked");
                bool locked = false;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    locked = IsLocked(db, trans, layerName);
                    trans.Commit();
                }
                //_logger.Debug("End IsLocked");
                return locked;
            }

            /// <summary>
            /// Gets a list of layer names.
            /// </summary>
            /// <param name="db">The database to read.</param>
            /// <returns></returns>
            public static bool IsLocked(Database db, ObjectId layerId)
            {
                //_logger.Debug("Start IsLocked");
                bool locked = false;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    locked = IsLocked(db, trans, layerId);
                    trans.Commit();
                }
                //_logger.Debug("End IsLocked");
                return locked;
            }

            /// <summary>
            /// Gets a list of layer names.
            /// </summary>
            /// <param name="db">The database to read.</param>
            /// <param name="trans">The transaction to use.</param>
            /// <returns>A list of layer names</returns>
            public static bool IsLocked(Database db, Transaction trans, string layername)
            {
                //_logger.Debug("Start IsLocked");
                bool locked = false;
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    ObjectId id = layerTable[layername];
                    locked = IsLocked(db, trans, id);
                }
                //_logger.Debug("End IsLocked");
                return locked;
            }

            /// <summary>
            /// Gets a list of layer names.
            /// </summary>
            /// <param name="db">The database to read.</param>
            /// <param name="trans">The transaction to use.</param>
            /// <returns>A list of layer names</returns>
            public static bool IsLocked(Database db, Transaction trans, ObjectId layerId)
            {
                //_logger.Debug("Start IsLocked");
                bool locked = false;
                using (LayerTableRecord layerTableRecord = trans.GetObject(layerId, OpenMode.ForRead, false) as LayerTableRecord)
                {
                    locked = layerTableRecord.IsLocked;
                }
                //_logger.Debug("End IsLocked");
                return locked;
            }

            /// <summary>
            /// Locks the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
            /// <returns></returns>
            public static void LockLayer(Database db, string layerName, bool isLocked)
            {
                //_logger.Debug("Start IsLocked");
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    LockLayer(db, trans, layerName, isLocked);
                    trans.Commit();
                }
                //_logger.Debug("End IsLocked");
            }

            /// <summary>
            /// Locks the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
            /// <returns></returns>
            public static void LockLayer(Database db, Transaction trans, string layername, bool isLocked)
            {
                //_logger.Debug("Start IsLocked");
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (layerTable.Has(layername) == true)
                    {
                        ObjectId id = layerTable[layername];

                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                        {
                            record.IsLocked = isLocked;
                        }
                    }
                }
                //_logger.Debug("End IsLocked");
            }

            /// <summary>
            /// Locks all layers.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
            public static void LockAllLayers(Database db, bool isLocked)
            {
                //_logger.Debug("Start LockAllLayers");
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    LockAllLayers(db, trans, isLocked);
                    trans.Commit();
                }
                //_logger.Debug("End LockAllLayers");
            }

            /// <summary>
            /// Locks all layers.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
            public static void LockAllLayers(Database db, Transaction trans, bool isLocked)
            {
                //_logger.Debug("Start LockAllLayers");
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    foreach (ObjectId id in layerTable)
                    {
                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                        {
                            record.IsLocked = isLocked;
                        }
                    }
                }
                //_logger.Debug("End LockAllLayers");
            }

            #endregion

            #region Freeze

            /// <summary>
            /// Freezes the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
            public static void FreezeLayer(Database db, string layerName, bool isFrozen)
            {
                //_logger.Debug("Start FreezeLayer");
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    FreezeLayer(db, trans, layerName, isFrozen);
                    trans.Commit();
                }
                //_logger.Debug("End FreezeLayer");
            }

            /// <summary>
            /// Freezes the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
            public static void FreezeLayer(Database db, Transaction trans, string layerName, bool isFrozen)
            {
                //_logger.Debug("Start FreezeLayer");
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {

                    string cLayerName = String.Empty;
                    ObjectId cLayerId = db.Clayer;
                    using (LayerTableRecord record = trans.GetObject(cLayerId, OpenMode.ForRead) as LayerTableRecord)
                    {
                        cLayerName = record.Name.ToUpper();
                    }
                    if (layerName.ToUpper() != cLayerName)
                    {
                        if (layerTable.Has(layerName) == true)
                        {
                            ObjectId id = layerTable[layerName];

                            using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                            {
                                record.IsFrozen = isFrozen;
                            }
                        }
                    }
                }
                //_logger.Debug("End FreezeLayer");
            }

            /// <summary>
            /// Freezes all layers.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
            public static void FreezeAllLayers(Database db, bool isFrozen)
            {
                //_logger.Debug("Start FreezeAllLayers");
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    FreezeAllLayers(db, trans, isFrozen);
                    trans.Commit();
                }
                //_logger.Debug("End FreezeAllLayers");
            }

            /// <summary>
            /// Freezes all layers.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
            public static void FreezeAllLayers(Database db, Transaction trans, bool isFrozen)
            {
                //_logger.Debug("Start FreezeAllLayers");
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    string cLayerName = String.Empty;
                    ObjectId cLayerId = db.Clayer;
                    using (LayerTableRecord record = trans.GetObject(cLayerId, OpenMode.ForRead) as LayerTableRecord)
                    {
                        cLayerName = record.Name.ToUpper();
                    }
                    foreach (ObjectId id in layerTable)
                    {
                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                        {
                            if (record.Name.ToUpper() != cLayerName)
                            {
                                record.IsFrozen = isFrozen;
                            }
                        }
                    }
                }
                //_logger.Debug("End FreezeAllLayers");
            }

            #endregion

            #region Off

            /// <summary>
            /// Determines whether the specified db is off.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <returns>
            ///   <c>true</c> if the specified db is off; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsOff(Database db, string layerName)
            {
                //_logger.Debug("Start IsOff");
                bool off = false;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    off = IsOff(db, trans, layerName);
                    trans.Commit();
                }
                //_logger.Debug("End IsOff");
                return off;
            }

            /// <summary>
            /// Determines whether the specified db is off.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layerId">The layer id.</param>
            /// <returns>
            ///   <c>true</c> if the specified db is off; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsOff(Database db, ObjectId layerId)
            {
                //_logger.Debug("Start IsOff");
                bool off = false;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    off = IsOff(db, trans, layerId);
                    trans.Commit();
                }
                //_logger.Debug("End IsOff");
                return off;
            }

            /// <summary>
            /// Determines whether the specified db is off.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layername">The layername.</param>
            /// <returns>
            ///   <c>true</c> if the specified db is off; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsOff(Database db, Transaction trans, string layername)
            {
                //_logger.Debug("Start IsOff");
                bool off = false;
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    ObjectId id = layerTable[layername];
                    off = IsOff(db, trans, id);
                }
                //_logger.Debug("End IsOff");
                return off;
            }

            /// <summary>
            /// Determines whether the specified db is off.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layerId">The layer id.</param>
            /// <returns>
            ///   <c>true</c> if the specified db is off; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsOff(Database db, Transaction trans, ObjectId layerId)
            {
                //_logger.Debug("Start IsOff");
                bool off = false;
                using (LayerTableRecord layerTableRecord = trans.GetObject(layerId, OpenMode.ForRead, false) as LayerTableRecord)
                {
                    off = layerTableRecord.IsOff;
                }
                //_logger.Debug("End IsOff");
                return off;
            }

            /// <summary>
            /// Offs the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <param name="isOff">if set to <c>true</c> [is off].</param>
            public static void OffLayer(Database db, string layerName, bool isOff)
            {
                //_logger.Debug("Start OffLayer");
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    OffLayer(db, trans, layerName, isOff);
                    trans.Commit();
                }
                //_logger.Debug("End OffLayer");
            }

            /// <summary>
            /// Offs the layer.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="layerName">Name of the layer.</param>
            /// <param name="isOff">if set to <c>true</c> [is off].</param>
            public static void OffLayer(Database db, Transaction trans, string layerName, bool isOff)
            {
                //_logger.Debug("Start OffLayer");
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (layerTable.Has(layerName) == true)
                    {
                        ObjectId id = layerTable[layerName];

                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                        {
                            record.IsOff = isOff;
                        }
                    }
                }
                //_logger.Debug("End OffLayer");
            }

            /// <summary>
            /// Offs all layers.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="isOff">if set to <c>true</c> [is off].</param>
            public static void OffAllLayers(Database db, bool isOff)
            {
                //_logger.Debug("Start OffAllLayers");
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    OffAllLayers(db, trans, isOff);
                    trans.Commit();
                }
                //_logger.Debug("End OffAllLayers");
            }

            /// <summary>
            /// Offs all layers.
            /// </summary>
            /// <param name="db">The db.</param>
            /// <param name="trans">The trans.</param>
            /// <param name="isOff">if set to <c>true</c> [is off].</param>
            public static void OffAllLayers(Database db, Transaction trans, bool isOff)
            {
                //_logger.Debug("Start OffAllLayers");
                using (LayerTable layerTable = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    foreach (ObjectId id in layerTable)
                    {
                        using (LayerTableRecord record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                        {
                            record.IsOff = isOff;
                        }
                    }
                }
                //_logger.Debug("End OffAllLayers");
            }

            #endregion

        }
    }

