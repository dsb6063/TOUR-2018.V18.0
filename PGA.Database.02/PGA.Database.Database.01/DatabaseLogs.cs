#region

using System;
using PGA.DataContext;

#endregion

namespace PGA.Database
{
    public static class DatabaseLogs
    {
        public static void FormatLogs(DateTime datetime, string message, string dwgid)
        {
            AddLogs(datetime + " " + message, dwgid);
        }

        public static void FormatLogs(string message)
        {
            AddLogs(DateTime.Now + " " + message, "");
        }

        public static void FormatLogs(string message, string dwgid)
        {
            AddLogs(DateTime.Now + " " + message, dwgid);
        }

        public static void AddLogs(string log, string dwgid)
        {
            var path = GetDataBasePath.GetAppPath();

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = new Logs();

                logs.DateStamp = DateTime.Now.ToShortDateString();
                logs.DrawingID = dwgid;
                logs.Issue = log;
                context.Logs.InsertOnSubmit(logs);
                context.SubmitChanges();
            }
        }

        public static void AddExceptionLogs(Exception ex, string dwgid)
        {
            var path = GetDataBasePath.GetAppPath();

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var logs = new Logs();

                logs.DateStamp = DateTime.Now.ToShortDateString();
                logs.DrawingID = dwgid;
                logs.Issue = string.Format("{0} {1}", ex.TargetSite, ex.Message);
                context.Logs.InsertOnSubmit(logs);
                context.SubmitChanges();
            }
        }
    }
}