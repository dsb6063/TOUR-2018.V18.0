using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices.Core;
using global::Autodesk.AutoCAD.DatabaseServices;

namespace CreateTINSurfaceFromCloud.ACAD
{
    class AcadApplictionDocument
    {
        public static Autodesk.AutoCAD.ApplicationServices.Document GetMdiDocument()
        {
            return Application.DocumentManager.MdiActiveDocument;
        }

        public static Transaction GetTransaction()
        {
            return HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction();
        }

    }
}
