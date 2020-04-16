using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    /// <summary>
    /// Hard code a value, gets poassed in as a string as is.
    /// </summary>
    public class RuleStoredProcedureInputValueHardCoded : RuleStoredProcedureInputValue
    {
        #region [ EF Properties ]

        [Required]
        public string ParameterValue { get; set; }

        #endregion

        #region [ override methods ]

        public override SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx, User user)
        {
            return GetSqlParameterForRuleParticipant(ctx);
        }
        public override SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx)
        {
            return new SqlParameter(this.ParameterName, this.ParameterValue);
        }

            #endregion
    }
}
