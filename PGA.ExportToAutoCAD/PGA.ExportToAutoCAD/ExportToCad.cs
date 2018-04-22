using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using PGA.Database;
using PGA.DataContext;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Polyline = Autodesk.AutoCAD.GraphicsInterface.Polyline;
using PGA.AcadUtilities;
using PGA.PlotManager;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using COMS=PGA.MessengerManager.MessengerManager;

namespace PGA.ExportToAutoCAD
{
    public class ExportToCad
    {
        static int DWGCounter = 0;
        //[CommandMethod("PGA-ExportToCAD", CommandFlags.Session)]
        public static void ExportDwgs()
        {
            try
            {
                Application.SetSystemVariable("FILEDIA", 0);
                //var path = @"C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819\DISJOINT-HOLES-2015-12-30-014212";
                //ProcessFiles(path);
            }
            catch
            {
            }
        }

        public static void ProcessFiles(string path)
        {

            var dir = Path.GetDirectoryName(path);
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (file.Contains(".dwg") || file.EndsWith("DWG"))
                {
                    //result = Path.Combine((path),
                    //    "ACAD-s-" + Path.GetFileName(file));
                    ExportToAutoCAD(file);
                    break;
                }
            }
        }

        public static void ExportToAutoCAD(string filename)
        {
            var doc = Application.DocumentManager.Open(filename, true);
            Application.DocumentManager.MdiActiveDocument = doc;
            var db = doc.Database;
            var ed = doc.Editor;
            using (doc.LockDocument())
            {
                var name = Path.GetFileName(filename);
                name = "ACAD-s-" + name;
                doc.SendStringToExecute("ExportToAutoCAD" + "\n", true, false, false);
                doc.SendStringToExecute(name + "\n", true, false, false);
                // doc.CloseAndDiscard();
            }
        }

        public static void ExportToAutoCADCurrentDWG(string filename, long? hole)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (doc.LockDocument())
            {
                var name = Path.GetFileName(filename);
                var dir = Path.GetDirectoryName(filename);
                var shole = GolfHoles.GetHoles((int) hole);
                name = "ACAD-" + name;
                var completepath = Path.Combine(dir, name);
                doc.SendStringToExecute("ExportToAutoCAD " + completepath + "\n", true, false, false);
            }
        }
        public static string GetDWGPrefix(string filename, long? hole)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (doc.LockDocument())
            {
                var name  = Path.GetFileName(filename);
                var dir   = Path.GetDirectoryName(filename);
                var shole = GolfHoles.GetHoles((int)hole);
                name = "ACAD-" + name;
                var completepath = Path.Combine(dir, name);
                return completepath;
            }
        }
        public static void ExportZToAutoCAD(string filename)
        {
            var doc = Application.DocumentManager.Open(filename, true);
            Application.DocumentManager.MdiActiveDocument = doc;
            var db = doc.Database;

            using (doc.LockDocument())
            {
                var name = Path.GetFileName(filename);
                name = "ACAD-z-" + name;
                doc.SendStringToExecute("-ExportToAutoCAD" + "\n", true, false, false);
                doc.SendStringToExecute(name + "\n", true, false, false);
                // doc.CloseAndDiscard();
            }
        }
        public static void ExportZToDXF(string filename)
        {
            var doc = Application.DocumentManager.Open(filename, true);
            Application.DocumentManager.MdiActiveDocument = doc;
            var db = doc.Database;

            using (doc.LockDocument())
            {
                var name = Path.GetFileName(filename);
                name = "ACAD-z-" + name;
                doc.SendStringToExecute("-ExportToAutoCAD" + "\n", true, false, false);
                doc.SendStringToExecute(name + "\n", true, false, false);
                // doc.CloseAndDiscard();
            }
        }
        public static void ExportZToAutoCADCurrentDWG(string filename, long? hole)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (doc.LockDocument())
            {
                var name = Path.GetFileName(filename);
                var dir = Path.GetDirectoryName(filename);
                var shole = GolfHoles.GetHoles((int) hole);
                //dir = Path.Combine(dir, "HOLE" + shole);
                name = "ACAD-" + name;
                var completepath = Path.Combine(dir, name);

              //  doc.SendStringToExecute("-ExportToAutoCAD" + "\n", true, false, false);
               // doc.SendStringToExecute("-ExportToAutoCAD 2013 YES INSERT ACAD-  test" + "\n\n", true, false, false);

                 doc.SendStringToExecute("ExportToAutoCAD " + completepath + "\n", true, false, false);
                //doc.CloseAndDiscard();
            }
        }

        public static void SendAsyncExportDWGs()
       {
           var doc = Application.DocumentManager.MdiActiveDocument;
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                if (commands.DWGCountExportToCAD() <= ++DWGCounter)
                {
                    //using (doc.LockDocument())
                    //{
                        doc.SendStringToExecute("PGA-SendAsyncExportDWGs" + "\n", true, false, false);
                //    }
                }
            }
       }
        public static void SendCloseAllButActiveDWGs()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                if (commands.DWGCountExportToCAD() <= ++DWGCounter)
                {
                    //using (doc.LockDocument())
                    //{
                        doc.SendStringToExecute("PGA-CloseAllButActiveDWGs" + "\n", true, false, false);
                    //}
                }
            }
        }

        [CommandMethod("PGA-CloseAllButActiveDWGs", CommandFlags.Session)]
        public static void CloseAllButActiveDWGs()
        {
            DrawingManager.Commands.CloseAllButActiveDocuments();
        }

        [CommandMethod("PGA-SendAsyncExportDWGs", CommandFlags.Session)]
        public static void AsyncExportDWGs()
        {
            List<ExportToCADStack> allExportToCadRecords = null;

            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    allExportToCadRecords = commands.GetAllExportToCadRecords();
                    if (allExportToCadRecords != null)
                    {
                        Application.SetSystemVariable("FILEDIA", 0);

                        foreach (var file in allExportToCadRecords)
                        {
                            if (!commands.IsDWGStartedEprtToCad(file.Id) &&
                                !commands.IsDXFStartedExportToCad(file.Id) &&
                                !commands.IsCompletedExportToCad(file.Id))
                            {
                                commands.SetStartFlagExportToCad(file.Id);
                                commands.LockFileExportToCad(file.Id);
                                if (!GolfHoles.IsWithinLimits((int) file.Hole)) return;

                                var c = commands.GetSettingsCourseNumByDate
                                    ((DateTime) file.DateStamp);
                                var shole = GolfHoles.GetHoles((int) file.Hole);

                                var SkipSDxf = commands.IsSkipDXFByDate
                                    ((DateTime)file.DateStamp);

                                bool DwgInsert=false;

                                if (file.Function != null)
                                   DwgInsert = file.Function.Contains("INSERT");
                                COMS.AddLog("DwgInsert=" + DwgInsert);
                                if (DwgInsert)
                                {
                                    ExportToAutoCADCurrentDWG(file.CompletePath, file.Hole);
                                    commands.SetCompletedFlagDWGExportToCad(file.Id);
                                    commands.UnLockFileExportToCad(file.Id);
                                    break;
                                }
                               
                                // Skip DXF and ACAD
                                if (SkipSDxf)
                                    if (!(IsZDrawing(file.CompletePath)))
                                    {
                                        commands.SetCompletedFlagDWGExportToCad(file.Id);
                                        commands.UnLockFileExportToCad(file.Id);
                                        break;
                                    }
                                if (File.Exists(file.CompletePath))
                                {   
                                    var doc = Application.DocumentManager.Open
                                        (file.CompletePath, true);
                                    Application.DocumentManager.MdiActiveDocument = doc;
                                    var db = doc.Database;
                                    using (doc.LockDocument())
                                    {
                                        try
                                        {


                                           //PGA.MessengerManager.MessengerManager.AddLog("Starting Snapshot!");

                                            
                                           // try
                                           // {
                                           //     PGA.PlotManager.PrintCommands.PublishLayouts(file.CompletePath,
                                           //         file.Hole.ToString());

                                           // }
                                           // catch (System.Exception ex)
                                           // {
                                           //     PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
                                           // }

                                           // try
                                           // {

                                           //     PGA.PlotManager.PlotCommands.SnapshotToFile(
                                           //         Path.ChangeExtension(file.CompletePath, "png"),
                                           //         VisualStyleType.Wireframe2D);

                                           //     PGA.PlotManager.PlotCommands.SnapShotFromPrintScreen(
                                           //         Path.ChangeExtension(file.CompletePath, "jpg"));
                                           // }
                                           // catch (System.Exception ex)
                                           // {
                                           //     PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
                                           // }

                                           // PGA.MessengerManager.MessengerManager.AddLog("Ending Snapshot!");

                                        }
                                        catch (System.Exception ex)
                                        {
                                            COMS.LogException(ex);
                                        }
                                        if (IsZDrawing(file.CompletePath))
                                        {

                                            ExportZToAutoCADCurrentDWG(file.CompletePath, file.Hole);
                                            commands.SetCompletedFlagDWGExportToCad(file.Id);
                                            commands.UnLockFileExportToCad(file.Id);
                                            break;
                                        }
                                        else
                                        {
                                            if (!SkipSDxf)
                                            {
                                                ExportToAutoCADCurrentDWG(file.CompletePath, file.Hole);
                                            }
                                            
                                            commands.SetCompletedFlagDWGExportToCad(file.Id);
                                            commands.UnLockFileExportToCad(file.Id);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public static void AsyncExportDXFs()
        {
            List<ExportToCADStack> allExportToCadRecords = null;
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    allExportToCadRecords = commands.GetAllExportToCadRecords();
             
                    if (allExportToCadRecords != null)
                    {
                        Application.SetSystemVariable("FILEDIA", 0);

                        bool Is2013Version = false;
                        bool SkipSDxf      = false;

                        foreach (var file in allExportToCadRecords)
                        {
                            if (!commands.IsDXFStartedExportToCad(file.Id) &&
                                !commands.IsCompletedExportToCad(file.Id))
                            {
                                commands.SetDXFFlagExportToCad(file.Id);
                                commands.LockFileExportToCad(file.Id);

                                var s = commands.GetSettingsByDate((DateTime)file.DateStamp);
                               

                                SkipSDxf = commands.IsSkipDXFByDate
                                    ((DateTime) file.DateStamp);

                                Is2013Version = commands.IsDXFVersionByDate
                                    ((DateTime) file.DateStamp);

                                COMS.AddLog(string.Format
                                    ("Set the DXF file to version 2013: {0}",
                                        Is2013Version));

                                if (!GolfHoles.IsWithinLimits((int) file.Hole)) return;

                                var c = commands.GetSettingsCourseNumByDate
                                    ((DateTime) file.DateStamp);
                                var shole = GolfHoles.GetHoles((int) file.Hole);
                                var modifiedname = ModifyDwgName(file.CompletePath);

                                // Skip DXF
                                if (SkipSDxf)
                                    if (!(IsZDrawing(file.CompletePath)))
                                    {
                                        COMS.AddLog("Skip DXF! Omitting S-Surface DXF.");
                                        commands.SetCompletedFlagExportToCad(file.Id);
                                        commands.UnLockFileExportToCad(file.Id);
                                        break;
                                    }

                                if (File.Exists(modifiedname))
                                {
                                    var doc = Application.DocumentManager.Open
                                        (modifiedname, true);
                                    Application.DocumentManager.MdiActiveDocument = doc;
                                    var db = doc.Database;

                                    using (doc.LockDocument())
                                    {
                                        //PGA.DrawingManager.Commands.
                                        //CloseAllButActiveDocuments();

                                        if (IsZDrawing(modifiedname))
                                        {
                                            AsyncDXFCall(file.CompletePath, modifiedname, c.ToString(), shole,
                                                Is2013Version, db);
                                            COMS.AddLog("Done with file Z-Surface DXF: " + modifiedname);

                                            commands.SetCompletedFlagExportToCad(file.Id);
                                            commands.UnLockFileExportToCad(file.Id);
                                            break;
                                        }
                                        else
                                        {
                                            if (!SkipSDxf)
                                            {
                                                if (s.SportVision == true)
                                                {
                                                    var _modifiedname = "";
                                                    if (!String.IsNullOrEmpty(_modifiedname = (CheckAndCreateDirectory(file.CompletePath, shole, c, s.TourCode))))
                                                    {
                                                        AsyncDXFCall(_modifiedname, file.CompletePath, c,
                                                            shole,
                                                            Is2013Version, db);

                                                        COMS.AddLog("Done with file S-Surface DXF: " + _modifiedname);
                                                    }

                                                }
                                                else
                                                    AsyncDXFCall(file.CompletePath, file.CompletePath, c.ToString(),
                                                        shole,
                                                        Is2013Version, db);
                                            }
                                            commands.SetCompletedFlagExportToCad(file.Id);
                                            commands.UnLockFileExportToCad(file.Id);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public static string CheckAndCreateDirectory(string CompletePath, string Hole, string CourseCode, string TourCode)
        {
            try
            {
                var dir = Path.GetDirectoryName(CompletePath);

                var name = String.Format("{0}{1}{2}{3}.dxf", 
                    TourCode,
                    DateTime.Now.Year, 
                    CourseCode, 
                    Hole);

                var modified = Path.Combine(dir, name);

                if (!File.Exists(modified))
                    return modified;
                return "";
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return "";
        }

        [CommandMethod("PGA-ExportToCAD", CommandFlags.Session)]
        public static void SynchExportDWGs()
        {
            List<ExportToCADStack> allExportToCadRecords = null;
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                allExportToCadRecords = commands.GetAllExportToCadRecords();
                bool Is2013Version= false;
                foreach (var file in allExportToCadRecords)
                {
                    Is2013Version = commands.IsDXFVersionByDate
                                   ((DateTime) file.DateStamp);

                    COMS.AddLog(string.Format
                        ("Set the DXF file to version 2013: {0}",
                        Is2013Version));

                    if (!commands.IsDWGStartedEprtToCad(file.Id) &&
                        !commands.IsDXFStartedExportToCad(file.Id) &&
                        !commands.IsCompletedExportToCad(file.Id))
                    {
                        commands.SetStartFlagExportToCad(file.Id);

                        if (!GolfHoles.IsWithinLimits((int)file.Hole)) return;

                        var c = commands.GetSettingsCourseNumByDate
                            ((DateTime)file.DateStamp);
                        var shole = GolfHoles.GetHoles((int)file.Hole);

                        if (File.Exists(file.CompletePath))
                        {
                            #region Use this if reading file
                            var doc = Application.DocumentManager.Open
                                (file.CompletePath, true);
                            Application.DocumentManager.MdiActiveDocument = doc;
                            var db = doc.Database;

                            #endregion

                            PGA.DrawingManager.Commands.
                                CloseAllButActiveDocuments();

                            using (doc.LockDocument())
                            {
                                var name = "";
                                if (IsZDrawing(file.CompletePath))
                                    name = GetDWGPrefix(file.CompletePath, file.Hole);
                                else
                                    name = GetDWGPrefix(file.CompletePath, file.Hole);

                                LoopSurfaces(file.CompletePath,name,c.ToString(),shole,Is2013Version);
                            }
                        }
                    }
                    commands.SetDXFFlagExportToCad(file.Id);
                    commands.SetCompletedFlagExportToCad(file.Id);  
                }
                commands.DeleteAllExportToCadRecords();
            }
        }
        public static bool IsZDrawing(string file)
        {
            return Regex.IsMatch
                (file, "z-", RegexOptions.IgnoreCase);
        }

        public static string ModifyDwgName(string file)
        {
            var dir = Path.GetDirectoryName(file);
            var fi = Path.GetFileName(file);
            var complete = String.Format("ACAD-{0}", fi);
            complete = Path.Combine(dir, complete);

            return complete;
        }

        public static DBObjectCollection ExtractFaces(TinSurface surf)
        {
            var faces = new DBObjectCollection();
            foreach (var triangle in surf.GetTriangles(false))
            {
                var face = new Face(triangle.Vertex1.Location, triangle.Vertex2.Location, triangle.Vertex3.Location,
                    true, true, true, true);
                faces.Add(face);
            }
            return faces;
        }

        public static void CreateFaces(DBObjectCollection faces, string filename,Autodesk.AutoCAD.DatabaseServices.Database db)
        {
           
            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    foreach (var face in faces)
                    {
                        PGA.AcadUtilities.AcadDatabaseManager.AddToDatabase(db, face as Face, trans);
                    }
                    trans.Commit();
                }
                //db.Save();
            }
            catch
            {
                //db?.Dispose();
            }
        }

        public static ObjectIdCollection GetAllSurfaces()
        {
            ObjectIdCollection collection = GetIdsByTypeTypeValue(
                                                   "AECC_TIN_SURFACE");
            return collection;
        }

        public static void AsyncDXFCall(string originalname, string filename, string coursecode, string hole,
            bool Is2013DXF, Autodesk.AutoCAD.DatabaseServices.Database db)
        {
            if (String.IsNullOrEmpty(filename))   throw new ArgumentNullException(nameof(filename));
            if (String.IsNullOrEmpty(coursecode)) throw new ArgumentNullException(nameof(coursecode));

            string name = "";
            var doc = Application.DocumentManager.MdiActiveDocument;
            //Autodesk.AutoCAD.DatabaseServices.Database db =
            //    Application.DocumentManager.MdiActiveDocument.Database;
            try
            {

               //Todo: A possible bug in IsZDrawing DXF
               //Todo: originalname to name ?

                if (IsZDrawing(filename))
                {  
                    name = string.Format("z_{0}_{1}.dxf", coursecode, hole);
                    filename = Path.Combine(Path.GetDirectoryName(filename), name);
                    COMS.AddLog("Creating file Z-Surface DXF: " + name);
                    originalname = filename;
                }
                else
                    COMS.AddLog("Creating file S-Surface DXF: " + name);

                if (!Is2013DXF)
                {                 
                    db.DxfOut(originalname.Replace(".DWG", ".dxf"), 16, DwgVersion.Current);
                }
                else
                {
                    db.DxfOut(originalname.Replace(".DWG", ".dxf"), 16, DwgVersion.AC1027);
                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }

        }

        public static void LoopSurfaces(string originalname,string filename,string coursecode,string hole,bool Is2013DXF)
        {
            string name = "";
            Autodesk.AutoCAD.DatabaseServices.Database db =
                 Application.DocumentManager.MdiActiveDocument.Database;

            try
            {

                //change working database
                HostApplicationServices.WorkingDatabase = db;
                //select objects from that database
                ObjectIdCollection collection = GetAllSurfaces();

                //save objects to side database
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {

                    foreach (ObjectId surfaceId in collection)
                    {
                        TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;
                        var facecol = ExtractFaces(surface);
                        CreateFaces(facecol, filename, db);
                    }
                    trans.Commit();
                }
               
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    db.SaveAs(filename, DwgVersion.Current);
                    trans.Commit();
                }
                if (IsZDrawing(filename))
                {
                    name = string.Format("z_{0}_{1}.dxf", coursecode, hole);
                    filename = Path.Combine(Path.GetDirectoryName(filename), name);
                }
                if (!Is2013DXF)
                {
                    db.DxfOut(filename.Replace(".DWG", ".dxf"), 16, DwgVersion.Current);
                }
                else
                {
                    db.DxfOut(filename.Replace(".DWG", ".dxf"), 16, DwgVersion.AC1027);

                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }

        }

        public static ObjectIdCollection GetIdsByTypeTypeValue(params string[] types)
        {

            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;

            // Get the editor to make the selection
            Editor oEd = doc.Editor;

            // Add our or operators so we can grab multiple types.
            IList<TypedValue> typedValueSelection = new List<TypedValue> {
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

        //public static DBObjectCollection ExtractFaces(this TinSurface surf)
        //{
        //    var faces = new DBObjectCollection();
        //    foreach (var triangle in surf.GetTriangles(false))
        //    {
        //        var face = new Face(triangle.Vertex1.Location, triangle.Vertex2.Location, triangle.Vertex3.Location,
        //            true, true, true, true);
        //        faces.Add(face);
        //    }
        //    return faces;
        //}

        //public static void ExtractContours(ObjectId surfaceId)
        //{
        //    Autodesk.AutoCAD.DatabaseServices.Database db =
        //        Application.DocumentManager.MdiActiveDocument.Database;
        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

        //        // Extract contours from the TinSurface
        //        ObjectIdCollection contourIds;
        //        double contourInterval = 10.0;
        //        contourIds = surface.ExtractContours(contourInterval);
        //        for (int i = 0; i < contourIds.Count; i++)
        //        {
        //            ObjectId contourId = contourIds[i];
        //            // Contours are lightweight Polyline objects:

        //            DBObject contour = contourId.GetObject(OpenMode.ForWrite);
        //            Polyline c = (Polyline) contour;
        //            // Let's set a color and highlight the extracted polylines
        //            //contour.ColorIndex = i + 1;
        //            //contour.Highlight();

        //        }

        //        // Extract Triangles from the TinSurface
        //        ObjectIdCollection triangleIds;
        //        triangleIds = surface.ExtractGridded(SurfaceExtractionSettingsType.Model);
        //        for (int i = 0; i < triangleIds.Count; i++)
        //        {
        //            ObjectId triangleId = triangleIds[i];
        //            // Contours are lightweight Polyline objects:
        //            Face contour = triangleId.GetObject(OpenMode.ForWrite) as Face;
        //            // Let's set a color and highlight the extracted polylines
        //            //contour.ColorIndex = i + 1;
        //            //contour.Highlight();

        //        }

        //    }
        //}

        //public static void ExtractBorder(ObjectId surfaceId)
        //{
        //    Autodesk.AutoCAD.DatabaseServices.Database db =
        //        Application.DocumentManager.MdiActiveDocument.Database;
        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

        //        // Extract Border from the TinSurface
        //        // The extracted entities can be Polyline, Polyline3d, or Face.

        //        ObjectIdCollection entityIds;
        //        entityIds = surface.ExtractBorder(Autodesk.Civil.SurfaceExtractionSettingsType.Model);
        //        for (int i = 0; i < entityIds.Count; i++)
        //        {
        //            ObjectId entityId = entityIds[i];
        //            if (entityId.ObjectClass == RXClass.GetClass(typeof (Polyline3d)))
        //            {
        //                Polyline3d border = entityId.GetObject(OpenMode.ForWrite) as Polyline3d;
        //                // Do what you want with the extrated 3d-polyline
        //            }

        //        }

        //    }
        //}

        double Area(Point3d p1, Point3d p2, Point3d p3)
        {
            return Math.Abs(
                (((p2.X - p1.X) * (p3.Y - p1.Y)) -
                ((p3.X - p1.X) * (p2.Y - p1.Y))) / 2.0);
        }

        private void Triangulate(ObjectId id)
        {
            if (id.ObjectClass != RXClass.GetClass(typeof (Face)))
                return;
            using (Transaction tr = id.Database.TransactionManager.StartTransaction())
            {
                Face f1 = (Face) tr.GetObject(id, OpenMode.ForRead);
                Point3d[] vertices = new Point3d[4];
                for (short i = 0; i < 4; i++)
                {
                    vertices[i] = f1.GetVertexAt(i);
                }
                if (vertices[2].IsEqualTo(vertices[3]))
                    return;
                f1.UpgradeOpen();
                Face f2;
                if (vertices[0].DistanceTo(vertices[2]) < vertices[1].DistanceTo(vertices[3]))
                {
                    f1.SetVertexAt(3, vertices[2]);
                    f2 = new Face(vertices[0], vertices[2], vertices[3], true, true, true, true);
                }
                else
                {
                    f1.SetVertexAt(2, vertices[3]);
                    f2 = new Face(vertices[1], vertices[2], vertices[3], true, true, true, true);
                }
                BlockTableRecord btr = (BlockTableRecord) tr.GetObject(f1.OwnerId, OpenMode.ForWrite);
                btr.AppendEntity(f2);
                tr.AddNewlyCreatedDBObject(f2, true);
                tr.Commit();
            }
        }
    }
}

