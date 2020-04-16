using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer.Models.MyHub
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string SavedBy { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string UncheckedUsers { get; set; }
        public bool? IncludeUncheckedUsers { get; set; }  
        [NotMapped]  
        public List<string> UncheckedUsersList { get; set; }

        public EmailTemplate()
        {
            UncheckedUsersList = new List<string>();
        }
    }
}
