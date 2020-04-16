namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MobilePhoneNewFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PhoneSummaryActivities", "ReportDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PhoneSummaryActivities", "LineRentalInc", c => c.Int());
            AddColumn("dbo.PhoneSummaryActivities", "TotalUsageChargesInc", c => c.Int());
            AddColumn("dbo.PhoneSummaryActivities", "ChargeableMinutes", c => c.Int());
            AddColumn("dbo.PhoneSummaryActivities", "ChargeableDataKB", c => c.Int());
            AlterColumn("dbo.PhoneSummaryActivities", "TotalDuration", c => c.Int());
            AlterColumn("dbo.PhoneSummaryActivities", "DataVolumeKB", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PhoneSummaryActivities", "DataVolumeKB", c => c.Int());
            AlterColumn("dbo.PhoneSummaryActivities", "TotalDuration", c => c.String());
            DropColumn("dbo.PhoneSummaryActivities", "ChargeableDataKB");
            DropColumn("dbo.PhoneSummaryActivities", "ChargeableMinutes");
            DropColumn("dbo.PhoneSummaryActivities", "TotalUsageChargesInc");
            DropColumn("dbo.PhoneSummaryActivities", "LineRentalInc");
            DropColumn("dbo.PhoneSummaryActivities", "ReportDate");
        }
    }
}
