using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PGA.Database;
using PGA.DataContext;
using UserControl = System.Windows.Controls.UserControl;

namespace PGA.CourseName
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public static  string _courseName = String.Empty;
        public static string _courseCity = String.Empty;
        public static string _courseState= String.Empty;
        public static string _courseNum= String.Empty;
        public static string _courseTOURCode= String.Empty;

        public UserControl1()
        {
            try
            {
                InitializeComponent();
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    LoadCourses();
                }
            }
            catch (Exception)
            {
                 
            }
        }

        private void LoadCourses()
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var details = commands.GetCourseDetails();
                    this.DataContext = details;

                    if (details == null)
                        return;
                    this.CourseUC.cboCourseName.ItemsSource = details.Select(courseDetails => courseDetails.Name);
                    this.CourseUC.cboCourseName.SelectedItem = details.Select(courseDetails => courseDetails.Name).First();

                }

            }
            catch (Exception exception)
            {
                PGA.MessengerManager.MessengerManager.LogException(exception);
            }
        }

        private void cmdExport_Click(object sender, RoutedEventArgs e)
        {
            string filepath = string.Empty;
            string name = "TOUR_COURSES.txt";

            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = directchoosedlg.SelectedPath;
                filepath = System.IO.Path.Combine(folderPath, name);
                try
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        var details = commands.GetCourseDetails().ToArray();


                        StreamWriter writer = new StreamWriter(filepath, false);
                        foreach (var line in details)
                        {
                            writer.WriteLine(String.Format("{0},{1},{2},{3},{4}", line.Name.Trim(), line.City.Trim(),
                                line.State.Trim(), line.CourseNum.Trim(), line.TOURCode.Trim()));
                        }
                        writer.Close();
                    }

                }
                catch (Exception exception)
                {
                    PGA.MessengerManager.MessengerManager.LogException(exception);
                }
            }
        }

        private string CleanField(string field)
        {
            try
            {
               return  field.Trim();
            }
            catch { }
            
            return String.Empty;
        }

        private void cmdImport_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        commands.DeleteAllFromCourses();
                        ClearBoxes();
                    }
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        string[] lines = File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);

                        foreach (var line in lines)
                        {
                            string[] contents = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            
                            commands.InsertCourseDetail
                            (CleanField(contents[0]), CleanField(contents[1]), CleanField(contents[2]),
                               CleanField(contents[3]), CleanField(contents[4]));

                        }

                    }
                   LoadCourses();

                }

            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.ShowMessageAndLog("cmdOpen_Click" + ex.Message);
            }
        }

        private void cboCourseName_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        

            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {
                    if (cboCourseName.ItemsSource == null)
                        return;
                    if (e.AddedItems.Count == 0)
                        return;
                    txtCourseName.Text = e.AddedItems[0].ToString();


                    var selected = commands.GetCourseDetails();

                    var item = selected.Where(p => p.Name.Equals(e.AddedItems[0]));
                    txtCourseCode.Text = item.FirstOrDefault().CourseNum.Trim();
                    txtState.Text      = item.FirstOrDefault().State.Trim();
                    txtPrjectCity.Text = item.FirstOrDefault().City.Trim();
                    txtTOURCode.Text   = item.FirstOrDefault().TOURCode.Trim();

                    //assign to globals

                    _courseName  = txtCourseName.Text;
                    _courseState = txtState.Text;
                    _courseCity = txtPrjectCity.Text;
                    _courseNum  = txtCourseCode.Text;
                    _courseTOURCode = txtTOURCode.Text;
                }
                catch (System.NullReferenceException)
                {
                    
                }

                catch (Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Error in Delete" + ex.Message);
                }
            }


        }

        private void txtDeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {

                    var selected = cboCourseName.SelectedItem.ToString();
                    commands.DeleteCourseByName(selected);
                    this.CourseUC.cboCourseName.SelectedItem =-1;
                    LoadCourses();
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Error in Delete" + ex.Message);
                }

            }
        }

        private void txtAddCourse_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {
                    try
                    {
                        if (String.IsNullOrEmpty(txtCourseName.Text)
                            || String.IsNullOrEmpty(txtPrjectCity.Text)
                            || String.IsNullOrEmpty(txtState.Text)
                            || String.IsNullOrEmpty(txtCourseCode.Text)
                            || String.IsNullOrEmpty(txtTOURCode.Text))
                            throw new Exception("Empty Fields");

                        if (!((txtCourseCode.Text.All(char.IsDigit)) && (txtTOURCode.Text.All(char.IsLetter))))
                            throw new Exception("Numbers in Text Fields");

                    }
                    catch(Exception ex)
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Empty Fields or " + ex.Message);
                        throw;
                    }

                    var n = txtCourseName.Text;
                    var c = txtPrjectCity.Text;
                    var s = txtState.Text;
                    var num = txtCourseCode.Text.Trim();
                    var cod = txtTOURCode.Text.Trim();

                    var detail = new CourseDetails();

                    detail.Name = n;
                    detail.City = c;
                    detail.State = s;
                    detail.CourseNum = num;
                    detail.TOURCode = cod;

                     
                    if ((commands.GetCourseDetails().Select(p => p.Name == n).FirstOrDefault()))
                    {
                        PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Course Name Already in Database");
                        return;
                    }
                
                    commands.InsertCourseDetail(n,c,s,num,cod);

                    LoadCourses();
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.AddLog("Error in Add" + ex.Message);
                }

            }

        }
    

        private void txtUpdateCourse_Click(object sender, RoutedEventArgs e)
        {
            using (DatabaseCommands commands = new DatabaseCommands())
            {
                try
                {

                    var selected = cboCourseName.SelectedItem.ToString();
                    var n = txtCourseName.Text;
                    var c = txtPrjectCity.Text;
                    var s = txtState.Text;
                    var num = txtCourseCode.Text;
                    var cod = txtTOURCode.Text;

                    var detail = new CourseDetails();

                    detail.Name = n;
                    detail.City = c;
                    detail.State = s;
                    detail.CourseNum = num.Trim();
                    detail.TOURCode  = cod.Trim();

                    commands.UpdateCourseDetails(selected,detail);

                    LoadCourses();
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Error in Delete" + ex.Message);
                }

            }
        }

        private void txtNewCourse_Click(object sender, RoutedEventArgs e)
        {
            ClearBoxes();
        }

        private void ClearBoxes()
        {
            try
            {
                cboCourseName.ItemsSource = String.Empty;
                cboCourseName.SelectedItem = null;
                txtTOURCode.Text = "";
                txtPrjectCity.Text = "";
                txtState.Text = "";
                txtCourseCode.Text = "";
                txtCourseName.Text = "";
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Error in ClearBoxes" + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearBoxes();
                LoadCourses();
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.ShowMessageAndLog("Error in ClearBoxes" + ex.Message);
            }
        }
    }
}
