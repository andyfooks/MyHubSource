namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SavedBy = c.String(),
                        TimeStamp = c.DateTime(),
                        DisplayText = c.String(),
                        Description = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        UncheckedUsers = c.String(),
                        IncludeUncheckedUsers = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EmailTemplates");
        }
    }
}
