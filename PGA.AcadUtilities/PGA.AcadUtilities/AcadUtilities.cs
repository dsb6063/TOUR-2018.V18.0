using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.EditorInput;
using AcadApplication = global::Autodesk.AutoCAD.ApplicationServices.Application;


namespace PGA.AcadUtilities
{
    public class AcadUtilities
    {
        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(AcadUtilities));
       
        #region PointInPolygon

        /// <summary>
        /// Determines if a point is inside a polygon
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="polyPoints"></param>
        /// <param name="outsidePoint"></param>
        /// <param name="includeTouching"></param>
        /// <returns></returns>
        //public static bool IsPointInsidePolygon(GemsPoint2d testPoint, IList<GemsPoint2d> polyPoints, bool includeTouching)
        //{
        //    GemsPoint2d outsidePoint = new GemsPoint2d();
        //    GetOutsidePoint(ref outsidePoint, polyPoints);

        //    GemsPoint2d onePoint = null;
        //    long pointCount = 0;
        //    long intersectionCount = 0;
        //    double howClose = 0;
        //    bool retval = false;

        //    GemsPoint2d startPoint = new GemsPoint2d();
        //    GemsPoint2d endPoint = new GemsPoint2d();
        //    GemsPoint2d nearPoint = new GemsPoint2d();
        //    pointCount = polyPoints.Count;
        //    for (int i = 0; i < polyPoints.Count - 1; i++)
        //    {
        //        onePoint = polyPoints[i];
        //        startPoint.X = onePoint.X;
        //        startPoint.Y = onePoint.Y;
        //        onePoint = polyPoints[i + 1];
        //        endPoint.X = onePoint.X;
        //        endPoint.Y = onePoint.Y;
        //        if ((includeTouching == true))
        //        {
        //            howClose = DistanceToSegment(testPoint, startPoint, endPoint, ref nearPoint);
        //            if (((howClose >= 0) && (howClose <= 1E-08)))
        //            {
        //                retval = true;
        //                break;
        //            }
        //        }
        //        if ((retval == false))
        //        {
        //            if ((DoSegmentsIntersect(startPoint, endPoint, outsidePoint, testPoint) == true))
        //            {
        //                // Check that testpoint is not on the boundary line
        //                if ((includeTouching == true))
        //                {
        //                    if ((DistanceToSegment(testPoint, startPoint, endPoint, ref nearPoint) > 0))
        //                    {
        //                        intersectionCount = (intersectionCount + 1);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if ((retval == false))
        //    {
        //        if (((intersectionCount % 2) == 1))
        //        {
        //            retval = true;
        //        }
        //    }
        //    return retval;
        //}

        /// <summary>
        /// Gets a point outside of a polygon
        /// </summary>
        /// <param name="outsidePoint"></param>
        /// <param name="polyPoints"></param>
        /// <returns></returns>
        //private static bool GetOutsidePoint(ref GemsPoint2d outsidePoint, IList<GemsPoint2d> polyPoints)
        //{
        //    GemsPoint2d minPoint = CoordinateService.GetMinFromPolygon(polyPoints);
        //    outsidePoint.X = minPoint.X - 10;
        //    outsidePoint.Y = minPoint.Y - 10;
        //    return true;
        //}

        /// <summary>
        /// Determines if two vectors intersect
        /// </summary>
        /// <param name="startPoint1"></param>
        /// <param name="endPoint1"></param>
        /// <param name="startPoint2"></param>
        /// <param name="endPoint2"></param>
        /// <returns></returns>
        //private static bool DoSegmentsIntersect(GemsPoint2d startPoint1, GemsPoint2d endPoint1, GemsPoint2d startPoint2, GemsPoint2d endPoint2)
        //{
        //    bool retval = false;
        //    double deltaX1 = (endPoint1.X - startPoint1.X);
        //    double deltaY1 = (endPoint1.Y - startPoint1.Y);
        //    double deltaX2 = (endPoint2.X - startPoint2.X);
        //    double deltaY2 = (endPoint2.Y - startPoint2.Y);
        //    if ((((deltaX2 * deltaY1) - (deltaY2 * deltaX1)) == 0))
        //    {
        //        //  The segments are parallel.
        //        return false;
        //    }

        //    double s = (((deltaX1 * (startPoint2.Y - startPoint1.Y)) + (deltaY1 * (startPoint1.X - startPoint2.X))) / ((deltaX2 * deltaY1) - (deltaY2 * deltaX1)));
        //    double t = (((deltaX2 * (startPoint1.Y - startPoint2.Y)) + (deltaY2 * (startPoint2.X - startPoint1.X))) / ((deltaY2 * deltaX1) - (deltaX2 * deltaY1)));

        //    if ((s >= 0.0) && (s <= 1.0) && (t >= 0.0) && (t <= 1.0))
        //    {
        //        retval = true;
        //    }
        //    return retval;
        //}

        /// <summary>
        /// Calculates the distance between a point and a segment.
        /// Note that nearpoint is returned as well as distance
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="nearPoint"></param>
        /// <returns></returns>
        //private static double DistanceToSegment(GemsPoint2d testPoint, GemsPoint2d startPoint, GemsPoint2d endPoint, ref GemsPoint2d nearPoint)
        //{
        //    double t;
        //    double deltaX = (endPoint.X - startPoint.X);
        //    double deltaY = (endPoint.Y - startPoint.Y);
        //    if (((deltaX == 0) && (deltaY == 0)))
        //    {
        //        //  It's a point not a line segment.
        //        deltaX = (testPoint.X - startPoint.X);
        //        deltaY = (testPoint.Y - startPoint.Y);
        //        nearPoint.X = startPoint.X;
        //        nearPoint.Y = startPoint.Y;
        //        return Math.Sqrt(((deltaX * deltaX) + (deltaY * deltaY)));
        //    }
        //    //  Calculate the t that minimizes the distance.
        //    t = ((((testPoint.X - startPoint.X) * deltaX) + ((testPoint.Y - startPoint.Y) * deltaY)) / ((deltaX * deltaX) + (deltaY * deltaY)));
        //    if ((t < 0))
        //    {
        //        deltaX = (testPoint.X - startPoint.X);
        //        deltaY = (testPoint.Y - startPoint.Y);
        //        nearPoint.X = startPoint.X;
        //        nearPoint.Y = startPoint.Y;
        //    }
        //    else if ((t > 1))
        //    {
        //        deltaX = (testPoint.X - endPoint.X);
        //        deltaY = (testPoint.Y - endPoint.Y);
        //        nearPoint.X = endPoint.X;
        //        nearPoint.Y = endPoint.Y;
        //    }
        //    else
        //    {
        //        nearPoint.X = (startPoint.X + (t * deltaX));
        //        nearPoint.Y = (startPoint.Y + (t * deltaY));
        //        deltaX = (testPoint.X - nearPoint.X);
        //        deltaY = (testPoint.Y - nearPoint.Y);
        //    }
        //    return Math.Sqrt(((deltaX * deltaX) + (deltaY * deltaY)));
        //}

        #endregion

        /// <summary>
        /// Passes command string to the current document
        /// </summary>
        /// <param name="acadCmd"></param>
        public static void SendStringToExecute(string acadCmd)
        { 
            try
            {
                Document currentDoc = Application.DocumentManager.MdiActiveDocument;
                currentDoc.SendStringToExecute(acadCmd, true, false, false);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
               PGA.MessengerManager.MessengerManager.AddLog("Failed to send command string tothe command line: ", ex);
                throw;
            }
        }

        /// <summary>
        /// Use COM to ZoomAll
        /// </summary>
        /// <returns></returns>
        public static bool ZoomAll()
        {
            bool retval = false;

            try
            {
                // Access the COM Preferences object  
                Autodesk.AutoCAD.Interop.AcadApplication acadApp = (Autodesk.AutoCAD.Interop.AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
                // your code to draw geometries, or something else
                acadApp.ZoomAll();
                retval = true;
            }
            catch (System.Exception ex)
            {   
               PGA.MessengerManager.MessengerManager.AddLog("Failed to zoom active window ", ex);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Use COM to ZoomExtents
        /// </summary>
        /// <returns></returns>
        public static bool ZoomExtents()
        {
            bool retval = false;

            try
            {
                // Access the COM Preferences object  
                Autodesk.AutoCAD.Interop.AcadApplication acadApp = (Autodesk.AutoCAD.Interop.AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
                // your code to draw geometries, or something else
                acadApp.ZoomExtents();
                retval = true;
            }
            catch (System.Exception ex)
            {
               PGA.MessengerManager.MessengerManager.AddLog("Failed to zoom active window ", ex);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Gets the name of the active profile.
        /// </summary>
        /// <returns></returns>
        public static string GetActiveProfileName()
        {   
            string retval = String.Empty;

            try
            {
                AcadPreferences acPrefComObj = (AcadPreferences)Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
                string sActiveProfile = acPrefComObj.Profiles.ActiveProfile;
                retval = sActiveProfile;
            }
            catch (System.Exception ex)
            {   
               PGA.MessengerManager.MessengerManager.AddLog("Failed to get AutoCAD Active Profile ", ex);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Creates a dwg file on disk and returns a database, the user is responible for disposing the returned database
        /// </summary>
        /// <param name="path"></param>
        /// <param name="IsReadOnly"></param>
        /// <returns></returns>
        public static Autodesk.AutoCAD.DatabaseServices.Database CreateSideDatabase(string fullFileNameAndPath, bool isReadOnly)
        {
            Autodesk.AutoCAD.DatabaseServices.Database db = null;
            try
            {
                db = new Autodesk.AutoCAD.DatabaseServices.Database(false, true);
            }
            catch (System.Exception )
            {
                db.Dispose();
                db = null;
                throw;

            }

            return db;
        }

        /// <summary>
        /// Opens a dwg file on disk and returns a database, the user is responible for disposing the returned database
        /// </summary>
        /// <param name="path"></param>
        /// <param name="IsReadOnly"></param>
        /// <returns></returns>
        public static Autodesk.AutoCAD.DatabaseServices.Database LoadSideDatabase(string fullFileNameAndPath, bool isReadOnly)
        {
            Autodesk.AutoCAD.DatabaseServices.Database db = null;
            try
            {
                db = new Autodesk.AutoCAD.DatabaseServices.Database(false, true);
                if (isReadOnly == true)
                {
                    db.ReadDwgFile(fullFileNameAndPath, System.IO.FileShare.Read, false, String.Empty);
                }
                else
                {
                    db.ReadDwgFile(fullFileNameAndPath, System.IO.FileShare.ReadWrite, false, String.Empty);
                }
            }
            catch (System.Exception exception)
            {
                // "\nUnable to read drawing file."
                db.Dispose();
                db = null;
               PGA.MessengerManager.MessengerManager.AddLog("Failed to LoadSideDatabase", exception);
                throw;

            }

            return db;
        }

        /// <summary>
        /// Function overwrites symbol table entries in the targetDb with symbol table entries in the sourceDb
        /// Table entries with the matching names are processed, all others are skipped
        /// Name match is not case sensitive
        /// </summary>
        /// <param name="sourceDb"></param>
        /// <param name="targetDb"></param>
        /// <returns></returns>
        /// <remarks>Use LoadSideDatabase to open the sourceDb as ReadOnly</remarks>
        /// <remarks>Use LoadSideDatabase to open the targetDb as ReadWrite</remarks>
        public static bool MergeSymbolTables(Autodesk.AutoCAD.DatabaseServices.Database sourceDb, Autodesk.AutoCAD.DatabaseServices.Database targetDb, ref bool isDirty)
        {

            string strActivity = String.Empty;
            bool retval = false;

            try
            {

                ObjectIdCollection acObjIdColl = new ObjectIdCollection();

                // Define these up here - outside the transaction so they will be valid on the next database
                ObjectIdCollection sourceDimStyleIds = new ObjectIdCollection();
                List<string> sourceDimStyleNames = new List<string>();

                ObjectIdCollection sourceLayerIds = new ObjectIdCollection();
                List<string> sourceLayerNames = new List<string>();

                ObjectIdCollection sourceLinetypeIds = new ObjectIdCollection();
                List<string> sourceLinetypeNames = new List<string>();

                ObjectIdCollection sourceTextStyleIds = new ObjectIdCollection();
                List<string> sourceTextStyleNames = new List<string>();

                strActivity = "Starting transaction on the sourceDb";
                using (Transaction trans = sourceDb.TransactionManager.StartTransaction())
                {

                    // ************** //
                    // DimStyle Table
                    // ************** //

                    strActivity = "Reading all DimStyle objects in the sourceDb";

                    // Get all source symbol entry names and Object Ids into a List (open for Read)
                    DimStyleTable acDimStyleTableSource;
                    acDimStyleTableSource = trans.GetObject(sourceDb.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;

                    string dimStyleName = String.Empty;
                    DimStyleTableRecord acDimStyleTableRecSource = null;
                    foreach (ObjectId oneObjectId in acDimStyleTableSource)
                    {
                        acDimStyleTableRecSource = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as DimStyleTableRecord;
                        dimStyleName = acDimStyleTableRecSource.Name.ToUpper();
                        sourceDimStyleNames.Add(dimStyleName);
                        sourceDimStyleIds.Add(acDimStyleTableRecSource.ObjectId);
                        acDimStyleTableRecSource.Dispose();
                    }

                    // *********** //
                    // Layer Table
                    // *********** //

                    strActivity = "Reading all Layer objects in the sourceDb";

                    // Get all source symbol entry names and Object Ids into a List (open for Read)
                    LayerTable acLayerTableSource;
                    acLayerTableSource = trans.GetObject(sourceDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                    string layerName = String.Empty;
                    LayerTableRecord acLayerTableRecSource = null;
                    foreach (ObjectId oneObjectId in acLayerTableSource)
                    {
                        acLayerTableRecSource = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as LayerTableRecord;
                        layerName = acLayerTableRecSource.Name.ToUpper();
                        sourceLayerNames.Add(layerName);
                        sourceLayerIds.Add(acLayerTableRecSource.ObjectId);
                        acLayerTableRecSource.Dispose();
                    }

                    // *************** //
                    // Linetype Table
                    // *************** //

                    strActivity = "Reading all Linetype objects in the sourceDb";

                    // Get all source symbol entry names and Object Ids into a List (open for Read)
                    LinetypeTable acLinetypeTableSource;
                    acLinetypeTableSource = trans.GetObject(sourceDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                    string linetypeName = String.Empty;
                    LinetypeTableRecord acLinetypeTableRecSource = null;
                    foreach (ObjectId oneObjectId in acLinetypeTableSource)
                    {
                        acLinetypeTableRecSource = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as LinetypeTableRecord;
                        linetypeName = acLinetypeTableRecSource.Name.ToUpper();
                        sourceLinetypeNames.Add(linetypeName);
                        sourceLinetypeIds.Add(acLinetypeTableRecSource.ObjectId);
                        acLinetypeTableRecSource.Dispose();
                    }

                    // *************** //
                    // TextStyle Table
                    // *************** //

                    strActivity = "Reading all TextStyle objects in the sourceDb";

                    // Get all source symbol entry names and Object Ids into a List (open for Read)
                    TextStyleTable acTextStyleTableSource;
                    acTextStyleTableSource = trans.GetObject(sourceDb.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                    string textStyleName = String.Empty;
                    TextStyleTableRecord acTextStyleTableRecSource = null;
                    foreach (ObjectId oneObjectId in acTextStyleTableSource)
                    {
                        acTextStyleTableRecSource = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as TextStyleTableRecord;
                        textStyleName = acTextStyleTableRecSource.Name.ToUpper();
                        sourceTextStyleNames.Add(textStyleName);
                        sourceTextStyleIds.Add(acTextStyleTableRecSource.ObjectId);
                        acTextStyleTableRecSource.Dispose();
                    }

                }

                // Start a transaction on the Target Db

                // ************** //
                // DimStyle Table
                // ************** //
                using (Transaction trans = targetDb.TransactionManager.StartTransaction())
                {

                    // Collection of DimStyle Ids from the sourceDb that match DimStyle Names in the targetDb
                    ObjectIdCollection cloneDimStyleIds = new ObjectIdCollection();

                    // Get all target symbol entry names and Object Ids into a List (open for Read)
                    DimStyleTable acDimStyleTableTarget;
                    acDimStyleTableTarget = trans.GetObject(targetDb.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;

                    string dimStyleName = String.Empty;
                    int dimStyleIndex = -1;
                    DimStyleTableRecord acDimStyleTableRecTarget = null;
                    foreach (ObjectId oneObjectId in acDimStyleTableTarget)
                    {
                        dimStyleIndex = -1;
                        acDimStyleTableRecTarget = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as DimStyleTableRecord;
                        dimStyleName = acDimStyleTableRecTarget.Name.ToUpper();
                        dimStyleIndex = sourceDimStyleNames.IndexOf(dimStyleName);
                        if (dimStyleIndex >= 0)
                        {
                            cloneDimStyleIds.Add(sourceDimStyleIds[dimStyleIndex]);
                        }
                        acDimStyleTableRecTarget.Dispose();
                    }

                    // Clone the objects to the Target database
                    if (cloneDimStyleIds.Count > 0)
                    {
                        IdMapping acIdMapDimStyle = new IdMapping();
                        sourceDb.WblockCloneObjects(cloneDimStyleIds, acDimStyleTableTarget.ObjectId, acIdMapDimStyle, DuplicateRecordCloning.Replace, false);
                        trans.Commit();
                        isDirty = true;
                    }
                }

                // *********** //
                // Layer Table
                // *********** //
                using (Transaction trans = targetDb.TransactionManager.StartTransaction())
                {

                    // Collection of Layer Ids from the sourceDb that match Layer Names in the targetDb
                    ObjectIdCollection cloneLayerIds = new ObjectIdCollection();

                    // Get all target symbol entry names and Object Ids into a List (open for Read)
                    LayerTable acLayerTableTarget;
                    acLayerTableTarget = trans.GetObject(targetDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                    string layerName = String.Empty;
                    int layerIndex = -1;
                    LayerTableRecord acLayerTableRecTarget = null;
                    foreach (ObjectId oneObjectId in acLayerTableTarget)
                    {
                        layerIndex = -1;
                        acLayerTableRecTarget = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as LayerTableRecord;
                        layerName = acLayerTableRecTarget.Name.ToUpper();
                        layerIndex = sourceLayerNames.IndexOf(layerName);
                        if (layerIndex >= 0)
                        {
                            cloneLayerIds.Add(sourceLayerIds[layerIndex]);
                        }
                        acLayerTableRecTarget.Dispose();
                    }

                    // Clone the objects to the Target database
                    if (cloneLayerIds.Count > 0)
                    {
                        IdMapping acIdMapLayer = new IdMapping();
                        sourceDb.WblockCloneObjects(cloneLayerIds, acLayerTableTarget.ObjectId, acIdMapLayer, DuplicateRecordCloning.Replace, false);
                        trans.Commit();
                        isDirty = true;
                    }
                }

                // ************** //
                // Linetype Table
                // ************** //
                using (Transaction trans = targetDb.TransactionManager.StartTransaction())
                {
                    // Collection of Linetype Ids from the sourceDb that match Linetype Names in the targetDb
                    ObjectIdCollection cloneLinetypeIds = new ObjectIdCollection();

                    // Get all target symbol entry names and Object Ids into a List (open for Read)
                    LinetypeTable acLinetypeTableTarget;
                    acLinetypeTableTarget = trans.GetObject(targetDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                    string linetypeName = String.Empty;
                    int linetypeIndex = -1;
                    LinetypeTableRecord acLinetypeTableRecTarget = null;
                    foreach (ObjectId oneObjectId in acLinetypeTableTarget)
                    {
                        linetypeIndex = -1;
                        acLinetypeTableRecTarget = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as LinetypeTableRecord;
                        linetypeName = acLinetypeTableRecTarget.Name.ToUpper();
                        linetypeIndex = sourceLinetypeNames.IndexOf(linetypeName);
                        if (linetypeIndex >= 0)
                        {
                            cloneLinetypeIds.Add(sourceLinetypeIds[linetypeIndex]);
                        }
                        acLinetypeTableRecTarget.Dispose();
                    }

                    // Clone the objects to the Target database
                    if (cloneLinetypeIds.Count > 0)
                    {
                        IdMapping acIdMapLinetype = new IdMapping();
                        sourceDb.WblockCloneObjects(cloneLinetypeIds, acLinetypeTableTarget.ObjectId, acIdMapLinetype, DuplicateRecordCloning.Replace, false);
                        trans.Commit();
                        isDirty = true;
                    }
                }

                // *************** //
                // TextStyle Table
                // *************** //
                using (Transaction trans = targetDb.TransactionManager.StartTransaction())
                {
                    // Collection of TextStyle Ids from the sourceDb that match TextStyle Names in the targetDb
                    ObjectIdCollection cloneTextStyleIds = new ObjectIdCollection();

                    // Get all target symbol entry names and Object Ids into a List (open for Read)
                    TextStyleTable acTextStyleTableTarget;
                    acTextStyleTableTarget = trans.GetObject(targetDb.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                    string textStyleName = String.Empty;
                    int textStyleIndex = -1;
                    TextStyleTableRecord acTextStyleTableRecTarget = null;
                    foreach (ObjectId oneObjectId in acTextStyleTableTarget)
                    {
                        textStyleIndex = -1;
                        acTextStyleTableRecTarget = trans.GetObject(oneObjectId, OpenMode.ForRead, false, true) as TextStyleTableRecord;
                        textStyleName = acTextStyleTableRecTarget.Name.ToUpper();

                        //*****************************************************************//
                        //Text styles with a blank name reference shape (shp/shx)
                        //Cloning is based on text style name. This is a problem as there can be multiple text styles derived from shapes. All will have a blank name.
                        //Cloning one or all un-named text styles crosses wires and corrupts/incorrectly overwrites the shape definitions
                        //This manifests as a corrupted linetype as complex linetypes may reference shapes
                        //Rather than just checking if this is a shape derived textstyle, check to verify the name is not blank bofore cloning.
                        //if (acTextStyleTableRecTarget.IsShapeFile == false)
                        //*****************************************************************//
                        if (textStyleName != String.Empty)
                        {
                            textStyleIndex = sourceTextStyleNames.IndexOf(textStyleName);
                            if (textStyleIndex >= 0)
                            {
                                cloneTextStyleIds.Add(sourceTextStyleIds[textStyleIndex]);
                            }
                        }
                        //*****************************************************************//

                        acTextStyleTableRecTarget.Dispose();
                    }

                    // Clone the objects to the Target database
                    if (cloneTextStyleIds.Count > 0)
                    {
                        IdMapping acIdMapTextStyle = new IdMapping();
                        sourceDb.WblockCloneObjects(cloneTextStyleIds, acTextStyleTableTarget.ObjectId, acIdMapTextStyle, DuplicateRecordCloning.Replace, false);
                        trans.Commit();
                        isDirty = true;
                    }
                }

                retval = true;

            }
            catch (System.Exception ex)
            {   
               PGA.MessengerManager.MessengerManager.AddLog("Failed to merge symbol tables", ex);
                throw;
            }

            return retval;
        }

        /// <summary>
        /// Converts the to 2d.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <returns></returns>
        public static Point2dCollection ConvertTo2d(Point3dCollection pt)
        {
            Vector3d normal = new Vector3d(0, 0, 1);
            Plane plane = new Plane(new Point3d(0, 0, 0), normal);
            Point2dCollection collection= new Point2dCollection();
            foreach (Point3d p in pt)
            {
                collection.Add(ConvertTo2d(p));
            }
            return collection;
        }
        /// <summary>
        /// Converts the to 2d.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <returns></returns>
        public static Point2d ConvertTo2d(Point3d pt)
        {
            Vector3d normal = new Vector3d(0, 0, 1);
            Plane plane = new Plane(new Point3d(0, 0, 0), normal);
            return pt.Convert2d(plane);
        }

        /// <summary>
        /// Converts the to 2d.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <returns></returns>
        public static Point2d ConvertTo2d(Point3d pt, Matrix3d ucs)
        {
            Vector3d normal = new Vector3d(0, 0, 1);
            normal = normal.TransformBy(ucs);
            Plane plane = new Plane(new Point3d(0, 0, 0), normal);
            return pt.Convert2d(plane);
        }

        /// <summary>
        /// Converts the to3d collection.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Point3dCollection ConvertTo3dCollection(Point2dCollection points)
        {
            return ConvertTo3dCollection(points, 0.0);
        }

        /// <summary>
        /// Converts the to3d collection.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="zValue">The z value.</param>
        /// <returns></returns>
        public static Point3dCollection ConvertTo3dCollection(Point2dCollection points, double zValue)
        {
            Point3dCollection points3d = new Point3dCollection();
            foreach (Point2d pt in points)
            {
                points3d.Add(new Point3d(pt.X, pt.Y, zValue));
            }
            return points3d;
        }

        /// <summary>
        /// Converts the to3d collection.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        //public static Point3dCollection ConvertTo3dCollection(IList<GemsPoint2d> points)
        //{
        //    return ConvertTo3dCollection(points, 0.0);
        //}

        /// <summary>
        /// Converts the to3d collection.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        //public static Point2dCollection ConvertTo2dCollection(IList<GemsPoint2d> points)
        //{
        //    Point2dCollection points2d = new Point2dCollection(points.Count);
        //    foreach (GemsPoint2d pt in points)
        //    {
        //        points2d.Add(new Point2d(pt.X, pt.Y));
        //    }
        //    return points2d;
        //}

        /// <summary>
        /// Converts the to3d collection.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="zValue">The z value.</param>
        /// <returns></returns>
        //public static Point3dCollection ConvertTo3dCollection(IList<GemsPoint2d> points, double zValue)
        //{
        //    Point3dCollection points3d = new Point3dCollection();
        //    foreach (GemsPoint2d pt in points)
        //    {
        //        points3d.Add(new Point3d(pt.X, pt.Y, zValue));
        //    }
        //    return points3d;
        //}

        /// <summary>
        /// Gets the hatch mid point.
        /// </summary>
        /// <param name="hatch">The hatch.</param>
        /// <returns></returns>
        public static Point3d GetHatchMidPoint(Hatch hatch)
        {
            // get the middle of the extents
            //
            double midPointX = hatch.GeometricExtents.MinPoint.X + ((hatch.GeometricExtents.MaxPoint.X - hatch.GeometricExtents.MinPoint.X) * 0.5);
            double midPointY = hatch.GeometricExtents.MinPoint.Y + ((hatch.GeometricExtents.MaxPoint.Y - hatch.GeometricExtents.MinPoint.Y) * 0.5);

            return new Point3d(midPointX, midPointY, hatch.GeometricExtents.MaxPoint.Z);
        }

        /// <summary>
        /// Gets the center point of a rectangle.
        /// </summary>
        /// <param name="hatch">The hatch.</param>
        /// <returns></returns>
        public static Point2d GetCentriod(Point2dCollection points)
        {
            if (points.Count != 4)
            {
                // throw new ArgumentException("points is not of length 4");
            }

            double? minX = null;
            double? minY = null;
            double? maxX = null;
            double? maxY = null;

            foreach (Point2d pt in points)
            {
                if (minX.HasValue)
                {
                    if (pt.X < minX)
                    {
                        minX = pt.X;
                    }
                }
                else
                {
                    minX = pt.X;
                }

                if (minY.HasValue)
                {
                    if (pt.Y < minY)
                    {
                        minY = pt.Y;
                    }
                }
                else
                {
                    minY = pt.Y;
                }

                if (maxX.HasValue)
                {
                    if (pt.X > maxX)
                    {
                        maxX = pt.X;
                    }
                }
                else
                {
                    maxX = pt.X;
                }

                if (maxY.HasValue)
                {
                    if (pt.Y > maxY)
                    {
                        maxY = pt.Y;
                    }
                }
                else
                {
                    maxY = pt.Y;
                }
            }

            // get the middle
            //
            double? midPointX = minX + ((maxX - minX) * 0.5);
            double? midPointY = minY + ((maxY - minY) * 0.5);

            return new Point2d((double)midPointX, (double)midPointY);
        }

        /// <summary>
        /// Gets the window selection.
        /// </summary>
        /// <returns></returns>
        public static Point3d[] GetWindowSelection()
        {
            Point3d[] selectedPoints = null;
            Document currentDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = currentDoc.Editor;

            PromptPointResult ppr1 = editor.GetPoint("\nSelect first corner of window: ");
            if (ppr1.Status != PromptStatus.OK)
                return null;

            PromptPointResult ppr2 = editor.GetCorner("\nSelect opposite corner of window: ", ppr1.Value);
            if (ppr2.Status != PromptStatus.OK)
                return null;

            selectedPoints = new Point3d[2];
            selectedPoints[0] = ppr1.Value;
            selectedPoints[1] = ppr2.Value;

            return selectedPoints;
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
        /// Writes a message to the command line
        /// </summary>        
        public static Autodesk.AutoCAD.Windows.Window AcadWindow
        {
            get
            {
                return AcadApplication.MainWindow;
            }
        }

        /// <summary>
        /// Writes a message to the command line
        /// </summary>        
        public static void WriteMessage(string message)
        {
            Editor editor = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage(message);
        }

        /// <summary>
        /// Highlights the entities.
        /// </summary>
        /// <param name="highlight">if set to <c>true</c> [highlight].</param>
        /// <param name="oids">The oids.</param>
        public static void HighlightEntities(bool highlight, ObjectIdCollection oids)
        {
            if (oids == null) throw new NullReferenceException("oids is null");

            // verify the arguments
            //
            if (oids.Count == 0)
            {
                return;
            }

            // get the database from the the first objectId
            //
           Autodesk.AutoCAD.DatabaseServices.Database db = oids[0].Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                HighlightEntities(trans, highlight, oids);
                trans.Commit();
            }
        }

        /// <summary>
        /// Highlights the entities.
        /// </summary>
        /// <param name="trans">The trans.</param>
        /// <param name="highlight">if set to <c>true</c> [highlight].</param>
        /// <param name="oids">The oids.</param>
        public static void HighlightEntities(Transaction trans, bool highlight, ObjectIdCollection oids)
        {
            if (oids == null) throw new NullReferenceException("oids is null");

            // verify the arguments
            //
            if (oids.Count == 0)
            {
                return;
            }

            // get the database from the the first objectId
            //
           Autodesk.AutoCAD.DatabaseServices.Database db = oids[0].Database;

            // create a transaction if needed
            //
            Transaction transaction = null;
            bool transCreated = false;
            if (trans == null)
            {
                transaction = db.TransactionManager.StartTransaction();
                transCreated = true;
            }
            else
            {
                transaction = trans;
            }

            foreach (ObjectId id in oids)
            {
                if (!id.IsErased)
                {
                    Entity ent = transaction.GetObject(id, OpenMode.ForRead) as Entity;
                    if (ent != null)
                    {
                        if (highlight)
                        {
                            ent.Highlight();
                        }
                        else
                        {
                            ent.Unhighlight();
                        }
                    }
                }
            }

            if (transCreated)
            {
                transaction.Commit();
                transaction.Dispose();
            }
        }

        /// <summary>
        /// Highlights the entity.
        /// </summary>
        /// <param name="highlight">if set to <c>true</c> [highlight].</param>
        public static void HighlightEntity(Transaction trans, bool highlight, ObjectId id)
        {
            using (ObjectIdCollection oids = new ObjectIdCollection())
            {
                oids.Add(id);
                HighlightEntities(trans, highlight, oids);
            }
        }

        /// <summary>
        /// Sets the display order.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="oids">The object ids.</param>
        /// <param name="sendToTop">The draw order.</param>
        public static void SetDisplayOrder(Transaction transaction, ObjectIdCollection oids, bool sendToTop)
        {
            if (oids == null) throw new NullReferenceException("oids is null");

            // verify the arguments
            //
            if (oids.Count == 0)
            {
                return;
            }

            // get the database from the the first objectId
            //
           Autodesk.AutoCAD.DatabaseServices.Database db = oids[0].Database;

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
        }

        /// <summary>
        /// Deletes the objects.
        /// </summary>
        /// <param name="oids">The oids.</param>
        static public void DeleteObjects(ObjectIdCollection oids)
        {
           Autodesk.AutoCAD.DatabaseServices.Database db = AcadApplication.DocumentManager.MdiActiveDocument.Database;
            DeleteObjects(db, oids);
        }

        /// <summary>
        /// Deletes the objects.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="oids"></param>
        static public void DeleteObjects(Autodesk.AutoCAD.DatabaseServices.Database db, ObjectIdCollection oids)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DeleteObjects(db, trans, oids);
                trans.Commit();
            }
        }

        /// <summary>
        /// Deletes the objects.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="oids"></param>
        static public void DeleteObjects(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, ObjectIdCollection oids)
        {
            foreach (ObjectId id in oids)
            {
                if (id.IsValid && !id.IsErased)
                {
                    DBObject obj = trans.GetObject(id, OpenMode.ForWrite);
                    obj.Erase();
                    //trans.TransactionManager.QueueForGraphicsFlush();
                   
                }
            }
        }

        /// <summary>
        /// Copies the objects.
        /// </summary>
        /// <param name="oids">The oids.</param>
        static public ObjectIdCollection CopyObjects(ObjectIdCollection oids)
        {
            ObjectIdCollection retVal = new ObjectIdCollection();
           Autodesk.AutoCAD.DatabaseServices.Database db = AcadApplication.DocumentManager.MdiActiveDocument.Database;
            retVal = CopyObjects(db, oids);
            return retVal;
        }

        /// <summary>
        /// Copies the objects.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="oids"></param>
        static public ObjectIdCollection CopyObjects(Autodesk.AutoCAD.DatabaseServices.Database db, ObjectIdCollection oids)
        {
            ObjectIdCollection retVal = new ObjectIdCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                retVal = CopyObjects(db, trans, oids);
                trans.Commit();
            }
            return retVal;
        }

        /// <summary>
        /// Copies the objects.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="oids"></param>
        static public ObjectIdCollection CopyObjects(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, ObjectIdCollection oids)
        {
            ObjectIdCollection retVal = new ObjectIdCollection();
            foreach (ObjectId id in oids)
            {
                if (!id.IsErased)
                {
                    DBObject obj = trans.GetObject(id, OpenMode.ForRead);
                    Entity cloneEnt = obj.Clone() as Entity;
                    ObjectId addedId = AcadDatabaseManager.AddToDatabase(db, cloneEnt, trans);
                    retVal.Add(addedId);
                }
            }
            return retVal;
         }

            /// <summary>
            /// Zooms to extents.
            /// </summary>
            /// <param name="oids">The oids.</param>
            static public void ZoomToObjectExtents(ObjectIdCollection oids)
        {
           Autodesk.AutoCAD.DatabaseServices.Database db = AcadApplication.DocumentManager.MdiActiveDocument.Database;
            Extents2d drawingExtents = new Extents2d(0, 0, 0, 0);

            Point3d ptMin3d = (Point3d)AcadApplication.GetSystemVariable("VSMIN");
            Point3d ptMax3d = (Point3d)AcadApplication.GetSystemVariable("VSMAX");

            double minX = ptMax3d.X;
            double minY = ptMax3d.Y;
            double maxX = ptMin3d.X;
            double maxY = ptMin3d.Y;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                bool isFirstEnt = true;
                foreach (ObjectId id in oids)
                {
                    if (!id.IsErased)
                    {
                        if (id.ObjectClass.Name != "AcDbPolyline")
                        {
                            continue;
                        }
                        Entity ent = trans.GetObject(id, OpenMode.ForRead) as Entity;
                        if (ent != null)
                        {
                            try
                            {
                                Extents3d extents = ent.GeometricExtents;
                                Extents2d entExtents = new Extents2d(extents.MinPoint.X, extents.MinPoint.Y, extents.MaxPoint.X, extents.MaxPoint.Y);

                                if (isFirstEnt == true)
                                {
                                    minY = entExtents.MinPoint.Y;
                                    minX = entExtents.MinPoint.X;
                                    maxX = entExtents.MaxPoint.X;
                                    maxY = entExtents.MaxPoint.Y;
                                    isFirstEnt = false;
                                }
                                else
                                {

                                    if (entExtents.MinPoint.X < minX)
                                    {
                                        minX = entExtents.MinPoint.X;
                                    }
                                    if (entExtents.MinPoint.Y < minY)
                                    {
                                        minY = entExtents.MinPoint.Y;
                                    }

                                    if (entExtents.MaxPoint.X > maxX)
                                    {
                                        maxX = entExtents.MaxPoint.X;
                                    }
                                    if (entExtents.MaxPoint.Y > maxY)
                                    {
                                        maxY = entExtents.MaxPoint.Y;
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            Point2d minPt = new Point2d(minX, minY);
            Point2d maxPt = new Point2d(maxX, maxY);

            ZoomWin(AcadApplication.DocumentManager.MdiActiveDocument.Editor, minPt, maxPt);
        }

        /// <summary>
        /// Zooms the win.
        /// </summary>
        /// <param name="ed">The ed.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        private static void ZoomWin(Editor ed, Point3d min, Point3d max)
        {
            Editor editor = AcadApplication.DocumentManager.MdiActiveDocument.Editor;

            Point2d min2d = new Point2d(min.X, min.Y);
            Point2d max2d = new Point2d(max.X, max.Y);

            using (ViewTableRecord view = new ViewTableRecord())
            {
                view.CenterPoint = min2d + ((max2d - min2d) / 2.0);
                view.Height = max2d.Y - min2d.Y;
                view.Width = max2d.X - min2d.X;
                editor.SetCurrentView(view);
                editor.UpdateScreen();
            }
        }

        /// <summary>
        /// Zooms the win.
        /// </summary>
        /// <param name="ed">The ed.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        private static void ZoomWin(Editor ed, Point2d min, Point2d max)
        {
            Editor editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (ViewTableRecord view = new ViewTableRecord())
            {
                view.CenterPoint = min + ((max - min) / 2.0);
                view.Height = max.Y - min.Y;
                view.Width = max.X - min.X;
                editor.SetCurrentView(view);
                editor.UpdateScreen();
            }
        }

        /// <summary>
        /// Gets the points from polyline.
        /// </summary>
        /// <param name="pline">The pline.</param>
        /// <returns></returns>
        public static Point2dCollection GetPointsFromPolyline(Polyline pline)
        {
            Point2dCollection points = new Point2dCollection();
            for (int i = 0; i < pline.NumberOfVertices; i++)
            {
                points.Add(pline.GetPoint2dAt(i));
            }
            return points;
        }

        /// <summary>
        /// Gets the points from polyline.
        /// </summary>
        /// <param name="pline">The pline.</param>
        /// <returns></returns>
        public static Point2dCollection GetPointsFromPolyline(ObjectId objectId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
           Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
           
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBObject obj =
                    tr.GetObject(objectId, OpenMode.ForRead);
                Polyline pline = obj as Polyline;

                if (pline != null)
                {
                    Point2dCollection points = new Point2dCollection();
                    for (int i = 0; i < pline.NumberOfVertices; i++)
                    {
                        points.Add(pline.GetPoint2dAt(i));
                    }
                    return points;
                }
                return null;
            }

        }

        /// <summary>
        /// Compares the areas from polylines.
        /// </summary>
        /// <param name="objectId1">The object id1.</param>
        /// <param name="objectId2">The object id2.</param>
        /// <returns><c>true</c> if percent less than 5.0 <<c>false</c> otherwise.</returns>
        public static bool CompareAreasFromPolylines(ObjectId objectId1,ObjectId objectId2)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
           Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBObject obj1 =
                    tr.GetObject(objectId1, OpenMode.ForRead);
                Polyline pline1 = obj1 as Polyline;

                DBObject obj2 =
                   tr.GetObject(objectId1, OpenMode.ForRead);
                Polyline pline2 = obj2 as Polyline;

                if (pline1 != null && pline2 !=null)
                {
                    var diff    =  Math.Abs(pline1.Area - pline2.Area);
                    var percent = (diff / pline1.Area)*100;

                    if (percent < 5.0)
                        return true;
                }
                return false;
            }

        }

        /// <summary>
        /// Purges all layers matching the filter passed
        /// Note, setting useWildCard to true will append a "*" to the end of layerFilter
        /// </summary>
        /// <param name="layerFilter"></param>
        /// <param name="useWildCard"></param>
        /// <returns></returns>
        public static bool PurgeLayers(string layerFilter, bool useWildCard)
        {
            bool retval = false;
           Autodesk.AutoCAD.DatabaseServices.Database db = AcadApplication.DocumentManager.MdiActiveDocument.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                retval = PurgeLayers(db, trans, layerFilter, useWildCard);
                trans.Commit();
            }
            return retval;
        }

        /// <summary>
        /// Purges the layers.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="layerFilter">The layer filter.</param>
        /// <param name="useWildCard">if set to <c>true</c> [use wild card].</param>
        /// <returns></returns>
        public static bool PurgeLayers(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, string layerFilter, bool useWildCard)
        {
            bool retval = false;
            ObjectIdCollection layerIds = new ObjectIdCollection();
            layerIds = LayerManager.GetLayerIds(db, trans, layerFilter, useWildCard);
            if (layerIds.Count > 0)
            {
                // Remove the layers that are in use and return the ones that can be erased
                db.Purge(layerIds);

                // Step through the returned ObjectIdCollection and erase each unreferenced layer
                foreach (ObjectId layerId in layerIds)
                {
                    LayerTableRecord record = trans.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                    record.Erase(true);
                }

                retval = true;
            }
            return retval;
        }

        /// <summary>
        /// Sets the database limits
        /// </summary>
        /// <param name="db"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        public static void SetLimits(Autodesk.AutoCAD.DatabaseServices.Database db, double minX, double minY, double maxX, double maxY)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                SetLimits(db, trans, minX, minY, maxX, maxY);
                trans.Commit();
            }
        }
        
        /// <summary>
        /// Sets the database limits
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        public static void SetLimits(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, double minX, double minY, double maxX, double maxY)
        {
            Point2d minPoint = new Point2d(minX, minY);
            Point2d maxPoint = new Point2d(maxX, maxY);

            db.Limmin = minPoint;
            db.Limmax = maxPoint;
        }

        /// <summary>
        /// Sets the grid spacing in the editor's active viewport
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ed"></param>
        /// <param name="gridSpacingX"></param>
        /// <param name="gridSpacingY"></param>
        public static void SetGridSpacing(Autodesk.AutoCAD.DatabaseServices.Database db, ObjectId viewportId, double gridSpacingX, double gridSpacingY)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                SetGridSpacing(db, trans, viewportId, gridSpacingX, gridSpacingY);
                trans.Commit();
            }
        }
        
        /// <summary>
        /// Sets the grid spacing in the editor's active viewport
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="ed"></param>
        /// <param name="gridSpacingX"></param>
        /// <param name="gridSpacingY"></param>
        public static void SetGridSpacing(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, ObjectId viewportId, double gridSpacingX, double gridSpacingY)
        {
            using (ViewportTableRecord viewportTableRecord = trans.GetObject(viewportId, OpenMode.ForWrite) as ViewportTableRecord)
            {
                // Adjust the spacing of the grid to gridSpacingX, gridSpacingY      
                viewportTableRecord.GridIncrements = new Point2d(gridSpacingX, gridSpacingY);
            }
        }

        /// <summary>
        /// Sets the snap spacing in the editor's active viewport
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ed"></param>
        /// <param name="snapSpacingX"></param>
        /// <param name="snapSpacingY"></param>
        public static void SetSnapSpacing(Autodesk.AutoCAD.DatabaseServices.Database db, ObjectId viewportId, double snapSpacingX, double snapSpacingY)
        {
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                SetSnapSpacing(db, trans, viewportId, snapSpacingX, snapSpacingY);
                trans.Commit();
            }
        }

        /// <summary>
        /// Sets the snap spacing in the editor's active viewport
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="ed"></param>
        /// <param name="snapSpacingX"></param>
        /// <param name="snapSpacingY"></param>
        public static void SetSnapSpacing(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, ObjectId viewportId, double snapSpacingX, double snapSpacingY)
        {
            using (ViewportTableRecord viewportTableRecord = trans.GetObject(viewportId, OpenMode.ForWrite) as ViewportTableRecord)
            {
                // Adjust the spacing of the snap to snapSpacingX, snapSpacingY     
                viewportTableRecord.SnapIncrements = new Point2d(snapSpacingX, snapSpacingY);
            }
        }

        /// <summary>
        /// Iterates model space filtering entities by layer name and object type
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="layerNames"></param>
        /// <param name="objectNames"></param>
        /// <returns></returns>
        public static ObjectIdCollection GetObjectIdsOnLayer(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, IList<string> layerNames, IList<string> objectNames)
        {
            ObjectIdCollection oids = new ObjectIdCollection();

            using (BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead))
            {
                using (BlockTableRecord btr = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead))
                {
                    foreach (ObjectId oid in btr)
                    {
                        if (!oid.IsErased)
                        {
                            Entity ent = trans.GetObject(oid, OpenMode.ForRead) as Entity;
                            if (ent != null)
                            {
                                bool isLayerMatch = false;
                                bool isObjectMatch = false;
                                if (layerNames.Count > 0)
                                {
                                    isLayerMatch = false;
                                    foreach (string layerName in layerNames)
                                    {
                                        // If this is a wildcard match
                                        if (layerName.Contains("*") == true)
                                        {
                                            if (ent.Layer.ToUpper().StartsWith(layerName.ToUpper().Replace("*", string.Empty)))
                                            {
                                                isLayerMatch = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (layerName.ToUpper() == ent.Layer.ToUpper())
                                            {
                                                isLayerMatch = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    isLayerMatch = true;
                                }
                                if (isLayerMatch == true)
                                {
                                    if (objectNames.Count > 0)
                                    {
                                        isObjectMatch = false;
                                        foreach (string objectName in objectNames)
                                        {
                                            try
                                            {
                                                if (objectName.ToUpper() == oid.ObjectClass.DxfName)
                                                {
                                                    isObjectMatch = true;
                                                }
                                            }
                                            catch (System.Exception)
                                            {
                                                isObjectMatch = false;
                                            }

                                            if (isObjectMatch == true)
                                            {
                                                oids.Add(ent.ObjectId);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        oids.Add(ent.ObjectId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return oids;
        }

        /// <summary>
        /// Deletes the objects.
        /// </summary>
        /// <param name="oids">The oids.</param>
        static public ObjectIdCollection GetAllObjectIdsInModel(bool includeErased)
        {
            ObjectIdCollection retVal = new ObjectIdCollection();
           Autodesk.AutoCAD.DatabaseServices.Database db = AcadApplication.DocumentManager.MdiActiveDocument.Database;
            retVal = GetAllObjectIdsInModel(db, includeErased);
            return retVal;
        }

        /// <summary>
        /// Deletes the objects.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="oids"></param>
        static public ObjectIdCollection GetAllObjectIdsInModel(Autodesk.AutoCAD.DatabaseServices.Database db, bool includeErased)
        {
            ObjectIdCollection retVal = new ObjectIdCollection();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                retVal = GetAllObjectIdsInModel(db, trans, includeErased);
                trans.Commit();
            }
            return retVal;
        }
        
        /// <summary>
        /// Iterates model space returning all object ids
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <param name="includeErased">if set to <c>true</c> [include erased].</param>
        /// <returns></returns>
        public static ObjectIdCollection GetAllObjectIdsInModel(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans, bool includeErased)
        {
            ObjectIdCollection oids = new ObjectIdCollection();

            using (BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead))
            {
                using (BlockTableRecord btr = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead))
                {
                    foreach (ObjectId oid in btr)
                    {
                        if ((includeErased == false) && (oid.IsErased == false))
                        {
                            oids.Add(oid);
                        }
                        else
                        {
                            oids.Add(oid);
                        }
                    }
                }
            }
            return oids;
        }

        /// <summary>
        /// Gets the model space viewport id.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="trans">The trans.</param>
        /// <returns></returns>
        public static ObjectId GetModelSpaceViewportId(Autodesk.AutoCAD.DatabaseServices.Database db, Transaction trans)
        {
            ObjectId msViewportId = ObjectId.Null;
            ViewportTable viewportTable = (ViewportTable)trans.GetObject(db.ViewportTableId, OpenMode.ForRead);
            foreach (ObjectId id in viewportTable)
            {
                ViewportTableRecord viewportTableRecord = (ViewportTableRecord)trans.GetObject(id, OpenMode.ForRead);
                if (viewportTableRecord.Name == "*Active")
                {
                    msViewportId = id;
                }
            }
            return msViewportId;
        }

        /// <summary>
        /// Fuzzies the equal to.
        /// </summary>
        /// <param name="dVal">The d val.</param>
        /// <param name="dValue2">The d value2.</param>
        /// <returns></returns>
        public static bool FuzzyEqualTo(double value1, double value2)
        {
            double tolerance = 0.0001;
            

            double absoluteDifference = 0;
            absoluteDifference = Math.Abs(value1 - value2);

            return (absoluteDifference < tolerance);
        }

        /// <summary>
        /// Fuzzies the less than.
        /// </summary>
        /// <param name="dLeftVal">The d left val.</param>
        /// <param name="dRightVal">The d right val.</param>
        /// <returns></returns>
        public static bool FuzzyLessThan(double leftSide, double rightSide)
        {
            bool retVal = false;

            if (((leftSide < rightSide) & !FuzzyEqualTo(leftSide, rightSide)))
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Fuzzies the greater than.
        /// </summary>
        /// <param name="dLeftVal">The d left val.</param>
        /// <param name="dRightVal">The d right val.</param>
        /// <returns></returns>
        public static bool FuzzyGreaterThan(double leftSide, double rightSide)
        {
            bool retVal = false;

            if (((leftSide > rightSide) & !FuzzyEqualTo(leftSide, rightSide)))
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Merges the object id collections.
        /// </summary>
        /// <param name="oids">The oids.</param>
        /// <param name="mergeIds">The merge ids.</param>
        public static void MergeObjectIdCollections(ref ObjectIdCollection oids, ObjectIdCollection mergeIds)
        {
            foreach (ObjectId oid in mergeIds)
            {
                if (!oids.Contains(oid))
                {
                    oids.Add(oid);
                }
            }
        }

        #region GetPolarPoint

        /// <summary>
        /// Gets the polar point.
        /// </summary>
        /// <param name="basePointX">The base point X.</param>
        /// <param name="basePointY">The base point Y.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public static Point2d GetPolarPoint(double basePointX, double basePointY, double angle, double distance)
        {
            return new Point2d(
            basePointX + (distance * Math.Cos(angle)),
            basePointY + (distance * Math.Sin(angle)));
        }

        /// <summary>
        /// Gets the polar point.
        /// </summary>
        /// <param name="basePoint">The base point.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public static Point2d GetPolarPoint(Point2d basePoint, double angle, double distance)
        {
            return new Point2d(
            basePoint.X + (distance * Math.Cos(angle)),
            basePoint.Y + (distance * Math.Sin(angle)));
        }

        /// <summary>
        /// Gets the polar point.
        /// </summary>
        /// <param name="basePoint">The base point.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public static Point3d GetPolarPoint(Point3d basePoint, double angle, double distance)
        {
            return new Point3d(
            basePoint.X + (distance * Math.Cos(angle)),
            basePoint.Y + (distance * Math.Sin(angle)),
            basePoint.Z);
        }

        #endregion
    }
}
