using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PGA.DataContext;

namespace PGA.DatabaseManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

    
    protected override void OnStartup(StartupEventArgs e)
        {
            //on start stuff here
            base.OnStartup(e);
            //or here, where you find it more appropriate
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //on start stuff here
           // PGA.DataContext.Settings GSettings = new PGA.DataContext.Settings();
        }

        //public Settings MySettings
        //{
        //    get {return GSettings;}
        //    set { GSettings = value ;}
        //}
     
    }
}
