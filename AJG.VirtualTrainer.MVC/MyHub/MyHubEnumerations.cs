using System.ComponentModel;

namespace AJG.VirtualTrainer.MVC.MyHub
{
    public enum ReportSystem
    {
        [Description("Printers")]
        Printers = 1,
        [Description("Mobile Phones")]
        MobilePhones = 2
    }

    public enum ReportScope
    {
        [Description("User")]
        User = 1,
        [Description("User Team (Same Level)")]
        UserTeamSameLevel = 2,
        [Description("User Team L1 (Directly Below)")]
        UserTeamLevel1 = 3,
        [Description("User All Subordinates (All people below)")]
        UserAllSubordinates = 4,
        [Description("Team Members")]
        TeamMembers = 5,
        [Description("Team Members + L1 below")]
        TeamMembersAndLevel1 = 6,
        [Description("Team Members + All people below")]
        TeamMembersAndAllBelow = 7,
        [Description("Top 10 Biggest Data Users")]
        Top10BiggestDataUsers = 8,
        [Description("Top 10 Biggest Spenders")]
        Top10BiggestSpenders = 9,
        [Description("Zero Use Users")]
        ZeroUseUsers = 10,
        [Description("Over Allowance Users")]
        OverAllowanceUsers = 11,
        [Description("Top 10")]
        PrinterTop10 = 12    
    }
    public enum Report
    {
        [Description("Pages and Costs Totals")]
        PrinterPagesAndCostsTotals = 1,     
        [Description("Calls Data and Costs Totals")]  
        MobilePhonesCallsDataAndCosts = 2,
        [Description("Cost Totals")]
        PrinterCostsTotals = 3,
        [Description("Colour Cost")]
        PrinterColourCost = 4,
        [Description("B & W Cost")]
        PrinterBandWCost = 5,
        [Description("Total Cost")]
        PrinterTotalCost = 6,
        [Description("Colour Pages")]
        PrinterColourCopies = 7,
        [Description("B & W Pages")]
        PrinterBandWCopies = 8,
        [Description("Total Pages")]
        PrinterTotalCopies = 9,
    }
    public enum Month
    {
        Jan = 1,
        Feb = 2,
        Mar = 3,
        Apr = 4,
        May = 5,
        Jun = 6,
        Jul = 7,
        Aug = 8,
        Sep = 9,
        Oct = 10,
        Nov = 11,
        Dec = 12
    }
}