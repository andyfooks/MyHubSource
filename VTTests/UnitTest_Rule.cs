using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualTrainer;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.IO;
using RazorEngine.Templating;
using System.Text;
using System.Net.Mail;
using AJG.VirtualTrainer.Services;
using VirtualTrainer.Utilities;

namespace VTTests
{
    [TestClass]
    public class UnitTest_Rule
    {
        private string TestProjectId = "A896DA6D-F233-42DE-8CF6-A3D21FF42C6D";
        
        [TestMethod]
        public void TestProcessingRule()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Project project = ctx.Project.Where(a => a.ProjectUniqueKey == new Guid(TestProjectId)).Include("Rules").FirstOrDefault();
                project.ExecuteAllRules(ctx, true);
            }
        }
        [TestMethod]
        public void TestProcessingRuleNoSaveToDB()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Project project = ctx.Project.Where(a => a.ProjectUniqueKey == new Guid(TestProjectId)).Include("Rules").FirstOrDefault();
                List<BreachLog> breaches = project.ExecuteAllRules(ctx, false);
            }
        }
        [TestMethod]
        public void TestRuleConfig()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                RuleConfiguration config = ctx.RuleConfigurations.FirstOrDefault();
                config.LoadContextRuleParticipants(ctx);

                List<User> allUsers = ctx.User.ToList();
                User Andy = ctx.User.Where(a => a.Name == "Andy Fooks").FirstOrDefault();

                allUsers.Remove(Andy);
            }
        }
    }
}
