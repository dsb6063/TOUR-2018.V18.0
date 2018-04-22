using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace QueueManager
{
 
    public class Commands
    {

        private static bool _launched = false;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        [CommandMethod("PGA-ShowQueueManager")]
        public static void Main()
        {
            ShowWindow();
        }

        private static void ShowWindow()
        {
            PGA.DatabaseManager.QueueManager _window = null;

            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.
                DocumentManager.MdiActiveDocument;

            if (doc == null)
                return;

            if (_window == null)
            {
                _window = new PGA.DatabaseManager.QueueManager(true);
                _window.ShowDialog();
            }
        }

        public static void StartDWG()
        {

            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
                return;

            // Add our command prefixing event handler

            _launched = false;

            doc.UnknownCommand += OnUnknownCommand;
            doc.CommandWillStart += OnCommandSomething;
            doc.CommandEnded += OnCommandSomething;
           
            // autoComplete and autocorrect cause problems with
            // this, so let's turn them off (we may want to warn
            // the user or reset the values, afterwards)

            Application.SetSystemVariable("NOMUTT", 1);
            Application.SetSystemVariable("CMDECHO", 0);

            doc.SendStringToExecute("._PGA-StartNotes\n", true, false, false);
            doc.SendStringToExecute("._PGA-StartCoalescing\n", true,false,false);
        }

        private static void OnCommandSomething(object sender, CommandEventArgs e)
        {
        }

        private static void OnUnknownCommand(object sender, UnknownCommandEventArgs e)
        {
        }
    }
}
