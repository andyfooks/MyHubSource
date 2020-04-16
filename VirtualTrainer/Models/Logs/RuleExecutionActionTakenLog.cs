using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class ProjectExecuteRulesActionTakenLog : ActionTakenLog
    {
        #region [ EF Properties ]

        public RuleExecutionHistoryLogOutcome ExecutionOutcome { get; set; }

        #endregion

        #region [ Constructors ]

        public ProjectExecuteRulesActionTakenLog() { }
        public ProjectExecuteRulesActionTakenLog(Project project, VirtualTrainerContext ctx)
        {
            this.Project = project;
            this.ProjectId = project.ProjectUniqueKey;
            this.ProjectName = project.ProjectName;
            this.ProjectDisplayName = project.ProjectDisplayName;
            this.ProjectDescription = project.ProjectDescription;

            this.TimeStamp = DateTime.Now;
            this.Start = DateTime.Now;
        }

        #endregion
    }
    public class RuleConfigurationActionTakenLog : RuleExecutionActionTakenLog
    {
        #region [ EF Properties ]  

        public int? RuleConfigurationId { get; set; }
        public string RuleConfigurationName { get; set; }
        public string RuleConfigurationDescription { get; set; }
        public RuleConfiguration RuleConfiguration { get; set; }
        public int? OfficeID { get; set; }
        public string OfficeName { get; set; }
        public int? TeamID { get; set; }
        public string TeamName { get; set; }
        public int? DbId { get; set; }
        public string DBName { get; set; }
        public User RuleUser { get; set; }

        #endregion

        #region [ Constructors ]

        public RuleConfigurationActionTakenLog() { }

        /// <summary>
        /// Pass in RuleConfig to auto populate this instance property values
        /// </summary>
        /// <param name="ruleConfig"></param>
        /// <param name="ctx"></param>
        public RuleConfigurationActionTakenLog(RuleConfiguration ruleConfig, VirtualTrainerContext ctx)
        {
            ruleConfig.LoadContextObjects(ctx);

            this.RuleConfiguration = ruleConfig;
            this.RuleConfigurationDescription = ruleConfig.Description;
            this.RuleConfigurationName = ruleConfig.Name;


            this.Rule = ruleConfig.Rule;
            this.RuleName = ruleConfig.Rule.Name;
            this.RuleDescription = ruleConfig.Rule.Description;
            this.Rule = ruleConfig.Rule;

            this.DbId = ruleConfig.TargetDbID;
            this.DBName = ruleConfig.TargetDb.DBName;

            this.OfficeID = ruleConfig.OfficeId;
            this.OfficeName = ruleConfig.Office == null ? "" : ruleConfig.Office.Name;

            this.TeamID = ruleConfig.TeamId;
            this.TeamName = ruleConfig.Team == null ? "" : ruleConfig.Team.Name;

            this.Project = ruleConfig.Rule.Project;
            this.ProjectId = ruleConfig.Rule.Project.ProjectUniqueKey;
            this.ProjectName = ruleConfig.Rule.Project.ProjectName;
            this.ProjectDisplayName = ruleConfig.Rule.Project.ProjectDisplayName;
            this.ProjectDescription = ruleConfig.Rule.Project.ProjectDescription;

            this.TimeStamp = DateTime.Now;
            this.Start = DateTime.Now;
        }

        #endregion
    }

    public class RuleExecutionActionTakenLog : ProjectExecuteRulesActionTakenLog
    {
        #region [ EF Properties ]  

        public string RuleName { get; set; }
        public string RuleDescription { get; set; }
        public Rule Rule { get; set; }

        #endregion

        #region [ Constructors ]

        public RuleExecutionActionTakenLog() { }

        /// <summary>
        /// Pass in Rule to auto populate this instance property values
        /// </summary>
        /// <param name="ruleConfig"></param>
        /// <param name="ctx"></param>
        public RuleExecutionActionTakenLog(Rule rule, VirtualTrainerContext ctx)
        {
            rule.LoadContextObjects(ctx);

            this.Project = rule.Project;
            this.ProjectId = rule.Project.ProjectUniqueKey;
            this.ProjectName = rule.Project.ProjectName;
            this.ProjectDisplayName = rule.Project.ProjectDisplayName;
            this.ProjectDescription = rule.Project.ProjectDescription;

            this.RuleName = rule.Name;
            this.RuleDescription = rule.Description;
            this.Rule = rule;

            this.TimeStamp = DateTime.Now;
            this.Start = DateTime.Now;
        }

        #endregion
    }
}
