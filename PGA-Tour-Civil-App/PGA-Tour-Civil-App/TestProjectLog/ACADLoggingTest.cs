#region

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGA.Common.Logging;

#endregion

namespace TestProjectLog
{
    /// <summary>
    ///     This is a test class for ACADLoggingTest and is intended
    ///     to contain all ACADLoggingTest Unit Tests
    /// </summary>
    [TestClass]
    public class ACADLoggingTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///     A test for LogMyExceptions
        /// </summary>
        [TestMethod]
        public void LogMyExceptionsTest()
        {
            var message = string.Empty; // TODO: Initialize to an appropriate value
            var exception = string.Empty; // TODO: Initialize to an appropriate value
            PGA.MessengerManager.MessengerManager.AddLog(message, exception);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for LogMyExceptions
        /// </summary>
        [TestMethod]
        public void LogMyExceptionsTest1()
        {
            var message = string.Empty; // TODO: Initialize to an appropriate value
            PGA.MessengerManager.MessengerManager.AddLog(message);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for LogMyExceptions
        /// </summary>
        [TestMethod]
        public void LogMyExceptionsTest2()
        {
            var message = string.Empty; // TODO: Initialize to an appropriate value
            Exception ex = null; // TODO: Initialize to an appropriate value
            PGA.MessengerManager.MessengerManager.AddLog(message, ex);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///     A test for SystemTracerLogMyExceptions
        /// </summary>
        [TestMethod]
        [DeploymentItem("PGA.Common.Logging.dll")]
        public void SystemTracerLogMyExceptionsTest()
        {
            var message = string.Empty; // TODO: Initialize to an appropriate value
            ACADLogging_Accessor.SystemTracerLogMyExceptions(message);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }


        /// <summary>
        ///     A test for SystemTracerLogMyExceptions
        /// </summary>
        [TestMethod]
        [DeploymentItem("PGA.Common.Logging.dll")]
        public void SystemTracerLogMyExceptionsTest2()
        {
            var message = string.Format("Test my values{0}", "ARG0"); // TODO: Initialize to an appropriate value
            ACADLogging_Accessor.SystemTracerLogMyExceptions(message);
            Assert.IsTrue(true);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }


        /// <summary>
        ///     A test for LogTextTraceWriter
        /// </summary>
        [TestMethod]
        public void LogTextTraceWriterTest()
        {
            var message = "Test My Values"; // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = ACADLogging.LogTextTraceWriter(message);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BulkWriteLogTextTraceWriterTest()
        {
            var message = "Test My Values"; // TODO: Initialize to an appropriate value
            var expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            for (var i = 0; i < 30; i++)
            {
                actual = ACADLogging.LogTextTraceWriter(message);
                Assert.AreEqual(expected, actual);
            }
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion
    }
}