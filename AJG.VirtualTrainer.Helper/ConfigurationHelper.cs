using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJG.VirtualTrainer.Helper
{
    public enum AppSettingsList
    {
        targetSystem,
        SystemSentFromEmailAddress,
        SystemSentFromName,
        RunSeedOnEFDeployment,
        IsVTMode,
        ExchangeRuleEnabled,
        ProjectName,
        EncryptionKey,
        ExcelRuleEnabled,
        EmailRazorTemplateBodyPath,
        EmailRazorTemplateSubjectPath,
        EmailUserEscalationsEnabled,
        EmailGenericEscalationsEnabled,
        EmailRazorTemplateAttachmentPath,
        EmailAssetsPath,
        MicrosServicesEnabled,
        oledbConnectionString,
        SystemAccessDomain,
        SystemAccessDomainGroup,
        PrintCostPerPage_BlackandWhite,
        PrintCostPerPage_Colour,
        TargetHomePage,
        OverrideUsersSamAccountNames,
        ADImportSamAccountNames,
        TimeSheetProjectId,
        DirectoryEntryPath
    }

    public static class ConfigurationHelper
    {
        public static string Get(AppSettingsList name)
        {
            string ret = ConfigurationManager.AppSettings.Get(name.ToString());

            if (!string.IsNullOrEmpty(ret))
            {
                return ret;
            }
            return string.Empty;
        }
    }
}
