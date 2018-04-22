using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.Geometry;
using global::Autodesk.AutoCAD.ApplicationServices;
using ACADDB = global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.Runtime;

namespace PGA.Breaklines
{
    public interface _ISelectPolylines
    {
        ACADDB.ObjectIdCollection GetIdsByTypeCollection();
    }
    public class _SelectPolylines: ISelectPolylines
    {
        public ACADDB.ObjectIdCollection GetIdsByTypeTypeValue(params string[] types)
        {

            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;

            // Get the editor to make the selection
            Editor oEd = doc.Editor;

            // Add our or operators so we can grab multiple types.
            IList<ACADDB.TypedValue> typedValueSelection = new List<ACADDB.TypedValue> {
                    new ACADDB.TypedValue(Convert.ToInt32(ACADDB.DxfCode.Operator), "<or"),
                    new ACADDB.TypedValue(Convert.ToInt32(ACADDB.DxfCode.Operator), "or>")
                };

            // We will need to insert our requested types into the collection.
            // Since we knew we would have to insert they types between the operators..
            // I used a Enumerable type which gave me that functionality. (IListf<T>)
            foreach (var type in types)
                typedValueSelection.Insert(1, new ACADDB.TypedValue(Convert.ToInt32(ACADDB.DxfCode.Start), type));

            SelectionFilter selectionFilter = new SelectionFilter(typedValueSelection.ToArray());

            // because we have to.. Not really sure why, I assume this is our only access point
            // to grab the entities that we want. (I am open to being corrected)
            PromptSelectionResult promptSelectionResult = oEd.SelectAll(selectionFilter);

            // return our new ObjectIdCollection that is "Hopefully" full of the types that we want.
            return new ACADDB.ObjectIdCollection(promptSelectionResult.Value.GetObjectIds());
        }

        public  ACADDB.ObjectIdCollection GetIdsByTypeCollection()
        {
            ObjectIdCollection collection = new ObjectIdCollection();

            Func<Type, RXClass> getClass = RXObject.GetClass;

            // You can set this anywhere
            var acceptableTypes = new HashSet<RXClass>
            {
                getClass(typeof (ACADDB.Polyline)),
                getClass(typeof (Polyline2d)),
                getClass(typeof (ACADDB.Polyline3d))
            };

            var doc = Application.DocumentManager.MdiActiveDocument;
            using (var trans = doc.TransactionManager.StartOpenCloseTransaction())
            {
                var modelspace = (ACADDB.BlockTableRecord)
                    trans.GetObject(ACADDB.SymbolUtilityServices.GetBlockModelSpaceId(doc.Database), ACADDB.OpenMode.ForRead);

                var polylineIds = (from id in modelspace.Cast<ACADDB.ObjectId>()
                                   where acceptableTypes.Contains(id.ObjectClass)
                                   select id).ToList();

                foreach (var id in polylineIds)
                {
                    collection.Add(id);
                }


                trans.Commit();
                return collection;
            }
        }
    }
}
