// ***********************************************************************
// Assembly         : PGA.UpdateManager_1.0
// Author           : Daryl Banks, PSM
// Created          : 05-22-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 07-21-2016
// ***********************************************************************
// <copyright file="Updater_1.0.xaml.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PGA.Database;
using  PGA.MessengerManager;

namespace PGA.UpdateManager
{
    /// <summary>
    /// Interaction logic for Updater_1.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class Updater : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Updater" /> class.
        /// </summary>
        public Updater()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the name of the updater.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        private string GetUpdaterName()
        {
            string filename = @"Autodesk\ApplicationPlugins\PGA-CivilTinSurf2018.bundle\" +
                              @"DB_Loader\Bin\Update\BBC.CloudManager.exe";


            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            PGA.MessengerManager.MessengerManager.AddLog("Set Path: " + System.IO.Path.Combine(path, filename));


            if (File.Exists(System.IO.Path.Combine(path, filename)))
            {
                PGA.MessengerManager.MessengerManager.AddLog("File Found: " + System.IO.Path.Combine(path, filename));
                return System.IO.Path.Combine(path, filename);
            }
            else
            {
                PGA.MessengerManager.MessengerManager.AddLog("FileNotFound: " + System.IO.Path.Combine(path, filename));
                throw new FileNotFoundException(System.IO.Path.Combine(path, filename));
            }
        }


        /// <summary>
        /// Handles the Click event of the cmdUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void cmdUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    if (CheckIsInstalled())
                        commands.SetClearFlagsInUpdate();
                    else if (HasNoRecord())
                    {
                         PGA.MessengerManager.MessengerManager.AddLog(
                            "If you cleared the Database, then Restart the Database Manager!");
                         PGA.MessengerManager.MessengerManager.AddLog("Contacting server to download latest update!");
                    }

                    if (!commands.GetUpdateDownloadFlag() || commands.GetUpdateCurrentVersionMD5() == null)
                    {
                        Process updateProcess = new Process();
                        ConfigUpdaterProperties(updateProcess);
                    }
                    else if (commands.GetUpdateDownloadFlag())
                    {
                        Process updateProcess = new Process();
                        ConfigUpdaterProperties(updateProcess);
                    }
                    else
                    {
                        Process updateProcess = new Process();
                        ConfigUpdaterProperties(updateProcess);

                    }
                }

                // Automatically Open to Install Download

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    Process patchProcess = new Process();
                    ConfigProperties(patchProcess);
                }

            }
        
            catch (InvalidOperationException)
            {
                 PGA.MessengerManager.MessengerManager.AddLog("Program not installed! Patch to install update!");
            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.LogException(ex);
            }
           
             PGA.MessengerManager.MessengerManager.AddLog("Ending Update Function!");
        }

        /// <summary>
        /// Determines whether [has no record].
        /// </summary>
        /// <returns><c>true</c> if [has no record]; otherwise, <c>false</c>.</returns>
        private bool HasNoRecord()
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var record = commands.GetUpdateRecord();

                    if (record == null)
                        return true;


                    if (record.CurrentVersion == null ||
                        record.DownloadPath   == null ||
                        record.RequestDate    == null ||
                        record.BuildDBVersion == null
                    )
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return false;
        }

        /// <summary>
        /// Checks the is installed.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CheckIsInstalled()
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                var record    = commands.GetUpdateRecord();
                var prevBuild = commands.GetCurrentBuild();
                var currBuild = commands.GetCurrentInstalledBuild();

                if (record == null || prevBuild == null)
                {
                     PGA.MessengerManager.MessengerManager.ShowMessageAndLog
                           ("Restart the Database Manager to refresh settings!");
                }

                else if (record.DateCompleted != null)
                {
                    if (prevBuild == currBuild)
                    {
                         PGA.MessengerManager.MessengerManager.ShowMessageAndLog
                            ("Build Installed Successfully!");

                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Configurations the updater properties.
        /// </summary>
        /// <param name="process">The process.</param>
        private void ConfigUpdaterProperties(Process process)
        {
            try
            {
                // Configure the process using the StartInfo properties.
                var name = GetUpdaterName();
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = "-n";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
                process.WaitForExit(); // Waits here for the process to exit.
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Configurations the properties.
        /// </summary>
        /// <param name="process">The process.</param>
        private void ConfigProperties(Process process)
        {
            // Configure the process using the StartInfo properties.

            var name = GetFileName();

            if (String.IsNullOrEmpty(name))
            {
                 PGA.MessengerManager.MessengerManager
                    .ShowMessageAndLog("No File Registered to Patch. Download Again!");
            }
            else if (!File.Exists(name))
                 PGA.MessengerManager.MessengerManager
                   .ShowMessageAndLog("No File in Temp Directory to Patch. Download Again!");
            else
            {
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = "-n";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
            }
        }

        /// <summary>
        /// Determines whether the specified process is running.
        /// </summary>
        /// <param name="process">The process.</param>
        private void IsRunning(Process process)
        {
            try
            {
                if (process != null)
                {
                    var name = process.ProcessName;
                    int count = Process.GetProcessesByName(name).Count();
                    if (count < 1)
                        Process.Start(name);
                }
            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetFileName()
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
               var record =  commands.GetUpdateRecord();

                if (record == null)
                    return null;

                if (record.DownloadPath != null)
                    return System.IO.Path.Combine(record.DownloadPath,record.ExeName);
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdPatch control.
        /// Minimal Checks to allow Pass Thru.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void cmdPatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    Process patchProcess = new Process();
                    ConfigProperties(patchProcess);
                }
            }
            catch (InvalidOperationException)
            {
                 PGA.MessengerManager.MessengerManager.AddLog("Program not installed! Patch to install update!");
            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }
    }
}
