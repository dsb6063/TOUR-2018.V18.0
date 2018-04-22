using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using PGA.Civil.Logging;

namespace C3DSurfacesDemo
{
    public class MapClean
    {

        public static void DrawingCleanUp(string layername,ObjectIdCollection polys)
        {

            if (String.IsNullOrEmpty(layername))
            {
                throw new ArgumentNullException(layername);
            }
           
            try
            {
              
                Autodesk.Gis.Map.Topology.Variable cadAction

                = new Autodesk.Gis.Map.Topology.Variable();
          
                string appfolder = PGA.Autodesk.Settings.AcadSettings.AppFolderScriptPath;
                 
                string filepath = System.IO.Path.Combine(appfolder,"MapClean",layername + ".dpf");

                if (!System.IO.File.Exists(filepath))
                    throw new FileNotFoundException(filepath);
                
                cadAction.LoadProfile(filepath);
             
                using (Autodesk.Gis.Map.Topology.TopologyClean cadCleanobj
                    = new Autodesk.Gis.Map.Topology.TopologyClean())
                {
                    cadCleanobj.Init(cadAction, null);
                    cadCleanobj.Start();
                    cadCleanobj.GroupNext();

                    while (!cadCleanobj.Completed)
                    {   
                            if (cadCleanobj.GroupErrorCount > 0)
                            {
                                try
                                {
                                    cadCleanobj.GroupFix();
                                }
                                catch 
                                {
                                    //No Need to Log//
                                } 
                                   
                            }
                            cadCleanobj.GroupNext();                      

                    }

                    cadCleanobj.End();

                }
            
            }
            catch (System.Exception ex)
            {
                ACADLogging.LogMyExceptions(String.Format("MapClean:{0}:{1}",layername, ex.Message));
            }
        }

        public static bool ProcessLineWork()
        {
            IList<string> layernames = new List<string>();
            Database db  = CivilApplicationManager.WorkingDatabase;
            Document doc =  Application.DocumentManager.MdiActiveDocument;
            ObjectIdCollection polys = new ObjectIdCollection();

            try
            {

                layernames = BBC.Common.AutoCAD.LayerManager.GetLayerNames(db);
                polys = GetSelection();

                if (layernames.Count == 0) return false;


                foreach (string layer in layernames)
                {
                    if (layer.Length <= 3) continue;
                    else if (layer.StartsWith("S"))
                    {
                        DrawingCleanUp(layer, polys);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ACADLogging.LogMyExceptions(string.Format(
                    "MapClean:{0} {1}", ex.Message
                                  ,ex.InnerException)
                );
            }

            return false;
        }

        public static ObjectIdCollection GetSelection()
        {
            ObjectIdCollection oids = new ObjectIdCollection();
            Autodesk.AutoCAD.EditorInput.SelectionSet selection  = null;
            selection = BBC.Common.AutoCAD.SelectionManager.GetSelectionSet("");

            foreach (ObjectId obj in selection.GetObjectIds())
            {
                if (obj.IsErased)
                    continue;
                else
                    oids.Add(obj);
            }
            return oids;
        }
    }
}
