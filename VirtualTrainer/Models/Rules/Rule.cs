using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class Rule
    {
        #region [EF Properties]

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        //[Key, Column(Order = 0)]
        //public string RuleUniqueName { get; set; }
        public string Description { get; set; }
        public string AdditionalDescription { get; set; }
        //[Key, Column(Order = 1), ForeignKey("Project")]
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }
        [InverseProperty("Rule")]
        public ICollection<RuleConfigurationBase> RuleConfigurations { get; set; }
        //[InverseProperty("Rule")]
        //public ICollection<RuleConfig> RuleConfigs { get; set; }
        //public ICollection<RuleExecutionActionTakenLog> RuleExecutionHistory { get; set; }
        //public ICollection<EscalationsFrameworkRuleConfig> EscalationsFrameWorkRuleConfigurations { get; set; }
        [InverseProperty("Rule")]
        public ICollection<ExclusionsGroupForRule> ExclusionGroups { get; set; }
        public ICollection<RuleExecutionActionTakenLog> ActionsTaken { get; set; }
        [InverseProperty("Rule")]
        public ICollection<BreachLog> BreachLogs { get; set; }
        public bool IsDeleted { get; set; }
        #endregion

        #region [Public Methods]

        public bool RuleHasOutstandingBreachesCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadContextObjects(ctx);

            return this.GetOutstandingBreachesByContextValueCountGtOrEq(ctx, count).Any();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadContextObjects(ctx);
            return this.BreachLogs.Where(d => d.IsArchived != true).OrderBy(a => a.TimeStamp).GroupBy(b => b.ContextRef).Select(c =>
            {
                c.LastOrDefault().BreachLiveContextRefCount = c.Count();
                c.LastOrDefault().FirstBreachDate = c.FirstOrDefault().TimeStamp;
                return c.LastOrDefault();
            }).ToList();
            //return GetOutstandingBreachesByContextValue(ctx).Where(a => a.GetBreachCountForContextRef(ctx) >= count).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValue(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            return this.GetAllOutstandingBreaches(ctx).OrderByDescending(d => d.TimeStamp).GroupBy(a => new
            {
                a.ContextRef
            }).Select(g => g.FirstOrDefault()).ToList();
        }
        public List<BreachLog> GetAllOutstandingBreaches(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            List<BreachLog> breachLogs = new List<BreachLog>();

            foreach (RuleConfigurationBase config in this.RuleConfigurations)
            {
                breachLogs.AddRange(config.GetAllOutstandingBreaches(ctx));
            }
            return breachLogs;
        }
        public List<BreachLog> GetAllBreaches(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            List<BreachLog> breachLogs = new List<BreachLog>();

            foreach (RuleConfigurationBase config in this.RuleConfigurations)
            {
                breachLogs.AddRange(config.GetAllBreaches(ctx));
            }
            return breachLogs;
        }
        public List<BreachLog> GetAllArchivedBreaches(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            List<BreachLog> breachLogs = new List<BreachLog>();

            foreach (RuleConfigurationBase config in this.RuleConfigurations)
            {
                breachLogs.AddRange(config.GetAllArchivedBreaches(ctx));
            }
            return breachLogs;
        }
        public bool UserHasCurrentBreaches(VirtualTrainerContext ctx, int UserId)
        {
            LoadContextObjects(ctx);
            
            foreach (RuleConfiguration config in this.RuleConfigurations)
            {
                if (config.UserHasCurrentBreaches(ctx, UserId))
                {
                    return true;
                }
            }
            return false;
        }
        public List<User> GetAllRuleParticipants(VirtualTrainerContext ctx)
        {
            List<User> users = new List<User>();
            LoadContextObjects(ctx);
            foreach (RuleConfiguration config in this.RuleConfigurations)
            {
                users.AddRange(config.GetParticipants(ctx));
            }
            return users.Distinct().ToList();
        }
        public List<BreachLog> ExecuteAllRuleConfigurations(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            LoadContextObjects(ctx);
            RuleExecutionActionTakenLog log = new RuleExecutionActionTakenLog(this, ctx);
            List<BreachLog> returnBreaches = new List<BreachLog>();
            try
            {
                if (this.IsActive)
                {
                    if (this.RuleConfigurations.Any())// || this.RuleConfigs.Any())
                    {
                        // Perform the main processing of the rule configs.
                        foreach (RuleConfigurationBase ruleConfig in this.RuleConfigurations)
                        {
                            // Perform pre processing breachj log action
                            ruleConfig.ExecutePreExecuteBreachAction(ctx, saveBreachesToDB);

                            returnBreaches.AddRange(ruleConfig.ExecuteRuleConfig(ctx, saveBreachesToDB));
                        }
                        // Now perform post processing for each. Incase any shared resources that need to be tidied up!!.
                        foreach (RuleConfigurationBase ruleConfig in this.RuleConfigurations)
                        {
                            ruleConfig.PostProcessing(ctx, saveBreachesToDB);
                        }
                        log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.RuleCofigurationExecutionSuccess;
                    }
                    else
                    {
                        log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.NoRuleConfigurationsForRule;
                    }
                }
                else
                {
                    log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.RuleNotActive;
                }
                log.Success = true;
            }
            catch (Exception ex)
            {
                // Log Exception
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                //ctx.SaveChanges();
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
                log.Finish = DateTime.Now;
                this.ActionsTaken.Add(log);
                ctx.SaveChanges();
                
            }
            return returnBreaches;
        }

        #endregion

        #region [Priate Methods]

        public void LoadContextObjects(VirtualTrainerContext ctx)
        {
            LoadRuleConfigurationsContextObjects(ctx);
            if (!ctx.Entry(this).Collection("ActionsTaken").IsLoaded)
            {
                ctx.Entry(this).Collection("ActionsTaken").Load();
            }
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }
        }
        public void LoadRuleConfigurationsContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("RuleConfigurations").IsLoaded)
            {
                ctx.Entry(this).Collection("RuleConfigurations").Load();
            }
        }
        #endregion
    }
}
