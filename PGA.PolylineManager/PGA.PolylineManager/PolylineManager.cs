using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using BBC.Common.AutoCAD;
using PGA.WindingNumAlgorithm;
#pragma warning disable CS0436 // Type conflicts with imported type
using static PGA.PolylineManager.Active;
#pragma warning restore CS0436 // Type conflicts with imported type
using AcadDb= Autodesk.AutoCAD.DatabaseServices;

using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace PGA.PolylineManager
{
    public class PolylineManager
    {
        public static ObjectIdCollection GetAllInternalPolyLines(ObjectId oid, ObjectIdCollection oids)
        {
            var objectIdCollection = new ObjectIdCollection();
            var num = 0.1;
            try
            {
                using (var workingDatabase = WorkingDatabase)
                {
                    foreach (ObjectId oid1 in oids)
                    {
                        Polyline polyFromObjId1;
                        Polyline polyFromObjId2;
                        try
                        {
                            if (
                                !(GetPolyFromObjId(oid1) == null))
                            {
                                polyFromObjId1 = GetPolyFromObjId(oid, workingDatabase);
                                polyFromObjId2 = GetPolyFromObjId(oid1, workingDatabase);
                                if (polyFromObjId1 == null ||
                                    polyFromObjId2 == null)
                                    continue;
                            }
                            else
                                continue;
                        }
                        catch (NullReferenceException ex)
                        {
                            PGA.MessengerManager.MessengerManager.LogException(ex);

                            continue;
                        }
                        if (Math.Abs(polyFromObjId1.Area - polyFromObjId2.Area) >= num)
                        {
                            var pointsFromPolyline = AcadUtilities.GetPointsFromPolyline(polyFromObjId2);
                            if (
                                PointInPolyline(AcadUtilities.GetPointsFromPolyline(polyFromObjId1),
                                    pointsFromPolyline) && oid1.IsValid)
                                objectIdCollection.Add(oid1);
                        }
                    }
                    if ((uint) objectIdCollection.Count > 0U)
                        return objectIdCollection;
                }
            }
            catch (NullReferenceException ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);

            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);

            }
            return null;
        }

        internal static bool PointInPolyline(Point2dCollection points, Point2dCollection tests)
        {
            var count1 = points.Count;
            var count2 = tests.Count;
            var flag = false;
            var index1 = 0;
            var num1 = count2 - 1;
            for (; index1 < count2; ++index1)
            {
                var point2d1 = tests[index1];
                var index2 = 0;
                var index3 = count1 - 1;
                for (; index2 < count1; index3 = index2++)
                {
                    var point2d2 = points[index2];
                    var num2 = point2d2.Y > point2d1.Y ? 1 : 0;
                    point2d2 = points[index3];
                    var num3 = point2d2.Y > point2d1.Y ? 1 : 0;
                    int num4;
                    if (num2 != num3)
                    {
                        var x1 = point2d1.X;
                        point2d2 = points[index3];
                        var x2 = point2d2.X;
                        point2d2 = points[index2];
                        var x3 = point2d2.X;
                        var num5 = x2 - x3;
                        var y1 = point2d1.Y;
                        point2d2 = points[index2];
                        var y2 = point2d2.Y;
                        var num6 = y1 - y2;
                        var num7 = num5*num6;
                        point2d2 = points[index3];
                        var y3 = point2d2.Y;
                        point2d2 = points[index2];
                        var y4 = point2d2.Y;
                        var num8 = y3 - y4;
                        var num9 = num7/num8;
                        point2d2 = points[index2];
                        var x4 = point2d2.X;
                        var num10 = num9 + x4;
                        num4 = x1 < num10 ? 1 : 0;
                    }
                    else
                        num4 = 0;
                    if (num4 != 0)
                        flag = !flag;
                }
                if (flag)
                    return flag;
            }
            return flag;
        }

        public static Polyline GetPolyFromObjId(ObjectId oid,  AcadDb.Database db)
        {
            try
            {
                using (var transaction = db.TransactionManager.StartTransaction())
                {
                    DBObject @object;
                    try
                    {
                        @object = transaction.GetObject(oid, OpenMode.ForRead);
                    }
                    catch (NullReferenceException ex)
                    {
                        PGA.MessengerManager.MessengerManager.LogException(ex);

                        return null;
                    }
                    var polyline = @object as Polyline;
                    if (polyline != null && polyline.Closed)
                        return polyline;
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);

            }
            return null;
        }

        public static Polyline GetPolyFromObjId(ObjectId oid)
        {
            using (var workingDatabase = WorkingDatabase)
            {
                try
                {
                    using (var transaction = workingDatabase.TransactionManager.StartTransaction())
                    {
                        DBObject @object;
                        try
                        {
                            @object = transaction.GetObject(oid, OpenMode.ForRead);
                        }
                        catch (NullReferenceException ex)
                        {
                            PGA.MessengerManager.MessengerManager.LogException(ex);

                            return null;
                        }
                        var polyline = @object as Polyline;
                        if (polyline != null && polyline.Closed)
                            return polyline;
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    PGA.MessengerManager.MessengerManager.LogException(ex);

                }
            }
            return null;
        }

        public static bool wn_PointInPolyline(Point2dCollection points, Point2dCollection tests)
        {
            try
            {
                var boundarypoints = points.ToArray();
                var num = points.Count;

                foreach (Point2d tpoint in tests)
                {
                    if (WNumAlgorithm.wn_PnPoly(tpoint, boundarypoints, num) != 0)
                        return true;
                }

            }
            catch (System.Exception)
            {

            }
            return false;
        }
    }
}
 
