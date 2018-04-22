using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.Geometry;
using BBC.Common.AutoCAD;
using System;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using PGA.Sv.PostAudit;

namespace PGA.Autodesk.Utils
{
    using Database = global::Autodesk.AutoCAD.DatabaseServices.Database;


    public class PLineToLayers
    {
       
        private static bool IsLayerDefined(Database db,string layername)
        {
            try
            {
                if (LayerManager.IsDefined(db,layername))
                  return LayerManager.IsDefined(db,layername);
                throw new Exception("Layer not found: " + layername);
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);
            }
            return false;
        }

        public static string GetPolylineLayer(ObjectId selectedObjectId)
        {
  
            var db = Active.Database;
            var doc = Active.Document;
            var ed = Active.Editor;

            

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                DBObject obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);

                Polyline lwp = obj as Polyline;

                if (lwp != null)
                {
                    if (lwp.Closed)
                    {
                        return lwp.Layer;
                    }

                }

                else
                {

                    Polyline2d p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        return p2d.Layer;
                    }

                    else
                    {

                        Polyline3d p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            return p3d.Layer;
                        }
                    }
                }

                tr.Commit();
            }

            return String.Empty;
        }

        private static void InteratePolyLines(ObjectId selectedObjectId, Database db)
        {
            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Editor ed = doc.Editor;
            //Database db = doc.Database;
            Transaction tr = db.TransactionManager.StartTransaction();

            // Get the current UCS

            //CoordinateSystem3d ucs =
            //    ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (tr)
            {
                DBObject obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                Polyline lwp = obj as Polyline;

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (lwp.Closed)
                    {
                        //Common.Logging.ACADLogging.LogMyExceptions(lwp.Layer);
                        lwp.UpgradeOpen();
                        if (IsLayerDefined(db, AssignPolyLinesToLayers(lwp).ToUpper()))
                            lwp.Layer = AssignPolyLinesToLayers(lwp).ToUpper() ?? lwp.Layer;
                        lwp.DowngradeOpen();
                        //Common.Logging.ACADLogging.LogMyExceptions(lwp.Layer);
                    }
                    // Use a for loop to get each vertex, one by one

                    int vn = lwp.NumberOfVertices;

                    for (int i = 0; i < vn; i++)
                    {
                        // Could also get the 3D point here

                        Point2d pt = lwp.GetPoint2dAt(i);

                        //ed.WriteMessage("\n" + pt.ToString());
                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    Polyline2d p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        // Use foreach to get each contained vertex

                        foreach (ObjectId vId in p2d)
                        {
                            Vertex2d v2d =
                                (Vertex2d) tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                    );

                            //ed.WriteMessage(
                            //    "\n" + v2d.Position.ToString()
                            //    );
                        }
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        Polyline3d p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            // Use foreach to get each contained vertex

                            foreach (ObjectId vId in p3d)
                            {
                                PolylineVertex3d v3d =
                                    (PolylineVertex3d) tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                        );

                                //ed.WriteMessage(
                                //    "\n" + v3d.Position.ToString()
                                //    );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
        }

        private static string AssignPolyLinesToLayers(Polyline p2d)
        {
            //Get the 3-digit layer name of the object and compare

            try
            {
                string name = p2d.Layer.ToUpper();

                switch (name)
                {
                    case "OBR":
                        return "S-Bridge".ToUpper();

                    case "OBD":
                        return "S-Building".ToUpper();

                    case "OST":
                        return "S-Bunker".ToUpper();

                    case "OBO":
                        return "S-Bush".ToUpper();

                    case "OCA":
                        return "S-CartPath".ToUpper();

                    case "OCO":
                        return "S-Collar".ToUpper();

                    case "ODO":
                        return "S-Dirt-Outline".ToUpper();

                    case "OFW":
                        return "S-Fairway".ToUpper();

                    case "OGR":
                        return "S-Green".ToUpper();

                    case "OGS":
                        return "S-GreenSide-Bunker".ToUpper();

                    case "OIR":
                        return "S-Intermediate-Rough".ToUpper();

                    case "OLN":
                        return "S-LandScaping".ToUpper();

                    case "ONA":
                        return "S-Native-Area".ToUpper();

                    case "ORK":
                        return "S-Rock-Outline".ToUpper();

                    case "OPT":
                        return "S-Path".ToUpper();

                    case "ORO":
                        return "S-Rough-Outlines".ToUpper();

                    case "OSS":
                        return "S-Steps".ToUpper();

                    case "OTB":
                        return "S-Tee-Box".ToUpper();

                    case "OTO":
                        return "S-Tree-Outline".ToUpper();

                    case "OWS":
                        return "S-Walk-Strip".ToUpper();

                    case "OWL":
                        return "S-Wall".ToUpper();

                    case "OWA":
                        return "S-Water".ToUpper();

                    case "OWD":
                        return "S-Water-Drop".ToUpper();
                    case "OTH":
                        return "S-Other".ToUpper();
                    default:
                        return name;
                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.LogException(ex);

            }
            return null;
        }


        public static void ProcessLayers(ObjectIdCollection objs, Database db)
        {
            foreach (ObjectId obj in objs)
            {
                InteratePolyLines(obj, db);
            }
        }
    }
}
