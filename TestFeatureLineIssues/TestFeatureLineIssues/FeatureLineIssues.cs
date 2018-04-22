using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil;
using BBC.Common.AutoCAD;

namespace TestFeatureLineIssues
{
    public class FeatureLineIssues
    {
        public static void AddStandardBoundary(ObjectId polyId, string surfacename,
    Autodesk.Civil.DatabaseServices.TinSurface surface)
        {
            Polyline poly = null;

            try
            {
                using (Transaction tr = CivilApplicationManager.StartTransaction())
                {

                    poly = polyId.GetObject(OpenMode.ForRead) as Polyline;

                    var points = AcadUtilities.GetPointsFromPolyline(poly);

                    Autodesk.Civil.DatabaseServices.SurfaceDefinitionBoundaries surfaceBoundaries =
                        surface.BoundariesDefinition;
                    if (points.Count == 0)
                    {
                        //throw new ArgumentException(
                        //    String.Format("Len={0} Nodes={1} Layer={2} Oid={3}",
                        //    poly.Length.ToString(),
                        //    poly.NumberOfVertices.ToString(),
                        //    poly.Layer.ToString(),
                        //    poly.ObjectId.ToString()), "AddStandardBoundary");
                    }
                    else
                    {
                        surfaceBoundaries.AddBoundaries(points, 1.0, SurfaceBoundaryType.Hide, true);
                    }
                    tr.Commit();
                }
            }
            catch { }
        }


    }
}
