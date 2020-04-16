using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.General;
using AJG.VirtualTrainer.MVC.Attributes;
using AJG.VirtualTrainer.MVC.MyHub;
using AJG.VirtualTrainer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    [AuthorizeWith401RedirectAttribute(Roles = "SystemAdmin,SystemSuperUser,everyone")]
    public class MyHubController : BaseController
    {
        public ActionResult Index()
        {
            try {
                ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
            }
            catch (Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return View();
        }
        public ActionResult MyHub()
        {
            try
            {
                // TEMP - check if we want to import the AD data - This will also run on a schedule - left thisin incase there is some issue with schedule at some point.!!!
                if (this.AdminService.HasSystemConfigByGroupName(SystemConfig.ConfigKeys.ADHierachy.ToString()))
                {
                    if (!this.AdminService.ConfigUpdatedToday(SystemConfig.ConfigKeys.ADHierachy.ToString()))
                    {
                        SaveADStructureToDB();
                    }
                }
                else { SaveADStructureToDB(); }
                // TEMP

                ViewBag.User = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
                ADHelper helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
                ViewBag.UserName = GetContextUserSamAccountName();
                ViewBag.ADUser = helper.GetUser(ViewBag.UserName);

                // If dev, if not on AJG domain, see if we can get uer from DB.
                if (ViewBag.ADUser == null && string.Compare(ConfigurationHelper.Get(AppSettingsList.targetSystem), "Dev") == 0)
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
        [HttpPost]
        public JsonResult CreateChartDownloadFile(string ChartDataJson, string userName, bool addRowTotalColumn = true)
        {
            string message = string.Empty;
            bool error = false;
            string filePath = string.Empty;
            var fileName = string.Empty;
            try
            {                
                var cd = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateChartDownloadFileChartData>(ChartDataJson);           
                // Create the file 
                fileName = string.Format("ChartData_{0}_for_{1}_{2}.csv", cd.chartTitle.Replace(" ", ""), userName, DateTime.Now.ToString("yyy_MM_dd_HH_mm_ss"));
                filePath =  GetDownloadFilePath(fileName);

                // Add header Row
                List<string> headerRow = new List<string>();
                headerRow.Add("");
                headerRow.AddRange(cd.labels);
                if (addRowTotalColumn)
                {
                    headerRow.Add("Total");
                }
                System.IO.File.AppendAllLines(filePath, new List<string>() { string.Join(",", headerRow) });

                // Add data
                foreach (var d in cd.datasets)
                {
                    List<string> dataRow = new List<string>();
                    dataRow.Add(d.label);
                    dataRow.AddRange(d.data.Select(s => s.ToString()));
                    if (addRowTotalColumn)
                    {
                        dataRow.Add(d.data.Sum(s => s).ToString());
                    }
                    System.IO.File.AppendAllLines(filePath, new List<string>() { string.Join(",", dataRow) });
                    message = string.Format("Your Chart Download File is on the way.");
                }
            }
            catch (Exception ex)
            {
                message = string.Format("There has been an error Gettng the Chart Download File: {0}", ex.Message);
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { docName = fileName, error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        public ActionResult DownloadChartFile(string docName)
        {
            var filePath = string.Empty;
            try
            {
                filePath = GetDownloadFilePath(docName);
                if (System.IO.File.Exists(filePath))
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                    return File(bytes, "application/csv", fileName);
                }
            }
            catch (Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            return null;
        }
        public JsonResult GetValuesForReportDDs()
        {
            var rsystems = new List<ReportDDLItem>();
            try
            {                
                #region [ Printer ]

                #region [ Printer report ]             

                var scopes = new List<ReportDDLItem>()
            {
                new ReportDDLItem() { ItemId = (int)ReportScope.User, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.User) },
                new ReportDDLItem() { ItemId = (int)ReportScope.UserTeamLevel1, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.UserTeamLevel1) },
                new ReportDDLItem() { ItemId = (int)ReportScope.UserAllSubordinates,  ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.UserAllSubordinates) },
                new ReportDDLItem() { ItemId = (int)ReportScope.TeamMembers, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.TeamMembers) },
                new ReportDDLItem() { ItemId = (int)ReportScope.TeamMembersAndLevel1, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.TeamMembersAndLevel1) },
                new ReportDDLItem() { ItemId = (int)ReportScope.TeamMembersAndAllBelow, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.TeamMembersAndAllBelow) },
            };
                var printerReports = new List<ReportDDLItem>()
            {
                new ReportDDLItem() { ItemId =  (int)Report.PrinterPagesAndCostsTotals, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterPagesAndCostsTotals)  }  ,
                new ReportDDLItem() { ItemId =  (int)Report.PrinterCostsTotals, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterCostsTotals)  }
            };

                rsystems.Add(
                        new ReportDDLItem()
                        {
                            ItemId = (int)ReportSystem.Printers,
                            ItemText = GeneralHelper.GetEnumDescription((ReportSystem)ReportSystem.Printers),
                            CascadeItems = scopes.Select(a => new ReportDDLItem()
                            {
                                ItemId = a.ItemId,
                                ItemText = a.ItemText,
                                CascadeItems = printerReports
                            }).ToList()
                        }
                    );

                #endregion

                #region [ Printer Top 10 ]            

                scopes = new List<ReportDDLItem>()
            {
                new ReportDDLItem() { ItemId = (int)ReportScope.PrinterTop10, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.PrinterTop10) },
            };
                printerReports = new List<ReportDDLItem>()
            {
                new ReportDDLItem() { ItemId =  (int)Report.PrinterBandWCopies, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterBandWCopies)  }  ,
                new ReportDDLItem() { ItemId =  (int)Report.PrinterColourCopies, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterColourCopies)  } ,
                new ReportDDLItem() { ItemId =  (int)Report.PrinterTotalCopies, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterTotalCopies)  } ,
                new ReportDDLItem() { ItemId =  (int)Report.PrinterBandWCost, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterBandWCost)  } ,
                new ReportDDLItem() { ItemId =  (int)Report.PrinterColourCost, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterColourCost)  } ,
                new ReportDDLItem() { ItemId =  (int)Report.PrinterTotalCost, ItemText = GeneralHelper.GetEnumDescription((Report)Report.PrinterTotalCost)  } ,
            };
                rsystems[0].CascadeItems.AddRange(scopes.Select(a => new ReportDDLItem()
                {
                    ItemId = a.ItemId,
                    ItemText = a.ItemText,
                    CascadeItems = printerReports
                }).ToList());

                #endregion

                #endregion

                #region [ Mobile Reports ]

                scopes = new List<ReportDDLItem>()
            {
                new ReportDDLItem() { ItemId = (int)ReportScope.User, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.User) },
                new ReportDDLItem() { ItemId = (int)ReportScope.UserTeamLevel1, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.UserTeamLevel1) },
                new ReportDDLItem() { ItemId = (int)ReportScope.UserAllSubordinates,  ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.UserAllSubordinates) },
                new ReportDDLItem() { ItemId = (int)ReportScope.TeamMembers, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.TeamMembers) },
                new ReportDDLItem() { ItemId = (int)ReportScope.TeamMembersAndLevel1, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.TeamMembersAndLevel1) },
                new ReportDDLItem() { ItemId = (int)ReportScope.TeamMembersAndAllBelow, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.TeamMembersAndAllBelow) },
            };
                var mobileReports = new List<ReportDDLItem>()
            {
                new ReportDDLItem() { ItemId =  (int)Report.MobilePhonesCallsDataAndCosts, ItemText = GeneralHelper.GetEnumDescription((Report)Report.MobilePhonesCallsDataAndCosts)  }
            };

                scopes.Add(new ReportDDLItem() { ItemId = (int)ReportScope.Top10BiggestDataUsers, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.Top10BiggestDataUsers) });
                scopes.Add(new ReportDDLItem() { ItemId = (int)ReportScope.Top10BiggestSpenders, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.Top10BiggestSpenders) });
                scopes.Add(new ReportDDLItem() { ItemId = (int)ReportScope.OverAllowanceUsers, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.OverAllowanceUsers) });
                scopes.Add(new ReportDDLItem() { ItemId = (int)ReportScope.ZeroUseUsers, ItemText = GeneralHelper.GetEnumDescription((ReportScope)ReportScope.ZeroUseUsers) });
                rsystems.Add(
                    new ReportDDLItem()
                    {
                        ItemId = (int)ReportSystem.MobilePhones,
                        ItemText = GeneralHelper.GetEnumDescription((ReportSystem)ReportSystem.MobilePhones),
                        CascadeItems = scopes.Select(a => new ReportDDLItem()
                        {
                            ItemId = a.ItemId,
                            ItemText = a.ItemText,
                            CascadeItems = mobileReports
                        }).ToList()
                    }
                );

                #endregion                
            }
            catch(Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return Json(rsystems, JsonRequestBehavior.AllowGet);
        }        
        [HttpPost]
        public JsonResult GetChartData(string RequestedReportInfo, string monthFrom, string yearFrom, string monthTo, string yearTo, string userName)
        {
            string message = string.Empty;
            bool error = false;

            try
            {
                var specificUser = AdminService.GetAdUserFromDB(userName);

                if(specificUser == null)
                {
                    ADHelper helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
                    specificUser = helper.GetUser(userName);
                }

                if (specificUser != null)
                {
                    var requestReportInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestedReportInfo>(RequestedReportInfo);
                    switch (requestReportInfo.System)
                    {
                        case ReportSystem.Printers:
                            MyHubPrinterSummary printerSummary = new MyHubPrinterSummary(MyHubService, monthFrom, yearFrom, monthTo, yearTo);
                            var data = printerSummary.GetChartDataDto(requestReportInfo, specificUser);
                            if(data != null)
                            {
                                specificUser.Managees = null;
                                data.UserDTO = specificUser;
                            }
                            message = string.Format("Chart Data Successfully Retrieved.");
                            return new JsonResult { Data = new { data = data, error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
                        case ReportSystem.MobilePhones:
                            MyHubMobilePhoneSummary mobileSummary = new MyHubMobilePhoneSummary(MyHubService, monthFrom, yearFrom, monthTo, yearTo);
                            var mobiledata = mobileSummary.GetChartDataDto(requestReportInfo, specificUser);
                            if (mobiledata != null)
                            {
                                specificUser.Managees = null;
                                mobiledata.UserDTO = specificUser;
                            }
                            //specificUser.Managees = null;
                            //mobiledata.UserDTO = specificUser;
                            message = string.Format("Chart Data Successfully Retrieved.");
                            return new JsonResult { Data = new { data = mobiledata, error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("There has been an error Gettng the Chart Data: {0}", ex.Message);
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }

        #region [ AD Methods ]

        [AuthorizeWith401RedirectAttribute(Roles = "SystemAdmin")]
        public JsonResult SaveADStructureToDB(string adUserSamAccountName = "")
        {
            try
            {
                string ADIMportUsers = ConfigurationHelper.Get(AppSettingsList.ADImportSamAccountNames);

                if (!string.IsNullOrEmpty(ADIMportUsers))
                {
                    var adImportUsersList = ADIMportUsers.Split(',').ToList();
                    adImportUsersList.Add(adUserSamAccountName);

                    foreach (var aduserSAmAccountName in adImportUsersList)
                    {
                        if (!string.IsNullOrEmpty(aduserSAmAccountName))
                        {
                            ADHelper helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
                            ADUserDTO result = helper.GetUser(aduserSAmAccountName, true, true);
                            if (result != null && result.UserDetails != null)
                            {
                                var seralizedObject = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                                this.AdminService.SaveUpdateSystemConfig(SystemConfig.ConfigKeys.ADHierachy.ToString(), aduserSAmAccountName, seralizedObject);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        [OutputCache(Duration = 0)]
        public JsonResult GetUserUpperHierachy(string currentTopOfTreeUser, string UserToFind)
        {
            string message = string.Empty;
            bool error = false;
            List<ADUserDTO> MatchedUsers = new List<ADUserDTO>();
            var hierachyOfUsers = new List<string>();

            try
            {
                var routeADUser = AdminService.GetRoutADUserFromDB();
                var topOfTreeUser = routeADUser.GetSpecifcUserFromHierachy(currentTopOfTreeUser);                

                MatchedUsers = topOfTreeUser.FindMatchingHierachyUsers(UserToFind);

                if (MatchedUsers.Count() == 1)
                {
                    hierachyOfUsers = topOfTreeUser.GetSubordinateUpperHierachy(MatchedUsers[0].UserDetails.SamAccountName);
                }
                else
                {
                    message = string.Format("{0}", MatchedUsers.Count() == 0 ? "No Users Found. Please refine your search." : string.Format("{0} User(s) Found. Click a name to load.", MatchedUsers.Count()));
                }

                foreach (var mu in MatchedUsers)
                {
                    mu.Managees = null;
                } 
            }
            catch (Exception ex)
            {
                message = string.Format("There has been an error Searching for the user: {0}", ex.Message);
                error = true;
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return new JsonResult { Data = new { matchedCount = MatchedUsers.Count, hierachyOfUsers = hierachyOfUsers, MatchedUsers = MatchedUsers , error = error, message = message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
        }
        public JsonResult GetUsersForUserOVerrideDD()
        {
            List<ADUserDTO> data = new List<ADUserDTO>();
            try
            {
                var configUsers = new List<string>();
                configUsers.Add(GetContextUserSamAccountName());

                // Lets see if the user should have any overrides users?
                var userWithPerms = AdminService.GetUserWithPermissions(HttpContext.User.Identity.Name);
                var perm = userWithPerms.GetSuperUserPermission();
                if (perm != null)
                {
                    string overrideUsersFromConfig = (!string.IsNullOrEmpty(perm.Info)) ? perm.Info : ConfigurationHelper.Get(AppSettingsList.OverrideUsersSamAccountNames);
                    configUsers.AddRange(overrideUsersFromConfig.Split(',').ToList());
                }

                ADHelper helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));                
                foreach (var user in configUsers)
                {
                    var u = helper.GetUser(user, true);

                    // If dev, if not on AJG domain, see if we can get uer from DB.
                    if (u == null && string.Compare(ConfigurationHelper.Get(AppSettingsList.targetSystem), "Dev") == 0)
                    {
                        u = AdminService.GetAdUserFromDB(user, true);
                    }
                    if (u != null)
                    {
                        data.Add(u);
                    }
                }
            }
            catch(Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetADDetails(string UserName = "", bool GetUserManageesOnly = false, string uid = "")
        {
            var returnData = new ADUserDTO();
            try
            {
                // NOTE: TODO: We should only do this if the current user has perms to see details!!
                // Is this the user?
                // is this user the manager?
                // is this user configured as override?
                // Is this user project admin or super user?
                // Is this user System Admin?                
                string userIdentity = string.IsNullOrEmpty(UserName) || UserName == "undefined" ? GetContextUserSamAccountName() : UserName;

                // Try to find the user from the config DB
                returnData = AdminService.GetAdUserFromDB(userIdentity);
                if (returnData == null)
                {
                    ADHelper helper = new ADHelper(ConfigurationHelper.Get(AppSettingsList.DirectoryEntryPath));
                    var user = helper.GetUser(userIdentity);
                    return Json(new { data = user, uid = uid }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Set properties so Kendo tree expands first level and selects root node.
                    returnData.expanded = true;
                    returnData.selected = true;
                    returnData = returnData.GetUserAndManageesOnly();

                    returnData.Managees = returnData.Managees.OrderBy(o => o.UserDetails.SamAccountName).ToList();

                    if (GetUserManageesOnly)
                    {
                        return Json(new { data = returnData.Managees, uid = uid }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { data = returnData, uid = uid }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch(Exception ex)
            {
                AdminService.SaveSystemLog(new SystemLog(ex));
                return Json(null, JsonRequestBehavior.AllowGet);
            }            
        }

        #endregion

        #region [ Private and Other ]
        public class ReportDDLItem
        {
            public int ItemId { get; set; }
            public string ItemText { get; set; }
            public List<ReportDDLItem> CascadeItems { get; set; }
        }
        public class CreateChartDownloadFileChartData
        {
            public List<dataset> datasets { get; set; }
            public List<string> labels { get; set; }
            public string chartTitle { get; set; }
            
            public class dataset
            {
                public string label { get; set; }
                public List<decimal> data { get; set; }
            }
        }

        #endregion
    }
}