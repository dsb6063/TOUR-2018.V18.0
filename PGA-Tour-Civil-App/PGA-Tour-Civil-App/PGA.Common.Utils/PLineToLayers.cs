#region

using System.IO;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using BBC.Common.AutoCAD;
using PGA.Common;
using Exception = System.Exception;

#endregion

namespace PGA.Autodesk.Utils
{
    public class PLineToLayers
    {

        public static PLineToLayers Instance
        {
            get { return PLineToLayersCreator.instance; }
        }

        public static void SaveToAcad2007()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                var output =
                    Path.GetDirectoryName(db.Filename) + "\\" +
                    Path.GetFileNameWithoutExtension(db.Filename) +
                    "-ACAD2007" + Path.GetExtension(db.Filename);


                db.SaveAs(output, DwgVersion.AC1021);
                tr.Commit();
            }
        }

        public static void SelectPolylinesToChange()
        {

            ObjectId[] selection = null;

            #region Database

            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            #endregion


            try

            {

                PromptSelectionResult selectionRes =

                    ed.SelectImplied();

                // If there's no pickfirst set available...

                if (selectionRes.Status == PromptStatus.Error)

                {

                    // ... ask the user to select entities

                    PromptSelectionOptions selectionOpts =

                        new PromptSelectionOptions();

                    selectionOpts.MessageForAdding =

                        "\nSelect polylines: ";

                    selectionRes =

                        ed.GetSelection(selectionOpts);

                }

                else

                {

                    // If there was a pickfirst set, clear it

                    ed.SetImpliedSelection(new ObjectId[0]);

                }

                // If the user has not cancelled...

                if (selectionRes.Status == PromptStatus.OK)
                {


                    try
                    {

                        selection = selectionRes.Value.GetObjectIds().Where
                        (id =>
                        {
                            return (
                            (id.ObjectClass == RXObject.GetClass(typeof(Polyline))) ||
                             (id.ObjectClass == RXObject.GetClass(typeof(Polyline2d))) ||
                              (id.ObjectClass == RXObject.GetClass(typeof(Polyline3d))));
                        }).ToArray();

                        foreach (var obj in selection)
                        {
                            if (obj.IsErased)
                                continue;

                            InteratePolyLinesToChange(obj);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessengerManager.MessengerManager.AddLog(ex.Message);
                    }
                 
                }
            }
            catch { }
        }

        private static void InteratePolyLinesToChange(ObjectId selectedObjectId)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();

            // Get the current UCS

            var ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (tr)
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);

                var lwp = obj as Polyline;

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (lwp.Closed)
                    {
                        #region Color Change

                        //PGA.MessengerManager.MessengerManager.AddLog(lwp.Layer);
                        //lwp.UpgradeOpen();
                        //if (IsLayerDefined(db, AssignPolyLinesToLayers(lwp)))
                        //    lwp.Color = AssignLayerColor(lwp) ?? lwp.Color;
                        //lwp.DowngradeOpen();
                        //PGA.MessengerManager.MessengerManager.AddLog(lwp.Layer); 

                        #endregion

                        #region Layer Assign

                        MessengerManager.MessengerManager.AddLog(lwp.Layer);
                        lwp.UpgradeOpen();
                        if (IsLayerDefined(db, AssignPolyLinesToLayers(lwp)))
                            lwp.Layer = AssignPolyLinesToLayers(lwp) ?? lwp.Layer;
                        lwp.DowngradeOpen();
                        MessengerManager.MessengerManager.AddLog(lwp.Layer);

                        #endregion
                    }
                    else
                    {
                        PGA.MessengerManager.MessengerManager.AddLog("Layer is not closed: " + lwp.Layer);

                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    var p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        // Use foreach to get each contained vertex

                        foreach (ObjectId vId in p2d)
                        {
                            var v2d =
                                (Vertex2d) tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                );

                            ed.WriteMessage(
                                "\n" + v2d.Position
                            );
                        }
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        var p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            // Use foreach to get each contained vertex

                            foreach (ObjectId vId in p3d)
                            {
                                var v3d =
                                    (PolylineVertex3d) tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                    );

                                ed.WriteMessage(
                                    "\n" + v3d.Position
                                );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
        }

        public static Color AssignLayerColor(Polyline lwp)
        {
            try
            {
                var name = lwp.Layer.ToUpper();

                switch (name)
                {
                    case "OBR":
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);

                    case "OBD":
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);

                    case "OST":
                        return Color.FromColorIndex(ColorMethod.ByColor, 1);

                    case "OBO":
                        return Color.FromColorIndex(ColorMethod.ByColor, 112);

                    case "OCA":
                        return Color.FromColorIndex(ColorMethod.ByColor, 253);

                    case "OCO":
                        return Color.FromColorIndex(ColorMethod.ByColor, 96);

                    case "ODO":
                        return Color.FromColorIndex(ColorMethod.ByColor, 63);

                    case "OFW":
                        return Color.FromColorIndex(ColorMethod.ByColor, 98);

                    case "OGR":
                        return Color.FromColorIndex(ColorMethod.ByColor, 3);

                    case "OGS":
                        return Color.FromColorIndex(ColorMethod.ByColor, 3);

                    case "OIR":
                        return Color.FromColorIndex(ColorMethod.ByColor, 40);

                    case "OLN":
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);

                    case "ONA":
                        return Color.FromColorIndex(ColorMethod.ByColor, 43);

                    case "ORK":
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);

                    case "ORO":
                        return Color.FromColorIndex(ColorMethod.ByColor, 51);

                    case "OSS":
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);

                    case "OTB":
                        return Color.FromColorIndex(ColorMethod.ByColor, 4);

                    case "OTO":
                        return Color.FromColorIndex(ColorMethod.ByColor, 101);

                    case "OWS":
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);

                    case "OWL":
                        return Color.FromColorIndex(ColorMethod.ByColor, 12);

                    case "OWA":
                        return Color.FromColorIndex(ColorMethod.ByColor, 5);

                    case "OWD":
                        return Color.FromColorIndex(ColorMethod.ByColor, 5);
                    case "OTH":
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);
                    default:
                        return Color.FromColorIndex(ColorMethod.ByColor, 255);
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
            return null;
        }


        //private static LayerStatesBLL GetLayerStates()
        //{
        //    IModel model = new Model.BLL.Model();
        //    return model.GetDataFromRegistry();
        //}

        //public Model.DAL.PGALayerStates LoadData()
        //{
        //    IModel model = new Model.BLL.Model();
        //    _view.LoadDataSet(model.GetDataFromRegistry());
        //    return null;
        //}

        private bool AssignPolyLinesToLayers()
        {
            return false;
        }

        private static bool IsLayerDefined(global::Autodesk.AutoCAD.DatabaseServices.Database db, string layername)
        {
            try
            {
                if (LayerManager.IsDefined(db, layername))
                    return LayerManager.IsDefined(db, layername);
                throw new Exception("Layer not found: " + layername);
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
            return false;
        }

        private static void InteratePolyLines(ObjectId selectedObjectId)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;
            var tr = db.TransactionManager.StartTransaction();

            // Get the current UCS

            var ucs =
                ed.CurrentUserCoordinateSystem.CoordinateSystem3d;

            using (tr)
            {
                var obj =
                    tr.GetObject(selectedObjectId, OpenMode.ForRead);


                // If a "lightweight" (or optimized) polyline

                var lwp = obj as Polyline;

                if (lwp != null)
                {
                    // Is Polyline Closed
                    if (lwp.Closed)
                    {
                        MessengerManager.MessengerManager.AddLog(lwp.Layer);
                        lwp.UpgradeOpen();
                        if (IsLayerDefined(db, AssignPolyLinesToLayers(lwp)))
                            lwp.Layer = AssignPolyLinesToLayers(lwp) ?? lwp.Layer;
                        lwp.DowngradeOpen();
                        MessengerManager.MessengerManager.AddLog(lwp.Layer);
                    }
                    // Use a for loop to get each vertex, one by one

                    var vn = lwp.NumberOfVertices;

                    for (var i = 0; i < vn; i++)
                    {
                        // Could also get the 3D point here

                        var pt = lwp.GetPoint2dAt(i);

                        ed.WriteMessage("\n" + pt);
                    }
                }

                else
                {
                    // If an old-style, 2D polyline

                    var p2d = obj as Polyline2d;

                    if (p2d != null)
                    {
                        // Use foreach to get each contained vertex

                        foreach (ObjectId vId in p2d)
                        {
                            var v2d =
                                (Vertex2d) tr.GetObject(
                                    vId,
                                    OpenMode.ForRead
                                );

                            ed.WriteMessage(
                                "\n" + v2d.Position
                            );
                        }
                    }

                    else
                    {
                        // If an old-style, 3D polyline

                        var p3d = obj as Polyline3d;

                        if (p3d != null)
                        {
                            // Use foreach to get each contained vertex

                            foreach (ObjectId vId in p3d)
                            {
                                var v3d =
                                    (PolylineVertex3d) tr.GetObject(
                                        vId,
                                        OpenMode.ForRead
                                    );

                                ed.WriteMessage(
                                    "\n" + v3d.Position
                                );
                            }
                        }
                    }
                }

                // Committing is cheaper than aborting

                tr.Commit();
            }
        }


        public class LayerStatesBLL
        {
            public virtual string Bridge { get; set; } = "OBR:S-BRIDGES";

            public virtual string Building { get; set; } = "OBD:S-BUILDING";

            public virtual string Bunker { get; set; } = "OST:S-BUNKER";

            public virtual string BushOutline { get; set; } = "OBO:S-BUSH";

            public virtual string CartPath { get; set; } = "OCA:S-CARTPATH";

            public virtual string Collar { get; set; } = "OCO:S-COLLAR";

            public virtual string DirtOutline { get; set; } = "ODO:S-DIRT-OUTLINE";

            public virtual string Fairway { get; set; } = "OFW:S-FAIRWAY";

            public virtual string Green { get; set; } = "OGR:S-GREEN";

            public virtual string GreenSideBunker { get; set; } = "OGS:S-GREENSIDE-BUNKER";

            public virtual string IntMedRough { get; set; } = "IOR:S-INTERMEDIATE-ROUGH";

            public virtual string LandScaping { get; set; } = "OLN:S-LANDSCAPING";

            public virtual string NativeArea { get; set; } = "ONA:S-NATIVE-AREA";

            public virtual string Other { get; set; } = "OTH:S-OTHER";

            public virtual string Path { get; set; } = "OPT:S-PATH";

            public virtual string RockOutline { get; set; } = "ORK:S-ROCK-OUTLINE";

            public virtual string RoughOutline { get; set; } = "ORO:S-ROUGH-OUTLINES";

            public virtual string Steps { get; set; } = "OSS:S-STEPS";

            public virtual string TeeBox { get; set; } = "OTB:S-TEE-BOX";

            public virtual string TreeOutline { get; set; } = "OTO:S-TREE-OUTLINE";

            public virtual string WalkStrip { get; set; } = "OWS:S-WALK-STRIP";

            public virtual string Wall { get; set; } = "OWL:S-WALL";

            public virtual string Water { get; set; } = "OWA:S-WATER";

            public virtual string WaterDrop { get; set; } = "OWD:S-WATER-DROP";

            public virtual uint LayerStateID { get; set; }
        }


        private static string AssignPolyLinesToLayers(Polyline lwp)
        {
            //Get the 3-digit layer name of the object and compare
            var states = new LayerStatesBLL();

            try
            {
                var name = lwp.Layer.ToUpper();

                switch (name)
                {
                    case "OBR":
                        return CommonUtilities.GetLayer(states.Bridge);

                    case "OBD":
                        return CommonUtilities.GetLayer(states.Building);

                    case "OST":
                        return CommonUtilities.GetLayer(states.Bunker);

                    case "OBO":
                        return CommonUtilities.GetLayer(states.BushOutline);

                    case "OCA":
                        return CommonUtilities.GetLayer(states.CartPath);

                    case "OCO":
                        return CommonUtilities.GetLayer(states.Collar);

                    case "ODO":
                        return CommonUtilities.GetLayer(states.DirtOutline);

                    case "OFW":
                        return CommonUtilities.GetLayer(states.Fairway);

                    case "OGR":
                        return CommonUtilities.GetLayer(states.Green);

                    case "OGS":
                        return CommonUtilities.GetLayer(states.GreenSideBunker);

                    case "OIR":
                        return CommonUtilities.GetLayer(states.IntMedRough);

                    case "OLN":
                        return CommonUtilities.GetLayer(states.LandScaping);

                    case "ONA":
                        return CommonUtilities.GetLayer(states.NativeArea);

                    case "OPT":
                        return CommonUtilities.GetLayer(states.Path);

                    case "ORK":
                        return CommonUtilities.GetLayer(states.RockOutline);

                    case "ORO":
                        return CommonUtilities.GetLayer(states.RoughOutline);

                    case "OSS":
                        return CommonUtilities.GetLayer(states.Steps);

                    case "OTB":
                        return CommonUtilities.GetLayer(states.TeeBox);

                    case "OTO":
                        return CommonUtilities.GetLayer(states.TreeOutline);

                    case "OWS":
                        return CommonUtilities.GetLayer(states.WalkStrip);

                    case "OWL":
                        return CommonUtilities.GetLayer(states.Wall);

                    case "OWA":
                        return CommonUtilities.GetLayer(states.Water);

                    case "OWD":
                        return CommonUtilities.GetLayer(states.WaterDrop);
                    case "OTH":
                        return CommonUtilities.GetLayer(states.Other);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                MessengerManager.MessengerManager.AddLog(ex.Message);
            }
            return null;
        }

        public static void SelectPolylines()
        {
            SelectionSet selection = null;
            selection = SelectionManager.GetSelectionSet("");

            foreach (var obj in selection.GetObjectIds())
            {
                InteratePolyLines(obj);
            }
        }

        class PLineToLayersCreator
        {
            internal static readonly PLineToLayers instance = new PLineToLayers();

            static PLineToLayersCreator()
            {
            }
        }

        #region Process

        //1. read values from registry

        //2. get the current plines in ms

        //3. iterate and assign colors to layers

        //4. or move to that layer 

        #endregion
    }
}