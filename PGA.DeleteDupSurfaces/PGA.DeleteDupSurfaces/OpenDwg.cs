using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
namespace PGA.DeleteDupSurfaces
{
    public class OpenDwg
    {

        public static string file = @"C:\Users\Daryl-Win-8\Downloads\BayHill_01\BayHill_01\BAY-HILL01.DWG";

        public static Document OpenDwgForWork(string filename)
        {
            var doc = Application.DocumentManager.Open(filename, true);
            Application.DocumentManager.MdiActiveDocument = doc;
            var db = doc.Database;
            var ed = doc.Editor;
            using (doc.LockDocument())
            {
                return doc;
            }
        }
    }
}
