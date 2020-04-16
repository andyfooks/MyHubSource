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
    public class Schedule
    {
        #region [ EF properties ]

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime ScheduleStartDate { get; set; }
        //public DateTime? LastRunDate { get; set; }
        [ForeignKey("ScheduleFrequency")]
        [Required]
        public int ScheduleFrequencyId { get; set; }
        public ScheduleFrequency ScheduleFrequency { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        #endregion

        #region [ Non EF properties ]

        [NotMapped]
        public string ScheduleFrequencyName { get; set; }

        #endregion

        #region [ Public Methods ]

        internal bool IsScheduledToRun(VirtualTrainerContext ctx, DateTime? LastRunDate)
        {
            LoadContextObjects(ctx);
            // Don't allow to run if the scheduled start date is not today or in the past.
            if (this.ScheduleStartDate <= DateTime.Now)
            {
                switch (this.ScheduleFrequencyId)
                {
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.Always:
                        return true;
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.DailyAllDays:
                        return
                            (
                                LastRunDate == null || LastRunDate < DateTime.Now.Date
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.DailyWeekdays:
                        return
                            (
                                DateTime.Now.Date.IsWeekday() &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.DailyWeekends:
                        return
                            (
                                !DateTime.Now.Date.IsWeekday() &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyMon:
                        return
                            (
                                DateTime.Now.DayOfWeek == DayOfWeek.Monday &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyTue:
                        return
                            (
                                DateTime.Now.DayOfWeek == DayOfWeek.Tuesday &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyWed:
                        return
                            (
                                DateTime.Now.DayOfWeek == DayOfWeek.Wednesday &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyThur:
                        return
                            (
                                DateTime.Now.DayOfWeek == DayOfWeek.Thursday &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklyFri:
                        return
                            (
                                DateTime.Now.DayOfWeek == DayOfWeek.Friday &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklySat:
                        return
                            (
                                DateTime.Now.DayOfWeek == DayOfWeek.Saturday &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.WeeklySun:
                        return
                            (
                                DateTime.Now.DayOfWeek == DayOfWeek.Sunday &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.MonthlyFirstDay:
                        return
                            (
                                DateTime.Now.IsFirstOfMonth() &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.MonthlyLastDay:
                        return
                            (
                                DateTime.Now.IsLastOfMonth() &&
                                (LastRunDate == null || LastRunDate.Value.Date < DateTime.Now.Date)
                            );
                    case (int)EscalationsFrameworkScheduleFrequencyEnum.Never:
                        return false;
                }
            }
            return false;
        }

        #endregion

        #region [Private methods]

        private void LoadContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("ScheduleFrequency").IsLoaded)
            {
                ctx.Entry(this).Reference("ScheduleFrequency").Load();
            }
        }

        #endregion
    }
}
