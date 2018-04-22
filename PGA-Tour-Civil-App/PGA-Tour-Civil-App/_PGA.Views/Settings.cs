#region

using System.Windows.Forms;
using PGA.Controller;

#endregion

namespace PGA.Views
{
    public partial class Settings : Form, ISettingsController
    {
        public Settings()
        {
            InitializeComponent();
        }
    }
}