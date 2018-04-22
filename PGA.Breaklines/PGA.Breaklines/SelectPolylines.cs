using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.EditorInput;
using global::Autodesk.AutoCAD.Runtime;

namespace PGA.Breaklines
{
    public interface ISelectPolylines
    {
        ObjectIdCollection GetIdsByTypeCollection();
    }
    public class SelectPolylines: ISelectPolylines
    {
        public ObjectIdCollection GetIdsByTypeTypeValue(params string[] types)
        {

            // Get the document
            var doc = Application.DocumentManager.MdiActiveDocument;

            // Get the editor to make the selection
            Editor oEd = doc.Editor;

            // Add our or operators so we can grab multiple types.
            IList<TypedValue> typedValueSelection = new List<TypedValue> {
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>")
                };

            // We will need to insert our requested types into the collection.
            // Since we knew we would have to insert they types between the operators..
            // I used a Enumerable type which gave me that functionality. (IListf<T>)
            foreach (var type in types)
                typedValueSelection.Insert(1, new TypedValue(Convert.ToInt32(DxfCode.Start), type));

            SelectionFilter selectionFilter = new SelectionFilter(typedValueSelection.ToArray());

            // because we have to.. Not really sure why, I assume this is our only access point
            // to grab the entities that we want. (I am open to being corrected)
            PromptSelectionResult promptSelectionResult = oEd.SelectAll(selectionFilter);

            // return our new ObjectIdCollection that is "Hopefully" full of the types that we want.
            return new ObjectIdCollection(promptSelectionResult.Value.GetObjectIds());
        }

        public  ObjectIdCollection GetIdsByTypeCollection()
        {
            ObjectIdCollection collection = new ObjectIdCollection();

            Func<Type, RXClass> getClass = RXObject.GetClass;

            // You can set this anywhere
            var acceptableTypes = new HashSet<RXClass>
            {
                getClass(typeof (Polyline)),
                getClass(typeof (Polyline2d)),
                getClass(typeof (Polyline3d))
            };

            var doc = Application.DocumentManager.MdiActiveDocument;
            using (var trans = doc.TransactionManager.StartOpenCloseTransaction())
            {
                var modelspace = (BlockTableRecord)
                    trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(doc.Database), OpenMode.ForRead);

                var polylineIds = (from id in modelspace.Cast<ObjectId>()
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
