using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using PGA.Database;
using PGA.DataContext;

namespace PGA.ExportToAutoCAD
{
    public static class CreateDXF
    {
        public static void CreateDXFs()
        {
            List<ExportToCADStack> allExportToCadRecords = null;
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                allExportToCadRecords = commands.GetAllExportToCadRecords();

                foreach (var file in allExportToCadRecords)
                {
                    if (!GolfHoles.IsWithinLimits((int) file.Hole)) return;
                    var s = commands.GetSettingsByDate((DateTime)file.DateStamp);
                    var c = commands.GetSettingsCourseNumByDate
                        ((DateTime) file.DateStamp);
                    var year = DateTime.Now.Year;
                    var TOUR_Code = String.IsNullOrEmpty(s.TourCode) ? "R": s.TourCode;
                    var sv_filename = TOUR_Code + year + c +".dwg";
                    var shole = GolfHoles.GetHoles((int) file.Hole);
                    var modifiedpath = ModifyDwgName(file.CompletePath);
                    if (File.Exists(modifiedpath))
                    {
                        var doc = Application.DocumentManager.Open
                            (modifiedpath, true);
                        Application.DocumentManager.MdiActiveDocument = doc;
                        var db = doc.Database;
                        using (doc.LockDocument())
                        {
                            var name = Path.GetFileName
                                (file.CompletePath);
                            var dir = Path.GetDirectoryName
                                (file.CompletePath);

                            if (IsZDrawing(file.CompletePath))
                                name = string.Format("z_{0}_{1}.dwg", c, shole);
                            else if (s.SportVision == true)
                                name = sv_filename;
                            else
                                name = "ACAD-" + name;

                            name = Path.GetFileNameWithoutExtension(name);
                            name = name + ".dxf";
                            var completepath = Path.Combine
                                (dir, name);

                            db.DxfOut(completepath, 16, DwgVersion.Current);

                        }
                    }
                }
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
            var fi  = Path.GetFileName(file);
            var complete = String.Format("ACAD-{0}",fi);
            complete = Path.Combine(dir, complete);

            return complete;
        }
    }
}
