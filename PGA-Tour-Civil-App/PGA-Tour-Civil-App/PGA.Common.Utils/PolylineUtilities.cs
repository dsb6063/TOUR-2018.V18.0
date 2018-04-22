#region

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AcadRuntime = Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

#endregion

namespace PGA.Autodesk.Utils
{
    public class PolylineUtilities

    {
        //[CommandMethod("PolylineToCSV", "plcsv", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void ExportPolyPointsToCSV()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;

            var db = doc.Database;

            var ed = doc.Editor;

            try
            {
                using (Transaction tr = db.TransactionManager.StartOpenCloseTransaction())
                {
                    var dec = ((short) Application.GetSystemVariable("LUPREC")).ToString();

                    var pso = new PromptSelectionOptions();

                    pso.MessageForRemoval = "\n >>  Nothing selected....";

                    pso.MessageForAdding = "\n  >>  Select a single polyline >> ";

                    pso.AllowDuplicates = false;

                    pso.SingleOnly = true;

                    var sf = new SelectionFilter
                        (new[] {new TypedValue(0, "lwpolyline")});

                    var res = ed.GetSelection(pso, sf);

                    if (res.Status != PromptStatus.OK) return;

                    var sb = new StringBuilder();
                    var ids = res.Value.GetObjectIds();

                    var poly = (Polyline) tr.GetObject(ids[0], OpenMode.ForRead, false);
                    for (var i = 0; i < poly.NumberOfVertices; i++)
                    {
                        var pt = poly.GetPoint2dAt(i);
                        var vexstr = string.Format("{0:f" + dec + "};{1:f" + dec + "}", pt.X, pt.Y);
                        sb.AppendLine(vexstr);
                    }
                    var csvFile = string.Empty;

                    var sfd = new SaveFileDialog();
                    sfd.ValidateNames = true;
                    sfd.Title = "Save polyline vertices to CSV file";
                    sfd.DefaultExt = ".csv";
                    sfd.InitialDirectory = @"C:\PGA\";
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    csvFile = sfd.FileName;

                    // write point to defined file
                    using (var sw = new StreamWriter(csvFile))
                    {
                        sw.Write(sb.ToString());

                        sw.Flush();
                    }
                    sfd.Dispose(); // non resident object, so kill 'em

                    tr.Commit();
                }
            }
            catch (AcadRuntime.Exception ex)
            {
                ed.WriteMessage("\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }


        //[CommandMethod("gi")]
        public static void GetIntersections()
        {
            var db = HostApplicationServices.WorkingDatabase;

            var doc = Application.DocumentManager.MdiActiveDocument;

            var ed = doc.Editor;

            using (var docLock = doc.LockDocument())
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var btr = (BlockTableRecord) tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                        var peo = new PromptEntityOptions("\nSelect a single polyline  >>");

                        peo.SetRejectMessage("\nSelected object might be of type polyline only >>");

                        peo.AddAllowedClass(typeof(Polyline), false);

                        PromptEntityResult res;

                        res = ed.GetEntity(peo);

                        if (res.Status != PromptStatus.OK)

                            return;

                        var ent = tr.GetObject(res.ObjectId, OpenMode.ForRead);

                        if (ent == null) return;

                        //Polyline poly = (Polyline)ent as Polyline;
                        var curv = ent as Curve;

                        var pcurves = new DBObjectCollection();

                        curv.Explode(pcurves);
                        TypedValue[] values =
                        {
                            new TypedValue(0, "lwpolyline")
                            //might be added layer name to select curve:
                            //, new TypedValue(8, "mylayer")
                        };
                        var filter = new SelectionFilter(values);

                        var fence = new Point3dCollection();

                        var leng = curv.GetDistanceAtParameter(curv.EndParam) -
                                   curv.GetDistanceAtParameter(curv.StartParam);
                        // number of divisions along polyline to create fence selection
                        var step = leng/256; // set number of steps to your suit

                        var num = Convert.ToInt32(leng/step);

                        var i = 0;

                        for (i = 0; i < num; i++)
                        {
                            var pp = curv.GetPointAtDist(step*i);

                            fence.Add(curv.GetClosestPointTo(pp, false));
                        }

                        var selres = ed.SelectFence(fence, filter);

                        if (selres.Status != PromptStatus.OK) return;
                        var intpts = new Point3dCollection();

                        var qcurves = new DBObjectCollection();

                        foreach (SelectedObject selobj in selres.Value)
                        {
                            var obj = tr.GetObject(selobj.ObjectId, OpenMode.ForRead, false);
                            if (selobj.ObjectId != curv.ObjectId)
                            {
                                var icurves = new DBObjectCollection();
                                var icurv = obj as Curve;
                                icurv.Explode(icurves);
                                foreach (DBObject dbo in icurves)
                                {
                                    if (!qcurves.Contains(dbo))
                                        qcurves.Add(dbo);
                                }
                            }
                        }
                        ed.WriteMessage("\n{0}", qcurves.Count);


                        var j = 0;
                        var polypts = new Point3dCollection();

                        for (i = 0; i < pcurves.Count; ++i)
                        {
                            for (j = 0; j < qcurves.Count; ++j)
                            {
                                var curve1 = pcurves[i] as Curve;

                                var curve2 = qcurves[j] as Curve;

                                var pts = new Point3dCollection();

                                curve1.IntersectWith(curve2, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                                foreach (Point3d pt in pts)
                                {
                                    if (!polypts.Contains(pt))
                                        polypts.Add(pt);
                                }
                            }
                        }

                        Application.SetSystemVariable("osmode", 0); // optional
                        // for debug only
                        Application.ShowAlertDialog(string.Format("\nNumber of Intersections: {0}", polypts.Count));
                        // test for visulization only
                        foreach (Point3d inspt in polypts)
                        {
                            var circ = new Circle(inspt, Vector3d.ZAxis, 10*db.Dimtxt);
                            circ.ColorIndex = 1;
                            btr.AppendEntity(circ);
                            tr.AddNewlyCreatedDBObject(circ, true);
                        }
                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        ed.WriteMessage("\n{0}\n{1}", ex.Message, ex.StackTrace);
                    }
                }
            }
        }

        // based on article by Wayne Brill
        // http://adndevblog.typepad.com/autocad/2012/06/use-getclosestpointto-and-onsegat-to-locate-a-polyline-vertex-near-a-point.html
        //[CommandMethod("oseg")]
        public static void TEST_PointOnPoly()
        {
            var db = Application.DocumentManager.MdiActiveDocument.Database;

            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            var cs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            var plan = new Plane(Point3d.Origin, cs.Zaxis);

            Application.SetSystemVariable("osmode", 512);

            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    Entity ename;

                    Point3d pt;

                    Point3d ptWcs;

                    var peo = new PromptEntityOptions("\nSelect Pline: ");

                    peo.SetRejectMessage("\nYou have to select LWPOLYLINE!");

                    peo.AddAllowedClass(typeof(Polyline), false);

                    var res = ed.GetEntity(peo);

                    if (res.Status != PromptStatus.OK) return;

                    var id = res.ObjectId;

                    // Convert to WCS incase selection was made 

                    // while in a UCS.

                    pt = res.PickedPoint;

                    // Transform from UCS to WCS

                    var mat =
                        Matrix3d.AlignCoordinateSystem(
                            Point3d.Origin,
                            Vector3d.XAxis,
                            Vector3d.YAxis,
                            Vector3d.ZAxis,
                            cs.Origin,
                            cs.Xaxis,
                            cs.Yaxis,
                            cs.Zaxis
                        );

                    ptWcs = pt.TransformBy(mat);

                    ename = tr.GetObject(id, OpenMode.ForRead) as Entity;

                    var pline = ename as Polyline;

                    if (pline == null)
                    {
                        ed.WriteMessage("\nSelected Entity is not a Polyline");

                        return;
                    }

                    var clickpt = pline.GetClosestPointTo(ptWcs, false);

                    for (var c = 0; c < pline.NumberOfVertices; c++)
                    {
                        var segParam = new double();

                        // This is the test filter here...it uses the 

                        // nifty API OnSegmentAt

                        if (pline.OnSegmentAt(c, clickpt.Convert2d(plan), segParam))
                        {
                            ed.WriteMessage("\nSelected Segment: {0} \n", c + 1);

                            break;
                        }
                    }
                }

                catch (Exception ex)
                {
                    ed.WriteMessage("\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        // [CommandMethod("wblockclone", CommandFlags.Session)]
        //static public void wblockclone()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    Editor ed = doc.Editor;

        //    //PromptEntityOptions peo = new PromptEntityOptions("\nSelect entity to copy: ");

        //    //PromptEntityResult per = ed.GetEntity(peo);

        //    //if (per.Status != PromptStatus.OK)
        //    //    return;
        //    Application.
        //    using (DocumentLock doclock = doc.LockDocument())
        //    {
        //        ObjectIdCollection ObjectcIds = new ObjectIdCollection();
        //        ObjectcIds.Add(per.ObjectId);

        //        Database destDb = new Database(false, true);
        //        destDb.ReadDwgFile(@"C:\Users\Daryl Banks\Downloads\doraltest\2014Doral\Doral-Closed-Polygons\Doral06.DWG", System.IO.FileShare.Read, true, "");

        //        using (Transaction Tx = destDb.TransactionManager.StartTransaction())
        //        {
        //            BlockTable bt = Tx.GetObject(
        //                destDb.BlockTableId,
        //                OpenMode.ForRead) as BlockTable;

        //            BlockTableRecord btr = Tx.GetObject(
        //                bt[BlockTableRecord.ModelSpace],
        //                OpenMode.ForWrite) as BlockTableRecord;

        //            IdMapping oIdMap = new IdMapping();

        //            destDb.WblockCloneObjects(
        //                ObjectcIds,
        //                btr.ObjectId,
        //                oIdMap,
        //                DuplicateRecordCloning.Ignore,
        //                false);

        //            Tx.Commit();
        //        }

        //        destDb.SaveAs(destDb.Filename, DwgVersion.Current);
        //    }
        //}

        public static void GetComplexity()
        {
            var db = HostApplicationServices.WorkingDatabase;

            var doc = Application.DocumentManager.MdiActiveDocument;

            var ed = doc.Editor;

            using (doc.LockDocument())
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var btr = (BlockTableRecord) tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                        var peo = new PromptEntityOptions("\nSelect a single polyline  >>");

                        peo.SetRejectMessage("\nSelected object might be of type polyline only >>");

                        peo.AddAllowedClass(typeof(Polyline), false);

                        PromptEntityResult res;

                        res = ed.GetEntity(peo);

                        if (res.Status != PromptStatus.OK)

                            return;

                        var ent = tr.GetObject(res.ObjectId, OpenMode.ForRead);

                        if (ent == null) return;

                        //Polyline poly = (Polyline)ent as Polyline;
                        var curv = ent as Curve;

                        var pcurves = new DBObjectCollection();

                        if (curv != null)
                        {
                            curv.Explode(pcurves);
                            TypedValue[] values =
                            {
                                new TypedValue(0, "lwpolyline")
                                //might be added layer name to select curve:
                                //, new TypedValue(8, "mylayer")
                            };
                            var filter = new SelectionFilter(values);

                            var fence = new Point3dCollection();

                            var leng = curv.GetDistanceAtParameter(curv.EndParam) -
                                       curv.GetDistanceAtParameter(curv.StartParam);
                            // number of divisions along polyline to create fence selection
                            //double step = leng / 256;// set number of steps to your suit
                            var step = leng/pcurves.Count;
                            Application.SetSystemVariable("osmode", 0); // optional
                            // for debug only
                            Application.ShowAlertDialog(string.Format("\nNumber of Steps: {0}", step));
                        }

                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        ed.WriteMessage("\n{0}\n{1}", ex.Message, ex.StackTrace);
                    }
                }
            }
        }

        public bool ExcludePolyBasedOnComplexity(ObjectId oid)
        {
            var db = HostApplicationServices.WorkingDatabase;

            var doc = Application.DocumentManager.MdiActiveDocument;

            var ed = doc.Editor;

            using (doc.LockDocument())
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var ent = tr.GetObject(oid, OpenMode.ForRead);

                        if (ent == null) return false;

                        //Polyline poly = (Polyline)ent as Polyline;
                        var curv = ent as Curve;

                        var pcurves = new DBObjectCollection();

                        if (curv != null)
                        {
                            curv.Explode(pcurves);
                            TypedValue[] values =
                            {
                                new TypedValue(0, "lwpolyline")
                                //might be added layer name to select curve:
                                //, new TypedValue(8, "mylayer")
                            };
                            var filter = new SelectionFilter(values);

                            var fence = new Point3dCollection();

                            var leng = curv.GetDistanceAtParameter(curv.EndParam) -
                                       curv.GetDistanceAtParameter(curv.StartParam);
                            // number of divisions along polyline to create fence selection
                            //double step = leng / 256;// set number of steps to your suit
                            var step = leng/pcurves.Count;
                        }
                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        ed.WriteMessage("\n{0}\n{1}", ex.Message, ex.StackTrace);
                    }

                    return false;
                }
            }
        }
    }
}