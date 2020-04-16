using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class EscalationsEmailDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime TimeStampDateOnly
        {
            get { return this.TimeStamp.Date; }
        }
    }
}
