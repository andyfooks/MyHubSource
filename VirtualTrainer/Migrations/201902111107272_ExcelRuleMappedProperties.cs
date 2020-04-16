namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExcelRuleMappedProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RuleConfigurations", "hasHeaderRow", c => c.Boolean());
            AddColumn("dbo.RuleConfigurations", "sourceFileNameSavedInBreachFieldName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RuleConfigurations", "sourceFileNameSavedInBreachFieldName");
            DropColumn("dbo.RuleConfigurations", "hasHeaderRow");
        }
    }
}
