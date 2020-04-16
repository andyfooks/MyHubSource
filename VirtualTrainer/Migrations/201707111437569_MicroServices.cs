namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MicroServices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActionTakenLogs", "ActionName", c => c.String());
            AddColumn("dbo.ActionTakenLogs", "Authenticated", c => c.Boolean());
            AddColumn("dbo.ActionTakenLogs", "ActionParameters", c => c.String());
            AddColumn("dbo.ActionTakenLogs", "SessionId", c => c.String());
            AddColumn("dbo.Projects", "IsMicroServicesProject", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "IsMicroServicesProject");
            DropColumn("dbo.ActionTakenLogs", "SessionId");
            DropColumn("dbo.ActionTakenLogs", "ActionParameters");
            DropColumn("dbo.ActionTakenLogs", "Authenticated");
            DropColumn("dbo.ActionTakenLogs", "ActionName");
        }
    }
}
