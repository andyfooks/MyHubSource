namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingIsDeletedToEFObjects2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rules", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rules", "IsDeleted");
        }
    }
}
