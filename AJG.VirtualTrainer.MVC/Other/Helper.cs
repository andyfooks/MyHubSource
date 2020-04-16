using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC
{
    public class Helper
    {
        private string DefaultRazorBodyEmailTemplatePath = "~/Views/RazorEmailTemplates/Body";
        private string DefaultRazorSubjectEmailTemplatePath = "~/Views/RazorEmailTemplates/Subject";
        private string DefaultRazorAttachmentEmailTemplatePath = "~/Views/RazorEmailTemplates/Attachment";

        public string GetEmailRazorTemplateBodyPath(Controller c)
        {
            return GetEmailRazorTemplateBodyPath(c, null);
        }
        public string GetEmailRazorTemplateSubjectPath(Controller c)
        {
            return GetEmailRazorTemplateSubjectPath(c, null);
        }
        public string GetEmailRazorTemplateAttachmentPath(Controller c)
        {
            return GetEmailRazorTemplateAttachmentPath(c, null);
        }
        public string GetEmailRazorTemplateBodyPath(Controller c, Project p)
        {
            string pathToReturn = string.Empty;
            if (p != null)
            {
                pathToReturn = p.EmailRazorTemplateBodyPath;
            }

            if (string.IsNullOrEmpty(pathToReturn))
            {
                pathToReturn = ConfigurationManager.AppSettings[AppSettingsEnum.EmailRazorTemplateBodyPath.ToString()];
            }

            if (string.IsNullOrEmpty(pathToReturn))
            {
                pathToReturn = c.Server.MapPath(DefaultRazorBodyEmailTemplatePath);
            }
            return pathToReturn;
        }
        public string GetEmailRazorTemplateSubjectPath(Controller c, Project p)
        {
            string pathToReturn = string.Empty;
            if (p != null)
            {
                pathToReturn = p.EmailRazorTemplateSubjectPath;
            }

            if (string.IsNullOrEmpty(pathToReturn))
            {
                pathToReturn = ConfigurationManager.AppSettings[AppSettingsEnum.EmailRazorTemplateSubjectPath.ToString()];
            }

            if (string.IsNullOrEmpty(pathToReturn))
            {
                pathToReturn = c.Server.MapPath(DefaultRazorSubjectEmailTemplatePath);
            }
            return pathToReturn;
        }
        public string GetEmailRazorTemplateAttachmentPath(Controller c, Project p)
        {
            string pathToReturn = string.Empty;
            if (p != null)
            {
                pathToReturn = p.EmailRazorTemplateAttachmentPath;
            }

            if (string.IsNullOrEmpty(pathToReturn))
            {
                pathToReturn = ConfigurationManager.AppSettings[AppSettingsEnum.EmailRazorTemplateAttachmentPath.ToString()];
            }

            if (string.IsNullOrEmpty(pathToReturn))
            {
                pathToReturn = c.Server.MapPath(DefaultRazorAttachmentEmailTemplatePath);
            }
            return pathToReturn;
        }
    }
    public class GenericNameValueObject_DTO
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}