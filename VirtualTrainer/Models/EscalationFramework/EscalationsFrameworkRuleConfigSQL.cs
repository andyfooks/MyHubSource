using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AJG.VirtualTrainer.Helper;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace VirtualTrainer
{
    public class EscalationsFrameworkRuleConfigSQL : EscalationsFrameworkRuleConfig
    {
        #region [ EF properties ]

        public string SqlQuery { get; set; }
        public bool IsStoredProcedure { get; set; }
        [ForeignKey("TargetDb")]
        [Required]
        public int TargetDbID { get; set; }
        public TargetDatabaseDetails TargetDb { get; set; }
        

        #endregion

        #region [ NOT Mapped Properties ]

        [NotMapped]
        public string TargetDbName { get; set; }
        [NotMapped]
        public string BreachInclusionName
        {
            get { return base.BreachInclusion == null ? "" : this.BreachInclusion.ToString(); }
        }

        #endregion

        #region [ Overrides ]

        public override void RunEscalationConfiguration(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            RuleConfigEscalationsActionTakenLog log = new RuleConfigEscalationsActionTakenLog(this, ctx);
            try
            {
                // Load the context objects
                LoadRequiredContextObjects(ctx);

                if (this.Schedule.IsScheduledToRun(ctx, this.LastRunDate))
                {
                    if (this.IsActive)
                    {                        
                        List<BreachLog> breachLogs = GetBreachLogs(ctx, this.BreachInclusion);                       

                        DataTable table = new DataTable();
                        table.Columns.Add(new DataColumn("BreachDisplayAlternateText"));
                        table.Columns.Add(new DataColumn("BreachDisplayText"));
                        table.Columns.Add(new DataColumn("ContextRef"));
                        table.Columns.Add(new DataColumn("RuleConfigurationName"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldOne"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldTwo"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldThree"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldFour"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldFive"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldSix"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldSeven"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldEight"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldNine"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldTen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldEleven"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldTwelve"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldThirteen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldFourteen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldFifteen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldSixteen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldSeventeen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldEighteen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldNineteen"));
                        table.Columns.Add(new DataColumn("RuleBreachFieldTwenty"));

                        foreach (var breach in breachLogs)
                        {
                            table.Rows.Add(
                                string.IsNullOrEmpty(breach.BreachDisplayAlternateText) ? "" : breach.BreachDisplayAlternateText,
                                string.IsNullOrEmpty(breach.BreachDisplayText) ? "" : breach.BreachDisplayText,
                                string.IsNullOrEmpty(breach.ContextRef) ? "" : breach.ContextRef,
                                string.IsNullOrEmpty(breach.RuleConfigurationName) ? "" : breach.RuleConfigurationName,
                                string.IsNullOrEmpty(breach.RuleBreachFieldOne) ? "" : breach.RuleBreachFieldOne,
                                string.IsNullOrEmpty(breach.RuleBreachFieldTwo) ? "" : breach.RuleBreachFieldTwo,
                                string.IsNullOrEmpty(breach.RuleBreachFieldThree) ? "" : breach.RuleBreachFieldThree,
                                string.IsNullOrEmpty(breach.RuleBreachFieldFour) ? "" : breach.RuleBreachFieldFour,
                                string.IsNullOrEmpty(breach.RuleBreachFieldFive) ? "" : breach.RuleBreachFieldFive,
                                string.IsNullOrEmpty(breach.RuleBreachFieldSix) ? "" : breach.RuleBreachFieldSix,
                                string.IsNullOrEmpty(breach.RuleBreachFieldSeven) ? "" : breach.RuleBreachFieldSeven,
                                string.IsNullOrEmpty(breach.RuleBreachFieldEight) ? "" : breach.RuleBreachFieldEight,
                                string.IsNullOrEmpty(breach.RuleBreachFieldNine) ? "" : breach.RuleBreachFieldNine,
                                string.IsNullOrEmpty(breach.RuleBreachFieldTen) ? "" : breach.RuleBreachFieldTen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldEleven) ? "" : breach.RuleBreachFieldEleven,
                                string.IsNullOrEmpty(breach.RuleBreachFieldTwelve) ? "" : breach.RuleBreachFieldTwelve,
                                string.IsNullOrEmpty(breach.RuleBreachFieldThirteen) ? "" : breach.RuleBreachFieldThirteen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldFourteen) ? "" : breach.RuleBreachFieldFourteen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldFifteen) ? "" : breach.RuleBreachFieldFifteen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldSixteen) ? "" : breach.RuleBreachFieldSixteen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldSeventeen) ? "" : breach.RuleBreachFieldSeventeen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldEighteen) ? "" : breach.RuleBreachFieldEighteen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldNineteen) ? "" : breach.RuleBreachFieldNineteen,
                                string.IsNullOrEmpty(breach.RuleBreachFieldTwenty) ? "" : breach.RuleBreachFieldTwenty
                            );
                        }

                        // Only execute if there are any Breaches.
                        if (breachLogs.Any())
                        {
                            using (SqlConnection sqlConnection = new SqlConnection(this.TargetDb.DBConnectionString))
                            {
                                using (SqlCommand cmd = new SqlCommand())
                                {
                                    cmd.CommandText = this.SqlQuery;
                                    cmd.CommandType = this.IsStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                                    cmd.Connection = sqlConnection;
                                    cmd.CommandTimeout = 0;

                                    var breachList = new SqlParameter("@VTBreaches", SqlDbType.Structured);
                                    breachList.TypeName = "VTBreachType";

                                    //System.Collections.Generic.IEnumerable
                                    breachList.Value = table;

                                    cmd.Parameters.Add(breachList);
                                    
                                    sqlConnection.Open();

                                    int rowsaffected = cmd.ExecuteNonQuery();                                                                      
                                }
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
            if (!ctx.Entry(this).Reference("TargetDb").IsLoaded)
            {
                ctx.Entry(this).Reference("TargetDb").Load();
            }

            base.LoadRequiredContextObjects(ctx);
        }

        #endregion

        #region [ Public Methods ]

        public bool ValuesAreValid(out string returnValue)
        {
            returnValue = string.Empty;

            return true;
        }

            #endregion
     }
}
