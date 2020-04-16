namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projecttypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Projects", "ProjectTypeId", c => c.Int());
            CreateIndex("dbo.Projects", "ProjectTypeId");
            AddForeignKey("dbo.Projects", "ProjectTypeId", "dbo.ProjectTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "ProjectTypeId", "dbo.ProjectTypes");
            DropIndex("dbo.Projects", new[] { "ProjectTypeId" });
            DropColumn("dbo.Projects", "ProjectTypeId");
            DropTable("dbo.ProjectTypes");
        }
    }
}
