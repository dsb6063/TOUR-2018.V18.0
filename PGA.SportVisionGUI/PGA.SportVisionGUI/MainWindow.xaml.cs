using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using PGA.Database;
using MessageBox = System.Windows.MessageBox;
using COMS = PGA.MessengerManager.MessengerManager;

namespace PGA.SportVisionGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog folderBrowserDialog1;
        private bool state = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void cmdSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var count = 0;
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);

                await ClearData(sender);

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var filtered = files.Where(p => p.EndsWith(".dwg", true, CultureInfo.InvariantCulture));
                    if (filtered.Any())
                    {
                        commands.InsertIntoGeneralUseDwgs(filtered.ToList());
                        count = filtered.Count();
                    }
                }

                MessageBox.Show("Files Loaded: " + count, "Message");
            }
            catch (Exception ex)
            {
                COMS.ShowMessageAndLog("Please select files!");
            }

        }

        private void cmdGetDir_Click(object sender, RoutedEventArgs e)
        {
            folderBrowserDialog1 = new FolderBrowserDialog();
            try
            {   
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
 
                    txtDirectory.Text = folderBrowserDialog1.SelectedPath;
 
                }
            }
            catch (Exception ex)
            {
                COMS.LogException(ex);
            }
        }

        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkbox = (System.Windows.Forms.CheckBox)sender;
                if (checkbox != null & checkbox.Checked == true)
                {
                    await ClearData(sender);
                }
            }
            catch (Exception ex)
            {
                COMS.LogException(ex);
            }
        }


        public async Task<int> ClearData(object sender)
        {

            try
            {
                if (state)
                {
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        var delete = commands.DeleteAllFromGUStack();
                    }
                }
      
            }
            catch (Exception ex)
            {
                COMS.LogException(ex);
            }
            return 1;
        }

        private void Clear_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = (System.Windows.Controls.CheckBox)sender;
            if (checkbox != null & checkbox.IsChecked == true)
            {
                state = true;
            }
            else
            {
                state = false;

            }
        }
    }
}
