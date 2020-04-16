using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class UserActivityLog
    {
        public int Id { get; set; }
        public ActivityLogActionType ActionType { get; set; }
        public string DisplayContent { get; set; }
        public DateTime Date { get; set; }
        public bool Success { get; set; }
        public string UserName { get; set; }
        public string UniqueRef { get; set; }
        public string Session { get; set; }
        public string JsonParameters { get; set; }
        public string ReferrerUrl { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public string ItemGuid { get; set; }
        [ForeignKey("Project")]
        public Guid ProjectId { get; set; }
        [Required]
        public Project Project { get; set; }
    }
    public enum ActivityLogActionType
    {
    }
}
