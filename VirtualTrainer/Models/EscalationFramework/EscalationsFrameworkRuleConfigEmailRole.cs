using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;
using System.IO;

namespace VirtualTrainer
{
    public class EscalationsFrameworkRuleConfigEmailRole : EscalationsFrameworkRuleConfigEmail
    {
        #region [ EF properties ]

        [ForeignKey("Action")]
        [Required]
        public int ActionId { get; set; }
        public EscalationsFrameworkAction Action { get; set; }
        public string OverrideRecipientEmail { get; set; }

        #endregion

        #region [ Non EF properties ]

        [NotMapped]
        public string ActionName { get; set; }

        #endregion

        public override void RunEscalationConfiguration(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            base.RunEscalationConfiguration(ctx, updateTimeStamp, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
            EmailRuleConfigEscalationsActionTakenLog log = new EmailRuleConfigEscalationsActionTakenLog(this, ctx);

            try
            {
                LoadRequiredContextObjects(ctx);
                if (this.Schedule.IsScheduledToRun(ctx, this.LastRunDate) || updateTimeStamp == false)
                {
                    if (this.IsActive)
                    {
                        // once got breaches should be able to process users from here.
                        List<BreachLog> breachLogs = GetUniqueBreachLogsFromBreachSources(ctx);

                        if (breachLogs.Any())
                        {

                            #region [ New breach centric implementation ]

                            using (SmtpClient smtpClient = new SmtpClient())
                            {
                                ProcessBreaches(ctx, breachLogs, smtpClient, updateTimeStamp, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
                            }
                            #endregion

                            #region [ Old User focused implmentation - SLOQ ]

                            //List<User> uniqueUsersFromBreachSources = this.GetUniqueUsersFromBreachSources(ctx);

                            //if (uniqueUsersFromBreachSources.Any())
                            //{
                            //    using (SmtpClient SmtpClient = new SmtpClient())
                            //    {
                            //        foreach (User user in uniqueUsersFromBreachSources)
                            //        {
                            //            EmailUser(user, ctx, breachLogs, SmtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                            //        }
                            //    }
                            //    log.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
                            //}
                            //else
                            //{
                            //    log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigNoRecipients;
                            //}

                            #endregion
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
        private void ProcessBreaches(VirtualTrainerContext ctx, List<BreachLog> breaches, SmtpClient smtpClient, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            List<BreachLog> breachesToProcess = new List<BreachLog>();
            switch (this.ActionId)
            {
                case ((int)ActionEnum.EmailToHandler):
                    // Group by User ID
                    foreach (var userGrp in breaches.GroupBy(b => b.UserId))
                    {
                        int userID = userGrp.Key.GetValueOrDefault();
                        // get the user
                        User user = ctx.User.Where(u => u.Id == userID).FirstOrDefault();

                        // filter the breaches for this user.
                        List<BreachLog> userLogs = userGrp.ToList();

                        EmailUserTheseBreaches(user, ctx, userLogs, smtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                    }
                    break;
                case ((int)ActionEnum.EmailToTeamLead):
                    breachesToProcess = PopulateBreachTeamOrOfficeRoleUserId(breaches, RoleEnum.TeamLead, ctx);
                    // Now process.
                    foreach (var userGrp in breachesToProcess.GroupBy(b => b.TeamLeadUserID))
                    {
                        int teamLeadID = userGrp.Key.GetValueOrDefault();
                        // get the user
                        User user = ctx.User.Where(u => u.Id == teamLeadID).FirstOrDefault();

                        // filter the breaches for this user.
                        List<BreachLog> teamLeadUserLogs = userGrp.ToList();

                        EmailUserTheseBreaches(user, ctx, teamLeadUserLogs, smtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                    }
                    break;
                case ((int)ActionEnum.EmailToBranchManager):
                    breachesToProcess = PopulateBreachTeamOrOfficeRoleUserId(breaches, RoleEnum.BranchManager, ctx);
                    // Now process.
                    foreach (var userGrp in breachesToProcess.GroupBy(b => b.OfficeManagerUserID))
                    {
                        int teamLeadID = userGrp.Key.GetValueOrDefault();
                        // get the user
                        User user = ctx.User.Where(u => u.Id == teamLeadID).FirstOrDefault();

                        // filter the breaches for this user.
                        List<BreachLog> branchManagerUserLogs = userGrp.ToList();

                        EmailUserTheseBreaches(user, ctx, branchManagerUserLogs, smtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                    }
                    break;
                case ((int)ActionEnum.EmailToQualityAuditor):
                    breachesToProcess = PopulateBreachTeamOrOfficeRoleUserId(breaches, RoleEnum.QualityAuditor, ctx);
                    // Now process.
                    foreach (var userGrp in breachesToProcess.GroupBy(b => b.QAUserID))
                    {
                        int teamLeadID = userGrp.Key.GetValueOrDefault();
                        // get the user
                        User user = ctx.User.Where(u => u.Id == teamLeadID).FirstOrDefault();

                        // filter the breaches for this user.
                        List<BreachLog> QAUserLogs = userGrp.ToList();

                        EmailUserTheseBreaches(user, ctx, QAUserLogs, smtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                    }
                    break;
                case ((int)ActionEnum.EmailToRegionalManager):
                    breachesToProcess = PopulateBreachTeamOrOfficeRoleUserId(breaches, RoleEnum.RegionalManager, ctx);
                    // Now process.
                    foreach (var userGrp in breachesToProcess.GroupBy(b => b.RegionalManagerUserID))
                    {
                        int teamLeadID = userGrp.Key.GetValueOrDefault();
                        // get the user
                        User user = ctx.User.Where(u => u.Id == teamLeadID).FirstOrDefault();

                        // filter the breaches for this user.
                        List<BreachLog> RegionalManagerUserLogs = userGrp.ToList();

                        EmailUserTheseBreaches(user, ctx, RegionalManagerUserLogs, smtpClient, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject);
                    }
                    break;
            }
        }
        private void EmailUserTheseBreaches(User user, VirtualTrainerContext ctx, List<BreachLog> breachesForUser, SmtpClient SmtpClient, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject)
        {
            EmailRuleConfigEscalationsActionTakenLog log = new EmailRuleConfigEscalationsActionTakenLog(this, ctx);
            try
            {
                //  only process if user is active
                if (user.IsActive)
                {
                    if (breachesForUser.Any())
                    {
                        // Create the Razor Object model passing in only stuff relevant to the user.
                        EscalationsFrameworkConfigEmailRazorModel razorModel = new EscalationsFrameworkConfigEmailRazorModel(ctx)
                        {
                            BreachLogs = breachesForUser,
                            Recipient = user,
                            EFEmailRuleConfig = this,
                            SendEmail = true,
                            SentFromName = this.InternalSentFromUserName,
                            AttachExcelofBreaches = this.AttachExcelOfBreaches
                        };

                        string emailBodyTemplateName = this.EmailBodyTemplate;
                        string emailSubjectTemplateName = string.Format("emailSubjectTemplateKey{0}{1}", this.ProjectId, this.Id);
                        string emailHtmlBody = string.Empty;
                        string emailHtmlSubject = string.Empty;

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

                        // For some reason templateservice parse seems to html endcode brackets resulting in <div> etc being rendered!!
                        emailHtmlBody = emailHtmlBody.Replace("&lt;", "<").Replace("&gt;", ">");
                        emailHtmlSubject = emailHtmlSubject.Replace("&lt;", "<").Replace("&gt;", ">");

                        // Populate log with email details.
                        log.PopulatEmailAndUserDetails(ctx, user, emailHtmlBody, emailHtmlSubject, this.InternalSentFromEmail);

                        string toEmail = string.IsNullOrEmpty(this.OverrideRecipientEmail) ? user.Email : this.OverrideRecipientEmail;
                        log.EmailTo = toEmail;

                        if (razorModel.SendEmail)
                        {
                            if (!string.IsNullOrEmpty(toEmail))
                            {
                                // Send the email
                                SendEmail(emailHtmlBody, toEmail, emailHtmlSubject, razorModel, ctx, SmtpClient);
                                log.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
                            }
                            else
                            {
                                log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigEmailUserHasNoEmail;
                            }
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
                //ctx.SaveChanges();
            }
        }
        private void EmailUser(User user, VirtualTrainerContext ctx, List<BreachLog> breaches, SmtpClient SmtpClient, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject)
        {
            EmailRuleConfigEscalationsActionTakenLog log = new EmailRuleConfigEscalationsActionTakenLog(this, ctx);
            try
            {
                //  only process if user is active and they have the role required by this ActionId.
                if (user.IsActive)
                {
                    if (user.HasRole(ctx, Action.RoleId))
                    {
                        List<BreachLog> breachesForUser = FilterBreaches(ctx, breaches, user);
                        

                        // TODO...
                    }
                }
                else
                {
                    log.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsRuleConfigEmailUserNotActive;
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

        private List<BreachLog> FilterBreaches(VirtualTrainerContext ctx, List<BreachLog> breaches, User user)
        {
            switch (this.ActionId)
            {
                case ((int)ActionEnum.EmailToHandler):
                    return breaches.Where(u => u.UserId == user.Id).ToList();
                case ((int)ActionEnum.EmailToTeamLead):
                    return breaches.Where(u => u.IsUserATeamLeadForBreach(ctx, user)).ToList();
                case ((int)ActionEnum.EmailToBranchManager):
                    return breaches.Where(u => u.IsUserAManagerForThisBreach(ctx, user)).ToList();
                case ((int)ActionEnum.EmailToQualityAuditor):
                    return breaches.Where(u => u.IsUserAQualityAuditorForBreach(ctx, user)).ToList();
                case ((int)ActionEnum.EmailToRegionalManager):
                    return breaches.Where(u => u.IsUserARegionalManagerForBreach(ctx, user)).ToList();
            }

            return breaches;
        }

        private new void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Action").IsLoaded)
            {
                ctx.Entry(this).Reference("Action").Load();
            }

            base.LoadRequiredContextObjects(ctx);
        }
    }
}