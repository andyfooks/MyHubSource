using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VirtualTrainer.Interfaces;
using VirtualTrainer;
using VirtualTrainer.DataAccess;
using AJG.VirtualTrainer.Services;
using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.DTOs;
using System.IO;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    public class BaseController : Controller
    {
        public IUnitOfWork Uow;
        public AdminService AdminService;
        public MyHubService MyHubService;
        public MicroServiceService MicroServiceService;
        public TimeSheetService TimeSheetService;

        public BaseController()
        {
            Uow = new UnitOfWork();
        }        

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            AdminService = new AdminService(Uow);
            MyHubService = new MyHubService(Uow);
            MicroServiceService = new MicroServiceService(Uow);
            TimeSheetService = new TimeSheetService(Uow);

            bool isSystemAdmin = User.IsInRole("SystemAdmin");

            ViewBag.Environment = AdminService.GetEnvironment();
            ViewBag.ExchangeRuleEnabled = AdminService.GetExchangeRuleEnabled();
            ViewBag.ExcelRuleEnabled = AdminService.GetExcelRuleEnabled();
            ViewBag.IsSystemAdmin = isSystemAdmin ? "true" : "false";
            ViewBag.IsSystemSuperUser = User.IsInRole("SystemSuperUser") ? "true" : "false";

            Guid projectId;
            var timeSheeUuser = Guid.TryParse(ConfigurationHelper.Get(AppSettingsList.TimeSheetProjectId), out projectId)
                ? AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name, projectId)
                : AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);            

            // If system admin or Time Sheet Project admin
            ViewBag.ShowTechTimeSheet = isSystemAdmin || timeSheeUuser.IsProjectAdmin || timeSheeUuser.IsProjectSuperUser ? 
                true : 
                TimeSheetService.GetTimeSheetUserSetting(AdminAction.TechTimeSheetAccess, AdminService, GetContextUserSamAccountName());            
        }
        public void Dispose()
        {
            Uow.Dispose();
            AdminService.Dispose();
            MyHubService.Dispose();
            MicroServiceService.Dispose();
        }
        internal string GetDownloadFilePath(string docName)
        {
            var directoryDetails = Directory.CreateDirectory(Path.Combine(System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location), "MyHubReportDownload"));
            return Path.Combine(directoryDetails.FullName, docName);
        }
        internal string GetContextUserSamAccountName()
        {
            return HttpContext.User.Identity.Name.ToLower().Split('\\')[1];            
        }
        internal string GetUserSamAccountName(string fullDomainName)
        {
            return fullDomainName.ToLower().Split('\\')[1];            
        }
        internal ADUserDTO GetContextUser()
        {
            return GetUser(GetContextUserSamAccountName());
        }
        internal ADUserDTO GetUser(string userSamAccountName)
        {            
            var helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
            var user = helper.GetUser(userSamAccountName);

            if (user == null)
            {
                user = AdminService.GetAdUserFromDB(userSamAccountName, true);
            }
            return user;
        }
    }
    public class MicroserviceBaseController : Controller
    {
        public IUnitOfWork Uow;
        public MicroServiceService MicroServiceService;

        public MicroserviceBaseController()
        {
            Uow = new UnitOfWork();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            MicroServiceService = new MicroServiceService(Uow);
            ViewBag.Environment = MicroServiceService.GetEnvironment();
            ViewBag.ExchangeRuleEnabled = MicroServiceService.GetExchangeRuleEnabled();
            ViewBag.ExcelRuleEnabled = MicroServiceService.GetExcelRuleEnabled();
        }
        public void Dispose()
        {
            Uow.Dispose();
            MicroServiceService.Dispose();
        }
    }
}