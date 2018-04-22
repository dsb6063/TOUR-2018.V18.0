using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using Exception = System.Exception;

namespace PGA.NewTemplate
{
    public class QNew
    {

        [CommandMethod("PGA-SETQNEW", CommandFlags.Session)]

        public static void SetQNewTemplate()
        {
            try
            {
                MessengerManager.MessengerManager.AddLog("Set QNew Template.");

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var app = AcadApplication as AcadApplication;

                    var acad = AcadPreferencesFilesClass;//Preferences;
                    var files = acad as AcadPreferencesFiles;
                    files.QNewTemplateFile = commands.GetTemplatePath();
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.LogException(ex);
            }
        }
    }
}
