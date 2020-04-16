namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SQLEscalation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "SqlQuery", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "IsStoredProcedure", c => c.Boolean());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "TargetDbID", c => c.Int());
            CreateIndex("dbo.EscalationsFrameworkRuleConfigs", "TargetDbID");
            AddForeignKey("dbo.EscalationsFrameworkRuleConfigs", "TargetDbID", "dbo.TargetDatabaseDetails", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EscalationsFrameworkRuleConfigs", "TargetDbID", "dbo.TargetDatabaseDetails");
            DropIndex("dbo.EscalationsFrameworkRuleConfigs", new[] { "TargetDbID" });
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "TargetDbID");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "IsStoredProcedure");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "SqlQuery");
        }
    }
}
