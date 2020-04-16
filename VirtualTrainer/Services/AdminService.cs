using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.Encryption;
using AJG.VirtualTrainer.Helper.Exchange;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using VirtualTrainer;
using VirtualTrainer.Interfaces;

namespace AJG.VirtualTrainer.Services
{
    public class AdminService : BaseService, IDisposable
    {
        public AdminService() : base()
        {
        }

        public AdminService(IUnitOfWork uow) : base(uow)
        {
        }

        #region [ System Config ]

        public void SaveUpdateSystemConfig(string groupName, string key, string value)
        {
            SystemConfig config = this.GetSystemConfig(key);

            if(config == null)
            {
                config = new SystemConfig()
                {
                    GroupName = groupName,
                    Key = key,
                    value = value,
                    TimeStamp = DateTime.Now
                };
                _unitOfWork.Context.SystemConfig.Add(config);
            }
            else
            {
                config.GroupName = groupName;
                config.value = value;
                config.TimeStamp = DateTime.Now;
                _unitOfWork.GetRepository<SystemConfig>().Update(config, true);
            }
           
            _unitOfWork.Context.SaveChanges();
        }

        public void DeleteSystemConfig(string groupName, string key)
        {
            if (GetSystemConfigByGroupNameAndKey(groupName, key).Any())
            {
                var config = GetSystemConfigByGroupNameAndKey(groupName, key).FirstOrDefault();
                _unitOfWork.GetRepository<SystemConfig>().Delete(config.Id);
                _unitOfWork.Commit();
            }
        }
        public SystemConfig GetSystemConfig(string key)
        {
            return _unitOfWork.GetRepository<SystemConfig>()
               .GetAll()
               .Where(w => w.Key == key).FirstOrDefault();
        }
        public List<SystemConfig> GetSystemConfigByGroupName(string groupName)
        {
            return _unitOfWork.GetRepository<SystemConfig>()
               .GetAll()
               .OrderByDescending(o => o.Id)
               .Where(w => w.GroupName == groupName).ToList();
        }

        public List<SystemConfig> GetSystemConfigByGroupNameAndKey(string groupName, string key)
        {
            return _unitOfWork.GetRepository<SystemConfig>()
               .GetAll()
               .OrderByDescending(o => o.Id)
               .Where(w => w.GroupName == groupName && w.Key == key).ToList();
        }

        public bool HasSystemConfigByGroupName(string groupName)
        {
            return _unitOfWork.GetRepository<SystemConfig>()
               .GetAll()
               .OrderByDescending(o => o.Id)
               .Where(w => w.GroupName == groupName).Any();
        }

        public bool ConfigUpdatedToday(string groupName)
        {
            if(HasSystemConfigByGroupName(groupName))
            {
                var config = GetSystemConfigByGroupName(groupName).FirstOrDefault();
                return DateTime.Compare(DateTime.Now.Date, config.TimeStamp.Date) == 1 ? false : true;
            }
            return false;
        }

        public ADUserDTO GetRoutADUserFromDB(bool returnUserOnly = false)
        {
            // Try to find the user from the config DB
            if (HasSystemConfigByGroupName(SystemConfig.ConfigKeys.ADHierachy.ToString()))
            {
                foreach (var adHieracy in GetSystemConfigByGroupName(SystemConfig.ConfigKeys.ADHierachy.ToString()))
                {
                    ADUserDTO AdUserFromConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ADUserDTO>(adHieracy.value);                    
                    if (AdUserFromConfig != null)
                    {
                        return returnUserOnly ? AdUserFromConfig.GetUserOnly() : AdUserFromConfig;
                    }
                }
            }
            return null;
        }

        public ADUserDTO GetAdUserFromDB(string userIdentity, bool returnUserOnly = false)
        {
            // Try to find the user from the config DB
            if (HasSystemConfigByGroupName(SystemConfig.ConfigKeys.ADHierachy.ToString()))
            {
                foreach (var adHieracy in GetSystemConfigByGroupName(SystemConfig.ConfigKeys.ADHierachy.ToString()))
                {
                    ADUserDTO AdUserFromConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ADUserDTO>(adHieracy.value);
                    var returnData = AdUserFromConfig.GetSpecifcUserFromHierachy(userIdentity);
                    if (returnData != null)
                    {
                        return returnUserOnly ? returnData.GetUserOnly() : returnData;
                    }
                }
            }
            return null;
        }        

        #endregion

        #region [ Logs ]

        public SystemLog SaveSystemLog(SystemLog log)
        {
            _unitOfWork.Context.SystemLogs.Add(log);
            _unitOfWork.Context.SaveChanges();
            return log;
        }
        public List<SystemLogDTO> ListAllSystemLogs()
        {
            return _unitOfWork.GetRepository<SystemLog>()
                .GetAll()
                .OrderByDescending(o => o.Id)
                .Select(a=> new SystemLogDTO()
                {
                    Id = a.Id,
                    MachineName = a.MachineName,
                    UserName = a.UserName,
                    TimeStamp = a.TimeStamp,
                    Level = a.Level,
                    ErrorMessage = a.ErrorMessage,
                    ProjectName = a.ProjectName
                }).ToList();
        }
        public string GetSystemLogStackTrace(int logId)
        {
            return _unitOfWork.Context.SystemLogs.Where(a => a.Id == logId).FirstOrDefault().StackTrace;
        }

        #endregion

        #region [ Exchange Account Details ]

        public List<AJGEmailMessage> TestExchangeAccountConfig(int configID)
        {
            ExchangeAccountDetails details = _unitOfWork.GetRepository<ExchangeAccountDetails>().GetAllNoTrack().Where(u => u.Id == configID).FirstOrDefault();
            ExchangeHelper helper = new ExchangeHelper(details.AutoDiscoverEmail, details.AutoDiscoverUserName, details.GetAutoDiscoverUserPasswordDecrypted(), details.AutoDiscoverUserDomain);
            return helper.GetEmailMessagesAndAttachementsFromInbox(null, AJGExchangeDeleteMode.None, 1);
            //return helper.GetEmailMessagesAndAttachementsFromInbox(null, null, null, null, AJGExchangeLogicalOperator.And, AJGExchangeDeleteMode.None, 1);
        }
        public List<ExchangeAccountDetails> ListProjectExchangeAccounts(Guid projectGuid)
        {
            return _unitOfWork.GetRepository<ExchangeAccountDetails>().GetAllNoTrack().Where(u => u.ProjectId == projectGuid).OrderBy(o => o.DisplayName).ToList();
        }
        public void AddProjectExchangeAccounts(ExchangeAccountDetails targetDbDetails)
        {
            targetDbDetails.AutoDiscoverUserPassword = EncryptionHelper.Encrypt(targetDbDetails.AutoDiscoverUserPassword, GetEncryptionKey());

            _unitOfWork.GetRepository<ExchangeAccountDetails>().Create(targetDbDetails);
            _unitOfWork.Commit();
        }
        public void DeleteProjectExchangeAccounts(int Id)
        {
            _unitOfWork.GetRepository<ExchangeAccountDetails>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateProjectExchangeAccounts(ExchangeAccountDetails targetDbDetails)
        {
            ExchangeAccountDetails existing = ListProjectExchangeAccounts(targetDbDetails.ProjectId).Where(t => t.Id == targetDbDetails.Id).FirstOrDefault();

            // If the password has changed we want to ensure it is encrypted before saving to db.
            if (existing.AutoDiscoverUserPassword != targetDbDetails.AutoDiscoverUserPassword)
            {
                targetDbDetails.AutoDiscoverUserPassword = EncryptionHelper.Encrypt(targetDbDetails.AutoDiscoverUserPassword, GetEncryptionKey());
            }

            _unitOfWork.GetRepository<ExchangeAccountDetails>().Update(targetDbDetails, targetDbDetails.Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Other ]

        public string GetEnvironment()
        {
            return ConfigurationManager.AppSettings[AppSettingsEnum.targetSystem.ToString()];
        }
        public string GetExchangeRuleEnabled()
        {
            string enabled = ConfigurationManager.AppSettings[AppSettingsEnum.ExchangeRuleEnabled.ToString()];
            return string.IsNullOrEmpty(enabled) ? "false" : enabled;
        }
        public string GetExcelRuleEnabled()
        {
            string enabled = ConfigurationManager.AppSettings[AppSettingsEnum.ExcelRuleEnabled.ToString()];
            return string.IsNullOrEmpty(enabled) ? "false" : enabled;
        }
        public string GetEncryptionKey()
        {
            string returnString = ConfigurationManager.AppSettings[AppSettingsEnum.EncryptionKey.ToString()];
            return string.IsNullOrEmpty(returnString) ? "" : returnString;
        }

        #endregion

        #region [ Regions ]

        public List<Region> ListRegionsForOfficeRegionsDropDown(Guid projectId)
        {
            return _unitOfWork.GetRepository<Region>().GetAll().Where(a => a.ProjectId == projectId).ToList();
        }
        public List<Region> ListProjectRegions(Guid projectGuid)
        {
            return _unitOfWork.GetRepository<Region>().GetAllNoTrack().Where(u => u.ProjectId == projectGuid).OrderBy(o => o.Name).ToList();
        }
        public void UpdateRegion(Region region)
        {
            _unitOfWork.GetRepository<Region>().Update(region, region.Id);
            _unitOfWork.Commit();
        }
        public void AddRegion(Region region)
        {
            _unitOfWork.GetRepository<Region>().Create(region);
            _unitOfWork.Commit();
        }
        public void DeleteRegion(int Id)
        {
            _unitOfWork.GetRepository<Region>().Delete(Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Acturis Import ]

        public List<ActurisBusinessStructureSyncConfig> ListProjectActurisImports(Guid projectId)
        {
            List<ActurisBusinessStructureSyncConfig> configs = _unitOfWork.GetRepository<ActurisBusinessStructureSyncConfig>().GetAllNoTrack().Where(a=>a.ProjectId == projectId).Include("TargetDatabaseDetails").Include("Schedule").OrderBy(o => o.Name).ToList();

            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.TargetDbName = a.TargetDatabaseDetails.DBName;
                a.Schedule = null;
                a.TargetDatabaseDetails = null;
                return a;
            }).ToList();
        }
        public void AddRuleConfiguration(ActurisBusinessStructureSyncConfig syncConfig)
        {
            _unitOfWork.GetRepository<ActurisBusinessStructureSyncConfig>().Create(syncConfig);
            _unitOfWork.Commit();
        }
        public void DestroyProjectActurisImport(int Id)
        {
            _unitOfWork.GetRepository<ActurisBusinessStructureSyncConfig>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateProjectActurisImport(ActurisBusinessStructureSyncConfig syncConfig)
        {
            _unitOfWork.GetRepository<ActurisBusinessStructureSyncConfig>().Update(syncConfig, syncConfig.Id);
            _unitOfWork.Commit();
        }
        public List<UserBusinessStructureInfo> ListActurisImportSnapShot(int importId)
        {
            ActurisBusinessStructureSyncConfig rule = _unitOfWork.GetRepository<ActurisBusinessStructureSyncConfig>().GetAll().Where(r => r.Id == importId).FirstOrDefault();
            return rule.GetStructureFromActuris(_unitOfWork.Context).OrderBy(a=>a.OfficeName).ToList();
        }

        #endregion

        #region [ Execute VT Tasks ]

        //public void ExecuteVirtualTrainer(string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        //{
        //    foreach (Project p in _unitOfWork.GetRepository<Project>().GetAll().ToList())
        //    {
        //        p.ExecuteAllScheduledJobs(_unitOfWork.Context, 
        //            string.IsNullOrEmpty(p.EmailRazorTemplateBodyPath) ? ServerRazorEmailTemplatePathBody : p.EmailRazorTemplateBodyPath,
        //            string.IsNullOrEmpty(p.EmailRazorTemplateSubjectPath) ? ServerRazorEmailTemplatePathSubject : p.EmailRazorTemplateSubjectPath,
        //            string.IsNullOrEmpty(p.EmailRazorTemplateAttachmentPath) ? ServerRazorEmailTemplatePathAttachment : p.EmailRazorTemplateAttachmentPath);
        //    }    
        //}
        public void ExecuteVirtualTrainerForProject(string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment, Guid projectId)
        {
            foreach (Project p in _unitOfWork.GetRepository<Project>().GetAll().Where(p=>p.ProjectUniqueKey == projectId).ToList())
            {
                p.ExecuteAllScheduledJobs(_unitOfWork.Context,
                    string.IsNullOrEmpty(p.EmailRazorTemplateBodyPath) ? ServerRazorEmailTemplatePathBody : p.EmailRazorTemplateBodyPath,
                    string.IsNullOrEmpty(p.EmailRazorTemplateSubjectPath) ? ServerRazorEmailTemplatePathSubject : p.EmailRazorTemplateSubjectPath,
                    string.IsNullOrEmpty(p.EmailRazorTemplateAttachmentPath) ? ServerRazorEmailTemplatePathAttachment : p.EmailRazorTemplateAttachmentPath);
            }
        }
        public void AddLog()
        {
            Project p =_unitOfWork.GetRepository<Project>().GetAll().FirstOrDefault();

            SystemLog sl = new SystemLog(new Exception("Log added"), p);

            _unitOfWork.GetRepository<SystemLog>().Create(sl);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Breaches ]

        public List<BreachLog> ListBreachesLOBSnapShot(int ruleId)
        {
            Rule rule = _unitOfWork.GetRepository<Rule>().GetAll().Where(r => r.Id == ruleId).FirstOrDefault();
            return rule.ExecuteAllRuleConfigurations(_unitOfWork.Context, false).Select(a =>
            {
                a.Project = null;
                a.Rule = null;
                a.RuleConfiguration = null;
                a.User = null;
                a.DatabaseDetails = null;
                a.Office = null;
                a.Team = null;
                a.Region = null;
                return a;
            }).ToList();
        }
        public List<BreachLog> ListProjectBreaches(Guid projectId)
        {
            List <BreachLog> breachLogs = _unitOfWork.GetRepository<BreachLog>().GetAllNoTrack().Where(r => r.ProjectId == projectId).ToList();
            return breachLogs;
        }
        public List<BreachLogDTO> ListProjectBreachesDTO(Guid projectId)
        {
            var breaches = (from u in _unitOfWork.Context.BreachLogs
                            where u.ProjectId == projectId
                            select new BreachLogDTO
                            {
                                Id = u.Id,
                                RuleName = u.RuleName,
                                RuleConfigurationName = u.RuleConfigurationName,
                                UserName = u.UserName,
                                OfficeName = u.OfficeName,
                                TeamName = u.TeamName,
                                ContextRef = u.ContextRef,
                                BreachDisplayText = u.BreachDisplayText,
                                BreachDisplayAlternateText = u.BreachDisplayAlternateText,
                                TimeStamp = u.TimeStamp,
                                RuleBreachFieldOne = u.RuleBreachFieldOne,
                                RuleBreachFieldTwo = u.RuleBreachFieldTwo,
                                RuleBreachFieldThree = u.RuleBreachFieldThree,
                                RuleBreachFieldFour = u.RuleBreachFieldFour,
                                RuleBreachFieldFive = u.RuleBreachFieldFive,
                                IsArchived = u.IsArchived,
                            }).ToList();

            return breaches;
        }

        public void DeleteBreachLog(int Id)
        {
            _unitOfWork.GetRepository<BreachLog>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateBreach(BreachLogDTO breachlogDto)
        {
            BreachLog bl = _unitOfWork.GetRepository<BreachLog>().GetAll().Where(b => b.Id == breachlogDto.Id).FirstOrDefault();
            bl.IsArchived = breachlogDto.IsArchived;

            if (breachlogDto.IsArchived == true)
            {
                bl.ArchivedTimeStamp = DateTime.Now;
            }
            else {
                bl.ArchivedTimeStamp = null;
            }
                
            _unitOfWork.GetRepository<BreachLog>().Update(bl, bl.Id);
            _unitOfWork.Commit();
        }
        
        #endregion

        #region [ Schedules ]

        public List<ScheduleFrequency> ListScheduleFrequencies()
        {
            return _unitOfWork.GetRepository<ScheduleFrequency>().GetAllNoTrack().OrderBy(o => o.Name).ToList();
        }
        public List<Schedule> ListProjectSchedules(Guid projectId)
        {
             List<Schedule> schedules =_unitOfWork.GetRepository<Schedule>()
                .GetAllNoTrack()
                .Where(s=>s.ProjectId == projectId)
                .OrderBy(o => o.Name)
                .Include("ScheduleFrequency")
                .ToList();

            return schedules.Select(a=> {
                a.ScheduleFrequencyName = a.ScheduleFrequency.Name;
                a.ScheduleFrequency = null;
                return a;
            }).ToList();
        }
        public void AddSchedule(Schedule schedule)
        {
            _unitOfWork.GetRepository<Schedule>().Create(schedule);
            _unitOfWork.Commit();
        }
        public void DeleteSchedule(int Id)
        {
            _unitOfWork.GetRepository<Schedule>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateSchedule(Schedule schedule)
        {
            _unitOfWork.GetRepository<Schedule>().Update(schedule, schedule.Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ db details ]

        public List<TargetDatabaseDetails> ListProjectTargetDBDetails(Guid projectGuid)
        {
            return _unitOfWork.GetRepository<TargetDatabaseDetails>().GetAllNoTrack().Where(u => u.ProjectId == projectGuid).OrderBy(o => o.DisplayName).ToList();
        }
        public void AddTargetDatabaseDetails(TargetDatabaseDetails targetDbDetails)
        {
            _unitOfWork.GetRepository<TargetDatabaseDetails>().Create(targetDbDetails);
            _unitOfWork.Commit();
        }
        public void DeleteTargetDatabaseDetails(int Id)
        {
            _unitOfWork.GetRepository<TargetDatabaseDetails>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateTargetDatabaseDetails(TargetDatabaseDetails targetDbDetails)
        {
            _unitOfWork.GetRepository<TargetDatabaseDetails>().Update(targetDbDetails, targetDbDetails.Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Roles / Permissions ]

        public UserPerms GetCurrentUserPermissions(string samAccountName, string userName)
        {
            // get time sheet project id from config
            Guid projectId;

            var user = Guid.TryParse(ConfigurationHelper.Get(AppSettingsList.TimeSheetProjectId), out projectId)
                ? GetUserWithPermissions(userName, projectId)
                : GetUserWithPermissions(userName);

            var perms = new UserPerms
            {
                IsSystemAdmin = user.IsSystemAdmin,
                IsSystemSuperUser = user.IsSystemSuperUser,
                IsProjectSuperUser = user.IsProjectSuperUser,
                IsProjectAdmin = user.IsProjectAdmin,
                ProjectSuperUserInfo = string.IsNullOrEmpty(user.ProjectSuperUserInfo) ? string.Empty : user.ProjectSuperUserInfo,
                SystemSuperUserInfo = string.IsNullOrEmpty(user.SystemSuperUserInfo) ? string.Empty : user.SystemSuperUserInfo
            };
            return perms;
        }

        public class UserPerms
        {
            public bool IsSystemAdmin { get; set; }
            public bool IsSystemSuperUser { get; set; }
            public bool IsProjectSuperUser { get; set; }
            public bool IsProjectAdmin { get; set; }
            public string ProjectSuperUserInfo { get; set; }
            public string SystemSuperUserInfo { get; set; }

        }
        public void RemoveUserFromOffice(User user, int officeId, Guid projectId)
        {
            List<OfficePermission> userOfficePerms = _unitOfWork
                       .GetRepository<OfficePermission>()
                       .GetAll()
                       .Where(p => p.UserId == user.Id
                               && p.OfficeId == officeId
                               && p.ProjectId == projectId)
                       .ToList();

            foreach (OfficePermission op in userOfficePerms)
            {
                _unitOfWork.GetRepository<OfficePermission>().Delete(op);
            }

            _unitOfWork.Commit();
        }

        public void UpdateUserOfficeRole(User user, int officeId, Guid projectId, RoleEnum role)
        {
            OfficePermission officeManager = _unitOfWork
                      .GetRepository<OfficePermission>()
                      .GetAll()
                      .Where(p => p.UserId == user.Id
                              && p.RoleId == (int)role
                              && p.OfficeId == officeId
                              && p.ProjectId == projectId)
                      .FirstOrDefault();

            bool currentUserRoleState = false;

            switch (role)
            {
                case RoleEnum.BranchManager:
                    currentUserRoleState = user.IsOfficeManager;
                    break;
                case RoleEnum.RegionalManager:
                    currentUserRoleState = user.IsOfficeRegionalManager;
                    break;
                case RoleEnum.QualityAuditor:
                    currentUserRoleState = user.IsOfficeQualityAuditor;
                    break;
                case RoleEnum.BranchMember:
                    currentUserRoleState = user.IsOfficeMember;
                    break;
            }

            if (currentUserRoleState)
            {
                if (officeManager == null)
                {
                    officeManager = new OfficePermission()
                    {
                        UserId = user.Id,
                        RoleId = (int)role,
                        OfficeId = officeId,
                        ProjectId = projectId
                    };

                    _unitOfWork.GetRepository<OfficePermission>().Create(officeManager);
                }
            }
            else
            {
                if (officeManager != null)
                {
                    _unitOfWork.GetRepository<OfficePermission>().Delete(officeManager);
                }
            }
            _unitOfWork.Commit();
        }
        /// <summary>
        /// If the user is the last admin then returns False, else true.
        /// </summary>
        /// <returns></returns>
        public bool IsSafeToLoseUserAsSystemAdmin(User user)
        {
            // if the current user is async system admin and there is only one left then, false.
            bool currentUserIsSystemAdmin = GetUserWithPermissions(user.Id).IsSystemAdmin;
            int count = _unitOfWork.GetRepository<SystemPermission>().GetAll().Where(p => p.RoleId == (int)RoleEnum.SystemAdmin).Count();
            bool candelete = !(count == 1 && currentUserIsSystemAdmin);
            return candelete;
        }
        public void UpdateSystemUserAdmin(User user)
        {
            SystemPermission sysadmin = _unitOfWork
                       .GetRepository<SystemPermission>()
                       .GetAll()
                       .Where(p => p.UserId == user.Id
                               && p.RoleId == (int)RoleEnum.SystemAdmin)
                       .FirstOrDefault();

            if (user.IsSystemAdmin)
            {
                if (sysadmin == null)
                {
                    sysadmin = new SystemPermission()
                    {
                        UserId = user.Id,
                        RoleId = (int)RoleEnum.SystemAdmin
                    };

                    _unitOfWork.GetRepository<SystemPermission>().Create(sysadmin);
                }
            }
            else
            {
                if (sysadmin != null)
                {
                    _unitOfWork.GetRepository<SystemPermission>().Delete(sysadmin);
                }
            }
            _unitOfWork.Commit();
        }
        public void UpdateSystemSuperUser(User user)
        {
            SystemPermission MyHUbOverrideUserPerm = _unitOfWork
                       .GetRepository<SystemPermission>()
                       .GetAll()
                       .Where(p => p.UserId == user.Id
                               && p.RoleId == (int)RoleEnum.SuperUser)
                       .FirstOrDefault();

            if (user.IsSystemSuperUser)
            {
                if (MyHUbOverrideUserPerm == null)
                {
                    string superUserInfo = string.IsNullOrEmpty(user.SystemSuperUserInfo) ? "" : user.SystemSuperUserInfo;
                    MyHUbOverrideUserPerm = new SystemPermission()
                    {
                        UserId = user.Id,
                        RoleId = (int)RoleEnum.SuperUser,
                        Info = superUserInfo
                    };

                    _unitOfWork.GetRepository<SystemPermission>().Create(MyHUbOverrideUserPerm);
                }
                else
                {
                    MyHUbOverrideUserPerm.Info = user.SystemSuperUserInfo;
                    _unitOfWork.GetRepository<SystemPermission>().Update(MyHUbOverrideUserPerm);
                }
            }
            else
            {
                if (MyHUbOverrideUserPerm != null)
                {
                    _unitOfWork.GetRepository<SystemPermission>().Delete(MyHUbOverrideUserPerm);
                }
            }
            _unitOfWork.Commit();
        }
        public List<OfficePermission> ListUserOfficeMemberships(int userId)
        {
            List<OfficePermission> systemRoles = _unitOfWork.GetRepository<OfficePermission>().GetAll().Where(u => u.UserId == userId).GroupBy(g => g.OfficeId).Select(grp => grp.FirstOrDefault()).Include("Office").ToList();
            return systemRoles.Select(a =>
            {
                a.OrganisationKey = a.Office.ActurisOrganisationKey;
                a.OrganisationName = a.Office.ActurisOrganisationName;
                a.OfficeName = a.Office.Name;
                a.OfficeKey = a.Office.AlsoKnownAs;
                a.Office = null;
                return a;
            }).ToList();
        }
        public List<TeamPermission> ListUserTeamMemberships(int userId)
        {
            List<TeamPermission> systemRoles = _unitOfWork.GetRepository<TeamPermission>().GetAll().Where(u => u.UserId == userId).GroupBy(g => g.TeamId).Select(grp => grp.FirstOrDefault()).Include("Team").ToList();
            return systemRoles.Select(a =>
            {
                a.OrganisationKey = a.Team.ActurisOrganisationKey;
                a.OrganisationName = a.Team.ActurisOrganisationName;
                a.TeamName = a.Team.Name;
                a.TeamKey = a.Team.AlsoKnownAs;
                a.Team = null;
                return a;
            }).ToList(); 
        }
        public List<ProjectMembershipDTO> ListUserSystemRoles(int userId)
        {
            List<ProjectPermission> systemRoles = _unitOfWork.GetRepository<ProjectPermission>().GetAll().Where(u => u.UserId == userId).GroupBy(g => g.ProjectId).Select(grp => grp.FirstOrDefault()).Include("Role").Include("Project").Include("User").ToList();
            return systemRoles.Select(a => new ProjectMembershipDTO()
            {
                ProjectId = a.ProjectId,
                ProjectName = a.Project.ProjectName,
                UserId = a.UserId,
                UserName = a.User.Name,
                isProjectAdmin = a.User.HasRole(_unitOfWork.Context, a.ProjectId, RoleEnum.ProjectAdmin)
            }).ToList();
        }
        public void UpdateUserProjectLevelPermission(RoleEnum projectRoleEnum, int userID, Guid projectGuid, bool hasRole, string info = "")
        {
            ProjectPermission adminpp = _unitOfWork
                        .GetRepository<ProjectPermission>()
                        .GetAll()
                        .Where(p => p.UserId == userID
                                && p.RoleId == (int)projectRoleEnum
                                && p.ProjectId == projectGuid)
                        .FirstOrDefault();
            if (hasRole)
            {
                if (adminpp == null)
                {
                    adminpp = new ProjectPermission()
                    {
                        UserId = userID,
                        RoleId = (int)projectRoleEnum,
                        ProjectId = projectGuid,
                        Info = info
                    };

                    _unitOfWork.GetRepository<ProjectPermission>().Create(adminpp);
                }
            }
            else
            {
                if (adminpp != null)
                {
                    _unitOfWork.GetRepository<ProjectPermission>().Delete(adminpp);
                }
            }
            _unitOfWork.Commit();
        }
        public void UpdateProjectUserAdminMembership(ProjectMembershipDTO projectMembershipDTO)
        {
            ProjectPermission adminpp = _unitOfWork
                        .GetRepository<ProjectPermission>()
                        .GetAll()
                        .Where(p => p.UserId == projectMembershipDTO.UserId
                                && p.RoleId == (int)RoleEnum.ProjectAdmin
                                && p.ProjectId == projectMembershipDTO.ProjectId)
                        .FirstOrDefault();

            if (projectMembershipDTO.isProjectAdmin)
            {
                if (adminpp == null)
                {
                    adminpp = new ProjectPermission()
                    {
                        UserId = projectMembershipDTO.UserId,
                        RoleId = (int)RoleEnum.ProjectAdmin,
                        ProjectId = projectMembershipDTO.ProjectId
                    };

                    _unitOfWork.GetRepository<ProjectPermission>().Create(adminpp);
                }
            }
            else
            {
                if (adminpp != null)
                {
                    _unitOfWork.GetRepository<ProjectPermission>().Delete(adminpp);
                }
            }
            _unitOfWork.Commit();
        }
        public void AddProjectUserMemberMembership(ProjectMembershipDTO projectMembershipDTO)
        {
            // TODO check if they exist first????
            ProjectPermission userProjectMember = _unitOfWork.GetRepository<ProjectPermission>()
                .GetAll()
                .Where(a => a.UserId == projectMembershipDTO.UserId
                       && a.ProjectId == projectMembershipDTO.ProjectId
                       && a.RoleId == (int)RoleEnum.ProjectMember)
                       .FirstOrDefault();

            if (userProjectMember == null)
            {
                userProjectMember = new ProjectPermission()
                {
                    UserId = projectMembershipDTO.UserId,
                    ProjectId = projectMembershipDTO.ProjectId,
                    RoleId = (int)RoleEnum.ProjectMember
                };
                _unitOfWork.GetRepository<ProjectPermission>().Create(userProjectMember);
            }

            _unitOfWork.Commit();
        }
        public void DeleteProjectUserMembership(ProjectMembershipDTO projectMembershipDTO)
        {
            // Just deleteall project memberships.
            List<ProjectPermission> perms = _unitOfWork.GetRepository<ProjectPermission>().GetAll().Where(p => p.UserId == projectMembershipDTO.UserId
                                                                                                    && p.ProjectId == projectMembershipDTO.ProjectId).ToList();

            foreach (ProjectPermission perm in perms)
            {
                _unitOfWork.GetRepository<ProjectPermission>().Delete(perm);
            }

            _unitOfWork.Commit();
        }

        public void UpdateTeamRole(User user, int teamId, Guid projectId, RoleEnum role)
        {
            TeamPermission teamLead = _unitOfWork
                       .GetRepository<TeamPermission>()
                       .GetAll()
                       .Where(p => p.UserId == user.Id
                               && p.RoleId == (int)role
                               && p.TeamId == teamId
                               && p.ProjectId == projectId)
                       .FirstOrDefault();

            bool currentUserRoleState = false;

            switch (role)
            {
                case RoleEnum.TeamLead:
                    currentUserRoleState = user.IsTeamLead;
                    break;
                case RoleEnum.ClaimsHandler:
                    currentUserRoleState = user.IsClaimsHandler;
                    break;
                case RoleEnum.TeamMember:
                    currentUserRoleState = user.IsTeamMember;
                    break;
            }

            if (currentUserRoleState)
            {
                if (teamLead == null)
                {
                    teamLead = new TeamPermission()
                    {
                        UserId = user.Id,
                        RoleId = (int)role,
                        TeamId = teamId,
                        ProjectId = projectId
                    };

                    _unitOfWork.GetRepository<TeamPermission>().Create(teamLead);
                }
            }
            else
            {
                if (teamLead != null)
                {
                    _unitOfWork.GetRepository<TeamPermission>().Delete(teamLead);
                }
            }
            _unitOfWork.Commit();
        }
        public void RemoveUserFromTeam(User user, int teamId, Guid projectId)
        {
            List<TeamPermission> userTeamPerms = _unitOfWork
                       .GetRepository<TeamPermission>()
                       .GetAll()
                       .Where(p => p.UserId == user.Id
                               && p.TeamId == teamId
                               && p.ProjectId == projectId)
                       .ToList();

            foreach (TeamPermission op in userTeamPerms)
            {
                _unitOfWork.GetRepository<TeamPermission>().Delete(op);
            }

            _unitOfWork.Commit();
        }

        #endregion

        #region [ Office ]

        public List<Office> ListProjectOffices(Guid projectGuid)
        {
            return _unitOfWork.GetRepository<Office>()
                .GetAllNoTrack()
                .Where(u => u.ProjectId == projectGuid)
                .Include("Region")
                .OrderBy(o => o.Name)
                .ToList()
                .Select(s => 
                {
                    s.RegionName = s.Region == null ? "" : s.Region.Name;
                    s.RegionId = s.Region == null ? 0 : s.Region.Id;
                    s.Region = null;
                    return s;
                }).ToList();
        }

        public void UpdateOffice(Office office)
        {
            if (office.RegionId == 0)
            {
                office.RegionId = null;
            }
            _unitOfWork.GetRepository<Office>().Update(office, office.Id, true);
            _unitOfWork.Commit();
        }

        public void AddOffice(Office office)
        {
            if (office.RegionId == 0)
            {
                office.RegionId = null;
            }
            _unitOfWork.GetRepository<Office>().Create(office);
            _unitOfWork.Commit();
        }

        public void DeleteOffice(int Id)
        {
            _unitOfWork.GetRepository<Office>().Delete(Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Rule Participants ]

        public void UpdateRuleConfigProjectParticipant(RuleParticipantProject ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantProject>().Update(ruleParticipant, ruleParticipant.Id);
            _unitOfWork.Commit();
        }
        public void UpdateRuleConfigOfficeParticipant(RuleParticipantOffice ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantOffice>().Update(ruleParticipant, ruleParticipant.Id);
            _unitOfWork.Commit();
        }
        public void UpdateRuleConfigTeamParticipant(RuleParticipantTeam ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantTeam>().Update(ruleParticipant, ruleParticipant.Id);
            _unitOfWork.Commit();
        }
        public void UpdateRuleConfigUserParticipant(RuleParticipantUser ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantUser>().Update(ruleParticipant, ruleParticipant.Id);
            _unitOfWork.Commit();
        }
        public void AddRuleConfigOfficeParticipant(RuleParticipantOffice ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantOffice>().Create(ruleParticipant);
            _unitOfWork.Commit();
        }
        public void AddRuleConfigTeamParticipant(RuleParticipantTeam ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantTeam>().Create(ruleParticipant);
            _unitOfWork.Commit();
        }
        public void AddRuleConfigUserParticipant(RuleParticipantUser ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantUser>().Create(ruleParticipant);
            _unitOfWork.Commit();
        }
        
        public void AddRuleConfigProjectParticipant(RuleParticipantProject ruleParticipant)
        {
            _unitOfWork.GetRepository<RuleParticipantProject>().Create(ruleParticipant);
            _unitOfWork.Commit();
        }

        public List<RuleParticipantProject> ListRuleConfigProjectParticipants(int ruleConfigId)
        {
            List<RuleParticipantProject> participants = _unitOfWork.GetRepository<RuleParticipantProject>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("Project").OrderBy(o => o.RuleConfigurationId).ToList();

            return participants.Select(a =>
            {
                a.ProjectName = a.Project.ProjectName;
                a.Project = null;
                return a;
            }).ToList();
        }
        public List<RuleParticipantOffice> ListRuleConfigOfficeParticipants(int ruleConfigId)
        {
            List<RuleParticipantOffice> participants = _unitOfWork.GetRepository<RuleParticipantOffice>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("Office").OrderBy(o => o.RuleConfigurationId).ToList();

            return participants.Select(a =>
            {
                a.OfficeName = a.Office.Name;
                a.ActurisOrganisationName = a.Office.ActurisOrganisationName;
                a.ActurisOrganisationKey = a.Office.ActurisOrganisationKey;
                a.AlsoKnownAs = a.Office.AlsoKnownAs;
                a.Office = null;
                return a;
            }).OrderBy(o=>o.OfficeName).ToList();
        }
        public List<RuleParticipantTeam> ListRuleConfigTeamParticipants(int ruleConfigId)
        {
            List<RuleParticipantTeam> participants = _unitOfWork.GetRepository<RuleParticipantTeam>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("Team").OrderBy(o => o.RuleConfigurationId).ToList();

            return participants.Select(a =>
            {
                a.TeamName = a.Team.Name;
                a.AlsoKnownAs = a.Team.AlsoKnownAs;
                a.ActurisOrganisationKey = a.Team.ActurisOrganisationKey;
                a.ActurisOrganisationName = a.Team.ActurisOrganisationName;
                a.Team = null;
                return a;
            }).OrderBy(o => o.TeamName).ToList();
        }
        public List<RuleParticipantUser> ListRuleConfigUserParticipants(int ruleConfigId)
        {
            List<RuleParticipantUser> participants = _unitOfWork.GetRepository<RuleParticipantUser>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("User").OrderBy(o => o.RuleConfigurationId).ToList();

            return participants.Select(a =>
            {
                a.UserName = a.User.Name;
                a.AlsoKnownAs = a.User.AlsoKnownAs;
                a.ActurisOrganisationKey = a.User.ActurisOrganisationKey;
                a.ActurisOrganisationName = a.User.ActurisOrganisationName;
                a.User = null;
                return a;
            }).OrderBy(o => o.UserName).ToList();
        }
        public List<Project> ListProjectsForRuleParticiantsDropDown(Guid ProjectID, int ruleConfigId)
        {
            List<Project> projects = _unitOfWork.GetRepository<Project>().GetAll().Where(r => r.ProjectUniqueKey == ProjectID).OrderBy(o => o.ProjectName).ToList();
            List<Project> ruleParticipantProject = _unitOfWork.GetRepository<RuleParticipantProject>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("Project").Select(p => p.Project).ToList();

            foreach (Project p in ruleParticipantProject)
            {
                projects.Remove(p);
            }
            return projects;
        }
        
        public List<Team> ListTeamsForRuleParticiantsDropDown(Guid ProjectID, int ruleConfigId)
        {
            List<Team> teams = _unitOfWork.GetRepository<Team>().GetAll().Where(r => r.ProjectId == ProjectID).OrderBy(o => o.Name).ToList();
            List<Team> ruleParticipantTeams = _unitOfWork.GetRepository<RuleParticipantTeam>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("Team").Select(p => p.Team).ToList();

            foreach (Team t in ruleParticipantTeams)
            {
                teams.Remove(t);
            }
            return teams;
        }
        public List<User> ListUsersForRuleParticiantsDropDown(Guid ProjectID, int ruleConfigId)
        {
            List<User> users = ListProjectUsersWithPermissions(ProjectID);
            List<User> ruleParticipantUsers = _unitOfWork.GetRepository<RuleParticipantUser>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("Team").Select(p => p.User).ToList();

            foreach (User t in ruleParticipantUsers)
            {
                users.Remove(t);
            }
            return users;
        }
        
        public List<Office> ListOfficesForRuleParticiantsDropDown(Guid ProjectID, int ruleConfigId)
        {
            List<Office> offices = _unitOfWork.GetRepository<Office>().GetAll().Where(r => r.ProjectId == ProjectID).OrderBy(o => o.Name).ToList();
            List<Office> ruleParticipantOffices = _unitOfWork.GetRepository<RuleParticipantOffice>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("Office").Select(p => p.Office).ToList();

            foreach (Office p in ruleParticipantOffices)
            {
                offices.Remove(p);
            }
            return offices;
        }
        public void DeleteRuleConfigProjectParticipant(int Id)
        {
            _unitOfWork.GetRepository<RuleParticipantProject>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfigOfficeParticipant(int Id)
        {
            _unitOfWork.GetRepository<RuleParticipantOffice>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfigTeamParticipant(int Id)
        {
            _unitOfWork.GetRepository<RuleParticipantTeam>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfigUserParticipant(int Id)
        {
            _unitOfWork.GetRepository<RuleParticipantUser>().Delete(Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Exclusions ]

        public List<ExclusionsGroup> ListProjectExclusionsGroups(Guid projectId)
        {
            List<ExclusionsGroup> exclusionsGroups = _unitOfWork.GetRepository<ExclusionsGroup>().GetAllNoTrack().Where(r => r.ProjectId == projectId).OrderBy(o=>o.GroupName).ToList();
            return exclusionsGroups;
        }
        public List<ExclusionsItem> ListExclusionGroupItems(int exclusionsGroupId)
        {
            List<ExclusionsItem> exclusionsItems = _unitOfWork.GetRepository<ExclusionsItem>().GetAllNoTrack().Where(r => r.ExclusionsGroupId == exclusionsGroupId).OrderBy(o=>o.Name).ToList();
            return exclusionsItems;
        }
        public void AddProjectExclusionsGroup(ExclusionsGroup exclusionGroup)
        {
            _unitOfWork.GetRepository<ExclusionsGroup>().Create(exclusionGroup);
            _unitOfWork.Commit();
        }
        public void AddExclusionGroupItem(ExclusionsItem exclusionItem)
        {
            _unitOfWork.GetRepository<ExclusionsItem>().Create(exclusionItem);
            _unitOfWork.Commit();
        }
        public void DeleteProjectExclusionsGroup(int Id)
        {
            _unitOfWork.GetRepository<ExclusionsGroup>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DestroyExclusionGroupItem(int Id)
        {
            _unitOfWork.GetRepository<ExclusionsItem>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateProjectExclusionsGroup(ExclusionsGroup exclusionGroup)
        {
            _unitOfWork.GetRepository<ExclusionsGroup>().Update(exclusionGroup, exclusionGroup.Id);
            _unitOfWork.Commit();
        }
        public void UpdateExclusionGroupItem(ExclusionsItem exclusionItem)
        {
            _unitOfWork.GetRepository<ExclusionsItem>().Update(exclusionItem, exclusionItem.Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Escalations ]

        #region [ SQL Escalations ]

        public List<EscalationsFrameworkRuleConfigSQL> ListProjectEscalationEmailSQLConfigs(Guid projectId)
        {
            List<EscalationsFrameworkRuleConfigSQL> configs = _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigSQL>().GetAllNoTrack().Where(r => r.EscalationsFrameworkId == projectId).Include("Schedule").Include("TargetDb").OrderBy(o => o.Name).ToList();
            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.TargetDbName = a.TargetDb.DisplayName;
                a.Schedule = null;
                a.TargetDb = null;
                return a;
            }).ToList();
        }
        public void AddProjectEscalationSQLConfig(EscalationsFrameworkRuleConfigSQL ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigSQL>().Create(ruleConfig);
            _unitOfWork.Commit();
        }
        public void DeleteProjectEscalationSQLConfig(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigSQL>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateProjectEscalationSQLConfig(EscalationsFrameworkRuleConfigSQL ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigSQL>().Update(ruleConfig, ruleConfig.Id);
            _unitOfWork.Commit();
        }
        #endregion

        #region [ Generic Email Escalation ]

        public List<EscalationsFrameworkRuleConfigEmailGeneric> ListProjectEscalationEmailGenericConfigs(Guid projectId)
        {
            List<EscalationsFrameworkRuleConfigEmailGeneric> configs = _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailGeneric>().GetAllNoTrack().Where(r => r.EscalationsFrameworkId == projectId).Include("Schedule").OrderBy(o => o.Name).ToList();
            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.Schedule = null;
                return a;
            }).ToList();
        }
        public void AddProjectEscalationGenericConfig(EscalationsFrameworkRuleConfigEmailGeneric ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailGeneric>().Create(ruleConfig);
            _unitOfWork.Commit();
        }
        public void DeleteProjectEscalationGenericConfig(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailGeneric>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateProjectEscalationGenericConfig(EscalationsFrameworkRuleConfigEmailGeneric ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailGeneric>().Update(ruleConfig, ruleConfig.Id);
            _unitOfWork.Commit();
        }
        #endregion

        public string GetEscalationsEmailBody(int ItemID)
        {
            return _unitOfWork.GetRepository<EmailRuleConfigEscalationsActionTakenLog>().GetAll().Where(r => r.Id == ItemID).FirstOrDefault().EmailBody;
        }
        public List<EscalationsEmailDTO> ListEscalationsEmailHistory(int escalationsConfigId)
        {
            return _unitOfWork.GetRepository<EmailRuleConfigEscalationsActionTakenLog>()
                .GetAll()
                .Where(r => r.EscalationsFrameworkRuleConfigId == escalationsConfigId && r.ExecutionOutcome == EscalationsActionTakenLogOutcome.ExecutionSuccess && !string.IsNullOrEmpty(r.EmailFrom))
                .Select(a => new EscalationsEmailDTO { Id = a.Id, EmailFrom = a.EmailFrom, EmailSubject = a.EmailSubject, EmailTo = a.EmailTo, TimeStamp = a.TimeStamp, UserName = a.UserName })
                .ToList();
        }
        public string ExecuteEscalationsRuleRoleConfig(int escalationConfigId, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            string returnText = string.Empty;

            EscalationsFrameworkRuleConfigEmailRole ruleConfig = _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailRole>().GetAll().Where(r => r.Id == escalationConfigId).FirstOrDefault();
            if (string.IsNullOrEmpty(ruleConfig.OverrideRecipientEmail))
            {
                returnText = "Override Recipient Email is required!";
            }
            else
            {
                ruleConfig.RunEscalationConfiguration(_unitOfWork.Context, false, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
            }

            return returnText;
        }
        public string ExecuteEscalationsRuleUserConfig(int escalationConfigId, string ServerRazorEmailTemplatePathBody, string ServerRazorEmailTemplatePathSubject, string ServerRazorEmailTemplatePathAttachment)
        {
            string returnText = string.Empty;
            EscalationsFrameworkRuleConfigEmail ruleConfig = _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmail>().GetAll().Where(r => r.Id == escalationConfigId).FirstOrDefault();
            ruleConfig.RunEscalationConfiguration(_unitOfWork.Context, false, ServerRazorEmailTemplatePathBody, ServerRazorEmailTemplatePathSubject, ServerRazorEmailTemplatePathAttachment);
            return returnText;
        }

        public List<Project> ListProjectDropDownEditorEFUserProjectBreachSource(Guid ProjectID, int escalationsConfigId)
        {
            List<Project> projects = _unitOfWork.GetRepository<Project>().GetAll().Where(r => r.ProjectUniqueKey == ProjectID).OrderBy(o => o.ProjectName).ToList();
            List<Project> ruleParticipantProject = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceProject>().GetAll().Where(r => r.EscalationsFrameworkRuleConfigId == escalationsConfigId).Include("Project").Select(p => p.Project).ToList();

            foreach (Project p in ruleParticipantProject)
            {
                projects.Remove(p);
            }
            return projects;
        }
        public List<Office> ListProjectDropDownEditorEFUserOfficeBreachSource(Guid ProjectID, int escalationsConfigId)
        {
            List<Office> offices = _unitOfWork.GetRepository<Office>().GetAll().Where(r => r.ProjectId == ProjectID).OrderBy(o => o.Name).ToList();
            List<Office> breachSourceOffices = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceOffice>().GetAll().Where(r => r.EscalationsFrameworkRuleConfigId == escalationsConfigId).Include("Office").Select(p => p.Office).ToList();

            foreach (Office p in breachSourceOffices)
            {
                offices.Remove(p);
            }
            return offices;
        }
        public List<Team> ListProjectDropDownEditorEFTeamBreachSource(Guid ProjectID, int escalationsConfigId)
        {
            List<Team> teams = _unitOfWork.GetRepository<Team>().GetAll().Where(r => r.ProjectId == ProjectID).OrderBy(o => o.Name).ToList();
            List<Team> breachSourceTeams = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceTeam>().GetAll().Where(r => r.EscalationsFrameworkRuleConfigId == escalationsConfigId).Include("Team").Select(p => p.Team).ToList();

            foreach (Team p in breachSourceTeams)
            {
                teams.Remove(p);
            }
            return teams;
        }
        public List<User> ListProjectDropDownEditorEFUserBreachSource(Guid ProjectID, int escalationsConfigId)
        {
            List<User> teams = ListProjectUsersWithPermissions(ProjectID);
            List<User> breachSourceTeams = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceUser>().GetAll().Where(r => r.EscalationsFrameworkRuleConfigId == escalationsConfigId).Include("User").Select(p => p.User).ToList();

            foreach (User p in breachSourceTeams)
            {
                teams.Remove(p);
            }
            return teams;
        }
        public List<Rule> ListProjectDropDownEditorEFRuleBreachSource(Guid ProjectID, int escalationsConfigId)
        {
            List<Rule> rules = ListProjectRules(ProjectID);
            List<Rule> breachSourceTeams = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRule>().GetAll().Where(r => r.EscalationsFrameworkRuleConfigId == escalationsConfigId).Include("Rule").Select(p => p.Rule).ToList();

            foreach (Rule p in breachSourceTeams)
            {
                rules.Remove(p);
            }
            return rules;
        }
        public List<RuleConfiguration> ListProjectDropDownEditorEFRuleConfigBreachSource(Guid ProjectID, int escalationsConfigId)
        {
            List<RuleConfiguration> rules = ListProjectRuleConfigurations(ProjectID);
            List<RuleConfiguration> breachSourceRuleConfigs = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRuleConfiguration>().GetAll().Where(r => r.EscalationsFrameworkRuleConfigId == escalationsConfigId).Include("RuleConfiguration").Select(p => p.RuleConfiguration).ToList();

            foreach (RuleConfiguration p in breachSourceRuleConfigs)
            {
                rules.Remove(p);
            }
            return rules;
        }
        public List<EscalationsFrameworkBreachSourceOffice> ListEFRuleConfigOfficeBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceOffice> configs = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceOffice>().GetAllNoTrack().Where(r => r.EscalationsFrameworkRuleConfigId == ruleConfigId).Include("Office").ToList();
            return configs.Select(a =>
            {
                a.OfficeName = a.Office.Name;
                a.AlsoKnownAs = a.Office.AlsoKnownAs;
                a.ActurisOrganisationKey = a.Office.ActurisOrganisationKey;
                a.ActurisOrganisationName = a.Office.ActurisOrganisationName;
                a.Office = null;
                return a;
            }).ToList();
        }
        public List<EscalationsFrameworkBreachSourceRule> ListEFRuleConfigRuleBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceRule> configs = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRule>().GetAllNoTrack().Where(r => r.EscalationsFrameworkRuleConfigId == ruleConfigId).Include("Rule").ToList();
            return configs.Select(a =>
            {
                a.RuleName = a.Rule.Name;
                a.Rule = null;
                return a;
            }).ToList();
        }
        public List<EscalationsFrameworkBreachSourceRuleConfiguration> ListEFRuleConfigRuleConfigBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceRuleConfiguration> configs = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRuleConfiguration>().GetAllNoTrack().Where(r => r.EscalationsFrameworkRuleConfigId == ruleConfigId).Include("RuleConfiguration").ToList();
            return configs.Select(a =>
            {
                a.RuleConfigName = a.RuleConfiguration.Name;
                a.RuleConfiguration = null;
                return a;
            }).ToList();
        }
        public List<EscalationsFrameworkBreachSourceTeam> ListEFRuleConfigTeamBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceTeam> configs = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceTeam>().GetAllNoTrack().Where(r => r.EscalationsFrameworkRuleConfigId == ruleConfigId).Include("Team").ToList();
            return configs.Select(a =>
            {
                a.TeamName = a.Team.Name;
                a.AlsoKnownAs = a.Team.AlsoKnownAs;
                a.ActurisOrganisationKey = a.Team.ActurisOrganisationKey;
                a.ActurisOrganisationName = a.Team.ActurisOrganisationName;
                a.Team = null;
                return a;
            }).ToList();
        }
        public List<EscalationsFrameworkBreachSourceUser> ListEFRuleConfigUserBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceUser> configs = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceUser>().GetAllNoTrack().Where(r => r.EscalationsFrameworkRuleConfigId == ruleConfigId).Include("User").ToList();
            return configs.Select(a =>
            {
                a.UserName = a.User.Name;
                a.AlsoKnownAs = a.User.AlsoKnownAs;
                a.ActurisOrganisationKey = a.User.ActurisOrganisationKey;
                a.ActurisOrganisationName = a.User.ActurisOrganisationName;
                a.User = null;
                return a;
            }).ToList();
        }
        public List<EscalationsFrameworkBreachSourceProject> ListEFRuleConfigProjectBreachSources(int ruleConfigId)
        {
            List<EscalationsFrameworkBreachSourceProject> configs = _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceProject>().GetAllNoTrack().Where(r => r.EscalationsFrameworkRuleConfigId == ruleConfigId).Include("Project").ToList();
            return configs.Select(a =>
            {
                a.ProjectName = a.Project.ProjectDisplayName;
                a.Project = null;
                return a;
            }).ToList();
        }
        public void AddEFRuleConfigProjectBreachSource(EscalationsFrameworkBreachSourceProject breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceProject>().Create(breachSource);
            _unitOfWork.Commit();
        }
        public void AddEFRuleConfigOfficeBreachSource(EscalationsFrameworkBreachSourceOffice breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceOffice>().Create(breachSource);
            _unitOfWork.Commit();
        }
        public void AddEFRuleConfigTeamBreachSource(EscalationsFrameworkBreachSourceTeam breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceTeam>().Create(breachSource);
            _unitOfWork.Commit();
        }
        public void AddEFRuleConfigUserBreachSource(EscalationsFrameworkBreachSourceUser breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceUser>().Create(breachSource);
            _unitOfWork.Commit();
        }
        public void AddEFRuleConfigRuleBreachSource(EscalationsFrameworkBreachSourceRule breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRule>().Create(breachSource);
            _unitOfWork.Commit();
        }
        public void AddEFRuleConfigRuleConfigBreachSource(EscalationsFrameworkBreachSourceRuleConfiguration breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRuleConfiguration>().Create(breachSource);
            _unitOfWork.Commit();
        }
        public void DeleteEFRuleConfigProjectBreachSource(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceProject>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteEFRuleConfigOfficeBreachSource(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceOffice>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteEFRuleConfigTeamBreachSource(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceTeam>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteEFRuleConfigUserBreachSource(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceUser>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteEFRuleConfigRuleBreachSource(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRule>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteEFRuleConfigRuleConfigBreachSource(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRuleConfiguration>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateEFRuleConfigProjectBreachSource(EscalationsFrameworkBreachSourceProject breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceProject>().Update(breachSource, breachSource.Id);
            _unitOfWork.Commit();
        }
        public void UpdateEFRuleConfigOfficeBreachSource(EscalationsFrameworkBreachSourceOffice breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceOffice>().Update(breachSource, breachSource.Id);
            _unitOfWork.Commit();
        }
        public void UpdateEFRuleConfigTeamBreachSource(EscalationsFrameworkBreachSourceTeam breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceTeam>().Update(breachSource, breachSource.Id);
            _unitOfWork.Commit();
        }
        public void UpdateEFRuleConfigUserBreachSource(EscalationsFrameworkBreachSourceUser breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceUser>().Update(breachSource, breachSource.Id);
            _unitOfWork.Commit();
        }
        public void UpdateEFRuleConfigRuleBreachSource(EscalationsFrameworkBreachSourceRule breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRule>().Update(breachSource, breachSource.Id);
            _unitOfWork.Commit();
        }
        public void UpdateEFRuleConfigRuleConfigBreachSource(EscalationsFrameworkBreachSourceRuleConfiguration breachSource)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkBreachSourceRuleConfiguration>().Update(breachSource, breachSource.Id);
            _unitOfWork.Commit();
        }

        public List<EscalationsFrameworkRuleConfigEmailUser> ListProjectEscalationEmailUserConfigs(Guid projectId)
        {
            List<EscalationsFrameworkRuleConfigEmailUser> configs = _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailUser>().GetAllNoTrack().Where(r => r.EscalationsFrameworkId == projectId).Include("Schedule").OrderBy(o => o.Name).ToList();
            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.Schedule = null;
                return a;
            }).ToList();
        }
        public List<EscalationsFrameworkRuleConfigEmailRole> ListProjectEscalationEmailRoleConfigs(Guid projectId)
        {
            List<EscalationsFrameworkRuleConfigEmailRole> configs = _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailRole>().GetAllNoTrack().Where(r => r.EscalationsFrameworkId == projectId).Include("Schedule").Include("Action").OrderBy(o => o.Name).ToList();
            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.ActionName = a.Action.Name;
                a.Action = null;
                a.Schedule = null;
                return a;
            }).ToList();
        }
         public List<EscalationsEmailRecipient> ListProjectEscalationEmailRoleConfigRecipients(int efConfigId)
        {
            List<EscalationsEmailRecipient> configs = _unitOfWork.GetRepository<EscalationsEmailRecipient>().GetAllNoTrack().Where(r => r.EscalationsFrameworkRuleConfigId == efConfigId).Include("Recipient").ToList();
            return configs.Select(a =>
            {
                a.RecipientName = a.Recipient.Name;
                a.RecipientEmail = a.Recipient.Email;
                a.Recipient = null;
                return a;
            }).ToList();
        }
        public void AddProjectEscalationUsersConfig(EscalationsFrameworkRuleConfigEmailUser ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailUser>().Create(ruleConfig);
            _unitOfWork.Commit();
        }
        public void AddPEscalationUsersRuleConfigRecipient(EscalationsEmailRecipient recipient)
        {
            _unitOfWork.GetRepository<EscalationsEmailRecipient>().Create(recipient);
            _unitOfWork.Commit();
        }
        public void AddProjectEscalationRolesConfig(EscalationsFrameworkRuleConfigEmailRole ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailRole>().Create(ruleConfig);
            _unitOfWork.Commit();
        }
        public List<EscalationsFrameworkAction> ListActionsForEscalationConfigRolesDropDown()
        {
            List<EscalationsFrameworkAction> actions = _unitOfWork.GetRepository<EscalationsFrameworkAction>().GetAll().OrderBy(o => o.Name).ToList();
            return actions;
        }
        public void DeleteEscalationUsersRuleConfigRecipient(int Id)
        {
            _unitOfWork.GetRepository<EscalationsEmailRecipient>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteProjectEscalationUsersConfig(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailUser>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void DeleteProjectEscalationRolesConfig(int Id)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailRole>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateProjectEscalationUsersConfig(EscalationsFrameworkRuleConfigEmailUser ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailUser>().Update(ruleConfig, ruleConfig.Id);
            _unitOfWork.Commit();
        }
        public void UpdateProjectEscalationRolesConfig(EscalationsFrameworkRuleConfigEmailRole ruleConfig)
        {
            _unitOfWork.GetRepository<EscalationsFrameworkRuleConfigEmailRole>().Update(ruleConfig, ruleConfig.Id);
            _unitOfWork.Commit();
        }
        public void UpdateEscalationUsersRuleConfigRecipient(EscalationsEmailRecipient recipient)
        {
            _unitOfWork.GetRepository<EscalationsEmailRecipient>().Update(recipient, recipient.Id);
            _unitOfWork.Commit();
        }
        public List<User> ListUsersForEscalationsConfigParticipantsDropDown(Guid projectId, int escalationsRuleConfigId)
        {
            List<User> projectUsers = _unitOfWork.GetRepository<ProjectPermission>().GetAll().Where(a => a.ProjectId == projectId).Include("User").Select(u => u.User).GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault()).ToList().OrderBy(o => o.Name).ToList();
            List<User> configRecipients = _unitOfWork.GetRepository<EscalationsEmailRecipient>().GetAll().Where(a => a.EscalationsFrameworkRuleConfigId == escalationsRuleConfigId).Include("Recipient").Select(u => u.Recipient).GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault()).ToList().OrderBy(o => o.Name).ToList();

            foreach (User user in configRecipients)
            {
                projectUsers.Remove(user);
            }

            return projectUsers;
        }
        public void CreateEscalationsFramework(EscalationsFramework escalationsFramework)
        {
            _unitOfWork.GetRepository<EscalationsFramework>().Create(escalationsFramework);
            _unitOfWork.Commit();
        }
        public void DeleteEscalationsFramework(Guid Id)
        {
            _unitOfWork.GetRepository<EscalationsFramework>().Delete(Id);
            _unitOfWork.Commit();
        }
        #endregion

        #region [ SQL input ]

        public void UpdateRuleConfigHardCodedInputItem(RuleStoredProcedureInputValueHardCoded inputValue)
        {
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueHardCoded>().Update(inputValue, inputValue.Id);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfigHardCodedInputItem(int Id)
        {
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueHardCoded>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void AddRuleConfigHardCodedInputItem(RuleStoredProcedureInputValueHardCoded InputItem)
        {
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueHardCoded>().Create(InputItem);
            _unitOfWork.Commit();
        }
        public List<RuleStoredProcedureInputValueHardCoded> ListRuleConfigHardCodedInputItems(int ruleConfigId)
        {
            return _unitOfWork.GetRepository<RuleStoredProcedureInputValueHardCoded>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).OrderBy(o => o.ParameterName).ToList();
        }
        public void UpdateRuleConfigExclusionsGroupInputItem(RuleStoredProcedureInputValueExclusionsGroup ExclusionsGroup)
        {
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueExclusionsGroup>().Update(ExclusionsGroup, ExclusionsGroup.Id);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfigExclusionsGroupInputItem(int Id)
        {
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueExclusionsGroup>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void AddRuleConfigExclusionsGroupInputItem(RuleStoredProcedureInputValueExclusionsGroup ExclusionsGroup)
        {
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueExclusionsGroup>().Create(ExclusionsGroup);
            _unitOfWork.Commit();
        }
        public List<RuleStoredProcedureInputValueExclusionsGroup> ListRuleConfigExclusionsGroupInputItems(int ruleConfigId)
        {
            List<RuleStoredProcedureInputValueExclusionsGroup> exclusionsGroups = _unitOfWork.GetRepository<RuleStoredProcedureInputValueExclusionsGroup>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("ExclusionsGroup").OrderBy(o => o.ParameterName).ToList();

            return exclusionsGroups.Select(a =>
            {
                a.ExclusionsGroupName = a.ExclusionsGroup.GroupName;
                a.ExclusionsGroup = null;
                return a;
            }).ToList();
        }
        public List<ExclusionsGroup> ListRuleConfigExclusionsGroupRefInputList(Guid ProjectID, int ruleConfigId)
        {
            return _unitOfWork.GetRepository<ExclusionsGroup>().GetAll().Where(r => r.ProjectId == ProjectID).OrderBy(o => o.GroupName).ToList();
            //List<ExclusionsGroup> ruleParticipantOffices = _unitOfWork.GetRepository<RuleStoredProcedureInputValueExclusionsGroup>().GetAll().Where(r => r.RuleConfigurationId == ruleConfigId).Include("ExclusionsGroup").Select(p => p.ExclusionsGroup).ToList();

            //foreach (ExclusionsGroup p in ruleParticipantOffices)
            //{
            //    ExclusiuonsGroups.Remove(p);
            //}
            //return ExclusiuonsGroups;
        }
        public List<ClassInputValueDTO> ListRuleConfigClassRefInputItems(int ruleConfigId)
        {
            List<RuleStoredProcedureInputValueClassReference> inputValues = _unitOfWork
                .GetRepository<RuleStoredProcedureInputValueClassReference>()
                .GetAll().Where(r => r.RuleConfigurationId == ruleConfigId)
                .OrderBy(o => o.ParameterName)
                .ToList();

            return inputValues.Select(s => new ClassInputValueDTO()
            {
                Id = s.Id,
                ClassProperty = string.Format("{0}-{1}", s.ClassName, s.ClassPropertyName),
                Description = s.Description,
                ParameterName = s.ParameterName,
                ProjectId = s.ProjectId,
                RuleConfigId = s.RuleConfigurationId
            }).ToList();
        }
        public void AddRuleConfigClassRefInputItem(ClassInputValueDTO inputValue)
        {
            RuleStoredProcedureInputValueClassReference classRef = new RuleStoredProcedureInputValueClassReference()
            {
                ClassName = inputValue.ClassProperty.Split('-')[0],
                ClassPropertyName = inputValue.ClassProperty.Split('-')[1],
                Description = inputValue.Description,
                ParameterName = inputValue.ParameterName,
                ProjectId = inputValue.ProjectId,
                RuleConfigurationId = inputValue.RuleConfigId
            };

            _unitOfWork.GetRepository<RuleStoredProcedureInputValueClassReference>().Create(classRef);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfigClassRefInputItemm(int Id)
        {
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueClassReference>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateRuleConfigClassRefInputItem(ClassInputValueDTO inputValue)
        {
            RuleStoredProcedureInputValueClassReference classRef = new RuleStoredProcedureInputValueClassReference()
            {
                Id = inputValue.Id,
                ClassName = inputValue.ClassProperty.Split('-')[0],
                ClassPropertyName = inputValue.ClassProperty.Split('-')[1],
                Description = inputValue.Description,
                ParameterName = inputValue.ParameterName,
                ProjectId = inputValue.ProjectId,
                RuleConfigurationId = inputValue.RuleConfigId
            };
            _unitOfWork.GetRepository<RuleStoredProcedureInputValueClassReference>().Update(classRef, classRef.Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Rules ]

        #region [ SQL Rule ]

        public List<Rule> ListProjectRules(Guid projectId)
        {
            return _unitOfWork.GetRepository<Rule>().GetAll().Where(r => r.ProjectId == projectId && r.IsDeleted != true).OrderBy(o=>o.Name).ToList();
        }
        public List<RuleConfiguration> ListProjectRuleConfigurations(Guid projectId)
        {
            return _unitOfWork.GetRepository<RuleConfiguration>().GetAll().Where(r => r.ProjectId == projectId && r.IsDeleted != true).OrderBy(o => o.Name).ToList();
        }
        public List<RuleConfiguration> ListRuleConfigurations(int ruleId)
        {
            List<RuleConfiguration> configs = _unitOfWork.GetRepository<RuleConfiguration>().GetAllNoTrack().Include("TargetDb").Include("Schedule").Include("SetBreachesToResolvedSchedule").Where(r => r.RuleId == ruleId && r.IsDeleted != true).OrderBy(o => o.Name).ToList();

            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.TargetDbName = a.TargetDb.DBName;
                a.SetBreachesToResolvedScheduleName = a.SetBreachesToResolvedSchedule.Name;
                a.Schedule = null;
                a.TargetDb = null;
                a.SetBreachesToResolvedSchedule = null;
                return a;
            }).ToList();
        }

        public void AddRuleConfiguration(RuleConfiguration ruleConfig)
        {
            _unitOfWork.GetRepository<RuleConfiguration>().Create(ruleConfig);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfiguration(int Id)
        {
            RuleConfiguration rc = _unitOfWork.GetRepository<RuleConfiguration>().GetAllNoTrack().Where(r => r.Id == Id).FirstOrDefault();
            rc.IsActive = false;
            rc.IsDeleted = true;
            UpdateRuleConfiguration(rc);
            //_unitOfWork.GetRepository<RuleConfiguration>().Delete(Id);
            //_unitOfWork.Commit();
        }
        public void UpdateRuleConfiguration(RuleConfiguration ruleConfig)
        {
            _unitOfWork.GetRepository<RuleConfiguration>().Update(ruleConfig, ruleConfig.Id);
            _unitOfWork.Commit();
        }
        public void UpdateRule(Rule rule)
        {
            _unitOfWork.GetRepository<Rule>().Update(rule, rule.Id);
            _unitOfWork.Commit();
        }
        public void DeleteRule(int Id)
        {
            Rule rule =_unitOfWork.GetRepository<Rule>().GetAllNoTrack().Where(r => r.Id == Id).FirstOrDefault();
            rule.IsActive = false;
            rule.IsDeleted = true;
            UpdateRule(rule);
        }
        public void AddRule(Rule rule)
        {
            _unitOfWork.GetRepository<Rule>().Create(rule);
            _unitOfWork.Commit();
        }
        public List<Schedule> ListSchedulesForRuleConfigDropDown(Guid projectId)
        {
            List<Schedule> projectSchedules = _unitOfWork.GetRepository<Schedule>().GetAll().Where(a => a.ProjectId == projectId).OrderBy(o => o.Name).ToList();
            return projectSchedules;
        }
        public List<TargetDatabaseDetails> ListTargetDbsForRuleConfigDropDown(Guid projectId)
        {
            List<TargetDatabaseDetails> DBDetails = _unitOfWork.GetRepository<TargetDatabaseDetails>().GetAll().Where(a => a.ProjectId == projectId).OrderBy(o => o.DisplayName).ToList();
            return DBDetails;
        }

        #endregion

        #region [ Exchange rule ]

        public List<string> GetBreachMappingFieldNamesForExchangeRuleConfig(int ruleConfigId)
        {
            List<string> names = new List<string>();
            ExchangeEmailRuleConfig config = _unitOfWork.GetRepository<ExchangeEmailRuleConfig>().GetAllNoTrack().Include("BreachFieldMappings").Where(r => r.Id == ruleConfigId).FirstOrDefault();
            foreach (var mapping in config.BreachFieldMappings)
            {
                names.Add(mapping.MappedToBreachTableColumnName);
            }

            return names;
        }
        public List<ExchangeEmailRuleConfigBreachFieldMappings> ListExchangeRuleConfigValueMappingItems(int ruleConfigId)
        {
            List<ExchangeEmailRuleConfigBreachFieldMappings> configs = _unitOfWork.GetRepository<ExchangeEmailRuleConfigBreachFieldMappings>().GetAllNoTrack().Where(r => r.ExchangeRuleConfigId == ruleConfigId).OrderBy(o => o.Name).ToList();
            return configs.Select(a =>
            {
                a.UpdateNotMappedFields();
                return a;
            }).ToList();
        }
        public List<ExchangeEmailRuleConfig> ListExchangeRuleConfigurations(int ruleId)
        {
            List<ExchangeEmailRuleConfig> configs = _unitOfWork.GetRepository<ExchangeEmailRuleConfig>().GetAllNoTrack().Include("Schedule").Include("ExchangeAccountDetails").Where(r => r.RuleId == ruleId).OrderBy(o => o.Name).ToList();

            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.ExchangeAccountDetailsName = a.ExchangeAccountDetails.DisplayName;
                a.ExchangeAccountDetails = null;
                a.Schedule = null;
                a.UpdateNotMappedFields();
                return a;
            }).ToList();
        }
        public void AddExchangeRuleConfiguration(ExchangeEmailRuleConfig ruleConfig)
        {
            _unitOfWork.GetRepository<ExchangeEmailRuleConfig>().Create(ruleConfig);
            _unitOfWork.Commit();
        }
        public void AddExchangeRuleConfigValueMapping(ExchangeEmailRuleConfigBreachFieldMappings ruleConfigValueMapping)
        {
            _unitOfWork.GetRepository<ExchangeEmailRuleConfigBreachFieldMappings>().Create(ruleConfigValueMapping);
            _unitOfWork.Commit();
        }
        public void DeleteExchangeRuleConfiguration(int Id)
        {
            _unitOfWork.GetRepository<ExchangeEmailRuleConfig>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateExchangeRuleConfiguration(ExchangeEmailRuleConfig ruleConfig)
        {
            _unitOfWork.GetRepository<ExchangeEmailRuleConfig>().Update(ruleConfig, ruleConfig.Id);
            _unitOfWork.Commit();
        }
        //
        public void UpdateExchangeRuleConfigValueMappingItems(ExchangeEmailRuleConfigBreachFieldMappings ruleConfigBreachFieldMapping)
        {
            _unitOfWork.GetRepository<ExchangeEmailRuleConfigBreachFieldMappings>().Update(ruleConfigBreachFieldMapping, ruleConfigBreachFieldMapping.Id);
            _unitOfWork.Commit();
        }
        public void DeleteRuleConfigBreachFieldMappings(int Id)
        {
            _unitOfWork.GetRepository<ExchangeEmailRuleConfigBreachFieldMappings>().Delete(Id);
            _unitOfWork.Commit();
        }

        #endregion


        #region [ Excel Rule ]

        public List<ExcelRuleConfig> ListExcelRuleConfigurations(int ruleId)
        {
            List<ExcelRuleConfig> configs = _unitOfWork.GetRepository<ExcelRuleConfig>().GetAllNoTrack().Include("Schedule").Where(r => r.RuleId == ruleId).OrderBy(o => o.Name).ToList();

            return configs.Select(a =>
            {
                a.ScheduleName = a.Schedule.Name;
                a.Schedule = null;
                return a;
            }).ToList();
        }
        public void DeleteExcelRuleConfiguration(int Id)
        {
            _unitOfWork.GetRepository<ExcelRuleConfig>().Delete(Id);
            _unitOfWork.Commit();
        }
        public void UpdateExcelRuleConfiguration(ExcelRuleConfig ruleConfig)
        {
            _unitOfWork.GetRepository<ExcelRuleConfig>().Update(ruleConfig, ruleConfig.Id);
            _unitOfWork.Commit();
        }
        public void AddExcelRuleConfiguration(ExcelRuleConfig ruleConfig)
        {
            _unitOfWork.GetRepository<ExcelRuleConfig>().Create(ruleConfig);
            _unitOfWork.Commit();
        }

        #endregion

        #endregion

        #region [ Team ]

        public List<Team> ListOfficeTeams(int officeId)
        {
            List<Team> teams = _unitOfWork.GetRepository<Team>().GetAllNoTrack().Include("Office").Where(u => u.OfficeId == officeId).OrderBy(o => o.Name).ToList();

            return teams.Select(a => { a.OfficeName = a.Office.Name; a.Office = null; return a; }).ToList();
        }

        public List<Team> ListProjectTeams(Guid projectGuid)
        {
            List<Team> teams = _unitOfWork.GetRepository<Team>().GetAllNoTrack().Include("Office").Where(u => u.ProjectId == projectGuid).OrderBy(o => o.Name).ToList();

            return teams.Select(a => { a.OfficeName = a.Office.Name; a.Office = null; return a; }).ToList();
        }

        public Team UpdateTeam(Team team)
        {
            _unitOfWork.GetRepository<Team>().Update(team, team.Id);
            _unitOfWork.Commit();
            Team teamReturn = _unitOfWork.GetRepository<Team>().GetAllNoTrack().Include("Office").Where(u => u.Id == team.Id).FirstOrDefault();
            teamReturn.OfficeName = teamReturn.Office.Name;
            teamReturn.Office = null;
            return teamReturn;
        }

        public void AddTeam(Team team)
        {
            _unitOfWork.GetRepository<Team>().Create(team);
            _unitOfWork.Commit();
        }

        public void DeleteTeam(int Id)
        {
            _unitOfWork.GetRepository<Team>().Delete(Id);
            _unitOfWork.Commit();
        }

        #endregion

        #region [ Users ]

        public List<User> ListTeamUsers(int teamId)
        {
            var users = (from u in _unitOfWork.Context.User
                         join perms in _unitOfWork.Context.TeamPermissions on u.Id equals perms.UserId
                         where perms.TeamId == teamId
                         select new { user = u, permissions = perms } into up
                         group up by up.user.Id into ug
                         select new
                         {
                             User = ug.FirstOrDefault().user,
                             IsTeamLead = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.TeamLead).Any(),
                             IsClaimsHandler = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.ClaimsHandler).Any()
                         }).ToList();

            foreach (var u in users)
            {
                if (u.IsTeamLead)
                {
                    u.User.IsTeamLead = true;
                }
                if (u.IsClaimsHandler)
                {
                    u.User.IsClaimsHandler = true;
                }
            }

            return users.Select(u => u.User).ToList();
        }
        public List<User> ListOfficeUsers(int officeId)
        {
            var users = (from u in _unitOfWork.Context.User
                         join perms in _unitOfWork.Context.OfficePermissions on u.Id equals perms.UserId
                         where perms.OfficeId == officeId
                         select new { user = u, permissions = perms } into up
                         group up by up.user.Id into ug
                         select new
                         {
                             User = ug.FirstOrDefault().user,
                             IsOfficeManager = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.BranchManager).Any(),
                             IsQA = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.QualityAuditor).Any(),
                             IsRM = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.RegionalManager).Any()
                         }).ToList();

            foreach (var u in users)
            {
                if (u.IsOfficeManager)
                {
                    u.User.IsOfficeManager = true;
                }
                if (u.IsQA)
                {
                    u.User.IsOfficeQualityAuditor = true;
                }
                if (u.IsRM)
                {
                    u.User.IsOfficeRegionalManager = true;
                }
            }

            return users.Select(u => u.User).ToList();
        }
        public List<User> ListProjectUsersForOfficeUsersDropDown(Guid projectId, int officeId)
        {
            List<User> projectUsers = _unitOfWork.GetRepository<ProjectPermission>().GetAll().Where(a => a.ProjectId == projectId).Include("User").Select(u => u.User).GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault()).ToList().OrderBy(o => o.Name).ToList();
            List<User> officeUsers = _unitOfWork.GetRepository<OfficePermission>().GetAll().Where(a => a.OfficeId == officeId).Include("User").Select(u => u.User).GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault()).ToList().OrderBy(o => o.Name).ToList();

            foreach (User user in officeUsers)
            {
                projectUsers.Remove(user);
            }

            return projectUsers;
            //return projectUsersNotInOffice.Select(a => { a.IsOfficeManager = a.HasRole(_unitOfWork.Context, officeId, RoleEnum.BranchManager); a.Permissions = null; return a; }).ToList();
        }
        public List<User> ListUsersForTeamUsersDropDown(Guid projectId, int teamId)
        {
            List<User> projectUsers = _unitOfWork.GetRepository<ProjectPermission>().GetAll().Where(a => a.ProjectId == projectId).Include("User").Select(u => u.User).GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault()).ToList().OrderBy(o => o.Name).ToList();
            List<User> teamUsers = _unitOfWork.GetRepository<TeamPermission>().GetAll().Where(a => a.TeamId == teamId).Include("User").Select(u => u.User).GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault()).ToList().OrderBy(o => o.Name).ToList();

            foreach (User user in teamUsers)
            {
                projectUsers.Remove(user);
            }

            return projectUsers;
            //return projectUsersNotInOffice.Select(a => { a.IsOfficeManager = a.HasRole(_unitOfWork.Context, officeId, RoleEnum.BranchManager); a.Permissions = null; return a; }).ToList();
        }
        public List<User> ListAllSystemUsersWithPermissions()
        {
            var users = (from u in _unitOfWork.Context.User
                         join ps in _unitOfWork.Context.SystemPermissions on u.Id equals ps.UserId into perms
                         from ps in perms.DefaultIfEmpty()
                         select new { user = u, permissions = ps } into up
                         group up by up.user.Id into ug
                         select new
                         {
                             User = ug.FirstOrDefault().user,
                             IsSystemAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SystemAdmin).Any(),
                             IsSystemSuperUser = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SuperUser).Any(),
                             SystemSuperUserInfo = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SuperUser).FirstOrDefault().permissions.Info
                         }).ToList();

            foreach (var u in users)
            {
                if (u.IsSystemAdmin)
                {
                    u.User.IsSystemAdmin = true;
                }
                if (u.IsSystemSuperUser)
                {
                    u.User.IsSystemSuperUser = true;
                    u.User.SystemSuperUserInfo = string.IsNullOrEmpty(u.SystemSuperUserInfo) ? string.Empty : u.SystemSuperUserInfo;
                }                
            }
            return users.Select(u => u.User).ToList();
        }
        public List<User> ListProjectUsers(Guid projectGuid)
        {
            //return _unitOfWork.GetRepository<ProjectPermission>().GetAll().Where(a => a.ProjectId == projectGuid).Include("User").Select(u => u.User).GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault()).Include("Permissions").ToList().OrderBy(o => o.Name).ToList();
            return _unitOfWork.GetRepository<ProjectPermission>()
                .GetAllNoTrack()
                .Where(a => a.ProjectId == projectGuid).Include("User")
                .Select(u => u.User)
                .GroupBy(g => g.Id).Select(grp => grp.FirstOrDefault())
                .ToList();

        }
        public List<User> ListProjectUsersWithPermissions(Guid projectGuid)
        {
            var user2s = (from u in _unitOfWork.Context.User
                          join perms in _unitOfWork.Context.ProjectPermissions on u.Id equals perms.UserId
                          //where perms.ProjectId == projectGuid
                          select new { user = u, permissions = perms } into up
                          group up by up.user.Id into ug
                          select new
                          {
                              User = ug.FirstOrDefault().user,
                              IsInCurrentProject = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.ProjectMember && q.permissions.ProjectId == projectGuid).Any(),
                              IsProjectAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.ProjectAdmin && q.permissions.ProjectId == projectGuid).Any(),
                              IsMicroServiceAdmin = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.MicroService && q.permissions.ProjectId == projectGuid).Any(),
                              IsProjectSuperUser = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SuperUser && q.permissions.ProjectId == projectGuid).Any(),
                              ProjectSuperUserInfo = ug.Where(q => q.permissions.RoleId == (int)RoleEnum.SuperUser && q.permissions.ProjectId == projectGuid).FirstOrDefault().permissions.Info
                          }).ToList();

            List<User> returnUsers = new List<User>();
            foreach (var u in user2s)
            {
                if (u.IsInCurrentProject)
                {
                    if (u.IsProjectAdmin)
                    {
                        u.User.IsProjectAdmin = true;
                    }
                    if (u.IsMicroServiceAdmin)
                    {
                        u.User.IsMicroServiceMethodAccessUser = true;
                    }
                    if(u.IsProjectSuperUser)
                    {
                        u.User.IsProjectSuperUser = true;
                        u.User.ProjectSuperUserInfo = u.ProjectSuperUserInfo;
                    }
                    
                    returnUsers.Add(u.User);
                }
            }

            return returnUsers;
        }
        public void UpdateUser(User user)
        {
            _unitOfWork.GetRepository<User>().Update(user, user.Id);
            _unitOfWork.Commit();
        }
        public User AddUser(User user)
        {
            _unitOfWork.GetRepository<User>().Create(user);
            _unitOfWork.Commit();
            return user;
        }
        public void GiveUserProjectMemberRole(User user, Guid projectGuid)
        {
            _unitOfWork.GetRepository<ProjectPermission>().Create(new ProjectPermission()
            {
                ProjectId = projectGuid,
                UserId = user.Id,
                RoleId = (int)RoleEnum.ProjectMember
            });
            _unitOfWork.Commit();
        }
        public void DeleteUser(int Id)
        {
            _unitOfWork.GetRepository<User>().Delete(Id);
            _unitOfWork.Commit();
        }
        public User GetUserWithPermissions(string domainName, Guid ProjectId)
        {
            User user = _unitOfWork.GetRepository<User>().GetAll().Where(u => u.DomainName == domainName).Include("Permissions").FirstOrDefault();
            if (user != null)
            {
                user.LoadPermissionBools(_unitOfWork.Context);
                user.LoadProjectPermissions(_unitOfWork.Context, ProjectId);
                return user;
            }
            return new User();
        }
        public User GetUserWithPermissions(string userDomain)
        {
            User user = _unitOfWork.GetRepository<User>().GetAll().Where(u => u.DomainName == userDomain).Include("Permissions").FirstOrDefault();
            if (user != null)
            {
                user.LoadPermissionBools(_unitOfWork.Context);
                return user;
            }
            return new User();
        }
        public User GetUserWithPermissions(int userID)
        {
            User user = _unitOfWork.GetRepository<User>().GetAll().Where(u => u.Id == userID).Include("Permissions").FirstOrDefault();
            user.LoadPermissionBools(_unitOfWork.Context);
            return user;
        }
        public User GetUser(int userID)
        {
            User user = _unitOfWork.GetRepository<User>().GetAll().Where(u => u.Id == userID).FirstOrDefault();
            return user;
        }
        #endregion

        #region [ Project ]

        public List<Project> ListProjects()
        {
            return _unitOfWork.GetRepository<Project>().GetAllNoTrack().Where(p=>p.IsSystemProject == false && p.IsDeleted != true).ToList();
        }
        public List<Project> ListProjectsUserIsNotAMemberOf(int userId)
        {
            List<Project> projects = this.ListProjects();
            return projects.Where(p => !p.UserIsMember(_unitOfWork.Context, userId)).ToList();
        }
        public List<Project> ListProjectsUserAdministersOnly(User user)
        {
            return _unitOfWork.GetRepository<Project>().GetAllNoTrack().Where(p => p.IsSystemProject == false && p.IsDeleted != true).ToList().Where(p => p.UserHasAdminRole(_unitOfWork.Context, user)).ToList();
        }
        public void AddProject(Project project)
        {
            project.ProjectUniqueKey = project.ProjectUniqueKey == null || project.ProjectUniqueKey == Guid.Empty ? Guid.NewGuid() : project.ProjectUniqueKey;
            _unitOfWork.GetRepository<Project>().Create(project);
            _unitOfWork.Commit();
        }

        public void DeleteProject(Guid Id)
        {
            Project project = _unitOfWork.GetRepository<Project>().GetAllNoTrack().Where(p => p.ProjectUniqueKey == Id).FirstOrDefault();
            project.IsActive = false;
            project.IsDeleted = true;            
            UpdateProject(project);
            //_unitOfWork.GetRepository<Project>().Delete(Id);
            //_unitOfWork.Commit();
        }

        public void UpdateProject(Project projectDto)
        {
            _unitOfWork.GetRepository<Project>().Update(projectDto, projectDto.ProjectUniqueKey);
            _unitOfWork.Commit();
        }

        #endregion

        public void Dispose()
        {
            base.Dispose();
        }
    }
}
