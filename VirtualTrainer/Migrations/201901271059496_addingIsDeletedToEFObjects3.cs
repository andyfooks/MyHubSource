namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingIsDeletedToEFObjects3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EscalationsFrameworks", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EscalationsFrameworks", "IsDeleted");
        }
    }
}
