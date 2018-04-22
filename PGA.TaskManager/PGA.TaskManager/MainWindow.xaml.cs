using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.IO;
using PGA.Database;
using PGA.DataContext;
using COMS=PGA.MessengerManager;

namespace PGA.TaskManager
{
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseCommands commands = new DatabaseCommands();

        public MainWindow()
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

        public void GetTasks()
        {
            using (DatabaseCommands commands = new DatabaseCommands() )
            {
                //IList<Tasks> tasks = new List<Tasks>();

                dgTasks.ItemsSource = commands.GetSettingsCustom();

                //using (var context = MyContext)
                //{
                //    var lsetting =
                //        from p in context.Settings
                //            //where p.DateStamp == Convert.ToDateTime(txtDateCreated.Text)
                //        select new { p.CourseName, p.DateStamp, p.ProjectCreator, p.ProjectName };

                //    dgTasks.ItemsSource = lsetting.ToList();
                //}
                //return settings;
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
            // 1. Load CommandStack
            // 2. Run  Stack
            //(PointFile,PolyFile,DestFile,Command,Args)

            using (var context = GetDataBasePath.GetSql4Connection())
            {

                commands.LoadDrawingStack(DateSelected);
                //PreparePointCloudFiles(DateSelected);
                //PreparePolylineFiles(DateSelected);
                //MergeStacks(PointClouds, Polylines);
                //CopyFilesToLocal();
                // InvokeMapClean();
            }
        }

        private void CopyFilesToLocal()
        {
            using (var context = GetDataBasePath.GetSql4Connection())
            {

                /*     command = String.Format(

                "{0},{1},{2},{3},{4}", order, pointFile, polyfile.FirstOrDefault(), destination, timestamp);*/

                //Get 

                var stack =
                    from p in context.DrawingStack
                    select p;

                foreach (var files in stack)
                {
                    CommandsForCopy fi = ParseCommand(files.Function);

                    FileFunctions.CopyFiles(fi.PointFile, fi.DestFile, System.IO.Path.GetFileName(fi.PointFile));
                    FileFunctions.CopyFiles(fi.PolyFile, fi.DestFile, System.IO.Path.GetFileName(fi.PolyFile));
                }


                //var lsetting =
                //    from p in context.CommandStack
                //    //where p.DateStamp == Convert.ToDateTime(txtDateCreated.Text)
                //    select new { p.CourseName, p.DateStamp, p.ProjectCreator, p.ProjectName };

                //dgTasks.ItemsSource = lsetting.ToList();

                //foreach (var val in settings)
                //{
                //    DateCreated = DateTime.Now;
                //    txtDateCreated.Text = DateCreated.ToString();
                //    return true;
                //}
                // settings =   lsetting;

                //return settings;
            }
        }

        private CommandsForCopy ParseCommand(string commands)
        {/*     command = String.Format(

                "{0},{1},{2},{3},{4}", order, pointFile, polyfile.FirstOrDefault(), destination, timestamp);*/

            string[] lfile = commands.Split(',');
            CommandsForCopy file = new CommandsForCopy();
            file.Order = lfile[0];
            file.PointFile = lfile[1];
            file.PolyFile = lfile[2];
            file.DestFile = System.IO.Path.Combine(lfile[2],lfile[3]);
            file.time = Convert.ToDateTime(lfile[4]);
            return file;   
        }

        private void MergeStacks(IList<Files> pointClouds, IList<Files> polylines)
        {
            string order, pointFile, destination, timestamp, command;

            using (var context = GetDataBasePath.GetSql4Connection())
            {
                string t = "";

                for (int i = 1; i < 19; i++)
                {
                    if (i < 10)
                    {
                        t = "0" + i.ToString();
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
                        DrawingStack stack = new DrawingStack();

                        order = po.Order;
                        pointFile = System.IO.Path.Combine(po.SourceFile, po.FileName);
                        if (String.IsNullOrEmpty(po.DestFile))
                            destination = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(po.SourceFile),
                                "HOLE" + po.Order);
                        else
                            destination = System.IO.Path.Combine(po.DestFile, "HOLE" + po.Order);
                        timestamp = po.time.ToString();

                        var polyfile = from p in pline
                            where order.Equals(p.Order)
                            select System.IO.Path.Combine(p.SourceFile, p.FileName);

                        command = String.Format(
                            "{0},{1},{2},{3},{4}", order, pointFile, polyfile.FirstOrDefault(), destination, timestamp);


                        //stack.Commands = command;
                        //context.CommandStack.InsertOnSubmit(stack);
                    }
                    context.SubmitChanges();
                }
            }
        }

        //public void LoadDrawingStack()
        //{
        //    using (var context = GetDataBasePath.GetSql4Connection())
        //    {

        //        var polylines =
        //            (from p in context.PolylineDWGS
        //                where (p.DateStamp == Convert.ToDateTime(DateSelected))
        //                select p);

        //        var pointclouds =
        //            (from p in context.PointCloud
        //                where (p.DateStamp == Convert.ToDateTime(DateSelected))
        //                select p);


        //        var drawingstack =
        //            (from pl in context.PolylineDWGS
        //                from p in context.PointCloud
        //                where (p.DateStamp == pl.DateStamp)
        //                select
        //                    new
        //                    {
        //                        DateStamp = pl.DateStamp,
        //                        PolylineName = pl.DrawingName,
        //                        PolylineID = pl.Id,
        //                        PointCloudName = p.DrawingName
        //                    }
        //                );


        //        foreach (var val in drawingstack)
        //        {
        //            DrawingStack stack = new DrawingStack();
        //            stack.DateStamp = val.DateStamp;
        //            stack.PointCloudDwgName = val.PointCloudName;
        //            stack.PolylineDwgName = val.PolylineName;
        //            stack.SourcePolylineDwgID = val.PolylineID;
        //            stack.Hole = lOrder(val.PointCloudName);
        //            context.DrawingStack.InsertOnSubmit(stack);
        //            context.SubmitChanges();
        //        }

        //    }
        //}

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdPause_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (((Settings) e.AddedItems[0]) != null)
                {
                    var dateStamp = ((Settings) e.AddedItems[0]).DateStamp;
                    if (dateStamp != null)
                        DateSelected = dateStamp.Value;
                }
                else
                {
                    DateSelected = Convert.ToDateTime(GetCurrentRow((DataGrid) sender));
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.ShowMessageAndLog("dgTasks_SelectionChanged " + ex.Message);
            }
        }

        public DateTime GetDateFromGrid(string val)
        {
            DateTime date;
            string[] values = val.Split('=');
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
                int i = 1;
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
                int i = 1;
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
                int i = 1;
                foreach (var val in points)
                {
                    DrawingStack stack = new DrawingStack();
                    command = String.Format(
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
                    var element = new FrameworkElement() { DataContext = cellInfo.Item };
                    BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
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

                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);

                    foreach (var gridColumn in dataGrid.Columns)
                    {
                        if ((gridColumn.Header.ToString().Equals("Date Created")))
                        {
                            var value = gridColumn.GetCellContent(row).DataContext;
                            cellContent = ((TextBlock)(GetCell(i, 1, dataGrid).Content)).Text;
                            return cellContent;
                        }
                    }

                }

            }
            return "";
        }


        //As I want the value of 3 column

        public DataGridCell GetCell(int row, int column, DataGrid customDataGrid)
        {
            DataGridRow rowContainer = GetRow(row, customDataGrid);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // Try to get the cell but it may possibly be virtualized.
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell != null)
                {
                    // Now try to bring into view and retreive the cell.
                    customDataGrid.ScrollIntoView(rowContainer, customDataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int _currentRowIndex, DataGrid customDataGrid)
        {
            DataGridRow row = (DataGridRow)customDataGrid.ItemContainerGenerator.ContainerFromIndex(_currentRowIndex);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                customDataGrid.UpdateLayout();
                customDataGrid.ScrollIntoView(customDataGrid.Items[_currentRowIndex]);
                row = (DataGridRow)customDataGrid.ItemContainerGenerator.ContainerFromIndex(_currentRowIndex);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
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

    public class AutoCADFunctions
    {

        public void InvokeMapClean()
        {

        }

    }

}
