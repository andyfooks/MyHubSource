using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer.Models.MyHub
{
    public class PhoneSummaryActivity
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public DateTime ReportDate { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmployeeID { get; set; }
        public string CostCentreID { get; set; }
        public int? NumberOfCalls { get; set; }
        public int? TotalDuration { get; set; }
        public long? DataVolumeKB { get; set; }
        public int? TotalCost { get; set; }
        public int? LineRentalInc { get; set; }
        public int? TotalUsageChargesInc { get; set; }
        public int? ChargeableMinutes { get; set; }
        public int? ChargeableDataKB { get; set; }
    }
}
