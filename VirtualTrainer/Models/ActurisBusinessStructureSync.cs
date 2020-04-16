using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace VirtualTrainer
{
    public class ActurisBusinessStructureSyncConfig
    {
        #region [ EF Properties ]

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string SqlCommandText { get; set; }
        public bool SqlCommandTextIsStoredProc { get; set; }
        [ForeignKey("TargetDatabaseDetails")]
        [Required]
        public int TargetDatabaseDetailsId { get; set; }
        public TargetDatabaseDetails TargetDatabaseDetails { get; set; }
        [ForeignKey("Schedule")]
        [Required]
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastRun { get; set; }
        public bool? LastRunSuccess { get; set; }
        //public bool UpdateUserEmails { get; set; }

        #endregion
        
        #region [ Not Mapped EF properties ]

        [NotMapped]
        public string TargetDbName { get; set; }
        [NotMapped]
        public string ScheduleName { get; set; }

        #endregion

        #region [ Public Methods ]

        private void AddOffices(VirtualTrainerContext ctx, List<UserBusinessStructureInfo> strctureFromActuris)
        {
            // Add all the offices
            foreach (var uniqueOffice in strctureFromActuris.GroupBy(g => g.OfficeKey))
            {
                UserBusinessStructureInfo ubsOffice = uniqueOffice.FirstOrDefault();

                Office office = ctx.Office.Where(o => o.ProjectId == this.ProjectId && o.AlsoKnownAs == ubsOffice.OfficeKey).FirstOrDefault();
                if (office == null)
                {
                    office = new Office()
                    {
                        ActurisOrganisationKey = ubsOffice.OrganisationKey,
                        ActurisOrganisationName = ubsOffice.OrganisationName,
                        Name = ubsOffice.OfficeName,
                        AlsoKnownAs = ubsOffice.OfficeKey,
                        IsActive = true,
                        ProjectId = this.ProjectId
                    };
                    ctx.Office.Add(office);
                }
            }
            // save all the offices
            ctx.SaveChanges();
        }

        private void AddTeams(VirtualTrainerContext ctx, List<UserBusinessStructureInfo> structureFromActuris)
        {
            List<Office> vtOffices = ctx.Office.Where(o => o.ProjectId == this.ProjectId).ToList();

            // Add all the Teams
            foreach (var uniqueOfficeGroup in structureFromActuris.GroupBy(g => g.OfficeKey))
            {
                // Get The office
                UserBusinessStructureInfo ubsOffice = uniqueOfficeGroup.FirstOrDefault();
                Office office = vtOffices.Where(o => o.AlsoKnownAs == ubsOffice.OfficeKey).FirstOrDefault();

                // Now create all teams for this office.
                foreach (var uniqueTeamGroup in uniqueOfficeGroup.GroupBy(g => g.TeamKey))
                {
                    UserBusinessStructureInfo ubsTeam = uniqueTeamGroup.FirstOrDefault();

                    Team team = ctx.Team.Where(t => t.ProjectId == this.ProjectId && t.AlsoKnownAs == ubsTeam.TeamKey).FirstOrDefault();
                    if (team == null)
                    {
                        team = new Team()
                        {
                            ActurisOrganisationKey = ubsTeam.OrganisationKey,
                            ActurisOrganisationName = ubsTeam.OrganisationName,
                            Name = ubsTeam.TeamName,
                            AlsoKnownAs = ubsTeam.TeamKey,
                            IsActive = true,
                            ProjectId = this.ProjectId,
                            OfficeId = office.Id
                        };

                        ctx.Team.Add(team);
                    }
                }
            }
            // Save all the teams.
            ctx.SaveChanges();
        }
        private void AddAliasesForUser(VirtualTrainerContext ctx, User user, IGrouping<string, UserBusinessStructureInfo> ActurisUserRows, List<UserAlias> userAliases)
        {
            if (user != null)
            {
                foreach (UserBusinessStructureInfo userRow in ActurisUserRows)
                {
                    UserAlias alias = userAliases.Where(a => !string.IsNullOrEmpty(a.ActurisOrganisationKey) && a.ActurisOrganisationKey == userRow.OrganisationKey &&
                                                            !string.IsNullOrEmpty(a.Alias) && a.Alias == userRow.UserKey && 
                                                            a.UserId == user.Id).FirstOrDefault();
                    if (alias == null)
                    {
                        alias = new UserAlias()
                        {
                            ActurisOrganisationKey = userRow.OrganisationKey,
                            ActurisOrganisationName = userRow.OrganisationName,
                            UserId = user.Id,
                            IsActive = userRow.UserStatusKey == "1" ? true : false,
                            Alias = userRow.UserKey,
                            AliasDescription = userRow.UserRole
                        };
                        ctx.UserAlias.Add(alias);
                    }
                    else
                    {
                        alias.IsActive = userRow.UserStatusKey == "1" ? true : false;
                    }
                }
            }
        }
        private void AddAndUpdateUsersAliases(VirtualTrainerContext ctx, List<UserBusinessStructureInfo> structureFromActuris)
        {
            List<User> vtUsers = ctx.User.Include("Aliases").Include("Permissions").ToList();

            List<UserAlias> userAliases = ctx.UserAlias.ToList();

            // Some users will have email some "Team Users" will not. We need to deal with each slightly differently.
            // Deal with the Users with email accounts first.
            foreach (var uniqueUser in structureFromActuris.Where(u => !string.IsNullOrEmpty(u.UserEmail)).GroupBy(g => g.UserEmail.Trim().ToLower()))
            {
                UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
                User user = vtUsers.Where(u => u.Email.Trim().ToLower() == ubsUser.UserEmail.Trim().ToLower()).FirstOrDefault();
                AddAliasesForUser(ctx, user, uniqueUser, userAliases);
            }
            ctx.SaveChanges();

            userAliases = ctx.UserAlias.ToList();
            // Now Deal with the Users with no email accounts.
            foreach (var uniqueUser in structureFromActuris.Where(u => string.IsNullOrEmpty(u.UserEmail)).GroupBy(g => g.UserName.Trim().ToLower()))
            {
                UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
                User user = vtUsers.Where(u => u.Name.Trim().ToLower() == ubsUser.UserName.Trim().ToLower()).FirstOrDefault();
                AddAliasesForUser(ctx, user, uniqueUser, userAliases);
            }
            ctx.SaveChanges();
        }
        private List<User> AddAndUpdateUsers(VirtualTrainerContext ctx, List<UserBusinessStructureInfo> structureFromActuris)
        {
            List<User> vtUsers = ctx.User.Include("Aliases").ToList();
            List<User> AddedUsers = new List<User>();

            // Some users will have email some "Team Users" will not. We need to deal with each slightly differently.
            // Deal with the Users with email accounts first.
            foreach (var uniqueUser in structureFromActuris.Where(u => !string.IsNullOrEmpty(u.UserEmail)).GroupBy(g => g.UserEmail.Trim().ToLower()))
            {
                UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
                if (uniqueUser.Where(u => u.UserStatusKey == "1").Any())
                {
                    User user = vtUsers.Where(u => u.ActurisUniqueIdentifier != null && u.ActurisUniqueIdentifier.Trim().ToLower() == ubsUser.UserEmail.Trim().ToLower()).FirstOrDefault();
                    if (user == null)
                    {
                        user = new User()
                        {
                            ActurisUniqueIdentifier = ubsUser.UserEmail.Trim().ToLower(),
                            Email = ubsUser.UserEmail.Trim().ToLower(),
                            Name = ubsUser.UserName,
                            IsActive = true,
                        };
                        ctx.User.Add(user);
                        AddedUsers.Add(user);
                    }
                    else
                    {
                        user.Name = ubsUser.UserName;
                        user.IsActive = uniqueUser.Where(u => u.UserStatusKey == "1").Any() ? true : false;//ubsUser.HandlerStatusKey == "3" ? true : false;
                    }
                }
            } 
            ctx.SaveChanges();

            // Now Deal with the Users with no email accounts.
            foreach (var uniqueUser in structureFromActuris.Where(u => string.IsNullOrEmpty(u.UserEmail)).GroupBy(g => g.UserName.Trim().ToLower()))
            {
                UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
                if (uniqueUser.Where(u => u.UserStatusKey == "1").Any())
                {
                    User user = vtUsers.Where(u => u.Name.Trim().ToLower() == ubsUser.UserName.Trim().ToLower()).FirstOrDefault();
                    if (user == null)
                    {
                        user = new User()
                        {
                            // NO EMail so use the user name instead. 
                            ActurisUniqueIdentifier = ubsUser.UserName.Trim().ToLower(),
                            Email = string.Empty,
                            Name = ubsUser.UserName,
                            IsActive = true,
                        };
                        ctx.User.Add(user);
                        AddedUsers.Add(user);
                    }
                    else
                    {
                        user.IsActive = uniqueUser.Where(u => u.UserStatusKey == "1").Any() ? true : false;
                    }
                }
            }
            ctx.SaveChanges();
            return AddedUsers;
        }
        private void AddProjectPermissionForUser(VirtualTrainerContext ctx, User user, List<ProjectPermission> projectPermissions)
        {
            if (user != null)
            {
                //Add a project permission -so the user will be associated with the VT project
                ProjectPermission pp = projectPermissions.Where(p => p.UserId == user.Id
                                                 && p.RoleId == (int)RoleEnum.ProjectMember
                                                 && p.ProjectId == this.ProjectId).FirstOrDefault();
                if (pp == null)
                {
                    pp = new ProjectPermission()
                    {
                        UserId = user.Id,
                        ProjectId = this.ProjectId,
                        RoleId = (int)RoleEnum.ProjectMember
                    };
                    ctx.ProjectPermissions.Add(pp);
                }
            }
        }

        private void AddUserEntityPermissions(VirtualTrainerContext ctx, User user, List<TeamPermission> teamPermissions,
            Team team, List<OfficePermission> officePermissions, Office office, List<ProjectPermission> projectPermissions)
        {
            if (user != null)
            {
                ProjectPermission pp = projectPermissions.Where(p => p.UserId == user.Id
                                                 && p.RoleId == (int)RoleEnum.ProjectMember
                                                 && p.ProjectId == this.ProjectId).FirstOrDefault();
                if (pp == null)
                {
                    pp = new ProjectPermission()
                    {
                        UserId = user.Id,
                        ProjectId = this.ProjectId,
                        RoleId = (int)RoleEnum.ProjectMember
                    };
                    ctx.ProjectPermissions.Add(pp);
                }

                // Add user as team member
                TeamPermission tmem = teamPermissions.Where(em =>
                                                em.TeamId == team.Id
                                                && em.UserId == user.Id
                                                && em.RoleId == (int)RoleEnum.TeamMember).FirstOrDefault();
                if (tmem == null)
                {
                    tmem = new TeamPermission()
                    {
                        TeamId = team.Id,
                        UserId = user.Id,
                        ProjectId = this.ProjectId,
                        RoleId = (int)RoleEnum.TeamMember
                    };
                    ctx.TeamPermissions.Add(tmem);
                }
                // Add User as team claims handler
                TeamPermission bem = teamPermissions.Where(em =>
                                                em.TeamId == team.Id
                                                && em.UserId == user.Id
                                                && em.RoleId == (int)RoleEnum.ClaimsHandler).FirstOrDefault();
                if (bem == null)
                {
                    bem = new TeamPermission()
                    {
                        TeamId = team.Id,
                        UserId = user.Id,
                        ProjectId = this.ProjectId,
                        RoleId = (int)RoleEnum.ClaimsHandler
                    };
                    ctx.TeamPermissions.Add(bem);
                }

                // Add user as office member
                OfficePermission omem = officePermissions.Where(em =>
                                                em.OfficeId == office.Id
                                                && em.UserId == user.Id
                                                && em.RoleId == (int)RoleEnum.BranchMember).FirstOrDefault();
                if (omem == null)
                {
                    omem = new OfficePermission()
                    {
                        OfficeId = office.Id,
                        UserId = user.Id,
                        ProjectId = this.ProjectId,
                        RoleId = (int)RoleEnum.BranchMember
                    };
                    ctx.OfficePermissions.Add(omem);
                }
            }
        }

        private void AddEntityPermissionsForUsers(VirtualTrainerContext ctx, List<UserBusinessStructureInfo> structureFromActuris)
        {
            List<Office> vtOffices = ctx.Office.Where(o => o.ProjectId == this.ProjectId).ToList();
            List<Team> vtTeams = ctx.Team.Where(t => t.ProjectId == this.ProjectId).ToList();
            List<User> vtUsers = ctx.User.ToList();

            foreach (var uniqueOfficeGrp in structureFromActuris.GroupBy(g => g.OfficeKey))
            {
                // Get The office
                UserBusinessStructureInfo ubsOffice = uniqueOfficeGrp.FirstOrDefault();
                Office office = vtOffices.Where(o => o.AlsoKnownAs == ubsOffice.OfficeKey).FirstOrDefault();
                List<ProjectPermission> projectPermissions = ctx.ProjectPermissions.Where(p=>p.ProjectId == this.ProjectId).ToList();

                if (office != null)
                {
                    List<OfficePermission> officePermissions = ctx.OfficePermissions.ToList();

                    foreach (var uniqueTeamsGrp in uniqueOfficeGrp.GroupBy(g => g.TeamKey))
                    {
                        UserBusinessStructureInfo ubsTeam = uniqueTeamsGrp.FirstOrDefault();
                        Team team = vtTeams.Where(t => t.AlsoKnownAs == ubsTeam.TeamKey).FirstOrDefault();

                        if (team != null)
                        {
                            List<TeamPermission> teamPermissions = ctx.TeamPermissions.ToList();

                            // Process Users with Emails first.
                            foreach (var uniqueUser in uniqueTeamsGrp.Where(u => !string.IsNullOrEmpty(u.UserEmail)).GroupBy(g => g.UserEmail.Trim().ToLower()))
                            {
                                UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
                                User user = vtUsers.Where(u => !string.IsNullOrEmpty(u.ActurisUniqueIdentifier) && u.ActurisUniqueIdentifier.Trim().ToLower() == ubsUser.UserEmail.Trim().ToLower()).FirstOrDefault();
                                if (user != null)
                                {
                                    // Add user permissions
                                    AddUserEntityPermissions(ctx, user, teamPermissions, team, officePermissions, office, projectPermissions);
                                }
                            }
                            // Process Users without emails.
                            foreach (var uniqueUser in uniqueTeamsGrp.Where(u => string.IsNullOrEmpty(u.UserEmail)).GroupBy(g => g.UserName.Trim().ToLower()))
                            {
                                UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
                                User user = vtUsers.Where(u => !string.IsNullOrEmpty(u.ActurisUniqueIdentifier) && u.ActurisUniqueIdentifier.Trim().ToLower() == ubsUser.UserName.Trim().ToLower()).FirstOrDefault();
                                if (user != null)
                                {
                                    // Add user permissions
                                    AddUserEntityPermissions(ctx, user, teamPermissions, team, officePermissions, office, projectPermissions);
                                }
                            }
                        }
                    }
                }
                ctx.SaveChanges();
            }
            ctx.SaveChanges();
        }

        public List<UserBusinessStructureInfo> GetStructureFromActuris(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            List<UserBusinessStructureInfo> structureFromActuris = new List<UserBusinessStructureInfo>();
            try
            {
                structureFromActuris = GetStructureFromActuris();
            }
            catch (Exception ex)
            {
                this.LastRun = DateTime.Now;
                this.LastRunSuccess = false;
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
            }
            return structureFromActuris;
        }
        public void ProcessStructureFromActuris(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            ctx.Configuration.AutoDetectChangesEnabled = false;
            ctx.Configuration.ValidateOnSaveEnabled = false;
            ActurisImportActionTakenLog log = new ActurisImportActionTakenLog(this, ctx);
            try
            {
                if (this.IsActive)
                {
                    if (this.Schedule.IsScheduledToRun(ctx, this.LastRun))
                    {
                        List<UserBusinessStructureInfo> structureFromActuris = GetStructureFromActuris();
                        AddOffices(ctx, structureFromActuris);

                        AddTeams(ctx, structureFromActuris);

                        List<User> addedUsers = new List<User>();
                        addedUsers = AddAndUpdateUsers(ctx, structureFromActuris);

                        AddAndUpdateUsersAliases(ctx, structureFromActuris);

                        AddEntityPermissionsForUsers(ctx, structureFromActuris);

                        this.LastRun = DateTime.Now;
                        this.LastRunSuccess = true;
                    }
                    else
                    {
                        log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.ImportConfigurationNotSchedules;
                    }
                }
                else
                {
                    log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.ImportConfigurationNotActive;
                }

                log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.Success;

                ctx.SaveChanges();
                log.Success = true;
            }
            catch (Exception ex)
            {
                this.LastRun = DateTime.Now;
                this.LastRunSuccess = false;
                SystemLog errorLog = new SystemLog(ex, this.Project);
                ctx.SystemLogs.Add(errorLog);
                log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.Failure;
                log.ErrorLogEntry = errorLog;
                log.Success = false;
                log.ErrorMessage = errorLog.ErrorMessage;
            }
            finally
            {
                log.TimeStamp = DateTime.Now;
                log.Finish = DateTime.Now;
                ctx.ActionTakenLogs.Add(log);
                ctx.SaveChanges();
                ctx.Configuration.AutoDetectChangesEnabled = true;
                ctx.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        //public void ProcessStructureFromActuris(VirtualTrainerContext ctx)
        //{
        //    LoadContextObjects(ctx);
        //    ctx.Configuration.AutoDetectChangesEnabled = false;
        //    ctx.Configuration.ValidateOnSaveEnabled = false;
        //    ActurisImportActionTakenLog log = new ActurisImportActionTakenLog(this, ctx);
        //    try
        //    {
        //        if (this.IsActive)
        //        {
        //            if (this.Schedule.IsScheduledToRun(ctx, this.LastRun))
        //            {
        //                List<UserBusinessStructureInfo> results = GetStructureFromActuris();
        //                // Add all the offices
        //                foreach (var uniqueOffice in results.GroupBy(g => g.OfficeKey))
        //                {
        //                    UserBusinessStructureInfo ubsOffice = uniqueOffice.FirstOrDefault();

        //                    Office office = ctx.Office.Where(o => o.ProjectId == this.ProjectId && o.AlsoKnownAs == ubsOffice.OfficeKey).FirstOrDefault();
        //                    if (office == null)
        //                    {
        //                        office = new Office()
        //                        {
        //                            ActurisOrganisationKey = ubsOffice.OrganisationKey,
        //                            ActurisOrganisationName = ubsOffice.OrganisationName,
        //                            Name = ubsOffice.OfficeName,
        //                            AlsoKnownAs = ubsOffice.OfficeKey,
        //                            IsActive = true,
        //                            ProjectId = this.ProjectId
        //                        };
        //                        ctx.Office.Add(office);
        //                    }
        //                }
        //                // save all the offices
        //                ctx.SaveChanges();
        //                List<Office> vtOffices = ctx.Office.Where(o => o.ProjectId == this.ProjectId).ToList();

        //                // Add all the Teams
        //                foreach (var uniqueOffice in results.GroupBy(g => g.OfficeKey))
        //                {
        //                    // Get The office
        //                    UserBusinessStructureInfo ubsOffice = uniqueOffice.FirstOrDefault();
        //                    Office office = vtOffices.Where(o => o.AlsoKnownAs == ubsOffice.OfficeKey).FirstOrDefault();

        //                    // Now create all teams for this office.
        //                    foreach (var uniqueTeam in results.Where(t => t.OfficeKey == uniqueOffice.Key).GroupBy(g => g.TeamKey))
        //                    {
        //                        UserBusinessStructureInfo ubsTeam = uniqueTeam.FirstOrDefault();

        //                        Team team = ctx.Team.Where(t => t.ProjectId == this.ProjectId && t.AlsoKnownAs == ubsTeam.TeamKey).FirstOrDefault();
        //                        if (team == null)
        //                        {
        //                            team = new Team()
        //                            {
        //                                ActurisOrganisationKey = ubsTeam.OrganisationKey,
        //                                ActurisOrganisationName = ubsTeam.OrganisationName,
        //                                Name = ubsTeam.TeamName,
        //                                AlsoKnownAs = ubsTeam.TeamKey,
        //                                IsActive = true,
        //                                ProjectId = this.ProjectId,
        //                                OfficeId = office.Id
        //                            };

        //                            ctx.Team.Add(team);
        //                        }
        //                    }
        //                }
        //                // Save all the teams.
        //                ctx.SaveChanges();
        //                List<Team> vtTeams = ctx.Team.Where(t => t.ProjectId == this.ProjectId).ToList();
        //                List<User> vtUsers = ctx.User.ToList();
        //                List<ProjectPermission> projectPermissions = ctx.ProjectPermissions.ToList();

        //                foreach (var uniqueUser in results.GroupBy(g => g.HandlerKey))
        //                {
        //                    UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
        //                    User user = vtUsers.Where(u => u.AlsoKnownAs == ubsUser.HandlerKey).FirstOrDefault();
        //                    if (user == null)
        //                    {
        //                        if (ubsUser.HandlerStatusKey == "3")
        //                        {
        //                            user = new User()
        //                            {
        //                                AlsoKnownAs = ubsUser.HandlerKey,
        //                                Email = ubsUser.HandlerEmail,
        //                                Name = ubsUser.UserName,
        //                                IsActive = true,
        //                                //ProjectUniqueKey = this.ProjectUniqueKey,
        //                                ActurisOrganisationKey = ubsUser.OrganisationKey,
        //                                ActurisOrganisationName = ubsUser.OrganisationName
        //                            };
        //                            ctx.User.Add(user);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        user.Email = ubsUser.HandlerEmail;
        //                        user.IsActive = ubsUser.HandlerStatusKey == "3" ? true : false;
        //                    }
        //                }
        //                ctx.SaveChanges();
        //                vtUsers = ctx.User.ToList();
        //                // Add a project permission for each user.
        //                foreach (var uniqueUser in results.GroupBy(g => g.HandlerKey))
        //                {
        //                    UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
        //                    User user = vtUsers.Where(u => u.AlsoKnownAs == ubsUser.HandlerKey).FirstOrDefault();
        //                    if (user != null)
        //                    {
        //                        // Add a project permission - so the user will be associated with the VT project
        //                        ProjectPermission pp = projectPermissions.Where(p => p.UserId == user.Id
        //                                                       && p.RoleId == (int)RoleEnum.ProjectMember
        //                                                       && p.ProjectId == this.ProjectId).FirstOrDefault();
        //                        if (pp == null)
        //                        {
        //                            pp = new ProjectPermission()
        //                            {
        //                                UserId = user.Id,
        //                                ProjectId = this.ProjectId,
        //                                RoleId = (int)RoleEnum.ProjectMember
        //                            };
        //                            ctx.ProjectPermissions.Add(pp);
        //                        }
        //                    }
        //                }
        //                ctx.SaveChanges();
        //                vtUsers = ctx.User.ToList();

        //                // Now add the entity mappings.
        //                foreach (var uniqueOffice in results.GroupBy(g => g.OfficeKey))
        //                {
        //                    // Get The office
        //                    UserBusinessStructureInfo ubsOffice = uniqueOffice.FirstOrDefault();
        //                    Office office = vtOffices.Where(o => o.AlsoKnownAs == ubsOffice.OfficeKey).FirstOrDefault();

        //                    if (office != null)
        //                    {
        //                        List<OfficePermission> officePermissions = ctx.OfficePermissions.ToList();

        //                        foreach (var uniqueTeams in results.Where(t => t.OfficeKey == uniqueOffice.Key).GroupBy(g => g.TeamKey))
        //                        {
        //                            UserBusinessStructureInfo ubsTeam = uniqueTeams.FirstOrDefault();
        //                            Team team = vtTeams.Where(t => t.AlsoKnownAs == ubsTeam.TeamKey).FirstOrDefault();

        //                            if (team != null)
        //                            {
        //                                List<TeamPermission> teamPermissions = ctx.TeamPermissions.ToList();

        //                                foreach (var uniqueUser in results.Where(t => t.OfficeKey == uniqueOffice.Key && t.TeamKey == uniqueTeams.Key).GroupBy(g => g.HandlerKey))
        //                                {
        //                                    UserBusinessStructureInfo ubsUser = uniqueUser.FirstOrDefault();
        //                                    User user = vtUsers.Where(u => u.AlsoKnownAs == ubsUser.HandlerKey).FirstOrDefault();

        //                                    if (user != null)
        //                                    {
        //                                        // Add user as team member
        //                                        TeamPermission tmem = teamPermissions.Where(em =>
        //                                                                        em.TeamId == team.Id
        //                                                                        && em.UserId == user.Id
        //                                                                        && em.RoleId == (int)RoleEnum.TeamMember).FirstOrDefault();
        //                                        if (tmem == null)
        //                                        {
        //                                            tmem = new TeamPermission()
        //                                            {
        //                                                TeamId = team.Id,
        //                                                UserId = user.Id,
        //                                                ProjectId = this.ProjectId,
        //                                                RoleId = (int)RoleEnum.TeamMember
        //                                            };
        //                                            ctx.TeamPermissions.Add(tmem);
        //                                        }
        //                                        // Add User as team claims handler
        //                                        TeamPermission bem = teamPermissions.Where(em =>
        //                                                                        em.TeamId == team.Id
        //                                                                        && em.UserId == user.Id
        //                                                                        && em.RoleId == (int)RoleEnum.ClaimsHandler).FirstOrDefault();
        //                                        if (bem == null)
        //                                        {
        //                                            bem = new TeamPermission()
        //                                            {
        //                                                TeamId = team.Id,
        //                                                UserId = user.Id,
        //                                                ProjectId = this.ProjectId,
        //                                                RoleId = (int)RoleEnum.ClaimsHandler
        //                                            };
        //                                            ctx.TeamPermissions.Add(bem);
        //                                        }

        //                                        // Add user as office member
        //                                        OfficePermission omem = officePermissions.Where(em =>
        //                                                                        em.OfficeId == office.Id
        //                                                                        && em.UserId == user.Id
        //                                                                        && em.RoleId == (int)RoleEnum.BranchMember).FirstOrDefault();
        //                                        if (omem == null)
        //                                        {
        //                                            omem = new OfficePermission()
        //                                            {
        //                                                OfficeId = office.Id,
        //                                                UserId = user.Id,
        //                                                ProjectId = this.ProjectId,
        //                                                RoleId = (int)RoleEnum.BranchMember
        //                                            };
        //                                            ctx.OfficePermissions.Add(omem);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                ctx.SaveChanges();
        //            }
        //            else
        //            {
        //                log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.ImportConfigurationNotSchedules;
        //            }
        //        }
        //        else
        //        {
        //            log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.ImportConfigurationNotActive;
        //        }
        //        log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.Success;
        //        this.LastRun = DateTime.Now;
        //        this.LastRunSuccess = true;
        //        ctx.SaveChanges();
        //        log.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.LastRun = DateTime.Now;
        //        this.LastRunSuccess = false;
        //        SystemLog errorLog = new SystemLog(ex, this.Project);
        //        ctx.SystemLogs.Add(errorLog);
        //        log.ExecutionOutcome = ActurisImportExecutionHistoryOutcome.Failure;
        //        log.ErrorLogEntry = errorLog;
        //        log.Success = false;
        //        log.ErrorMessage = errorLog.ErrorMessage;
        //    }
        //    finally
        //    {
        //        log.TimeStamp = DateTime.Now;
        //        log.Finish = DateTime.Now;
        //        ctx.ActionTakenLogs.Add(log);
        //        ctx.SaveChanges();
        //        ctx.Configuration.AutoDetectChangesEnabled = true;
        //        ctx.Configuration.ValidateOnSaveEnabled = true;
        //    }
        //}

        public void LoadContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("TargetDatabaseDetails").IsLoaded)
            {
                ctx.Entry(this).Reference("TargetDatabaseDetails").Load();
            }
            if (!ctx.Entry(this).Reference("Schedule").IsLoaded)
            {
                ctx.Entry(this).Reference("Schedule").Load();
            }
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
        }

        #endregion

        #region [ Private Methods ]

        private List<UserBusinessStructureInfo> GetStructureFromActuris()
        {
            List<UserBusinessStructureInfo> returnList = new List<UserBusinessStructureInfo>();
            using (SqlConnection sqlConnection = new SqlConnection(this.TargetDatabaseDetails.DBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = this.SqlCommandText;
                    cmd.CommandType = this.SqlCommandTextIsStoredProc ? CommandType.StoredProcedure : CommandType.Text;
                    cmd.Connection = sqlConnection;
                    sqlConnection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                UserBusinessStructureInfo newLog = new UserBusinessStructureInfo();
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
                                        prop.SetValue(newLog, field.Value.Trim().Replace("  ", " "), null);
                                    }
                                    // Set the Target Breach Field Value Type from Results
                                    PropertyInfo propType = newLog.GetType().GetProperty(string.Format("{0}Type", field.FieldName), BindingFlags.Public | BindingFlags.Instance);
                                    if (null != propType && propType.CanWrite)
                                    {
                                        propType.SetValue(newLog, field.FieldType.Name, null);
                                    }
                                }
                                returnList.Add(newLog);
                            }
                            reader.NextResult();
                        }
                    }
                }
            }
            return returnList;
        }

        private class SqlRowField
        {
            public string FieldName { get; set; }
            public string Value { get; set; }
            public Type FieldType { get; set; }
        }

        #endregion
    }

    public class UserBusinessStructureInfo
    {
        public string UserKey { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }
        public string UserStatusKey { get; set; }
        public string UserStatusDescription { get; set; }
        public string TeamKey { get; set; }
        public string TeamName { get; set; }
        public string TeamStatusKey { get; set; }
        public string TeamStatusDescription { get; set; }
        public string OfficeKey { get; set; }
        public string OfficeName { get; set; }
        public string OrganisationKey { get; set; }
        public string OrganisationName { get; set; }
    }
}
