using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class BreachLog
    {
        #region [Entity Framework Properties]

        public int Id { get; set; }
        [ForeignKey("RuleConfiguration")]
        [Required]
        public int RuleConfigurationId { get; set; }
        public RuleConfigurationBase RuleConfiguration { get; set; }
        public string RuleConfigurationName { get; set; }
        public string RuleConfigurationDescription { get; set; }
        [ForeignKey("Rule")]
        [Required]
        public int RuleID { get; set; }
        public Rule Rule { get; set; }
        public string RuleName { get; set; }
        public string RuleDescription { get; set; }
        public string RuleAdditionalDescription { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; }
        public string UserName { get; set; }
        [ForeignKey("Office")]
        public int? OfficeId { get; set; }
        public Office Office { get; set; }
        public string OfficeKey { get; set; }
        public string OfficeName { get; set; }
        [ForeignKey("Region")]
        public int? RegionId { get; set; }
        public Region Region { get; set; }
        public string RegionName { get; set; }
        [ForeignKey("Team")]
        public int? TeamId { get; set; }
        public Team Team { get; set; }
        public string TeamKey { get; set; }
        public string TeamName { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        [ForeignKey("DatabaseDetails")]
        public int? DatabaseDetailsId { get; set; }
        public TargetDatabaseDetails DatabaseDetails { get; set; }
        public string StoredProecdureName { get; set; }
        public string ActurisOrganisationKey { get; set; }
        public string ActurisOrganisationName { get; set; }
        public string ActurisInstanceFriendlyName { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedTimeStamp { get; set; }
        [Required]
        public string ContextRef                    { get; set; }
        public string ContextRefType                { get; set; }
        public string BreachDisplayText             { get; set; }
        public string BreachDisplayAlternateText    { get; set; }
        public DateTime TimeStamp                   { get; set; }
        public string RuleBreachFieldOne            { get; set; }
        public string RuleBreachFieldOneType        { get; set; }
        public string RuleBreachFieldTwo            { get; set; }
        public string RuleBreachFieldTwoType        { get; set; }
        public string RuleBreachFieldThree          { get; set; }
        public string RuleBreachFieldThreeType      { get; set; }
        public string RuleBreachFieldFour           { get; set; }
        public string RuleBreachFieldFourType       { get; set; }
        public string RuleBreachFieldFive           { get; set; }
        public string RuleBreachFieldFiveType       { get; set; }
        public string RuleBreachFieldSix            { get; set; }
        public string RuleBreachFieldSixType        { get; set; }
        public string RuleBreachFieldSeven          { get; set; }
        public string RuleBreachFieldSevenType      { get; set; }
        public string RuleBreachFieldEight          { get; set; }
        public string RuleBreachFieldEightType      { get; set; }
        public string RuleBreachFieldNine           { get; set; }
        public string RuleBreachFieldNineType       { get; set; }
        public string RuleBreachFieldTen            { get; set; }
        public string RuleBreachFieldTenType        { get; set; }
        public string RuleBreachFieldEleven         { get; set; }
        public string RuleBreachFieldElevenType     { get; set; }
        public string RuleBreachFieldTwelve         { get; set; }
        public string RuleBreachFieldTwelveType     { get; set; }
        public string RuleBreachFieldThirteen       { get; set; }
        public string RuleBreachFieldThirteenType   { get; set; }
        public string RuleBreachFieldFourteen       { get; set; }
        public string RuleBreachFieldFourteenType   { get; set; }
        public string RuleBreachFieldFifteen        { get; set; }
        public string RuleBreachFieldFifteenType    { get; set; }
        public string RuleBreachFieldSixteen        { get; set; }
        public string RuleBreachFieldSixteenType    { get; set; }
        public string RuleBreachFieldSeventeen      { get; set; }
        public string RuleBreachFieldSeventeenType  { get; set; }
        public string RuleBreachFieldEighteen       { get; set; }
        public string RuleBreachFieldEighteenType   { get; set; }
        public string RuleBreachFieldNineteen       { get; set; }
        public string RuleBreachFieldNineteenType   { get; set; }
        public string RuleBreachFieldTwenty         { get; set; }
        public string RuleBreachFieldTwentyType     { get; set; }

        #endregion

        #region [Non Entity Framework Properties]

        [NotMapped]
        public int? TeamLeadUserID { get; set; }
        [NotMapped]
        public int? OfficeManagerUserID { get; set; }
        [NotMapped]
        public int? RegionalManagerUserID { get; set; }
        [NotMapped]
        public int? QAUserID { get; set; }
        [NotMapped]
        public int BreachLiveContextRefCount { get; set; }
        [NotMapped]
        public DateTime FirstBreachDate { get; set; }
        [NotMapped]
        public DateTime TimeStampDateOnly
        {
            get { return this.TimeStamp.Date; }
        }

        #endregion

        #region [ Constructors ]

        public BreachLog() { }

        /// <summary>
        /// Pass in Rule Config to auto populate properties.
        /// </summary>
        /// <param name="ruleConfig"></param>
        /// <param name="ctx"></param>
        public BreachLog(RuleConfiguration ruleConfig, VirtualTrainerContext ctx)
        {
            ruleConfig.LoadContextObjects(ctx);
            Office =  ruleConfig.Office;
            DatabaseDetails = ruleConfig.TargetDb;
            Project = ruleConfig.Project;
            Team = ruleConfig.Team;
            RuleConfiguration = ruleConfig;
        }
        public BreachLog(RuleConfigurationBase ruleConfig, VirtualTrainerContext ctx)
        {
            ruleConfig.LoadContextObjects(ctx);
            Project = ruleConfig.Project;
            RuleConfiguration = ruleConfig;
            Rule = ruleConfig.Rule;

            this.RuleID = this.Rule.Id;
            this.RuleConfigurationId = this.RuleConfiguration.Id;
            this.RuleName = this.Rule.Name;
            this.RuleDescription = this.Rule.Description;
            this.RuleConfigurationName = this.RuleConfiguration.Name;
            this.RuleConfigurationDescription = this.RuleConfiguration.Description;
            this.TimeStamp = DateTime.Now;
        }

        #endregion

        #region [ Public Methods ]

        public void LoadTeamLeadId(VirtualTrainerContext context)
        {

        }

        public static List<string> GetBreachFieldNamesForForms()
        {
            List<string> returndata = new List<string>();

            returndata.Add("");
            returndata.Add("ContextRef");
            returndata.Add("BreachDisplayText");
            returndata.Add("RuleBreachFieldOne");
            returndata.Add("RuleBreachFieldTwo");
            returndata.Add("RuleBreachFieldThree");
            returndata.Add("RuleBreachFieldFour");
            returndata.Add("RuleBreachFieldFive");
            returndata.Add("RuleBreachFieldSix");
            returndata.Add("RuleBreachFieldSeven");
            returndata.Add("RuleBreachFieldEight");
            returndata.Add("RuleBreachFieldNine");
            returndata.Add("RuleBreachFieldTen");
            returndata.Add("RuleBreachFieldEleven");
            returndata.Add("RuleBreachFieldTwelve");
            returndata.Add("RuleBreachFieldThirteen");
            returndata.Add("RuleBreachFieldFourteen");
            returndata.Add("RuleBreachFieldFifteen");
            returndata.Add("RuleBreachFieldSixteen");
            returndata.Add("RuleBreachFieldSeventeen");
            returndata.Add("RuleBreachFieldEighteen");
            returndata.Add("RuleBreachFieldNineteen");
            returndata.Add("RuleBreachFieldTwenty");

            return returndata;
        }

        public static List<string> GetMappingFields()
        {
            List<string> mappingFieldsNames = new List<string>();

            mappingFieldsNames.Add("ContextRef");
            mappingFieldsNames.Add("ContextRefType");
            mappingFieldsNames.Add("BreachDisplayText");
            mappingFieldsNames.Add("BreachDisplayAlternateText");
            mappingFieldsNames.Add("TimeStamp");
            mappingFieldsNames.Add("RuleBreachFieldOne");
            //mappingFieldsNames.Add("RuleBreachFieldOneType");
            mappingFieldsNames.Add("RuleBreachFieldTwo");
            //mappingFieldsNames.Add("RuleBreachFieldTwoType");
            mappingFieldsNames.Add("RuleBreachFieldThree");
            // mappingFieldsNames.Add("RuleBreachFieldThreeType");
            mappingFieldsNames.Add("RuleBreachFieldFour");
            //mappingFieldsNames.Add("RuleBreachFieldFourType");
            mappingFieldsNames.Add("RuleBreachFieldFive");
            //mappingFieldsNames.Add("RuleBreachFieldFiveType");
            mappingFieldsNames.Add("RuleBreachFieldSix");
            //mappingFieldsNames.Add("RuleBreachFieldSixType");
            mappingFieldsNames.Add("RuleBreachFieldSeven");
            //mappingFieldsNames.Add("RuleBreachFieldSevenType");
            mappingFieldsNames.Add("RuleBreachFieldEight");
            //mappingFieldsNames.Add("RuleBreachFieldEightType");
            mappingFieldsNames.Add("RuleBreachFieldNine");
            //mappingFieldsNames.Add("RuleBreachFieldNineType");
            mappingFieldsNames.Add("RuleBreachFieldTen");
            //mappingFieldsNames.Add("RuleBreachFieldTenType");
            mappingFieldsNames.Add("RuleBreachFieldEleven");
            //mappingFieldsNames.Add("RuleBreachFieldElevenType");
            mappingFieldsNames.Add("RuleBreachFieldTwelve");
            //mappingFieldsNames.Add("RuleBreachFieldTwelveType");
            mappingFieldsNames.Add("RuleBreachFieldThirteen");
            //mappingFieldsNames.Add("RuleBreachFieldThirteenType");
            mappingFieldsNames.Add("RuleBreachFieldFourteen");
            // mappingFieldsNames.Add("RuleBreachFieldFourteenType");
            mappingFieldsNames.Add("RuleBreachFieldFifteen");
            //mappingFieldsNames.Add("RuleBreachFieldFifteenType");
            mappingFieldsNames.Add("RuleBreachFieldSixteen");
            //mappingFieldsNames.Add("RuleBreachFieldSixteenType");
            mappingFieldsNames.Add("RuleBreachFieldSeventeen");
            //mappingFieldsNames.Add("RuleBreachFieldSeventeenType");
            mappingFieldsNames.Add("RuleBreachFieldEighteen");
            //mappingFieldsNames.Add("RuleBreachFieldEighteenType");
            mappingFieldsNames.Add("RuleBreachFieldNineteen");
            //mappingFieldsNames.Add("RuleBreachFieldNineteenType");
            mappingFieldsNames.Add("RuleBreachFieldTwenty");
            //mappingFieldsNames.Add("RuleBreachFieldTwentyType");

            return mappingFieldsNames;
        }
        public void UpdateBreachFieldWithValue(string fieldName, string fieldValue)
        {
            // Set the Target Breach Field Value from Results
            PropertyInfo prop = this.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(this, fieldValue, null);
            }
        }
        public string GetBreachFieldValue(string fieldName)
        {
            // Set the Target Breach Field Value from Results
            PropertyInfo prop = this.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                return prop.GetValue(this).ToString();
            }
            return string.Empty;
        }
        public BreachLogExcelReport getBreachDetailsForExcel(VirtualTrainerContext context)
        {
            LoadContextObjectsForExcelOutput(context);
            return new BreachLogExcelReport()
            {
                RuleName = this.RuleConfiguration.Rule.Name,
                BreachCount = this.GetBreachCountForContextRef(context),
                BreachDisplayHtml = this.BreachDisplayText,
                BreachField1 = this.RuleBreachFieldOne,
                BreachField2 = this.RuleBreachFieldTwo,
                BreachField3 = this.RuleBreachFieldThree,
                BreachField4 = this.RuleBreachFieldFour,
                BreachField5 = this.RuleBreachFieldFive,
                BreachField6 = this.RuleBreachFieldSix,
                BreachField7 = this.RuleBreachFieldSeven,
                ContextRef = this.ContextRef,
                TimeStamp = this.TimeStamp,
                ProjectId = this.ProjectId.ToString(),
                RuleConfigName = this.RuleConfiguration.Name,
                //RuleConfigStoredProc = this.RuleConfiguration.SqlCommandText,
                UserName = this.User.Name,
                OfficeName = this.OfficeName,
                TeamName = this.TeamName,
                BrokerKey = this.ActurisOrganisationKey,
                BrokerName = this.ActurisOrganisationName
            };
        }
        public int GetBreachCountForContextRef(VirtualTrainerContext context)
        {
            return context.BreachLogs.Where(a => a.UserId == this.UserId
                    && a.ContextRef == this.ContextRef
                    && a.IsArchived != true).Count();
        }
        public bool IsUserAManagerForThisBreach(VirtualTrainerContext ctx, User user)
        {
            LoadRequiredContextObjects(ctx);

            if (this.Office.HasOfficeManager(ctx))
            {
                return this.Office.IsUserAManager(ctx, user);
            }

            return false;
        }
        public bool IsUserATeamLeadForBreach(VirtualTrainerContext ctx, User user)
        {
            LoadRequiredContextObjects(ctx);

            if (this.Team.HasTeamLeads(ctx))
            {
                return this.Team.IsUserATeamLead(ctx, user);
            }

            return false;
        }
        public bool IsUserARegionalManagerForBreach(VirtualTrainerContext ctx, User user)
        {
            LoadRequiredContextObjects(ctx);

            if (this.Team.HasTeamRegionalManager(ctx) || this.Office.HasOfficeRegionalManager(ctx))
            {
                return this.Team.IsUserARegionalManager(ctx, user) || this.Office.IsUserARegionalManager(ctx, user);
            }

            return false;
        }
        public bool IsUserAQualityAuditorForBreach(VirtualTrainerContext ctx, User user)
        {
            LoadRequiredContextObjects(ctx);

            if (this.Team.HasTeamQualityAuditor(ctx) || this.Office.HasOfficeQualityAuditor(ctx))
            {
                return this.Team.IsUserAQualityAuditor(ctx, user) || this.Office.IsUserAQualityAuditor(ctx, user);
            }

            return false;
        }

        #endregion

        #region [ Private methods ]

        private void LoadRequiredContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Office").IsLoaded)
            {
                ctx.Entry(this).Reference("Office").Load();
            }
            if (!ctx.Entry(this).Reference("Team").IsLoaded)
            {
                ctx.Entry(this).Reference("Team").Load();
            }
        }
        private void LoadContextObjectsForExcelOutput(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("Office").IsLoaded)
            {
                ctx.Entry(this).Reference("Office").Load();
            }
            if (!ctx.Entry(this).Reference("Team").IsLoaded)
            {
                ctx.Entry(this).Reference("Team").Load();
            }
            if (!ctx.Entry(this).Reference("RuleConfiguration").IsLoaded)
            {
                ctx.Entry(this).Reference("RuleConfiguration").Load();
            }
            if (!ctx.Entry(this.RuleConfiguration).Reference("Rule").IsLoaded)
            {
                ctx.Entry(this.RuleConfiguration).Reference("Rule").Load();
            }
            if (!ctx.Entry(this).Reference("User").IsLoaded)
            {
                ctx.Entry(this).Reference("User").Load();
            }
            if (!ctx.Entry(this).Reference("DatabaseDetails").IsLoaded)
            {
                ctx.Entry(this).Reference("DatabaseDetails").Load();
            }
            if (!ctx.Entry(this).Reference("Project").IsLoaded)
            {
                ctx.Entry(this).Reference("Project").Load();
            }
        }

        #endregion
    }
}
