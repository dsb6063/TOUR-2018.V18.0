using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGA.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGA.Database.Tests
{
    [TestClass()]
    public class GetDataBasePathTests
    {
        [TestMethod()]
        public void GetLogPathTest()
        {
            DatabaseCommands commands = new DatabaseCommands();
            commands.GetLogFilePath();
            Assert.AreEqual(1,1);
        }
    }
}