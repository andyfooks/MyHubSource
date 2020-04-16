using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTTests
{
    [Table("Introducer Codes")]
    public class IntroducerCodes
    {
        public int Id { get; set; }
        public string AGENT_Code { get; set; }
        public string AGENT_Contact { get; set; }
        public string AGENT_Name { get; set; }
        public string AGENT_TradingName { get; set; }
        public string AGENT_Address { get; set; }
        public string AGENT_Email { get; set; }
    }
    [Table("Debtors List")]
    public class DebtorsList
    {
        public int Id { get; set; }
        public string Debtor { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public string Balance { get; set; }
        [Column("Not Due")]
        public string NotDue { get; set; }
        [Column("1-30 days")]
        public string OneTo30Days { get; set; }
        [Column("31-60 days")]
        public string ThrityOneTo60Days { get; set; }
        [Column("61-90 days")]
        public string SixtyOneto90Days { get; set; }
        [Column("91-120 days")]
        public string NinetyOneTo120Days { get; set; }
        [Column("over 120 days")]
        public string Over120Days { get; set; }
    }
    [Table("Suspense")]
    public class Suspense
    {
        public int Id { get; set; }
        [Column("INTRODUCER (IF KNOWN)")]
        public string Introducer { get; set; }
        [Column("DATE")]
        public string Date { get; set; }
        [Column("METHOD")]
        public string Method { get; set; }
        [Column("AMOUNT")]
        public string Amount { get; set; }
    }
}
