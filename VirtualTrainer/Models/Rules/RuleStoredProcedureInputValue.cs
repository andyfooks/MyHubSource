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
    /// Supports either hard coded text value
    /// Or a reference to a class property value that would be resolved in run time.
    /// </summary>
    public abstract class RuleStoredProcedureInputValue
    {
        #region [ EF Properties ]
        public int Id { get; set; }
        public string Description { get; set; }
        [ForeignKey("Project")]
        [Required]
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        [ForeignKey("RuleConfiguration")]
        [Required]
        public int RuleConfigurationId { get; set; }
        public RuleConfiguration RuleConfiguration { get; set; }
        [Required]
        public string ParameterName { get; set; }

        #endregion

        #region [ Abstract Methods ]

        public abstract SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx, User user);

        public abstract SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx);

        #endregion
    }
}

