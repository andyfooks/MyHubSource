using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsProject { get; set; }
        public bool IsOffice { get; set; }
        public bool IsTeam { get; set; }
        public bool IsActive { get; set; }
    }
}
