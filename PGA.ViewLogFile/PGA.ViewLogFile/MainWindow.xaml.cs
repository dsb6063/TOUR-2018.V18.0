using System;
using System.Collections.Generic;
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
using PGA.DataContext;
using COMS=PGA.MessengerManager;

namespace PGA.ViewLogFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitialLoadofLogs(ref dgTasks);
        }

        private static Int32 logID=0;

        public static int LogID
        {
            get { return logID; }

            set { logID = value; }
        }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    commands.ClearLogFileFromDB();
                }
            }
            catch (Exception ex)
            {
               COMS.MessengerManager.ShowMessageAndLog("cmdClear_Click" + ex.Message);
            }
        }

        private void cmdRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {

                    var lower = LogID;
                    var upper = 0;
                    Logs[] logs = commands.GetLastXNumberofLogs(lower, upper);
                    if (logs != null)
                        dgTasks.ItemsSource = logs.ToList();
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.ShowMessageAndLog("cmdRefresh_Click" + ex.Message);
            }
        }

        private async void cmdOpen_Click(object sender, RoutedEventArgs e)
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

                    System.Diagnostics.Process.Start(commands.GetLogFilePath());
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.ShowMessageAndLog("cmdOpen_Click" + ex.Message);
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
                    System.Diagnostics.Process.Start(commands.GetLogFilePath());
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.ShowMessageAndLog("cmdExportLogs_Click" + ex.Message);
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
                COMS.MessengerManager.ShowMessageAndLog("InitialLoadofLogs" + ex.Message);
            }
        }
    }
}
