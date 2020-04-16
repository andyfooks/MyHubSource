using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public abstract class ActionTakenLog
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Success { get; set; }
        public SystemLog ErrorLogEntry { get; set; }
        public string ErrorMessage { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public User User { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDisplayName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? Finish { get; set; }
    }
}
