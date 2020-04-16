using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ClassInputValueDTO
    {
        public int Id { get; set; }
        public string ParameterName { get; set; }
        public string Description { get; set; }
        public string ClassProperty { get; set; }
        public int RuleConfigId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
