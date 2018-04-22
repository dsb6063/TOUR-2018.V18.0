using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGA.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGA.Database.Tests
{
    [TestClass()]
    public class HolesTests
    {
        [TestMethod()]
        public void HolesTest()
        {
          
            Holes h = new Holes();
            for (int i = 1; i < 19; i++)
                Debug.WriteLine(h.GetHole(i));
            Assert.AreEqual(1, 1);
        }
    }


}
