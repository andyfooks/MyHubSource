using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VirtualTrainer;

namespace VTTests
{
    [TestClass]
    public class UnitTest1
    {
        private string TestProjectId = "A896DA6D-F233-42DE-8CF6-A3D21FF42C6D";
        
        [TestMethod]
        public void TestEmailBreaches()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Project project = ctx.Project.FirstOrDefault();

                string bodyTemplatePath = ConfigurationManager.AppSettings[AppSettingsEnum.EmailRazorTemplateBodyPath.ToString()]; ;
                string subjectTemplatePath = ConfigurationManager.AppSettings[AppSettingsEnum.EmailRazorTemplateSubjectPath.ToString()];
                string attachmentTemplatePath = ConfigurationManager.AppSettings[AppSettingsEnum.EmailRazorTemplateAttachmentPath.ToString()];
                project.ExecuteEscalationsFramework(ctx, false, bodyTemplatePath, subjectTemplatePath, attachmentTemplatePath);
            }
        }

        [TestMethod]
        public void TestDateFormat()
        {
            List<BreachLog> logs = new List<BreachLog>();
            
            logs.Where(a => a.UserId == 1);

            string d = DateTime.Now.ToString("dd MMM yyy");
            double i = 1;

            if(double.TryParse("10", out i))
            {

            }

            string dt2 = DateTime.Now.ToString("dd/MM/yyy");

            DateTime dt = DateTime.Now;
            if (DateTime.TryParse("27/06/2016 ", out dt))
            {

            }

        }
    }
}
