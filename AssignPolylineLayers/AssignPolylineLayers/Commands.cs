using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGA.Database;

namespace AssignPolylineLayers
{
    using global::Autodesk.AutoCAD.ApplicationServices;
    using global::Autodesk.AutoCAD.DatabaseServices;
    using global::Autodesk.AutoCAD.EditorInput;
    using global::Autodesk.AutoCAD.Runtime;
    using Application = global::Autodesk.AutoCAD.ApplicationServices.Core.Application;


    public static class Commands
    {
        [CommandMethod("PGA-ChangeLayers2")]
        public static void ChangeLayers()
        {
            // Get the document
            try
            {
                var doc = Application.DocumentManager.MdiActiveDocument;

                using (DocumentLock dlock = doc.LockDocument())
                {
                    
                    ObjectIdCollection collection = Layers.GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3D");

                    Layers.ProcessLayers(collection);

                }
            }
            catch (System.Exception ex)
            {
                DatabaseLogs.FormatLogs(string.Format("ChangesLayers: {0}", ex.Message));
            }
        }


    }
}
