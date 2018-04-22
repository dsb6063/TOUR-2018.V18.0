using Autodesk.AutoCAD.Runtime;
using PGA.Common.Logging.MapClean;
using System;

namespace PGA.MapClean
{
	public class MainClass
	{
		public MainClass()
		{
		}

		[CommandMethod("PGA-MapClean")]
		public static void MapClean()
		{
			try
			{
				PGA.MapClean.MapClean.ProcessLineWork();
			}
			catch (Exception exception)
			{
				ACADLogging.LogMyExceptions(string.Concat("MapClean", exception.Message));
			}
		}
	}
}