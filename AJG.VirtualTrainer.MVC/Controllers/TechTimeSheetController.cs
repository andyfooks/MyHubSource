using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.DTOs;
using AJG.VirtualTrainer.Helper.General;
using AJG.VirtualTrainer.MVC.Attributes;
using AJG.VirtualTrainer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using VirtualTrainer;
using VirtualTrainer.Models.MyHub;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    [AuthorizeWith401RedirectAttribute(Roles = "SystemAdmin,SystemSuperUser,everyone")]
    [OutputCache(NoStore = true, Duration = 0)]
    public class TechTimeSheetController : BaseController
    {
        #region [ Enums ]

        public enum TargetUsers
        {
            [Description("Team")]
            Team,
            [Description("All Subordinates")]
            AllSubordinates
        }
        public enum EmailTemplateAction
        {
            [Description("New")]
            New,
            [Description("Update")]
            Update
        }                

        #endregion

        public ActionResult TechTimeSheet()
        {
            try
            {
                Guid projectId;
                var user = Guid.TryParse(ConfigurationHelper.Get(AppSettingsList.TimeSheetProjectId), out projectId)
                    ? AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name, projectId)
                    : AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
                ViewBag.User = user;
                var helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
                ViewBag.UserName = GetContextUserSamAccountName();
                ViewBag.ADUser = helper.GetUser(ViewBag.UserName);
                if (ViewBag.ADUser == null)
                {
                    ViewBag.ADUser = AdminService.GetAdUserFromDB(GetContextUserSamAccountName());
                }
            }
            catch(Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return View();
        }                      
        public JsonResult GetCurrentUserWithPermissions()
        {
            try
            {
                var perms = AdminService.GetCurrentUserPermissions(GetContextUserSamAccountName(), HttpContext.User.Identity.Name);

                ADHelper helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
                var adUser = helper.GetUser(GetContextUserSamAccountName(), true);

                if (adUser == null)
                {
                    adUser = AdminService.GetAdUserFromDB(GetContextUserSamAccountName());
                }

                var adminActions = new List<KeyValuePair<string, string>>();
                var adminBulkActions = new List<KeyValuePair<string, string>>();
                if (perms.IsProjectAdmin)
                {
                    adminActions.AddRange(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.SendEmail), AdminAction.SendEmail.ToString()),
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.SaveToDB), AdminAction.SaveToDB.ToString()),
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.LocationRequired), AdminAction.LocationRequired.ToString()),
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.ApprovalRequired), AdminAction.ApprovalRequired.ToString()),
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.TechTimeSheetAccess), AdminAction.TechTimeSheetAccess.ToString()),
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.ConfigureBankHolidays), AdminAction.ConfigureBankHolidays.ToString()),
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.ConfigureDropDownLists), AdminAction.ConfigureDropDownLists.ToString()),
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.RunReports), AdminAction.RunReports.ToString())
                });
                    adminBulkActions.AddRange(new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.SendEmail), AdminAction.SendEmail.ToString())
                });
                }
                if (adUser.Managees != null && adUser.Managees.Any())
                {
                    adminActions.AddRange(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.SendEmail), AdminAction.SendEmail.ToString())
                });
                    adminBulkActions.AddRange(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.SendEmail), AdminAction.SendEmail.ToString()),
                });
                }
                if (perms.IsProjectSuperUser)
                {
                    adminActions.AddRange(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.SendEmail), AdminAction.SendEmail.ToString())
                });
                    adminBulkActions.AddRange(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(GeneralHelper.GetEnumDescription(AdminAction.SendEmail), AdminAction.SendEmail.ToString())
                });
                }

                var userDetails = new { ADUser = adUser, UserPerms = perms, AdminActions = adminActions.Distinct().OrderBy(t => t.Key).ToList(), AdminBulkActions = adminBulkActions.Distinct().OrderBy(t => t.Key).ToList() };
                return Json(userDetails, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #region [ Admin ]

        [Authorize(Roles="SystemAdmin, SystemSuperUser")]
        public JsonResult GetTimeSheetStatusForManager(string month, string year, string userSamAccountName, TargetUsers target = TargetUsers.Team, TimeSheetService.Status status = TimeSheetService.Status.NotSubmitted )
        {
            string message = string.Empty;
            bool error = false;

            try
            {      
                var userIdentity = string.IsNullOrEmpty(userSamAccountName) || userSamAccountName == "undefined" ? GetContextUserSamAccountName() : userSamAccountName;                

                // Try to find the user from the config DB
                var returnData = AdminService.GetAdUserFromDB(userIdentity);
                if (returnData == null)
                {
                    ADHelper helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
                    var user = helper.GetUser(userIdentity);                    
                }

                // we want to check the state of all the users monthly time sheet
                var userTeamMembersSamAccountNames = returnData.GetUserAndUserTeamList();
                var passedDate = TimeSheetService.GetDateFromMonthAndYear(month, year);
                passedDate = passedDate.AddDays(10);

                var users = new List<ADUserDTO>();

                switch(target)
                {
                    case TargetUsers.Team:
                        users = returnData.Managees;
                        break;
                    case TargetUsers.AllSubordinates:
                        users = returnData.GetFlattened();
                        break;
                }
               
                var aaa = TimeSheetService.GetMonthTimeSheetForUsers(users, passedDate, status);

                var returnDAta = aaa.Select(s => new
                {
                    SamAccountName = s.SamAccountName,
                    UserName = s.UserName,
                    EmployeeID = s.EmployeeID,
                    Email = s.Email,
                    ManagerEmail = s.ManagerEmail,
                    Include = s.Include,
                    Approved = s.Approved,
                    Submitted = s.Submitted
                }).ToList();
                message =  string.Format("{0} Users successfully Loaded", returnDAta.Count());
                return new JsonResult { Data = new {data = returnDAta.OrderBy(o => o.SamAccountName), error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                message = string.Format("There has been an error Gettng the user list: {0}", ex.Message);
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { data = "", error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }                        
        [HttpPost]
        public JsonResult SaveUserSheetsToWorkSheetMonthActivityTable(string month, string year, string userNames)
        {
            string message = string.Empty;
            bool error = false;
            List<string> userNamesList = new List<string>();

            try
            {
                var passedDate = TimeSheetService.GetDateFromMonthAndYear(month, year);
                passedDate = passedDate.AddDays(10);
                userNamesList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(userNames);
                
                // Get complete list of all users then pass into SaveTimeSheetsToReportingDB.
                if ((userNamesList != null && userNamesList.Count < 1) || userNamesList == null)
                {
                    var routeADUser = AdminService.GetRoutADUserFromDB();

                    userNamesList = new List<string>();
                    var a = AdminService.GetSystemConfigByGroupNameAndKey(AdminAction.SaveToDB.ToString(), AdminAction.SaveToDB.ToString()).FirstOrDefault();
                    var timeSheetRules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimeSheetRuleDTO>>(a.value);
                    foreach (var rule in timeSheetRules)
                    {
                        if (!string.IsNullOrEmpty(rule.UserID) && rule.Enabled)
                        {
                            var user = routeADUser.GetSpecifcUserFromHierachy(rule.UserID);
                            if (user != null)
                            {
                                switch (rule.Target)
                                {
                                    case TimeSheetRuleTarget.User:
                                        userNamesList.Add(user.UserDetails.EmployeeID);
                                        break;
                                    case TimeSheetRuleTarget.Team:
                                        userNamesList.AddRange(user.GetUserTeamEmployeeIdList());
                                        break;
                                    case TimeSheetRuleTarget.AllSubordinates:
                                        userNamesList.AddRange(user.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(em => em.Value).ToList());
                                        break;
                                }
                            }
                        }
                    }
                }

                if (userNamesList != null)
                {
                    var savedTimeSheetUserNames = TimeSheetService.SaveTimeSheetsToReportingDB(passedDate, userNamesList.Distinct().ToList(), AdminService);
                    message = string.Format("{0} {1} Successfully Saved to the Reporting DB", 
                        savedTimeSheetUserNames.Distinct().Count(),
                        savedTimeSheetUserNames.Distinct().Count() == 1 ? "TimeSheet" : "TimeSheets");
                }                             
            }
            catch (Exception ex)
            {
                message = string.Format("There has been an error saving the TIme sheets to the Reporting DB: {0}", ex.Message);
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        [HttpPost]
        public JsonResult CreateTimeSheetReport(string TimesheetReport)
        {
            string message = string.Empty;
            bool error = false;
            string path = string.Empty;
            string fileName = string.Empty;
            try
            {
                var tsr = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeSheetReport>(TimesheetReport);
                
                // Create Dates
                var fromDate = new DateTime(tsr.YearFrom, tsr.MonthFrom, 1);
                var toDate = new DateTime(tsr.YearTo, tsr.MonthTo, 1).AddMonths(1).AddDays(-1);

                // Create the file
                fileName = string.Format("TimeSheetReport_From{0}_To{1}_{2}.xlsx", fromDate.ToString("yyyMMMdd"), toDate.ToString("yyyMMMdd"), DateTime.Now.ToString("yyy_MM_dd_HH_mm_ss"));
                string filePath = GetDownloadFilePath(fileName);

                List<string> targetUserEmployeeIds = new List<string>();

                // Who are we getting the report for?
                if (tsr.TargetSpecificUser)
                {
                    var user = AdminService.GetAdUserFromDB(tsr.UserId);
                    switch (tsr.ReportTarget)
                    {
                        case TimeSheetRuleTarget.User:
                            targetUserEmployeeIds.Add(user.UserDetails.EmployeeID);
                            break;
                        case TimeSheetRuleTarget.Team:
                            targetUserEmployeeIds.AddRange(user.GetUserTeamEmployeeIdList());
                            break;
                        case TimeSheetRuleTarget.AllSubordinates:
                            targetUserEmployeeIds.AddRange(user.UserAndSubordinatesSamAccountAndEmployeeIdList().Select(em => em.Value).ToList());
                            break;
                    }
                }

                // Get the report data from the DB.
                var timeSheetData = TimeSheetService.GetTimeSheetReportData(tsr.TargetSpecificUser, targetUserEmployeeIds, fromDate, toDate);

                ExcelUtlity eu = new ExcelUtlity();
                List<objectProperty> objectProperties = ExcelUtlity.GetOrderedObjectPropertyDetails<WorkSheetActivity>();
                path = eu.WriteObjectsToExcel<WorkSheetActivity>(timeSheetData, objectProperties, filePath);
            }
            catch (Exception ex)
            {
                message = string.Format("There has been an error getting the Time Sheet Report: {0}", ex.Message);
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { docName = fileName, error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        public JsonResult GetAdminSingleFieldSetting(AdminAction adminAction)
        {
            string message = string.Empty;
            bool error = false;
            try
            {
                SystemConfig template = null;
                switch (adminAction)
                {
                    case AdminAction.TechTimeSheetAccess:
                    case AdminAction.ApprovalRequired:
                    case AdminAction.LocationRequired:
                    case AdminAction.SaveToDB:
                        template = AdminService.GetSystemConfigByGroupNameAndKey(adminAction.ToString(), adminAction.ToString()).FirstOrDefault();
                        break;
                    case AdminAction.RunReports:
                        var returnObject = new TimeSheetReport(true);                        
                        return Json(new { TimeSheetReport = returnObject, option = adminAction.ToString(), error = error, message = message }, JsonRequestBehavior.AllowGet);
                }
                var ruleTemplate = new TimeSheetRuleDTO()
                {
                    Enabled = true,
                    Permission = TimeSheetRulePermission.Allow,
                    Target = TimeSheetRuleTarget.User,
                    UserID = string.Empty
                };

                if (template == null)
                {
                    List<TimeSheetRuleDTO> TimeSheetRuleList = new List<TimeSheetRuleDTO>()
                    {
                        ruleTemplate
                    };
                    return Json(new { ruleList = TimeSheetRuleList, timeSheetRuleTemplate = ruleTemplate, option = adminAction.ToString(), error = error, message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var et = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimeSheetRuleDTO>>(template.value);
                    return Json(new { ruleList = et, timeSheetRuleTemplate = ruleTemplate, option = adminAction.ToString(), error = error, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }        
        [HttpPost]
        public JsonResult SaveDDL(string ddl = "")
        {
            string message = string.Empty;
            bool error = false;            
            try
            {
                var dropDownList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CascadingDropDownItem>>(ddl);
                this.AdminService.SaveUpdateSystemConfig(SystemConfig.ConfigKeys.TechTimeSheetDropDownVals.ToString(), SystemConfig.ConfigKeys.TechTimeSheetDropDownVals.ToString(), ddl);
                message = string.Format("Drop Down List Successfully Saved");              
            }
            catch (Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        [HttpPost]
        public JsonResult SaveUpdateEmailTemplate(string EmailTemplateJson, bool Delete = false)
        {
            string message = string.Empty;
            bool error = false;
            EmailTemplate returnTemp = new EmailTemplate();
            try
            {
                var et = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailTemplate>(EmailTemplateJson);
                et.TimeStamp = DateTime.Now;
                et.SavedBy = GetContextUserSamAccountName();            
                et.UncheckedUsers = et.IncludeUncheckedUsers == true ? string.Join(",", et.UncheckedUsersList) : string.Empty;
                returnTemp = TimeSheetService.AddUpdateEmailTemplate(et, Delete);
                message = string.Format("Email Template Saved");
                if (Delete)
                {
                    returnTemp.Id = 0;
                    message = string.Format("Email Template Deleted");
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { template = returnTemp, error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        public JsonResult GetEmailTemplates()
        {
            string message = string.Empty;
            bool error = false;
            var templates = new List<EmailTemplate>
                {
                    new EmailTemplate()
                    {
                        Body = "",
                        Description = "",
                        DisplayText = "",
                        Id = 0,
                        SavedBy = "",
                        Subject = "",
                        IncludeUncheckedUsers = false,
                        UncheckedUsers = ""
                    }
                };

            try
            {                
                templates.AddRange(TimeSheetService.GetEmailTemplates());
                foreach (var temp in templates)
                {
                    if (temp.IncludeUncheckedUsers == true && !string.IsNullOrEmpty(temp.UncheckedUsers))
                    {
                        temp.UncheckedUsersList = temp.UncheckedUsers.Split(',').ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { templates = templates, error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        [HttpPost]
        public JsonResult SaveAdminSingleFieldSetting(string value, AdminAction actionEnum)
        {
            string message = string.Empty;
            bool error = false;
            try
            {
                switch (actionEnum)
                {
                    case AdminAction.TechTimeSheetAccess:
                    case AdminAction.ApprovalRequired:
                    case AdminAction.LocationRequired:
                    case AdminAction.SaveToDB:
                        this.AdminService.SaveUpdateSystemConfig(actionEnum.ToString(), actionEnum.ToString(), value);
                        message = string.Format("'{0}' Setting Successfully Saved", GeneralHelper.GetEnumDescription(actionEnum));                        
                        break;
                }                
            }
            catch (Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        public JsonResult GetBankHolidaysForYear(string year, bool systemGenerated = false)
        {
            var dates = new List<double>();
            string message = string.Empty;
            bool error = false;
            try
            {        
                dates = this.TimeSheetService.GetBankHolidaysJSFriendly(year, this.AdminService, systemGenerated);
            }
            catch(Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { dates = dates, error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };            
        }
        [HttpPost]
        public JsonResult SaveBankHolidaysForYear(string dates, string year)
        {
            string message = string.Empty;
            bool error = false;
            try
            {
                var datesconvered = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(dates);
                this.TimeSheetService.SaveUpdateBankHolidays(datesconvered, year, this.AdminService);
                message = string.Format("Bank Holidays Successfully Saved");           
            }
            catch (Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }

        #endregion

        #region [ Time Sheet ]

        [HttpPost]
        public JsonResult MonthDataAction(string monthDataJson, TimeSheetAction actionEnum)
        {
            string message = string.Empty;
            bool error = false;
            var monthTimeSheet = new MonthTimeSheet();
            try
            {
                monthTimeSheet = Newtonsoft.Json.JsonConvert.DeserializeObject<MonthTimeSheet>(monthDataJson);                
                message = monthTimeSheet.PerformTimeSheetAction(AdminService, TimeSheetService, GetContextUser(), actionEnum);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { data = monthTimeSheet.GetMonthTimeSheetForUI(), error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }       
        [HttpPost]
        public JsonResult GetMonthTimeSheetData(string month, string year, string userSamAccountName, string actionEnum)
        {
            string message = string.Empty;
            bool error = false;
            MonthTimeSheet monthData = new MonthTimeSheet();

            try
            {
                var passedDate = TimeSheetService.GetDateFromMonthAndYear(month, year);
                passedDate = passedDate.AddDays(10);
                var user = GetUser(userSamAccountName);
                monthData = new MonthTimeSheet(passedDate, user, this.AdminService, this.TimeSheetService);                
            }
            catch(Exception ex)
            {
                message = ex.Message;
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }

            return new JsonResult { Data = new { monthData = monthData.GetMonthTimeSheetForUI(), error = error, message = message } , JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };            
        }
        public JsonResult GetDDLOptions()
        {
            string message = string.Empty;
            bool error = false;
            var template = new CascadingDropDownItem() { Name = "New" };
            List<CascadingDropDownItem> latestCascadeDdl = new List<CascadingDropDownItem>();
            try
            {
                latestCascadeDdl = TimeSheetService.GetCascadingDropDownList(AdminService);
            }
            catch (Exception ex)
            {
                message = string.Format("There has been an issue in GetDDLOptions: {0}", ex.Message);
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult
            {
                Data = new { template = template, latestCascadeDDL = latestCascadeDdl, error = error, message = message },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        #endregion       
    }
}