namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printersummaryUPdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrinterSummaryActivities", "Month", c => c.String());
            AddColumn("dbo.PrinterSummaryActivities", "Year", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrinterSummaryActivities", "Year");
            DropColumn("dbo.PrinterSummaryActivities", "Month");
        }
    }
}
