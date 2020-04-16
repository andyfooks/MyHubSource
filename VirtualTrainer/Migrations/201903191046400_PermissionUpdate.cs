namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PermissionUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permission", "Info", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Permission", "Info");
        }
    }
}
