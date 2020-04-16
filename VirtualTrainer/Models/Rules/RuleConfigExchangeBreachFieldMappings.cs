using AJG.VirtualTrainer.Helper.Exchange;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace VirtualTrainer
{
    public class ExchangeEmailRuleConfigBreachFieldMappings
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public AJGExchangeMessageDataExtractionSource? EmailSearchSource { get; set; }
        public AJGExchangeMessageSearchType? EmailMessgaeSearchType { get; set; }
        public string SearchText { get; set; }
        public string MappedToBreachTableColumnName { get; set; }
        public AJGExchangeMessageSearchFilter? AttachmentNameSearchFilter { get; set; }
        public string AttachmentNameSearchString { get; set; }
        public AJGExchangeMessageAttachmentDocumentTypes? AttachmentDocumentType { get; set; }

        [ForeignKey("ExchangeRuleConfig")]
        [Required]
        public int ExchangeRuleConfigId { get; set; }
        public ExchangeEmailRuleConfig ExchangeRuleConfig { get; set; }

        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }

        #region [ Not Mapped ]

        [NotMapped]
        public string EmailSearchSourceName { get; set; }
        [NotMapped]
        public string EmailMessgaeSearchTypeName { get; set; }
        [NotMapped]
        public string AttachmentNameSearchFilterName { get; set; }
        [NotMapped]
        public string AttachmentDocumentTypeName { get; set; }

        #endregion

        public void UpdateNotMappedFields()
        {
            this.AttachmentNameSearchFilterName = this.AttachmentNameSearchFilter == null ? "" : this.AttachmentNameSearchFilter.ToString();
            this.EmailSearchSourceName = this.EmailSearchSource == null ? "" : this.EmailSearchSource.ToString();
            this.EmailMessgaeSearchTypeName = this.EmailMessgaeSearchType == null ? "" : this.EmailMessgaeSearchType.ToString();
            this.AttachmentDocumentTypeName = this.AttachmentDocumentType == null ? "" : this.AttachmentDocumentType.ToString();
        }
    }
}
