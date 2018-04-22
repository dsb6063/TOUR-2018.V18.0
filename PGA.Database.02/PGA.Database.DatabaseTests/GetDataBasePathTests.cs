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
        public void CreateIfNotExistsTest()
        {
            string value = GetDataBasePath.DbFileName;

            if (String.IsNullOrEmpty(value))Assert.Fail();
        }

        [TestMethod()]
        public void CreateIfNotExistsTest1()
        {
            string value = GetDataBasePath.PathDb;

            if (String.IsNullOrEmpty(value)) Assert.Fail();
        }

        [TestMethod()]
        public void CreateIfNotExistsTest2()
        {
            string value = GetDataBasePath.PathDb;
            string res =
            GetDataBasePath.CreateIfNotExists(value);


        }
        [TestMethod()]
        public void CreateIfNotExistsTes3()
        {
            string value = GetDataBasePath.DbFileName;
            string res =
            GetDataBasePath.GetAppPath(value);

        }
    }
}