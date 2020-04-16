using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class SystemLogDTO
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
        public string ProjectName { get; set; }
        public string ProjectDisplayName { get; set; }
        public string ProjectDescription { get; set; }

    }
}
