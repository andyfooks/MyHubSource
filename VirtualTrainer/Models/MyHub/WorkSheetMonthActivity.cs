using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer.Models.MyHub
{
    public class WorkSheetActivity
    {        
        public int Id { get; set; }
        [Description("1")]
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        [Description("2")]
        [DisplayName("Employee ID")]
        public string EmployeeID { get; set; }
        [Description("3")]
        [DisplayName("Team")]
        public string Team { get; set; }
        [Description("4")]
        [DisplayName("High Level Team")]
        public string HighLevelTeam { get; set; }
        [Description("5")]
        [DisplayName("IT Activity Type")]
        public string ITActivityType { get; set; }
        [Description("6")]
        [DisplayName("High Level IT Activity Type")]
        public string HighLevelITActivityType { get; set; }
        [Description("7")]
        [DisplayName("Change Stack")]
        public string ChangeStack { get; set; }
        [Description("8")]
        [DisplayName("Work Item Project Task Type")]
        public string WorkItem_ProjectTaskType { get; set; }
        [Description("9")]
        [DisplayName("Project Task")]
        public string ProjectTask { get; set; }
        [Description("10")]
        [DisplayName("Expenditure Type")]
        public string ExpenditureType { get; set; }
        [Description("11")]
        [DisplayName("Application")]
        public string Application { get; set; }
        [Description("12")]
        [DisplayName("Business Unit")]
        public string BusinessUnit { get; set; }
        [Description("13")]
        [DisplayName("IT Activity")]
        public int ITActivity { get; set; }
        [Description("14")]
        [DisplayName("Location")]
        public string Location { get; set; }
        [Description("17")]
        [DisplayName("Date Processed")]
        public DateTime DateProcessed { get; set; }
        [Description("15")]
        [DisplayName("Month")]
        public DateTime Month { get; set; }
        public int MonthInt { get; set; }
        public int YearInt { get; set; }
        [Description("16")]
        [DisplayName("Date")]
        public DateTime Date { get; set; }
    }
}
