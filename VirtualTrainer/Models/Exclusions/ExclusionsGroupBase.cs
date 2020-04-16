using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class ExclusionsGroup
    {
        public int Id { get; set; }
        [Required]
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string ReasonAdded { get; set; }
        public DateTime DateAdded { get; set; }
        public string AddedBy { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        [InverseProperty("ExclusionsGroup")]
        public ICollection<ExclusionsItem> ExclusionItems { get; set; }

        #region [ Public Methods ]

        public List<ExclusionsItem> GetExlusionItems(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);
            return this.ExclusionItems.ToList();
        }

        #endregion

        #region [ Private Methods ]

        private void LoadContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Collection("ExclusionItems").IsLoaded)
            {
                ctx.Entry(this).Collection("ExclusionItems").Load();
            }
        }

        #endregion
    }
}
