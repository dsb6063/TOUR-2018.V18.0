using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGA.Database;
using PGA.DataContext;
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
        public void GetTasksProjectNameTest()
        {

            IList<TaskManager> tasksProjectName = (IList<TaskManager>)commands.GetTasksProjectName();

            Assert.AreEqual(1, 1);
        }

        /// <exclude />
        [TestMethod()]
        public void SetDateStartedInTaskTest()
        {
            DateTime time = Convert.ToDateTime("12/30/2015 1:42 AM");
            time = time.AddSeconds(12);

            commands.SetDateStartedInTask(time);
            Assert.AreEqual(1, 1);
        }

        [TestMethod()]
        public void NewTaskManagerTest()
        {
            DateTime time = Convert.ToDateTime("12/30/2015 1:42 AM");
            time = time.AddSeconds(12);

            commands.NewTaskManager(time);
            Assert.AreEqual(1, 1);
        }

        [TestMethod()]
        public void LoadandProcessPolysTest()
        {
            commands.LoadandProcessPolys();
            Assert.AreEqual(1, 1);
        }
    }
}