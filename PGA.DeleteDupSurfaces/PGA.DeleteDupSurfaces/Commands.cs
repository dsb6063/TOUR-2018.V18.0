using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace PGA.DeleteDupSurfaces
{
    public  class Commands
    {

        [CommandMethod("PGA-DeleteDupSurfacesV1",CommandFlags.Session)]
        public void CommandDeleteSurfaces()
        {
            try
            {
                DeleteDupSurfaces.RemoveDuplicateSurfaces_V1();
            }
            catch (System.Exception)
            {

            }
        }

 
        [CommandMethod("PGA-DeleteDupSurfacesV2",CommandFlags.Session)]
        public void CommandDeleteSurfacesV2()
        {
            try
            {
                DeleteDupSurfaces.RemoveDuplicateSurfaces_V2();

            }
            catch (System.Exception)
            {

            }
        }
    }
}
