using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VirtualTrainer
{
    [Table("Permission")]
    public abstract class Permission
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        public string Info { get; set; }
    }

    public class SystemPermission : Permission
    {
        [ForeignKey("Role")]
        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        [NotMapped]
        public string RoleDescription { get; set; }
    }
    public class ProjectPermission : SystemPermission
    {
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        [NotMapped]
        public string ProjectName { get; set; }
    }
    public class OfficePermission : ProjectPermission
    {
        [ForeignKey("Office")]
        [Required]
        public int OfficeId { get; set; }
        public Office Office { get; set; }
        [NotMapped]
        public string OfficeName { get; set; }
        [NotMapped]
        public string OfficeKey { get; set; }
        [NotMapped]
        public string OrganisationKey { get; set; }
        [NotMapped]
        public string OrganisationName { get; set; }
    }
    public class TeamPermission : ProjectPermission
    {
        [ForeignKey("Team")]
        [Required]
        public int TeamId { get; set; }
        public Team Team { get; set; }
        [NotMapped]
        public string TeamName { get; set; }
        [NotMapped]
        public string TeamKey { get; set; }
        [NotMapped]
        public string OrganisationKey { get; set; }
        [NotMapped]
        public string OrganisationName { get; set; }
    }
}
