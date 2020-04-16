using AJG.VirtualTrainer.Helper.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace VirtualTrainer
{
    public class Project
    {
        #region [ EF Properties ]
        [Key]
        public Guid ProjectUniqueKey { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDisplayName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectOwner { get; set; }
        //public ICollection<User> Users { get; set; }
        public ICollection<TargetDatabaseDetails> TargetDatabases { get; set; }
        public ICollection<Team> Teams { get; set; }
        public ICollection<Office> Offices { get; set; }
        [InverseProperty("Project")]
        public ICollection<ProjectPermission> ProjectPermissions { get; set; }
        // public ICollection<BusinessEntitiesMappings> BusinessEntityMappings { get; set; }
        public ICollection<Rule> Rules { get; set; }
        public ICollection<SystemLog> SystemLogs { get; set; }
        public ICollection<UserActivityLog> UserActivityLogs { get; set; }
        public ICollection<ActionTakenLog> ActionTakenLog { get; set; }
        public EscalationsFramework EscalationsFramework { get; set; }
        public ICollection<BreachLog> BreachLogs { get; set; }
        //public ICollection<ExclusionsGroupForProject> ExclusionGroupsProjectLevel { get; set; }
        [InverseProperty("Project")]
        public ICollection<ExclusionsGroup> ExclusionGroupsAll { get; set; }
        public ICollection<ActurisBusinessStructureSyncConfig> ActurisBusinessStructureSyncConfigs { get; set; }
        [InverseProperty("Project")]
        public ICollection<Schedule> Schedules { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystemProject { get; set; }
        public bool? IsMicroServicesProject { get; set; }
        public string EmailRazorTemplateBodyPath { get; set; }
        public string EmailRazorTemplateSubjectPath { get; set; }
        public string EmailRazorTemplateAttachmentPath { get; set; }
        [ForeignKey("ProjectType")]
        public int? ProjectTypeId { get; set; }
        public ProjectType ProjectType { get; set; }
        public string ProjectSenderEmail { get; set; }
        public string ProjectSenderDisplayName { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region [ Not Mapped properties ]

        [NotMapped]
        public string PojectTypeName
        {
            get
            {
                return ProjectTypeId == null ? string.Empty : ((ProjectTypeEnum)ProjectTypeId).ToString();
            }
        }

        #endregion

        #region [ Public Methods ]

        public bool UserIsMember(VirtualTrainerContext ctx, int userId)
        {
            return ctx.ProjectPermissions.Where(p => p.ProjectId == this.ProjectUniqueKey && p.RoleId == (int)RoleEnum.ProjectMember && p.UserId == userId).Any();
        }

        public bool ValuesAreValid(out string returnValue)
        {
            returnValue = string.Empty;
            if (this.ProjectTypeId.GetValueOrDefault() != (int)ProjectTypeEnum.MicroService)
            {
                // Implement validation here.
                string validationMessage = string.Empty;
                if (!DirectoryHelper.ValidateDirectoryAccess(this.EmailRazorTemplateAttachmentPath, this.ProjectName, out validationMessage))
                {
                    returnValue = string.Format("EmailRazorTemplateAttachmentPath: {0}", validationMessage);
                    return false;
                }
                if (!DirectoryHelper.ValidateDirectoryAccess(this.EmailRazorTemplateBodyPath, this.ProjectName, out validationMessage))
                {
                    returnValue = string.Format("EmailRazorTemplateBodyPath: {0}", validationMessage);
                    return false;
                }
                if (!DirectoryHelper.ValidateDirectoryAccess(this.EmailRazorTemplateSubjectPath, this.ProjectName, out validationMessage))
                {
                    returnValue = string.Format("EmailRazorTemplateSubjectPath: {0}", validationMessage);
                    return false;
                }
            }
            if (this.ProjectTypeId.GetValueOrDefault() != (int)ProjectTypeEnum.MicroService)
            {
                if (string.IsNullOrEmpty(this.ProjectSenderEmail))
                {
                    returnValue = string.Format("ProjectSenderEmail: {0}", "This field must contain a valid ajg email address.");
                    return false;
                }
                if (string.IsNullOrEmpty(this.ProjectSenderDisplayName))
                {
                    returnValue = string.Format("ProjectSenderDisplayName: {0}", "This field must not be empty.");
                    return false;
                }
            }

            return true;
        }

        public void ExecuteAllScheduledJobs(VirtualTrainerContext ctx, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            this.LoadLogContextObjects(ctx);
            try
            {
                this.ExecuteAllActurisBusinessStructureConfigurations(ctx);
            }
            catch (Exception ex)
            {
                SystemLog errorLog = new SystemLog(ex, this);
                errorLog.Message = "ExecuteAllActurisBusinessStructureConfigurations Issue";
                this.SystemLogs.Add(errorLog);
                ctx.SaveChanges();
            }

            try
            {
                this.ExecuteAllRules(ctx, true);
            }
            catch (Exception ex)
            {
                SystemLog errorLog = new SystemLog(ex, this);
                errorLog.Message = "ExecuteAllRules Issue";
                this.SystemLogs.Add(errorLog);
                ctx.SaveChanges();
            }

            try
            {
                this.ExecuteEscalationsFramework(ctx, true, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
            }
            catch (Exception ex)
            {
                SystemLog errorLog = new SystemLog(ex, this);
                errorLog.Message = "ExecuteEscalationsFramework Issue";
                this.SystemLogs.Add(errorLog);
                ctx.SaveChanges();
            }
        }
        public void ExecuteAllActurisBusinessStructureConfigurations(VirtualTrainerContext ctx)
        {
            LoadActurisBusinessStructureSyncContextObjects(ctx);
            ActurisImportProjectActionTakenLog log = new ActurisImportProjectActionTakenLog(this, ctx);
            try
            {
                if (this.IsActive)
                {
                    if (this.ActurisBusinessStructureSyncConfigs.Any())
                    {
                        foreach (ActurisBusinessStructureSyncConfig config in ActurisBusinessStructureSyncConfigs)
                        {
                            config.ProcessStructureFromActuris(ctx);
                        }
                        log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.ProjectExecutionSuccess;
                    }
                    else
                    {
                        log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.NoConfigurations;
                    }
                }
                else
                {
                    log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.ProjectNotActive;
                }
                log.Success = true;
            }
            catch (Exception ex)
            {
                SystemLog errorLog = new SystemLog(ex, this);
                this.SystemLogs.Add(errorLog);
                log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.Failure;
                log.ErrorLogEntry = errorLog;
                log.Success = false;
                log.ErrorMessage = errorLog.ErrorMessage;
            }
            finally
            {
                log.TimeStamp = DateTime.Now;
                this.ActionTakenLog.Add(log);
                ctx.SaveChanges();
            }
        }
        public List<BreachLog> ExecuteAllRules(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            LoadExecuteAllRulesContextObjects(ctx);
            ProjectExecuteRulesActionTakenLog log = new ProjectExecuteRulesActionTakenLog(this, ctx);
            List<BreachLog> returnBreaches = new List<BreachLog>();

            try
            {
                if (this.IsActive)
                {
                    foreach (Rule rule in this.Rules)
                    {
                        returnBreaches.AddRange(rule.ExecuteAllRuleConfigurations(ctx, saveBreachesToDB));  
                    }
                    log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.RuleExecutionSuccess;
                }
                // Log that this Project is not active.
                else
                {
                    log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.ProjectNotActive;
                }
                log.Success = true;
            }
            catch (Exception ex)
            {
                // Log Exception
                SystemLog errorLog = new SystemLog(ex, this);
                this.SystemLogs.Add(errorLog);
                // ctx.SaveChanges();
                // update Rule
                log.ErrorLogEntry = errorLog;
                log.ErrorMessage = ex.Message;
                log.Success = false;
                log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Failure;
            }
            finally
            {
                // Log Execution
                log.TimeStamp = DateTime.Now;
                this.ActionTakenLog.Add(log);
                ctx.SaveChanges();
            }
            return returnBreaches;
        }
        public void ExecuteEscalationsFramework(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            LoadEscalationsFrameowrkContextObject(ctx);
            ProjectEscalationsActionTakenLog log = new ProjectEscalationsActionTakenLog(this, ctx);
            try
            {
                if (this.IsActive)
                {
                    this.EscalationsFramework.RunEscelationFramework(ctx, updateTimeStamp, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
                    log.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
                }
                else
                {
                    log.ExecutionOutcome = EscalationsActionTakenLogOutcome.ProjectNotActive;
                }
                log.Success = true;
            }
            catch (Exception ex)
            {
                // Log Exception
                SystemLog errorLog = new SystemLog(ex, this);
                this.SystemLogs.Add(errorLog);
                // update Rule
                log.ErrorLogEntry = errorLog;
                log.ErrorMessage = ex.Message;
                log.Success = false;
                log.ExecutionOutcome = EscalationsActionTakenLogOutcome.Failure;
            }
            finally
            {
                // Log Execution
                log.TimeStamp = DateTime.Now;
                this.ActionTakenLog.Add(log);
                ctx.SaveChanges();
            }
        }
        public bool ProjectHasOutstandingBreachesCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadBreachLogs(ctx);
            return this.BreachLogs.Where(b => b.IsArchived != true && b.GetBreachCountForContextRef(ctx) >= count).Any();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadBreachLogs(ctx);
            return this.BreachLogs.Where(d=>d.IsArchived != true).OrderBy(a => a.TimeStamp).GroupBy(b => b.ContextRef).Select(c => {
                c.LastOrDefault().BreachLiveContextRefCount = c.Count();
                c.LastOrDefault().FirstBreachDate = c.FirstOrDefault().TimeStamp;
                return c.LastOrDefault();
            }).ToList();
            //return GetOutstandingBreachesByContextValue(ctx).Where(a => a.GetBreachCountForContextRef(ctx) >= count).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValue(VirtualTrainerContext ctx)
        {
            LoadBreachLogs(ctx);
            return this.GetAllOutstandingBreaches(ctx).OrderByDescending(d => d.TimeStamp).GroupBy(a => new
            {
                a.ContextRef
            }).Select(g => g.FirstOrDefault()).ToList();
        }
        public List<BreachLog> GetAllOutstandingBreaches(VirtualTrainerContext ctx)
        {
            LoadBreachLogs(ctx);
            // Include filtering by IsActive on project, office, team.  
            return this.BreachLogs.Where(b => b.IsArchived != true).ToList();
        }
        public List<User> GetAllUsers(VirtualTrainerContext ctx)
        {
            List<User> users = ctx.ProjectPermissions.Where(a => a.ProjectId == this.ProjectUniqueKey).Include("User").Select(u=>u.User).GroupBy(g=>g.Id).Select(grp => grp.FirstOrDefault()).Include("Aliases").ToList();

            // Include filtering by IsActive on project, office, team.  
            return users;
        }
        public bool UserHasAdminRole(VirtualTrainerContext ctx, User user)
        {
            bool returnValue = ctx.ProjectPermissions.Where(p => p.ProjectId == this.ProjectUniqueKey && p.UserId == user.Id && p.RoleId == (int)RoleEnum.ProjectAdmin).Any();
            return returnValue;
        }

        #endregion

        #region[ Private Methods ]

        private class UserBusinessStructureInfo
        {
            public string HandlerKey { get; set; }
            public string HandlerName { get; set; }
            public string HandlerEmail { get; set; }
            public string HandlerStatusKey { get; set; }
            public string HandlerOfficeKey { get; set; }
            public string HandlerStatusDescription { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public string UserStatusKey { get; set; }
            public string UserDescription { get; set; }
            public string TeamStatusKey { get; set; }
            public string TeamStatusDescription { get; set; }
            public string TeamName { get; set; }
            public string TeamOfficeKey { get; set; }
            public string OfficeKey { get; set; }
            public string OfficeName { get; set; }
            public string OrganisationKey { get; set; }
            public string OrganisationName { get; set; }
            public string TeamKey { get;set; }
        }
        private class SqlRowField
        {
            public string FieldName { get; set; }
            public string Value { get; set; }
            public Type FieldType { get; set; }
        }
        private void LoadBreachLogs(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }            
        }
        private void LoadEscalationsFrameowrkContextObject(VirtualTrainerContext ctx)
        {
            LoadLogContextObjects(ctx);
            if (!ctx.Entry(this).Reference("EscalationsFramework").IsLoaded)
            {
                ctx.Entry(this).Reference("EscalationsFramework").Load();
            }
        }
        private void LoadExecuteAllRulesContextObjects(VirtualTrainerContext ctx)
        {
            LoadLogContextObjects(ctx);
            if (!ctx.Entry(this).Collection("Rules").IsLoaded)
            {
                ctx.Entry(this).Collection("Rules").Load();
            }
        }
        private void LoadLogContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("ActionTakenLog").IsLoaded)
            {
                ctx.Entry(this).Collection("ActionTakenLog").Load();
            }
            if (!ctx.Entry(this).Collection("SystemLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("SystemLogs").Load();
            }
        }
        private void LoadActurisBusinessStructureSyncContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("ActurisBusinessStructureSyncConfigs").IsLoaded)
            {
                ctx.Entry(this).Collection("ActurisBusinessStructureSyncConfigs").Load();
            }
            if (!ctx.Entry(this).Collection("ProjectPermissions").IsLoaded)
            {
                ctx.Entry(this).Collection("ProjectPermissions").Load();
            }
            if (!ctx.Entry(this).Collection("Offices").IsLoaded)
            {
                ctx.Entry(this).Collection("Offices").Load();
            }
            if (!ctx.Entry(this).Collection("Teams").IsLoaded)
            {
                ctx.Entry(this).Collection("Teams").Load();
            }
            if (!ctx.Entry(this).Collection("Rules").IsLoaded)
            {
                ctx.Entry(this).Collection("Rules").Load();
            }
        }

        #endregion
    }
}
