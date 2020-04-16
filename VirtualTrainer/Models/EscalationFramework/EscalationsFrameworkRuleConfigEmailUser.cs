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

namespace VirtualTrainer
{
    public class EscalationsFrameworkRuleConfigEmailUser : EscalationsFrameworkRuleConfigEmail
    {
        #region [ EF properties ]
        
        [InverseProperty("EscalationsFrameworkRuleConfig")]
        public ICollection<EscalationsEmailRecipient> Recipients { get; set; }

        #endregion

        public override void RunEscalationConfiguration(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            base.RunEscalationConfiguration(ctx, updateTimeStamp, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
            RuleConfigEscalationsActionTakenLog log = new RuleConfigEscalationsActionTakenLog(this, ctx);

            try
            {
                LoadRequiredContextObjects(ctx);
                if (this.Schedule.IsScheduledToRun(ctx, this.LastRunDate))
                {
                    if (this.IsActive)
                    {
                        // we just want to get all the breaches and send them into the email template for razor to do its Magic.
                        List<BreachLog> breachLogs = GetUniqueBreachLogsFromBreachSources(ctx);

                        if (breachLogs.Any())
                        {
                            if (Recipients.Any())
                            {
                                using (SmtpClient SmtpClient = new SmtpClient())
                                {
                                    foreach (EscalationsEmailRecipient recipient in Recipients.ToList())
                                    {
                                        EmailUser(recipient, breachLogs, ctx, SmtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                                    }
                                }
                                log.Success = true;
                                log.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
                            }
                            else
                            {
                                log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigNoRecipients;
                            }
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

        private void EmailUser(EscalationsEmailRecipient recipient, List<BreachLog> breachLogs, VirtualTrainerContext ctx, SmtpClient SmtpClient, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject)
        {
            EmailRuleConfigEscalationsActionTakenLog log = new EmailRuleConfigEscalationsActionTakenLog(this, ctx);

            try
            {
                // If the recipient object is active and the actual user object is active.
                if (recipient.IsActive && recipient.GetRecipient(ctx).IsActive)
                {
                    if (breachLogs.Any())
                    {
                        EscalationsFrameworkConfigEmailRazorModel razorModel = new EscalationsFrameworkConfigEmailRazorModel(ctx)
                        {
                            BreachLogs = breachLogs,
                            Recipient = recipient.GetRecipient(ctx),
                            EFEmailRuleConfig = this,
                            SendEmail = true,
                            SentFromName = this.InternalSentFromUserName,
                            AttachExcelofBreaches = this.AttachExcelOfBreaches
                        };

                        string emailBodyTemplateName = this.EmailBodyTemplate;
                        string emailSubjectTemplateName = this.EmailSubjectTemplate;
                        string emailHtmlBody = string.Empty;
                        string emailHtmlSubject = string.Empty;

                        //Engine.Razor.
                        //Razor.Parse(this.EmailBodyTemplate, razorModel);

                        if (Engine.Razor.IsTemplateCached(emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
                        {
                            emailHtmlBody = Engine.Razor.Run(emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                        }
                        else
                        {
                            string filePath = string.Format("{0}\\{1}", ServerRazorEmailTemplatePathBody, this.EmailBodyTemplate);
                            if (System.IO.File.Exists(filePath))
                            {
                                string emailRazorTemplate = System.IO.File.OpenText(filePath).ReadToEnd();
                                emailHtmlBody = Engine.Razor.RunCompile(emailRazorTemplate, emailBodyTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                            }
                        }

                        if (Engine.Razor.IsTemplateCached(emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel)))
                        {
                            emailHtmlSubject = Engine.Razor.Run(emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                        }
                        else
                        {
                            string filePath = string.Format("{0}\\{1}", ServerRazorEmailTemplatePathSubject, this.EmailSubjectTemplate);
                            if (System.IO.File.Exists(filePath))
                            {
                                string emailRazorTemplate = System.IO.File.OpenText(filePath).ReadToEnd();
                                emailHtmlSubject = Engine.Razor.RunCompile(emailRazorTemplate, emailSubjectTemplateName, typeof(EscalationsFrameworkConfigEmailRazorModel), razorModel, null);
                            }
                            else
                            {
                                emailHtmlSubject = this.EmailSubjectTemplate;
                            }
                        }
                        //var emailHtmlBody = templateService.Parse(this.EmailBodyTemplate, razorModel, null, null);
                        //var emailHtmlSubject = templateService.Parse(this.EmailSubjectTemplate, razorModel, null, null);
                        // For some reason templateservice parse seems to html endcode brackets resulting in <div> etc being rendered!!
                        emailHtmlBody = emailHtmlBody.Replace("&lt;", "<").Replace("&gt;", ">");
                        emailHtmlSubject = emailHtmlSubject.Replace("&lt;", "<").Replace("&gt;", ">");

                        // Populate log with email details.
                        log.PopulatEmailAndUserDetails(ctx, recipient.GetRecipient(ctx), emailHtmlBody, emailHtmlSubject, this.InternalSentFromEmail);

                        if (razorModel.SendEmail)
                        {
                            SendEmail(emailHtmlBody, recipient.GetRecipient(ctx).Email, emailHtmlSubject, razorModel, ctx, SmtpClient);
                            log.Success = true;
                        }
                        else
                        {
                            log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigEmailRazorModelSetSendEmailToFalse;
                        }
                    }
                    else
                    {
                        log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigEmailNoBreacheLogsForUser;
                    }
                }
                else
                {
                    log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigEmailUserNotActive;
                }
                log.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
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
                this.LastRunDate = DateTime.Now.Date;
                this.ActionTakenLog.Add(log);
                ctx.SaveChanges();
            }
        }

        public new void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("Recipients").IsLoaded)
            {
                ctx.Entry(this).Collection("Recipients").Load();
            }

            base.LoadRequiredContextObjects(ctx);
        }
    }
}
