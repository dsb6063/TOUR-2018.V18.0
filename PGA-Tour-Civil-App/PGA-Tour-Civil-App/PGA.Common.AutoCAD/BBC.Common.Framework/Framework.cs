using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
//using Common.Logging;
//using Common.Logging.Log4Net;

namespace Pge.Common.Framework
{
    public static class Framework
    {
        #region Member Variables

        /// <summary>
        ///     This is the logger
        /// </summary>
        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(Framework));
        private const string BATCH_PLOT_PATH = @"C:\Program Files\mapping2009\gems\support\BatchPlot.exe";
        #endregion
        

        #region Public Methods

        /// <summary>
        /// Launches AutoCAD with the specified arguments
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static bool LaunchAutoCAD(string profile, string scriptFile, string workingFolder, string support, bool showLogo)
        {
            StringBuilder args = new StringBuilder();
            
            if (!String.IsNullOrEmpty(profile))
            {
                args.Append("/p " + profile + " ");                
            }

            if (!String.IsNullOrEmpty(scriptFile))
            {
                args.Append("/b " + scriptFile + " ");
            }

            if (!String.IsNullOrEmpty(workingFolder))
            {
                args.Append("/f " + workingFolder + " ");
            }

            if (!String.IsNullOrEmpty(support))
            {
                args.Append("/s" + support + " ");
            }

            if (showLogo == false)
            {
                args.Append("/nologo");
            }

            string fileName = BBC.Common.Framework.Properties.Settings.Default.AUTOCAD_PATH;
            string arguments = args.ToString();

            return ProcessMananger.LaunchProcess(fileName, arguments, string.Empty, false);
        }

        /// <summary>
        /// Detemines if an instance of AutoCAD is already running
        /// </summary>
        /// <returns></returns>
        public static bool IsAcadRunning
        {
            get
            {
                string acadLocation = RegistryUtilities.AcadLocation;

                if (null != acadLocation)
                {
                    string acadPath = FileUtilities.Combine(acadLocation, @"acad.exe");

                    return ProcessMananger.FindProcessByName(@"acad", acadPath);
                }

                throw new ApplicationException("Could not find AcadLocation in the Registry.");
            }
        }

        /// <summary>
        /// Detemines if an instance of AutoCAD is already running
        /// </summary>
        /// <returns></returns>
        public static bool IsBatchPlotRunning
        {
            get
            {
                string fileName = FileUtilities.GetFileName(BATCH_PLOT_PATH, false);
                return ProcessMananger.FindProcessByName(fileName, BATCH_PLOT_PATH);
            }
        }

        #endregion
    }
}
