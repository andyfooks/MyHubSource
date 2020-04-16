using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class Office : VirtualTrainerBase
    {
        #region [ EF Properties ]

        public int Id { get; set; }
        [IsRuleConfigurationParticipant]
        public string Name { get; set; }
        [IsRuleConfigurationParticipant]
        public string Description { get; set; }
        [IsRuleConfigurationParticipant]
        public string Address { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationKey { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationName { get; set; }
        [IsRuleConfigurationParticipant]
        public string AlsoKnownAs { get; set; }
        [InverseProperty("Office")]
        public ICollection<Team> Teams { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("Region")]
        public int? RegionId { get; set; }
        public Region Region { get; set; }
        public ICollection<RuleConfiguration> Rules { get; set; }
        public ICollection<OfficePermission> OfficePermissions { get; set; }
        public ICollection<BreachLog> BreachLogs { get; set; }

        #endregion

        #region [ Not Mapped EF properties ]

        [NotMapped]
        public string RegionName { get; set; }

        #endregion

        #region [ Public Methods ]

        public List<User> GetOfficeManagers(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityContextObject(ctx);
            List<int> userIds = this.OfficePermissions.Where(a => a.RoleId == (int)RoleEnum.BranchManager).Select(a => a.UserId).ToList();
            List<User> officeManagers = ctx.User.Where(u => userIds.Contains(u.Id)).ToList();
            return GetDistinctUsers(officeManagers);
        }
        public bool IsUserAManager(VirtualTrainerContext ctx, User user)
        {
            return GetOfficeManagers(ctx).Contains(user);
        }
        public bool IsUserARegionalManager(VirtualTrainerContext ctx, User user)
        {
            if (HasOfficeRegionalManager(ctx))
            {
                return GetOfficeRegionalManagers(ctx).Contains(user);
            }
            return false;
        }
        public bool IsUserAQualityAuditor(VirtualTrainerContext ctx, User user)
        {
            if (HasOfficeQualityAuditor(ctx))
            {
                return GetOfficeQualityAuditors(ctx).Contains(user);
            }
            return false;
        }
        public bool HasOfficeManager(VirtualTrainerContext ctx)
        {
            return GetOfficeManagers(ctx).Any();
        }
        public List<User> GetOfficeStaff(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);

            var usersIds = this.OfficePermissions.GroupBy(a => new
            {
                a.UserId
            }).Select(g => g.FirstOrDefault()).Select(q => q.UserId).ToList();

            List<User> users = (from user in ctx.User.Include("Aliases")
                    where usersIds.Contains(user.Id)
                    select user).ToList();

            return GetDistinctUsers(users);
        }
        public bool OfficeHasOutstandingBreaches(VirtualTrainerContext ctx)
        {
            LoadBreachLogsContextObject(ctx);
            return this.BreachLogs.Where(b => b.IsArchived != true).Any();
        }
        public bool OfficeHasOutstandingBreachesCountGtOrEq(VirtualTrainerContext ctx, int count)
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
            }).ToList();            //return GetOutstandingBreachesByContextValue(ctx).Where(a => a.GetBreachCountForContextRef(ctx) >= count).ToList();
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
        public List<User> GetOfficeRegionalManagers(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            List<User> users = this.OfficePermissions.Where(a => a.RoleId == (int)RoleEnum.RegionalManager).Select(a => a.User).ToList();
            return GetDistinctUsers(users);
        }
        public bool HasOfficeRegionalManager(VirtualTrainerContext ctx)
        {
            return GetOfficeRegionalManagers(ctx).Any();
        }
        public List<User> GetOfficeQualityAuditors(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            List<User> users = this.OfficePermissions.Where(a => a.RoleId == (int)RoleEnum.QualityAuditor).Select(a => a.User).ToList();
            return GetDistinctUsers(users);
        }
        public bool HasOfficeQualityAuditor(VirtualTrainerContext ctx)
        {
            return GetOfficeQualityAuditors(ctx).Any();
        }
        public List<Team> GetTeams(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.Teams.ToList();
        }

        #endregion

        #region [ Private Methods ]

        private List<User> GetDistinctUsers(List<User> users)
        {
            return users.GroupBy(u => u.Id).Select(group => group.First()).ToList();
        }

        private void LoadBusinessEntityContextObject(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("OfficePermissions").IsLoaded)
            {
                ctx.Entry(this).Collection("OfficePermissions").Load();
            }
        }
        private void LoadBreachLogsContextObject(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }
        }
        public void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            LoadBusinessEntityContextObject(ctx);
            LoadBreachLogsContextObject(ctx);
            if (!ctx.Entry(this).Collection("Rules").IsLoaded)
            {
                ctx.Entry(this).Collection("Rules").Load();
            }
            if (!ctx.Entry(this).Collection("Teams").IsLoaded)
            {
                ctx.Entry(this).Collection("Teams").Load();
            }
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
            if (!ctx.Entry(this).Reference("Region").IsLoaded)
            {
                ctx.Entry(this).Reference("Region").Load();
            }
        }

        #endregion
    }
}
