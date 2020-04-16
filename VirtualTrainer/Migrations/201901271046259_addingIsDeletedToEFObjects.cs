namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingIsDeletedToEFObjects : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.RuleConfigurations", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "IsDeleted");
            DropColumn("dbo.RuleConfigurations", "IsDeleted");
            DropColumn("dbo.Projects", "IsDeleted");
        }
    }
}
