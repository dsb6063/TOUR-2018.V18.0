using System;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using PGA.Database;
using WinForms = System.Windows.Forms;
using PGA.DataContext;

namespace PGA.DatabaseManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public WinForms.FolderBrowserDialog PointFilePath    { get; private set; }
        public WinForms.FolderBrowserDialog PolylineFilePath { get; private set; }
        public WinForms.FolderBrowserDialog TemplateFilePath { get; private set; }
        public WinForms.FolderBrowserDialog ServerFilePath    { get; private set; }
        public WinForms.FolderBrowserDialog DestFolderPath    { get; private set; }
        public WinForms.FolderBrowserDialog DxfFolderFilePath { get; private set; }
        public string MyconnectionString { get; private set; }

        public MainWindow()
        {   
            InitializeComponent();

            PointFilePath = new WinForms.FolderBrowserDialog();
            PolylineFilePath = new WinForms.FolderBrowserDialog();
            TemplateFilePath = new WinForms.FolderBrowserDialog();
            ServerFilePath = new WinForms.FolderBrowserDialog();
            DestFolderPath = new WinForms.FolderBrowserDialog();
            DxfFolderFilePath = new WinForms.FolderBrowserDialog();

            InitializeTaskManager();
         

        }

        private void InitializeTaskManager()
        {
            txtDateCreated.IsReadOnly   = true;
            txtStartTime.IsReadOnly     = true;
            txtEndTime.IsReadOnly       = true;

            txtManager.ToolTip = "Enter Manager";
            txtPrjectCity.ToolTip = "Course City";
            txtProjectName.ToolTip = "RTJ Grand National_819";
            txtState.ToolTip = "Course State";
            txtSurveyor.ToolTip = "Surveyor";
            txtUserName.ToolTip = "Initials RB";

            DateCreated = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            txtDateCreated.Text = DateCreated.ToString();
            chkCreateDXF.IsChecked = true;

    }

        public DateTime DateCreated { get; set; }
        public int TaskId { get; set; }
        public int SettingID { get; set; }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
       
        }


        //public void LoadSettingsData(PGAContext source)
        //{

        //   //DateCreated = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

        //    //Settings myseSettings = new Settings();

        //    //myseSettings.DateStamp = DateTime.Now;

        //    //source.Settings.InsertOnSubmit(myseSettings);
        //    //source.SubmitChanges();
                
       
        //}


        private void cmdPointPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath;
            WinForms.FolderBrowserDialog directchoosedlg = new WinForms.FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == WinForms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                txtPointPath.Text = folderPath;
            }
        }

        private void cmdNewTask_Click(object sender, RoutedEventArgs e)
        {
            //check time stamp

            if (CheckTimeStamp())
                return;
   
            using (CleanUp clean = new CleanUp())
            {
                  
                Settings settings = new Settings();

                if (txtDateCreated.Text != null)
                {
                    settings.DateStamp = DateCreated;
                    settings.CourseName = txtCourseName.Text;
                    settings.CreateDXF = chkCreateDXF.IsChecked != null && ((bool)chkCreateDXF.IsChecked ? true : false);
                    settings.DestDXFFolder = txtNewDXFPath.Text;
                    settings.DestinationFolder = txtNewDWGPath.Text;
                    settings.GlobalPointFilePath = txtPointPath.Text;
                    settings.GlobalPolylineFilePath = txtPolylinePath.Text;
                    settings.ProjectCity = txtPrjectCity.Text;
                    settings.ProjectCreator = txtUserName.Text;
                    settings.ProjectDate = DateCreated;
                    settings.ProjectManager = txtManager.Text;
                    settings.ProjectSurveyor = txtSurveyor.Text;
                    settings.SeverPath = txtServerPath.Text;
                    settings.ProjectName = txtProjectName.Text;
                    settings.TemplateDWG = txtTemplatePath.Text;
                    //myseSettings.StartTime = String.Empty;
                    //myseSettings.EndTime   =  String.Empty;
                }
                
                DatabaseCommands commands = new DatabaseCommands();
                commands.NewTask(DateCreated, settings );

            }
            MessageBox.Show("Task Created! " + DateCreated);
        }

        public bool CheckTimeStamp()
        {

            DatabaseCommands commands = new DatabaseCommands();
            return commands.CheckTimeStamp(txtDateCreated.Text);
            return false;
        }



        private static void CreateIfNotExists(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Set the data directory to the users %AppData% folder            
            // So the database file will be placed in:  C:\\Users\\<Username>\\AppData\\Roaming\\            
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            // Enure that the database file is present
            if (!System.IO.File.Exists(System.IO.Path.Combine(path, fileName)))
            {
                //Get path to our .exe, which also has a copy of the database file
                var exePath = System.IO.Path.GetDirectoryName(
                    new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                //Copy the file from the .exe location to the %AppData% folder
                System.IO.File.Copy(
                    System.IO.Path.Combine(exePath, fileName),
                    System.IO.Path.Combine(path, fileName));
            }
        }

        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmdPolylinePath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            WinForms.FolderBrowserDialog directchoosedlg = new WinForms.FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == WinForms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                txtPolylinePath.Text = folderPath;
            }
        }

        private void cmdTemplatePath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            WinForms.FolderBrowserDialog directchoosedlg = new WinForms.FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == WinForms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                txtTemplatePath.Text = folderPath;
            }
        }

        private void cmdServerPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            WinForms.FolderBrowserDialog directchoosedlg = new WinForms.FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == WinForms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                txtServerPath.Text = folderPath;
            }
        }

        private void cmdDWGPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            WinForms.FolderBrowserDialog directchoosedlg = new WinForms.FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == WinForms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                txtNewDWGPath.Text = folderPath;
            }
        }

        private void cmdDXFPath_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = "";
            WinForms.FolderBrowserDialog directchoosedlg = new WinForms.FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == WinForms.DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
                txtNewDXFPath.Text = folderPath;
            }
        }

        private void cmdTest_Click(object sender, RoutedEventArgs e)
        {
            TestAccess(txtPointPath.Text);
            TestAccess(txtPolylinePath.Text);
            TestAccess(txtTemplatePath.Text);
            TestAccess(txtNewDWGPath.Text);
            TestAccess(txtNewDXFPath.Text);
            //TestAccess(txtServerPath.Text);

            MessageBox.Show("Test Completed!");
        }

        private void cmdLoadData_Click(object sender, RoutedEventArgs e)
        {    
           
            using (CleanUp clean = new CleanUp())
            {
                DatabaseCommands commands = new DatabaseCommands();
                FileInfo[] Pointfiles    = FileLoader.ReadFiles(txtPointPath.Text, "*.txt");
                FileInfo[] PolyLinefiles = FileLoader.ReadFiles(txtPolylinePath.Text, "*.dwg");
                commands.LoadData(DateCreated, Pointfiles, PolyLinefiles, DestFolderPath.SelectedPath);
                #region MyRegion

                // renew DateStamp 
                #endregion
                DateCreated = DateTime.Now;
            }
            MessageBox.Show("Load Completed!");
        }



        private bool TestAccess(string path)
        {
            if (String.IsNullOrEmpty(path)) return false;//throw new ArgumentNullException("path");
            
            ReadWriteTest.CurrentUserSecurity security = new ReadWriteTest.CurrentUserSecurity();
            
            DirectoryInfo directory = new DirectoryInfo(path);

            if (!security.HasAccess(directory, FileSystemRights.Read))
                MessageBox.Show("Directory does not have Read Access! " + path);
            else if(!security.HasAccess(directory, FileSystemRights.Write))
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
            return false;    //failed
        }

    }

    class CleanUp : IDisposable
    {
        public void Dispose()
        {
            //Console.WriteLine("Execute  Second");
        }
    }

    public static class FileLoader
    {
        public static FileInfo[] ReadFiles(string folder,string delimiter)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            FileInfo[] files = di.GetFiles(delimiter);

            return files;
        }

    }

    public static class ReadWriteTest
    {
        public  class CurrentUserSecurity
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
            string[] str =  conn.Split(new string[] { ";" }, StringSplitOptions.None);

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
            string s =  ";";
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
