// ***********************************************************************
// Assembly         : PGA.TaskManager
// Author           : Daryl Banks, PSM
// Created          : 12-21-2015
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 12-22-2015
// ***********************************************************************
// <copyright file="TaskWindow.xaml.cs" company="Banks & Banks Consulting">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Windows;
using PGA.Database;

namespace PGA.DatabaseManager
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskWindow" /> class.
        /// </summary>
        public TaskWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the settings data.
        /// </summary>
        public void LoadSettingsData()
           {
               GetTasks();
           }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetTasks()
        {

            DatabaseCommands commands = new DatabaseCommands();

            dgTasks.ItemsSource = commands.GetAllTasks();

            return true;
        }

        /// <summary>
        /// Handles the Loaded event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettingsData();
        }

        private void dgTasks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
