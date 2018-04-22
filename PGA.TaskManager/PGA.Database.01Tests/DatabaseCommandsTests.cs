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
    public class DatabaseCommandsTests
    {
        DatabaseCommands commands = new DatabaseCommands();


        [TestMethod()]
        public void LoadDrawingStackTest()
        {

            DateTime time = Convert.ToDateTime("12/30/2015 1:42 AM");
            time = time.AddSeconds(12);
            var res = commands.LoadDrawingStack(time);

        }
    }
}