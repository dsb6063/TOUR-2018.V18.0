#region

using System.ComponentModel.Design;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using PGA.Autodesk.Utils;
using PGA.Database;
using PGA_Tour_Civil_App;
using Exception = System.Exception;
using BBC.Common.AutoCAD;

#endregion

[assembly: ExtensionApplication(typeof(Program.Main))]
[assembly: CommandClass(typeof(Program.Main))]

namespace PGA_Tour_Civil_App
{
    public class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        public class Main : IExtensionApplication
        {
            public void Initialize()
            {
            }

            public void Terminate()
            {
            }
            //[CommandMethod("PGA-STARTCOALESCING", CommandFlags.Session)]
            //public void StartCoalescing()
            //{
            //    try
            //    {
            //        AcadUtilities.WriteMessage("Starting StartCoalescing \n\n");
            //        PGA.CourseCoalesceProject.Coalesce.LoadandProcessPolys();

            //    }
            //    catch (Exception ex)
            //    {
            //        AcadUtilities.WriteMessage(ex.Message);
            //        PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
            //    }
            //}

            [CommandMethod("PGA-CHECK")]
            public void CheckDrawing()
            {
                try
                {
                    AcadUtilities.WriteMessage("Starting Audit \n\n");

                    AcadUtilities.SendStringToExecute("_.AUDIT Y \n\n");


                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
                }
            }

            [CommandMethod("PGA-SETTINGS")]
            public void Settings()
            {
                try
                {
                    Application.SetSystemVariable("FILEDIA", 1);
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-IPTCLOUD")]
            public void ImportPointCloud()
            {
                try
                {
                    AcadUtilities.SendStringToExecute("_.AeccCreatePointCloud \n");
            
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-EXPORTTOAUTOCAD")]
            public void ExportToAutoCAD2()
            {
                try
                {
                    AcadUtilities.SendStringToExecute("_.EXPORTTOAUTOCAD \n\n");
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-OROISOLATE")]
            public void OROIsoPolyLines()
            {
                try
                {
                    ClosePolylines.OROIsoSelectedPolylines();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-TURNONALLLAYERS")]
            public void OnAllLayers()
            {
                try
                {
                    ClosePolylines.TurnOnAllLayers();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-TURNOFFSURFACESLAYERS")]
            public void TurnOffSurfaceLayers()
            {
                try
                {
                    ClosePolylines.TurnOffSurfaceLayers();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-TURNONSURFACESLAYERS")]
            public void TurnOnSurfaceLayers()
            {
                try
                {
                    ClosePolylines.TurnOnSurfaceLayers();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-CLOSEPLINE",CommandFlags.UsePickSet)]
            public void ClosePolyLines()
            {
                try
                {
                    ClosePolylines.CloseSelectedPolylines();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-ASSIGNPLINE", CommandFlags.UsePickSet)]
            public void AssignPolyLinesLayerColors()
            {
                try
                {
                    PLineToLayers.SelectPolylinesToChange();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-SAVE")]
            public void SaveToACAD2007Format()
            {
                try
                {
                    PLineToLayers.SaveToAcad2007();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            public void AssignPolyLinesToLayers()
            {
                try
                {
                    PLineToLayers.SelectPolylines();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
                }
            }

            [CommandMethod("PGA-LOADTPLATE", CommandFlags.Session)]
            public void LoadTemplate()
            {
                try
                {
                    using (PGA.Database.DatabaseCommands commands = new DatabaseCommands())
                    {
                        PGA.Autodesk.Utils.LoadTemplate.LoadPGATemplate(commands.GetTemplatePath());

                    }
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }


            [CommandMethod("PGA-LOADINSERT", CommandFlags.Session)]
            public void LoadByComandInsertTemplate()
            {
                try
                {
                    using (PGA.Database.DatabaseCommands commands = new DatabaseCommands())
                    {
                        PGA.Autodesk.Utils.LoadTemplate.LoadPGATemplate(commands.GetTemplatePath());

                    }
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-CHECKINTERSECTIONS", CommandFlags.Session)]
            public void GetIntersections()
            {
                try
                {
                    PolylineUtilities.GetIntersections();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-CyclomaticComplexity", CommandFlags.Session)]
            public void GetComplexity()
            {
                try
                {
                    PolylineUtilities.GetComplexity();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }

            [CommandMethod("PGA-EXPORTPLINES", CommandFlags.Session | CommandFlags.Modal | CommandFlags.UsePickSet)]
            public void ExportPolylinesToCSV()
            {
                try
                {
                    PolylineUtilities.ExportPolyPointsToCSV();
                }
                catch (Exception ex)
                {
                    AcadUtilities.WriteMessage(ex.Message);
                    PGA.MessengerManager.MessengerManager.AddLog(ex.Message);

                }
            }
        }
    }
}