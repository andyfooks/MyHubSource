using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public abstract class RuleParticipant : VirtualTrainerBase
    {
        #region [ EF properties ]

        public int Id { get; set; }
        [ForeignKey("RuleConfiguration")]
        [Required]
        public int RuleConfigurationId { get; set; }
        public RuleConfiguration RuleConfiguration { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region [ Non EF proeprties ]

        [NotMapped]
        public string ProjectName { get; set; }

        #endregion

        #region [ Methods ]

        public abstract List<User> GetUsers(VirtualTrainerContext ctx);

        #endregion

        #region [ Private Methods ]

        internal void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
            if (!ctx.Entry(this).Reference("RuleConfiguration").IsLoaded)
            {
                ctx.Entry(this).Reference("RuleConfiguration").Load();
            }
        }

        #endregion
    }
    public class RuleParticipantUser : RuleParticipant
    {
        #region [ Constructors ]

        public RuleParticipantUser() : base() { }

        #endregion

        #region [EF properties]

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        [IsRuleConfigurationParticipant]
        public User User { get; set; }
        [IsRuleConfigurationParticipant]
        public UserAlias Alias { get; set; }

        #endregion

        #region [ Non EF properties ]

        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string ActurisOrganisationKey { get; set; }
        [NotMapped]
        public string ActurisOrganisationName { get; set; }
        [NotMapped]
        public string AlsoKnownAs { get; set; }

        #endregion

        #region [Public Methods]

        public override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            List<User> users = new List<User>();
            User user = ctx.User.Include("Aliases").Where(u => u.Id == this.UserId).FirstOrDefault();
            users.Add(user);
            return users;
        }
        public Rule GetRule(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return ctx.Rules.Where(a => a.Id == this.RuleConfiguration.RuleId && a.ProjectId == this.Project.ProjectUniqueKey).FirstOrDefault();
        }

        #endregion

        #region [ Private methods ]

        private new void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            base.LoadAllContextObjects(ctx);

            if (!ctx.Entry(this).Reference("Alias").IsLoaded)
            {
                ctx.Entry(this).Reference("Alias").Load();
            }
            if (!ctx.Entry(this).Reference("User").IsLoaded)
            {
                ctx.Entry(this).Reference("User").Load();
            }
        }

        #endregion
    }
    public class RuleParticipantTeam : RuleParticipant
    {
        #region [ Constructors ]

        public RuleParticipantTeam() : base() { }

        #endregion

        #region [EF properties]

        [ForeignKey("Team")]
        [Required]
        public int TeamId { get; set; }
        public Team Team { get; set; }

        #endregion

        #region [ Non EF Properties]

        [NotMapped]
        public string TeamName { get; set; }
        [NotMapped]
        public string ActurisOrganisationKey { get; set; }
        [NotMapped]
        public string ActurisOrganisationName { get; set; }
        [NotMapped]
        public string AlsoKnownAs { get; set; }

        #endregion

        #region [Public Methods]

        public override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.Team.GetTeamMembers(ctx);
        }
        public Rule GetRule(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return ctx.Rules.Where(a => a.Id == this.RuleConfiguration.RuleId && a.ProjectId == this.Project.ProjectUniqueKey).FirstOrDefault();
        }
        public Office GetOffice(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return ctx.Office.Where(o => o.Id == this.Team.OfficeId).FirstOrDefault();
        }
        public Team GetTeam(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.Team;
        }

        #endregion

        #region [ Private methods ]

        private new void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            base.LoadAllContextObjects(ctx);

            if (!ctx.Entry(this).Reference("Team").IsLoaded)
            {
                ctx.Entry(this).Reference("Team").Load();
            }
        }

        #endregion
    }
    public class RuleParticipantOffice : RuleParticipant
    {
        #region [ Constructors ]

        public RuleParticipantOffice() : base() { }

        #endregion

        #region [EF properties]

        [ForeignKey("Office")]
        [Required]
        public int OfficeId { get; set; }
        public Office Office { get; set; }

        #endregion

        #region [ Non EF Properties ]

        [NotMapped]
        public string OfficeName { get; set; }
        [NotMapped]
        public string ActurisOrganisationKey { get; set; }
        [NotMapped]
        public string ActurisOrganisationName { get; set; }
        [NotMapped]
        public string AlsoKnownAs { get; set; }

        #endregion

        #region [Public Methods]

        public override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.Office.GetOfficeStaff(ctx);
        }
        public Rule GetRule(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return ctx.Rules.Where(a => a.Id == this.RuleConfiguration.RuleId && a.ProjectId == this.Project.ProjectUniqueKey).FirstOrDefault();
        }
        public Office GetOffice(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.Office;
        }
        public List<Team> GetTeams(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.Office.Teams.ToList();
        }

        #endregion

        #region [ Private methods ]

        private new void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            base.LoadAllContextObjects(ctx);

            if (!ctx.Entry(this).Reference("Office").IsLoaded)
            {
                ctx.Entry(this).Reference("Office").Load();
            }
        }

        #endregion
    }

    public class RuleParticipantProject: RuleParticipant
    {
        #region [ Constructors ]

        public RuleParticipantProject() : base() { }

        #endregion

        #region [Public Methods]

        public override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return this.Project.GetAllUsers(ctx);
        }
        public Rule GetRule(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            return ctx.Rules.Where(a => a.Id == this.RuleConfiguration.RuleId && a.ProjectId == this.Project.ProjectUniqueKey).FirstOrDefault();
        }
        public List<Office> GetOffices(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);
            
            return ctx.Office.Where(a => a.ProjectId == this.Project.ProjectUniqueKey).ToList();
        }
        public List<Team> GetTeams(VirtualTrainerContext ctx)
        {
            LoadAllContextObjects(ctx);

            return ctx.Team.Where(a => a.ProjectId == this.Project.ProjectUniqueKey).ToList();
        }

        #endregion

        #region [ Private methods ]

        private new void LoadAllContextObjects(VirtualTrainerContext ctx)
        {
            base.LoadAllContextObjects(ctx);

            //if (!ctx.Entry(this).Reference("Office").IsLoaded)
            //{
            //    ctx.Entry(this).Reference("Office").Load();
            //}
        }

        #endregion
    }
}
