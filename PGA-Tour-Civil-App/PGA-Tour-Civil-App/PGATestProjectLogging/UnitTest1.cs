
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PGATestProjectLogging
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           
            PGA.Common.Logging.ACADLogging.LogMyExceptions("Test");
            Assert.Fail("TEst Failed");
           
        }
    }
}
