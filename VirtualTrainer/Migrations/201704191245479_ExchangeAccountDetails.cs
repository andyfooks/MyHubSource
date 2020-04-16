namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExchangeAccountDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExchangeAccountDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(nullable: false),
                        Description = c.String(),
                        AutoDiscoverUserName = c.String(nullable: false),
                        AutoDiscoverUserPassword = c.String(nullable: false),
                        AutoDiscoverUserDomain = c.String(nullable: false),
                        AutoDiscoverEmail = c.String(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExchangeAccountDetails", "ProjectId", "dbo.Projects");
            DropIndex("dbo.ExchangeAccountDetails", new[] { "ProjectId" });
            DropTable("dbo.ExchangeAccountDetails");
        }
    }
}
