using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using C3DSurfacesDemo;

namespace PGA.Surfaces.Civil3D
{
    public static class Utilities
    {
        public static  Polyline GetPolyFromObjId(Autodesk.AutoCAD.DatabaseServices.ObjectId oid)
        {
            using (Autodesk.AutoCAD.DatabaseServices.Database db = CivilApplicationManager.WorkingDatabase)
            {

                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        DBObject obj;

                        try
                        {
                            obj = tr.GetObject(oid, OpenMode.ForRead);
                        }
                        catch (NullReferenceException e)
                        {
                            PGA.MessengerManager.MessengerManager.LogException(e);
                            return null;
                        }

                        Polyline lwp = obj as Polyline;

                        if (lwp != null)
                        {
                            // Is Polyline Closed
                            if (lwp.Closed)
                            {
                                return lwp;
                            }
                        }

                        tr.Commit();
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception e)
                {
                    PGA.MessengerManager.MessengerManager.LogException(e);
                }
            }

            return null;
        }

    }
}
