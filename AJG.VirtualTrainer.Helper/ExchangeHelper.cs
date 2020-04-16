using AJG.VirtualTrainer.Helper.General;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJG.VirtualTrainer.Helper.Exchange
{
    public enum AJGExchangeMessageDataExtractionSource
    {
        From,
        To,
        CC,
        ReceivedDate,
        Subject,
        Body,
        Attachment
    }
    public enum AJGExchangeMessageSearchType
    {
        AbsoluteValue,
        Regex,
        XPath,
        ToString,
        SQL
    }
    public enum AJGExchangeMessageAttachmentDocumentTypes
    {
        None,
        txt,
        excel,
        csv
    }
    public enum AJGExchangeMessageSearchFilter
    {
        None,
        IsEqualTo,
        IsNotEqual,
        ContainsSubString,
        IsGreaterThan,
        IsLessThan,
        IsGreaterThanOrEqualTo,
        IsLessThanOrEqualTo
    }
    public enum AJGExchangeLogicalOperator
    {
        None,
        And,
        Or
    }
    public enum AJGDateOffsetPeriod
    {
        None,
        Hours,
        Days,
        Months,
        Years
    }
    public enum AJGDateSelection
    {
        None,
        Today
    }
    public enum AJGExchangeDeleteMode
    {
        None,
        MoveToDeletedItems,
        SoftDelete,
        HardDelete
    }
    public enum AJGEmailPropertyDefinition
    {
        Subject,
        Sender,
        DateTimeReceived
    }
    //public abstract class AJGSearchFilterObjectBase{

    //    public AJGExchangeMessageSearchFilter SearchFilter { get; set; }
    //    public List<AJGSearchFilterObjectBase> SearchFilters { get; set; }
    //    public LogicalOperator Operator { get; set; }
    //}
    public class AJGSearchFilter
    {
        internal AJGSearchFilter() { }

        public AJGSearchFilter(AJGExchangeLogicalOperator Operator, List<AJGSearchFilter> searchFilters)
        {
            this.Operator = Operator;
            this.SearchFilters = searchFilters;
        }
        public AJGExchangeMessageSearchFilter SearchFilter { get; set; }
        public List<AJGSearchFilter> SearchFilters { get; set; }
        public AJGExchangeLogicalOperator Operator { get; set; }
        public AJGEmailPropertyDefinition PropertyDefinition { get; set; }
    }
    public interface IGetValueAsString
    {
        string GetValueAsString();
    }
    public class AJGSearchFilterString : AJGSearchFilter, IGetValueAsString
    {
        public AJGSearchFilterString(string searchValue, AJGExchangeMessageSearchFilter searchFilter, AJGEmailPropertyDefinition propertyDefinition)
        {
            this.SearchValue = searchValue;
            this.SearchFilter = searchFilter;
            this.PropertyDefinition = propertyDefinition;
        }

        public string SearchValue { get; set; }
        public string GetValueAsString()
        {
            return SearchValue;
        }
    }
    public class AJGSearchFilterDate : AJGSearchFilter, IGetValueAsString
    {
        public AJGSearchFilterDate(DateTime searchValue, bool useDateOnly, AJGExchangeMessageSearchFilter searchFilter, AJGEmailPropertyDefinition propertyDefinition)
        {
            this.SearchValue = searchValue;
            this.UseDateOnly = useDateOnly;
            this.SearchFilter = searchFilter;
            this.PropertyDefinition = propertyDefinition;
        }
        public DateTime SearchValue { get; set; }
        public bool UseDateOnly { get; set; }
        public string GetValueAsString()
        {
            return UseDateOnly ? SearchValue.ToString() : SearchValue.Date.ToString();
        }
    }
    public class ExchangeHelper
    {
        #region [ Properties ]

        private string autodiscoverEmail = string.Empty;
        private string autoDiscoverUserName = string.Empty;
        private string autoDiscoverUserDomain = string.Empty;
        private string autoDiscoverUserPassword = string.Empty;
        private ExchangeService service = null;

        public string AutodiscoverEmail
        {
            get { return autodiscoverEmail; }
        }
        public string AutoDiscoverUserName
        {
            get { return autoDiscoverUserName; }
        }
        public string AutoDiscoverUserDomain
        {
            get { return autoDiscoverUserDomain; }
        }
        public string AutoDiscoverUserPassword
        {
            get { return autoDiscoverUserPassword; }
        }

        #endregion

        #region [ Constructors ]

        public ExchangeService ExchangeService
        {
            get
            {
                Microsoft.Exchange.WebServices.Data.ExchangeService service = new Microsoft.Exchange.WebServices.Data.ExchangeService(ExchangeVersion.Exchange2010_SP2);

                if (string.IsNullOrEmpty(this.autoDiscoverUserDomain) || string.IsNullOrEmpty(this.autoDiscoverUserName) || string.IsNullOrEmpty(this.autoDiscoverUserPassword))
                {
                    service.UseDefaultCredentials = true;
                }
                else
                {
                    service.UseDefaultCredentials = false;
                    service.Credentials = new WebCredentials(this.autoDiscoverUserName, this.autoDiscoverUserPassword, this.autoDiscoverUserDomain);
                }

                service.TraceEnabled = false;
                service.TraceFlags = TraceFlags.All;
                service.AutodiscoverUrl(autodiscoverEmail, RedirectionUrlValidationCallback);

                return service;
            }
        }

        public ExchangeHelper(string autodiscoverEmail, string autoDiscoverUserName, string autoDiscoverUserPassword, string autoDiscoverUserDomain)
        {
            this.autodiscoverEmail = autodiscoverEmail;
            this.autoDiscoverUserName = autoDiscoverUserName;
            this.autoDiscoverUserPassword = autoDiscoverUserPassword;
            this.autoDiscoverUserDomain = autoDiscoverUserDomain;
        }

        #endregion [ Constructors ]

        #region [ Public Methods ]

        public List<AJGEmailMessage> GetEmailMessageAndAttachementsFromInbox(string EmailTitle, string EmailSenderEmailAddress, AJGExchangeDeleteMode deleteMode = AJGExchangeDeleteMode.None)
        {
            return this.GetEmailMessageAndAttachements(this.RetrieveExchangeItems(EmailTitle, EmailSenderEmailAddress), deleteMode);
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="SubjectFilters"></param>
        ///// <param name="BodyFilters"></param>
        ///// <param name="FromFilters"></param>
        ///// <param name="DateReceivedFilters"></param>
        ///// <param name="LogicalOperator"></param>
        ///// <param name="deleteMode"></param>
        ///// <param name="ItemViewPageSize"></param>
        ///// <param name="itemOffset"></param>
        ///// <returns></returns>
        //public List<AJGEmailMessage> GetEmailMessagesAndAttachementsFromInbox(List<AJGStringSearchFilter> SubjectFilters, List<AJGStringSearchFilter> BodyFilters, List<AJGStringSearchFilter> FromFilters,
        //    List<AJGDateSearchFilter> DateReceivedFilters, AJGExchangeLogicalOperator LogicalOperator, AJGExchangeDeleteMode deleteMode = AJGExchangeDeleteMode.None,
        //    int ItemViewPageSize = 20, int itemOffset = 0)
        //{
        //    List<AJGEmailMessage> returnEmails = new List<AJGEmailMessage>();

        //    FindItemsResults<Item> items = RetrieveExchangeItems(SubjectFilters, BodyFilters, FromFilters, DateReceivedFilters, LogicalOperator, ItemViewPageSize, itemOffset);

        //    return GetEmailMessageAndAttachements(items, deleteMode);
        //}

        public List<AJGEmailMessage> GetEmailMessagesAndAttachementsFromInbox(AJGSearchFilter SearchFilter, AJGExchangeDeleteMode deleteMode = AJGExchangeDeleteMode.None, int ItemViewPageSize = 20, int itemOffset = 0)
        {
            List<AJGEmailMessage> returnEmails = new List<AJGEmailMessage>();
            SearchFilter sf = null;
            if (SearchFilter != null)
            {
                sf = ConvertSearchFilters(SearchFilter);
            }
            FindItemsResults<Item> items = RetrieveExchangeItems(sf, ItemViewPageSize, itemOffset);

            return GetEmailMessageAndAttachements(items, deleteMode);
        }

        #endregion

        #region [ Private Methods ]

        private PropertyDefinitionBase ConvertPropertyDef(AJGEmailPropertyDefinition propertyDefinition)
        {
            switch (propertyDefinition)
            {
                case AJGEmailPropertyDefinition.DateTimeReceived:
                    return ItemSchema.DateTimeReceived;
                case AJGEmailPropertyDefinition.Sender:
                    return EmailMessageSchema.From;
                case AJGEmailPropertyDefinition.Subject:
                    return EmailMessageSchema.Subject;
            }
            return null;
        }
        private LogicalOperator ConvertLogicalOperator(AJGExchangeLogicalOperator logicalOperator)
        {
            switch(logicalOperator)
            {
                case AJGExchangeLogicalOperator.And:
                    return LogicalOperator.And;
                case AJGExchangeLogicalOperator.Or:
                    return LogicalOperator.Or;
                default:
                    return LogicalOperator.And;
            }
        }
        private SearchFilter ConvertSearchFilter(AJGExchangeMessageSearchFilter filter, AJGEmailPropertyDefinition propertyDef, object value)
        {
            switch (filter)
            {
                case AJGExchangeMessageSearchFilter.ContainsSubString:
                    return new SearchFilter.ContainsSubstring(ConvertPropertyDef(propertyDef), value.ToString());
                case AJGExchangeMessageSearchFilter.IsEqualTo:
                    return new SearchFilter.IsEqualTo(ConvertPropertyDef(propertyDef), value);
                case AJGExchangeMessageSearchFilter.IsGreaterThan:
                    return new SearchFilter.IsGreaterThan(ConvertPropertyDef(propertyDef), value);
                case AJGExchangeMessageSearchFilter.IsGreaterThanOrEqualTo:
                    return new SearchFilter.IsGreaterThanOrEqualTo(ConvertPropertyDef(propertyDef), value);
                case AJGExchangeMessageSearchFilter.IsLessThan:
                    return new SearchFilter.IsLessThan(ConvertPropertyDef(propertyDef), value);
                case AJGExchangeMessageSearchFilter.IsLessThanOrEqualTo:
                    return new SearchFilter.IsLessThanOrEqualTo(ConvertPropertyDef(propertyDef), value);
                case AJGExchangeMessageSearchFilter.IsNotEqual:
                    return new SearchFilter.IsNotEqualTo(ConvertPropertyDef(propertyDef), value);
            }
            return null;
        }
        private SearchFilter ConvertSearchFilters(AJGSearchFilter SearchFilter)
        {
            SearchFilter returnFilter = null;

            if (typeof(AJGSearchFilterDate) == SearchFilter.GetType())
            {
                var a = (AJGSearchFilterDate)SearchFilter;
                returnFilter = ConvertSearchFilter(a.SearchFilter, a.PropertyDefinition, a.UseDateOnly ? a.SearchValue.Date : a.SearchValue);
            }
            else if (typeof(AJGSearchFilterString) == SearchFilter.GetType())
            {
                var a = (AJGSearchFilterString)SearchFilter;
                returnFilter = ConvertSearchFilter(a.SearchFilter, a.PropertyDefinition, a.SearchValue);
            }
            else
            {
                List<SearchFilter> searchFilters = new List<SearchFilter>();
                if (SearchFilter.SearchFilters.Count > 0)
                {
                    foreach (AJGSearchFilter filterItem in SearchFilter.SearchFilters)
                    {
                        searchFilters.Add(ConvertSearchFilters(filterItem));
                    }
                    returnFilter = new SearchFilter.SearchFilterCollection(ConvertLogicalOperator(SearchFilter.Operator), searchFilters);
                }
            }

            return returnFilter;
        }
        private void GetAllFolders(WellKnownFolderName wellKNownFolderName, ExchangeService service, List<Folder> completeListOfFolderIds)
        {
            FolderView folderView = new FolderView(int.MaxValue);
            FindFoldersResults fidnFolderResults = service.FindFolders(wellKNownFolderName, folderView);
            foreach (Folder folder in fidnFolderResults)
            {
                completeListOfFolderIds.Add(folder);
                FindAllSubFolders(service, folder.Id, completeListOfFolderIds);
            }
        }
        private void FindAllSubFolders(ExchangeService service, FolderId parentFolderId, List<Folder> completeListOfFolderIds)
        {
            FolderView view = new FolderView(int.MaxValue);
            try {
                FindFoldersResults foundFolders = service.FindFolders(parentFolderId, view);
                completeListOfFolderIds.AddRange(foundFolders);

                foreach (Folder folder in foundFolders)
                {
                    FindAllSubFolders(service, folder.Id, completeListOfFolderIds);
                }
            }catch(Exception ed)
            { }
        }
        private FindItemsResults<Item> RetrieveExchangeItems(SearchFilter searchFilter, int ItemViewPageSize, int itemOffset)
        {
            ExchangeService service = this.ExchangeService;
            List<Folder> fullListOfFolders = new List<Folder>();
            
            // Create a view with a page size of 1.
            ItemView view = new ItemView(ItemViewPageSize, itemOffset);

            // Identify the Subject and DateTimeReceived properties to return. Indicate that the base property will be the item identifier
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived);

            // Order the search results by the DateTimeReceived in descending order.
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // Set the traversal to shallow. (Shallow is the default option; other options are Associated and SoftDeleted.)
            view.Traversal = ItemTraversal.Shallow;

            if (searchFilter == null)
            {
                return service.FindItems(WellKnownFolderName.Inbox, view);
            }
            else
            {
                return service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);
            }
        }
        private static ExchangeService GetExchangeService(string autodiscoverEmail, string username, string userPW, string userDomain)
        {
            Microsoft.Exchange.WebServices.Data.ExchangeService service = new Microsoft.Exchange.WebServices.Data.ExchangeService(ExchangeVersion.Exchange2010_SP2);
            if (string.IsNullOrEmpty(username))
            {
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.UseDefaultCredentials = false;
                service.Credentials = new WebCredentials(username, userPW, userDomain);
            }
            service.TraceEnabled = true;
            service.TraceFlags = TraceFlags.All;
            service.AutodiscoverUrl(autodiscoverEmail, RedirectionUrlValidationCallback);
            return service;
        }
        private FindItemsResults<Item> RetrieveExchangeItems(string messageSubject, string senderEmail)
        {
            Microsoft.Exchange.WebServices.Data.ExchangeService service = this.ExchangeService;

            // Add a search filter that searches on the body or subject.
            List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
            if (!string.IsNullOrEmpty(messageSubject))
            {
                searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Subject, messageSubject));
            }

            searchFilterCollection.Add(new SearchFilter.ContainsSubstring(EmailMessageSchema.From, senderEmail));

            // Create the search filter.
            SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, searchFilterCollection.ToArray());

            // Create a view with a page size of 1.
            ItemView view = new ItemView(1);

            // Identify the Subject and DateTimeReceived properties to return. Indicate that the base property will be the item identifier
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived);
            view.PropertySet.RequestedBodyType = BodyType.Text;
            // Order the search results by the DateTimeReceived in descending order.
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Descending);

            // Set the traversal to shallow. (Shallow is the default option; other options are Associated and SoftDeleted.)
            view.Traversal = ItemTraversal.Shallow;

            //var items = service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);
            //service.LoadPropertiesForItems(items, new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.Attachments));

            // Send the request to search the Inbox and get the results.
            return service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);
        }
        private List<AJGEmailMessage> GetEmailMessageAndAttachements(FindItemsResults<Item> findResults, AJGExchangeDeleteMode deleteMode)
        {
            List<AJGEmailMessage> returnItems = new List<AJGEmailMessage>();
            // Process each item.
            foreach (Item myItem in findResults.Items)
            {
                if (myItem is EmailMessage)
                {
                    EmailMessage message = myItem as EmailMessage;

                    AJGEmailMessage ajgMailMessage = new AJGEmailMessage(message);

                    if (deleteMode != AJGExchangeDeleteMode.None)
                    {
                        switch (deleteMode)
                        {
                            case AJGExchangeDeleteMode.HardDelete:
                                message.Delete(DeleteMode.HardDelete);
                                break;
                            case AJGExchangeDeleteMode.MoveToDeletedItems:
                                message.Delete(DeleteMode.MoveToDeletedItems);
                                break;
                            case AJGExchangeDeleteMode.SoftDelete:
                                message.Delete(DeleteMode.SoftDelete);
                                break;
                        }
                        message.Delete(DeleteMode.MoveToDeletedItems);
                    }
                    returnItems.Add(ajgMailMessage);
                }
            }
            return returnItems;
        }
        public void DeleteEmail(string itemId, AJGExchangeDeleteMode deleteMode)
        {
            if (deleteMode != AJGExchangeDeleteMode.None)
            {
                EmailMessage message = EmailMessage.Bind(this.service, new ItemId(itemId));
                if (message != null)
                {
                    switch (deleteMode)
                    {
                        case AJGExchangeDeleteMode.HardDelete:
                            message.Delete(DeleteMode.HardDelete);
                            break;
                        case AJGExchangeDeleteMode.MoveToDeletedItems:
                            message.Delete(DeleteMode.MoveToDeletedItems);
                            break;
                        case AJGExchangeDeleteMode.SoftDelete:
                            message.Delete(DeleteMode.SoftDelete);
                            break;
                    }
                }
            }
        }
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        #endregion
    }
    public class AJGEmailMessage
    {
        private GeneralHelper.EmailMetadataFields emailInfo = new GeneralHelper.EmailMetadataFields();

        public GeneralHelper.EmailMetadataFields EmailMetadata
        {
            get { return this.emailInfo; }
        }
        public string MessageConversationID
        {
            get { return this.emailInfo.MessageConversationID; }
        }
        public DateTime MessageRecievedDate
        {
            get { return this.emailInfo.MessageRecievedDate; }
        }
        public string MessageID
        {
            get { return this.emailInfo.MessageID; }
        }
        public string MessageCC
        {
            get { return this.emailInfo.MessageCC; }
        }
        public string MessageFrom
        {
            get { return this.emailInfo.MessageFrom; }
        }
        public string MessageTo
        {
            get { return this.emailInfo.MessageTo; }
        }
        public string MessageSubject
        {
            get { return this.emailInfo.MessageSubject; }
        }
        public byte[] Message
        {
            get { return this.emailInfo.Message; }
        }
        public List<GeneralHelper.Attachment> Attachemnts
        {
            get { return this.emailInfo.Attachemnts; }
        }
        public bool MessageHasAttachments
        {
            get { return this.emailInfo.MessageHasAttachments; }
        }

        public AJGEmailMessage(EmailMessage exchangeEmailMessage)
        {
            exchangeEmailMessage.Load(new PropertySet(ItemSchema.MimeContent, ItemSchema.Subject,
                ItemSchema.HasAttachments, ItemSchema.Body, ItemSchema.DateTimeReceived,
                ItemSchema.Id, EmailMessageSchema.ToRecipients, EmailMessageSchema.From, EmailMessageSchema.CcRecipients,
                ItemSchema.ConversationId));

            this.emailInfo.ToAddressesFromHeader = GetToAddressFromHeader(exchangeEmailMessage);

            var mimeContent = exchangeEmailMessage.MimeContent;
            // Set message fields.
            this.emailInfo.MessageSubject = exchangeEmailMessage.Subject;
            this.emailInfo.Message = mimeContent.Content;
            this.emailInfo.Attachemnts = this.GetAttachments(exchangeEmailMessage);
            //this.emailInfo.MessageToList = new List<string>();
            foreach (EmailAddress recipient in exchangeEmailMessage.ToRecipients)
            {
                this.emailInfo.MessageTo = string.IsNullOrEmpty(this.emailInfo.MessageTo) ? recipient.Address : string.Format("{0}; {1}", this.emailInfo.MessageTo, recipient.Address);
                //this.emailInfo.MessageToList.Add(recipient.Address);
            }
            if (exchangeEmailMessage.From != null)
            {
                this.emailInfo.MessageFrom = exchangeEmailMessage.From.Address;
            }
            foreach (EmailAddress ccRecipient in exchangeEmailMessage.CcRecipients)
            {
                this.emailInfo.MessageCC = string.IsNullOrEmpty(this.emailInfo.MessageCC) ? ccRecipient.Address : string.Format("{0}; {1}", this.emailInfo.MessageCC, ccRecipient.Address);
            }
            this.emailInfo.MessageID = exchangeEmailMessage.Id.UniqueId;
            this.emailInfo.MessageConversationID = exchangeEmailMessage.ConversationId;
            this.emailInfo.MessageRecievedDate = exchangeEmailMessage.DateTimeReceived;
            this.emailInfo.MessageBody = exchangeEmailMessage.Body.ToString();
        }
        public List<string> GetToAddressFromHeader(EmailMessage exchangeEmailMessage)
        {
            List<string> emailsInheader = new List<string>();

            ExtendedPropertyDefinition PR_TRANSPORT_MESSAGE_HEADERS = new ExtendedPropertyDefinition(0x007D, MapiPropertyType.String);
            PropertySet psPropSet = new PropertySet(BasePropertySet.FirstClassProperties) { PR_TRANSPORT_MESSAGE_HEADERS, ItemSchema.MimeContent };

            exchangeEmailMessage.Load(psPropSet);

            Object valHeaders;
            if (exchangeEmailMessage.TryGetProperty(PR_TRANSPORT_MESSAGE_HEADERS, out valHeaders))
            {
                Regex regexGetToString = new Regex(@"(?<=To:)(?s).[^:]*");
                Regex regexGetEmails = new Regex(@"\b[A-Za-z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,4}\b");

                // Get the To: section of the header.
                Match match = regexGetToString.Match(valHeaders.ToString());

                // If a match was found,
                if (match.Success)
                {
                    // Then get all the emails from the to section.
                    MatchCollection emailMatches = regexGetEmails.Matches(match.Value);

                    foreach (Match emailMatch in emailMatches)
                    {
                        if (!emailsInheader.Contains(emailMatch.Value))
                        {
                            emailsInheader.Add(emailMatch.Value);
                        }
                    }
                }
            }

            return emailsInheader;
        }
        private List<GeneralHelper.Attachment> GetAttachments(EmailMessage exchangeEmailMessage)
        {
            List<GeneralHelper.Attachment> ajgHelperAttachments = new List<GeneralHelper.Attachment>();

            // Get the attachements.
            exchangeEmailMessage.Load(new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.Attachments));

            this.emailInfo.MessageHasAttachments = exchangeEmailMessage.Attachments.Count > 0 ? true : false;

            foreach (var attachment in exchangeEmailMessage.Attachments)
            {
                attachment.Load();
                Type attachementType = attachment.GetType();

                // This will pick up document attachments, including embedded images.
                if (attachementType == typeof(FileAttachment))
                {
                    FileAttachment fileAttachment = attachment as FileAttachment;
                    ajgHelperAttachments.Add(new GeneralHelper.Attachment { Name = attachment.Name, FileStream = new MemoryStream(fileAttachment.Content), contenttype = attachment.ContentType, isInline = attachment.IsInline });
                }
                // This will pick up attachemnts that are emails.
                else
                {
                    // This will pick up email attachments that are emails!
                    ItemAttachment itemAttachment = attachment as ItemAttachment;

                    // Load Item with additionalProperties of MimeContent
                    itemAttachment.Load(EmailMessageSchema.MimeContent);

                    // MimeContent.Content will give you the byte[] for the ItemAttachment
                    // Now all you have to do is write the byte[] to a file
                    ajgHelperAttachments.Add(new GeneralHelper.Attachment { Name = string.Format("{0}.eml", attachment.Name), FileStream = new MemoryStream(itemAttachment.Item.MimeContent.Content), contenttype = attachment.ContentType, isInline = attachment.IsInline });
                }
            }

            return ajgHelperAttachments;
        }
    }
}

