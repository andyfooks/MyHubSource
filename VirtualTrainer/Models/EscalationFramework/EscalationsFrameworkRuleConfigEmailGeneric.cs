using RazorEngine.Templating;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using AJG.VirtualTrainer.Helper;
using System.Text.RegularExpressions;
using AJG.VirtualTrainer.Helper.General;

namespace VirtualTrainer
{
    public class EscalationsFrameworkRuleConfigEmailGeneric : EscalationsFrameworkRuleConfigEmail
    {
        #region [ EF properties ]
        
        //[InverseProperty("EscalationsFrameworkRuleConfig")]
        //public ICollection<EscalationsEmailRecipient> Recipients { get; set; }
        public EscalationEmailGenericActionEnum Action { get; set; }
        public int? PdfPageWidth { get; set; }
        public int? PdfPageHeight { get; set; }
        public string DestinationPath { get; set; }
        public string EmailBreachColumnName { get; set; }
        public string EmailAddress { get; set; }
       // public EscalationInclusionEnum BreachInclusion { get; set; }
        public bool? ArchiveBreachesOnSuccess { get; set; }
        public string GroupBreachesColumnName { get; set; }
        public string PDFHeaderHtml { get; set; }
        public string PDFFooterHtml { get; set; }
        public string PDFHeaderMargins { get; set; }
        public string PDFFooterMargins { get; set; }
        public string PdfMargins { get; set; }

        #endregion

        #region [ NOT Mapped Properties ]

        [NotMapped]
        public string EmailActionName
        {
            get { return this.Action == null ? "" : this.Action.ToString(); }
        }
        [NotMapped]
        public string BreachInclusionName
        {
            get { return this.BreachInclusion == null ? "" : this.BreachInclusion.ToString(); }
        }
        [NotMapped]
        private string ServerRazorEmailTemplatePathBody { get; set; }
        [NotMapped]
        private string ServerRazorEmailTemplatePathSubject { get; set; }
        [NotMapped]
        private string ServerRazorEmailTemplatePathAttachment { get; set; }

        #endregion

        private List<EmailHtml> EmailsHTML { get; set; }

        public bool ValuesAreValid(out string returnValue)
        {
            returnValue = string.Empty;

            switch (this.Action)
            {
                //case EscalationEmailGenericActionEnum.AmalgamatedEmail_ToSpecifiedUser:
                case EscalationEmailGenericActionEnum.IndividualEmails_ToSpecifiedUser:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_All:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_AttachmentOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_BodyOnly:
                    // validate the email address has content and is valid.
                    if (string.IsNullOrEmpty(this.EmailAddress))
                    {
                        returnValue = "The EmailAddress field must have a value.";
                        return false;
                    }
                    else
                    {
                        Regex emailValidator = new Regex(@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$");
                        if (!emailValidator.IsMatch(this.EmailAddress))
                        {
                            returnValue = "The EmailAddress field must be a valid Email Address.";
                            return false;
                        }
                    }
                    break;
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_All:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_AttachmentOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_BodyOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_All:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_All:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_All:
               
                    // validate that the loaction path has content and is valid e.g. exists and can beaccessed.
                    if (string.IsNullOrEmpty(this.DestinationPath))
                    {
                        returnValue = "The DestinationPath field must have a value.";
                        return false;
                    }
                    else
                    {
                        string validationMessage = string.Empty;
                        if (!DirectoryHelper.ValidateDirectoryAccess(this.DestinationPath, ConfigurationManager.AppSettings[AppSettingsEnum.ProjectName.ToString()], out validationMessage))
                        {
                            returnValue = string.Format("Destination Path: {0}", validationMessage);
                            return false;
                        }
                    }
                    break;
                case EscalationEmailGenericActionEnum.IndividualEmails_UseEmailInBreachField:
                case EscalationEmailGenericActionEnum.IndividualEmails_UseEmailInBreachField_ManualSend:
                    // Validate that the entered breach field has value and exists.
                    if (string.IsNullOrEmpty(this.EmailBreachColumnName))
                    {
                        returnValue = "The Email Breach Column Name field must have a value.";
                        return false;
                    }
                    else
                    {
                        // Must be a member of the breachLog fields.
                        if (!BreachLog.GetMappingFields().Where(f => f.Equals(this.EmailBreachColumnName)).Any())
                        {
                            returnValue = "The Specified Breach Field Column is not valid.";
                            return false;
                        }
                    }

                    if(this.Action == EscalationEmailGenericActionEnum.IndividualEmails_UseEmailInBreachField_ManualSend)
                    {
                        // validate that the loaction pat has content and is valid e.g. exists and can beaccessed.
                        if (string.IsNullOrEmpty(this.DestinationPath))
                        {
                            returnValue = "The DestinationPath field must have a value.";
                            return false;
                        }
                        else
                        {
                            string validationMessage = string.Empty;
                            if (!DirectoryHelper.ValidateDirectoryAccess(this.DestinationPath, ConfigurationManager.AppSettings[AppSettingsEnum.ProjectName.ToString()], out validationMessage))
                            {
                                returnValue = string.Format("Destination Path: {0}", validationMessage);
                                return false;
                            }
                        }
                    }
                    
                    break;
            }

            if (string.IsNullOrEmpty(this.GroupBreachesColumnName))
            {
                returnValue = "The GroupBreachesColumnName Field must have a value.";
                return false;
            }
            else
            {
                if (!BreachLog.GetMappingFields().Where(f => f.Equals(this.GroupBreachesColumnName)).Any())
                {
                    returnValue = string.Format("GroupBreachesColumnName: {0}", "the Entered field value is not valid.");
                    return false;
                }
            }

            if (this.CreateAttachementUsingTemplate.GetValueOrDefault())
            {
                if (string.IsNullOrEmpty(this.EmailAttachementTemplate))
                {
                    returnValue = string.Format("EmailAttachementTemplate: {0}", "uncheck 'Attach Document Using template' or select an 'Attachment template'.");
                    return false;
                }
            }

            string marginsRegex = "^([0-9]*,){3}[0-9]*$";

            // Ensure PDF Margins values are valid if entered
            if (!string.IsNullOrEmpty(this.PdfMargins))
            {
                Regex r = new Regex(marginsRegex);
                if (!r.IsMatch(this.PdfMargins))
                {
                    returnValue = string.Format("PdfMargins: {0}", "If you enter a value it must match: integer,integer,integer,integer e.g. 10,0,100,12");
                    return false;
                }
            }
            // Ensure PDF Header Margins values are valid if entered
            if (!string.IsNullOrEmpty(this.PDFHeaderMargins))
            {
                Regex r = new Regex(marginsRegex);
                if (!r.IsMatch(this.PDFHeaderMargins))
                {
                    returnValue = string.Format("PdfHeaderMargins: {0}", "If you enter a value it must match: integer,integer,integer,integer e.g. 10,0,100,12");
                    return false;
                }
            }
            // Ensure PDF Footer Margins values are valid if entered
            if (!string.IsNullOrEmpty(this.PDFFooterMargins))
            {
                Regex r = new Regex(marginsRegex);
                if (!r.IsMatch(this.PDFFooterMargins))
                {
                    returnValue = string.Format("PDFFooterMargins: {0}", "If you enter a value it must match: integer,integer,integer,integer e.g. 10,0,100,12");
                    return false;
                }
            }

            returnValue = string.Empty;
            return true;
        }

        public override void RunEscalationConfiguration(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            base.RunEscalationConfiguration(ctx, updateTimeStamp, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
            RuleConfigEscalationsActionTakenLog log = new RuleConfigEscalationsActionTakenLog(this, ctx);
            try
            {
                // Instatiate the internal email list.
                EmailsHTML = new List<EmailHtml>();
                this.ServerRazorEmailTemplatePathBody = ServerRazorEmailTemplatePathBody;
                this.ServerRazorEmailTemplatePathSubject = ServerRazorEmailTemplatePathSubject;
                this.ServerRazorEmailTemplatePathAttachment = ServerRazorEmailTemplatePathAttachment;

                // Load the context objects
                LoadRequiredContextObjects(ctx);

                if (this.Schedule.IsScheduledToRun(ctx, this.LastRunDate))
                {
                    if (this.IsActive)
                    {
                        // Get the breach logs
                        List<BreachLog> breachLogs = GetBreachLogs(ctx, this.BreachInclusion);

                        // Only execute if there are any Breaches.
                        if (breachLogs.Any())
                        {
                            using (SmtpClient SmtpClient = new SmtpClient())
                            {
                                // Process breaches
                                ProcessBreaches(breachLogs, ctx, SmtpClient);
                                 //EmailUser(recipient, breachLogs, ctx, SmtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                            }
                            log.Success = true;
                            log.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
                        }
                        else
                        {
                            log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigNoBreachLogs;
                        }
                    }
                    else
                    {
                        log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigNotActive;
                    }
                }
                else
                {
                    log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigNotScheduledToRun;
                }

                if (updateTimeStamp)
                {
                    this.LastRunDate = DateTime.Now;
                }
                log.Success = true;
            }
            catch (Exception ex)
            {
                // Log Exception
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                // update Rule
                log.ErrorLogEntry = errorLog;
                log.ErrorMessage = ex.Message;
                log.Success = false;
                log.ExecutionOutcome = EscalationsActionTakenLogOutcome.Failure;
            }
            finally
            {
                log.TimeStamp = DateTime.Now;
                log.Finish = DateTime.Now;
                this.ActionTakenLog.Add(log);
                ctx.SaveChanges();
            }
        }
        public new void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            //if (!ctx.Entry(this).Collection("Recipients").IsLoaded)
            //{
            //    ctx.Entry(this).Collection("Recipients").Load();
            //}

            base.LoadRequiredContextObjects(ctx);
        }

        #region [ Private Methods ]

        private void ProcessBreaches(List<BreachLog> breachLogs, VirtualTrainerContext ctx, SmtpClient SmtpClient)
        {
            // Load all the templates results first.
            GetEmailsHtml(breachLogs, ctx);
            string dateTime = DateTime.Now.ToString("ddMMMyyyy_HHmm");
            StringBuilder amalgamatedHtml = new StringBuilder();

            switch (this.Action)
            {
                case EscalationEmailGenericActionEnum.IndividualEmails_UseEmailInBreachField:
                    // We need to send an email per Group - person - (Based on the specified field)
                    foreach (EmailHtml emailHtml in this.EmailsHTML)
                    {
                        base.SendEmail(emailHtml, null, ctx, SmtpClient);
                    }
                    break;
                case EscalationEmailGenericActionEnum.IndividualEmails_UseEmailInBreachField_ManualSend:
                    // We want to create the email bodies and headers but not send them yet. The user can look at the email history table and send manually.
                    SmtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    SmtpClient.PickupDirectoryLocation = this.DestinationPath;
                    SmtpClient.EnableSsl = false;
                    foreach (EmailHtml emailHtml in this.EmailsHTML)
                    {
                        base.SendEmail(emailHtml, null, ctx, SmtpClient);
                    }
                    break;
                case EscalationEmailGenericActionEnum.IndividualEmails_ToSpecifiedUser:
                    // We want to create an email per grouping but send to specified user.
                    foreach(EmailHtml emailHtml in this.EmailsHTML)
                    {
                        base.SendEmail(emailHtml, null, ctx, SmtpClient);
                    }
                    break;
                //case EscalationEmailGenericActionEnum.AmalgamatedEmail_ToSpecifiedUser:
                //    // We need to get all email bodies, merge them and send to 'ActionDestination' user email address.
                //    // Should output all email bodies into one email body and attach all documents.
                //    foreach(EmailHtml emailHtml in this.EmailsHTML)
                //    {
                //        amalgamatedHtml.Append(emailHtml.Body);
                //    }
                //    base.SendEmail(amalgamatedHtml.ToString(), this.EmailsHTML, this.EmailAddress, "Amalgamated Email To Specific Person", null, ctx, SmtpClient);
                //    break;
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_All:
                    // We want to create the html body and header for each groupoing and save each email / or html of header and body, to a shared drive as specified. 
                    foreach (EmailHtml emailHtml in this.EmailsHTML)
                    {
                        string documentFullPath = System.IO.Path.Combine(this.DestinationPath, string.Format("TemplateFor_{0}_{1}.html", emailHtml.GroupByKey, dateTime));
                        // Write A file to the Specified Path.
                        System.IO.File.WriteAllText(documentFullPath, GetEmailContentAsHtml(emailHtml, ConvertGenericActionToHtmlIncludeEnum(this.Action)));
                    }
                    break;
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_All:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_AttachmentOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_BodyOnly:
                    // Create the local path for the html file.
                    System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
                    string fullProcessPath = ass.Location;
                    string currentDir = Path.GetDirectoryName(fullProcessPath);
                    string docFullPath = string.Format(@"{0}\AmalgamatedHTML{1}.html", currentDir.TrimEnd('\\'), dateTime);
                    try
                    {
                        // Save the html content to the local file path.
                        System.IO.File.WriteAllText(docFullPath, GetAmalgamatedEmailFullContentAsHtml(ConvertGenericActionToHtmlIncludeEnum(this.Action)));
                        string emailSubject = string.Format("The Amalgamated HTML file from Escalation: {0}, is Attached.", this.Name);
                        // Now call the send email method with path to file.
                        base.SendEmail(emailSubject, this.EmailAddress, "", docFullPath, SmtpClient, ctx);
                    }
                    finally
                    {
                        // Now delete the file.
                        if (File.Exists(docFullPath))
                        {
                            File.Delete(docFullPath);
                        }
                    }
                    break;
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_All:
                    // We want to create the html body and header for each groupoing and save each email / or html of header and body, to a shared drive as specified. 
                    string documentFullPath2 = System.IO.Path.Combine(this.DestinationPath, string.Format("AmalgamatedTemplate_{0}.html", dateTime));
                    // Write the file to the Specified Path.
                    System.IO.File.WriteAllText(documentFullPath2, GetAmalgamatedEmailFullContentAsHtml(ConvertGenericActionToHtmlIncludeEnum(this.Action)));
                    break;
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_All:
                    AsposeHelper asposeHelper = new AsposeHelper();
                    foreach (EmailHtml emailHtml in this.EmailsHTML)
                    {
                        string documentFullPath = System.IO.Path.Combine(this.DestinationPath, string.Format("TemplateFor_{0}_{1}.pdf", emailHtml.GroupByKey, dateTime));

                        // Save the PDF file.
                        base.SavePdfToFile(documentFullPath, emailHtml, ConvertGenericActionToHtmlIncludeEnum(this.Action));
                    }
                    break;
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_AttachmentOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_BodyOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_All:
                //    string documentFullPath3 = System.IO.Path.Combine(this.DestinationPath, string.Format("AmalgamatedTemplate_{0}.pdf", dateTime));
                //    AsposeHelper asposeHelper2 = new AsposeHelper();
                //    asposeHelper2.SaveHtmlToPDFFile(GetAmalgamatedEmailFullContentAsHtml(ConvertGenericActionToHtmlIncludeEnum(this.Action)), documentFullPath3, this.PdfPageHeight, this.PdfPageWidth);
                //    break;
            }
        }

        private HtmlFromEmailIncludionEnum ConvertGenericActionToHtmlIncludeEnum(EscalationEmailGenericActionEnum convertFrom)
        {
            switch (convertFrom)
            {
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_AttachmentOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_AttachmentOnly:
                    return HtmlFromEmailIncludionEnum.AttachmentOnly;
                    
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_BodyOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_BodyOnly:
                    return HtmlFromEmailIncludionEnum.BodyOnly;

                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_All:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_All:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_All:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_All:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedUser_All:
                    return HtmlFromEmailIncludionEnum.All;
            }
            return HtmlFromEmailIncludionEnum.All;
        }

        private string GetAmalgamatedEmailFullContentAsHtml(HtmlFromEmailIncludionEnum contentToInclude)
        {
            StringBuilder builder = new StringBuilder();

            foreach (EmailHtml emailHtml in this.EmailsHTML)
            {
                builder.Append(string.Format("{0} <hr />", GetEmailContentAsHtml(emailHtml, contentToInclude)));
            }

            return builder.ToString();
        }



        private void GetEmailsHtml(List<BreachLog> breachLogs, VirtualTrainerContext ctx)
        {
            // TODO: need to tagert specified field not HardCode ContextRef.
            foreach (var group in breachLogs.GroupBy(l => l.ContextRef).ToList())
            {
                string key = group.Key;
                List<BreachLog> logs = group.ToList();

                if (logs.Any())
                {
                    GetEMailHtml(key, logs, ctx);
                }
            }
        }

        private void GetEMailHtml(string key, List<BreachLog> logs, VirtualTrainerContext ctx)
        {
            EscalationsFrameworkConfigEmailRazorModel razorModel = new EscalationsFrameworkConfigEmailRazorModel(ctx)
            {
                BreachLogs = logs,
                // Recipient = recipient.GetRecipient(ctx),
                EFEmailRuleConfig = this,
                SendEmail = true,
                SentFromName = this.InternalSentFromUserName,
                AttachExcelofBreaches = this.AttachExcelOfBreaches,
                SentFromEmail = this.InternalSentFromEmail
            };

            string emailHtmlBody = string.Empty;
            string emailHtmlSubject = string.Empty;
            string emailAttachmentContent = string.Empty;

            emailHtmlBody = GetContentFromTemplate(this.EmailBodyTemplate, this.ServerRazorEmailTemplatePathBody, razorModel, false);
            emailHtmlSubject = GetContentFromTemplate(this.EmailSubjectTemplate, this.ServerRazorEmailTemplatePathSubject, razorModel, true);
            if (this.CreateAttachementUsingTemplate.GetValueOrDefault() && !string.IsNullOrEmpty(this.EmailAttachementTemplate))
            {
                emailAttachmentContent = GetContentFromTemplate(this.EmailAttachementTemplate, this.ServerRazorEmailTemplatePathAttachment, razorModel, false);
            }

            #region [ Delete ]

            //if (Engine.Razor.IsTemplateCached(emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
            //{
            //    emailHtmlBody = Engine.Razor.Run(emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
            //}
            //else
            //{
            //    string filePath = string.Format("{0}\\{1}", this.ServerRazorEmailTemplatePathBody, this.EmailBodyTemplate);
            //    if (System.IO.File.Exists(filePath))
            //    {
            //        string emailRazorTemplate = System.IO.File.OpenText(filePath).ReadToEnd();
            //        emailHtmlBody = Engine.Razor.RunCompile(emailRazorTemplate, emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
            //    }
            //}

            //if (Engine.Razor.IsTemplateCached(emailAttachmentTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
            //{
            //    emailAttachmentContent = Engine.Razor.Run(emailAttachmentTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
            //}
            //else
            //{
            //    string filePath = string.Format("{0}\\{1}", this.ServerRazorEmailTemplatePathAttachment, this.EmailSubjectTemplate);
            //    if (System.IO.File.Exists(filePath))
            //    {
            //        string emailRazorTemplate = System.IO.File.OpenText(filePath).ReadToEnd();
            //        emailAttachmentContent = Engine.Razor.RunCompile(emailRazorTemplate, emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
            //    }
            //    else
            //    {
            //        emailAttachmentContent = this.EmailSubjectTemplate;
            //    }
            //}
            //if (Engine.Razor.IsTemplateCached(emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
            //{
            //    emailHtmlSubject = Engine.Razor.Run(emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
            //}
            //else
            //{
            //    string filePath = string.Format("{0}\\{1}", this.ServerRazorEmailTemplatePathSubject, this.EmailSubjectTemplate);
            //    if (System.IO.File.Exists(filePath))
            //    {
            //        string emailRazorTemplate = System.IO.File.OpenText(filePath).ReadToEnd();
            //        emailHtmlSubject = Engine.Razor.RunCompile(emailRazorTemplate, emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
            //    }
            //    else
            //    {
            //        emailHtmlSubject = this.EmailSubjectTemplate;
            //    }
            //}
            // For some reason templateservice parse seems to html endcode brackets resulting in <div> etc not being rendered correctly!!
            //emailHtmlBody = emailHtmlBody.Replace("&lt;", "<").Replace("&gt;", ">");
            //emailHtmlSubject = emailHtmlSubject.Replace("&lt;", "<").Replace("&gt;", ">");

            #endregion

            string EmailTo = GetEmailTo(logs[0].GetBreachFieldValue(this.EmailBreachColumnName));

            EmailHtml emailHtml = new EmailHtml()
            {
                Body = emailHtmlBody,
                Subject = emailHtmlSubject,
                EmailFrom = string.IsNullOrEmpty(razorModel.SentFromEmail) ? string.Empty : razorModel.SentFromEmail,
                SendEMail = razorModel.SendEmail,
                EmailTo = string.IsNullOrEmpty(razorModel.ToEmail) ? EmailTo : razorModel.ToEmail,
                GroupByKey = key,
                AttachmentHtmlContent = emailAttachmentContent,
                AddHtmlAttachment = this.CreateAttachementUsingTemplate.GetValueOrDefault(),
                HTMLAttachmentName = string.Format("Attachment_{0}_{1}.Pdf", key, Guid.NewGuid().ToString()),
                pdfHeight = this.PdfPageHeight,
                pdfWidth = this.PdfPageWidth,
                PDFFooterMargins = this.PDFFooterMargins,
                PDFHeaderMargins = this.PDFHeaderMargins,
                PDFFooterText = this.PDFFooterHtml,
                PDFHeaderText = this.PDFHeaderHtml,
                PDFMargins = this.PdfMargins
            };

            this.EmailsHTML.Add(emailHtml);
        }
        private string GetContentFromTemplate(string templateName, string temapltePath, EscalationsFrameworkConfigEmailRazorModel razorModel, bool returnInputIfTemplateNotFound)
        {
            string content = string.Empty;

            if (Engine.Razor.IsTemplateCached(templateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
            {
                content = Engine.Razor.Run(templateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
            }
            else
            {
                string filePath = string.Format("{0}\\{1}", temapltePath, templateName);
                if (System.IO.File.Exists(filePath))
                {
                    string emailRazorTemplate = System.IO.File.OpenText(filePath).ReadToEnd();
                    content = Engine.Razor.RunCompile(emailRazorTemplate, templateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                }
                else
                {
                    if (returnInputIfTemplateNotFound)
                    {
                        content = templateName;
                    }
                    else
                    {
                        content = string.Format("Template: '{0}' Could not be found at: {1}. ", templateName, temapltePath);
                    }
                }
            }

            return content.Replace("&lt;", "<").Replace("&gt;", ">");
        }
        private string GetEmailTo(string potentialEmail)
        {
            switch (this.Action)
            {
                //case EscalationEmailGenericActionEnum.AmalgamatedEmail_ToSpecifiedUser:
                case EscalationEmailGenericActionEnum.IndividualEmails_ToSpecifiedUser:
                    return this.EmailAddress;
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_All:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_AttachmentOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_BodyOnly:
                //case EscalationEmailGenericActionEnum.AmalgamatedPdf_ToSpecifiedLocation_All:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualHtml_ToSpecifiedLocation_All:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_AttachmentOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_BodyOnly:
                case EscalationEmailGenericActionEnum.IndividualPdfs_ToSpecifiedLocation_All:
                    return string.Empty;
                case EscalationEmailGenericActionEnum.IndividualEmails_UseEmailInBreachField:
                case EscalationEmailGenericActionEnum.IndividualEmails_UseEmailInBreachField_ManualSend:
                    return string.IsNullOrEmpty(potentialEmail) ? string.Empty : potentialEmail;
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}
