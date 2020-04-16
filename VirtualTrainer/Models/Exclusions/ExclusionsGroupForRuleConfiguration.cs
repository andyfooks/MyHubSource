using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ExclusionsGroupForRuleConfiguration : ExclusionsGroup
    {
        [ForeignKey("RuleConfiguration")]
        [Required]
        public int RuleConfigurationId { get; set; }
        public RuleConfiguration RuleConfiguration { get; set; }
    }
}
