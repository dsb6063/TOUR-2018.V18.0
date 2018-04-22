using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Civil3D = Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
namespace PGA.Surfaces.Civil3D
{
    class FeatureLines
    {
        //        public static void GetFeatureLine(string name ,Transaction trans,Autodesk.Civil.DatabaseServices.Entity entRes)
        //        {
        //          using trans Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction()
        //          {
        //          }

        //  Try

        //    If oAcadApp Is Nothing Then

        //      oAcadApp = GetObject(, "AutoCAD.Application")

        //    End If

        //  Catch ex As Exception

        //    ed.WriteMessage(ex.Message)

        //  End Try



        //  Try

        //    oAeccApp = oAcadApp.GetInterfaceObject("AeccXUiLand.AeccApplication.10.0")

        //    oAeccDoc = oAeccApp.ActiveDocument

        //    oAeccDB = oAeccApp.ActiveDocument.Database



        ////    ' Select the 3D Polyline which you want to convert to Feature Line



        ////    Dim promptEntOp As New PromptEntityOptions(vbCrLf + "Select a 3D Polyline : ")

        ////    Dim promptEntRs As PromptEntityResult

        ////    promptEntRs = ed.GetEntity(promptEntOp)

        ////    If promptEntRs.Status <> PromptStatus.OK Then

        ////      ed.WriteMessage("Exiting! Try Again !")

        ////      Exit Sub

        ////    End If

        ////    Dim idEnt As ObjectId

        ////    idEnt = promptEntRs.ObjectId



        ////    Dim oFtrLn As AeccLandFeatureLine = Nothing

        ////    Dim oFtrLns As AeccLandFeatureLines = oAeccDoc.Sites.Item(0).FeatureLines



        ////    Dim plineObjId As Long = idEnt.OldIdPtr



        ////    oFtrLn = oFtrLns.AddFromPolyline(plineObjId, oAeccDB.FeatureLineStyles.Item(0))



        ////    trans.Commit()

        ////  Catch ex As Exception

        ////    ed.WriteMessage("Error : ", ex.Message & vbCrLf)

        ////  End Try

        ////End Using

        //        }
        //         public static void CreateFeatureLine(string name ,Transaction trans,Autodesk.Civil.DatabaseServices.Entity entRes)
        //        {
        //          FeatureLine fl = (FeatureLine)trans.GetObject(entRes.ObjectId, OpenMode.ForWrite);

        //                       Autodesk.AECC.Interop.Land.AeccLandFeatureLine oFtrLn = (Autodesk.AECC.Interop.Land.AeccLandFeatureLine)fl​.AcadObject;
        //                       foreach (Point3d pt in ListaNuevosVertices)
        //                       {
        //                           double[] punto = new double[3];
        //                           punto[0] = pt.X;
        //                           punto[1] = pt.Y;
        //                           punto[2] = pt.Z;

        //                           oFtrLn.InsertFeaturePoint((object)punto, AeccLandFeatureLinePointType.aeccLandFeatureLinePo​intPI);

        //    }
        //}
    }
}