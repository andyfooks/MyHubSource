using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class EscalationsFrameworkConfigEmailRazorModel
    {
        public VirtualTrainerContext Context { get; }
        public List<BreachLog> BreachLogs { get; set; }
        public EscalationsFrameworkRuleConfigEmail EFEmailRuleConfig { get; set; }
        public User Recipient { get; set; }
        public bool SendEmail { get; set; }
        public bool AttachExcelofBreaches { get; set; }
        public string SentFromName { get; set; }
        public EscalationsFrameworkConfigEmailRazorModel(VirtualTrainerContext ctx)
        {
            this.Context = ctx;
        }
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string SentFromEmail { get; set; }
    }
}
