using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualTrainer;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace VTTests
{
    /// <summary>
    /// Summary description for UnitTestUser
    /// </summary>
    [TestClass]
    public class UnitTest_User
    {
        private string TestProjectId = "A896DA6D-F233-42DE-8CF6-A3D21FF42C6D";
        private string MicroServiceProjectID = "ffd61007-6872-4598-9687-9ddc48c29253";

        #region [ Default class bumf ]

        public UnitTest_User() { }

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
        public void GetOfficeUsersWithPerms()
        {
            Stopwatch sw = new Stopwatch();
            Guid projectGuid = new Guid(TestProjectId);
            Guid MSProjectGuid = new Guid(MicroServiceProjectID);
            using (var ctx = new VirtualTrainerContext())
            {
                sw.Start();

                //List<User> users2 = new List<User>();
                var users = (from u in ctx.User
                             join perms in ctx.SystemPermissions on u.Id equals perms.UserId
                             select new { user = u, permissions = perms } into up
                             group up by up.user.Id into ug
                             select new
                             {
                                 User = ug.FirstOrDefault().user,
                                 IsSystemAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SystemAdmin).Any(),
                             }).ToList();

                foreach (var u in users)
                {
                    if (u.IsSystemAdmin)
                    {
                        u.User.IsSystemAdmin = true;
                    }

                }
                sw.Stop();
            }
        }

        [TestMethod]
        public void GetSystemUsersWithPerms()
        {
            Stopwatch sw = new Stopwatch();
            int officeId = 11;
            using (var ctx = new VirtualTrainerContext())
            {
                sw.Start();

                //List<User> users2 = new List<User>();
                var users = (from u in ctx.User
                              join perms in ctx.OfficePermissions on u.Id equals perms.UserId
                              where perms.OfficeId == officeId
                              select new { user = u, permissions = perms } into up
                              group up by up.user.Id into ug
                              select new
                              {
                                  User = ug.FirstOrDefault().user,
                                  IsOfficeManager = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.BranchManager).Any(),
                                  IsQA = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.QualityAuditor).Any(),
                                  IsRM = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.RegionalManager).Any()
                              }).ToList();

                foreach (var u in users)
                {
                    if (u.IsOfficeManager)
                    {
                        u.User.IsOfficeManager = true;
                    }
                    if (u.IsQA)
                    {
                        u.User.IsOfficeQualityAuditor = true;
                    }
                    if (u.IsRM)
                    {
                        u.User.IsOfficeRegionalManager = true;
                    }
                }
                sw.Stop();
            }
        }

        [TestMethod]
        public void getProjectUsers()
        {
            Stopwatch sw = new Stopwatch();
            Guid projectGuid = new Guid(TestProjectId);
            Guid MSProjectGuid = new Guid(MicroServiceProjectID);
            using (var ctx = new VirtualTrainerContext())
            {
                sw.Start();

                //List<User> users2 = new List<User>();
                var users2 = (from u in ctx.User
                              join perms in ctx.ProjectPermissions on u.Id equals perms.UserId
                              where perms.ProjectId == MSProjectGuid
                              select new { user = u, permissions = perms } into up
                              group up by up.user.Id into ug
                              select new
                              {
                                  User = ug.FirstOrDefault().user,
                                  IsProjectAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.ProjectAdmin).Any(),
                                  IsMicroServiceAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.MicroService).Any()
                              }).ToList();

                foreach (var u in users2)
                {
                    if (u.IsProjectAdmin)
                    {
                        u.User.IsProjectAdmin = true;
                    }
                    if (u.IsMicroServiceAdmin)
                    {
                        u.User.IsMicroServiceMethodAccessUser = true;
                    }
                }
                sw.Stop();
            }
        }

        [TestMethod]
        public void TestGetOustandingBreachesByContextForUser()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                User user = ctx.User.Where(u => u.Name.ToLower().Contains("fooks")).FirstOrDefault();
                List<BreachLog> allLogs = user.GetAllBreachesByContextValue(ctx);
                List<BreachLog> logs = user.GetOutstandingBreachesByContextValue(ctx);
            }
        }
        [TestMethod]
        public void TestGetOustandingBreachesByContextValueForNamedManager()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Office office = ctx.Office.Where(o => o.Id > 0).FirstOrDefault();
                if (office.HasOfficeManager(ctx))
                {
                    User officeManager = office.GetOfficeManagers(ctx).FirstOrDefault();
                    List<BreachLog> logs = officeManager.GetOutstandingBreachesByContextValueForNamedManager(ctx, officeManager);
                }
            }
        }
        [TestMethod]
        public void TestGetOustandingBreachesByContextValueForNamedTeamLead()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Team team = ctx.Team.Where(o => o.Id > 0).FirstOrDefault();
                if (team.HasTeamLeads(ctx))
                {
                    User teamLead = team.GetTeamLeads(ctx).FirstOrDefault();
                    List<BreachLog> logs = teamLead.GetOutstandingBreachesByContextValueForNamedTeamLead(ctx, teamLead);
                }
            }
        }
        [TestMethod]
        public void TestGetMyTeams()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                User user = ctx.User.Where(u => u.Id == 1).FirstOrDefault();
                List<Team> myTeams = user.GetMyTeams(ctx);
            }
        }
    }
}
