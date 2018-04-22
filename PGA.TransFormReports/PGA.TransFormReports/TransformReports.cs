using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGA.Database;
using PGA.DataContext;
using COMS = PGA.MessengerManager.MessengerManager;

namespace PGA.TransFormReports
{
    public class TransformReports
    {
        private IEnumerable<Logs> _logs = null; 
        private List<Logs> _alldata  = new List<Logs>();
        List<Logs> _drawings;
        List<Logs> _result;
        List<Logs> _errors;

        public TransformReports()
        {
            _drawings = GetDrawings();

            if (_drawings == null)
                return;

            if (_drawings.FirstOrDefault() != null)
            {
                _result = GetStartStopPoint(_drawings);
                _errors = FiltersByAllErrors(_result);


                if (_result.FirstOrDefault() == null)
                    return;

                GetDrawingName(_result);
            }

        }


        public TransformReports(PGA.DataContext.Logs logs)
        {
            var masterdump = new List<Logs>();

            if (masterdump.FirstOrDefault() == null)
                return;

            var drawings = GetDrawings();
            var result = GetStartStopPoint(drawings);
            var errors = FiltersByAllErrors(result);

            GetDrawingName(result);
        }

        enum ErrorType
        {
            Acad,
            Exception,
            Alert,
            Error,
            Locked,
            Runtime
        };

        public List<Logs> GetDrawings()
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var logs = _logs = commands.GetLastAllLogs();

                    if (logs.FirstOrDefault() == null)
                        return null;

                    _drawings = new List<Logs>(logs.Where(l => l.Issue.Contains(".DWG") && (l.Issue.Contains("Process Drawing"))).OrderBy(o => o.Id));
                }

                if (_drawings.FirstOrDefault() == null)
                    return null;
                else
                    return _drawings.ToList();
            }
            catch (Exception ex)
            {
                COMS.LogException(ex);
            }
            return null;
        }

        public List<Logs> GetStartStopPoint(List<Logs> logs)
        {
            var first = logs.OrderBy(p=>p.Id).FirstOrDefault().Id;
            var last  = logs.OrderBy(p=>p.Id).LastOrDefault().Id;

            var result = _logs.Where(l=>l.Id <= last && l.Id >= first);
            return result.ToList();
        }

        public List<Logs> FiltersByErrorType(List<Logs> logs, string type)
        {
            var result = _logs.Where(p =>
                         p.Issue.Contains(type));

            return result.ToList();
        }

        public List<Logs> FiltersByAllErrors(List<Logs> logs)
        {
            var result = logs.Where(p => p.Issue.Contains("Alert") ||
                              p.Issue.Contains("Exception") ||
                              p.Issue.Contains("Error") ||
                              p.Issue.Contains("Locked") ||
                              p.Issue.Contains("Runtime"));

            return result.ToList();
        }

        public string GetDrawingName(List<Logs> logs)
        {
            var log   = logs.FirstOrDefault();
            var array = log.Issue.Split(new char[] { ':' }, 4);
            if (!String.IsNullOrEmpty(array[3]))
               return array[3].Trim();
            return "";
        }

        public Collection<List<Logs>> FiltersCombinedAllErrors(List<Logs> logs)
        {
            var localdump  = new List<Logs>();
            var collection = new Collection<List<Logs>>();

            var dwgs = logs.Where(l => (l.Issue.Contains(".DWG") || l.Issue.Contains(".dwg") )&&
                          (l.Issue.Contains("Process Drawing")))
                          .OrderBy(o => o.Id);


            for (int i = 0; i < dwgs.Count(); i++)
            {
                if (i%2 == 0 && ((i + 2) <= dwgs.Count()))
                {
                    var tworecs = dwgs.Skip(i).Take(2);
                    var name = GetDrawingName(tworecs.ToList());
                    var first = tworecs.FirstOrDefault().Id;
                    var last  = tworecs.LastOrDefault().Id;

                    var bounded = _logs.Where(l => l.Id <= last && l.Id >= first);

                    var result  = bounded.Where(p => p.Issue.Contains("Alert") ||
                                                    p.Issue.Contains("Exception") ||
                                                    p.Issue.Contains("Error") ||
                                                    p.Issue.Contains("Locked") ||
                                                    p.Issue.Contains("Runtime"));
                    result.All(p =>
                    {
                        if (String.IsNullOrEmpty(p.Source))
                        {
                            p.Source = name;
                            return true;
                        }
                        return false;
                    });

                    if (result.FirstOrDefault() != null)
                        collection.Add(result.ToList());
                }
            }

            return collection;
        }

        public void WriteReport(List<Logs> errors)
        {
            try
            {
                COMS.AddLog("*****************Batch Errors********************");

                errors.All(p =>
                {

                    COMS.AddLog(String.Format("{0}: {1}", p.Source ?? "No Source Drawing Information", p.Issue));
                    return true;

                });
                COMS.AddLog("*****************Batch Errors********************");

            }
            catch (Exception ex)
            {
                COMS.LogException(ex);
            }
        }
    }
}
