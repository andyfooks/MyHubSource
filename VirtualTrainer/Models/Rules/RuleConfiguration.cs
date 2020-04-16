using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Z.EntityFramework.Plus;

namespace VirtualTrainer
{
    [Table("RuleConfigurations")]
    public abstract class RuleConfigurationBase : VirtualTrainerBase
    {
        #region [EF Properties]

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("Schedule")]
        [Required]
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        [ForeignKey("Rule")]
        [Required]
        public int RuleId { get; set; }
        public Rule Rule { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        [IsRuleConfigurationParticipant]
        public DateTime? LastRun { get; set; }
        public bool? LastRunSuccess { get; set; }
        public ICollection<BreachLog> BreachLogs { get; set; }
        public ICollection<RuleConfigurationActionTakenLog> ActionsTaken { get; set; }

        // Other
        public bool? SetExistingBreachesToArchivedBeforeExecute { get; set; }
        public BreachActionEnum? PreExecuteBreachesAction { get; set; }
        //public BreachActionEnum? BeforeExecuteBreachAction { get;set; }

        public bool IsDeleted { get; set; }

        #endregion

        #region [ Non EF properties ]

        [NotMapped]
        public string ScheduleName { get; set; }

        [NotMapped]
        public string PreExecuteBreachesActionName
        {
            get
            {
                return this.PreExecuteBreachesAction == null ? "" : this.PreExecuteBreachesAction.ToString();
            }
        }
        #endregion

        public delegate void GetIt(VirtualTrainerContext ctx);

        #region [ Abstract Methods ]

        public abstract List<BreachLog> ExecuteRuleConfig(VirtualTrainerContext ctx, bool saveBreachesToDB);

        /// <summary>
        ///  Could be used for tidying up shared resource e.g. source document that multiple rule configs may be using!!.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="saveBreachesToDB">Determines if we are saving data, typically false during a test mode.</param>
        public abstract void PostProcessing(VirtualTrainerContext ctx, bool saveBreachesToDB);

        #endregion

        #region [ Public Methods ]

        public void ExecutePreExecuteBreachAction(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            if (saveBreachesToDB)
            {
                switch (this.PreExecuteBreachesAction.GetValueOrDefault())
                {
                    case BreachActionEnum.Archive:
                        ctx.BreachLogs.Where(x => x.RuleConfigurationId == this.Id).Update(x => new BreachLog() { IsArchived = true });
                        break;
                    case BreachActionEnum.Delete:
                        //ctx.BreachLogs.Where(x => x.RuleConfigurationId == this.Id).Delete(x => x.BatchSize = 10000);
                        ctx.BreachLogs.RemoveRange(ctx.BreachLogs.Where(x => x.RuleConfigurationId == this.Id));
                        ctx.SaveChanges();
                        break;
                }
                //ctx.SaveChanges();
                //this.LoadContextObjects(ctx);
            }
        }
        public List<BreachLog> GetAllOutstandingBreaches(VirtualTrainerContext ctx)
        {

            LoadContextObjects(ctx);
            // Include filtering by IsActive on project, office, team.  
            return this.BreachLogs.Where(b => b.IsArchived != true).ToList();
        }
        public List<BreachLog> GetAllBreaches(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            // Include filtering by IsActive on project, office, team.  
            return this.BreachLogs.ToList();
        }
        public List<BreachLog> GetAllArchivedBreaches(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            // Include filtering by IsActive on project, office, team.  
            return this.BreachLogs.Where(b => b.IsArchived == true).ToList();
        }
        public void LoadContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
            if (!ctx.Entry(this).Reference("Rule").IsLoaded)
            {
                ctx.Entry(this).Reference("Rule").Load();
            }
            if (!ctx.Entry(this).Reference("Schedule").IsLoaded)
            {
                ctx.Entry(this).Reference("Schedule").Load();
            }
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }
            if (!ctx.Entry(this).Collection("ActionsTaken").IsLoaded)
            {
                ctx.Entry(this).Collection("ActionsTaken").Load();
            }
        }

        #endregion
    }
    public class RuleConfiguration : RuleConfigurationBase
    {
        #region [EF Properties]
   
        public string UserIdentifyingResultsFieldName { get; set; }
        public string UserPropertyName { get; set; }
        [Required]
        public string SqlCommandText { get; set; }
        public bool SqlCommandTextIsStoredProc { get; set; }
        [ForeignKey("SetBreachesToResolvedSchedule")]
        [Required]
        public int SetBreachesToResolvedScheduleId { get; set; }
        public Schedule SetBreachesToResolvedSchedule { get; set; }
        [ForeignKey("TargetDb")]
        [Required]
        public int TargetDbID { get; set; }
        [IsRuleConfigurationParticipant]
        public TargetDatabaseDetails TargetDb { get; set; }
        [Required]
        public RuleConfigExecutionMode UserTargetExecutionMode { get; set; }
        [ForeignKey("Office")]
        public int? OfficeId { get; set; }
        [IsRuleConfigurationParticipant]
        public Office Office { get; set; }
        [ForeignKey("Team")]
        public int? TeamId { get; set; }
        [IsRuleConfigurationParticipant]
        public Team Team { get; set; }
        public RuleTarget RuleTarget { get; set; }
        //public ICollection<RuleExecutionActionTakenLog> RuleExecutionHistory { get; set; }
        [IsRuleConfigurationParticipant]
        [InverseProperty("RuleConfiguration")]
        public ICollection<RuleParticipant> RuleParticipants { get; set; }
        public ICollection<RuleStoredProcedureInputValue> RuleStoredProcedureInputValues { get; set; }
        [InverseProperty("RuleConfiguration")]
        public ICollection<ExclusionsGroupForRuleConfiguration> ExclusionGroups { get; set; }

        #endregion

        #region [ Non EF properties ]
        [NotMapped]
        public string TargetDbName { get; set; }

        [NotMapped]
        public string SetBreachesToResolvedScheduleName { get; set; }

        #endregion

        #region [Public Methods]

        public string GetRuleName(VirtualTrainerContext ctx)
        {
            LoadContextRule(ctx);
            return this.Rule.Name;
        }
        public string GetRuleDescription(VirtualTrainerContext ctx)
        {
            LoadContextRule(ctx);
            return this.Rule.Description;
        }
        public bool RuleConfigurqationHasOutstandingBreachesCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadContextObjects(ctx);
            return this.BreachLogs.Where(b => b.IsArchived != true && b.GetBreachCountForContextRef(ctx) >= count).Any();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadContextObjects(ctx);
            return this.BreachLogs.Where(d => d.IsArchived != true).OrderBy(a => a.TimeStamp).GroupBy(b => b.ContextRef).Select(c => {
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
        //public List<BreachLog> GetAllOutstandingBreaches(VirtualTrainerContext ctx)
        //{
        //    LoadContextObjects(ctx);
        //    // Include filtering by IsActive on project, office, team.  
        //    return this.BreachLogs.Where(b => b.IsArchived != true).ToList();
        //}
        //public List<BreachLog> GetAllBreaches(VirtualTrainerContext ctx)
        //{
        //    LoadContextObjects(ctx);
        //    // Include filtering by IsActive on project, office, team.  
        //    return this.BreachLogs.ToList();
        //}
        //public List<BreachLog> GetAllArchivedBreaches(VirtualTrainerContext ctx)
        //{
        //    LoadContextObjects(ctx);
        //    // Include filtering by IsActive on project, office, team.  
        //    return this.BreachLogs.Where(b => b.IsArchived == true).ToList();
        //}
        public List<Office> GetParticipatingOffices(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            List<Office> offices = new List<Office>();

            foreach (RuleParticipantProject rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantProject)))
            {
                if (rp.IsActive)
                {
                    offices.AddRange(rp.GetOffices(ctx).Where(t=>t.IsActive).ToList());
                }
            }

            foreach (RuleParticipantOffice rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantOffice)))
            {
                Office officeParticipant = rp.GetOffice(ctx);
                if (rp.IsActive)
                {
                    if (officeParticipant.IsActive)
                    {
                        offices.Add(officeParticipant);
                    }
                }
                else
                {
                    offices.Remove(officeParticipant);
                }
            }

            foreach (RuleParticipantTeam rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantTeam)))
            {
                Office officeParticipant = rp.GetOffice(ctx);
                if (rp.IsActive)
                {
                    if (officeParticipant.IsActive)
                    {
                        offices.Add(officeParticipant);
                    }
                }
            }
            foreach (RuleParticipantUser rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantUser)).ToList())
            {
                List<User> userParticipants = rp.GetUsers(ctx);
                if (rp.IsActive)
                {
                    offices.AddRange(userParticipants.Where(u => u.IsActive).ToList().FirstOrDefault().GetMyOffices(ctx));
                }
            }

            return offices;
        }
        public List<Team> GetParticipatingTeams(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            List<Team> teams = new List<Team>();

            foreach (RuleParticipantProject rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantProject)))
            {
                if (rp.IsActive)
                {
                    teams.AddRange(rp.GetTeams(ctx).Where(t=>t.IsActive).ToList());
                }
            }

            foreach (RuleParticipantOffice rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantOffice)))
            {
                List<Team> teamParticipants = rp.GetTeams(ctx);
                if (rp.IsActive)
                {
                    teams.AddRange(teamParticipants.Where(t=>t.IsActive).ToList());
                }
                else
                {
                    foreach (Team t in teamParticipants)
                    {
                        teams.Remove(t);
                    }
                }
            }

            foreach (RuleParticipantTeam rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantTeam)))
            {
                Team teamParticipant = rp.GetTeam(ctx);
                if (rp.IsActive)
                {
                    if (teamParticipant.IsActive)
                    {
                        teams.Add(teamParticipant);
                    }
                }
                else
                {
                    teams.Remove(teamParticipant);
                }
            }
            foreach (RuleParticipantTeam rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantTeam)))
            {
                Team teamParticipant = rp.GetTeam(ctx);
                if (rp.IsActive)
                {
                    if (teamParticipant.IsActive)
                    {
                        teams.Add(teamParticipant);
                    }
                }
                else
                {
                    teams.Remove(teamParticipant);
                }
            }
            foreach (RuleParticipantUser rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantUser)).ToList())
            {
                List<User> userParticipants = rp.GetUsers(ctx);
                if (rp.IsActive)
                {
                    teams.AddRange(userParticipants.Where(u => u.IsActive).ToList().FirstOrDefault().GetMyTeams(ctx));
                }
            }

            return teams;
        }

        public List<User> GetParticipants(VirtualTrainerContext ctx)
        {
            // We want to use the participants to filter structure at a time starting with the largest e.g. Project and moving down to users, adding or removing as necessary.
            LoadContextObjects(ctx);
            List<User> participants = new List<User>();

            foreach (RuleParticipantProject rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantProject)).ToList())
            {
                if(rp.IsActive)
                {
                    participants.AddRange(rp.GetUsers(ctx).Where(u => u.IsActive).ToList());
                }
            }

            foreach (RuleParticipantOffice rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantOffice)).OrderBy(a => a.IsActive).ToList())
            {
                List<User> officeParticipants = rp.GetUsers(ctx);
                if (rp.IsActive)
                {
                    participants.AddRange(officeParticipants.Where(u => u.IsActive).ToList());
                }
                else
                {
                    foreach(User user in officeParticipants)
                    {
                        participants.Remove(user);
                    }
                }
            }
            
            foreach (RuleParticipantTeam rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantTeam)).OrderBy(a => a.IsActive).ToList())
            {
                List<User> teamParticipants = rp.GetUsers(ctx);
                if (rp.IsActive)
                {
                    participants.AddRange(teamParticipants.Where(u=>u.IsActive).ToList());
                }
                else
                {
                    foreach (User user in teamParticipants)
                    {
                        participants.Remove(user);
                    }
                }
            }

            foreach (RuleParticipantUser rp in this.RuleParticipants.Where(r => r.GetType() == typeof(RuleParticipantUser)).ToList())
            {
                List<User> teamParticipants = rp.GetUsers(ctx);
                if (rp.IsActive)
                {
                    participants.AddRange(teamParticipants.Where(u => u.IsActive).ToList());
                }
                else
                {
                    foreach (User user in teamParticipants)
                    {
                        participants.Remove(user);
                    }
                }
            }

            return participants;
        }

        public List<BreachLog> GetCurrentBreachesForUser(VirtualTrainerContext ctx, int userId)
        {
            return ctx.BreachLogs.Where(a => a.IsArchived != true
                && a.UserId == userId 
                && a.RuleConfigurationId == this.Id
                ).ToList();
        }
        public List<BreachLog> GetCurrentUniquBreachesForUser(VirtualTrainerContext ctx, int userId)
        {
            return ctx.BreachLogs.Where(b => b.IsArchived != true && b.RuleConfigurationId == this.Id && b.UserId == userId).OrderByDescending(d => d.TimeStamp).GroupBy(a => new
            {
                a.ContextRef
            }).Select(g => g.FirstOrDefault()).ToList();
        }
        public bool UserHasCurrentBreaches(VirtualTrainerContext ctx, int userId)
        {
            return GetCurrentBreachesForUser(ctx, userId).Any();
        }

        #region [ Overrides ]

        public override void PostProcessing(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
        }
        public override List<BreachLog> ExecuteRuleConfig(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            LoadContextObjects(ctx);
            RuleConfigurationActionTakenLog log = new RuleConfigurationActionTakenLog(this, ctx);
            List<BreachLog> returnBreaches = new List<BreachLog>();
            try
            {
                if (this.IsActive)
                {
                    if (this.Schedule.IsScheduledToRun(ctx, this.LastRun)
                    || this.SetBreachesToResolvedSchedule.IsScheduledToRun(ctx, this.LastRun)
                    || saveBreachesToDB == false)
                    {
                        // check Office is active
                        if (this.Office == null || this.Office.IsActive)
                        {
                            // check that Team is active
                            if (this.Team == null || this.Team.IsActive)
                            {
                                switch (this.RuleTarget)
                                {
                                    case RuleTarget.Office:
                                        throw new NotImplementedException("RuleTarget.Office Not Implemented");
                                    case RuleTarget.Team:
                                        throw new NotImplementedException("RuleTarget.Team Not Implemented");
                                    case RuleTarget.User:
                                        returnBreaches.AddRange(RunRuleForUsers(ctx, saveBreachesToDB));
                                        log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.RuleTargetUserExecutionSuccess;
                                        break;
                                    case RuleTarget.LogAllReturnedBreaches:
                                        returnBreaches.AddRange(RunRuleNonVT(ctx, saveBreachesToDB));
                                        break;
                                }
                            }
                            // Log that Team is not active
                            else
                            {
                                log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.TeamNotActive;
                            }
                        }
                        // Log Office Is not active
                        else
                        {
                            log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.OfficeNotActive;
                        }
                    }
                    else
                    {
                        log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.RuleNotActive;
                    }
                    // Only register execution if saving results to the db. Otherwise time stamp will influence schedule!
                    if (saveBreachesToDB)
                    {
                        this.LastRun = DateTime.Now;
                        this.LastRunSuccess = true;
                    }
                }
                log.Success = true;
            }
            catch (Exception ex)
            {
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                this.LastRunSuccess = false;
                // Update execution log
                log.ErrorLogEntry = errorLog;
                log.ErrorMessage = ex.Message;
                log.Success = false;
                log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Failure;
            }
            finally
            {
                log.TimeStamp = DateTime.Now;
                log.Finish = DateTime.Now;
                this.ActionsTaken.Add(log);
                ctx.SaveChanges();
            }
            return returnBreaches;
        }

        #endregion

        #endregion

        #region [Private Methods]

        private List<BreachLog> RunRuleNonVT(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            LoadContextRuleParticipants(ctx);
            RuleConfigurationActionTakenLog log = new RuleConfigurationActionTakenLog(this, ctx);
            List<BreachLog> returnBreaches = new List<BreachLog>();
            try
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;

                using (SqlConnection sqlConnection = new SqlConnection(this.TargetDb.DBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = this.SqlCommandText;
                        cmd.CommandType = this.SqlCommandTextIsStoredProc ? CommandType.StoredProcedure : CommandType.Text;
                        cmd.Connection = sqlConnection;
                        cmd.CommandTimeout = 0;

                        foreach (RuleStoredProcedureInputValue ruleInput in this.RuleStoredProcedureInputValues)
                        {
                            cmd.Parameters.Add(ruleInput.GetSqlParameterForRuleParticipant(ctx));
                        }
                        sqlConnection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        var sqlRowFieldData = reader.GetSchemaTable().Rows.OfType<DataRow>().Select(g => new SqlRowField()
                                        {
                                            FieldName = g["ColumnName"].ToString(),
                                            Value = reader[g["ColumnName"].ToString()].ToString(),
                                            FieldType = reader[g["ColumnName"].ToString()].GetType()
                                        }).ToList();

                                        string identifyingFieldValue = string.Empty;

                                        // Get the user from the returned results field.
                                        foreach (SqlRowField field in sqlRowFieldData)
                                        {
                                            if (field.FieldName == this.UserIdentifyingResultsFieldName)
                                            {
                                                identifyingFieldValue = field.Value.ToString();
                                                break;
                                            }
                                        }
                                        string contextRefFieldValue = string.Empty;
                                        foreach (SqlRowField field in sqlRowFieldData)
                                        {
                                            if (field.FieldName == "ContextRef")
                                            {
                                                contextRefFieldValue = field.Value.ToString();
                                                break;
                                            }
                                        }

                                        //User ruleUser = null;
                                        BreachLog newLog = new BreachLog(this, ctx);

                                        foreach (SqlRowField field in sqlRowFieldData)
                                        {
                                            // Set the Target Breach Field Value from Results
                                            PropertyInfo prop = newLog.GetType().GetProperty(field.FieldName, BindingFlags.Public | BindingFlags.Instance);
                                            if (null != prop && prop.CanWrite)
                                            {
                                                prop.SetValue(newLog, field.Value, null);
                                            }
                                            // Set the Target Breach Field Value Type from Results
                                            PropertyInfo propType = newLog.GetType().GetProperty(string.Format("{0}Type", field.FieldName), BindingFlags.Public | BindingFlags.Instance);
                                            if (null != propType && propType.CanWrite)
                                            {
                                                propType.SetValue(newLog, field.FieldType.Name, null);
                                            }
                                        }

                                        // Just get the first user in this case as it is a required field.
                                        newLog.User = ctx.User.FirstOrDefault();
                                        newLog.RuleName = this.Rule.Name;
                                        newLog.RuleConfigurationName = this.Name;
                                        //newLog.ActurisInstanceFriendlyName = this.Description;
                                        //newLog.UserName = ruleUser.Name;
                                        newLog.TimeStamp = DateTime.Now;
                                        newLog.RuleID = this.Rule.Id;
                                        newLog.StoredProecdureName = this.SqlCommandText;
                                        newLog.RuleDescription = this.Rule.Description;
                                        newLog.RuleAdditionalDescription = this.Rule.AdditionalDescription;
                                        newLog.RuleConfigurationDescription = this.Description;
                                        newLog.IsArchived = false;

                                        //if (!returnBreaches.Where(rb => rb.ContextRef == newLog.ContextRef).Any())
                                        //{
                                            returnBreaches.Add(newLog);
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        SystemLog errorLog = new SystemLog(ex, this.Project);
                                    }
                                }
                                reader.NextResult();
                            }
                        }

                        // Save the breaches as a batch job here...Much Quicker than during the above loop, only if this is not the test Execute from MVC site!
                        if (saveBreachesToDB)
                        {
                            //this.SetBreachesToResolvedSchedule.IsScheduledToRun(ctx, this.LastRun)
                            if (this.Schedule.IsScheduledToRun(ctx, this.LastRun))
                            {
                                foreach (BreachLog newLog in returnBreaches)
                                {
                                    // Add the log to the DB Table
                                    this.BreachLogs.Add(newLog);
                                }
                            }
                            ctx.SaveChanges();
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                log.ErrorLogEntry = errorLog;
                log.ErrorMessage = ex.Message;
                log.Success = false;
                log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Failure;
            }
            finally
            {
                log.TimeStamp = DateTime.Now;
                log.Finish = DateTime.Now;
                this.ActionsTaken.Add(log);
                ctx.SaveChanges();
                ctx.Configuration.AutoDetectChangesEnabled = true;
                ctx.Configuration.ValidateOnSaveEnabled = true;
            }
            return returnBreaches;
        }

        private List<BreachLog> RunRuleForUsers(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            LoadContextRuleParticipants(ctx);
            RuleConfigurationActionTakenLog log = new RuleConfigurationActionTakenLog(this, ctx);
            List<BreachLog> returnBreaches = new List<BreachLog>();

            try
            {
                if (this.RuleParticipants.Any())
                {
                    if (this.UserTargetExecutionMode == RuleConfigExecutionMode.ExecutePerUser)
                    {
                        foreach (User ruleUser in this.GetParticipants(ctx))
                        {
                            if (ruleUser.IsActive)
                            {
                                returnBreaches.AddRange(RunRuleForEachUser(ruleUser, ctx, saveBreachesToDB));
                                log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Success;
                            }
                            else
                            {
                                log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.RuleUserNotActive;
                            }
                        }
                    }
                    else
                    {
                        returnBreaches.AddRange(RunRuleToGetAllUserBreaches(this.GetParticipants(ctx), ctx, saveBreachesToDB));
                        //returnBreaches.AddRange(RunRuleToGetAllUserBreaches_test1(this.GetParticipants(ctx), ctx, saveBreachesToDB));
                        log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Success;
                    }
                }
                else
                {
                    log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.NoUsersAssignedToUserTargetedRule;
                }
                log.Success = true;
            }
            catch (Exception ex)
            {
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
 
                log.ErrorLogEntry = errorLog;
                log.ErrorMessage = ex.Message;
                log.Success = false;
                log.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Failure;
            }
            finally
            {
                log.TimeStamp = DateTime.Now;
                log.Finish = DateTime.Now;
                this.ActionsTaken.Add(log);
                ctx.SaveChanges();
            }
            return returnBreaches;
        }

        private List<BreachLog> RunRuleToGetAllUserBreaches(List<User> ruleParticipants, VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            //LoadContextForRunRuleForUsers(ctx, ruleUser);
            //RuleConfigurationActionTakenLog executionLogEntry = new RuleConfigurationActionTakenLog(this, ctx);
            //executionLogEntry.RuleUser = ruleUser;
            //executionLogEntry.UserName = ruleUser.User.Name;
            //executionLogEntry.UserId = ruleUser.User.Id;

            // We need to get the existing live ruleconfig breaches for this user. 
            // Then we'll remove them from the list as we process the new ones and the ones left over should be assumed resolved.
            List<BreachLog> liveLogsForThisRuleConfig = this.BreachLogs.ToList();
            List<BreachLog> allReturnedSqlQueryBreaches = new List<BreachLog>();
            List<BreachLog> returnBreaches = new List<BreachLog>();
            List<Office> projectOffices = ctx.Office.Where(o => o.ProjectId == this.ProjectId).ToList();
            List<Team> projectTeams = ctx.Team.Where(o => o.ProjectId == this.ProjectId).ToList();
            List<Office> participatingOffices = GetParticipatingOffices(ctx);
            List<Team> participatingTeams = GetParticipatingTeams(ctx);
            List<Region> ProjectRegions = ctx.Regions.Where(o => o.ProjectId == this.ProjectId).ToList();

            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(this.TargetDb.DBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = this.SqlCommandText;
                        cmd.CommandType = this.SqlCommandTextIsStoredProc ? CommandType.StoredProcedure : CommandType.Text;
                        cmd.Connection = sqlConnection;
                        cmd.CommandTimeout = 0;

                        foreach (RuleStoredProcedureInputValue ruleInput in this.RuleStoredProcedureInputValues)
                        {
                            cmd.Parameters.Add(ruleInput.GetSqlParameterForRuleParticipant(ctx));
                        }
                        sqlConnection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        var sqlRowFieldData = reader.GetSchemaTable().Rows.OfType<DataRow>().Select(g => new SqlRowField()
                                        {
                                            FieldName = g["ColumnName"].ToString(),
                                            Value = reader[g["ColumnName"].ToString()].ToString(),
                                            FieldType = reader[g["ColumnName"].ToString()].GetType()
                                        }).ToList();

                                        string identifyingFieldValue = string.Empty;

                                        // Get the user from the returned results field.
                                        foreach (SqlRowField field in sqlRowFieldData)
                                        {
                                            if (field.FieldName == this.UserIdentifyingResultsFieldName)
                                            {
                                                identifyingFieldValue = field.Value.ToString();
                                                break;
                                            }
                                        }
                                        string contextRefFieldValue = string.Empty;
                                        foreach (SqlRowField field in sqlRowFieldData)
                                        {
                                            if (field.FieldName == "ContextRef")
                                            {
                                                contextRefFieldValue = field.Value.ToString();
                                                break;
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(contextRefFieldValue))
                                        {
                                            allReturnedSqlQueryBreaches.Add(new BreachLog() { ContextRef = contextRefFieldValue });
                                        }

                                        User ruleUser = null;
                                        BreachLog newLog = new BreachLog(this, ctx);

                                        foreach (SqlRowField field in sqlRowFieldData)
                                        {
                                            // Set the Target Breach Field Value from Results
                                            PropertyInfo prop = newLog.GetType().GetProperty(field.FieldName, BindingFlags.Public | BindingFlags.Instance);
                                            if (null != prop && prop.CanWrite)
                                            {
                                                prop.SetValue(newLog, field.Value, null);
                                            }
                                            // Set the Target Breach Field Value Type from Results
                                            PropertyInfo propType = newLog.GetType().GetProperty(string.Format("{0}Type", field.FieldName), BindingFlags.Public | BindingFlags.Instance);
                                            if (null != propType && propType.CanWrite)
                                            {
                                                propType.SetValue(newLog, field.FieldType.Name, null);
                                            }
                                        }

                                        if (this.UserPropertyName.ToLower() == "aliases")
                                        {
                                            ruleUser = ruleParticipants
                                                .Where(p => p.IsActive == true && 
                                                p.Aliases.Where(a => a.Alias == identifyingFieldValue && a.ActurisOrganisationKey == newLog.ActurisOrganisationKey).Any()).FirstOrDefault();
                                        }
                                        else
                                        {
                                            ruleUser = ruleParticipants.Where(p => p.IsActive == true && p.GetValueForSpecifiedProperty(ctx, this.UserPropertyName) == identifyingFieldValue).Select(r => r).FirstOrDefault();
                                        }

                                        if (ruleUser != null)
                                        {
                                            Team team = participatingTeams.Where(t => t.IsActive == true && t.AlsoKnownAs == newLog.TeamKey).FirstOrDefault();
                                            Office office = participatingOffices.Where(t => t.IsActive == true && t.AlsoKnownAs == newLog.OfficeKey).FirstOrDefault();

                                            if ((team != null || participatingTeams.Count() == 0)
                                                && (office != null || participatingOffices.Count() == 0))
                                            {
                                                newLog.User = ruleUser;
                                                newLog.RuleName = this.Rule.Name;
                                                newLog.RuleConfigurationName = this.Name;
                                                newLog.ActurisInstanceFriendlyName = this.Description;
                                                newLog.UserName = ruleUser.Name;
                                                newLog.TimeStamp = DateTime.Now;
                                                newLog.RuleID = this.Rule.Id;
                                                newLog.StoredProecdureName = this.SqlCommandText;
                                                newLog.RuleDescription = this.Rule.Description;
                                                newLog.RuleAdditionalDescription = this.Rule.AdditionalDescription;
                                                newLog.RuleConfigurationDescription = this.Description;
                                                newLog.IsArchived = false;

                                                if (!string.IsNullOrEmpty(newLog.TeamKey))
                                                {
                                                    if (projectTeams.Where(t => t.AlsoKnownAs == newLog.TeamKey).Any())
                                                    {
                                                        newLog.TeamId = projectTeams.Where(t => t.AlsoKnownAs == newLog.TeamKey).FirstOrDefault().Id;
                                                    }
                                                }
                                                if (!string.IsNullOrEmpty(newLog.OfficeKey))
                                                {
                                                    if (projectOffices.Where(t => t.AlsoKnownAs == newLog.OfficeKey).Any())
                                                    {
                                                        Office o = projectOffices.Where(t => t.AlsoKnownAs == newLog.OfficeKey).FirstOrDefault();
                                                        newLog.OfficeId = o.Id;
                                                        // Update the region info if available.
                                                        if (o.RegionId != null)
                                                        {
                                                            Region r = ProjectRegions.Where(a => a.Id == o.RegionId).FirstOrDefault();
                                                            newLog.RegionId = r.Id;
                                                            newLog.RegionName = r.Name;
                                                        }
                                                    }
                                                }
                                                if (!returnBreaches.Where(rb => rb.ContextRef == newLog.ContextRef).Any())
                                                {
                                                    returnBreaches.Add(newLog);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        SystemLog errorLog = new SystemLog(ex, this.Project);
                                    }
                                }
                                reader.NextResult();
                            }
                        }

                        // Save the breaches as a batch job here...Much Quicker than during the above loop, only if this is not the test Execute from MVC site!
                        if (saveBreachesToDB)
                        {
                            //this.SetBreachesToResolvedSchedule.IsScheduledToRun(ctx, this.LastRun)
                            if (this.Schedule.IsScheduledToRun(ctx, this.LastRun))
                            {
                                foreach (BreachLog newLog in returnBreaches)
                                {
                                    // Add the log to the DB Table
                                    this.BreachLogs.Add(newLog);
                                }
                            }
                            foreach (BreachLog newLog in allReturnedSqlQueryBreaches)
                            {
                                // Remove all existing breaches where the context ref has been found again.
                                liveLogsForThisRuleConfig = liveLogsForThisRuleConfig.Where(b => b.ContextRef != newLog.ContextRef).ToList();
                            }
                            // Set all remaining liveLogsForThisRuleConfig to archieved! A breach with the same context has not been found this rule execution so it must be fixed!!
                            foreach (BreachLog log in liveLogsForThisRuleConfig)
                            {
                                log.IsArchived = true;
                                log.ArchivedTimeStamp = DateTime.Now;
                            }
                            // Single save for all breaches.
                            ctx.SaveChanges();
                        }
                    }
                }
                //executionLogEntry.Success = true;
            }
            catch (Exception ex)
            {
                // Empty the live user log list as we wont want to change the status of these in the "finally".
                liveLogsForThisRuleConfig = new List<BreachLog>();

                //// Log Exception
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                //// update Rule 
                //executionLogEntry.ErrorLogEntry = errorLog;
                //executionLogEntry.ErrorMessage = ex.Message;
                //executionLogEntry.Success = false;
                //executionLogEntry.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Failure;
            }
            finally
            {
                //executionLogEntry.TimeStamp = DateTime.Now;
                //this.ActionsTaken.Add(executionLogEntry);
                ctx.SaveChanges();
                ctx.Configuration.AutoDetectChangesEnabled = true;
                ctx.Configuration.ValidateOnSaveEnabled = true;
            }
            return returnBreaches;
        }

        private List<BreachLog> RunRuleForEachUser(User ruleUser, VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            throw new NotImplementedException();

            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;

            List<BreachLog> returnBreaches = new List<BreachLog>();
            List<Office> projectOffices = ctx.Office.Where(o => o.ProjectId == this.ProjectId).ToList();
            List<Team> projectTeams = ctx.Team.Where(o => o.ProjectId == this.ProjectId).ToList();

            ruleUser.LoadRuleBreachLogs(ctx);
            //LoadContextForRunRuleForUsers(ctx, ruleUser);
            RuleConfigurationActionTakenLog executionLogEntry = new RuleConfigurationActionTakenLog(this, ctx);
            executionLogEntry.RuleUser = ruleUser;
            executionLogEntry.UserName = ruleUser.Name;
            executionLogEntry.UserId = ruleUser.Id;

            // We need to get the existing live ruleconfig breaches for this user. 
            // Then we'll remove them from the list as we process the new ones and the ones left over should be assumed resolved.
            List<BreachLog> liveLogsForThisUser = GetLiveBreachLogsForUser(ctx, ruleUser);

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(this.TargetDb.DBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = this.SqlCommandText;
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (RuleStoredProcedureInputValue ruleInput in this.RuleStoredProcedureInputValues)
                        {
                            cmd.Parameters.Add(ruleInput.GetSqlParameterForRuleParticipant(ctx, ruleUser));
                        }

                        cmd.Connection = sqlConnection;
                        sqlConnection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        BreachLog newLog = new BreachLog(this, ctx);
                                        var sqlRowFieldData = reader.GetSchemaTable().Rows.OfType<DataRow>().Select(g => new SqlRowField()
                                        {
                                            FieldName = g["ColumnName"].ToString(),
                                            Value = reader[g["ColumnName"].ToString()].ToString(),
                                            FieldType = reader[g["ColumnName"].ToString()].GetType()
                                        }).ToList();

                                        foreach (SqlRowField field in sqlRowFieldData)
                                        {
                                            // Set the Target Breach Field Value from Results
                                            PropertyInfo prop = newLog.GetType().GetProperty(field.FieldName, BindingFlags.Public | BindingFlags.Instance);
                                            if (null != prop && prop.CanWrite)
                                            {
                                                prop.SetValue(newLog, field.Value, null);
                                            }
                                            // Set the Target Breach Field Value Type from Results
                                            PropertyInfo propType = newLog.GetType().GetProperty(string.Format("{0}Type", field.FieldName), BindingFlags.Public | BindingFlags.Instance);
                                            if (null != propType && propType.CanWrite)
                                            {
                                                propType.SetValue(newLog, field.FieldType.Name, null);
                                            }
                                        }

                                        newLog.User = ruleUser;
                                        newLog.RuleName = this.Rule.Name;
                                        newLog.RuleConfigurationName = this.Name;
                                        newLog.UserName = ruleUser.Name;
                                        newLog.TimeStamp = DateTime.Now;
                                        newLog.RuleID = this.Rule.Id;
                                        newLog.StoredProecdureName = this.SqlCommandText;
                                        newLog.RuleDescription = this.Rule.Description;
                                        newLog.RuleConfigurationDescription = this.Description;
                                        newLog.IsArchived = false;
                                        if (!string.IsNullOrEmpty(newLog.TeamKey))
                                        {
                                            if (projectTeams.Where(t => t.AlsoKnownAs == newLog.TeamKey).Any())
                                            {
                                                newLog.TeamId = projectTeams.Where(t => t.AlsoKnownAs == newLog.TeamKey).FirstOrDefault().Id;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(newLog.OfficeKey))
                                        {
                                            if (projectOffices.Where(t => t.AlsoKnownAs == newLog.OfficeKey).Any())
                                            {
                                                newLog.OfficeId = projectOffices.Where(t => t.AlsoKnownAs == newLog.OfficeKey).FirstOrDefault().Id;
                                            }
                                        }

                                        returnBreaches.Add(newLog);

                                        if (saveBreachesToDB)
                                        {
                                            this.BreachLogs.Add(newLog);
                                            ctx.SaveChanges();
                                        }

                                        // Remove all existing breaches for this user where the context ref has been found again.
                                        liveLogsForThisUser = liveLogsForThisUser.Where(b => b.ContextRef != newLog.ContextRef).ToList();
                                    }
                                    catch (Exception ex)
                                    {
                                        SystemLog errorLog = new SystemLog(ex, this.Project);
                                    }
                                }
                                reader.NextResult();
                            }
                        }
                    }
                }
                executionLogEntry.Success = true;
            }
            catch (Exception ex)
            {
                // Empty the live user log list as we wont want to change the status of these in the "finally".
                liveLogsForThisUser = new List<BreachLog>();

                // Log Exception
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                // update Rule 
                executionLogEntry.ErrorLogEntry = errorLog;
                executionLogEntry.ErrorMessage = ex.Message;
                executionLogEntry.Success = false;
                executionLogEntry.ExecutionOutcome = RuleExecutionHistoryLogOutcome.Failure;
            }
            finally
            {
                executionLogEntry.TimeStamp = DateTime.Now;
                executionLogEntry.Finish = DateTime.Now;
                this.ActionsTaken.Add(executionLogEntry);

                // Set all and every remaining liveLogsForThisUser to archieved! A breach with the same context has not been found this rule execution so it must be fixed!!
                foreach (BreachLog log in liveLogsForThisUser)
                {
                    log.IsArchived = true;
                    log.ArchivedTimeStamp = DateTime.Now;
                }

                ctx.SaveChanges();
                ctx.Configuration.AutoDetectChangesEnabled = true;
                ctx.Configuration.ValidateOnSaveEnabled = true;
            }
            return returnBreaches;
        }
        private List<BreachLog> GetLiveBreachLogsForUser(VirtualTrainerContext ctx, User user)
        {
           return BreachLogs.Where(b => b.UserId == user.Id && b.IsArchived != true).ToList();
        }
        private void LoadContextRule(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Rule").IsLoaded)
            {
                ctx.Entry(this).Reference("Rule").Load();
            }
        }
        public void LoadContextRuleParticipants(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("RuleParticipants").IsLoaded)
            {
                ctx.Entry(this).Collection("RuleParticipants").Load();
            }
        }
        private void LoadContextForRunRuleForUsers(VirtualTrainerContext ctx, RuleParticipantUser ruleUser)
        {
            if (!ctx.Entry(ruleUser).Reference("User").IsLoaded)
            {
                ctx.Entry(ruleUser).Reference("User").Load();
            }
            if (!ctx.Entry(ruleUser).Reference("Alias").IsLoaded)
            {
                ctx.Entry(ruleUser).Reference("Alias").Load();
            }
            if (!ctx.Entry(ruleUser.User).Collection("RuleBreachLogs").IsLoaded)
            {
                ctx.Entry(ruleUser.User).Collection("RuleBreachLogs").Load();
            }
        }

        public void LoadContextObjects(VirtualTrainerContext ctx)
        {
            LoadContextRuleParticipants(ctx);
            LoadContextRule(ctx);
            if (!ctx.Entry(this).Reference("Office").IsLoaded)
            {
                ctx.Entry(this).Reference("Office").Load();
            }
            if (!ctx.Entry(this).Reference("TargetDb").IsLoaded)
            {
                ctx.Entry(this).Reference("TargetDb").Load();
            }
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
            if (!ctx.Entry(this).Reference("Team").IsLoaded)
            {
                ctx.Entry(this).Reference("Team").Load();
            }
            if(!ctx.Entry(this).Reference("Schedule").IsLoaded)
            {
                ctx.Entry(this).Reference("Schedule").Load();
            }
            if (!ctx.Entry(this).Reference("SetBreachesToResolvedSchedule").IsLoaded)
            {
                ctx.Entry(this).Reference("SetBreachesToResolvedSchedule").Load();
            }
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }
            if (!ctx.Entry(this).Collection("RuleStoredProcedureInputValues").IsLoaded)
            {
                ctx.Entry(this).Collection("RuleStoredProcedureInputValues").Load();
            }
            if (!ctx.Entry(this).Collection("ActionsTaken").IsLoaded)
            {
                ctx.Entry(this).Collection("ActionsTaken").Load();
            }
        }
        private class SqlRowField
        {
            public string FieldName { get; set; }
            public string Value { get; set; }
            public Type FieldType { get; set; }
        }

        #endregion
    }
}
