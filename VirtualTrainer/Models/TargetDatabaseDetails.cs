using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class TargetDatabaseDetails : VirtualTrainerBase
    {
        public int Id { get; set; }
        [IsRuleConfigurationParticipant]
        public int Key { get; set; }
        [IsRuleConfigurationParticipant]
        [Required]
        public string DisplayName { get; set; }
        [IsRuleConfigurationParticipant]
        public string DBServerName { get; set; }
        [IsRuleConfigurationParticipant]
        public string DBName { get; set; }
        [IsRuleConfigurationParticipant]
        public string DBConnectionString { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RuleConfiguration> Rules { get; set; }
    }
}
