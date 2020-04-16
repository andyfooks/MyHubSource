using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ProjectMembershipDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public bool isProjectAdmin { get; set; }
    }
}
