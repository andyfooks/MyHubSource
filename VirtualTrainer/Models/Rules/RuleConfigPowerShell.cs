using AJG.VirtualTrainer.Helper.Exchange;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace VirtualTrainer
{
    public class PowershellRuleConfig : RuleConfigurationBase
    {
        public override void PostProcessing(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
        }
        public override List<BreachLog> ExecuteRuleConfig(VirtualTrainerContext ctx, bool saveBreachesToDB)
        {
            throw new NotImplementedException();
        }
    }
}
