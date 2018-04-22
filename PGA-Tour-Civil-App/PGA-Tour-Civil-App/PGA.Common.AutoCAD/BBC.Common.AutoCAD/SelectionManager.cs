#region

using System;
using System.Collections.Generic;
using global::Autodesk.AutoCAD.ApplicationServices.Core;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.Geometry;

#endregion

//using Common.Logging;

namespace BBC.Common.AutoCAD
{
    public class SelectionManager
    {
        //private static readonly ILog //_logger = LogManager.GetLogger(typeof(SelectionManager));

        /// <summary>
        ///     Adds to pick set.
        /// </summary>
        /// <param name="oids">The oids.</param>
        /// <returns></returns>
        public static bool AddToPickSet(ObjectIdCollection oids)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddToPickSet");
            var retVal = false;
            try
            {
                var ed = Application.DocumentManager.MdiActiveDocument.Editor;

                var isPickSetModified = false;
                var pickedOids = new ObjectIdCollection();

                //Access the current PickSet
                var resBuf = ed.SelectImplied();
                if (resBuf.Status != PromptStatus.Error)
                {
                    //Load into a selection set object
                    var ssPickSet = resBuf.Value;
                    //Get the object ID's
                    pickedOids = new ObjectIdCollection(ssPickSet.GetObjectIds());
                }
                foreach (ObjectId oid in oids)
                {
                    if (!pickedOids.Contains(oid))
                    {
                        //Adding object to pickSet selection
                        pickedOids.Add(oid);
                        isPickSetModified = true;
                    }
                }
                if (isPickSetModified)
                {
                    var pickedOidList = new List<ObjectId>();
                    foreach (ObjectId oid in pickedOids)
                    {
                        pickedOidList.Add(oid);
                    }
                    ed.SetImpliedSelection(pickedOidList.ToArray());
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in AddToPickSet", ex);
                throw;
            }
            PGA.MessengerManager.MessengerManager.AddLog("End AddToPickSet");
            return retVal;
        }

        /// <summary>
        ///     Gets the window selection.
        /// </summary>
        /// <returns></returns>
        public static Point3d[] GetWindowSelection()
        {
            Point3d[] selectedPoints = null;
            var currentDoc = Application.DocumentManager.MdiActiveDocument;
            var editor = currentDoc.Editor;

            var ppr1 = editor.GetPoint("\nSelect first corner of window: ");
            if (ppr1.Status != PromptStatus.OK)
                return null;

            var ppr2 = editor.GetCorner("\nSelect opposite corner of window: ", ppr1.Value);
            if (ppr2.Status != PromptStatus.OK)
                return null;

            selectedPoints = new Point3d[2];
            selectedPoints[0] = ppr1.Value;
            selectedPoints[1] = ppr2.Value;

            return selectedPoints;
        }

        /// <summary>
        ///     Gets the window selection.
        /// </summary>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(string prompt)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetSelectionSet");
            var currentDoc = Application.DocumentManager.MdiActiveDocument;
            var editor = currentDoc.Editor;

            //PromptPointResult ppr1 = editor.GetPoint("Select first corner of window: ");
            //if (ppr1.Status != PromptStatus.OK)
            //    return null;

            //PromptPointResult ppr2 = editor.GetCorner("Select opposite corner of window: ", ppr1.Value);
            //if (ppr2.Status != PromptStatus.OK)
            //    return null;

            var options = new PromptSelectionOptions();
            if (string.IsNullOrEmpty(prompt))
            {
                prompt = "\nSelect objects:";
            }
            options.MessageForAdding = prompt;
            options.RejectObjectsFromNonCurrentSpace = true;
            options.RejectObjectsOnLockedLayers = true;
            options.RejectPaperspaceViewport = true;

            var psr = editor.GetSelection(options);

            if (psr.Status != PromptStatus.OK)
                return null;

            if (psr.Value.Count == 0)
                return null;

            PGA.MessengerManager.MessengerManager.AddLog("End GetSelectionSet");
            return psr.Value;
        }

        /// <summary>
        ///     Gets the selection set.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="layerFilter">The layer filter.</param>
        /// <param name="objectFilter">The object filter.</param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(string prompt, string layerFilter, string objectFilter)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetSelectionSet");
            SelectionSet selectionSet = null;

            var currentDoc = Application.DocumentManager.MdiActiveDocument;
            var editor = currentDoc.Editor;

            var options = new PromptSelectionOptions();
            if (string.IsNullOrEmpty(prompt))
            {
                prompt = "\nSelect objects:";
            }
            options.MessageForAdding = prompt;
            options.RejectObjectsFromNonCurrentSpace = true;
            options.RejectObjectsOnLockedLayers = true;
            options.RejectPaperspaceViewport = true;

            var typedValues = new TypedValue[2];
            var i = 0;

            var tvLayer = new TypedValue((int) DxfCode.LayerName, layerFilter);
            typedValues[i++] = tvLayer;

            var tvObject = new TypedValue((int) DxfCode.Start, objectFilter);
            typedValues[i++] = tvObject;

            // Assign the filter criteria to a SelectionFilter object
            var selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;
            promptResult = editor.GetSelection(options, selectionFilter);

            // If the prompt status is OK, objects were selected
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
            }

            PGA.MessengerManager.MessengerManager.AddLog("End GetSelectionSet");
            return selectionSet;
        }

        /// <summary>
        ///     Gets all objects in the current document within the window and layer filter defined
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="layerFilter"></param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(double minX, double minY, double maxX, double maxY,
            string layerFilter, bool isCrossing)
        {
            SelectionSet selectionSet = null;

            var currentDoc = Application.DocumentManager.MdiActiveDocument;
            var editor = currentDoc.Editor;

            var typedValues = new TypedValue[5];
            var i = 0;

            var beginningOr = new TypedValue((int) DxfCode.Operator, "<or");
            var endingOr = new TypedValue((int) DxfCode.Operator, "or>");
            var beginningAnd = new TypedValue((int) DxfCode.Operator, "<and");
            var endingAnd = new TypedValue((int) DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;
            typedValues[i++] = beginningAnd;

            var tv = new TypedValue((int) DxfCode.LayerName, layerFilter);
            typedValues[i++] = tv;

            typedValues[i++] = endingAnd;
            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            var selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;

            var minPoint = new Point3d(minX, minY, 0.0);
            var maxPoint = new Point3d(maxX, maxY, 0.0);
            if (isCrossing)
            {
                promptResult = editor.SelectCrossingWindow(minPoint, maxPoint, selectionFilter);
            }
            else
            {
                promptResult = editor.SelectWindow(minPoint, maxPoint, selectionFilter);
            }

            // If the prompt status is OK, objects were selected
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
            }

            return selectionSet;
        }

        /// <summary>
        ///     Gets all objects in the current document within the polygon and layer filter defined
        /// </summary>
        /// <param name="polyPoints"></param>
        /// <param name="layerFilter"></param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(Point3dCollection polyPoints, string layerFilter, bool isCrossing,
            bool isFence)
        {
            SelectionSet selectionSet = null;

            var currentDoc = Application.DocumentManager.MdiActiveDocument;
            var editor = currentDoc.Editor;

            var typedValues = new TypedValue[5];
            var i = 0;

            var beginningOr = new TypedValue((int) DxfCode.Operator, "<or");
            var endingOr = new TypedValue((int) DxfCode.Operator, "or>");
            var beginningAnd = new TypedValue((int) DxfCode.Operator, "<and");
            var endingAnd = new TypedValue((int) DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;
            typedValues[i++] = beginningAnd;

            var tv = new TypedValue((int) DxfCode.LayerName, layerFilter);
            typedValues[i++] = tv;

            typedValues[i++] = endingAnd;
            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            var selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;
            if (isFence)
            {
                promptResult = editor.SelectFence(polyPoints, selectionFilter);
            }
            else
            {
                if (isCrossing)
                {
                    promptResult = editor.SelectCrossingPolygon(polyPoints, selectionFilter);
                }
                else
                {
                    promptResult = editor.SelectWindowPolygon(polyPoints, selectionFilter);
                }
            }

            // If the prompt status is OK, objects were selected
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
            }

            return selectionSet;
        }

        /// <summary>
        ///     Gets all objects in the current document within the polygon, layer filter and object filter defined
        /// </summary>
        /// <param name="polyPoints"></param>
        /// <param name="layerFilter"></param>
        /// <param name="objectFilter"></param>
        /// <param name="isCrossing"></param>
        /// <param name="isFence"></param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(Point3dCollection polyPoints, string layerFilter, string objectFilter,
            bool isCrossing, bool isFence)
        {
            SelectionSet selectionSet = null;

            var currentDoc = Application.DocumentManager.MdiActiveDocument;
            var editor = currentDoc.Editor;

            var typedValues = new TypedValue[2];
            var i = 0;

            var tvLayer = new TypedValue((int) DxfCode.LayerName, layerFilter);
            typedValues[i++] = tvLayer;

            var tvObject = new TypedValue((int) DxfCode.Start, objectFilter);
            typedValues[i++] = tvObject;

            // Assign the filter criteria to a SelectionFilter object
            var selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;
            if (isFence)
            {
                promptResult = editor.SelectFence(polyPoints, selectionFilter);
            }
            else
            {
                if (isCrossing)
                {
                    promptResult = editor.SelectCrossingPolygon(polyPoints, selectionFilter);
                }
                else
                {
                    promptResult = editor.SelectWindowPolygon(polyPoints, selectionFilter);
                }
            }

            // If the prompt status is OK, objects were selected
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
            }

            return selectionSet;
        }

        /// <summary>
        ///     Gets the filtered layers selection set.
        /// </summary>
        /// <param name="layerNames">The layer names.</param>
        /// <returns></returns>
        public static SelectionSet GetObjectsOnLayers(IList<string> layerNames)
        {
            SelectionSet selectionSet = null;

            var editor = Application.DocumentManager.MdiActiveDocument.Editor;

            var typedValues = new TypedValue[layerNames.Count*3 + 2];
            var i = 0;

            var beginningOr = new TypedValue((int) DxfCode.Operator, "<or");
            var endingOr = new TypedValue((int) DxfCode.Operator, "or>");
            var beginningAnd = new TypedValue((int) DxfCode.Operator, "<and");
            var endingAnd = new TypedValue((int) DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;

            foreach (var layerName in layerNames)
            {
                typedValues[i++] = beginningAnd;
                var tv = new TypedValue((int) DxfCode.LayerName, layerName);
                typedValues[i++] = tv;
                typedValues[i++] = endingAnd;
            }

            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            var selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;

            promptResult = editor.SelectAll(selectionFilter);

            // If the prompt status is OK, objects were selected
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
            }

            return selectionSet;
        }

        /// <summary>
        ///     Gets the filtered layers selection set.
        /// </summary>
        /// <param name="layerNames">The layer names.</param>
        /// <returns></returns>
        public static SelectionSet GetObjectsOnLayer(string layerName)
        {
            SelectionSet selectionSet = null;

            var editor = Application.DocumentManager.MdiActiveDocument.Editor;

            var typedValues = new TypedValue[5];
            var i = 0;

            var beginningOr = new TypedValue((int) DxfCode.Operator, "<or");
            var endingOr = new TypedValue((int) DxfCode.Operator, "or>");
            var beginningAnd = new TypedValue((int) DxfCode.Operator, "<and");
            var endingAnd = new TypedValue((int) DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;
            typedValues[i++] = beginningAnd;

            var tv = new TypedValue((int) DxfCode.LayerName, layerName);
            typedValues[i++] = tv;

            typedValues[i++] = endingAnd;
            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            var selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;

            promptResult = editor.SelectAll(selectionFilter);

            // If the prompt status is OK, objects were selected
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
            }

            return selectionSet;
        }

        /// <summary>
        ///     Gets the object ids from previous selection.
        /// </summary>
        /// <returns></returns>
        public static ObjectIdCollection GetObjectIdsFromPreviousSelection()
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetObjectIdsFromPreviousSelection");
            SelectionSet selectionSet = null;
            ObjectIdCollection oids = null;

            var editor = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult promptResult;
            promptResult = editor.SelectPrevious();
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
                oids = new ObjectIdCollection(selectionSet.GetObjectIds());
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetObjectIdsFromPreviousSelection");
            return oids;
        }
    }
}