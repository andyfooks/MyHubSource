using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer.Models.MyHub
{
    public class TimeSheetData
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? TimeSheetMonthYear { get; set; }
        public string UserName { get; set; }
        public string EmployeeID { get; set; }
        public string SamAccountName { get; set; }
        public string TimeSheetTeamName { get; set; }
        public bool CascadingDropDownListOutOfDate { get; set; }
        public bool Submitted { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? SubmittedTimeStamp { get; set; }
        public bool Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedTimeStamp { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime? UnApprovedTimeStamp { get; set; }
        public DateTime? LastSavedTimeStamp { get; set; }
        public string LastSavedBy { get; set; }
        public string TimeSheetDataJson { get; set; }
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string ManagerEmail { get; set; }
        [NotMapped]
        public bool? Include { get; set; }

        public TimeSheetData SetProperties(string Email,string ManagerEmail, bool Include, string TimeSheetDataJson = "")
        {
            this.Email = Email;
            this.ManagerEmail = ManagerEmail;
            this.Include = Include;
            this.TimeSheetDataJson = TimeSheetDataJson;
            return this;
        }
    }
}
