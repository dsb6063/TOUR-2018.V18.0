using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGA.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGA.DataContext;

namespace PGA.Database.Tests
{
    [TestClass()]
    public class DatabaseCommandsTests
    {
        [TestMethod()]
        public void LoadandProcessPolysTest()
        {
            DatabaseCommands commands = new DatabaseCommands();
           // var dwgs = commands.LoadandProcessPolys();
            var test = commands.GetTasksProjectName();
            //var tasks = (from p in context.TaskManager
            //             where p.Started.GetValueOrDefault(false).Equals(true) &&
            //                   p.Completed.Value == null
            //             //p.Completed.GetValueOrDefault(false).Equals(false)
            //             select p).FirstOrDefault();



            Assert.AreEqual(1,1);
        }

        [TestMethod()]
        public void TestDatabaseMethod()
        {
            //polylines = GetIdsByTypeTypeValue("POLYLINE", "LWPOLYLINE", "POLYLINE2D", "POLYLINE3d");
            //IQueryable<string> excludedfeatures=null;
            //using (DatabaseCommands commands = new DatabaseCommands())
            //{
                DatabaseCommands commands = new DatabaseCommands();
                IList<ExcludedFeatures> excludedfeatures;
                excludedfeatures = commands.GetAllExcludedFeatures_V2();
                {
                    foreach (var str in excludedfeatures)
                    {
                        Debug.WriteLine(str.Handle.Trim());
                        //ObjectId compare = GetObjectId(str.Trim());
                        //if (polylines.Contains(compare))
                        //{
                        //    polylines.Remove(compare);
                        //    PGA.Database.DatabaseLogs.FormatLogs("Filtered Polylines");
                        //}
                    }
                }
            //}
        }

        [TestMethod()]
        public void TestDatabaseMethodSkipDXF()
        {
             
            DatabaseCommands commands = new DatabaseCommands();
            IList<ExcludedFeatures> excludedfeatures;
            var datetime = (from p in commands.GetSettingsCustom()
                select p.DateStamp).Distinct().FirstOrDefault();
            var resultdxf = commands.IsSkipDXFByDate(date:Convert.ToDateTime(datetime));
            
        }
    }
}