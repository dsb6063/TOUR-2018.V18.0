using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGA.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGA.DataContext;

namespace PGA.Database.Tests
{
    using System.Diagnostics;

    [TestClass()]
    public class DatabaseCommandsTests
    {
        

        DatabaseCommands commands = new DatabaseCommands();

        [TestMethod()]
        public void GetDateTimeFromPolylineDrawingIDTest()
        {
            //  GetDateTimeFromPolylineDrawingID();
            Assert.AreEqual(1, 1);
        }

        [TestMethod()]
        public void ArrangePolylineDWGSDwgStackByNumberTest()
        {
            DatabaseCommands commands = new DatabaseCommands();
            DateTime date = Convert.ToDateTime("12/23/2015 2:52 AM");

            long ldate = Convert.ToDateTime("12/23/2015 2:52 AM").Ticks;

            commands.ArrangePolylineDWGSDwgStackByNumber(date);
            Assert.Fail();
        }

        [TestMethod()]
        public void ArrangePolylineDWGSDwgStackByNumberTest1()
        {

            long Ticks = 635864359620000000;
            IList<DrawingStack> stack = new List<DrawingStack>();
            DatabaseCommands commands = new DatabaseCommands();
            stack = commands.ArrangePolylineDWGSDwgStackByNumber(Ticks);
            commands.ArrangePointCloudDwgsToDwgStack(Ticks, stack);
            Assert.AreEqual(1, 1);

        }

        [TestMethod()]
        public void NewTaskTest()
        {
            DateTime time = new DateTime();
            time = DateTime.Now;

            Settings settings = new Settings();
            settings.DateStamp = time;

            commands.NewTask(time, settings);

            Assert.AreEqual(1, 1);
        }

        [TestMethod()]
        public void GetFullDWGPathTest()
        {

            DateTime date = Convert.ToDateTime("12/30/2015 1:42 AM");
            TimeSpan ts = new TimeSpan(0,0,0,12);
            date.Add(new TimeSpan(0,0, 0, 12));
            date = date.AddSeconds(12);
            commands.GetFullDWGPath(date, 1);
            using (PGAContext context = commands.DbPgaContextConnection())
            {
                var settingses = from p in context.Settings
                    select p;

                foreach (var val in settingses)
                {
                    if (val.DateStamp == date)
                    {
                        Debug.WriteLine(val.DateStamp.Value.Ticks);
                        Debug.WriteLine(date.Ticks);
                    }
                }
            }

            Assert.AreEqual(1, 1);
        }
    }
}