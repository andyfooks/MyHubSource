using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public enum ProjectTypeEnum
    {
        Standard = 1,
        VirtualTrainer = 2,
        MicroService = 3
    }
    public enum AppSettingsEnum
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
        TechTimeSheetEnabled
    }
    public enum MessageDTOInfoTypeEnum
    {
        Success,
        Information,
        InvalidOperation,
        Error
    }
    public enum BreachActionEnum
    {
        None,
        Delete,
        Archive
    }
    public enum EscalationEmailGenericActionEnum
    {
        // Email options
        IndividualEmails_UseEmailInBreachField = 1,
        IndividualEmails_ToSpecifiedUser = 2,
        //AmalgamatedEmail_ToSpecifiedUser = 3,
        IndividualEmails_UseEmailInBreachField_ManualSend = 4,
        // Html TO specified location
        IndividualHtml_ToSpecifiedLocation_BodyOnly = 5,
        IndividualHtml_ToSpecifiedLocation_AttachmentOnly = 6,
        IndividualHtml_ToSpecifiedLocation_All = 7,
        // Amalgamated to specific location
        AmalgamatedHtml_ToSpecifiedLocation_BodyOnly = 8,
        AmalgamatedHtml_ToSpecifiedLocation_AttachmentOnly = 9,
        AmalgamatedHtml_ToSpecifiedLocation_All = 10,
        // INdividual Pdfs to specified location
        IndividualPdfs_ToSpecifiedLocation_BodyOnly = 11,
        IndividualPdfs_ToSpecifiedLocation_AttachmentOnly = 12,
        IndividualPdfs_ToSpecifiedLocation_All = 13,
        // Amalgamated pdf to specific location
        //AmalgamatedPdf_ToSpecifiedLocation_BodyOnly = 14,
        //AmalgamatedPdf_ToSpecifiedLocation_AttachmentOnly = 15,
        //AmalgamatedPdf_ToSpecifiedLocation_All = 16
        AmalgamatedHtml_ToSpecifiedUser_BodyOnly = 17,
        AmalgamatedHtml_ToSpecifiedUser_AttachmentOnly = 18,
        AmalgamatedHtml_ToSpecifiedUser_All = 19,
    }
    public enum EscalationInclusionEnum
    {
        AllBreaches,
        OnlyNotArchived,
        OnlyArchived
    }
    public enum RuleConfigExecutionMode
    {
        ExecutePerUser,
        UserReferencedResults
    }
    public enum ActionTakenLogAction
    {

    }
    public enum RuleTarget
    {
        Office,
        Team,
        User,
        LogAllReturnedBreaches
    }
    public enum EscalationsActionTakenLogOutcome
    {
        ProjectNotActive = 1,
        ExecutionSuccess = 2,
        Failure = 3,
        EscalationsFrameworkNotActive = 4,
        NoEscalationsRuleConfigurations = 5,
        EscalationsRuleConfigNotActive = 6,
        EscalationsRuleConfigNoBreachLogs = 7,
        EscalationsRuleConfigNoRecipients = 8,
        EscalationsRuleConfigEmailUserNotActive = 9,
        EscalationsRuleConfigEmailNoBreacheLogsForUser = 10,
        EscalationsRuleConfigEmailRazorModelSetSendEmailToFalse = 11,
        EscalationsRuleConfigNotScheduledToRun = 12,
        EscalationsRuleConfigEmailUserHasNoEmail = 13
    }
    public enum EscalationsFrameworkScheduleFrequencyEnum
    {
        Always = 1,
        DailyAllDays = 2,
        DailyWeekdays = 3,
        DailyWeekends = 4,
        WeeklyMon = 5,
        WeeklyTue = 6,
        WeeklyWed = 7,
        WeeklyThur = 8,
        WeeklyFri = 9,
        WeeklySat = 10,
        WeeklySun = 11,
        MonthlyFirstDay = 12,
        MonthlyLastDay = 13,
        Never = 14
    }
    public enum ActurisImportExecutionHistoryOutcome
    {
        Failure = 1,
        // Success
        Success = 2,
        ProjectNotActive = 3,
        NoConfigurations = 4,
        ProjectExecutionSuccess = 5,
        ImportConfugurationExecutionSuccess = 6,
        ImportConfigurationNotActive = 7,
        ImportConfigurationNotSchedules = 8
    }
    public enum RuleExecutionHistoryLogOutcome
    {
        Failure = 1,
        // Success
        Success = 2,
        ProjectExecutionSuccess = 3,
        RuleExecutionSuccess = 4,
        RuleCofigurationExecutionSuccess = 5,
        RuleTargetUserExecutionSuccess = 6,
        RuleUserExecutionSuccess = 7,
        // Not Active
        ProjectNotActive = 8,
        RuleNotActive = 9,
        RuleConfigurationNotActive = 10,
        OfficeNotActive = 11,
        TeamNotActive = 12,
        RuleUserNotActive = 13,
        UserNotActive = 14,
        // None
        NoUsersAssignedToUserTargetedRule = 15,
        NoRuleConfigurationsForRule = 16
    }
    public enum RoleEnum
    {
        ProjectMember = 1,
        ClaimsHandler = 2,
        TeamLead = 3,
        BranchManager = 4,
        QualityAuditor = 5,
        RegionalManager = 6,
        ProjectAdmin = 7,
        SystemAdmin = 8,
        BranchMember = 9,
        TeamMember = 10, 
        MicroService = 11,
        SuperUser = 12
    }
    public enum ActionEnum
    {
        EmailToHandler = 1,
        EmailToTeamLead = 2,
        EmailToBranchManager = 3,
        EmailToQualityAuditor = 4,
        EmailToRegionalManager = 5
    }
    public enum TitleEnum
    {
        Ms = 1,
        Miss = 2,
        Master = 3,
        Rev = 4,
        Fr = 5,
        Dr = 6,
        Atty = 7,
        Prof = 8, 
        Hon = 9,
        Pres = 10,
        Gov = 11,
        Mr = 12,
        Mrs = 13
    }
    public enum LoggingLevel
    {
        Information,
        Error
    }
}
