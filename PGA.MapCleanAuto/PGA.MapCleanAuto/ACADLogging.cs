#define TRACE
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
//using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.Runtime;
//using acApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Exception = System.Exception;
//using BBC.Common.AutoCAD;
using SystemLogger = System.Diagnostics;


namespace PGA.Common.Logging
{
    public static class ACADLogging
    {

        // Create a trace listener for the event log.
        static  SystemLogger.EventLogTraceListener myTraceListener = new SystemLogger.EventLogTraceListener("c:\\myEventLogSource.txt");
        static SystemLogger.TextWriterTraceListener myTextListener = null;
        static Stream myFile = null;
        //public static void LogException(string message)
        //{
        //    BBC.Common.AutoCAD.AcadUtilities.WriteMessage("");
        //}
        public static bool LogMyExceptions(string message, Exception ex)
        {
            string time = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            if (message != null)
            {
                SystemTracerLogMyExceptions(String.Format("{0} {1}\n", time, message));
            }
            return true;
        }
        public static void LogMyExceptions(string message)
        {
            //Write to File
            string time = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            if (message != null)
            {
                LogTextTraceWriter(String.Format("{0} {1}\n", time, message));
            }
        }
        //public static void LogMyExceptionsToCommandline(string message)
        //{
        //    //Write to Commandline
        //    string time = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        //    if (message != null)
        //    {
        //        LogMyException(String.Format("{1} {2}\n", time, message));
        //    }
        //}

        private static void SystemTracerLogMyExceptions(string message)
        {

            // Add the event log trace listener to the collection.
            SystemLogger.Trace.Listeners.Add(myTraceListener);

            // Write output to the event log.
            SystemLogger.Trace.WriteLine(message);

        }

        public static bool LogMyExceptions(string message, string exception)
        {
            //Write to File
            string time = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            if (message != null)
            {
                SystemTracerLogMyExceptions(String.Format("{1} {2} {3}\n", time, message, exception));
            }
            return true;
        }

        public static int LogTextTraceWriter(string message)
        {
            string filename = "c:\\PGALogFile.txt";
            //string time = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            if (!String.IsNullOrEmpty(message))
                message = string.Format("{0}\n", message);
              
            // Create a file for output named TestFile.txt.
            if (myFile == null)
            {
                myFile = File.Create(filename, 4096, FileOptions.RandomAccess);
            }
            if (myTextListener == null)
            {
            /* Create a new text writer using the output stream, and add it to
             * the trace listeners. */
                myTextListener = new
                    SystemLogger.TextWriterTraceListener(myFile);
                SystemLogger.Trace.Listeners.Add(myTextListener);
            }
            // Write output to the file.
            SystemLogger.Trace.WriteLine(message);


            // Flush the output.
            SystemLogger.Trace.Flush();

            return 0;
        }

    }

}
