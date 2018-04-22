using System;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace BBC.AutoCAD.Trans
{
    public class Commands
    {      
        [CommandMethod("BBC-TRANSLATE",CommandFlags.UsePickSet)]
        public static void Translate()
        {
            // move first point to second point

            var point1 = new Point3d(1, 0, 0);
            var point2 = new Point3d(2, 0, 0);

            // ******************************* //

            var vec = point1.GetVectorTo(point2);

            var mat = Matrix3d.Displacement(vec);

            Program.CreatePoint(point1);
            Program.CreatePoint(point2);

            Program.CreatePoint(point1.TransformBy(mat));

        }

        /// <summary>
        /// Rotates this instance.
        /// </summary>
        [CommandMethod("BBC-ROTATE", CommandFlags.UsePickSet)]
        public static void Rotate()
        {
            // move first point to second point

            var start1 = new Point3d(1, 0, 0);
            var end1   = new Point3d(2, 0, 0);

            var start2 = new Point3d(1, 0, 0);
            var end2   = new Point3d(2, 2, 0);

            var vec1 = start1.GetVectorTo(end1);
            var vec2 = start2.GetVectorTo(end2);

            var ang = vec1.GetAngleTo(vec2) * 180.0 / Math.PI;

            var mat = Matrix3d.Rotation(vec1.GetAngleTo(vec2), new Vector3d(0, 0, 1), start1 );

            Program.CreatePoint(start1);
            Program.CreatePoint(start2);

            Program.CreatePoint(end1);
            Program.CreatePoint(end2);

            Program.CreatePoint(start1.TransformBy(mat));
           // Program.CreatePoint(start2.TransformBy(mat));
            Program.CreatePoint(  end1.TransformBy(mat));
            //Program.CreatePoint(  end2.TransformBy(mat));

            Program.CreateLine(start1, end1);
            Program.CreateLine(start2, end2);
        }

        [CommandMethod("BBC-ALIGN", CommandFlags.UsePickSet)]
        public static void TranslateAndRotate()
        {
            var sourceStart = GetPoint("\nPick source (Field) point. ");

            if (sourceStart == null) return;

            var sourceEnd = GetPoint("\nPick fixed destination (Record) point. ");

            if (sourceEnd == null) return;

            var desStart    = GetPoint("\nPick fixed destination (Record) point. ");

            if (desStart == null) return;
        
            var desEnd      = GetPoint("\nPick destination (Record) point. ");

            if (desEnd == null) return;

            var translationVector = sourceStart.Value.GetVectorTo(sourceEnd.Value);
            
            var picksetbefore = new PointObject(sourceStart.Value,sourceEnd.Value,desStart.Value,desEnd.Value);
            var picksetafter  = new PointObject(picksetbefore, translationVector);

            //CreatePickSetLines(picksetbefore);
            CreatePickSetLines(picksetafter) ;
            var vec1 = picksetafter.SouStart.GetVectorTo(picksetafter.SouEnd);
            var vec2 = picksetafter.DesStart.GetVectorTo(picksetafter.DesEnd);

            var ang = vec1.GetAngleTo(vec2) * 180.0 / Math.PI;

            var mat = Matrix3d.Rotation(vec1.GetAngleTo(vec2), new Vector3d(0, 0, 1), picksetafter.SouEnd);

            Program.CreatePoint(picksetafter.SouEnd.TransformBy(mat));
            Program.CreatePoint(picksetafter.DesEnd.TransformBy(mat));
            // move first point to second point

            //var start1 = new Point3d(1, 0, 0);
            //var end1   = new Point3d(2, 0, 0);

            //var start2 = new Point3d(1, 0, 0);
            //var end2   = new Point3d(2, 2, 0);

            //var vec1 = start1.GetVectorTo(end1);
            //var vec2 = start2.GetVectorTo(end2);

            //var ang = vec1.GetAngleTo(vec2) * 180.0 / Math.PI;

            //var mat = Matrix3d.Rotation(vec1.GetAngleTo(vec2), new Vector3d(0, 0, 1), start1);

            //Program.CreatePoint(start1);
            //Program.CreatePoint(start2);

            //Program.CreatePoint(end1);
            //Program.CreatePoint(end2);

            //Program.CreatePoint(start1.TransformBy(mat));
            //// Program.CreatePoint(start2.TransformBy(mat));
            //Program.CreatePoint(end1.TransformBy(mat));
            ////Program.CreatePoint(  end2.TransformBy(mat));

            //Program.CreateLine(start1, end1);
            //Program.CreateLine(start2, end2);
        }

        private static void CreatePickSetLines(PointObject picksetbefore)
        {
            Program.CreateLine(picksetbefore.SouStart, picksetbefore.SouEnd);
            Program.CreateLine(picksetbefore.DesStart, picksetbefore.DesEnd);
        }

        private static Point3d? GetPoint(string message)
        {
            
            PromptPointOptions po = new PromptPointOptions(message);
            po.AllowNone = false;
            po.AllowArbitraryInput = false;
            
            PromptPointResult pr = Active.Editor.GetPoint(po);

            if (pr.Status == PromptStatus.OK)
                return pr.Value;
            else
            {
                return null;
            }
        }

        public class PointObject
        {
            public Point3d SouStart;
            public Point3d SouEnd;
            public Point3d DesStart;
            public Point3d DesEnd;

            public PointObject(Point3d sourcestart,Point3d sourceend,Point3d deststart,Point3d destend)
            {
                SouStart = sourcestart;
                SouEnd   = sourceend;
                DesStart = deststart;
                DesEnd   = destend;
            }

            public PointObject(PointObject pickset, Vector3d translate)
            {
                SouStart = pickset.SouStart + translate;
                SouEnd   = pickset.SouEnd   + translate;
                DesStart = pickset.DesStart + translate;
                DesEnd   = pickset.DesEnd   + translate;
            }
        }
    }
}
