using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using System.Linq;
using Acaddb = Autodesk.AutoCAD.DatabaseServices;
using Process = ProcessPolylines.ProcessPolylines;
namespace PGA.WindingNumAlgorithm
{
    public static class SelectionManager
    {

        public static ObjectIdCollection GetAllPolylines()
        {
            return Process.GetIdsByTypeTypeValue(
                "POLYLINE",
                "LWPOLYLINE",
                "POLYLINE2D");
        }


        public static IOrderedEnumerable<Acaddb.Polyline> OrderPolylines(Acaddb.ObjectIdCollection polyoId)
        {
            var ordered = new List<Acaddb.Polyline>();
            foreach (Acaddb.ObjectId i in polyoId)
            {
                using (var tr = Active.StartTransaction())
                {
                    ordered.Add(tr.GetObject(i, Acaddb.OpenMode.ForWrite) as Acaddb.Polyline);
                }
            }

            return ordered.OrderBy(p => p.Area);
        }


    }
}
