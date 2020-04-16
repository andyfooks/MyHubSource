using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.DTOs;
using AJG.VirtualTrainer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public enum TimeSheetAction
    {
        [Description("Save")]
        Save,
        [Description("Submit")]
        Submit,
        [Description("UnSubmit")]
        UnSubmit,
        [Description("Approve")]
        Approve,
        [Description("Save ss Template")]
        SaveAsTemplate,
        [Description("Delete Saved Template")]
        DeleteSavedTemplate,
        [Description("Get Latest DDL")]
        GetLatestDDL,
        [Description("Get DDL For Team")]
        GetDDLForTeam,
        [Description("Get Saved Template")]
        GetSavedTemplate
    }
    public class CascadingDropDownItem
    {
        public string Name { get; set; }
        public string ActivityInternalName { get; set; }
        public bool IsFreetext { get; set; }
        public bool DisplayFreeTextInputField { get; set; }
        public bool DisplayFreeTextInputFieldRequired { get; set; }
        public string UserInput { get; set; }
        public string DDLItemLevel { get; set; }        
        public string HighLevelTeamName { get; set; }
        public string ExpenditureType { get; set; }
        public string HighLevelActivityType { get; set; }
        
        public List<CascadingDropDownItem> CascadeItems { get; set; }

        public CascadingDropDownItem()
        {
            DDLItemLevel = string.Empty;
            ActivityInternalName = string.Empty;
            HighLevelActivityType = string.Empty;
            ExpenditureType = string.Empty;
            IsFreetext = false;
            HighLevelTeamName = string.Empty;
            UserInput = string.Empty;
            DisplayFreeTextInputField = false;
            DisplayFreeTextInputFieldRequired = false;
            CascadeItems = new List<CascadingDropDownItem>();
        }
    }
    public class TimeSheetRow
    {
        public CascadingDropDownItem ItActivity { get; set; }
        public string SelectedActivityName { get; set; }
        public string SelectedActivityFreeTextValue { get; set; }
        public CascadingDropDownItem WorkItemOrProjectTask { get; set; }
        public string WorkItemOrProjectTaskName { get; set; }
        public string SelectedWorkItemFreeTextValue { get; set; }
        public CascadingDropDownItem BusinessUnit { get; set; }
        public string BusinessUnitName { get; set; }
        public List<int> dataEntryFields { get; set; }
    }
    public class ActvityLogDate
    {
        public bool isWorkDay { get; set; }
        public bool isWeekend { get; set; }
        public bool isBankHoliday { get; set; }
        public DateTime DateTime { get; set; }
    }
    public class MonthTimeSheet
    {
        public bool LocationRequired { get; set; }
        public bool ApprovalRequired { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime? SubmittedTimeStamp { get; set; }
        public CascadingDropDownItem Team { get; set; }
        public string SelectedTeamName { get; set; }
        public string SubmittedBySamAccountName { get; set; }
        public DateTime TimeSheetMonthYear { get; set; }
        public string UserName { get; set; }
        public string UserSamAccount { get; set; }
        public string UserEmployeeId { get; set; }
        public List<ActvityLogDate> thisMonthsDates { get; set; }
        public List<TimeSheetRow> TimeSheetActviityLogRows { get; set; }
        public TimeSheetRow TimeSheetRowTemplate { get; set; }
        public List<CascadingDropDownItem> CascadingDropDownList { get; set; }
        public bool CascadingDropDownListOutOfDate { get; set; }
        public List<string> DayLocations { get; set; }
        public bool Approved { get; set; }
        public string approvedBySamAccountName { get; set; }
        public DateTime? ApprovedTimeStamp { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime? UnApprovedTimeStamp { get; set; }
        public DateTime? LastSavedTimeStamp { get; set; }
        public string LastSavedBy { get; set; }
        public bool TemplateAvailable { get; set; }
        public bool LatestDDLAppliedSinceLastSave { get; set; }

        #region [ Constructors ]  

        public MonthTimeSheet()
        {
            SetUpDefaultValues();
        }
        public MonthTimeSheet(DateTime timeSheetDate, ADUserDTO user, AdminService adminService, TimeSheetService timeSheetService)
        {
            SetUpDefaultValues();

            var latestCascadeDdl = timeSheetService.GetCascadingDropDownList(adminService);            
            var hasSavedTemplate = adminService.GetSystemConfigByGroupNameAndKey(SystemConfig.ConfigKeys.TechTimeSheetUserTemplate.ToString(), user.UserDetails.SamAccountName).Any();

            // See if there is one stored already for this month and Year.
            var monthData = timeSheetService.GetMonthTimeSheet(user, timeSheetDate);
            if (monthData == null)
                SetTimeSheetDataForMonth(user, timeSheetDate, latestCascadeDdl, hasSavedTemplate, timeSheetService, adminService);
            else
            {
                this.UserEmployeeId = monthData.UserEmployeeId;
                this.UserName = monthData.UserName;
                this.UserSamAccount = monthData.UserSamAccount;
                this.SelectedTeamName = monthData.SelectedTeamName;
                this.TimeSheetMonthYear = monthData.TimeSheetMonthYear;
                this.thisMonthsDates = monthData.thisMonthsDates;
                this.TimeSheetActviityLogRows = monthData.TimeSheetActviityLogRows;
                this.TimeSheetRowTemplate = monthData.TimeSheetRowTemplate;
                this.IsSubmitted = monthData.IsSubmitted;
                this.CascadingDropDownList = monthData.CascadingDropDownList;
                this.DayLocations = monthData.DayLocations;
                this.Approved = monthData.Approved;
                this.CascadingDropDownListOutOfDate = CascadeDDLOutOfDate(latestCascadeDdl, monthData.CascadingDropDownList);
                this.TemplateAvailable = hasSavedTemplate;
                this.thisMonthsDates = GetMonthDates(timeSheetDate, adminService, timeSheetService);
                this.SubmittedBySamAccountName = monthData.SubmittedBySamAccountName;
                this.SubmittedTimeStamp = monthData.SubmittedTimeStamp;
                this.UnApprovedBy = monthData.UnApprovedBy;
                this.UnApprovedTimeStamp = monthData.UnApprovedTimeStamp;
                this.approvedBySamAccountName = monthData.approvedBySamAccountName;
                this.ApprovedTimeStamp = monthData.ApprovedTimeStamp;
            }

            this.LocationRequired = timeSheetService.GetTimeSheetUserSetting(AdminAction.LocationRequired, adminService, user.UserDetails.SamAccountName);
            this.ApprovalRequired = timeSheetService.GetTimeSheetUserSetting(AdminAction.ApprovalRequired, adminService, user.UserDetails.SamAccountName);
        }
        private bool CascadeDDLOutOfDate(List<CascadingDropDownItem> one, List<CascadingDropDownItem> two)
        {
            var latestCascadeDdlString = Newtonsoft.Json.JsonConvert.SerializeObject(one);
            var monthDataDdlString = Newtonsoft.Json.JsonConvert.SerializeObject(two);
            return string.CompareOrdinal(latestCascadeDdlString, monthDataDdlString) != 0;
        }
        private List<ActvityLogDate> GetMonthDates(DateTime passedDate, AdminService adminService, TimeSheetService timeSheetService)
        {
            var bankHolidays = timeSheetService.GetBankHolidays(passedDate.Year.ToString(), adminService, false);
            var bhc = new UKBankHolidayCalculator(bankHolidays);
            var dates = Enumerable.Range(1, DateTime.DaysInMonth(passedDate.Year, passedDate.Month))
                .Select(day => new ActvityLogDate()
                {
                    isWorkDay = bhc.IsWorkingDay(new DateTime(passedDate.Year, passedDate.Month, day)),
                    isWeekend = bhc.IsWeekend(new DateTime(passedDate.Year, passedDate.Month, day)),
                    isBankHoliday = bhc.IsBankHoliday(new DateTime(passedDate.Year, passedDate.Month, day)),
                    DateTime = new DateTime(passedDate.Year, passedDate.Month, day)
                })
                .ToList();
            return dates;
        }

        private void SetTimeSheetDataForMonth(ADUserDTO user, DateTime passedDate, List<CascadingDropDownItem> latestCascadeDdl, bool hasSavedTemplate, TimeSheetService timeSheetService, AdminService adminService)
        {
            var dates = GetMonthDates(passedDate, adminService, timeSheetService);

            // Set up a template for entering Daily %
            var dateEntryFields = dates.Select(s => 0).ToList();
            var timeSheetRowTemplate = new TimeSheetRow() { ItActivity = null, WorkItemOrProjectTask = null, BusinessUnit = null, dataEntryFields = dateEntryFields };
            var timeSheetActvityLogRows = new List<TimeSheetRow>() { new TimeSheetRow() { ItActivity = null, WorkItemOrProjectTask = null, BusinessUnit = null, dataEntryFields = dateEntryFields } };

            this.UserEmployeeId = user.GetUniqueIdentifier();
            this.UserName = user.UserDetails.FullName;
            this.UserSamAccount = user.UserDetails.SamAccountName;
            this.Team = null;
            this.TimeSheetMonthYear = passedDate;
            this.thisMonthsDates = dates;
            this.TimeSheetActviityLogRows = timeSheetActvityLogRows;
            this.TimeSheetRowTemplate = timeSheetRowTemplate;
            this.IsSubmitted = false;
            this.CascadingDropDownList = latestCascadeDdl;
            this.DayLocations = dates.Select(s => string.Empty).ToList();
            this.Approved = false;
            this.TemplateAvailable = hasSavedTemplate;                        
        }

        #endregion

        #region [ Public Methods ]

        public string PerformTimeSheetAction(AdminService adminService, TimeSheetService timeSheetService, ADUserDTO contextUser, TimeSheetAction actionEnum)
        {
            populateTimeSheetDDL(adminService, timeSheetService);
            switch (actionEnum)
            {
                case TimeSheetAction.Save:                    
                    return this.SaveTimeSheet(timeSheetService, contextUser);
                case TimeSheetAction.Submit:
                    return this.SubmitTimeSheet(timeSheetService, contextUser);
                case TimeSheetAction.UnSubmit:
                    return this.UnsubmitTimeSheet(timeSheetService, contextUser);
                case TimeSheetAction.Approve:
                    return this.ApproveTimeSheet(timeSheetService, contextUser);
                case TimeSheetAction.SaveAsTemplate:
                    return this.SaveAsTemplate(adminService);
                case TimeSheetAction.DeleteSavedTemplate:
                    return this.DeleteSavedTemplate(adminService);
                case TimeSheetAction.GetLatestDDL:
                    return this.GetLatestDDL(timeSheetService, adminService);
                case TimeSheetAction.GetDDLForTeam:
                    return "";
                case TimeSheetAction.GetSavedTemplate:
                    return this.GetSavedTemplate(adminService);                
                default:
                    return string.Format("{0} in NOT Supported in this function", actionEnum.ToString());
            }
        }
        public MonthTimeSheet GetMonthTimeSheetForUI()
        {
            var thisClone = (MonthTimeSheet)this.MemberwiseClone();

            // Only include DDL for selected team, if one selected.
            foreach (var ddl in thisClone.CascadingDropDownList)
            {
                if (string.IsNullOrWhiteSpace(this.SelectedTeamName) || this.SelectedTeamName != ddl.Name)
                {
                    ddl.CascadeItems = null;
                }
            }
            return thisClone;
        }

        #endregion

        #region [ Private Methods ]

        private void SetUpDefaultValues()
        {
            CascadingDropDownListOutOfDate = false;
            IsSubmitted = false;
            Approved = false;
            TemplateAvailable = false;
            LatestDDLAppliedSinceLastSave = false;
        }
        private string GetSavedTemplate(AdminService adminService)
        {
            var template = adminService.GetSystemConfigByGroupNameAndKey(SystemConfig.ConfigKeys.TechTimeSheetUserTemplate.ToString(), this.UserSamAccount).FirstOrDefault();
            var mts = Newtonsoft.Json.JsonConvert.DeserializeObject<MonthTimeSheet>(template.value);

            this.SelectedTeamName = mts.SelectedTeamName;
            //this.TimeSheetActviityLogRows = new List<TimeSheetRow>();
            foreach (var timesheetrow in mts.TimeSheetActviityLogRows)
            {
                this.TimeSheetActviityLogRows.Add(new TimeSheetRow()
                {
                    SelectedActivityName = timesheetrow.SelectedActivityName,
                    BusinessUnitName = timesheetrow.BusinessUnitName,
                    SelectedActivityFreeTextValue = timesheetrow.SelectedActivityFreeTextValue,
                    WorkItemOrProjectTaskName = timesheetrow.WorkItemOrProjectTaskName,
                    dataEntryFields = this.TimeSheetRowTemplate.dataEntryFields,
                    BusinessUnit = timesheetrow.BusinessUnit,
                    ItActivity = timesheetrow.ItActivity,
                    SelectedWorkItemFreeTextValue = timesheetrow.SelectedWorkItemFreeTextValue,
                    WorkItemOrProjectTask = timesheetrow.WorkItemOrProjectTask
                });
            }
            return "Time Sheet Template Successfully Applied";
        }

        private void populateTimeSheetDDL(AdminService adminService, TimeSheetService timeSheetService)
        {
            var savedMonthData = timeSheetService.GetMonthTimeSheet(this.UserSamAccount, this.TimeSheetMonthYear);
            if (savedMonthData != null && !this.LatestDDLAppliedSinceLastSave)
            {
                this.CascadingDropDownList = savedMonthData.CascadingDropDownList;
            }
            else
            {
                this.CascadingDropDownList = timeSheetService.GetCascadingDropDownList(adminService);
            }
        }

        private string SaveTimeSheet(TimeSheetService service, ADUserDTO contextUser)
        {
            this.LastSavedBy = contextUser.UserDetails.SamAccountName;
            this.LastSavedTimeStamp = DateTime.Now;            
            service.SaveUpdateTimeSheetConfig(contextUser, this.TimeSheetMonthYear, this);
            return "Time Sheet Successfully Saved";
        }
        private string SubmitTimeSheet(TimeSheetService service, ADUserDTO contextUser)
        {
            this.IsSubmitted = true;
            this.SubmittedTimeStamp = DateTime.Now;
            this.SubmittedBySamAccountName = contextUser.UserDetails.SamAccountName;
            return string.Format("{0} and Submitted", this.SaveTimeSheet(service, contextUser));
        }
        private string UnsubmitTimeSheet(TimeSheetService service, ADUserDTO contextUser)
        {
            this.IsSubmitted = false;
            this.SubmittedTimeStamp = null;
            this.Approved = false;
            this.ApprovedTimeStamp = null;
            this.UnApprovedBy = contextUser.UserDetails.SamAccountName;
            this.UnApprovedTimeStamp = DateTime.Now;
            service.SaveUpdateTimeSheetConfig(contextUser, this.TimeSheetMonthYear, this);
            return "Time Sheet Successfully Un-Submitted";
        }
        private string ApproveTimeSheet(TimeSheetService service, ADUserDTO contextUser)
        {
            this.Approved = true;
            this.ApprovedTimeStamp = DateTime.Now;
            this.approvedBySamAccountName = contextUser.UserDetails.SamAccountName;
            service.SaveUpdateTimeSheetConfig(contextUser, this.TimeSheetMonthYear, this);
            return "Time Sheet Successfully Approved";
        }
        private string GetLatestDDL(TimeSheetService service, AdminService adminService)
        {
            this.CascadingDropDownList = service.GetCascadingDropDownList(adminService);
            this.CascadingDropDownListOutOfDate = false;
            this.LatestDDLAppliedSinceLastSave = true;
            return "Latest DDL Successfully Retrieved";
        }
        private string SaveAsTemplate(AdminService service)
        {
            var monthDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            service.SaveUpdateSystemConfig(SystemConfig.ConfigKeys.TechTimeSheetUserTemplate.ToString(), this.UserSamAccount, monthDataJson);
            this.TemplateAvailable = true;
            return "Time Sheet Successfully Saved as template";
        }
        private string DeleteSavedTemplate(AdminService service)
        {
            service.DeleteSystemConfig(SystemConfig.ConfigKeys.TechTimeSheetUserTemplate.ToString(), this.UserSamAccount);
            this.TemplateAvailable = false;
            return "Time Sheet template successfully Deleted";
        }

        #endregion        
    }
}
