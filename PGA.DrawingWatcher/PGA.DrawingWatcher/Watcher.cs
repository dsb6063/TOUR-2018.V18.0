using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using PGA.Database;
using PGA.DrawingManager;
using COMS=PGA.MessengerManager;
using ACADRT=Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace PGA.DrawingWatcher
{
    public static class Watcher
    {
        public static TimerClass TM_Watcher  = null;

        [ACADRT.CommandMethod("PGA-StartWatcher", ACADRT.CommandFlags.Session)]
        public static void StartWatcher()
        {
            try
            {
                TM_Watcher  = new TimerClass();

                COMS.MessengerManager.AddLog("Started Watcher!");
            }
            catch (Exception ex2)
            {
                COMS.MessengerManager.LogException(ex2);
            }
        }

        [ACADRT.CommandMethod("PGA-StoptWatcher", ACADRT.CommandFlags.Session)]
        public static void StoptWatcher()
        {
            try
            {
                TimerClass.WrapUpOperations();
                TM_Watcher = null;
            }
            catch (Exception ex2)
            {
                COMS.MessengerManager.LogException(ex2);
            }
        }

    }
    public class TimerClass
    {
        public static System.Windows.Forms.Timer myTimer  = new System.Windows.Forms.Timer();

        private static int MaxDWGs   = 0;
        private static int MaxDXFs   = 0;
        private static int TotalDWGs = 0;

        private static Sv.PostAudit.PostAudit _postAudit 
            = new Sv.PostAudit.PostAudit();

        // This is the method to run when the timer is raised.
        private static void TimerEventProcessor(Object sender,
            EventArgs myEventArgs)
        {
            try
            {
                

                myTimer.Enabled = false;
                PGA.DrawingManager.Commands.ForceCloseAllButActiveDocuments();
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    if (TotalDWGs ==0)  
                        TotalDWGs = commands.DWGCountExportToCAD();
                    if (TotalDWGs == 0)
                        return;

                    if (commands.AnyFilesLocked())
                    {
                        COMS.MessengerManager.AddLog("Exiting Tm_Elasped.File Locked!");
                        COMS.MessengerManager.AddLog("Closing Drawings: File Locked!");
                        WrapUpOperations();
                    }
                    if (commands.AllCompletedFlagsExportToCad())
                    {

                      //  _postAudit.ProcessFiles();

                        COMS.MessengerManager.AddLog("Starting Post Audit!");

                        COMS.MessengerManager.AddLog("Starting Tm_Elasped.WrapUPOperations!");
                        commands.SendCompleteFlagToSettings();
                        commands.DeleteAllExportToCadRecords();
                        WrapUpOperations(tm: (System.Windows.Forms.Timer) sender);

                    }
                    else if ((!commands.IsDXFStartedEportToCad() || commands.HasMoreDWGsExportToCAD()) && MaxDWGs < TotalDWGs)
                    {   // Create DWGs
                        commands.InsertNotifications(String.Format("{0},{1},{2}",2,++MaxDWGs,TotalDWGs));
                        COMS.MessengerManager.AddLog("Starting Tm_Elasped.AsyncExportDWGs!");
                        PGA.ExportToAutoCAD.ExportToCad.AsyncExportDWGs();
                        commands.SetS2TimerInfo(MaxDWGs);
                    }
                    else if ((commands.HasMoreDXFsExportToCAD()) && MaxDXFs < TotalDWGs)
                    {   // Crete DXFs
                        commands.InsertNotifications(String.Format("{0},{1},{2}", 3, ++MaxDXFs, TotalDWGs));
                        COMS.MessengerManager.AddLog("Starting Tm_Elasped..Create DXF!");
                        PGA.ExportToAutoCAD.ExportToCad.AsyncExportDXFs();
                        commands.SetS3TimerInfo(MaxDXFs);
                    }
                }
           
                COMS.MessengerManager.AddLog("Ending Tm_Elasped...");


                myTimer.Enabled = true;
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
                    MaxDWGs   = 0;
                    MaxDXFs   = 0;
                    myTimer.Stop();
                    myTimer.Enabled = false;
                    myTimer.Tick -= TimerEventProcessor;
                    Application.SetSystemVariable("FILEDIA", 1);
                    COMS.MessengerManager.AddLog("Stopped Watcher!");

                    Application.SetSystemVariable("PROXYNOTICE", 1);
                    Application.SetSystemVariable("PROXYGRAPHICS", 1);
                    Application.SetSystemVariable("FILEDIA", 1);

                    OpenandCloseDwgs.CloseDocuments();

                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        public static void WrapUpOperations(Timer tm)
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
                Application.SetSystemVariable("FILEDIA", 1);
                COMS.MessengerManager.AddLog("Stopped Watcher!");

                Application.SetSystemVariable("PROXYNOTICE", 1);
                Application.SetSystemVariable("PROXYGRAPHICS", 1);
                Application.SetSystemVariable("FILEDIA", 1);

                COMS.MessengerManager.AddLog("Set Exit Variables!");

                OpenandCloseDwgs.CloseDocuments();


            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }
        


        public TimerClass()
        {
            try
            {
                // Sets the timer interval 1000 msec * factor = seconds.

                myTimer.Tick += new EventHandler(TimerEventProcessor);
                myTimer.Enabled = true;
                myTimer.Interval = 1000 * 5; //90
                myTimer.Start();
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

        }

 
    }

}
