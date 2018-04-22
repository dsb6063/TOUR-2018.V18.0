using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ACADDB = Autodesk.AutoCAD.DatabaseServices;

namespace PGA.SportVision
{
    public class HatchPolyline
    {

        //[CommandMethod("PGA-Hatch")]
        static public void TestHatch(Polyline poly)
        {
            if (poly == null) throw new ArgumentNullException(nameof(poly));

            Document doc = Application.DocumentManager.MdiActiveDocument;
            global::Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Editor ed = doc.Editor;

            try
            {
                using (Transaction Tx = db.TransactionManager.StartTransaction())
                {
                    ObjectId ModelSpaceId =
                            SymbolUtilityServices.GetBlockModelSpaceId(db);

                    BlockTableRecord btr = Tx.GetObject(ModelSpaceId,
                                      OpenMode.ForWrite) as BlockTableRecord;

                   // Point2d pt = new Point2d(0.0, 0.0);
                   //ACADDB.Polyline plBox =
                   //           new ACADDB.Polyline(4);

                   // plBox.Normal = Vector3d.ZAxis;
                   // plBox.AddVertexAt(0, pt, 0.0, -1.0, -1.0);
                   // plBox.AddVertexAt(1,
                   //              new Point2d(pt.X + 10, pt.Y), 0.0, -1.0, -1.0);
                   // plBox.AddVertexAt(2,
                   //           new Point2d(pt.X + 10, pt.Y + 5), 0.0, -1.0, -1.0);
                   // plBox.AddVertexAt(3,
                   //               new Point2d(pt.X, pt.Y + 5), 0.0, -1.0, -1.0);
                   // plBox.Closed = true;

                   // ObjectId pLineId;
                   // pLineId = btr.AppendEntity(plBox);
                   // Tx.AddNewlyCreatedDBObject(plBox, true);

                    ObjectIdCollection ObjIds = new ObjectIdCollection();
                    ObjIds.Add(poly.Id);

                    Hatch oHatch = new Hatch();
                    Vector3d normal = new Vector3d(0.0, 0.0, 1.0);
                    oHatch.Normal = normal;
                    oHatch.Elevation = 0.0;
                    oHatch.PatternScale = 2.0;
                    oHatch.SetHatchPattern(HatchPatternType.PreDefined, "ZIGZAG");
                    oHatch.ColorIndex = poly.Color.ColorIndex;
                    oHatch.Layer = poly.Layer;
                    
                    btr.AppendEntity(oHatch);
                    Tx.AddNewlyCreatedDBObject(oHatch, true);
                    //this works ok  
                    oHatch.Associative = true;
                    oHatch.AppendLoop((int)HatchLoopTypes.Default, ObjIds);
                    oHatch.EvaluateHatch(true);

                    Tx.Commit();
                }
            }
            catch (System.Exception ex)
            {
               // throw;
            }

        }
    }
}
