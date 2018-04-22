// ***********************************************************************
// Assembly         : BBC.CivilSurf.001
// Author           : Daryl Banks, PSM
// Created          : 04-01-2017
//
// Last Modified By : Daryl Banks, PSM
// Last Modified On : 04-04-2017
// ***********************************************************************
// <copyright file="Class1.cs" company="Banks & Banks Consulting">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using BBC.SurfOffset002;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Exception = System.Exception;
using BBC.UCSHelper;

namespace BBC.CivilSurf
{

    /// <summary>
    /// Class GenerateSurfaceOffsets.
    /// </summary>
    public class GenerateSurfaceOffsets
    {
        private static int _featurelinecounter;

        /// <summary>
        /// The starttime
        /// </summary>
        private static DateTime _starttime;

        /// <summary>
        /// The _interval
        /// </summary>
        private static double _interval;

        /// <summary>
        /// The offset
        /// </summary>
        private static double _offset;

        /// <summary>
        /// The sampledist
        /// </summary>
        private static double _sampledist = 1.0;

        /// <summary>
        /// Handles the CommandEnded event of the CurrentDocument control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
        private static void CurrentDocument_CommandEnded(object sender, CommandEventArgs e)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;


            try
            {
                if (e.GlobalCommandName == "BBC-CreateSurfaceOffsets")
                {
                    ed.Command("_.BBC-TimeElasped", 1, 1, 0);

                    doc.CurrentDocument.CommandEnded -=
                        CurrentDocument_CommandEnded;
                }
            }
            catch (Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        /// <summary>
        /// Times this instance.
        /// </summary>
        [CommandMethod("BBC-TimeElasped")]
        public static void Time()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                ed.WriteMessage("\nTime Elapsed: " + ((DateTime.Now.Subtract(_starttime)).ToString(@"dd\.hh\:mm\:ss")));
                ed.WriteMessage("\nDone!");
            }
            catch (Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        //[CommandMethod("BBC-CreateSurfaceOffsets")]
        //public static void CreateSurfaceOffsets()
        //{
        //    var doc   = Application.DocumentManager;
        //    var ed = Application.DocumentManager.MdiActiveDocument.Editor;

        //  //  doc.CurrentDocument.CommandEnded += CurrentDocument_CommandEnded;

        //    try
        //    {
        //        CoordinateSystem3d ucs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

        //        PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a TIN Surface: ");
        //        selSurface.SetRejectMessage("\nOnly Tin Surface allowed");
        //        selSurface.AddAllowedClass(typeof(TinSurface), true);
        //        PromptEntityResult resSurface = ed.GetEntity(selSurface);
        //        if (resSurface.Status != PromptStatus.OK) return;
        //        ObjectId surfaceId = resSurface.ObjectId;

        //        //Add an Offset Distance
        //        PromptDoubleOptions pIntOpts = new PromptDoubleOptions("");
        //        pIntOpts.Message = "\nAdd an Offset Distance in Feet: ";
        //        pIntOpts.AllowNegative = true;
        //        pIntOpts.AllowZero = false;

        //        PromptDoubleResult doubleResult = ed.GetDouble(pIntOpts);
        //        if (doubleResult.Status != PromptStatus.OK) return;

        //        //Option to create a Line Normal to Triangle at center
        //        PromptKeywordOptions selYesNo =
        //            new PromptKeywordOptions(
        //                "\nCreate a Test Line from center of Base Triangle Normal to base of Offset Triangle?");
        //        selYesNo.Keywords.Add("Yes");
        //        selYesNo.Keywords.Add("No");
        //        PromptResult resYesNo = ed.GetKeywords(selYesNo);
        //        if (resYesNo.Status != PromptStatus.OK) return;
        //        bool createLine = resYesNo.StringResult.Equals("Yes");

        //        //select a polyline
        //        PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a Polyline Boundary: ");
        //        selPline.SetRejectMessage("\nOnly 2D Polylines allowed");
        //        selPline.AddAllowedClass(typeof(Polyline), true);
        //        PromptEntityResult resPline = ed.GetEntity(selPline);
        //        if (resPline.Status != PromptStatus.OK) return;
        //        ObjectId plineId = resPline.ObjectId;

        //        //erase the pline?
        //        PromptKeywordOptions selYesNopline = new PromptKeywordOptions("\nErase Polyline Boundary?");
        //        selYesNopline.Keywords.Add("Yes");
        //        selYesNopline.Keywords.Add("No");
        //        PromptResult resYesNopline = ed.GetKeywords(selYesNopline);
        //        if (resYesNopline.Status != PromptStatus.OK) return;
        //        bool erasePline = resYesNopline.StringResult.Equals("Yes");

        //        bool addboundary = false;
        //        if (!erasePline)
        //        {
        //            //add the pline as the outer boundary?
        //            PromptKeywordOptions selYesNobndy =
        //                new PromptKeywordOptions("\nAdd the Polyline as the Outer Boundary?");
        //            selYesNobndy.Keywords.Add("Yes");
        //            selYesNobndy.Keywords.Add("No");
        //            PromptResult resYesNobndy = ed.GetKeywords(selYesNobndy);
        //            if (resYesNobndy.Status != PromptStatus.OK) return;
        //            addboundary = resYesNobndy.StringResult.Equals("Yes");

        //        }
        //        ed.WriteMessage("\nStarting the Program!\n");
        //        starttime = DateTime.Now;

        //        Database db = Application.DocumentManager.MdiActiveDocument.Database;
        //        using (Transaction trans = db.TransactionManager.StartTransaction())
        //        {
        //            Point3dCollection newsurfacepoints = new Point3dCollection();

        //            TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

        //            Polyline pline = trans.GetObject(plineId, OpenMode.ForRead) as Polyline;
        //            //find all vertices inside a pline area
        //            var test = surface.ExtractGridded(SurfaceExtractionSettingsType.Model);
        //            ObjectIdCollection plinesBorder = new ObjectIdCollection();
        //            plinesBorder.Add(plineId);
        //            TinSurfaceVertex[] verticesInsidePline = surface.GetVerticesInsidePolylines(plinesBorder);

        //            var triangles = GetTrianglesFromSurface(surface);


        //            ed.WriteMessage("\nNumber of Triangles Processes: " + verticesInsidePline.Length + "\n");


        //            foreach (var tri in triangles)
        //            {
        //                //Filter out or rewrite for Vertices's Not Triangles

        //                if(!( verticesInsidePline.Contains(tri.Vertex1)|| 
        //                      verticesInsidePline.Contains(tri.Vertex2)|| 
        //                      verticesInsidePline.Contains(tri.Vertex3)))
        //                {
        //                    continue;
        //                }

        //                //Set the UCS to Triangle Plane  

        //                ///
        //                /// Todo: Implement the correct coordinate rotation for better performance
        //                ///

        //                // Center of Triangle Offset

        //                var surfaceTri = new SurfaceTriangle(tri.Vertex1.Location, tri.Vertex2.Location,
        //                    tri.Vertex3.Location);

        //                var mathnetTri = new SurfaceTriangleToMathNETFormat(surfaceTri.Vertex1, surfaceTri.Vertex2,
        //                    surfaceTri.Vertex3);

        //                var resulpoint = BBC.SurfOffet.PlaneGeometry.FindOffset
        //                (mathnetTri.Vertex1, mathnetTri.Vertex2,
        //                    mathnetTri.Vertex3, doubleResult.Value);

        //                var cross = BBC.SurfOffet.PlaneGeometry.Cross(mathnetTri.Vertex1, mathnetTri.Vertex2);

        //                var nativeVec12 = surfaceTri.Vertex1.GetVectorTo(surfaceTri.Vertex2);
        //                var nativeVec13 = surfaceTri.Vertex1.GetVectorTo(surfaceTri.Vertex3);
        //                var nativeCross = nativeVec12.CrossProduct(nativeVec13);

        //                var normal = new Vector3d(cross[0], cross[1], cross[2]);

        //                //var areequal = nativeCross.CrossProduct(normal);

        //                var point = new MathNETToPoint3DFormat(resulpoint);

        //                var center = BBC.SurfOffet.PlaneGeometry.FindTriangleCenter(mathnetTri.Vertex1,
        //                    mathnetTri.Vertex2,
        //                    mathnetTri.Vertex3, cross);

        //                //Assumption: Z is always positive
        //                //A point on the normal Z axis
        //                var p2 = new Point3d(normal.X + center[0], normal.Y + center[1], Math.Abs(normal.Z + center[2]));


        //                var centerP3d = new Point3d(center[0], center[1], center[2]);

        //                //EditorExtension.GetZAxisUcs(ed, centerP3d, p2);
        //                EditorExtension.SetZAxisUcs(ed, centerP3d, p2);
        //                EditorExtension.GetZAxisUcs(ed, centerP3d, p2);
        //                //BBC.UCSHelper.UcsHelper.GetZAxisUcsToWorld(ed, centerP3d, p2);
        //                ed.WriteMessage("\n*************BEFORE***************\n");
        //                ed.WriteMessage("\nP3d " + tri.Vertex1.Location.X +" "+ tri.Vertex2.Location.Y + " " +  tri.Vertex3.Location.Z);
        //                UcsHelper.GetZAxisUcsToWorld(ed, new Point3d(tri.Vertex1.Location.X,tri.Vertex2.Location.Y,tri.Vertex3.Location.Z), p2);

        //                if (createLine)
        //                    SkeletonSurf.SkeletonSurf.CreateLine(centerP3d,
        //                        new Point3d(point.Vertex1.X, point.Vertex1.Y, point.Vertex1.Z));

        //                newsurfacepoints.Add(new Point3d(tri.Vertex1.Location.X, tri.Vertex1.Location.Y,
        //                    tri.Vertex1.Location.Z + doubleResult.Value));
        //                newsurfacepoints.Add(new Point3d(tri.Vertex2.Location.X, tri.Vertex2.Location.Y,
        //                    tri.Vertex2.Location.Z + doubleResult.Value));
        //                newsurfacepoints.Add(new Point3d(tri.Vertex3.Location.X, tri.Vertex3.Location.Y,
        //                    tri.Vertex3.Location.Z + doubleResult.Value));
        //                newsurfacepoints.Add(new Point3d(centerP3d.X, centerP3d.Y, centerP3d.Z + doubleResult.Value));

        //                ed.WriteMessage("\n*************AFTER***************\n");
        //                UcsHelper.GetZAxisUcsToWorld(ed, new Point3d(tri.Vertex1.Location.X, tri.Vertex2.Location.Y, tri.Vertex3.Location.Z), p2);
        //            }

        //            var surfaceOid = TinSurface.Create(db, "AutoSurface");
        //            var newSurface = trans.GetObject(surfaceOid, OpenMode.ForWrite) as TinSurface;
        //            newSurface.AddVertices(newsurfacepoints);

        //            if(erasePline)
        //              EraseObject(pline);
        //            else
        //            {
        //                if (addboundary)
        //                    AddStandardBoundary(plineId,newSurface.Name,newSurface);
        //            }

        //            trans.Commit();

        //            ed.CurrentUserCoordinateSystem = Matrix3d.Identity;
        //            ed.Regen();

        //        }

        //    }
        //    catch (System.Exception ex)
        //    {
        //        ed.WriteMessage("Error " + ex.Message);
        //    }
        //}
        /// <summary>
        /// Tests the create surface offsets.
        /// </summary>
        [CommandMethod("BBC-CreateSurfaceOffsets")]
        public static void TestCreateSurfaceOffsets()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
            var ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            doc.CurrentDocument.CommandEnded += CurrentDocument_CommandEnded;

            try
            {
                CoordinateSystem3d ucs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

                PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a TIN Surface: ");
                selSurface.SetRejectMessage("\nOnly Tin Surface allowed");
                selSurface.AddAllowedClass(typeof(TinSurface), true);
                PromptEntityResult resSurface = ed.GetEntity(selSurface);
                if (resSurface.Status != PromptStatus.OK) return;
                ObjectId surfaceId = resSurface.ObjectId;

                //Add an Offset Distance
                PromptDoubleOptions pIntOpts = new PromptDoubleOptions("");
                pIntOpts.Message = "\nAdd an Offset Distance in Feet: ";
                pIntOpts.AllowNegative = true;
                pIntOpts.AllowZero = false;

                PromptDoubleResult doubleResult = ed.GetDouble(pIntOpts);
                if (doubleResult.Status != PromptStatus.OK) return;
                var offset = doubleResult.Value;

                //Option to create a Line Normal to Triangle at center
                PromptKeywordOptions selYesNo =
                    new PromptKeywordOptions(
                        "\nCreate a Test Line from center of Base Triangle Normal to base of Offset Triangle?");
                selYesNo.Keywords.Add("Yes");
                selYesNo.Keywords.Add("No");
                PromptResult resYesNo = ed.GetKeywords(selYesNo);
                if (resYesNo.Status != PromptStatus.OK) return;
                bool createLine = resYesNo.StringResult.Equals("Yes");

                //select a polyline
                PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a Polyline Boundary: ");
                selPline.SetRejectMessage("\nOnly 2D Polylines allowed");
                selPline.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult resPline = ed.GetEntity(selPline);
                if (resPline.Status != PromptStatus.OK) return;
                ObjectId plineId = resPline.ObjectId;

                //erase the pline?
                PromptKeywordOptions selYesNopline = new PromptKeywordOptions("\nErase Polyline Boundary?");
                selYesNopline.Keywords.Add("Yes");
                selYesNopline.Keywords.Add("No");
                PromptResult resYesNopline = ed.GetKeywords(selYesNopline);
                if (resYesNopline.Status != PromptStatus.OK) return;
                bool erasePline = resYesNopline.StringResult.Equals("Yes");

                bool addboundary = false;
                if (!erasePline)
                {
                    //add the pline as the outer boundary?
                    PromptKeywordOptions selYesNobndy =
                        new PromptKeywordOptions("\nAdd the Polyline as the Outer Boundary?");
                    selYesNobndy.Keywords.Add("Yes");
                    selYesNobndy.Keywords.Add("No");
                    PromptResult resYesNobndy = ed.GetKeywords(selYesNobndy);
                    if (resYesNobndy.Status != PromptStatus.OK) return;
                    addboundary = resYesNobndy.StringResult.Equals("Yes");

                }
                ed.WriteMessage("\nStarting the Program!\n");
                _starttime = DateTime.Now;

                Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Point3dCollection newsurfacepoints = new Point3dCollection();

                    TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

                    Polyline pline = trans.GetObject(plineId, OpenMode.ForRead) as Polyline;
                    //find all vertices inside a pline area
                    if (surface != null)
                    {
                        surface.ExtractGridded(SurfaceExtractionSettingsType.Model);
                        ObjectIdCollection plinesBorder = new ObjectIdCollection();
                        plinesBorder.Add(plineId);
                        TinSurfaceVertex[] verticesInsidePline = surface.GetVerticesInsidePolylines(plinesBorder);

                        var triangles = GetTrianglesFromSurface(surface);


                        ed.WriteMessage("\nNumber of Triangles Processes: " + verticesInsidePline.Length/3 + "\n");


                        foreach (var tri in triangles)
                        {
                            //Filter out or rewrite for Vertices's Not Triangles

                            if (!(verticesInsidePline.Contains(tri.Vertex1) ||
                                  verticesInsidePline.Contains(tri.Vertex2) ||
                                  verticesInsidePline.Contains(tri.Vertex3)))
                            {
                                continue;
                            }

                            //Set the UCS to Triangle Plane  

                            ///
                            /// Todo: Implement the correct coordinate rotation for better performance
                            ///

                            // Center of Triangle Offset

                            var surfaceTri = new SurfaceTriangle(tri.Vertex1.Location, tri.Vertex2.Location,
                                tri.Vertex3.Location);

                            var mathnetTri = new SurfaceTriangleToMathNetFormat(surfaceTri.Vertex1, surfaceTri.Vertex2,
                                surfaceTri.Vertex3);

                            var cross = SurfOffet.PlaneGeometry.Cross(mathnetTri.Vertex1, mathnetTri.Vertex2);

                            var nativeVec12 = surfaceTri.Vertex1.GetVectorTo(surfaceTri.Vertex2);
                            var nativeVec13 = surfaceTri.Vertex1.GetVectorTo(surfaceTri.Vertex3);
                            var nativeNormal = nativeVec12.CrossProduct(nativeVec13);
 

                            var center = BBC.SurfOffet.PlaneGeometry.FindTriangleCenter(mathnetTri.Vertex1,
                                mathnetTri.Vertex2,
                                mathnetTri.Vertex3, cross);


                            var centerP3D = new Point3d(center[0], center[1], center[2]);


                            if (createLine)
                            {
                                var baseptp3D = GetPoint3D(centerP3D);
                                var offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                SkeletonSurf.SkeletonSurf.CreateLine(centerP3D, offsetp3D);

                                baseptp3D = GetPoint3D(tri.Vertex1.Location);
                                offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                SkeletonSurf.SkeletonSurf.CreateLine(baseptp3D, offsetp3D);
                                newsurfacepoints.Add(offsetp3D);

                                baseptp3D = GetPoint3D(tri.Vertex2.Location);
                                offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                SkeletonSurf.SkeletonSurf.CreateLine(baseptp3D, offsetp3D);
                                newsurfacepoints.Add(offsetp3D);

                                baseptp3D = GetPoint3D(tri.Vertex3.Location);
                                offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                SkeletonSurf.SkeletonSurf.CreateLine(baseptp3D, offsetp3D);
                                newsurfacepoints.Add(offsetp3D);

                            }

                        }
                    }

                    var surfaceOid = TinSurface.Create(db, "AutoSurface");
                    var newSurface = trans.GetObject(surfaceOid, OpenMode.ForWrite) as TinSurface;
                    newSurface.AddVertices(newsurfacepoints);

                    if (erasePline)
                        EraseObject(pline);
                    else
                    {
                        if (addboundary)
                            AddStandardBoundary(plineId, newSurface.Name, newSurface);
                    }

                    trans.Commit();

                    ed.CurrentUserCoordinateSystem = Matrix3d.Identity;
                    ed.Regen();

                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        [CommandMethod("BBC-CreateSurfaceOffsetsManual")]
        public static void TestCreateSurfaceOffsetsManual()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
            var ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            doc.CurrentDocument.CommandEnded += CurrentDocument_CommandEnded;

            try
            {
                CoordinateSystem3d ucs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

                PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a TIN Surface: ");
                selSurface.SetRejectMessage("\nOnly Tin Surface allowed");
                selSurface.AddAllowedClass(typeof(TinSurface), true);
                PromptEntityResult resSurface = ed.GetEntity(selSurface);
                if (resSurface.Status != PromptStatus.OK) return;
                ObjectId surfaceId = resSurface.ObjectId;

                //Add an Offset Distance
                PromptDoubleOptions pIntOpts = new PromptDoubleOptions("");
                pIntOpts.Message = "\nAdd an Offset Distance in Feet: ";
                pIntOpts.AllowNegative = true;
                pIntOpts.AllowZero = false;

                PromptDoubleResult doubleResult = ed.GetDouble(pIntOpts);
                if (doubleResult.Status != PromptStatus.OK) return;
                var offset = doubleResult.Value;


                //Add an Cutoff Angle
                PromptDoubleOptions pIntAngleOpts = new PromptDoubleOptions("");
                pIntAngleOpts.Message = "\nAdd a cutoff angle in degrees (90): ";
                pIntAngleOpts.AllowNegative = true;
                pIntAngleOpts.AllowZero = false;

                PromptDoubleResult doubleAngleResult = ed.GetDouble(pIntAngleOpts);
                if (doubleAngleResult.Status != PromptStatus.OK) return;
                var angle = doubleAngleResult.Value;

                //Add an Weed Distance
                PromptDoubleOptions pIntWeedOpts = new PromptDoubleOptions("");
                pIntWeedOpts.Message = "\nAdd an weed distance in feet: ";
                pIntWeedOpts.AllowNegative = true;
                pIntWeedOpts.AllowZero = false;

                PromptDoubleResult doubleWeedResult = ed.GetDouble(pIntWeedOpts);
                if (doubleWeedResult.Status != PromptStatus.OK) return;
                var weeding = doubleWeedResult.Value;

                //Option to create a Line Normal to Triangle at center
                PromptKeywordOptions selYesNo =
                    new PromptKeywordOptions(
                        "\nCreate a Test Line from center of Base Triangle Normal to base of Offset Triangle?");
                selYesNo.Keywords.Add("Yes");
                selYesNo.Keywords.Add("No");
                PromptResult resYesNo = ed.GetKeywords(selYesNo);
                if (resYesNo.Status != PromptStatus.OK) return;
                bool createLine = resYesNo.StringResult.Equals("Yes");

                //select a polyline
                PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a Polyline Boundary: ");
                selPline.SetRejectMessage("\nOnly 2D Polylines allowed");
                selPline.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult resPline = ed.GetEntity(selPline);
                if (resPline.Status != PromptStatus.OK) return;
                ObjectId plineId = resPline.ObjectId;

                //erase the pline?
                PromptKeywordOptions selYesNopline = new PromptKeywordOptions("\nErase Polyline Boundary?");
                selYesNopline.Keywords.Add("Yes");
                selYesNopline.Keywords.Add("No");
                PromptResult resYesNopline = ed.GetKeywords(selYesNopline);
                if (resYesNopline.Status != PromptStatus.OK) return;
                bool erasePline = resYesNopline.StringResult.Equals("Yes");

                bool addboundary = false;
                if (!erasePline)
                {
                    //add the pline as the outer boundary?
                    PromptKeywordOptions selYesNobndy =
                        new PromptKeywordOptions("\nAdd the Polyline as the Outer Boundary?");
                    selYesNobndy.Keywords.Add("Yes");
                    selYesNobndy.Keywords.Add("No");
                    PromptResult resYesNobndy = ed.GetKeywords(selYesNobndy);
                    if (resYesNobndy.Status != PromptStatus.OK) return;
                    addboundary = resYesNobndy.StringResult.Equals("Yes");

                }
                ed.WriteMessage("\nStarting the Program!\n");
                _starttime = DateTime.Now;

                Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Point3dCollection newsurfacepoints = new Point3dCollection();

                    TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

                    Polyline pline = trans.GetObject(plineId, OpenMode.ForRead) as Polyline;
                    //find all vertices inside a pline area
                    if (surface != null)
                    {
                        surface.ExtractGridded(SurfaceExtractionSettingsType.Model);
                        ObjectIdCollection plinesBorder = new ObjectIdCollection();
                        plinesBorder.Add(plineId);
                        TinSurfaceVertex[] verticesInsidePline = surface.GetVerticesInsidePolylines(plinesBorder);

                        var triangles = GetTrianglesFromSurface(surface);


                        ed.WriteMessage("\nNumber of Triangles Processes: " + verticesInsidePline.Length / 3 + "\n");

                        // Collection of Points for Weeding

                        var usedTriangles = new List<Point3d>();


                        // Iterate the Triangles

                        foreach (var tri in triangles)
                        {
                            //Filter out or rewrite for Vertices's Not Triangles

                            if (!(verticesInsidePline.Contains(tri.Vertex1) ||
                                  verticesInsidePline.Contains(tri.Vertex2) ||
                                  verticesInsidePline.Contains(tri.Vertex3)))
                            {
                                continue;
                            }

                            //Set the UCS to Triangle Plane  

                            ///
                            /// Todo: Implement the correct coordinate rotation for better performance
                            ///

                            // Center of Triangle Offset

                            var surfaceTri = new SurfaceTriangle(tri.Vertex1.Location, tri.Vertex2.Location,
                                tri.Vertex3.Location);

                            var mathnetTri = new SurfaceTriangleToMathNetFormat(surfaceTri.Vertex1, surfaceTri.Vertex2,
                                surfaceTri.Vertex3);

                            var cross = SurfOffet.PlaneGeometry.Cross(mathnetTri.Vertex1, mathnetTri.Vertex2);

                            var nativeVec12 = surfaceTri.Vertex1.GetVectorTo(surfaceTri.Vertex2);
                            var nativeVec13 = surfaceTri.Vertex1.GetVectorTo(surfaceTri.Vertex3);
                            var nativeNormal = nativeVec12.CrossProduct(nativeVec13);


                            var center = BBC.SurfOffet.PlaneGeometry.FindTriangleCenter(mathnetTri.Vertex1,
                                mathnetTri.Vertex2,
                                mathnetTri.Vertex3, cross);


                            var centerP3D = new Point3d(center[0], center[1], center[2]);

                            if (ExceedsNormalCutoff(angle,nativeNormal))
                                continue;
                            

                            if (WithinWeedDistance(weeding, centerP3D, usedTriangles))
                                continue;

                            usedTriangles.Add(centerP3D);

                            if (createLine)
                            {
                                var baseptp3D = GetPoint3D(centerP3D);
                                var offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                if (IsPointInsidePoly(pline.ObjectId,offsetp3D) &&  (IsPointInsidePoly(pline.ObjectId, baseptp3D)))
                                    SkeletonSurf.SkeletonSurf.CreateLine(centerP3D, offsetp3D);

                                baseptp3D = GetPoint3D(tri.Vertex1.Location);
                                offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                if (IsPointInsidePoly(pline.ObjectId, offsetp3D) && (IsPointInsidePoly(pline.ObjectId, baseptp3D)))
                                {
                                    SkeletonSurf.SkeletonSurf.CreateLine(baseptp3D, offsetp3D);
                                    newsurfacepoints.Add(offsetp3D);
                                }
                                baseptp3D = GetPoint3D(tri.Vertex2.Location);
                                offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                if (IsPointInsidePoly(pline.ObjectId, offsetp3D) && (IsPointInsidePoly(pline.ObjectId, baseptp3D)))
                                {
                                    SkeletonSurf.SkeletonSurf.CreateLine(baseptp3D, offsetp3D);
                                    newsurfacepoints.Add(offsetp3D);
                                }

                                baseptp3D = GetPoint3D(tri.Vertex3.Location);
                                offsetp3D = GetTriangleTranslationOffset(baseptp3D, nativeNormal, offset);

                                if (IsPointInsidePoly(pline.ObjectId, offsetp3D) && (IsPointInsidePoly(pline.ObjectId, baseptp3D)))
                                {
                                    SkeletonSurf.SkeletonSurf.CreateLine(baseptp3D, offsetp3D);
                                    newsurfacepoints.Add(offsetp3D);
                                }

                            }

                        }
                    }

                    var surfaceOid = TinSurface.Create(db, "AutoSurface");
                    var newSurface = trans.GetObject(surfaceOid, OpenMode.ForWrite) as TinSurface;
                    newSurface.AddVertices(newsurfacepoints);

                    if (erasePline)
                        EraseObject(pline);
                    else
                    {
                        if (addboundary)
                            AddStandardBoundary(plineId, newSurface.Name, newSurface);
                    }

                    trans.Commit();

                    ed.CurrentUserCoordinateSystem = Matrix3d.Identity;
                    ed.Regen();

                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        [CommandMethod("BBC-CreateSurfaceOffsetsOWA")]
        public static void TestCreateSurfaceOffsetsOWA()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
            var ed  = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            doc.CurrentDocument.CommandEnded += CurrentDocument_CommandEnded;

            try
            {
                CoordinateSystem3d ucs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

                PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a TIN Surface: ");
                selSurface.SetRejectMessage("\nOnly Tin Surface allowed");
                selSurface.AddAllowedClass(typeof(TinSurface), true);

                PromptEntityResult resSurface = ed.GetEntity(selSurface);

                if (resSurface.Status != PromptStatus.OK) return;
                ObjectId surfaceId = resSurface.ObjectId;

                //Add an Offset Distance
                PromptDoubleOptions pIntOpts = new PromptDoubleOptions("");
                pIntOpts.Message = "\nAdd an Offset Distance in Feet: ";
                pIntOpts.AllowNegative = true;
                pIntOpts.AllowZero = false;

                PromptDoubleResult doubleResult = ed.GetDouble(pIntOpts);
                if (doubleResult.Status != PromptStatus.OK) return;
                var offset = doubleResult.Value;


                //Add an Cutoff Angle
                PromptDoubleOptions pIntAngleOpts = new PromptDoubleOptions("");
                pIntAngleOpts.Message = "\nAdd a cutoff angle in degrees (90): ";
                pIntAngleOpts.AllowNegative = true;
                pIntAngleOpts.AllowZero = false;

                PromptDoubleResult doubleAngleResult = ed.GetDouble(pIntAngleOpts);
                if (doubleAngleResult.Status != PromptStatus.OK) return;
                var angle = doubleAngleResult.Value;

                //Add an Weed Distance
                PromptDoubleOptions pIntWeedOpts = new PromptDoubleOptions("");
                pIntWeedOpts.Message = "\nAdd an weed distance in feet: ";
                pIntWeedOpts.AllowNegative = true;
                pIntWeedOpts.AllowZero = false;

                PromptDoubleResult doubleWeedResult = ed.GetDouble(pIntWeedOpts);
                if (doubleWeedResult.Status != PromptStatus.OK) return;
                var weeding = doubleWeedResult.Value;

                //Option to create a Line Normal to Triangle at center
                PromptKeywordOptions selYesNo =
                    new PromptKeywordOptions(
                        "\nCreate a Test Line from center of Base Triangle Normal to base of Offset Triangle?");
                selYesNo.Keywords.Add("Yes");
                selYesNo.Keywords.Add("No");
                PromptResult resYesNo = ed.GetKeywords(selYesNo);
                if (resYesNo.Status != PromptStatus.OK) return;
                bool createLine = resYesNo.StringResult.Equals("Yes");

                //select a polyline
                PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a Polyline Boundary: ");
                selPline.SetRejectMessage("\nOnly 2D Polylines allowed");
                selPline.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult resPline = ed.GetEntity(selPline);
                if (resPline.Status != PromptStatus.OK) return;
                ObjectId plineId = resPline.ObjectId;

                //erase the pline?
                PromptKeywordOptions selYesNopline = new PromptKeywordOptions("\nErase Polyline Boundary?");
                selYesNopline.Keywords.Add("Yes");
                selYesNopline.Keywords.Add("No");
                PromptResult resYesNopline = ed.GetKeywords(selYesNopline);
                if (resYesNopline.Status != PromptStatus.OK) return;
                bool erasePline = resYesNopline.StringResult.Equals("Yes");

                bool addboundary = false;
                if (!erasePline)
                {
                    PromptKeywordOptions selYesNobndy =
                        new PromptKeywordOptions("\nAdd the Polyline as the Outer Boundary?");
                    selYesNobndy.Keywords.Add("Yes");
                    selYesNobndy.Keywords.Add("No");
                    PromptResult resYesNobndy = ed.GetKeywords(selYesNobndy);

                    if (resYesNobndy.Status != PromptStatus.OK) return;
                    addboundary = resYesNobndy.StringResult.Equals("Yes");

                }
                ed.WriteMessage("\nStarting the Program!\n");
                _starttime = DateTime.Now;

                Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    Point3dCollection newsurfacepoints = new Point3dCollection();

                    TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

                    Polyline pline = trans.GetObject(plineId, OpenMode.ForRead) as Polyline;

                    //find all vertices inside a pline area

                    if (surface != null)
                    {
                        surface.ExtractGridded(SurfaceExtractionSettingsType.Model);
                        ObjectIdCollection plinesBorder = new ObjectIdCollection();
                        plinesBorder.Add(plineId);
                        TinSurfaceVertex[] verticesInsidePline = surface.GetVerticesInsidePolylines(plinesBorder);

                        var corrected = new List<Vector3d>();

                        // Iterate all the connected Triangles

                        foreach (var vert in verticesInsidePline.ToList())
                        {

                            Debug.WriteLine("Before Correction " + vert.Location.Z.ToString());

                            corrected = BBC.SurfOffset002.OffsetPoints.GetOffsetPoints(vert, offset);

                            var outtriangle = new Point3dCollection();

                            foreach (var item in corrected)
                            {
                                if (Math.Abs(vert.Location.Z - item.Z) < Math.Abs(offset + 0.5))
                                {

                                    newsurfacepoints.Add(new Point3d(item.X, item.Y, item.Z));
                                    


                                    Debug.WriteLine("After Correction " + item.Z);
                                }
                                else
                                {
                                   // newsurfacepoints.Add(new Point3d(item.X, item.Y, vert.Location.Z + offset));

                                    Debug.WriteLine("After Correction (Z outside tol: " + item.Z);


                                }

                               
                            }

                            if (createLine)
                            {

                                var start = vert.Location;
                                var end = corrected.FirstOrDefault().ToArray();
                                var endpt = new Point3d(end[0], end[1], end[2]);
                                var p3dcol = new Point3dCollection();
                                p3dcol.Add(start);
                                p3dcol.Add(endpt);
                                Create3DPolyline(p3dcol);

                                newsurfacepoints.RemoveAt(0);

                                //Create3DPolyline(newsurfacepoints);
                            }

                        }

                    }

                    var surfaceOid = TinSurface.Create(db, "AutoSurface");
                    var newSurface = trans.GetObject(surfaceOid, OpenMode.ForWrite) as TinSurface;
                    if (newsurfacepoints.Count > 0)
                    {
                        newSurface.AddVertices(newsurfacepoints);
                        //newSurface.BreaklinesDefinition.AddStandardBreaklines(newsurfacepoints, 1, 1, 1, 1);
                    }


                    if (erasePline)
                        EraseObject(pline);
                    else
                    {
                        if (addboundary)
                            AddStandardBoundary(plineId, newSurface.Name, newSurface);
                    }

                    trans.Commit();

                    ed.CurrentUserCoordinateSystem = Matrix3d.Identity;
                    ed.Regen();

                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }


        [CommandMethod("BBC-CreateSurfaceOffsetsTriangles")]
        public static void TestCreateSurfaceOffsetsTriangles()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
            var ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            doc.CurrentDocument.CommandEnded += CurrentDocument_CommandEnded;

            try
            {
                CoordinateSystem3d ucs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

                PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a TIN Surface: ");
                selSurface.SetRejectMessage("\nOnly Tin Surface allowed");
                selSurface.AddAllowedClass(typeof(TinSurface), true);

                PromptEntityResult resSurface = ed.GetEntity(selSurface);

                if (resSurface.Status != PromptStatus.OK) return;
                ObjectId surfaceId = resSurface.ObjectId;

                //Add an Offset Distance
                PromptDoubleOptions pIntOpts = new PromptDoubleOptions("");
                pIntOpts.Message = "\nAdd an Offset Distance in Feet: ";
                pIntOpts.AllowNegative = true;
                pIntOpts.AllowZero = false;

                PromptDoubleResult doubleResult = ed.GetDouble(pIntOpts);
                if (doubleResult.Status != PromptStatus.OK) return;
                var offset = doubleResult.Value;


                //Add an Cutoff Angle
                PromptDoubleOptions pIntAngleOpts = new PromptDoubleOptions("");
                pIntAngleOpts.Message = "\nAdd a cutoff angle in degrees (90): ";
                pIntAngleOpts.AllowNegative = true;
                pIntAngleOpts.AllowZero = false;

                PromptDoubleResult doubleAngleResult = ed.GetDouble(pIntAngleOpts);
                if (doubleAngleResult.Status != PromptStatus.OK) return;
                var angle = doubleAngleResult.Value;

                //Add an Weed Distance
                PromptDoubleOptions pIntWeedOpts = new PromptDoubleOptions("");
                pIntWeedOpts.Message = "\nAdd an weed distance in feet: ";
                pIntWeedOpts.AllowNegative = true;
                pIntWeedOpts.AllowZero = false;

                PromptDoubleResult doubleWeedResult = ed.GetDouble(pIntWeedOpts);
                if (doubleWeedResult.Status != PromptStatus.OK) return;
                var weeding = doubleWeedResult.Value;

                //Option to create a Line Normal to Triangle at center
                PromptKeywordOptions selYesNo =
                    new PromptKeywordOptions(
                        "\nCreate a Test Line from center of Base Triangle Normal to base of Offset Triangle?");
                selYesNo.Keywords.Add("Yes");
                selYesNo.Keywords.Add("No");
                PromptResult resYesNo = ed.GetKeywords(selYesNo);
                if (resYesNo.Status != PromptStatus.OK) return;
                bool createLine = resYesNo.StringResult.Equals("Yes");

                //select a polyline
                PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a Polyline Boundary: ");
                selPline.SetRejectMessage("\nOnly 2D Polylines allowed");
                selPline.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult resPline = ed.GetEntity(selPline);
                if (resPline.Status != PromptStatus.OK) return;
                ObjectId plineId = resPline.ObjectId;

                //erase the pline?
                PromptKeywordOptions selYesNopline = new PromptKeywordOptions("\nErase Polyline Boundary?");
                selYesNopline.Keywords.Add("Yes");
                selYesNopline.Keywords.Add("No");
                PromptResult resYesNopline = ed.GetKeywords(selYesNopline);
                if (resYesNopline.Status != PromptStatus.OK) return;
                bool erasePline = resYesNopline.StringResult.Equals("Yes");

                bool addboundary = false;
                if (!erasePline)
                {
                    PromptKeywordOptions selYesNobndy =
                        new PromptKeywordOptions("\nAdd the Polyline as the Outer Boundary?");
                    selYesNobndy.Keywords.Add("Yes");
                    selYesNobndy.Keywords.Add("No");
                    PromptResult resYesNobndy = ed.GetKeywords(selYesNobndy);

                    if (resYesNobndy.Status != PromptStatus.OK) return;
                    addboundary = resYesNobndy.StringResult.Equals("Yes");

                }
                ed.WriteMessage("\nStarting the Program!\n");
                _starttime = DateTime.Now;

                Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    var corrected      = new Point3dCollection();
                    var edgescorrected = new Point3dCollection();

                    TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

                    Polyline pline = trans.GetObject(plineId, OpenMode.ForRead) as Polyline;

                    //find all vertices inside a pline area

                    if (surface != null)
                    {
                        surface.ExtractGridded(SurfaceExtractionSettingsType.Model);
                        ObjectIdCollection plinesBorder = new ObjectIdCollection();
                        plinesBorder.Add(plineId);
                        TinSurfaceVertex[] verticesInsidePline = surface.GetVerticesInsidePolylines(plinesBorder);


                        // Iterate all the connected Triangles

                        var surfaceOid = TinSurface.Create(db, "AutoSurface");
                        var newSurface = trans.GetObject(surfaceOid, OpenMode.ForWrite) as TinSurface;

                        foreach (var vert in verticesInsidePline.ToList())
                        {

                            Debug.WriteLine("Before Correction " + vert.Location.Z);

                            foreach (var tri in vert.Triangles)
                            {
                                corrected = BBC.SurfOffset002.OffsetPoints.GetOffsetTriangles(tri,offset);                         
                                edgescorrected = BBC.SurfOffset002.OffsetPoints.GetOffsetTrianglesEdges(tri, offset);
                                Create3DPolyline(corrected);
                                break;
                                if (corrected.Count > 0)
                                {
                                    newSurface.AddVertices(corrected);
                                    newSurface.BreaklinesDefinition.AddStandardBreaklines(corrected, 0.5, 0.5, 1, 1);
                                }
                            }
                            break;
                        }


                        if (erasePline)
                            EraseObject(pline);
                        else
                        {
                            if (addboundary)
                                AddStandardBoundary(plineId, newSurface.Name, newSurface);
                        }

                        trans.Commit();

                        ed.CurrentUserCoordinateSystem = Matrix3d.Identity;
                        ed.Regen();
                    }

                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }
        private static bool ExceedsNormalCutoff(double angle, Vector3d normal)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ucs = doc.Editor.CurrentUserCoordinateSystem;
            var cs  = ucs.CoordinateSystem3d;

            var radians = angle*Math.PI/180;

            if (normal.GetAngleTo(cs.Zaxis) < radians)
                return false;

            Debug.WriteLine(normal.GetAngleTo(cs.Zaxis) * 180 / Math.PI);

            return true;

        }
        private static bool WithinWeedDistance(double weeddistange, Point3d centerP3D, List<Point3d> usedTriangles)
        {

            return usedTriangles.Any(p => p.DistanceTo(centerP3D) < weeddistange); 
        }

        /// <summary>
        /// Traces the surface.
        /// </summary>
        [CommandMethod("BBC-TraceSurface")]
        public static void TraceSurface()
        {
            var doc = Application.DocumentManager;
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                _featurelinecounter = 0;
                var xinfo = new List<IntersectionInfo>();
                var olducs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;
                UcsHelper.SetUCSToWorld();

                PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a TIN Surface: ");
                selSurface.SetRejectMessage("\nOnly Tin Surface allowed");
                selSurface.AddAllowedClass(typeof(TinSurface), true);
                PromptEntityResult resSurface = ed.GetEntity(selSurface);
                if (resSurface.Status != PromptStatus.OK) return;
                ObjectId surfaceId = resSurface.ObjectId;

                //Add an Horizontal Offset Distance
                PromptDoubleOptions pIntOpts = new PromptDoubleOptions("");
                pIntOpts.Message = "\nAdd an interval (horizontal) distance in feet: ";
                pIntOpts.AllowNegative = true;
                pIntOpts.AllowZero = false;

                PromptDoubleResult doubleResult = ed.GetDouble(pIntOpts);
                if (doubleResult.Status != PromptStatus.OK) return;
                _interval = doubleResult.Value;

                //Add an Vertical Offset Distance
                PromptDoubleOptions pIntOptsV = new PromptDoubleOptions("");
                pIntOptsV.Message = "\nAdd an interval (vertical) distance in feet: ";
                pIntOptsV.AllowNegative = true;
                pIntOptsV.AllowZero = false;

                PromptDoubleResult doubleResultV = ed.GetDouble(pIntOptsV);
                if (doubleResultV.Status != PromptStatus.OK) return;
                _offset = doubleResultV.Value;

                //select a polyline
                PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a Polyline Boundary: ");
                selPline.SetRejectMessage("\nOnly 2D Polylines allowed");
                selPline.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult resPline = ed.GetEntity(selPline);
                if (resPline.Status != PromptStatus.OK) return;
                ObjectId plineId = resPline.ObjectId;
                
                //erase the pline?
                PromptKeywordOptions selYesNopline = new PromptKeywordOptions("\nErase Polyline Boundary?");
                selYesNopline.Keywords.Add("Yes");
                selYesNopline.Keywords.Add("No");
                PromptResult resYesNopline = ed.GetKeywords(selYesNopline);
                if (resYesNopline.Status != PromptStatus.OK) return;
                bool erasePline = resYesNopline.StringResult.Equals("Yes");

                bool addboundary = false;
                if (!erasePline)
                {
                    //add the pline as the outer boundary?
                    PromptKeywordOptions selYesNobndy =
                        new PromptKeywordOptions("\nAdd the Polyline as the Outer Boundary?");
                    selYesNobndy.Keywords.Add("Yes");
                    selYesNobndy.Keywords.Add("No");
                    PromptResult resYesNobndy = ed.GetKeywords(selYesNobndy);
                    if (resYesNobndy.Status != PromptStatus.OK) return;
                    addboundary = resYesNobndy.StringResult.Equals("Yes");

                }


                ed.WriteMessage("\nStarting the Program!\n");

                Database db = Application.DocumentManager.MdiActiveDocument.Database;

                var site = new ObjectId(IntPtr.Zero);

                var lines = SkeletonSurf.SkeletonSurf.CreateBoundary(GetExtentsFromSurface(surfaceId), _interval);
                
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (var lineOid in lines)
                    {
                        using (Transaction trans = db.TransactionManager.StartTransaction())
                        {
                            var disposableWrapper = trans.GetObject(plineId, OpenMode.ForWrite) as Polyline;
                            if (disposableWrapper != null)
                            {
                                var pline = disposableWrapper.ConvertTo(false);

                                pline.Elevation = 0;
                           
                                var pts = new Point3dCollection();

                                var line = trans.GetObject(lineOid, OpenMode.ForWrite) as Line;

                                line.IntersectWith(pline, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                                if (pts.Count == 0) continue;

                                var ordered = OrderByDistanceFromBase(line, pts);
                            
                                for (int i=0;i < ordered.Count()-1;i++)
                                {
                                    var start = ordered.Skip(i).FirstOrDefault();
                                    var end   = ordered.Skip(i + 1).FirstOrDefault();
                                    var lline = new Line(start,end);

                                    if (IsMidPointInsidePoly(lline,plineId))
                                    { 
                                        xinfo.Add(new IntersectionInfo(start, end, line));
                                    }
                                }
                            }
                            trans.Commit();
                        }
                    }
                    tr.Commit();
                }
                
                site = CleanupZeroElevations(AddFeatureLinesToSurface(xinfo, surfaceId));
                CleanupReferenceLines(lines);
                if (addboundary)
                    AddStandardBoundary(CreateNewSurfaceAndAddFeaturelines(site), plineId);
                else
                    CreateNewSurfaceAndAddFeaturelines(site);

                UcsHelper.SetUCSToCustom(Matrix3d.Identity.CoordinateSystem3d, olducs);

            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ed.WriteMessage("error " + e.Message);
            }
        }

        [CommandMethod("BBC-TraceSurfaceManual")]
        public static void TraceSurfaceManual()
        {
            var doc = Application.DocumentManager;
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                _featurelinecounter = 0;
                var xinfo = new List<IntersectionInfo>();
                var olducs = ed.CurrentUserCoordinateSystem.CoordinateSystem3d;
                UcsHelper.SetUCSToWorld();

                PromptEntityOptions selSurface = new PromptEntityOptions("\nSelect a TIN Surface: ");
                selSurface.SetRejectMessage("\nOnly Tin Surface allowed");
                selSurface.AddAllowedClass(typeof(TinSurface), true);
                PromptEntityResult resSurface = ed.GetEntity(selSurface);
                if (resSurface.Status != PromptStatus.OK) return;
                ObjectId surfaceId = resSurface.ObjectId;

                //Add an Horizontal Offset Distance
                PromptDoubleOptions pIntOpts = new PromptDoubleOptions("");
                pIntOpts.Message = "\nAdd an interval (horizontal) distance in feet: ";
                pIntOpts.AllowNegative = true;
                pIntOpts.AllowZero = false;

                PromptDoubleResult doubleResult = ed.GetDouble(pIntOpts);
                if (doubleResult.Status != PromptStatus.OK) return;
                _interval = doubleResult.Value;

                //Add an Vertical Offset Distance
                PromptDoubleOptions pIntOptsV = new PromptDoubleOptions("");
                pIntOptsV.Message = "\nAdd an interval (vertical) distance in feet: ";
                pIntOptsV.AllowNegative = true;
                pIntOptsV.AllowZero = false;

                PromptDoubleResult doubleResultV = ed.GetDouble(pIntOptsV);
                if (doubleResultV.Status != PromptStatus.OK) return;
                _offset = doubleResultV.Value;

                //select a polyline
                PromptEntityOptions selPline = new PromptEntityOptions("\nSelect a Polyline Boundary: ");
                selPline.SetRejectMessage("\nOnly 2D Polylines allowed");
                selPline.AddAllowedClass(typeof(Polyline), true);
                PromptEntityResult resPline = ed.GetEntity(selPline);
                if (resPline.Status != PromptStatus.OK) return;
                ObjectId plineId = resPline.ObjectId;

                //erase the pline?
                PromptKeywordOptions selYesNopline = new PromptKeywordOptions("\nErase Polyline Boundary?");
                selYesNopline.Keywords.Add("Yes");
                selYesNopline.Keywords.Add("No");
                PromptResult resYesNopline = ed.GetKeywords(selYesNopline);
                if (resYesNopline.Status != PromptStatus.OK) return;
                bool erasePline = resYesNopline.StringResult.Equals("Yes");

                bool addboundary = false;
                if (!erasePline)
                {
                    //add the pline as the outer boundary?
                    PromptKeywordOptions selYesNobndy =
                        new PromptKeywordOptions("\nAdd the Polyline as the Outer Boundary?");
                    selYesNobndy.Keywords.Add("Yes");
                    selYesNobndy.Keywords.Add("No");
                    PromptResult resYesNobndy = ed.GetKeywords(selYesNobndy);
                    if (resYesNobndy.Status != PromptStatus.OK) return;
                    addboundary = resYesNobndy.StringResult.Equals("Yes");

                }


                ed.WriteMessage("\nStarting the Program!\n");

                Database db = Application.DocumentManager.MdiActiveDocument.Database;

                var site = new ObjectId(IntPtr.Zero);

                var lines = SkeletonSurf.SkeletonSurf.CreateBoundary(GetExtentsFromSurface(surfaceId), _interval);

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (var lineOid in lines)
                    {
                        using (Transaction trans = db.TransactionManager.StartTransaction())
                        {
                            var disposableWrapper = trans.GetObject(plineId, OpenMode.ForWrite) as Polyline;
                            if (disposableWrapper != null)
                            {
                                var pline = disposableWrapper.ConvertTo(false);

                                pline.Elevation = 0;

                                var pts = new Point3dCollection();

                                var line = trans.GetObject(lineOid, OpenMode.ForWrite) as Line;

                                line.IntersectWith(pline, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                                if (pts.Count == 0) continue;

                                var ordered = OrderByDistanceFromBase(line, pts);

                                for (int i = 0; i < ordered.Count() - 1; i++)
                                {
                                    var start = ordered.Skip(i).FirstOrDefault();
                                    var end = ordered.Skip(i + 1).FirstOrDefault();
                                    var lline = new Line(start, end);

                                    if (IsMidPointInsidePoly(lline, plineId))
                                    {
                                        xinfo.Add(new IntersectionInfo(start, end, line));
                                    }
                                }
                            }
                            trans.Commit();
                        }
                    }
                    tr.Commit();
                }

                site = CleanupZeroElevations(AddFeatureLinesToSurface(xinfo, surfaceId));
                CleanupReferenceLines(lines);
                if (addboundary)
                    AddStandardBoundary(CreateNewSurfaceAndAddFeaturelines(site), plineId);
                else
                    CreateNewSurfaceAndAddFeaturelines(site);

                UcsHelper.SetUCSToCustom(Matrix3d.Identity.CoordinateSystem3d, olducs);

            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ed.WriteMessage("error " + e.Message);
            }
        }

        /// <summary>
        /// Determines whether [is mid point inside poly] [the specified line].
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="pid">The pid.</param>
        /// <returns><c>true</c> if [is mid point inside poly] [the specified line]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Polyline is null!</exception>
        private static bool IsMidPointInsidePoly(Line line, ObjectId pid)
        {

            var linemid = new Point3d(((line.EndPoint.X + line.StartPoint.X) / 2),
                ((line.EndPoint.Y + line.StartPoint.Y) / 2), 0.0);

            var ptslist = new List<Point3d>();

            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                var polyline = tr.GetObject(pid, OpenMode.ForRead) as Polyline;

                if (polyline == null) throw new ArgumentNullException("Polyline is null!");

                for (int i = 0; i < polyline.NumberOfVertices; i++)
                {
                    ptslist.Add(polyline.GetPoint3dAt(i));
                }

                tr.Commit();
            }

            if (IsPointInsidePoly(ptslist, linemid))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Orders the by distance from base.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="pts">The PTS.</param>
        /// <returns>IEnumerable&lt;Point3d&gt;.</returns>
        private static IEnumerable<Point3d> OrderByDistanceFromBase(Line line, Point3dCollection pts)
        {
            var origin  = UcsHelper.GetOrigin();
            var ordered = new List<Point3d>();

            foreach (Point3d p in pts)
            {
               ordered.Add(p);    
            }

            return ordered.Where(q=>q != null).OrderBy(k => k.DistanceTo(origin));
        }


        /// <summary>
        /// Determines whether [is mid point inside poly] [the specified line].
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="polyline">The polyline.</param>
        /// <returns><c>true</c> if [is mid point inside poly] [the specified line]; otherwise, <c>false</c>.</returns>
        private static bool IsMidPointInsidePoly(Line line, Polyline polyline)
        {
            var linemid = new Point3d(((line.EndPoint.X + line.StartPoint.X)/2),
                ((line.EndPoint.Y + line.StartPoint.Y)/2), 0.0);

            var ptslist = new List<Point3d>();

            for (int j = 0; j < polyline.NumberOfVertices; j++)
            {
                ptslist.Add(polyline.GetPoint3dAt(j));
            }

            if (IsPointInsidePoly(ptslist, linemid))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is point inside poly] [the specified points].
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="test">The test.</param>
        /// <returns><c>true</c> if [is point inside poly] [the specified points]; otherwise, <c>false</c>.</returns>
        public static bool IsPointInsidePoly(List<Point3d> points, Point3d test)
        {
            int nvert = points.Count;

            int i, j;
            bool c = false;

            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {

                if (((points[i].Y > test.Y) != (points[j].Y > test.Y)) &&
                    (test.X < (points[j].X - points[i].X) * (test.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                    c = !c;
            }

            return c;
        }

        public static bool IsPointInsidePoly(ObjectId pid, Point3d test)
        {
             

            var ptslist = new List<Point3d>();

            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                var polyline = tr.GetObject(pid, OpenMode.ForRead) as Polyline;

                if (polyline == null) throw new ArgumentNullException("Polyline is null!");

                for (int i = 0; i < polyline.NumberOfVertices; i++)
                {
                    ptslist.Add(polyline.GetPoint3dAt(i));
                }

                tr.Commit();
            }

            if (IsPointInsidePoly(ptslist, test))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the standard boundary.
        /// </summary>
        /// <param name="surId">The sur identifier.</param>
        /// <param name="plineId">The pline identifier.</param>
        private static void AddStandardBoundary(ObjectId surId, ObjectId plineId)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                Database db = Application.DocumentManager.MdiActiveDocument.Database;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {

                    var surface = trans.GetObject(surId, OpenMode.ForWrite) as TinSurface;

                    var boundaryEntities = GetObjectIdCollectionFromEntity(plineId);

                    var surfaceBoundaries = surface.BoundariesDefinition;


                    surfaceBoundaries.AddBoundaries(boundaryEntities, 0.5, SurfaceBoundaryType.Outer, true);

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                ed.WriteMessage("AddStandardBoundary: " + (ex.Message));
            }
        }

        /// <summary>
        /// Creates the new surface and add featurelines.
        /// </summary>
        /// <param name="siteObjectId">The site object identifier.</param>
        /// <returns>ObjectId.</returns>
        private static ObjectId CreateNewSurfaceAndAddFeaturelines(ObjectId siteObjectId)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
            var surId = new ObjectId(IntPtr.Zero);
            try
            {

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    var featurelines = FilterFeatureLines(GetSurfaceEntityIDs(), siteObjectId);
                    if (featurelines.Count == 0) return ObjectId.Null;
                    surId = TinSurface.Create(db, "AutoSurface");
                    var offsetFeaturelines = AddNewSetOfFeatureLinesWithOffsetOnSkew(featurelines, _offset, GetNewSiteId());
                    if (offsetFeaturelines.Count != 0)
                        AddFeatureLinesToSurface(offsetFeaturelines, surId, _sampledist);

                    tr.Commit();

                    return surId;
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ed.WriteMessage("error " + e.Message);
            }
            return surId;

        }

        /// <summary>
        /// Adds the feature lines to surface.
        /// </summary>
        /// <param name="featureIds">The feature ids.</param>
        /// <param name="surfaceId">The surface identifier.</param>
        /// <param name="sampledist">The sampledist.</param>
        public static void AddFeatureLinesToSurface(ObjectIdCollection featureIds, ObjectId surfaceId, double sampledist)
        {
            try
            {
                Debug.WriteLine("Starting AddFeatureLinesToAllSurface");
                using (Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {

                        TinSurface surface = surfaceId.GetObject(OpenMode.ForRead) as TinSurface;
                        if (surface != null)
                        {
                            surface.UpgradeOpen();
                            surface.BreaklinesDefinition.AddStandardBreaklines(featureIds, 1.0, sampledist, 0.0, 0.0);
                            surface.DowngradeOpen();
                        }

                        tr.Commit();
                        Debug.WriteLine("Ending AddFeatureLinesToAllSurface");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error AddFeatureLinesToAllSurface " +ex.Message);
            }
        }

        /// <summary>
        /// Adds the new set of feature lines with offset.
        /// </summary>
        /// <param name="featurelines">The featurelines.</param>
        /// <param name="vertoffset">The vertoffset.</param>
        /// <param name="getNewSiteId">The get new site identifier.</param>
        /// <returns>ObjectIdCollection.</returns>
        private static ObjectIdCollection AddNewSetOfFeatureLinesWithOffset(List<ObjectId> featurelines, double vertoffset, ObjectId getNewSiteId)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            var outfeaturelines = new ObjectIdCollection(); 
            foreach (var oid in featurelines)
            {
                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        var oldFeatureline = tr.GetObject(oid, OpenMode.ForWrite) as FeatureLine;
                        if (oldFeatureline != null)
                        {
                            var new3DPolyid = Create3DPolyline(oldFeatureline.GetPoints(FeatureLinePointType.AllPoints));
                            var newfeatulineId = FeatureLine.Create("AutoFeatureLine-" + GetNextRandom(new Random(1000), _featurelinecounter++), new3DPolyid,
                                getNewSiteId);
                            var newFeatureline = tr.GetObject(newfeatulineId, OpenMode.ForWrite) as FeatureLine;

                            using (var tran = db.TransactionManager.StartTransaction())

                            {
                                if (newFeatureline != null)
                                {
                                    var points = newFeatureline.GetPoints(FeatureLinePointType.AllPoints);

                                    for (int i = 0; i < points.Count; i++)
                                    {
                                        try
                                        {
                                            newFeatureline.SetPointElevation(i, points[i].Z + _offset);

                                        }
                                        catch (ArgumentOutOfRangeException ex)
                                        {
                                            Debug.WriteLine("Argument Exception " + ex.Message);
                                        }
                                    }
                                }
                                tran.Commit();
                            }
                            outfeaturelines.Add(newfeatulineId);

                            tr.Commit();

                            CleanupReference3DPoly(new3DPolyid);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("AddNewSetOfFeatureLinesWithOffset " + ex.Message);
                }
            }
            return outfeaturelines;
        }

        private static ObjectIdCollection AddNewSetOfFeatureLinesWithOffsetOnSkew(List<ObjectId> featurelines, double vertoffset, ObjectId getNewSiteId)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
            using (var outfeaturelines = new ObjectIdCollection())
            {
                foreach (var oid in featurelines)
                {
                    try
                    {
                        using (Transaction tr  = db.TransactionManager.StartTransaction())
                        {
                            var oldFeatureline = tr.GetObject(oid, OpenMode.ForWrite) as FeatureLine;
                            if (oldFeatureline != null)
                            {
                                var new3DPolyid    = Create3DPolyline(oldFeatureline.GetPoints(FeatureLinePointType.AllPoints));
                                var newfeatulineId = FeatureLine.Create("AutoFeatureLine-" + GetNextRandom(new Random(1000), _featurelinecounter++), new3DPolyid,
                                    getNewSiteId);
                                var newFeatureline = tr.GetObject(newfeatulineId, OpenMode.ForWrite) as FeatureLine;

                                using (var tran = db.TransactionManager.StartTransaction())

                                {
                                    if (newFeatureline != null)
                                    {
                                        var points = newFeatureline.GetPoints(FeatureLinePointType.PIPoint);

                                        for (int i = 0; i < points.Count; i++)
                                        {
                                            var j = (i == points.Count - 1) ? i - 1 : i + 1;

                                            try
                                            {
                                                newFeatureline.SetPointElevation(i, points[i].Z + GetSkew(vertoffset, points[i], points[j]));
                                            }
                                            catch (ArgumentOutOfRangeException ex)
                                            {
                                                Debug.WriteLine("Argument Exception " + ex.Message);
                                            }
                                        }
                                    }
                                    tran.Commit();
                                }
                                outfeaturelines.Add(newfeatulineId);

                                tr.Commit();

                                CleanupReference3DPoly(new3DPolyid);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("AddNewSetOfFeatureLinesWithOffset " + ex.Message);
                    }
                }
                return outfeaturelines;
            }
        }

        private static double GetSkew(double offset, Point3d point3D, Point3d point3D1)
        {          
            var deltaZ  = point3D1.Z - point3D.Z;
            var deltaXy = point3D.DistanceTo(point3D1);

            var angle      = Math.Atan(deltaZ/deltaXy);
            var adjustment = Math.Abs(offset*Math.Cos(angle));

            return Math.Abs((offset - adjustment) + offset); 
        }

        /// <summary>
        /// Cleanups the reference3d poly.
        /// </summary>
        /// <param name="lid">The lid.</param>
        private static void CleanupReference3DPoly(ObjectId lid)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                // Get the current document and database
                Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;

                Database acCurDb = acDoc.Database;

                // Start a transaction
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    var acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;
                    var toErase = acTrans.GetObject(lid, OpenMode.ForWrite) as Polyline3d;

                    if (toErase == null) return;

                    // Erase the line from the drawing
                    toErase.UpgradeOpen();

                    toErase.Erase(true);

                    // Save the new object to the database
                    acTrans.Commit();
                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        /// <summary>
        /// Create3s the d polyline.
        /// </summary>
        /// <param name="getPoints">The get points.</param>
        /// <returns>ObjectId.</returns>
        private static ObjectId Create3DPolyline(Point3dCollection getPoints)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                // Get the current document and database
                Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                ObjectId polyline;


                // Start a transaction
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    Polyline3d p3D = new Polyline3d();

                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;


                    // Add the new object to the block table record and the transaction
                    polyline = acBlkTblRec.AppendEntity(p3D);
                    acTrans.AddNewlyCreatedDBObject(p3D, true);


                    // Save the new object to the database
                    acTrans.Commit();
                }

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    var p3D = acTrans.GetObject(polyline, OpenMode.ForWrite) as Polyline3d;

                    if (p3D != null)

                    {
                        foreach (Point3d v3D in getPoints)

                        {
                            p3D.AppendVertex(new PolylineVertex3d(v3D));
                        }
                    }
                    acTrans.Commit();
                }

                //Check the Geometry and close the Polyline3d
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    var p3D = acTrans.GetObject(polyline, OpenMode.ForWrite) as Polyline3d;

                    if (p3D != null)

                    {
                        // Use foreach to get each contained vertex

                        foreach (ObjectId vId in p3D)

                        {

                            PolylineVertex3d v3D =

                                (PolylineVertex3d)acTrans.GetObject(

                                    vId,

                                    OpenMode.ForRead

                                );
                        }
                        p3D.Closed = false;
                    }
                    acTrans.Commit();
                }
                return polyline;
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }

            return new ObjectId();
        }
        /// <summary>
        /// Filters the feature lines.
        /// </summary>
        /// <param name="getSurfaceEntityIDs">The get surface entity i ds.</param>
        /// <param name="siteObjectId">The site object identifier.</param>
        /// <returns>List&lt;ObjectId&gt;.</returns>
        private static List<ObjectId> FilterFeatureLines(List<ObjectId> getSurfaceEntityIDs, ObjectId siteObjectId)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;

            var filterlines = new List<ObjectId>();
            foreach (ObjectId oid in getSurfaceEntityIDs)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    var featureline = tr.GetObject(oid, OpenMode.ForWrite) as FeatureLine;
                    var disposableWrapper = tr.GetObject(siteObjectId, OpenMode.ForRead) as Site;
                    if (disposableWrapper != null)
                    {
                        var osite = disposableWrapper.Name;
                        var wrapper = tr.GetObject(featureline.SiteId, OpenMode.ForRead) as Site;
                        if (wrapper != null)
                        {
                            var nsite = wrapper.Name;
                            if (osite.Equals(nsite))
                            {
                                filterlines.Add(oid);
                            }
                        }
                    }
                }
            }

            return filterlines;
        }

        /// <summary>
        /// Cleanups the zero elevations.
        /// </summary>
        /// <param name="sitObjectId">The sit object identifier.</param>
        /// <returns>ObjectId.</returns>
        private static ObjectId CleanupZeroElevations(ObjectId sitObjectId)
        {
            var db    = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.CurrentDocument.Database;

            var featureLinesIds = GetSurfaceEntityIDs();

            foreach (ObjectId fId in featureLinesIds)
            {
                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var featureline = tran.GetObject(fId, OpenMode.ForWrite) as FeatureLine;

                    if (featureline != null && featureline.SiteId != sitObjectId) continue;

                    if (featureline != null && Math.Abs(featureline.MinElevation) < 0.01)
                    {
                        var points = featureline.GetPoints(FeatureLinePointType.AllPoints);

                        var last = points.Count - 1;
                        for(int i = 0; i < points.Count; i++)
                        {

                            if (Math.Abs(points[i].Z) < 0.01)
                            {
                                if (i == last)
                                   featureline.SetPointElevation(i,points[i-1].Z);
                                featureline.SetPointElevation(i, points[i + 1].Z);
                            }    
                        }
                    }

                    tran.Commit();
                }
            }
            return sitObjectId;
        }

        /// <summary>
        /// Gets the surface entity i ds.
        /// </summary>
        /// <returns>List&lt;ObjectId&gt;.</returns>
        public static List<ObjectId> GetSurfaceEntityIDs()

        {
            List<ObjectId> ids = null;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            var db = Application.DocumentManager.CurrentDocument.Database;
            try
            {
                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (BlockTable)tran.GetObject(db.BlockTableId, OpenMode.ForRead);

                    var br =
                        (BlockTableRecord)tran.GetObject(tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                    var b = br.Cast<ObjectId>();

                    #region Other Types

                    //==============search certain entity========================//

                    //"LINE" for line

                    //"LWPOLYLINE" for polyline

                    //"CIRCLE" for circle

                    //"INSERT" for block reference

                    //...

                    //We can use "||" (or) to search for more then one entity types

                    //============================================================//


                    //Use lambda extension method

                    //ids = b.Where(id => id.ObjectClass.DxfName.ToUpper() == "LINE" ||

                    //    id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE").ToList<ObjectId>();

                    #endregion

                    ids = (from id in b
                           where id.ObjectClass.DxfName.ToUpper() == "AECC_FEATURE_LINE"
                           select id).ToList();


                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);

            }

            return ids;
        }

        /// <summary>
        /// Cleanups the reference lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        private static void CleanupReferenceLines(List<ObjectId> lines)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                // Get the current document and database
                Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;

                Database acCurDb = acDoc.Database;

                foreach (var lid in lines)
                {
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        // Open the Block table for read
                        BlockTable acBlkTbl;
                        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                            OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        var acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                            OpenMode.ForWrite) as BlockTableRecord;
                        var toErase = acTrans.GetObject(lid, OpenMode.ForWrite) as Line;

                        if (toErase == null) continue;

                        // Erase the line from the drawing
                        toErase.UpgradeOpen();

                        toErase.Erase(true);

                        // Save the new object to the database
                        acTrans.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }
        /// <summary>
        /// Cleanups the reference3d poly.
        /// </summary>
        /// <param name="lines">The lines.</param>
        private static void CleanupReference3DPoly(List<ObjectId> lines)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                // Get the current document and database
                Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;

                Database acCurDb = acDoc.Database;

                foreach (var lid in lines)
                {
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        // Open the Block table for read
                        BlockTable acBlkTbl;
                        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                            OpenMode.ForRead) as BlockTable;

                        // Open the Block table record Model space for write
                        var acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                            OpenMode.ForWrite) as BlockTableRecord;
                        var toErase = acTrans.GetObject(lid, OpenMode.ForWrite) as Polyline3d;

                        if (toErase == null) continue;

                        // Erase the line from the drawing
                        toErase.UpgradeOpen();

                        toErase.Erase(true);

                        // Save the new object to the database
                        acTrans.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the new site identifier.
        /// </summary>
        /// <returns>ObjectId.</returns>
        public static ObjectId GetNewSiteId()
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.CurrentDocument.Editor;

            var site = ObjectId.Null;
            Random rand = new Random();
            try
            {
                using (Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.CurrentDocument.Database;

                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        var num = rand.Next(10000, 99999).ToString();

                        var doc = CivilDocument.GetCivilDocument(db);

                        site = Site.Create(doc, "AutoSite-" + num);

                        trans.Commit();
                    }
                }
            }
      
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error: " + ex.Message);
            }
            return site;
        }

        /// <summary>
        /// Adds the feature lines to surface.
        /// </summary>
        /// <param name="xinfo">The xinfo.</param>
        /// <param name="surfaceId">The surface identifier.</param>
        /// <returns>ObjectId.</returns>
        private static ObjectId AddFeatureLinesToSurface(List<IntersectionInfo> xinfo, ObjectId surfaceId)
        {
            Editor ed   = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.CurrentDocument.Editor;
            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.CurrentDocument.Database;
            var toErase = new List<ObjectId>();
            var site = new ObjectId(IntPtr.Zero);
            try
            {
                site = GetNewSiteId();
                foreach (var info in xinfo)
                {

                    try
                    {
                        using (Transaction trans = db.TransactionManager.StartTransaction())
                        {
                            TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

                            if (surface == null)
                                break;
                            var clippedLine = SkeletonSurf.SkeletonSurf.CreateLine(info.Intersec1, info.Intersec2);
                            var featurelineId = FeatureLine.Create("AutoFeatureLine-" + GetNextRandom(new Random(1000), _featurelinecounter++), clippedLine, site);

                            var featureline = trans.GetObject(featurelineId, OpenMode.ForWrite) as FeatureLine;

                            if (featureline != null) featureline.AssignElevationsFromSurface(surfaceId, true);
                            trans.Commit();

                            CleanupReferenceLines(clippedLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        ed.WriteMessage("Error: " + ex.Message);
                    }
                }    
                CleanupReferenceLines(toErase);                
                return site;
            }
            catch (System.ArgumentException ex)
            {
                ed.WriteMessage("Site or Featureline Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                ed.WriteMessage("Error: " + ex.Message);
                
            }
            return site;
        }

        private static long GetNextRandom(Random random, int i)
        {
            return  (i + DateTime.Now.Millisecond + random.Next(1000, 99999));
        }

        /// <summary>
        /// Cleanups the reference lines.
        /// </summary>
        /// <param name="clippedLine">The clipped line.</param>
        private static void CleanupReferenceLines(ObjectId clippedLine)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                // Get the current document and database
                Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;

                Database acCurDb = acDoc.Database;


                // Start a transaction
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    var acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;
                    var toErase = acTrans.GetObject(clippedLine, OpenMode.ForWrite) as Line;

                    if (toErase == null) return;

                    // Erase the line from the drawing
                    toErase.UpgradeOpen();

                    toErase.Erase(true);

                    // Save the new object to the database
                    acTrans.Commit();
                }

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the triangle translation offset.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Point3d.</returns>
        private static Point3d GetTriangleTranslationOffset(Point3d vertex, Vector3d normal, double offset)
        {
            return new Point3d(normal.X + vertex[0],
                normal.Y + vertex[1],
                offset + vertex[2]);
        }

        /// <summary>
        /// Gets the point3 d.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>Point3d.</returns>
        private static Point3d GetPoint3D(Point3d vertex)
        {
            return new Point3d(vertex.X, vertex.Y, vertex.Z);
        }

        /// <summary>
        /// Gets the object identifier collection from entity.
        /// </summary>
        /// <param name="polyId">The poly identifier.</param>
        /// <returns>ObjectIdCollection.</returns>
        public static ObjectIdCollection GetObjectIdCollectionFromEntity(ObjectId polyId)
        {
            ObjectIdCollection entities = new ObjectIdCollection();
            entities.Add(polyId);
            return entities;
        }

        /// <summary>
        /// Adds the standard boundary.
        /// </summary>
        /// <param name="polyId">The poly identifier.</param>
        /// <param name="surfacename">The surfacename.</param>
        /// <param name="surface">The surface.</param>
        public static void AddStandardBoundary(ObjectId polyId, string surfacename,
            TinSurface surface)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {

                    ObjectIdCollection boundaryEntities;
                    boundaryEntities = GetObjectIdCollectionFromEntity(polyId);

                    var surfaceBoundaries =
                        surface.BoundariesDefinition;

                    surfaceBoundaries.AddBoundaries(boundaryEntities, 0.2, SurfaceBoundaryType.Outer, true);

                    trans.Commit();
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("AddStandardBoundary: " + (ex.Message));
            }
        }

        /// <summary>
        /// Gets the triangles from surface.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <returns>TinSurfaceTriangleCollection.</returns>
        public static TinSurfaceTriangleCollection GetTrianglesFromSurface(TinSurface surface)
        {
            var db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            TinSurfaceTriangleCollection triangles;
            try
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    triangles = surface.GetTriangles(true);
                    trans.Commit();
                }
                return triangles;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                ed.WriteMessage("Error " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// Erases the object.
        /// </summary>
        /// <param name="acPoly">The ac poly.</param>
        public static void EraseObject(Polyline acPoly)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;
            try
            {
                // Get the current document and database
                Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;

                Database acCurDb = acDoc.Database;

                // Start a transaction
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    var acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite) as BlockTableRecord;
                    var acPolyToErase = acTrans.GetObject(acPoly.ObjectId, OpenMode.ForWrite) as Polyline;
                    // Erase the polyline from the drawing
                    acPoly.UpgradeOpen();

                    acPoly.Erase(true);

                    // Save the new object to the database
                    acTrans.Commit();
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("Error " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the extents from surface.
        /// </summary>
        /// <param name="surfObjectId">The surf object identifier.</param>
        /// <returns>Extents3d.</returns>
        public static Extents3d GetExtentsFromSurface(ObjectId surfObjectId)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var extents = new Extents3d();
            try
            {
                Database db = Application.DocumentManager.MdiActiveDocument.Database;

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {

                    var surface = trans.GetObject(surfObjectId, OpenMode.ForRead) as TinSurface;
                    return extents = surface.GeometricExtents;
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("AddStandardBoundary: " + (ex.Message));
            }
            return extents;
        }

        /// <summary>
        /// Gets the intersection points.
        /// </summary>
        /// <param name="poly">The poly.</param>
        /// <param name="lineObjectId">The line object identifier.</param>
        /// <returns>IntersectionInfo.</returns>
        public static IntersectionInfo GetIntersectionPoints(Polyline poly, ObjectId lineObjectId)
        {
            var db = Application.DocumentManager.CurrentDocument.Database;
            var pts = new Point3dCollection();

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var line = tr.GetObject(lineObjectId,OpenMode.ForWrite) as Line;

                line.IntersectWith(poly, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                if (pts.Count > 2 || pts.Count == 0)
                    return null;
                return new IntersectionInfo(pts[0], pts[1], line);

            }

        }

        //public class Commands
        //{
        //    [CommandMethod("SURFRAY")]
        //    public void SurfaceRay()
        //    {
        //        var doc = Application.DocumentManager.MdiActiveDocument;
        //        var db = doc.Database;
        //        var ed = doc.Editor;

        //        // Ask the user to pick a surface

        //        var peo = new PromptEntityOptions("\nSelect a surface");
        //        peo.SetRejectMessage("\nMust be a surface.");
        //        peo.AddAllowedClass(typeof(TinSurface), false);

        //        var per = ed.GetEntity(peo);
        //        if (per.Status != PromptStatus.OK)
        //            return;

        //        var sf =
        //          new SelectionFilter(
        //            new TypedValue[] {
        //    new TypedValue((int)DxfCode.Start, "LINE")
        //            }
        //          );

        //        // Ask the user to select some lines

        //        var pso = new PromptSelectionOptions();
        //        pso.MessageForAdding = "Select lines";
        //        pso.MessageForRemoval = "Remove lines";

        //        var psr = ed.GetSelection(pso, sf);
        //        if (psr.Status == PromptStatus.OK && psr.Value.Count > 0)
        //        {
        //            using (var tr = db.TransactionManager.StartTransaction())
        //            {
        //                // We'll start by getting the block table and modelspace
        //                // (which are really only needed for results we'll add to
        //                // the database)

        //                var bt =
        //                  (BlockTable)tr.GetObject(
        //                    db.BlockTableId, OpenMode.ForRead
        //                  );

        //                var btr =
        //                  (BlockTableRecord)tr.GetObject(
        //                    bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite
        //                  );

        //                // Next we'll get our surface object

        //                var obj = tr.GetObject(per.ObjectId, OpenMode.ForRead);
        //                var surf = obj as TinSurface;
        //                if (surf == null)
        //                {
        //                    // Should never happen, but anyway

        //                    ed.WriteMessage("\nFirst object must be a surface.");
        //                }
        //                else
        //                {
        //                    DoubleCollection heights;
        //                    SubentityId[] ids;

        //                    // Fire ray for each selected line

        //                    foreach (var id in psr.Value.GetObjectIds())
        //                    {
        //                        var ln = tr.GetObject(id, OpenMode.ForRead) as Line;
        //                        if (ln != null)
        //                        {
        //                            surf.RayTest(
        //                              ln.StartPoint,
        //                              ln.StartPoint.GetVectorTo(ln.EndPoint),
        //                              0.01,
        //                              out ids,
        //                              out heights
        //                            );

        //                            if (ids.Length == 0)
        //                            {
        //                                ed.WriteMessage("\nNo intersections found.");
        //                            }
        //                            else
        //                            {
        //                                // Highlight each subentity and add point
        //                                // at intersection

        //                                for (int i = 0; i < ids.Length; i++)
        //                                {
        //                                    var subEntityPath =
        //                                      new FullSubentityPath(
        //                                        new ObjectId[] { per.ObjectId },
        //                                        ids[i]
        //                                      );

        //                                    // Highlight the sub entity

        //                                    surf.Highlight(subEntityPath, true);

        //                                    // Create a point at the line-surface
        //                                    // intersection

        //                                    var pt =
        //                                      new DBPoint(
        //                                        new Point3d(
        //                                          ln.StartPoint.X,
        //                                          ln.StartPoint.Y,
        //                                          heights[i]
        //                                        )
        //                                      );

        //                                    // Add the new object to the block table
        //                                    // record and the transaction

        //                                    btr.AppendEntity(pt);
        //                                    tr.AddNewlyCreatedDBObject(pt, true);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                tr.Commit();
        //            }
        //        }
        //    }

        //    [CommandMethod("POXY")]
        //    public static void PointOnXY()
        //    {
        //        var doc = Application.DocumentManager.MdiActiveDocument;
        //        var db = doc.Database;
        //        var ed = doc.Editor;

        //        // Ask the user to pick a surface

        //        var peo = new PromptEntityOptions("\nSelect a surface");
        //        peo.SetRejectMessage("\nMust be a surface.");
        //        peo.AddAllowedClass(typeof(AcDb.Surface), false);

        //        var per = ed.GetEntity(peo);
        //        if (per.Status != PromptStatus.OK)
        //            return;

        //        using (var tr = db.TransactionManager.StartTransaction())
        //        {
        //            // We'll start by getting the block table and modelspace
        //            // (which are really only needed for results we'll add to
        //            // the database)

        //            var bt =
        //              (BlockTable)tr.GetObject(
        //                db.BlockTableId, OpenMode.ForRead
        //              );

        //            var btr =
        //              (BlockTableRecord)tr.GetObject(
        //                bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite
        //              );

        //            var obj = tr.GetObject(per.ObjectId, OpenMode.ForRead);
        //            var surf = obj as AcDb.Surface;
        //            if (surf == null)
        //            {
        //                ed.WriteMessage("\nFirst object must be a surface.");
        //            }
        //            else
        //            {
        //                DoubleCollection heights;
        //                SubentityId[] ids;

        //                // Fire ray for each selected line

        //                var ucs = ed.CurrentUserCoordinateSystem;
        //                PromptPointResult ppr;
        //                do
        //                {
        //                    ppr = ed.GetPoint("\nSelect point beneath surface");
        //                    if (ppr.Status != PromptStatus.OK)
        //                    {
        //                        break;
        //                    }

        //                    // Our selected point needs to be transformed to WCS
        //                    // from the current UCS

        //                    var start = ppr.Value.TransformBy(ucs);

        //                    // Fire a ray from the selected point in the direction
        //                    // of the UCS' Z-axis

        //                    surf.RayTest(
        //                      start,
        //                      ucs.CoordinateSystem3d.Zaxis,
        //                      0.01,
        //                      out ids,
        //                      out heights
        //                    );

        //                    if (ids.Length == 0)
        //                    {
        //                        ed.WriteMessage("\nNo intersections found.");
        //                    }
        //                    else
        //                    {
        //                        // Add point at each intersection found

        //                        for (int i = 0; i < ids.Length; i++)
        //                        {
        //                            // Create a point at the intersection

        //                            var end =
        //                              start +
        //                              new Vector3d(0, 0, heights[i]).TransformBy(ucs);

        //                            var pt = new DBPoint(end);
        //                            var ln = new Line(start, end);

        //                            // Add the new objects to the block table
        //                            // record and the transaction

        //                            btr.AppendEntity(pt);
        //                            tr.AddNewlyCreatedDBObject(pt, true);
        //                            btr.AppendEntity(ln);
        //                            tr.AddNewlyCreatedDBObject(ln, true);
        //                        }
        //                    }
        //                }
        //                while (ppr.Status == PromptStatus.OK);
        //            }
        //            tr.Commit();
        //        }
        //    }
        //}
    }


    /// <summary>
    /// Class SurfaceTriangle.
    /// </summary>
    public class SurfaceTriangle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceTriangle" /> class.
        /// </summary>
        /// <param name="vx1">The VX1.</param>
        /// <param name="vx2">The VX2.</param>
        /// <param name="vx3">The VX3.</param>
        public SurfaceTriangle(Point3d vx1, Point3d vx2, Point3d vx3)
        {
            Vertex1 = vx1;
            Vertex2 = vx2;
            Vertex3 = vx3;
        }

        /// <summary>
        /// Gets or sets the vertex1.
        /// </summary>
        /// <value>The vertex1.</value>
        public Point3d Vertex1 { get; set; }
        /// <summary>
        /// Gets or sets the vertex2.
        /// </summary>
        /// <value>The vertex2.</value>
        public Point3d Vertex2 { get; set; }
        /// <summary>
        /// Gets or sets the vertex3.
        /// </summary>
        /// <value>The vertex3.</value>
        public Point3d Vertex3 { get; set; }
    }

    /// <summary>
    /// Class SurfaceTriangleToMathNETFormat.
    /// </summary>
    public class SurfaceTriangleToMathNetFormat
    {
        private List<SurfaceTriangleToMathNetFormat> mathnetformat;

        public SurfaceTriangleToMathNetFormat(List<SurfaceTriangleToMathNetFormat> mathnetformat)
        {
            this.mathnetformat = mathnetformat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceTriangleToMathNetFormat" /> class.
        /// </summary>
        /// <param name="vx1">The VX1.</param>
        /// <param name="vx2">The VX2.</param>
        /// <param name="vx3">The VX3.</param>
        public SurfaceTriangleToMathNetFormat(Point3d vx1, Point3d vx2, Point3d vx3)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> v1 = new DenseVector(new[] {vx1.X, vx1.Y, vx1.Z});
            MathNet.Numerics.LinearAlgebra.Vector<double> v2 = new DenseVector(new[] {vx2.X, vx2.Y, vx2.Z});
            MathNet.Numerics.LinearAlgebra.Vector<double> v3 = new DenseVector(new[] {vx3.X, vx3.Y, vx3.Z});

            Vertex1 = v1;
            Vertex2 = v2;
            Vertex3 = v3;
        }

        public SurfaceTriangleToMathNetFormat(TinSurfaceVertex vx1, TinSurfaceVertex vx2, TinSurfaceVertex vx3)
        {
            MathNet.Numerics.LinearAlgebra.Vector<double> v1 = new DenseVector(new[] { vx1.Location.X, vx1.Location.Y, vx1.Location.Z });
            MathNet.Numerics.LinearAlgebra.Vector<double> v2 = new DenseVector(new[] { vx2.Location.X, vx2.Location.Y, vx2.Location.Z });
            MathNet.Numerics.LinearAlgebra.Vector<double> v3 = new DenseVector(new[] { vx3.Location.X, vx3.Location.Y, vx3.Location.Z });

            Vertex1 = v1;
            Vertex2 = v2;
            Vertex3 = v3;
        }

        /// <summary>
        /// Gets or sets the vertex1.
        /// </summary>
        /// <value>The vertex1.</value>
        public Vector<double> Vertex1 { get; set; }
        /// <summary>
        /// Gets or sets the vertex2.
        /// </summary>
        /// <value>The vertex2.</value>
        public Vector<double> Vertex2 { get; set; }
        /// <summary>
        /// Gets or sets the vertex3.
        /// </summary>
        /// <value>The vertex3.</value>
        public Vector<double> Vertex3 { get; set; }
    }

    /// <summary>
    /// Class MathNETToSurfaceTriangleFormat.
    /// </summary>
    public class MathNetToSurfaceTriangleFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathNetToSurfaceTriangleFormat" /> class.
        /// </summary>
        /// <param name="vx1">The VX1.</param>
        /// <param name="vx2">The VX2.</param>
        /// <param name="vx3">The VX3.</param>
        public  MathNetToSurfaceTriangleFormat(MathNet.Numerics.LinearAlgebra.Vector<double> vx1,
            MathNet.Numerics.LinearAlgebra.Vector<double> vx2, MathNet.Numerics.LinearAlgebra.Vector<double> vx3)
        {
            var v1 = new Point3d(vx1[0], vx1[1], vx1[2]);
            var v2 = new Point3d(vx2[0], vx2[1], vx2[2]);
            var v3 = new Point3d(vx3[0], vx3[1], vx3[2]);

            Vertex1 = v1;
            Vertex2 = v2;
            Vertex3 = v3;
        }

        /// <summary>
        /// Gets or sets the vertex1.
        /// </summary>
        /// <value>The vertex1.</value>
        public Point3d Vertex1 { get; set; }
        /// <summary>
        /// Gets or sets the vertex2.
        /// </summary>
        /// <value>The vertex2.</value>
        public Point3d Vertex2 { get; set; }
        /// <summary>
        /// Gets or sets the vertex3.
        /// </summary>
        /// <value>The vertex3.</value>
        public Point3d Vertex3 { get; set; }
    }

    /// <summary>
    /// Class MathNETToPoint3DFormat.
    /// </summary>
    public  class MathNetToPoint3DFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathNetToPoint3DFormat" /> class.
        /// </summary>
        /// <param name="vx1">The VX1.</param>
        public MathNetToPoint3DFormat(MathNet.Numerics.LinearAlgebra.Vector<double> vx1)
        {
            var v1 = new Point3d(vx1[0], vx1[1], vx1[2]);

            Vertex1 = v1;
        }

        /// <summary>
        /// Gets or sets the vertex1.
        /// </summary>
        /// <value>The vertex1.</value>
        public Point3d Vertex1 { get; set; }
    }

    /// <summary>
    /// Class IntersectionInfo.
    /// </summary>
    public class IntersectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectionInfo" /> class.
        /// </summary>
        /// <param name="pnt1">The PNT1.</param>
        /// <param name="pnt2">The PNT2.</param>
        /// <param name="line">The line.</param>
        public IntersectionInfo(Point3d pnt1, Point3d pnt2, Line line)
        {
            Intersec1 = pnt1;
            Intersec2 = pnt2;
            Line = line;
        }
        /// <summary>
        /// Gets or sets the intersec1.
        /// </summary>
        /// <value>The intersec1.</value>
        public Point3d Intersec1 { get; set; }
        /// <summary>
        /// Gets or sets the intersec2.
        /// </summary>
        /// <value>The intersec2.</value>
        public Point3d Intersec2 { get; set; }
        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>The line.</value>
        public Line Line { get; set; }
    }
}

namespace EditorExtentions
{
    /// <summary>
    /// Class EditorExtension.
    /// </summary>
    public static class EditorExtension
    {
        /// <summary>
        /// Sets the z axis ucs.
        /// </summary>
        /// <param name="ed">The ed.</param>
        /// <param name="basePoint">The base point.</param>
        /// <param name="positiveZaxisPoint">The positive zaxis point.</param>
        public static void SetZAxisUcs(this Editor ed, Point3d basePoint, Point3d positiveZaxisPoint)
        {
            Plane plane = new Plane(basePoint, basePoint.GetVectorTo(positiveZaxisPoint));
            Matrix3d ucs = Matrix3d.PlaneToWorld(plane);
            ed.CurrentUserCoordinateSystem = ucs;
        }
        /// <summary>
        /// Gets the z axis ucs.
        /// </summary>
        /// <param name="ed">The ed.</param>
        /// <param name="basePoint">The base point.</param>
        /// <param name="positiveZaxisPoint">The positive zaxis point.</param>
        /// <returns>CoordinateSystem3d.</returns>
        public static CoordinateSystem3d GetZAxisUcs(this Editor ed, Point3d basePoint, Point3d positiveZaxisPoint)
        {
            Plane plane = new Plane(basePoint, basePoint.GetVectorTo(positiveZaxisPoint));
            Matrix3d ucs = Matrix3d.PlaneToWorld(plane);
            var cs = ucs.CoordinateSystem3d;
            ed.WriteMessage(String.Format("\nX={0},Y={1},Z={2}", cs.Xaxis,cs.Yaxis,cs.Zaxis));

            return ucs.CoordinateSystem3d;
        }
    }

}

