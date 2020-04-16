namespace VirtualTrainer.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Diagnostics;


    internal sealed class Configuration : DbMigrationsConfiguration<VirtualTrainer.VirtualTrainerContext>
    {
        internal string defaultScheduleName = "Default Schedule";
        internal string neverScheduleName = "Never Schedule";
        internal string SystemProjectGuidString = "C09D6AF1-2A18-46FC-AC7D-C35886A04B23";
        internal string VTProjectGuidString = "A896DA6D-F233-42DE-8CF6-A3D21FF42C6D";
        internal string MicroServicesProjectGuidString = "FFD61007-6872-4598-9687-9DDC48C29253";
        internal string TechTimeSheetProjectGuidstring = "a9fbcd4a-d7df-46e5-bfc3-c124dad930e5";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(VirtualTrainer.VirtualTrainerContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            Assembly _assembly = Assembly.GetExecutingAssembly();
            string targetSystem = ConfigurationManager.AppSettings[AppSettingsEnum.targetSystem.ToString()];
            TargetSystem targetSystemEnum;
            bool targetEnumFound = Enum.TryParse(targetSystem, out targetSystemEnum);

            SetUpSystemTables(context);
            AddUsers(targetSystemEnum, context);
            AddUserSystemPermissions(context, targetSystemEnum);

            string RunSeedOnEFDeployment = ConfigurationManager.AppSettings[AppSettingsEnum.RunSeedOnEFDeployment.ToString()];
            bool RunSeedOnEFDeploymentBool = false;
            bool runSeedTargetEnumFound = bool.TryParse(RunSeedOnEFDeployment, out RunSeedOnEFDeploymentBool);

            string microsServicesEnabled = ConfigurationManager.AppSettings[AppSettingsEnum.MicrosServicesEnabled.ToString()];
            bool microsServicesEnabledBool = false;
            bool microsServicesEnabledFound = bool.TryParse(microsServicesEnabled, out microsServicesEnabledBool);

            // Set up microservices project.
            CreateMicrosServicesProject(context, (microsServicesEnabledFound && microsServicesEnabledBool));
            

            string TechTimeSheetEnabled = ConfigurationManager.AppSettings[AppSettingsEnum.TechTimeSheetEnabled.ToString()];
            bool TechTimeSheetEnabledBool = false;
            bool TechTimeSheetEnabledFound = bool.TryParse(TechTimeSheetEnabled, out TechTimeSheetEnabledBool);
            CreateTechTimeSheetProject(context, (TechTimeSheetEnabledFound && TechTimeSheetEnabledBool));

            // ensure VT exists and has project type id set up.
            Guid ProjectGuid = new Guid(VTProjectGuidString);
            Project project = context.Project.Where(a => a.ProjectUniqueKey == ProjectGuid).FirstOrDefault();
            if (project == null)
            {
                // Add Project.
                project = new Project()
                {
                    IsActive = true,
                    ProjectDescription = "Virtual Trainer",
                    ProjectDisplayName = "Virtual Trainer",
                    ProjectName = "Virtual Trainer",
                    ProjectOwner = "Andy Fooks",
                    ProjectUniqueKey = ProjectGuid,
                    IsSystemProject = false
                };
                context.Project.AddOrUpdate(project);
                context.SaveChanges();
            }
            else
            {
                project.ProjectTypeId = (int)ProjectTypeEnum.VirtualTrainer;
                context.SaveChanges();
            }

            string SplitStringFunctionSqlScript = new StreamReader(_assembly.GetManifestResourceStream("VirtualTrainer.SQLScripts.SplitStringFunction.sql")).ReadToEnd();
            context.Database.ExecuteSqlCommand(@"
                                            IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[SplitString]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
                                                DROP FUNCTION [dbo].[SplitString]            
                            ");
            context.Database.ExecuteSqlCommand(SplitStringFunctionSqlScript);

            DOMyHubSQLStuff(context, _assembly);
        }

        private void DOMyHubSQLStuff(VirtualTrainerContext context, Assembly _assembly)
        {
            // Delete Stored Procs that use CreateVTBreachType
            string MyHubInsertPrinterSummaryActivitySQLScript = new StreamReader(_assembly.GetManifestResourceStream("VirtualTrainer.SQLScripts.MyHub.MyHubInsertPrinterSummaryActivity.sql")).ReadToEnd();
            context.Database.ExecuteSqlCommand(@"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DBO].[MyHubInsertPrinterSummaryActivity]') AND type IN ( N'P' ))
                                                DROP PROCEDURE [dbo].[MyHubInsertPrinterSummaryActivity]");
            string MyHubInsertPhoneSummaryActivitySQLScript = new StreamReader(_assembly.GetManifestResourceStream("VirtualTrainer.SQLScripts.MyHub.MyHubInsertPhoneSummaryActivity.sql")).ReadToEnd();
            context.Database.ExecuteSqlCommand(@"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DBO].[MyHubInsertPhoneSummaryActivity]') AND type IN ( N'P' ))
                                                DROP PROCEDURE [dbo].[MyHubInsertPhoneSummaryActivity]");

            // Then delete the type CreateVTBreachType
            string CreateVTBreachTypeSQLScript = new StreamReader(_assembly.GetManifestResourceStream("VirtualTrainer.SQLScripts.MyHub.CreateVTBreachType.sql")).ReadToEnd();
            context.Database.ExecuteSqlCommand(@"IF EXISTS (SELECT * FROM sys.types WHERE name = 'VTBreachType') DROP Type [VTBreachType]");
            context.Database.ExecuteSqlCommand(CreateVTBreachTypeSQLScript);

            // Then add the Sprocs.
            context.Database.ExecuteSqlCommand(MyHubInsertPrinterSummaryActivitySQLScript);
            context.Database.ExecuteSqlCommand(MyHubInsertPhoneSummaryActivitySQLScript);
        }

        private void CreateMicrosServicesProject(VirtualTrainerContext context, bool isActive)
        {
            Guid ProjectGuid = new Guid(MicroServicesProjectGuidString);
            Project project = context.Project.Where(a => a.ProjectUniqueKey == ProjectGuid).FirstOrDefault();
            if (project == null)
            {
                // Add Project.
                project = new Project()
                {
                    IsActive = isActive,
                    ProjectDescription = "Micro Services",
                    ProjectDisplayName = "Micro Services",
                    ProjectName = "Micro Services",
                    ProjectOwner = "Andy Fooks",
                    ProjectUniqueKey = ProjectGuid,
                    IsSystemProject = false,
                    IsMicroServicesProject = true,
                    ProjectTypeId = (int)ProjectTypeEnum.MicroService
                };
                context.Project.AddOrUpdate(project);
            }
            else
            {
                project.IsActive = isActive;
                project.ProjectTypeId = (int)ProjectTypeEnum.MicroService;
            }
            context.SaveChanges();
        }
        private void CreateTechTimeSheetProject(VirtualTrainerContext context, bool isActive)
        {
            Guid ProjectGuid = new Guid(this.TechTimeSheetProjectGuidstring);
            Project project = context.Project.Where(a => a.ProjectUniqueKey == ProjectGuid).FirstOrDefault();
            if (project == null)
            {
                // Add Project.
                project = new Project()
                {
                    IsActive = isActive,
                    ProjectDescription = "Tech Time Sheet",
                    ProjectDisplayName = "Tech Time Sheet",
                    ProjectName = "Tech Time Sheet",
                    ProjectOwner = "Andy Fooks",
                    ProjectUniqueKey = ProjectGuid,
                    IsSystemProject = false,
                    IsMicroServicesProject = false,
                    ProjectTypeId = (int)ProjectTypeEnum.Standard                     
                };
                context.Project.AddOrUpdate(project);
            }
            else
            {
                project.IsActive = isActive;
                project.ProjectTypeId = (int)ProjectTypeEnum.Standard;
            }
            context.SaveChanges();
        }
        private ExclusionsGroup CreateExclusionsGroup(VirtualTrainer.VirtualTrainerContext context, Guid projectUniqueKey, string ExclusionsName)
        {
            ExclusionsGroup egroup = context.ExclusionsGroups.Where(a => a.GroupName == ExclusionsName).FirstOrDefault();
            if (egroup == null)
            {
                egroup = new ExclusionsGroup()
                {
                    GroupName = ExclusionsName,
                    ProjectId = projectUniqueKey,
                    AddedBy = "Deployment",
                    DateAdded = DateTime.Now
                };
                context.ExclusionsGroups.Add(egroup);
                context.SaveChanges();
            }
            return egroup;
        }
        private void AddUserSystemPermissions(VirtualTrainer.VirtualTrainerContext context, TargetSystem targetSystem)
        {
            List<string> usersNames = new List<string>();
            usersNames.Add("emea\\a-ajfooks");
            usersNames.Add("emea\\afooks");
            usersNames.Add("emea\\v-czera");
            usersNames.Add("emea\\AChristie");

            List<RoleEnum> SysteRoles = new List<RoleEnum>();
            SysteRoles.Add(RoleEnum.SystemAdmin);
            SysteRoles.Add(RoleEnum.SuperUser);

            foreach(var userName in usersNames)
            {
                User user = context.User.Where(u => u.DomainName == userName).FirstOrDefault();

                if (user != null)
                {
                    foreach (RoleEnum role in SysteRoles)
                    {
                        SystemPermission perm = context.SystemPermissions.Where(p => p.UserId == user.Id && p.RoleId == (int)role).FirstOrDefault();
                        if (perm == null)
                        {
                            perm = new SystemPermission()
                            {
                                User = user,
                                RoleId = (int)role
                            };
                            context.SystemPermissions.Add(perm);
                            context.SaveChanges();
                        }
                    }
                }
            }            
        }

        private void SetUpEscalations(VirtualTrainerContext context, Project project, Assembly assembly, TargetSystem targetSystem)
        {
            EscalationsFramework ef = context.EscalationsFrameWork.Where(e => e.Id == project.ProjectUniqueKey).FirstOrDefault();
            if (ef == null)
            {
                ef = new EscalationsFramework()
                {
                    Id = project.ProjectUniqueKey,
                    IsActive = true,
                    Project = project
                };
                context.EscalationsFrameWork.Add(ef);
                context.SaveChanges();
            }

            #region [ Email 'Project' People All breaches ]

           // string emailTextToSpecificUser = new StreamReader(assembly.GetManifestResourceStream("VirtualTrainer.EmailTemplates.RazorEmailToUser.txt")).ReadToEnd();
            string efConfigAllConToAndy = "Email User All PNO results";

            EscalationsFrameworkRuleConfigEmailUser efruleconfigemasiluser = context.EscalationsFrameworkRuleConfigEmailUsers.Where(a => a.Name == efConfigAllConToAndy).FirstOrDefault();
            if (efruleconfigemasiluser == null)
            {
                string SystemSentFromEmailAddress = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromEmailAddress.ToString()];
                string SystemSentFromName = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromName.ToString()];

                efruleconfigemasiluser = new EscalationsFrameworkRuleConfigEmailUser
                {
                    BreachCount = 1,
                    ProjectId = project.ProjectUniqueKey,
                    Project = project,
                    EmailBodyTemplate = "RazorEmailToUser.cshtml",//emailTextToSpecificUser,
                    EmailSubjectTemplate = "All PNO Breaches For UAT",
                    //ScheduleFrequencyId = (int)EscalationsFrameworkScheduleFrequencyEnum.Development,
                    //ScheduleStartDate = DateTime.Now.Date,
                    ScheduleId = context.Schedules.Where(s => s.Name == defaultScheduleName).FirstOrDefault().Id,
                    //SentFromEmail = SystemSentFromEmailAddress,
                    // SentFromUserName = SystemSentFromName,
                    IsActive = false,
                    EscalationsFramework = ef,
                    Name = efConfigAllConToAndy,
                    AttachExcelOfBreaches = false
                };
                context.EscalationsFrameWorkRuleConfigurations.Add(efruleconfigemasiluser);
                context.SaveChanges();
            }

            User recipientAndy = context.User.Where(u => u.Name.ToLower() == "andy fooks").FirstOrDefault();

            // add source and recipients
            EscalationsEmailRecipient escEmailRecipientAndy = context.EscalationsEmailRecipients.Where(a => a.RecipientId == recipientAndy.Id && a.EscalationsFrameworkRuleConfigId == efruleconfigemasiluser.Id).FirstOrDefault();
            if (escEmailRecipientAndy == null)
            {
                escEmailRecipientAndy = new EscalationsEmailRecipient()
                {
                    EscalationsFrameworkRuleConfigId = efruleconfigemasiluser.Id,
                    IsActive = true,
                    RecipientId = recipientAndy.Id
                };
                context.EscalationsEmailRecipients.Add(escEmailRecipientAndy);
                context.SaveChanges();
            }

            // Add other business users as recipients.
            if (targetSystem == TargetSystem.UAT)
            {
                // Fiona Haston
                User recipientFiona = context.User.Where(u => u.Name.ToLower() == "fiona haston").FirstOrDefault();
                EscalationsEmailRecipient escEmailRecipientFiona = context.EscalationsEmailRecipients.Where(a => a.RecipientId == recipientFiona.Id && a.EscalationsFrameworkRuleConfigId == efruleconfigemasiluser.Id).FirstOrDefault();
                if (escEmailRecipientFiona == null)
                {
                    escEmailRecipientFiona = new EscalationsEmailRecipient()
                    {
                        EscalationsFrameworkRuleConfigId = efruleconfigemasiluser.Id,
                        IsActive = false,
                        RecipientId = recipientFiona.Id
                    };
                    context.EscalationsEmailRecipients.Add(escEmailRecipientFiona);
                    context.SaveChanges();
                }
                User recipientColin = context.User.Where(u => u.Name.ToLower() == "colin young").FirstOrDefault();
                EscalationsEmailRecipient escEmailRecipientColin = context.EscalationsEmailRecipients.Where(a => a.RecipientId == recipientColin.Id && a.EscalationsFrameworkRuleConfigId == efruleconfigemasiluser.Id).FirstOrDefault();
                if (escEmailRecipientColin == null)
                {
                    escEmailRecipientColin = new EscalationsEmailRecipient()
                    {
                        EscalationsFrameworkRuleConfigId = efruleconfigemasiluser.Id,
                        IsActive = false,
                        RecipientId = recipientColin.Id
                    };
                    context.EscalationsEmailRecipients.Add(escEmailRecipientColin);
                    context.SaveChanges();
                }
            }

            Rule rule = context.Rules.Where(r => r.Name == "PNO").FirstOrDefault();
            EscalationsFrameworkBreachSource ruleBreachSource = context
                .EscalationsFrameworkBreachSourcesRule
                .Where(a => a.RuleId == rule.Id
                        && a.ProjectId == project.ProjectUniqueKey
                        && a.EscalationsFrameworkRuleConfigId == efruleconfigemasiluser.Id)
                .FirstOrDefault();

            if (ruleBreachSource == null)
            {
                ruleBreachSource = new EscalationsFrameworkBreachSourceRule()
                {
                    ProjectId = project.ProjectUniqueKey,
                    EscalationsFrameworkRuleConfigId = efruleconfigemasiluser.Id,
                    RuleId = rule.Id,
                    IsActive = true
                };
                context.EscalationsFrameworkBreachSources.Add(ruleBreachSource);
                context.SaveChanges();
            }

            #endregion

            #region [ Handlers ]

            //string handlerRoleEmailTemplate = new StreamReader(assembly.GetManifestResourceStream("VirtualTrainer.EmailTemplates.HandlerRoleEmailTemplate.txt")).ReadToEnd();
            string EscalationsFrameworkRuleConfigEmailHandlersName = "Email Handlers";

            EscalationsFrameworkRuleConfigEmailRole efruleconfigemailHandlers = context.EscalationsFrameworkRuleConfigEmailRoles.Where(a => a.Name == EscalationsFrameworkRuleConfigEmailHandlersName).FirstOrDefault();
            if (efruleconfigemailHandlers == null)
            {
                string SystemSentFromEmailAddress = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromEmailAddress.ToString()];
                string SystemSentFromName = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromName.ToString()];

                efruleconfigemailHandlers = new EscalationsFrameworkRuleConfigEmailRole
                {
                    BreachCount = 1,
                    ProjectId = project.ProjectUniqueKey,
                    Project = project,
                    EmailBodyTemplate = "RazorEmailToHandlerTemplate.cshtml", //handlerRoleEmailTemplate,
                    EmailSubjectTemplate = "Virtual Trainer - Processing Alert",
                    ScheduleId = context.Schedules.Where(s => s.Name == defaultScheduleName).FirstOrDefault().Id,
                    //SentFromEmail = SystemSentFromEmailAddress,
                    //SentFromUserName = SystemSentFromName,
                    IsActive = true,
                    ActionId = (int)ActionEnum.EmailToHandler,
                    OverrideRecipientEmail = SystemSentFromEmailAddress,
                    EscalationsFramework = ef,
                    Name = EscalationsFrameworkRuleConfigEmailHandlersName,
                    AttachExcelOfBreaches = false,
                    Description = "Email Handlers their breaches."
                };
                context.EscalationsFrameWorkRuleConfigurations.Add(efruleconfigemailHandlers);
                context.SaveChanges();
            }

            EscalationsFrameworkBreachSource ruleBreachSourceForHandler = context
                .EscalationsFrameworkBreachSourcesRule
                .Where(a => a.RuleId == rule.Id
                        && a.ProjectId == project.ProjectUniqueKey
                        && a.EscalationsFrameworkRuleConfigId == efruleconfigemailHandlers.Id)
                .FirstOrDefault();

            if (ruleBreachSourceForHandler == null)
            {
                ruleBreachSourceForHandler = new EscalationsFrameworkBreachSourceRule()
                {
                    ProjectId = project.ProjectUniqueKey,
                    EscalationsFrameworkRuleConfigId = efruleconfigemailHandlers.Id,
                    RuleId = rule.Id,
                    IsActive = true
                };
                context.EscalationsFrameworkBreachSources.Add(ruleBreachSourceForHandler);
                context.SaveChanges();
            }

            #endregion

            #region [ Managers ]

            //string managerRoleEmailTemplate = new StreamReader(assembly.GetManifestResourceStream("VirtualTrainer.EmailTemplates.ManagerRoleEmailTemplate.txt")).ReadToEnd();
            string EscalationsFrameworkRuleConfigEmailManagersName = "Email Managers";

            EscalationsFrameworkRuleConfigEmailRole efruleconfigemailManagers = context.EscalationsFrameworkRuleConfigEmailRoles.Where(a => a.Name == EscalationsFrameworkRuleConfigEmailManagersName).FirstOrDefault();
            if (efruleconfigemailManagers == null)
            {
                string SystemSentFromEmailAddress = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromEmailAddress.ToString()];
                string SystemSentFromName = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromName.ToString()];

                efruleconfigemailManagers = new EscalationsFrameworkRuleConfigEmailRole
                {
                    BreachCount = 3,
                    ProjectId = project.ProjectUniqueKey,
                    Project = project,
                    EmailBodyTemplate = "RazorEmailToManagerTemplate.cshtml", //managerRoleEmailTemplate,
                    EmailSubjectTemplate = "Virtual Trainer - Processing Alert",
                    ScheduleId = context.Schedules.Where(s => s.Name == defaultScheduleName).FirstOrDefault().Id,
                    //SentFromEmail = SystemSentFromEmailAddress,
                    //SentFromUserName = SystemSentFromName,
                    IsActive = true,
                    ActionId = (int)ActionEnum.EmailToBranchManager,
                    OverrideRecipientEmail = SystemSentFromEmailAddress,
                    EscalationsFramework = ef,
                    Name = EscalationsFrameworkRuleConfigEmailManagersName,
                    AttachExcelOfBreaches = false,
                    Description = "Email Managers, their handlers' breaches."
                };
                context.EscalationsFrameWorkRuleConfigurations.Add(efruleconfigemailManagers);
                context.SaveChanges();
            }

            EscalationsFrameworkBreachSource ruleBreachSourceForManager = context
                .EscalationsFrameworkBreachSourcesRule
                .Where(a => a.RuleId == rule.Id
                        && a.ProjectId == project.ProjectUniqueKey
                        && a.EscalationsFrameworkRuleConfigId == efruleconfigemailManagers.Id)
                .FirstOrDefault();

            if (ruleBreachSourceForManager == null)
            {
                ruleBreachSourceForManager = new EscalationsFrameworkBreachSourceRule()
                {
                    ProjectId = project.ProjectUniqueKey,
                    EscalationsFrameworkRuleConfigId = efruleconfigemailManagers.Id,
                    RuleId = rule.Id,
                    IsActive = true
                };
                context.EscalationsFrameworkBreachSources.Add(ruleBreachSourceForManager);
                context.SaveChanges();
            }

            #endregion

            #region [ QA ]

            //string qaRoleEmailTemplate = new StreamReader(assembly.GetManifestResourceStream("VirtualTrainer.EmailTemplates.QualityAdvisorsRoleEmailTemplate.txt")).ReadToEnd();
            string EscalationsFrameworkRuleConfigEmailQAName = "Email QA";

            EscalationsFrameworkRuleConfigEmailRole efruleconfigemailQA = context.EscalationsFrameworkRuleConfigEmailRoles.Where(a => a.Name == EscalationsFrameworkRuleConfigEmailQAName).FirstOrDefault();
            if (efruleconfigemailQA == null)
            {
                string SystemSentFromEmailAddress = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromEmailAddress.ToString()];
                string SystemSentFromName = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromName.ToString()];

                efruleconfigemailQA = new EscalationsFrameworkRuleConfigEmailRole
                {
                    BreachCount = 4,
                    ProjectId = project.ProjectUniqueKey,
                    Project = project,
                    EmailBodyTemplate = "RazorEmailToQualityAuditorTemplate.cshtml", //qaRoleEmailTemplate,
                    EmailSubjectTemplate = "Virtual Trainer - Processing Alert",
                    ScheduleId = context.Schedules.Where(s => s.Name == defaultScheduleName).FirstOrDefault().Id,
                    //SentFromEmail = SystemSentFromEmailAddress,
                    //SentFromUserName = SystemSentFromName,
                    IsActive = true,
                    ActionId = (int)ActionEnum.EmailToQualityAuditor,
                    OverrideRecipientEmail = SystemSentFromEmailAddress,
                    EscalationsFramework = ef,
                    Name = EscalationsFrameworkRuleConfigEmailQAName,
                    AttachExcelOfBreaches = false,
                    Description = "Email Quality Advisors, their handlers' breaches."
                };
                context.EscalationsFrameWorkRuleConfigurations.Add(efruleconfigemailQA);
                context.SaveChanges();
            }

            EscalationsFrameworkBreachSource ruleBreachSourceForQA = context
                .EscalationsFrameworkBreachSourcesRule
                .Where(a => a.RuleId == rule.Id
                        && a.ProjectId == project.ProjectUniqueKey
                        && a.EscalationsFrameworkRuleConfigId == efruleconfigemailQA.Id)
                .FirstOrDefault();

            if (ruleBreachSourceForQA == null)
            {
                ruleBreachSourceForQA = new EscalationsFrameworkBreachSourceRule()
                {
                    ProjectId = project.ProjectUniqueKey,
                    EscalationsFrameworkRuleConfigId = efruleconfigemailQA.Id,
                    RuleId = rule.Id,
                    IsActive = true
                };
                context.EscalationsFrameworkBreachSources.Add(ruleBreachSourceForQA);
                context.SaveChanges();
            }

            #endregion

            #region [ RMD ]

            // string RMDRoleEmailTemplate = new StreamReader(assembly.GetManifestResourceStream("VirtualTrainer.EmailTemplates.RegionalManagingDirectorRoleEmailTemplate.txt")).ReadToEnd();
            string EscalationsFrameworkRuleConfigEmailRMDName = "Email RMD";

            EscalationsFrameworkRuleConfigEmailRole efruleconfigemailRMD = context.EscalationsFrameworkRuleConfigEmailRoles.Where(a => a.Name == EscalationsFrameworkRuleConfigEmailRMDName).FirstOrDefault();
            if (efruleconfigemailRMD == null)
            {
                string SystemSentFromEmailAddress = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromEmailAddress.ToString()];
                string SystemSentFromName = ConfigurationManager.AppSettings[AppSettingsEnum.SystemSentFromName.ToString()];

                efruleconfigemailRMD = new EscalationsFrameworkRuleConfigEmailRole
                {
                    BreachCount = 4,
                    ProjectId = project.ProjectUniqueKey,
                    Project = project,
                    EmailBodyTemplate = "RazorEmailToRegionalManagerTemplate.cshtml", //RMDRoleEmailTemplate,
                    EmailSubjectTemplate = "Virtual Trainer - Processing Alert",
                    ScheduleId = context.Schedules.Where(s => s.Name == defaultScheduleName).FirstOrDefault().Id,
                    //SentFromEmail = SystemSentFromEmailAddress,
                    //SentFromUserName = SystemSentFromName,
                    IsActive = true,
                    ActionId = (int)ActionEnum.EmailToRegionalManager,
                    OverrideRecipientEmail = SystemSentFromEmailAddress,
                    EscalationsFramework = ef,
                    Name = EscalationsFrameworkRuleConfigEmailRMDName,
                    AttachExcelOfBreaches = false,
                    Description = "Email Regional Managing Director, their handlers' breaches."
                };
                context.EscalationsFrameWorkRuleConfigurations.Add(efruleconfigemailRMD);
                context.SaveChanges();
            }

            EscalationsFrameworkBreachSource ruleBreachSourceForRMD = context
                .EscalationsFrameworkBreachSourcesRule
                .Where(a => a.RuleId == rule.Id
                        && a.ProjectId == project.ProjectUniqueKey
                        && a.EscalationsFrameworkRuleConfigId == efruleconfigemailRMD.Id)
                .FirstOrDefault();

            if (ruleBreachSourceForRMD == null)
            {
                ruleBreachSourceForRMD = new EscalationsFrameworkBreachSourceRule()
                {
                    ProjectId = project.ProjectUniqueKey,
                    EscalationsFrameworkRuleConfigId = efruleconfigemailRMD.Id,
                    RuleId = rule.Id,
                    IsActive = true
                };
                context.EscalationsFrameworkBreachSources.Add(ruleBreachSourceForRMD);
                context.SaveChanges();
            }

            #endregion
        }
        private void AddUsers(TargetSystem targetSystem, VirtualTrainer.VirtualTrainerContext context)
        {
            switch (targetSystem)
            {
                case TargetSystem.Dev:
                case TargetSystem.Test:
                case TargetSystem.UAT:
                case TargetSystem.Prod:
                    User andyAAccountDevNewPC = context.User.Where(a => a.DomainName == "emea\\a-ajfooks").FirstOrDefault();
                    if (andyAAccountDevNewPC == null)
                    {
                        andyAAccountDevNewPC = new User()
                        {
                            //ProjectUniqueKey = project.ProjectUniqueKey,
                            DomainName = "emea\\a-ajfooks",
                            Email = "andy_fooks@ajg.com",
                            IsActive = true,
                            Name = "Andy Fooks",
                            TitleId = (int)TitleEnum.Mr
                        };
                        context.User.AddOrUpdate(andyAAccountDevNewPC);
                    }
                
                    User andyTest = context.User.Where(a => a.DomainName == "emea\\afooks").FirstOrDefault();
                    if (andyTest == null)
                    {
                        andyTest = new User()
                        {
                            //ProjectUniqueKey = project.ProjectUniqueKey,
                            DomainName = "emea\\afooks",
                            Email = "andy_fooks@ajg.com",
                            IsActive = true,
                            Name = "Andy Fooks",
                            TitleId = (int)TitleEnum.Mr
                        };
                        context.User.AddOrUpdate(andyTest);
                    }
                    User andrewTest = context.User.Where(a => a.DomainName == "EMEA\\achristie").FirstOrDefault();
                    if (andrewTest == null)
                    {
                        andrewTest = new User()
                        {
                            //ProjectUniqueKey = project.ProjectUniqueKey,
                            DomainName = "EMEA\\achristie",
                            Email = "andrew_christie@ajg.com",
                            IsActive = true,
                            Name = "Andrew Christie",
                            TitleId = (int)TitleEnum.Mr
                        };
                        context.User.AddOrUpdate(andrewTest);
                    }
                    User Claudiotest = context.User.Where(a => a.DomainName == "emea\\v-czera").FirstOrDefault();
                    if (Claudiotest == null)
                    {
                        Claudiotest = new User()
                        {
                            //ProjectUniqueKey = project.ProjectUniqueKey,
                            DomainName = "emea\\v-czera",
                            Email = "Claudio_Zera@ajg.com",
                            IsActive = true,
                            Name = "Claudio Zera",
                            TitleId = (int)TitleEnum.Mr
                        };
                        context.User.AddOrUpdate(Claudiotest);
                    }
                    context.SaveChanges();
                    break;
            }
        }
        private void SetUpRuleConfigRuleStoredProcedureInputValues(VirtualTrainer.VirtualTrainerContext context, Rule rule, Project project)
        {
            rule.LoadContextObjects(context);
            switch (rule.Name)
            {
                case "PNO":
                    ExclusionsGroup eGroup = CreateExclusionsGroup(context, rule.ProjectId, "PNO Context ref Exclusions");
                    ExclusionsGroup PNOProductKeysEGroup = CreateExclusionsGroup(context, rule.ProjectId, "PNO Product Exclusions");
                    ExclusionsGroup PNOAgentKeysEGroup = CreateExclusionsGroup(context, rule.ProjectId, "PNO Agent Exclusions");

                    // Set up input values for all PNO configurations
                    if (rule.RuleConfigurations.Any())
                    {
                        foreach (RuleConfiguration ruleconfig in rule.RuleConfigurations)
                        {
                            ruleconfig.LoadContextObjects(context);
                            string sqlParameterName_contextref = "@excludedContextRefs";
                            AddExclusionsInputValueToRuleConfig(sqlParameterName_contextref, eGroup.Id, "Group of Context refs that should not be returned as PNO Breaches.", ruleconfig, project.ProjectUniqueKey, context);

                            ruleconfig.LoadContextObjects(context);
                            string sqlParameterName_productKeys = "@excludedProductKeys";
                            AddExclusionsInputValueToRuleConfig(sqlParameterName_productKeys, PNOProductKeysEGroup.Id, "Group of products that should not be returned as PNO Breaches.", ruleconfig, project.ProjectUniqueKey, context);

                            ruleconfig.LoadContextObjects(context);
                            string sqlParameterName_AgentKeys = "@excludedAgentKeys";
                            AddExclusionsInputValueToRuleConfig(sqlParameterName_AgentKeys, PNOAgentKeysEGroup.Id, "Group of Agents that should not be returned as PNO Breaches.", ruleconfig, project.ProjectUniqueKey, context);
                        }
                    }
                    break;
                case "CON":
                    ExclusionsGroup ConContextRefEGroup = CreateExclusionsGroup(context, rule.ProjectId, "CON Context ref Exclusions");
                    ExclusionsGroup ConProductKeysEGroup = CreateExclusionsGroup(context, rule.ProjectId, "CON Product Exclusions");
                    ExclusionsGroup ConAgentKeysEGroup = CreateExclusionsGroup(context, rule.ProjectId, "CON Agent Exclusions");

                    // Set up input values for all CON configurations
                    if (rule.RuleConfigurations.Any())
                    {
                        foreach (RuleConfiguration ruleconfig in rule.RuleConfigurations)
                        {
                            ruleconfig.LoadContextObjects(context);
                            string sqlParameterName_contextref = "@excludedContextRefs";
                            AddExclusionsInputValueToRuleConfig(sqlParameterName_contextref, ConContextRefEGroup.Id, "Group of Context refs that should not be returned as CON Breaches.", ruleconfig, project.ProjectUniqueKey, context);

                            ruleconfig.LoadContextObjects(context);
                            string sqlParameterName_productKeys = "@excludedProductKeys";
                            AddExclusionsInputValueToRuleConfig(sqlParameterName_productKeys, ConProductKeysEGroup.Id, "Group of products that should not be returned as CON Breaches.", ruleconfig, project.ProjectUniqueKey, context);

                            ruleconfig.LoadContextObjects(context);
                            string sqlParameterName_AgentKeys = "@excludedAgentKeys";
                            AddExclusionsInputValueToRuleConfig(sqlParameterName_AgentKeys, ConAgentKeysEGroup.Id, "Group of Agents that should not be returned as CON Breaches.", ruleconfig, project.ProjectUniqueKey, context);
                        }
                    }
                    break;
            }
        }
        private void AddExclusionsInputValueToRuleConfig(string sqlParameterName, int exclusionsGroupId, string description, RuleConfiguration ruleconfig, Guid projectUniqueKey, VirtualTrainerContext context)
        {
            RuleStoredProcedureInputValue ruleInputValue = ruleconfig.RuleStoredProcedureInputValues.Where(a => a.RuleConfigurationId == ruleconfig.Id && a.ParameterName == sqlParameterName).FirstOrDefault();

            if (ruleInputValue == null)
            {
                ruleInputValue = new RuleStoredProcedureInputValueExclusionsGroup()
                {
                    RuleConfigurationId = ruleconfig.Id,
                    ProjectId = projectUniqueKey,
                    ParameterName = sqlParameterName,
                    Description = description,
                    ExclusionsGroupId = exclusionsGroupId
                };
                ruleconfig.RuleStoredProcedureInputValues.Add(ruleInputValue);
                context.SaveChanges();
            }
        }
        private class RuleConfigSetup
        {
            public string RuleName { get; set; }
            public string SqlCommandText { get; set; }
            public bool SqlCommandTextIsStoredProc { get; set; }
            public string RuleDescription { get; set; }
        }

        private string GetRuleConfigSqlQueryText(SqlQueryTextReplacementDetails scriptInfo, VirtualTrainer.VirtualTrainerContext context, Assembly _assembly)
        {
            string LogBreachScript = new StreamReader(_assembly.GetManifestResourceStream(scriptInfo.ManifestResourceStreamPathSqlString)).ReadToEnd();
            LogBreachScript = LogBreachScript.Replace("<%ServerandDBName%>", scriptInfo.ServerAndDbName);
            return LogBreachScript;
        }
        private class SqlQueryTextReplacementDetails
        {
            public string ServerAndDbName { get; set; }
            public string ManifestResourceStreamPathSqlString { get; set; }
        }
        private void SetUpRuleConfigurations(TargetSystem targetSystem, VirtualTrainer.VirtualTrainerContext context, Rule rule, Project project, TargetDatabaseDetails targetDBDetails, Assembly _assembly)
        {
            rule.LoadContextObjects(context);
            List<RuleConfigSetup> RuleConfigSetups = new List<RuleConfigSetup>();
            switch (targetSystem)
            {
                case TargetSystem.Dev:
                    
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}Lloyds", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[dm_Acturis_Lloyds_60]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "Lloyds"
                    });
                    RuleConfigSetups.Add(
                        new Configuration.RuleConfigSetup()
                        {
                            RuleName = string.Format("{0}GHIS", rule.Name),
                            SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[dm_Acturis_GHIS]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                            SqlCommandTextIsStoredProc = false,
                            RuleDescription = "GHIS"
                        });
                    break;
                case TargetSystem.Test:
                case TargetSystem.UAT:
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}AJG", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesAJG]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ajg]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "AJG"
                    });
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}Ink", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesInk]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ink]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "Ink"
                    });
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}NABGiles", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesNABGiles]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_nab]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "NAB"
                    });
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}RBSGiles", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesRBSGiles]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_rbs]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "RBS Giles"
                    });
                    break;
                case TargetSystem.Prod:
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}AJG", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesAJG]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_ajg]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "AJG"
                    });
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}Ink", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesInk]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_ink]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "Ink"
                    });
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}NABGiles", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesNABGiles]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_nab]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "NAB"
                    });
                    RuleConfigSetups.Add(new Configuration.RuleConfigSetup()
                    {
                        RuleName = string.Format("{0}RBSGiles", rule.Name),
                        //SqlCommandText = string.Format("[VirtualTrainer].[dbo].[Log{0}BreachesRBSGiles]", rule.Name),
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_rbs]", ManifestResourceStreamPathSqlString = string.Format("VirtualTrainer.SQLScripts.RuleScripts.Log{0}Breaches.sql", rule.Name) }, context, _assembly),
                        SqlCommandTextIsStoredProc = false,
                        RuleDescription = "RBS Giles"
                    });
                    break;
            }

            foreach (RuleConfigSetup setup in RuleConfigSetups)
            {
                RuleConfiguration ruleConfiguration = context.RuleConfigurations.Where(a => a.Name == setup.RuleName).FirstOrDefault();
                if (ruleConfiguration == null)
                {
                    ruleConfiguration = new RuleConfiguration()
                    {
                        Name = setup.RuleName,
                        Description = setup.RuleDescription,
                        IsActive = true,
                        RuleId = rule.Id,
                        ProjectId = project.ProjectUniqueKey,
                        RuleTarget = RuleTarget.User,
                        SqlCommandText = setup.SqlCommandText,
                        TargetDbID = targetDBDetails.Id,
                        UserTargetExecutionMode = RuleConfigExecutionMode.UserReferencedResults,
                        UserIdentifyingResultsFieldName = "AccountHandlerUserKey",
                        UserPropertyName = "Aliases",
                        ScheduleId = context.Schedules.Where(s => s.Name == defaultScheduleName).FirstOrDefault().Id,
                        SetBreachesToResolvedScheduleId = context.Schedules.Where(s => s.Name == neverScheduleName).FirstOrDefault().Id,
                        SqlCommandTextIsStoredProc = setup.SqlCommandTextIsStoredProc
                    };
                    rule.RuleConfigurations.Add(ruleConfiguration);
                    context.SaveChanges();
                }
            }
        }
        private class ActurisImportSetup
        {
            public string SyncConfigName { get; set; }
            public string SyncConfigDescription { get; set; }
            public string SqlCommandText { get; set; }
            public bool SqlCommandTextIsStoredProc { get; set; }
        }
        private void SetUpActurisBusinessDataImport(TargetSystem targetSystem, VirtualTrainer.VirtualTrainerContext context, Project project, TargetDatabaseDetails targetDBDetails, Assembly _assembly)
        {
            List<ActurisImportSetup> importSetUps = new List<ActurisImportSetup>();

            switch (targetSystem)
            {
                case TargetSystem.Dev:
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "AJGLlyods",
                        SyncConfigDescription = "AJGLlyods Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[dm_Acturis_Lloyds_60]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureAJGLloyds",
                        SqlCommandTextIsStoredProc = false
                    });
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "GHIS",
                        SyncConfigDescription = "GHIS Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[dm_Acturis_GHIS]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureGHIS",
                        SqlCommandTextIsStoredProc = false
                    });
                    break;
                case TargetSystem.Test:
                case TargetSystem.UAT:
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "AJG",
                        SyncConfigDescription = "AJG Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ajg]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureAJG",
                        SqlCommandTextIsStoredProc = false
                    });
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "Ink",
                        SyncConfigDescription = "Ink Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ink]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureInk",
                        SqlCommandTextIsStoredProc = false
                    });
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "NABGiles",
                        SyncConfigDescription = "NAB Giles Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_nab]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureNABGiles",
                        SqlCommandTextIsStoredProc = false
                    });
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "RBSGiles",
                        SyncConfigDescription = "RBS Giles Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[acturis_nonprod].[acturis_dw_rbs]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureRBSGiles",
                        SqlCommandTextIsStoredProc = false
                    });
                    break;
                case TargetSystem.Prod:
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "AJG",
                        SyncConfigDescription = "AJG Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_ajg]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureAJG",
                        SqlCommandTextIsStoredProc = false
                    });
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "Ink",
                        SyncConfigDescription = "Ink Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_ink]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureInk",
                        SqlCommandTextIsStoredProc = false
                    });
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "NABGiles",
                        SyncConfigDescription = "NAB Giles Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_nab]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureNABGiles",
                        SqlCommandTextIsStoredProc = false
                    });
                    importSetUps.Add(new ActurisImportSetup()
                    {
                        SyncConfigName = "RBSGiles",
                        SyncConfigDescription = "RBS Giles Acturis Database",
                        SqlCommandText = GetRuleConfigSqlQueryText(new SqlQueryTextReplacementDetails() { ServerAndDbName = @"[ACTURISDW3].[acturis_dw_rbs]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" }, context, _assembly),
                        //SqlCommandText = "GetUserBusinessStructureRBSGiles",
                        SqlCommandTextIsStoredProc = false
                    });
                    break;
            }

            foreach (ActurisImportSetup importSetUp in importSetUps)
            {
                ActurisBusinessStructureSyncConfig syncConfig = context.ActurisBusinessStructureSyncConfigs.Where(r => r.Name == importSetUp.SyncConfigName && r.ProjectId == project.ProjectUniqueKey).FirstOrDefault();
                if (syncConfig == null)
                {
                    syncConfig = new ActurisBusinessStructureSyncConfig()
                    {
                        Name = importSetUp.SyncConfigName,
                        SqlCommandText = importSetUp.SqlCommandText,
                        SqlCommandTextIsStoredProc = importSetUp.SqlCommandTextIsStoredProc,
                        Description = importSetUp.SyncConfigDescription,
                        IsActive = false,
                        ProjectId = project.ProjectUniqueKey,
                        TargetDatabaseDetails = targetDBDetails,
                        ScheduleId = context.Schedules.Where(s => s.Name == defaultScheduleName).FirstOrDefault().Id
                    };
                    context.ActurisBusinessStructureSyncConfigs.Add(syncConfig);
                    context.SaveChanges();
                }
            }
        }        
        private TargetDatabaseDetails CreateDefaultDBConnectionDetails(VirtualTrainer.VirtualTrainerContext context, Project project)
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings["VirtualTrainer"];

            if (connectionStringSetting != null)
            {
                string dataSourceValueAKAServerName = connectionStringSetting.ConnectionString.Split(';').Where(a => a.ToLower().StartsWith("data source")).FirstOrDefault().ToString().Split('=')[1].Trim();

                string connectionStringName = connectionStringSetting.Name;
                Guid ProjectUniqueKey = project.ProjectUniqueKey;

                // Create the default DB connection object.
                TargetDatabaseDetails vtDbDetails = context.TargetDatabaseDetails.Where(db => db.DisplayName == connectionStringName && db.ProjectId == ProjectUniqueKey).FirstOrDefault();
                if (vtDbDetails == null)
                {
                    vtDbDetails = new TargetDatabaseDetails()
                    {
                        DBConnectionString = connectionStringSetting.ConnectionString,
                        DBName = connectionStringSetting.Name,
                        DBServerName = dataSourceValueAKAServerName,
                        DisplayName = connectionStringSetting.Name,
                        IsActive = true,
                        Key = 121,
                        Project = project
                    };
                    context.TargetDatabaseDetails.Add(vtDbDetails);
                    context.SaveChanges();
                }
                return vtDbDetails;
            }
            return null;
        }
        private void CreateRulesAndRuleConfigs(VirtualTrainer.VirtualTrainerContext context, Project project, TargetSystem targetSystem, TargetDatabaseDetails dbDetails, Assembly _assembly)
        {
            Rule pnoRule = context.Rules.Where(r => r.Name == "PNO" && r.ProjectId == project.ProjectUniqueKey).FirstOrDefault();
            if (pnoRule == null)
            {
                pnoRule = new Rule()
                {
                    Project = project,
                    Name = "PNO",
                    Description = "Missing or Invalid Policy Number. Policy Numbers are used to make payments to insurers.",
                    AdditionalDescription = "To resolve this issue, please input the correct Policy Number at all Policy levels that are Live or Accepted.",
                    IsActive = true
                };
                context.Rules.Add(pnoRule);
                context.SaveChanges();
            }
            SetUpRuleConfigurations(targetSystem, context, pnoRule, project, dbDetails, _assembly);
            SetUpRuleConfigRuleStoredProcedureInputValues(context, pnoRule, project);

            Rule conRule = context.Rules.Where(r => r.Name == "CON" && r.ProjectId == project.ProjectUniqueKey).FirstOrDefault();
            if (conRule == null)
            {
                conRule = new Rule()
                {
                    Project = project,
                    Name = "CON",
                    Description = "Missing Primary Contact Details. No Salutation, Telephone Number or Email Address of the Primary Contact or Date of Birth of Director contact is provided.",
                    AdditionalDescription = "Please fill in the Salutation field, Email and at least one phone number for the Primary Contact. Also, at least one of the Client's contacts must be a Director or decision maker with the Date of Birth recorded.",
                    IsActive = false
                };
                context.Rules.Add(conRule);
                context.SaveChanges();
            }
            SetUpRuleConfigurations(targetSystem, context, conRule, project, dbDetails, _assembly);
            SetUpRuleConfigRuleStoredProcedureInputValues(context, conRule, project);
        }
        private class InstallStoredProc
        {
            public string SprocName { get; set; }
            public string ServerAndDbName { get; set; }
            public string ManifestResourceStreamPathSqlString { get; set; }
        }
        private void InstallScripts(List<InstallStoredProc> storedProcsToIntall, VirtualTrainer.VirtualTrainerContext context, Assembly _assembly)
        {
            foreach (InstallStoredProc scriptInfo in storedProcsToIntall)
            {
                string LogBreachScript = new StreamReader(_assembly.GetManifestResourceStream(scriptInfo.ManifestResourceStreamPathSqlString)).ReadToEnd();
                LogBreachScript = LogBreachScript.Replace("<%ServerandDBName%>", scriptInfo.ServerAndDbName).Replace("<%ProcedureName%>", scriptInfo.SprocName);
                context.Database.ExecuteSqlCommand(string.Format(@"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}') AND type IN ( N'P', N'PC' )) DROP Procedure {0}", scriptInfo.SprocName));
                context.Database.ExecuteSqlCommand(LogBreachScript);
            }
        }

        private void InstallActurisBusinessStructureSyncScripts(TargetSystem targetSystem, VirtualTrainer.VirtualTrainerContext context, Assembly _assembly)
        {
            List<InstallStoredProc> storedProcsToIntall = new List<InstallStoredProc>();

            // Populate the List with set up details for requried sproc for given environment
            switch (targetSystem)
            {
                case TargetSystem.Dev:
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[GetUserBusinessStructureAJGLloyds]", ServerAndDbName = @"[dm_Acturis_Lloyds_60]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[GetUserBusinessStructureGHIS]", ServerAndDbName = @"[dm_Acturis_GHIS]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" });
                    break;
                case TargetSystem.Test:
                case TargetSystem.UAT:
                    // At the moment, Test and UAT are the same. e.g they are both looking at linked server to UAT DW3
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[GetUserBusinessStructureAJG]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ajg]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[GetUserBusinessStructureInk]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ink]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[GetUserBusinessStructureNABGiles]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_nab]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[GetUserBusinessStructureRBSGiles]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_rbs]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.ActurisBusinessStructureSyncScripts.GetUserBusinessStructure.sql" });
                    break;
                case TargetSystem.Prod:
                    break;
            }
            // Install the Procs.
            InstallScripts(storedProcsToIntall, context, _assembly);
        }
        private void InstallRuleScripts(TargetSystem targetSystem, VirtualTrainer.VirtualTrainerContext context, Assembly _assembly)
        {
            List<InstallStoredProc> storedProcsToIntall = new List<InstallStoredProc>();
            // Populate the List with set up details for requried sproc for given environment
            switch (targetSystem)
            {
                case TargetSystem.Dev:
                    // CON
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogCONBreachesAJGLloyds]", ServerAndDbName = @"[dm_Acturis_Lloyds_60]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogCONBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogCONBreachesGHIS]", ServerAndDbName = @"[dm_Acturis_GHIS]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogCONBreaches.sql" });
                    // PNO
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogPNOBreachesAJGLloyds]", ServerAndDbName = @"[dm_Acturis_Lloyds_60]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogPNOBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogPNOBreachesGHIS]", ServerAndDbName = @"[dm_Acturis_GHIS]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogPNOBreaches.sql" });
                    break;
                case TargetSystem.Test:
                case TargetSystem.UAT:
                    // At the moment, Test and UAT are the same. e.g they are both looking at linked server to UAT DW3
                    // CON
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogCONBreachesAJG]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ajg]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogCONBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogCONBreachesInk]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ink]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogCONBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogCONBreachesNABGiles]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_nab]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogCONBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogCONBreachesRBSGiles]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_rbs]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogCONBreaches.sql" });
                    // PNO
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogPNOBreachesAJG]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ajg]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogPNOBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogPNOBreachesInk]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_ink]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogPNOBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogPNOBreachesNABGiles]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_nab]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogPNOBreaches.sql" });
                    storedProcsToIntall.Add(new InstallStoredProc() { SprocName = "[dbo].[LogPNOBreachesRBSGiles]", ServerAndDbName = @"[acturis_nonprod].[acturis_dw_rbs]", ManifestResourceStreamPathSqlString = "VirtualTrainer.SQLScripts.RuleScripts.LogPNOBreaches.sql" });
                    break;
                case TargetSystem.Prod:
                    break;
            }

            // Install the Procs.
            InstallScripts(storedProcsToIntall, context, _assembly);
        }

        #region [ System Set up ]

        enum TargetSystem
        {
            Dev,
            Test,
            UAT,
            Prod
        }
        private void SetUpSystemTables(VirtualTrainer.VirtualTrainerContext context)
        {
            SetUpTitlesTable(context);
            SetUpRolesTable(context);
            SetUpActionsTable(context);
            SetUpProjectTypes(context);
            //SetUpEscalationsFrameworkEmailCollationOptionsTable(context);
            SetUpEscalationsFrameworkScheduleFrequencyOptionsTable(context);
            context.SaveChanges();
        }
        private void SetUpProjectTypes(VirtualTrainer.VirtualTrainerContext context)
        {
            context.ProjectType.AddOrUpdate(new ProjectType() { Id = (int)ProjectTypeEnum.Standard, Name = "Standard", Description = "Standard Project", IsActive = true });
            context.ProjectType.AddOrUpdate(new ProjectType() { Id = (int)ProjectTypeEnum.VirtualTrainer, Name = "Virtual Trainer", Description = "Virtual Trainer", IsActive = true });
            context.ProjectType.AddOrUpdate(new ProjectType() { Id = (int)ProjectTypeEnum.MicroService, Name = "Micro Service", Description = "Micro Service Project", IsActive = true });
            context.SaveChanges();
        }
        private void SetUpTitlesTable(VirtualTrainer.VirtualTrainerContext context)
        {
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Ms, Name = "Ms", Description = "Ms", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Miss, Name = "Miss", Description = "Miss", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Master, Name = "Master", Description = "Master", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Rev, Name = "Rev", Description = "Reverend", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Fr, Name = "Fr", Description = "Father", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Dr, Name = "Dr", Description = "Doctor", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Atty, Name = "Atty", Description = "Attorney", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Prof, Name = "Prof", Description = "Professor", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Hon, Name = "Hon", Description = "Honorable", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Pres, Name = "Pres", Description = "President", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Gov, Name = "Gov", Description = "Governor", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Mr, Name = "Mr", Description = "Mr", IsActive = true });
            context.Title.AddOrUpdate(new Title() { Id = (int)TitleEnum.Mrs, Name = "Mrs", Description = "Mrs", IsActive = true });
            context.SaveChanges();
        }
        private void SetUpEscalationsFrameworkScheduleFrequencyOptionsTable(VirtualTrainer.VirtualTrainerContext context)
        {
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.Always, Name = "Always", Description = "The task will be allowed to run as frequently as required." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.DailyAllDays, Name= "Daily All Days", Description= "The task will run 7 days a week." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.DailyWeekdays, Name = "Daily Weekdays", Description = "The task will run on weekdays only." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.DailyWeekends, Name = "Daily Weekends", Description = "The task will run on weekends only." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyMon, Name = "Weekly Mon", Description = "The task will run on Monday of each week." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyTue, Name = "Weekly Tue", Description = "The task will run on Tuesday of each week." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyWed, Name = "Weekly Wed", Description = "The task will run on Wednesday of each week." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyThur, Name = "Weekly Thur", Description = "The task will run on Thursday of each week." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyFri, Name = "Weekly Fri", Description = "The task will run on Fridsay of each week." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklySat, Name = "Weekly Sat", Description = "The task will run on Saturday of each week." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklySun, Name = "Weekly Sun", Description = "The task will run on Sunday of Each week" });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.MonthlyFirstDay, Name = "Monthly First Day", Description = "The task will run on the first day of the month." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.MonthlyLastDay, Name = "Monthly Last Day", Description = "The task will run on the last day of the month." });
            context.EscaltionsFrameworkScehduleFrequency.AddOrUpdate(new ScheduleFrequency() { Id = (int)EscalationsFrameworkScheduleFrequencyEnum.Never, Name = "Never", Description = "The task will never run." });
            context.SaveChanges();
        }
        private void SetUpActionsTable(VirtualTrainer.VirtualTrainerContext context)
        {
            // Set up Actions table
            context.Actions.AddOrUpdate(new VirtualTrainer.EscalationsFrameworkAction()
            {
                Id = (int)ActionEnum.EmailToHandler,
                RoleId = (int)RoleEnum.ClaimsHandler,
                Name = "Email to Handler",
                Description = "Email being sent to the handler responsible for the context in which the breach has been discovered"
            });
            context.Actions.AddOrUpdate(new VirtualTrainer.EscalationsFrameworkAction()
            {
                Id = (int)ActionEnum.EmailToTeamLead,
                RoleId = (int)RoleEnum.TeamLead,
                Name = "Email to Team Lead",
                Description = "Email being sent to the Team Leader responsible for the context in which the breach has been discovered"
            });
            context.Actions.AddOrUpdate(new VirtualTrainer.EscalationsFrameworkAction()
            {
                Id = (int)ActionEnum.EmailToBranchManager,
                RoleId = (int)RoleEnum.BranchManager,
                Name = "Email to Branch Manager",
                Description = "Email being sent to the Branch manager responsible for the context in which the breach has been found"
            });
            context.Actions.AddOrUpdate(new VirtualTrainer.EscalationsFrameworkAction()
            {
                Id = (int)ActionEnum.EmailToQualityAuditor,
                RoleId = (int)RoleEnum.QualityAuditor,
                Name = "Email to Quality Auditor",
                Description = "Email being sent to the QA responsible for the context in which the breach has been found"
            });
            context.Actions.AddOrUpdate(new VirtualTrainer.EscalationsFrameworkAction()
            {
                Id = (int)ActionEnum.EmailToRegionalManager,
                RoleId = (int)RoleEnum.RegionalManager,
                Name = "Email to Regional Manager",
                Description = "Email being sent to the Regional Manager who is in charge of the context where the breach has been discovered"
            });
            context.SaveChanges();
        }
        private void SetUpRolesTable(VirtualTrainer.VirtualTrainerContext context)
        {
            // Add Roles
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Project Member",
                Id = (int)RoleEnum.ProjectMember,
                Name = "ProjectMember",
                IsSystem = true,
                IsProject = true,
                IsOffice = false,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "System Admin",
                Id = (int)RoleEnum.SystemAdmin,
                Name = "SystemAdmin",
                IsSystem = true,
                IsProject = false,
                IsOffice = false,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Branch Manager",
                Id = (int)RoleEnum.BranchManager,
                Name = "BranchManager",
                IsSystem = false,
                IsProject = false,
                IsOffice = true,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "QA Manager",
                Id = (int)RoleEnum.QualityAuditor,
                Name = "QualityAuditor",
                IsSystem = false,
                IsProject = false,
                IsOffice = true,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Regional Manager",
                Id = (int)RoleEnum.RegionalManager,
                Name = "RegionalManager",
                IsSystem = false,
                IsProject = false,
                IsOffice = true,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Project Admin",
                Id = (int)RoleEnum.ProjectAdmin,
                Name = "ProjectAdmin",
                IsSystem = true,
                IsProject = true,
                IsOffice = false,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Team Lead",
                Id = (int)RoleEnum.TeamLead,
                Name = "TeamLead",
                IsSystem = false,
                IsProject = false,
                IsOffice = false,
                IsTeam = true,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Claims Handler",
                Id = (int)RoleEnum.ClaimsHandler,
                Name = "ClaimsHandler",
                IsSystem = false,
                IsProject = false,
                IsOffice = false,
                IsTeam = true,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Branch Member",
                Id = (int)RoleEnum.BranchMember,
                Name = "Branch Member",
                IsSystem = false,
                IsProject = false,
                IsOffice = true,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Team Member",
                Id = (int)RoleEnum.TeamMember,
                Name = "Team Member",
                IsSystem = false,
                IsProject = false,
                IsOffice = false,
                IsTeam = true,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Role Allows account to call Micro Service web methods.",
                Id = (int)RoleEnum.MicroService,
                Name = "Micro Service",
                IsSystem = false,
                IsProject = true,
                IsOffice = false,
                IsTeam = false,
                IsActive = true
            });
            context.Role.AddOrUpdate(new Role()
            {
                Description = "Provides super user access.",
                Id = (int)RoleEnum.SuperUser,
                Name = "SystemSuperUser",
                IsSystem = true,
                IsProject = false,
                IsOffice = false,
                IsTeam = false,
                IsActive = false
            });
            context.SaveChanges();
        }

        #endregion
    }
}
