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
        DatabaseCommands commands = new DatabaseCommands();
        [TestMethod()]
        public void GetTemplatePathTest()
        {
            commands.GetTemplatePath();
            Assert.AreEqual(1,1);
        }
    }
}