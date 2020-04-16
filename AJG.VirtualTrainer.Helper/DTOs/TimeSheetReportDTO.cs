using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AJG.VirtualTrainer.Helper.DTOs
{
    public class TimeSheetReport
    {
        public string UserId { get; set; }
        public TimeSheetRuleTarget ReportTarget { get; set; }
        public bool TargetSpecificUser { get; set; }
        public int YearFrom { get; set; }
        public int MonthFrom { get; set; }
        public int YearTo { get; set; }
        public int MonthTo { get; set; }
        public List<KeyValuePair<string, int>> YearsDropDown { get; set; }
        public List<KeyValuePair<string, int>> MonthsDropDown { get; set; }
        public List<KeyValuePair<string, TimeSheetRuleTarget>> ReportTargetDropDown { get; set; }
        public TimeSheetReport() { }
        public TimeSheetReport(bool configure = false)
        {
            if (configure)
            {
                var DT = DateTime.Now;

                TargetSpecificUser = false;
                UserId = string.Empty;
                YearsDropDown = GetReportingYears();
                MonthsDropDown = GetReportingMonths();
                MonthFrom = DT.Month;
                MonthTo = DT.Month;
                YearFrom = DT.Year;
                YearTo = DT.Year;
                ReportTarget = TimeSheetRuleTarget.User;
                ReportTargetDropDown = new List<KeyValuePair<string, TimeSheetRuleTarget>>
                {
                    new KeyValuePair<string, TimeSheetRuleTarget> ("User", TimeSheetRuleTarget.User),
                    new KeyValuePair<string, TimeSheetRuleTarget> ("Team", TimeSheetRuleTarget.Team),
                    new KeyValuePair<string, TimeSheetRuleTarget> ("All Subordinates", TimeSheetRuleTarget.AllSubordinates)
                };
            }
        }
        public List<KeyValuePair<string, int>> GetReportingMonths()
        {
            var dt = new DateTime(2000, 1, 1);
            var months = new List<KeyValuePair<string, int>>()
            {
                new KeyValuePair<string, int>(dt.ToString("MMM"), dt.Month)
            };
            for (int i = 1; i < 12; i++)
            {
                months.Add(new KeyValuePair<string, int>(dt.AddMonths(i).ToString("MMM"), dt.AddMonths(i).Month));
            }
            return months;
        }

        public List<KeyValuePair<string, int>> GetReportingYears()
        {
            var returnYears = new List<KeyValuePair<string, int>>() { new KeyValuePair<string, int>("2018", 2018) };
            var todaysYear = DateTime.Now.Year;
            for (int i = returnYears[0].Value + 1; i <= todaysYear; i++)
            {
                returnYears.Add(new KeyValuePair<string, int>(i.ToString(), i));
            }
            return returnYears;
        }
    }
}
