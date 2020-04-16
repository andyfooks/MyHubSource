namespace VirtualTrainer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class phaseTwoChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActionTakenLogs", "RuleConfigurationId", "dbo.RuleConfigurations");
            DropForeignKey("dbo.RuleConfigurations", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.BreachLogs", "UserId", "dbo.Users");
            DropForeignKey("dbo.BreachLogs", "DatabaseDetailsId", "dbo.TargetDatabaseDetails");
            DropIndex("dbo.RuleConfigurations", new[] { "SetBreachesToResolvedScheduleId" });
            DropIndex("dbo.RuleConfigurations", new[] { "TargetDbID" });
            DropIndex("dbo.BreachLogs", new[] { "UserId" });
            DropIndex("dbo.BreachLogs", new[] { "DatabaseDetailsId" });
            CreateTable(
                "dbo.ExcelRuleConfigBreachFieldMappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        SqlQueryResultColumnName = c.String(),
                        MappedToBreachTableColumnName = c.String(),
                        ExcelRuleConfigId = c.Int(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.RuleConfigurations", t => t.ExcelRuleConfigId, cascadeDelete: false)
                .Index(t => t.ExcelRuleConfigId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.ExchangeEmailRuleConfigBreachFieldMappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        EmailSearchSource = c.Int(),
                        EmailMessgaeSearchType = c.Int(),
                        SearchText = c.String(),
                        MappedToBreachTableColumnName = c.String(),
                        AttachmentNameSearchFilter = c.Int(),
                        AttachmentNameSearchString = c.String(),
                        AttachmentDocumentType = c.Int(),
                        ExchangeRuleConfigId = c.Int(nullable: false),
                        ProjectId = c.Guid(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .ForeignKey("dbo.RuleConfigurations", t => t.ExchangeRuleConfigId, cascadeDelete: true)
                .Index(t => t.ExchangeRuleConfigId)
                .Index(t => t.ProjectId);
            
            AddColumn("dbo.ActionTakenLogs", "RuleConfigurationBase_Id", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "SetExistingBreachesToArchivedBeforeExecute", c => c.Boolean());
            AddColumn("dbo.RuleConfigurations", "PreExecuteBreachesAction", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "SourceDocFolderPath", c => c.String());
            AddColumn("dbo.RuleConfigurations", "SourceDocSelectionType", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "SourceDocNameSearchText", c => c.String());
            AddColumn("dbo.RuleConfigurations", "SQLQuery", c => c.String());
            AddColumn("dbo.RuleConfigurations", "DocumentDeleteMode", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "MoveDocumentDestinationDirectory", c => c.String());
            AddColumn("dbo.RuleConfigurations", "AppendTodaysDateToMovedDocName", c => c.Boolean());
            AddColumn("dbo.RuleConfigurations", "AutoMapResultsToBreachTableFields", c => c.Boolean());
            AddColumn("dbo.RuleConfigurations", "ExchangeAccountDetailsId", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ExchangeItemDeleteMode", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "SentFromFilter", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "SentFromSearchText", c => c.String());
            AddColumn("dbo.RuleConfigurations", "OperatorForSentFromAndSubject", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "SubjectFilter", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "SubjectSearchText", c => c.String());
            AddColumn("dbo.RuleConfigurations", "OperatorForSubjectAndDate", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ReceivedDate", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ReceivedDateUseDateOnly", c => c.Boolean());
            AddColumn("dbo.RuleConfigurations", "ReceivedDateOneFilter", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ReceivedDateOneOffset", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ReceivedDateOneOffsetPeriod", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ReceivedDateTwoFilter", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ReceivedDateTwoOffset", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "ReceivedDateTwoOffsetPeriod", c => c.Int());
            AddColumn("dbo.RuleConfigurations", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "EmailAttachementTemplate", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "CreateAttachementUsingTemplate", c => c.Boolean());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "Action", c => c.Int());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "PdfPageWidth", c => c.Int());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "PdfPageHeight", c => c.Int());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "DestinationPath", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "EmailBreachColumnName", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "EmailAddress", c => c.String());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "BreachInclusion", c => c.Int());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "ArchiveBreachesOnSuccess", c => c.Boolean());
            AddColumn("dbo.EscalationsFrameworkRuleConfigs", "GroupBreachesColumnName", c => c.String());
            AlterColumn("dbo.RuleConfigurations", "SqlCommandText", c => c.String());
            AlterColumn("dbo.RuleConfigurations", "SqlCommandTextIsStoredProc", c => c.Boolean());
            AlterColumn("dbo.RuleConfigurations", "SetBreachesToResolvedScheduleId", c => c.Int());
            AlterColumn("dbo.RuleConfigurations", "TargetDbID", c => c.Int());
            AlterColumn("dbo.RuleConfigurations", "UserTargetExecutionMode", c => c.Int());
            AlterColumn("dbo.RuleConfigurations", "RuleTarget", c => c.Int());
            AlterColumn("dbo.BreachLogs", "UserId", c => c.Int());
            AlterColumn("dbo.BreachLogs", "DatabaseDetailsId", c => c.Int());
            CreateIndex("dbo.ActionTakenLogs", "RuleConfigurationBase_Id");
            CreateIndex("dbo.RuleConfigurations", "SetBreachesToResolvedScheduleId");
            CreateIndex("dbo.RuleConfigurations", "TargetDbID");
            CreateIndex("dbo.RuleConfigurations", "ExchangeAccountDetailsId");
            CreateIndex("dbo.BreachLogs", "UserId");
            CreateIndex("dbo.BreachLogs", "DatabaseDetailsId");
            AddForeignKey("dbo.RuleConfigurations", "ExchangeAccountDetailsId", "dbo.ExchangeAccountDetails", "Id", cascadeDelete: false);
            AddForeignKey("dbo.ActionTakenLogs", "RuleConfigurationBase_Id", "dbo.RuleConfigurations", "Id");
            AddForeignKey("dbo.RuleConfigurations", "ScheduleId", "dbo.Schedules", "Id", cascadeDelete: false);
            AddForeignKey("dbo.BreachLogs", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.BreachLogs", "DatabaseDetailsId", "dbo.TargetDatabaseDetails", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BreachLogs", "DatabaseDetailsId", "dbo.TargetDatabaseDetails");
            DropForeignKey("dbo.BreachLogs", "UserId", "dbo.Users");
            DropForeignKey("dbo.RuleConfigurations", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.ActionTakenLogs", "RuleConfigurationBase_Id", "dbo.RuleConfigurations");
            DropForeignKey("dbo.RuleConfigurations", "ExchangeAccountDetailsId", "dbo.ExchangeAccountDetails");
            DropForeignKey("dbo.ExchangeEmailRuleConfigBreachFieldMappings", "ExchangeRuleConfigId", "dbo.RuleConfigurations");
            DropForeignKey("dbo.ExchangeEmailRuleConfigBreachFieldMappings", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ExcelRuleConfigBreachFieldMappings", "ExcelRuleConfigId", "dbo.RuleConfigurations");
            DropForeignKey("dbo.ExcelRuleConfigBreachFieldMappings", "ProjectId", "dbo.Projects");
            DropIndex("dbo.ExchangeEmailRuleConfigBreachFieldMappings", new[] { "ProjectId" });
            DropIndex("dbo.ExchangeEmailRuleConfigBreachFieldMappings", new[] { "ExchangeRuleConfigId" });
            DropIndex("dbo.ExcelRuleConfigBreachFieldMappings", new[] { "ProjectId" });
            DropIndex("dbo.ExcelRuleConfigBreachFieldMappings", new[] { "ExcelRuleConfigId" });
            DropIndex("dbo.BreachLogs", new[] { "DatabaseDetailsId" });
            DropIndex("dbo.BreachLogs", new[] { "UserId" });
            DropIndex("dbo.RuleConfigurations", new[] { "ExchangeAccountDetailsId" });
            DropIndex("dbo.RuleConfigurations", new[] { "TargetDbID" });
            DropIndex("dbo.RuleConfigurations", new[] { "SetBreachesToResolvedScheduleId" });
            DropIndex("dbo.ActionTakenLogs", new[] { "RuleConfigurationBase_Id" });
            AlterColumn("dbo.BreachLogs", "DatabaseDetailsId", c => c.Int(nullable: false));
            AlterColumn("dbo.BreachLogs", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.RuleConfigurations", "RuleTarget", c => c.Int(nullable: false));
            AlterColumn("dbo.RuleConfigurations", "UserTargetExecutionMode", c => c.Int(nullable: false));
            AlterColumn("dbo.RuleConfigurations", "TargetDbID", c => c.Int(nullable: false));
            AlterColumn("dbo.RuleConfigurations", "SetBreachesToResolvedScheduleId", c => c.Int(nullable: false));
            AlterColumn("dbo.RuleConfigurations", "SqlCommandTextIsStoredProc", c => c.Boolean(nullable: false));
            AlterColumn("dbo.RuleConfigurations", "SqlCommandText", c => c.String(nullable: false));
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "GroupBreachesColumnName");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "ArchiveBreachesOnSuccess");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "BreachInclusion");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "EmailAddress");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "EmailBreachColumnName");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "DestinationPath");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "PdfPageHeight");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "PdfPageWidth");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "Action");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "CreateAttachementUsingTemplate");
            DropColumn("dbo.EscalationsFrameworkRuleConfigs", "EmailAttachementTemplate");
            DropColumn("dbo.RuleConfigurations", "Discriminator");
            DropColumn("dbo.RuleConfigurations", "ReceivedDateTwoOffsetPeriod");
            DropColumn("dbo.RuleConfigurations", "ReceivedDateTwoOffset");
            DropColumn("dbo.RuleConfigurations", "ReceivedDateTwoFilter");
            DropColumn("dbo.RuleConfigurations", "ReceivedDateOneOffsetPeriod");
            DropColumn("dbo.RuleConfigurations", "ReceivedDateOneOffset");
            DropColumn("dbo.RuleConfigurations", "ReceivedDateOneFilter");
            DropColumn("dbo.RuleConfigurations", "ReceivedDateUseDateOnly");
            DropColumn("dbo.RuleConfigurations", "ReceivedDate");
            DropColumn("dbo.RuleConfigurations", "OperatorForSubjectAndDate");
            DropColumn("dbo.RuleConfigurations", "SubjectSearchText");
            DropColumn("dbo.RuleConfigurations", "SubjectFilter");
            DropColumn("dbo.RuleConfigurations", "OperatorForSentFromAndSubject");
            DropColumn("dbo.RuleConfigurations", "SentFromSearchText");
            DropColumn("dbo.RuleConfigurations", "SentFromFilter");
            DropColumn("dbo.RuleConfigurations", "ExchangeItemDeleteMode");
            DropColumn("dbo.RuleConfigurations", "ExchangeAccountDetailsId");
            DropColumn("dbo.RuleConfigurations", "AutoMapResultsToBreachTableFields");
            DropColumn("dbo.RuleConfigurations", "AppendTodaysDateToMovedDocName");
            DropColumn("dbo.RuleConfigurations", "MoveDocumentDestinationDirectory");
            DropColumn("dbo.RuleConfigurations", "DocumentDeleteMode");
            DropColumn("dbo.RuleConfigurations", "SQLQuery");
            DropColumn("dbo.RuleConfigurations", "SourceDocNameSearchText");
            DropColumn("dbo.RuleConfigurations", "SourceDocSelectionType");
            DropColumn("dbo.RuleConfigurations", "SourceDocFolderPath");
            DropColumn("dbo.RuleConfigurations", "PreExecuteBreachesAction");
            DropColumn("dbo.RuleConfigurations", "SetExistingBreachesToArchivedBeforeExecute");
            DropColumn("dbo.ActionTakenLogs", "RuleConfigurationBase_Id");
            DropTable("dbo.ExchangeEmailRuleConfigBreachFieldMappings");
            DropTable("dbo.ExcelRuleConfigBreachFieldMappings");
            CreateIndex("dbo.BreachLogs", "DatabaseDetailsId");
            CreateIndex("dbo.BreachLogs", "UserId");
            CreateIndex("dbo.RuleConfigurations", "TargetDbID");
            CreateIndex("dbo.RuleConfigurations", "SetBreachesToResolvedScheduleId");
            AddForeignKey("dbo.BreachLogs", "DatabaseDetailsId", "dbo.TargetDatabaseDetails", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BreachLogs", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.RuleConfigurations", "ScheduleId", "dbo.Schedules", "Id");
            AddForeignKey("dbo.ActionTakenLogs", "RuleConfigurationId", "dbo.RuleConfigurations", "Id");
        }
    }
}
