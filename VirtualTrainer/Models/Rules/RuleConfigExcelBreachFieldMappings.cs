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
    public class ExcelRuleConfigBreachFieldMapping
    {
        #region [ RF Mapped properties ]

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string SqlQueryResultColumnName { get; set; }
        public string MappedToBreachTableColumnName { get; set; }

        [ForeignKey("ExcelRuleConfig")]
        [Required]
        public int ExcelRuleConfigId { get; set; }
        public ExcelRuleConfig ExcelRuleConfig { get; set; }

        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }

        #endregion
    }
}
