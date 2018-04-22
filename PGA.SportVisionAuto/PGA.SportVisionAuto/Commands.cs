using System;
using System.IO;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using PGA.Database;
using PGA.DrawingManager;
using COMS = PGA.MessengerManager;
using ACADRT = Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace PGA.SportVisionAuto
{
    public class Commands
    {
        public static TimerClass TM_Watcher = null;

        [ACADRT.CommandMethod("PGA-StartSportV", ACADRT.CommandFlags.Session)]
        public static void StartWatcher()
        {
            TM_Watcher = new TimerClass();
            COMS.MessengerManager.AddLog("Started Sport Vision Watcher!");
        }

        [ACADRT.CommandMethod("PGA-StopSportV", ACADRT.CommandFlags.Session)]
        public static void StoptWatcher()
        {
            COMS.MessengerManager.AddLog("Stopped Sport Vision Watcher!");
            TimerClass.WrapUpOperations();
            TM_Watcher = null;
        }
        [ACADRT.CommandMethod("PGA-SV-Timer-Disable", ACADRT.CommandFlags.Session)]
        public static void DisableTimer()
        {
            TimerClass.myTimer.Enabled = false;

        }
        [ACADRT.CommandMethod("PGA-SV-Timer-Enable", ACADRT.CommandFlags.Session)]
        public static void EnableTimer()
        {
            TimerClass.myTimer.Enabled = true;
        }
    }

    public class TimerClass
    {
        public static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        static int alarmCounter = 0;
        static bool exitFlag = false;
        private static int MaxDWGs = 0;
        private static int MaxDXFs = 0;
        private static int TotalDWGs = 0;


        public void UnlockCommands(Document doc)
        {

            // Add our command handlers

            //doc.CommandEnded += OnCommandEnded;
            //doc.CommandCancelled += OnCommandEnded;
            //doc.CommandFailed += OnCommandEnded;
            doc.BeginDocumentClose += Doc_BeginDocumentClose;
        }

        private void Doc_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            try
            {
                using (DocumentLock lLock = Active.Document.LockDocument())
                {
                    using (var db = Active.Database)
                    {
                        var dir = Path.GetDirectoryName(db.Filename);
                        var path = Path.Combine(dir, "SV");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        var file = Path.GetFileName(db.Filename);

                        path = Path.Combine(path, "SV-" + file);
                        if (!File.Exists(path))
                            db.SaveAs(path, DwgVersion.Current);
                    }
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:CommandEnded" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
        private void OnCommandEnded(object sender, CommandEventArgs e)
        {
            try
            {
                #region Comments
                //var doc = (Document)sender;
                //if (doc.CommandInProgress == "DeleteDuplicates")
                //{
                //    if (stopwatch.Elapsed > MaxWait)
                //    Counter++;
                //    doc.Editor.Command("PGA-StartSportVisionSynch");
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            UnlockCommands(Active.Document);
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        public void Terminate()
        {
            ResetVars();
        }

        private static void TimerEventProcessor(Object sender,
            EventArgs myEventArgs)
        {
            try
            {


                PGA.DrawingManager.Commands.ForceCloseAllButActiveDocuments();


                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    if (TotalDWGs == 0)
                        TotalDWGs = commands.DWGCountGenCadStk();
                    myTimer.Enabled = false;
                    if (MaxDWGs++ < TotalDWGs)
                    {
                        myTimer.Interval = 15000;

                        commands.InsertNotifications(String.Format("{0},{1},{2}", 2, MaxDWGs, TotalDWGs));
                        COMS.MessengerManager.AddLog("Starting Drawing Processing!");

                        var name = commands.GUgetNextDwg();

                        PGA.SV30MOffset.Program.InvokeProgressBar(name,MaxDWGs,TotalDWGs);

                        if (!String.IsNullOrEmpty(name))
                        {
                            commands.GUMarkCompleted(name);

                            Application.SetSystemVariable("FILEDIA", 0);
                            Application.SetSystemVariable("NOMUTT", 0);
                            Application.SetSystemVariable("CMDECHO", 0);

                            Program.OpenAutoCAD(name);
                            return;
                        }
                        else
                        {
                            COMS.MessengerManager.ShowMessageAndLog("No Files to Process!");
                            return;
                        }

                    }
                    else
                    {
                        if (MaxDWGs > 3 + TotalDWGs)
                        {
                            PGA.SV30MOffset.Program.SaveDwg();
                            PGA.DrawingManager.Commands.CloseDocuments();
                            ResetVars();
                        }
                    }
                    myTimer.Enabled = true;

                }
                COMS.MessengerManager.AddLog("Ending Timer Elasped...");
            }
            catch (ACADRT.Exception ex2)
            {
            }
            catch (Exception ex2)
            {
            }

        }

        public static void AsyncSaveAsLastDwg(string name)
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            ed.Document.SendStringToExecute(".SaveAS\n", true, false, true);
            ed.Document.SendStringToExecute(
                "(command \"_.SAVEAS\" \"\" \"" + name + "\")",
                false,
                false,
                false
                );
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
                    Application.SetSystemVariable("FILEDIA", 1);
                    COMS.MessengerManager.AddLog("Stopped Watcher!");

                    Application.SetSystemVariable("PROXYNOTICE", 1);
                    Application.SetSystemVariable("PROXYGRAPHICS", 1);
                    Application.SetSystemVariable("FILEDIA", 1);

                    OpenandCloseDwgs.CloseDocuments();

                }
                catch (Exception ex)
                {
                    COMS.MessengerManager.LogException(ex);
                }
            }
        }

        //Constructor Starts Automatically
        public TimerClass()
        {
            try
            {
                /* Adds the event and the event handler for the method that will 
                   process the timer event to the timer. 
                   Sets the timer interval 1000 msec * factor = seconds.*/

                myTimer.Tick += new EventHandler(TimerEventProcessor);
                myTimer.Enabled = true;
                myTimer.Interval = 1000 * 5; 
                myTimer.Start();
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

        }

        public static void ResetVars()
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
                    COMS.MessengerManager.AddLog("Stopped Watcher!");

                    if (Active.Document != null)
                    {
                        Application.SetSystemVariable("FILEDIA", 1);
                        Application.SetSystemVariable("PROXYNOTICE", 1);
                        Application.SetSystemVariable("PROXYGRAPHICS", 1);
                        Application.SetSystemVariable("FILEDIA", 1);

                    }
                }
            }
            catch (Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

    }

}
