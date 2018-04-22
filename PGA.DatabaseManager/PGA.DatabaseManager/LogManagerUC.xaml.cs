using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGA.Database;
using PGA.DataContext;
using PGA.MessengerManager;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PGA.LogManagerUC
{
    /// <summary>
    /// Interaction logic for LogManagerUC.xaml
    /// </summary>
    public partial class LogManagerUC : UserControl
    {
        public LogManagerUC()
        {
            try
            {
                InitializeComponent();
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    dgTasks.CanUserAddRows = false;
                    dgTasks.CanUserDeleteRows = false;
                    InitialLoadofLogs(ref dgTasks);
                    _DataGrid = dgTasks;
                }
            }
            catch (Exception)
            {
               //  PGA.MessengerManager.MessengerManager.AddLog("Log Initialize Constructor: " + ex.Message);
            }
        }

        private static DataGrid _DataGrid;
        private static List<Logs> _Logs = new List<Logs>();

        private static Int32 logID = 0;

        public static int LogID
        {
            get { return logID; }

            set { logID = value; }
        }

        public static void DoRefresh(string text)
        {
            try
            {
                cmdRefreshCtrl();

            }
            catch (Exception)
            {

            }
        }

        private async void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_Logs != null)
                {
                    _Logs.Clear();    //Clear the local cache
                    await ClearLogs();
                    dgTasks.ItemsSource = null; //_Logs;

                    await RefreshLogs();
                }
            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.ShowMessageAndLog("cmdClear_Click" + ex.Message);
            }
        }


        private async Task ClearLogs()
        {
            try
            {
              
                    await Task.Run(() =>
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        commands.ClearLogFileFromDB();

                        // cmdRefresh_Click(null, null);
                    }
                }
                );
               
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }
        private async void cmdRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                await RefreshLogs();

                dgTasks.ItemsSource = _Logs;


            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

  

        private static async void cmdRefreshCtrl()
        {
            try
            {
                bool filter = false;

                _Logs.Clear();
               
                await Task.Run(() =>
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        if (filter)
                        {
                            var results = commands.FilterLogsForErrors();
                            if (results != null)
                            {
                                Logs[] logs = results.ToArray();

                                if (logs != null)
                                    _Logs = logs.ToList();
                                else
                                    _Logs = null;
                            }
                            else
                            {
                                _Logs = null;
                            }
                        }
                        else
                        {
                            var lower = LogID;
                            var upper = 0;

                            Logs[] logs = commands.GetLastXNumberofLogs(lower, upper);

                            if (logs != null)
                                _Logs = logs.ToList();
                            else
                                _Logs = null;
                        }
                    }
                });

                _DataGrid.ItemsSource = _Logs;

            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }

        }
    

        private bool IsFileLocked(string filename)
        {
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    fs.Close();
                }
                return false;
            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.ShowMessageAndLog("IsFileLocked" + ex.Message);
            }
            return true;
        }

        private async void cmdOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filename = "";
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    filename = commands.GetLogFilePath();
                    var logs = commands.GetLastAllLogs().ToArray();



                    StreamWriter writer = new StreamWriter(filename, false);
                    foreach (var line in logs)
                    {
                        await writer.WriteLineAsync(String.Format("{0}: {1}", line.DateStamp, line.Issue));
                    }
                    writer.Close();
                }
                var flag = true;
                do
                {
                    if (!IsFileLocked(filename))
                    {
                        flag = false;
                        using (Process exeProcess = Process.Start(filename))
                        {
                            exeProcess.WaitForExit();
                            exeProcess.Close();
                        }
                    }

                } while (flag);

            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.ShowMessageAndLog("cmdOpen_Click" + ex.Message);
            }

        }

        private async void cmdExportLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var logs = commands.GetLastAllLogs().ToArray();



                    StreamWriter writer = new StreamWriter(commands.GetLogFilePath(), false);
                    foreach (var line in logs)
                    {
                        await writer.WriteLineAsync(String.Format("{0}: {1}", line.DateStamp, line.Issue));
                    }
                    writer.Close();
                    System.Diagnostics.Process.Start(commands.GetLogFilePath());
                }
            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.ShowMessageAndLog("cmdExportLogs_Click" + ex.Message);
            }
        }

        public static void InitialLoadofLogs(ref DataGrid dgTasks)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {

                    Logs[] logs = commands.GetLastAllLogs().ToArray();
                    if (logs != null)
                        dgTasks.ItemsSource = logs.ToList();
                }
            }
            catch (Exception ex)
            {
                 PGA.MessengerManager.MessengerManager.ShowMessageAndLog("InitialLoadofLogs" + ex.Message);
            }
        }

        public async Task<int> RefreshLogs()
        {
            try
            {
                bool filter = false;

                if (chkFilterErrors.IsChecked == true)
                {
                    filter = true;
                    _Logs.Clear();
                }
                await Task.Run(() =>
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        if (filter)
                        {
                            var results = commands.FilterLogsForErrors();
                            if (results != null)
                            {
                                Logs[] logs = results.ToArray();

                                if (logs != null)
                                    _Logs = logs.ToList();
                                else
                                    _Logs = null;
                            }
                            else
                            {
                                _Logs = null;
                            }
                        }
                        else
                        {
                            var lower = LogID;
                            var upper = 0;

                            Logs[] logs = commands.GetLastXNumberofLogs(lower, upper);

                            if (logs != null)
                                _Logs = logs.ToList();
                            else
                                _Logs = null;
                        }
                    }
                });
            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }


            return 1;
        }
    }
}
