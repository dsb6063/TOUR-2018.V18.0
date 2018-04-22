using System;
using global::Autodesk.AutoCAD.DatabaseServices;
using global::Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.ApplicationServices;

namespace C3DApplicationDocuments
{
    public class CivilApplicationManager
    {
        public static CivilDocument ActiveCivilDocument 
        {
            get { return CivilApplication.ActiveDocument; }
        }

        public static Database WorkingDatabase
        {
            get { return HostApplicationServices.WorkingDatabase; }
        }

        public static Transaction StartTransaction()
        {
            return HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction();
        }

      

    }
}