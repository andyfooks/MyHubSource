using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class Region : VirtualTrainerBase
    {
        #region [ EF Properties ]

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }

        #endregion
    }
}
