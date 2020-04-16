using AJG.VirtualTrainer.Helper.Exchange;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace VirtualTrainer
{
    public class ExchangeEmailRuleConfig : RuleConfigurationBase
    {
        #region [ Mapped Properties ]

        [ForeignKey("ExchangeAccountDetails")]
        [Required]
        public int ExchangeAccountDetailsId { get; set; }
        public ExchangeAccountDetails ExchangeAccountDetails { get; set; }
        public AJGExchangeDeleteMode? ExchangeItemDeleteMode { get; set; }

        // Sent from Filter, SearchFilter (Equal, not equal, contains), text field
        public AJGExchangeMessageSearchFilter? SentFromFilter { get; set; }
        public string SentFromSearchText { get; set; }

        // Logical Operator between Sent From and Subject
        public AJGExchangeLogicalOperator? OperatorForSentFromAndSubject { get; set; }

        // Subject Filter
        public AJGExchangeMessageSearchFilter? SubjectFilter { get; set; }
        public string SubjectSearchText { get; set; }

        // Logical Operator between Subject and Received Date
        public AJGExchangeLogicalOperator? OperatorForSubjectAndDate { get; set; }

        // Date
        public AJGDateSelection? ReceivedDate { get; set; }
        public bool? ReceivedDateUseDateOnly { get; set; }

        // Recieved Date Search Filter - one
        public AJGExchangeMessageSearchFilter? ReceivedDateOneFilter { get; set; }
        public int? ReceivedDateOneOffset { get; set; }
        public AJGDateOffsetPeriod? ReceivedDateOneOffsetPeriod { get; set; }

        // Recieved Date Search Filter - Two
        public AJGExchangeMessageSearchFilter? ReceivedDateTwoFilter { get; set; }
        public int? ReceivedDateTwoOffset { get; set; }
        public AJGDateOffsetPeriod? ReceivedDateTwoOffsetPeriod { get; set; }

        [InverseProperty("ExchangeRuleConfig")]
        public List<ExchangeEmailRuleConfigBreachFieldMappings> BreachFieldMappings { get; set; }

        #endregion

        #region [ Not Mapped Properties ]

        [NotMapped]
        public string ExchangeItemDeleteModeName { get; set; }
        [NotMapped]
        public string SentFromFilterName { get; set; }
        [NotMapped]
        public string OperatorForSentFromAndSubjectName { get; set; }
        [NotMapped]
        public string OperatorForSubjectAndDateName { get; set; }
        [NotMapped]
        public string ReceivedDateName { get; set; }
        [NotMapped]
        public string SubjectFilterName { get; set; }
        [NotMapped]
        public string ReceivedDateOneFilterName { get; set; }
        [NotMapped]
        public string ReceivedDateOneOffsetPeriodName { get; set; }
        [NotMapped]
        public string ReceivedDateTwoFilterName { get; set; }
        [NotMapped]
        public string ReceivedDateTwoOffsetPeriodName { get; set; }
        [NotMapped]
        public string ExchangeAccountDetailsName { get; set; }

        #endregion

        #region [ Private Properties ]

        #endregion

        #region [ Public Methods ] 

        #region [ Overrides ]

        public override void PostProcessing(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
        }
        public override List<BreachLog> ExecuteRuleConfig(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            List<BreachLog> breaches = new List<BreachLog>();
            if (this.IsActive)
            {
                LoadContextObjects(ctx);

                // Retrieve the emails
                List<AJGEmailMessage> messages = GetEmailsFromExchange(saveBreachesToDB);

                // Get the breaches for each email.
                foreach (AJGEmailMessage message in messages)
                {
                    breaches.Add(GetBreachFromEmail(message));
                }

                if (saveBreachesToDB)
                {
                    // Save the breaches to the database. 
                    foreach (BreachLog log in breaches)
                    {
                        this.BreachLogs.Add(log);
                        ctx.SaveChanges();
                    }

                    // If all saved Ok, then we want to do the delete/move action.
                    PerformDeleteActionOnRetrievedMessages(messages);
                }

            }
            return breaches;
        }

        #endregion

        public void UpdateNotMappedFields()
        {
            this.ExchangeItemDeleteModeName = this.ExchangeItemDeleteMode == null ? "" : this.ExchangeItemDeleteMode.ToString();
            this.SentFromFilterName = this.SentFromFilter == null ? "" : this.SentFromFilter.ToString();
            this.OperatorForSentFromAndSubjectName = this.OperatorForSentFromAndSubject == null ? "" : this.OperatorForSentFromAndSubject.ToString();
            this.OperatorForSubjectAndDateName = this.OperatorForSubjectAndDate == null ? "" : this.OperatorForSubjectAndDate.ToString();
            this.ReceivedDateName = this.ReceivedDate == null ? "" : this.ReceivedDate.ToString();
            this.SubjectFilterName = this.SubjectFilter == null ? "" : this.SubjectFilter.ToString();
            this.ReceivedDateOneFilterName = this.ReceivedDateOneFilter == null ? "" : this.ReceivedDateOneFilter.ToString();
            this.ReceivedDateOneOffsetPeriodName = this.ReceivedDateOneOffsetPeriod == null ? "" : this.ReceivedDateOneOffsetPeriod.ToString();
            this.ReceivedDateTwoFilterName = this.ReceivedDateTwoFilter == null ? "" : this.ReceivedDateTwoFilter.ToString();
            this.ReceivedDateTwoOffsetPeriodName = this.ReceivedDateTwoOffsetPeriod == null ? "" : this.ReceivedDateTwoOffsetPeriod.ToString();
        }

        #endregion

        #region [ Private Methods ]

        private List<AJGEmailMessage> GetEmailsFromExchange(bool deleteModeEnabled)
        {
            ExchangeHelper eh = new ExchangeHelper(this.ExchangeAccountDetails.AutoDiscoverEmail, this.ExchangeAccountDetails.AutoDiscoverUserName, this.ExchangeAccountDetails.GetAutoDiscoverUserPasswordDecrypted(), this.ExchangeAccountDetails.AutoDiscoverUserDomain);
            AJGSearchFilter search = GetEmailSearchParameters();
            // We dont want to delete or move the items here. We Want to do it like a transation at the end once all breaches have been stashed away.
            return eh.GetEmailMessagesAndAttachementsFromInbox (search, AJGExchangeDeleteMode.None);
        }
        private void PerformDeleteActionOnRetrievedMessages(List<AJGEmailMessage> messages)
        {
            ExchangeHelper eh = new ExchangeHelper(this.ExchangeAccountDetails.AutoDiscoverEmail, this.ExchangeAccountDetails.AutoDiscoverUserName, this.ExchangeAccountDetails.GetAutoDiscoverUserPasswordDecrypted(), this.ExchangeAccountDetails.AutoDiscoverUserDomain);
            foreach (AJGEmailMessage message in messages)
            {
                eh.DeleteEmail(message.EmailMetadata.MessageID, this.ExchangeItemDeleteMode.GetValueOrDefault());
            }
        }
        private void GetFilter(AJGSearchFilter currentFilter, List<AJGSearchFilter> andFilters, List<AJGSearchFilter> orFilters, AJGExchangeLogicalOperator op)
        {
            if (op == AJGExchangeLogicalOperator.Or)
            {
                orFilters.Add(currentFilter);
            }
            else
            {
                andFilters.Add(currentFilter);
            }
        }
        
        private AJGSearchFilter GetEmailSearchParameters()
        {
            List<AJGSearchFilter> dateFilters = new List<AJGSearchFilter>();
            List<AJGSearchFilter> stringFilters = new List<AJGSearchFilter>();
            DateTime now = DateTime.Now;

            // Get the First Date Filter
            AJGSearchFilterDate Date1SearchFilter = null;
            if ((this.ReceivedDate != null && this.ReceivedDate != AJGDateSelection.None) &&
               (this.ReceivedDateOneFilter != null && this.ReceivedDateOneFilter != AJGExchangeMessageSearchFilter.None))
            {
                DateTime filterDate = GetDateForSearchFilter(this.ReceivedDateOneOffsetPeriod.GetValueOrDefault(), this.ReceivedDateOneOffset.GetValueOrDefault(), now);
                Date1SearchFilter = new AJGSearchFilterDate(filterDate, this.ReceivedDateUseDateOnly.GetValueOrDefault(), this.ReceivedDateOneFilter.GetValueOrDefault(), AJGEmailPropertyDefinition.DateTimeReceived);
                dateFilters.Add(Date1SearchFilter);
            }

            // Get the Second Date Filter
            AJGSearchFilterDate Date2SearchFilter = null;
            if ((this.ReceivedDate != null && this.ReceivedDate != AJGDateSelection.None) &&
               (this.ReceivedDateTwoFilter != null && this.ReceivedDateTwoFilter != AJGExchangeMessageSearchFilter.None))
            {
                DateTime filterDate = GetDateForSearchFilter(this.ReceivedDateTwoOffsetPeriod.GetValueOrDefault(), this.ReceivedDateTwoOffset.GetValueOrDefault(), now);
                Date2SearchFilter = new AJGSearchFilterDate(filterDate, this.ReceivedDateUseDateOnly.GetValueOrDefault(), this.ReceivedDateTwoFilter.GetValueOrDefault(), AJGEmailPropertyDefinition.DateTimeReceived);
                dateFilters.Add(Date2SearchFilter);
            }

            // Get the From Filter
            AJGSearchFilterString fromSearchFilter = null;
            if ((this.SentFromFilter != null && this.SentFromFilter != AJGExchangeMessageSearchFilter.None) &&
                !string.IsNullOrEmpty(this.SentFromSearchText))
            {
                fromSearchFilter = new AJGSearchFilterString(this.SentFromSearchText, this.SentFromFilter.GetValueOrDefault(), AJGEmailPropertyDefinition.Sender);
                stringFilters.Add(fromSearchFilter);
            }

            // Get the Subject Filter
            AJGSearchFilterString SubjectSearchFilter = null;
            if ((this.SubjectFilter != null && this.SubjectFilter != AJGExchangeMessageSearchFilter.None) &&
                !string.IsNullOrEmpty(this.SubjectSearchText))
            {
                SubjectSearchFilter = new AJGSearchFilterString(this.SubjectSearchText, this.SubjectFilter.GetValueOrDefault(), AJGEmailPropertyDefinition.Subject);
                stringFilters.Add(SubjectSearchFilter);
            }

            AJGSearchFilter DateFilterFilter = new AJGSearchFilter(AJGExchangeLogicalOperator.And, dateFilters);
            AJGSearchFilter StringFiltersFilter = new AJGSearchFilter(this.OperatorForSentFromAndSubject.GetValueOrDefault() == AJGExchangeLogicalOperator.None ? AJGExchangeLogicalOperator.Or : this.OperatorForSentFromAndSubject.GetValueOrDefault(), stringFilters);
            List<AJGSearchFilter> dateAndStringSearchFilterList = new List<AJGSearchFilter>();

            if (dateFilters.Count > 0 && stringFilters.Count > 0)
            {
                dateAndStringSearchFilterList.Add(DateFilterFilter);
                dateAndStringSearchFilterList.Add(StringFiltersFilter);
                AJGSearchFilter returnFilter = new AJGSearchFilter(this.OperatorForSubjectAndDate.GetValueOrDefault() == AJGExchangeLogicalOperator.None ? AJGExchangeLogicalOperator.Or : this.OperatorForSubjectAndDate.GetValueOrDefault(), dateAndStringSearchFilterList);

                return returnFilter;
            }
            else if (dateFilters.Count > 0 && stringFilters.Count == 0)
            {
                return DateFilterFilter;
            }
            else if (dateFilters.Count == 0 && stringFilters.Count > 0)
            {
                return StringFiltersFilter;
            }
            else
            {
                return null;
            }
        }
        private DateTime GetDateForSearchFilter(AJGDateOffsetPeriod offset, int offestDuration, DateTime fromDateTime)
        {
            switch (offset)
            {
                case AJGDateOffsetPeriod.Days:
                    return fromDateTime.AddDays(offestDuration);
                case AJGDateOffsetPeriod.Hours:
                    return fromDateTime.AddHours(offestDuration);
                case AJGDateOffsetPeriod.Months:
                    return fromDateTime.AddMonths(offestDuration);
                case AJGDateOffsetPeriod.None:
                    return fromDateTime;
                case AJGDateOffsetPeriod.Years:
                    return fromDateTime.AddYears(offestDuration);
            }
            return fromDateTime;
        }
        private BreachLog GetBreachFromEmail(AJGEmailMessage message)
        {
            BreachLog returnLog = new BreachLog();

            foreach (ExchangeEmailRuleConfigBreachFieldMappings mapping in this.BreachFieldMappings)
            {
                string mappingvalue = string.Empty;
                if (mapping.IsActive)
                {
                    switch (mapping.EmailSearchSource)
                    {
                        case AJGExchangeMessageDataExtractionSource.Attachment:
                            throw new NotImplementedException();
                        case AJGExchangeMessageDataExtractionSource.Body:
                            mappingvalue = GetValuefromString(message.EmailMetadata.MessageBody, mapping.SearchText, mapping.EmailMessgaeSearchType.GetValueOrDefault());
                            break;
                        case AJGExchangeMessageDataExtractionSource.CC:
                            mappingvalue = GetValuefromString(message.EmailMetadata.MessageCC, mapping.SearchText, mapping.EmailMessgaeSearchType.GetValueOrDefault());
                            break;
                        case AJGExchangeMessageDataExtractionSource.From:
                            mappingvalue = GetValuefromString(message.EmailMetadata.MessageFrom, mapping.SearchText, mapping.EmailMessgaeSearchType.GetValueOrDefault());
                            break;
                        case AJGExchangeMessageDataExtractionSource.ReceivedDate:
                            mappingvalue = GetValueFromDate(message.EmailMetadata.MessageRecievedDate, mapping.SearchText, mapping.EmailMessgaeSearchType.GetValueOrDefault());
                            break;
                        case AJGExchangeMessageDataExtractionSource.Subject:
                            mappingvalue = GetValuefromString(message.EmailMetadata.MessageSubject, mapping.SearchText, mapping.EmailMessgaeSearchType.GetValueOrDefault());
                            break;
                        case AJGExchangeMessageDataExtractionSource.To:
                            mappingvalue = GetValuefromString(message.EmailMetadata.MessageTo, mapping.SearchText, mapping.EmailMessgaeSearchType.GetValueOrDefault());
                            break;
                    }
                }

                // Update return breach log with retrieved value.
                returnLog.UpdateBreachFieldWithValue(mapping.MappedToBreachTableColumnName, mappingvalue);
            }

            // Add the exchange Item id for deleting later on;
            UpdateGenericBreachLogFields(returnLog);

            return returnLog;
        }
        private void UpdateGenericBreachLogFields(BreachLog log)
        {
            log.RuleName = this.Rule.Name;
            log.RuleConfigurationName = this.Name;
            log.TimeStamp = DateTime.Now;
            log.RuleID = this.Rule.Id;
            log.RuleDescription = this.Rule.Description;
            log.RuleAdditionalDescription = this.Rule.AdditionalDescription;
            log.RuleConfigurationDescription = this.Description;
            log.IsArchived = false;
        }
        private string GetValuefromString(string sourceString, string searchFilterText, AJGExchangeMessageSearchType searchType)
        {
            string returnString = string.Empty;

            switch (searchType)
            {
                case AJGExchangeMessageSearchType.AbsoluteValue:
                    returnString = sourceString;
                    break;
                case AJGExchangeMessageSearchType.Regex:
                    Regex rgx = new Regex(searchFilterText, RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(sourceString);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            returnString = string.IsNullOrEmpty(returnString) ? match.Value : string.Format("{0}, ", returnString, match.Value);
                        }
                    }
                    break;
                case AJGExchangeMessageSearchType.SQL:
                    throw new NotImplementedException("AJGExchangeMessageSearchType.SQL: not valid for string type value.");
                case AJGExchangeMessageSearchType.ToString:
                    throw new NotImplementedException("AJGExchangeMessageSearchType.ToString: not valid for string type value.");
                case AJGExchangeMessageSearchType.XPath:
                    throw new NotImplementedException("Exchange html is not valid xml so unable to use XPath!");
                    //var doc = XDocument.Parse(sourceString.CloseTags());
                    //XElement node  = doc.XPathSelectElement(searchFilterText);
                    //if (node != null)
                    //{
                    //    returnString = node.ToString();
                    //}
                    break;
            }
            return returnString;
        }
        private string GetValueFromDate(DateTime sourcedate,string searchFilterText, AJGExchangeMessageSearchType searchType)
        {
            switch (searchType)
            {
                case AJGExchangeMessageSearchType.AbsoluteValue:
                    return sourcedate.ToString();
                case AJGExchangeMessageSearchType.ToString:
                    return sourcedate.ToString(searchFilterText);
            }
            return string.Empty;
        }
        private void LoadContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("ExchangeAccountDetails").IsLoaded)
            {
                ctx.Entry(this).Reference("ExchangeAccountDetails").Load();
            }
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
