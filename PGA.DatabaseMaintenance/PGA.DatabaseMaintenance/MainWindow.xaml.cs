// ***********************************************************************
// Assembly         : PGA.DatabaseMaintenance
// Author           : Daryl Banks, PSM
// Created          : 01-24-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 01-25-2016
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Banks & Banks Consulting">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Configuration;
using System.Data.SqlServerCe;
using System.Windows;
using PGA.Database;

namespace PGA.DatabaseMaintenance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the cmdRepair control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void cmdRepair_Click(object sender, RoutedEventArgs e)
        {
            DatabaseCommands commands = new DatabaseCommands();

            try
            {
                using (SqlCeEngine engine = new SqlCeEngine(commands.GetConnectionString()))
                {
                    ConnectionStringSettings conSetting = new ConnectionStringSettings();
                    conSetting = ConfigHelper.save_new_connection(commands.GetConnectionString());

                    engine.Repair(conSetting.ConnectionString, RepairOption.RecoverAllPossibleRows);
                }
            }
            catch (SqlCeException ex)
            {
                foreach (SqlCeError error in ex.Errors)
                {
                    //Error handling, etc.
                    MessageBox.Show(error.Message);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
           
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                commands.DeleteDataFromAllTables();
            }
        }
    }
}
