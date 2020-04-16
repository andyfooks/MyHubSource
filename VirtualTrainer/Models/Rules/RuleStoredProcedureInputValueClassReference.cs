using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    /// <summary>
    /// Used to reference a class property with IsRuleConfigurationParticipant property.
    /// During runtime this property value will be passed into the stored proc.
    /// </summary>
    public class RuleStoredProcedureInputValueClassReference : RuleStoredProcedureInputValue
    {
        #region [ EF Properties ]

        public string ClassName { get; set; }
        public string ClassPropertyName { get; set; }

        #endregion

        #region [ override methods ]

        public override SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx, User ruleParticipant)
        {
            return new SqlParameter(this.ParameterName, GetPropertyValueFromClassReference(ruleParticipant));
        }

        public override SqlParameter GetSqlParameterForRuleParticipant(VirtualTrainerContext ctx)
        {
            throw new NotImplementedException("This method has not yet been implmented.");
        }

        #endregion

        #region [ Private Methods ]

        public string GetPropertyValueFromClassReference(User ruleParticipant)
        {
            string returnValue = string.Empty;

            // Get All property values for this rule
            List<RuleConfigurationObject> rulePropertyValues = RuleConfiguration.GetAllConfigProps();

            // get all property values for this user
            List<RuleConfigurationObject> ruleUserPropertyValues = ruleParticipant.GetAllConfigProps();
            rulePropertyValues.AddRange(ruleUserPropertyValues);
            var linqRulePropertyValues = rulePropertyValues.Where(a => a.ClassName == this.ClassName).Where(a => a.ClassPropertyName == this.ClassPropertyName);

            // Itearate over Linq result set and take first non null contextual value.
            foreach (var rulePropertyValue in linqRulePropertyValues)
            {
                if (!string.IsNullOrEmpty(rulePropertyValue.ContextValue))
                {
                    return rulePropertyValue.ContextValue;
                }
            }
            return returnValue;
        }

        #endregion
    }
}
