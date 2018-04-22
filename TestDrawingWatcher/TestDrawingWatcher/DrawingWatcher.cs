using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using PGA.DrawingWatcher;

namespace TestDrawingWatcher
{
    public class DrawingWatcher
    {
        public static TimerClass TM_Watcher;

       [CommandMethod("PGA-StartWatcher", CommandFlags.Session)]
        public static void StartWatcher()
        {
            TM_Watcher = new TimerClass();
        }

        [CommandMethod("PGA-StoptWatcher", CommandFlags.Session)]
        public static void StoptWatcher()
        {
            TimerClass.WrapUpOperations();
            TM_Watcher = null;
        }
    }
}
