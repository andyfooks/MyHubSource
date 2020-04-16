namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectEmailFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "ProjectSenderEmail", c => c.String());
            AddColumn("dbo.Projects", "ProjectSenderDisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "ProjectSenderDisplayName");
            DropColumn("dbo.Projects", "ProjectSenderEmail");
        }
    }
}
