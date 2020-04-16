using AJG.VirtualTrainer.Helper.Excel;
using AJG.VirtualTrainer.Helper.Exchange;
using AJG.VirtualTrainer.MVC.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    [AuthorizeWith401RedirectAttribute(Roles = "ProjectAdmin,SystemAdmin")]
    public class AdminController : BaseController
    {
        public class ErrorMessgaeObject
        {
            public string MessageType { get; set; }
            public string ErrorMessage { get; set; }
        }
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult IndexAngularJS()
        {
            ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            return View();
        }
        public ActionResult Projects()
        {
            //ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            ViewBag.PageNAme = "Projects";
            return View();
        }
        public ActionResult SystemUsers()
        {
            // Log Exception
            //SystemLog errorLog = new SystemLog(new Exception("Some Exception"));
            //AdminService.SaveSystemLog(errorLog);

            //ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            ViewBag.PageNAme = "SystemUsers";
            return View();
        }
        public ActionResult SystemLogs()
        {
            //ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            ViewBag.PageNAme = "SystemLogs";
            return View();
        }

        #region [ Execute VT Tasks ]

        public JsonResult ExecuteVirtualTrainer()
        {
            try
            {
                Helper h = new Helper();
                foreach (Project p in AdminService.ListProjects())
                {
                    string bodyTemplatePath = h.GetEmailRazorTemplateBodyPath(this, p);
                    string subjectTemplatePath = h.GetEmailRazorTemplateSubjectPath(this, p);
                    string attachmentTemplatePath = h.GetEmailRazorTemplateAttachmentPath(this, p);
                    this.AdminService.ExecuteVirtualTrainerForProject(bodyTemplatePath, subjectTemplatePath, attachmentTemplatePath, p.ProjectUniqueKey);
                    //this.AdminService.ExecuteVirtualTrainer(bodyTemplatePath, subjectTemplatePath, attachmentTemplatePath);
                }
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json("false", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExecuteVirtualTrainerForProject(Guid projectId)
        {
            try
            {
                Helper h = new Helper();

                foreach (Project p in AdminService.ListProjects().Where(a => a.ProjectUniqueKey == projectId).ToList())
                {
                    string bodyTemplatePath = h.GetEmailRazorTemplateBodyPath(this, p);
                    string subjectTemplatePath = h.GetEmailRazorTemplateSubjectPath(this, p);
                    string attachmentTemplatePath = h.GetEmailRazorTemplateAttachmentPath(this, p);
                    this.AdminService.ExecuteVirtualTrainerForProject(bodyTemplatePath, subjectTemplatePath, attachmentTemplatePath, projectId);
                    return Json(string.Format("Successfully Executed Project: {0}.", projectId), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("false", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Exchange Accounts ]

        public JsonResult TestExchangeAccountConfig(int configId)
        {
            string returnMessage = string.Empty;
            try
            {
                List<AJGEmailMessage> emails = this.AdminService.TestExchangeAccountConfig(configId);
                AJGEmailMessage email = emails.FirstOrDefault();

                returnMessage = email == null ?
                    "Success: but no messages were retrieved. Add a message to the inbox and try again." :
                    string.Format("Success: here are some details for the first 'Inbox' email retrieved: From: {0}, To: {1}.", email.MessageFrom, email.MessageTo);
            }
            catch (Exception ex)
            {
                returnMessage = string.Format("Exception: There was an Exception: {0}. For further info please check the system logs.", ex.Message);
            }
            return Json(returnMessage, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectExchangeAccounts(Guid projectId)
        {
            List<ExchangeAccountDetails> accountDetails = this.AdminService.ListProjectExchangeAccounts(projectId);
            return Json(accountDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectExchangeAccounts(Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var targetExchangeAccountDetails = JsonConvert.DeserializeObject<IEnumerable<ExchangeAccountDetails>>(Request.QueryString["models"], jsonSettings);

            ExchangeAccountDetails accountdetails = targetExchangeAccountDetails.FirstOrDefault();
            accountdetails.ProjectId = projectId;
            this.AdminService.AddProjectExchangeAccounts(accountdetails);

            return Json(targetExchangeAccountDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyProjectExchangeAccounts()
        {
            var details = JsonConvert.DeserializeObject<IEnumerable<ExchangeAccountDetails>>(Request.QueryString["models"]);
            ExchangeAccountDetails firstdetails = details.FirstOrDefault();

            this.AdminService.DeleteProjectExchangeAccounts(firstdetails.Id);

            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateProjectExchangeAccounts()
        {
            var details = JsonConvert.DeserializeObject<IEnumerable<ExchangeAccountDetails>>(Request.QueryString["models"]);
            ExchangeAccountDetails firstdetails = details.FirstOrDefault();

            this.AdminService.UpdateProjectExchangeAccounts(firstdetails);

            return Json(details, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Regions ]

        public JsonResult ListRegionsForOfficeRegionsDropDown(Guid projectId)
        {
            List<RegionForDropDownDTO> regions = this.AdminService.ListRegionsForOfficeRegionsDropDown(projectId).Select(r => new RegionForDropDownDTO()
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            regions.Insert(0, new RegionForDropDownDTO() { Id = 0, Name = "" });

            return Json(regions, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectRegions(Guid projectGuid)
        {
            List<Region> regions = this.AdminService.ListProjectRegions(projectGuid);
            return Json(regions, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateProjectRegion()
        {
            var regions = JsonConvert.DeserializeObject<IEnumerable<Region>>(Request.QueryString["models"]);
            Region region = regions.FirstOrDefault();
            this.AdminService.UpdateRegion(region);

            return Json(regions, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectRegion(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var regions = JsonConvert.DeserializeObject<IEnumerable<Region>>(Request.QueryString["models"], jsonSettings);

            Region region = regions.FirstOrDefault();
            region.ProjectId = projectGuid;
            this.AdminService.AddRegion(region);

            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Delete/5
        public JsonResult RemoveProjectRegion()
        {
            var regions = JsonConvert.DeserializeObject<IEnumerable<Region>>(Request.QueryString["models"]);
            Region region = regions.FirstOrDefault();

            this.AdminService.DeleteRegion(region.Id);

            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Acturis Import ]

        public JsonResult ListActurisImportSnapShot([DataSourceRequest] DataSourceRequest request, int importId)
        {
            List<UserBusinessStructureInfo> businessStructures = this.AdminService.ListActurisImportSnapShot(importId);

            Kendo.Mvc.UI.DataSourceResult result = businessStructures.ToDataSourceResult(request);
            return Json(new { data = result.Data, total = result.Total }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListProjectActurisImports(Guid projectId)
        {
            List<ActurisBusinessStructureSyncConfig> syncConfigs = this.AdminService.ListProjectActurisImports(projectId);
            return Json(syncConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectActurisImport(Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var syncConfigs = JsonConvert.DeserializeObject<IEnumerable<ActurisBusinessStructureSyncConfig>>(Request.QueryString["models"], jsonSettings);
            ActurisBusinessStructureSyncConfig syncConfig = syncConfigs.FirstOrDefault();
            syncConfig.ProjectId = projectId;

            this.AdminService.AddRuleConfiguration(syncConfig);

            syncConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectId).Where(a => a.Id == syncConfig.ScheduleId).FirstOrDefault().Name;
            syncConfig.TargetDbName = this.AdminService.ListProjectTargetDBDetails(projectId).Where(a => a.Id == syncConfig.TargetDatabaseDetailsId).FirstOrDefault().DisplayName;

            return Json(syncConfig, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DestroyProjectActurisImport()
        {
            var syncConfigs = JsonConvert.DeserializeObject<IEnumerable<ActurisBusinessStructureSyncConfig>>(Request.QueryString["models"]);
            ActurisBusinessStructureSyncConfig syncConfig = syncConfigs.FirstOrDefault();
            this.AdminService.DestroyProjectActurisImport(syncConfig.Id);
            return Json(syncConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateProjectActurisImport()
        {
            var syncConfigs = JsonConvert.DeserializeObject<IEnumerable<ActurisBusinessStructureSyncConfig>>(Request.QueryString["models"]);
            ActurisBusinessStructureSyncConfig syncConfig = syncConfigs.FirstOrDefault();

            this.AdminService.UpdateProjectActurisImport(syncConfig);

            syncConfig.ScheduleName = this.AdminService.ListProjectSchedules(syncConfig.ProjectId).Where(a => a.Id == syncConfig.ScheduleId).FirstOrDefault().Name;
            syncConfig.TargetDbName = this.AdminService.ListProjectTargetDBDetails(syncConfig.ProjectId).Where(a => a.Id == syncConfig.TargetDatabaseDetailsId).FirstOrDefault().DisplayName;

            return Json(syncConfigs, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [ Breaches ]

        public JsonResult ListBreachesSnapShot(int ruleId)
        {
            List<BreachLog> breaches = this.AdminService.ListBreachesLOBSnapShot(ruleId);

            if (breaches.Count > 900)
            {
                List<BreachLogDTO> breachesDTo = breaches.Select(b => new BreachLogDTO()
                {
                    Id = b.Id,
                    IsArchived = b.IsArchived,
                    OfficeName = b.OfficeName,
                    RuleConfigurationName = b.RuleConfigurationName,
                    RuleName = b.RuleName,
                    TeamName = b.TeamName,
                    UserName = b.UserName,
                    TimeStamp = b.TimeStamp,
                    ContextRef = b.ContextRef,
                    BreachDisplayText = b.BreachDisplayText,
                    RuleBreachFieldOne = b.RuleBreachFieldOne,
                    RuleBreachFieldTwo = b.RuleBreachFieldTwo,
                    RuleBreachFieldThree = b.RuleBreachFieldThree,
                    RuleBreachFieldFour = b.RuleBreachFieldFour,
                    RuleBreachFieldFive = b.RuleBreachFieldFive,
                    RuleBreachFieldSix = b.RuleBreachFieldSix,
                    RuleBreachFieldSeven = b.RuleBreachFieldSeven,
                    RuleBreachFieldEight = b.RuleBreachFieldEight,
                    RuleBreachFieldNine = b.RuleBreachFieldNine,
                    RuleBreachFieldTen = b.RuleBreachFieldTen,
                    RuleBreachFieldEleven = b.RuleBreachFieldEleven,
                    RuleBreachFieldTwelve = b.RuleBreachFieldTwelve,
                    RuleBreachFieldThirteen = b.RuleBreachFieldThirteen,
                    RuleBreachFieldFourteen = b.RuleBreachFieldFourteen,
                    RuleBreachFieldFifteen = b.RuleBreachFieldFifteen,
                    RuleBreachFieldSixteen = b.RuleBreachFieldSixteen,
                    RuleBreachFieldSeventeen = b.RuleBreachFieldSeventeen,
                    RuleBreachFieldEighteen = b.RuleBreachFieldEighteen,
                    RuleBreachFieldNineteen = b.RuleBreachFieldNineteen,
                    RuleBreachFieldTwenty = b.RuleBreachFieldTwenty
                }).ToList().Take(900).ToList();
                return Json(breachesDTo, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(breaches, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ListProjectBreaches(Guid projectId, [DataSourceRequest] DataSourceRequest request)
        {
            List<BreachLogDTO> breaches = this.AdminService.ListProjectBreachesDTO(projectId);

            Kendo.Mvc.UI.DataSourceResult result = breaches.ToDataSourceResult(request);
            return Json(new { data = result.Data, total = result.Total }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyProjectBreach(int Id)
        {
            this.AdminService.DeleteBreachLog(Id);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateProjectBreach(BreachLogDTO data)
        {
            this.AdminService.UpdateBreach(data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBreachFields()
        {
            List<string> data = BreachLog.GetBreachFieldNamesForForms();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Schedules ]

        public JsonResult ListScheduleFrequencies()
        {
            List<ScheduleFrequency> scheduleFrequencies = this.AdminService.ListScheduleFrequencies();
            return Json(scheduleFrequencies, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListSchedules(Guid projectId)
        {
            List<Schedule> schedules = this.AdminService.ListProjectSchedules(projectId);
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateSchedule(Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var Schedules = JsonConvert.DeserializeObject<IEnumerable<Schedule>>(Request.QueryString["models"], jsonSettings);

            Schedule schedule = Schedules.FirstOrDefault();
            schedule.ProjectId = projectId;
            this.AdminService.AddSchedule(schedule);
            schedule.ScheduleFrequencyName = AdminService.ListScheduleFrequencies().Where(a => a.Id == schedule.ScheduleFrequencyId).FirstOrDefault().Name;
            return Json(Schedules, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroySchedule()
        {
            var schedules = JsonConvert.DeserializeObject<IEnumerable<Schedule>>(Request.QueryString["models"]);
            Schedule schedule = schedules.FirstOrDefault();

            this.AdminService.DeleteSchedule(schedule.Id);

            return Json(schedules, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSchedule()
        {
            var schedules = JsonConvert.DeserializeObject<IEnumerable<Schedule>>(Request.QueryString["models"]);
            Schedule schedule = schedules.FirstOrDefault();

            this.AdminService.UpdateSchedule(schedule);
            schedule.ScheduleFrequencyName = AdminService.ListScheduleFrequencies().Where(a => a.Id == schedule.ScheduleFrequencyId).FirstOrDefault().Name;

            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ TargetDBDetails ]

        public JsonResult ListTargetDBDetails(Guid projectId)
        {
            List<TargetDatabaseDetails> dbDetails = this.AdminService.ListProjectTargetDBDetails(projectId);
            return Json(dbDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateTargetDBDetails(Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var TargetDatabaseDetails = JsonConvert.DeserializeObject<IEnumerable<TargetDatabaseDetails>>(Request.QueryString["models"], jsonSettings);

            TargetDatabaseDetails dbDetails = TargetDatabaseDetails.FirstOrDefault();
            dbDetails.ProjectId = projectId;
            this.AdminService.AddTargetDatabaseDetails(dbDetails);

            return Json(TargetDatabaseDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyTargetDBDetails()
        {
            var DBsDetails = JsonConvert.DeserializeObject<IEnumerable<TargetDatabaseDetails>>(Request.QueryString["models"]);
            TargetDatabaseDetails dbDetails = DBsDetails.FirstOrDefault();

            this.AdminService.DeleteTargetDatabaseDetails(dbDetails.Id);

            return Json(DBsDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateTargetDBDetails()
        {
            var targetDBsDetails = JsonConvert.DeserializeObject<IEnumerable<TargetDatabaseDetails>>(Request.QueryString["models"]);
            TargetDatabaseDetails targetDBDetails = targetDBsDetails.FirstOrDefault();

            // it is possible for the user to change project here, so we need to check.
            this.AdminService.UpdateTargetDatabaseDetails(targetDBDetails);

            return Json(targetDBsDetails, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Roles/Permissions - team, office and project]

        public JsonResult ListUserTeamMemberships(int userId)
        {
            List<TeamPermission> systemRoles = this.AdminService.ListUserTeamMemberships(userId);
            return Json(systemRoles, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListUserOfficeMemberships(int userId)
        {
            List<OfficePermission> systemRoles = this.AdminService.ListUserOfficeMemberships(userId);
            return Json(systemRoles, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListUserProjectMemberships(int userId)
        {
            List<ProjectMembershipDTO> systemRoles = this.AdminService.ListUserSystemRoles(userId);
            return Json(systemRoles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProjectUserMembership()
        {
            var projectMemberDtos = JsonConvert.DeserializeObject<IEnumerable<ProjectMembershipDTO>>(Request.QueryString["models"]);
            ProjectMembershipDTO projectMemberDto = projectMemberDtos.FirstOrDefault();

            // it is possible for the user to change project here, so we need to check.
            this.AdminService.UpdateProjectUserAdminMembership(projectMemberDto);

            return Json(projectMemberDto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateProjectUserMembership(int userId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var rojectMembershipDTOs = JsonConvert.DeserializeObject<IEnumerable<ProjectMembershipDTO>>(Request.QueryString["models"], jsonSettings);

            ProjectMembershipDTO projectMembershipDTO = rojectMembershipDTOs.FirstOrDefault();
            projectMembershipDTO.UserId = userId;
            projectMembershipDTO.ProjectName = this.AdminService.ListProjects().Where(p => p.ProjectUniqueKey == projectMembershipDTO.ProjectId).FirstOrDefault().ProjectName;
            this.AdminService.AddProjectUserMemberMembership(projectMembershipDTO);
            this.AdminService.UpdateProjectUserAdminMembership(projectMembershipDTO);
            return Json(projectMembershipDTO, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Delete/5
        public JsonResult DestroyProjectUserMembership()
        {
            var projectMembershipDTOs = JsonConvert.DeserializeObject<IEnumerable<ProjectMembershipDTO>>(Request.QueryString["models"]);
            ProjectMembershipDTO projectMembershipDTO = projectMembershipDTOs.FirstOrDefault();

            this.AdminService.DeleteProjectUserMembership(projectMembershipDTO);

            return Json(projectMembershipDTOs, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Team ]

        public JsonResult ListOfficeTeams(int officeId)
        {
            List<Team> teams = this.AdminService.ListOfficeTeams(officeId);
            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectTeams(Guid projectGuid)
        {
            List<Team> teams = this.AdminService.ListProjectTeams(projectGuid);
            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateTeam()
        {
            var teams = JsonConvert.DeserializeObject<IEnumerable<Team>>(Request.QueryString["models"]);
            Team team = teams.FirstOrDefault();
            Team returnTeam = this.AdminService.UpdateTeam(team);

            return Json(returnTeam, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateTeamUser(int teamId, Guid projectId)
        {
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"]);
            User user = users.FirstOrDefault();
            this.AdminService.UpdateTeamRole(user, teamId, projectId, RoleEnum.TeamLead);
            this.AdminService.UpdateTeamRole(user, teamId, projectId, RoleEnum.ClaimsHandler);

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveTeamUser(int teamId, Guid projectId)
        {
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"]);
            User user = users.FirstOrDefault();
            user.IsTeamMember = false;
            this.AdminService.RemoveUserFromTeam(user, teamId, projectId);

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateTeam(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var teams = JsonConvert.DeserializeObject<IEnumerable<Team>>(Request.QueryString["models"], jsonSettings);

            Team team = teams.FirstOrDefault();
            team.ProjectId = projectGuid;
            this.AdminService.AddTeam(team);

            return Json(teams, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Delete/5
        public JsonResult DestroyTeam()
        {
            var teams = JsonConvert.DeserializeObject<IEnumerable<Team>>(Request.QueryString["models"]);
            Team team = teams.FirstOrDefault();

            this.AdminService.DeleteTeam(team.Id);

            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddUserToTeam(int teamId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"], jsonSettings);
            User user = users.FirstOrDefault();
            user.IsTeamMember = true;
            this.AdminService.UpdateTeamRole(user, teamId, projectId, RoleEnum.TeamMember);
            this.AdminService.UpdateTeamRole(user, teamId, projectId, RoleEnum.TeamLead);
            this.AdminService.UpdateTeamRole(user, teamId, projectId, RoleEnum.ClaimsHandler);

            user = this.AdminService.ListTeamUsers(teamId).Where(u => u.Id == user.Id).FirstOrDefault();
            user.IsTeamMember = true;

            return Json(user, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [ Office ]

        public JsonResult ListProjectOffices(Guid projectGuid)
        {
            List<Office> offices = this.AdminService.ListProjectOffices(projectGuid);
            return Json(offices, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOffice()
        {
            var offices = JsonConvert.DeserializeObject<IEnumerable<Office>>(Request.QueryString["models"]);
            Office office = offices.FirstOrDefault();
            this.AdminService.UpdateOffice(office);
            if (office.RegionId != null)
            {
                office.RegionName = this.AdminService.ListProjectRegions(office.ProjectId).Where(r => r.Id == office.RegionId).FirstOrDefault().Name;
            }
            else {
                office.RegionName = "";
                office.RegionId = 0;
            }
            office.Region = null;
            return Json(offices, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateOfficeUser(int officeId, Guid projectId)
        {
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"]);
            User user = users.FirstOrDefault();
            this.AdminService.UpdateUserOfficeRole(user, officeId, projectId, RoleEnum.BranchManager);
            this.AdminService.UpdateUserOfficeRole(user, officeId, projectId, RoleEnum.QualityAuditor);
            this.AdminService.UpdateUserOfficeRole(user, officeId, projectId, RoleEnum.RegionalManager);

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveOfficeUser(int officeId, Guid projectId)
        {
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"]);
            User user = users.FirstOrDefault();
            this.AdminService.RemoveUserFromOffice(user, officeId, projectId);

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateOffice(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var offices = JsonConvert.DeserializeObject<IEnumerable<Office>>(Request.QueryString["models"], jsonSettings);

            Office office = offices.FirstOrDefault();
            office.ProjectId = projectGuid;
            this.AdminService.AddOffice(office);

            if (office.RegionId != null)
            {
                office.RegionName = this.AdminService.ListProjectRegions(projectGuid).Where(r => r.Id == office.RegionId).FirstOrDefault().Name;
                office.Region = null;
            }
            else
            {
                office.RegionName = "";
                office.RegionId = 0;
            }

            return Json(offices, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Delete/5
        public JsonResult DestroyOffice()
        {
            var offices = JsonConvert.DeserializeObject<IEnumerable<Office>>(Request.QueryString["models"]);
            Office office = offices.FirstOrDefault();

            this.AdminService.DeleteOffice(office.Id);

            return Json(offices, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddUserToOffice(int officeId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"], jsonSettings);
            User user = users.FirstOrDefault();
            user.IsOfficeMember = true;
            this.AdminService.UpdateUserOfficeRole(user, officeId, projectId, RoleEnum.BranchMember);
            this.AdminService.UpdateUserOfficeRole(user, officeId, projectId, RoleEnum.BranchManager);
            this.AdminService.UpdateUserOfficeRole(user, officeId, projectId, RoleEnum.QualityAuditor);
            this.AdminService.UpdateUserOfficeRole(user, officeId, projectId, RoleEnum.RegionalManager);

            // populate return data.
            user = this.AdminService.ListOfficeUsers(officeId).Where(u => u.Id == user.Id).FirstOrDefault();
            user.IsOfficeMember = true;

            return Json(user, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Sql Input ]

        public JsonResult ListRuleConfigHardCodedInputItems(int ruleConfigId)
        {
            List<RuleStoredProcedureInputValueHardCoded> InputValues = this.AdminService.ListRuleConfigHardCodedInputItems(ruleConfigId);
            return Json(InputValues, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfigHardCodedInputItem(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var inputItems = JsonConvert.DeserializeObject<IEnumerable<RuleStoredProcedureInputValueHardCoded>>(Request.QueryString["models"], jsonSettings);
            RuleStoredProcedureInputValueHardCoded inputItem = inputItems.FirstOrDefault();

            inputItem.RuleConfigurationId = ruleConfigId;
            inputItem.ProjectId = projectId;
            inputItem.ParameterName = inputItem.ParameterName.StartsWith("@") ? inputItem.ParameterName : string.Format("@{0}", inputItem.ParameterName);
            this.AdminService.AddRuleConfigHardCodedInputItem(inputItem);

            return Json(inputItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyRuleConfigHardCodedInputItem()
        {
            var inputItems = JsonConvert.DeserializeObject<IEnumerable<RuleStoredProcedureInputValueHardCoded>>(Request.QueryString["models"]);
            RuleStoredProcedureInputValueHardCoded inputItem = inputItems.FirstOrDefault();

            this.AdminService.DeleteRuleConfigHardCodedInputItem(inputItem.Id);

            return Json(inputItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfigHardCodedInputItem()
        {
            var inputItems = JsonConvert.DeserializeObject<IEnumerable<RuleStoredProcedureInputValueHardCoded>>(Request.QueryString["models"]);
            RuleStoredProcedureInputValueHardCoded inputItem = inputItems.FirstOrDefault();
            inputItem.ParameterName = inputItem.ParameterName.StartsWith("@") ? inputItem.ParameterName : string.Format("@{0}", inputItem.ParameterName);
            this.AdminService.UpdateRuleConfigHardCodedInputItem(inputItem);
            return Json(inputItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RuleConfigSqlClassRefInputList()
        {
            List<string> inputOptions = new List<string>();
            inputOptions.Add("User-AlsoKnownAs");
            return Json(inputOptions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListRuleConfigClassRefInputItems(int ruleConfigId)
        {
            List<ClassInputValueDTO> RuleConfigInputValues = this.AdminService.ListRuleConfigClassRefInputItems(ruleConfigId);
            return Json(RuleConfigInputValues, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfigClassRefInputItem(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var ClassInputValueDTOs = JsonConvert.DeserializeObject<IEnumerable<ClassInputValueDTO>>(Request.QueryString["models"], jsonSettings);
            ClassInputValueDTO ClassInputValueDTO = ClassInputValueDTOs.FirstOrDefault();

            ClassInputValueDTO.RuleConfigId = ruleConfigId;
            ClassInputValueDTO.ProjectId = projectId;
            ClassInputValueDTO.ParameterName = ClassInputValueDTO.ParameterName.StartsWith("@") ? ClassInputValueDTO.ParameterName : string.Format("@{0}", ClassInputValueDTO.ParameterName);
            this.AdminService.AddRuleConfigClassRefInputItem(ClassInputValueDTO);

            return Json(ClassInputValueDTOs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyRuleConfigClassRefInputItemm()
        {
            var ClassInputValueDTOs = JsonConvert.DeserializeObject<IEnumerable<ClassInputValueDTO>>(Request.QueryString["models"]);
            ClassInputValueDTO ClassInputValueDTO = ClassInputValueDTOs.FirstOrDefault();

            this.AdminService.DeleteRuleConfigClassRefInputItemm(ClassInputValueDTO.Id);

            return Json(ClassInputValueDTO, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfigClassRefInputItem()
        {
            var ClassInputValueDTOs = JsonConvert.DeserializeObject<IEnumerable<ClassInputValueDTO>>(Request.QueryString["models"]);
            ClassInputValueDTO ClassInputValueDTO = ClassInputValueDTOs.FirstOrDefault();
            ClassInputValueDTO.ParameterName = ClassInputValueDTO.ParameterName.StartsWith("@") ? ClassInputValueDTO.ParameterName : string.Format("@{0}", ClassInputValueDTO.ParameterName);
            this.AdminService.UpdateRuleConfigClassRefInputItem(ClassInputValueDTO);
            return Json(ClassInputValueDTOs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListRuleConfigExclusionsGroupRefInputList(Guid projectId, int ruleConfigId)
        {
            List<ExclusionsGroup> exclusionsGroups = this.AdminService.ListRuleConfigExclusionsGroupRefInputList(projectId, ruleConfigId).ToList();
            return Json(exclusionsGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListRuleConfigExclusionsGroupInputItems(int ruleConfigId)
        {
            List<RuleStoredProcedureInputValueExclusionsGroup> ExclusionsGroups = this.AdminService.ListRuleConfigExclusionsGroupInputItems(ruleConfigId);
            return Json(ExclusionsGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfigExclusionsGroupInputItem(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var ExclusionsGroups = JsonConvert.DeserializeObject<IEnumerable<RuleStoredProcedureInputValueExclusionsGroup>>(Request.QueryString["models"], jsonSettings);
            RuleStoredProcedureInputValueExclusionsGroup ExclusionsGroup = ExclusionsGroups.FirstOrDefault();
            ExclusionsGroup.RuleConfigurationId = ruleConfigId;
            ExclusionsGroup.ProjectId = projectId;
            ExclusionsGroup.ParameterName = ExclusionsGroup.ParameterName.StartsWith("@") ? ExclusionsGroup.ParameterName : string.Format("@{0}", ExclusionsGroup.ParameterName);

            this.AdminService.AddRuleConfigExclusionsGroupInputItem(ExclusionsGroup);
            ExclusionsGroup.ExclusionsGroupName = AdminService.ListProjectExclusionsGroups(projectId).Where(p => p.Id == ExclusionsGroup.ExclusionsGroupId).FirstOrDefault().GroupName;

            return Json(ExclusionsGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyRuleConfigExclusionsGroupInputItem()
        {
            var ExclusionsGroups = JsonConvert.DeserializeObject<IEnumerable<RuleStoredProcedureInputValueExclusionsGroup>>(Request.QueryString["models"]);
            RuleStoredProcedureInputValueExclusionsGroup ExclusionsGroup = ExclusionsGroups.FirstOrDefault();

            this.AdminService.DeleteRuleConfigExclusionsGroupInputItem(ExclusionsGroup.Id);

            return Json(ExclusionsGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfigExclusionsGroupInputItem()
        {
            var ExclusionsGroups = JsonConvert.DeserializeObject<IEnumerable<RuleStoredProcedureInputValueExclusionsGroup>>(Request.QueryString["models"]);
            RuleStoredProcedureInputValueExclusionsGroup ExclusionsGroup = ExclusionsGroups.FirstOrDefault();
            this.AdminService.UpdateRuleConfigExclusionsGroupInputItem(ExclusionsGroup);
            ExclusionsGroup.ExclusionsGroupName = AdminService.ListProjectExclusionsGroups(ExclusionsGroup.ProjectId).Where(p => p.Id == ExclusionsGroup.ExclusionsGroupId).FirstOrDefault().GroupName;
            return Json(ExclusionsGroups, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Rule Participants ]

        public JsonResult ListRuleConfigOfficeParticipants(int ruleConfigId)
        {
            List<RuleParticipantOffice> rulesPartipants = this.AdminService.ListRuleConfigOfficeParticipants(ruleConfigId);
            return Json(rulesPartipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListRuleConfigTeamParticipants(int ruleConfigId)
        {
            List<RuleParticipantTeam> rulesPartipants = this.AdminService.ListRuleConfigTeamParticipants(ruleConfigId);
            return Json(rulesPartipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListRuleConfigUserParticipants(int ruleConfigId)
        {
            List<RuleParticipantUser> rulesPartipants = this.AdminService.ListRuleConfigUserParticipants(ruleConfigId);
            return Json(rulesPartipants, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListRuleConfigProjectParticipants(int ruleConfigId)
        {
            List<RuleParticipantProject> rulesPartipants = this.AdminService.ListRuleConfigProjectParticipants(ruleConfigId);
            return Json(rulesPartipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectsForRuleParticiantsDropDown(Guid projectId, int ruleConfigId)
        {
            List<Project> projects = this.AdminService.ListProjectsForRuleParticiantsDropDown(projectId, ruleConfigId).ToList();
            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListOfficeForRuleParticiantsDropDown(Guid projectId, int ruleConfigId)
        {
            List<OfficeForDropDownDTO> offices = this.AdminService
                .ListOfficesForRuleParticiantsDropDown(projectId, ruleConfigId)
                .ToList()
                .Select(s => new OfficeForDropDownDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    NameWithActurisAKA = string.Format("{0} (AKA:{1})", s.Name, s.AlsoKnownAs),
                    NameWithActurisOrgKey = string.Format("{0} (Org Key:{1})", s.Name, s.ActurisOrganisationKey),
                    NameWithActurisOrgName = string.Format("{0} ({1})", s.Name, s.ActurisOrganisationName)
                }).ToList();
            return Json(offices, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListTeamForRuleParticiantsDropDown(Guid projectId, int ruleConfigId)
        {
            List<TeamForDropDownDTO> teams = this.AdminService
                .ListTeamsForRuleParticiantsDropDown(projectId, ruleConfigId)
                .ToList()
                .Select(s => new TeamForDropDownDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    NameWithActurisAKA = string.Format("{0} (AKA:{1})", s.Name, s.AlsoKnownAs),
                    NameWithActurisOrgKey = string.Format("{0} (Org Key:{1})", s.Name, s.ActurisOrganisationKey),
                    NameWithActurisOrgName = string.Format("{0} ({1})", s.Name, s.ActurisOrganisationName)
                }).ToList();
            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListUsersForRuleParticiantsDropDown(Guid projectId, int ruleConfigId)
        {
            List<UserForDropDownDTO> users = this.AdminService
                .ListUsersForRuleParticiantsDropDown(projectId, ruleConfigId)
                .ToList()
                .Select(s => new UserForDropDownDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    NameWithActurisAKA = string.Format("{0} (AKA:{1})", s.Name, s.AlsoKnownAs),
                    NameWithActurisOrgKey = string.Format("{0} (Org Key:{1})", s.Name, s.ActurisOrganisationKey),
                    NameWithActurisOrgName = string.Format("{0} ({1})", s.Name, s.ActurisOrganisationName)
                }).ToList(); ;
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfigOfficeParticipant(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var rulesParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantOffice>>(Request.QueryString["models"], jsonSettings);
            RuleParticipantOffice rulesParticipant = rulesParticipants.FirstOrDefault();
            //rulesprojectParticipant.ProjectId = projectId;
            rulesParticipant.RuleConfigurationId = ruleConfigId;
            rulesParticipant.ProjectId = projectId;

            this.AdminService.AddRuleConfigOfficeParticipant(rulesParticipant);
            rulesParticipant.ProjectName = AdminService.ListProjects().Where(p => p.ProjectUniqueKey == rulesParticipant.ProjectId).FirstOrDefault().ProjectName;

            Office office = AdminService.ListProjectOffices(projectId).Where(o => o.Id == rulesParticipant.OfficeId).FirstOrDefault();

            rulesParticipant.OfficeName = office.Name;
            rulesParticipant.ActurisOrganisationKey = office.ActurisOrganisationKey;
            rulesParticipant.ActurisOrganisationName = office.ActurisOrganisationName;
            rulesParticipant.AlsoKnownAs = office.AlsoKnownAs;
            rulesParticipant.Office = null;

            return Json(rulesParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfigTeamParticipant(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var rulesParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantTeam>>(Request.QueryString["models"], jsonSettings);
            RuleParticipantTeam rulesParticipant = rulesParticipants.FirstOrDefault();
            //rulesprojectParticipant.ProjectId = projectId;
            rulesParticipant.RuleConfigurationId = ruleConfigId;
            rulesParticipant.ProjectId = projectId;

            this.AdminService.AddRuleConfigTeamParticipant(rulesParticipant);
            Team team = AdminService.ListProjectTeams(projectId).Where(o => o.Id == rulesParticipant.TeamId).FirstOrDefault();
            rulesParticipant.ProjectName = AdminService.ListProjects().Where(p => p.ProjectUniqueKey == rulesParticipant.ProjectId).FirstOrDefault().ProjectName;
            rulesParticipant.TeamName = AdminService.ListProjectTeams(projectId).Where(o => o.Id == rulesParticipant.TeamId).FirstOrDefault().Name;
            rulesParticipant.ActurisOrganisationKey = team.ActurisOrganisationKey;
            rulesParticipant.ActurisOrganisationName = team.ActurisOrganisationName;
            rulesParticipant.AlsoKnownAs = team.AlsoKnownAs;
            rulesParticipant.Team = null;

            return Json(rulesParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfigUserParticipant(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var rulesParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantUser>>(Request.QueryString["models"], jsonSettings);
            RuleParticipantUser rulesParticipant = rulesParticipants.FirstOrDefault();
            //rulesprojectParticipant.ProjectId = projectId;
            rulesParticipant.RuleConfigurationId = ruleConfigId;
            rulesParticipant.ProjectId = projectId;

            this.AdminService.AddRuleConfigUserParticipant(rulesParticipant);
            User user = AdminService.ListProjectUsers(rulesParticipant.ProjectId).Where(o => o.Id == rulesParticipant.UserId).FirstOrDefault();
            rulesParticipant.ProjectName = AdminService.ListProjects().Where(p => p.ProjectUniqueKey == rulesParticipant.ProjectId).FirstOrDefault().ProjectName;
            rulesParticipant.UserName = user.Name;
            rulesParticipant.ActurisOrganisationKey = user.ActurisOrganisationKey;
            rulesParticipant.ActurisOrganisationName = user.ActurisOrganisationName;
            rulesParticipant.AlsoKnownAs = user.AlsoKnownAs;
            rulesParticipant.User = null;

            return Json(rulesParticipants, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DestroyRuleConfigOfficeParticipant()
        {
            var rulesParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantOffice>>(Request.QueryString["models"]);
            RuleParticipantOffice rulesParticipant = rulesParticipants.FirstOrDefault();

            this.AdminService.DeleteRuleConfigOfficeParticipant(rulesParticipant.Id);

            return Json(rulesParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyRuleConfigTeamParticipant()
        {
            var rulesParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantTeam>>(Request.QueryString["models"]);
            RuleParticipantTeam rulesParticipant = rulesParticipants.FirstOrDefault();

            this.AdminService.DeleteRuleConfigTeamParticipant(rulesParticipant.Id);

            return Json(rulesParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyRuleConfigUserParticipant()
        {
            var rulesParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantUser>>(Request.QueryString["models"]);
            RuleParticipantUser rulesParticipant = rulesParticipants.FirstOrDefault();

            this.AdminService.DeleteRuleConfigUserParticipant(rulesParticipant.Id);

            return Json(rulesParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfigProjectParticipant(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            var rulesprojectParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantProject>>(Request.QueryString["models"], jsonSettings);
            RuleParticipantProject rulesprojectParticipant = rulesprojectParticipants.FirstOrDefault();
            //rulesprojectParticipant.ProjectId = projectId;
            rulesprojectParticipant.RuleConfigurationId = ruleConfigId;
            rulesprojectParticipant.ProjectName = AdminService.ListProjects().Where(p => p.ProjectUniqueKey == rulesprojectParticipant.ProjectId).FirstOrDefault().ProjectName;
            this.AdminService.AddRuleConfigProjectParticipant(rulesprojectParticipant);

            return Json(rulesprojectParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyRuleConfigProjectParticipant()
        {
            var rulesprojectParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantProject>>(Request.QueryString["models"]);
            RuleParticipantProject rulesprojectParticipant = rulesprojectParticipants.FirstOrDefault();

            this.AdminService.DeleteRuleConfigProjectParticipant(rulesprojectParticipant.Id);

            return Json(rulesprojectParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfigProjectParticipant()
        {
            var ruleParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantProject>>(Request.QueryString["models"]);
            RuleParticipantProject ruleParticipant = ruleParticipants.FirstOrDefault();
            this.AdminService.UpdateRuleConfigProjectParticipant(ruleParticipant);
            return Json(ruleParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfigOfficeParticipant(int ruleConfigId)
        {
            var ruleParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantOffice>>(Request.QueryString["models"]);
            RuleParticipantOffice ruleParticipant = ruleParticipants.FirstOrDefault();
            //ruleParticipant.OfficeId = this.AdminService.ListRuleConfigOfficeParticipants(ruleConfigId).Where(a => a.Id == ruleParticipant.Id).FirstOrDefault().OfficeId;
            this.AdminService.UpdateRuleConfigOfficeParticipant(ruleParticipant);

            Office office = AdminService.ListProjectOffices(ruleParticipant.ProjectId).Where(o => o.Id == ruleParticipant.OfficeId).FirstOrDefault();
            ruleParticipant.OfficeName = office.Name;
            ruleParticipant.ActurisOrganisationKey = office.ActurisOrganisationKey;
            ruleParticipant.ActurisOrganisationName = office.ActurisOrganisationName;
            ruleParticipant.AlsoKnownAs = office.AlsoKnownAs;
            ruleParticipant.Office = null;

            return Json(ruleParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfigTeamParticipant(int ruleConfigId)
        {
            var ruleParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantTeam>>(Request.QueryString["models"]);
            RuleParticipantTeam ruleParticipant = ruleParticipants.FirstOrDefault();
            //ruleParticipant.OfficeId = this.AdminService.ListRuleConfigOfficeParticipants(ruleConfigId).Where(a => a.Id == ruleParticipant.Id).FirstOrDefault().OfficeId;
            this.AdminService.UpdateRuleConfigTeamParticipant(ruleParticipant);

            Team team = AdminService.ListProjectTeams(ruleParticipant.ProjectId).Where(o => o.Id == ruleParticipant.TeamId).FirstOrDefault();
            ruleParticipant.ProjectName = AdminService.ListProjects().Where(p => p.ProjectUniqueKey == ruleParticipant.ProjectId).FirstOrDefault().ProjectName;
            ruleParticipant.TeamName = team.Name;
            ruleParticipant.ActurisOrganisationKey = team.ActurisOrganisationKey;
            ruleParticipant.ActurisOrganisationName = team.ActurisOrganisationName;
            ruleParticipant.AlsoKnownAs = team.AlsoKnownAs;
            ruleParticipant.Team = null;

            return Json(ruleParticipants, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfigUserParticipant(int ruleConfigId)
        {
            var ruleParticipants = JsonConvert.DeserializeObject<IEnumerable<RuleParticipantUser>>(Request.QueryString["models"]);
            RuleParticipantUser ruleParticipant = ruleParticipants.FirstOrDefault();
            //ruleParticipant.OfficeId = this.AdminService.ListRuleConfigOfficeParticipants(ruleConfigId).Where(a => a.Id == ruleParticipant.Id).FirstOrDefault().OfficeId;
            this.AdminService.UpdateRuleConfigUserParticipant(ruleParticipant);

            User user = AdminService.ListProjectUsers(ruleParticipant.ProjectId).Where(o => o.Id == ruleParticipant.UserId).FirstOrDefault();
            ruleParticipant.ProjectName = AdminService.ListProjects().Where(p => p.ProjectUniqueKey == ruleParticipant.ProjectId).FirstOrDefault().ProjectName;
            ruleParticipant.UserName = user.Name;
            ruleParticipant.ActurisOrganisationKey = user.ActurisOrganisationKey;
            ruleParticipant.ActurisOrganisationName = user.ActurisOrganisationName;
            ruleParticipant.AlsoKnownAs = user.AlsoKnownAs;
            ruleParticipant.User = null;

            return Json(ruleParticipants, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Exclusions ]

        public JsonResult ListProjectExclusionsGroups(Guid projectId)
        {
            List<ExclusionsGroup> exclusionGroups = this.AdminService.ListProjectExclusionsGroups(projectId);
            return Json(exclusionGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExclusionGroupItems(int exclusionsGroupId)
        {
            List<ExclusionsItem> exclusionItems = this.AdminService.ListExclusionGroupItems(exclusionsGroupId);
            return Json(exclusionItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectExclusionsGroup(Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var exclusionGroups = JsonConvert.DeserializeObject<IEnumerable<ExclusionsGroup>>(Request.QueryString["models"], jsonSettings);
            ExclusionsGroup exclusionGroup = exclusionGroups.FirstOrDefault();
            exclusionGroup.ProjectId = projectId;
            exclusionGroup.AddedBy = User.Identity.Name;

            this.AdminService.AddProjectExclusionsGroup(exclusionGroup);

            return Json(exclusionGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateExclusionGroupItem(int exclusionsGroupId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var exclusionItems = JsonConvert.DeserializeObject<IEnumerable<ExclusionsItem>>(Request.QueryString["models"], jsonSettings);
            ExclusionsItem exclusionItem = exclusionItems.FirstOrDefault();
            exclusionItem.ProjectId = projectId;
            exclusionItem.ExclusionsGroupId = exclusionsGroupId;
            exclusionItem.AddedBy = User.Identity.Name;

            this.AdminService.AddExclusionGroupItem(exclusionItem);

            return Json(exclusionItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyProjectExclusionsGroup()
        {
            var exclusionGroups = JsonConvert.DeserializeObject<IEnumerable<ExclusionsGroup>>(Request.QueryString["models"]);
            ExclusionsGroup exclusionGroup = exclusionGroups.FirstOrDefault();
            this.AdminService.DeleteProjectExclusionsGroup(exclusionGroup.Id);
            return Json(exclusionGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyExclusionGroupItem()
        {
            var exclusionItems = JsonConvert.DeserializeObject<IEnumerable<ExclusionsItem>>(Request.QueryString["models"]);
            ExclusionsItem exclusionItem = exclusionItems.FirstOrDefault();
            this.AdminService.DestroyExclusionGroupItem(exclusionItem.Id);
            return Json(exclusionItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateProjectExclusionsGroup()
        {
            var exclusionGroups = JsonConvert.DeserializeObject<IEnumerable<ExclusionsGroup>>(Request.QueryString["models"]);
            ExclusionsGroup exclusionGroup = exclusionGroups.FirstOrDefault();
            this.AdminService.UpdateProjectExclusionsGroup(exclusionGroup);
            return Json(exclusionGroups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateExclusionGroupItem()
        {
            var exclusionItems = JsonConvert.DeserializeObject<IEnumerable<ExclusionsItem>>(Request.QueryString["models"]);
            ExclusionsItem exclusionItem = exclusionItems.FirstOrDefault();
            this.AdminService.UpdateExclusionGroupItem(exclusionItem);
            return Json(exclusionItems, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Escalations ]

        #region [ SQL Exscalations ]

        public JsonResult ListProjectEscalationSQLConfigs(Guid projectGuid)
        {
            List<EscalationsFrameworkRuleConfigSQL> rulesConfigs = this.AdminService.ListProjectEscalationEmailSQLConfigs(projectGuid);
            return Json(rulesConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectEscalationSQLConfig(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigSQL>>(Request.Unvalidated.QueryString["models"], jsonSettings);
            EscalationsFrameworkRuleConfigSQL ruleConfig = ruleConfigs.FirstOrDefault();
            string validateOutString = string.Empty;
            if (ruleConfig.ValuesAreValid(out validateOutString))
            {
                ruleConfig.ProjectId = projectGuid;
                ruleConfig.EscalationsFrameworkId = projectGuid;
                ruleConfig.BreachCount = 0;

                this.AdminService.AddProjectEscalationSQLConfig(ruleConfig);

                ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectGuid).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
                ruleConfig.TargetDbName = this.AdminService.ListProjectTargetDBDetails(projectGuid).Where(a => a.Id == ruleConfig.TargetDbID).FirstOrDefault().DisplayName;
                
                return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = validateOutString,
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DestroyProjectEscalationSQLConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigSQL>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigSQL ruleConfig = ruleConfigs.FirstOrDefault();

            this.AdminService.DeleteProjectEscalationSQLConfig(ruleConfig.Id);

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult UpdateProjectEscalationSQLConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigSQL>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigSQL ruleConfig = ruleConfigs.FirstOrDefault();

            string validateOutString = string.Empty;
            if (ruleConfig.ValuesAreValid(out validateOutString))
            {
                this.AdminService.UpdateProjectEscalationSQLConfig(ruleConfig);
                ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
                ruleConfig.TargetDbName = this.AdminService.ListProjectTargetDBDetails(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.TargetDbID).FirstOrDefault().DisplayName;
                return Json(ruleConfig, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = validateOutString,
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ Generic Escalations ]

        public JsonResult ListProjectEscalationGenericConfigs(Guid projectGuid)
        {
            List<EscalationsFrameworkRuleConfigEmailGeneric> rulesConfigs = this.AdminService.ListProjectEscalationEmailGenericConfigs(projectGuid);
            return Json(rulesConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectEscalationGenericConfig(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailGeneric>>(Request.Unvalidated.QueryString["models"], jsonSettings);
            EscalationsFrameworkRuleConfigEmailGeneric ruleConfig = ruleConfigs.FirstOrDefault();
            string validateOutString = string.Empty;
            if (ruleConfig.ValuesAreValid(out validateOutString))
            {
                ruleConfig.ProjectId = projectGuid;
                ruleConfig.EscalationsFrameworkId = projectGuid;
                ruleConfig.BreachCount = 0;
                this.AdminService.AddProjectEscalationGenericConfig(ruleConfig);
                ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectGuid).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
                
                return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = validateOutString,
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DestroyProjectEscalationGenericConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailGeneric>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigEmailGeneric ruleConfig = ruleConfigs.FirstOrDefault();

            this.AdminService.DeleteProjectEscalationGenericConfig(ruleConfig.Id);

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult UpdateProjectEscalationGenericConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailGeneric>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigEmailGeneric ruleConfig = ruleConfigs.FirstOrDefault();

            string validateOutString = string.Empty;
            if (ruleConfig.ValuesAreValid(out validateOutString))
            {
                this.AdminService.UpdateProjectEscalationGenericConfig(ruleConfig);
                ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
                return Json(ruleConfig, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = validateOutString,
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ListEmailActionOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<EscalationEmailGenericActionEnum>(EscalationEmailGenericActionEnum.AmalgamatedHtml_ToSpecifiedLocation_All);
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListBreachInclusionOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<EscalationInclusionEnum>(EscalationInclusionEnum.AllBreaches);
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult ListEscalationsEmailHistory(int escalationsConfigId, [DataSourceRequest] DataSourceRequest request)
        {
            List<EscalationsEmailDTO> escalationsEmails = this.AdminService.ListEscalationsEmailHistory(escalationsConfigId);
            Kendo.Mvc.UI.DataSourceResult result = escalationsEmails.ToDataSourceResult(request);
            return Json(new { data = result.Data, total = result.Total }, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult EscalationsEmailHistory(string EscalationsConfigId, string ParentDivID, string AdminRoot)
        {
            string a = EscalationsConfigId;
            //var db = new EFServiceStatusHistoryRepository();
            //IList<ServiceStatusHistory> logs = db.GetAllStatusLogs(id);
            EscalationsEmailHistoryDTO returnModel = new EscalationsEmailHistoryDTO()
            {
                EscalationsConfigId = EscalationsConfigId,
                ParentDivID = ParentDivID,
                AdminRoot = AdminRoot
            };
            return PartialView("_EmailHistory", returnModel);
        }
        public JsonResult GetEscalationsEmailBody(int ItemId)
        {
            string escalationsEmailBody = this.AdminService.GetEscalationsEmailBody(ItemId);

            return Json(escalationsEmailBody, JsonRequestBehavior.AllowGet);
        }

        public class EscalationsEmailHistoryDTO
        {
            public string EscalationsConfigId { get; set; }
            public string ParentDivID { get; set; }
            public string AdminRoot { get; set; }
        }
        private class RazorBodyTemplates
        {
            public string Id { get; set; }
            public string FileName { get; set; }
        }
        public JsonResult GetProjectBodyRazorTemplateNames(Guid projectId)
        {
            Helper h = new Helper();
            Project p = AdminService.ListProjects().Where(a => a.ProjectUniqueKey == projectId).FirstOrDefault();
            string bodyTemplatePath = h.GetEmailRazorTemplateBodyPath(this, p);

            string[] filePaths = Directory.GetFiles(bodyTemplatePath);
            List<RazorBodyTemplates> returnList = filePaths.Select(a => new RazorBodyTemplates() { Id = a.Substring(a.LastIndexOf('\\') + 1), FileName = a.Substring(a.LastIndexOf('\\') + 1) }).ToList();
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectAttachmentRazorTemplateNames(Guid projectId)
        {
            Helper h = new Helper();
            Project p = AdminService.ListProjects().Where(a => a.ProjectUniqueKey == projectId).FirstOrDefault();
            string subjectTemplatePath = h.GetEmailRazorTemplateAttachmentPath(this, p);
            string[] filePaths = Directory.GetFiles(subjectTemplatePath);
            List<RazorBodyTemplates> returnList = filePaths.Select(a => new RazorBodyTemplates() { Id = a.Substring(a.LastIndexOf('\\') + 1), FileName = a.Substring(a.LastIndexOf('\\') + 1) }).OrderBy(a => a.FileName).ToList();
            returnList.Insert(0, new RazorBodyTemplates() { FileName = "", Id = "0" });
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectSubjectRazorTemplateNames(Guid projectId)
        {
            Helper h = new Helper();
            Project p = AdminService.ListProjects().Where(a => a.ProjectUniqueKey == projectId).FirstOrDefault();
            string subjectTemplatePath = h.GetEmailRazorTemplateSubjectPath(this, p);
            string[] filePaths = Directory.GetFiles(subjectTemplatePath);
            List<RazorBodyTemplates> returnList = filePaths.Select(a => new RazorBodyTemplates() { Id = a.Substring(a.LastIndexOf('\\') + 1), FileName = a.Substring(a.LastIndexOf('\\') + 1) }).ToList();
            return Json(returnList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExecuteEscalationsRuleRoleConfig(int escalationConfigId, Guid projectId)
        {
            string returnText = string.Empty;
            Helper h = new Helper();
            Project p = AdminService.ListProjects().Where(proj => proj.ProjectUniqueKey == projectId).FirstOrDefault();
            string bodyTemplatePath = h.GetEmailRazorTemplateBodyPath(this, p);
            string subjectTemplatePath = h.GetEmailRazorTemplateSubjectPath(this, p);
            string attachmentTemplatePath = h.GetEmailRazorTemplateAttachmentPath(this, p);
            returnText = this.AdminService.ExecuteEscalationsRuleRoleConfig(escalationConfigId, bodyTemplatePath, subjectTemplatePath, attachmentTemplatePath);
            return Json(string.IsNullOrEmpty(returnText) ? null : returnText, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExecuteEscalationsRuleUserConfig(int escalationConfigId, Guid projectId)
        {
            string returnText = string.Empty;
            Helper h = new Helper();
            Project p = AdminService.ListProjects().Where(proj => proj.ProjectUniqueKey == projectId).FirstOrDefault();
            string bodyTemplatePath = h.GetEmailRazorTemplateBodyPath(this, p);
            string subjectTemplatePath = h.GetEmailRazorTemplateSubjectPath(this, p);
            string attachmentTemplatePath = h.GetEmailRazorTemplateAttachmentPath(this, p);
            returnText = this.AdminService.ExecuteEscalationsRuleUserConfig(escalationConfigId, bodyTemplatePath, subjectTemplatePath, attachmentTemplatePath);
            return Json(string.IsNullOrEmpty(returnText) ? null : returnText, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListProjectDropDownEditorEFProjectBreachSource(Guid projectId, int escalationsConfigId)
        {
            List<Project> projects = this.AdminService.ListProjectDropDownEditorEFUserProjectBreachSource(projectId, escalationsConfigId).ToList();
            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectDropDownEditorEFPOfficeBreachSource(Guid projectId, int escalationsConfigId)
        {
            List<OfficeForDropDownDTO> offices = this.AdminService
                .ListProjectDropDownEditorEFUserOfficeBreachSource(projectId, escalationsConfigId)
                .ToList()
                .Select(s => new OfficeForDropDownDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    NameWithActurisAKA = string.Format("{0} (AKA:{1})", s.Name, s.AlsoKnownAs),
                    NameWithActurisOrgKey = string.Format("{0} (Org Key:{1})", s.Name, s.ActurisOrganisationKey),
                    NameWithActurisOrgName = string.Format("{0} ({1})", s.Name, s.ActurisOrganisationName)
                }).ToList();
            return Json(offices, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectDropDownEditorEFTeamBreachSource(Guid projectId, int escalationsConfigId)
        {
            List<TeamForDropDownDTO> teams = this.AdminService.ListProjectDropDownEditorEFTeamBreachSource(projectId, escalationsConfigId)
                .ToList()
                .Select(s => new TeamForDropDownDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    NameWithActurisAKA = string.Format("{0} (AKA:{1})", s.Name, s.AlsoKnownAs),
                    NameWithActurisOrgKey = string.Format("{0} (Org Key:{1})", s.Name, s.ActurisOrganisationKey),
                    NameWithActurisOrgName = string.Format("{0} ({1})", s.Name, s.ActurisOrganisationName)
                }).ToList();
            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectDropDownEditorEFUserBreachSource(Guid projectId, int escalationsConfigId)
        {
            List<UserForDropDownDTO> teams = this.AdminService
                .ListProjectDropDownEditorEFUserBreachSource(projectId, escalationsConfigId)
                .ToList()
                .Select(s => new UserForDropDownDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    NameWithActurisAKA = string.Format("{0} (AKA:{1})", s.Name, s.AlsoKnownAs),
                    NameWithActurisOrgKey = string.Format("{0} (Org Key:{1})", s.Name, s.ActurisOrganisationKey),
                    NameWithActurisOrgName = string.Format("{0} ({1})", s.Name, s.ActurisOrganisationName)
                }).ToList(); ;
            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectDropDownEditorEFRuleBreachSource(Guid projectId, int escalationsConfigId)
        {
            List<Rule> teams = this.AdminService.ListProjectDropDownEditorEFRuleBreachSource(projectId, escalationsConfigId).ToList();
            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectDropDownEditorEFRuleConfigBreachSource(Guid projectId, int escalationsConfigId)
        {
            List<RuleConfiguration> teams = this.AdminService.ListProjectDropDownEditorEFRuleConfigBreachSource(projectId, escalationsConfigId).ToList();
            return Json(teams, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEFRuleConfigProjectBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceProject> breachSources = this.AdminService.ListEFRuleConfigProjectBreachSources(ruleConfigId);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEFRuleConfigOfficeBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceOffice> breachSources = this.AdminService.ListEFRuleConfigOfficeBreachSources(ruleConfigId);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEFRuleConfigTeamBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceTeam> breachSources = this.AdminService.ListEFRuleConfigTeamBreachSources(ruleConfigId);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEFRuleConfigUserBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceUser> breachSources = this.AdminService.ListEFRuleConfigUserBreachSources(ruleConfigId);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEFRuleConfigRuleBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceRule> breachSources = this.AdminService.ListEFRuleConfigRuleBreachSources(ruleConfigId);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEFRuleConfigRuleConfigBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceRuleConfiguration> breachSources = this.AdminService.ListEFRuleConfigRuleConfigBreachSources(ruleConfigId);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateEFRuleConfigProjectBreachSource(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceProject>>(Request.QueryString["models"], jsonSettings);
            EscalationsFrameworkBreachSourceProject breachSource = breachSources.FirstOrDefault();
            breachSource.ProjectId = projectId;
            breachSource.EscalationsFrameworkRuleConfigId = ruleConfigId;
            this.AdminService.AddEFRuleConfigProjectBreachSource(breachSource);
            breachSource.ProjectName = this.AdminService.ListProjects().Where(a => a.ProjectUniqueKey == projectId).FirstOrDefault().ProjectDisplayName;

            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateEFRuleConfigOfficeBreachSource(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceOffice>>(Request.QueryString["models"], jsonSettings);
            EscalationsFrameworkBreachSourceOffice breachSource = breachSources.FirstOrDefault();
            breachSource.ProjectId = projectId;
            breachSource.EscalationsFrameworkRuleConfigId = ruleConfigId;
            this.AdminService.AddEFRuleConfigOfficeBreachSource(breachSource);

            Office office = AdminService.ListProjectOffices(breachSource.ProjectId).Where(o => o.Id == breachSource.OfficeId).FirstOrDefault();
            breachSource.OfficeName = office.Name;
            breachSource.ActurisOrganisationKey = office.ActurisOrganisationKey;
            breachSource.ActurisOrganisationName = office.ActurisOrganisationName;
            breachSource.AlsoKnownAs = office.AlsoKnownAs;
            breachSource.Office = null;

            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateEFRuleConfigTeamBreachSource(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceTeam>>(Request.QueryString["models"], jsonSettings);
            EscalationsFrameworkBreachSourceTeam breachSource = breachSources.FirstOrDefault();
            breachSource.ProjectId = projectId;
            breachSource.EscalationsFrameworkRuleConfigId = ruleConfigId;
            this.AdminService.AddEFRuleConfigTeamBreachSource(breachSource);

            Team team = AdminService.ListProjectTeams(breachSource.ProjectId).Where(o => o.Id == breachSource.TeamID).FirstOrDefault();
            breachSource.TeamName = team.Name;
            breachSource.ActurisOrganisationKey = team.ActurisOrganisationKey;
            breachSource.ActurisOrganisationName = team.ActurisOrganisationName;
            breachSource.AlsoKnownAs = team.AlsoKnownAs;
            breachSource.Team = null;

            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateEFRuleConfigUserBreachSource(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceUser>>(Request.QueryString["models"], jsonSettings);
            EscalationsFrameworkBreachSourceUser breachSource = breachSources.FirstOrDefault();
            breachSource.ProjectId = projectId;
            breachSource.EscalationsFrameworkRuleConfigId = ruleConfigId;
            this.AdminService.AddEFRuleConfigUserBreachSource(breachSource);

            User user = AdminService.ListProjectUsers(breachSource.ProjectId).Where(o => o.Id == breachSource.UserId).FirstOrDefault();
            breachSource.UserName = user.Name;
            breachSource.ActurisOrganisationKey = user.ActurisOrganisationKey;
            breachSource.ActurisOrganisationName = user.ActurisOrganisationName;
            breachSource.AlsoKnownAs = user.AlsoKnownAs;
            breachSource.User = null;

            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateEFRuleConfigRuleBreachSource(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceRule>>(Request.QueryString["models"], jsonSettings);
            EscalationsFrameworkBreachSourceRule breachSource = breachSources.FirstOrDefault();
            breachSource.ProjectId = projectId;
            breachSource.EscalationsFrameworkRuleConfigId = ruleConfigId;
            this.AdminService.AddEFRuleConfigRuleBreachSource(breachSource);
            breachSource.RuleName = this.AdminService.ListProjectRules(projectId).Where(a => a.Id == breachSource.RuleId).FirstOrDefault().Name;

            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateEFRuleConfigRuleConfigBreachSource(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceRuleConfiguration>>(Request.QueryString["models"], jsonSettings);
            EscalationsFrameworkBreachSourceRuleConfiguration breachSource = breachSources.FirstOrDefault();
            breachSource.ProjectId = projectId;
            breachSource.EscalationsFrameworkRuleConfigId = ruleConfigId;
            this.AdminService.AddEFRuleConfigRuleConfigBreachSource(breachSource);
            breachSource.RuleConfigName = this.AdminService.ListProjectRuleConfigurations(projectId).Where(a => a.Id == breachSource.RuleConfigurationId).FirstOrDefault().Name;

            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyEFRuleConfigProjectBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceProject>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceProject breachSource = breachSources.FirstOrDefault();
            this.AdminService.DeleteEFRuleConfigProjectBreachSource(breachSource.Id);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyEFRuleConfigOfficeBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceOffice>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceOffice breachSource = breachSources.FirstOrDefault();
            this.AdminService.DeleteEFRuleConfigOfficeBreachSource(breachSource.Id);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyEFRuleConfigTeamBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceTeam>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceTeam breachSource = breachSources.FirstOrDefault();
            this.AdminService.DeleteEFRuleConfigTeamBreachSource(breachSource.Id);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyEFRuleConfigUserBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceUser>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceUser breachSource = breachSources.FirstOrDefault();
            this.AdminService.DeleteEFRuleConfigUserBreachSource(breachSource.Id);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyEFRuleConfigRuleBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceRule>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceRule breachSource = breachSources.FirstOrDefault();
            this.AdminService.DeleteEFRuleConfigRuleBreachSource(breachSource.Id);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyEFRuleConfigRuleConfigBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceRuleConfiguration>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceRuleConfiguration breachSource = breachSources.FirstOrDefault();
            this.AdminService.DeleteEFRuleConfigRuleConfigBreachSource(breachSource.Id);
            return Json(breachSources, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEFRuleConfigProjectBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceProject>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceProject breachSource = breachSources.FirstOrDefault();
            this.AdminService.UpdateEFRuleConfigProjectBreachSource(breachSource);
            return Json(breachSource, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEFRuleConfigOfficeBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceOffice>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceOffice breachSource = breachSources.FirstOrDefault();
            this.AdminService.UpdateEFRuleConfigOfficeBreachSource(breachSource);

            Office office = AdminService.ListProjectOffices(breachSource.ProjectId).Where(o => o.Id == breachSource.OfficeId).FirstOrDefault();
            breachSource.OfficeName = office.Name;
            breachSource.ActurisOrganisationKey = office.ActurisOrganisationKey;
            breachSource.ActurisOrganisationName = office.ActurisOrganisationName;
            breachSource.AlsoKnownAs = office.AlsoKnownAs;
            breachSource.Office = null;

            return Json(breachSource, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEFRuleConfigTeamBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceTeam>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceTeam breachSource = breachSources.FirstOrDefault();
            this.AdminService.UpdateEFRuleConfigTeamBreachSource(breachSource);

            Team team = AdminService.ListProjectTeams(breachSource.ProjectId).Where(o => o.Id == breachSource.TeamID).FirstOrDefault();
            breachSource.TeamName = team.Name;
            breachSource.ActurisOrganisationKey = team.ActurisOrganisationKey;
            breachSource.ActurisOrganisationName = team.ActurisOrganisationName;
            breachSource.AlsoKnownAs = team.AlsoKnownAs;
            breachSource.Team = null;

            return Json(breachSource, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEFRuleConfigUserBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceUser>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceUser breachSource = breachSources.FirstOrDefault();
            this.AdminService.UpdateEFRuleConfigUserBreachSource(breachSource);

            User user = AdminService.ListProjectUsers(breachSource.ProjectId).Where(o => o.Id == breachSource.UserId).FirstOrDefault();
            breachSource.UserName = user.Name;
            breachSource.ActurisOrganisationKey = user.ActurisOrganisationKey;
            breachSource.ActurisOrganisationName = user.ActurisOrganisationName;
            breachSource.AlsoKnownAs = user.AlsoKnownAs;
            breachSource.User = null;

            return Json(breachSource, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEFRuleConfigRuleBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceRule>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceRule breachSource = breachSources.FirstOrDefault();
            this.AdminService.UpdateEFRuleConfigRuleBreachSource(breachSource);
            return Json(breachSource, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEFRuleConfigRuleConfigBreachSource()
        {
            var breachSources = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkBreachSourceRuleConfiguration>>(Request.QueryString["models"]);
            EscalationsFrameworkBreachSourceRuleConfiguration breachSource = breachSources.FirstOrDefault();
            this.AdminService.UpdateEFRuleConfigRuleConfigBreachSource(breachSource);
            return Json(breachSource, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectEscalationRolesConfigs(Guid projectGuid)
        {
            List<EscalationsFrameworkRuleConfigEmailRole> rulesConfigs = this.AdminService.ListProjectEscalationEmailRoleConfigs(projectGuid);
            return Json(rulesConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEscalationUsersRuleConfigRecipients(int escalationsFrameworkRuleConfigId)
        {
            List<EscalationsEmailRecipient> recipients = this.AdminService.ListProjectEscalationEmailRoleConfigRecipients(escalationsFrameworkRuleConfigId);
            return Json(recipients, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectEscalationUsersConfigs(Guid projectGuid)
        {
            List<EscalationsFrameworkRuleConfigEmailUser> rulesConfigs = this.AdminService.ListProjectEscalationEmailUserConfigs(projectGuid);
            return Json(rulesConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectEscalationRolesConfig(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailRole>>(Request.Unvalidated.QueryString["models"], jsonSettings);
            EscalationsFrameworkRuleConfigEmailRole ruleConfig = ruleConfigs.FirstOrDefault();
            ruleConfig.ProjectId = projectGuid;
            ruleConfig.EscalationsFrameworkId = projectGuid;
            this.AdminService.AddProjectEscalationRolesConfig(ruleConfig);
            ruleConfig.ActionName = this.AdminService.ListActionsForEscalationConfigRolesDropDown().Where(a => a.Id == ruleConfig.ActionId).FirstOrDefault().Name;
            ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectGuid).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateEscalationUsersRuleConfigRecipient(int escalationsFrameworkRuleConfigId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var recipients = JsonConvert.DeserializeObject<IEnumerable<EscalationsEmailRecipient>>(Request.QueryString["models"], jsonSettings);
            EscalationsEmailRecipient recipient = recipients.FirstOrDefault();
            recipient.EscalationsFrameworkRuleConfigId = escalationsFrameworkRuleConfigId;
            this.AdminService.AddPEscalationUsersRuleConfigRecipient(recipient);
            User recipientUser = this.AdminService.ListAllSystemUsersWithPermissions().Where(a => a.Id == recipient.RecipientId).FirstOrDefault();
            recipient.RecipientName = recipientUser.Name;
            recipient.RecipientEmail = recipientUser.Email;

            return Json(recipients, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateProjectEscalationUsersConfig(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailUser>>(Request.Unvalidated.QueryString["models"], jsonSettings);
            EscalationsFrameworkRuleConfigEmailUser ruleConfig = ruleConfigs.FirstOrDefault();
            ruleConfig.ProjectId = projectGuid;
            ruleConfig.EscalationsFrameworkId = projectGuid;
            this.AdminService.AddProjectEscalationUsersConfig(ruleConfig);
            ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectGuid).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListActionsForEscalationConfigRolesDropDown()
        {
            List<EscalationsFrameworkAction> actions = this.AdminService.ListActionsForEscalationConfigRolesDropDown();
            return Json(actions, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListUsersForEscalationsConfigParticipantsDropDown(Guid projectId, int selectedConfigId)
        {
            List<User> actions = this.AdminService.ListUsersForEscalationsConfigParticipantsDropDown(projectId, selectedConfigId);
            return Json(actions, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyEscalationUsersRuleConfigRecipient()
        {
            var recipients = JsonConvert.DeserializeObject<IEnumerable<EscalationsEmailRecipient>>(Request.QueryString["models"]);
            EscalationsEmailRecipient recipient = recipients.FirstOrDefault();

            this.AdminService.DeleteEscalationUsersRuleConfigRecipient(recipient.Id);

            return Json(recipients, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyProjectEscalationUsersConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailUser>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigEmailUser ruleConfig = ruleConfigs.FirstOrDefault();

            this.AdminService.DeleteProjectEscalationUsersConfig(ruleConfig.Id);

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult DestroyProjectEscalationRolesConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailRole>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigEmailRole ruleConfig = ruleConfigs.FirstOrDefault();

            this.AdminService.DeleteProjectEscalationRolesConfig(ruleConfig.Id);

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult UpdateProjectEscalationUsersConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailUser>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigEmailUser ruleConfig = ruleConfigs.FirstOrDefault();
            this.AdminService.UpdateProjectEscalationUsersConfig(ruleConfig);
            ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
            return Json(ruleConfig, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateProjectEscalationRolesConfig()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<EscalationsFrameworkRuleConfigEmailRole>>(Request.Unvalidated.QueryString["models"]);
            EscalationsFrameworkRuleConfigEmailRole ruleConfig = ruleConfigs.FirstOrDefault();
            this.AdminService.UpdateProjectEscalationRolesConfig(ruleConfig);
            ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
            return Json(ruleConfig, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateEscalationUsersRuleConfigRecipient(Guid projectId)
        {
            var recipients = JsonConvert.DeserializeObject<IEnumerable<EscalationsEmailRecipient>>(Request.QueryString["models"]);
            EscalationsEmailRecipient recipient = recipients.FirstOrDefault();
            this.AdminService.UpdateEscalationUsersRuleConfigRecipient(recipient);
            User user = this.AdminService.ListProjectUsersWithPermissions(projectId).Where(u => u.Id == recipient.RecipientId).FirstOrDefault();
            recipient.RecipientName = user.Name;
            recipient.RecipientEmail = user.Email;
            return Json(recipients, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Rules ]

        public JsonResult ListRuleConfigPreExecuteBreachesActionOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<BreachActionEnum>(BreachActionEnum.Archive);
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        #region [ sql Rule ]

        public JsonResult ListProjectRules(Guid projectGuid)
        {
            List<Rule> rules = this.AdminService.ListProjectRules(projectGuid);
            return Json(rules, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRule()
        {
            var rules = JsonConvert.DeserializeObject<IEnumerable<Rule>>(Request.QueryString["models"]);
            Rule rule = rules.FirstOrDefault();
            this.AdminService.UpdateRule(rule);
            return Json(rules, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyRule()
        {
            var rules = JsonConvert.DeserializeObject<IEnumerable<Rule>>(Request.QueryString["models"]);
            Rule rule = rules.FirstOrDefault();

            this.AdminService.DeleteRule(rule.Id);

            return Json(rules, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRule(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var rules = JsonConvert.DeserializeObject<IEnumerable<Rule>>(Request.QueryString["models"], jsonSettings);
            Rule rule = rules.FirstOrDefault();
            rule.ProjectId = projectGuid;
            this.AdminService.AddRule(rule);

            return Json(rules, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListRuleConfigurations(int ruleId)
        {
            List<RuleConfiguration> ruleConfigs = this.AdminService.ListRuleConfigurations(ruleId);
            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRuleConfiguration(int ruleId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<RuleConfiguration>>(Request.QueryString["models"], jsonSettings);
            RuleConfiguration ruleConfig = ruleConfigs.FirstOrDefault();
            ruleConfig.RuleId = ruleId;
            ruleConfig.ProjectId = projectId;
            ruleConfig.UserTargetExecutionMode = RuleConfigExecutionMode.UserReferencedResults;

            Project project = this.AdminService.ListProjects().Where(p => p.ProjectUniqueKey == projectId).FirstOrDefault();
            ruleConfig.RuleTarget = project.ProjectTypeId.GetValueOrDefault() == (int)ProjectTypeEnum.VirtualTrainer ? RuleTarget.User : RuleTarget.LogAllReturnedBreaches;

            this.AdminService.AddRuleConfiguration(ruleConfig);

            ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
            ruleConfig.TargetDbName = this.AdminService.ListProjectTargetDBDetails(projectId).Where(a => a.Id == ruleConfig.TargetDbID).FirstOrDefault().DisplayName;
            ruleConfig.SetBreachesToResolvedScheduleName = this.AdminService.ListProjectSchedules(projectId).Where(a => a.Id == ruleConfig.SetBreachesToResolvedScheduleId).FirstOrDefault().Name;

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DestroyRuleConfiguration()
        {
            var ruleconfigs = JsonConvert.DeserializeObject<IEnumerable<RuleConfiguration>>(Request.QueryString["models"]);
            RuleConfiguration ruleConfig = ruleconfigs.FirstOrDefault();

            this.AdminService.DeleteRuleConfiguration(ruleConfig.Id);

            return Json(ruleconfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRuleConfiguration()
        {
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<RuleConfiguration>>(Request.QueryString["models"]);
            RuleConfiguration ruleConfig = ruleConfigs.FirstOrDefault();
            this.AdminService.UpdateRuleConfiguration(ruleConfig);
            ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
            ruleConfig.TargetDbName = this.AdminService.ListProjectTargetDBDetails(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.TargetDbID).FirstOrDefault().DisplayName;
            ruleConfig.SetBreachesToResolvedScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.SetBreachesToResolvedScheduleId).FirstOrDefault().Name;
            return Json(ruleConfig, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListSchedulesForRuleConfigDropDown(Guid projectId)
        {
            List<Schedule> schedules = this.AdminService.ListSchedulesForRuleConfigDropDown(projectId);
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListTargetDbsForRuleConfigDropDown(Guid projectId)
        {
            List<TargetDatabaseDetails> schedules = this.AdminService.ListTargetDbsForRuleConfigDropDown(projectId);
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Exchange Rule ]

        public JsonResult ListExchangeRuleConfigBreachTableColumnsFilterOptions(int exchangeRuleConfigId, int currentExtractionConfigId)
        {
            // Get all Breach mapping fields
            List<string> allMappingFields = BreachLog.GetMappingFields();

            // get all breach mapping fields already used for this rule config
            List<string> existingMappings = this.AdminService.GetBreachMappingFieldNamesForExchangeRuleConfig(exchangeRuleConfigId);

            if (currentExtractionConfigId != -1)
            {
                // Remove the current item from the currents list.
                ExchangeEmailRuleConfigBreachFieldMappings currentExtractionConfig = this.AdminService.ListExchangeRuleConfigValueMappingItems(exchangeRuleConfigId).Where(a => a.Id == currentExtractionConfigId).FirstOrDefault();
                existingMappings.Remove(currentExtractionConfig.MappedToBreachTableColumnName);
            }
            // deduct one from other to get fields not yet mapped to.
            List<string> differenceFields = allMappingFields.Except(existingMappings).ToList();

            // Add to dto list
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();
            foreach (string field in differenceFields)
            {
                options.Add(new GenericNameValueObject_DTO() { Name = field });
            }

            options.OrderBy(a => a.Name);

            return Json(options, JsonRequestBehavior.AllowGet);
        }
        private List<GenericNameValueObject_DTO> GetEnumNameValues<e>(e enumType, params e[] exclusions)
        {
            Type type = enumType.GetType();

            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();
            foreach (var enumValue in Enum.GetValues(type))
            {
                if (!exclusions.Contains<e>((e)enumValue))
                {
                    options.Add(new GenericNameValueObject_DTO() { Id = (int)enumValue, Name = enumValue.ToString() });
                }
            }

            return options.OrderBy(a => a.Name).ToList();
        }
        public JsonResult EmailMessageAttachmentDocTypeDropDownFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<AJGExchangeMessageAttachmentDocumentTypes>(AJGExchangeMessageAttachmentDocumentTypes.csv);
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEmailMessgaeSearchTypeDropDownFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<AJGExchangeMessageSearchType>(AJGExchangeMessageSearchType.AbsoluteValue);
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListEmailSearchSourceDropDownFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<AJGExchangeMessageDataExtractionSource>(AJGExchangeMessageDataExtractionSource.Attachment);
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeRuleConfigValueMappingItems(int ruleConfigId)
        {
            List<ExchangeEmailRuleConfigBreachFieldMappings> ruleConfigDataMappings = this.AdminService.ListExchangeRuleConfigValueMappingItems(ruleConfigId);
            return Json(ruleConfigDataMappings, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DestroyExchangeRuleConfigValueMappingItems()
        {
            var ruleConfigBreachFieldMappings = JsonConvert.DeserializeObject<IEnumerable<ExchangeEmailRuleConfigBreachFieldMappings>>(Request.QueryString["models"]);
            ExchangeEmailRuleConfigBreachFieldMappings ruleConfigBreachFieldMapping = ruleConfigBreachFieldMappings.FirstOrDefault();
            this.AdminService.DeleteRuleConfigBreachFieldMappings(ruleConfigBreachFieldMapping.Id);

            return Json(ruleConfigBreachFieldMapping, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateExchangeRuleConfigValueMappingItems()
        {
            try
            {
                var ruleConfigBreachFieldMappings = JsonConvert.DeserializeObject<IEnumerable<ExchangeEmailRuleConfigBreachFieldMappings>>(Request.QueryString["models"]);
                ExchangeEmailRuleConfigBreachFieldMappings ruleConfigBreachFieldMapping = ruleConfigBreachFieldMappings.FirstOrDefault();

                this.AdminService.UpdateExchangeRuleConfigValueMappingItems(ruleConfigBreachFieldMapping);

                ruleConfigBreachFieldMapping.UpdateNotMappedFields();

                return Json(ruleConfigBreachFieldMappings, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateExchangeRuleConfigValueMappingItems(int ruleConfigId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<ExchangeEmailRuleConfigBreachFieldMappings>>(Request.QueryString["models"], jsonSettings);
            ExchangeEmailRuleConfigBreachFieldMappings ruleConfig = ruleConfigs.FirstOrDefault();

            ruleConfig.ExchangeRuleConfigId = ruleConfigId;
            ruleConfig.ProjectId = projectId;
            //ruleConfig.OperatorForSubjectAndDate = ruleConfig.OperatorForSubjectAndDate == null ? AJGExchangeLogicalOperator.None : ruleConfig.OperatorForSubjectAndDate;
            //ruleConfig.OperatorForSentFromAndSubject = ruleConfig.OperatorForSentFromAndSubject == null ? AJGExchangeLogicalOperator.None : ruleConfig.OperatorForSentFromAndSubject;
            //ruleConfig.ExchangeItemDeleteMode = ruleConfig.ExchangeItemDeleteMode == null ? AJGExchangeDeleteMode.None : ruleConfig.ExchangeItemDeleteMode;
            //ruleConfig.SentFromFilter = ruleConfig.SentFromFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.SentFromFilter;
            //ruleConfig.ReceivedDate = ruleConfig.ReceivedDate == null ? AJGDateSelection.None : ruleConfig.ReceivedDate;
            //ruleConfig.SubjectFilter = ruleConfig.SubjectFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.SubjectFilter;
            //ruleConfig.ReceivedDateOneFilter = ruleConfig.ReceivedDateOneFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.ReceivedDateOneFilter;
            //ruleConfig.ReceivedDateOneOffsetPeriod = ruleConfig.ReceivedDateOneOffsetPeriod == null ? AJGDateOffsetPeriod.None : ruleConfig.ReceivedDateOneOffsetPeriod;
            //ruleConfig.ReceivedDateTwoFilter = ruleConfig.ReceivedDateTwoFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.ReceivedDateTwoFilter;
            //ruleConfig.ReceivedDateTwoOffsetPeriod = ruleConfig.ReceivedDateTwoOffsetPeriod == null ? AJGDateOffsetPeriod.None : ruleConfig.ReceivedDateTwoOffsetPeriod;

            this.AdminService.AddExchangeRuleConfigValueMapping(ruleConfig);

            ruleConfig.UpdateNotMappedFields();

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeItemDeleteModeOptions()
        {
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();
            foreach (AJGExchangeDeleteMode enumValue in Enum.GetValues(typeof(AJGExchangeDeleteMode)))
            {
                options.Add(new GenericNameValueObject_DTO() { Id = (int)enumValue, Name = enumValue.ToString() });
            }
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListStringFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();

            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.None, Name = AJGExchangeMessageSearchFilter.None.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.ContainsSubString, Name = AJGExchangeMessageSearchFilter.ContainsSubString.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsEqualTo, Name = AJGExchangeMessageSearchFilter.IsEqualTo.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsNotEqual, Name = AJGExchangeMessageSearchFilter.IsNotEqual.ToString() });

            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeItemOperatorFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();
            foreach (AJGExchangeLogicalOperator enumValue in Enum.GetValues(typeof(AJGExchangeLogicalOperator)))
            {
                options.Add(new GenericNameValueObject_DTO() { Id = (int)enumValue, Name = enumValue.ToString() });
            }
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeItemReceivedDatedateFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();
            foreach (AJGDateSelection enumValue in Enum.GetValues(typeof(AJGDateSelection)))
            {
                options.Add(new GenericNameValueObject_DTO() { Id = (int)enumValue, Name = enumValue.ToString() });
            }
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeItemDateOffsetFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();
            foreach (AJGDateOffsetPeriod enumValue in Enum.GetValues(typeof(AJGDateOffsetPeriod)))
            {
                options.Add(new GenericNameValueObject_DTO() { Id = (int)enumValue, Name = enumValue.ToString() });
            }
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeItemSubjectFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();

            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.None, Name = AJGExchangeMessageSearchFilter.None.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.ContainsSubString, Name = AJGExchangeMessageSearchFilter.ContainsSubString.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsEqualTo, Name = AJGExchangeMessageSearchFilter.IsEqualTo.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsNotEqual, Name = AJGExchangeMessageSearchFilter.IsNotEqual.ToString() });

            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeItemReceivedDateFilterOptions()
        {
            List<GenericNameValueObject_DTO> options = new List<GenericNameValueObject_DTO>();

            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.None, Name = AJGExchangeMessageSearchFilter.None.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsEqualTo, Name = AJGExchangeMessageSearchFilter.IsEqualTo.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsNotEqual, Name = AJGExchangeMessageSearchFilter.IsNotEqual.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsGreaterThan, Name = AJGExchangeMessageSearchFilter.IsGreaterThan.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsGreaterThanOrEqualTo, Name = AJGExchangeMessageSearchFilter.IsGreaterThanOrEqualTo.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsLessThan, Name = AJGExchangeMessageSearchFilter.IsLessThan.ToString() });
            options.Add(new GenericNameValueObject_DTO() { Id = (int)AJGExchangeMessageSearchFilter.IsLessThanOrEqualTo, Name = AJGExchangeMessageSearchFilter.IsLessThanOrEqualTo.ToString() });

            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExchangeRuleConfigurations(int ruleId)
        {
            List<ExchangeEmailRuleConfig> ruleConfigs = this.AdminService.ListExchangeRuleConfigurations(ruleId);
            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateExchangeRuleConfiguration(int ruleId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<ExchangeEmailRuleConfig>>(Request.QueryString["models"], jsonSettings);
            ExchangeEmailRuleConfig ruleConfig = ruleConfigs.FirstOrDefault();

            ruleConfig.RuleId = ruleId;
            ruleConfig.ProjectId = projectId;
            ruleConfig.OperatorForSubjectAndDate = ruleConfig.OperatorForSubjectAndDate == null ? AJGExchangeLogicalOperator.None : ruleConfig.OperatorForSubjectAndDate;
            ruleConfig.OperatorForSentFromAndSubject = ruleConfig.OperatorForSentFromAndSubject == null ? AJGExchangeLogicalOperator.None : ruleConfig.OperatorForSentFromAndSubject;
            ruleConfig.ExchangeItemDeleteMode = ruleConfig.ExchangeItemDeleteMode == null ? AJGExchangeDeleteMode.None : ruleConfig.ExchangeItemDeleteMode;
            ruleConfig.SentFromFilter = ruleConfig.SentFromFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.SentFromFilter;
            ruleConfig.ReceivedDate = ruleConfig.ReceivedDate == null ? AJGDateSelection.None : ruleConfig.ReceivedDate;
            ruleConfig.SubjectFilter = ruleConfig.SubjectFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.SubjectFilter;
            ruleConfig.ReceivedDateOneFilter = ruleConfig.ReceivedDateOneFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.ReceivedDateOneFilter;
            ruleConfig.ReceivedDateOneOffsetPeriod = ruleConfig.ReceivedDateOneOffsetPeriod == null ? AJGDateOffsetPeriod.None : ruleConfig.ReceivedDateOneOffsetPeriod;
            ruleConfig.ReceivedDateTwoFilter = ruleConfig.ReceivedDateTwoFilter == null ? AJGExchangeMessageSearchFilter.None : ruleConfig.ReceivedDateTwoFilter;
            ruleConfig.ReceivedDateTwoOffsetPeriod = ruleConfig.ReceivedDateTwoOffsetPeriod == null ? AJGDateOffsetPeriod.None : ruleConfig.ReceivedDateTwoOffsetPeriod;

            this.AdminService.AddExchangeRuleConfiguration(ruleConfig);

            ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
            ruleConfig.UpdateNotMappedFields();

            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DestroyExchangeRuleConfiguration()
        {
            var ruleconfigs = JsonConvert.DeserializeObject<IEnumerable<ExchangeEmailRuleConfig>>(Request.QueryString["models"]);
            ExchangeEmailRuleConfig ruleConfig = ruleconfigs.FirstOrDefault();
            this.AdminService.DeleteExchangeRuleConfiguration(ruleConfig.Id);

            return Json(ruleconfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateExchangeRuleConfiguration()
        {
            try
            {
                var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<ExchangeEmailRuleConfig>>(Request.QueryString["models"]);
                ExchangeEmailRuleConfig ruleConfig = ruleConfigs.FirstOrDefault();

                this.AdminService.UpdateExchangeRuleConfiguration(ruleConfig);

                ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;

                ruleConfig.UpdateNotMappedFields();

                return Json(ruleConfig, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Excel Rule ]

        //ListExcelRuleConfigurations
        public JsonResult ListExcelRuleConfigurations(int ruleId)
        {
            List<ExcelRuleConfig> ruleConfigs = this.AdminService.ListExcelRuleConfigurations(ruleId);
            return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateExcelRuleConfiguration(int ruleId, Guid projectId)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<ExcelRuleConfig>>(Request.QueryString["models"], jsonSettings);
            ExcelRuleConfig ruleConfig = ruleConfigs.FirstOrDefault();

            ruleConfig.RuleId = ruleId;
            ruleConfig.ProjectId = projectId;

            // Do some validation e.g. check that file path is accessible and process has access.
            string validationMessage = string.Empty;
            if (!ruleConfig.ValuesAreValid(out validationMessage))
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = validationMessage,
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
            else
            {
                this.AdminService.AddExcelRuleConfiguration(ruleConfig);

                ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(projectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;

                return Json(ruleConfigs, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DestroyExcelRuleConfiguration()
        {
            var ruleconfigs = JsonConvert.DeserializeObject<IEnumerable<ExcelRuleConfig>>(Request.QueryString["models"]);
            ExcelRuleConfig ruleConfig = ruleconfigs.FirstOrDefault();
            this.AdminService.DeleteExcelRuleConfiguration(ruleConfig.Id);

            return Json(ruleconfigs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateExcelRuleConfiguration()
        {
            try
            {
                var ruleConfigs = JsonConvert.DeserializeObject<IEnumerable<ExcelRuleConfig>>(Request.QueryString["models"]);
                ExcelRuleConfig ruleConfig = ruleConfigs.FirstOrDefault();

                // Do some validation e.g. check that file path is accessible and process has access.
                string validationMessage = string.Empty;
                if (!ruleConfig.ValuesAreValid(out validationMessage))
                {
                    ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                    {
                        ErrorMessage = validationMessage,
                    };
                    return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    this.AdminService.UpdateExcelRuleConfiguration(ruleConfig);
                    ruleConfig.ScheduleName = this.AdminService.ListProjectSchedules(ruleConfig.ProjectId).Where(a => a.Id == ruleConfig.ScheduleId).FirstOrDefault().Name;
                    return Json(ruleConfig, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExcelDocSelectionTypeModeOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<AJGExcelDocSelectionType>(AJGExcelDocSelectionType.DocName_AbsoluteValue);
            return Json(options, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListExcelDocDeleteModeOptions()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<AJGExcelDocDeleteMode>(AJGExcelDocDeleteMode.Delete);
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region [ System Logs ]

        public JsonResult ListSystemLogs([DataSourceRequest] DataSourceRequest request)
        {
            List<SystemLogDTO> systemLogs = this.AdminService.ListAllSystemLogs();
            Kendo.Mvc.UI.DataSourceResult result = systemLogs.ToDataSourceResult(request);
            return Json(new { data = result.Data, total = result.Total }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSysemLogStackTrace(int logId)
        {
            string stackTrace = this.AdminService.GetSystemLogStackTrace(logId);
            return Json(stackTrace, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Users ]

        public JsonResult listOfficeUsers(int officeId)
        {
            List<User> users = this.AdminService.ListOfficeUsers(officeId);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult listTeamUsers(int teamId)
        {
            List<User> users = this.AdminService.ListTeamUsers(teamId);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult listProjectUsersForOfficeUsersDropDown(Guid projectId, int officeId)
        {
            List<UserForDropDownDTO> users = this.AdminService.ListProjectUsersForOfficeUsersDropDown(projectId, officeId).Select(u =>
                new UserForDropDownDTO
                {
                    Id = u.Id,
                    Name = string.Format("{0} - {1}", u.Name, u.Email)
                }
            ).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult listUsersForTeamUsersDropDown(Guid projectId, int teamId)
        {
            List<UserForDropDownDTO> users = this.AdminService.ListUsersForTeamUsersDropDown(projectId, teamId).Select(u =>
                new UserForDropDownDTO
                {
                    Id = u.Id,
                    Name = string.Format("{0} - {1}", u.Name, u.Email)
                }
            ).ToList(); ;
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectUsers(Guid projectGuid, [DataSourceRequest] DataSourceRequest request)
        {
            List<User> users = this.AdminService.ListProjectUsersWithPermissions(projectGuid);
            foreach(var u in users)
            {
                if (u.Permissions != null)
                {
                    foreach (var p in u.Permissions)
                    {
                        p.User = null;
                    }
                }
            }
            Kendo.Mvc.UI.DataSourceResult result = users.ToDataSourceResult(request);
            return Json(new { data = result.Data, total = result.Total }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProjectUser(Guid projectGuid, User data)
        {
            this.AdminService.UpdateUser(data);
            this.AdminService.UpdateUserProjectLevelPermission(RoleEnum.ProjectAdmin, data.Id, projectGuid, data.IsProjectAdmin);
            this.AdminService.UpdateUserProjectLevelPermission(RoleEnum.MicroService, data.Id, projectGuid, data.IsMicroServiceMethodAccessUser);
            this.AdminService.UpdateUserProjectLevelPermission(RoleEnum.SuperUser, data.Id, projectGuid, data.IsProjectSuperUser, data.ProjectSuperUserInfo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListSystemUsers([DataSourceRequest] DataSourceRequest request)
        {
            List<User> users = this.AdminService.ListAllSystemUsersWithPermissions();

            foreach (var u in users)
            {
                if (u.Permissions != null)
                {
                    foreach (var p in u.Permissions)
                    {
                        p.User = null;
                    }
                }
            }

            Kendo.Mvc.UI.DataSourceResult result = users.ToDataSourceResult(request);
            return Json(new { data = result.Data, total = result.Total }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateSystemUser(User data)
        {
            this.AdminService.AddUser(data);
            this.AdminService.UpdateSystemUserAdmin(data);
            this.AdminService.UpdateSystemSuperUser(data);
            data.Permissions = null;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSystemUser(User data)
        {
            // The system must have at least 1 system admin. else logically the system would die!
            User existingUserInfo = this.AdminService.GetUserWithPermissions(data.Id);
            if (((existingUserInfo.IsSystemAdmin && !data.IsSystemAdmin) || (existingUserInfo.IsActive && !data.IsActive))
                && !this.AdminService.IsSafeToLoseUserAsSystemAdmin(existingUserInfo))
            {
                MessageDTO NoNoNo = new MessageDTO(MessageDTOInfoTypeEnum.InvalidOperation, "This is the last system Admin. It is invalid to delete, deactivate or remove the system admin permission. Please refresh the grid.");
                return Json(NoNoNo, JsonRequestBehavior.AllowGet);
            }
            else
            {
                this.AdminService.UpdateUser(data);
                this.AdminService.UpdateSystemUserAdmin(data);
                this.AdminService.UpdateSystemSuperUser(data);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Admin/Delete/5
        public JsonResult DestroySystemUser(User data)
        {
            User existingUserInfo = this.AdminService.GetUserWithPermissions(data.Id);
            if (!this.AdminService.IsSafeToLoseUserAsSystemAdmin(existingUserInfo))
            {
                MessageDTO NoNoNo = new MessageDTO(MessageDTOInfoTypeEnum.InvalidOperation, "This is the last system Admin. It is invalid to delete, deactivate or remove the system admin permission. Please refresh the grid.");
                return Json(NoNoNo, JsonRequestBehavior.AllowGet);
            }
            else
            {
                this.AdminService.DeleteUser(data.Id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CreateProjectUser(Guid projectGuid)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"], jsonSettings);
            User user = users.FirstOrDefault();
            this.AdminService.AddUser(user);
            this.AdminService.GiveUserProjectMemberRole(user, projectGuid);
            user.Permissions = null;
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Delete/5
        public JsonResult RemoveUserFromProject(Guid projectGuid, User data)
        {
            //var users = JsonConvert.DeserializeObject<IEnumerable<User>>(Request.QueryString["models"]);
            //User user = users.FirstOrDefault();

            ProjectMembershipDTO projectMembershipDto = new ProjectMembershipDTO()
            {
                ProjectId = projectGuid,
                UserId = data.Id
            };

            this.AdminService.DeleteProjectUserMembership(projectMembershipDto);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ Project ]

        public JsonResult ListProjectTypes()
        {
            List<GenericNameValueObject_DTO> options = GetEnumNameValues<ProjectTypeEnum>(ProjectTypeEnum.MicroService, ProjectTypeEnum.MicroService);
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult LoadAdminContentPartialViewForProjectType(Guid projectID)
        {
            string returnView = string.Empty;
            // Get the poroject and work out the view to return.
            Project project = this.AdminService.ListProjects().Where(p => p.ProjectUniqueKey == projectID).FirstOrDefault();

            switch (project.ProjectTypeId.GetValueOrDefault())
            {
                case (int)ProjectTypeEnum.Standard:
                    returnView = "_Standard_NonVTView";
                    break;
                case (int)ProjectTypeEnum.VirtualTrainer:
                    returnView = "_VTView";
                    break;
                case (int)ProjectTypeEnum.MicroService:
                    returnView = "_MicroServiceView";
                    break;
            }

            string returnModel = projectID.ToString();

            return PartialView(returnView, returnModel);
        }
        
        public JsonResult ListProjects()
        {
            User user = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            List<Project> projects = this.AdminService.ListProjectsUserAdministersOnly(user);
            
            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListProjectsForSystemUserProjectMembershipDropDown(int SystemUserID)
        {
            List<Project> projects = this.AdminService.ListProjectsUserIsNotAMemberOf(SystemUserID);

            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateProject()
        {
            var projects = JsonConvert.DeserializeObject<IEnumerable<Project>>(Request.QueryString["models"]);
            Project project = projects.FirstOrDefault();

            Helper h = new Helper();

            project.EmailRazorTemplateAttachmentPath = h.GetEmailRazorTemplateAttachmentPath(this, project); //string.IsNullOrEmpty(project.EmailRazorTemplateAttachmentPath) ? h.GetEmailRazorTemplateAttachmentPath(this) : project.EmailRazorTemplateAttachmentPath;
            project.EmailRazorTemplateBodyPath = h.GetEmailRazorTemplateBodyPath(this, project); // string.IsNullOrEmpty(project.EmailRazorTemplateBodyPath) ? h.GetEmailRazorTemplateBodyPath(this) : project.EmailRazorTemplateBodyPath;
            project.EmailRazorTemplateSubjectPath = h.GetEmailRazorTemplateSubjectPath(this, project); //string.IsNullOrEmpty(project.EmailRazorTemplateSubjectPath) ? h.GetEmailRazorTemplateSubjectPath(this) : project.EmailRazorTemplateSubjectPath;

            string validateOutString = string.Empty;
            if (project.ValuesAreValid(out validateOutString))
            {
                this.AdminService.UpdateProject(project);
                return Json(projects, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = validateOutString,
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Create
        public JsonResult CreateProject()
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var projects = JsonConvert.DeserializeObject<IEnumerable<Project>>(Request.QueryString["models"], jsonSettings);

            Helper h = new Helper();

            Project project = projects.FirstOrDefault();
            project.EmailRazorTemplateAttachmentPath = h.GetEmailRazorTemplateAttachmentPath(this, project); //string.IsNullOrEmpty(project.EmailRazorTemplateAttachmentPath) ? h.GetEmailRazorTemplateAttachmentPath(this) : project.EmailRazorTemplateAttachmentPath;
            project.EmailRazorTemplateBodyPath = h.GetEmailRazorTemplateBodyPath(this, project); // string.IsNullOrEmpty(project.EmailRazorTemplateBodyPath) ? h.GetEmailRazorTemplateBodyPath(this) : project.EmailRazorTemplateBodyPath;
            project.EmailRazorTemplateSubjectPath = h.GetEmailRazorTemplateSubjectPath(this, project); //string.IsNullOrEmpty(project.EmailRazorTemplateSubjectPath) ? h.GetEmailRazorTemplateSubjectPath(this) : project.EmailRazorTemplateSubjectPath;

            string validateOutString = string.Empty;
            if (project.ValuesAreValid(out validateOutString))
            {
                this.AdminService.AddProject(project);

                EscalationsFramework ef = new EscalationsFramework()
                {
                    Id = project.ProjectUniqueKey,
                    IsActive = true
                };
                this.AdminService.CreateEscalationsFramework(ef);
                project.EscalationsFramework = null;
                return Json(projects, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = validateOutString,
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Delete/5
        public JsonResult DestroyProject()
        {
            var projects = JsonConvert.DeserializeObject<IEnumerable<Project>>(Request.QueryString["models"]);
            Project project = projects.FirstOrDefault();

            if (!project.IsMicroServicesProject.GetValueOrDefault())
            {
                //this.AdminService.DeleteEscalationsFramework(project.ProjectUniqueKey);
                this.AdminService.DeleteProject(project.ProjectUniqueKey);

                return Json(projects, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ErrorMessgaeObject returnErrorMessage = new ErrorMessgaeObject()
                {
                    ErrorMessage = "The Micro Service project cannot be deleted.",
                };
                return Json(returnErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}

