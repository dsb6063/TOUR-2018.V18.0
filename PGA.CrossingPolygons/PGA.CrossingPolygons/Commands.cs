using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;

namespace PGA.CrossingPolygons
{
    public static class Commands
    {
        [CommandMethod("COR")]
        public static void CommandCentroidOfRegion()
        {
            CrossingPolygonsManager.CentroidOfRegion();
        }

        [CommandMethodAttribute("RTP")]
        public static void RegionToPolyline()
        {
            //CrossingPolygonsManager.RegionToPolyline();
            CrossingPolygonsManager.RegionToPolylineTest();
        }
    }
}
