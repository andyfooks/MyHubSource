using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTrainer.Utilities;

namespace VirtualTrainer
{
    public abstract class EscalationsFrameworkRuleConfig
    {
        #region [ EF Properties ]

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        [ForeignKey("EscalationsFramework")]
        [Required]
        public Guid EscalationsFrameworkId { get; set; }
        public EscalationsFramework EscalationsFramework { get; set; }
        public int BreachCount { get; set; }
        [ForeignKey("Schedule")]
        [Required]
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        //[Required]
        //public DateTime ScheduleStartDate { get; set; }
        public DateTime? LastRunDate { get; set; }
        //[ForeignKey("ScheduleFrequency")]
        //public int ScheduleFrequencyId { get; set; }
        //[Required]
        //public ScheduleFrequency ScheduleFrequency { get; set; }
        // public ICollection<EscalationsActionTakenLog> EscalationsFrameworkActionsTaken { get; set; }
        public ICollection<ActionTakenLog> ActionTakenLog { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<EscalationsFrameworkBreachSource> BreachSources { get; set; }
        public EscalationInclusionEnum BreachInclusion { get; set; }

        #endregion

        #region [ Non EF Properties ]

        [NotMapped]
        public string ScheduleName { get; set; }

        #endregion

        #region [ Abstract Methods ]

        public abstract void RunEscalationConfiguration(VirtualTrainerContext ctx, bool updateTimeStamp, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment);

        #endregion

        #region [ internal Methods ]

        internal List<BreachLog> ReturnOnlyUniqueBreaches(List<BreachLog> breaches)
        {
            return breaches.GroupBy(u => u.ContextRef).Select(group => group.First()).ToList();
        }
        public void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
            if (!ctx.Entry(this).Reference("EscalationsFramework").IsLoaded)
            {
                ctx.Entry(this).Reference("EscalationsFramework").Load();
            }
            if (!ctx.Entry(this).Collection("ActionTakenLog").IsLoaded)
            {
                ctx.Entry(this).Collection("ActionTakenLog").Load();
            }
            if (!ctx.Entry(this).Reference("Schedule").IsLoaded)
            {
                ctx.Entry(this).Reference("Schedule").Load();
            }
            if (!ctx.Entry(this).Collection("BreachSources").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachSources").Load();
            }
        }

        internal List<BreachLog> GetBreachLogs(VirtualTrainerContext ctx, EscalationInclusionEnum breachMatch)
        {
            LoadRequiredContextObjects(ctx);

            List<BreachLog> breachLogs = new List<BreachLog>();

            foreach (EscalationsFrameworkBreachSource source in this.BreachSources.ToList())
            {
                breachLogs.AddRange(source.GetBreaches(ctx, breachMatch));
            }

            return breachLogs;
        }
        internal List<BreachLog> GetUniqueBreachLogsFromBreachSources(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);

            List<BreachLog> breachLogs = new List<BreachLog>();

            foreach (EscalationsFrameworkBreachSource source in this.BreachSources.ToList())
            {
                breachLogs.AddRange(source.GetBreaches(ctx));
            }
            return GetUniqueBreachesFromList(breachLogs);
        }
        internal List<BreachLog> GetUniqueBreachesFromList(List<BreachLog> breachLogs)
        {
            return breachLogs.GroupBy(u => u.ContextRef).Select(group => group.First()).ToList();
        }
        internal List<User> GetUniqueUsersFromBreachSources(VirtualTrainerContext ctx)
        {
            LoadRequiredContextObjects(ctx);

            List<User> breachSourceUsers = new List<User>();

            foreach (EscalationsFrameworkBreachSource source in this.BreachSources)
            {
                breachSourceUsers.AddRange(source.GetUsers(ctx));
            }
            return GetUniqueUsersFromList(breachSourceUsers);
        }
        internal List<User> GetUniqueUsersFromList(List<User> users)
        {
            return users.GroupBy(u => u.Id).Select(group => group.First()).ToList();
        }

        #endregion
    }
}
