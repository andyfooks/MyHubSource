using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AJG.VirtualTrainer.MVC.MyHub
{
    public abstract class MyHubSystemSummaryBase
    {
        internal MyHubService MyHubService;
        internal List<KeyValuePair<string, string>> DateRange = new List<KeyValuePair<string, string>>();
        internal List<DateTime> ReportDates = new List<DateTime>();

        public MyHubSystemSummaryBase(MyHubService myHubService, string monthFrom, string yearFrom, string monthTo, string yearTo)
        {
            this.MyHubService = myHubService;
            GetDateRange(monthFrom, yearFrom, monthTo, yearTo);
        }

        public abstract ChartDTO GetChartDataDto(RequestedReportInfo RequestedReportInfo, ADUserDTO user);

        internal List<KeyValuePair<string, string>> GetDateRange(string monthFrom, string yearFrom, string monthTo, string yearTo)
        {
            List<KeyValuePair<string, string>> dateRange = new List<KeyValuePair<string, string>>();
            Month fromMonth = (Month)Enum.Parse(typeof(Month), monthFrom);
            DateTime fromDate = new DateTime((int)Int32.Parse(yearFrom), (int)fromMonth, 1);

            Month toMonth = (Month)Enum.Parse(typeof(Month), monthTo);
            DateTime toDate = new DateTime((int)Int32.Parse(yearTo), (int)toMonth, 1);

            while (fromDate <= toDate)
            {
                dateRange.Add(new KeyValuePair<string, string>(fromDate.ToString("MMM"), fromDate.Year.ToString()));
                ReportDates.Add(new DateTime(fromDate.Year, fromDate.Month, 1));
                fromDate = fromDate.AddMonths(1);
            }

            DateRange = dateRange;

            return dateRange;
        }
        internal DateTime GetDateFromMonthAndYear(string month, string year)
        {
            int yearInt = int.Parse(year);
            int monthInt = 0;
            switch (month.ToLower())
            {
                case "jan":
                    monthInt = 1;
                    break;
                case "feb":
                    monthInt = 2;
                    break;
                case "mar":
                    monthInt = 3;
                    break;
                case "apr":
                    monthInt = 4;
                    break;
                case "may":
                    monthInt = 5;
                    break;
                case "jun":
                    monthInt = 6;
                    break;
                case "jul":
                    monthInt = 7;
                    break;
                case "aug":
                    monthInt = 8;
                    break;
                case "sep":
                    monthInt = 9;
                    break;
                case "oct":
                    monthInt = 10;
                    break;
                case "nov":
                    monthInt = 11;
                    break;
                case "dec":
                    monthInt = 12;
                    break;
            }
            return new DateTime(yearInt, monthInt, 1).Date;
        }
    }
    public class RequestedReportInfo
    {
        public ReportSystem System { get; set; }
        public ReportScope Scope { get; set; }
        public Report Report { get; set; }
        public string ReportName { get; set; }
    }   
}