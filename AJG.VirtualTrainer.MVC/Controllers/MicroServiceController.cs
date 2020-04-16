using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.Excel;
using AJG.VirtualTrainer.Helper.FTP;
using AJG.VirtualTrainer.Helper.RazorEngine;
using AJG.VirtualTrainer.Helper.SharePointHelper;
using AJG.VirtualTrainer.Helper.ZipHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using VirtualTrainer;

namespace AJG.VirtualTrainer.MVC.Controllers
{
    [Authorize(Roles = "Micro Service")]
    [MicroServiceActionFilter]
    public class MicroServiceController : BaseController
    {
        #region [ App Controller Methods ]

        public JsonResult GetWebMethodData()
        {
            return Json("Not Yet Implmented", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [ public methods ]

        #region [ Application Specific ]

        #region [ TPS Applciation ]

        #region [ TPS Helper ]

        private class TPSProspectCleanser
        {
            #region [ properties ]

            string tpsFileFullPath = string.Empty;
            string ctpsFileFullPath = string.Empty;
            string prospectDocuemntFullPath = string.Empty;
            string sqlQuery = string.Empty;
            string jsonResultFileCompletePath = string.Empty;
            bool includeTPS = false;
            bool includeCTPS = false;
            public List<string> tpsNumbers = new List<string>();
            public List<string> ctpsNumbers = new List<string>();
            public List<ProspectInfo> prospectsInfo = new List<ProspectInfo>();
            public DateTime? tpsLastUpdated = null;
            public DateTime? ctpsLastUpdated = null;
            public string errorMessage = string.Empty;


            public string tpsFTPFileFullPath = string.Empty;
            public string ctpsFTPFileFullPath = string.Empty;
            public string FTPHost = string.Empty;
            public string FTPUsername = string.Empty;
            public string FTPPassword = string.Empty;
            public string prospectDocumentFullPath = string.Empty;
            public string spWebUrl = string.Empty;
            public string SPLibraryName = string.Empty;
            public int spItemID = 0;

            #endregion

            #region [ Constructors ]  

            public TPSProspectCleanser(string tpsFileFullPath, string ctpsFileFullPath, string prospectDocuemntFullPath,
                string sqlQuery, string jsonResultFileCompletePath, bool includeTPS = false, bool includeCTPS = false)
            {
                this.tpsFileFullPath = tpsFileFullPath;
                this.ctpsFileFullPath = ctpsFileFullPath;
                this.prospectDocuemntFullPath = prospectDocuemntFullPath;
                this.sqlQuery = sqlQuery;
                this.jsonResultFileCompletePath = jsonResultFileCompletePath;
                this.includeTPS = includeTPS;
                this.includeCTPS = includeCTPS;
            }
            public TPSProspectCleanser(string tpsFTPFileFullPath, string ctpsFTPFileFullPath, string FTPHost, string FTPUsername, string FTPPassword,
                string prospectDocumentFullPath, string spWebUrl, string SPLibraryName, int spItemID, bool includeTPS, bool includeCTPS)
            {
                this.tpsFTPFileFullPath = tpsFTPFileFullPath;
                this.ctpsFTPFileFullPath = ctpsFTPFileFullPath;
                this.FTPHost = FTPHost;
                this.FTPUsername = FTPUsername;
                this.FTPPassword = FTPPassword;
                this.prospectDocumentFullPath = prospectDocumentFullPath;
                this.spWebUrl = spWebUrl;
                this.SPLibraryName = SPLibraryName;
                this.spItemID = spItemID;
                this.includeTPS = includeTPS;
                this.includeCTPS = includeCTPS;
            }

            #endregion

            #region [ Public Methods ] 

            public bool ExecuteFullMonty()
            {
                string errorMessage = string.Empty;
                ServiceResponse r = new ServiceResponse();
                try
                {
                    // set up some paths.
                    string workingDirectory = Path.GetDirectoryName(prospectDocumentFullPath);
                    string TPSZipPath = Path.Combine(workingDirectory, "tps.zip");
                    string CTPSZipPath = Path.Combine(workingDirectory, "ctps.zip");
                    string TPSDataFilePath = Path.Combine(workingDirectory, "tps.txt");
                    string TPSDataFileNewPath = Path.Combine(workingDirectory, "tps.csv");
                    string CTPSDataFilePath = Path.Combine(workingDirectory, "ctps.txt");
                    string CTPSDataFileNewPath = Path.Combine(workingDirectory, "ctps.csv");

                    // Download the zip files.
                    FTPHelper helper = new FTPHelper(FTPHost, FTPUsername, FTPPassword);
                    if (this.includeTPS && helper.FTPFileExists(tpsFTPFileFullPath))
                    {
                        helper.DownloadDocument(tpsFTPFileFullPath, TPSZipPath);
                        // Extract the zip file.
                        ZipHelper.ExtractAll(TPSZipPath, workingDirectory);
                        // Change the name of the file to csv
                        //System.IO.File.Move(TPSDataFilePath, TPSDataFileNewPath);
                    }
                    if (this.includeCTPS && helper.FTPFileExists(ctpsFTPFileFullPath))
                    {
                        helper.DownloadDocument(ctpsFTPFileFullPath, CTPSZipPath);
                        // Extract the zip file.
                        ZipHelper.ExtractAll(CTPSZipPath, workingDirectory);
                        // Change the name of the file to csv
                        //System.IO.File.Move(CTPSDataFilePath, CTPSDataFileNewPath);
                    }

                    string SqlQuery = "select [Name], [Phone] from [prospects$]";

                    // Read in the prospect excel doc.
                    if (!ReadInProspectData(prospectDocumentFullPath, SqlQuery, out errorMessage))
                    {
                        throw new Exception(string.Format("There has been an issue reading in the Prospect document. {0}.", errorMessage));
                    }

                    if (includeTPS)
                    {
                        if (!GetFileLastUpdated(TPSDataFilePath, out tpsLastUpdated, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue getting last updated date from TPS file. {0}.", errorMessage));
                        }
                        if (!CheckProspectsAgainstTPS(TPSDataFilePath, this.tpsNumbers, true, false, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue checking prospects against TPS numbers. {0}.", errorMessage));
                        }
                    }

                    // Set which numbers can be called.
                    if (includeCTPS)
                    {
                        if (!GetFileLastUpdated(CTPSDataFilePath, out ctpsLastUpdated, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue getting last updated date from CTPS file. {0}.", errorMessage));
                        }
                        if (!CheckProspectsAgainstTPS(CTPSDataFilePath, this.ctpsNumbers, false, true, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue checking prospects against CTPS numbers. {0}.", errorMessage));
                        }
                    }
                    using (var prospectDoc = new ExcelHelper(prospectDocumentFullPath, true))
                    {
                        // Update the prospects file
                        foreach (var prospect in this.prospectsInfo)
                        {
                            string updateCommand = string.Format("Update [prospects$] Set [TPS cleansing result - can be contacted] = '{0}' where [Phone] = '{1}'", prospect.CanContact ? "Yes" : "No", prospect.Number);
                            prospectDoc.ExecuteCommandNoResult(updateCommand);
                        }
                    }

                    r.Success = true;
                }
                catch (Exception ex)
                {
                    r.Success = false;
                    r.ErrorMessage = ex.Message;
                }
                try
                {
                    // Update the SP list with info.
                    Dictionary<string, object> spItemProperties = new Dictionary<string, object>();
                    spItemProperties.Add("MicroServiceCleansingCallSucceeded", r.Success ? "Yes" : "No");
                    spItemProperties.Add("MicroServiceCleansingCallFailureErrorMessage", string.IsNullOrEmpty(r.ErrorMessage) ? "" : r.ErrorMessage);

                    SharePointHelper spHelper = new SharePointHelper(spWebUrl);
                    spHelper.UpdateSPItemById(SPLibraryName, spItemID, spItemProperties, out errorMessage);
                }
                catch (Exception ex)
                {
                    SystemLog errorLog = new SystemLog(new Exception("Some Exception"));
                    //System.IO.File.AppendAllText(@"\\development01\devteam$\Andy Fooks\TPSCleansing\Test\GenerateJsonResultFromProspectCleansingAgainstTPSData_Async_Error.txt", string.Format("After: {0}, {1}, {2}, Error: {3}.", spWebUrl, SPLibraryName, spItemID, ex.Message));
                }
                return r.Success;
            }

            public bool Execute(string spWebUrl, string SPLibraryName, int spItemID)
            {
                string errorMessage = string.Empty;
                ServiceResponse r = new ServiceResponse();
                try
                {
                    if (!includeTPS && !includeCTPS)
                    {
                        throw new Exception("At least one tps file has to be 'included = true' for the process to execute.");
                    }

                    string oledbConnectionString = ConfigurationHelper.Get(AppSettingsList.oledbConnectionString);

                    // Read in the prospect excel doc.
                    if (!ReadInProspectData(prospectDocuemntFullPath, sqlQuery, out errorMessage))
                    {
                        throw new Exception(string.Format("There has been an issue reading in the Prospect document. {0}.", errorMessage));
                    }

                    if (includeTPS)
                    {
                        if (!GetFileLastUpdated(tpsFileFullPath, out tpsLastUpdated, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue getting last updated date from TPS file. {0}.", errorMessage));
                        }
                        if (!CheckProspectsAgainstTPS(this.tpsFileFullPath, this.tpsNumbers, true, false, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue checking prospects against TPS numbers. {0}.", errorMessage));
                        }
                    }

                    // Set which numbers can be called.
                    if (includeCTPS)
                    {
                        if (!GetFileLastUpdated(ctpsFileFullPath, out ctpsLastUpdated, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue getting last updated date from CTPS file. {0}.", errorMessage));
                        }
                        if (!CheckProspectsAgainstTPS(this.ctpsFileFullPath, this.ctpsNumbers, false, true, out errorMessage))
                        {
                            throw new Exception(string.Format("There has been an issue checking prospects against CTPS numbers. {0}.", errorMessage));
                        }
                    }

                    // Now save the prospects results as json to file.
                    System.IO.File.WriteAllText(this.jsonResultFileCompletePath, JsonConvert.SerializeObject(this.prospectsInfo));

                    r.Success = true;
                }
                catch (Exception ex)
                {
                    r.Success = false;
                    r.ErrorMessage = ex.Message;
                }
                try
                {
                    // Update the SP list with info.
                    Dictionary<string, object> spItemProperties = new Dictionary<string, object>();
                    spItemProperties.Add("MicroServiceCleansingCallSucceeded", r.Success ? "Yes" : "No");
                    spItemProperties.Add("MicroServiceCleansingCallFailureErrorMessage", string.IsNullOrEmpty(r.ErrorMessage) ? "" : r.ErrorMessage);

                    SharePointHelper spHelper = new SharePointHelper(spWebUrl);
                    spHelper.UpdateSPItemById(SPLibraryName, spItemID, spItemProperties, out errorMessage);
                }
                catch (Exception ex)
                {
                    //System.IO.File.AppendAllText(@"\\development01\devteam$\Andy Fooks\TPSCleansing\Test\GenerateJsonResultFromProspectCleansingAgainstTPSData_Async_Error.txt", string.Format("After: {0}, {1}, {2}, Error: {3}.", spWebUrl, SPLibraryName, spItemID, ex.Message));
                }
                return r.Success;
            }

            #endregion

            #region [ Private Methods ]

            private bool GetFileLastUpdated(string filePath, out DateTime? lastUpdated, out string errorMessage)
            {
                errorMessage = string.Empty;
                lastUpdated = null;
                if (System.IO.File.Exists(filePath))
                {
                    FileInfo info = new FileInfo(filePath);
                    lastUpdated = info.LastWriteTime;
                    return true;
                }
                else
                {
                    errorMessage = string.Format("File could not be found...", filePath);
                }
                return false;
            }

            private bool ReadInProspectData(string fileFullPath, string sqlQuery, out string errorMessage)
            {
                errorMessage = string.Empty;
                try
                {
                    if (System.IO.File.Exists(fileFullPath))
                    {
                        FileInfo fileInfo = new FileInfo(fileFullPath);
                        string prospectFileName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
                        using (ExcelHelper eHelper = new ExcelHelper(fileFullPath, true))
                        {
                            eHelper.GetResultsFromDocument(sqlQuery);
                            TransformProspectData(eHelper, fileFullPath, prospectFileName);
                        }

                        return true;
                    }
                    else
                    {
                        errorMessage = string.Format("File could not be found...", fileFullPath);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format("An Error has occured reading in prospect data. Path: {0}, Error: {1}", fileFullPath, ex.Message);
                }
                return false;
            }
            private bool TransformProspectData(ExcelHelper eHelper, string prospectDocumentFullPath, string prospectFileName)
            {
                DateTime readTime = DateTime.Now;
                foreach (ResultRow row in eHelper.Results)
                {
                    ProspectInfo pi = new ProspectInfo();
                    pi.MatchedAgainstCTPS = false;
                    pi.MatchedAgainstTPS = false;
                    pi.CanContact = true;
                    pi.ProspectFileFullPath = prospectDocumentFullPath;
                    pi.ProspectFileName = prospectFileName;
                    pi.ReadTime = readTime;

                    foreach (var column in row.Columns)
                    {
                        if (column.Key == "Name")
                        {
                            pi.Name = column.Value;
                        }
                        if (column.Key == "Phone")
                        {
                            pi.Number = column.Value;
                        }
                    }
                    this.prospectsInfo.Add(pi);
                }
                return false;
            }
            private bool CheckProspectsAgainstTPS(string filePath, List<string> tpsNumbers, bool isTPS, bool isCTPS, out string errorMessage)
            {
                errorMessage = string.Empty;
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        int counter = 1;
                        List<string> tpsNumbersBatch = new List<string>();
                        int tpsLineCount = System.IO.File.ReadLines(filePath).Count();

                        foreach (string line in System.IO.File.ReadLines(filePath))
                        {
                            tpsNumbersBatch.Add(line.Replace(" ", ""));
                            counter++;
                            
                            // Process tps numbers in batches of 1 million.
                            if (((counter % 1000000) == 0) || counter == tpsLineCount)
                            {
                                this.prospectsInfo = this.prospectsInfo
                                    .Select(s =>
                                    {
                                        // The number in the excel spreadsheet could have a 0 missing from the front or have . or +44 at the start, so lets do some basic processing in the number.
                                        string number = s.Number.Replace(".", "").Replace(" ", "").Replace("+44", "0").Trim();
                                        number = number.StartsWith("0") ? number : string.Format("0{0}", number);

                                        // if can contact already false then must have been excluded previously so keep it false.
                                        s.CanContact = s.CanContact == false ? s.CanContact : !tpsNumbersBatch.Contains(number);
                                        s.MatchedAgainstCTPS = s.MatchedAgainstCTPS == true ? s.MatchedAgainstCTPS : isCTPS;
                                        s.MatchedAgainstTPS = s.MatchedAgainstTPS == true ? s.MatchedAgainstTPS : isTPS;
                                        s.CTPSLastUpdatedDateTime = this.ctpsLastUpdated;
                                        s.TPSLastUpdatedDateTime = this.tpsLastUpdated;
                                        return s;
                                    }).ToList();
                                tpsNumbersBatch = new List<string>();
                            }
                        }

                        return true;
                    }
                    else
                    {
                        errorMessage = string.Format("Error (C)TPS file could not be found at: {0}", filePath);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = string.Format("Error Checking Prosepct numbers against (C)TPS file: {0}", ex.Message);
                }
                return false;
            }

            #endregion

            #region [ Internal Classes ]

            internal class ProspectInfo
            {
                public string Name { get; set; }
                public string Number { get; set; }
                public bool CanContact { get; set; }
                public bool MatchedAgainstTPS = false;
                public bool MatchedAgainstCTPS = false;
                public DateTime? TPSLastUpdatedDateTime { get; set; }
                public DateTime? CTPSLastUpdatedDateTime { get; set; }
                public string ProspectFileName { get; set; }
                public string ProspectFileFullPath { get; set; }
                public DateTime ReadTime { get; set; }
            }

            #endregion
        }

        #endregion

        public JsonResult TPSProcessProspectList(string tpsFTPFileFullPath, string ctpsFTPFileFullPath, string FTPHost, string FTPUsername, string FTPPassword,
            string prospectDocumentFullPath, string spWebUrl, string SPLibraryName, int spItemID, bool includeTPS = false, bool includeCTPS = false)
        {
            // Need to add site url, library name, item id
            ServiceResponse r = new ServiceResponse();

            try
            {
                // Do some validation here so we can return an error message asap before the async work.
                if (!includeTPS && !includeCTPS) { throw new Exception("At least one tps file has to be 'included = true' for the process to execute."); }
                if (includeTPS && string.IsNullOrEmpty(tpsFTPFileFullPath)) { throw new Exception(string.Format("tpsFTPFileFullPath property is requried: {0}.", tpsFTPFileFullPath)); }
                if (includeCTPS && string.IsNullOrEmpty(ctpsFTPFileFullPath)) { throw new Exception(string.Format("ctpsFTPFileFullPath property is requried: {0}.", ctpsFTPFileFullPath)); }
                if (string.IsNullOrEmpty(FTPHost)) { throw new Exception(string.Format("FTPHost property is requried: {0}.", FTPHost)); }
                if (string.IsNullOrEmpty(FTPUsername)) { throw new Exception(string.Format("FTPUsername property is requried: {0}.", FTPUsername)); }
                if (string.IsNullOrEmpty(prospectDocumentFullPath)) { throw new Exception(string.Format("prospectDocumentFullPath property is requried: {0}.", prospectDocumentFullPath)); }
                if (string.IsNullOrEmpty(spWebUrl)) { throw new Exception(string.Format("spWebUrl property is requried: {0}.", spWebUrl)); }
                if (string.IsNullOrEmpty(SPLibraryName)) { throw new Exception(string.Format("SPLibraryName property is requried: {0}.", SPLibraryName)); }

                if (!System.IO.File.Exists(prospectDocumentFullPath)) { throw new Exception(string.Format("The specified Prospect docuemnt could not be found: {0}.", prospectDocumentFullPath)); }

                FTPHelper helper = new FTPHelper(FTPHost, FTPUsername, FTPPassword);
                if (includeTPS && !helper.FTPFileExists(tpsFTPFileFullPath))
                {
                    throw new Exception(string.Format("The TPS File specified could not be found: {0}.", tpsFTPFileFullPath));
                }
                if (includeCTPS && !helper.FTPFileExists(ctpsFTPFileFullPath))
                {
                    throw new Exception(string.Format("The CTPS File specified could not be found: {0}.", ctpsFTPFileFullPath));
                }

                // We dont want to wait for this action to complete. This task takes a while and we dont want the caller to time out.
                TPSProcessProspectList_Async(tpsFTPFileFullPath, ctpsFTPFileFullPath, FTPHost, FTPUsername, FTPPassword, prospectDocumentFullPath, spWebUrl, SPLibraryName, spItemID, includeTPS, includeCTPS);

                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }
        private async Task TPSProcessProspectList_Async(string tpsFTPFileFullPath, string ctpsFTPFileFullPath, string FTPHost, string FTPUsername, string FTPPassword,
            string prospectDocumentFullPath, string spWebUrl, string SPLibraryName, int spItemID, bool includeTPS, bool includeCTPS)
        {
            // Need to add site url, library name, item id
            ServiceResponse r = new ServiceResponse();
            string errorMessage = string.Empty;
            try
            {
                TPSProspectCleanser prospectCleanser = new TPSProspectCleanser(tpsFTPFileFullPath, ctpsFTPFileFullPath, FTPHost, FTPUsername, FTPPassword, prospectDocumentFullPath, spWebUrl, SPLibraryName, spItemID, includeTPS, includeCTPS);
                bool a = await Task.Run(() => prospectCleanser.ExecuteFullMonty());

                r.Success = true;
            }
            catch (Exception ex)
            {
                r.Success = false;
                r.ErrorMessage = ex.Message;
            }
        }
        private async Task GenerateJsonResultFromProspectCleansingAgainstTPSData_Async(string tpsFileFullPath, string ctpsFileFullPath, string prospectDocuemntFullPath,
            string sqlQuery, string jsonResultFileCompletePath, string spWebUrl, string SPLibraryName, int spItemID, bool includeTPS, bool includeCTPS)
        {
            // Need to add site url, library name, item id
            ServiceResponse r = new ServiceResponse();
            string errorMessage = string.Empty;
            try
            {
                TPSProspectCleanser prospectCleanser = new TPSProspectCleanser(tpsFileFullPath, ctpsFileFullPath, prospectDocuemntFullPath, sqlQuery, jsonResultFileCompletePath, includeTPS, includeCTPS);
                bool a = await Task.Run(() => prospectCleanser.Execute(spWebUrl, SPLibraryName, spItemID));

                r.Success = true;
            }
            catch (Exception ex)
            {
                r.Success = false;
                r.ErrorMessage = ex.Message;
            }
        }


        // Output json file from tps data and excel doc
        [Display(Name = "GenerateJsonResultFromProspectCleansingAgainstTPSData", Description = "")]
        public JsonResult GenerateJsonResultFromProspectCleansingAgainstTPSData(string tpsFileFullPath,
            string ctpsFileFullPath, string prospectDocuemntFullPath, string sqlQuery, string jsonResultFileCompletePath, string spWebUrl, string SPLibraryName, int spItemID,
            bool includeTPS = false, bool includeCTPS = false)
        {
            // Need to add site url, library name, item id
            ServiceResponse r = new ServiceResponse();

            try
            {
                // Do some validation here so we can return an error message asap before the async work.
                if (!includeTPS && !includeCTPS)
                {
                    throw new Exception("At least one tps file has to be 'included = true' for the process to execute.");
                }
                // Check TPS file path exists.
                if (includeTPS && !System.IO.File.Exists(tpsFileFullPath))
                {
                    throw new Exception(string.Format("The TPS FIle could not be found at: {0}.", tpsFileFullPath));
                }
                // Check CTPS file exists.
                if (includeCTPS && !System.IO.File.Exists(ctpsFileFullPath))
                {
                    throw new Exception(string.Format("The CTPS FIle could not be found at: {0}.", ctpsFileFullPath));
                }
                if (!System.IO.File.Exists(prospectDocuemntFullPath))
                {
                    throw new Exception(string.Format("The specified Prospect docuemnt could not be found: {0}.", prospectDocuemntFullPath));
                }

                // We dont want to wait for this action to complete. This task takes a while and we dont want the caller to time out.
                GenerateJsonResultFromProspectCleansingAgainstTPSData_Async(tpsFileFullPath, ctpsFileFullPath, prospectDocuemntFullPath, sqlQuery, jsonResultFileCompletePath, spWebUrl, SPLibraryName, spItemID, includeTPS, includeCTPS);

                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion

        #region [ MPS Methods ]

        [Display(Name = "MPSProcessMailingList", Description = "")]
        public JsonResult MPSProcessMailingList(string mpsFileFullPath, string FTPHost, string FTPUsername, string FTPPassword, string prospectDocumentFullPath, string spWebUrl, string SPLibraryName, int spItemID)
        {
            // Need to add site url, library name, item id
            ServiceResponse r = new ServiceResponse();

            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(mpsFileFullPath)) { throw new Exception(string.Format("mpsFileFullPath property is requried: {0}.", mpsFileFullPath)); }
                if (string.IsNullOrEmpty(FTPHost)) { throw new Exception(string.Format("FTPHost property is requried: {0}.", FTPHost)); }
                if (string.IsNullOrEmpty(FTPUsername)) { throw new Exception(string.Format("FTPUsername property is requried: {0}.", FTPUsername)); }
                if (string.IsNullOrEmpty(prospectDocumentFullPath)) { throw new Exception(string.Format("prospectDocumentFullPath property is requried: {0}.", prospectDocumentFullPath)); }
                if (string.IsNullOrEmpty(spWebUrl)) { throw new Exception(string.Format("spWebUrl property is requried: {0}.", spWebUrl)); }
                if (string.IsNullOrEmpty(SPLibraryName)) { throw new Exception(string.Format("SPLibraryName property is requried: {0}.", SPLibraryName)); }

                if (!System.IO.File.Exists(prospectDocumentFullPath)) { throw new Exception(string.Format("The specified Prospect docuemnt could not be found: {0}.", prospectDocumentFullPath)); }

                FTPHelper helper = new FTPHelper(FTPHost, FTPUsername, FTPPassword);
                if (!helper.FTPFileExists(mpsFileFullPath))
                {
                    throw new Exception(string.Format("The MPS File specified could not be found: {0}.", mpsFileFullPath));
                }

                // We dont want to wait for this action to complete. This task takes a while and we dont want the caller to time out.
                ProcessMPSCleansingRequest_Async(mpsFileFullPath, FTPHost, FTPUsername, FTPPassword, prospectDocumentFullPath, spWebUrl, SPLibraryName, spItemID);

                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task ProcessMPSCleansingRequest_Async(string mpsFileFullPath, string FTPHost, string FTPUsername, string FTPPassword, string prospectDocumentFullPath, string spWebUrl, string SPLibraryName, int spItemID)
        {
            // Need to add site url, library name, item id
            ServiceResponse r = new ServiceResponse();
            string errorMessage = string.Empty;
            try
            {
                MPSProspectCleanser prospectCleanser = new MPSProspectCleanser(mpsFileFullPath, FTPHost, FTPUsername, FTPPassword, prospectDocumentFullPath, spWebUrl, SPLibraryName, spItemID);
                bool a = await Task.Run(() => prospectCleanser.Execute(spWebUrl, SPLibraryName, spItemID));

                r.Success = true;
            }
            catch (Exception ex)
            {
                r.Success = false;
                r.ErrorMessage = ex.Message;
            }
        }

        private class MPSProspectCleanser
        {
            private string mpsFileFullPath;
            private string FTPHost;
            private string FTPUsername;
            private string FTPPassword;
            private string prospectDocumentFullPath;
            private string spWebUrl;
            private string SPLibraryName;
            private int spItemID;

            public MPSProspectCleanser(string mpsFileFullPath, string FTPHost, string FTPUsername, string FTPPassword, string prospectDocumentFullPath, string spWebUrl, string SPLibraryName, int spItemID)
            {
                this.mpsFileFullPath = mpsFileFullPath;
                this.FTPHost = FTPHost;
                this.FTPUsername = FTPUsername;
                this.FTPPassword = FTPPassword;
                this.prospectDocumentFullPath = prospectDocumentFullPath;
                this.spWebUrl = spWebUrl;
                this.SPLibraryName = SPLibraryName;
                this.spItemID = spItemID;
            }
            public bool Execute(string spWebUrl, string SPLibraryName, int spItemID)
            {
                string errorMessage = string.Empty;
                ServiceResponse sr = new ServiceResponse();
                try
                {
                    // set up some paths.
                    string workingDirectory = Path.GetDirectoryName(prospectDocumentFullPath);
                    string MPSZipPath = Path.Combine(workingDirectory, "mps_personal_csv.zip");
                    string MPSDataFilePath = Path.Combine(workingDirectory, "mps_personal_csv.dat");
                    string MPSDataFileNewPath = Path.Combine(workingDirectory, "mps_personal_csv.csv");

                    // Download the MPS csv File.
                    FTPHelper helper = new FTPHelper(FTPHost, FTPUsername, FTPPassword);
                    if (helper.FTPFileExists(mpsFileFullPath))
                    {
                        helper.DownloadDocument(mpsFileFullPath, MPSZipPath);
                    }

                    // Extract the zip file.
                    ZipHelper.ExtractAll(MPSZipPath, workingDirectory);

                    // Change the name of the file to csv
                    System.IO.File.Move(MPSDataFilePath, MPSDataFileNewPath);

                    using (ExcelHelper MPSDocument = new ExcelHelper(MPSDataFileNewPath, false))
                    using (ExcelHelper mpsBusinessData = new ExcelHelper(prospectDocumentFullPath, true))
                    {
                        // Get everthing from the mps csv file.
                        var query = "SELECT F14 as [Postcode], F6 as [First Name], F7 as [Last Name] FROM [" + Path.GetFileName(MPSDataFileNewPath) + "]";
                        MPSDocument.GetResultsFromDocument(query);

                        // Want to ensure Postcode  has no spaces in mps data
                        var mpsDocumentResults = MPSDocument.Results.Select(c => { c.Columns[0] = new KeyValuePair<string, string>(c.Columns[0].Key, c.Columns[0].Value.Replace(" ", "")); return c; }).ToList();

                        // Read in mps business data.
                        mpsBusinessData.GetResultsFromDocument("Select * from [MPSSheet$]");
                        foreach (var contact in mpsBusinessData.Results)
                        {
                            bool any = mpsDocumentResults.Where(r => r.Columns.Where(q => q.Key == "First Name" && q.Value == contact.Columns[0].Value.ToUpper()).Any() && r.Columns.Where(q => q.Key == "Last Name" && q.Value == contact.Columns[1].Value.ToUpper()).Any() && r.Columns.Where(q => q.Key == "Postcode" && q.Value == contact.Columns[2].Value.Replace(" ", "").ToUpper()).Any()).Any();
                            string updateCommand = string.Format("Update [MPSSheet$] Set [MPS Result - Can be Contacted] = '{0}' where [First Name] = \"{1}\" and [Last Name] = \"{2}\" and [Postcode] = \"{3}\"", any ? "No" : "Yes", contact.Columns[0].Value, contact.Columns[1].Value, contact.Columns[2].Value);
                            mpsBusinessData.ExecuteCommandNoResult(updateCommand);
                        }
                    }

                    sr.Success = true;
                }
                catch (Exception ex)
                {
                    sr.Success = false;
                    sr.ErrorMessage = ex.Message;
                }

                try
                {
                    // Update the SP list with info.
                    Dictionary<string, object> spItemProperties = new Dictionary<string, object>();
                    spItemProperties.Add("MicroServiceCleansingCallSucceeded", sr.Success ? "Yes" : "No");
                    spItemProperties.Add("MicroServiceCleansingCallFailureErrorMessage", string.IsNullOrEmpty(sr.ErrorMessage) ? "" : sr.ErrorMessage);

                    SharePointHelper spHelper = new SharePointHelper(spWebUrl);
                    spHelper.UpdateSPItemById(SPLibraryName, spItemID, spItemProperties, out errorMessage);
                }
                catch (Exception ex)
                {
                    sr.Success = false;
                    sr.ErrorMessage = ex.Message;
                }
                return sr.Success;
            }
        }

        #endregion

        #region [ Miscilenious ]

        public ActionResult ConvertJsonToXML(string xmlRootName, string json)
        {
            XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(json, string.IsNullOrEmpty(xmlRootName) ? "Root" : xmlRootName);

            return Content(xmlDoc.DocumentElement.OuterXml, "text/xml");
        }

        #endregion

        #region [ Shared Drive ]

        public JsonResult SharedDrive_MoveFile(string sourceFileName, string destFileName)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                // Ensure paths are not null.
                if (string.IsNullOrEmpty(sourceFileName) || string.IsNullOrEmpty(destFileName)) { throw new FormatException(string.Format("a value is required for both fileFullPath {0} and fileNewPath {1}.", sourceFileName, destFileName)); }

                // Must be a shared drive
                if (!sourceFileName.StartsWith(@"\\")) { throw new FormatException("The path must be a shared drive path beginning with"); }

                // Check file exists
                if (!System.IO.File.Exists(sourceFileName)) { throw new FormatException(string.Format("The file does not exist at: {0} \\", sourceFileName)); }

                // move the file. if is the same location, it will just do a rename.
                System.IO.File.Move(sourceFileName, destFileName);

                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                MicroServiceActionTakenLog log = new MicroServiceActionTakenLog() { ErrorMessage = ex.Message, ActionName = "SharedDrive_MoveFile", ActionParameters = string.Format("sourceFileName:{0}, destFileName {1}", sourceFileName, destFileName) };
                this.MicroServiceService.AddMicroServiceActionLog(log);

                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SharedDrive_FileExists(string fileFullPath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                // Must be a shared drive
                if (fileFullPath.StartsWith(@"\\"))
                {
                    r.JsonResult = System.IO.File.Exists(fileFullPath).ToString();
                }
                else
                {
                    throw new FormatException("The path must be a shared drive path beginning with \\");
                }
                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteSharedDriveLeafDirectory(string directoryPath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                // Must be a shared drive
                if (directoryPath.StartsWith(@"\\"))
                {
                    // Must be a leaf directory
                    if (Directory.GetDirectories(directoryPath).Count() == 0)
                    {
                        Directory.Delete(directoryPath, true);
                    }
                    else
                    {
                        throw new FormatException("Only a leaf directory can be deleted.");
                    }
                }
                else
                {
                    throw new FormatException("The path must be a shared drive path beginning with \\");
                }
                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult EnsureSharedPathExists(string fullSharedPath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                if (fullSharedPath.StartsWith(@"\\"))
                {
                    System.IO.Directory.CreateDirectory(fullSharedPath);
                }
                else
                {
                    throw new FormatException("The path must be a shared drive path beginning with \\");
                }
                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ SharePoint ]

        public JsonResult SaveDocumentToSharePoint(string spWebUrl, string uploadFileFullPath, string spDocLibraryName, string spItemPropertiesNameValueDictionaryJson)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                if (!System.IO.File.Exists(uploadFileFullPath))
                {
                    throw new Exception(string.Format("UploadFile Doc could not be found: {0}.", uploadFileFullPath));
                }

                string fileName = new System.IO.FileInfo(uploadFileFullPath).Name;
                string errorMessage = string.Empty;
                string spFileRelativePath = string.Empty;

                Dictionary<string, object> spItemProperties = string.IsNullOrEmpty(spItemPropertiesNameValueDictionaryJson) ? new Dictionary<string, object>() : JsonConvert.DeserializeObject<Dictionary<string, object>>(spItemPropertiesNameValueDictionaryJson);

                SharePointHelper spHelper = new SharePointHelper(spWebUrl);
                if (!spHelper.SaveDocument(uploadFileFullPath, spDocLibraryName, fileName, spItemProperties, out spFileRelativePath, out errorMessage))
                {
                    throw new Exception(string.Format("Saving the document to Sharepoint failed: {0}. Error Message: {1}.", spFileRelativePath, errorMessage));
                }

                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ PDF From HTML ]

        public JsonResult SaveHtmlToPDFFile(string pdfcontent, string filePath,
            int? pageHeight, int? pageWidth,
            int? marginTop, int? marginBottom, int? marginLeft, int? marginRigh)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                AsposeHelper helper = new AsposeHelper();
                helper.SaveHtmlToPDFFile(pdfcontent, filePath, pageHeight, pageWidth, marginTop, marginBottom, marginLeft, marginRigh);
                r.Success = true;
                r.JsonResult = string.Empty;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ Razor to HTML ]

        public JsonResult GenerateHTMLFromRazorTemplate(string razorTemplateFullPath, string htmlFileSaveLocation, string jsonModelFilePath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                string jsonModelFromFile = string.Empty;
                string htmlFIleContent = string.Empty;
                string razorTemplateFileName = string.Empty;

                // Read in the json Model file content
                if (System.IO.File.Exists(jsonModelFilePath))
                {
                    jsonModelFromFile = System.IO.File.ReadAllText(jsonModelFilePath);
                }
                else
                {
                    throw new Exception(string.Format("Json Model File not found at: {0}.", jsonModelFilePath));
                }

                // Read in the json Model file content
                if (System.IO.File.Exists(razorTemplateFullPath))
                {
                    razorTemplateFileName = new FileInfo(razorTemplateFullPath).Name;
                }
                else
                {
                    throw new Exception(string.Format("Json Model File not found at: {0}.", jsonModelFilePath));
                }

                // Validate html save path
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(htmlFileSaveLocation)))
                {
                    throw new Exception(string.Format("htmlFileSaveLocation directory does not exist: {0}.", System.IO.Path.GetDirectoryName(htmlFileSaveLocation)));
                }

                // Call Razor engine
                RazorEngineHelper helper = new RazorEngineHelper();
                if (helper.GetContentFromTemplate(razorTemplateFileName, razorTemplateFullPath, jsonModelFromFile, true, out htmlFIleContent))
                {
                    System.IO.File.WriteAllText(htmlFileSaveLocation, htmlFIleContent);
                }
                else
                {
                    throw new Exception(string.Format("Creating the html from the razor model failed."));
                }

                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ Zip ]

        public JsonResult Zip_ExtractAll(string zipPath, string extractDirectoryPath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                ZipHelper.ExtractAll(zipPath, extractDirectoryPath);
                r.Success = true;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ FTP ]

        [AllowAnonymous]
        public JsonResult FTP_FileExists(string host, string username, string password, string downloadFilePath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                FTPHelper helper = new FTPHelper(host, username, password);
                bool returnValue = helper.FTPFileExists(downloadFilePath);
                r.Success = returnValue;
                r.JsonResult = returnValue.ToString();
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FTP_DownloadFile(string host, string username, string password, string downloadFilePath, string downloadFileDestinationPath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                FTPHelper helper = new FTPHelper(host, username, password);
                bool returnValue = helper.DownloadDocument(downloadFilePath, downloadFileDestinationPath);
                r.Success = returnValue;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FTP_GetFileLastWriteTime(string host, string username, string password, string downloadFilePath)
        {
            ServiceResponse r = new ServiceResponse();
            try
            {
                FTPHelper helper = new FTPHelper(host, username, password);
                string returnMessage = helper.GetLastWriteTime(downloadFilePath);
                r.Success = true;
                r.JsonResult = DateTime.Parse(returnMessage).ToString();
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Exception = ex;
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion

        public class ServiceResponse
        {
            public bool Success { get; set; }
            public string JsonResult { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}