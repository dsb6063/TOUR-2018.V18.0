using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.AutoCAD.Runtime;
using PGA.Database;
using PGA.DataContext;
using PGA.MessengerManager;
using COMS= PGA.MessengerManager.MessengerManager;

namespace PGA.TransFormReports
{
    public class Commands
    {
        [CommandMethod("PGA-GetReports")]
        public void GetReports()
        {
            TransformReports report = new TransformReports();

            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var logs   = commands.GetLastAllLogs();
                    var errors = report.FiltersCombinedAllErrors(logs.ToList());

                    foreach (var items in errors)
                      report.WriteReport(items);
                }
            }
            catch (System.Exception ex)
            {
                COMS.LogException(ex);
            }
        }


        public static Collection<List<Logs>> GetReportsToWrite()
        {
            TransformReports report = new TransformReports();

            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var logs   = commands.GetLastAllLogs();
                    var errors = report.FiltersCombinedAllErrors(logs.ToList());

                    return errors; 
                }
            }
            catch (System.Exception ex)
            {
                COMS.LogException(ex);
            }
            return null;
        }
    }
}
