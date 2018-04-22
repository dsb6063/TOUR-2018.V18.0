using PGA.Database;
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

namespace PGA.ApplicationProgress
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : UserControl
    {

        static int alarmCounter = 1;
        static bool exitFlag = false;

        //public static System.Windows.Forms.Timer myTimer = 
        //    new System.Windows.Forms.Timer();

        public static ApplicationProgress.BackgroundWorker worker = 
            new PGA.ApplicationProgress.BackgroundWorker();
          
        
        public ProgressBar()
        {
            InitializeComponent();
            Main();
        }

        public int Main()
        {

            try
            {
                /* Adds the event and the event handler for the method that will 
                     process the timer event to the timer. */
                worker.myTimer.Tick += new EventHandler(TimerEventProcessor);

                // Sets the timer interval to 5 seconds.
                worker.myTimer.Interval = 5000;
                worker.myTimer.Start();

                // Runs the timer, and raises the event.
                if (exitFlag == false)
                {
                    // Processes all the events in the queue.
                    using (DatabaseCommands commands = new DatabaseCommands())
                    {
                        var results = commands.GetTimerInfo().FirstOrDefault();
                        worker.stageone = Convert.ToInt16(results.S1Percentage);
                        worker.stagetwo = Convert.ToInt16(results.S2Percentage);
                        worker.stagethree = Convert.ToInt16(results.S3Percentage);
                        worker.total = Convert.ToInt16(results.TotalDwgs);
                    }
                }
            }
            catch (Exception)
            {

            }
            return 0;
        }


        private  void TimerEventProcessor(Object myObject,
                                               EventArgs myEventArgs)
        {
            worker.myTimer.Stop();

            // Displays a message box asking whether to continue running the timer.
             if (!exitFlag)
            {
                // Restarts the timer and increments the counter.
                alarmCounter += 1;
                worker.myTimer.Enabled = true;
                GetProgressBarValues();
                SetProgressBarValues();
            }
            else
            {
                // Stops the timer.
                exitFlag = true;
            }
        }

         
        public  void SetProgressBarValues()
        {
            try
            {
                pbStage1.Value = Math.Ceiling(100.0 * worker.stageone   / worker.total);
                pbStage2.Value = Math.Ceiling(100.0 * worker.stagetwo   / (2 * worker.total));
                pbStage3.Value = Math.Ceiling(100.0 * worker.stagethree / (2 * worker.total));
            }
            catch (Exception)
            {
                pbStage1.Value = 0;
                pbStage2.Value = 0;
                pbStage3.Value = 0;

            }
        }

        public  void GetProgressBarValues()
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var results = commands.GetTimerInfo().FirstOrDefault();
                    worker.stageone = Convert.ToInt16(results.S1Percentage);
                    worker.stagetwo = Convert.ToInt16(results.S2Percentage);
                    worker.stagethree = Convert.ToInt16(results.S3Percentage);
                    worker.total = Convert.ToInt16(results.TotalDwgs);
                }
            }
            catch (Exception)
            {

            }
        }

    }
}
