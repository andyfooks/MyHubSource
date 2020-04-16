using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{   
    [NotMapped]
    public class RuleConfigurationObject
    {
        public string ClassName { get; set; }
        public string ClassPropertyName { get; set; }
        public string ContextValue { get; set; }
        public Type PropertyType { get; set; }
    }
    [NotMapped]
    public class BreachLogExcelReport
    {
        [Description("1")]
        [DisplayName("Rule Name")]
        public string RuleName { get; set; }
        [Description("2")]
        [DisplayName("Rule Config Stored Procedure")]
        public string RuleConfigStoredProc { get; set; }
        [Description("3")]
        [DisplayName("Rule Config Name")]
        public string RuleConfigName { get; set; }
        [Description("4")]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Description("5")]
        [DisplayName("Team Name")]
        public string TeamName { get; set; }
        [Description("6")]
        [DisplayName("Office Name")]
        public string OfficeName { get; set; }
        [Description("7")]
        [DisplayName("Project Id")]
        public string ProjectId { get; set; }
        [Description("8")]
        [DisplayName("Broker Name")]
        public string BrokerName { get; set; }
        [Description("9")]
        [DisplayName("Broker Key")]
        public string BrokerKey { get; set; }
        [Description("10")]
        [DisplayName("Context Ref")]
        public string ContextRef { get; set; }
        [Description("11")]
        [DisplayName("Breach Display Html")]
        public string BreachDisplayHtml { get; set; }
        [Description("12")]
        [DisplayName("Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Description("13")]
        [DisplayName("Breach Count")]
        public int BreachCount { get; set; }
        [Description("14")]
        [DisplayName("Breach Field 1")]
        public string BreachField1 { get; set; }
        [Description("15")]
        [DisplayName("Breach Field 2")]
        public string BreachField2 { get; set; }
        [Description("16")]
        [DisplayName("Breach Field 3")]
        public string BreachField3 { get; set; }
        [Description("17")]
        [DisplayName("Breach Field 4")]
        public string BreachField4 { get; set; }
        [Description("18")]
        [DisplayName("Breach Field 5")]
        public string BreachField5 { get; set; }
        [Description("19")]
        [DisplayName("Breach Field 6")]
        public string BreachField6 { get; set; }
        [Description("20")]
        [DisplayName("Breach Field 7")]
        public string BreachField7 { get; set; }
        [Description("21")]
        [DisplayName("Breach Field 8")]
        public string BreachField8 { get; set; }
        [Description("22")]
        [DisplayName("Breach Field 9")]
        public string BreachField9 { get; set; }
        [Description("23")]
        [DisplayName("Breach Field 10")]
        public string BreachField10 { get; set; }
    }
}
