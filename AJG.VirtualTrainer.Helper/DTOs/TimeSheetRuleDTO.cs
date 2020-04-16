using System.ComponentModel;

namespace AJG.VirtualTrainer.Helper.DTOs
{
    public enum TimeSheetRuleTarget
    {
        [Description("User")]
        User,
        [Description("Team")]
        Team,
        [Description("All Subordinates")]
        AllSubordinates
    }
    public enum TimeSheetRulePermission
    {
        [Description("Allow")]
        Allow,
        [Description("Disallow")]
        Disallow
    }
    public enum AdminAction
    {
        [Description("Location Required")]
        LocationRequired,
        [Description("Approval Required")]
        ApprovalRequired,
        [Description("View Status and Send Emails")]
        SendEmail,
        [Description("Save To Reporting DB")]
        SaveToDB,
        [Description("Can Access Time Sheet")]
        TechTimeSheetAccess,
        [Description("Configure Drop Down Lists")]
        ConfigureDropDownLists,
        [Description("Configure Bank Holidays")]
        ConfigureBankHolidays,
        [Description("Run Data Extract from Reporting DB")]
        RunReports
    }
    public class TimeSheetRuleDTO
    {
        public TimeSheetRuleTarget Target { get; set; }
        public bool Enabled { get; set; }
        public TimeSheetRulePermission Permission { get; set; }
        public string UserID { get; set; }
    }
}
