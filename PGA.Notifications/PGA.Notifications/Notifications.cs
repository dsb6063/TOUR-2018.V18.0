using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.AcInfoCenterConn;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using Autodesk.Internal.InfoCenter;
using PGA.Database;
using COMS=PGA.MessengerManager;
using ACADDOC=Autodesk.AutoCAD.ApplicationServices;
using ACADRT=Autodesk.AutoCAD.Runtime;
using ACADDB=Autodesk.AutoCAD.DatabaseServices;
using Application = System.Windows.Forms.Application;

namespace PGA.Notifications
{
    public static class Notifications
    {

        public static TimerClass TM_NoteWatcher = null;

        [ACADRT.CommandMethod("PGA-StartNotes", ACADRT.CommandFlags.Session)]
        public static void StartWatcher()
        {
            TM_NoteWatcher = new TimerClass();
            COMS.MessengerManager.AddLog("Started Notifier!");
        }

        [ACADRT.CommandMethod("PGA-StopNotes", ACADRT.CommandFlags.Session)]
        public static void StoptWatcher()
        {
            TimerClass.WrapUpOperations();
            TM_NoteWatcher = null;
        }

    }

    public class TimerClass
    {

        private static int alarmCounter = 0;
        private static bool exitFlag = false;
        private static int MaxDWGs = 0;
        private static int MaxDXFs = 0;
        private static int TotalDWGs = 0;

        public static System.Windows.Forms.Timer myTimer =
            new System.Windows.Forms.Timer();

        private static void TimerEventProcessor(Object sender,
            EventArgs myEventArgs)
        {
            try
            {

                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var notes = commands.GetNotifications();
                    if (notes != null)
                    {
                        var result = notes.Command.Split(Convert.ToChar(","));

                        infoCenterBalloon(Convert.ToInt32(result[0]), result[1], result[2]);
                    }
                }
                //COMS.MessengerManager.AddLog("Ending Notifications...");
            }
            catch (ACADRT.Exception ex2)
            {
                COMS.MessengerManager.LogException(ex2);

            }
            catch (Exception ex2)
            {
                COMS.MessengerManager.LogException(ex2);
            }

        }

        public static void WrapUpOperations()
        {
            try
            {
                using (DatabaseCommands cmd = new DatabaseCommands())
                {
                    TotalDWGs = 0;
                    MaxDWGs = 0;
                    MaxDXFs = 0;
                    myTimer.Stop();
                    myTimer.Enabled = false;
                    myTimer.Tick -= TimerEventProcessor;
                    COMS.MessengerManager.AddLog("Stopped Notifications!");
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        public static void WrapUpOperations(System.Windows.Forms.Timer tm)
        {
            using (DatabaseCommands cmd = new DatabaseCommands())
            {
                try
                {
                    TotalDWGs = 0;
                    MaxDWGs = 0;
                    MaxDXFs = 0;
                    tm.Stop();
                    myTimer.Stop();
                    myTimer.Enabled = false;
                    myTimer.Tick -= TimerEventProcessor;
                    COMS.MessengerManager.AddLog("Stopped Notifications!");
                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
            }
        }

        public TimerClass()
        {
            try
            {
                myTimer.Tick += new EventHandler(TimerEventProcessor);
                myTimer.Enabled = true;
                myTimer.Interval = 1000*3;
                myTimer.Start();
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

        }

        private static void ShowBalloon(string title, string body)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;

            if (title != null)
            {
                notifyIcon.BalloonTipTitle = title;
            }

            if (body != null)
            {
                notifyIcon.BalloonTipText = body;
            }
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.ShowBalloonTip(3000);
            notifyIcon.BalloonTipClosed += NotifyIcon_BalloonTipClosed;
        }

        private static void NotifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            try
            {
                var bubble = (NotifyIcon)sender;
                if (bubble == null)
                return;

                bubble.Visible = false;
                bubble.Dispose();
            }
            catch (Exception)
            {

                
            }
        }

        public static void infoCenterBalloon(int Level, string start, string finish)

        {

            InfoCenterManager icm = new InfoCenterManager();

            ResultItem ri = new ResultItem();
            try
            {

                switch (Level)
                {
                    case 1:

                        ri.Category = "Stage 1 of 3";

                        ri.Title = string.Format("Drawing {0} of {1}.", start, finish);

                        ri.IsFavorite = true;

                        ri.IsNew = true;
                        break;
                    case 2:

                        ri.Category = "Stage 2 of 3";

                        ri.Title = string.Format("Drawing {0} of {1}.", start, finish);

                        ri.IsFavorite = true;

                        ri.IsNew = true;
                        break;

                    case 3:

                        ri.Category = "Stage 3 of 3";

                        ri.Title = string.Format("Drawing {0} of {1}.", start, finish);

                        ri.IsFavorite = true;

                        ri.IsNew = true;
                        break;

                    case 4:

                        ri.Category = "File Watcher";

                        ri.Title = string.Format("Starting Watcher...");

                        ri.IsFavorite = true;

                        ri.IsNew = true;
                        break;
                    default:

                        ri.Category = "PGA Application Notification";

                        ri.Title = string.Format("Current Time: {0}", DateTime.Now);

                        ri.IsFavorite = true;

                        ri.IsNew = true;
                        break;

                }


                icm.PaletteManager.ShowBalloon(ri); //Show Upper Pallets Note
                ShowBalloon(ri.Category, ri.Title); //Show Windows Note
                //StatusBarNotifications.StatusBarBalloon(ri.Category, ri.Title); 
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

    }

    public static class
        StatusBarNotifications
    {
        public static void StatusBarBalloon(string category, string title)
        {
            const string appName =

                "PGA TOUR CivTinSurface Notification";

            Document doc =
                ACADDOC.Application.DocumentManager.MdiActiveDocument;

            var statusBar = doc.GetStatusBar();

            TrayItem ti = new TrayItem();

            ti.ToolTipText = appName;

            ti.Icon =
               
                statusBar.TrayItems[0].Icon;

            statusBar.TrayItems.Add(ti);

            TrayItemBubbleWindow bw =

                new TrayItemBubbleWindow();

            bw.Title = title;

            //bw.HyperText = htext;

            //bw.HyperLink = hlink;

            bw.Text = title;

            bw.Text2 = String.Format("Finishing {0}",category);

            bw.IconType = IconType.Information;

            ti.ShowBubbleWindow(bw);

            statusBar.Update();

            bw.Closed +=

                delegate(

                    object o,

                    TrayItemBubbleWindowClosedEventArgs args

                    )

                {

                    // Use a try-catch block, as an exception

                    // will occur when AutoCAD is closed with

                    // one of our bubbles open

                    try

                    {

                        statusBar.TrayItems.Remove(ti);

                        statusBar.Update();

                    }

                    catch

                    {
                    }

                };
        }
    }
}




