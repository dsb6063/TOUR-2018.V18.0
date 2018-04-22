#region

using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using global::Autodesk.AutoCAD.DatabaseServices;

#endregion

//using Common.Logging;

namespace BBC.Common.AutoCAD
{
    /// <summary>
    ///     Contains helper methods for interacting with AutoCAD layers
    /// </summary>
    public class LayerManager
    {
        /// <summary>
        ///     Deletes the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layername">The layername.</param>
        public static void DeleteLayer(Database db, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayer");
            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            using (var trans = db.TransactionManager.StartTransaction())
            {
                DeleteLayer(db, trans, layername);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayer");
        }

        /// <summary>
        ///     Deletes the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <returns></returns>
        public static bool DeleteLayer(Database db, Transaction trans, string layername)
            //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayer");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            var erased = false;
            try
            {
                using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (layerTable.Has(layername))
                    {
                        var layerId = layerTable[layername];
                        using (
                            var layerTableRecord =
                                trans.GetObject(layerId, OpenMode.ForWrite, false) as LayerTableRecord)
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
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in DeleteLayer", ex);
                throw;
            }

            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayer");
            return erased;
        }

        /// <summary>
        ///     Returns the layer's color.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <returns></returns>
        /// <summary>
        ///     Deletes the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layername">The layername.</param>
        /// <returns></returns>
        public static void GetLayerColor(Database db, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayer");
            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            using (var trans = db.TransactionManager.StartTransaction())
            {
                GetLayerColor(db, trans, layername);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayer");
        }

        public static void ChangeLayerColor(Database db, string layername, Color color)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayer");
            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            using (var trans = db.TransactionManager.StartTransaction())
            {
                ChangeLayerColor(db, trans, layername, color);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayer");
        }

        public static Color ChangeLayerColor(Database db, Transaction trans, string layername, Color color)
            //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayer");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            //var erased = false;
            try
            {
                using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (layerTable.Has(layername))
                    {
                        var layerId = layerTable[layername];
                        using (
                            var layerTableRecord =
                                trans.GetObject(layerId, OpenMode.ForWrite, false) as LayerTableRecord)
                        {
                            if (!layerTableRecord.IsErased)
                            {
                                layerTableRecord.Color = color;
                                return color;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in DeleteLayer", ex);
                throw;
            }

            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayer");
            return null;
        }

        public static Color GetLayerColor(Database db, Transaction trans, string layername)
            //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DeleteLayer");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

           // var erased = false;
            try
            {
                using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
                {
                    if (layerTable.Has(layername))
                    {
                        var layerId = layerTable[layername];
                        using (
                            var layerTableRecord =
                                trans.GetObject(layerId, OpenMode.ForWrite, false) as LayerTableRecord)
                        {
                            if (!layerTableRecord.IsErased)
                            {
                                return layerTableRecord.Color;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in DeleteLayer", ex);
                throw;
            }

            PGA.MessengerManager.MessengerManager.AddLog("End DeleteLayer");
            return null;
        }

        /// <summary>
        ///     Gets the layer id.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layername">The layername.</param>
        /// <returns>The object id the layer if it exists, otherwise ObjectId.Null</returns>
        public static ObjectId GetLayerId(Database db, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayerId");
            var layerId = ObjectId.Null;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                layerId = GetLayerId(db, trans, layername);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayerId");
            return layerId;
        }

        /// <summary>
        ///     Gets the layer id.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <returns>The object id the layer if it exists, otherwise ObjectId.Null</returns>
        public static ObjectId GetLayerId(Database db, Transaction trans, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayerId");
            var layerId = ObjectId.Null;
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                if (layerTable.Has(layername))
                {
                    layerId = layerTable[layername];
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayerId");
            return layerId;
        }

        /// <summary>
        ///     Gets the Object Ids of all layers matching the filter
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layerFilter">The layer filter.</param>
        /// <param name="useWildCard">if set to <c>true</c> [use wild card].</param>
        /// <returns></returns>
        public static ObjectIdCollection GetLayerIds(Database db, Transaction trans, string layerFilter,
            bool useWildCard)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayerIds");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            var layerIds = new ObjectIdCollection();
            var layerId = ObjectId.Null;

            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForRead, false))
            {
                foreach (var id in layerTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForRead) as LayerTableRecord)
                    {
                        if (useWildCard)
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
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayerIds");
            return layerIds;
        }

        /// <summary>
        ///     Gets a list of layer names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <returns></returns>
        public static IList<string> GetLayerNames(Database db)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayerNames");
            IList<string> layerNames = new List<string>();
            using (var trans = db.TransactionManager.StartTransaction())
            {
                layerNames = GetLayerNames(db, trans);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayerNames");
            return layerNames;
        }

        /// <summary>
        ///     Gets a list of layer names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of layer names</returns>
        public static IList<string> GetLayerNames(Database db, Transaction trans)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayerNames");
            IList<string> layerNames = new List<string>();
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                foreach (var id in layerTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForRead) as LayerTableRecord)
                    {
                        layerNames.Add(record.Name);
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayerNames");
            return layerNames;
        }

        /// <summary>
        ///     Gets a list of layer names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of layer names</returns>
        public static IList<string> GetLayerNames(Database db, Transaction trans, string filter)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetLayerNames");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            IList<string> layerNames = new List<string>();

            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForRead, false))
            {
                foreach (var id in layerTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForRead) as LayerTableRecord)
                    {
                        if (record.Name.ToUpper().StartsWith(filter.ToUpper().Replace("*", string.Empty)))
                        {
                            layerNames.Add(record.Name);
                        }
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetLayerNames");
            return layerNames;
        }

        #region ON

        /// <summary>
        ///     Ons the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="isOff">if set to <c>true</c> [is off].</param>
        public static void ONLayer(Database db, string layerName, bool isOff)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start OffLayer");
            using (var trans = db.TransactionManager.StartTransaction())
            {
                OffLayer(db, trans, layerName, isOff);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End OffLayer");
        }

        #endregion

        #region Private Members

        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(LayerManager));

        #endregion

        #region CreateLayer

        /// <summary>
        ///     Determines whether the specified db is defined.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <returns>
        ///     <c>true</c> if the specified db is defined; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefined(Database db, string layerName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsDefined");
            var defined = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                defined = IsDefined(db, trans, layerName);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsDefined");
            return defined;
        }

        /// <summary>
        ///     Determines whether the specified db is defined.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <returns>
        ///     <c>true</c> if the specified db is defined; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefined(Database db, Transaction trans, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsDefined");
            var defined = false;
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                if (layerTable.Has(layername))
                {
                    defined = true;
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsDefined");
            return defined;
        }

        /// <summary>
        ///     Creates the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layername">The layername.</param>
        /// <returns></returns>
        public static ObjectId CreateLayer(Database db, string layername)
            // , string linetype, Color color, PlotStyleDescriptor poltStyle, bool plots, bool frewezeInViewPorts)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start CreateLayer");
            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            var id = ObjectId.Null;

            using (var trans = db.TransactionManager.StartTransaction())
            {
                id = CreateLayer(db, trans, layername);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End CreateLayer");
            return id;
        }

        /// <summary>
        ///     Creates the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layername">The layername.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static ObjectId CreateLayer(Database db, string layername, Color color)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start CreateLayer");
            if (db == null)
                throw new ArgumentNullException("db", "The argument 'db' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            if (color == null)
                throw new ArgumentNullException("color", "The argument 'color' was null.");

            var id = ObjectId.Null;

            using (var trans = db.TransactionManager.StartTransaction())
            {
                id = CreateLayer(db, trans, layername, color);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End CreateLayer");
            return id;
        }

        /// <summary>
        ///     Creates the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <returns></returns>
        public static ObjectId CreateLayer(Database db, Transaction trans, string layername)
            //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
        {
            return CreateLayer(db, trans, layername, null, ObjectId.Null);
        }

        /// <summary>
        ///     Creates the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static ObjectId CreateLayer(Database db, Transaction trans, string layername, Color color)
            //, ObjectId linetypeId, Color color, ObjectId plotStyleNameId, bool plots, bool frewezeInViewPorts)
        {
            return CreateLayer(db, trans, layername, color, ObjectId.Null);
        }

        /// <summary>
        ///     Creates the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <param name="color">The color.</param>
        /// <param name="linetypeId">The linetype id.</param>
        /// <returns></returns>
        public static ObjectId CreateLayer(Database db, Transaction trans, string layername, Color color,
            ObjectId linetypeId)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start CreateLayer");
            if (trans == null)
                throw new ArgumentNullException("trans", "The argument 'trans' was null.");

            if (string.IsNullOrEmpty(layername))
                throw new ArgumentNullException("layername", "The argument 'layername' was null or empty.");

            var id = ObjectId.Null;

            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                if (!layerTable.Has(layername))
                {
                    using (var layerTableRecord = new LayerTableRecord())
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

            PGA.MessengerManager.MessengerManager.AddLog("End CreateLayer");
            return id;
        }

        #endregion

        #region Lock

        /// <summary>
        ///     Gets a list of layer names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <returns></returns>
        public static bool IsLocked(Database db, string layerName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLocked");
            var locked = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                locked = IsLocked(db, trans, layerName);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLocked");
            return locked;
        }

        /// <summary>
        ///     Gets a list of layer names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <returns></returns>
        public static bool IsLocked(Database db, ObjectId layerId)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLocked");
            var locked = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                locked = IsLocked(db, trans, layerId);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLocked");
            return locked;
        }

        /// <summary>
        ///     Gets a list of layer names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of layer names</returns>
        public static bool IsLocked(Database db, Transaction trans, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLocked");
            var locked = false;
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                var id = layerTable[layername];
                locked = IsLocked(db, trans, id);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLocked");
            return locked;
        }

        /// <summary>
        ///     Gets a list of layer names.
        /// </summary>
        /// <param name="db">The database to read.</param>
        /// <param name="trans">The transaction to use.</param>
        /// <returns>A list of layer names</returns>
        public static bool IsLocked(Database db, Transaction trans, ObjectId layerId)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLocked");
            var locked = false;
            using (var layerTableRecord = trans.GetObject(layerId, OpenMode.ForRead, false) as LayerTableRecord)
            {
                locked = layerTableRecord.IsLocked;
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLocked");
            return locked;
        }

        /// <summary>
        ///     Locks the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns></returns>
        public static void LockLayer(Database db, string layerName, bool isLocked)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLocked");
            using (var trans = db.TransactionManager.StartTransaction())
            {
                LockLayer(db, trans, layerName, isLocked);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLocked");
        }

        /// <summary>
        ///     Locks the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns></returns>
        public static void LockLayer(Database db, Transaction trans, string layername, bool isLocked)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsLocked");
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                if (layerTable.Has(layername))
                {
                    var id = layerTable[layername];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                    {
                        record.IsLocked = isLocked;
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsLocked");
        }

        /// <summary>
        ///     Locks all layers.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        public static void LockAllLayers(Database db, bool isLocked)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start LockAllLayers");
            using (var trans = db.TransactionManager.StartTransaction())
            {
                LockAllLayers(db, trans, isLocked);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End LockAllLayers");
        }

        /// <summary>
        ///     Locks all layers.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        public static void LockAllLayers(Database db, Transaction trans, bool isLocked)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start LockAllLayers");
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                foreach (var id in layerTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                    {
                        record.IsLocked = isLocked;
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End LockAllLayers");
        }

        #endregion

        #region Freeze

        /// <summary>
        ///     Freezes the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
        public static void FreezeLayer(Database db, string layerName, bool isFrozen)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start FreezeLayer");
            using (var trans = db.TransactionManager.StartTransaction())
            {
                FreezeLayer(db, trans, layerName, isFrozen);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End FreezeLayer");
        }

        /// <summary>
        ///     Freezes the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
        public static void FreezeLayer(Database db, Transaction trans, string layerName, bool isFrozen)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start FreezeLayer");
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                var cLayerName = string.Empty;
                var cLayerId = db.Clayer;
                using (var record = trans.GetObject(cLayerId, OpenMode.ForRead) as LayerTableRecord)
                {
                    cLayerName = record.Name.ToUpper();
                }
                if (layerName.ToUpper() != cLayerName)
                {
                    if (layerTable.Has(layerName))
                    {
                        var id = layerTable[layerName];

                        using (var record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                        {
                            record.IsFrozen = isFrozen;
                        }
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End FreezeLayer");
        }

        /// <summary>
        ///     Freezes all layers.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
        public static void FreezeAllLayers(Database db, bool isFrozen)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start FreezeAllLayers");
            using (var trans = db.TransactionManager.StartTransaction())
            {
                FreezeAllLayers(db, trans, isFrozen);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End FreezeAllLayers");
        }

        /// <summary>
        ///     Freezes all layers.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="isFrozen">if set to <c>true</c> [is frozen].</param>
        public static void FreezeAllLayers(Database db, Transaction trans, bool isFrozen)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start FreezeAllLayers");
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                var cLayerName = string.Empty;
                var cLayerId = db.Clayer;
                using (var record = trans.GetObject(cLayerId, OpenMode.ForRead) as LayerTableRecord)
                {
                    cLayerName = record.Name.ToUpper();
                }
                foreach (var id in layerTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                    {
                        if (record.Name.ToUpper() != cLayerName)
                        {
                            record.IsFrozen = isFrozen;
                        }
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End FreezeAllLayers");
        }

        #endregion

        #region Off

        /// <summary>
        ///     Determines whether the specified db is off.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <returns>
        ///     <c>true</c> if the specified db is off; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOff(Database db, string layerName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsOff");
            var off = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                off = IsOff(db, trans, layerName);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsOff");
            return off;
        }

        /// <summary>
        ///     Determines whether the specified db is off.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layerId">The layer id.</param>
        /// <returns>
        ///     <c>true</c> if the specified db is off; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOff(Database db, ObjectId layerId)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsOff");
            var off = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                off = IsOff(db, trans, layerId);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsOff");
            return off;
        }

        /// <summary>
        ///     Determines whether the specified db is off.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layername">The layername.</param>
        /// <returns>
        ///     <c>true</c> if the specified db is off; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOff(Database db, Transaction trans, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsOff");
            var off = false;
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                var id = layerTable[layername];
                off = IsOff(db, trans, id);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsOff");
            return off;
        }

        /// <summary>
        ///     Determines whether the specified db is off.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layerId">The layer id.</param>
        /// <returns>
        ///     <c>true</c> if the specified db is off; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOff(Database db, Transaction trans, ObjectId layerId)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsOff");
            var off = false;
            using (var layerTableRecord = trans.GetObject(layerId, OpenMode.ForRead, false) as LayerTableRecord)
            {
                off = layerTableRecord.IsOff;
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsOff");
            return off;
        }

        /// <summary>
        ///     Offs the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="isOff">if set to <c>true</c> [is off].</param>
        public static void OffLayer(Database db, string layerName, bool isOff)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start OffLayer");
            using (var trans = db.TransactionManager.StartTransaction())
            {
                OffLayer(db, trans, layerName, isOff);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End OffLayer");
        }

        /// <summary>
        ///     Offs the layer.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="isOff">if set to <c>true</c> [is off].</param>
        public static void OffLayer(Database db, Transaction trans, string layerName, bool isOff)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start OffLayer");
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                if (layerTable.Has(layerName))
                {
                    var id = layerTable[layerName];

                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                    {
                        record.IsOff = isOff;
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End OffLayer");
        }

        /// <summary>
        ///     Offs all layers.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="isOff">if set to <c>true</c> [is off].</param>
        public static void OffAllLayers(Database db, bool isOff)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start OffAllLayers");
            using (var trans = db.TransactionManager.StartTransaction())
            {
                OffAllLayers(db, trans, isOff);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End OffAllLayers");
        }

        /// <summary>
        ///     Offs all layers.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="isOff">if set to <c>true</c> [is off].</param>
        public static void OffAllLayers(Database db, Transaction trans, bool isOff)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start OffAllLayers");
            using (var layerTable = (LayerTable) trans.GetObject(db.LayerTableId, OpenMode.ForWrite, false))
            {
                foreach (var id in layerTable)
                {
                    using (var record = trans.GetObject(id, OpenMode.ForWrite) as LayerTableRecord)
                    {
                        record.IsOff = isOff;
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End OffAllLayers");
        }

        #endregion
    }
}