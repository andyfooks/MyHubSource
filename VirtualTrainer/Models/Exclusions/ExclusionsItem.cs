using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ExclusionsItem
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ReasonAdded { get; set; }
        public DateTime DateAdded { get; set; }
        public string AddedBy { get; set; }
        [ForeignKey("ExclusionsGroup")]
        [Required]
        public int ExclusionsGroupId { get; set; }
        public ExclusionsGroup ExclusionsGroup { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
