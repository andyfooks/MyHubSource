using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ProjectEscalationsActionTakenLog : ActionTakenLog
    {
        public EscalationsActionTakenLogOutcome ExecutionOutcome { get; set; }

        #region [ Constructors ]

        public ProjectEscalationsActionTakenLog() { }

        public ProjectEscalationsActionTakenLog(Project project, VirtualTrainerContext ctx)
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
    public class EscalationsActionTakenLog : ProjectEscalationsActionTakenLog
    {
        public Guid? EscalationsFrameworkId { get; set; }
        public EscalationsFramework EscalationsFramework { get; set; }

        #region [ Constructors ]

        public EscalationsActionTakenLog() { }

        public EscalationsActionTakenLog(EscalationsFramework ef, VirtualTrainerContext ctx)
        {
            ef.LoadContextForRunEscelationFramework(ctx);

            this.Project = ef.Project;
            this.ProjectId = ef.Project.ProjectUniqueKey;
            this.ProjectName = ef.Project.ProjectName;
            this.ProjectDisplayName = ef.Project.ProjectDisplayName;
            this.ProjectDescription = ef.Project.ProjectDescription;

            this.EscalationsFrameworkId = ef.Id;
            this.EscalationsFramework = ef;

            this.Start = DateTime.Now;
            this.TimeStamp = DateTime.Now;
        }

        #endregion
    }
    public class RuleConfigEscalationsActionTakenLog : EscalationsActionTakenLog
    {
        public int? EscalationsFrameworkRuleConfigId { get; set; }
        public string EscalationsFrameworkRuleConfigName { get; set; }
        public string EscalationsFrameworkRuleConfigDescription { get; set; }
        public string EscalationsRuleConfigActionName { get; set; }
        public string EscalationsRuleConfigActionDescription { get; set; }
        public EscalationsFrameworkRuleConfig EscalationsFrameWorkRuleConfiguration { get; set; }

        #region [ Constructors ]

        public RuleConfigEscalationsActionTakenLog() { }

        public RuleConfigEscalationsActionTakenLog(EscalationsFrameworkRuleConfig ef, VirtualTrainerContext ctx)
        {
            ef.LoadRequiredContextObjects(ctx);

            this.Project = ef.Project;
            this.ProjectId = ef.Project.ProjectUniqueKey;
            this.ProjectName = ef.Project.ProjectName;
            this.ProjectDisplayName = ef.Project.ProjectDisplayName;
            this.ProjectDescription = ef.Project.ProjectDescription;

            this.EscalationsFrameworkId = ef.EscalationsFrameworkId;
            this.EscalationsFramework = ef.EscalationsFramework;

            this.EscalationsFrameworkRuleConfigDescription = ef.Description;
            this.EscalationsFrameworkRuleConfigId = ef.Id;
            this.EscalationsFrameworkRuleConfigName = ef.Name;
            this.EscalationsFrameWorkRuleConfiguration = ef;

            this.Start = DateTime.Now;
            this.TimeStamp = DateTime.Now;
        }

        #endregion
    }
    public class EmailRuleConfigEscalationsActionTakenLog : RuleConfigEscalationsActionTakenLog
    {
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        [NotMapped]
        public DateTime TimeStampDateOnly
        {
            get { return this.TimeStamp.Date; }
        }

        public EmailRuleConfigEscalationsActionTakenLog() { }
        /// <summary>
        /// Pass an EscalationsFrameworkRuleConfigEmailRole object to auto populate fields.
        /// </summary>
        /// <param name="ef"></param>
        /// <param name="ctx"></param>
        public EmailRuleConfigEscalationsActionTakenLog(EscalationsFrameworkRuleConfigEmailRole ef, VirtualTrainerContext ctx)
        {
            ef.LoadRequiredContextObjects(ctx);
            this.Project = ef.Project;
            this.ProjectId = ef.Project.ProjectUniqueKey;
            this.ProjectName = ef.Project.ProjectName;
            this.ProjectDisplayName = ef.Project.ProjectDisplayName;
            this.ProjectDescription = ef.Project.ProjectDescription;

            this.EscalationsFrameworkId = ef.EscalationsFrameworkId;
            this.EscalationsFramework = ef.EscalationsFramework;

            this.EscalationsFrameworkRuleConfigDescription = ef.Description;
            this.EscalationsFrameworkRuleConfigId = ef.Id;
            this.EscalationsFrameworkRuleConfigName = ef.Name;
            this.EscalationsFrameWorkRuleConfiguration = ef;

            this.Start = DateTime.Now;
            this.TimeStamp = DateTime.Now;
        }
        /// <summary>
        /// Pass an EscalationsFrameworkRuleConfigEmailUser object to auto populate fields.
        /// </summary>
        /// <param name="ef"></param>
        /// <param name="ctx"></param>
        public EmailRuleConfigEscalationsActionTakenLog(EscalationsFrameworkRuleConfigEmailUser ef, VirtualTrainerContext ctx)
        {
            ef.LoadRequiredContextObjects(ctx);
            this.Project = ef.Project;
            this.ProjectId = ef.Project.ProjectUniqueKey;
            this.ProjectName = ef.Project.ProjectName;
            this.ProjectDisplayName = ef.Project.ProjectDisplayName;
            this.ProjectDescription = ef.Project.ProjectDescription;

            this.EscalationsFrameworkId = ef.EscalationsFrameworkId;
            this.EscalationsFramework = ef.EscalationsFramework;

            this.EscalationsFrameworkRuleConfigDescription = ef.Description;
            this.EscalationsFrameworkRuleConfigId = ef.Id;
            this.EscalationsFrameworkRuleConfigName = ef.Name;
            this.EscalationsFrameWorkRuleConfiguration = ef;

            this.Start = DateTime.Now;
            this.TimeStamp = DateTime.Now;
        }
        public void PopulatEmailAndUserDetails(VirtualTrainerContext ctx, User user, string emailBody, string emailSubject, string emailFrom)
        {
            this.EmailBody = emailBody;
            this.EmailSubject = emailSubject;
            this.EmailTo = user.Email;
            this.EmailFrom = emailFrom;
            this.User = user;
            this.UserId = user.Id;
            this.UserName = user.Name;

            this.Start = DateTime.Now;
            this.TimeStamp = DateTime.Now;
        }
    }
}
