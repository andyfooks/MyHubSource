using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using VirtualTrainer.Interfaces;

namespace VirtualTrainer
{
    public class User : VirtualTrainerBase
    {

        #region [Mapped Properties]

        public int Id { get; set; }
        [IsRuleConfigurationParticipant]
        public string Name { get; set; }
        [IsRuleConfigurationParticipant]
        public string DomainName { get; set; }
        [IsRuleConfigurationParticipant]
        public string Email { get; set; }
        [ForeignKey("Title")]
        public int? TitleId { get; set; }
        [IsRuleConfigurationParticipant]
        public Title Title { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationKey { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationName { get; set; }
        [IsRuleConfigurationParticipant]
        public string AlsoKnownAs { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisUniqueIdentifier { get; set; }
        public bool IsActive { get; set; }
        [InverseProperty("User")]
        public ICollection<UserAlias> Aliases { get; set; }
        [InverseProperty("User")]
        public ICollection<SystemPermission> Permissions { get; set; }
        public ICollection<RuleParticipantUser> RuleParticipations{ get; set; }
        public ICollection<BreachLog> BreachLogs { get; set; }

        #endregion

        #region [Unmapped Proeperties]

        [NotMapped]
        public VirtualTrainerContext Context { get; set; }
        [NotMapped]
        public int EscalationsFrameWorkRuleConfigurationBreachCount { get; set; }
        [NotMapped]
        public bool IsSystemAdmin { get; set; }
        [NotMapped]
        public bool IsSystemSuperUser { get; set; }
        [NotMapped]
        public string SystemSuperUserInfo { get; set; }
        [NotMapped]
        public bool IsProjectAdmin { get; set; }
        [NotMapped]
        public bool IsMicroServiceMethodAccessUser { get; set; }
        [NotMapped]
        public bool IsProjectMember { get; set; }
        [NotMapped]
        public bool IsProjectSuperUser { get; set; }
        [NotMapped]
        public string ProjectSuperUserInfo { get; set; }
        [NotMapped]
        public bool IsOfficeManager { get; set; }
        [NotMapped]
        public bool IsTeamLead { get; set; }
        [NotMapped]
        public bool IsClaimsHandler { get; set; }
        [NotMapped]
        public bool IsOfficeRegionalManager { get; set; }
        [NotMapped]
        public bool IsOfficeQualityAuditor { get; set; }
        [NotMapped]
        public bool IsTeamMember { get; set; }
        [NotMapped]
        public bool IsOfficeMember { get; set; }
        [NotMapped]
        public List<OfficePermission> OfficePermissions { get; set; }
        [NotMapped]
        public List<TeamPermission> TeamPermissions { get; set; }
        [NotMapped]
        public List<ProjectPermission> ProjectPermissions { get; set; }
        #endregion

        #region [Public Methods]

        public SystemPermission GetSuperUserPermission()
        {
            if(this.Permissions != null && this.Permissions.Count > 0)
            {
                var perm = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.SuperUser).FirstOrDefault();
                if(perm != null)
                {
                    return perm;
                }
            }
            return null;
        }
        public ProjectPermission GetProjectSuperUSer(Guid ProjectIdGuid)
        {
            if (this.Permissions != null && this.Permissions.Count > 0)
            {
                var perm = this.ProjectPermissions.Where(p => p.ProjectId == ProjectIdGuid && p.RoleId == (int)RoleEnum.SuperUser).FirstOrDefault();
                if (perm != null)
                {
                    return perm;
                }
            }
            return null;
        }
        public void LoadRuleBreachLogs(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }
        }
        public List<BreachLog> GetAllOutstandingBreaches(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            // Include filtering by IsActive on project, office, team.  
            return this.BreachLogs.Where(b => b.IsArchived != true).ToList();
        }
        public List<BreachLog> GetAllBreaches(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            // Include filtering by IsActive on project, office, team.  
            return this.BreachLogs.ToList();
        }
        public bool UserHasOutstandingBreaches(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.GetAllOutstandingBreaches(ctx).Any();
        }
        public bool UserHasOutstandingBreachesCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadAllContextObjects(ctx);
            return GetOutstandingBreachesByContextValueCountGtOrEq(ctx, count).Any();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadAllContextObjects(ctx);
            return this.BreachLogs.Where(d => d.IsArchived != true).OrderBy(a => a.TimeStamp).GroupBy(b => b.ContextRef).Select(c => {
                c.LastOrDefault().BreachLiveContextRefCount = c.Count();
                c.LastOrDefault().FirstBreachDate = c.FirstOrDefault().TimeStamp;
                return c.LastOrDefault();
            }).ToList();
            //return GetOutstandingBreachesByContextValue(ctx).Where(a => a.GetBreachCountForContextRef(ctx) >= count).ToList();
        } 
        public List<BreachLog> GetOutstandingBreachesByContextValue(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.GetAllOutstandingBreaches(ctx).OrderByDescending(d => d.TimeStamp).GroupBy(a => new
            {
                a.ContextRef
            }).Select(g => g.FirstOrDefault()).ToList();
        }
        public List<BreachLog> GetAllBreachesByContextValue(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.GetAllBreaches(ctx).OrderByDescending(d => d.TimeStamp).GroupBy(a => new
            {
                a.ContextRef
            }).Select(g => g.FirstOrDefault()).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedManager(VirtualTrainerContext ctx, User manager)
        {
            // Get my breaches
            List<BreachLog> breaches = GetOutstandingBreachesByContextValue(ctx);
            // Now filter so only breaches this manager cares about are returned.
            return breaches.Where(b => manager.GetMyOffices(ctx).Contains(b.Office)).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedManagerEqOrAboveCount(VirtualTrainerContext ctx, User manager, int count)
        {
            // Get my breaches
            List<BreachLog> logs = GetOutstandingBreachesByContextValueForNamedManager(ctx, manager);
            return logs.Where(l => l.GetBreachCountForContextRef(ctx) >= count).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedTeamLead(VirtualTrainerContext ctx, User teamLead)
        {
            // Get my breaches
            List<BreachLog> breaches = GetOutstandingBreachesByContextValue(ctx);
            // Now filter so only breaches this manager cares about are returned.
            return breaches.Where(b => teamLead.GetTeamsILead(ctx).Contains(b.Team)).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedTeamLeadEqOrAboveCount(VirtualTrainerContext ctx, User teamLead, int count)
        {
            // Get my breaches
            List<BreachLog> logs = GetOutstandingBreachesByContextValueForNamedManager(ctx, teamLead);
            return logs.Where(l => l.GetBreachCountForContextRef(ctx) >= count).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedQualityAuditor(VirtualTrainerContext ctx, User qualityAuditor)
        {
            // Get my breaches
            List<BreachLog> breaches = GetOutstandingBreachesByContextValue(ctx);
            // Now filter so only breaches this manager cares about are returned.
            return breaches.Where(b => qualityAuditor.GetTeamsIQualityAssure(ctx).Contains(b.Team)).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedQualityAuditorEqOrAboveCount(VirtualTrainerContext ctx, User qualityAuditor, int count)
        {
            // Get my breaches
            List<BreachLog> logs = GetOutstandingBreachesByContextValueForNamedQualityAuditor(ctx, qualityAuditor);
            return logs.Where(l => l.GetBreachCountForContextRef(ctx) >= count).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedRegionalManager(VirtualTrainerContext ctx, User regionalManager)
        {
            // Get my breaches
            List<BreachLog> breaches = GetOutstandingBreachesByContextValue(ctx);
            // Now filter so only breaches this manager cares about are returned.
            return breaches.Where(b => regionalManager.GetTeamsIRegioanllyManage(ctx).Contains(b.Team)).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueForNamedRegionalManagerEqOrAboveCount(VirtualTrainerContext ctx, User regionalManager, int count)
        {
            // Get my breaches
            List<BreachLog> logs = GetOutstandingBreachesByContextValueForNamedRegionalManager(ctx, regionalManager);
            return logs.Where(l => l.GetBreachCountForContextRef(ctx) >= count).ToList();
        }
        public List<String> GetAllPossibleUserNames(VirtualTrainerContext ctx)
        {
            LoadAliasesContextObject(ctx);

            List<String> returnValue = Aliases.Select(a => a.Alias).ToList<string>();
            returnValue.Add(this.Name);
            returnValue.Add(this.DomainName);
            returnValue.Add(this.Email);
            returnValue.Add(this.AlsoKnownAs);

            return returnValue;
        }
        public List<Role> GetAllRoles(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappings(ctx);
            var rolesIds = this.Permissions.GroupBy(a => new
            {
                a.RoleId
            }).Select(g => g.FirstOrDefault()).Select(q => q.RoleId).ToList();

            return (from role in ctx.Role
                    where rolesIds.Contains(role.Id)
                    select role).ToList();
        }
        public void LoadPermissionBools(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappings(ctx);

            this.IsClaimsHandler = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.ClaimsHandler).Any();
            this.IsTeamMember = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.TeamMember).Any();
            this.IsTeamLead = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.TeamLead).Any();
            this.IsOfficeMember = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.BranchMember).Any();
            this.IsOfficeManager = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.BranchManager).Any();
            this.IsOfficeQualityAuditor = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.QualityAuditor).Any();
            this.IsOfficeRegionalManager = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.RegionalManager).Any();
            this.IsProjectMember = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.ProjectMember).Any();
            this.IsProjectAdmin = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.ProjectAdmin).Any();
            this.IsSystemAdmin = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.SystemAdmin).Any();
            this.IsSystemSuperUser = this.Permissions.Where(p => p.RoleId == (int)RoleEnum.SuperUser).Any();
        }
        public void LoadProjectPermissions(VirtualTrainerContext ctx, Guid ProjectId)
        {
            LoadBusinessEntityMappings(ctx);
            this.IsProjectAdmin = ctx.ProjectPermissions.Where(a => a.ProjectId == ProjectId && a.UserId == this.Id && a.RoleId == (int)RoleEnum.ProjectAdmin).Any();

            if(ctx.ProjectPermissions.Where(a => a.ProjectId == ProjectId && a.UserId == this.Id && a.RoleId == (int)RoleEnum.SuperUser).Any())
            {
                var perm = ctx.ProjectPermissions.Where(a => a.ProjectId == ProjectId && a.UserId == this.Id && a.RoleId == (int)RoleEnum.SuperUser).FirstOrDefault();
                this.IsProjectSuperUser = true;
                this.ProjectSuperUserInfo = string.IsNullOrEmpty(perm.Info) ? string.Empty : perm.Info;
            }
        }
        public bool HasRole(params string[] strings)
        {
            return false;
        }
        public bool HasRole()
        {
            return true;
        }
        public bool HasRole(VirtualTrainerContext ctx, RoleEnum role)
        {
            LoadBusinessEntityMappings(ctx);
            return this.Permissions.Where(a => a.RoleId == (int)role).Any();
        }
        public bool HasTeamRole(VirtualTrainerContext ctx, int teamId, RoleEnum role)
        {
            LoadBusinessEntityMappings(ctx);
            return ctx.TeamPermissions.Where(a => a.TeamId == teamId && a.UserId == this.Id && a.RoleId == (int)role).Any();
        }
        public bool HasOfficeRole(VirtualTrainerContext ctx, int officeId, RoleEnum role)
        {
            LoadBusinessEntityMappings(ctx);
            return ctx.OfficePermissions.Where(a => a.OfficeId == officeId && a.UserId == this.Id && a.RoleId == (int)role).Any();
        }
        public bool HasRole(VirtualTrainerContext ctx, Guid ProjectId, RoleEnum role)
        {
            LoadBusinessEntityMappings(ctx);
            return ctx.ProjectPermissions.Where(a => a.ProjectId == ProjectId && a.UserId == this.Id && a.RoleId == (int)role).Any();
        }
        public bool HasRole(VirtualTrainerContext ctx, Role role)
        {
            LoadBusinessEntityMappings(ctx);
            return this.Permissions.Where(a => a.RoleId == role.Id).Any();
        }
        public bool HasRole(VirtualTrainerContext ctx, int roleId)
        {
            LoadBusinessEntityMappings(ctx);
            return this.Permissions.Where(a => a.RoleId == roleId).Any();
        }
        public bool IsManager(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappings(ctx);
            return this.Permissions.Where(a => a.RoleId == (int)RoleEnum.BranchManager).Any();
        }
        public bool IsAQualityAuditor(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappings(ctx);
            return this.Permissions.Where(a => a.RoleId == (int)RoleEnum.QualityAuditor).Any();
        }
        public bool IsARegionalManager(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappings(ctx);
            return this.Permissions.Where(a => a.RoleId == (int)RoleEnum.RegionalManager).Any();
        }
        public bool IsATeamLead(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappings(ctx);
            return this.Permissions.Where(a => a.RoleId == (int)RoleEnum.TeamLead).Any();
        }
        public bool HasBreaches(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.BreachLogs.Where(b => b.IsArchived != true).Any();
        }
        public List<User> GetPeopleInAllMyTeams(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);

            List<User> users = new List<User>();
            List<Team> myTeams = GetMyTeams(ctx);
            foreach (Team team in myTeams)
            {
                users.AddRange(team.GetTeamMembers(ctx));
            }
            return GetDistinctUsers(users);
        }
        public List<User> GetPeopleInTeam(VirtualTrainerContext ctx, int TeamId)
        {
            LoadAllContextObjects(ctx);
            Team myTeams = ctx.Team.Where(a => a.Id == TeamId).FirstOrDefault();
            List<User> users = myTeams.GetTeamMembers(ctx);
            return GetDistinctUsers(users);
        }
        public List<Office> GetOfficesIManage(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            List<Office> myOffices = GetMyOffices(ctx);
            return myOffices.Where(t => t.GetOfficeManagers(ctx).Contains(this)).ToList();
        }
        public List<Office> GetMyOffices(VirtualTrainerContext ctx)
        {
            var officeIds = ctx.OfficePermissions.Where(u=>u.UserId == this.Id).GroupBy(a => new
            {
                a.OfficeId
            }).Select(g => g.FirstOrDefault()).Select(q => q.OfficeId).ToList();

            return (from office in ctx.Office
                    where officeIds.Contains(office.Id)
                    select office).ToList();
        }
        public List<User> GetPeopleIManange(VirtualTrainerContext ctx)
        {
            List<Office> myOffices = this.GetOfficesIManage(ctx);

            List<User> users = new List<User>();

            foreach (Office office in myOffices)
            {
                users.AddRange(office.GetOfficeStaff(ctx));
            }
            return GetDistinctUsers(users);
        }
        public List<User> GetPeopleITeamLead(VirtualTrainerContext ctx)
        {
            List<Team> myTeams = this.GetTeamsILead(ctx);
            List<User> users = new List<User>();
            foreach (Team team in myTeams)
            {
                users.AddRange(team.GetTeamMembers(ctx));
            }
            return GetDistinctUsers(users);
        }

        public List<Team> GetTeamsILead(VirtualTrainerContext ctx)
        {
            List<Team> myTeams = GetMyTeams(ctx);
            return myTeams.Where(t => t.GetTeamLeads(ctx).Contains(this)).ToList();
        }
        public List<Team> GetMyTeams(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);

            var teamIds = ctx.TeamPermissions.GroupBy(a => new
            {
                a.TeamId
            }).Select(g => g.FirstOrDefault()).Select(q => q.TeamId).ToList();

            return (from team in ctx.Team
                    where teamIds.Contains(team.Id)
                    select team).ToList();
        }
        public Team GetTeamById(VirtualTrainerContext ctx, int TeamID)
        {
            LoadAllContextObjects(ctx);
            return ctx.Team.Where(t=>t.Id == TeamID).FirstOrDefault();
        }

        public List<Rule> GetAllRulesIParticipateIn(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            var rules = this.RuleParticipations.Select(q =>
                    q.GetRule(ctx)
                ).GroupBy(a => a.Description).Select(g => g.FirstOrDefault()).ToList();

            return rules;
        }
        public List<Team> GetTeamsIQualityAssure(VirtualTrainerContext ctx)
        {
            List<Team> myTeams = GetMyTeams(ctx);
            myTeams = myTeams.Where(t => t.GetTeamQualityAuditors(ctx).Contains(this)).ToList();
            return GetDistinctTeams(myTeams);
        }
        public List<Team> GetTeamsIRegioanllyManage(VirtualTrainerContext ctx)
        {
            List<Team> myTeams = GetMyTeams(ctx);
            myTeams = myTeams.Where(t => t.GetTeamRegionalManagers(ctx).Contains(this)).ToList();
            return GetDistinctTeams(myTeams);
        }
        public string GetValueForSpecifiedProperty(VirtualTrainerContext ctx, string propertyName)
        {
            string returnPropertyValue = string.Empty;

            if (!string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo prop = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop)
                {
                    var value = prop.GetValue(this);
                    if (value != null)
                        return value.ToString();
                }
            }
                
            return returnPropertyValue;
        }
        #endregion

        #region [ Private methods ]

        private void LoadBusinessEntityMappings(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("Permissions").IsLoaded)
            {
                ctx.Entry(this).Collection("Permissions").Load();
            }
        }
        private void LoadAliasesContextObject(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("Aliases").IsLoaded)
            {
                ctx.Entry(this).Collection("Aliases").Load();
            }
        }

        private void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappings(ctx);
            LoadAliasesContextObject(ctx);
            LoadRuleBreachLogs(ctx);
            if (!ctx.Entry(this).Collection("RuleParticipations").IsLoaded)
            {
                ctx.Entry(this).Collection("RuleParticipations").Load();
            }
        }
        private List<Team> GetDistinctTeams(List<Team> teams)
        {
            return teams.GroupBy(u => u.Id).Select(group => group.First()).ToList();
        }
        private List<User> GetDistinctUsers(List<User> users)
        {
            return users.GroupBy(u => u.Id).Select(group => group.First()).ToList();
        }

        #endregion
    }
}
