namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printerSummaryActivityUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PrinterSummaryActivities", "TotalPages", c => c.Int(nullable: false));
            AlterColumn("dbo.PrinterSummaryActivities", "BWPages", c => c.Int(nullable: false));
            AlterColumn("dbo.PrinterSummaryActivities", "ColourPages", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PrinterSummaryActivities", "ColourPages", c => c.String());
            AlterColumn("dbo.PrinterSummaryActivities", "BWPages", c => c.String());
            AlterColumn("dbo.PrinterSummaryActivities", "TotalPages", c => c.String());
        }
    }
}
