using PGA.Database;
using PGA.DatabaseManager.Helpers;
using PGA.DataContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using PGA.AdjustSimplify;
using PGA.DatabaseManager.Annotations;
using Brushes = System.Drawing.Brushes;
using Button = System.Windows.Forms.Button;
using MessageBox = System.Windows.MessageBox;

namespace PGA.DatabaseManager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static Settings GSettings;

        public static System.Windows.Forms.Timer myTimer;

        public static int alarmCounter;

        public static bool exitFlag;

        public static System.Windows.Controls.TextBox _TextEventPlaceHolder;

        private Validate validate = new Validate();

        private CustomTimeSpan time = new CustomTimeSpan();



        public string CourseCode
        {
            get;
            set;
        }

        public string CourseName
        {
            get;
            set;
        }

        public DateTime DateCreated
        {
            get;
            set;
        }

        public FolderBrowserDialog DestFolderPath
        {
            get;
            private set;
        }

        public FolderBrowserDialog DxfFolderFilePath
        {
            get;
            private set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public string MyconnectionString
        {
            get;
            private set;
        }

        public FolderBrowserDialog PointFilePath
        {
            get;
            private set;
        }

        public FolderBrowserDialog PolylineFilePath
        {
            get;
            private set;
        }

        public string ProjectName
        {
            get;
            set;
        }

        public FolderBrowserDialog ServerFilePath
        {
            get;
            private set;
        }

        public int SettingID
        {
            get;
            set;
        }

        public int TaskId
        {
            get;
            set;
        }

        public FolderBrowserDialog TemplateFilePath
        {
            get;
            private set;
        }

        public bool TestFileAccess
        {
            get;
            set;
        }

        public bool Testing
        {
            get;
            set;
        }

        public static System.Version Version
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().Version;
            }
        }
        

        static MainWindow()
        {
            MainWindow.GSettings = null;
            MainWindow.myTimer = new System.Windows.Forms.Timer();
            MainWindow._TextEventPlaceHolder = null;
        }

        public MainWindow(string test)
        {
        }

        public MainWindow()
        {
            try
            {
                this.InitializeComponent();
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    MainWindow.GSettings = new Settings();
                    GSettings.PropertyChanged += GSettings_PropertyChanged;
                    GSettings.PropertyChanging += GSettings_PropertyChanging;
                    this.PointFilePath = new FolderBrowserDialog();
                    this.PolylineFilePath = new FolderBrowserDialog();
                    this.TemplateFilePath = new FolderBrowserDialog();
                    this.ServerFilePath = new FolderBrowserDialog();
                    this.DestFolderPath = new FolderBrowserDialog();
                    this.DxfFolderFilePath = new FolderBrowserDialog();
                    MainWindow._TextEventPlaceHolder = new System.Windows.Controls.TextBox();
                    MainWindow._TextEventPlaceHolder.TextChanged += new TextChangedEventHandler(this._Text_TextChanged);
                    this.txtStartTime.TextChanged += new TextChangedEventHandler(this.TxtStartTime_TextChanged);
                    this.txtEndTime.TextChanged += new TextChangedEventHandler(this.TxtStartTime_TextChanged);
                  

                    this.InitializeTaskManager();
                    MainWindow.InitializeTimer();

                    this.chkOmitSDXF.IsChecked = new bool?(false);
                    this.chkErrReports.IsChecked = new bool?(false);
                    this.chkProbing.IsChecked = new bool?(false);
                    this.chkDuplicates.IsChecked = new bool?(false);
                    this.chkPDFReport.IsChecked = new bool?(false);
                    this.chkSimplifyPolylines.IsChecked = new bool?(false);
                    this.chkStrictNames.IsChecked = new bool?(false);
 
                    //Event handler to refresh log and task datagrids.
                    Action loghandler = () => PGA.LogManagerUC.LogManagerUC.DoRefresh(DbMaintanenceUC.DbMaintanenceUC.RefreshFlag);
                    DbMaintanenceUC.DbMaintanenceUC.RefreshChanged += loghandler;
                    Action taskhandler = () => PGA.TaskManagerUC.TaskManagerUC.DoRefresh(DbMaintanenceUC.DbMaintanenceUC.RefreshFlag);
                    DbMaintanenceUC.DbMaintanenceUC.RefreshChanged += taskhandler;

                }


            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void _Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.GSettings_PropertyChanged(sender, null);
        }


        private void ClearProgressBar()
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {
                    commands.ClearTimerInfo();
                }
                catch (Exception exception)
                {
                    PGA.MessengerManager.MessengerManager.LogException(exception);
                }
            }
        }

        private void CheckForVersionConsistency()
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {
                    string curr = MainWindow.Version.ToString();
                    //string curr = commands.GetCurrentBuild();
                    string prev = commands.GetCurrentInstalledBuild();
                    if (curr != prev)
                    {
                        if ((curr == null ? true : this.CheckIsNewerVersion(curr, prev)))
                        {
                            commands.SetBuildVersionInUpdate(prev);
                            commands.SetClearFlagsInUpdate();
                        }
                    }
                }
                catch (Exception exception)
                {
                    PGA.MessengerManager.MessengerManager.LogException(exception);
                }
            }
        }

        private bool CheckIsNewerVersion(string curr, string prev)
        {
            bool flag;
            double c = this.CvtBuildToDouble(curr);
            flag = (this.CvtBuildToDouble(prev) < c ? true : false);
            return flag;
        }

        public bool RenewTimeStamp()
        {

            try
            {
                this.DateCreated = DateConverts.GetDateTimeNow();
                this.txtDateCreated.Text  = DateConverts.DateTimeToStringForDatabase(this.DateCreated);
                this.txtDateCreated2.Text = DateCreated.ToString("yyyy-MM-dd hh:mm:ss"); //for display only
                return true;

            }

            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            return false;
        }
        public bool CheckTimeStamp()
        {
            bool flag;
            try
            {
                if (string.IsNullOrEmpty(this.txtDateCreated.Text))
                {
                    throw new ArgumentNullException("DateStamp");
                }
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    flag = commands.CheckTimeStamp(this.txtDateCreated.Text);
                    return flag;
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            flag = true;
            return flag;
        }

        private void chkSportVision_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox value = (System.Windows.Controls.CheckBox)sender;
            if (value != null)
            {
                if (value.IsChecked.Value)
                {
                    this.chk3DPolys.IsChecked = new bool?(true);
                    this.chkCreateBL.IsChecked = new bool?(true);
                    this.chkOmitSDXF.IsChecked = new bool?(false);
                }
            }
        }

        public void ClearTextBoxes()
        {
            try
            {
                this.txtPointPath.Text = string.Empty;
                this.txtPolylinePath.Text = string.Empty;
                this.txtTemplatePath.Text = string.Empty;
                this.txtServerPath.Text = string.Empty;
                this.txtNewDWGPath.Text = string.Empty;
               // this.txtNewDXFPath.Text = string.Empty;
                this.txtManager.Text = string.Empty;
                this.txtPrjectCity.Text = string.Empty;
                this.txtProjectName.Text = string.Empty;
                this.txtState.Text = string.Empty;
                this.txtSurveyor.Text = string.Empty;
                this.txtUserName.Text = string.Empty;
                this.txtStartTime.Text = string.Empty;
                this.txtEndTime.Text = string.Empty;
                this.txtCourseCode.Text = "000";
                this.chkCreate2013DXF.IsChecked = new bool?(true);
                this.chkSimplifyPolylines.IsChecked = new bool?(true);
                this.chkCreateBL.IsChecked = new bool?(true);
                this.chkDuplicates.IsChecked = new bool?(true);
                this.chkOmitSDXF.IsChecked = new bool?(true);
                this.chkReports.IsChecked = new bool?(true);
                this.Testing = false;
                this.ErrorMessage = "";
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void cmdDWGPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            directchoosedlg.SelectedPath= txtNewDWGPath.Text;
            if (directchoosedlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                this.txtNewDWGPath.Text = folderPath;
            }
        }

        private void cmdDXFPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
            }
        }

        private void LoadCourseInfoToTextboxes()
        {
            try
            {
                //1/31/2018
                txtTOURCode.Text = PGA.CourseName.UserControl1._courseTOURCode;
                txtCourseCode.Text = PGA.CourseName.UserControl1._courseNum;
                txtCourseName.Text = PGA.CourseName.UserControl1._courseName;
                txtState.Text      = PGA.CourseName.UserControl1._courseState;
                txtPrjectCity.Text = PGA.CourseName.UserControl1._courseCity;
                txtProjectName.Text = txtCourseName.Text +"-"+ DateTime.Now.Millisecond;
                txtSurveyor.Text = "Surveyor";
                txtManager.Text  = "Manager";
                txtUserName.Text = "Admin User";

               

            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void cmdExecute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.validate.ClearRedBorderOnEmpties(this);
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            try
            {
                LoadCourseInfoToTextboxes(); //1.31.2018

                if (this.StepTesting() && this.StepCreateTask())
                {
                    if (!this.StepLoadData())
                    {
                        this.ValidateTextboxes(this);
                        PGA.MessengerManager.MessengerManager.AddLog("Task Created: Failed!");
                    }
                    else
                    {
                        PGA.MessengerManager.MessengerManager.AddLog("Task Created: Successful!");
                    }
                }
            }
            catch (Exception exception1)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception1);
            }
        }

        private void cmdPointPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            directchoosedlg.SelectedPath = txtPointPath.Text;

            if (directchoosedlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               
                string folderPath = directchoosedlg.SelectedPath;
                this.txtPointPath.Text = folderPath;      
            }
        }

        private void cmdPolylinePath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            directchoosedlg.SelectedPath = txtPolylinePath.Text;

            if (directchoosedlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                this.txtPolylinePath.Text = folderPath;
            }
        }

        private void cmdReformPlys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    this.ConfigProperties(new Process());
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void cmdRefreshDate_Click(object sender, RoutedEventArgs e)
        {
            this.DateCreated = DateConverts.GetDateTimeNow();
            this.txtDateCreated.Text = DateConverts.DateTimeToStringForDatabase(this.DateCreated);
            this.txtDateCreated2.Text = this.DateCreated.ToString("yyyy-MM-dd hh:mm:ss");
        }

        private void cmdServerPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                this.txtServerPath.Text = folderPath;
            }
        }

        private void cmdTemplatePath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                this.txtTemplatePath.Text = folderPath;
            }
        }

        private void cmdTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string.IsNullOrEmpty(this.txtCourseCode.Text) ? false : this.txtCourseCode.Text.Length == 3))
                {
                    this.TestAccess(this.txtPointPath.Text);
                    this.TestAccess(this.txtPolylinePath.Text);
                    this.TestAccess(this.txtTemplatePath.Text);
                    this.TestAccess(this.txtNewDWGPath.Text);
                    //this.TestAccess(this.txtNewDXFPath.Text);
                    if ((string.IsNullOrEmpty(this.txtCourseName.Text) || string.IsNullOrEmpty(this.txtManager.Text) || string.IsNullOrEmpty(this.txtPrjectCity.Text) || string.IsNullOrEmpty(this.txtState.Text) || string.IsNullOrEmpty(this.txtSurveyor.Text) || string.IsNullOrEmpty(this.txtProjectName.Text) ? false : !string.IsNullOrEmpty(this.txtCourseCode.Text)))
                    {
                        this.Testing = true;
                    }
                    else
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAlert("Empty Fields!");
                        this.ErrorMessage = "Please fill out Project Information";
                        this.Testing = false;
                    }
                    PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Test Completed!");
                }
                else
                {
                    LoadCourseInfoToTextboxes();
                    //this.CourseCode = "000";
                    //this.txtCourseCode.Text = "000";
                    PGA.MessengerManager.MessengerManager.ShowMessage("Please enter the 3 digit Course Code!");
                    this.ErrorMessage = "Please enter the 3 digit Course Code!";
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void ConfigProperties(Process process)
        {
            try
            {
                string name = this.Get30MReformName();
                process.StartInfo.FileName = name;
                process.StartInfo.Arguments = "-n";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private static void CreateIfNotExists(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            if (!File.Exists(Path.Combine(path, fileName)))
            {
                string exePath = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath);
                File.Copy(Path.Combine(exePath, fileName), Path.Combine(path, fileName));
            }
        }

        private double CvtBuildToDouble(string version)
        {
            double num;
            try
            {
                int len = version.Length;
                version.Substring(9, 2);
                version.Substring(6, 2);
                version.Substring(0, 2);
                string BuildDay = version.Substring(0, 5);
                string BuildMin = "";
                BuildMin = (len != 15 ? version.Substring(12, len - 12) : version.Substring(12, 3));
                num = Convert.ToDouble(string.Format("{0}.{1}", BuildDay, BuildMin));
                return num;
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            num = 0;
            return num;
        }

        private string Get30MReformName()
        {
            string filename = "Autodesk\\ApplicationPlugins\\PGA-CivilTinSurf2018.bundle\\DB_Loader\\Bin\\30MReform\\PGA.SportVisionGUI.exe";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            PGA.MessengerManager.MessengerManager.AddLog(string.Concat("Set Path: ", Path.Combine(path, filename)));
            if (!File.Exists(Path.Combine(path, filename)))
            {
                PGA.MessengerManager.MessengerManager.AddLog(string.Concat("FileNotFound: ", Path.Combine(path, filename)));
                throw new FileNotFoundException(Path.Combine(path, filename));
            }
            PGA.MessengerManager.MessengerManager.AddLog(string.Concat("File Found: ", Path.Combine(path, filename)));
            return Path.Combine(path, filename);
        }



        private int GetCourseCode(string changes)
        {
            int num;
            try
            {
                int totallength = changes.Length;
                char[] delimiter = new char[] { Convert.ToChar("_") };
                string[] larray = changes.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                int result = 0;
                string[] strArrays = larray;
                int num1 = 0;
                while (num1 < (int)strArrays.Length)
                {
                    string item = strArrays[num1];
                    int.TryParse(item.Substring(0, 3), out result);
                    if (result <= 0)
                    {
                        num1++;
                    }
                    else
                    {
                        num = Convert.ToInt16(result);
                        return num;
                    }
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            num = 0;
            return num;
        }

        public static string GetProductVersion()
        {
            return MainWindow.Version.ToString();
        }

        private string GetProjectName(string changes)
        {
            string str;
            try
            {
                int totallength = changes.Length;
                char[] delimiter = new char[] { Convert.ToChar("\\") };
                string[] larray = changes.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                int result = 0;
                string[] strArrays = larray;
                int num = 0;
                while (num < (int)strArrays.Length)
                {
                    string item = strArrays[num];
                    string substr = null;
                    if (item.Contains("_"))
                    {
                        substr = item.Substring(item.Length - 3, 3);
                        int.TryParse(substr, out result);
                    }
                    if (result <= 0)
                    {
                        num++;
                    }
                    else
                    {
                        str = item;
                        return str;
                    }
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            str = null;
            return str;
        }

        public bool GetValueFromCheckBox(object sender)
        {
            bool flag;
            bool flag1;
            try
            {
                System.Windows.Controls.CheckBox chkBox = (System.Windows.Controls.CheckBox)sender;
                if (chkBox != null)
                {
                    if (!chkBox.IsChecked.HasValue)
                    {
                        flag1 = false;
                    }
                    else
                    {
                        flag1 = (chkBox.IsChecked.Value ? true : false);
                    }
                    flag = flag1;
                    return flag;
                }
                else
                {
                    flag = false;
                    return flag;
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            flag = false;
            return flag;
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void Grid_TextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                this.validate.ClearRedBorderOnEmpties(this);
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }



        private void GSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                GSettings = TaskManagerUC.TaskManagerUC._settings as Settings;
                if (GSettings.StartTime != null)
                    txtStartTime.Text = GSettings.StartTime.ToString();
                else
                    txtStartTime.Text = "";
                if (GSettings.EndTime != null)
                    txtEndTime.Text = GSettings.EndTime.ToString();
                else
                {
                    txtEndTime.Text = "";
                }
                if (GSettings.ProjectManager != null) txtManager.Text = GSettings.ProjectManager;
                if (GSettings.ProjectCity != null) txtPrjectCity.Text = GSettings.ProjectCity;
                if (GSettings.ProjectName != null) txtProjectName.Text = GSettings.ProjectName;
                txtState.Text = GSettings.ProjectState;
                txtSurveyor.Text = GSettings.ProjectSurveyor;
                txtUserName.Text = GSettings.ProjectCreator;
                chkPDFReport.IsChecked = GSettings.PDFReport == true ? true : false;
                chk3DPolys.IsChecked = GSettings.Out3DPolys == true ? true : false;
                chkProbing.IsChecked = GSettings.Probing == true ? true : false;
                chkCreate2013DXF.IsChecked = GSettings.CreateDXF == true ? true : false;
                //chkSimplifyPolylines.IsChecked = GSettings.SimplifyPlines == true ? true : false;
                chkOmitSDXF.IsChecked = GSettings.SkipSDxf == true ? true : false;
                chkCreateBL.IsChecked = GSettings.CreateBreaklines == true ? true : false;
                chkDuplicates.IsChecked = GSettings.DuplicateSurfaces == true ? true : false;
                chkErrReports.IsChecked = GSettings.GenErrReport == true ? true : false;
                chkReports.IsChecked = GSettings.GenReports == true ? true : false;
                chkSportVision.IsChecked = GSettings.SportVision == true ? true : false;
                chkWaterBL.IsChecked = GSettings.AddFLWaterSurf == true ? true : false;
                txtPointPath.Text = GSettings.GlobalPointFilePath;
                txtPolylinePath.Text = GSettings.GlobalPolylineFilePath;
                txtTemplatePath.Text = GSettings.TemplateDWG;
                txtServerPath.Text = GSettings.SeverPath;
                txtNewDWGPath.Text = GSettings.DestinationFolder;
                //txtNewDXFPath.Text = GSettings.DestDXFFolder;
                txtCourseCode.Text = GSettings.CourseCode.ToString();
                txtCourseName.Text = GSettings.CourseName;
                txtUserName.Text = GSettings.ProjectCreator;
                txtState.Text = GSettings.ProjectState;
                UC_Simplify.placeholder.Text = GSettings.SimplifyValue.Value.ToString();
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        private void GSettings_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void InitializeTaskManager()
        {
            try
            {
                this.txtDateCreated.IsReadOnly = true;
                this.txtStartTime.IsReadOnly = true;
                this.txtEndTime.IsReadOnly = true;
                this.txtManager.ToolTip = "Enter Manager";
                this.txtPrjectCity.ToolTip = "Course City";
                this.txtProjectName.ToolTip = "ie. RTJ Grand National";
                this.txtState.ToolTip = "Course State";
                this.txtSurveyor.ToolTip = "Surveyor";
                this.txtUserName.ToolTip = "Initials RB";
                this.DateCreated = DateConverts.GetDateTimeNow();
                this.txtDateCreated.Text = DateConverts.DateTimeToStringForDatabase(this.DateCreated);
                this.txtDateCreated2.Text = this.DateCreated.ToString("yyyy-MM-dd hh:mm:ss"); //for display only
                this.chkCreate2013DXF.IsChecked = new bool?(true);
                this.chkSimplifyPolylines.IsChecked = new bool?(true);
                this.chkCreateBL.IsChecked = new bool?(false);
                this.chkSportVision.IsChecked = new bool?(false);
                this.chk3DPolys.IsChecked = new bool?(false);
                this.chkProbing.IsChecked = new bool?(true);
                this.chkPDFReport.IsChecked = new bool?(true);
                this.chkErrReports.IsChecked = new bool?(true);
                this.chkDuplicates.IsChecked = new bool?(true);
                this.chkOmitSDXF.IsChecked = new bool?(true);
                this.chkReports.IsChecked = new bool?(true);
                this.chkWaterBL.IsChecked = new bool?(true);
                this.Testing = false;
                this.ErrorMessage = "";
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        public static int InitializeTimer()
        {
            MainWindow.myTimer.Tick += new EventHandler(MainWindow.TimerEventProcessor);
            MainWindow.myTimer.Interval = 5000;
            MainWindow.myTimer.Stop();
            return 0;
        }

        private void SetVersionInDatabase(string getProductVersion)
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {
                    commands.SetBuildDBVersionInUpdate(getProductVersion);
                }
                catch (Exception exception)
                {
                    PGA.MessengerManager.MessengerManager.LogException(exception);
                }
            }
        }
        private void SetVersionDBDatabase(string getProductVersion)
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {
                    commands.SetBuildVersionInUpdate(getProductVersion);
                }
                catch (Exception exception)
                {
                    PGA.MessengerManager.MessengerManager.LogException(exception);
                }
            }
        }


        private bool StepCreateTask()
        {
            try
            {
                if (!Testing)
                {
                    PGA.MessengerManager.MessengerManager.ShowMessageAndLog(string.IsNullOrEmpty
                      (ErrorMessage) ? "StepCreateTask: Unknown Error Occurred!" : ErrorMessage);
                    return false;
                }
                //check time stamp

                if (CheckTimeStamp())
                {
                    RenewTimeStamp();
                }

                using (DatabaseCommands commands = new DatabaseCommands())
                {
              
                    Settings settings = new Settings();

                    if (txtDateCreated.Text != null)
                    {
                        settings.DateStamp = DateCreated;
                        settings.CourseName = txtCourseName.Text;
                        settings.SkipSDxf = GetValueFromCheckBox(chkOmitSDXF);
                        settings.CreateDXF = GetValueFromCheckBox(chkCreate2013DXF);
                        //settings.CreateHolesXX = GetValueFromCheckBox(chkCreateHoles);
                        //settings.Smoothing = GetValueFromCheckBox(chkSmoothing);
                        settings.PDFReport = GetValueFromCheckBox(chkPDFReport);
                        settings.Out3DPolys = GetValueFromCheckBox(chk3DPolys);
                        settings.SimplifyPlines = GetValueFromCheckBox(chkSimplifyPolylines);
                        settings.GenReports = GetValueFromCheckBox(chkReports);
                        settings.AddFLWaterSurf = GetValueFromCheckBox(chkWaterBL);
                        settings.CreateBreaklines = GetValueFromCheckBox(chkCreateBL);
                        settings.GenErrReport = GetValueFromCheckBox(chkErrReports);
                        settings.SportVision = GetValueFromCheckBox(chkSportVision);
                        settings.Probing = GetValueFromCheckBox(chkProbing);
                        settings.DuplicateSurfaces = GetValueFromCheckBox(chkDuplicates);
                        //settings.DestDXFFolder = txtNewDXFPath.Text;
                        settings.DestinationFolder = txtNewDWGPath.Text;
                        settings.GlobalPointFilePath = txtPointPath.Text;
                        settings.GlobalPolylineFilePath = txtPolylinePath.Text;
                        settings.ProjectCity = txtPrjectCity.Text;
                        settings.ProjectCreator = txtUserName.Text;
                        settings.ProjectDate = DateCreated;
                        settings.ProjectManager = txtManager.Text;
                        settings.ProjectSurveyor = txtSurveyor.Text;
                        //settings.SeverPath = txtServerPath.Text;
                        settings.ProjectName = txtProjectName.Text;
                        settings.ProjectState = txtState.Text;
                        //settings.TemplateDWG = txtTemplatePath.Text;
                        settings.CourseCode = txtCourseCode.Text;
                        settings.TourCode = txtTOURCode.Text; //PGA.CourseName.UserControl1._courseTOURCode;
                        settings.SimplifyValue = UC_Simplify.SimplifyValue;
                    }

                    commands.NewTask(DateCreated, settings);
                    TaskManagerUC.TaskManagerUC._TextEventPlaceHolder.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                }
                MessageBox.Show("Task Created! " + DateCreated);
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return true;
        }

        private bool StepLoadData()
        {
            bool flag;
            if (this.Testing)
            {
                try
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        FileInfo[] Pointfiles = FileLoader.ReadFiles(this.txtPointPath.Text, "*.txt");
                        FileInfo[] PolyLinefiles = FileLoader.ReadFiles(this.txtPolylinePath.Text, "*.dwg");
                        commands.LoadData(this.DateCreated, Pointfiles, PolyLinefiles, this.DestFolderPath.SelectedPath);
                        this.DateCreated = DateConverts.GetDateTimeNow();
                        this.txtDateCreated.Text = DateConverts.DateTimeToStringForDatabase(this.DateCreated);

                        commands.ClearTimerInfo();

                    }
                    this.ClearTextBoxes();
                }
                catch (Exception exception)
                {
                    Exception ex = exception;
                    PGA.MessengerManager.MessengerManager.ShowMessageAlert(ex);
                    PGA.MessengerManager.MessengerManager.LogException(ex);
                }
                flag = true;
            }
            else
            {
                PGA.MessengerManager.MessengerManager.ShowMessageAndLog((string.IsNullOrEmpty(this.ErrorMessage) ? " StepLoadData: Unknown Error Occurred!" : this.ErrorMessage));
                flag = false;
            }
            return flag;
        }

        private bool StepTesting()
        {
            bool flag;
            try
            {
                if (!(string.IsNullOrEmpty(this.txtCourseCode.Text)))
                {
                    this.TestAccess(this.txtPointPath.Text);
                    if (!this.TestFileAccess)
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Network Failure: Point Path!");
                    }
                    this.TestAccess(this.txtPolylinePath.Text);
                    if (!this.TestFileAccess)
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Network Failure: Polyline Path!");
                    }
                    this.TestAccess(this.txtNewDWGPath.Text);
                    if (!this.TestFileAccess)
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Network Failure: Drawing Path!");
                    }
                    if ((string.IsNullOrEmpty(this.txtCourseName.Text) || string.IsNullOrEmpty(this.txtManager.Text) || string.IsNullOrEmpty(this.txtPrjectCity.Text) || string.IsNullOrEmpty(this.txtState.Text) || string.IsNullOrEmpty(this.txtSurveyor.Text) || string.IsNullOrEmpty(this.txtProjectName.Text) || string.IsNullOrEmpty(this.txtCourseCode.Text) || string.IsNullOrEmpty(this.txtPointPath.Text) || string.IsNullOrEmpty(this.txtPolylinePath.Text) ? false : !string.IsNullOrEmpty(this.txtNewDWGPath.Text)))
                    {
                        this.Testing = true;
                    }
                    else
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAlert("Empty Fields!");
                        this.ErrorMessage = "Please fill out Project Information!";
                        this.ValidateTextboxes(this);
                        this.Testing = false;
                        flag = false;
                        return flag;
                    }
                }
                else
                {
                    PGA.MessengerManager.MessengerManager.ShowMessage("Please enter the 3 digit Course Code!");
                    this.ErrorMessage = "Please enter the 3 digit Course Code!";
                    this.ValidateTextboxes(this);
                    flag = false;
                    return flag;
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            flag = true;
            return flag;
        }



        private bool TestAccess(string path)
        {

            TestFileAccess = true;

            if (String.IsNullOrEmpty(path)) return false;

            ReadWriteTest.CurrentUserSecurity security = new ReadWriteTest.CurrentUserSecurity();

            DirectoryInfo directory = new DirectoryInfo(path);

            if (!security.HasAccess(directory, FileSystemRights.Read))
                MessageBox.Show("Directory does not have Read Access! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.Write))
                MessageBox.Show("Directory does not have Write Access! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.Delete))
                MessageBox.Show("Directory does not have Delete Permissions! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.AppendData))
                MessageBox.Show("Directory does not have Append Permissions! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.CreateDirectories))
                MessageBox.Show("Directory does not have Create Directory Permissions! " + path);
            else if (!security.HasAccess(directory, FileSystemRights.CreateFiles))
                MessageBox.Show("Directory does not have Create file Permissions! " + path);
            else
            {
                return true; //has access
            }

            TestFileAccess = false;
            return false; //failed
        }

        private static void TimerEventProcessor(object myObject, EventArgs myEventArgs)
        {
            MainWindow.GSettings = PGA.TaskManagerUC.TaskManagerUC._settings as Settings;
        }

        private void txtCourseCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                this.CourseCode = this.txtCourseCode.Text;
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void txtCourseName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Random r = new Random();
                string text = this.txtCourseName.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.ToUpper();
                }
                if ((!e.Changes.Any<TextChange>() || text.Length <= 0 ? false : text.Length < 5))
                {
                    IEnumerable<TextChange> additions =
                        from tc in e.Changes
                        where tc.AddedLength > 0
                        select tc;
                    IEnumerable<string> newTexts =
                        from tc in additions
                        select text.Substring(tc.Offset, tc.AddedLength);
                    if (text.Length <= 5)
                    {
                        this.txtProjectName.Text = string.Format("T-{0}-{1}", text.Substring(0, text.Length).TrimEnd(new char[0]), r.Next(1000, 9999));
                    }
                    else
                    {
                        this.txtProjectName.Text = string.Format("T-{0}-{1}", text.Substring(0, 5).TrimEnd(new char[0]), r.Next(1000, 9999));
                    }
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void TxtStartTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.txtElapsedTime.Text = "";
            try
            {
                if ((this.txtStartTime.Text == null ? false : this.txtEndTime.Text != null))
                {
                    try
                    {
                        DateTime d1 = Convert.ToDateTime(this.txtStartTime.Text);
                        DateTime d2 = Convert.ToDateTime(this.txtEndTime.Text);
                        this.txtElapsedTime.Text = this.time.CalcTimeSpan(d1, d2);
                    }
                    catch
                    {
                        this.txtElapsedTime.Text = "";
                    }
                }
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        public void ValidateTextboxes(MainWindow _MainWindow)
        {
            try
            {
                this.validate.SetRedBorderOnEmpties(_MainWindow);
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.lblVersion.Content = string.Format("{0}", MainWindow.GetProductVersion());
                this.lblVersion.Visibility = System.Windows.Visibility.Hidden;
                this.lnkHomePage.Inlines.Add(this.lblVersion.Content.ToString());
            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
            try
            {
                this.SetVersionInDatabase(MainWindow.GetProductVersion());
                SetVersionDBDatabase(MainWindow.GetProductVersion());
            }
            catch (Exception exception1)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception1);
            }
        }


        private SolidColorBrush colorBegin = new SolidColorBrush(Colors.Coral);

        private SolidColorBrush colorEnd   = new SolidColorBrush(Colors.Transparent);

        public SolidColorBrush ColorBegin
        {
            get { return colorBegin; }
            set
            {
                if (value != colorBegin)
                {
                    colorBegin = value;
                    OnPropertyChanged("ColorBegin");
                }
            }
        }


        public SolidColorBrush ColorEnd
        {
            get { return colorEnd; }
            set
            {
                if (value != colorEnd)
                {
                    colorEnd = value;
                    OnPropertyChanged("ColorEnd");
                }
            }
        }

          
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private void togSportvisionRun_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.chkSportVision.IsChecked = new bool?(true);
                this.chk3DPolys.IsChecked     = new bool?(true);
                this.chkCreateBL.IsChecked    = new bool?(true);
                this.chkOmitSDXF.IsChecked    = new bool?(false);
                this.chkErrReports.IsChecked  = new bool?(false);
                this.chkProbing.IsChecked     = new bool?(false);
                this.chkDuplicates.IsChecked  = new bool?(false);
                this.chkPDFReport.IsChecked   = new bool?(false);
                this.chkSimplifyPolylines.IsChecked     = new bool?(true);
                this.chkStrictNames.IsChecked = new bool?(false);

            }
            catch (Exception exception1)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception1);
            }
        }

       
    }



    public static class FileLoader
    {
        public static FileInfo[] ReadFiles(string folder, string delimiter)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            FileInfo[] files = di.GetFiles(delimiter);

            return files;
        }
    }


    public static class ReadWriteTest
    {
        public class CurrentUserSecurity
        {
            WindowsIdentity _currentUser;
            WindowsPrincipal _currentPrincipal;

            public CurrentUserSecurity()
            {
                _currentUser = WindowsIdentity.GetCurrent();
                _currentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            }

            public bool HasAccess(DirectoryInfo directory, FileSystemRights right)
            {
                // Get the collection of authorization rules that apply to the directory.
                AuthorizationRuleCollection acl = directory.GetAccessControl()
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));
                return HasFileOrDirectoryAccess(right, acl);
            }

            public bool HasAccess(FileInfo file, FileSystemRights right)
            {
                // Get the collection of authorization rules that apply to the file.
                AuthorizationRuleCollection acl = file.GetAccessControl()
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));
                return HasFileOrDirectoryAccess(right, acl);
            }

            private bool HasFileOrDirectoryAccess(FileSystemRights right,
                                                  AuthorizationRuleCollection acl)
            {
                bool allow = false;
                bool inheritedAllow = false;
                bool inheritedDeny = false;

                for (int i = 0; i < acl.Count; i++)
                {
                    FileSystemAccessRule currentRule = (FileSystemAccessRule)acl[i];
                    // If the current rule applies to the current user.
                    if (_currentUser.User.Equals(currentRule.IdentityReference) ||
                        _currentPrincipal.IsInRole(
                                        (SecurityIdentifier)currentRule.IdentityReference))
                    {

                        if (currentRule.AccessControlType.Equals(AccessControlType.Deny))
                        {
                            if ((currentRule.FileSystemRights & right) == right)
                            {
                                if (currentRule.IsInherited)
                                {
                                    inheritedDeny = true;
                                }
                                else
                                { // Non inherited "deny" takes overall precedence.
                                    return false;
                                }
                            }
                        }
                        else if (currentRule.AccessControlType
                                                        .Equals(AccessControlType.Allow))
                        {
                            if ((currentRule.FileSystemRights & right) == right)
                            {
                                if (currentRule.IsInherited)
                                {
                                    inheritedAllow = true;
                                }
                                else
                                {
                                    allow = true;
                                }
                            }
                        }
                    }
                }

                if (allow)
                { // Non inherited "allow" takes precedence over inherited rules.
                    return true;
                }
                return inheritedAllow && !inheritedDeny;
            }
        }

    }

    public static class Parser
    {
        public static string ParseConnectionString(string conn)
        {
            string[] str = conn.Split(new string[] { ";" }, StringSplitOptions.None);

            foreach (string s in str)
            {
                if (CheckFirstName(s))
                {
                    return s;
                }
            }

            return String.Empty;
        }

        public static string strdelimiter()
        {
            string s = ";";
            return s.ToString();
        }

        public static char cdelimiter()
        {
            char[] value = new char[1];
            value[0] = ';';
            return ';';
        }

        public static bool CheckFirstName(string value)
        {
            string[] str = value.Split(new string[] { "\\" }, StringSplitOptions.None);
            foreach (string s in str)
            {
                string word = "DATA SOURCE";
                CultureInfo culture = new CultureInfo("en");
                if (culture.CompareInfo.IndexOf(s.ToUpper(), "Data Source", CompareOptions.IgnoreCase) > 0)
                    return true;

            }
            return false;
        }

    }

}

