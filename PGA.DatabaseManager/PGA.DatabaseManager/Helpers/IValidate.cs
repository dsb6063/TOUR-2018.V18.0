using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGA.DatabaseManager
{
    public interface IValidate
    {
        void SetRedBorderOnEmpties(MainWindow view);
        void ClearRedBorderOnEmpties(MainWindow view);
    }


    public class Validate : IValidate
    {
        public void SetRedBorderOnEmpties(MainWindow mainWindow)
        {
            var controls = AllTextBoxes(mainWindow);
            SetRedBorder(controls);
        }

        private List<TextBox> AllTextBoxes(MainWindow mainWindow)
        {
            var txtboxCollection = new List<TextBox>();
            var cntls = mainWindow.GetControls();

            foreach (var c in cntls)
            {
                if(c.GetType().Equals(typeof(TextBox)))
                    txtboxCollection.Add((TextBox) c);
            }
            return (List<TextBox>) txtboxCollection;
        }

        private void SetRedBorder(List<TextBox> _textBoxes)
        {
            foreach (var item in _textBoxes)
            {
                if (item.Text == "" && 
                    item.Name !="txtStartTime" &&
                    item.Name != "txtEndTime"  &&
                    item.Name != "txtElapsedTime")

                item.BorderBrush = System.Windows.Media.Brushes.Red;
            }
        }

        public void ClearRedBorderOnEmpties(MainWindow mainWindow)
        {
            var controls = AllTextBoxes(mainWindow);
            ClearRedBorder(controls);
        }
        public void ClearRedBorder(List<TextBox> _textBoxes)
        {
            foreach (var item in _textBoxes)
            {
                item.BorderBrush = System.Windows.Media.Brushes.DarkGray;
            }
        }
    }

    public static class Extentions
    {
        
        public static IList<Control> GetControls(this DependencyObject parent)
        {
            var result = new List<Control>();
            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(parent); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, x);
                var instance = child as Control;

                if (null != instance)
                    result.Add(instance);

                result.AddRange(child.GetControls());
            }
            return result;
        }
    }
}