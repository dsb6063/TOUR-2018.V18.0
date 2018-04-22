using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CircularGauge;
using PGA.Database;

namespace CircularGaugeDemoWPF
{
    /// <summary>
    /// Interaction logic for GuageUC.xaml
    /// </summary>
    public partial class GuageUC : UserControl
    {
        public static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public GuageUC()
        {
            InitializeComponent();
            SetEvent();
            InitializeTimer();
        }

        public void InitializeTimer()
        {
            myTimer.Enabled = true;
            myTimer.Interval = 3000;
            myTimer.Start();
        }

        public void SetEvent()
        {
            myTimer.Tick += MyTimer_Tick;
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                myTimer.Stop();
                UpdateGaugeValue();
                myTimer.Start();

            }
            catch (Exception)
            {

            }
        }

        public void UpdateGaugeValue()
        {
            try
            {
                using (DatabaseCommands cmd = new DatabaseCommands())
                {
                    var info = cmd.GetTimerInfo().FirstOrDefault();
                    double s1, s2, s3, totaldwgs;

                    totaldwgs = info.TotalDwgs ?? 0;

                    if (totaldwgs == 0)
                    {
                        myGauge4.CurrentValue = 0;
                        myGauge4.DialText = "0%";
                    }
                    else
                    {
                        if (info.S1Percentage == null)
                            s1 = 0;
                        else
                            s1 = (double) info.S1Percentage;

                        if (info.S2Percentage == null)
                            s2 = 0;
                        else
                            s2 = ( (double) info.S2Percentage) * 1/2 ;

                        if (info.S3Percentage == null)
                            s3 = 0;
                        else
                            s3 = ( (double) info.S3Percentage) * 1/2 ;

                        myGauge4.CurrentValue = Math.Ceiling((100/(3*totaldwgs))*(s1 + s2 + s3));
                        if (Convert.ToInt16(myGauge4.CurrentValue).Equals(0))
                        {
                            myGauge4.DialText = "0%";
                        }

                        myGauge4.DialText = String.Format("{0}%", myGauge4.CurrentValue.ToString());
                    }
                }
            }
            catch (Exception)
            {
                myGauge4.DialText = "0%";
            }
        }
    }
}
