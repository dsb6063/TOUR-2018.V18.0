using System;
using System.Collections.Generic;
using System.Text;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;

using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CivilSurface = Autodesk.Civil.DatabaseServices.Surface;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry; //conflicts with Autodesk.AutoCAD.DatabaseServices.Surface

[assembly: CommandClass(typeof(C3DSurfacesDemo.TinSurfaceSample))]

namespace C3DSurfacesDemo
{
    public class TinSurfaceSample
    {
        [CommandMethod("testCreatePlateau")]
        public static void testCreatePlateau()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //select a surface
            PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a tin surface: ");
            selSurface.SetRejectMessage("\nOnly tin surface allowed");
            selSurface.AddAllowedClass(typeof(TinSurface), true);
            PromptEntityResult resSurface = ed.GetEntity(selSurface);
            if (resSurface.Status != PromptStatus.OK) return;
            ObjectId surfaceId = resSurface.ObjectId;

            //select a polyline
            PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a polyline: ");
            selPline.SetRejectMessage("\nOnly polylines allowed");
            selPline.AddAllowedClass(typeof(Polyline), true);
            PromptEntityResult resPline = ed.GetEntity(selPline);
            if (resPline.Status != PromptStatus.OK) return;
            ObjectId plineId = resPline.ObjectId;

            //erase the pline?
            PromptKeywordOptions selYesNo = new PromptKeywordOptions("\nErase polyline?");
            selYesNo.Keywords.Add("Yes");
            selYesNo.Keywords.Add("No");
            PromptResult resYesNo = ed.GetKeywords(selYesNo);
            if (resYesNo.Status != PromptStatus.OK) return;
            bool erasePline = resYesNo.StringResult.Equals("Yes");

            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                Polyline pline = trans.GetObject(plineId, OpenMode.ForWrite) as Polyline;
                
                //find all vertices inside a pline area
                ObjectIdCollection plinesBorder = new ObjectIdCollection();
                plinesBorder.Add(plineId);
                TinSurfaceVertex[] verticesInsidePline = surface.GetVerticesInsidePolylines(plinesBorder);

                //set the new elevation for all vertices founded
                surface.SetVerticesElevation(verticesInsidePline, pline.Elevation);

                //now create a surface vertex at each pline vertex
                for (int plineVertexIndex = 0; plineVertexIndex < pline.NumberOfVertices; plineVertexIndex++)
                {
                    //get the pline coordinate
                    Point3d plineVertex0 = pline.GetPoint3dAt(plineVertexIndex);
                    Point3d plineVertex1 = pline.GetPoint3dAt(plineVertexIndex < (pline.NumberOfVertices - 1) ? plineVertexIndex + 1 : 0);

                    //create a surface vertex at each pline vertex
                    //this will ensure that we have a vertex at each corner,
                    //which is required for the next step (AddLine)
                    SurfaceOperationAddTinVertex res0 = surface.AddVertex(plineVertex0);
                    SurfaceOperationAddTinVertex res1 = surface.AddVertex(plineVertex1);

                    //finally create a line connecting the newly creted vertices
                    TinSurfaceVertex vertex0 = surface.FindVertexAtXY(res0.Location.X, res0.Location.Y);
                    TinSurfaceVertex vertex1 = surface.FindVertexAtXY(res1.Location.X, res1.Location.Y);
                    surface.AddLine(vertex0, vertex1);
                }

                trans.Commit();
            }
        }

      
    }
}
