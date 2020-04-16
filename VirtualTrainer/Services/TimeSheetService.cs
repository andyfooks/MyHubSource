using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualTrainer;
using VirtualTrainer.Interfaces;
using VirtualTrainer.Models.MyHub;

namespace AJG.VirtualTrainer.Services
{
    public class TimeSheetService : BaseService, IDisposable
    {
        #region [ private vars ]

        private string timesheetGroupNamepostfix = "TimeSheet";
        private string emailTemplateGroupName = "TimeSheetEmailTemplates";

        #endregion

        #region [ Constructors ]

        public TimeSheetService() : base()
        {
        }

        public TimeSheetService(IUnitOfWork uow) : base(uow)
        {
        }

        #endregion

        #region [ UK Holiday ]

        private string BankHolidayYearSavedConfigName = "BankHolidayYearSavedConfig";

        public List<Double> GetBankHolidaysJSFriendly(string year, AdminService adminService, bool systemGenerated = true)
        {                     
            return MakeDatesJSFriendly(GetBankHolidays( year, adminService, systemGenerated));
        }
        public List<DateTime> GetBankHolidays(string year, AdminService adminService, bool systemGenerated = true)
        {
            var date = DateTime.Parse(string.Format("1/1/{0}", year));
            var systemGeneratedDates = new UKBankHolidayCalculator(date);
            // if not system generated then we should check to see if there is a config stored
            if (systemGenerated == false)
            {
                if (adminService.GetSystemConfigByGroupNameAndKey(BankHolidayYearSavedConfigName, year).Any())
                {
                    var datesString = adminService.GetSystemConfigByGroupNameAndKey(BankHolidayYearSavedConfigName, year)[0].value;
                    var dates = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(datesString);
                    return dates;
                }
            }
            return systemGeneratedDates.GetBankHolidays();
        }
        public List<Double> MakeDatesJSFriendly(List<DateTime> dates)
        {
            var returnDates = dates.Select(a => a.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).OrderBy(a => a).ToList();
            return returnDates;
        }
        public void SaveUpdateBankHolidays(List<DateTime> dates, string year, AdminService adminService)
        {
            if(dates != null && dates.Count > 0)
            {
                adminService.SaveUpdateSystemConfig(BankHolidayYearSavedConfigName, year, Newtonsoft.Json.JsonConvert.SerializeObject(dates));
            }
        }

        #endregion        

        public List<CascadingDropDownItem> GetCascadingDropDownList(AdminService adminService)
        {
            if (adminService.HasSystemConfigByGroupName(SystemConfig.ConfigKeys.TechTimeSheetDropDownVals.ToString()))
            {
                var techTimeSheetDropDownVals = adminService.GetSystemConfigByGroupName(SystemConfig.ConfigKeys.TechTimeSheetDropDownVals.ToString()).FirstOrDefault();
                var cd = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CascadingDropDownItem>>(techTimeSheetDropDownVals.value);
                return cd;
            }
            else
            {
                var dropDownDetails = new List<CascadingDropDownItem>
                {
                    new CascadingDropDownItem() {Name = "New", CascadeItems = new List<CascadingDropDownItem>()}
                };
                var seralizedObject = Newtonsoft.Json.JsonConvert.SerializeObject(dropDownDetails);
                adminService.SaveUpdateSystemConfig(SystemConfig.ConfigKeys.TechTimeSheetDropDownVals.ToString(), SystemConfig.ConfigKeys.TechTimeSheetDropDownVals.ToString(), seralizedObject);
                return dropDownDetails;
            }
        }

        public DateTime GetDateFromMonthAndYear(string month, string year)
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

        public List<WorkSheetActivity> GetTimeSheetReportData(bool TargetSpecificUser, List<string> userEmployeeIds, DateTime From, DateTime To)
        {
            if(TargetSpecificUser)
            {
               return _unitOfWork.Context.WorkSheetMonthActivity.Where(s => userEmployeeIds.Contains(s.EmployeeID) && s.Date >= From && s.Date <= To).ToList();
            }
            else
            {
                return _unitOfWork.Context.WorkSheetMonthActivity.Where(s => s.Date >= From && s.Date <= To).ToList();
            }
        }
        public List<string> SaveTimeSheetsToReportingDB(DateTime date, List<string> userNames, AdminService AdminService)
        {
            List<string> savedUserNames = new List<string>();
            List<WorkSheetActivity> workSheets = new List<WorkSheetActivity>();
            DeleteWorkSheetMonthActivties(date, userNames);

            var bankHolidayWorksheetDetails = new {
                ItActivityType = "General",
                WorkItem = "General - Bank Holiday",
                HighLevelITActivityType  = "General",
                ExpenditureType = "OpEx",
                BusinessUnit = "IT Central",
                Location = "",
                ItActivity = 100
            };

            var routeADUser = AdminService.GetRoutADUserFromDB();

            foreach (string userName in userNames)
            {
                var processedDate = DateTime.Now;
                // Get the root USer from the config in the db.               
                var adUser = routeADUser.GetSpecifcUserFromHierachy(userName);
                //DeleteWorkSheetMonthActivties(date, adUser.UserDetails.EmployeeID);

                var monthData = GetMonthTimeSheet(adUser, date);

                // Add Month Data row for any bank holidays                
                if (monthData != null && monthData.IsSubmitted)
                {
                    savedUserNames.Add(adUser.UserDetails.SamAccountName);
                    var selectedTeamDD = monthData.CascadingDropDownList.Where(s => s.Name == monthData.SelectedTeamName).FirstOrDefault();
                    foreach (var d in monthData.thisMonthsDates)
                    {
                        if(d.isBankHoliday)
                        {
                            var wsma = new WorkSheetActivity()
                            {
                                EmployeeName = adUser.UserDisplayName,
                                EmployeeID = adUser.UserDetails.EmployeeID,
                                YearInt = date.Year,
                                MonthInt = date.Month,
                                Date = d.DateTime,
                                DateProcessed = processedDate,
                                Month = d.DateTime,
                                Team = selectedTeamDD.Name,
                                HighLevelTeam = selectedTeamDD.HighLevelTeamName,
                                ITActivityType = bankHolidayWorksheetDetails.ItActivityType,
                                HighLevelITActivityType = bankHolidayWorksheetDetails.HighLevelITActivityType,
                                ChangeStack = string.Empty,
                                ITActivity = bankHolidayWorksheetDetails.ItActivity,
                                WorkItem_ProjectTaskType = bankHolidayWorksheetDetails.WorkItem,
                                ExpenditureType = bankHolidayWorksheetDetails.ExpenditureType,
                                BusinessUnit = bankHolidayWorksheetDetails.BusinessUnit,
                                Location = bankHolidayWorksheetDetails.Location
                            };

                            workSheets.Add(wsma);
                        }
                    }

                    foreach (var md in monthData.TimeSheetActviityLogRows)
                    {
                        for (int i = 0; i < md.dataEntryFields.Count; i++)
                        {
                            if ((monthData.thisMonthsDates[i].isWorkDay && md.dataEntryFields[i] > 0))
                            {                                
                                var selectedActivity = selectedTeamDD.CascadeItems.Where(s => s.Name == md.SelectedActivityName).FirstOrDefault();
                                var selectedWorkItem = selectedActivity.CascadeItems.Where(s => s.Name == md.WorkItemOrProjectTaskName).FirstOrDefault();
                                var selecetdBusinessUnit = selectedWorkItem.CascadeItems.Where(s => s.Name == md.BusinessUnitName).FirstOrDefault();                               

                                // We automatically want to deal with Bank Holidays.
                                var wsma = new WorkSheetActivity()
                                {
                                    EmployeeName = adUser.UserDisplayName,
                                    EmployeeID = adUser.UserDetails.EmployeeID,
                                    YearInt = date.Year,
                                    MonthInt = date.Month,
                                    Date = monthData.thisMonthsDates[i].DateTime,
                                    DateProcessed = processedDate,
                                    Month = monthData.thisMonthsDates[i].DateTime,
                                    Team = selectedTeamDD.Name,
                                    HighLevelTeam = selectedTeamDD.HighLevelTeamName,
                                    ITActivityType = selectedActivity.ActivityInternalName,
                                    HighLevelITActivityType = selectedActivity.HighLevelActivityType,
                                    ChangeStack = selectedActivity.DisplayFreeTextInputFieldRequired ? md.SelectedActivityFreeTextValue : string.Empty,
                                    ITActivity = md.dataEntryFields[i],
                                    WorkItem_ProjectTaskType = (selectedWorkItem.IsFreetext ? md.SelectedWorkItemFreeTextValue : selectedWorkItem.Name),
                                    ExpenditureType = selectedWorkItem.ExpenditureType,
                                    BusinessUnit = selecetdBusinessUnit.Name,
                                    Location = monthData.DayLocations[i]
                                };
                                                                
                                workSheets.Add(wsma);                                                                                      
                                //SaveWorkSheetMonthActivties(wsma);
                            }
                        }                        
                    }
                }                
            }
            SaveWorkSheetMonthActivties(workSheets);
            return savedUserNames;
        }
        public bool GetTimeSheetUserSetting(AdminAction targetSetting, AdminService AdminService, string UserSamAccountName)
        {
            bool returnValue = false;
            var configObject = AdminService.GetSystemConfigByGroupNameAndKey(targetSetting.ToString(), targetSetting.ToString()).FirstOrDefault();

            if (configObject != null)
            {
                var et = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimeSheetRuleDTO>>(configObject.value).OrderBy(d => d.Enabled);
                foreach (var rule in et)
                {
                    if (rule.Enabled)
                    {
                        if (!string.IsNullOrEmpty(rule.UserID))
                        {
                            var user = AdminService.GetAdUserFromDB(rule.UserID);
                            switch (rule.Target)
                            {
                                case TimeSheetRuleTarget.User:
                                    returnValue = string.Compare(UserSamAccountName, rule.UserID, true) == 0 ? (rule.Permission == TimeSheetRulePermission.Allow ? true : false) : returnValue;
                                    break;
                                case TimeSheetRuleTarget.Team:
                                    returnValue = user.HasTeamMember(UserSamAccountName) ? (rule.Permission == TimeSheetRulePermission.Allow ? true : false) : returnValue;
                                    break;
                                case TimeSheetRuleTarget.AllSubordinates:
                                    returnValue = user.HasSubordinate(UserSamAccountName) ? (rule.Permission == TimeSheetRulePermission.Allow ? true : false) : returnValue;
                                    break;
                            };
                        }
                    }
                }
            }

            return returnValue;
        }
        public void SaveUpdateTimeSheetConfig(ADUserDTO User, DateTime date, MonthTimeSheet TimeSheetData)
        {
            TimeSheetData tsd = this.GetMonthTimeSheetData(TimeSheetData.UserSamAccount, date);

            if (tsd == null)
            {
                tsd = new TimeSheetData()
                {
                    UserName = TimeSheetData.UserName,
                    Year = date.Year,
                    Month = date.Month,
                    Approved = TimeSheetData.Approved,
                    ApprovedBy = TimeSheetData.approvedBySamAccountName,
                    EmployeeID = TimeSheetData.UserEmployeeId,
                    ApprovedTimeStamp = TimeSheetData.ApprovedTimeStamp,
                    LastSavedBy = TimeSheetData.LastSavedBy,
                    LastSavedTimeStamp = TimeSheetData.LastSavedTimeStamp,
                    SamAccountName = TimeSheetData.UserSamAccount,
                    Submitted = TimeSheetData.IsSubmitted,
                    SubmittedBy = TimeSheetData.SubmittedBySamAccountName,
                    SubmittedTimeStamp = TimeSheetData.SubmittedTimeStamp,
                    TimeSheetMonthYear = date,
                    TimeSheetTeamName = TimeSheetData.SelectedTeamName,
                    TimeSheetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(TimeSheetData),
                    UnApprovedBy = TimeSheetData.UnApprovedBy,
                    UnApprovedTimeStamp = TimeSheetData.UnApprovedTimeStamp,                    
                };
                _unitOfWork.Context.TimeSheetData.Add(tsd);
            }
            else
            {
                tsd.UserName = TimeSheetData.UserName;
                tsd.Year = date.Year;
                tsd.Month = date.Month;
                tsd.Approved = TimeSheetData.Approved;
                tsd.ApprovedBy = TimeSheetData.approvedBySamAccountName;
                tsd.EmployeeID = TimeSheetData.UserEmployeeId;
                tsd.ApprovedTimeStamp = TimeSheetData.ApprovedTimeStamp;
                tsd.LastSavedBy = TimeSheetData.LastSavedBy;
                tsd.LastSavedTimeStamp = TimeSheetData.LastSavedTimeStamp;
                tsd.SamAccountName = TimeSheetData.UserSamAccount;
                tsd.Submitted = TimeSheetData.IsSubmitted;
                tsd.SubmittedBy = TimeSheetData.SubmittedBySamAccountName;
                tsd.SubmittedTimeStamp = TimeSheetData.SubmittedTimeStamp;
                tsd.TimeSheetMonthYear = date;
                tsd.TimeSheetTeamName = TimeSheetData.SelectedTeamName;
                tsd.TimeSheetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(TimeSheetData);
                tsd.UnApprovedBy = TimeSheetData.UnApprovedBy;
                tsd.UnApprovedTimeStamp = TimeSheetData.UnApprovedTimeStamp;
                _unitOfWork.GetRepository<TimeSheetData>().Update(tsd, true);
            }

            _unitOfWork.Context.SaveChanges();
        }
        public enum Status
        {
            NotSubmitted,
            NotApproved,
            Submitted,
            Approved,
            All,
            NotSaved,
            Saved
        }
        public void DeleteWorkSheetMonthActivties( DateTime date, string EmployeeId )
        {
            _unitOfWork.GetRepository<WorkSheetActivity>().Delete(m => m.MonthInt == date.Month && m.YearInt == date.Year && m.EmployeeID == EmployeeId);            
            _unitOfWork.Commit();
        }
        public void DeleteWorkSheetMonthActivties(DateTime date, List<string> EmployeeId)
        {
            _unitOfWork.GetRepository<WorkSheetActivity>().Delete(m => m.MonthInt == date.Month && m.YearInt == date.Year && EmployeeId.Contains(m.EmployeeID));
            _unitOfWork.Commit();
        }
        public void SaveWorkSheetMonthActivties(WorkSheetActivity activity)
        {
            _unitOfWork.Context.WorkSheetMonthActivity.Add(activity);
            _unitOfWork.Commit();
        }
        public void SaveWorkSheetMonthActivties(List<WorkSheetActivity> activity)
        {
            _unitOfWork.Context.WorkSheetMonthActivity.AddRange(activity);
            _unitOfWork.Commit();
        }
        public List<TimeSheetData> GetMonthTimeSheetForUsers(List<ADUserDTO> users, DateTime date, Status status)
        {
            List<TimeSheetData> tsl = new List<TimeSheetData>();

            List<string> userSamAccountNames = users.Select(s => s.UserDetails.SamAccountName).ToList();

            // Compile objects for users that have created a time sheet
            var timeSheetData = _unitOfWork.GetRepository<TimeSheetData>()
               .GetAll()
               .OrderByDescending(o => o.Id)
               .Where(w => (userSamAccountNames.Contains(w.SamAccountName)) && (w.Month == date.Month && w.Year == date.Year)).ToList();

            var existingTimeSheets = from c in timeSheetData
                       join u in users on c.SamAccountName equals u.UserDetails.SamAccountName
                       select c.SetProperties(u.UserDetails.EmailAddress, u.ManagerDetails.EmailAddress, string.IsNullOrEmpty(u.UserDetails.EmailAddress) ? false : true);

            tsl.AddRange(existingTimeSheets);

            // Now need to create time sheet object for those that dont have one saved.
            var sss = timeSheetData.Select(s=>s.SamAccountName).ToList();
            var aaa = users.Where(u => !sss.Contains(u.UserDetails.SamAccountName)).Select(s => new TimeSheetData
            {
                SamAccountName = s.UserDetails.SamAccountName,
                UserName = s.UserDetails.FullName,
                EmployeeID = s.UserDetails.EmployeeID,
                Email = s.UserDetails.EmailAddress,
                ManagerEmail = s.ManagerDetails.EmailAddress,
                Include = !string.IsNullOrEmpty(s.UserDetails.EmailAddress),
                TimeSheetDataJson = ""
            }).ToList();
            tsl.AddRange(aaa);            

            switch (status)
            {
                case Status.NotSubmitted:
                    tsl = tsl.Where(w => w.Submitted == false).ToList();
                    break;
                case Status.NotApproved:
                    tsl = tsl.Where(w => w.Approved == false).ToList();
                    break;
                case Status.Approved:
                    tsl = tsl.Where(w => w.Approved).ToList();
                    break;
                case Status.Submitted:
                    tsl = tsl.Where(w => w.Submitted).ToList();
                    break;
                case Status.Saved:
                    tsl = tsl.Where(w => w.LastSavedBy != null).ToList();
                    break;
                case Status.NotSaved:
                    tsl = tsl.Where(w => w.LastSavedBy == null).ToList();
                    break;
            }

            return tsl;
        }
        public EmailTemplate AddUpdateEmailTemplate (EmailTemplate et, bool delete)
        {
            //var et = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailTemplate>(template);
            if(et.Id == 0)
            {
                // this is a new item
               _unitOfWork.Context.EmailTemplate.Add(et);
            }
            else
            {
                _unitOfWork.GetRepository<EmailTemplate>().GetAll().OrderByDescending(o => o.DisplayText).Where(w => w.Id == et.Id).FirstOrDefault();
                if (delete)
                {
                    _unitOfWork.GetRepository<EmailTemplate>().Delete(et.Id);
                }
                else
                {
                    _unitOfWork.GetRepository<EmailTemplate>().Update(et, et.Id, true);
                }             
            }
            _unitOfWork.Context.SaveChanges();
            return et;
        }
        public bool CanAccessTImeSheet(AdminService adminService, bool systemAdmin)
        {
            // Is system admin or Is Time Sheet Admin or member of configured group            
            return false;
        }
        public List<EmailTemplate> GetEmailTemplates ()
        {
            return _unitOfWork.GetRepository<EmailTemplate>()
               .GetAll()
               .OrderByDescending(o => o.DisplayText).ToList();
        }
        public MonthTimeSheet GetMonthTimeSheet(ADUserDTO user, DateTime date)
        {
            return GetMonthTimeSheet(user.UserDetails.SamAccountName, date);
        }
        public MonthTimeSheet GetMonthTimeSheet(string userSamAccountName, DateTime date)
        {
            var timeSheetData = _unitOfWork.GetRepository<TimeSheetData>()
               .GetAll()
               .OrderByDescending(o => o.Id)
               .Where(w => (w.SamAccountName == userSamAccountName) && (w.Month == date.Month && w.Year == date.Year)).FirstOrDefault();

            if (timeSheetData != null)
            {
                var timeSheet = Newtonsoft.Json.JsonConvert.DeserializeObject<MonthTimeSheet>(timeSheetData.TimeSheetDataJson);

                timeSheet.Approved = timeSheetData.Approved;
                timeSheet.ApprovedTimeStamp = timeSheetData.ApprovedTimeStamp;
                timeSheet.UnApprovedBy = timeSheetData.ApprovedBy;
                timeSheet.IsSubmitted = timeSheetData.Submitted;
                timeSheet.SubmittedTimeStamp = timeSheetData.SubmittedTimeStamp;
                timeSheet.SubmittedBySamAccountName = timeSheetData.SubmittedBy;

                return timeSheet;
            }
            return null;
        }
        private TimeSheetData GetMonthTimeSheetData(string samAccountName, DateTime date)
        {
            var TimeSheetData = _unitOfWork.GetRepository<TimeSheetData>()
               .GetAll()
               .OrderByDescending(o => o.Id)
               .Where(w => (w.SamAccountName == samAccountName) && (w.Month == date.Month && w.Year == date.Year)).FirstOrDefault();

            if (TimeSheetData != null)
            {
                return TimeSheetData;
            }
            return null;
        }
        private string EnsureGroupNamePostfix(string groupName)
        {
            return groupName.EndsWith(this.timesheetGroupNamepostfix) ? groupName : string.Format("{0}{1}", groupName, this.timesheetGroupNamepostfix);
        }
        public void Dispose()
        {
            base.Dispose();
        }
    }
}
