namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhoneSummaryModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PhoneSummaryActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Month = c.String(),
                        Year = c.String(),
                        UserName = c.String(),
                        PhoneNumber = c.String(),
                        EmployeeID = c.String(),
                        CostCentreID = c.String(),
                        NumberOfCalls = c.Int(),
                        TotalDuration = c.String(),
                        DataVolumeKB = c.Int(),
                        TotalCost = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PhoneSummaryActivities");
        }
    }
}
