namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GenericEmailEscalationsNewPDFOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFHeaderHtml", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFFooterHtml", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFHeaderMargins", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFFooterMargins", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "PdfMargins", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "PdfMargins");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFFooterMargins");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFHeaderMargins");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFFooterHtml");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "PDFHeaderHtml");
        }
    }
}
