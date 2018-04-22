using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGA.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGA.DataContext;

namespace PGA.Database.Tests
{
    [TestClass()]
    public class DatabaseCommandsTests
    {
        DatabaseCommands _commands = new DatabaseCommands();

        [TestMethod()]
        public void GetGlobalDWGPathTest()
        {
            var res = _commands.GetGlobalDWGPath();
            Assert.AreEqual(1, 1);
        }

        [TestMethod()]
        public void GetFullDWGPathTest()
        {
            DateTime time = Convert.ToDateTime("12/30/2015 1:42 AM");
            time = time.AddSeconds(12);
            var res = _commands.GetGlobalDWGPath();
            Assert.AreEqual(1, 1);
        }

        [TestMethod()]
        public void LoadandProcessPolysTest()
        {

            var res = _commands.LoadandProcessPolys();
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadandProcessPolysTest1()
        {
            var res = _commands.LoadandProcessPolys();

            Assert.Fail();
        }

        [TestMethod()]
        public void LoadandProcessPolysTest2()
        {
            var res = DateTime.Now.ToFileTime();

            Assert.Fail();
        }
    }
}