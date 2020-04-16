namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newone : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EscalationsFrameworkRuleConfigs", "BreachInclusion", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EscalationsFrameworkRuleConfigs", "BreachInclusion", c => c.Int());
        }
    }
}
