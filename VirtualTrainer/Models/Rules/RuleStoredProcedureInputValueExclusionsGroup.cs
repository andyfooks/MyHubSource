using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    /// <summary>
    /// This will pass the ExclusionsGroup Entries Key value as a comma seperated list into the stored proc
    /// </summary>
    public class RuleStoredProcedureInputValueExclusionsGroup : RuleStoredProcedureInputValue
    {
        #region [ EF properties ]

        [ForeignKey("ExclusionsGroup")]
        [Required]
        public int ExclusionsGroupId { get; set; }
        public ExclusionsGroup ExclusionsGroup { get; set; }

        #endregion

        #region [ non ef properties ]

        [NotMapped]
        public string ExclusionsGroupName { get; set; }

        #endregion

        #region [ override methods ]

        public override SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx, User user)
        {
            return GetSqlParameterForRuleParticipant(ctx);
        }

        public override SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx)
        {
            LoadContextObjects(ctx);

            List<ExclusionsItem> exclusionsItems = ExclusionsGroup.GetExlusionItems(ctx);

            string commaSeperatedLIst = string.Join(",", exclusionsItems.Select(e => e.Key));

            return new SqlParameter(this.ParameterName, commaSeperatedLIst);
        }

        #endregion

        private void LoadContextObjects(VirtualTrainerContext ctx)
        {
            if (!ctx.Entry(this).Reference("ExclusionsGroup").IsLoaded)
            {
                ctx.Entry(this).Reference("ExclusionsGroup").Load();
            }
        }
    }
}
