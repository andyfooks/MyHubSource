using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ActurisImportProjectActionTakenLog : ActionTakenLog
    {
        [Required]
        public ActurisImportExecutionHistoryOutcome ExecutionOutcome { get; set; }

        #region [ Constructors ]

        public ActurisImportProjectActionTakenLog() { }

        public ActurisImportProjectActionTakenLog(Project project, VirtualTrainerContext ctx)
        {
            this.Project = project;
            this.ProjectId = project.ProjectUniqueKey;
            this.ProjectName = project.ProjectName;
            this.ProjectDisplayName = project.ProjectDisplayName;
            this.ProjectDescription = project.ProjectDescription;
            this.TimeStamp = DateTime.Now;
            this.Start = DateTime.Now;
        }

        #endregion
    }
    public class ActurisImportActionTakenLog : ActurisImportProjectActionTakenLog
    {
        public int? ActurisImportSyncConfigId { get; set; }
        public string ActurisImportSyncConfigName { get; set; }
        public string ActurisImportSyncConfigDescription { get; set; }
        public ActurisBusinessStructureSyncConfig ActurisImportSyncConfig { get; set; }

        #region [ Constructors ]

        public ActurisImportActionTakenLog() { }

        public ActurisImportActionTakenLog(ActurisBusinessStructureSyncConfig acturisImportConfig, VirtualTrainerContext ctx)
        {
            acturisImportConfig.LoadContextObjects(ctx);

            this.ActurisImportSyncConfig = acturisImportConfig;
            this.ActurisImportSyncConfigDescription = acturisImportConfig.Description;
            this.ActurisImportSyncConfigName = acturisImportConfig.Name;
            this.ActurisImportSyncConfigId = acturisImportConfig.Id;

            this.Project = acturisImportConfig.Project;
            this.ProjectId = acturisImportConfig.Project.ProjectUniqueKey;
            this.ProjectName = acturisImportConfig.Project.ProjectName;
            this.ProjectDisplayName = acturisImportConfig.Project.ProjectDisplayName;
            this.ProjectDescription = acturisImportConfig.Project.ProjectDescription;
            this.TimeStamp = DateTime.Now;
            this.Start = DateTime.Now;
        }

        #endregion
    }
}
