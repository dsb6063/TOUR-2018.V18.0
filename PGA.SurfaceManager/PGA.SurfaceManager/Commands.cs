using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using COMS=PGA.MessengerManager;

namespace PGA.SurfaceManager
{
    public static class Commands
    {
        [CommandMethod("PGA-CheckSurfaceForErrors", CommandFlags.UsePickSet)]
        public static void CheckSurfaceForErrors()
        {

            try
            {
                var theSurfaces = SurfaceManager.GetSurfacesList();

                //PGA.SportVision.Program.RegisterPolys();

                foreach (var theSurface in theSurfaces)
                {
                    var t   = SurfaceManager.GetTerrainSurfaceProperties(theSurface);
                    var tin = SurfaceManager.GetTinSurfaceProperties(theSurface);
 

                    SurfaceManager.gCount = 0;
                    SurfaceManager.gError = 0;
                    SurfaceManager.gOuterArea = t?.SurfaceArea2D ?? 0.0;

                    COMS.MessengerManager.AddLog
                      ("Audit Zero Areas: " + theSurface.Name);

                        SurfaceManager.ProcessSurfaceOps(theSurface);                       
                       // SurfaceManager.DisableSurface(theSurface);
                       // SurfaceManager.DisableSurfaceByArea(theSurface);

                        theSurface.Rebuild();


                }
                Active.Editor.Regen();
            }
        
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

    }
}
