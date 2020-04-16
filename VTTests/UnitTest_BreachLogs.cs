using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualTrainer;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using System.Diagnostics;

namespace VTTests
{
    /// <summary>
    /// Summary description for UnitTestUser
    /// </summary>
    [TestClass]
    public class UnitTest_Breaches
    {
        private string TestProjectId = "A896DA6D-F233-42DE-8CF6-A3D21FF42C6D";

        #region [ Default class bumf ]

        public UnitTest_Breaches() { }

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

        private List<BreachLog> PopulateBreachTeamOrOfficeRoleUserId(List<BreachLog> logs, RoleEnum role, VirtualTrainerContext ctx)
        {
            List<BreachLog> returnBreachLogs = new List<BreachLog>();
            switch (role)
            {
                case RoleEnum.BranchManager:
                case RoleEnum.QualityAuditor:
                case RoleEnum.RegionalManager:
                    var officeBreachResults = from b in logs
                             join o in ctx.Office on b.OfficeId equals o.Id
                             join officeperm in ctx.OfficePermissions on o.Id equals officeperm.OfficeId
                             where officeperm.RoleId == (int)role
                             select new { breachLog = b, ManagerID = officeperm.UserId };
                    foreach (var result in officeBreachResults)
                    {
                        switch(role)
                        {
                            case RoleEnum.BranchManager:
                                result.breachLog.OfficeManagerUserID = result.ManagerID;
                                break;
                            case RoleEnum.QualityAuditor:
                                result.breachLog.QAUserID = result.ManagerID;
                                break;
                            case RoleEnum.RegionalManager:
                                result.breachLog.RegionalManagerUserID = result.ManagerID;
                                break;
                        }
                    }
                    returnBreachLogs = officeBreachResults.Select(a => a.breachLog).ToList();
                    break;
                case RoleEnum.TeamLead:
                    var teamBreachResults = from b in logs
                             join t in ctx.Team on b.TeamId equals t.Id
                             join teamperm in ctx.TeamPermissions on t.Id equals teamperm.TeamId
                             where teamperm.RoleId == (int)role
                             select new { breachLog = b, ManagerID = teamperm.UserId };
                    foreach (var result in teamBreachResults)
                    {
                        switch (role)
                        {
                            case RoleEnum.BranchManager:
                                result.breachLog.OfficeManagerUserID = result.ManagerID;
                                break;
                            case RoleEnum.QualityAuditor:
                                result.breachLog.QAUserID = result.ManagerID;
                                break;
                            case RoleEnum.RegionalManager:
                                result.breachLog.RegionalManagerUserID = result.ManagerID;
                                break;
                        }
                    }
                    returnBreachLogs = teamBreachResults.Select(a => a.breachLog).ToList();
                    break;
            }

            return returnBreachLogs.Count() == 0 ? logs : returnBreachLogs;
        }

        [TestMethod]
        public void Test_a()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Stopwatch sw = new Stopwatch();

                List<BreachLog> logs = ctx.BreachLogs.ToList();
                sw.Start();
                List<BreachLog> returnLogs = PopulateBreachTeamOrOfficeRoleUserId(logs, RoleEnum.BranchManager, ctx);

                foreach (var log in returnLogs)
                {

                }

                sw.Stop();
                TimeSpan elapsed = sw.Elapsed;
                int SA = 0;

                //var re = from b in logs
                //         join o in ctx.Office on b.OfficeId equals o.Id
                //         join officeperm in ctx.OfficePermissions on o.Id equals officeperm.OfficeId
                //         where officeperm.RoleId == (int)RoleEnum.BranchManager
                //         select new { breachLog = b, ManagerID = officeperm.UserId };
                ////select new
                ////{
                ////    officeID = o.Id,
                ////    officeManagerUserId = officeperm.UserId
                ////} into x
                ////group x by new { x } into y
                ////select new
                ////{
                ////    officeID = y.FirstOrDefault().officeID,
                ////    officeManagerUserId = y.FirstOrDefault().officeManagerUserId
                ////};
                //int count = re.Count();
                //try
                //{
                //    foreach(var result in re)
                //    {
                //        result.breachLog.OfficeManagerUserID = result.ManagerID;
                //    }


                //    List<BreachLog> returnLogs = re.Select(a => a.breachLog).ToList();

                //    //foreach (var log in logs)
                //    //{
                //    //    if (log.OfficeId.HasValue)
                //    //    {
                //    //        log.OfficeManagerUserID = re.ToList().Where(a => a.officeID == log.OfficeId).FirstOrDefault().officeManagerUserId;
                //    //    }
                //    //}
                //}
                //catch (Exception ex)
                //{
                //}
            }
        }

        [TestMethod]
        public void Test_EFStoredProc()
        {
            using (var db = new VirtualTrainerContext())
            {
                db.Database.Initialize(force: false);

                // Create a SQL command to execute the sproc
                var cmd = db.Database.Connection.CreateCommand();
                DbParameter parameter = cmd.CreateParameter();
                parameter.Direction = System.Data.ParameterDirection.Input;
                parameter.ParameterName = "EscalationsFrameworkId";
                parameter.Value = 1;
                parameter.DbType = System.Data.DbType.Int32;
               
                cmd.Parameters.Add(parameter);
                cmd.CommandText = "[dbo].[GetBreachesForEscalationConfig]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                try
                {

                    db.Database.Connection.Open();
                    // Run the sproc 
                    var reader = cmd.ExecuteReader();

                    // Read Blogs from the first result set
                    var blogs = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<BreachLog>(reader, "BreachLogs", MergeOption.AppendOnly);

                   

                    foreach (var item in blogs)
                    {
                        Console.WriteLine(item.TeamLeadUserID);
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
        }
        [TestMethod]
        public void Test_UserBreach()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                //ctx.Database.SqlQuery<User>("");
                User user = ctx.User.Where(o => o.Id == 91).Include("BreachLogs").FirstOrDefault();
                List<BreachLog> returnBreaches = new List<BreachLog>();
                List<BreachLog> logs = user.BreachLogs.OrderBy(a => a.TimeStamp).ToList();
                List<BreachLog> logsSingle = user.BreachLogs.OrderBy(a => a.TimeStamp).GroupBy(b => b.ContextRef).Select(c=> {
                    c.LastOrDefault().BreachLiveContextRefCount = c.Count();
                    c.LastOrDefault().FirstBreachDate = c.FirstOrDefault().TimeStamp;
                    c.LastOrDefault().User = null;
                    return c.LastOrDefault();
                }).ToList();


                //foreach (var log in logs)
                //{
                //    string key = log.Key;
                //    int count = log.Count();
                //    DateTime firstDate = log.FirstOrDefault().TimeStamp;
                //}
                //int logCount = office.BreachLogs.Count();
            }
        }
    }
}
