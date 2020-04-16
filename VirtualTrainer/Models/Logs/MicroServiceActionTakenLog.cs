using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class MicroServiceActionTakenLog : ActionTakenLog
    {
        #region [ EF Properties ]

        public string ActionName { get; set; }
        public bool? Authenticated { get; set; }
        public string ActionParameters { get; set; }
        public string SessionId { get; set; }

        #endregion

        #region [ Constructors ]

        public MicroServiceActionTakenLog() { }

        public MicroServiceActionTakenLog(Project project)
        {
            // Update project details if exist.
            if (project != null)
            {
                this.Project = project;
                this.ProjectId = project.ProjectUniqueKey;
                this.ProjectName = project.ProjectName;
                this.ProjectDisplayName = project.ProjectDisplayName;
                this.ProjectDescription = project.ProjectDescription;
            }
            this.TimeStamp = DateTime.Now;
            this.Start = DateTime.Now;
        }

        #endregion
    }
}
