using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PGA.Database;

namespace PGA.AdjustSimplify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public DateTime DateSelected { get; set; }
        public double SimplifyValue { get; set; }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cbo = (ComboBox)sender;
                SimplifyValue = Convert.ToDouble(cbo.SelectionBoxItem.ToString());
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
    }
}
