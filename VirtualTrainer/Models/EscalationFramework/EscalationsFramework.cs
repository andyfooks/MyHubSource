using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using VirtualTrainer.Utilities;

namespace VirtualTrainer
{
    public class EscalationsFramework
    {
        #region [ EF properties ]

        [Key]
        [ForeignKey("Project")]
        public Guid Id { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastRunDate { get; set; }
        public bool? LastRunSuccess { get; set; }
        public ICollection<EscalationsActionTakenLog> ActionsTaken { get; set; }
        [InverseProperty("EscalationsFramework")]
        public ICollection<EscalationsFrameworkRuleConfig> EscalationsFrameworkRuleConfigurations { get; set; }
        public bool IsDeleted { get; set; }
        #endregion

        public void RunEscelationFramework(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            LoadContextForRunEscelationFramework(ctx);
            EscalationsActionTakenLog executionLogEntry = new EscalationsActionTakenLog(this, ctx);

            try
            {
                if (this.IsActive)
                {
                    if (this.EscalationsFrameworkRuleConfigurations.Any())
                    {
                        foreach (var ruleConfig in this.EscalationsFrameworkRuleConfigurations.ToList())
                        {
                            ruleConfig.RunEscalationConfiguration(ctx, updateTimeStamp, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
                            executionLogEntry.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
                        }
                    }
                    else
                    {
                        executionLogEntry.ExecutionOutcome = EscalationsActionTakenLogOutcome.NoEscalationsRuleConfigurations;
                    }
                }
                else
                {
                    executionLogEntry.ExecutionOutcome = EscalationsActionTakenLogOutcome.EscalationsFrameworkNotActive;
                }

                if (updateTimeStamp)
                {
                    this.LastRunDate = DateTime.Now.Date;
                }
                executionLogEntry.Success = true;
                executionLogEntry.ExecutionOutcome = EscalationsActionTakenLogOutcome.ExecutionSuccess;
            }
            catch (Exception ex)
            {
                // Log Exception
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                // update Rule
                executionLogEntry.ErrorLogEntry = errorLog;
                executionLogEntry.ErrorMessage = ex.Message;
                executionLogEntry.Success = false;
                executionLogEntry.ExecutionOutcome = EscalationsActionTakenLogOutcome.Failure;
            }
            finally
            {
                executionLogEntry.TimeStamp = DateTime.Now;
                executionLogEntry.Finish = DateTime.Now;
                ctx.SaveChanges();
            }
        }
        public void LoadContextForRunEscelationFramework(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
            if (!ctx.Entry(this).Collection("ActionsTaken").IsLoaded)
            {
                ctx.Entry(this).Collection("ActionsTaken").Load();
            }
            if (!ctx.Entry(this).Collection("EscalationsFrameworkRuleConfigurations").IsLoaded)
            {
                ctx.Entry(this).Collection("EscalationsFrameworkRuleConfigurations").Load();
            }
        }
    }
}
