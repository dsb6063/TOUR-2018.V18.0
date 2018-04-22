using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace PGA.AdjustSimplify
{
    /// <summary>
    /// Interaction logic for UC_Simplify.xaml
    /// </summary>
    public partial class UC_Simplify : UserControl, INotifyPropertyChanged
    {
        public UC_Simplify()
        {
            InitializeComponent();

            placeholder= new TextBox();
            placeholder.TextChanged += Placeholder_TextChanged;
        }

        private static DateTime DateSelected { get; set; }
        public static double SimplifyValue  { get; set; }
        public static TextBox placeholder;


        private void Placeholder_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(placeholder.Text))
                    return;

                SimplifyValue = Convert.ToDouble(placeholder.Text.Trim());
                int i = 0;
                foreach (var item in comboBox.Items)
                {
                    if (Math.Abs(Convert.ToDouble(placeholder.Text.Trim()) -  Convert.ToDouble((item as ListBoxItem).Content.ToString())) < 0.0001)
                    {
                        comboBox.SelectedIndex = i;
                        Debug.WriteLine("placeholder " + placeholder.Text + "index " + i);
                        break;
                    }

                    i++;
                }

            }
           
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

       


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null)
                    SimplifyValue = Convert.ToDouble
                        (((e.AddedItems[0]) as ListBoxItem).Content.ToString());
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
