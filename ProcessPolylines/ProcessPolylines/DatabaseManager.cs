
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using PGA.Sv.PostAudit;

namespace PGA.Common.AutoCAD
{
    public class AcadDatabaseManager
    {
       // private static readonly object DocumentManager;

        /// <summary>
        /// Adds the polyline.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="points">The points.</param>
        /// <param name="layername">The layername.</param>
        /// <param name="isClosed">if set to <c>true</c> [is closed].</param>
        /// <returns></returns>
        public static ObjectId AddPolyline(Transaction trans, Point2dCollection points, string layername, bool isClosed)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddPolyline");
            ObjectId id = ObjectId.Null;
            var db =   Active.Database;
            var tm = db.TransactionManager;

            using (Polyline pline = new Polyline())
            {
                pline.SetDatabaseDefaults();
                pline.Layer = layername;

                int index = 0;
                double buldge = 0.0;
                double width = 0.0;

                foreach (Point2d pt in points)
                {
                    pline.AddVertexAt(index++, pt, buldge, width, width);
                }

                pline.Closed = isClosed;
                id = AddToDatabase(db, pline, trans);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End AddPolyline");
            return id;
        }

        /// <summary>
        /// Adds the polyline.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="points">The points.</param>
        /// <param name="layername">The layername.</param>
        /// <param name="isClosed">if set to <c>true</c> [is closed].</param>
        /// <param name="colorIndex">Index of the color.</param>
        /// <returns></returns>
        public static ObjectId AddPolyline(Transaction trans, Point2dCollection points, string layername, bool isClosed, int colorIndex)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddPolyline");
            ObjectId id = ObjectId.Null;
            var db = Active.Database;
            var tm = db.TransactionManager;

            using (Polyline pline = new Polyline())
            {
                pline.SetDatabaseDefaults();
                pline.Layer = layername;
                if ((colorIndex > 0) && (colorIndex < 256))
                {
                    pline.ColorIndex = colorIndex;
                }
                int index = 0;
                double buldge = 0.0;
                double width = 0.0;

                foreach (Point2d pt in points)
                {
                    pline.AddVertexAt(index++, pt, buldge, width, width);
                }

                pline.Closed = isClosed;
                id = AddToDatabase(db, pline, trans);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End AddPolyline");
            return id;
        }

        /// <summary>
        /// Adds the predefined hatch.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="points">The points.</param>
        /// <param name="layername">The layername.</param>
        /// <param name="isClosed">if set to <c>true</c> [is closed].</param>
        /// <param name="patternName">Name of the pattern.</param>
        /// <returns></returns>
        public static ObjectId AddPredefinedHatch(Transaction trans, Point2dCollection points, string layername, bool isClosed, string patternName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddPredefinedHatch");
            ObjectId id = ObjectId.Null;
            global::Autodesk.AutoCAD.DatabaseServices.Database db = Active.Database;
            var tm = db.TransactionManager;

            using (Hatch hatch = new Hatch())
            {
                hatch.SetDatabaseDefaults();
                hatch.Layer = layername;

                id = AddToDatabase(db, hatch, trans);

                hatch.UpgradeOpen();
                //hatch.HatchStyle = HatchStyle.Outer;
                hatch.Associative = true;
                hatch.PatternScale = 100.0;
                hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);

                double buldge = 0.0;

                HatchLoop loop = new HatchLoop(HatchLoopTypes.Polyline);
                foreach (Point2d pt in points)
                {
                    BulgeVertex bv = new BulgeVertex(pt, buldge);
                    loop.Polyline.Add(bv);
                }
                hatch.AppendLoop(loop);
                hatch.EvaluateHatch(true);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End AddPredefinedHatch");
            return id;
        }

        /// <summary>
        /// Gets the hatch mid point.
        /// </summary>
        /// <param name="hatch">The hatch.</param>
        /// <returns></returns>
        public static Point3d GetHatchMidPoint(Hatch hatch)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetHatchMidPoint");
            // get the middle of the extents
            //
            double midPointX = hatch.GeometricExtents.MinPoint.X + ((hatch.GeometricExtents.MaxPoint.X - hatch.GeometricExtents.MinPoint.X) * 0.5);
            double midPointY = hatch.GeometricExtents.MinPoint.Y + ((hatch.GeometricExtents.MaxPoint.Y - hatch.GeometricExtents.MinPoint.Y) * 0.5);

            PGA.MessengerManager.MessengerManager.AddLog("End GetHatchMidPoint");
            return new Point3d(midPointX, midPointY, hatch.GeometricExtents.MaxPoint.Z);
        }

        /// <summary>
        /// Degrees to radian.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns></returns>
        public static double DegreeToRadian(double degree)
        {
            return degree * (Math.PI / 180.0);
        }

        /// <summary>
        /// radians to Degrees.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns></returns>
        public static double RadianToDegree(double degree)
        {
            return degree * (180.0 / Math.PI);
        }

        /// <summary>
        /// Adds the DB text.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="insert">The insert.</param>
        /// <param name="text">The text.</param>
        /// <param name="layerId">The layer id.</param>
        /// <param name="textStyleId">The text style id.</param>
        /// <param name="height">The height.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="hoizonatlMode">The hoizonatl mode.</param>
        /// <returns></returns>
        public static ObjectId AddMText(Transaction trans, Point3d insert, string text, string layer, string textStyle, double height, double rotation)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddMText");
            try
            {
                global::Autodesk.AutoCAD.DatabaseServices.Database db = Active.Database;

                ObjectId id = ObjectId.Null;

                MText itemText = new MText();
                itemText.SetDatabaseDefaults();
                itemText.Location = insert;
                itemText.Layer = layer;
                if (height > 0)
                {
                    itemText.TextHeight = height;
                }
                itemText.Rotation = rotation;
                itemText.Attachment = AttachmentPoint.MiddleCenter;
                itemText.Contents = text;

                using (TextStyleTable textStyleTable = (TextStyleTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead, false))
                {
                    if (textStyleTable.Has(textStyle))
                    {
                        itemText.TextStyleId = textStyleTable[textStyle];
                    }
                }

                id = AddToDatabase(db, itemText, trans);

                PGA.MessengerManager.MessengerManager.AddLog("End AddMText");
                return id;
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in AddDBText ", ex);
                throw;
            }
        }

        /// <summary>
        /// Adds the M text.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="insert">The insert.</param>
        /// <param name="text">The text.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="textStyle">The text style.</param>
        /// <param name="height">The height.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="colorIndex">Index of the color.</param>
        /// <returns></returns>
        public static ObjectId AddMText(Transaction trans, Point3d insert, string text, string layer, string textStyle, double height, double rotation, int colorIndex)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddMText");
            try
            {
                global::Autodesk.AutoCAD.DatabaseServices.Database db = Active.Database;

                ObjectId id = ObjectId.Null;

                MText itemText = new MText();
                itemText.SetDatabaseDefaults();
                itemText.Location = insert;
                itemText.Layer = layer;
                if (height > 0)
                {
                    itemText.TextHeight = height;
                }
                itemText.Rotation = rotation;
                itemText.Attachment = AttachmentPoint.MiddleCenter;
                itemText.Contents = text;
                itemText.ColorIndex = colorIndex;

                using (TextStyleTable textStyleTable = (TextStyleTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead, false))
                {
                    if (textStyleTable.Has(textStyle))
                    {
                        itemText.TextStyleId = textStyleTable[textStyle];
                    }
                }

                id = AddToDatabase(db, itemText, trans);

                PGA.MessengerManager.MessengerManager.AddLog("End AddMText");
                return id;
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in AddMText ", ex);
                throw;
            }
        }

        /// <summary>
        /// Adds the DB text.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="insert">The insert.</param>
        /// <param name="text">The text.</param>
        /// <param name="layerId">The layer id.</param>
        /// <param name="textStyleId">The text style id.</param>
        /// <param name="height">The height.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="hoizonatlMode">The hoizonatl mode.</param>
        /// <returns></returns>
        public static ObjectId AddDBText(Transaction trans, Point3d insert, string text, string layer, string textStyle, double height, double rotation, TextHorizontalMode horizontalMode)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddDBText");
            try
            {
                global::Autodesk.AutoCAD.DatabaseServices.Database db = Active.Database;

                ObjectId id = ObjectId.Null;

                DBText itemText = new DBText();
                itemText.SetDatabaseDefaults();

                itemText.Layer = layer;
                itemText.Height = height;
                itemText.Rotation = rotation;
                itemText.TextString = text;
                itemText.HorizontalMode = TextHorizontalMode.TextCenter;
                if (horizontalMode == TextHorizontalMode.TextCenter)
                {
                    itemText.AlignmentPoint = insert;
                }
                else
                {
                    itemText.Position = insert;
                }

                using (TextStyleTable textStyleTable = (TextStyleTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead, false))
                {
                    if (textStyleTable.Has(textStyle))
                    {
                        itemText.TextStyleId = textStyleTable[textStyle];
                    }
                }

                id = AddToDatabase(db, itemText, trans);

                PGA.MessengerManager.MessengerManager.AddLog("End AddDBText");
                return id;
            }
            catch(System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in AddDBText ", ex);
                throw;
            }
        }


        /// <summary>
        /// Adds an entity to the database and returns the id
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="databaseObject">The database object.</param>
        /// <param name="trans">The trans.</param>
        /// <returns>
        /// ObjectId of the entity after it is added to the database
        /// </returns>
        public static ObjectId AddToDatabase(global::Autodesk.AutoCAD.DatabaseServices.Database db, Entity databaseObject, Transaction trans)
        {
            ObjectId id = ObjectId.Null;
            PGA.MessengerManager.MessengerManager.AddLog("Start AddToDatabase");
            try
            {
                using (BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead))
                {
                    using (BlockTableRecord btr = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite))
                    {
                        id = btr.AppendEntity(databaseObject);
                        trans.AddNewlyCreatedDBObject(databaseObject, true);
                    }
                }
                PGA.MessengerManager.MessengerManager.AddLog("End AddToDatabase");
                return id;
            }
            //catch (Autodesk.AutoCAD.Runtime.Exception arex)
            //{
            //    //PGA.MessengerManager.MessengerManager.AddLog("Object already in database", arex);
            //    //System.Diagnostics.Debug.WriteLine(databaseObject.GetType().Name + " already in database");
            //}

            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error adding object to the database", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the X data.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="xData">The x data.</param>
        public static void SetXData(Transaction transaction, ObjectId objectId, string applicationName, ResultBuffer xData)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetXData");
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction.IsDisposed) throw new ArgumentException("transaction is disposed", "transaction");

            RegAppTable rat = transaction.GetObject(objectId.Database.RegAppTableId, OpenMode.ForWrite) as RegAppTable;
            if (!rat.Has(applicationName))
            {
                RegAppTableRecord raRec = new RegAppTableRecord();
                raRec.Name = applicationName;
                rat.Add(raRec);
                transaction.AddNewlyCreatedDBObject(raRec, true);
            }

            Entity entity = transaction.GetObject(objectId, OpenMode.ForWrite) as Entity;
            entity.XData = xData;
            PGA.MessengerManager.MessengerManager.AddLog("End SetXData");
        }

        /// <summary>
        /// Gets the X data.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        public static TypedValue[] GetXData(Transaction transaction, ObjectId objectId, string applicationName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetXData");
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction.IsDisposed) throw new ArgumentException("transaction is disposed", "transaction");

            if (objectId.IsNull) throw new NullReferenceException("objectId is null");
            if (!objectId.IsValid) throw new ArgumentNullException("objectId", "objectId is invalid");

            using (Entity entity = transaction.GetObject(objectId, OpenMode.ForRead, true) as Entity)
            {
                if (entity != null)
                {
                    return GetXData(entity, applicationName);
                }
            }
            return null;
        }

        /// <summary>
        /// Get XData from entity for an application
        /// </summary>
        /// <param name="entityId">The object id of entity.</param>
        /// <param name="applicationName">The application name of the XData.</param>
        /// <returns>The XData value or null if not found or error caught</returns>
        public static TypedValue[] GetXData(Entity entity, string applicationName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetXData");
            if (entity == null) throw new NullReferenceException("entity is null");
            if (string.IsNullOrEmpty(applicationName)) throw new NullReferenceException("applicationName is null");

            using (ResultBuffer resBuf = entity.GetXDataForApplication(applicationName))
            {
                if (resBuf != null)
                {
                    return resBuf.AsArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Returns EED for an Entity with a specific applicationname 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="applicationName"></param>
        /// <returns>Empty dictionary if no EED found for application</returns>
        public static Dictionary<string, string> GetEED(Entity entity, string applicationName)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetEED");
            Dictionary<string, string> eedDict = new Dictionary<string, string>();
            TypedValue[] resArr = GetXData(entity, applicationName);

            if (resArr == null || resArr.Length < 3) return eedDict;

            for (int i = 2; i < resArr.Length - 1; i++)
            {
                if (resArr[i].TypeCode.ToString() == "1000")
                {
                    string val = resArr[i].Value.ToString();
                    string[] valArr = val.Split('=');
                    if (eedDict.ContainsKey(valArr[0]) == false)
                        eedDict.Add(valArr[0], valArr[1]);
                    else
                    {
                        eedDict.Remove(valArr[0]);
                        eedDict.Add(valArr[0], valArr[1]);
                    }
                }

            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetEED");
            return eedDict;
        }

        /// <summary>
        /// Set EED.
        /// </summary>
        /// <param name="objId">The object id of the entity.</param>
        /// <param name="eedDict">The key-value pair of the EED data.</param>
        public static void SetEED(Transaction trans, ObjectId objId, Dictionary<string, string> eedDict, string appname)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetEED");
            using (ResultBuffer resBuffer = new ResultBuffer())
            {
                resBuffer.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, appname));
                resBuffer.Add(new TypedValue((int)DxfCode.ExtendedDataControlString, "{"));

                foreach (KeyValuePair<string, string> kvp in eedDict)
                {
                    string val = string.Format("{0}={1}", kvp.Key, kvp.Value);
                    resBuffer.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, val));
                }

                resBuffer.Add(new TypedValue((int)DxfCode.ExtendedDataControlString, "}"));

                SetXData(trans, objId, appname, resBuffer);
            }
            PGA.MessengerManager.MessengerManager.AddLog("End SetEED");
        }

        /// <summary>
        /// Writes a message to the command line
        /// </summary>        
        public static void WriteMessage(string message)
        {
            Editor editor = Active.Editor;
            editor.WriteMessage(message);
        }


        /// <summary>
        /// Sets the display order.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="oids">The object ids.</param>
        /// <param name="sendToTop">The draw order.</param>
        public static void SetDisplayOrder(Transaction transaction, ObjectIdCollection oids, bool sendToTop)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start SetDisplayOrder");
            if (oids == null) throw new NullReferenceException("oids is null");

            // verify the arguments
            //
            if ( oids.Count == 0 )
            {
                return;
            }

            // get the database from the the first objectId
            //
            global::Autodesk.AutoCAD.DatabaseServices.Database db = oids[0].Database;           

            // create a transaction if needed
            //
            Transaction trans = null;
            if (transaction == null)
            {
                trans = db.TransactionManager.StartTransaction();
            }
            else
            {
                trans = transaction;
            }
            
            // now get the draworder table and set accordingly
            //
            using (BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead))
            {
                using (BlockTableRecord btr = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite))
                {
                    using (DrawOrderTable drawOrderTable = trans.GetObject(btr.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable)
                    {
                        if (drawOrderTable != null)
                        {
                            if (sendToTop)
                            {
                                drawOrderTable.MoveToTop(oids);
                            }
                            else
                            {
                                drawOrderTable.MoveToBottom(oids);
                            }
                        }
                    }
                }
            }

            // cleanup transaction if needed
            //
            if (transaction == null)
            {
                trans.Commit();
                trans.Dispose();
            }
            PGA.MessengerManager.MessengerManager.AddLog("End SetDisplayOrder");
        }
    }
}
