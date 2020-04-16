using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class Team : VirtualTrainerBase
    {
        #region [EF Properties]

        public int Id { get; set; }
        [IsRuleConfigurationParticipant]
        public string Name { get; set; }
        [IsRuleConfigurationParticipant]
        public string Description { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationKey { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationName { get; set; }
        [IsRuleConfigurationParticipant]
        public string AlsoKnownAs { get; set; }
        [ForeignKey("Office")]
        [Required]
        public int OfficeId { get; set; }
        public Office Office { get; set; }
        [NotMapped]
        public string OfficeName { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }
        //[ForeignKey("TeamLead")]
        //public int? TeamLeadId { get; set; }     
        //public User TeamLead { get; set; }
        public ICollection<TeamPermission> TeamPermissions { get; set; }
        public ICollection<RuleConfiguration> Rules { get; set; }
        public ICollection<BreachLog> BreachLogs { get; set; }

        #endregion

        #region [ public methods ]

        public List<User> GetTeamLeads(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappingsContextObjects(ctx);
            List<User> users = this.TeamPermissions.Where(a => a.RoleId == (int)RoleEnum.TeamLead).Select(a => a.User).ToList();
            return GetDistinctUsers(users);
        }
        public bool IsUserATeamLead(VirtualTrainerContext ctx, User user)
        {
            if(HasTeamLeads(ctx))
            {
                return GetTeamLeads(ctx).Contains(user);
            }
            return false;
        }
        public bool IsUserARegionalManager(VirtualTrainerContext ctx, User user)
        {
            if (HasTeamRegionalManager(ctx))
            {
                return GetTeamRegionalManagers(ctx).Contains(user);
            }
            return false;
        }
        public bool IsUserAQualityAuditor(VirtualTrainerContext ctx, User user)
        {
            if (HasTeamQualityAuditor(ctx))
            {
                return GetTeamQualityAuditors(ctx).Contains(user);
            }
            return false;
        }
        public bool HasTeamLeads(VirtualTrainerContext ctx)
        {
            return GetTeamLeads(ctx).Any();
        }
        public List<User> GetTeamRegionalManagers(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappingsContextObjects(ctx);
            List<User> users = this.TeamPermissions.Where(a => a.RoleId == (int)RoleEnum.RegionalManager).Select(a => a.User).ToList();
            return GetDistinctUsers(users);
        }
        public bool HasTeamRegionalManager(VirtualTrainerContext ctx)
        {
            return GetTeamRegionalManagers(ctx).Any();
        }
        public List<User> GetTeamQualityAuditors(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappingsContextObjects(ctx);
            List<User> users = this.TeamPermissions.Where(a => a.RoleId == (int)RoleEnum.QualityAuditor).Select(a => a.User).ToList();
            return GetDistinctUsers(users);
        }
        public bool HasTeamQualityAuditor(VirtualTrainerContext ctx)
        {
            return GetTeamQualityAuditors(ctx).Any();
        }
        public List<User> GetTeamMembers(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappingsContextObjects(ctx);

            var usersIds = this.TeamPermissions.GroupBy(a => new
            {
                a.UserId
            }).Select(g => g.FirstOrDefault()).Select(q => q.UserId).ToList();

            List<User> users = (from user in ctx.User.Include("Aliases")
                                where usersIds.Contains(user.Id)
                                select user).ToList();

            //List<User> users = this.TeamPermissions.Where(a => a.TeamId == this.Id).Select(a => a.User).ToList();
            return GetDistinctUsers(users);
        }


        public bool TeamHasOutstandingBreaches(VirtualTrainerContext ctx)
        {
            LoadBreachLogsContextObject(ctx);
            return this.GetAllOutstandingBreaches(ctx).Any();
        }
        public bool TeamHasOutstandingBreachesCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadBreachLogsContextObject(ctx);
            return this.BreachLogs.Where(b => b.IsArchived != true && b.GetBreachCountForContextRef(ctx) >= count).Any();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValueCountGtOrEq(VirtualTrainerContext ctx, int count)
        {
            LoadBreachLogsContextObject(ctx);
            return this.BreachLogs.Where(d => d.IsArchived != true).OrderBy(a => a.TimeStamp).GroupBy(b => b.ContextRef).Select(c => {
                c.LastOrDefault().BreachLiveContextRefCount = c.Count();
                c.LastOrDefault().FirstBreachDate = c.FirstOrDefault().TimeStamp;
                return c.LastOrDefault();
            }).ToList();
            //return GetOutstandingBreachesByContextValue(ctx).Where(a => a.GetBreachCountForContextRef(ctx) >= count).ToList();
        }
        public List<BreachLog> GetOutstandingBreachesByContextValue(VirtualTrainerContext ctx)
        {
            LoadBreachLogsContextObject(ctx);
            return this.GetAllOutstandingBreaches(ctx).OrderByDescending(d => d.TimeStamp).GroupBy(a => new
            {
                a.ContextRef
            }).Select(g => g.FirstOrDefault()).ToList();
        }
        public List<BreachLog> GetAllOutstandingBreaches(VirtualTrainerContext ctx)
        {
            LoadBreachLogsContextObject(ctx);
            // Include filtering by IsActive on project, office, team.  
            return this.BreachLogs.Where(b => b.IsArchived != true).ToList();
        }

        #endregion

        #region [Private methods]

        private List<User> GetDistinctUsers(List<User> users)
        {
            return users.GroupBy(u => u.Id).Select(group => group.First()).ToList();
        }

        private void LoadBusinessEntityMappingsContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("TeamPermissions").IsLoaded)
            {
                ctx.Entry(this).Collection("TeamPermissions").Load();
            }
        }
        private void LoadBreachLogsContextObject(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }
        }
        private void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityMappingsContextObjects(ctx);
            LoadBreachLogsContextObject(ctx);
            if (!ctx.Entry(this).Collection("Rules").IsLoaded)
            {
                ctx.Entry(this).Collection("Rules").Load();
            }
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
        }
        #endregion
    }
}
