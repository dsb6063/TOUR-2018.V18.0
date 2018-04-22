// ***********************************************************************
// Assembly         : PGA.QueueManager
// Author           : Daryl Banks, PSM
// Created          : 02-16-2016
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 02-16-2018
// ***********************************************************************
// <copyright file="QueueManager.xaml.cs" company="Banks & Banks Consulting">
//     Copyright ©  2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGA.Database;
using PGA.DataContext;
using COMS = PGA.MessengerManager;

namespace PGA.DatabaseManager
{
    using System.Windows.Controls.Primitives;
    using global::QueueManager;

    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class QueueManager : System.Windows.Window
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManager" /> class.
        /// </summary>
        /// <param name="unknown">if set to <c>true</c> [unknown].</param>
        public QueueManager(bool unknown)
        {
            //_contentLoaded = unknown;
            InitializeComponent();
            dgTasks.CanUserAddRows = false;
            dgTasks.CanUserDeleteRows = false;
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
            try
            {

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    IList<TaskManager> task=
                     commands.GetTasksProjectName();
                    if (task == null) return false;
                    dgTasks.ItemsSource = task; 
                    return true;
                }
            }
            catch (Exception ex)
            {                
                COMS.MessengerManager.LogException(ex);
            }
            return false;
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

        /// <summary>
        /// Handles the SelectionChanged event of the dgTasks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void dgTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (((TaskManager)e.AddedItems[0]) != null)
                {
                    var dateStamp = ((TaskManager)e.AddedItems[0]).DateStamp;
                    if (dateStamp != null)
                        DateSelected = dateStamp;
                }
                else
                {
                    DateSelected = Convert.ToDateTime(GetCurrentRow((DataGrid)sender));
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.ShowMessageAndLog("dgTasks_SelectionChanged " + ex.Message);
            }
         
        }

        /// <summary>
        /// Gets or sets the date selected.
        /// </summary>
        /// <value>The date selected.</value>
        public DateTime DateSelected { get; set; }


        /// <summary>
        /// Gets the current row.
        /// </summary>
        /// <param name="dataGrid">The data grid.</param>
        /// <returns>System.String.</returns>
        public string GetCurrentRow(DataGrid dataGrid)
        {
            string str;
            if (dataGrid.SelectedValue != null)
            {
                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                    foreach (DataGridColumn gridColumn in dataGrid.Columns)
                    {
                        if (gridColumn.Header.ToString().Equals("Date Created"))
                        {
                            object value = gridColumn.GetCellContent(row).DataContext;
                            string cellContent = ((TextBlock)this.GetCell(i, 1, dataGrid).Content).Text;
                            str = cellContent;
                            return str;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("select any record from list..!", "select atleast one record", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
            }
            str = "";
            return str;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="customDataGrid">The custom data grid.</param>
        /// <returns>DataGridCell.</returns>
        public DataGridCell GetCell(int row, int column, DataGrid customDataGrid)
        {
            DataGridCell dataGridCell;
            DataGridRow rowContainer = this.GetRow(row, customDataGrid);
            if (rowContainer == null)
            {
                dataGridCell = null;
            }
            else
            {
                DataGridCellsPresenter presenter = QueueManager.GetVisualChild<DataGridCellsPresenter>(rowContainer);
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell != null)
                {
                    customDataGrid.ScrollIntoView(rowContainer, customDataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                dataGridCell = cell;
            }
            return dataGridCell;
        }

        /// <summary>
        /// Gets the visual child.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <returns>T.</returns>
        public static T GetVisualChild<T>(Visual parent)
       where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            int i = 0;
            while (i < numVisuals)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = (T)(v as T);
                if (child == null)
                {
                    child = QueueManager.GetVisualChild<T>(v);
                }
                if (child == null)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="_currentRowIndex">Index of the _current row.</param>
        /// <param name="customDataGrid">The custom data grid.</param>
        /// <returns>DataGridRow.</returns>
        public DataGridRow GetRow(int _currentRowIndex, DataGrid customDataGrid)
        {
            DataGridRow row = (DataGridRow)customDataGrid.ItemContainerGenerator.ContainerFromIndex(_currentRowIndex);
            if (row == null)
            {
                customDataGrid.UpdateLayout();
                customDataGrid.ScrollIntoView(customDataGrid.Items[_currentRowIndex]);
                row = (DataGridRow)customDataGrid.ItemContainerGenerator.ContainerFromIndex(_currentRowIndex);
            }
            return row;
        }


        /// <summary>
        /// Handles the Click event of the cmdPause control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void cmdPause_Click(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// Handles the Click event of the cmdCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }



        /// <summary>
        /// Handles the Click event of the cmdRunTask control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void cmdRunTask_Click(object sender, RoutedEventArgs e)
        {
            #region Process Steps
            //1. get the selected row id or Date Created
            //2. load the drawing stack --done
            //3. start the processing of polylines
            //change layers
            //load template
            //create point cloud
            //create "All" surface
            //process surfaces with dll. 
            #endregion

            using (DatabaseCommands commands = new DatabaseCommands())

            {
                commands.NewTaskManager(DateSelected);
                commands.SetTaskPaused(DateSelected);
                this.Hide();
                Button button = sender as Button;
                cmdRunTask.Tag = "true";
                this.Close();
            }


        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
         

            try
            {
                QueueManager manager = sender as QueueManager;
                Button button = manager.cmdRunTask as Button;

                if (button != null)

                {
                    if (button.Name.Contains("cmdRunTask"))
                    {
                        if (button.Tag.ToString() == "true")
                        {
                            button.Tag = "";

                            Commands.StartDWG();
                            COMS.MessengerManager.
                                ShowMessageAndLog
                                ("Starting Surface Manager!");
                        }
                    }

                }
            }
            catch (System.NullReferenceException)
            {
                COMS.MessengerManager.AddLog("Window Closed by user!");

            }
            catch (Exception ex)
            {
                COMS.MessengerManager.AddLog("Window_Closing " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
