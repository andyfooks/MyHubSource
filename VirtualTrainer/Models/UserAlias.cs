using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class UserAlias : VirtualTrainerBase
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [IsRuleConfigurationParticipant]
        public string Alias { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationKey { get; set; }
        [IsRuleConfigurationParticipant]
        public string ActurisOrganisationName { get; set; }
        [IsRuleConfigurationParticipant]
        public string AliasDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
