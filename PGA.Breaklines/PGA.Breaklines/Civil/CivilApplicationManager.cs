using System;

using global::Autodesk.AutoCAD.ApplicationServices;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;

namespace PGA.Breaklines
{
    public class CivilApplicationManager
    {
        public static CivilDocument ActiveCivilDocument 
        {
            get { return CivilApplication.ActiveDocument; }
        }

        public static Autodesk.AutoCAD.DatabaseServices.Database WorkingDatabase
        {
            get { return HostApplicationServices.WorkingDatabase; }
        }

        public static Transaction StartTransaction()
        {
            return HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction();
        }
    }
}