(function ($, pub, utils) {
    'use strict'

    var pageVarsPri = {};
    var currentEscalationsGridID = "";
    
    pub.mouseOver = function (message) {
        alert(message);
    }
    pub.pageVarsPri = function () {
        return  pageVarsPri;
    }
    pub.init = function (pageVars) {
        pageVarsPri = pageVars;
        loadProjectsGrid();
    }
    pub.clearInputFieldVal = function(a)
    {
        $(a).val('');
    }
    pub.LoadGridForTabClick = function (targetGridId) {
        var selectedProjectUniqueKey = getSelectedProject();
        if (targetGridId == pageVarsPri.tags.projectTeamsGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadProjectTeamsGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectOfficesGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadProjectOfficesGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectUsersGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadProjectUsersGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectRegionsGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadProjectRegionsGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectRulesGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadProjectRulesGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectDbDetailsGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadProjectDbDetailsGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectSchedulesGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadProjectSchedulesGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRolesGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadprojectEscalationsConfigRolesGrid(selectedProjectUniqueKey);
            loadprojectEscalationsConfigUsersGrid(selectedProjectUniqueKey);
            loadprojectEscalationsConfigGenericGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadprojectEscalationsConfigUsersGrid(selectedProjectUniqueKey);
            loadprojectEscalationsConfigRolesGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigGenericGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadprojectEscalationsConfigGenericGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigGSQLGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadprojectEscalationsConfigSQLGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectExclusionsGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadprojectExclusionsGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.projectActurisImportsGridId) {
            utils.jumpToElement("#ProjectDetailsLevel1");
            $(pageVarsPri.tags.level3TabStripClass).hide();
            $(pageVarsPri.tags.level4TabStripClass).hide();
            loadprojectActurisImportsGrid(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.OfficeTeamsGridId) {
            var officeId = getSelectedOfficeId();
            loadProjectOfficeTeamsGrid(officeId);
        }
        else if (targetGridId == pageVarsPri.tags.OfficeUsersGridId) {
            var officeId = getSelectedOfficeId();
            loadOfficeUsersGrid(officeId);
        }
        else if (targetGridId == pageVarsPri.tags.TeamUsersGridId) {
            var teamId = getSelectedTeamId();
            loadTeamUsersGrid(teamId);
        }
        else if (targetGridId == pageVarsPri.tags.ruleConfigProjectParticipantsGridId) {
            var ruleConfigId = getSelectedRuleConfigId();
            loadRuleConfigProjectParticipantsGrid(ruleConfigId);
        }
        else if (targetGridId == pageVarsPri.tags.ruleExchangeConfigurationsGridId) {
            var ruleId = getSelectedRuleId();
            loadruleExchangeConfigurationsGrid(ruleId);
        }
        else if (targetGridId == pageVarsPri.tags.ruleExcelConfigurationsGridId) {
            var ruleId = getSelectedRuleId();
            loadruleExcelConfigurationsGrid(ruleId);
        }
        else if (targetGridId == pageVarsPri.tags.ruleConfigOfficeParticipantsGridId) {
            var ruleConfigId = getSelectedRuleConfigId();
            loadRuleConfigOfficeParticipantsGrid(ruleConfigId);
        }
        else if (targetGridId == pageVarsPri.tags.ruleConfigTeamParticipantsGridId) {
            var ruleConfigId = getSelectedRuleConfigId();
            loadRuleConfigTeamParticipantsGrid(ruleConfigId);
        }
        else if (targetGridId == pageVarsPri.tags.ruleConfigUserParticipantsGridId) {
            var ruleConfigId = getSelectedRuleConfigId();
            loadRuleConfigUserParticipantsGrid(ruleConfigId);
        }
        //else if (targetGridId == pageVarsPri.tags.ruleSqlClassRefernceInputGridId) {
        //    var ruleConfigId = getSelectedRuleConfigId();
        //    loadruleSqlHardCodedInputGrid(ruleConfigId);
        //    loadruleSqlExclusionsGroupInputGrid(ruleConfigId);
        //    loadruleSqlClassRefernceInputGrid(ruleConfigId);
        //}
        else if (targetGridId == pageVarsPri.tags.ruleSqlExclusionsInputGridId) {
            var ruleConfigId = getSelectedRuleConfigId();
            loadruleSqlHardCodedInputGrid(ruleConfigId);
            loadruleSqlExclusionsGroupInputGrid(ruleConfigId);
        }
        else if (targetGridId == pageVarsPri.tags.ruleSqlHardCodedInputGridId) {
            var ruleConfigId = getSelectedRuleConfigId();
            loadruleSqlHardCodedInputGrid(ruleConfigId);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleGridId) {
            var escConfig = getSelectedRoleEscalationsConfigId();
            loadprojectEscalationsConfigRolesRuleBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRoleEscalationsHistoryPanelContentId) {    
            var escConfig = getSelectedRoleEscalationsConfigId();
            loadprojectEscalationsConfigEscalationsHistoryPartialView(escConfig, pageVarsPri.tags.projectEscalationConfigRoleEscalationsHistoryPanelContentId);
            $(pageVarsPri.tags.projectEscalationConfigRoleEscalationsHistoryPanelContentId).show();
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleConfigGridId) {
            var escConfig = getSelectedRoleEscalationsConfigId();
            loadprojectEscalationsConfigRolesRuleConfigBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRoleBreachSourceProjectGridId) {
            var escConfig = getSelectedRoleEscalationsConfigId();
            loadprojectEscalationsConfigRolesProjectBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRoleBreachSourceOfficeGridId) {
            var escConfig = getSelectedRoleEscalationsConfigId();
            loadprojectEscalationsConfigRolesOfficeBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRoleBreachSourceTeamGridId) {
            var escConfig = getSelectedRoleEscalationsConfigId();
            loadprojectEscalationsConfigRolesTeamBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigRoleBreachSourceUserGridId) {
            var escConfig = getSelectedRoleEscalationsConfigId();
            loadprojectEscalationsConfigRolesUserBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigUsersRuleBreachSourcesGrid(escConfig);
            loadprojectEscalationsConfigUsersRuleConfigBreachSourcesGrid(escConfig);
            loadprojectEscalationsConfigUsersProjectBreachSourcesGrid(escConfig);
            loadprojectEscalationsConfigUsersOfficeBreachSourcesGrid(escConfig);
            loadprojectEscalationsConfigUsersTeamBreachSourcesGrid(escConfig);
            loadprojectEscalationsConfigUsersUserBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigGenericBreachSourceRuleGridId) {
            var escConfig = getSelectedGenericEscalationsConfigId(pageVarsPri.tags.projectEscalationConfigGenericBreachSourceRuleGridId);
            currentEscalationsGridID = pageVarsPri.tags.projectEscalationConfigGenericGridId;
            loadprojectEscalationsConfigGenericRuleBreachSourcesGrid(escConfig, pageVarsPri.tags.projectEscalationConfigGenericBreachSourceRuleGridId);
            //loadprojectEscalationsConfigGenericRuleConfigBreachSourcesGrid(escConfig, pageVarsPri.tags.projectEscalationConfigGenericBreachSourceRuleGridId);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigSQLBreachSourceRuleGridId) {
            var escConfig = getSelectedGenericEscalationsConfigId(pageVarsPri.tags.projectEscalationConfigSQLBreachSourceRuleGridId);
            currentEscalationsGridID = pageVarsPri.tags.projectEscalationConfigGSQLGridId;
            loadprojectEscalationsConfigGenericRuleBreachSourcesGrid(escConfig, pageVarsPri.tags.projectEscalationConfigSQLBreachSourceRuleGridId);
            //loadprojectEscalationsConfigGenericRuleConfigBreachSourcesGrid(escConfig, pageVarsPri.tags.projectEscalationConfigSQLBreachSourceRuleGridId);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUserEscalationsHistoryGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigEscalationsHistoryPartialView(escConfig, pageVarsPri.tags.projectEscalationConfigUserEscalationsHistoryPanelContentId)
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleConfigGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigUsersRuleConfigBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersBreachSourceProjectGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigUsersProjectBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersBreachSourceOfficeGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigUsersOfficeBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersBreachSourceTeamGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigUsersTeamBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersBreachSourceUserGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigUsersUserBreachSourcesGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectEscalationConfigUsersRecipientsGridId) {
            var escConfig = getSelectedUserEscalationsConfigId();
            loadprojectEscalationsConfigUsersRecipientsGrid(escConfig);
        }
        else if (targetGridId == pageVarsPri.tags.projectBreachesGridId) {
            loadProjectsBreaches(selectedProjectUniqueKey);
        }
        else if (targetGridId == pageVarsPri.tags.userofficemembershipsGridId) {
            var userId = getSelectedProjectUser();
            loadUsersOfficeMemberships(userId);
        }
        else if (targetGridId == pageVarsPri.tags.userteammembershipsGridId) {
            var userId = getSelectedProjectUser();
            loadUsersTeamMemberships(userId);
        }
        else if (targetGridId == pageVarsPri.tags.projectExchangeAccountsGridId) {
            var selectedProjectUniqueKey = getSelectedProject();
            loadprojectExchangeAccountsGrid(selectedProjectUniqueKey);
        }
    }
    function getSelectedProject() {
        var grid = $(pageVarsPri.tags.projectsGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.ProjectUniqueKey;
            }
        }
    }
    function getSelectedOfficeId() {
        var grid = $(pageVarsPri.tags.projectOfficesGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedProjectUser() {
        var grid = $(pageVarsPri.tags.projectUsersGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedTeamId() {
        var grid = $(pageVarsPri.tags.projectTeamsGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedRuleId() {
        var grid = $(pageVarsPri.tags.projectRulesGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedRuleConfigId() {
        var grid = $(pageVarsPri.tags.ruleConfigurationsGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedRuleConfigExchangeId() {
        var grid = $(pageVarsPri.tags.ruleExchangeConfigurationsGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedRuleConfigExcelId() {
        var grid = $(pageVarsPri.tags.ruleExcelConfigurationsGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedRuleConfigExchangeExtractionItemId() {
        var grid = $(pageVarsPri.tags.ruleExchangeConfigExtractionItemsTableId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedOfficeParticipantId() {
        var grid = $(pageVarsPri.tags.ruleConfigOfficeParticipantsGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedUserEscalationsConfigId() {
        var grid = $(pageVarsPri.tags.projectEscalationConfigUsersGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    
    function getSelectedGenericEscalationsConfigId(GridID) {
        var grid = $(GridID).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedRoleEscalationsConfigId() {
        var grid = $(pageVarsPri.tags.projectEscalationConfigRolesGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function getSelectedSystemUserId() {
        var grid = $(pageVarsPri.tags.systemUsersGridId).data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function EscalationConfigGridDropDownEditorEmailBodyTemplate (container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId();
        $('<input required Name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "FileName",
                dataValueField: "FileName",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/GetProjectBodyRazorTemplateNames?projectId=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    
    function EscalationConfigGridDropDownEditorEmailAttachmentTemplate(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId();
        $('<input Name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "FileName",
                dataValueField: "FileName",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/GetProjectAttachmentRazorTemplateNames?projectId=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    function EscalationConfigGridDropDownEditorEmailSubjectTemplate(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId();
        $('<input required Name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "FileName",
                dataValueField: "FileName",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/GetProjectSubjectRazorTemplateNames?projectId=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorProjectParticipants(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "ProjectName",
                dataValueField: "ProjectUniqueKey",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectsForRuleParticiantsDropDown?projectId=' + selectedProjectUniqueKey + '&ruleConfigId=' + ruleConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFUserProjectBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedUserEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "ProjectName",
                dataValueField: "ProjectUniqueKey",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFProjectBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFUserOfficeBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedUserEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFPOfficeBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectExchangeAccountsDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "DisplayName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectExchangeAccounts?projectId=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFRoleOfficeBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedRoleEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFPOfficeBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFRoleRuleConfigBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var EscalationsConfigId = getSelectedRoleEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFRuleConfigBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + EscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFUserRuleConfigBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var EscalationsConfigId = getSelectedUserEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFRuleConfigBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + EscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFUserRuleBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var EscalationsConfigId = getSelectedUserEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFRuleBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + EscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFGenericRuleBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var EscalationsConfigId = getSelectedGenericEscalationsConfigId(currentEscalationsGridID);
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFRuleBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + EscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFGenericRuleConfigBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var EscalationsConfigId = getSelectedGenericEscalationsConfigId(currentEscalationsGridID);
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFRuleConfigBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + EscalationsConfigId,
                        cache: false
                    }
                }
            });
    }


    function ProjectDropDownEditorEFRoleRuleBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var EscalationsConfigId = getSelectedRoleEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFRuleBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + EscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFRoleTeamBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedRoleEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFTeamBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFRoleUserBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedRoleEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFUserBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFUserUserBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedUserEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFUserBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFUserTeamBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedUserEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFTeamBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorEFRoleProjectBreachSource(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var userEscalationsConfigId = getSelectedRoleEscalationsConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "ProjectName",
                dataValueField: "ProjectUniqueKey",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectDropDownEditorEFProjectBreachSource?projectId=' + selectedProjectUniqueKey + '&escalationsConfigId=' + userEscalationsConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorUserParticipants(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListUsersForRuleParticiantsDropDown?projectId=' + selectedProjectUniqueKey + '&ruleConfigId=' + ruleConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorTeamParticipants(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListTeamForRuleParticiantsDropDown?projectId=' + selectedProjectUniqueKey + '&ruleConfigId=' + ruleConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorOfficeParticipants(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId();
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "NameWithActurisOrgName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListOfficeForRuleParticiantsDropDown?projectId=' + selectedProjectUniqueKey + '&ruleConfigId=' + ruleConfigId,
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorScheduleFrequencies(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListScheduleFrequencies',
                        cache: false
                    }
                }
            });
    }
    function ProjectTypeDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectTypes',
                        cache: false
                    }
                }
            });
    }
    function ProjectDropDownEditorSystemUserProjectMembership(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedSystemUserId = getSelectedSystemUserId();
        
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "ProjectName",
                dataValueField: "ProjectUniqueKey",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectsForSystemUserProjectMembershipDropDown?SystemUserID=' + selectedSystemUserId,
                        cache: false
                    }
                }
            });
    }
    function UserDropDownEditorTeam(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var selecteTeamId = getSelectedTeamId();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/listUsersForTeamUsersDropDown?projectId=' + selectedProjectUniqueKey + '&teamId=' + selecteTeamId,
                        cache: false
                    }
                }
            });
    }
    function ActionDropDownEditorEscalationConfigRoles(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListActionsForEscalationConfigRolesDropDown',
                        cache: false
                    }
                }
            });
    }
    function UserDropDownEditorRecipients(container, options) {
        var selectedConfigID = getSelectedUserEscalationsConfigId();
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListUsersForEscalationsConfigParticipantsDropDown?projectId=' + selectedProjectUniqueKey + '&selectedConfigId=' + selectedConfigID,
                        cache: false
                    }
                }
            });
    }
    function ScheduleDropDownEditorRuleConfig(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListSchedulesForRuleConfigDropDown?projectId=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    function OfficeRegionDropDownEditor(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListRegionsForOfficeRegionsDropDown?projectId=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    function TargetDbDropDownEditorRuleConfig(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "DisplayName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListTargetDbsForRuleConfigDropDown?projectId=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    function UserDropDownEditorOffice(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var selecteOfficeId = getSelectedOfficeId();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/listProjectUsersForOfficeUsersDropDown?projectId=' + selectedProjectUniqueKey + '&officeId=' + selecteOfficeId,
                        cache: false
                    }
                }
            });
    }
    function officeDropDownEditor(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListProjectOffices?projectGuid=' + selectedProjectUniqueKey,
                        cache: false
                    }
                }
            });
    }
    function RuleConfigSqlClassRefInputListDropDownEditor(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
               //// dataTextField: "Name",
               // dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/RuleConfigSqlClassRefInputList',
                        cache: false
                    }
                }
            });
    }
    function RuleConfigExclusionsGroupInputListDropDownEditor(container, options) {
        var selectedProjectUniqueKey = getSelectedProject();
        var ruleConfigId = getSelectedRuleConfigId(); 
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "GroupName",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListRuleConfigExclusionsGroupRefInputList?projectId=' + selectedProjectUniqueKey + '&ruleConfigId=' + ruleConfigId,
                        cache: false
                    }
                }
            });
    }
    
    function EmailMessageAttachmentDocTypeDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/EmailMessageAttachmentDocTypeDropDownFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function EmailMessgaeSearchTypeDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListEmailMessgaeSearchTypeDropDownFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function EmailSearchSourceDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListEmailSearchSourceDropDownFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function ExchangeItemDateOffsetDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExchangeItemDateOffsetFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function ExchangeItemReceivedDateFilterDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExchangeItemReceivedDateFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function ExchangeItemSubjectFilterDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExchangeItemSubjectFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function ExchangeItemReceivedDateDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExchangeItemReceivedDateDateFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function ExchangeRuleConfigBreachTableColumnsDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var exchangeRuleConfigId = getSelectedRuleConfigExchangeId();
        var currentExtractionItemId = getSelectedRuleConfigExchangeExtractionItemId();
        currentExtractionItemId = currentExtractionItemId == null ? "-1" : currentExtractionItemId;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Name",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExchangeRuleConfigBreachTableColumnsFilterOptions?exchangeRuleConfigId=' + exchangeRuleConfigId + '&currentExtractionConfigId=' + currentExtractionItemId,
                        cache: false
                    }
                }
            });
    }
    function ExchangeItemOperatorDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExchangeItemOperatorFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function StringFilterDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListStringFilterOptions',
                        cache: false
                    }
                }
            });
    }
    function ExcelSourceDocSelectionTypeDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExcelDocSelectionTypeModeOptions',
                        cache: false
                    }
                }
            });
    }
    function RuleConfigPreExecuteBreachesActionSelectionTypeDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListRuleConfigPreExecuteBreachesActionOptions',
                        cache: false
                    }
                }
            });
    }
    function BreachFieldDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                //dataTextField: "Name",
                //dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/GetBreachFields',
                        cache: false
                    }
                }
            });
    }
    function EmailActionDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListEmailActionOptions',
                        cache: false
                    }
                }
            });
    }
    function BreachInclusionDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListBreachInclusionOptions',
                        cache: false
                    }
                }
            });
    }
    function ExcelDocumentDeleteModeDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExcelDocDeleteModeOptions',
                        cache: false
                    }
                }
            });
    }
    function ExchangeItemDeleteModesDropDownEditor(container, options) {
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: true,
                dataTextField: "Name",
                dataValueField: "Id",
                dataSource: {
                    type: "json",
                    transport: {
                        read: crudServiceBaseUrl + '/ListExchangeItemDeleteModeOptions',
                        cache: false
                    }
                }
            });
    }
    pub.LoadSystemUsersGrid = function () {
        loadSystemUsersGrid();
    }
    pub.LoadAllSystemUsersGrid = function (gridId, dataRead, dataUpdate, dataDestroy, dataCreate) {
        loadAllSystemUsersGrid(gridId, dataRead, dataUpdate, dataDestroy, dataCreate);
        $("#systemuserspermissionstabstrip .nav-tabs, #systemuserspermissionstabstrip .tab-content").hide();
    }
    pub.LoadAllSystemLogsGrid = function () {
        LoadAllSystemLogsGrid();
    }

    function LoadAllSystemLogsGrid() {
        if ($(pageVarsPri.tags.systemLogsGridId).kendoGrid()) {
            $(pageVarsPri.tags.systemLogsGridId).kendoGrid('destroy').empty();
        }

        $(pageVarsPri.tags.systemLogErrorMessageId).html('');
        $(pageVarsPri.tags.systemLogStackTraceId).html('');

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/" + 'ListSystemLogs',
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            serverSorting: true,
            serverFiltering: true,
            serverGrouping: true,
            serverPaging: true,
            batch: false,
            pageSize: 100,
            schema: {
                data: "data",
                total: "total",
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        MachineName: { validation: { required: true } },
                        UserName: { validation: { required: false } },
                        TimeStamp: {  type: "date", editable: false, validation: { required: false } },
                        Level: { validation: { required: true } },
                        ErrorMessage: { validation: { required: false } },
                        //StackTrace: { validation: { required: false } },
                        ProjectName: { validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.systemLogsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            resizable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            columns: [
                { field: "ProjectName", title: "Project Name", width: "120px" },
                { field: "MachineName", title: "Machine Name", width: "80px" },
                { field: "UserName", title: "User Name", width: "80px" },
                { field: "TimeStamp", title: "Time Stamp", width: "60px", format: "{0:dd-MMM-yyyy hh:mm}" },
                {
                    field: "Level",
                    width: "40px",
                    headerTemplate: '<span style="" title="0 = Information, 1 = Error">Level (i)</span>'
                },
                { field: "ErrorMessage", title: "Error Message", width: "300px" },
                //{ field: "StackTrace", title: "Stack Trace", width: "300px" },
            ],
            change: function (e) {
                var selectedRows = this.select();
                if (selectedRows.length > 0) {
                    var itemId = this.dataItem(selectedRows[0]).Id;
                    var errorMessage = this.dataItem(selectedRows[0]).ErrorMessage;
                    var stackTrace = this.dataItem(selectedRows[0]).StackTrace;
                    $(pageVarsPri.tags.systemLogErrorMessageId).html(errorMessage);
                   
                    $.ajax({
                        type: 'GET',
                        url: crudServiceBaseUrl + "/GetSysemLogStackTrace?logId=" + itemId,
                        success: function (result) {
                            $(pageVarsPri.tags.systemLogStackTraceId).html(result);
                        }
                    });
                }
            }
        });
    }
    function loadruleSqlHardCodedInputGrid (ruleConfigId) {
        if ($(pageVarsPri.tags.ruleSqlHardCodedInputGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleSqlHardCodedInputGridId).kendoGrid('destroy').empty();
        }
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;

        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigHardCodedInputItems?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfigHardCodedInputItem",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfigHardCodedInputItem",
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfigHardCodedInputItem?ruleConfigId=" + ruleConfigId + "&projectId=" + selectedProjectUniqueKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        ParameterName: { editable: true, validation: { required: true } },
                        Description: { editable: true, validation: { required: false } },
                        ParameterValue: { editable: true, validation: { required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleSqlHardCodedInputGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "ParameterName", title: "Sql Parameter", width: "120px" },
                { field: "ParameterValue", title: "Parameter Value", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadruleSqlExclusionsGroupInputGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.ruleSqlExclusionsInputGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleSqlExclusionsInputGridId).kendoGrid('destroy').empty();
        }
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;

        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigExclusionsGroupInputItems?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfigExclusionsGroupInputItem",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfigExclusionsGroupInputItem",
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfigExclusionsGroupInputItem?ruleConfigId=" + ruleConfigId + "&projectId=" + selectedProjectUniqueKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        ParameterName: { editable: true, validation: { required: true } },
                        Description: { editable: true, validation: { required: false } },
                        ExclusionsGroupId: { editable: true, validation: { required: true } },
                        ExclusionsGroupName: { editable: false, validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleSqlExclusionsInputGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "ParameterName", title: "Sql Parameter", width: "120px" },
                { field: "ExclusionsGroupId", title: "Excluisions Group", width: "120px", editor: RuleConfigExclusionsGroupInputListDropDownEditor, template: "#=ExclusionsGroupName#" },
                { field: "Description", title: "Description", width: "120px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadruleSqlClassRefernceInputGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.ruleSqlClassRefernceInputGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleSqlClassRefernceInputGridId).kendoGrid('destroy').empty();
        }
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;

        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigClassRefInputItems?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfigClassRefInputItem",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfigClassRefInputItemm",
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfigClassRefInputItem?ruleConfigId=" + ruleConfigId + "&projectId=" + selectedProjectUniqueKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        ParameterName: { editable: true, validation: { required: true } },
                        Description: { editable: true, validation: { required: false } },
                        ClassProperty: { editable: true, validation: { required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleSqlClassRefernceInputGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "ParameterName", title: "Sql Parameter Name", width: "120px" },
                { field: "ClassProperty", title: "Class Property mapping", width: "120px", editor: RuleConfigSqlClassRefInputListDropDownEditor },
                { field: "Description", title: "Description", width: "120px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadExclusionsItemsGrid(exclusionsGroupId) {
        if ($(pageVarsPri.tags.projectExclusionItemsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectExclusionItemsGridId).kendoGrid('destroy').empty();
        }
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListExclusionGroupItems?exclusionsGroupId=" + exclusionsGroupId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateExclusionGroupItem",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyExclusionGroupItem",
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateExclusionGroupItem?exclusionsGroupId=" + exclusionsGroupId + "&projectId=" + selectedProjectUniqueKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Name: { editable: true, validation: { required: true } },
                        Key: { editable: true, validation: { required: true } },
                        ReasonAdded: { editable: true, validation: { required: false } },
                        DateAdded: {  type: "date", editable: false, validation: { required: false } },
                        AddedBy: { editable: false, validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectExclusionItemsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Key", title: "Key", width: "120px" },
                { field: "ReasonAdded", title: "Reason Added", width: "120px" },
                { field: "DateAdded", title: "Date Added", width: "120px", format: "{0:dd-MMM-yyyy}" },
                { field: "AddedBy", title: "Added By", width: "120px" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadprojectActurisImportsGrid(projectId) {
        if ($(pageVarsPri.tags.projectActurisImportsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectActurisImportsGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectActurisImports?projectId=" + projectId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectActurisImport",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectActurisImport",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectActurisImport?projectId=" + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        SqlCommandText: { validation: { required: true } },
                        SqlCommandTextIsStoredProc: { type: "boolean" },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        TargetDbName: { validation: { required: false } },
                        TargetDatabaseDetailsId: { validation: { required: true } },
                        LastRun: { type: "date", editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectActurisImportsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy", { text: "Test", click: showExecuteActurisImportSnapShotBreachDetails }], title: "&nbsp;", width: "150px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                {
                    field: "SqlCommandText",
                    // title: "Sql Command",
                    width: "120px",
                    height: "200px",
                    headerTemplate: '<span style="" title="Can be a stored procedure name or sql query.">Sql Command (i)</span>',
                    template: "<div data-toggle='tooltip' title='#:SqlCommandText#'>#:SqlCommandText#</div>",
                },
                { field: "SqlCommandTextIsStoredProc", title: "Sql Command Is Stored Proc", width: "80px" },
                { field: "ScheduleId", title: "Schedule", width: "120px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "TargetDatabaseDetailsId", title: "Target Db", width: "120px", editor: TargetDbDropDownEditorRuleConfig, template: "#=TargetDbName#" },
                { field: "LastRun", title: "Last Run", width: "120px", format: "{0:dd-MMM-yyyy hh:mm}" },
            ],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];              
            }
        });
    }
    function loadprojectExclusionsGrid(projectId) {

        if ($(pageVarsPri.tags.projectExclusionsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectExclusionsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectExclusionsGroups?projectId=" + projectId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectExclusionsGroup",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectExclusionsGroup",
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectExclusionsGroup?projectId=" + projectId,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        GroupName: { validation: { required: true } },
                        Description: { editable: true, validation: { required: false } },
                        ReasonAdded: { editable: true, validation: { required: false } },
                        DateAdded: { type: "date", editable: false, validation: { required: false } },
                        AddedBy: { editable: false, validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectExclusionsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" },
                { field: "GroupName", title: "Group Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "ReasonAdded", title: "Reason Added", width: "120px" },
                { field: "DateAdded", title: "Date Added", width: "120px", format: "{0:dd-MMM-yyyy}" },
                { field: "AddedBy", title: "Added By", width: "120px" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ItemId = this.dataItem(selectedRows[0]).Id
                    loadExclusionsItemsGrid(ItemId);
                    $(pageVarsPri.tags.level3TabStripClass).show();
                    utils.jumpToElement(pageVarsPri.tags.projectExclusionItemsGridId);
                }
            }
        });
    }
    function loadUsersProjectMemberships(userId) {
        
        if ($(pageVarsPri.tags.systemUserSystemPermissionsGridId).kendoGrid()) {
            $(pageVarsPri.tags.systemUserSystemPermissionsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListUserProjectMemberships?userId=" + userId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectUserMembership",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectUserMembership",
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectUserMembership?userId=" + userId,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "UserId",
                    fields: {
                        //UserId: { editable: false, nullable: true },
                        //UserName: { validation: { required: true } },
                        ProjectId: { validation: { required: true } },
                        ProjectName: { editable: false, validation: { required: false } },
                        isProjectAdmin: { type: "boolean" }                        
                    }
                }
            }
        });

        $(pageVarsPri.tags.systemUserSystemPermissionsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                //{ field: "UserId", title: "User Id", width: "120px" },
                //{ field: "UserName", title: "User Name", width: "120px" },
                { field: "ProjectId", title: "Project Id", width: "120px", editor: ProjectDropDownEditorSystemUserProjectMembership, template: "#=ProjectName#" },
                //{ field: "ProjectName", title: "Project Name", width: "120px" },
                { field: "isProjectAdmin", title: "is Project Admin", width: "120px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadProjectRegionsGrid(projectKey) {
        if ($(pageVarsPri.tags.projectRegionsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectRegionsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/" + 'ListProjectRegions?projectGuid=' + projectKey,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/" + 'UpdateProjectRegion',
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/" + 'RemoveProjectRegion',
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/" + 'CreateProjectRegion?projectGuid=' + projectKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return {
                            models: kendo.stringify(options.models)
                        };
                    }
                }
            },
            serverSorting: false,
            serverFiltering: false,
            serverGrouping: false,
            serverPaging: false,
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true }},
                        Description: { validation: { required: false } },
                        IsActive: { type: "boolean" }
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectRegionsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadProjectUsersGrid(projectKey) {
        if ($(pageVarsPri.tags.projectUsersGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectUsersGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/" + 'ListProjectUsers?projectGuid=' + projectKey,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/" + 'UpdateProjectUser?projectGuid=' + projectKey,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/" + 'RemoveUserFromProject?projectGuid=' + projectKey,
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/" + 'CreateProjectUser?projectGuid=' + projectKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            serverSorting: true,
            serverFiltering: true,
            serverGrouping: true,
            serverPaging: true,
            batch: false,
            pageSize: 150,
            schema: {
                data: "data",
                total: "total",
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        DomainName: { validation: { required: false } },
                        Email: { validation: { required: true } },
                        TitleId: { validation: { required: false } },
                        ActurisOrganisationKey: { validation: { required: false } },
                        ActurisOrganisationName: { validation: { required: false } },
                        AlsoKnownAs: { validation: { required: false } },
                        ActurisUniqueIdentifier: { validation: { required: false } },
                        IsProjectAdmin: { type: "boolean" },
                        IsMicroServiceMethodAccessUser: { type: "boolean" },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                        IsProjectSuperUser: { type: "boolean" },
                        ProjectSuperUserInfo: { validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectUsersGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            //toolbar: ["create"],
            columns: GetColumnsForProjectUsersGird(),
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var userId = this.dataItem(selectedRows[0]).Id
                    loadUsersOfficeMemberships(userId);
                    loadUsersTeamMemberships(userId);
                }
                $("#projectusergridmembershiptabstrip .nav-tabs, #projectusergridmembershiptabstrip .tab-content").show();
                if (pageVarsPri.vars.IsVTProject == 'true') {
                    utils.jumpToElement("#projectusergridmembershiptabstrip");
                }
            }
        });
    }
    function GetColumnsForProjectUsersGird()
    {
        if (pageVarsPri.vars.IsVTProject == "true") {
            return [
                    { command: ["edit", { name: "destroy", text: "Remove" }], title: "&nbsp;", width: "70px" },
                    { field: "Name", title: "Name", width: "120px" },
                    { field: "DomainName", title: "Domain Name", width: "120px" },
                    { field: "Email", title: "Email", width: "120px", editor: EmailEditorWithValidatior },
                    {
                        field: "ActurisUniqueIdentifier",
                        //title: "Acturis Unique ID",
                        width: "120px",
                        headerTemplate: '<span style="" title="Changing this value may cause issues with the Acturis Import">Acturis Unique ID (i)</span>'
                    },
                    { field: "IsProjectAdmin", title: "Is Project Admin", width: "120px" },
                    //{ field: "IsActive", title: "Is Active", width: "80px" }
            ];
        };
        if (pageVarsPri.vars.IsMicroServiceProject == "true") {
            return [
                { command: ["edit", { name: "destroy", text: "Remove" }], title: "&nbsp;", width: "80px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "DomainName", title: "Domain Name", width: "120px" },
                {
                    field: "Email",
                    title: "Email",
                    width: "120px",
                    editor: EmailEditorWithValidatior
                },
                { field: "IsProjectAdmin", title: "Project Admin", width: "80px" },
                { field: "IsMicroServiceMethodAccessUser", title: "Can call Micro Service Web-Methods", width: "130px" },
                { field: "IsActive", title: "Is Active", width: "60px" }
            ];
        };
        return [
                { field: "Name", title: "Name", width: "120px" },
                { field: "DomainName", title: "Domain Name", width: "120px" },
                {
                    field: "Email",
                    title: "Email",
                    width: "120px",
                    editor: EmailEditorWithValidatior
                },
                { field: "IsProjectSuperUser", title: "Is Project Super User", width: "80px" },
                { field: "ProjectSuperUserInfo", title: "Super User Override", width: "80px" },
                { field: "IsProjectAdmin", title: "Is Project Admin", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" },

             { command: ["edit", { name: "destroy", text: "Remove" }], title: "&nbsp;", width: "170px" }
        ];
    }
    function loadUsersTeamMemberships(userId) {

        if ($(pageVarsPri.tags.userteammembershipsGridId).kendoGrid()) {
            $(pageVarsPri.tags.userteammembershipsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListUserTeamMemberships?userId=" + userId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        TeamName: { validation: { required: true } },
                        TeamKey: { validation: { required: true } },
                        OrganisationName: { validation: { required: true } },
                        OrganisationKey: { validation: { required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.userteammembershipsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            columns: [
                { field: "TeamName", title: "Team Name", width: "120px" },
                { field: "TeamKey", title: "Team Key", width: "120px" },
                { field: "OrganisationName", title: "Organisation Name", width: "120px" },
                { field: "OrganisationKey", title: "Organisation Key", width: "120px" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadUsersOfficeMemberships(userId) {

        if ($(pageVarsPri.tags.userofficemembershipsGridId).kendoGrid()) {
            $(pageVarsPri.tags.userofficemembershipsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListUserOfficeMemberships?userId=" + userId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        OfficeName: { validation: { required: true } },
                        OfficeKey: { validation: { required: true } },
                        OrganisationName: { validation: { required: true } },
                        OrganisationKey: { validation: { required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.userofficemembershipsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            columns: [
                { field: "OfficeName", title: "Office Name", width: "120px" },
                { field: "OfficeKey", title: "Office Key", width: "120px" },
                { field: "OrganisationName", title: "Organisation Name", width: "120px" },
                { field: "OrganisationKey", title: "Organisation Key", width: "120px" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function loadAllSystemUsersGrid() {
        if ($(pageVarsPri.tags.systemUsersGridId).kendoGrid()) {
            $(pageVarsPri.tags.systemUsersGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/" + 'ListSystemUsers',
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/" + 'UpdateSystemUser',
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/" + 'DestroySystemUser',
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/" + 'CreateSystemUser',
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            serverSorting: true,
            serverFiltering: true,
            serverGrouping: true,
            serverPaging: true,
            batch: false,
            pageSize: 500,
            schema: {
                data: "data",
                total: "total",
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        DomainName: { validation: { required: false } },
                        Email: { validation: { required: true } },
                        TitleId: { validation: { required: false } },
                        ActurisOrganisationKey: { validation: { required: false } },
                        ActurisOrganisationName: { validation: { required: false } },
                        AlsoKnownAs: { validation: { required: false } },
                        ActurisUniqueIdentifier: { validation: { required: false } },
                        IsSystemAdmin: { type: "boolean" },
                        IsSystemSuperUser: { type: "boolean" },
                        SystemSuperUserInfo: { validation: { required: false } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                },
                errors: function (response) {
                    if (response.MessageType && response.MessageType !== "1") {
                        alert(response.MessageTypeString + ': ' +response.Message);
                    }
                    return false;
                },
            }
        });

        $(pageVarsPri.tags.systemUsersGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: {
                pagesize: 500,
                refresh: true
            },
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "Name", title: "Name", width: "120px" },
                { field: "DomainName", title: "Domain Name", width: "120px" },
                { field: "Email", title: "Email", width: "120px", editor: EmailEditorWithValidatior },
                { field: "IsSystemAdmin", title: "System Admin", width: "120px" },
                { field: "IsSystemSuperUser", title: "MyHub Super User", width: "120px" },
                { field: "SystemSuperUserInfo", title: "Super User Override", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var userId = this.dataItem(selectedRows[0]).Id
                    loadUsersProjectMemberships(userId);
                }
                $("#systemuserspermissionstabstrip .nav-tabs, #systemuserspermissionstabstrip .tab-content").show();
                utils.jumpToElement("#ProjectDetailsLevel1");
            }
        });
    }
    function loadOfficeUsersGrid(officeId) {
        if ($(pageVarsPri.tags.OfficeUsersGridId).kendoGrid()) {
            $(pageVarsPri.tags.OfficeUsersGridId).kendoGrid('destroy').empty();
        }
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/" + 'ListOfficeUsers?officeId=' + officeId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/" + 'UpdateOfficeUser?officeId=' + officeId + '&projectId=' + selectedProjectUniqueKey,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/" + 'RemoveOfficeUser?officeId=' + officeId + '&projectId=' + selectedProjectUniqueKey,
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/" + 'AddUserToOffice?officeId=' + officeId + '&projectId=' + selectedProjectUniqueKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        if (operation === "create") {
                            var user = options.models[0].Id;
                            user.IsOfficeManager = options.models[0].IsOfficeManager;
                            user.IsOfficeRegionalManager = options.models[0].IsOfficeRegionalManager;
                            user.IsOfficeQualityAuditor = options.models[0].IsOfficeQualityAuditor;
                            user.IsActive = options.models[0].IsActive;
                            return { models: '[' + kendo.stringify(options.models[0].Id) + ']' };
                        }
                        else {
                            return { models: kendo.stringify(options.models) };
                        }
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: true, nullable: true },
                        Name: { editable: false, validation: { required: true } },
                        DomainName: { editable: false, validation: { required: false } },
                        Email: { editable: false, validation: { required: true } },
                        TitleId: {editable: false,  validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisUniqueIdentifier: { editable: false, validation: { required: false } },
                        IsOfficeManager: { type: "boolean" },
                        IsOfficeRegionalManager: { type: "boolean" },
                        IsOfficeQualityAuditor: { type: "boolean" },
                        //IsActive: { editable: false, type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.OfficeUsersGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", { name: "destroy", text: "Remove" }], title: "&nbsp;", width: "120px" },
                { field: "Id", title: "Name", width: "180px", editor: UserDropDownEditorOffice, template: "#=Name#" },
                { field: "Name", title: "Name (filter)", width: "200px" },
                { field: "DomainName", title: "Domain Name", width: "120px" },
                { field: "Email", title: "Email", width: "120px" },
                //{ field: "TitleId", title: "Title Id", width: "120px" },
                //{ field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "120px" },
                //{ field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "120px" },
                //{ field: "AlsoKnownAs", title: "Also Known As", width: "120px" },
                { field: "ActurisUniqueIdentifier", title: "Acturis Unique ID", width: "120px" },
                { field: "IsOfficeManager", title: "Office Manager", width: "120px" },
                { field: "IsOfficeRegionalManager", title: "Regional Managing Director", width: "120px" },
                { field: "IsOfficeQualityAuditor", title: "Quality Advisor", width: "120px" },
                //{ field: "IsActive", title: "Is Active", width: "80px" },
                ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];

                if (selectedRows.length > 0) {
                    var userId = this.dataItem(selectedRows[0]).Id
                    //loadUsersProjectMemberships(userId);
                }
                utils.jumpToElement("#ProjectDetailsLevel1");
            }
        });
    }
    function loadProjectOfficeTeamsGrid(officeId) {
        if ($(pageVarsPri.tags.OfficeTeamsGridId).kendoGrid()) {
            $(pageVarsPri.tags.OfficeTeamsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListOfficeTeams?officeId=" + officeId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateTeam",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyTeam",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateTeam?projectGuid=" + officeId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: true, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        OfficeId: { validation: { required: true } },
                        ActurisOrganisationKey: { validation: { required: false } },
                        ActurisOrganisationName: { validation: { required: false } },
                        AlsoKnownAs: { validation: { required: false } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.OfficeTeamsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            //toolbar: ["create"],
            columns: [
                //{ field: "Id", title: "Id", width: "120px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "OfficeId", title: "Office", width: "180px", editor: officeDropDownEditor, template: "#=OfficeName#" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "120px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "120px" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" }],
            //{ command: ["edit"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];

            }
        });
    }
    function loadProjectTeamsGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectTeamsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectTeamsGridId).kendoGrid('destroy').empty();
        }
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectTeams?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateTeam",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyTeam",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateTeam?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        OfficeId: { validation: { required: true } },
                        OfficeName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { validation: { required: false } },
                        ActurisOrganisationName: { validation: { required: false } },
                        AlsoKnownAs: { validation: { required: false } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectTeamsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" },
                //{ field: "Id", title: "Id", width: "120px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "OfficeId", title: "Office", width: "180px", editor: officeDropDownEditor, template: "#=OfficeName#" },
                { field: "OfficeName", title: "Office (filter)", width: "180px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "120px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "120px" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var teamId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.teamSubGridsContainerclass).show();
                    $(pageVarsPri.tags.level3TabStripClass).show();
                    loadTeamUsersGrid(teamId);
                }
                utils.jumpToElement(pageVarsPri.tags.TeamUsersGridId);
            }
        });
    }
    function loadTeamUsersGrid(teamId) {
        if ($(pageVarsPri.tags.TeamUsersGridId).kendoGrid()) {
            $(pageVarsPri.tags.TeamUsersGridId).kendoGrid('destroy').empty();
        }
        var selectedProjectUniqueKey = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/" + 'listTeamUsers?teamId=' + teamId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/" + 'UpdateTeamUser?teamId=' + teamId + '&projectId=' + selectedProjectUniqueKey,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/" + 'RemoveTeamUser?teamId=' + teamId + '&projectId=' + selectedProjectUniqueKey,
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/" + 'AddUserToTeam?teamId=' + teamId + '&projectId=' + selectedProjectUniqueKey,
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        if (operation === "create") {
                            var user = options.models[0].Id;
                            user.IsClaimsHandler = options.models[0].IsClaimsHandler;
                            user.IsTeamLead = options.models[0].IsTeamLead;
                            user.IsActive = options.models[0].IsActive;
                            return { models: '[' + kendo.stringify(options.models[0].Id) + ']' };
                        }
                        else {
                            return { models: kendo.stringify(options.models) };
                        }
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: true, nullable: true },
                        Name: { editable: false, validation: { required: true } },
                        DomainName: { editable: false, validation: { required: false } },
                        Email: { editable: false, validation: { required: true } },
                        TitleId: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisUniqueIdentifier: { editable: false, validation: { required: false } },
                        IsClaimsHandler: { type: "boolean" },
                        IsTeamLead: { type: "boolean" },
                        //IsActive: { editable: false, type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.TeamUsersGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", { name: "destroy", text: "Remove" }], title: "&nbsp;", width: "170px" },
                { field: "Id", title: "Name", width: "180px", editor: UserDropDownEditorTeam, template: "#=Name#" },
                { field: "Name", title: "Name (filter)", width: "200px" },
                { field: "DomainName", title: "Domain Name", width: "120px" },
                { field: "Email", title: "Email", width: "120px" },
                //{ field: "TitleId", title: "Title Id", width: "120px" },
                //{ field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "120px" },
                //{ field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "120px" },
                //{ field: "AlsoKnownAs", title: "Also Known As", width: "120px" },
                { field: "ActurisUniqueIdentifier", title: "Acturis Unique ID", width: "120px" },
                { field: "IsClaimsHandler", title: "Claims Handler", width: "80px" },
                //{ field: "IsTeamLead", title: "Team Lead", width: "80px" },
                //{ field: "IsActive", title: "Is Active", width: "80px" },
                ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];

                //if (selectedRows.length > 0) {
                //    var userId = this.dataItem(selectedRows[0]).Id
                //    loadUsersProjectMemberships(userId);
                //}
                //utils.jumpToElement("#ProjectDetailsLevel1");
            }
        });
    }
    function loadProjectOfficesGrid(projectGuid)
    {
        if ($(pageVarsPri.tags.projectOfficesGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectOfficesGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectOffices?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateOffice",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyOffice",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateOffice?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        Address: { validation: { required: false } },
                        ActurisOrganisationKey: { validation: { required: false } },
                        ActurisOrganisationName: { validation: { required: false } },
                        RegionName: { editable: false, validation: { required: false } },
                        RegionId: { validation: { required: false } },
                        AlsoKnownAs: { validation: { required: false } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectOfficesGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "Address", title: "Address", width: "120px" },
                { field: "RegionId", title: "Region", width: "120px", editor: OfficeRegionDropDownEditor, template: "#=RegionName#" },
                { field: "RegionName", title: "Region (filter)", width: "120px"},
                //{ field: "TitleId", title: "Title Id", width: "120px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "120px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "120px" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" }
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.level3TabStripClass).show();
                    $(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    loadProjectOfficeTeamsGrid(officeId);
                    loadOfficeUsersGrid(officeId)
                }
                utils.jumpToElement("#ProjectDetailsLevel1");
            }
        });
    }
    function loadRuleConfigUserParticipantsGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.ruleConfigUserParticipantsGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleConfigUserParticipantsGridId).kendoGrid('destroy').empty();
        }
        //var selectedOfficeParticipantId = getSelectedOfficeParticipantId();
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigUserParticipants?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfigUserParticipant?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfigUserParticipant",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfigUserParticipant?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        UserId: { validation: { required: true } },
                        UserName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleConfigUserParticipantsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "UserId", title: "Team", width: "120px", editor: ProjectDropDownEditorUserParticipants, template: "#=UserName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadRuleConfigTeamParticipantsGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.ruleConfigTeamParticipantsGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleConfigTeamParticipantsGridId).kendoGrid('destroy').empty();
        }
        //var selectedOfficeParticipantId = getSelectedOfficeParticipantId();
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigTeamParticipants?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfigTeamParticipant?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfigTeamParticipant",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfigTeamParticipant?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        TeamId: { validation: { required: true } },
                        TeamName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleConfigTeamParticipantsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "TeamId", title: "Team", width: "120px", editor: ProjectDropDownEditorTeamParticipants, template: "#=TeamName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadRuleConfigOfficeParticipantsGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.ruleConfigOfficeParticipantsGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleConfigOfficeParticipantsGridId).kendoGrid('destroy').empty();
        }
        //var selectedOfficeParticipantId = getSelectedOfficeParticipantId();
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigOfficeParticipants?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfigOfficeParticipant?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfigOfficeParticipant",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfigOfficeParticipant?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        OfficeId: { validation: { required: true } },
                        OfficeName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false,  validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleConfigOfficeParticipantsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "OfficeId", title: "Office", width: "120px", editor: ProjectDropDownEditorOfficeParticipants, template: "#=OfficeName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadRuleConfigProjectParticipantsGrid(ruleConfigId)
    {
        if ($(pageVarsPri.tags.ruleConfigProjectParticipantsGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleConfigProjectParticipantsGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigProjectParticipants?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfigProjectParticipant",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfigProjectParticipant",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfigProjectParticipant?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        ProjectId: { validation: { required: true } },
                        ProjectName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleConfigProjectParticipantsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "ProjectId", title: "Project", width: "120px", editor: ProjectDropDownEditorProjectParticipants, template: "#=ProjectName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit","destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadRuleExchangeConfigExtractionsConfigPanelItemsGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.ruleExchangeConfigExtractionItemsTableId).kendoGrid()) {
            $(pageVarsPri.tags.ruleExchangeConfigExtractionItemsTableId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListExchangeRuleConfigValueMappingItems?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateExchangeRuleConfigValueMappingItems",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyExchangeRuleConfigValueMappingItems",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateExchangeRuleConfigValueMappingItems?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        EmailSearchSource: { validation: { required: true } },
                        EmailMessgaeSearchType: { validation: { required: true } },
                        SearchText: { validation: { required: true } },
                        MappedToBreachTableColumnName: { validation: { required: true } },
                        AttachmentNameSearchFilter: { validation: { required: true } },
                        AttachmentNameSearchString: { validation: { required: false } },
                        AttachmentDocumentType: { validation: { required: true } },
                        EmailSearchSourceName: { validation: { required: false } },
                        EmailMessgaeSearchTypeName: { validation: { required: false } },
                        AttachmentNameSearchFilterName: { validation: { required: false } },
                        AttachmentDocumentTypeName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleExchangeConfigExtractionItemsTableId).kendoGrid({
            dataSource: dataSource,
            resizable: true,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "Name", title: "Name", width: "200px" },
                { field: "Description", title: "Description", width: "200px" },
                { field: "IsActive", title: "Is Active", width: "120px" },
                { field: "EmailSearchSource", title: "Email Search Source", width: "200px", template: "#=EmailSearchSourceName#", editor: EmailSearchSourceDropDownEditor},
                { field: "AttachmentNameSearchFilter", title: "Attachment Name Search Filter", width: "200px", template: "#=AttachmentNameSearchFilterName#", editor: StringFilterDropDownEditor },
                { field: "AttachmentDocumentType", title: "Attachment Document Type", width: "200px", template: "#=AttachmentDocumentTypeName#", editor: EmailMessageAttachmentDocTypeDropDownEditor },
                { field: "AttachmentNameSearchString", title: "Attachment Name Search String", width: "200px" }, //editor: ExchangeItemSubjectFilterDropDownEditor
                { field: "EmailMessgaeSearchType", title: "Email Message Search Type", width: "200px", template: "#=EmailMessgaeSearchTypeName#", editor: EmailMessgaeSearchTypeDropDownEditor },
                { field: "SearchText", title: "Search Text", width: "200px" },
                { field: "MappedToBreachTableColumnName", title: "Breach Table Column", width: "200px", editor: ExchangeRuleConfigBreachTableColumnsDropDownEditor },
                
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px", locked: true }],
            editable: "popup",
            change: function (e) {
                //$(pageVarsPri.tags.level4TabStripClass).show();
                //if (this.dataItem(this.select()).EmailSearchSource == 6) {
                //    var disableField = $("div[data-container-for=AttachmentNameSearchFilter], label[for=AttachmentNameSearchFilter],div[data-container-for=AttachmentDocumentType], label[for=AttachmentDocumentType],div[data-container-for=AttachmentNameSearchString], label[for=AttachmentNameSearchString]");
                //    disableField.show();
                //}
                //else
                //{
                //    var disableField = $("div[data-container-for=AttachmentNameSearchFilter], label[for=AttachmentNameSearchFilter],div[data-container-for=AttachmentDocumentType], label[for=AttachmentDocumentType],div[data-container-for=AttachmentNameSearchString], label[for=AttachmentNameSearchString]");
                //    disableField.hide();
                //}
            },
        });
    }
    
    function loadruleExcelConfigurationsGrid(ruleId) {
        if ($(pageVarsPri.tags.ruleExcelConfigurationsGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleExcelConfigurationsGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            error: KendoGridDataSourceError,
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListExcelRuleConfigurations?ruleId=" + ruleId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateExcelRuleConfiguration",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyExcelRuleConfiguration",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateExcelRuleConfiguration?ruleId=" + ruleId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        SourceDocFolderPath: { validation: { required: true } },
                        SourceDocSelectionType: { validation: { required: true } },
                        SourceDocNameSearchText: { validation: { required: false } },
                        SQLQuery: { validation: { required: true } },
                        hasHeaderRow: { type: "boolean" },
                        DocumentDeleteMode: { validation: { required: true } },
                        MoveDocumentDestinationDirectory: { validation: { required: false } },
                        AppendTodaysDateToMovedDocName: { type: "boolean" },
                        DocumentSelectionTypeName: { validation: { required: false } },
                        AJGExcelDocDeleteModeName: { validation: { required: false } },
                        sourceFileNameSavedInBreachFieldName: { validation: { required: false } },
                        PreExecuteBreachesAction: { validation: { required: true } },
                        PreExecuteBreachesActionName: { validation: { required: false } },
                        AutoMapResultsToBreachTableFields: { type: "boolean" },
                        //SetExistingBreachesToArchivedBeforeExecute: { type: "boolean" },
                    }
                },
                errors: KendoGridSchemaErrors
            }
        });

        $(pageVarsPri.tags.ruleExcelConfigurationsGridId).kendoGrid({
            dataSource: dataSource,
            resizable: true,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "Name", title: "Name", width: "200px" },
                { field: "Description", title: "Description", width: "200px" },
                { field: "IsActive", title: "Is Active", width: "120px" },
                { field: "ScheduleId", title: "Rule Schedule", width: "200px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "SourceDocFolderPath", title: "Source Doc Path", width: "200px" },
                { field: "SourceDocSelectionType", title: "Source Doc Selection Type", width: "200px", editor: ExcelSourceDocSelectionTypeDropDownEditor, template: "#=DocumentSelectionTypeName#" },
                { field: "SourceDocNameSearchText", title: "Source Doc Name Search Text", width: "200px" },
                {
                    field: "SQLQuery",
                    title: "SQL Query",
                    editor: textAreaEditor, 
                    width: "200px"
                },
                { field: "hasHeaderRow", title: "Has Header Row", width: "120px" },
                { field: "DocumentDeleteMode", title: "Doc Post Process Action", width: "200px", editor: ExcelDocumentDeleteModeDropDownEditor, template: "#=AJGExcelDocDeleteModeName#" },
                { field: "MoveDocumentDestinationDirectory", title: "Move Document Destination Directory", width: "120px" },
                { field: "AppendTodaysDateToMovedDocName", title: "Append Todays Date To Moved Doc Name", width: "200px" },
                { field: "AutoMapResultsToBreachTableFields", title: "Auto Map Sql Columns to Breach Fields", width: "200px" },
                { field: "sourceFileNameSavedInBreachFieldName", title: "Map File Name To Breach Field", width: "200px", editor: BreachFieldDropDownEditor },
                { field: "PreExecuteBreachesAction", title: "Pre Execute Breach Action", width: "200px", editor: RuleConfigPreExecuteBreachesActionSelectionTypeDropDownEditor, template: "#=PreExecuteBreachesActionName#" },
                //{ field: "SetExistingBreachesToArchivedBeforeExecute", title: "Set Existing Breaches To Archived Before Execute", width: "200px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px", locked: true }],
            editable: "popup",
            change: function (e) {
                $(pageVarsPri.tags.level4TabStripClass).show();
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ruleConfigId = this.dataItem(selectedRows[0]).Id;
                    loadRuleExchangeConfigExtractionsConfigPanelItemsGrid(ruleConfigId);
                }
            }
        });
    }
    function KendoGridSchemaErrors(e) {
        if (e.ErrorMessage && e.ErrorMessage !== "") {
            return e.ErrorMessage;
        }
        return false;
    }
    function KendoGridDataSourceError(e) {
        if (e.errors && e.errors !== "") {
            alert(e.errors);
            $(".k-grid").each(function () {
                var grid = $(this).data("kendoGrid");
                if (grid !== null && grid.dataSource == e.sender) {
                    grid.one("dataBinding", function (e) {
                        e.preventDefault();   // cancel grid rebind
                    });
                }
            });
        }
    }
    function loadruleExchangeConfigurationsGrid(ruleId) {
        if ($(pageVarsPri.tags.ruleExchangeConfigurationsGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleExchangeConfigurationsGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListExchangeRuleConfigurations?ruleId=" + ruleId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateExchangeRuleConfiguration",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyExchangeRuleConfiguration",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateExchangeRuleConfiguration?ruleId=" + ruleId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        ExchangeItemDeleteMode: { validation: { required: true } },
                        SentFromFilter: { validation: { required: false } },
                        SentFromSearchText: { validation: { required: false } },
                        OperatorForSentFromAndSubject: { validation: { required: false } },
                        SubjectFilter: { validation: { required: false } },
                        SubjectSearchText: { validation: { required: false } },
                        OperatorForSubjectAndDate: { validation: { required: false } },
                        ReceivedDate: { validation: { required: false } },
                        ReceivedDateUseDateOnly: { type: "boolean" },
                        ReceivedDateOneFilter: { validation: { required: false } },
                        ReceivedDateOneOffset: { type: "number", validation: { required: false } },
                        ReceivedDateOneOffsetPeriod: { validation: { required: false } },
                        ReceivedDateTwoFilter: { validation: { required: false } },
                        ReceivedDateTwoOffset: { type: "number", validation: { required: false } },
                        ReceivedDateTwoOffsetPeriod: { validation: { required: false } },
                        ExchangeItemDeleteModeName: { validation: { required: false } },
                        SentFromFilterName: { validation: { required: false } },
                        OperatorForSentFromAndSubjectName: { validation: { required: false } },
                        OperatorForSubjectAndDateName: { validation: { required: false } },
                        ReceivedDateName: { validation: { required: false } },
                        SubjectFilterName: { validation: { required: false } },
                        ReceivedDateOneFilterName: { validation: { required: false } },
                        ReceivedDateOneOffsetPeriodName: { validation: { required: false } },
                        ReceivedDateTwoFilterName: { validation: { required: false } },
                        ReceivedDateTwoOffsetPeriodName: { validation: { required: false } },
                        ExchangeAccountDetailsId: { validation: { required: true } },
                        ExchangeAccountDetailsName: { validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleExchangeConfigurationsGridId).kendoGrid({
            dataSource: dataSource,
            resizable: true,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "Name", title: "Name", width: "200px" },
                { field: "Description", title: "Description", width: "200px" },
                { field: "IsActive", title: "Is Active", width: "120px" },
                { field: "ExchangeAccountDetailsId", title: "Exchange Account", width: "200px", template: "#=ExchangeAccountDetailsName#", editor: ProjectExchangeAccountsDropDownEditor },
                { field: "ScheduleId", title: "Rule Schedule", width: "200px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "ExchangeItemDeleteMode", title: "Outlook Delete Mode", width: "200px", editor: ExchangeItemDeleteModesDropDownEditor, template: "#=ExchangeItemDeleteModeName#" },
                { field: "SentFromFilter", title: "From Search Filter", width: "200px", editor: StringFilterDropDownEditor, template: "#=SentFromFilterName#" },
                { field: "SentFromSearchText", title: "From Search String", width: "200px" },
                { field: "OperatorForSentFromAndSubject", title: "Logic Operator", width: "200px", editor: ExchangeItemOperatorDropDownEditor, template: "#=OperatorForSentFromAndSubjectName#" },
                { field: "SubjectFilter", title: "Subject Search Filter", width: "200px", editor: ExchangeItemSubjectFilterDropDownEditor, template: "#=SubjectFilterName#" },
                { field: "SubjectSearchText", title: "Subject Search", width: "200px" },
                { field: "OperatorForSubjectAndDate", title: "Logic Operator", width: "200px", editor: ExchangeItemOperatorDropDownEditor, template: "#=OperatorForSubjectAndDateName#" },
                { field: "ReceivedDate", title: "Date Filter From", width: "200px", editor: ExchangeItemReceivedDateDropDownEditor, template: "#=ReceivedDateName#" },
                { field: "ReceivedDateUseDateOnly", title: "Filter Using Date Only", width: "120px" },
                { field: "ReceivedDateOneFilter", title: "Received Date (1) Filter", width: "200px", editor: ExchangeItemReceivedDateFilterDropDownEditor, template: "#=ReceivedDateOneFilterName#" },
                { field: "ReceivedDateOneOffset", title: "Received Date (1) Offset", width: "200px" },
                { field: "ReceivedDateOneOffsetPeriod", title: "Received Date (1) Offset Period", width: "200px", editor: ExchangeItemDateOffsetDropDownEditor, template: "#=ReceivedDateOneOffsetPeriodName#" },
                { field: "ReceivedDateTwoFilter", title: "Received Date (2) Filter", width: "200px", editor: ExchangeItemReceivedDateFilterDropDownEditor, template: "#=ReceivedDateTwoFilterName#" },
                { field: "ReceivedDateTwoOffset", title: "Received Date (2) Offset", width: "200px" },
                { field: "ReceivedDateTwoOffsetPeriod", title: "Received Date (2) Offset Period", width: "200px", editor: ExchangeItemDateOffsetDropDownEditor, template: "#=ReceivedDateTwoOffsetPeriodName#" },


                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px", locked: true }],
            editable: "popup",
            change: function (e) {
                $(pageVarsPri.tags.level4TabStripClass).show();
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ruleConfigId = this.dataItem(selectedRows[0]).Id;
                    loadRuleExchangeConfigExtractionsConfigPanelItemsGrid(ruleConfigId);
                }
            }
        });
    }
    function loadRuleConfigurationsGrid(ruleId) {
        if ($(pageVarsPri.tags.ruleConfigurationsGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleConfigurationsGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListRuleConfigurations?ruleId=" + ruleId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRuleConfiguration",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRuleConfiguration",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRuleConfiguration?ruleId=" + ruleId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        UserIdentifyingResultsFieldName: { validation: { required: true } },
                        UserPropertyName: { validation: { required: true } },
                        SqlCommandText: { validation: { required: true } },
                        SqlCommandTextIsStoredProc: { type: "boolean" },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        SetBreachesToResolvedScheduleName: { validation: { required: false } },
                        SetBreachesToResolvedScheduleId: { validation: { required: true } },
                        TargetDbName: { validation: { required: false } },
                        TargetDbID: { validation: { required: true } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                        PreExecuteBreachesAction: { validation: { required: true } },
                        PreExecuteBreachesActionName: { validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleConfigurationsGridId).kendoGrid({
            dataSource: dataSource,
            resizable: true,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: pageVarsPri.vars.IsVTProject == 'true' ? [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "120px" },
                { field: "UserIdentifyingResultsFieldName", width: "250px",
                    headerTemplate: '<span style="color:red;" title="This is used to map the SQL breach result to a VT handler/User.">Sql Results User Field Name (i)</span>'
                },
                { field: "UserPropertyName", width: "250px",
                    headerTemplate: '<span style="color:red;" title="This is used to map the SQL breach result to a VT handler/User.">VT User Property Name (i)</span>'
                },
                {
                    field: "SqlCommandText", width: "160px",
                    headerTemplate: '<span style="" title="Can be a stored procedure name or sql query.">Sql Command (i)</span>',
                    title: "Sql Command",
                    editor: textAreaEditor,
                },
                { field: "SqlCommandTextIsStoredProc", title: "Sql Command Is Stored Proc", width: "240px" },
                { field: "ScheduleId", title: "Rule Schedule", width: "160px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "SetBreachesToResolvedScheduleId", title: "Resolve Breaches Schedule", width: "160px", editor: ScheduleDropDownEditorRuleConfig, template: "#=SetBreachesToResolvedScheduleName#" },
                { field: "TargetDbID", title: "Target Db", width: "120px", editor: TargetDbDropDownEditorRuleConfig, template: "#=TargetDbName#" }
            ] : [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "200px" },
                { field: "IsActive", title: "Is Active", width: "120px" },
                {
                    field: "SqlCommandText", width: "180px",
                    headerTemplate: '<span style="" title="Can be a stored procedure name or sql query.">Sql Command (i)</span>',
                    title: "Sql Command",
                    editor: textAreaEditor,
                },
                { field: "SqlCommandTextIsStoredProc", title: "Sql Command Is Stored Proc", width: "120px" },
                { field: "ScheduleId", title: "Rule Schedule", width: "140px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "SetBreachesToResolvedScheduleId", title: "Resolve Breaches Schedule", width: "160px", editor: ScheduleDropDownEditorRuleConfig, template: "#=SetBreachesToResolvedScheduleName#" },
                { field: "TargetDbID", title: "Target Db", width: "120px", editor: TargetDbDropDownEditorRuleConfig, template: "#=TargetDbName#" },
                { field: "PreExecuteBreachesAction", title: "Pre Execute Breach Action", width: "200px", editor: RuleConfigPreExecuteBreachesActionSelectionTypeDropDownEditor, template: "#=PreExecuteBreachesActionName#" },
            ],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ruleConfigId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.level4TabStripClass).show();
                    
                    //loadruleSqlClassRefernceInputGrid(ruleConfigId);
                    loadruleSqlExclusionsGroupInputGrid(ruleConfigId);
                    loadruleSqlHardCodedInputGrid(ruleConfigId);
                    
                    if (pageVarsPri.vars.IsVTProject == 'true') {
                        loadRuleConfigProjectParticipantsGrid(ruleConfigId);
                        loadRuleConfigOfficeParticipantsGrid(ruleConfigId);
                        loadRuleConfigTeamParticipantsGrid(ruleConfigId);
                        loadRuleConfigUserParticipantsGrid(ruleConfigId);

                        utils.jumpToElement(pageVarsPri.tags.ruleConfigProjectParticipantsGridId);
                        utils.jumpToElement(pageVarsPri.tags.ruleConfigOfficeParticipantsGridId);
                        utils.jumpToElement(pageVarsPri.tags.ruleConfigTeamParticipantsGridId);
                        utils.jumpToElement(pageVarsPri.tags.ruleConfigUserParticipantsGridId);
                    }
                }
            }
        });
    }
    function loadProjectSchedulesGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectSchedulesGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectSchedulesGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListSchedules?projectId=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateSchedule",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroySchedule",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateSchedule?projectId=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        ScheduleStartDate: { type: "date", validation: { required: true } },
                        ScheduleFrequencyId: { validation: { required: true } },
                        ScheduleFrequencyName: { validation: { required: false } },
                        // IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectSchedulesGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" },
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "200px" },
                { field: "ScheduleStartDate", title: "Scheduled Start Date", width: "120px", format: "{0:dd-MMM-yyyy}" },
                { field: "ScheduleFrequencyId", title: "Frequency", width: "120px", editor: ProjectDropDownEditorScheduleFrequencies, template: "#=ScheduleFrequencyName#" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ruleId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadRuleConfigurationsGrid(ruleId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }

    function passwordEditor(container, options) {
        $('<input type="password" onfocus="app.admin.clearInputFieldVal(this);" required data-bind="value:' + options.field + '"/>').appendTo(container);
    };
    function loadprojectExchangeAccountsGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectExchangeAccountsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectExchangeAccountsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectExchangeAccounts?projectId=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectExchangeAccounts",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectExchangeAccounts",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectExchangeAccounts?projectId=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        DisplayName: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        AutoDiscoverUserName: { validation: { required: true } },
                        AutoDiscoverUserPassword: { validation: { required: true } },
                        AutoDiscoverUserDomain: { validation: { required: true } },
                        AutoDiscoverEmail: { validation: { required: true } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectExchangeAccountsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "DisplayName", title: "Exchange Account Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px" },
                { field: "AutoDiscoverUserName", title: "Auto Discover User Name", width: "120px" },
                { field: "AutoDiscoverUserPassword", title: "Auto Discover User Password", editor: passwordEditor, template: "*********", width: "120px" },
                { field: "AutoDiscoverUserDomain", title: "Auto Discover User Domain", width: "120px" },
                { field: "AutoDiscoverEmail", title: "Auto Discover Email", editor: EmailEditorWithValidatior, width: "120px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy", { text: "Test", click: showExchangeAccountTestWindow }], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ruleId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadRuleConfigurationsGrid(ruleId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }

    function loadProjectDbDetailsGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectDbDetailsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectDbDetailsGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListTargetDBDetails?projectId=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateTargetDBDetails",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyTargetDBDetails",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateTargetDBDetails?projectId=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        DisplayName: { validation: { required: true } },
                        DBServerName: { validation: { required: true } },
                        DBName: { validation: { required: true } },
                        DBConnectionString: { validation: { required: true } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectDbDetailsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy"], title: "&nbsp;", width: "120px" },
                { field: "DisplayName", title: "Name", width: "120px" },
                { field: "IsActive", title: "Is Active", width: "60px" },
                { field: "DBServerName", title: "Server Name", width: "200px" },
                { field: "DBName", title: "DB Name", width: "150px" },
                { field: "DBConnectionString", title: "Connection String", width: "350px" }
            ],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ruleId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadRuleConfigurationsGrid(ruleId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigUsersProjectBreachSourcesGrid(escalationsFrameworkRuleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceProjectGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceProjectGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigProjectBreachSources?ruleConfigId=" + escalationsFrameworkRuleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigProjectBreachSource",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigProjectBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigProjectBreachSource?ruleConfigId=" + escalationsFrameworkRuleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        ProjectId: { validation: { required: true } },
                        ProjectName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceProjectGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "ProjectId", title: "Project", width: "120px", editor: ProjectDropDownEditorEFUserProjectBreachSource, template: "#=ProjectName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigRolesProjectBreachSourcesGrid(escalationsFrameworkRuleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceProjectGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceProjectGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigProjectBreachSources?ruleConfigId=" + escalationsFrameworkRuleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigProjectBreachSource",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigProjectBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigProjectBreachSource?ruleConfigId=" + escalationsFrameworkRuleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        ProjectId: { validation: { required: true } },
                        ProjectName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceProjectGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "ProjectId", title: "Project", width: "120px", editor: ProjectDropDownEditorEFRoleProjectBreachSource, template: "#=ProjectName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigUsersOfficeBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceOfficeGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceOfficeGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigOfficeBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigOfficeBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigOfficeBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigOfficeBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        OfficeId: { validation: { required: true } },
                        OfficeName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceOfficeGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "OfficeId", title: "Office", width: "120px", editor: ProjectDropDownEditorEFUserOfficeBreachSource, template: "#=OfficeName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigRolesOfficeBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceOfficeGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceOfficeGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigOfficeBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigOfficeBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigOfficeBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigOfficeBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        OfficeId: { validation: { required: true } },
                        OfficeName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceOfficeGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "OfficeId", title: "Office", width: "120px", editor: ProjectDropDownEditorEFRoleOfficeBreachSource, template: "#=OfficeName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigUsersTeamBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceTeamGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceTeamGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigTeamBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigTeamBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigTeamBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigTeamBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        TeamID: { validation: { required: true } },
                        TeamName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceTeamGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "TeamID", title: "Team", width: "120px", editor: ProjectDropDownEditorEFUserTeamBreachSource, template: "#=TeamName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }



    function loadprojectEscalationsConfigGenericRuleConfigBreachSourcesGrid(ruleConfigId, GridID) {
        if ($(GridID).kendoGrid()) {
            $(GridID).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigRuleConfigBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigRuleConfigBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigRuleConfigBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigRuleConfigBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        RuleConfigurationId: { validation: { required: true } },
                        RuleConfigName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });       

        $(GridID).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "RuleConfigurationId", title: "Rule Configuration", width: "120px", editor: ProjectDropDownEditorEFGenericRuleConfigBreachSource, template: "#=RuleConfigName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigGenericRuleBreachSourcesGrid(ruleConfigId, GridID) {
        if ($(GridID).kendoGrid()) {
            $(GridID).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigRuleBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigRuleBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigRuleBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigRuleBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        RuleId: { validation: { required: true } },
                        RuleName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(GridID).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "RuleId", title: "Rule", width: "120px", editor: ProjectDropDownEditorEFGenericRuleBreachSource, template: "#=RuleName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }




    function loadprojectEscalationsConfigRolesTeamBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceTeamGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceTeamGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigTeamBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigTeamBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigTeamBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigTeamBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        TeamID: { validation: { required: true } },
                        TeamName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceTeamGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "TeamID", title: "Team", width: "120px", editor: ProjectDropDownEditorEFRoleTeamBreachSource, template: "#=TeamName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigUsersRuleConfigBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleConfigGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleConfigGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigRuleConfigBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigRuleConfigBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigRuleConfigBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigRuleConfigBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        RuleConfigurationId: { validation: { required: true } },
                        RuleConfigName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleConfigGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "RuleConfigurationId", title: "Rule Configuration", width: "120px", editor: ProjectDropDownEditorEFUserRuleConfigBreachSource, template: "#=RuleConfigName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigRolesRuleConfigBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleConfigGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleConfigGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigRuleConfigBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigRuleConfigBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigRuleConfigBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigRuleConfigBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        RuleConfigurationId: { validation: { required: true } },
                        RuleConfigName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleConfigGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "RuleConfigurationId", title: "Rule Configuration", width: "120px", editor: ProjectDropDownEditorEFRoleRuleConfigBreachSource, template: "#=RuleConfigName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigRolesRuleBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigRuleBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigRuleBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigRuleBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigRuleBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        RuleId: { validation: { required: true } },
                        RuleName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "RuleId", title: "Rule", width: "120px", editor: ProjectDropDownEditorEFRoleRuleBreachSource, template: "#=RuleName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigUsersRuleBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigRuleBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigRuleBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigRuleBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigRuleBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        RuleId: { validation: { required: true } },
                        RuleName: { validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "RuleId", title: "Rule", width: "120px", editor: ProjectDropDownEditorEFUserRuleBreachSource, template: "#=RuleName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigUsersUserBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceUserGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceUserGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigUserBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigUserBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigUserBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigUserBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        UserId: { validation: { required: true } },
                        UserName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceUserGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "UserId", title: "User", width: "120px", editor: ProjectDropDownEditorEFUserUserBreachSource, template: "#=UserName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigRolesUserBreachSourcesGrid(ruleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceUserGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceUserGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEFRuleConfigUserBreachSources?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEFRuleConfigUserBreachSource?ruleConfigId=" + ruleConfigId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEFRuleConfigUserBreachSource",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEFRuleConfigUserBreachSource?ruleConfigId=" + ruleConfigId + '&projectId=' + projectId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        UserId: { validation: { required: true } },
                        UserName: { validation: { required: false } },
                        AlsoKnownAs: { editable: false, validation: { required: false } },
                        ActurisOrganisationName: { editable: false, validation: { required: false } },
                        ActurisOrganisationKey: { editable: false, validation: { required: false } },
                        IsActive: { type: "boolean" },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceUserGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "UserId", title: "User", width: "120px", editor: ProjectDropDownEditorEFRoleUserBreachSource, template: "#=UserName#" },
                { field: "AlsoKnownAs", title: "Also Known As", width: "80px" },
                { field: "ActurisOrganisationName", title: "Acturis Organisation Name", width: "80px" },
                { field: "ActurisOrganisationKey", title: "Acturis Organisation Key", width: "80px" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var officeId = this.dataItem(selectedRows[0]).Id;
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadProjectOfficeTeamsGrid(officeId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigUsersRecipientsGrid(escalationsFrameworkRuleConfigId) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersRecipientsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersRecipientsGridId).kendoGrid('destroy').empty();
        }
        var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListEscalationUsersRuleConfigRecipients?escalationsFrameworkRuleConfigId=" + escalationsFrameworkRuleConfigId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateEscalationUsersRuleConfigRecipient?projectId=" + projectId,
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyEscalationUsersRuleConfigRecipient",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateEscalationUsersRuleConfigRecipient?escalationsFrameworkRuleConfigId=" + escalationsFrameworkRuleConfigId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        RecipientId: { validation: { required: true } },
                        RecipientName: { validation: { required: true } },
                        RecipientEmail: { editable: false, nullable: true, required: false },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersRecipientsGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "RecipientId", title: "User", width: "120px", editor: UserDropDownEditorRecipients, template: "#=RecipientName#" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { field: "RecipientEmail", title: "Email", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var selectedId = this.dataItem(selectedRows[0]).Id;
                    //loadprojectEscalationsConfigUsersRecipientsGrid(selectedId)
                    //$(pageVarsPri.tags.officeSubGridsContainerclass).show();
                    //loadRuleConfigurationsGrid(ruleId);
                    //loadOfficeUsersGrid(officeId)
                }
            }
        });
    }
    function loadprojectEscalationsConfigEscalationsHistoryPartialView(escalationsConfigId, ParentDivID) {
        $.ajax({
            type: 'GET',
            url: pageVarsPri.urls.adminRoot + "/EscalationsEmailHistory",
            data: {
                EscalationsConfigId: escalationsConfigId,
                ParentDivID: ParentDivID,
                AdminRoot: pageVarsPri.urls.adminRoot,
            },
            dataType: 'html',
            success: function (result) {
                $(ParentDivID).html(result);
            }
        });
    }
    function loadAdminContentPartialViewForProjectType(projectID, ContentTargetDivID) {
        $.ajax({
            type: 'GET',
            url: pageVarsPri.urls.adminRoot + "/LoadAdminContentPartialViewForProjectType",
            data: {
                projectID: projectID
            },
            dataType: 'html',
            success: function (result) {
                $(ContentTargetDivID).html(result);
                loadProjectUsersGrid(projectID);
                $('#projectTabs a[href="#projectUsersPanel"]').tab('show');
                utils.jumpToElement("#ProjectDetailsLevel1");
            }
        });
    }
    function loadprojectEscalationsConfigSQLGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectEscalationConfigGSQLGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigGSQLGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            error: KendoGridDataSourceError,
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectEscalationSQLConfigs?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectEscalationSQLConfig",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectEscalationSQLConfig",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectEscalationSQLConfig?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        SqlQuery: { validation: { required: true } },
                        IsStoredProcedure: { type: "boolean" },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        TargetDbName: { validation: { required: false } },
                        TargetDbID: { validation: { required: true } },
                        BreachInclusion: { validation: { required: true } },
                        BreachInclusionName: { validation: { required: true } },
                    }
                },
                errors: KendoGridSchemaErrors
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigGSQLGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            resizable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy", { text: "Run", click: showEscalationsFrameWorkUserExecuteWindow }], title: "&nbsp;", width: "250px" },
                { field: "Name", title: "Name", width: "200px" },
                { field: "Description", title: "Description", width: "200px", template: "<div data-toggle='tooltip' title='#:Description#'>#:Description#</div>" },
                { field: "IsActive", title: "Is Active", width: "100px" },
                { field: "SqlQuery", title: "Sql Query", width: "170px" },
                { field: "IsStoredProcedure", title: "Is Stored Procedure", width: "100px" },
                { field: "ScheduleId", title: "Schedule", width: "120px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "TargetDbID", title: "Target Db", width: "120px", editor: TargetDbDropDownEditorRuleConfig, template: "#=TargetDbName#" },
                { field: "BreachInclusion", title: "Breach Inclusion", width: "160px", editor: BreachInclusionDropDownEditor, template: "#=BreachInclusionName#" },
            ],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var selectedRowObjectId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.level3TabStripClass).show();
                    currentEscalationsGridID = pageVarsPri.tags.projectEscalationConfigGSQLGridId;
                    loadprojectEscalationsConfigGenericRuleBreachSourcesGrid(selectedRowObjectId, pageVarsPri.tags.projectEscalationConfigSQLBreachSourceRuleGridId);
                   // loadprojectEscalationsConfigGenericRuleConfigBreachSourcesGrid(selectedRowObjectId, pageVarsPri.tags.projectEscalationConfigSQLBreachSourceRuleGridId);

                    utils.jumpToElement('#tabstrip100');
                }
            }
        });
    }
    function loadprojectEscalationsConfigGenericGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectEscalationConfigGenericGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigGenericGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            error: KendoGridDataSourceError,
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectEscalationGenericConfigs?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectEscalationGenericConfig",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectEscalationGenericConfig",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectEscalationGenericConfig?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                       // InternalSentFromEmail: { editable: false, validation: { required: false } },
                        //InternalSentFromUserName: { editable: false, validation: { required: false } },
                        EmailBodyTemplate: { validation: { required: true } },
                        EmailSubjectTemplate: { validation: { required: true } },
                        EmailAttachementTemplate: { validation: { required: false } },
                        CreateAttachementUsingTemplate: { type: "boolean" },
                        Description: { validation: { required: false } },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        Action: { validation: { required: true } },
                        EmailActionName: { validation: { required: true } },
                        PdfPageWidth: { type: 'number', validation: { required: false }, format: "{0:d}" },
                        PdfPageHeight: { type: 'number', validation: { required: false }, format: "{0:d}" },
                        PdfMargins: { validation: { required: false } },
                        PDFHeaderHtml: { validation: { required: false } },
                        PDFHeaderMargins: { validation: { required: false } },
                        PDFFooterHtml: { validation: { required: false } },
                        PDFFooterMargins: { validation: { required: false } },
                        EmailBreachColumnName: { validation: { required: false } },
                        EmailAddress: { validation: { required: false } },
                        DestinationPath: { validation: { required: false } },
                        BreachInclusion: { validation: { required: true } },
                        BreachInclusionName: { validation: { required: true } },
                        ArchiveBreachesOnSuccess: { type: "boolean" },
                        GroupBreachesColumnName: { validation: { required: true } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                },
                errors: KendoGridSchemaErrors
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigGenericGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            resizable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { command: ["edit", "destroy", { text: "Run", click: showEscalationsFrameWorkUserExecuteWindow }], title: "&nbsp;", width: "250px" },
                { field: "Name", title: "Name", width: "200px" },
                { field: "Description", title: "Description", width: "200px", template: "<div data-toggle='tooltip' title='#:Description#'>#:Description#</div>" },
                { field: "IsActive", title: "Is Active", width: "100px" },
                {
                    field: "EmailBodyTemplate",
                    title: "Email Body Template",
                    width: "180px",
                    template: "<div data-toggle='tooltip' title='#:EmailBodyTemplate#'>#:EmailBodyTemplate#</div>",
                    editor: EscalationConfigGridDropDownEditorEmailBodyTemplate
                },
                {
                    field: "EmailSubjectTemplate",
                    title: "Email Subject Template",
                    width: "200px",
                    template: "<div data-toggle='tooltip' title='#:EmailSubjectTemplate#'>#:EmailSubjectTemplate#</div>",
                    editor: EscalationConfigGridDropDownEditorEmailSubjectTemplate
                },
                {
                    field: "EmailAttachementTemplate",
                    title: "Email Attachement Template",
                    width: "220px",
                    template: "<div data-toggle='tooltip' title='#:EmailAttachementTemplate#'>#:EmailAttachementTemplate#</div>",
                    editor: EscalationConfigGridDropDownEditorEmailAttachmentTemplate
                },
                { field: "PdfPageWidth", title: "Pdf Page Width", width: "150px" },
                { field: "PdfPageHeight", title: "Pdf Page Height", width: "150px" },
                {
                    title: "Pdf Margins (t,r,b,l)",
                    field: "PdfMargins",
                    headerTemplate: '<span style="" title="in the format of comma seperated integers: top,right,bottom,left">Pdf Margins (i)</span>',
                    width: "150px"
                },
                { field: "CreateAttachementUsingTemplate", title: "Attach Document Using template", width: "160px" },
                { field: "PDFHeaderHtml", title: "Pdf Header HTML", width: "170px", editor: textAreaEditor },
                {
                    title: "Pdf Header Margins (t,r,b,l)",
                    field: "PDFHeaderMargins",
                    headerTemplate: '<span style="" title="in the format of comma seperated integers: top,right,bottom,left">Pdf Header Margins (i)</span>',
                    width: "200px"
                },
                { field: "PDFFooterHtml", title: "Pdf Footer HTML", width: "170px", editor: textAreaEditor },
                {
                    title: "Pdf Footer Margins (t,r,b,l)",
                    field: "PDFFooterMargins",
                    headerTemplate: '<span style="" title="in the format of comma seperated integers: top,right,bottom,left">Pdf Footer Margins (i)</span>',
                    width: "200px"
                },
                { field: "ScheduleId", title: "Schedule", width: "120px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "Action", title: "Email Action", width: "200px", editor: EmailActionDropDownEditor, template: "#=EmailActionName#" },
                { field: "DestinationPath", title: "Action Destination", width: "170px" },
                { field: "EmailBreachColumnName", title: "Email Breach Column Name", width: "230px" },
                { field: "EmailAddress", title: "Destination Email Address", width: "200px"},//, editor: EmailEditorWithValidatior },
                { field: "BreachInclusion", title: "Breach Inclusion", width: "160px", editor: BreachInclusionDropDownEditor, template: "#=BreachInclusionName#" },
                { field: "ArchiveBreachesOnSuccess", title: "Archive Breaches On Success", width: "250px" },
                { field: "GroupBreachesColumnName", title: "Group Breaches By Column", width: "220px" },
                ],//, locked:true }],
            // { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var selectedRowObjectId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.level3TabStripClass).show();
                    currentEscalationsGridID = pageVarsPri.tags.projectEscalationConfigGenericGridId;
                    loadprojectEscalationsConfigGenericRuleBreachSourcesGrid(selectedRowObjectId, pageVarsPri.tags.projectEscalationConfigGenericBreachSourceRuleGridId);
                    //loadprojectEscalationsConfigGenericRuleConfigBreachSourcesGrid(selectedRowObjectId, pageVarsPri.tags.projectEscalationConfigGenericBreachSourceRuleGridId);

                    utils.jumpToElement('#tabstrip100');
                }
            }
        });
    }
    var textAreaEditor = function (container, options) {
        $('<textarea cols="100" style="width:120%;height:100px;" data-bind="value: ' + options.field + '"></textarea>').appendTo(container);
    };
    function loadprojectEscalationsConfigUsersGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectEscalationConfigUsersGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigUsersGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectEscalationUsersConfigs?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectEscalationUsersConfig",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectEscalationUsersConfig",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectEscalationUsersConfig?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        //InternalSentFromEmail: { editable: false, validation: { required: false } },
                        //InternalSentFromUserName: { editable: false, validation: { required: false } },
                        EmailBodyTemplate: { validation: { required: true } },
                        EmailSubjectTemplate: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        BreachCount: { type: 'number', validation: { required: true, min: 1, max: 10 } },
                        AttachExcelOfBreaches: { type: "boolean" },//, validation: { min: 0, required: true } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigUsersGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            resizable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "120px", template: "<div data-toggle='tooltip' title='#:Description#'>#:Description#</div>" },
                { field: "IsActive", title: "Is Active", width: "80px" },
                { field: "BreachCount", title: "Breach Count", width: "120px", format: '{0:n0}' },
                //{
                //    field: "InternalSentFromEmail",
                //    title: "Sent From Email",
                //    width: "120px",
                //    editor: EmailEditorWithValidatior
                //},
                //{ field: "InternalSentFromUserName", title: "Sent From User Name", width: "120px" },
                { field: "EmailBodyTemplate",
                    title: "Email Body Template",
                    width: "120px",
                    template: "<div data-toggle='tooltip' title='#:EmailBodyTemplate#'>#:EmailBodyTemplate#</div>",
                    editor: EscalationConfigGridDropDownEditorEmailBodyTemplate
                },
                {
                    field: "EmailSubjectTemplate",
                    title: "Email Subject Template",
                    width: "120px",
                    template: "<div data-toggle='tooltip' title='#:EmailSubjectTemplate#'>#:EmailSubjectTemplate#</div>",
                    editor: EscalationConfigGridDropDownEditorEmailSubjectTemplate
                },
                { field: "ScheduleId", title: "Schedule", width: "120px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "AttachExcelOfBreaches", title: "Attach Breaches Excel", width: "80px"},
                { command: ["edit", "destroy", { text: "Run", click: showEscalationsFrameWorkUserExecuteWindow }], title: "&nbsp;", width: "250px", locked:true }],//, locked:true }],
               // { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var selectedRowObjectId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.level3TabStripClass).show();

                    loadprojectEscalationsConfigUsersRecipientsGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigUsersRuleBreachSourcesGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigUsersRuleConfigBreachSourcesGrid(selectedRowObjectId);

                    if (pageVarsPri.vars.IsVTProject == 'true') {
                        loadprojectEscalationsConfigUsersProjectBreachSourcesGrid(selectedRowObjectId);
                        loadprojectEscalationsConfigUsersOfficeBreachSourcesGrid(selectedRowObjectId);
                        loadprojectEscalationsConfigUsersTeamBreachSourcesGrid(selectedRowObjectId);
                        loadprojectEscalationsConfigUsersUserBreachSourcesGrid(selectedRowObjectId);

                        utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUsersRecipientsGridId);
                        utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceProjectGridId);
                        utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceOfficeGridId);
                        utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceTeamGridId);
                        utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceUserGridId);
                    }

                    utils.jumpToElement('#tabstrip9');
                    
                    //utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleGridId);
                    //utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUsersBreachSourceRuleConfigGridId);
                    //utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigUserEscalationsHistoryPanelContentId);
                    //Load the email history for the escalations config.
                    loadprojectEscalationsConfigEscalationsHistoryPartialView(selectedRowObjectId, pageVarsPri.tags.projectEscalationConfigUserEscalationsHistoryPanelContentId);
                }
            }
        });
    }
    function loadprojectEscalationsConfigRolesGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectEscalationConfigRolesGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectEscalationConfigRolesGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectEscalationRolesConfigs?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectEscalationRolesConfig",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectEscalationRolesConfig",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProjectEscalationRolesConfig?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        OverrideRecipientEmail: { validation: {required: false} },
                        //InternalSentFromEmail: { editable: false, validation: { required: true } },
                        //InternalSentFromUserName: { editable: false, validation: { required: true } },
                        EmailBodyTemplate: { validation: { required: true } },
                        EmailSubjectTemplate: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        ScheduleName: { validation: { required: false } },
                        ScheduleId: { validation: { required: true } },
                        ActionName: { validation: { required: false } },
                        ActionId: { validation: { required: true } },
                        BreachCount: { type: 'number', validation: { required: true, min: 1, max: 10 } },
                        AttachExcelOfBreaches: { type: "boolean" },//, validation: { min: 0, required: true } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectEscalationConfigRolesGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            resizable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "Name", title: "Name", width: "120px" },
                { field: "Description", title: "Description", width: "160px" },
                { field: "IsActive", title: "Is Active", width: "100px" },
                { field: "BreachCount", title: "Breach Count", width: "120px", format: '{0:n0}' },
                { field: "OverrideRecipientEmail",
                    //title: "Override Recipient Email",
                    width: "190px",
                    editor: EmailEditorWithValidatiorNotRequired,
                    headerTemplate: '<span title="Add an Email to be used in place of the recipient user email. Good for testing email output during validation.">Override Recipient Email</span>'
                },
                { field: "ScheduleId", title: "Schedule", width: "120px", editor: ScheduleDropDownEditorRuleConfig, template: "#=ScheduleName#" },
                { field: "ActionId", title: "Action", width: "140px", editor: ActionDropDownEditorEscalationConfigRoles, template: "#=ActionName#" },
                { field: "EmailBodyTemplate",
                    title: "Email Body Template",
                    width: "240px",
                    template: "<div data-toggle='tooltip' title='#:EmailBodyTemplate#'>#:EmailBodyTemplate#</div>",
                    editor: EscalationConfigGridDropDownEditorEmailBodyTemplate
                },
                {
                    field: "EmailSubjectTemplate",
                    title: "Email Subject Template",
                    width: "220px",
                    template: "<div data-toggle='tooltip' title='#:EmailSubjectTemplate#'>#:EmailSubjectTemplate#</div>",
                    editor: EscalationConfigGridDropDownEditorEmailSubjectTemplate
                },

                { field: "AttachExcelOfBreaches", title: "Attach Breaches Excel", width: "160px" },
                { command: ["edit", "destroy", { text: "Test", click: showEscalationsFrameWorkRoleExecuteWindow }], title: "&nbsp;", width: "250px", locked: true }],//, locked:true }],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var selectedRowObjectId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.level3TabStripClass).show();
                    //$(pageVarsPri.tags.level4TabStripClass).show();
                    loadprojectEscalationsConfigRolesProjectBreachSourcesGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigRolesOfficeBreachSourcesGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigRolesTeamBreachSourcesGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigRolesUserBreachSourcesGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigRolesRuleBreachSourcesGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigRolesRuleConfigBreachSourcesGrid(selectedRowObjectId);
                    //loadprojectEscalationsConfigRolesRuleEscalationsHistoryGrid(selectedRowObjectId);
                    loadprojectEscalationsConfigEscalationsHistoryPartialView(selectedRowObjectId, pageVarsPri.tags.projectEscalationConfigRoleEscalationsHistoryPanelContentId)

                    utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceProjectGridId);
                    utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceOfficeGridId);
                    utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceTeamGridId);
                    utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceUserGridId);
                    utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleGridId);
                    utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigRoleBreachSourceRuleConfigGridId);
                    utils.jumpToElement(pageVarsPri.tags.projectEscalationConfigRoleEscalationsHistoryPanelContentId);
                }
            }
        });
    }
    function loadProjectRulesGrid(projectGuid) {
        if ($(pageVarsPri.tags.projectRulesGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectRulesGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectRules?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateRule",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyRule",
                    dataType: "json",
                    cache: false
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateRule?projectGuid=" + projectGuid,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pagesize: 200,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { editable: false, nullable: true },
                        Name: { validation: { required: true } },
                        Description: { validation: { required: false } },
                        AdditionalDescription: { validation: { required: false } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectRulesGridId).kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "Name", title: "Name", width: "250px" },
                { field: "IsActive", title: "Is Active", width: "100px" },
                { field: "Description", title: "Description", width: "500px", template: "<div data-toggle='tooltip' title='#=Description#'>#:Description#</div>" },
                {
                    field: "AdditionalDescription",
                    headerTemplate: '<span title="This can be used in the email templates e.g. how to resolve the issue.">Additional Information</span>',
                    width: "500px",
                    template: "<div data-toggle='tooltip' title='#=AdditionalDescription#'>#:AdditionalDescription#</div>"
                },
                { command: ["edit", "destroy", { text: "Execute Rule Test", click: showExecuteRuleSnapShotBreachDetails }], title: "&nbsp;", width: "300px", locked: true }],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var ruleId = this.dataItem(selectedRows[0]).Id;
                    $(pageVarsPri.tags.level3TabStripClass).show();
                    $(pageVarsPri.tags.level4TabStripClass).hide();
                    loadRuleConfigurationsGrid(ruleId);
                    loadruleExchangeConfigurationsGrid(ruleId);
                    loadruleExcelConfigurationsGrid(ruleId);
                    utils.jumpToElement(pageVarsPri.tags.ruleConfigurationsGridId);
                    if (pageVarsPri.vars.IsVTProject == "false" && pageVarsPri.vars.isMicroServiceProject == false) {
                        utils.jumpToElement(pageVarsPri.tags.ruleExchangeConfigurationsGridId);
                        utils.jumpToElement(pageVarsPri.tags.ruleExcelConfigurationsGridId);
                    }
                }
            }
        });
    }
    pub.loadProjectsGrid = function () {
        loadProjectsGrid();
        $(pageVarsPri.tags.level3TabStripClass).hide();
    }
    function loadProjectsGrid() {
        if ($(pageVarsPri.tags.projectsGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectsGridId).kendoGrid('destroy').empty();
        }
        
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            error: KendoGridDataSourceError,
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjects",
                    dataType: "json"
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProject",
                    dataType: "json"
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProject",
                    dataType: "json"
                },
                create: {
                    url: crudServiceBaseUrl + "/CreateProject",
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            batch: true,
            pageSize: 10,
            schema: {
                model: {
                    id: "ProjectUniqueKey",
                    fields: {
                        ProjectUniqueKey: { editable: false, nullable: true },
                        ProjectName: { validation: { required: true } },
                        ProjectDisplayName: { validation: { required: true } },
                        ProjectDescription: { validation: { required: true } },
                        ProjectOwner: { validation: { required: true } },
                        ProjectSenderEmail: { editable: true, validation: { required: false } },
                        ProjectSenderDisplayName: { editable: true, validation: { required: false } },
                        EmailRazorTemplateBodyPath: { validation: { required: false } },
                        EmailRazorTemplateSubjectPath: { validation: { required: false } },
                        EmailRazorTemplateAttachmentPath: { validation: { required: false } },
                        PojectTypeName: { validation: { required: false } },
                        ProjectTypeId: { validation: { required: true } },
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                        //IsSystemProject: { type: "boolean" },//, validation: { min: 0, required: true } }
                    }
                },
                errors: KendoGridSchemaErrors
            }
        });

        $(pageVarsPri.tags.projectsGridId).kendoGrid({
            dataSource: dataSource,
            dataBound: function (e) {
                this.element.find('tbody tr:first').addClass('k-state-selected');
                var selectedRows = this.select();
                var selectedDataItems = [];

                if (selectedRows.length > 0) {
                    var projectKey = this.dataItem(selectedRows[0]).ProjectUniqueKey;
                    var isVTProject = this.dataItem(selectedRows[0]).ProjectTypeId == 2 ? "true" : "false";
                    var isMicroServiceProject = this.dataItem(selectedRows[0]).ProjectTypeId == 3 ? "true" : "false";
                    pageVarsPri.vars.IsVTProject = isVTProject;
                    pageVarsPri.vars.IsMicroServiceProject = isMicroServiceProject;
                    loadAdminContentPartialViewForProjectType(projectKey, "#projectsAdminContent");
                }

                //IsSystemAdmin
                // We dont want to be able to delete micro service projects.
                // We dont want non system admins to be able to delete projects either.
                $(pageVarsPri.tags.projectsGridId + " tbody tr .k-grid-delete").each(function () {
                    var currentDataItem = $(pageVarsPri.tags.projectsGridId).data("kendoGrid").dataItem($(this).closest("tr"));
                    if (currentDataItem.ProjectTypeId == 3 || pageVarsPri.vars.IsSystemAdmin == "false")
                    {
                        $(this).remove();
                    }
                });

                $(pageVarsPri.tags.projectsGridId + " tbody tr .k-grid-edit").each(function () {
                    var currentDataItem = $(pageVarsPri.tags.projectsGridId).data("kendoGrid").dataItem($(this).closest("tr"));
                    if (currentDataItem.ProjectTypeId == 3 || pageVarsPri.vars.IsSystemAdmin == "false") {
                        $(this).remove();
                    }
                });
                // Only show execute for defauilt project types
                $(pageVarsPri.tags.projectsGridId + " tbody tr .k-grid-ExecuteAll").each(function () {
                    var currentDataItem = $(pageVarsPri.tags.projectsGridId).data("kendoGrid").dataItem($(this).closest("tr"));
                    if (currentDataItem.ProjectTypeId == 3 || currentDataItem.ProjectTypeId == 2) {
                        $(this).remove();
                    }
                });
            },
            selectable: true,
            pageable: pageVarsPri.vars.IsSystemAdmin == true ? true : false,
            resizable: pageVarsPri.vars.IsSystemAdmin == true ? true : false,
            filterable: pageVarsPri.vars.IsSystemAdmin == true ? true : false,
            groupable: pageVarsPri.vars.IsSystemAdmin == true ? true : false,
            sortable: pageVarsPri.vars.IsSystemAdmin == true ? true : false,
            height: pageVarsPri.vars.IsSystemAdmin == false ? 0 : 0,
            toolbar: pageVarsPri.vars.IsSystemAdmin == true ? ["create"] : [],
            columns: [
                {
                    command: pageVarsPri.vars.IsSystemAdmin == true ? ["edit", { name: "destroy", text: "Remove" }, { text: "Execute All", click: showExecuteProject }] : ["edit", { text: "Execute All", click: showExecuteProject }],
                    title: "&nbsp;",
                    width: "265px"
                },
                { field: "ProjectName", title: "Project Name", width: "150px" },
                { field: "ProjectDisplayName", title: "Project Display Name", width: "180px" },
                { field: "ProjectDescription", title: "Project Description", width: "200px" },
                { field: "ProjectOwner", title: "Project Owner", width: "150px" },
                { field: "ProjectUniqueKey", title: "Project ID", width: "290px" },
                { field: "IsActive", title: "Is Active", width: "100px" },
                {
                    field: "ProjectSenderEmail",
                    title: "Project Sender Email",
                    width: "250px",
                    editor: EmailEditorWithValidatiorNotRequired
                },
                { field: "ProjectSenderDisplayName", title: "Project Sender Display Name", width: "250px" },
                { field: "EmailRazorTemplateBodyPath", title: "Email Body Template Path", width: "250px" },
                { field: "EmailRazorTemplateSubjectPath", title: "Email Subject Template Path", width: "250px" },
                { field: "EmailRazorTemplateAttachmentPath", title: "Email Attachment Template Path", width: "250px" },
                { field: "ProjectTypeId", title: "Project Type", width: "150px", editor: ProjectTypeDropDownEditor, template: "#=PojectTypeName#" },

                //{ field: "IsSystemProject", title: "Is System Project", width: "120px" },
                
            ],
            editable: "popup",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];

                if (selectedRows.length > 0) {
                    var projectKey = this.dataItem(selectedRows[0]).ProjectUniqueKey
                    var isVTProject = this.dataItem(selectedRows[0]).ProjectTypeId == 2 ? "true" : "false";
                    var isMicroServiceProject = this.dataItem(selectedRows[0]).ProjectTypeId == 3 ? "true" : "false";
                    pageVarsPri.vars.IsVTProject = isVTProject;
                    pageVarsPri.vars.IsMicroServiceProject = isMicroServiceProject;
                    loadAdminContentPartialViewForProjectType(projectKey, "#projectsAdminContent");
                }
            }
        });
    }
    function loadProjectsBreaches(projectId) {
        if ($(pageVarsPri.tags.projectBreachesGridId).kendoGrid()) {
            $(pageVarsPri.tags.projectBreachesGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListProjectBreaches?projectId=" + projectId,
                    dataType: "json"
                },
                update: {
                    url: crudServiceBaseUrl + "/UpdateProjectBreach",
                    dataType: "json"
                },
                destroy: {
                    url: crudServiceBaseUrl + "/DestroyProjectBreach",
                    dataType: "json"
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            serverSorting: true,
            serverFiltering: true,
            serverGrouping: true,
            serverPaging: true,
            batch: false,
            pageSize: 150,
            schema: {
                data: "data",
                total: "total",
                model: {
                    id: "Id",
                    fields: {
                        RuleName: { editable: false, nullable: false},
                        RuleConfigurationName: { editable: false, validation: { required: false } },
                        UserName: { editable: false, validation: { required: false } },
                        OfficeName: { editable: false, validation: { required: false } },
                        TeamName: { editable: false, validation: { required: false } },
                        ContextRef: { editable: false, validation: { required: false } },
                        BreachDisplayText: { editable: false, validation: { required: false } },
                        BreachDisplayAlternateText: { editable: false, validation: { required: false } },
                        TimeStamp: { editable: false, type: "date", validation: { required: false } },
                        TimeStampDateOnly: { editable: false, type: "date", validation: { required: false } },
                        RuleBreachFieldOne: { editable: false, validation: { required: false } },
                        RuleBreachFieldTwo: { editable: false, validation: { required: false } },
                        RuleBreachFieldThree: { editable: false, validation: { required: false } },
                        RuleBreachFieldFour: { editable: false, validation: { required: false } },
                        RuleBreachFieldFive: { editable: false, validation: { required: false } },
                        IsArchived: { editable: true, type: "boolean", nullable: true },//, validation: { min: 0, required: true } },
                        //IsSystemProject: { type: "boolean" },//, validation: { min: 0, required: true } }
                    }
                }
            }
        });

        $(pageVarsPri.tags.projectBreachesGridId).kendoGrid({
            dataSource: dataSource,
            dataBound: function (e) {
            },
            resizable: true,
            selectable: true,
            filterable: true,
            groupable: true,
            sortable: true,
            pageable: true,
            height: 400,
            toolbar: ["excel"],
            excel: {
                fileName: "VirtualTrainerBreaches.xlsx",
                proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
                filterable: true
            },
            columns: pageVarsPri.vars.IsVTProject == 'true' ? [
                { command: ['destroy', 'edit'], title: "&nbsp;", width: "120px" },
                { field: "RuleName", title: "Rule Name", width: "120px" },
                { field: "RuleConfigurationName", title: "Rule Configuration Name", width: "120px" },
                { field: "ContextRef", title: "Context Ref", width: "120px" },
                { field: "BreachDisplayText", title: "Breach Message", width: "120px", template: "<div data-toggle='tooltip' title='#=BreachDisplayText#'>#:BreachDisplayText#</div>" },
                { field: "OfficeName", title: "Office Name", width: "120px" },
                { field: "TeamName", title: "Team Name", width: "120px" },
                { field: "UserName", title: "User Name", width: "120px" },
                { field: "IsArchived", title: "Is Archived", width: "120px" },
                { field: "TimeStampDateOnly", title: "Date Added", width: "120px", format: "{0:dd-MMM-yyyy}" },
                { field: "TimeStamp", title: "Time Added", width: "120px", format: "{0:hh:mm}" },
                //{ field: "IsSystemProject", title: "Is System Project", width: "120px" },
            ] : [
                { command: ['destroy', 'edit'], title: "&nbsp;", width: "120px" },
                { field: "RuleName", title: "Rule Name", width: "120px" },
                { field: "RuleConfigurationName", title: "Rule Configuration Name", width: "120px" },
                { field: "ContextRef", title: "Context Ref", width: "120px" },
                { field: "BreachDisplayText", title: "Breach Message", width: "120px", template: "<div data-toggle='tooltip' title='#=BreachDisplayText#'>#:BreachDisplayText#</div>" },
                { field: "BreachDisplayAlternateText", title: "Breach Additional Message", width: "120px", template: "<div data-toggle='tooltip' title='#=BreachDisplayAlternateText#'>#:BreachDisplayAlternateText#</div>" },
                { field: "TimeStampDateOnly", title: "Date Added", width: "120px", format: "{0:dd-MMM-yyyy}" },
                { field: "TimeStamp", title: "Time Added", width: "120px", format: "{0:hh:mm}" },
                { field: "IsArchived", title: "Is Archived", width: "120px" },
                { field: "RuleBreachFieldOne", title: "Return Field One", width: "120px" },
                { field: "RuleBreachFieldTwo", title: "Return Field Two", width: "120px" },
                { field: "RuleBreachFieldThree", title: "Return Field Three", width: "120px" },
                { field: "RuleBreachFieldFour", title: "Return Field Four", width: "120px" },
                { field: "RuleBreachFieldFive", title: "Return Field Five", width: "120px" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    pub.ExecuteEscalationsRuleUserConfig = function (escalationConfigId) {
        $(pageVarsPri.tags.escalationUserConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        $(pageVarsPri.tags.escalationUserConfigExecutionyButtonId).attr("disabled", "disabled");
        $(pageVarsPri.tags.escalationUserConfigExecutionTemplateResultsMessageId).html('');

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();
        $.ajax({
            url: crudServiceBaseUrl + '/ExecuteEscalationsRuleUserConfig?escalationConfigId=' + escalationConfigId + '&projectId=' + selectedProjectUniqueKey,
            contentType: 'application/html; charset=utf-8',
            type: 'GET',
            dataType: 'html',
            async: true
        })
        .success(function (result) {
            if (result) {
                $(pageVarsPri.tags.escalationUserConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'blue');
                $(pageVarsPri.tags.escalationUserConfigExecutionTemplateResultsMessageId).html(result);
            }
            else {
                $(pageVarsPri.tags.escalationUserConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'green');
            }
            $(pageVarsPri.tags.escalationUserConfigExecutionyButtonId).removeAttr("disabled");
        })
        .error(function (xhr, status) {
            $(pageVarsPri.tags.escalationUserConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'red');
            $(pageVarsPri.tags.escalationUserConfigExecutionyButtonId).removeAttr("disabled");
        })
    }
    pub.ExecuteEscalationsRuleConfigTest = function (escalationConfigId)
    {
        $(pageVarsPri.tags.escalationRoleConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        $(pageVarsPri.tags.rulesnapshotrunQueryButtonId).attr("disabled", "disabled");
        $(pageVarsPri.tags.escalationRoleConfigExecutionTemplateResultsMessageId).html('');

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var selectedProjectUniqueKey = getSelectedProject();

        $.ajax({
            url: crudServiceBaseUrl + '/ExecuteEscalationsRuleRoleConfig?escalationConfigId=' + escalationConfigId + '&projectId=' + selectedProjectUniqueKey,
            contentType: 'application/html; charset=utf-8',
            type: 'GET',
            dataType: 'html',
            async: true
        })
        .success(function (result) {
            if (result) {
                $(pageVarsPri.tags.escalationRoleConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'blue');
                $(pageVarsPri.tags.escalationRoleConfigExecutionTemplateResultsMessageId).html(result);
            }
            else {
                $(pageVarsPri.tags.escalationRoleConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'green');
            }
            $(pageVarsPri.tags.rulesnapshotrunQueryButtonId).removeAttr("disabled");
        })
        .error(function (xhr, status) {
            $(pageVarsPri.tags.escalationRoleConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'red');
            $(pageVarsPri.tags.rulesnapshotrunQueryButtonId).removeAttr("disabled");
        })
    }
    pub.ExecuteExchangeAccountConfigTest = function (configId)
    {
        $(pageVarsPri.tags.executeExchangeAccountExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        $(pageVarsPri.tags.exchangeAccountTestQueryButtonId).attr("disabled", "disabled");
        $(pageVarsPri.tags.exchangeAccountTestTemplateResultsMessageId).html('');

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;

        $.ajax({
            url: crudServiceBaseUrl + '/TestExchangeAccountConfig?configId=' + configId,
            contentType: 'application/html; charset=utf-8',
            type: 'GET',
            dataType: 'html',
            async: true
        })
        .success(function (result) {
            if (result.toString().startsWith("\"Exception")) {
                $(pageVarsPri.tags.executeExchangeAccountExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'red');
            }
            else {
                $(pageVarsPri.tags.executeExchangeAccountExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'green');
            }
            $(pageVarsPri.tags.exchangeAccountTestTemplateResultsMessageId).html(result);
            $(pageVarsPri.tags.exchangeAccountTestQueryButtonId).removeAttr("disabled");
        })
        .error(function (xhr, status) {
            $(pageVarsPri.tags.executeExchangeAccountExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'red');
            $(pageVarsPri.tags.exchangeAccountTestQueryButtonId).removeAttr("disabled");
        })
    }
    pub.LoadActurisImportSnapShottGrid = function (importId) {
        $(pageVarsPri.tags.executeActurisImportSnapShotDetailsWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        $(pageVarsPri.tags.acturisImportSnapShotQueryButtonId).attr("disabled", "disabled");

        if ($(pageVarsPri.tags.acturisImportSnapShotGridId).kendoGrid()) {
            $(pageVarsPri.tags.acturisImportSnapShotGridId).kendoGrid('destroy').empty();
        }

        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListActurisImportSnapShot?importId=" + importId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            serverSorting: true,
            serverFiltering: true,
            serverGrouping: false,
            serverPaging: true,
            batch: false,
            pageSize: 1000,
            schema: {
                data: "data",
                total: "total",
                model: {
                    id: "Id",
                    fields: {
                        UserKey: { editable: false, nullable: false },
                        UserName: { editable: false, validation: { required: false } },
                        UserEmail: { editable: false, validation: { required: false } },
                        UserRole: { editable: false, validation: { required: false } },
                        UserStatusKey: { editable: false, validation: { required: false } },
                        UserStatusDescription: { editable: false, validation: { required: false } },
                        TeamKey: { editable: false, nullable: false },
                        TeamName: { editable: false, validation: { required: false } },
                        TeamStatusKey: { editable: false, validation: { required: false } },
                        TeamStatusDescription: { editable: false, validation: { required: false } },
                        OfficeKey: { editable: false, validation: { required: false } },
                        OfficeName: { editable: false, validation: { required: false } },
                        OrganisationKey: { editable: false, validation: { required: false } },
                        OrganisationName: { editable: false, validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.acturisImportSnapShotGridId).kendoGrid({
            dataSource: dataSource,
            dataBound: function (e) {
                $(pageVarsPri.tags.executeActurisImportSnapShotDetailsWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'green');
                $(pageVarsPri.tags.acturisImportSnapShotQueryButtonId).removeAttr("disabled");
            },
            resizable: true,
            selectable: true,
            filterable: true,
            groupable: true,
            scrollable: { virtual: true },
            sortable: true,
            pageable: true,
            height: 400,
            toolbar: ["excel"],
            excel: {
                fileName: "VirtualTrainerSnapShotImport.xlsx",
                proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
                filterable: true
            },
            columns: [
                { field: "OrganisationName", title: "Organisation Name", width: "120px" },
                { field: "OrganisationKey", title: "Organisation Key", width: "120px" },
                { field: "OfficeName", title: "Office Name", width: "120px" },
                { field: "OfficeKey", title: "Office Key", width: "120px" },
                { field: "TeamName", title: "Team Name", width: "120px" },
                { field: "TeamKey", title: "Team Key", width: "120px" },
                { field: "TeamStatusKey", title: "Team Status Key", width: "120px" },
                { field: "TeamStatusDescription", title: "Team Status Description", width: "120px" },

                { field: "UserName", title: "User Name", width: "120px" },
                { field: "UserEmail", title: "User Email", width: "120px" },
                { field: "UserKey", title: "User Key", width: "120px" },
                { field: "UserRole", title: "User Role", width: "120px" },
                { field: "UserStatusDescription", title: "User Status Description", width: "120px" },
                { field: "UserStatusKey", title: "User Status Key", width: "120px" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    pub.ExecuteProjectRulesEtc = function (projectId) {
        $(pageVarsPri.tags.executeProjectExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        $(pageVarsPri.tags.projectExecuteButtonId).attr("disabled", "disabled");

        $.ajax({
            type: 'GET',
            url: pageVarsPri.urls.adminRoot + "/ExecuteVirtualTrainerForProject",
            data: {
                projectId: projectId,
            },
            dataType: 'html',
            success: function (result) {
                $(pageVarsPri.tags.executeProjectExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'green');
                $(pageVarsPri.tags.projectExecuteButtonId).removeAttr("disabled");
                //$(ParentDivID).html(result);
            },
            error: function (result) {
                $(pageVarsPri.tags.executeProjectExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'red');
                $(pageVarsPri.tags.projectExecuteButtonId).removeAttr("disabled");
            }
        });
    }
    pub.LoadRuleLOBSnapShotGrid = function (ruleId)
    {
        $(pageVarsPri.tags.executeRuleSnapShotBreachDetailsWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        $(pageVarsPri.tags.rulesnapshotrunQueryButtonId).attr("disabled", "disabled");

        if ($(pageVarsPri.tags.ruleSnapShotBreachesGridId).kendoGrid()) {
            $(pageVarsPri.tags.ruleSnapShotBreachesGridId).kendoGrid('destroy').empty();
        }
        //var projectId = getSelectedProject();
        var crudServiceBaseUrl = pageVarsPri.urls.adminRoot;
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: crudServiceBaseUrl + "/ListBreachesSnapShot?ruleId=" + ruleId,
                    dataType: "json",
                    cache: false
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options.models) {
                        return { models: kendo.stringify(options.models) };
                    }
                }
            },
            serverSorting: false,
            serverFiltering: false,
            serverGrouping: false,
            serverPaging: false,
            batch: false,
            pageSize: 1000,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        RuleName: { editable: false, nullable: false },
                        RuleConfigurationName: { editable: false, validation: { required: false } },
                        UserName: { editable: false, validation: { required: false } },
                        OfficeName: { editable: false, validation: { required: false } },
                        TeamName: { editable: false, validation: { required: false } },
                        ContextRef: { editable: false, validation: { required: false } },
                        BreachDisplayHTML: { editable: false, validation: { required: false } },
                        TimeStamp: { editable: false, type: "date", validation: { required: false } },
                        IsArchived: { editable: true, type: "boolean", nullable: true },
                        //RuleBreachFieldOne: { editable: false, validation: { required: false } },
                        //RuleBreachFieldTwo: { editable: false, validation: { required: false } },
                        //RuleBreachFieldThree: { editable: false, validation: { required: false } },
                        //RuleBreachFieldFour: { editable: false, validation: { required: false } },
                        //RuleBreachFieldFive: { editable: false, validation: { required: false } },
                        //RuleBreachFieldSix: { editable: false, validation: { required: false } },
                        //RuleBreachFieldSeven: { editable: false, validation: { required: false } },
                        //RuleBreachFieldEight: { editable: false, validation: { required: false } },
                        //RuleBreachFieldNine: { editable: false, validation: { required: false } },
                        //RuleBreachFieldTen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldEleven: { editable: false, validation: { required: false } },
                        //RuleBreachFieldTwelve: { editable: false, validation: { required: false } },
                        //RuleBreachFieldThirteen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldFourteen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldFifteen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldSixteen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldSeventeen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldEighteen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldNineteen: { editable: false, validation: { required: false } },
                        //RuleBreachFieldTwenty: { editable: false, validation: { required: false } },
                    }
                }
            }
        });

        $(pageVarsPri.tags.ruleSnapShotBreachesGridId).kendoGrid({
            dataSource: dataSource,
            dataBound: function (e) {
                $(pageVarsPri.tags.executeRuleSnapShotBreachDetailsWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', 'green');
                $(pageVarsPri.tags.rulesnapshotrunQueryButtonId).removeAttr("disabled");
            },
            resizable: true,
            selectable: true,
            filterable: true,
            groupable: true,
            sortable: true,
            pageable: true,
            reorderable:true,
            height: 400,
            toolbar: ["excel"],
            excel: {
                fileName: "VirtualTrainerSnapShotBreaches.xlsx",
                proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
                filterable: true
            },
            columns: pageVarsPri.vars.IsVTProject == 'true' ? [
                { field: "RuleName", title: "Rule Name", width: "120px" },
                { field: "RuleConfigurationName", title: "Rule Configuration Name", width: "120px" },
                { field: "ContextRef", title: "Context Ref", width: "120px" },
                { field: "BreachDisplayHTML", title: "Breach Message", width: "120px", template: "<div data-toggle='tooltip' title='#=BreachDisplayHTML#'>#:BreachDisplayHTML#</div>" },
                { field: "OfficeName", title: "Office Name", width: "120px" },
                { field: "TeamName", title: "Team Name", width: "120px" },
                { field: "UserName", title: "User Name", width: "120px" },
            ] : [
                { field: "RuleName", title: "Rule Name", width: "120px" },
                { field: "RuleConfigurationName", title: "Rule Configuration Name", width: "120px" },
                { field: "ContextRef", title: "Context Ref", width: "120px" },
                { field: "BreachDisplayText", title: "Breach Display Text", width: "120px", template: "<div data-toggle='tooltip' title='#=BreachDisplayText#'>#:BreachDisplayText#</div>" },
                { field: "RuleBreachFieldOne", title: "Field 1", width : "120px" },
                { field: "RuleBreachFieldTwo", title: "Field 2", width: "120px" },
                { field: "RuleBreachFieldThree", title: "Field 3", width: "120px" },
                { field: "RuleBreachFieldFour", title: "Field 4", width : "120px" },
                { field: "RuleBreachFieldFive", title: "Field 5", width: "120px" },
                { field: "RuleBreachFieldSix", title: "Field 6", width: "120px" },
                { field: "RuleBreachFieldSeven", title: "Field 7", width : "120px" },
                { field: "RuleBreachFieldEight", title: "Field 8", width: "120px" },
                { field: "RuleBreachFieldNine", title: "Field 9", width: "120px" },
                { field: "RuleBreachFieldTen", title: "Field 10", width : "120px" },
                { field: "RuleBreachFieldEleven", title: "Field 11", width: "120px" },
                { field: "RuleBreachFieldTwelve", title: "Field 12", width: "120px" },
                { field: "RuleBreachFieldThirteen", title: "Field 13", width: "120px" },
                { field: "RuleBreachFieldFourteen", title: "Field 14", width : "120px" },
                { field: "RuleBreachFieldFifteen", title: "Field 15", width: "120px" },
                { field: "RuleBreachFieldSixteen", title: "Field 16", width: "120px" },
                { field: "RuleBreachFieldSeventeen", title: "Field 17", width : "120px" },
                { field: "RuleBreachFieldEighteen", title: "Field 18", width: "120px" },
                { field: "RuleBreachFieldNineteen", title: "Field 19", width: "120px" },
                { field: "RuleBreachFieldTwenty", title: "Field 20", width: "120px" },
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
    function showExecuteActurisImportSnapShotBreachDetails(e) {
        e.preventDefault();

        var window;
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var detailsTemplate = kendo.template($(pageVarsPri.tags.acturisImportSnapShotTemplateId).html());
        window = $(pageVarsPri.tags.executeActurisImportSnapShotDetailsWindowId)
                .kendoWindow({
                    title: "Rule Breaches LOB Snap Shot",
                    modal: false,
                    visible: false,
                    resizable: true,
                    width: "70%",
                    actions: [
                        "Pin",
                        "Minimize",
                        "Maximize",
                        "Close"
                    ],
                }).data("kendoWindow");
        window.content(detailsTemplate(dataItem));
        $(pageVarsPri.tags.executeActurisImportSnapShotDetailsWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        window.center().open();
    }
    function showExecuteRuleSnapShotBreachDetails(e) {
        e.preventDefault();

        var window;
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var detailsTemplate = kendo.template($(pageVarsPri.tags.ruleSnapshottemplateId).html());
        window = $(pageVarsPri.tags.executeRuleSnapShotBreachDetailsWindowId)
                .kendoWindow({
                    title: "Rule Breaches LOB Snap Shot",
                    modal: false,
                    visible: false,
                    resizable: true,
                    width: "70%",
                    actions: [
                        "Pin",
                        "Minimize",
                        "Maximize",
                        "Close"
                    ],
                }).data("kendoWindow");
        window.content(detailsTemplate(dataItem));
        $(pageVarsPri.tags.executeRuleSnapShotBreachDetailsWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        window.center().open();
    }
    function showExecuteProject(e) {
        e.preventDefault();

        var window;
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var detailsTemplate = kendo.template($(pageVarsPri.tags.executeProjectTemplateId).html());
        window = $(pageVarsPri.tags.executeProjectExecutionWindowId)
                .kendoWindow({
                    title: "Execute All Rules and Escalations",
                    modal: false,
                    visible: false,
                    resizable: true,
                    width: "70%",
                    actions: [
                        "Pin",
                        "Minimize",
                        "Maximize",
                        "Close"
                    ],
                }).data("kendoWindow");
        window.content(detailsTemplate(dataItem));
        $(pageVarsPri.tags.executeProjectExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        window.center().open();
    }
    function showEscalationsFrameWorkUserExecuteWindow(e) {
        e.preventDefault();

        var window;
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var detailsTemplate = kendo.template($(pageVarsPri.tags.escalationUserConfigExecutionTemplateId).html());
        window = $(pageVarsPri.tags.escalationUserConfigExecutionWindowId)
                .kendoWindow({
                    title: "Execute Escalations Framework Configuration",
                    modal: false,
                    visible: false,
                    resizable: true,
                    width: "70%",
                    actions: [
                        "Pin",
                        "Minimize",
                        "Maximize",
                        "Close"
                    ],
                }).data("kendoWindow");
        window.content(detailsTemplate(dataItem));
        $(pageVarsPri.tags.escalationUserConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        window.center().open();
    }
    function showExchangeAccountTestWindow(e) {
        e.preventDefault();

        var window;
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var detailsTemplate = kendo.template($(pageVarsPri.tags.exchangeAccountTestTemplateId).html());
        window = $(pageVarsPri.tags.executeExchangeAccountExecutionWindowId)
                .kendoWindow({
                    title: "Test Exchange Account Configuration",
                    modal: false,
                    visible: false,
                    resizable: true,
                    width: "70%",
                    actions: [
                        "Pin",
                        "Minimize",
                        "Maximize",
                        "Close"
                    ],
                }).data("kendoWindow");
        window.content(detailsTemplate(dataItem));
        $(pageVarsPri.tags.executeExchangeAccountExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        window.center().open();
    }
    function showEscalationsFrameWorkRoleExecuteWindow(e) {
        e.preventDefault();

        var window;
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var detailsTemplate = kendo.template($(pageVarsPri.tags.escalationRoleConfigExecutionTemplateId).html());
        window = $(pageVarsPri.tags.escalationRoleConfigExecutionWindowId)
                .kendoWindow({
                    title: "Execute Escalations Framework Configuration",
                    modal: false,
                    visible: false,
                    resizable: true,
                    width: "70%",
                    actions: [
                        "Pin",
                        "Minimize",
                        "Maximize",
                        "Close"
                    ],
                }).data("kendoWindow");
        window.content(detailsTemplate(dataItem));
        $(pageVarsPri.tags.escalationRoleConfigExecutionWindowId).parent().find('.k-window-titlebar,.k-window-actions').css('backgroundColor', '#f35800');
        window.center().open();
    }
    function EmailEditorWithValidatior (container, options) {
        var input = $('<input type="email" data-email-msg="Invalid email!" class="k-textbox" required/>');
        input.attr("name", options.field);
        input.appendTo(container);
    }
    function EmailEditorWithValidatiorNotRequired(container, options) {
        var input = $('<input type="email" data-email-msg="Invalid email!" class="k-textbox" />');
        input.attr("name", options.field);
        input.appendTo(container);
    }
})(jQuery, app.admin, app.utils)