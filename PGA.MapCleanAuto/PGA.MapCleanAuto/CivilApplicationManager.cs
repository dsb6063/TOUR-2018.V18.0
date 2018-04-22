using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using System;

namespace PGA.MapCleanAuto
{
	public class CivilApplicationManager
	{
		public static CivilDocument ActiveCivilDocument
		{
			get
			{
				return CivilApplication.get_ActiveDocument();
			}
		}

		public static Database WorkingDatabase
		{
			get
			{
				return HostApplicationServices.get_WorkingDatabase();
			}
		}

		public CivilApplicationManager()
		{
		}

		public static Transaction StartTransaction()
		{
			return HostApplicationServices.get_WorkingDatabase().get_TransactionManager().StartTransaction();
		}
	}
}