// ***********************************************************************
// Assembly         : PGA.TaskManager
// Author           : Daryl Banks, PSM
// Created          : 01-24-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 01-25-2016
// ***********************************************************************
// <copyright file="DbMaintanenceUC.xaml.cs" company="Banks & Banks Consulting">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using PGA.Database;
using System.Data.SqlServerCe;
using System.Threading.Tasks;
using  PGA.MessengerManager;
namespace PGA.DbMaintanenceUC
{
    /// <summary>
    /// Class DbMaintanenceUC.
    /// </summary>
    public partial class DbMaintanenceUC : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbMaintanenceUC"/> class.
        /// </summary>
        public DbMaintanenceUC()
        {
            try
            {
                InitializeComponent();

            }
            catch
            {
            }

        }
        public static event Action RefreshChanged;
        public static string RefreshFlag { get; set; } // some property

        /// <summary>
        /// Handles the Click event of the cmdRepair control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void cmdRepair_Click(object sender, RoutedEventArgs e)
        {
            DatabaseCommands commands = new DatabaseCommands();
           
            try
            {
                using (SqlCeEngine engine = new SqlCeEngine(commands.GetConnectionString()))
                {
                    ConnectionStringSettings conSetting = new ConnectionStringSettings();
                    conSetting = ConfigHelper.save_new_connection(commands.GetConnectionString());

                   // engine.Repair(conSetting.ConnectionString, RepairOption.RecoverAllPossibleRows);
                     await CallEngineRepairAsync();                   
                }
            }
            catch (SqlCeException ex)
            {
                foreach (SqlCeError error in ex.Errors)
                {
                    DatabaseLogs.FormatLogs("SQLCeException: " + error.Message);
                }
                 PGA.MessengerManager.MessengerManager.ShowMessage((ex.Message));
            }
             PGA.MessengerManager.MessengerManager.ShowMessage(("Complete!"));

        }

        /// <summary>
        /// Calls the engine repair asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public static Task CallEngineRepairAsync()
        {
            Task t = Task.Run(() =>
            {

                DatabaseCommands commands = new DatabaseCommands();

                using (SqlCeEngine engine = new SqlCeEngine(commands.GetConnectionString()))
                {
                    ConnectionStringSettings conSetting = new ConnectionStringSettings();
                    conSetting = ConfigHelper.save_new_connection(commands.GetConnectionString());

                    engine.Repair(conSetting.ConnectionString, RepairOption.RecoverAllPossibleRows);
                }

            });
            t.Wait();
            return t;
        }

        /// <summary>
        /// Calls the engine clear asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public static Task CallEngineClearAsync()
        {
            Task t = Task.Run(() =>
            {
                try
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        commands.DeleteDataFromAllTables();
                    }
                }
                catch (Exception ex1)
                {
                    DatabaseLogs.FormatLogs("cmdClear_Click: " + ex1.Message);
                }
                // PGA.MessengerManager.MessengerManager.ShowMessage(("Complete!"));

            });
            t.Wait();
            return t;
        }
        /// <summary>
        /// Handles the Click event of the cmdClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void cmdClear_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    await CallEngineClearAsync();
                }

                // Clear Cache
                cmdClearCache_Click(sender, e);

                //Notify MainForm Controls
                if (RefreshChanged != null)
                    RefreshChanged();
            }
            catch (Exception ex1)
            {
                DatabaseLogs.FormatLogs("cmdClear_Click: " + ex1.Message);
            }
             PGA.MessengerManager.MessengerManager.ShowMessage(("Complete!"));

        }

        /// <summary>
        /// Handles the Click event of the cmdClearCache control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void cmdClearCache_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    await CallEngineClearCacheAsync();
                }
            }
            catch (Exception ex1)
            {
                DatabaseLogs.FormatLogs("cmdClear_Click: " + ex1.Message);
            }
        }
        /// <summary>
        /// Calls the engine clear cache asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public static Task CallEngineClearCacheAsync()
        {
            Task t = Task.Run(() =>
            {
                try
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        commands.DeleteAllExportToCadRecords();
                    }
                }
                catch (Exception ex1)
                {
                    DatabaseLogs.FormatLogs("cmdClear_Click: " + ex1.Message);
                }

            });
            t.Wait();
            return t;
        }

        /// <summary>
        /// Handles the Click event of the cmdCompact control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void cmdCompact_Click(object sender, RoutedEventArgs e)
        {
            DatabaseCommands commands = new DatabaseCommands();

            try
            {
                using (SqlCeEngine engine = new SqlCeEngine(commands.GetConnectionString()))
                {
                    ConnectionStringSettings conSetting = new ConnectionStringSettings();
                    conSetting = ConfigHelper.save_new_connection(commands.GetConnectionString());

                    await CallEngineRepairAsync();

                }
            }
            catch (SqlCeException ex)
            {
                foreach (SqlCeError error in ex.Errors)
                {
                    DatabaseLogs.FormatLogs("SQLCeException: " + error.Message);
                }
                 PGA.MessengerManager.MessengerManager.ShowMessage((ex.Message));
            }
             PGA.MessengerManager.MessengerManager.ShowMessage(("Complete!"));
        }

        /// <summary>
        /// Calls the engine compact asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public static Task CallEngineCompactAsync()
        {
            Task t = Task.Run(() =>
            {

                DatabaseCommands commands = new DatabaseCommands();

                using (SqlCeEngine engine = new SqlCeEngine(commands.GetConnectionString()))
                {
                    ConnectionStringSettings conSetting = new ConnectionStringSettings();
                    conSetting = ConfigHelper.save_new_connection(commands.GetConnectionString());

                    engine.Compact(conSetting.ConnectionString);
                }

            });
            t.Wait();
            return t;
        }
    }
}
