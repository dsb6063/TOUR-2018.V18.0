using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using PGA.Database;
using PGA.DatabaseManager;
using PGA.DataContext;

namespace PGA.TaskManagerUC
{
    /// <summary>
    ///     Interaction logic for TaskManagerUC.xaml
    /// </summary>
    public partial class TaskManagerUC : UserControl
    {
        private static DataGrid _dataGrid;

        public TaskManagerUC()
        {
            try
            {
                InitializeComponent();
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    GetTasks();

                    _settings = null;
                    _dataGrid = dgTasks;
                }
        }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Task Initialize Constructor: " + ex.Message);
            }
}

        public DateTime DateSelected { get; set; }
        public IList<Files> PointClouds = null;
        public IList<Files> Polylines = null;
        //public static Settings _settings = null;
        public static object _settings = null;

        public static TextBox _TextEventPlaceHolder = null;
        
        public void LoadSettingsData()
        {
            try
            {
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    return;
                }

                GetTasks();

            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public static void DoRefresh(string text)
        {
            try
            {
                _dataGrid.ItemsSource = null;

            }
            catch (Exception)
            {

            }
        }

        public bool IsNull<T>(IEnumerable<Settings> t)
        {
            bool flag = false;
            try
            {
                var i = t;
                if (t.FirstOrDefault() != null)
                    return flag;
            }
            catch (Exception)
            {
                flag = true;
            }
            return flag;
        }

        public void GetTasks()
        {
            try
            {
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    return;
                }

                PointClouds = new List<Files>();
                Polylines = new List<Files>();
                _TextEventPlaceHolder = new TextBox();
                _TextEventPlaceHolder.TextChanged += _Text_TextChanged;

                dgTasks.ItemsSource = null;
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var result = commands.GetSettingsAndTaskInfo();

                    //Filter Canceled Tasks
                    if (result != null)
                    {
                        var removedCanceled = result.Where(p => p.IsCancelled != true);

                        if (!IsNull<PGA.DataContext.Settings>(result))
                        {
                            dgTasks.CanUserAddRows = false;
                            dgTasks.CanUserDeleteRows = false;
                            dgTasks.ItemsSource = CleanTable(removedCanceled.ToList());
                        }
                        else
                            dgTasks.ItemsSource = null;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private IList<Settings> CleanTable(IList<Settings> settings)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return null;
            }

            var lsetting = new List<Settings>();

            try
            {
              
                foreach (var item in settings)
                {
                    if (!String.IsNullOrEmpty(item.CourseName))
                        lsetting.Add(item);
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return lsetting;
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    return;
                }

                _TextEventPlaceHolder = new TextBox();
                LoadSettingsData();

            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private void _Text_TextChanged(object sender, TextChangedEventArgs e)
        {
           GetTasks();
        }


        private void cmdRunTask_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (DateSelected.Ticks == 0)
                    return;

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                   
                    commands.LoadDrawingStack(DateSelected);

                    if (commands.GetAllDrawingStackDwg().Count != 0)
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAndLog
                            (commands.UpdateTaskStartDate(DateSelected) != null
                                ? "Run Completed! Start Civil 3D 2016."
                                : "Failed to Start Civil 3D 2016.");

                        commands.SetSettingsNOTPaused(DateSelected);
                        commands.SetTasksNOTPaused(DateSelected);
                        GetTasks();
                    }
                    else 
                        PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Files NOT Loaded in Drawing Stack!");
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private void CopyFilesToLocal()
        {
            try
            {

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var stack = commands.GetAllDrawingStackDwg();

                    //var stack =
                    //    from p in context.DrawingStack
                    //    select p;

                    foreach (var files in stack)
                    {
                        CommandsForCopy fi = ParseCommand(files.Function);

                        FileFunctions.CopyFiles(fi.PointFile, fi.DestFile, Path.GetFileName(fi.PointFile));
                        FileFunctions.CopyFiles(fi.PolyFile, fi.DestFile, Path.GetFileName(fi.PolyFile));
                    }
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private CommandsForCopy ParseCommand(string commands)
        {

            try
            {
                string[] lfile = commands.Split(',');
                CommandsForCopy file = new CommandsForCopy();
                file.Order = lfile[0];
                file.PointFile = lfile[1];
                file.PolyFile = lfile[2];
                file.DestFile = System.IO.Path.Combine(lfile[2], lfile[3]);
                file.time = Convert.ToDateTime(lfile[4]);
                return file;
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return null;
        }


        //private void MergeStacks(IList<Files> pointClouds, IList<Files> polylines)
        //{
        //    string order, pointFile, destination, timestamp, command;

        //    try
        //    {
        //        using (DatabaseCommands commands = new DatabaseCommands())

        //        // using (PGAContext context = GetDataBasePath.GetSql4Connection())
        //        {
        //            var t = "";

        //            for (var i = 1; i < 19; i++)
        //            {
        //                if (i < 10)
        //                {
        //                    t = "0" + i;
        //                }
        //                else
        //                {
        //                    t = i.ToString();
        //                }
        //                var point = from po in pointClouds
        //                            where ((po.Order).Equals(t))
        //                            select po;


        //                var pline = from pl in polylines
        //                            where ((pl.Order).Equals(t))
        //                            select pl;


        //                foreach (var po in point)
        //                {
        //                    var stack = new DrawingStack();

        //                    order = po.Order;
        //                    pointFile = Path.Combine(po.SourceFile, po.FileName);
        //                    if (string.IsNullOrEmpty(po.DestFile))
        //                        destination = Path.Combine(Path.GetDirectoryName(po.SourceFile),
        //                            "HOLE" + po.Order);
        //                    else
        //                        destination = Path.Combine(po.DestFile, "HOLE" + po.Order);
        //                    timestamp = po.time.ToString();

        //                    var polyfile = from p in pline
        //                                   where order.Equals(p.Order)
        //                                   select Path.Combine(p.SourceFile, p.FileName);

        //                    command = string.Format(
        //                        "{0},{1},{2},{3},{4}", order, pointFile, polyfile.FirstOrDefault(), destination, timestamp);
        //                }
        //              //  context.SubmitChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //         PGA.MessengerManager.MessengerManager.LogException(ex);
        //    }
        //}

 

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DateSelected.Ticks == 0)
                    return;

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    commands.UpdateTaskCompleteDate(DateSelected);
                    commands.SetTaskCancelled(DateSelected);
                    commands.SetSettingsCanceled(DateSelected);
                    GetTasks();
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private void cmdPause_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (DateSelected.Ticks == 0)
                    return;

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    commands.ClearTaskStartDate(DateSelected);
                    commands.SetTaskPaused(DateSelected);
                    commands.SetSettingsPaused(DateSelected);
                    GetTasks();
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private void dgTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            try
            {
                if (e.AddedItems.Count > 0 && ((Settings) e.AddedItems[0]) != null)
                {
                    var dateStamp = ((Settings) e.AddedItems[0]).DateStamp;
                    if (dateStamp != null)
                        DateSelected = dateStamp.Value;
                }
                else
                {
                    DateSelected = Convert.ToDateTime(GetCurrentRow((DataGrid) sender));
                }
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var settings = commands.GetSettingsByDate(DateSelected);
                    _settings = settings;
                    MainWindow._TextEventPlaceHolder.Text = DateTime.Now.ToString();
                }
                   
            }
            catch (System.InvalidCastException ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            catch (System.FormatException)
            {
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
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
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            PointClouds.Clear();

            try
            {
                using (PGAContext context = GetDataBasePath.GetSql4Connection())
                {
                    var points =
                        from p in context.PointCloud
                        where (p.DateStamp == Convert.ToDateTime(DateSelected))
                        select p;


                    //string command;
                    //var i = 1;
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

                    //context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }


        public void PreparePolylineFiles(DateTime time)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            Polylines.Clear();

            try
            {
                using (PGAContext context = GetDataBasePath.GetSql4Connection())
                {
                    var points =
                        from p in context.PolylineDWGS
                        where (p.DateStamp == Convert.ToDateTime(DateSelected))
                        select p;


                    //string command;
                    //var i = 1;
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

                    //context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public void PullPointCloudFiles(DateTime time)
        {
            try
            {

                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    return;
                }

                using (PGAContext context = GetDataBasePath.GetSql4Connection())
                {
                    var points =
                        from p in context.PointCloud
                        where (p.DateStamp == Convert.ToDateTime(DateSelected))
                        select p;


                    string command;
                   // var i = 1;
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
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        //private long? lOrder(string file)
        //{
        //    return Convert.ToInt64(file.Substring(file.Length - 6, 2));
        //}

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
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return "";
            }

            string cellContent;

            if (dataGrid.SelectedValue == null)
            {
                //MessageBox.Show("select any record from list!", "select at least one record", MessageBoxButton.OKCancel,
                //    MessageBoxImage.Warning);
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

        public class Files
        {
            public string Order { get; set; }
            public string FileName { get; set; }
            public string SourceFile { get; set; }
            public string DestFile { get; set; }
            public DateTime time { get; set; }
        }

        public class CommandsForCopy
        {
            public string Order { get; set; }
            public string PointFile { get; set; }
            public string PolyFile { get; set; }
            public string DestFile { get; set; }
            public DateTime time { get; set; }
        }

        private void cmdRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetTasks();
        }
    }

    public static class FileFunctions
    {

        public static void CopyFiles(string source, string destination, string fileName)
        {
            //source = System.IO.Path.Combine(source, fileName);
            destination = System.IO.Path.Combine(destination, fileName);
            var test = System.IO.Path.GetDirectoryName(destination);

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(destination)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination));
            else
                System.IO.File.Copy(source, destination, true);
        }
        public static void CopyFiles(string source, string destination)
        {
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(destination)))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destination));
            else
                System.IO.File.Copy(source, destination);
        }

    }


}

