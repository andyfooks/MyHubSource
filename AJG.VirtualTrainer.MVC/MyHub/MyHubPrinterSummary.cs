using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.General;
using AJG.VirtualTrainer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC.MyHub
{
    public class MyHubPrinterSummary : MyHubSystemSummaryBase
    {
        public MyHubPrinterSummary(MyHubService myHubService, string monthFrom, string yearFrom, string monthTo, string yearTo) : base(myHubService, monthFrom, yearFrom, monthTo, yearTo) { }

        public override ChartDTO GetChartDataDto(RequestedReportInfo RequestedReportInfo, ADUserDTO user)
        {
            switch (RequestedReportInfo.Scope)
            {
                case ReportScope.User:
                    return GetPrinterGraphData(RequestedReportInfo, user.UserDetails.SamAccountName, string.Format("User Total Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report)));
                case ReportScope.UserTeamSameLevel:
                    string teamamtesNames = user.GetUserTeammates_CommaSeperatedString();
                    return GetPrinterGraphData(RequestedReportInfo, teamamtesNames, string.Format("Team Mates Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report)));
                case ReportScope.UserTeamLevel1:
                    string teammemberNames = string.Join(",", user.UserAndManageesSamAccountAndEmployeeIdList().Select(s => s.Key).ToList());
                    return GetPrinterGraphData(RequestedReportInfo, teammemberNames, string.Format("Team (Level 1) Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report)));
                case ReportScope.UserAllSubordinates:
                    string subordinateNames = string.Join(",", user.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(s => s.Key).ToList());
                    return GetPrinterGraphData(RequestedReportInfo, subordinateNames, string.Format("All Below Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report)));
                case ReportScope.TeamMembers:
                case ReportScope.TeamMembersAndLevel1:
                case ReportScope.TeamMembersAndAllBelow:
                    return GetTeamMembersPagesAndCostGraphData(RequestedReportInfo, user);
                case ReportScope.PrinterTop10:
                    string subNames = string.Join(",", user.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(s => s.Key).ToList());
                    return GetTop10(RequestedReportInfo, user, subNames);
            }

            throw new NotImplementedException(string.Format("Scope, {0}, Not Yet Implemented.", RequestedReportInfo.Scope));
        }
        private ChartDTO GetTop10(RequestedReportInfo RequestedReportInfo, ADUserDTO user, string userNames)
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();

            var totalsByUser = MyHubService.GetPrinterTotalsByUserId(userNames.Split(',').ToList(), this.ReportDates);

            List<MyHubPrinterSummaryDTO> results = null;

            switch (RequestedReportInfo.Report)
            {
                case Report.PrinterBandWCopies:
                    results = totalsByUser.OrderByDescending(o => o.BWPagesPrinted).Take(10).ToList();
                    break;
                case Report.PrinterColourCopies:
                    results = totalsByUser.OrderByDescending(o => o.ColourPagesPrinted).Take(10).ToList();
                    break;
                case Report.PrinterTotalCopies:
                    results = totalsByUser.OrderByDescending(o => o.TotalPages).Take(10).ToList();
                    break;
                case Report.PrinterBandWCost:
                    results = totalsByUser.OrderByDescending(o => o.BWTotalCost).Take(10).ToList();
                    break;
                case Report.PrinterColourCost:
                    results = totalsByUser.OrderByDescending(o => o.ColourTotalCost).Take(10).ToList();
                    break;
                case Report.PrinterTotalCost:
                    results = totalsByUser.OrderByDescending(o => o.TotalCost).Take(10).ToList();
                    break;
            }
            
            var dataLabels = results.Select(s => s.UserName).ToList();            
            string chartTitle = chartTitle = string.Format("Top 10 {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report));
            ChartDTO chartDto = null;        

            switch (RequestedReportInfo.Report)
            {
                case Report.PrinterBandWCopies:
                case Report.PrinterColourCopies:
                case Report.PrinterTotalCopies:
                    List<decimal> bwpagesPrinted = results.Select(u => (decimal)u.BWPagesPrinted).ToList();
                    List<decimal> colourPagesPrinted = results.Select(u => (decimal)u.ColourPagesPrinted).ToList();
                    List<decimal> TotalPages = results.Select(u => (decimal)u.TotalPages).ToList();                    
                    cds.Add(new ChartDTO.ChartDataSetDto(bwpagesPrinted, "B&W pages", whichYAxis.left, false, RequestedReportInfo.Report == Report.PrinterBandWCopies ? "bold" : "normal"));
                    cds.Add(new ChartDTO.ChartDataSetDto(colourPagesPrinted, "Colour pages", whichYAxis.left, false, RequestedReportInfo.Report == Report.PrinterColourCopies ? "bold" : "normal"));
                    cds.Add(new ChartDTO.ChartDataSetDto(TotalPages, "Total Pages", whichYAxis.left, false, RequestedReportInfo.Report == Report.PrinterTotalCopies ? "bold" : "normal"));
                    chartDto = new ChartDTO(chartType.bar, dataLabels, cds, chartTitle, "Pages", "Cost (£)", "Month", true, true, null, true);
                    break;
                case Report.PrinterBandWCost:
                case Report.PrinterColourCost:
                case Report.PrinterTotalCost:
                    List<decimal> bwpagesCost = results.Select(s => s.BWTotalCost).ToList();
                    List<decimal> colourPagesCost = results.Select(s => s.ColourTotalCost).ToList();
                    List<decimal> TotalCost = results.Select(s => s.TotalCost).ToList();
                    cds.Add(new ChartDTO.ChartDataSetDto(bwpagesCost, "B&W Cost", whichYAxis.left, true, RequestedReportInfo.Report == Report.PrinterBandWCost ? "bold" : "normal"));
                    cds.Add(new ChartDTO.ChartDataSetDto(colourPagesCost, "Colour Cost", whichYAxis.left, true, RequestedReportInfo.Report == Report.PrinterColourCost ? "bold" : "normal"));
                    cds.Add(new ChartDTO.ChartDataSetDto(TotalCost, "Total Cost", whichYAxis.left, true, RequestedReportInfo.Report == Report.PrinterTotalCost ? "bold" : "normal"));
                    chartDto = new ChartDTO(chartType.bar, dataLabels, cds, chartTitle, "Cost (£)", "", "Month", true, true, null, true);
                    break;
            }
            return chartDto;
        }
        private ChartDTO GetTeamMembersPagesAndCostGraphData(RequestedReportInfo RequestedReportInfo, ADUserDTO user)
        {
            List<MyHierachyPrinterSummaryDTO> costResults = new List<MyHierachyPrinterSummaryDTO>();

            int BWCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_BlackandWhite));
            int ColourCostPerPage = (int)Int32.Parse(ConfigurationHelper.Get(AppSettingsList.PrintCostPerPage_Colour));

            foreach (var managee in user.Managees)
            {
                MyHierachyPrinterSummaryDTO usageDetails = new MyHierachyPrinterSummaryDTO();
                
                switch (RequestedReportInfo.Scope)
                {
                    case ReportScope.TeamMembers:
                        usageDetails.User = MyHubService.GetPrinterTotal(managee.UserDetails.SamAccountName.Split(',').ToList(), this.ReportDates);
                        usageDetails.User.SetPrinterCostPerPage(BWCostPerPage, ColourCostPerPage);
                        break;
                    case ReportScope.TeamMembersAndLevel1:
                        usageDetails.Team = MyHubService.GetPrinterTotal(managee.GetUserAndUserTeam_CommaSeperatedString().Split(',').ToList(), this.ReportDates);
                        usageDetails.Team.SetPrinterCostPerPage(BWCostPerPage, ColourCostPerPage);
                        break;
                    case ReportScope.TeamMembersAndAllBelow:
                        usageDetails.AllPeopleBelowUser = MyHubService.GetPrinterTotal(managee.GetUserAndUserSubordinates_CommaSeperatedString().Split(',').ToList(), this.ReportDates);
                        usageDetails.AllPeopleBelowUser.SetPrinterCostPerPage(BWCostPerPage, ColourCostPerPage);
                        break;
                }
                usageDetails.User.UserName = string.IsNullOrEmpty(managee.UserDetails.SamAccountName) ? managee.UserDetails.EmployeeID : managee.UserDetails.SamAccountName;
                costResults.Add(usageDetails);
            }
            switch(RequestedReportInfo.Report)
            {
                case Report.PrinterPagesAndCostsTotals:
                    return GetTeamMembersPagesAndCostGraphData(costResults, RequestedReportInfo);                    
                case Report.PrinterCostsTotals:
                    return GetTeamMembersCostGraphData(costResults, RequestedReportInfo);
            }
            return new ChartDTO(chartType.bar, new List<string>(), new List<ChartDTO.ChartDataSetDto>(), "Report Type Not Recognised");
        }

        private ChartDTO GetTeamMembersPagesAndCostGraphData(List<MyHierachyPrinterSummaryDTO> printerSummaryDTOItems, RequestedReportInfo RequestedReportInfo)
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();

            string chartTitle = string.Empty;
            // Labels are the names of the grouped costresults below
            List<string> dataLabels = new List<string>();
            List<decimal> bwpagesPrinted = new List<decimal>();
            List<decimal> colourPagesPrinted = new List<decimal>();
            List<decimal> TotlaCost = new List<decimal>();

            foreach (var userGroup in printerSummaryDTOItems.OrderBy(o => o.User.UserName).GroupBy(a => a.User.UserName))
            {
                dataLabels.Add(userGroup.Key);
                // We have querried for each user for each month, now create totals for each user: e.g. add up 
                switch (RequestedReportInfo.Scope)
                {
                    case ReportScope.TeamMembers:
                        bwpagesPrinted.Add(userGroup.Select(u => (decimal)u.User.BWPagesPrinted).Sum());
                        colourPagesPrinted.Add(userGroup.Select(u => (decimal)u.User.ColourPagesPrinted).Sum());
                        TotlaCost.Add(userGroup.Select(u => (decimal)u.User.TotalCost).Sum());
                        chartTitle = string.Format("Team Members Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report));
                        break;
                    case ReportScope.TeamMembersAndLevel1:
                        bwpagesPrinted.Add(userGroup.Select(u => (decimal)u.Team.BWPagesPrinted).Sum());
                        colourPagesPrinted.Add(userGroup.Select(u => (decimal)u.Team.ColourPagesPrinted).Sum());
                        TotlaCost.Add(userGroup.Select(u => (decimal)u.Team.TotalCost).Sum());
                        chartTitle = string.Format("Team Members and Their L1 Users Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report));
                        break;
                    case ReportScope.TeamMembersAndAllBelow:
                        bwpagesPrinted.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.BWPagesPrinted).Sum());
                        colourPagesPrinted.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.ColourPagesPrinted).Sum());
                        TotlaCost.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.TotalCost).Sum());
                        chartTitle = string.Format("Team Members and all Their Subordinates Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report));
                        break;
                }
            }

            cds.Add(new ChartDTO.ChartDataSetDto(bwpagesPrinted, "B&W pages", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(colourPagesPrinted, "Colour pages", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(TotlaCost, "Total Cost", whichYAxis.right, true, "bold"));

            return new ChartDTO(chartType.bar, dataLabels, cds, chartTitle, "Pages", "Cost (£)", "Month", true, true, null, true);
        }
        private ChartDTO GetTeamMembersCostGraphData(List<MyHierachyPrinterSummaryDTO> printerSummaryDTOItems, RequestedReportInfo RequestedReportInfo)
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();

            string chartTitle = string.Empty;
            // Labels are the names of the grouped costresults below
            List<string> dataLabels = new List<string>();
            List<decimal> bwpagesCost = new List<decimal>();
            List<decimal> colourPagesCost = new List<decimal>();
            List<decimal> TotlaCost = new List<decimal>();

            foreach (var userGroup in printerSummaryDTOItems.OrderBy(o => o.User.UserName).GroupBy(a => a.User.UserName))
            {
                dataLabels.Add(userGroup.Key);
                // We have querried for each user for each month, now create totals for each user: e.g. add up 
                switch (RequestedReportInfo.Scope)
                {
                    case ReportScope.TeamMembers:
                        bwpagesCost.Add(userGroup.Select(u => u.User.BWTotalCost).Sum());
                        colourPagesCost.Add(userGroup.Select(u => u.User.ColourTotalCost).Sum());
                        TotlaCost.Add(userGroup.Select(u => (decimal)u.User.TotalCost).Sum());
                        chartTitle = string.Format("Team Members Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report));
                        break;
                    case ReportScope.TeamMembersAndLevel1:
                        bwpagesCost.Add(userGroup.Select(u => u.Team.BWTotalCost).Sum());
                        colourPagesCost.Add(userGroup.Select(u => u.Team.ColourTotalCost).Sum());
                        TotlaCost.Add(userGroup.Select(u => (decimal)u.Team.TotalCost).Sum());
                        chartTitle = string.Format("Team Members and Their L1 Users Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report));
                        break;
                    case ReportScope.TeamMembersAndAllBelow:
                        bwpagesCost.Add(userGroup.Select(u => u.AllPeopleBelowUser.BWTotalCost).Sum());
                        colourPagesCost.Add(userGroup.Select(u => u.AllPeopleBelowUser.ColourTotalCost).Sum());
                        TotlaCost.Add(userGroup.Select(u => (decimal)u.AllPeopleBelowUser.TotalCost).Sum());
                        chartTitle = string.Format("Team Members and all Their Subordinates Printer {0}", GeneralHelper.GetEnumDescription((Report)RequestedReportInfo.Report));
                        break;
                }
            }

            cds.Add(new ChartDTO.ChartDataSetDto(bwpagesCost, "B&W Cost", whichYAxis.left, true));
            cds.Add(new ChartDTO.ChartDataSetDto(colourPagesCost, "Colour Cost", whichYAxis.left, true));
            cds.Add(new ChartDTO.ChartDataSetDto(TotlaCost, "Total Cost", whichYAxis.left, true, "bold"));

            return new ChartDTO(chartType.bar, dataLabels, cds, chartTitle, "Cost (£)", "", "Month", true, true, null, true);
        }

        private ChartDTO GetPrinterGraphData(RequestedReportInfo RequestedReportInfo, string userNames, string chartTitle)
        {            
            var results = MyHubService.GetPrinterTotalByMonth(userNames.Split(',').ToList(), this.ReportDates);

            switch(RequestedReportInfo.Report)
            {
                case Report.PrinterPagesAndCostsTotals:
                    return GetPrinterGraphDataForPrinterPagesAndCostsTotals(results, chartTitle);
                case Report.PrinterCostsTotals:
                    return GetPrinterGraphDataForPrinterCostsTotals(results, chartTitle);                
            }

            return new ChartDTO(chartType.bar, new List<string>(), new List<ChartDTO.ChartDataSetDto>(), "Report Type Not Recognised");
        }
        private ChartDTO GetPrinterGraphDataForPrinterPagesAndCostsTotals(List<MyHubPrinterSummaryDTO> data, string chartTitle)        
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();

            List<decimal> bwpagesPrinted = data.Select(s => (decimal)s.BWPagesPrinted).ToList();
            List<decimal> colourPagesPrinted = data.Select(s => (decimal)s.ColourPagesPrinted).ToList();
            List<decimal> TotlaCost = data.Select(s => s.TotalCost).ToList();

            cds.Add(new ChartDTO.ChartDataSetDto(bwpagesPrinted, "B&W pages", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(colourPagesPrinted, "Colour pages", whichYAxis.left));
            cds.Add(new ChartDTO.ChartDataSetDto(TotlaCost, "Total Cost", whichYAxis.right, true, "bold"));

            List<string> dataLabels = DateRange.Select(dr => string.Format("{0} {1}", dr.Key, dr.Value)).ToList();
            chartType chartType = DateRange.Count() == 1 ? chartType.bar : chartType.line;

            return new ChartDTO(chartType, dataLabels, cds, chartTitle, "Pages", "Cost (£)", "Month", true, true, null, true);
        }
        private ChartDTO GetPrinterGraphDataForPrinterCostsTotals(List<MyHubPrinterSummaryDTO> data, string chartTitle)
        {
            List<ChartDTO.ChartDataSetDto> cds = new List<ChartDTO.ChartDataSetDto>();

            List<decimal> bwpagesCost = data.Select(s => s.BWTotalCost).ToList();
            List<decimal> colourPagesCost = data.Select(s => s.ColourTotalCost).ToList();
            List<decimal> TotalCost = data.Select(s => s.TotalCost).ToList();

            cds.Add(new ChartDTO.ChartDataSetDto(bwpagesCost, "B&W Cost", whichYAxis.left, true));
            cds.Add(new ChartDTO.ChartDataSetDto(colourPagesCost, "Colour Cost", whichYAxis.left, true));
            cds.Add(new ChartDTO.ChartDataSetDto(TotalCost, "Total Cost", whichYAxis.left, true, "bold"));

            List<string> dataLabels = DateRange.Select(dr => string.Format("{0} {1}", dr.Key, dr.Value)).ToList();
            chartType chartType = DateRange.Count() == 1 ? chartType.bar : chartType.line;

            return new ChartDTO(chartType, dataLabels, cds, chartTitle, "Cost (£)", "", "Month", true, true, null, true);
        }
    }
}