using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using ACADDB = Autodesk.AutoCAD.DatabaseServices;

namespace PGA.SV30MOffset
{
    public class HatchPolyline
    {

        //[CommandMethod("PGA-Hatch")]
        static public void TestHatch(Polyline poly)
        {
            if (poly == null) throw new ArgumentNullException(nameof(poly));

            Document doc = Active.Document;
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
