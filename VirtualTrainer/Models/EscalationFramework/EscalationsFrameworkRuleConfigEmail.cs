using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace VirtualTrainer
{
    public abstract class EscalationsFrameworkRuleConfigEmail : EscalationsFrameworkRuleConfig
    {
        #region [EF Mapped Properties]

        //public EscalationsFrameworkEmailCollationOption EmailCollationOption { get; set; }
        public string SentFromEmail { get; set; }
        public string SentFromUserName { get; set; }
        public string EmailBodyTemplate { get; set; }
        public string EmailSubjectTemplate { get; set; }
        public string EmailAttachementTemplate { get; set; }
        public bool? CreateAttachementUsingTemplate { get; set; }
        public bool AttachExcelOfBreaches { get; set; }

        #endregion

        #region [ Not Mapped EF Properties ]

        public override void RunEscalationConfiguration(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            base.LoadRequiredContextObjects(ctx);
            this.SentFromEmail = string.IsNullOrEmpty(this.Project.ProjectSenderEmail) ? this.SentFromEmail : this.Project.ProjectSenderEmail;
            this.SentFromUserName = string.IsNullOrEmpty(this.Project.ProjectSenderDisplayName) ? this.SentFromUserName : this.Project.ProjectSenderDisplayName;
        }

        [NotMapped]
        public string InternalSentFromUserName
        {
            get
            {
                return string.IsNullOrEmpty(this.SentFromUserName) ? ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromName.ToString()] : this.SentFromUserName;
            }
        }
        [NotMapped]
        public string InternalSentFromEmail
        {
            get
            {
                return string.IsNullOrEmpty(this.SentFromEmail) ? ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromEmailAddress.ToString()] : this.SentFromEmail;
            }
        }

        #endregion

        #region [ Methods ]
                

        #endregion

        #region [ Internal Methods ]

        internal void SendEmail(string bdyHtml, List<EmailHtml> emailInformations, string toAddress, string subject, EscalationsFrameworkConfigEmailRazorModel razorModel, VirtualTrainerContext ctx, SmtpClient SmtpClient)
        {
            string pathToExcelDoc = string.Empty;
            List<string> documentPaths = new List<string>();

            try
            {
                // using (SmtpClient SmtpServer = new SmtpClient())
                using (MailMessage mail = new MailMessage())
                {
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = bdyHtml;
                    mail.To.Add(toAddress);

                    if (razorModel != null && razorModel.AttachExcelofBreaches)
                    {
                        pathToExcelDoc = GenerateExcelDocOFBreaches(razorModel, ctx);
                        mail.Attachments.Add(new Attachment(pathToExcelDoc));
                    }

                    // If we want to add the pdf.
                    foreach (EmailHtml htmlContent in emailInformations)
                    {
                        if (htmlContent.AddHtmlAttachment && !string.IsNullOrEmpty(htmlContent.AttachmentHtmlContent))
                        {
                            string docName = string.IsNullOrEmpty(htmlContent.HTMLAttachmentName) ? string.Format("Attachment_{0}.pdf", Guid.NewGuid().ToString()) : htmlContent.HTMLAttachmentName;
                            string pathToHtmlDoc = DirectoryHelper.GetSystemSaveLocationForDocument(docName);

                            documentPaths.Add(pathToHtmlDoc);

                            SavePdfToFile(pathToHtmlDoc, htmlContent, HtmlFromEmailIncludionEnum.AttachmentOnly);

                            //AsposeHelper aspose = new AsposeHelper();
                            //aspose.SaveHtmlToPDFFile(htmlContent.AttachmentHtmlContent, pathToHtmlDoc, htmlContent.pdfHeight, htmlContent.pdfWidth);

                            mail.Attachments.Add(new Attachment(pathToHtmlDoc));
                        }
                    }

                    SmtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                SystemLog log = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(log);
                ctx.SaveChanges();
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(pathToExcelDoc))
                {
                    File.Delete(pathToExcelDoc);
                }
                foreach(string path in documentPaths)
                {
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
        }
        internal void SendEmail(string emailSubject, string toEmailAddress, string emailBody, string attachmentPath, SmtpClient smtpClient, VirtualTrainerContext ctx)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.Subject = emailSubject;
                    mail.IsBodyHtml = true;
                    mail.Body = emailBody;
                    mail.To.Add(toEmailAddress);
                    mail.From = new MailAddress(this.InternalSentFromEmail, this.InternalSentFromUserName);

                    if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                    {
                        mail.Attachments.Add(new Attachment(attachmentPath));
                    }

                    smtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                SystemLog log = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(log);
                ctx.SaveChanges();
                throw;
            }
        }
        internal void SavePdfToFile(string docPath, EmailHtml EmailInformation, HtmlFromEmailIncludionEnum contentToInclude)
        {
            // Deal with the main PDF file
            AsposeHelper asposeHelper = new AsposeHelper();
            if (string.IsNullOrEmpty(EmailInformation.PDFMargins) || EmailInformation.PDFMargins.Split(',').Count() != 4)
            {
                asposeHelper.SaveHtmlToPDFFile(GetEmailContentAsHtml(EmailInformation, contentToInclude), docPath, EmailInformation.pdfHeight, EmailInformation.pdfWidth);
            }
            else
            {
                asposeHelper.SaveHtmlToPDFFile(GetEmailContentAsHtml(EmailInformation, contentToInclude), docPath, EmailInformation.pdfHeight, EmailInformation.pdfWidth,
                    EmailInformation.GetPDFMarginValue(Margin.Top), EmailInformation.GetPDFMarginValue(Margin.Right),
                    EmailInformation.GetPDFMarginValue(Margin.Bottom), EmailInformation.GetPDFMarginValue(Margin.Left));
            }

            // Now the header.
            if (EmailInformation.PDFAddHeader)
            {
                asposeHelper.AddHeadersFooters(docPath, AsposeHelper.HeaderFooterEnum.header,
                    EmailInformation.GetPDFHeaderMarginValue(Margin.Top), EmailInformation.GetPDFHeaderMarginValue(Margin.Right),
                    EmailInformation.GetPDFHeaderMarginValue(Margin.Bottom), EmailInformation.GetPDFHeaderMarginValue(Margin.Left),
                    EmailInformation.PDFHeaderText);
            }

            // Now the footer.
            if (EmailInformation.PDFAddFooter)
            {
                asposeHelper.AddHeadersFooters(docPath, AsposeHelper.HeaderFooterEnum.footer,
                    EmailInformation.GetPDFFooterMarginValue(Margin.Top), EmailInformation.GetPDFFooterMarginValue(Margin.Right),
                    EmailInformation.GetPDFFooterMarginValue(Margin.Bottom), EmailInformation.GetPDFFooterMarginValue(Margin.Left),
                    EmailInformation.PDFFooterText);
            }
        }
        internal void SendEmail(EmailHtml emailInformation, EscalationsFrameworkConfigEmailRazorModel razorModel, VirtualTrainerContext ctx, SmtpClient SmtpClient)
        {
            string pathToExcelDoc = string.Empty;
            string pathToHtmlDoc = string.Empty;

            try
            {
                // using (SmtpClient SmtpServer = new SmtpClient())
                using (MailMessage mail = new MailMessage())
                {
                    mail.Subject = emailInformation.Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = emailInformation.Body;
                    mail.To.Add(emailInformation.EmailTo);
                    
                    if (razorModel != null && razorModel.AttachExcelofBreaches)
                    {
                        pathToExcelDoc = GenerateExcelDocOFBreaches(razorModel, ctx);
                        mail.Attachments.Add(new Attachment(pathToExcelDoc));
                    }

                    // If we want to add the pdf.
                    if (emailInformation.AddHtmlAttachment && !string.IsNullOrEmpty(emailInformation.AttachmentHtmlContent))
                    {
                        string docName = string.IsNullOrEmpty(emailInformation.HTMLAttachmentName) ? string.Format("Attachment_{0}.pdf", Guid.NewGuid().ToString()) : emailInformation.HTMLAttachmentName;
                        pathToHtmlDoc = DirectoryHelper.GetSystemSaveLocationForDocument(docName);

                        SavePdfToFile(pathToHtmlDoc, emailInformation, HtmlFromEmailIncludionEnum.AttachmentOnly);
                        //AsposeHelper aspose = new AsposeHelper();
                        //aspose.SaveHtmlToPDFFile(emailInformation.AttachmentHtmlContent, pathToHtmlDoc, emailInformation.pdfHeight, emailInformation.pdfWidth);

                        mail.Attachments.Add(new Attachment(pathToHtmlDoc));
                    }

                    SmtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                SystemLog log = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(log);
                ctx.SaveChanges();
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(pathToExcelDoc))
                {
                    File.Delete(pathToExcelDoc);
                }
                if (!string.IsNullOrEmpty(pathToHtmlDoc))
                {
                    File.Delete(pathToHtmlDoc);
                }
            }
        }

        internal void SendEmail(string body, string toEmail, string subject, EscalationsFrameworkConfigEmailRazorModel razorModel, VirtualTrainerContext ctx, SmtpClient SmtpClient)
        {
            string pathToExcelDoc = string.Empty;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = body;
                    mail.To.Add(toEmail);
                    mail.From = new MailAddress(this.InternalSentFromEmail, this.InternalSentFromUserName);

                    if (razorModel != null && razorModel.AttachExcelofBreaches)
                    {
                        pathToExcelDoc = GenerateExcelDocOFBreaches(razorModel, ctx);
                        mail.Attachments.Add(new Attachment(pathToExcelDoc));
                    }

                    SmtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                SystemLog log = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(log);
                ctx.SaveChanges();
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(pathToExcelDoc))
                {
                    File.Delete(pathToExcelDoc);
                }
            }
        }
        internal enum HtmlFromEmailIncludionEnum
        {
            All,
            BodyOnly,
            AttachmentOnly
        }
        internal string GetEmailContentAsHtml(EmailHtml emailHtml, HtmlFromEmailIncludionEnum contentToInclude)
        {
            string returnHtml = string.Empty;
            switch (contentToInclude)
            {
                case HtmlFromEmailIncludionEnum.All:
                    returnHtml = string.Format("<h1>Subject:</h1> <br /> {0} <br /> <h1>Body:</h1> <br /> {1} <br /> <h1>Attachment:</h1> {2} <br />", emailHtml.Subject, emailHtml.Body, emailHtml.AttachmentHtmlContent);
                    break;
                case HtmlFromEmailIncludionEnum.AttachmentOnly:
                    returnHtml = string.Format("{0}", emailHtml.AttachmentHtmlContent);
                    break;
                case HtmlFromEmailIncludionEnum.BodyOnly:
                    returnHtml = string.Format("{0}", emailHtml.Body);
                    break;
            }
            return returnHtml;
        }
        internal class EmailHtml
        {
            #region [ Properties ]

            public string Body { get; set; }
            public string Subject { get; set; }
            public string EmailTo { get; set; }
            public string EmailFrom { get; set; }
            public string AttachmentHtmlContent { get; set; }
            public bool SendEMail { get; set; }
            public string GroupByKey { get; set; }
            public bool AddHtmlAttachment { get; set; }
            public string HTMLAttachmentName { get; set; }
            public int? pdfWidth { get; set; }
            public int? pdfHeight { get; set; }
            public string PDFMargins { get; set; }
            public bool PDFAddHeader
            {
                get
                {
                    return !string.IsNullOrEmpty(this.PDFHeaderText);
                }
            }
            public string PDFHeaderText { get; set; }
            public string PDFHeaderMargins { get; set; }
            public bool PDFAddFooter
            {
                get
                {
                    return !string.IsNullOrEmpty(this.PDFFooterText);
                }
            }
            public string PDFFooterText { get; set; }
            public string PDFFooterMargins { get; set; }

            #endregion

            #region [ Public Methods ]

            public int? GetPDFMarginValue(Margin marginDescriptor)
            {
                return GetMarginValue(marginDescriptor, this.PDFMargins);
            }
            public int? GetPDFHeaderMarginValue(Margin marginDescriptor)
            {
                return GetMarginValue(marginDescriptor, this.PDFHeaderMargins);
            }
            public int? GetPDFFooterMarginValue(Margin marginDescriptor)
            {
                return GetMarginValue(marginDescriptor, this.PDFFooterMargins);
            }

            #endregion

            #region [ private methods ]

            private int? GetMarginValue(Margin marginDescriptor, string margins)
            {
                // Check there is a value added
                if (!string.IsNullOrEmpty(margins))
                {
                    List<string> marginParts = margins.Split(',').ToList();
                    int parsedNumber = 0;
                    bool parseToStringSuccess = false;
                    // There should be 4 parts
                    if (marginParts.Count() == 4)
                    {
                        switch (marginDescriptor)
                        {
                            case Margin.Top:
                                parseToStringSuccess = int.TryParse(marginParts[0], out parsedNumber);
                                break;
                            case Margin.Left:
                                parseToStringSuccess = int.TryParse(marginParts[1], out parsedNumber);
                                break;
                            case Margin.Bottom:
                                parseToStringSuccess = int.TryParse(marginParts[2], out parsedNumber);
                                break;
                            case Margin.Right:
                                parseToStringSuccess = int.TryParse(marginParts[3], out parsedNumber);
                                break;
                        }
                        if (parseToStringSuccess)
                        {
                            return parsedNumber;
                        }
                    }
                }
                return null;
            }

            #endregion
        }
        internal enum Margin
        {
            Top,
            Left,
            Bottom,
            Right
        }
        internal string GenerateExcelDocOFBreaches(EscalationsFrameworkConfigEmailRazorModel razorModel, VirtualTrainerContext ctx)
        {
            ExcelUtlity eu = new ExcelUtlity();
            //List<BreachLogExcelReport> logsForExcel = razorModel.BreachLogs.Select(a => a.getBreachDetailsForExcel(ctx)).ToList();

            List<BreachLogExcelReport> logsForExcel = razorModel.BreachLogs.Select(b => new BreachLogExcelReport()
            {
                BreachCount = b.GetBreachCountForContextRef(ctx),
                BreachDisplayHtml = b.BreachDisplayText,
                BreachField1 = b.RuleBreachFieldOne,
                BreachField2 = b.RuleBreachFieldTwo,
                BreachField3 = b.RuleBreachFieldThree,
                BreachField4 = b.RuleBreachFieldFour,
                BreachField5 = b.RuleBreachFieldFive,
                BreachField6 = b.RuleBreachFieldSix,
                BreachField7 = b.RuleBreachFieldSeven,
                ContextRef = b.ContextRef,
                TimeStamp = b.TimeStamp,
                ProjectId = b.ProjectId.ToString(),
                RuleConfigName = b.RuleConfigurationName,
                RuleConfigStoredProc = b.StoredProecdureName,
                UserName = b.UserName,
                RuleName = b.RuleName
            }).ToList();


            List<objectProperty> objectProperties = ExcelUtlity.GetOrderedObjectPropertyDetails<BreachLogExcelReport>();
            return eu.WriteObjectsToExcel<BreachLogExcelReport>(logsForExcel, objectProperties);
        }

        internal new void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("BreachSources").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachSources").Load();
            }
            base.LoadRequiredContextObjects(ctx);
        }

        #endregion
    }
}