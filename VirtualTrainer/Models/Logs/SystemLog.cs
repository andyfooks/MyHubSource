using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class SystemLog
    {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public DateTime TimeStamp { get; set; }
        public LoggingLevel Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string LogType { get; set; }
        public string StackTrace { get; set; }
        public Guid ProjectID { get; set; }
        public Project Project { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDisplayName { get; set; }
        public string ProjectDescription { get; set; }
        

        public SystemLog() { }
        /// <summary>
        /// Pass in an exception and the instantiated SystemLog's details will be populted with the ex info :)
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="project"></param>
        public SystemLog(Exception ex, Project project)
        {
            Level = LoggingLevel.Error;
            StackTrace = ex.StackTrace;
            ErrorMessage = ex.Message;
            ProjectID = project.ProjectUniqueKey;
            Project = project;
            ProjectName = project.ProjectName;
            ProjectDisplayName = project.ProjectDisplayName;
            ProjectDescription = project.ProjectDescription;
            TimeStamp = DateTime.Now;
            UserName = "System";
            MachineName = System.Environment.MachineName;
        }
        public SystemLog(Exception ex)
        {
            Level = LoggingLevel.Error;
            StackTrace = ex.StackTrace;
            ErrorMessage = ex.Message;
            TimeStamp = DateTime.Now;
            UserName = "System";
            MachineName = System.Environment.MachineName;
        }
    }
}
