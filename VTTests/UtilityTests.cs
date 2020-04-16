using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualTrainer;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Data;
using RazorEngine;
using RazorEngine.Templating;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using VirtualTrainer.Models.MyHub;

namespace VTTests
{
    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void TestThis()
        {
            string number = "3343434343";
            number = number.Replace(".", "").Replace(" ", "").Replace("+44", "0").Trim();
            number = number.StartsWith("0") ? number : string.Format("0{0}", number);
        }
        [TestMethod]
        public void TestRegex()
        {
            Regex r = new Regex("^([0-9]*,){3}[0-9]*$");
            bool isMathc = r.IsMatch("3424324,2,3,43423");
        }

        [TestMethod]
        public void TestSomething()
        {
            using (var ctx = new VirtualTrainerContext())
            {
                Project p = ctx.Project.FirstOrDefault();
                List<User> users = p.GetAllUsers(ctx);

                Office o = ctx.Office.FirstOrDefault();
                users = o.GetOfficeStaff(ctx);

                Team t = ctx.Team.FirstOrDefault();
                users = t.GetTeamMembers(ctx);

                User u = ctx.User.Include("Aliases").FirstOrDefault();
            }
        }
        
        [TestMethod]
        public void TestMethod1()
        {
            ExcelUtlity eu = new ExcelUtlity();
            using (var ctx = new VirtualTrainerContext())
            {
                List<WorkSheetActivity> logs = ctx.WorkSheetMonthActivity.ToList();                
                List<objectProperty> objectProperties = ExcelUtlity.GetOrderedObjectPropertyDetails<WorkSheetActivity>();
                string path = eu.WriteObjectsToExcel<WorkSheetActivity>(logs, objectProperties);
            }
        }
        [TestMethod]
        public void TestmailSettings()
        {
            try
            {
                using (SmtpClient mystmptClient = new SmtpClient())
                using (MailMessage myMessage = new MailMessage())
                {
                    myMessage.Subject = "hello World";
                    myMessage.Body = "Some Stuff here";
                    myMessage.To.Add(new MailAddress("andyfooks@live.com"));
                    mystmptClient.Send(myMessage);
                }
            }
            catch (Exception ex)
            { }
        }
        private class SqlRowField
        {
            public string FieldName { get; set; }
            public string Value { get; set; }
            public Type FieldType { get; set; }
        }
        [TestMethod]
        public void Test()
        {
            try
            {
                using (var ctx = new VirtualTrainerContext())
                {
                    EscalationsFrameworkRuleConfigEmailRole config = ctx.EscalationsFrameworkRuleConfigEmailRoles.Where(a => a.Name == "Email Handlers").FirstOrDefault();

                    string emailBodyTemplateName = string.Format("emailBodyTemplateKey{0}{1}", config.ProjectId, config.Id);
                    string emailSubjectTemplateName = string.Format("emailSubjectTemplateKey{0}{1}", config.ProjectId, config.Id); ;
                    string emailHtmlBody = string.Empty;
                    string emailHtmlSubject = string.Empty;

                    //Engine.Razor.
                    
                    EscalationsFrameworkConfigEmailRazorModel razorModel = new EscalationsFrameworkConfigEmailRazorModel(ctx)
                    {
                        BreachLogs = new List<BreachLog>(),
                        Recipient = ctx.User.FirstOrDefault(),
                        EFEmailRuleConfig = config,
                        SendEmail = true,
                        SentFromName = config.InternalSentFromUserName,
                        AttachExcelofBreaches = false
                    };

                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(config.EmailBodyTemplate);

                    string b = System.Text.Encoding.ASCII.GetString(bytes);

                    b = b.Replace("  ", "\r\n");

                    foreach (char www in b)
                    {
                        int code = (int)www;
                        string codeand = string.Format("{0}:{1}", www, code);
                    }

                    string c = WebUtility.HtmlEncode(b);

                    b = b.Replace(Environment.NewLine, "\r\n");

                    MemoryStream stream = new MemoryStream(bytes);

                    string ba = new StreamReader(stream, System.Text.Encoding.UTF8).ReadToEnd();


                    string qa = File.OpenText(@"C:\Users\afooks\Source\Repos\vt\VirtualTrainer\EmailTemplates\HandlerRoleEmailTemplate.txt").ReadToEnd();


                     qa = Razor.Parse(b, razorModel);

                    if (Engine.Razor.IsTemplateCached(emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
                    {
                        emailHtmlBody = Engine.Razor.Run(emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                    }
                    else
                    {
                        emailHtmlBody = Engine.Razor.RunCompile(b, emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                    }

                    if (Engine.Razor.IsTemplateCached(emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
                    {
                        emailHtmlSubject = Engine.Razor.Run(emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                    }
                    else
                    {
                        emailHtmlSubject = Engine.Razor.RunCompile(config.EmailSubjectTemplate, emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                    }
                }
            }
            catch (Exception ex)
            {
                int a = 1;
            }
        }
    }
}
