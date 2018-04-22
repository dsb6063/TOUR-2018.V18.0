using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Exception = System.Exception;

namespace TestFeatureLineIssues
{
    public class Program
    {
        ObjectIdCollection _polyCollection = new ObjectIdCollection();

        Breaklines breaklines           = new Breaklines(DateTime.Now, null);
        SelectPolylines selectPolylines = new SelectPolylines();

        /// <summary>
        /// Adds the break lines synch.
        /// </summary>
        /// 
        [CommandMethod("PGA-AddBreaklines", CommandFlags.Session)]
        public void AddBreakLinesSynch()
        {
            try
            {
             
                if (_polyCollection.Count == 0)
                    _polyCollection = selectPolylines.
                        GetIdsByTypeTypeValue("POLYLINE",
                            "LWPOLYLINE", "POLYLINE2D");


                var surfaceId = breaklines.FindCivilTinSurface("All");
                var siteId    = breaklines.GetSiteId(surfaceId); //Use New 

                breaklines._originalPolys = _polyCollection;


                var _siteId      = siteId;
                var _surObjectId = surfaceId;
                var TotalDWGs    = _polyCollection.Count;
                AddBreaklinesByCommandLine();
            }
            catch (Exception ex)
            {
            }
        }


        public void AddBreaklinesByCommandLine()
        {
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    HideAmbientSettingsShowEvntVwer();
                    var surfaceId = breaklines.FindCivilTinSurface("All");
                    var poly3dCollection = new List<Polyline3d>();

                    var _surObjectId = surfaceId;

                    foreach (ObjectId poly in _polyCollection)
                    {           

                        var _siteId = breaklines.GetNewSiteId();

                        using (Transaction tr = CivilApplicationManager.StartTransaction())
                        {
                            //*************ADD BREAKLINES HERE************//

                            var featurelineId = breaklines.AddCivil2016BreaklineByTrans
                                          (surfaceId, poly, _siteId, null);

                            //*************ADD BREAKLINES HERE************//

                            if (featurelineId == ObjectId.Null)
                                continue;
                            //*************ADD ELEVATIONS HERE************//
                            breaklines.AddCivil2016ElevationsToFeature
                                (_surObjectId, featurelineId, _siteId, null);
                            //*************ADD ELEVATIONS HERE************//

                            tr.Commit();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        public void HideAmbientSettingsShowEvntVwer()
        {
            try
            {
                using (Application.DocumentManager.MdiActiveDocument.LockDocument())
                {
                    using (var doc = CivilDocument.GetCivilDocument(CivilApplicationManager.WorkingDatabase))
                    {
                        var ambientSettings = doc.Settings.DrawingSettings.AmbientSettings;
                        ambientSettings.General.ShowEventViewer.Value = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


    }
}
