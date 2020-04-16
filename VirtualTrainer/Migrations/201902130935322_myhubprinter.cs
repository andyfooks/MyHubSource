namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class myhubprinter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrinterSummaryActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountID = c.String(),
                        AccountIDBillable = c.Boolean(nullable: false),
                        AccountName = c.String(),
                        AccountDescription = c.String(),
                        PrinterName = c.String(),
                        PrinterDescription = c.String(),
                        PrinterID = c.String(),
                        Department = c.String(),
                        TotalPages = c.String(),
                        BWPages = c.String(),
                        ColourPages = c.String(),
                        Amount = c.String(),
                        AltCost = c.String(),
                        Jobs = c.String(),
                        Billable = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PrinterSummaryActivities");
        }
    }
}
