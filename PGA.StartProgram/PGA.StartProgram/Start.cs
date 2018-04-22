using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Interop.Common;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using PGA.Database;

namespace PGA.StartProgram
{
    public  class Start
    {
        [CommandMethod("PGA-STARTCOALESCING", CommandFlags.Session)]
        public static void StartProgram()
        {
            try
            {
                CourseCoalesceProject.Coalesce.LoadandProcessPolys();
            }
            catch (System.Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }
    }
}
