using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace PGA.SportVisionAuto
{
    public static class Program
    {
        public static void OpenAutoCAD(string filename)
        {
            var doc = Application.DocumentManager.Open(filename, true);
            Application.DocumentManager.MdiActiveDocument = doc;
            var db = doc.Database;
            var ed = doc.Editor;
            using (doc.LockDocument())
            {
                var name = Path.GetFileName(filename);
                Application.SetSystemVariable("FILEDIA", 0);
                Application.SetSystemVariable("NOMUTT", 0);
                Application.SetSystemVariable("CMDECHO", 0);

                doc.SendStringToExecute(".PGA-StartSportVision" + "\n", true, false, false);

            }
        }
    }
}
