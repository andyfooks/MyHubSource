namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeSheetData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeSheetDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Month = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        TimeSheetMonthYear = c.DateTime(),
                        UserName = c.String(),
                        EmployeeID = c.String(),
                        SamAccountName = c.String(),
                        TimeSheetTeamName = c.String(),
                        CascadingDropDownListOutOfDate = c.Boolean(nullable: false),
                        Submitted = c.Boolean(nullable: false),
                        SubmittedBy = c.String(),
                        SubmittedTimeStamp = c.DateTime(),
                        Approved = c.Boolean(nullable: false),
                        ApprovedBy = c.String(),
                        ApprovedTimeStamp = c.DateTime(),
                        UnApprovedBy = c.String(),
                        UnApprovedTimeStamp = c.DateTime(),
                        LastSavedTimeStamp = c.DateTime(),
                        LastSavedBy = c.String(),
                        TimeSheetDataJson = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TimeSheetDatas");
        }
    }
}
