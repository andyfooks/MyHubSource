namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printerSummaryAddReportDateField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrinterSummaryActivities", "ReportDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrinterSummaryActivities", "ReportDate");
        }
    }
}
