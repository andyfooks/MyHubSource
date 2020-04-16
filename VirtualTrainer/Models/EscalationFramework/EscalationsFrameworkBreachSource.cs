using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public abstract class EscalationsFrameworkBreachSource
    {
        #region [ EF properties ]

        public int Id { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }

        public Project Project { get; set; }

        [ForeignKey("EscalationsFrameworkRuleConfig")]
        [Required]
        public int EscalationsFrameworkRuleConfigId { get; set; }

        public EscalationsFrameworkRuleConfigEmail EscalationsFrameworkRuleConfig { get; set; }

        #endregion

        #region [ Methods ]

        internal abstract List<BreachLog> GetBreaches(VirtualTrainerContext ctx);

        internal abstract List<BreachLog> GetBreaches(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch);

        internal abstract List<User> GetUsers(VirtualTrainerContext ctx);
        #endregion
    }

    public class EscalationsFrameworkBreachSourceUser : EscalationsFrameworkBreachSource
    {
        #region [ EF properties ]

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

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

        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch)
        {
            throw new NotImplementedException();
        }
        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.User.GetOutstandingBreachesByContextValueCountGtOrEq(ctx, this.EscalationsFrameworkRuleConfig.BreachCount);
        }
        internal override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            List<User> users = new List<User>();

            users.Add(this.User);
            return users;
        }
        private void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("User").IsLoaded)
            {
                ctx.Entry(this).Reference("User").Load();
            }
        }
    }
    public class EscalationsFrameworkBreachSourceTeam : EscalationsFrameworkBreachSource
    {
        #region [ EF properties ]

        [ForeignKey("Team")]
        [Required]
        public int TeamID { get; set; }
        public Team Team { get; set; }

        #endregion

        #region [ Non EF properties ]

        [NotMapped]
        public string TeamName { get; set; }
        [NotMapped]
        public string ActurisOrganisationKey { get; set; }
        [NotMapped]
        public string ActurisOrganisationName { get; set; }
        [NotMapped]
        public string AlsoKnownAs { get; set; }

        #endregion

        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch)
        {
            throw new NotImplementedException();
        }
        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.Team.GetOutstandingBreachesByContextValueCountGtOrEq(ctx, this.EscalationsFrameworkRuleConfig.BreachCount);
        }
        internal override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);

            return this.Team.GetTeamMembers(ctx);
        }
        private void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Team").IsLoaded)
            {
                ctx.Entry(this).Reference("Team").Load();
            }
        }
    }
    public class EscalationsFrameworkBreachSourceOffice : EscalationsFrameworkBreachSource
    {
        #region [ EF properties ]

        [ForeignKey("Office")]
        [Required]
        public int OfficeId { get; set; }
        public Office Office { get; set; }

        #endregion

        #region [ Non EF properties ]
        
        [NotMapped]
        public string OfficeName { get; set; }
        [NotMapped]
        public string ActurisOrganisationKey { get; set; }
        [NotMapped]
        public string ActurisOrganisationName { get; set; }
        [NotMapped]
        public string AlsoKnownAs { get; set; }

        #endregion

        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch)
        {
            throw new NotImplementedException();
        }
        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.Office.GetOutstandingBreachesByContextValueCountGtOrEq(ctx, this.EscalationsFrameworkRuleConfig.BreachCount);
        }
        internal override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.Office.GetOfficeStaff(ctx);
        }
        private void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Office").IsLoaded)
            {
                ctx.Entry(this).Reference("Office").Load();
            }
        }
    }
    public class EscalationsFrameworkBreachSourceProject : EscalationsFrameworkBreachSource
    {
        #region [ Non EF Properties ]

        [NotMapped]
        public string ProjectName { get; set; }

        #endregion

        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch)
        {
            throw new NotImplementedException();
        }
        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.Project.GetOutstandingBreachesByContextValueCountGtOrEq(ctx, this.EscalationsFrameworkRuleConfig.BreachCount);
        }
        internal override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.Project.GetAllUsers(ctx);
        }
        private void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
        }
    }
    public class EscalationsFrameworkBreachSourceRule : EscalationsFrameworkBreachSource
    {
        #region [ EF properties ]

        [ForeignKey("Rule")]
        [Required]
        public int RuleId { get; set; }
        public Rule Rule { get; set; }

        #endregion

        #region [ Non EF properties ]

        [NotMapped]
        public string RuleName { get; set; }

        #endregion
        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch)
        {
            LoadRequiredContextObjects(ctx);
            List<BreachLog> breaches = new List<BreachLog>();
            switch (breachMatch)
            {
                case EscalationInclusionEnum.AllBreaches:
                    return this.Rule.GetAllBreaches(ctx);
                case EscalationInclusionEnum.OnlyArchived:
                    return this.Rule.GetAllArchivedBreaches(ctx);
                case EscalationInclusionEnum.OnlyNotArchived:
                    return this.Rule.GetAllOutstandingBreaches(ctx);
            }
            return breaches;
        }
        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.Rule.GetOutstandingBreachesByContextValueCountGtOrEq(ctx, this.EscalationsFrameworkRuleConfig.BreachCount);
        }
        internal override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);

            return this.Rule.GetAllRuleParticipants(ctx);
        }
        private void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Rule").IsLoaded)
            {
                ctx.Entry(this).Reference("Rule").Load();
            }
        }
    }
    public class EscalationsFrameworkBreachSourceRuleConfiguration : EscalationsFrameworkBreachSource
    {
        #region [ EF properties ]

        [ForeignKey("RuleConfiguration")]
        [Required]
        public int RuleConfigurationId { get; set; }
        public RuleConfiguration RuleConfiguration { get; set; }

        #endregion

        #region [ Non EF properties ]

        [NotMapped]
        public string RuleConfigName { get; set; }

        #endregion

        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch)
        {
            LoadRequiredContextObjects(ctx);
            List<BreachLog> breaches = new List<BreachLog>();
            switch (breachMatch)
            {
                case EscalationInclusionEnum.AllBreaches:
                    return this.RuleConfiguration.GetAllBreaches(ctx);
                case EscalationInclusionEnum.OnlyArchived:
                    return this.RuleConfiguration.GetAllArchivedBreaches(ctx);
                case EscalationInclusionEnum.OnlyNotArchived:
                    return this.RuleConfiguration.GetAllOutstandingBreaches(ctx);
            }
            return breaches;
        }
        internal override List<BreachLog> GetBreaches(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.RuleConfiguration.GetOutstandingBreachesByContextValueCountGtOrEq(ctx, this.EscalationsFrameworkRuleConfig.BreachCount);
        }
        internal override List<User> GetUsers(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);
            return this.RuleConfiguration.GetParticipants(ctx);
        }
        private void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("RuleConfiguration").IsLoaded)
            {
                ctx.Entry(this).Reference("RuleConfiguration").Load();
            }
        }
    }
}
