#region

using System.Collections;
using System.IO;
using global::Autodesk.AutoCAD.ApplicationServices.Core;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.Runtime;
//using Common.Logging;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

#endregion

namespace BBC.Common.AutoCAD
{
    public class BlockManager
    {
        /// <summary>
        ///     Adds the block.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="point">The point.</param>
        /// <param name="angle">The angle in radians.</param>
        /// <param name="attributeTagValue">The attribute tag value.</param>
        /// <param name="layername">The layername.</param>
        public static ObjectId AddBlock(Database db, Transaction trans, string blockName, Point3d point, double angle,
            Hashtable attributeTagValue, string layername)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddBlock");
            var id = ObjectId.Null;
            var tm = db.TransactionManager;

            using (var bt = (BlockTable) tm.GetObject(db.BlockTableId, OpenMode.ForRead, false))
            {
                if (bt.Has(blockName))
                {
                    using (var blockDefinition = (BlockTableRecord) tm.GetObject(bt[blockName], OpenMode.ForRead, false)
                    )
                    {
                        using (
                            var btr =
                                (BlockTableRecord)
                                tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false))
                        {
                            using (var blockRef = new BlockReference(point, blockDefinition.ObjectId))
                            {
                                blockRef.Layer = layername;
                                blockRef.Rotation = angle;
                                id = btr.AppendEntity(blockRef);
                                tm.AddNewlyCreatedDBObject(blockRef, true);
                                AddBlockAttributes(blockRef, blockDefinition, attributeTagValue, tm);
                            }
                        }
                    }
                }
                PGA.MessengerManager.MessengerManager.AddLog("End AddBlock");
                return id;
            }
        }

        /// <summary>
        ///     Adds the block attributes.
        /// </summary>
        /// <param name="blockRef">The block ref.</param>
        /// <param name="blockDefinition">The block definition.</param>
        /// <param name="attributeTagValue">The attribute tag value.</param>
        /// <param name="tm">The tm.</param>
        private static void AddBlockAttributes(BlockReference blockRef, BlockTableRecord blockDefinition,
            Hashtable attributeTagValue, TransactionManager tm)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddBlockAttributes");
            var m = blockRef.BlockTransform;
            IEnumerator i;

            i = blockDefinition.GetEnumerator();
            while (i.MoveNext())
            {
                var obj = tm.GetObject((ObjectId) i.Current, OpenMode.ForRead, false);
                if (typeof(AttributeDefinition) == obj.GetType())
                {
                    var attDef = (AttributeDefinition) obj;
                    if (!attDef.Constant)
                    {
                        var attRef = new AttributeReference();
                        attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);
                        if (attributeTagValue.ContainsKey(attRef.Tag))
                            attRef.TextString = (string) attributeTagValue[attRef.Tag];

                        attRef.Position = attDef.Position.TransformBy(m);
                        blockRef.AttributeCollection.AppendAttribute(attRef);
                        tm.AddNewlyCreatedDBObject(attRef, true);
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End AddBlockAttributes");
        }

        /// <summary>
        ///     Set an attribute value on a block
        /// </summary>
        /// <param name="blockRef">The block reference</param>
        /// <param name="tag">The tag to find</param>
        /// <param name="value">The value to set</param>
        public static void SetAttributeValue(Transaction trans, BlockReference blockRef, string tag, string value)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetAttributeValue");
            if (value == null)
            {
                return;
            }
            value = value.Trim();


            foreach (ObjectId id in blockRef.AttributeCollection)
            {
                using (var attrRef = trans.GetObject(id, OpenMode.ForWrite) as AttributeReference)
                {
                    if (attrRef != null)
                    {
                        if (attrRef.Tag.CompareTo(tag) == 0)
                        {
                            attrRef.TextString = value;
                        }
                    }
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End SetAttributeValue");
        }

        #region Private Members

        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(BlockManager));

        #endregion

        #region IsBlockDefined

        /// <summary>
        ///     Determines whether [is block defined] [the specified block name].
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <returns>
        ///     <c>true</c> if [is block defined] [the specified block name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBlockDefined(string blockName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsBlockDefined");
            var retVal = false;
            var db = Application.DocumentManager.MdiActiveDocument.Database;
            retVal = IsBlockDefined(db, blockName);
            PGA.MessengerManager.MessengerManager.AddLog("End IsBlockDefined");
            return retVal;
        }

        /// <summary>
        ///     Determines whether [is block defined] [the specified db].
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="blockName">Name of the block.</param>
        /// <returns>
        ///     <c>true</c> if [is block defined] [the specified db]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBlockDefined(Database db, string blockName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsBlockDefined");
            var retVal = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retVal = IsBlockDefined(db, trans, blockName);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsBlockDefined");
            return retVal;
        }

        /// <summary>
        ///     Determines whether [is block defined] [the specified db].
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="blockName">Name of the block.</param>
        /// <returns>
        ///     <c>true</c> if [is block defined] [the specified db]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBlockDefined(Database db, Transaction trans, string blockName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start IsBlockDefined");
            var retval = false;
            var bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            if (bt != null)
            {
                if (bt.Has(blockName))
                {
                    retval = true;
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End IsBlockDefined");
            return retval;
        }

        #endregion

        #region DefineBlockFromFileOnDisk

        /// <summary>
        ///     Defines the block from file on disk.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockPath">The block path.</param>
        /// <returns></returns>
        public static bool DefineBlockFromFileOnDisk(string blockName, string blockPath)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DefineBlockFromFileOnDisk");
            var retVal = false;
            var db = Application.DocumentManager.MdiActiveDocument.Database;
            retVal = DefineBlockFromFileOnDisk(db, blockName, blockPath);
            PGA.MessengerManager.MessengerManager.AddLog("End DefineBlockFromFileOnDisk");
            return retVal;
        }

        /// <summary>
        ///     Defines the block from file on disk.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockPath">The block path.</param>
        /// <returns></returns>
        public static bool DefineBlockFromFileOnDisk(Database db, string blockName, string blockPath)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DefineBlockFromFileOnDisk");
            var retVal = false;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                retVal = DefineBlockFromFileOnDisk(db, trans, blockName, blockPath);
                trans.Commit();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DefineBlockFromFileOnDisk");
            return retVal;
        }

        /// <summary>
        ///     Defines the block from file on disk.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockPath">The block path.</param>
        /// <returns></returns>
        public static bool DefineBlockFromFileOnDisk(Database db, Transaction trans, string blockName, string blockPath)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start DefineBlockFromFileOnDisk");
            var retval = false;
            try
            {
                // build the dwg file path
                if (blockPath.Substring(blockPath.Length - 1, 1) != @"\")
                {
                    blockPath = blockPath + @"\";
                }
                var dwgFileName = blockPath + blockName + ".dwg";
                var dwgFileNameLocated = HostApplicationServices.Current.FindFile(dwgFileName, db, FindFileHint.Default);

                // check if the dwg file exists in the AutoCAD search path
                if ((dwgFileNameLocated != string.Empty) || (dwgFileNameLocated != ""))
                {
                    // check if the block is already defined in the target database
                    var isBlockDefined = IsBlockDefined(db, trans, blockName);

                    // if the block is already defined - we need to use wblock clone to overwrite definition
                    if (isBlockDefined)
                    {
                        // start a new side database
                        var redefineDb = new Database(true, true);
                        // read the DWG into a side database
                        var blockDb = new Database(false, true);
                        blockDb.ReadDwgFile(dwgFileNameLocated, FileShare.Read, true, "");
                        // insert the the database loaded from block on disk into side database
                        var btrId = redefineDb.Insert(blockName, blockDb, false);
                        // clone inserted into side database into current database
                        var blockIds = new ObjectIdCollection();
                        blockIds.Add(btrId);
                        var mapping = new IdMapping();
                        redefineDb.WblockCloneObjects(blockIds, db.BlockTableId, mapping, DuplicateRecordCloning.Replace,
                            false);
                        blockDb.Dispose();
                        redefineDb.Dispose();
                    }
                    else
                    {
                        // read the DWG into a side database
                        var defineDb = new Database(false, true);
                        defineDb.ReadDwgFile(dwgFileNameLocated, FileShare.Read, true, "");
                        // insert the the database loaded from block on disk into current database
                        var oid = db.Insert(blockName, defineDb, false);
                        defineDb.Dispose();
                    }
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.ErrorStatus == ErrorStatus.FilerError) //Block drawing not found on disk
                {
                    retval = false;
                }
                else
                {
                    PGA.MessengerManager.MessengerManager.AddLog("Error thrown in DefineBlockFromFileOnDisk", ex);
                    throw;
                }
            }
            PGA.MessengerManager.MessengerManager.AddLog("End DefineBlockFromFileOnDisk");
            return retval;
        }

        #endregion
    }
}