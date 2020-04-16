using AJG.VirtualTrainer.Helper;
using AJG.VirtualTrainer.Helper.Excel;
using AJG.VirtualTrainer.Helper.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace VirtualTrainer
{
    public class ExcelRuleConfig : RuleConfigurationBase
    {
        #region [ EF Mapped Properties ]

        // Which Document to select
        public string SourceDocFolderPath { get; set; }
        public AJGExcelDocSelectionType? SourceDocSelectionType { get; set; }
        // We could introduce switches e.g. {{date:MM:dd:yy}} = replaces date at run time in format provided etc. So could be SomeDocName{{date:MM:dd:yy}}.xls
        public string SourceDocNameSearchText { get; set; }

        // Extracting and processing data
        public string SQLQuery { get; set; }
        public bool hasHeaderRow { get; set; }
        public string sourceFileNameSavedInBreachFieldName { get; set; }
        [InverseProperty("ExcelRuleConfig")]
        public List<ExcelRuleConfigBreachFieldMapping> BreachFieldMappings { get; set; }

        // What to Do with the Doc Once processed.
        public AJGExcelDocDeleteMode? DocumentDeleteMode { get; set; }
        public string MoveDocumentDestinationDirectory { get; set; }
        public bool? AppendTodaysDateToMovedDocName { get; set; }
        public bool? AutoMapResultsToBreachTableFields { get; set; }

        #endregion

        #region [ Not Mapped Properties ]

        [NotMapped]
        public string DocumentSelectionTypeName
        {
            get
            {
                return this.SourceDocSelectionType == null ? "" : this.SourceDocSelectionType.ToString();
            }
        }
        [NotMapped]
        public string AJGExcelDocDeleteModeName {
            get
            {
                return this.DocumentDeleteMode == null ? "" : this.DocumentDeleteMode.ToString();
            }
        }
        [NotMapped]
        public string ExecuteRuleConfigErrorMessage { get; set; }
        [NotMapped]
        public bool? ExecuteRuleConfigSuccess { get; set; }

        #endregion

        #region [ Public override methods ]

        public override void PostProcessing(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            try
            {
                if (this.IsActive)
                {
                    if (saveBreachesToDB)
                    {
                        // Get the document path.
                        string documentPath = GetDocumentPath();

                        // If all saved Ok, then we want to do the delete/move action.
                        PerformPostProcessAction(documentPath);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public override List<BreachLog> ExecuteRuleConfig(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            List<BreachLog> breaches = new List<BreachLog>();

            try
            {
                if (this.IsActive)
                {
                    // Load context objects
                    LoadContextObjects(ctx);

                    // Get the document path.
                    string documentPath = GetDocumentPath();

                    // Now get the breaches from the excel doc.
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        string oledbConnectionString = ConfigurationHelper.Get(AppSettingsList.oledbConnectionString);
                        breaches = GetBreaches(documentPath, ctx, oledbConnectionString);

                        if (saveBreachesToDB)
                        {
                            // Get the existing Outstanding breaches.
                            List<BreachLog> outstandingBreaches = this.GetAllOutstandingBreaches(ctx);

                            // Save the new breaches to the database. 
                            foreach (BreachLog log in breaches)
                            {
                                this.BreachLogs.Add(log);
                            }
                            //ctx.BreachLogs.AddRange(breaches);
                            ctx.SaveChanges();

                        }
                    }
                    //else
                    //{
                    //    throw new Exception("No File has been found.");
                    //}
                }
            }
            catch (Exception)
            {
                throw;
                //SystemLog errorlog = new SystemLog(ex, this.Project);
                //ctx.SystemLogs.Add(errorlog);
                //ctx.SaveChanges();
            }
            finally { }

            return breaches;
        }
        public bool ValuesAreValid(out string returnValue)
        {
            returnValue = string.Empty;

            switch (this.DocumentDeleteMode)
            {
                case AJGExcelDocDeleteMode.Delete:
                    return true;
                case AJGExcelDocDeleteMode.MoveTo:
                    if (string.IsNullOrEmpty(this.MoveDocumentDestinationDirectory))
                    {
                        returnValue = "The MoveDocumentDestinationDirectory must have a value";
                        return false;
                    }
                    else
                    {
                        if (!DirectoryHelper.ValidateDirectoryAccess(this.MoveDocumentDestinationDirectory, ConfigurationManager.AppSettings[AppSettingsEnum.ProjectName.ToString()], out returnValue))
                        {
                            return false;
                        }
                    }
                    break;
            }
            if (string.IsNullOrEmpty(this.SourceDocFolderPath))
            {
                returnValue = "the SourceDocFolderPath must have a value.";
                return false;
            }
            else
            {
                string errorMessage = string.Empty;
                if (!DirectoryHelper.ValidateDirectoryAccess(this.SourceDocFolderPath, ConfigurationManager.AppSettings[AppSettingsEnum.ProjectName.ToString()], out errorMessage))
                {
                    returnValue = string.Format("SourceDocFolderPath: {0}", errorMessage);
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region [ Private Methods ]

        private void PerformPostProcessAction(string documentPath)
        {
            switch (this.DocumentDeleteMode)
            {
                case AJGExcelDocDeleteMode.Delete:
                    if(System.IO.File.Exists(documentPath))
                    {
                        System.IO.File.Delete(documentPath);
                    }
                    break;
                case AJGExcelDocDeleteMode.MoveTo:
                    if (System.IO.File.Exists(documentPath) && System.IO.Directory.Exists(this.MoveDocumentDestinationDirectory))
                    {
                        string fileNameWithoutExtension = this.AppendTodaysDateToMovedDocName.GetValueOrDefault() ? 
                                                          string.Format("{0}_{1}",System.IO.Path.GetFileNameWithoutExtension(documentPath), DateTime.Now.ToString("ddMMMyyyy_HHmm")) :
                                                          System.IO.Path.GetFileNameWithoutExtension(documentPath);

                        string fileName = string.Format("{0}{1}", fileNameWithoutExtension, System.IO.Path.GetExtension(documentPath));
                        string destinationPath = System.IO.Path.Combine(this.MoveDocumentDestinationDirectory, fileName);
                        System.IO.File.Move(documentPath, destinationPath);
                    }
                    break;
            }
        }
        private List<BreachLog> GetBreaches(string documentPath, VirtualTrainerContext ctx, string oledbConnectionString)
        {
            List<BreachLog> breaches = new List<BreachLog>();

            // Get the results from the excel document.
            using (ExcelHelper helper = new ExcelHelper(documentPath, this.hasHeaderRow))
            {
                helper.GetResultsFromDocument(this.SQLQuery);
                // Now do the mapping from the excel doc to the breachlogs.
                breaches = GetBreachesFromExcelHelper(helper, ctx);
            }

            return breaches;
        }
        private List<BreachLog> GetBreachesFromExcelHelper(ExcelHelper helper, VirtualTrainerContext ctx)
        {
            List<BreachLog> breaches = new List<BreachLog>();

            if (this.AutoMapResultsToBreachTableFields.GetValueOrDefault())
            {
                foreach (ResultRow row in helper.Results)
                {
                    BreachLog log = new BreachLog(this, ctx);

                    foreach (var column in row.Columns)
                    {
                        log.UpdateBreachFieldWithValue(column.Key, column.Value);
                    }

                    // IF config set to store file name in breach field.
                    if(!string.IsNullOrEmpty(this.sourceFileNameSavedInBreachFieldName))
                    {
                        log.UpdateBreachFieldWithValue(this.sourceFileNameSavedInBreachFieldName, helper.FileName);
                    }

                    breaches.Add(log);
                }
            }
            else
            {
                // Don't bother processing if there are no mappings
                if (this.BreachFieldMappings.Any())
                {
                    // Do the mapping from the results.
                    foreach (ResultRow row in helper.Results)
                    {
                        BreachLog log = new BreachLog();
                        foreach (ExcelRuleConfigBreachFieldMapping mapping in this.BreachFieldMappings)
                        {
                            string value = row.Columns.Where(a => a.Key == mapping.SqlQueryResultColumnName).FirstOrDefault().Value;
                            log.UpdateBreachFieldWithValue(mapping.MappedToBreachTableColumnName, value);
                        }
                        breaches.Add(log);
                    }
                }
            } 

            return breaches;
        }
        private string GetDocumentPath()
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(this.SourceDocFolderPath);

            string filePath = string.Empty;
            System.IO.FileInfo fileInfo = null;

            switch (this.SourceDocSelectionType)
            {
                case AJGExcelDocSelectionType.MostRecent:
                    fileInfo = info.GetFiles().OrderByDescending(a => a.LastWriteTime).FirstOrDefault();
                    break;
                case AJGExcelDocSelectionType.DocName_AbsoluteValue:
                    fileInfo = info.GetFiles().Where(a => a.Name.ToLower() == this.SourceDocNameSearchText.ToLower()).FirstOrDefault();
                    break;
                case AJGExcelDocSelectionType.DocName_ContainsSubString:
                    fileInfo = info.GetFiles().Where(a => a.Name.ToLower().Contains(this.SourceDocNameSearchText.ToLower())).FirstOrDefault();
                    break;
                case AJGExcelDocSelectionType.DocName_Regex:
                    throw new NotImplementedException("regex has not been implemented.");
                case AJGExcelDocSelectionType.DocName_StartsWith:
                    fileInfo = info.GetFiles().Where(a => a.Name.ToLower().StartsWith(this.SourceDocNameSearchText.ToLower())).FirstOrDefault();
                    break;
            }

            if (fileInfo != null)
            {
                filePath = fileInfo.FullName;
            }

            return filePath;
        }
        private new void LoadContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Rule").IsLoaded)
            {
                ctx.Entry(this).Reference("Rule").Load();
            }
            if (!ctx.Entry(this).Collection("BreachFieldMappings").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachFieldMappings").Load();
            }
            if (!ctx.Entry(this).Collection("BreachLogs").IsLoaded)
            {
                ctx.Entry(this).Collection("BreachLogs").Load();
            }
        }

        #endregion
    }
}
