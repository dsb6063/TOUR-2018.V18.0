using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.Topology;
using BBC.Common.AutoCAD;
using PGA.Autodesk.Settings;
using PGA.Common.Logging.MapClean;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PGA.MapClean
{
	public class MapClean
	{
		public MapClean()
		{
		}

		private static ObjectIdCollection AddToList(ObjectIdCollection Mpolys)
		{
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			foreach (ObjectId mpoly in Mpolys)
			{
				objectIdCollection.Add(mpoly);
			}
			return objectIdCollection;
		}

		public void DrawingCleanUp(string layername, ObjectIdCollection polys)
		{
			Exception exception;
			try
			{
				if (string.IsNullOrEmpty(layername))
				{
					throw new ArgumentNullException(layername);
				}
				Variable variable = new Variable();
				string appFolderScriptPath = AcadSettings.get_AppFolderScriptPath();
				string str = Path.Combine(appFolderScriptPath, "MapClean", string.Concat(layername, ".dpf"));
				if (!File.Exists(str))
				{
					throw new FileNotFoundException(str);
				}
				variable.LoadProfile(str);
				TopologyClean topologyClean = new TopologyClean();
				try
				{
					try
					{
						topologyClean.Init(variable, polys);
					}
					catch (MapTopologyException mapTopologyException1)
					{
						MapTopologyException mapTopologyException = mapTopologyException1;
						ACADLogging.LogMyExceptions(string.Format("MapClean:{0}:{1}", layername, string.Concat("Initialization Error ", mapTopologyException.Message)));
						return;
					}
					topologyClean.Start();
					topologyClean.GroupNext();
					while (!topologyClean.get_Completed())
					{
						ACADLogging.LogMyExceptions(string.Format("MapClean:{0}:{1}", (int)topologyClean.get_GroupErrorCount(), "cadCleanobj.GroupErrorCount"));
						if (topologyClean.get_GroupErrorCount() > 0)
						{
							try
							{
								topologyClean.GroupFix();
							}
							catch (Exception exception1)
							{
								exception = exception1;
								ACADLogging.LogMyExceptions(string.Format("MapClean:{0}:{1}", layername, "Group Fix"));
							}
						}
						topologyClean.GroupNext();
					}
					topologyClean.End();
				}
				finally
				{
					if (topologyClean != null)
					{
						topologyClean.Dispose();
					}
				}
			}
			catch (Exception exception2)
			{
				exception = exception2;
				ACADLogging.LogMyExceptions(string.Format("MapClean:{0}:{1}", layername, exception.Message));
			}
		}

		private static ObjectIdCollection FilterResults(string layername, ObjectIdCollection polys)
		{
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			foreach (ObjectId poly in polys)
			{
				if (PGA.MapClean.MapClean.IsOnLayer(layername, poly))
				{
					objectIdCollection.Add(poly);
				}
			}
			return objectIdCollection;
		}

		public static ObjectIdCollection GetSelection()
		{
			ObjectIdCollection objectIdCollection;
			ObjectIdCollection objectIdCollection1 = new ObjectIdCollection();
			SelectionSet selectionSet = null;
			selectionSet = SelectionManager.GetSelectionSet("Select Polylines: ", "*", "LWPolyline");
			if (selectionSet.get_Count() != 0)
			{
				ObjectId[] objectIds = selectionSet.GetObjectIds();
				for (int i = 0; i < (int)objectIds.Length; i++)
				{
					ObjectId objectId = objectIds[i];
					if (!objectId.get_IsErased())
					{
						objectIdCollection1.Add(objectId);
					}
				}
				objectIdCollection = objectIdCollection1;
			}
			else
			{
				objectIdCollection = objectIdCollection1;
			}
			return objectIdCollection;
		}

		public static ObjectIdCollection GetSelection(string layername)
		{
			ObjectIdCollection objectIdCollection;
			ObjectIdCollection objectIdCollection1 = new ObjectIdCollection();
			SelectionSet objectsOnLayer = null;
			objectsOnLayer = SelectionManager.GetObjectsOnLayer(layername);
			if (objectsOnLayer.get_Count() != 0)
			{
				ObjectId[] objectIds = objectsOnLayer.GetObjectIds();
				for (int i = 0; i < (int)objectIds.Length; i++)
				{
					ObjectId objectId = objectIds[i];
					if (!objectId.get_IsErased())
					{
						objectIdCollection1.Add(objectId);
					}
				}
				objectIdCollection = objectIdCollection1;
			}
			else
			{
				objectIdCollection = objectIdCollection1;
			}
			return objectIdCollection;
		}

		public static bool IsOnLayer(string layername, ObjectId oPolyline)
		{
			bool flag;
			Database workingDatabase = CivilApplicationManager.WorkingDatabase;
			Application.get_DocumentManager().get_MdiActiveDocument();
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			IList<string> strs = new List<string>()
			{
				layername
			};
			IList<string> strs1 = new List<string>()
			{
				"LWPolyline"
			};
			Transaction transaction = workingDatabase.get_TransactionManager().StartTransaction();
			try
			{
				objectIdCollection = AcadUtilities.GetObjectIdsOnLayer(workingDatabase, transaction, strs, strs1);
				transaction.Commit();
			}
			finally
			{
				if (transaction != null)
				{
					transaction.Dispose();
				}
			}
			if (objectIdCollection.get_Count() != 0)
			{
				foreach (ObjectId objectId in objectIdCollection)
				{
					if (objectId == oPolyline)
					{
						flag = true;
						return flag;
					}
				}
				flag = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool ProcessLineWork()
		{
			bool flag;
			IList<string> strs = new List<string>();
			Database workingDatabase = CivilApplicationManager.WorkingDatabase;
			Application.get_DocumentManager().get_MdiActiveDocument();
			ObjectIdCollection objectIdCollection = new ObjectIdCollection();
			ObjectIdCollection list = new ObjectIdCollection();
			PGA.MapClean.MapClean mapClean = new PGA.MapClean.MapClean();
			try
			{
				strs = LayerManager.GetLayerNames(workingDatabase);
				objectIdCollection = PGA.MapClean.MapClean.GetSelection();
				if (objectIdCollection.get_Count() == 0)
				{
					flag = false;
				}
				else if (strs.Count != 0)
				{
					foreach (string str in strs)
					{
						list = PGA.MapClean.MapClean.AddToList(objectIdCollection);
						ACADLogging.LogMyExceptions(string.Format("MapClean:{0} {1}", "Starting layer: ", str));
						try
						{
							list = PGA.MapClean.MapClean.FilterResults(str, list);
							if (list.get_Count() == 0)
							{
								continue;
							}
						}
						catch
						{
							continue;
						}
						if ((!str.StartsWith("S") ? false : str.Length > 3))
						{
							mapClean.DrawingCleanUp(str, list);
						}
						ACADLogging.LogMyExceptions(string.Format("MapClean:{0} {1}", "Ending layer: ", str));
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				ACADLogging.LogMyExceptions(string.Format("MapClean:{0} {1}", exception.Message, exception.InnerException));
				flag = false;
				return flag;
			}
			return flag;
		}
	}
}