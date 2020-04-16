namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRazorTemplatePropertiestoProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "EmailRazorTemplateBodyPath", c => c.String());
            AddColumn("dbo.Projects", "EmailRazorTemplateSubjectPath", c => c.String());
            AddColumn("dbo.Projects", "EmailRazorTemplateAttachmentPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "EmailRazorTemplateAttachmentPath");
            DropColumn("dbo.Projects", "EmailRazorTemplateSubjectPath");
            DropColumn("dbo.Projects", "EmailRazorTemplateBodyPath");
        }
    }
}
