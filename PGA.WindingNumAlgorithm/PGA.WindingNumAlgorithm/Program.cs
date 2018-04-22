using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using COMS = PGA.MessengerManager;

namespace PGA.WindingNumAlgorithm
{
    public class Program
    {
        [CommandMethod("PGA-Test-WN-Alg")]
        public static void  Test_WN_Algorithm()
        {
            if (TestPolys())
                COMS.MessengerManager.AddLog("Found Points is Inside");
            COMS.MessengerManager.AddLog("No Points Inside Polyline");


        }

        public static bool WN_Algorithm(Point2dCollection b, Point2dCollection t)
        {
            var ba = b.ToArray();

            foreach (Point2d p in t)
            {
                if (WNumAlgorithm.wn_PnPoly(p, ba, b.Count) != 0)
                {
                    return true;
                }    
            }
            return false;
        }

        public static bool TestPolysRuntime()
        {
            try
            {
                var polyoId = SelectionManager.GetAllPolylines();
                var ordered = SelectionManager.OrderPolylines(polyoId);

                foreach (Polyline p in ordered)
                {
                    
                    if (p == null) continue;

                    try
                    {
                        var bndypnts = GetPoints(p);
                        var num = bndypnts.Count;

                        foreach (Polyline t in ordered)
                        {
                            if (t.Layer == p.Layer &&
                                t.Area == p.Area)
                                continue;

                            var tstpnts = GetPoints(t);

                            foreach (Point2d tpnt in tstpnts)
                            {
                                if (WNumAlgorithm.wn_PnPoly(tpnt, bndypnts.ToArray(), num) != 0)
                                {
                                    return true;
                                }
                            }

                        }

                    }
                    catch (System.Exception ex)
                    {
                        COMS.MessengerManager.LogException(ex);
                    }
                }
            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return false;
        }


        public static bool TestPolys()
        {
            try
            {
                var polyoId = SelectionManager.GetAllPolylines();
                var ordered = SelectionManager.OrderPolylines(polyoId);

                foreach (Polyline p in ordered)
                {
                    //Identify Outer Boundary

                    if (p.Area < 55000)
                        continue;

                    if (p == null) continue;

                    try
                    {
                        var bndypnts = GetPoints(p);
                        var num = bndypnts.Count;

                        foreach (Polyline t in ordered)
                        {
                            if (t.Layer == p.Layer &&
                                t.Area == p.Area)
                                continue;

                            var tstpnts = GetPoints(t);

                            foreach (Point2d tpnt in tstpnts)
                            {
                                if (WNumAlgorithm.wn_PnPoly(tpnt, bndypnts.ToArray(), num) != 0)
                                {
                                    COMS.MessengerManager.AddLog("Point is Inside" + tpnt.ToString());
                                    AddCircle(tpnt);
                                    //return true;
                                }
                            }

                        }

                    }
                    catch (System.Exception ex)
                    {
                        COMS.MessengerManager.LogException(ex);
                    }
                }
            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return false;
        }

        public static void AddCircle (Point2d p)
        {
            using (DocumentLock lLock = Active.Document.LockDocument())
            {
                using (Transaction tr = Active.StartTransaction())
                {
                    var center = new Point3d(p.X, p.Y, 0);


                    Circle c = new Circle();
                    c.Center = center;
                    c.Radius = 2;
                    c.SetDatabaseDefaults();
                    c.Layer = "0";
                    Entity ent = c;

                    BBC.Common.AutoCAD.AcadDatabaseManager.AddToDatabase(Active.Database, ent, tr);

                    tr.Commit();
                }
            }
        }

        public static Point2dCollection GetPoints(Polyline poly)
        {
            return BBC.Common.AutoCAD.AcadUtilities.GetPointsFromPolyline(poly);
        }

    }
}
