namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorksheetMonthActivity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkSheetActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeName = c.String(),
                        EmployeeID = c.String(),
                        Team = c.String(),
                        HighLevelTeam = c.String(),
                        ITActivityType = c.String(),
                        HighLevelITActivityType = c.String(),
                        ChangeStack = c.String(),
                        WorkItem_ProjectTaskType = c.String(),
                        ProjectTask = c.String(),
                        ExpenditureType = c.String(),
                        Application = c.String(),
                        BusinessUnit = c.String(),
                        ITActivity = c.Int(nullable: false),
                        Location = c.String(),
                        DateProcessed = c.DateTime(nullable: false),
                        Month = c.DateTime(nullable: false),
                        MonthInt = c.Int(nullable: false),
                        YearInt = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WorkSheetActivities");
        }
    }
}
