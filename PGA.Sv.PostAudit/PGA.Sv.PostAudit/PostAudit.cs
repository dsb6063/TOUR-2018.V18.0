using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Interop.Common;
using Autodesk.AutoCAD.Runtime;
using PGA.Database;
using PGA.DataContext;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Exception = System.Exception;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using Acad = Autodesk.AutoCAD.DatabaseServices;

namespace PGA.Sv.PostAudit
{
    public class PostAudit
    {
        private ISettings _settings = null;
        private ILogger _logger = null;
        private IDatabase _database = null;
        private DateTime? _datetime = null;
        private string _directory = null;
        private string _cdir = null;
        private string _sdir = null;
        private string _pdir = null;
        private string _tourcode = String.Empty;

        public PostAudit()
        {
            try
            {
                _database = new DataBaseRepository();
                _settings = new SettingsRepository();
                _logger = new LoggerRepository();
                _datetime = null;
                _directory = String.Empty;
                _cdir = String.Empty;
                _sdir = String.Empty;
                _pdir = String.Empty;
                _tourcode = String.Empty;

                InitializeExportToCad();
            }
            catch (Exception)
            {

            }

        }

        public PostAudit(DateTime dt, string filepath)
        {
            _database = new DataBaseRepository();
            _settings = new SettingsRepository();
            _logger   = new LoggerRepository();
        }

        private void TestingExportToCad()
        {
            //_datetime= new DateTime?();
            //_directory = @"C:\Civil 3D Projects\2016Muirfield-Village\2016Muirfield-Village\SPORTVISION-20160624-194936";
            //_directory = @"C:\Users\m4800\Downloads\2016Bay-Hill\2016Bay-Hill\TINSURF-20160529-155100";
        }

        private void InitializeExportToCad()
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var records = commands.GetAllExportToCadRecords();

                    if (records.FirstOrDefault() != null)
                    {
                        _datetime  = records.Select(p => p.DateStamp)   .FirstOrDefault();
                        _directory = records.Select(p => p.CompletePath).FirstOrDefault();

                        if (_datetime != null) _settings.GetSettings(_datetime.Value);

                        //Get Top Root Directory for SportVision
                        _directory = GetSportVisionDir(_directory);
                        _logger.AddLog("Set Directory: " + _directory);
                    }
                    else
                    {
                        _logger.AddLog("Initializing Class Post Audit: " +
                                       "No Records in GetAllExportToCadRecords!");

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }


        public string GetSportVisionDir(string curdir)
        {
            var dirout = "";
            var split = curdir.Split(new char[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);

             foreach (var d in  split)
             {
                if (!String.IsNullOrEmpty(dirout))
                   dirout = Path.Combine(dirout, d);
                else
                  dirout = String.Format("{0}\\",d);

                if (d.Contains("SPORTVISION"))
                     return dirout;
                else if (d.Contains("TINSURF"))
                {
                    return dirout;
                }
            }

            return dirout;
        }

        [CommandMethod("PGA-POST-AUDIT")]
        public void ProcessFiles()
        {
            try
            {
                if (!CreateDirectories())
                    throw new FileNotFoundException("CreateDirectories!");
             
                foreach (string d in Directory.GetDirectories(_directory))
                {
                    if (Path.GetFileName(d).EndsWith("SURFACE")   ||
                        Path.GetFileName(d)==("POLYLINE") ||
                        Path.GetFileName(d).EndsWith("COMBINED"))
                        continue;

                    if (d.Contains("SUFACE") ||
                        d.Contains("POLYLINE") ||
                        d.Contains("COMBINED")
                       )
                        continue;
                    ProcessFilesInDirFiles(_pdir,d);
                    ProcessCombinedFiles(_cdir,d);
                    ProcessSurfaceFiles (_sdir,d);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

        }

        private void ProcessSurfaceFiles(string sdir, string directory)
        {
            try
            {
                var fileInDir = Directory.GetFiles(directory);
                var filtered = fileInDir.Where(p => p.Contains("ACAD-s"));
                var Rfile = GetRFile(fileInDir);
                DeletePoly2D3DObjects(filtered.FirstOrDefault(),sdir,Rfile);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private void ProcessFilesInDirFiles(string target, string directory)
        {

          
            try
            {
                var files = Directory.GetFiles(directory);
                var filtered = files.Where(p => p.Contains("ACAD-") && p.Contains("3DP"));
                var Rfile = GetRFile(files);
                var fileName = "";
                foreach (var source in filtered)
                {
                    if (System.IO.Directory.Exists(target))
                    {
                        if (String.IsNullOrEmpty(Rfile))
                            fileName = System.IO.Path.GetFileName(source);
                        else
                            fileName = Rfile;

                        fileName = String.Format("P-{0}.DWG", fileName);


                        var destFile = System.IO.Path.Combine(target, fileName);
                        System.IO.File.Copy(source, destFile, true);
                        SaveAsDxf(source, destFile);
                    }

                    else
                    {
                        _logger.AddLog("Target Directory Not Created!");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private void ProcessCombinedFiles(string target, string directory)
        {

      

            try
            {
                var files = Directory.GetFiles(directory);
                var filtered = files.Where(p => p.Contains("ACAD-s"));
                var Rfile = GetRFile(files);
                var fileName = "";
                foreach (var source in filtered)
                {
                    if (System.IO.Directory.Exists(target))
                    {
                        if (String.IsNullOrEmpty(Rfile))
                            fileName = System.IO.Path.GetFileName(source);
                        else
                            fileName = Rfile;

                        fileName = String.Format("C-{0}.DWG", fileName);


                        var destFile = System.IO.Path.Combine(target, fileName);
                        System.IO.File.Copy(source, destFile, true);

                        SaveAsDxf(source,destFile);
                    }

                    else
                    {
                        _logger.AddLog("Target Directory Not Created!");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        //public string GetRFile(string[] files)
        //{
        //    foreach (var file in files)
        //    {
        //        var filename = System.IO.Path.GetFileName(file);
        //        var ext = System.IO.Path.GetExtension(file);

        //        if (filename.StartsWith("R") && filename.Contains(ext))
        //        {
        //            //replace "R" with dynamic value
        //            var tourcode = _settings.GetSettings(_datetime.Value).TourCode;

        //            if (String.IsNullOrEmpty(tourcode))
        //                return Path.GetFileNameWithoutExtension(file);
        //            else
        //            {
        //                return Path.GetFileNameWithoutExtension(tourcode + file.Substring(1, file.Length - 1));
        //            }
        //        }
        //    }
        //    return string.Empty;
        //}

        public string GetRFile(string[] files)
        {
            //Replace "R" with dynamic value R201800001.dxf

            try
            {

                var file = files.ToList()
                    .Select(x => new FileInfo(x))
                    .Where((x => (x.Extension).ToLower() == ".dxf"))
                    .Where(x => !x.Name.Contains("_"))
                    .OrderByDescending(x => x.Length)
                    .LastOrDefault()
                    ?.Name;

                if (String.IsNullOrEmpty(file))
                    return string.Empty;

                var tourcode = _settings.GetSettings(_datetime.Value).TourCode;

                if (String.IsNullOrEmpty(tourcode))
                    return Path.GetFileNameWithoutExtension(file);

                var filename = System.IO.Path.GetFileName(file);
                var ext = System.IO.Path.GetExtension(file);

                return Path.GetFileNameWithoutExtension(tourcode + file.Substring(1, file.Length - 1));

            }
            catch (Exception ex)
            {

                _logger.LogException("GetRFile!", ex);
            }

            return string.Empty;
        }

        public bool CreateDirectories()
        {
            try
            {
                _logger.AddLog("Start Create Directories!");
                _logger.AddLog("Directory = " + _directory);
                var dirs = new List<string>();
                        
               _sdir  = Path.Combine(_directory, "SURFACE" );
               _cdir  = Path.Combine(_directory, "COMBINED");
               _pdir  = Path.Combine(_directory, "POLYLINE");
 
                dirs.Add(_sdir);
                dirs.Add(_cdir);
                dirs.Add(_pdir);

                foreach (var d in dirs)
                {
                    if (!Directory.Exists(d))
                    {
                        Directory.CreateDirectory(d);
                        _logger.AddLog(String.Format("{0}", d));
                    }

                }

                _logger.AddLog("End Create Directories!");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            return false;
        }


        private void DeletePoly2D3DObjects(string source, string dest, string drawingName)
        {

            bool eraseOrig = true;

            try
            {
                if (drawingName == null) throw new ArgumentNullException(nameof(drawingName));

                _logger.AddLog(String.Format("Drawing to Open: {0}", source));


                Autodesk.AutoCAD.DatabaseServices.Database oldDb = HostApplicationServices.WorkingDatabase;

                using (Autodesk.AutoCAD.DatabaseServices.Database db =
                    new Autodesk.AutoCAD.DatabaseServices.Database(false, true))
                {
                    db.ReadDwgFile(source, FileOpenMode.OpenForReadAndWriteNoShare, true, null);

                    db.CloseInput(true);

                    HostApplicationServices.WorkingDatabase = db;

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        // Collect our exploded objects in a single collection
                        var selected = BBC.Common.AutoCAD.AcadUtilities.
                            GetAllObjectIdsInModel(db, false);

                        DBObjectCollection objs = new DBObjectCollection();

                        // Loop through the selected objects

                        foreach (ObjectId so in selected)
                        {
                            // Open one at a time

                            Entity ent =
                                (Entity)tr.GetObject(
                                    so,
                                    OpenMode.ForRead
                                    );
;

                            if (ent.GetType() == typeof(Polyline3d) ||
                                ent.GetType() == typeof(Polyline2d) ||
                                ent.GetType() == typeof(Acad.Polyline) ||
                                ent.GetType() == typeof(Line)) 
                            {
                                if (eraseOrig)
                                {
                                    ent.UpgradeOpen();
                                    ent.Erase();
                                }
                            }
                        }
                        tr.Commit();
                    }
                  
                    var output = Path.Combine(_sdir, "S-" + drawingName + ".DWG");
                    var dxfout = output.Replace(".DWG", ".DXF");
                    db.SaveAs(output, DwgVersion.Current);

                    HostApplicationServices.WorkingDatabase = oldDb;

                    db.DxfOut(dxfout, 16, DwgVersion.Current);

                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private void SaveAsDxf(string source, string dest, string drawingName)
        {

            try
            {
                if (drawingName == null) throw new ArgumentNullException(nameof(drawingName));

                _logger.AddLog(String.Format("Drawing to Open: {0}", source));

                Autodesk.AutoCAD.DatabaseServices.Database oldDb = HostApplicationServices.WorkingDatabase;

                using (Autodesk.AutoCAD.DatabaseServices.Database db =
                    new Autodesk.AutoCAD.DatabaseServices.Database(false, true))
                {
                    db.ReadDwgFile(source, FileOpenMode.OpenForReadAndWriteNoShare, true, null);

                    db.CloseInput(true);

                    HostApplicationServices.WorkingDatabase = db;

                    var dxfout = Path.Combine(dest,drawingName.Replace(".DWG", ".DXF"));

                    HostApplicationServices.WorkingDatabase = oldDb;

                    db.DxfOut(dxfout, 16, DwgVersion.Current);

                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private void SaveAsDxf(string source, string dest)
        {

            try
            {
                if (dest == null) throw new ArgumentNullException(nameof(dest));

                _logger.AddLog(String.Format("Drawing to Open: {0}", source));

                Autodesk.AutoCAD.DatabaseServices.Database oldDb = HostApplicationServices.WorkingDatabase;

                using (Autodesk.AutoCAD.DatabaseServices.Database db =
                    new Autodesk.AutoCAD.DatabaseServices.Database(false, true))
                {
                    db.ReadDwgFile(source, FileOpenMode.OpenForReadAndWriteNoShare, true, null);

                    db.CloseInput(true);

                    HostApplicationServices.WorkingDatabase = db;

                    var filename = Path.GetFileName(dest);
                    var destPath = Path.GetDirectoryName(dest);

                    var dxfout = Path.Combine(destPath, filename.Replace(".DWG", ".DXF"));

                    HostApplicationServices.WorkingDatabase = oldDb;

                    db.DxfOut(dxfout, 16, DwgVersion.Current);

                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

    }

}
