using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using PGA.Database;
using PGA.DataContext;
using COMS=PGA.MessengerManager;

namespace PGA.TaskManager
{
    /// <summary>
    ///     Interaction logic for TaskManagerUC.xaml
    /// </summary>
    partial class TaskManagerUC : UserControl
    {
        public TaskManagerUC()
        {
            InitializeComponent();
            GetTasks();
        }

        public PGAContext MyContext { get; set; }
        public DateTime DateSelected { get; set; }
        public IList<Files> PointClouds = new List<Files>();
        public IList<Files> Polylines = new List<Files>();
        public void LoadSettingsData()
        {
            GetTasks();
        }
        private readonly DatabaseCommands commands = new DatabaseCommands();

        public void GetTasks()
        {
            try
            {
                using (var commands = new DatabaseCommands())
                {
                    var results = commands.GetSettingsCustom();
                    if (results.FirstOrDefault() == null)
                        return;

                    dgTasks.ItemsSource = results;
                }
            }
            catch (Exception ex)
            {
                 COMS.MessengerManager.ShowMessageAndLog("GetTasks: " + ex.Message);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettingsData();
        }

        private void cmdRunTask_Click(object sender, RoutedEventArgs e)
        {
 
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                commands.LoadDrawingStack(DateSelected);           
            }
        }

        private void CopyFilesToLocal()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
   
                var stack =
                    from p in context.DrawingStack
                    select p;

                foreach (var files in stack)
                {
                    CommandsForCopy fi = ParseCommand(files.Function);

                    FileFunctions.CopyFiles(fi.PointFile, fi.DestFile, Path.GetFileName(fi.PointFile));
                    FileFunctions.CopyFiles(fi.PolyFile, fi.DestFile, Path.GetFileName(fi.PolyFile));
                }
            }
        }

        private CommandsForCopy ParseCommand(string commands)
        { 

            string[] lfile = commands.Split(',');
            CommandsForCopy file = new CommandsForCopy();
            file.Order     = lfile[0];
            file.PointFile = lfile[1];
            file.PolyFile  = lfile[2];
            file.DestFile  = System.IO.Path.Combine(lfile[2], lfile[3]);
            file.time      = Convert.ToDateTime(lfile[4]);
            return file;
        }

        private void MergeStacks(IList<Files> pointClouds, IList<Files> polylines)
        {
            string order, pointFile, destination, timestamp, command;

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var t = "";

                for (var i = 1; i < 19; i++)
                {
                    if (i < 10)
                    {
                        t = "0" + i;
                    }
                    else
                    {
                        t = i.ToString();
                    }
                    var point = from po in pointClouds
                        where ((po.Order).Equals(t))
                        select po;


                    var pline = from pl in polylines
                        where ((pl.Order).Equals(t))
                        select pl;


                    foreach (var po in point)
                    {
                        var stack = new DrawingStack();

                        order = po.Order;
                        pointFile = Path.Combine(po.SourceFile, po.FileName);
                        if (string.IsNullOrEmpty(po.DestFile))
                            destination = Path.Combine(Path.GetDirectoryName(po.SourceFile),
                                "HOLE" + po.Order);
                        else
                            destination = Path.Combine(po.DestFile, "HOLE" + po.Order);
                        timestamp = po.time.ToString();

                        var polyfile = from p in pline
                            where order.Equals(p.Order)
                            select Path.Combine(p.SourceFile, p.FileName);

                        command = string.Format(
                            "{0},{1},{2},{3},{4}", order, pointFile, polyfile.FirstOrDefault(), destination, timestamp);
                    }
                    context.SubmitChanges();
                }
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdPause_Click(object sender, RoutedEventArgs e)
        {
        }

        private void dgTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateSelected = Convert.ToDateTime(GetCurrentRow((DataGrid) sender));
        }

        public DateTime GetDateFromGrid(string val)
        {
            DateTime date;
            var values = val.Split('=');
            date = Convert.ToDateTime(values[2].Split(',').ToString());
            return date;
        }

        public void PreparePointCloudFiles(DateTime time)
        {
            PointClouds.Clear();

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var points =
                    from p in context.PointCloud
                    where (p.DateStamp == Convert.ToDateTime(DateSelected))
                    select p;


                string command;
                var i = 1;
                foreach (var val in points)
                {
                    Files stack = new Files();

                    stack.Order = Order(val.DrawingName);
                    stack.FileName = val.DrawingName;
                    stack.SourceFile = val.SourcePath;
                    stack.DestFile = val.DestinationPath;
                    stack.time = Convert.ToDateTime(val.DateStamp);
                    PointClouds.Add(stack);
                }

                context.SubmitChanges();
            }
        }

        public void PreparePolylineFiles(DateTime time)
        {
            Polylines.Clear();

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var points =
                    from p in context.PolylineDWGS
                    where (p.DateStamp == Convert.ToDateTime(DateSelected))
                    select p;


                string command;
                var i = 1;
                foreach (var val in points)
                {
                    Files stack = new Files();

                    stack.Order = Order(val.DrawingName);
                    stack.FileName = val.DrawingName;
                    stack.SourceFile = val.SourcePath;
                    stack.DestFile = val.DestinationPath;
                    stack.time = Convert.ToDateTime(val.DateStamp);
                    Polylines.Add(stack);
                }

                context.SubmitChanges();
            }
        }

        public void PullPointCloudFiles(DateTime time)
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {
                var points =
                    from p in context.PointCloud
                    where (p.DateStamp == Convert.ToDateTime(DateSelected))
                    select p;


                string command;
                var i = 1;
                foreach (var val in points)
                {
                    var stack = new DrawingStack();
                    command = string.Format(
                        "{0},{1},{2},{3},{4}", Order(val.DrawingName), val.DrawingName, val.SourcePath,
                        val.DestinationPath,
                        val.DateStamp);
                    stack.Function = command;
                    context.DrawingStack.InsertOnSubmit(stack);
                    // PointClouds.Add(stack);
                }

                context.SubmitChanges();
            }
        }

        private long? lOrder(string file)
        {
            return Convert.ToInt64(file.Substring(file.Length - 6, 2));
        }

        private string Order(string file)
        {
            return file.Substring(file.Length - 6, 2);
        }

        public void PullPolyLineFiles(DateTime time)
        {
        }

        public void GetCurrentCell(DataGrid dataGrid)
        {
            var cellInfo = dataGrid.CurrentCell;
            if (cellInfo != null)
            {
                var column = cellInfo.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var element = new FrameworkElement {DataContext = cellInfo.Item};
                    BindingOperations.SetBinding(element, TagProperty, column.Binding);
                    var cellValue = element.Tag;
                }
            }
        }

        public string GetCurrentRow(DataGrid dataGrid)
        {
            string cellContent;

            if (dataGrid.SelectedValue == null)
            {
                MessageBox.Show("select any record from list..!", "select atleast one record", MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);
            }
            else
            {
                for (var i = 0; i < dataGrid.Items.Count; i++)
                {
                    var row = (DataGridRow) dataGrid.ItemContainerGenerator.ContainerFromIndex(i);

                    foreach (var gridColumn in dataGrid.Columns)
                    {
                        if ((gridColumn.Header.ToString().Equals("Date Created")))
                        {
                            var value = gridColumn.GetCellContent(row).DataContext;
                            cellContent = ((TextBlock) (GetCell(i, 1, dataGrid).Content)).Text;
                            return cellContent;
                        }
                    }
                }
            }
            return "";
        }

        public DataGridCell GetCell(int row, int column, DataGrid customDataGrid)
        {
            var rowContainer = GetRow(row, customDataGrid);

            if (rowContainer != null)
            {
                var presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // Try to get the cell but it may possibly be virtualized.
                var cell = (DataGridCell) presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell != null)
                {
                    // Now try to bring into view and retreive the cell.
                    customDataGrid.ScrollIntoView(rowContainer, customDataGrid.Columns[column]);
                    cell = (DataGridCell) presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int _currentRowIndex, DataGrid customDataGrid)
        {
            var row = (DataGridRow) customDataGrid.ItemContainerGenerator.ContainerFromIndex(_currentRowIndex);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                customDataGrid.UpdateLayout();
                customDataGrid.ScrollIntoView(customDataGrid.Items[_currentRowIndex]);
                row = (DataGridRow) customDataGrid.ItemContainerGenerator.ContainerFromIndex(_currentRowIndex);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = (Visual) VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
 
