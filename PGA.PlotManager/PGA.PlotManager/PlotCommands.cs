using System;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.GraphicsInterface;
using global::Autodesk.AutoCAD.GraphicsSystem;
using global::Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Runtime.InteropServices;
using BBC.Common.AutoCAD;

namespace PGA.PlotManager
{
    public static class PlotCommands
    {
        [CommandMethod("PGA-OSC")]
        public static void OffscreenSnapshot()
        {
            try
            {
                SnapshotToFile(
                @"C:\Civil 3D Projects\Test.png", VisualStyleType.Wireframe2D
                );

            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
        }

        public static void CreateSphere()
        {
            try
            {
                Document doc =
             Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
                Editor ed = doc.Editor;

                Transaction tr =
                  doc.TransactionManager.StartTransaction();
                using (tr)
                {
                    BlockTable bt =
                      (BlockTable)tr.GetObject(
                        db.BlockTableId,
                        OpenMode.ForRead
                      );
                    BlockTableRecord btr =
                      (BlockTableRecord)tr.GetObject(
                        bt[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite
                      );
                    Solid3d sol = new Solid3d();
                    sol.CreateSphere(10.0);

                    const string matname =
                      "Sitework.Paving - Surfacing.Riverstone.Mortared";
                    DBDictionary matdict =
                      (DBDictionary)tr.GetObject(
                        db.MaterialDictionaryId,
                        OpenMode.ForRead
                      );
                    if (matdict.Contains(matname))
                    {
                        sol.Material = matname;
                    }
                    else
                    {
                        ed.WriteMessage(
                          "\nMaterial (" + matname + ") not found" +
                          " - sphere will be rendered without it.",
                          matname
                        );
                    }
                    btr.AppendEntity(sol);

                    tr.AddNewlyCreatedDBObject(sol, true);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
            }

            using (var doc = Active.Document.LockDocument())
            {

                AcadUtilities.ZoomExtents();
            }
        }


        public static void SnapShotFromPrintScreen(string path)
        {
            try
            {
                var image = ScreenCapture.CaptureActiveWindow();
                image.Save(path, ImageFormat.Jpeg);
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog(ex.Message);
            }
        }

        public static void SnapshotToFile(
          string filename,
          VisualStyleType vst
        )
        {
            Document doc =
              Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Manager gsm = doc.GraphicsManager;

            // Get some AutoCAD system variables
            int vpn =
              System.Convert.ToInt32(
                Application.GetSystemVariable("CVPORT")
              );

            using (View view = new View())
            {
                gsm.SetViewFromViewport(view, vpn);

                // Set the visual style to the one passed in
                view.VisualStyle = new VisualStyle(vst);
                Device dev =
                  gsm.CreateAutoCADOffScreenDevice(new GraphicsKernel());
                using (dev)
                {
                    dev.OnSize(new Size((int)gsm.DeviceIndependentDisplaySize.Height, (int)gsm.DeviceIndependentDisplaySize.Width));

                    // Set the render type and the background color
                    dev.DeviceRenderType = RendererType.Default;
                    dev.BackgroundColor = Color.White;
                    
                    // Add the view to the device and update it
                    dev.Add(view);
                    dev.Update();

                    using (Model model = gsm.CreateAutoCADModel(new GraphicsKernel()))
                    {
                        Transaction tr =
                          db.TransactionManager.StartTransaction();
                        using (tr)
                        {
                            // Add the modelspace to the view
                            // It's a container but also a drawable
                            BlockTable bt =
                              (BlockTable)tr.GetObject(
                                db.BlockTableId,
                                OpenMode.ForRead
                              );
                            BlockTableRecord btr =
                              (BlockTableRecord)tr.GetObject(
                                bt[BlockTableRecord.ModelSpace],
                                OpenMode.ForRead
                              );
                            view.Add(btr, model);
                            tr.Commit();
                        }

                        // Take the snapshot
                        Rectangle rect = new Rectangle
                                        (new System.Drawing.Point(
                                         (int)view.ViewportExtents.MinPoint.X,
                                         (int)view.ViewportExtents.MinPoint.Y),
                                         new Size((int)gsm.DeviceIndependentDisplaySize.Height,
                                                  (int)gsm.DeviceIndependentDisplaySize.Width)
                                         );
                        System.Threading.Thread.Sleep(1000);

                        using (Bitmap bitmap = view.GetSnapshot(rect))
                        {
                            System.Threading.Thread.Sleep(1000);

                            bitmap.Save(filename);
                            
                            // Clean up
                            view.EraseAll();
                            dev.Erase(view);
                        }

    
                    }
                }
            }
        }
    }

    public class ScreenCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static Bitmap CaptureDesktop()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
    }

}
