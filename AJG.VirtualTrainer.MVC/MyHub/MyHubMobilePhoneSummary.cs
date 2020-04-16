using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC.MyHub
{
    public class MyHubMobilePhoneSummary : MyHubSystemSummaryBase
    {
        public MyHubMobilePhoneSummary(MyHubService myHubService, string monthFrom, string yearFrom, string monthTo, string yearTo) 
            : base(myHubService,  monthFrom, yearFrom, monthTo, yearTo) { }

        public override ChartDTO GetChartDataDto(RequestedReportInfo RequestedReportInfo, ADUserDTO user)
        {
            switch (RequestedReportInfo.Scope)
            {
                case ReportScope.User:
                    return GetMobilesGraphData(RequestedReportInfo, user.UserDetails.EmployeeID, "User Total Mobile Phone minutes, data and Cost");
                case ReportScope.UserTeamSameLevel:
                    return GetMobilesGraphData(RequestedReportInfo, user.GetUserTeammates_CommaSeperatedString(), "Team Mates Total Mobile Phone minutes, data and Cost");                
                case ReportScope.UserTeamLevel1:
                    return GetMobilesGraphData(RequestedReportInfo, string.Join(",", user.UserAndManageesSamAccountAndEmployeeIdList().Select(s => s.Value).ToList()), "Team (Level 1) Total Mobile Phone minutes, data and Cost");
                case ReportScope.UserAllSubordinates:
                    return GetMobilesGraphData(RequestedReportInfo, string.Join(",", user.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(s => s.Value).ToList()), "All Below Total Mobile Phone minutes, data and Cost");                    
                case ReportScope.TeamMembers:
                case ReportScope.TeamMembersAndLevel1:
                case ReportScope.TeamMembersAndAllBelow:
                    return GetTeamMembersPagesAndCostGraphData(RequestedReportInfo, user);               
                case ReportScope.Top10BiggestDataUsers:
                case ReportScope.Top10BiggestSpenders:
                    return GetTop10Biggest(RequestedReportInfo, user, string.Join(",", user.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(s => s.Value).ToList()));
                case ReportScope.OverAllowanceUsers:
                case ReportScope.ZeroUseUsers:
                    return Geta(RequestedReportInfo, user, string.Join(",", user.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(s => s.Value).ToList()));
            }
            throw new NotImplementedException(string.Format("Scope, {0}, Not Yet Implemented.", RequestedReportInfo.Scope));
        }
        private ChartDTO Geta(RequestedReportInfo RequestedReportInfo, ADUserDTO user, string userNames)
        {
            int includedMonthlyDataAllowanceMB = 5000;
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();

            var totalsByUser = MyHubService.GetMobilePhoneTotalsByUserId(userNames.Split(',').ToList(), this.ReportDates);

            var results = RequestedReportInfo.Scope == ReportScope.ZeroUseUsers
                ? totalsByUser.Where(s=>s.TotalDuration == 0 && s.TotalData < 1).ToList() 
                : totalsByUser.Where(o => o.TotalUsageCharges > 0 || o.TotalData > (includedMonthlyDataAllowanceMB * ReportDates.Count)).ToList();

            foreach(var result in results)
            {
                cds.Add(new ChartDTO.ChartDataSetDto(
                    new List<decimal>()
                    {
                        (decimal)result.TotalDuration,
                        (decimal)result.TotalData,
                        (decimal)result.TotalUsageCharges,
                        (decimal)result.LineRental,
                        (decimal)result.TotalCost
                    },
                    user.GetSpecifcUserFromHierachy(result.UserName).UserDetails.FullName, 
                    whichYAxis.left));
            }

            List<string> dataLabels = new List<string>()
            {
                "Minutes",
                "Data (MB)",
                "Additional Charges",
                "Total Line Rental",
                "Total Cost"
            };
                        
            chartType chartType = chartType.bar;
            var columnDataTypes = new List<ChartColumnDataType>()
            {
                ChartColumnDataType.none,
                ChartColumnDataType.none,
                ChartColumnDataType.financial,
                ChartColumnDataType.financial,
                ChartColumnDataType.financial
            };
            var chartTitle = RequestedReportInfo.Scope == ReportScope.ZeroUseUsers ? "Zero Use Users" : "Over Allowance Users";

            return new ChartDTO(chartType, dataLabels, cds.OrderBy(o=>o.label).ToList(), chartTitle, "Minutes and Data", "Cost (£)", "Month", true, false, columnDataTypes, false);
        }
        private ChartDTO GetTop10Biggest(RequestedReportInfo RequestedReportInfo, ADUserDTO user, string userNames)
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();

            var totalsByUser = MyHubService.GetMobilePhoneTotalsByUserId(userNames.Split(',').ToList(), this.ReportDates);

            var results = RequestedReportInfo.Scope == ReportScope.Top10BiggestDataUsers 
                ? totalsByUser.OrderByDescending(o => o.TotalData).Take(10).ToList() : totalsByUser.OrderByDescending(o => o.TotalCost).Take(10).ToList();

            List<decimal> totalDuration = results.Select(s => (decimal)s.TotalDuration).ToList();
            List<decimal> totalData = results.Select(s => (decimal)s.TotalData).ToList();
            List<decimal> totalUsageCharge = results.Select(s => (decimal)s.TotalUsageCharges).ToList();
            List<decimal> totalLineRental = results.Select(s => (decimal)s.LineRental).ToList();
            List<decimal> totalCost = results.Select(s => (decimal)s.TotalCost).ToList();

            cds.Add(new ChartDTO.ChartDataSetDto(totalDuration, "Minutes", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(totalData, "Data (MB)", whichYAxis.left, false, "normal", RequestedReportInfo.Scope == ReportScope.User));
            cds.Add(new ChartDTO.ChartDataSetDto(totalUsageCharge, "Additional Charges", whichYAxis.right, true, "bold"));
            cds.Add(new ChartDTO.ChartDataSetDto(totalLineRental, "Total Line Rental", whichYAxis.right, true));
            cds.Add(new ChartDTO.ChartDataSetDto(totalCost, "Total Cost", whichYAxis.right, true, "bold"));

            List<string> dataLabels = results.Select(dr => user.GetSpecifcUserFromHierachy(dr.UserName).UserDetails.FullName).ToList();

            //List<string> dataLabels = DateRange.Select(dr => string.Format("{0} {1}", dr.Key, dr.Value)).ToList();
            chartType chartType = chartType.bar;

            var chartTitle = RequestedReportInfo.Scope == ReportScope.Top10BiggestDataUsers ? "Top 10 Biggest Data Users" : "Top 10 Biggest Spenders";

            return new ChartDTO(chartType, dataLabels, cds, chartTitle, "Minutes and Data", "Cost (£)", "Month", true, true, null, true);
        }
        private ChartDTO GetTeamMembersPagesAndCostGraphData(RequestedReportInfo RequestedReportInfo, ADUserDTO user)
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();
            List<MyHierachyMobilePhoneSummaryDTO> costResults = new List<MyHierachyMobilePhoneSummaryDTO>();

            foreach (var managee in user.Managees)
            {
                MyHierachyMobilePhoneSummaryDTO usageDetails = new MyHierachyMobilePhoneSummaryDTO();

                switch (RequestedReportInfo.Scope)
                {
                    case ReportScope.TeamMembers:
                        usageDetails.User = MyHubService.GetMobilePhoneTotal(managee.UserDetails.EmployeeID.Split(',').ToList(), this.ReportDates);
                        break;
                    case ReportScope.TeamMembersAndLevel1:
                        usageDetails.Team = MyHubService.GetMobilePhoneTotal(managee.GetUserAndUserTeamEmployeeIdList(), this.ReportDates);
                        break;
                    case ReportScope.TeamMembersAndAllBelow:
                        usageDetails.AllPeopleBelowUser = MyHubService.GetMobilePhoneTotal(managee.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(s => s.Value).ToList(), this.ReportDates);                        
                        break;
                }
                usageDetails.User.UserName = string.IsNullOrEmpty(managee.UserDetails.SamAccountName) ? managee.UserDetails.EmployeeID : managee.UserDetails.SamAccountName;
                costResults.Add(usageDetails);
            }

            string chartTitle = string.Empty;

            List<decimal> TotalDuration = new List<decimal>();
            List<decimal> TotalData = new List<decimal>();
            List<decimal> TotalUsageCharge = new List<decimal>();
            List<decimal> TotalLineRental = new List<decimal>();
            List<decimal> TotalCost = new List<decimal>();

            List<string> dataLabels = new List<string>();

            foreach (var userGroup in costResults.OrderBy(o => o.User.UserName).GroupBy(a => a.User.UserName))
            {
                dataLabels.Add(userGroup.Key);
                // We have querried for each user for each month, now create totals for each user: e.g. add up 
                switch (RequestedReportInfo.Scope)
                {
                    case ReportScope.TeamMembers:
                        TotalDuration.Add(userGroup.Select(u => (decimal)u.User.TotalDuration).Sum());
                        TotalData.Add(userGroup.Select(u => (decimal)u.User.TotalData).Sum());
                        TotalUsageCharge.Add(userGroup.Select(u => (decimal)u.User.TotalUsageCharges).Sum());
                        TotalLineRental.Add(userGroup.Select(u => (decimal)u.User.LineRental).Sum());
                        TotalCost.Add(userGroup.Select(u => (decimal)u.User.TotalCost).Sum());
                        chartTitle = "Mobile Phone Summary for Team Members";
                        break;
                    case ReportScope.TeamMembersAndLevel1:
                        TotalDuration.Add(userGroup.Select(u => (decimal)u.Team.TotalDuration).Sum());
                        TotalData.Add(userGroup.Select(u => (decimal)u.Team.TotalData).Sum());
                        TotalUsageCharge.Add(userGroup.Select(u => (decimal)u.Team.TotalUsageCharges).Sum());
                        TotalLineRental.Add(userGroup.Select(u => (decimal)u.Team.LineRental).Sum());
                        TotalCost.Add(userGroup.Select(u => (decimal)u.Team.TotalCost).Sum());
                        chartTitle = "Mobile Phone Summary for Team Members and Their L1 Users";
                        break;
                    case ReportScope.TeamMembersAndAllBelow:
                        TotalDuration.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.TotalDuration).Sum());
                        TotalData.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.TotalData).Sum());
                        TotalUsageCharge.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.TotalUsageCharges).Sum());
                        TotalLineRental.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.LineRental).Sum());
                        TotalCost.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.TotalCost).Sum());
                        chartTitle = "Mobile Phone Summary for Team Members and all Their Subordinates";
                        break;
                }
            }

            cds.Add(new ChartDTO.ChartDataSetDto(TotalDuration, "Minutes", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(TotalData, "Data (MB)", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(TotalUsageCharge, "Additional Charges", whichYAxis.right, true, "bold"));
            cds.Add(new ChartDTO.ChartDataSetDto(TotalLineRental, "Total Line Rental", whichYAxis.right, true));
            cds.Add(new ChartDTO.ChartDataSetDto(TotalCost, "Total Cost", whichYAxis.right, true, "bold"));

            return new ChartDTO(chartType.bar, dataLabels, cds, chartTitle, "Minutes and Data", "Cost (£)", "Month", true, true, null, true);
        }

        private ChartDTO GetMobilesGraphData(RequestedReportInfo RequestedReportInfo, string userNames, string chartTitle)
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();       
            
            var results = MyHubService.GetMobilePhoneTotalByMonth(userNames.Split(',').ToList(), this.ReportDates);

            List<decimal> totalDuration = results.Select(s => (decimal)s.TotalDuration).ToList();
            List<decimal> totalData = results.Select(s => (decimal)s.TotalData).ToList();
            List<decimal> totalUsageCharge = results.Select(s => (decimal)s.TotalUsageCharges).ToList();
            List<decimal> totalLineRental = results.Select(s => (decimal)s.LineRental).ToList();
            List<decimal> totalCost = results.Select(s => (decimal)s.TotalCost).ToList();                    
              
            cds.Add(new ChartDTO.ChartDataSetDto(totalDuration, "Minutes", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(totalData, "Data (MB)", whichYAxis.left, false,"normal", RequestedReportInfo.Scope == ReportScope.User));
            cds.Add(new ChartDTO.ChartDataSetDto(totalUsageCharge, "Additional Charges", whichYAxis.right, true, "bold"));
            cds.Add(new ChartDTO.ChartDataSetDto(totalLineRental, "Total Line Rental", whichYAxis.right, true));
            cds.Add(new ChartDTO.ChartDataSetDto(totalCost, "Total Cost", whichYAxis.right, true, "bold"));

            List<string> dataLabels = DateRange.Select(dr => string.Format("{0} {1}", dr.Key, dr.Value)).ToList();
            chartType chartType = DateRange.Count() == 1 ? chartType.bar : chartType.line;

            return new ChartDTO(chartType, dataLabels, cds, chartTitle, "Minutes and Data", "Cost (£)", "Month", true, true, null, true);
        }        
    }
}