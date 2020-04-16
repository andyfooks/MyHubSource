using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ExclusionsGroupForRule : ExclusionsGroup
    {
        [ForeignKey("Rule")]
        [Required]
        public int RuleId { get; set; }
        public Rule Rule { get; set; }
    }
}
