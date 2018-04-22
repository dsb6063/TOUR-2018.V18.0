
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using Autodesk.AutoCAD.Runtime;
using BBC.Common.Active;
using Acaddb = global::Autodesk.AutoCAD.DatabaseServices;

namespace PGA.Common.AutoCAD
{
    public class SelectionManager
    {

        /// <summary>
        /// Adds to pick set.
        /// </summary>
        /// <param name="oids">The oids.</param>
        /// <returns></returns>
        public static bool AddToPickSet(Acaddb.ObjectIdCollection oids)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start AddToPickSet");
            bool retVal = false;
            try
            {
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

                bool isPickSetModified = false;
                Acaddb.ObjectIdCollection pickedOids = new Acaddb.ObjectIdCollection();

                //Access the current PickSet
                PromptSelectionResult resBuf = ed.SelectImplied();
                if (resBuf.Status != PromptStatus.Error)
                {
                    //Load into a selection set object
                    SelectionSet ssPickSet = resBuf.Value;
                    //Get the object ID's
                    pickedOids = new Acaddb.ObjectIdCollection(ssPickSet.GetObjectIds());
                }
                foreach (Acaddb.ObjectId oid in oids)
                {
                    if (!pickedOids.Contains(oid))
                    {
                        //Adding object to pickSet selection
                        pickedOids.Add(oid);
                        isPickSetModified = true;
                    }
                }
                if (isPickSetModified == true)
                {
                    List<Acaddb.ObjectId> pickedOidList = new List<Acaddb.ObjectId>();
                    foreach (Acaddb.ObjectId oid in pickedOids)
                    {
                        pickedOidList.Add(oid);
                    }
                    ed.SetImpliedSelection(pickedOidList.ToArray());
                    retVal = true;
                }
            }
            catch (System.Exception ex)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error in AddToPickSet", ex);
                throw;
            }
            PGA.MessengerManager.MessengerManager.AddLog("End AddToPickSet");
            return retVal;

        }

        /// <summary>
        /// Gets the window selection.
        /// </summary>
        /// <returns></returns>
        public static Point3d[] GetWindowSelection()
        {
            Point3d[] selectedPoints = null;
            Document currentDoc = Active.MdiDocument;
            Editor editor = currentDoc.Editor;

            PromptPointResult ppr1 = editor.GetPoint("\nSelect first corner of window: ");
            if (ppr1.Status != PromptStatus.OK)
                return null;

            PromptPointResult ppr2 = editor.GetCorner("\nSelect opposite corner of window: ", ppr1.Value);
            if (ppr2.Status != PromptStatus.OK)
                return null;

            selectedPoints = new Point3d[2];
            selectedPoints[0] = ppr1.Value;
            selectedPoints[1] = ppr2.Value;

            return selectedPoints;
        }

        /// <summary>
        /// Gets the window selection.
        /// </summary>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(string prompt)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetSelectionSet");
            Document currentDoc = Active.MdiDocument;
            Editor editor = currentDoc.Editor;

            //PromptPointResult ppr1 = editor.GetPoint("Select first corner of window: ");
            //if (ppr1.Status != PromptStatus.OK)
            //    return null;

            //PromptPointResult ppr2 = editor.GetCorner("Select opposite corner of window: ", ppr1.Value);
            //if (ppr2.Status != PromptStatus.OK)
            //    return null;

            PromptSelectionOptions options = new PromptSelectionOptions();
            if ( string.IsNullOrEmpty(prompt))
            {
                prompt = "\nSelect objects:";
            }
            options.MessageForAdding = prompt;
            options.RejectObjectsFromNonCurrentSpace = true;
            options.RejectObjectsOnLockedLayers = true;
            options.RejectPaperspaceViewport = true;

            PromptSelectionResult psr = editor.GetSelection(options);

            if (psr.Status != PromptStatus.OK)
                return null;

            if (psr.Value.Count == 0)
                return null;

            PGA.MessengerManager.MessengerManager.AddLog("End GetSelectionSet");
            return psr.Value;
        }

        /// <summary>
        /// Gets the selection set.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <param name="layerFilter">The layer filter.</param>
        /// <param name="objectFilter">The object filter.</param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(string prompt, string layerFilter, string objectFilter)
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetSelectionSet");
            SelectionSet selectionSet = null;

            Document currentDoc = Active.MdiDocument;
            Editor editor = currentDoc.Editor;

            PromptSelectionOptions options = new PromptSelectionOptions();
            if (string.IsNullOrEmpty(prompt))
            {
                prompt = "\nSelect objects:";
            }
            options.MessageForAdding = prompt;
            options.RejectObjectsFromNonCurrentSpace = true;
            options.RejectObjectsOnLockedLayers = true;
            options.RejectPaperspaceViewport = true;

            Acaddb.TypedValue[] typedValues = new Acaddb.TypedValue[2];
            int i = 0;

            Acaddb.TypedValue tvLayer = new Acaddb.TypedValue((int)Acaddb.DxfCode.LayerName, layerFilter);
            typedValues[i++] = tvLayer;

            Acaddb.TypedValue tvObject = new Acaddb.TypedValue((int)Acaddb.DxfCode.Start, objectFilter);
            typedValues[i++] = tvObject;

            // Assign the filter criteria to a SelectionFilter object
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);

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
        /// Gets all objects in the current document within the window and layer filter defined
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="layerFilter"></param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(double minX, double minY, double maxX, double maxY, string layerFilter, bool isCrossing)
        {
            SelectionSet selectionSet = null;

            Document currentDoc = Active.MdiDocument;
            Editor editor = currentDoc.Editor;

            Acaddb.TypedValue[] typedValues = new Acaddb.TypedValue[5];
            int i = 0;

            Acaddb.TypedValue beginningOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<or");
            Acaddb.TypedValue endingOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "or>");
            Acaddb.TypedValue beginningAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<and");
            Acaddb.TypedValue endingAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;
            typedValues[i++] = beginningAnd;

            Acaddb.TypedValue tv = new Acaddb.TypedValue((int)Acaddb.DxfCode.LayerName, layerFilter);
            typedValues[i++] = tv;

            typedValues[i++] = endingAnd;
            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;

            Point3d minPoint = new Point3d(minX, minY, 0.0);
            Point3d maxPoint = new Point3d(maxX, maxY, 0.0);
            if (isCrossing == true)
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
        /// Gets all objects in the current document within the polygon and layer filter defined
        /// </summary>
        /// <param name="polyPoints"></param>
        /// <param name="layerFilter"></param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(Point3dCollection polyPoints, string layerFilter, bool isCrossing, bool isFence)
        {
            SelectionSet selectionSet = null;

            Document currentDoc = Active.MdiDocument;
            Editor editor = currentDoc.Editor;

            Acaddb.TypedValue[] typedValues = new Acaddb.TypedValue[5];
            int i = 0;

            Acaddb.TypedValue beginningOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<or");
            Acaddb.TypedValue endingOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "or>");
            Acaddb.TypedValue beginningAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<and");
            Acaddb.TypedValue endingAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;
            typedValues[i++] = beginningAnd;

            Acaddb.TypedValue tv = new Acaddb.TypedValue((int)Acaddb.DxfCode.LayerName, layerFilter);
            typedValues[i++] = tv;

            typedValues[i++] = endingAnd;
            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;
            if (isFence == true)
            {
                promptResult = editor.SelectFence(polyPoints, selectionFilter);
            }
            else
            {
                if (isCrossing == true)
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
        /// Gets all objects in the current document within the polygon, layer filter and object filter defined
        /// </summary>
        /// <param name="polyPoints"></param>
        /// <param name="layerFilter"></param>
        /// <param name="objectFilter"></param>
        /// <param name="isCrossing"></param>
        /// <param name="isFence"></param>
        /// <returns></returns>
        public static SelectionSet GetSelectionSet(Point3dCollection polyPoints, string layerFilter, string objectFilter, bool isCrossing, bool isFence)
        {
            SelectionSet selectionSet = null;

            Document currentDoc = Active.MdiDocument;
            Editor editor = currentDoc.Editor;

            Acaddb.TypedValue[] typedValues = new Acaddb.TypedValue[2];
            int i = 0;

            Acaddb.TypedValue tvLayer = new Acaddb.TypedValue((int)Acaddb.DxfCode.LayerName, layerFilter);
            typedValues[i++] = tvLayer;

            Acaddb.TypedValue tvObject = new Acaddb.TypedValue((int)Acaddb.DxfCode.Start, objectFilter);
            typedValues[i++] = tvObject;

            // Assign the filter criteria to a SelectionFilter object
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);

            // Request for objects to be selected in the drawing area
            PromptSelectionResult promptResult;
            if (isFence == true)
            {
                promptResult = editor.SelectFence(polyPoints, selectionFilter);
            }
            else
            {
                if (isCrossing == true)
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
        /// Gets the filtered layers selection set.
        /// </summary>
        /// <param name="layerNames">The layer names.</param>
        /// <returns></returns>
        public static SelectionSet GetObjectsOnLayers(IList<string> layerNames)
        {
            SelectionSet selectionSet = null;

            Editor editor =Active.Editor;

            Acaddb.TypedValue[] typedValues = new Acaddb.TypedValue[(layerNames.Count * 3) + 2];
            int i = 0;

            Acaddb.TypedValue beginningOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<or");
            Acaddb.TypedValue endingOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "or>");
            Acaddb.TypedValue beginningAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<and");
            Acaddb.TypedValue endingAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;

            foreach (string layerName in layerNames)
            {
                typedValues[i++] = beginningAnd;
                Acaddb.TypedValue tv = new Acaddb.TypedValue((int)Acaddb.DxfCode.LayerName, layerName);
                typedValues[i++] = tv;
                typedValues[i++] = endingAnd;
            }

            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);

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
        /// Gets the filtered layers selection set.
        /// </summary>
        /// <param name="layerNames">The layer names.</param>
        /// <returns></returns>
        public static SelectionSet GetObjectsOnLayer(string layerName)
        {
            SelectionSet selectionSet = null;

            Editor editor = Active.Editor;

            Acaddb.TypedValue[] typedValues = new Acaddb.TypedValue[5];
            int i = 0;

            Acaddb.TypedValue beginningOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<or");
            Acaddb.TypedValue endingOr = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "or>");
            Acaddb.TypedValue beginningAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "<and");
            Acaddb.TypedValue endingAnd = new Acaddb.TypedValue((int)Acaddb.DxfCode.Operator, "and>");

            typedValues[i++] = beginningOr;
            typedValues[i++] = beginningAnd;

            Acaddb.TypedValue tv = new Acaddb.TypedValue((int)Acaddb.DxfCode.LayerName, layerName);
            typedValues[i++] = tv;

            typedValues[i++] = endingAnd;
            typedValues[i++] = endingOr;

            // Assign the filter criteria to a SelectionFilter object
            SelectionFilter selectionFilter = new SelectionFilter(typedValues);

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
        /// Gets the object ids from previous selection.
        /// </summary>
        /// <returns></returns>
        public static Acaddb.ObjectIdCollection GetObjectIdsFromPreviousSelection()
        {
            PGA.MessengerManager.MessengerManager.AddLog("Start GetObjectIdsFromPreviousSelection");
            SelectionSet selectionSet = null;
            Acaddb.ObjectIdCollection oids = null;

            Editor editor = Active.Editor;
            PromptSelectionResult promptResult;
            promptResult= editor.SelectPrevious();
            if (promptResult.Status == PromptStatus.OK)
            {
                selectionSet = promptResult.Value;
                oids = new Acaddb.ObjectIdCollection(selectionSet.GetObjectIds());
            }
            PGA.MessengerManager.MessengerManager.AddLog("End GetObjectIdsFromPreviousSelection");
            return oids;
        }

    }
}

