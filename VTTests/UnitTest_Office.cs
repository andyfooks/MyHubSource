using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualTrainer;
using System.Linq;
using System.Data.Entity;

namespace VTTests
{
    /// <summary>
    /// Summary description for UnitTestUser
    /// </summary>
    [TestClass]
    public class UnitTest_Office
    {
        private string TestProjectId = "A896DA6D-F233-42DE-8CF6-A3D21FF42C6D";
        #region [ Default class bumf ]

        public UnitTest_Office() { }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #endregion

        [TestMethod]
        public void Test_OfficeBreaches()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Office office = ctx.Office.Where(o => o.Id == 6).Include("BreachLogs").FirstOrDefault();
                int logCount = office.BreachLogs.Count();
            }
        }

        [TestMethod]
        public void TestGetOustandingBreachesByContextValueForNamedManager()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Office office = ctx.Office.Where(o => o.Id > 0).FirstOrDefault();
                User user = ctx.User.Where(u => u.Id == 1).FirstOrDefault();
                bool userismanager = office.IsUserAManager(ctx, user);
            }
        }
    }
}
