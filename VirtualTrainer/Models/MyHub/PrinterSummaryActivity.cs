using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer.Models.MyHub
{
    public class PrinterSummaryActivity
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string AccountID { get; set; }
        public bool AccountIDBillable { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public string PrinterName { get; set; }
        public string PrinterDescription { get; set; }
        public string PrinterID { get; set; }
        public string Department { get; set; }
        public int TotalPages { get; set; }
        public int BWPages { get; set; }
        public int ColourPages { get; set; }
        public string Amount { get; set; }
        public string AltCost { get; set; }
        public string Jobs { get; set; }        
        public string Billable { get; set; }        
        public DateTime ReportDate{get;set;}
    }
}
