using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using global::Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using COMS=PGA.MessengerManager;
using Autodesk.AutoCAD.Runtime;
using PGA.Database;
using PGA.DataContext;
using PGA.Sv.PostAudit;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace PGA.ReportWriter
{
    public class ReportWriter
    {
        public static string output = "";
        public static int greens = 0;
        public static int teebox = 0;
        public static int fairway = 0;
        public static int bunker = 0;

        private List<ObjectId> GetSurfaceEntityIDs(Autodesk.AutoCAD.DatabaseServices.Database db)

        {
            List<ObjectId> ids = null;

            try
            {

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (BlockTable) tran.GetObject(db.BlockTableId, OpenMode.ForRead);

                    var br =
                        (BlockTableRecord) tran.GetObject(tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                    var b = br.Cast<ObjectId>();

                    #region Other Types

                    //==============search certain entity========================//

                    //"LINE" for line

                    //"LWPOLYLINE" for polyline

                    //"CIRCLE" for circle

                    //"INSERT" for block reference

                    //...

                    //We can use "||" (or) to search for more then one entity types

                    //============================================================//


                    //Use lambda extension method

                    //ids = b.Where(id => id.ObjectClass.DxfName.ToUpper() == "LINE" ||

                    //    id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE").ToList<ObjectId>();

                    #endregion

                    ids = (from id in b
                        where id.ObjectClass.DxfName.ToUpper() == "AECC_TIN_SURFACE"
                        select id).ToList();


                    tran.Commit();
                }

            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }
        private List<ObjectId> GetPolylineEntityIDs(Autodesk.AutoCAD.DatabaseServices.Database db)

        {
            List<ObjectId> ids = null;

            try
            {

                using (var tran = db.TransactionManager.StartTransaction())

                {
                    var tbl =
                        (BlockTable)tran.GetObject(db.BlockTableId, OpenMode.ForRead);

                    var br =
                        (BlockTableRecord)tran.GetObject(tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                    var b = br.Cast<ObjectId>();

                    #region Other Types

                    //==============search certain entity========================//

                    //"LINE" for line

                    //"LWPOLYLINE" for polyline

                    //"CIRCLE" for circle

                    //"INSERT" for block reference

                    //...

                    //We can use "||" (or) to search for more then one entity types

                    //============================================================//


                    //Use lambda extension method

                    //ids = b.Where(id => id.ObjectClass.DxfName.ToUpper() == "LINE" ||

                    //    id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE").ToList<ObjectId>();

                    #endregion

                    ids = (from id in b
                           where id.ObjectClass.DxfName.ToUpper() == "LWPOLYLINE"
                           select id).ToList();


                    tran.Commit();
                }

            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }

            return ids;
        }


        private void FeatureCount()
        {
            _write(String.Format("\nReport Date: {0}",DateTime.Now));

            _write("\n***********************");

            _write("\nTIN Feature Inventory:");

            _write("\n***********************");
            _write("\n-----------------------");
            _write("\nGREENS  = GREENS_COUNT  ");
            _write("\nFAIRWAY = FAIRWAY_COUNT ");
            _write("\nBUNKER  = BUNKER_COUNT  ");
            _write("\nTEE BOX = TEEBOX_COUNT ");
            _write("\n-----------------------");
        }

        private void writeTinSurfaceProperties(TinSurfaceProperties p)

        {
            _write("\n-----------------------");

            _write("\nTIN Surface Properties:");

            _write("\n-----------------------");

            _write("\nMin Triangle Area: " + Math.Round(p.MinimumTriangleArea,3));

            _write("\nMin Triangle Length: " + Math.Round(p.MinimumTriangleLength,3));

            _write("\nMax Triangle Area: " + Math.Round(p.MaximumTriangleArea,3));

            _write("\nMax Triangle Length: " + Math.Round(p.MaximumTriangleLength,3));

            _write("\nNumber of Triangles: " + p.NumberOfTriangles);

            _write("\n-----------------------");

        }

        private void writeTerrainProperties(TerrainSurfaceProperties p)

        {
            _write("\n---------------------------");

            _write("\nTerrain Surface Properties:");

            _write("\n---------------------------");

            _write("\nMin Grade/Slope: " + Math.Round(p.MinimumGradeOrSlope,4));

            _write("\nMax Grade/Slope: " + Math.Round(p.MaximumGradeOrSlope,4));

            _write("\nMean Grade/Slope: " + Math.Round(p.MeanGradeOrSlope,4));

            _write("\n2D Area: " + Math.Round(p.SurfaceArea2D,3));

            _write("\n3D Area: " + Math.Round(p.SurfaceArea3D,3));

            _write("\n-----------------------");

        }

        private void writeGeneralProperites(GeneralSurfaceProperties p)

        {
            _write("\n-------------------");

            _write("\nGeneral Properties:");

            _write("\n-------------------");

            _write("\nMin X: " + Math.Round(p.MinimumCoordinateX,3));

            _write("\nMin Y: " + Math.Round(p.MinimumCoordinateY,3));

            _write("\nMin Z: " + Math.Round(p.MinimumElevation,3));

            _write("\nMax X: " + Math.Round(p.MaximumCoordinateX,3));

            _write("\nMax Y: " + Math.Round(p.MaximumCoordinateY,3));

            _write("\nMax Z: " + Math.Round(p.MaximumElevation,3));

            _write("\nMean Elevation: " + Math.Round(p.MeanElevation,3));

            _write("\nNumber of Points: " + p.NumberOfPoints);

            _write("\n-----------------------");

        }
        #region Testing Implementation
        //[CommandMethod("PGA-Report",CommandFlags.Session)]
        //public  void StartReportText()
        //{
        //    InitializeReportWriter(
        //        @"C:\Users\Daryl-Win-8\Downloads\RTJ Grand National_819H11\TINSURF-20160224-131244\HOLE11");
        //} 
        #endregion

        public void InitializeReportWriter(string fullpath)
        {
            try
            {
                output  = "";
                greens  = 0;
                fairway = 0;
                bunker  = 0;
                teebox  = 0;
                output = Path.GetDirectoryName(fullpath);
                output = Path.Combine(output, "Report_QAQC.txt");
                TinSurfacePropertiesWriter();
            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        public struct FeatureItem
        {
            public string name;
            public bool active;
            public string layer;
            public int count;
        }


        public string GetShortLayerName(string name)
        {
            try
            {
                using (DatabaseCommands commands = new DatabaseCommands())
                {
                    var item = commands.GetFeatureIndexByName(name);
                    return item;
                }
            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return "";
        }

        public IList<FeatureItem> ProcessFeatureItems()
        {
            try
            {
                var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
                using (Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database)
                {
                    var features = new List<FeatureItem>();
                    var pid = GetPolylineEntityIDs(db);
                    var temp = new List<Polyline>();

                    foreach (ObjectId item in pid)
                    {
                        using (OpenCloseTransaction tr = db.TransactionManager.StartOpenCloseTransaction())
                        {
                            Polyline pline = (Polyline)tr.GetObject(item, OpenMode.ForWrite);
                            temp.Add(pline);
                            tr.Commit();
                        }
                    }

                    var distpolys = temp.Select(p=>p.Layer).Distinct();

                    foreach (var item in distpolys)
                    {
                        var feature = new FeatureItem();
                        feature.active = true;
                        feature.layer  = item;
                        feature.name   = GetShortLayerName(item);
                        feature.count = temp.Count(p => p.Layer == item);
                        features.Add(feature);
                    }
                    return features;
                }
            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
            return null;
        }

        public void InitializeXMLReportWriter(string fullpath,DateTime date,string hole, string drawingname)
        {
            var course = "";
            var code   = "";
            var name = drawingname ?? "Feature_Report.xml";
            name = name.ToUpper();
            name   = name.Replace (".DWG", ".XML");
            output = Path.GetDirectoryName(fullpath);
            output = Path.Combine(output, name);
            using (DatabaseCommands cmd = new DatabaseCommands())
            {
                var settings = cmd.GetSettingsByDate(date);
                course = settings.CourseName;
                code = settings.CourseCode;
            }
            using (XmlWriter writer = XmlWriter.Create(output))
            {
                writer.WriteStartDocument();
                writer.WriteComment("PGA TOUR Outputs");
                writer.WriteComment("Active Feature Report");
                //writer.WriteComment(String.Format("Task Id: {0}", date));
                writer.WriteComment(String.Format("Date Created: {0}", DateTime.Now));
                writer.WriteComment(String.Format("Course Name: {0}  Code: {1}",course,code));
                writer.WriteComment(String.Format("Hole: {0}", hole));

                writer.WriteComment("Note: The Feature Count May Not Be Accurate!");

                writer.WriteStartElement("Features");

                var features = ProcessFeatureItems();

                foreach (FeatureItem feature in features)
                {
                    writer.WriteStartElement("Feature");

                    writer.WriteElementString("ID", feature.name.ToString());   // <-- These are new
                    writer.WriteElementString("Layer", feature.layer);
                    writer.WriteElementString("Count", feature.count.ToString());
                    writer.WriteElementString("Active", feature.active.ToString());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }


        }

        public void TinSurfacePropertiesWriter()

        {

            var doc = Active.MdiDocument;
            var db = doc.Database;

            var surfaces = GetSurfaceEntityIDs(db);
            
            //Write PlaceHolder
             
            FeatureCount();

            foreach (var surfid in surfaces)
            {

                ObjectId surfaceId = surfid;

                if (ObjectId.Null == surfaceId)

                {
                    return; // We don't have a surface; we can't continue.
                }

                using (var tran = db.TransactionManager.StartTransaction())

                {

                    TinSurface surface = surfaceId.GetObject(

                        OpenMode.ForRead) as TinSurface;

                    if (surface.Name.Contains("GREEN"))
                        greens++;
                    else if (surface.Name.Contains("FAIRWAY"))
                        fairway++;
                    else if (surface.Name.Contains("BUNKER"))
                        bunker++;
                    else if (surface.Name.Contains("TEEBOX") ||
                             surface.Name.Contains("TEE"))
                        teebox++;
                   

                    _write("\nInformation for TIN Surface: " + surface.Name);

                    try
                    {
                        writeGeneralProperites(surface.GetGeneralProperties());
                        writeTerrainProperties(surface.GetTerrainProperties());
                        writeTinSurfaceProperties(surface.GetTinProperties());
                    }
                    catch (System.Exception ex)
                    {
                        COMS.MessengerManager.LogException(ex);
                        _write("Failure retrieving Surface Properties!");
                    }

                    tran.Commit();
                }

            }

            AddFooterInfo();
          

        }

        private void AddFooterInfo()
        {
            // output = Path.Combine(output, "Report.txt");

            string fileout = "";
            using (StreamReader reader = new StreamReader(output))
            {
                fileout = reader.ReadToEnd();

                reader.Close();

                if (fileout.Contains("GREENS_COUNT"))
                    fileout= fileout.Replace("GREENS_COUNT", greens.ToString());
                if (fileout.Contains("FAIRWAY_COUNT"))
                    fileout = fileout.Replace("FAIRWAY_COUNT", fairway.ToString());
                if (fileout.Contains("BUNKER_COUNT"))
                    fileout = fileout.Replace("BUNKER_COUNT", bunker.ToString());
                if (fileout.Contains("TEEBOX_COUNT"))
                    fileout = fileout.Replace("TEEBOX_COUNT", teebox.ToString());
            }
            using (StreamWriter writer = File.CreateText(output))
            {
                writer.Write(fileout);
                writer.Close();
            }
        }

        public void ExportSurfaceInfo(string _output, List<string> lines)
        {
            try
            {

                //output = Path.Combine(_output, "Report.txt");

                using (StreamWriter writer = File.AppendText(output))
                {
                    foreach (var line in lines)
                    {
                        writer.WriteLine(String.Format("{0}: {1}", line, 0));
                    }
                    writer.Close();
                }

            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.LogException(ex);
            }
        }

        public void _write( string lines)
        {
            try
            {

               // output = Path.Combine(output, "Report.txt");
               
                using (StreamWriter writer = File.AppendText(output))
                {
                    
                    writer.WriteLine(lines);
                   
                    writer.Close();
                }

            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.ShowMessageAndLog("cmdExportLogs_Click" + ex.Message);
            }
        }

        public void WriteNewFile(string lines)
        {
            try
            {

                // output = Path.Combine(output, "Report.txt");

                using (StreamWriter writer = File.AppendText(output))
                {
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }
                    writer.Close();
                }

            }
            catch (System.Exception ex)
            {
                COMS.MessengerManager.ShowMessageAndLog("cmdExportLogs_Click" + ex.Message);
            }
        }
    }
}
