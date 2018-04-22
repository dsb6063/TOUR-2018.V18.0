using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace BBC.UCSHelper
{
    public class UcsHelper
    {

        [CommandMethod("NewUCS")]
        public static void NewUCS()
        {
            Editor ed = Application.DocumentManager.CurrentDocument.Editor;
            // Get the current document and database, and start a transaction
            try
            {
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the UCS table for read
                    UcsTable acUCSTbl;
                    acUCSTbl = acTrans.GetObject(acCurDb.UcsTableId,
                        OpenMode.ForRead) as UcsTable;

                    UcsTableRecord acUCSTblRec;

                    // Check to see if the "New_UCS" UCS table record exists
                    if (acUCSTbl.Has("New_UCS") == false)
                    {
                        acUCSTblRec = new UcsTableRecord();
                        acUCSTblRec.Name = "New_UCS";

                        // Open the UCSTable for write
                        acUCSTbl.UpgradeOpen();

                        // Add the new UCS table record
                        acUCSTbl.Add(acUCSTblRec);
                        acTrans.AddNewlyCreatedDBObject(acUCSTblRec, true);

                        //acUCSTblRec.Dispose();
                    }
                    else
                    {
                        acUCSTblRec = acTrans.GetObject(acUCSTbl["New_UCS"],
                            OpenMode.ForWrite) as UcsTableRecord;
                    }

                    acUCSTblRec.Origin = new Point3d(4, 5, 3);
                    acUCSTblRec.XAxis = new Vector3d(1, 0, 0);
                    acUCSTblRec.YAxis = new Vector3d(0, 1, 0);

                    // Open the active viewport
                    ViewportTableRecord acVportTblRec;
                    acVportTblRec = acTrans.GetObject(acDoc.Editor.ActiveViewportId,
                        OpenMode.ForWrite) as ViewportTableRecord;

                    // Display the UCS Icon at the origin of the current viewport
                    acVportTblRec.IconAtOrigin = true;
                    acVportTblRec.IconEnabled = true;

                    // Set the UCS current
                    acVportTblRec.SetUcs(acUCSTblRec.ObjectId);
                    acDoc.Editor.UpdateTiledViewportsFromDatabase();

                    // Display the name of the current UCS
                    UcsTableRecord acUCSTblRecActive;
                    acUCSTblRecActive = acTrans.GetObject(acVportTblRec.UcsName,
                        OpenMode.ForRead) as UcsTableRecord;

                    Application.ShowAlertDialog("The current UCS is: " +
                                                acUCSTblRecActive.Name);

                    PromptPointResult pPtRes;
                    PromptPointOptions pPtOpts = new PromptPointOptions("");

                    // Prompt for a point
                    pPtOpts.Message = "\nEnter a point: ";
                    pPtRes = acDoc.Editor.GetPoint(pPtOpts);

                    Point3d pPt3dWCS;
                    Point3d pPt3dUCS;

                    // If a point was entered, then translate it to the current UCS
                    if (pPtRes.Status == PromptStatus.OK)
                    {
                        pPt3dWCS = pPtRes.Value;
                        pPt3dUCS = pPtRes.Value;

                        // Translate the point from the current UCS to the WCS
                        Matrix3d newMatrix = new Matrix3d();
                        newMatrix = Matrix3d.AlignCoordinateSystem(Point3d.Origin,
                            Vector3d.XAxis,
                            Vector3d.YAxis,
                            Vector3d.ZAxis,
                            acVportTblRec.Ucs.Origin,
                            acVportTblRec.Ucs.Xaxis,
                            acVportTblRec.Ucs.Yaxis,
                            acVportTblRec.Ucs.Zaxis);

                        pPt3dWCS = pPt3dWCS.TransformBy(newMatrix);

                        Application.ShowAlertDialog("The WCS coordinates are: \n" +
                                                    pPt3dWCS.ToString() + "\n" +
                                                    "The UCS coordinates are: \n" +
                                                    pPt3dUCS.ToString());
                    }

                    // Save the new objects to the database
                    acTrans.Commit();
                }
            }
            catch (System.Exception ex)
            {
               ed.WriteMessage("Error " + ex.Message);
            }
        }

        public static void SetZAxisUcs( Editor ed, Point3d basePoint, Point3d positiveZaxisPoint)
        {
            Plane plane = new Plane(basePoint, basePoint.GetVectorTo(positiveZaxisPoint));
            Matrix3d ucs = Matrix3d.PlaneToWorld(plane);
            ed.CurrentUserCoordinateSystem = ucs;
        }
        public static CoordinateSystem3d GetZAxisUcsToWorld( Editor ed, Point3d basePoint, Point3d positiveZaxisPoint)
        {
            var pPt3dWCS = positiveZaxisPoint;
            var pPt3dUCS = positiveZaxisPoint;

            Plane plane = new Plane(basePoint, basePoint.GetVectorTo(positiveZaxisPoint));
            Matrix3d ucs = Matrix3d.PlaneToWorld(plane);
            var cs = ucs.CoordinateSystem3d;
            ed.WriteMessage(String.Format(" GetZAxisUcsToWorld: X={0},Y={1},Z={2}", cs.Xaxis, cs.Yaxis, cs.Zaxis));

            // Translate the point from the current UCS to the WCS
            Matrix3d newMatrix = new Matrix3d();
            newMatrix = Matrix3d.AlignCoordinateSystem(Point3d.Origin,
                Vector3d.XAxis,
                Vector3d.YAxis,
                Vector3d.ZAxis,
                ucs.CoordinateSystem3d.Origin,
                ucs.CoordinateSystem3d.Xaxis,
                ucs.CoordinateSystem3d.Yaxis,
                ucs.CoordinateSystem3d.Zaxis);

            pPt3dWCS = pPt3dUCS.TransformBy(newMatrix);

            ed.WriteMessage("\nThe WCS coordinates are: \n" +
                                        pPt3dWCS.ToString() + "\n" +
                                        "\nThe UCS coordinates are: \n" +
                                        pPt3dUCS.ToString());


            return ucs.CoordinateSystem3d;
        }

        public static void SetUCSToWorld()
        {
            Editor ed = Application.DocumentManager.CurrentDocument.Editor;
            ed.CurrentUserCoordinateSystem = Matrix3d.Identity;
            ed.Regen();
        }

        public static CoordinateSystem3d SetUCSToCustom(CoordinateSystem3d FromUcs, CoordinateSystem3d toUcs)
        {

            Editor ed = Application.DocumentManager.CurrentDocument.Editor;
            Matrix3d newMatrix = new Matrix3d();
            newMatrix = Matrix3d.AlignCoordinateSystem(FromUcs.Origin,
                FromUcs.Xaxis,
                FromUcs.Yaxis,
                FromUcs.Zaxis,
                toUcs.Origin,
                toUcs.Xaxis,
                toUcs.Yaxis,
                toUcs.Zaxis);

            ed.CurrentUserCoordinateSystem =  newMatrix;
            ed.Regen();

            return newMatrix.CoordinateSystem3d;
        }

        public static Point3d GetOrigin()
        {
            Editor ed = Application.DocumentManager.CurrentDocument.Editor;
            return ed.CurrentUserCoordinateSystem.CoordinateSystem3d.Origin;
        }

        public static Matrix3d CreateTranslationMatrix(double[]data)
        {
            return new Matrix3d(data);
        }
        public static Matrix3d CreateTranslationMatrix(Vector3d V)
        {
            var Xt = new Point3d(V.X, 0, 0).X;
            var Yt = new Point3d(0, V.Y, 0).Y;
            var Zt = new Point3d(0, 0, V.Z).Z;

            return new Matrix3d(new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0,Xt,Yt,Zt, 1 });
        }
        public static Matrix3d CreateTranslationMatrixFlatten(Vector3d V)
        {
            var Xt = new Point3d(V.X, 0, 0).X;
            var Yt = new Point3d(0, V.Y, 0).Y;
            var Zt = new Point3d(0, 0,  0 ).Z;

            return new Matrix3d(new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, Xt, Yt, Zt, 1 });
        }
        public static Matrix3d CreateTranslationMatrixFlatten()
        {   
            return new Matrix3d(new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 });
        }
        public static double GetNormal(Editor ed, Point3d basePoint, Point3d positiveZaxisPoint)
        {
            Plane plane = new Plane(basePoint, basePoint.GetVectorTo(positiveZaxisPoint));
            Matrix3d ucs = Matrix3d.PlaneToWorld(plane);
            ed.WriteMessage(String.Format("Normal" +  ucs.Normal ));

            return ucs.Normal;
        }

        public static Vector3d GetCrossProduct(Vector3d v1, Vector3d v2)
        {
            return v1.GetNormal(Tolerance.Global);
        }

        public static Matrix3d GetProjectionMatrix3D(Editor ed, Point3d basePoint, Point3d positiveZaxisPoint)
        {
            Plane plane  = new Plane(basePoint, basePoint.GetVectorTo(positiveZaxisPoint));
            Matrix3d ucs = Matrix3d.Projection(plane,new Vector3d
                (positiveZaxisPoint.X,positiveZaxisPoint.Y,positiveZaxisPoint.Z));
            var cs = ucs.CoordinateSystem3d;
            ed.WriteMessage(String.Format("X={0},Y={1},Z={2}", cs.Xaxis, cs.Yaxis, cs.Zaxis));
            ed.WriteMessage("Normal Vector" + plane.Normal);
            return ucs;
        }
    }
}
