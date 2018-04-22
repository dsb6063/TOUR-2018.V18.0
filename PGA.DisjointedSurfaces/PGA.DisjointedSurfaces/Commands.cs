using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using Autodesk.Aec.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using ACAD = Autodesk.AutoCAD.ApplicationServices;
using ACADRT = Autodesk.AutoCAD.Runtime;
using ACADDB = Autodesk.AutoCAD.DatabaseServices;
using PGA.Database;
using PGA.DataContext;
using Process = ProcessPolylines.ProcessPolylines;
using AssignNames = AssignPolylineLayers.Commands;
using CreateAllSurface = CreateTINSurfaceFromCloud;
using C3D = C3DSurfacesDemo;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.Civil.Settings;
using Utils = Pge.Common.Framework;

namespace PGA.DisjointedSurfaces
{
    public static class Commands
    {
        public static int NumberofPolylines;
        public static DateTime datetime;
        public static string outdxf;
        public static string outdwg;

        public static ACADDB.ObjectIdCollection collection = null;
        public static ACADDB.ObjectIdCollection OriginalCollection;
        static IList<PolylineChildren> m_PolylineChildren = new List<PolylineChildren>();

        [ACADRT.CommandMethodAttribute("PGA-DisjoinSurfaces", ACADRT.CommandFlags.Session)]
        public static void LoadandProcessPolys()
        {


            using (DatabaseCommands commands = new DatabaseCommands())
            {
                var TemplatePath = commands.GetTemplatePath();

                #region Get Dwgs to Process

                var dwgs = commands.LoadandProcessPolys();

                if (dwgs == null) throw new ArgumentNullException(nameof(dwgs));
                IList<DrawingStack> DwDrawingStacks = new List<DrawingStack>();
                foreach (var dwg in dwgs)
                {
                    if (DwDrawingStacks != null) DwDrawingStacks.Add(dwg);
                }

                #endregion

                GDwgPath = commands.GetGlobalDWGPath();
                GCloudPath = commands.GetGlobalPointCloudPath();



                foreach (DrawingStack dwg in DwDrawingStacks)
                {
                    m_PolylineChildren.Clear();
                    var acDocMgr = ACAD.Application.DocumentManager;
                    Document acNewDoc = null;
                    Document acDoc = ACAD.Application.DocumentManager.MdiActiveDocument;
                    
                    if (acDocMgr.Count == 1)
                    {

                        acNewDoc = acDocMgr.Add(TemplatePath);
                        using (acDocMgr.MdiActiveDocument.LockDocument())
                        {
                            acDocMgr.MdiActiveDocument = acDoc;
                            acDocMgr.CurrentDocument.CloseAndDiscard();
                            acDocMgr.MdiActiveDocument = acNewDoc;

                        }

                    }

                    using (acDocMgr.MdiActiveDocument.LockDocument())
                    {
                        if (acDoc != null)

                        {
                            ACADDB.Database acDbNewDoc = acNewDoc.Database;

                            //var ed = doc.Editor;

                            string gpath = Convert.ToString(GDwgPath);

                            if (gpath != null)
                            {
                                string path = Path.Combine(gpath, dwg.PolylineDwgName);

                                if (path == null) throw new ArgumentNullException(nameof(path));


                                if (!File.Exists(path))
                                {
                                    DatabaseLogs.FormatLogs("File Not Found", path);
                                    return;

                                }

                                try
                                {


                                    using (ACADDB.Database db = new ACADDB.Database(false, true))
                                    {
                                        // Read the DWG file into our Database object

                                        db.ReadDwgFile(
                                            path,
                                            ACADDB.FileOpenMode.OpenForReadAndReadShare,
                                            false,
                                            ""
                                            );

                                        // No graphical changes, so we can keep the preview
                                        // bitmap

                                        db.RetainOriginalThumbnailBitmap = true;

                                        // We'll store the current working database, to reset
                                        // after the purge operation

                                        var wdb = ACADDB.HostApplicationServices.WorkingDatabase;
                                        ACADDB.HostApplicationServices.WorkingDatabase = db;

                                        // Purge unused DGN linestyles from the drawing
                                        // (returns false if nothing is erased)
                                        collection = Process.GetIdsByTypeTypeValue(
                                            "POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");
                                        NumberofPolylines = collection.Count;
                                        Process.CopyPolylinesBetweenDatabases(db, collection);

                                        // Still need to reset the working database
                                        datetime = (DateTime) dwg.DateStamp;
                                        ACADDB.HostApplicationServices.WorkingDatabase = wdb;

                                        string output = SetOutPutFolder(dwg, commands);

                                        // PLineToLayers.ProcessLayers(collection, wdb);                                     
                                        string fileName = System.IO.Path.GetFileNameWithoutExtension(dwg.PolylineDwgName);
                                        outdxf = Path.Combine(output, String.Format("{0}{1}", fileName, ".dxf"));
                                        outdwg = Path.Combine(output, dwg.PolylineDwgName);
                                        collection.Clear();
                                        //Before Simplification Create Breakline Data
                                        OriginalCollection =
                                            RefreshSelectionOfPolylines(collection);

                                        m_PolylineChildren = CreatePolylineChildren(OriginalCollection);
                                    }
                                }
                                catch (Exception e)
                                {
                                    DatabaseLogs.FormatLogs("Exception: {0}", e.Message);
                                }
                            }
                        }
                    }

                    PGA.SimplifyPolylines.Commands.SimplifyPolylines();

                    using (acDocMgr.MdiActiveDocument.LockDocument())
                    {
                        try
                        {
                            Document acDocument = ACAD.Core.Application.DocumentManager.MdiActiveDocument;
                            ACADDB.Database acDbNewDoc = acDocument.Database;
                           // if (collection != null) collection.Clear();
                            //collection = RefreshSelectionOfPolylines(collection);
                            collection = OriginalCollection;
                            NumberofPolylines = collection.Count;

                            //Check Polyline Count

                            //Change Layer Names
                            AssignNames.ChangeLayers();

                           // collection.Clear();
                            // get the copied collection of this drawing
                            ///collection = RefreshSelectionOfPolylines(collection);

                            //NumberofPolylines = collection.Count;
                            //Get the Distinct LIDAR Points
                            //var p3d = ProcessLidarFile(
                            //    PlineToPoints(collection), datetime, dwg.Hole.ToString());
                            var lidarFile = RetrieveLidarFile(datetime, dwg.Hole.ToString());

                            //Get the Nearest Neighbor to add to Point Set
                            var neighbors = IncludeIntersectingNeighbor(lidarFile, OriginalCollection);
                            Point3dCollection totalpoints = MergePoint3DCollections(lidarFile, neighbors);
                            var p5d = ProcessLidarFile(
                                PlineToPoints(collection), datetime, dwg.Hole.ToString(), totalpoints);
                            //var p5d = ProcessLidarFileSubtractRegions(
                            //  PlineToPoints(collection), datetime, dwg.Hole.ToString(), totalpoints,collection);

     
                            //Insert into DB 

                            InsertPointCloudToDB(dwg, datetime, p5d, PlineToPoints(collection));

                            //Create all the Distinct surfaces
                            CreateTinForDistinctSurfaces(p5d, collection);

                            acDbNewDoc.SaveAs(outdwg, ACADDB.DwgVersion.Current);
                            acDbNewDoc.DxfOut(outdxf, 16, ACADDB.DwgVersion.Current);

                        }
                        catch (ACADRT.Exception ex)
                        {
                            DatabaseLogs.FormatLogs("Exception: {0}", ex.Message);
                        }

                    }
                }
            }
        }

        private static IList<Point3dCollection> SubtractRegions(IList<Point3dCollection> p5D, ACADDB.ObjectIdCollection objectIdCollection)
        {
            throw new NotImplementedException();
        }


        public static IList<PolylineChildren> CreatePolylineChildren(ACADDB.ObjectIdCollection polylines)
        {   
            IList<PolylineChildren> children = new List<PolylineChildren>();
            foreach (ACADDB.ObjectId polyoid in polylines)
            {
                ACADDB.ObjectIdCollection collection = null;
                Point2dCollection innerpoints = null;
                Point2dCollection outerpoints = null;

                outerpoints = PlineToPoints(polyoid);
                collection = new ACADDB.ObjectIdCollection();

                //Is Polyline within Outer Polyline
                foreach (ACADDB.ObjectId polyline in polylines)
                {
                    innerpoints = PlineToPoints(polyline);
                    if(PointUtilities.PointInPolyline(outerpoints, innerpoints))
                        collection.Add(polyline);
                }
                children.Add(new PolylineChildren(polyoid,collection));
            }


            return children;
        } 

        private static Point3dCollection MergePoint3DCollections(Point3dCollection p3D, IList<Point3dCollection> p3Modified)
        {
            for (int i = 0; i < p3Modified.Count; i++)
            {
                for (int j = 0; j < p3Modified[i].Count; j++)
                    p3D.Add(p3Modified[i][j]);
            }
            return p3D;
        }

        private static IList<Point3dCollection> IncludeIntersectingNeighbor(Point3dCollection p3D, ACADDB.ObjectIdCollection originalCollection)
        {
            IList<Point3dCollection> outCollection = new List<Point3dCollection>();
            Point3dCollection temPoint3DCollection = null;
            foreach (ACADDB.ObjectId polyoid in originalCollection)
            {
                
                var index = originalCollection.IndexOf(polyoid);
                Point2dCollection polypoints = PGA.AcadUtilities.AcadUtilities.GetPointsFromPolyline(polyoid);
                Point2d nearPoint = new Point2d();
                Point2d outPoint = new Point2d();
                var pointindex = 0;
                temPoint3DCollection = new Point3dCollection();
                Point2dCollection p2d = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(p3D);
                foreach (var point in polypoints)
                {
                   
                    if (PGA.AcadUtilities.PolygonFunctions.IsPointInsidePolygon(point, p2d.ToArray(), true))
                    {
                        double maxdist = 100000;
                        double mindist = 0;
                        var matchingIndex =-1;
                        //Points on Poly are not Lidar Points Must add manually
                        for (int i = 0; i < p2d.Count - 1; i++)
                        {
                           mindist= PGA.AcadUtilities.PolygonFunctions.DistanceToSegment(point, p2d[i], p2d[i + 1], ref nearPoint);
                            if (mindist < maxdist)
                            {
                                maxdist = mindist;
                                outPoint = nearPoint;
                                pointindex = i;
                            }
                        }
                        //for (int i = 0; i < p3D.Count; i++)
                        //{
                        //    if (Equals(p3D[i], outPoint))
                        //        temPoint3DCollection.Add(p3D[i]);
                        //}
                        if (maxdist < 5.0)
                            temPoint3DCollection.Add(new Point3d(point.X, point.Y, p3D[pointindex].Z));
                       // temPoint3DCollection.Add(p3D[pointindex]);
                        //var matchingIndex = p2d.IndexOf(outPoint);
                        //if (matchingIndex != -1)
                        //  temPoint3DCollection.Add(p3D[index][matchingIndex]);
                    }
                }
                if(temPoint3DCollection !=null) outCollection.Add(temPoint3DCollection);
            }

            return outCollection;
        }

        public static bool Equals(Point3d first, Point2d second)
        {
            if (first.X == (second.X) && (Math.Abs(first.Y - (second.Y)) < .001))
                return true;

            return false;
        }

        private static ACADDB.ObjectIdCollection RefreshSelectionOfPolylines(ACADDB.ObjectIdCollection collection)
        {  
            if (collection != null)
               collection.Clear();

            return Process.GetIdsByTypeTypeValue(
                                       "POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");
        }

        private static string SetOutPutFolder(DrawingStack dwg, DatabaseCommands commands)
        {
            if (commands ==null)
                commands = new DatabaseCommands();

            var date = DateConverts.ConDateTimeToStringForFileSafeDatabase(dwg.DateStamp.Value);
            var output = Path.Combine(commands.GetGlobalDestinationPath(),
                String.Format("DISJOINT-HOLES-{0}", date));
            Utils.FileUtilities.CreateDirectory(output);
            return output;
        }

        private static void CreateTinForDistinctSurfaces(IList<Point3dCollection> p3D,
            ACADDB.ObjectIdCollection collection)
        {

            if (p3D.Count != collection.Count)
                return;
            PolylineChildren children;
            try
            {
                foreach (ACADDB.ObjectId polys in collection)
                {
                    children = new PolylineChildren();

                    foreach (PolylineChildren item in m_PolylineChildren)
                    {
                        if (item.Oid.Equals(polys))
                        {
                            for (int i = 0; i < item.Children.Count; i++)
                            {
                                if (item.Children[i].Equals(polys))
                                    item.Children.RemoveAt(i);
                                else if (PGA.AcadUtilities.AcadUtilities.CompareAreasFromPolylines(polys,
                                    item.Children[i]))
                                    item.Children.RemoveAt(i);
                            }

                            children = item;
                            break;
                        }
                    }
                    var layer = PGA.SimplifyPolylines.Commands.GetPolylineName(polys);
                    var index = collection.IndexOf(polys);
                    CreateAllSurface.CreateTINSurface pTinSurface;
                    C3D.PasteSurfaces surfaces = new C3D.PasteSurfaces();

                    TinSurface theSurface;
                    if (index < p3D.Count)
                    {
                        pTinSurface = new CreateAllSurface.CreateTINSurface();
                        theSurface = pTinSurface.CreateTINSurfaceByPointCollection(p3D[index], layer);
                        surfaces.AddBoundariesForSurfaces(PlineToPoints(children.Oid), theSurface.ObjectId);
                       // ACADDB.ObjectId materId = ACADDB.Material.FromAcadObject(theSurface.AcadObject);
                       
                        //adds outer
                        //Adds hide polylines
                        for (int i = 0; i < children.Children.Count; i++)
                        {
                            var polyboundary = new  ACADDB.ObjectIdCollection();
                            polyboundary.Add(children.Oid);
                            SurfaceMaskCreationData mask = new SurfaceMaskCreationData
                                (String.Format("Hide-{0}",i),"Hide",theSurface.ObjectId,polyboundary,3,theSurface.MaterialId,SurfaceMaskType.InSide, false);
                            if (!PGA.AcadUtilities.AcadUtilities.CompareAreasFromPolylines(polys, children.Children[i]))
                            {
                                surfaces.AddStandardBoundary(children.Children[i], theSurface.Name, theSurface);
                                //theSurface.Masks.Add(mask);
                            }
                        }
                    }
                }
            }
            catch (Exception ex1)
            {
                DatabaseLogs.FormatLogs(ex1.Message);
            }

        }

        public static string GCloudPath { get; set; }

        public static IList<DrawingStack> DwDrawingStacks { get; set; }

        public static string GDwgPath { get; set; }

        public static IList<Point2dCollection> PlineToPoints(ACADDB.ObjectIdCollection collection)
        {
            IList<Point2dCollection> boundary= new List<Point2dCollection>();

            foreach (ACADDB.ObjectId pline in collection)
            {
                boundary.Add( SimplifyPolylines.Commands.GetPoint2dFromPolylines(pline));

            }

            return boundary;
        }

        public static Point2dCollection PlineToPoints(ACADDB.ObjectId pline)
        {

            return SimplifyPolylines.Commands.GetPoint2dFromPolylines(pline);
 
        }

        public struct PointsWithPolyId
        {
            public PointsWithPolyId(Point3dCollection local, int indexOf)
            {
                Points = local;
                Polys = indexOf;
            }

            public Point3dCollection Points { get; set; }
            public int Polys { get; set; }
        }

        public struct PolylineChildren
        {
            public PolylineChildren(ACADDB.ObjectId _oid, ACADDB.ObjectIdCollection _chIdCollection)
            {
                Oid = _oid;
                Children = _chIdCollection;
            }

            public ACADDB.ObjectId Oid { get; set; }
            public ACADDB.ObjectIdCollection Children { get; set; }
        }
        public static IList<Point3dCollection> ProcessLidarFile(IList<Point2dCollection> boundary, DateTime time, string hole, Point3dCollection lidarpoints)
        {
            IList<PointsWithPolyId> processedpoints = new List<PointsWithPolyId>();
            if (boundary == null) throw new ArgumentNullException(nameof(boundary));
            if (hole == null) throw new ArgumentNullException(nameof(hole));

            DatabaseCommands command = new DatabaseCommands();
            var path = command.GetPointPathByDate(time, hole);
            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);

            if (name == null) throw new ArgumentNullException(nameof(name));

            //Point3dCollection lidarpoints = CreateAllSurface.ReadPointCloudFile.ReadFile(dir, name);
            Point2dCollection points2D = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(lidarpoints);
            IList<Point3dCollection> parsecollection = new List<Point3dCollection>();

            foreach (Point2dCollection pline in boundary)
            {
                Point3dCollection local = new Point3dCollection();
                for (int i = 0; i < points2D.Count; i++)
                {
                    if (PointUtilities.PointInPolyline(pline, points2D[i]))
                        local.Add(lidarpoints[i]);
                }
                parsecollection.Add(local);
                
                processedpoints.Add(new PointsWithPolyId(local,boundary.IndexOf(pline)));
            }
            return parsecollection;
        }

        public static ACADDB.Region AddRegion(ACADDB.ObjectId acadObjectId)
        {
            ACADDB.Region returnvalue = null;
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            ACADDB.Database acCurDb = acDoc.Database;

            // Start a transaction
            using (ACADDB.Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                ACADDB.BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                ACADDB.OpenMode.ForRead) as ACADDB.BlockTable;

                // Open the Block table record Model space for write
                ACADDB.BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[ACADDB.BlockTableRecord.ModelSpace],
                                                ACADDB.OpenMode.ForWrite) as ACADDB.BlockTableRecord;

                ACADDB.Polyline polyline = acTrans.GetObject(acadObjectId,
                                    ACADDB.OpenMode.ForRead) as ACADDB.Polyline;
                if (polyline != null)

                {
                    ACADDB.DBObjectCollection acDBObjColl = new ACADDB.DBObjectCollection();
                    acDBObjColl.Add((ACADDB.DBObject) polyline.AcadObject);

                    // Calculate the regions based on each closed loop
                    ACADDB.DBObjectCollection myRegionColl = new ACADDB.DBObjectCollection();
                    myRegionColl = ACADDB.Region.CreateFromCurves(acDBObjColl);
                    ACADDB.Region acRegion = myRegionColl[0] as ACADDB.Region;
                    returnvalue = acRegion;
                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acRegion);
                    acTrans.AddNewlyCreatedDBObject(acRegion, true);

                    // Dispose 
                }
                
                // Save the new object to the database
                acTrans.Commit();
            }
            return returnvalue;
        }

        public static IList<Point3dCollection> ProcessLidarFileSubtractRegions(IList<Point2dCollection> boundary, DateTime time, string hole, Point3dCollection lidarpoints, ACADDB.ObjectIdCollection polylineCollection)
        {
            IList<PointsWithPolyId> processedpoints = new List<PointsWithPolyId>();
            if (boundary == null) throw new ArgumentNullException(nameof(boundary));
            if (hole == null) throw new ArgumentNullException(nameof(hole));

            DatabaseCommands command = new DatabaseCommands();
            var path = command.GetPointPathByDate(time, hole);
            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);

            if (name == null) throw new ArgumentNullException(nameof(name));

            //Point3dCollection lidarpoints = CreateAllSurface.ReadPointCloudFile.ReadFile(dir, name);
            Point2dCollection points2D = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(lidarpoints);
            IList<Point3dCollection> parsecollection = new List<Point3dCollection>();

            try
            {
                foreach (Point2dCollection pline in boundary)
                {
                    Point3dCollection local = new Point3dCollection();
                    for (int i = 0; i < points2D.Count; i++)
                    {
                        if (PointUtilities.PointInPolyline(pline, points2D[i]))
                            local.Add(lidarpoints[i]);
                    }
                    parsecollection.Add(local);
                    //contains the points for each polyline
                    processedpoints.Add(new PointsWithPolyId(local, boundary.IndexOf(pline)));
                }

                m_PolylineChildren.Count();
                //subtract inner points
                foreach (ACADDB.ObjectId maspolyoid in polylineCollection)
                {

                    int masterregloc = polylineCollection.IndexOf(maspolyoid);
                    PolylineChildren subChildren = m_PolylineChildren[masterregloc];

                    if (maspolyoid != subChildren.Oid)
                        throw new Exception("ProcesssLidarPoints: Subtract regions unordered!");

                  


                    for (int i = 0; i < subChildren.Children.Count; i++)
                    {
                        ACADDB.ObjectId subregionoid = subChildren.Children[i];
                        //Filter Out == (Area,ObjectId)
                        if ((subregionoid != maspolyoid) ||
                            !(AcadUtilities.AcadUtilities.CompareAreasFromPolylines
                            (subregionoid, maspolyoid)))
                        {
                            //var outterRegion = ACADDB.Region.FromAcadObject(maspolyoid);
                            var outterRegion = AddRegion(maspolyoid);
                            var innerRegion = AddRegion(subregionoid);

                            if (outterRegion.Area > innerRegion.Area)

                            {
                                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, innerRegion);
                                innerRegion.Dispose();
                               
                            }
                            //Point3dCollection pointstodelete = new Point3dCollection();

                            //int subregloc = polylineCollection.IndexOf(subregionoid);
                            //if (subregloc != -1)
                            //{
                            //    ACADDB.Region.FromAcadObject(maspolyoid);
                            //    pointstodelete = parsecollection[subregloc];

                            //    for (int j = 0; j < parsecollection[subregloc].Count; j++)
                            //    {                             
                            //        foreach (Point3d point in pointstodelete)
                            //        {
                            //            if (parsecollection[masterregloc].Contains(point))
                            //                parsecollection[masterregloc].Remove(point);
                            //        }
                            //    }
                            //}
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                DatabaseLogs.FormatLogs("ProcessLidarFileSubtractRegions"+ ex.Message);
            }

            return parsecollection;
        }

        public static ACADDB.Region CompareResultsofRegion(ACADDB.Region region1, ACADDB.Region region2)
        {
            var outterRegion = region1;
            var innerRegion  = region2;

            if (outterRegion.Area > innerRegion.Area)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, innerRegion);
                innerRegion.Dispose();

            }
            return null;
        }

        public static ACADDB.Region SubtractRegions(ACADDB.ObjectId outter, ACADDB.ObjectId inner)
        {
            var outterRegion = AddRegion(outter);
            var innerRegion  = AddRegion(inner);

            if (outterRegion.Area > innerRegion.Area)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, innerRegion);
                innerRegion.Dispose();
                return outterRegion;

            }
            else
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolSubtract, outterRegion);
                outterRegion.Dispose();
                return innerRegion;
            }

        }

        public static ACADDB.Region UniteRegions(ACADDB.ObjectId outter, ACADDB.ObjectId inner)
        {
            var outterRegion = AddRegion(outter);
            var innerRegion = AddRegion(inner);

            if (outterRegion.Area > innerRegion.Area)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, innerRegion);
                innerRegion.Dispose();
                return outterRegion;

            }
            else
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, outterRegion);
                outterRegion.Dispose();
                return innerRegion;
            }
        }

        public static ACADDB.Region UniteRegions(ACADDB.ObjectId outter, ACADDB.ObjectId inner, bool firstchoice)
        {
            var outterRegion = AddRegion(outter);
            var innerRegion = AddRegion(inner);

            if (firstchoice)

            {
                outterRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, innerRegion);
                innerRegion.Dispose();
                return outterRegion;

            }
            else
            {
                innerRegion.BooleanOperation(ACADDB.BooleanOperationType.BoolUnite, outterRegion);
                outterRegion.Dispose();
                return innerRegion;
            }
        }

        public static IList<Point3dCollection> ProcessLidarFile(IList<Point2dCollection> boundary, DateTime time, string hole)
        {
            if (boundary == null) throw new ArgumentNullException(nameof(boundary));
            if (hole == null) throw new ArgumentNullException(nameof(hole));

            DatabaseCommands command = new DatabaseCommands();
            var path = command.GetPointPathByDate(time, hole);
            var dir  = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);

            if (name == null) throw new ArgumentNullException(nameof(name));

            Point3dCollection lidarpoints = CreateAllSurface.ReadPointCloudFile.ReadFile(dir, name);
            Point2dCollection points2D = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(lidarpoints);
            IList<Point3dCollection> parsecollection = new List<Point3dCollection>();

            foreach (Point2dCollection pline in boundary)
            {
                Point3dCollection local = new Point3dCollection();
                for (int i = 0; i < points2D.Count; i++)
                {
                    if (PointUtilities.PointInPolyline(pline, points2D[i]))
                        local.Add(lidarpoints[i]);
                }
                parsecollection.Add(local);
            }
            return parsecollection;
        }

        public static IList<Point3dCollection> Generate3DBreaklinesFromLidarFile(IList<Point2dCollection> boundary, DateTime time, string hole)
        {
            if (boundary == null) throw new ArgumentNullException(nameof(boundary));
            if (hole == null) throw new ArgumentNullException(nameof(hole));

            DatabaseCommands command = new DatabaseCommands();
            var path = command.GetPointPathByDate(time, hole);
            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);

            if (name == null) throw new ArgumentNullException(nameof(name));

            Point3dCollection lidarpoints = CreateAllSurface.ReadPointCloudFile.ReadFile(dir, name);
            //Create a Surface called "ALL".






            Point2dCollection points2D = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(lidarpoints);
            IList<Point3dCollection> parsecollection = new List<Point3dCollection>();

            foreach (Point2dCollection pline in boundary)
            {
                Point3dCollection local = new Point3dCollection();
                for (int i = 0; i < points2D.Count; i++)
                {
                    if (PointUtilities.PointInPolyline(pline, points2D[i]))
                        local.Add(lidarpoints[i]);
                }
                parsecollection.Add(local);
            }
            return parsecollection;
        }

        public static Point3dCollection RetrieveLidarFile(DateTime time, string hole)
        {
            if (hole == null) throw new ArgumentNullException(nameof(hole));

            DatabaseCommands command = new DatabaseCommands();
            var path = command.GetPointPathByDate(time, hole);
            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);

            if (name == null) throw new ArgumentNullException(nameof(name));

            Point3dCollection lidarpoints = CreateAllSurface.ReadPointCloudFile.ReadFile(dir, name);
            
            return lidarpoints;
        }
        public static IList<Point3dCollection> ProcessLidarFile(IList<Point2dCollection> boundary, string filename, string path)
        {
            if (boundary == null) throw new ArgumentNullException(nameof(boundary));
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (path == null) throw new ArgumentNullException(nameof(path));

            Point3dCollection lidarpoints = CreateAllSurface.ReadPointCloudFile.ReadFile(path, filename);
            Point2dCollection points2D = PGA.AcadUtilities.AcadUtilities.ConvertTo2d(lidarpoints);
            IList< Point3dCollection> parsecollection = new List<Point3dCollection>();

            foreach (Point2dCollection pline in boundary)
            {    Point3dCollection local = new Point3dCollection();
                for (int i = 0; i < points2D.Count; i++)
                {
                    if (PointUtilities.PointInPolyline(pline, points2D[i]))
                        local.Add(lidarpoints[i]);
                }
                parsecollection.Add(local);
            }
            return parsecollection;
        }

        public static void InsertPointCloudToDB(DrawingStack dwg, DateTime time, IList<Point3dCollection> pointsList, IList<Point2dCollection> boundaryList)
        {
           
            var UPPER = pointsList.Count;
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {
                    for (int i = 0; i < UPPER; i++)
                    {
                        if (((pointsList[i].Count == 0) || boundaryList[i].Count == 0))
                            throw new ArgumentNullException(string.Format("PointsList: {0}", pointsList[i].Count));
                        //Store as Points
                        SqlBytes p = GeometryManager.SerializeSqlGeographyMultiPoint(pointsList[i]);
                        SqlBytes b = GeometryManager.SerializeSqlGeographyMultiPoint(boundaryList[i]);
                        commands.InsertNewPolylineGeometry(dwg, time, GeometryManager.ToBytes(b),
                            GeometryManager.ToBytes(p));
                    }
                }

                catch
                    (Exception ex)
                {
                    DatabaseLogs.FormatLogs(ex.Message);
                }
            }
        }
    }
}
