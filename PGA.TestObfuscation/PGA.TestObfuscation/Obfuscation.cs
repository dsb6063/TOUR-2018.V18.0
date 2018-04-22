using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autodesk.AutoCAD.Runtime;

[assembly: Obfuscation(Feature = "debug", Exclude = false)]
namespace PGA.TestObfuscation
{
    public class Obfuscation
    {
        [CommandMethod("PGA-STARTCOALESCING", CommandFlags.Session)]
        public static void Test()
        {
            try
            {
                PGA.CourseCoalesceProject.Coalesce.LoadandProcessPolys();

            }
            catch (System.Exception)
            {

                throw;
            }        }

    }
}
