using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using VirtualTrainer.Models.MyHub;

namespace VirtualTrainer
{
    public class VirtualTrainerContext : DbContext , IDisposable
    {
        public VirtualTrainerContext() : base("name=VirtualTrainer")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.ValidateOnSaveEnabled = true;
        }
        public void Dispose()
        {
            base.Dispose();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer<VirtualTrainerContext>(null);
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Title>().HasRequired(a => a.Project).WithMany(a => a.Titles).WillCascadeOnDelete(false);
            modelBuilder.Entity<Team>().HasRequired(a => a.Project).WithMany(a => a.Teams).WillCascadeOnDelete(false);
            //modelBuilder.Entity<User>().HasRequired(a => a.Project).WithMany(a => a.Users).WillCascadeOnDelete(false);
            modelBuilder.Entity<Team>().HasRequired(a => a.Office).WithMany(a => a.Teams).WillCascadeOnDelete(false);
            modelBuilder.Entity<Rule>().HasRequired(a => a.Project).WithMany(a => a.Rules).WillCascadeOnDelete(false);
            modelBuilder.Entity<TargetDatabaseDetails>().HasRequired(a => a.Project).WithMany(a => a.TargetDatabases).WillCascadeOnDelete(false);
            //modelBuilder.Entity<Role>().HasRequired(a => a.Project).WithMany(a => a.Roles).WillCascadeOnDelete(false);
            modelBuilder.Entity<Office>().HasRequired(a => a.Project).WithMany(a => a.Offices).WillCascadeOnDelete(false);
            modelBuilder.Entity<BreachLog>().HasRequired(a => a.RuleConfiguration).WithMany(a => a.BreachLogs).WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleStoredProcedureInputValue>().HasRequired(a => a.RuleConfiguration).WithMany(a => a.RuleStoredProcedureInputValues).WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleConfigurationBase>().HasRequired(a => a.Rule).WithMany(a => a.RuleConfigurations).WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleParticipant>().HasRequired(a => a.RuleConfiguration).WithMany(a => a.RuleParticipants).WillCascadeOnDelete(false);
            //modelBuilder.Entity<EscalationsFrameworkRuleConfig>().HasRequired(c => c.ScheduleFrequency).WithMany().WillCascadeOnDelete(false);
            //--> modelBuilder.Entity<EscalationsFrameworkRuleConfigEmailPerRuleConfig>().HasRequired(c => c.RuleConfiguration).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ExclusionsGroupForRule>().HasRequired(c => c.Rule).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ExclusionsGroupForRuleConfiguration>().HasRequired(c => c.RuleConfiguration).WithMany().WillCascadeOnDelete(false);
            //modelBuilder.Entity<ExclusionsItem>().HasRequired(c => c.Project).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleStoredProcedureInputValue>().HasRequired(a => a.RuleConfiguration).WithMany(a=>a.RuleStoredProcedureInputValues).WillCascadeOnDelete(false);
            modelBuilder.Entity<ExclusionsGroup>().HasRequired(a => a.Project).WithMany(a => a.ExclusionGroupsAll).WillCascadeOnDelete(false);
            modelBuilder.Entity<EscalationsFrameworkRuleConfig>().HasRequired(a => a.Schedule).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleConfiguration>().HasRequired(a => a.Schedule).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleConfiguration>().HasRequired(a => a.SetBreachesToResolvedSchedule).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ActurisBusinessStructureSyncConfig>().HasRequired(a => a.Schedule).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<EscalationsFrameworkBreachSource>().HasRequired(a => a.Project).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<EscalationsFrameworkBreachSourceRuleConfiguration>().HasRequired(a => a.RuleConfiguration).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<EscalationsFrameworkBreachSourceUser>().HasRequired(a => a.User).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ExclusionsItem>().HasRequired(a=>a.ExclusionsGroup).WithMany(b=>b.ExclusionItems).WillCascadeOnDelete(false);
            modelBuilder.Entity<RuleStoredProcedureInputValueExclusionsGroup>().HasRequired(a => a.ExclusionsGroup).WithMany().WillCascadeOnDelete(false);
           // modelBuilder.Entity<RuleConfig>().HasRequired(a => a.Schedule).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ExchangeEmailRuleConfigBreachFieldMappings>().HasRequired(a => a.Project).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ExchangeEmailRuleConfig>().HasRequired(a => a.Project).WithMany().WillCascadeOnDelete(false);
            //modelBuilder.Entity<ExcelRuleConfigBreachFieldMapping>().HasRequired(a => a.Project).WithMany().WillCascadeOnDelete(false);
            //modelBuilder.Entity<ExcelRuleConfig>().HasRequired(a => a.Project).WithMany().WillCascadeOnDelete(false);
        }

        public DbSet<SystemConfig> SystemConfig { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectType> ProjectType { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserAlias> UserAlias { get; set; }
        public DbSet<Title> Title { get; set; }
        public DbSet<Office> Office { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<TargetDatabaseDetails> TargetDatabaseDetails { get; set; }
        //public DbSet<Permission> Permissions { get; set; }
        public DbSet<SystemPermission> SystemPermissions { get; set; }
        public DbSet<ProjectPermission> ProjectPermissions { get; set; }
        public DbSet<OfficePermission> OfficePermissions { get; set; }
        public DbSet<TeamPermission> TeamPermissions { get; set; }
       
        public DbSet<RuleConfiguration> RuleConfigurations { get; set; }
        public DbSet<RuleConfigurationBase> RuleConfigurationBase { get; set; }
       // public DbSet<RuleConfig> RuleConfigs { get; set; }
        public DbSet<ExchangeEmailRuleConfig> RuleConfigsExchange { get; set; }
        public DbSet<ExchangeEmailRuleConfigBreachFieldMappings> RuleConfigsExchangeBreachFieldMappings { get; set; }
        public DbSet<ExcelRuleConfig> RuleConfigsExcel { get; set; }
        public DbSet<ExcelRuleConfigBreachFieldMapping> ExcelRuleConfigBreachFieldMappings { get; set; }

        public DbSet<Role> Role { get; set; }
        public DbSet<RuleStoredProcedureInputValue> RuleStoredProcedureInputValues { get; set; }
        public DbSet<RuleStoredProcedureInputValueHardCoded> RuleStoredProcedureInputValuesHardCoded { get; set; }
        public DbSet<RuleStoredProcedureInputValueClassReference> RuleStoredProcedureInputValuesClassReference { get; set; }
        public DbSet<RuleStoredProcedureInputValueExclusionsGroup> RuleStoredProcedureInputValuesExclusionsGroup { get; set; }
        public DbSet<RuleParticipant> RuleParticipants { get; set; }
        public DbSet<RuleParticipantUser> RuleParticipantUsers { get; set; }
        public DbSet<RuleParticipantTeam> RuleParticipantTeams { get; set; }
        public DbSet<RuleParticipantOffice> RuleParticipantOffices { get; set; }
        public DbSet<RuleParticipantProject> RuleParticipantProjects { get; set; }
        public DbSet<EscalationsFrameworkAction> Actions { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<EscalationsFrameworkRuleConfig> EscalationsFrameWorkRuleConfigurations { get; set; }
        public DbSet<EscalationsFrameworkRuleConfigEmail> EscalationsFrameWorkEmailRuleConfigurations { get; set; }
        public DbSet<EscalationsFrameworkRuleConfigSQL> EscalationsFrameWorkSQLRuleConfigurations { get; set; }

        // LOG DBSets
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<BreachLog> BreachLogs { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }

        public DbSet<ActionTakenLog> ActionTakenLogs { get; set; }
        public DbSet<RuleExecutionActionTakenLog> RuleExecutionActionTakenLogs { get; set; }
        public DbSet<EscalationsActionTakenLog> EscalationsActionTakenLogs { get; set; }
        public DbSet<ProjectEscalationsActionTakenLog> ProjectEscalationsActionTakenLogs { get; set; }
        public DbSet<RuleConfigEscalationsActionTakenLog> RuleConfigEscalationsActionTakenLogs { get; set; }
        public DbSet<EmailRuleConfigEscalationsActionTakenLog> EmailRuleConfigEscalationsActionTakenLogs { get; set; }

        public DbSet<EscalationsFramework> EscalationsFrameWork { get; set; }
        public DbSet<ScheduleFrequency> EscaltionsFrameworkScehduleFrequency { get; set; }
        public DbSet<ExclusionsGroup> ExclusionsGroups { get; set; }
        //public DbSet<ExclusionsGroupForProject> ExclusionsGroupForProjects { get; set; }
        public DbSet<ExclusionsGroupForRule> ExclusionsGroupForRules { get; set; }
        public DbSet<ExclusionsGroupForRuleConfiguration> ExclusionsGroupForRuleConfigurations { get; set; }
        public DbSet<EscalationsFrameworkRuleConfigEmailUser> EscalationsFrameworkRuleConfigEmailUsers { get; set; }
        public DbSet<EscalationsFrameworkRuleConfigEmailRole> EscalationsFrameworkRuleConfigEmailRoles { get; set; }
        public DbSet<ExclusionsItem> ExclusionsItem { get; set; }   

        public DbSet<EscalationsFrameworkBreachSource> EscalationsFrameworkBreachSources { get; set; }
        public DbSet<EscalationsFrameworkBreachSourceUser> EscalationsFrameworkBreachSourcesUser { get; set; }
        public DbSet<EscalationsFrameworkBreachSourceTeam> EscalationsFrameworkBreachSourcesTeam { get; set; }
        public DbSet<EscalationsFrameworkBreachSourceOffice> EscalationsFrameworkBreachSourcesOffice { get; set; }
        public DbSet<EscalationsFrameworkBreachSourceRule> EscalationsFrameworkBreachSourcesRule { get; set; }
        public DbSet<EscalationsFrameworkBreachSourceRuleConfiguration> EscalationsFrameworkBreachSourcesRuleConfiguration { get; set; }
        public DbSet<EscalationsEmailRecipient> EscalationsEmailRecipients { get; set; }

        public DbSet<ActurisBusinessStructureSyncConfig> ActurisBusinessStructureSyncConfigs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<ExchangeAccountDetails> ExchangeAccountDetails { get; set; }

        #region [ My Hub ]

        public DbSet<PrinterSummaryActivity> PrinterSummaryActivity { get; set; }
        public DbSet<PhoneSummaryActivity> PhoneSummaryActivity { get; set; }
        public DbSet<TimeSheetData> TimeSheetData { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<WorkSheetActivity> WorkSheetMonthActivity { get; set; }

        #endregion
    }
}
