angApp.controller("SystemUsersController", function ($scope, $location, $http) {
    $scope.ShowSystemUserPerms = false;
    loadAllSystemUsersGrid($scope);

    function EmailEditorWithValidatior(container, options) {
        var input = $('<input type="email" data-email-msg="Invalid email!" class="k-textbox" required/>');
        input.attr("name", options.field);
        input.appendTo(container);
    }
    function ProjectDropDownEditorSystemUserProjectMembership(container, options) {
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
                        read: "/Admin" + '/ListProjectsForSystemUserProjectMembershipDropDown?SystemUserID=' + selectedSystemUserId,
                        cache: false
                    }
                }
            });
    }
    function getSelectedSystemUserId() {
        var grid = $("#systemUsersGrid").data("kendoGrid");
        if (grid) {
            var selectedItem = grid.dataItem(grid.select());
            if (selectedItem) {
                return selectedItem.Id;
            }
        }
    }
    function loadAllSystemUsersGrid($scope) {
        var pageVarsPri = {
            vars: {},
            urls: {},
            tags: {
                systemUsersGridId: '#systemUsersGrid',
                systemUserSystemPermissionsGridId: '#systemUserSystemPermissionsGrid',
            }
        }

        if ($(pageVarsPri.tags.systemUsersGridId).kendoGrid()) {
            $(pageVarsPri.tags.systemUsersGridId).kendoGrid('destroy').empty();
        }

        var dataSource = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    url: "/Admin" + "/" + 'ListSystemUsers',
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: "/Admin" + "/" + 'UpdateSystemUser',
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: "/Admin" + "/" + 'DestroySystemUser',
                    dataType: "json"
                },
                create: {
                    url: "/Admin" + "/" + 'CreateSystemUser',
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
                        IsActive: { type: "boolean" },//, validation: { min: 0, required: true } },
                    }
                },
                errors: function (response) {
                    if (response.MessageType && response.MessageType !== "1") {
                        alert(response.MessageTypeString + ': ' + response.Message);
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
                { field: "IsActive", title: "Is Active", width: "80px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }
            ],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
                if (selectedRows.length > 0) {
                    var userId = this.dataItem(selectedRows[0]).Id
                    $scope.ShowSystemUserPerms = true;
                    loadUsersProjectMemberships(userId);
                }   
                
            }
        });
    }
    function loadUsersProjectMemberships(userId) {
        
        if ($("#systemUserSystemPermissionsGrid").kendoGrid()) {
            $("#systemUserSystemPermissionsGrid").kendoGrid('destroy').empty();
        }
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/Admin" + "/ListUserProjectMemberships?userId=" + userId,
                    dataType: "json",
                    cache: false
                },
                update: {
                    url: "/Admin" + "/UpdateProjectUserMembership",
                    dataType: "json",
                    cache: false
                },
                destroy: {
                    url: "/Admin" + "/DestroyProjectUserMembership",
                    dataType: "json"
                },
                create: {
                    url: "/Admin" + "/CreateProjectUserMembership?userId=" + userId,
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
                        ProjectId: { validation: { required: true } },
                        ProjectName: { editable: false, validation: { required: false } },
                        isProjectAdmin: { type: "boolean" },
                    }
                }
            }
        });

        $("#systemUserSystemPermissionsGrid").kendoGrid({
            dataSource: dataSource,
            selectable: true,
            filterable: true,
            pageable: true,
            height: 400,
            groupable: true,
            sortable: true,
            toolbar: ["create"],
            columns: [
                { field: "ProjectId", title: "Project Id", width: "120px", editor: ProjectDropDownEditorSystemUserProjectMembership, template: "#=ProjectName#" },
                { field: "isProjectAdmin", title: "is Project Admin", width: "120px" },
                { command: ["edit", "destroy"], title: "&nbsp;", width: "170px" }],
            editable: "inline",
            change: function (e) {
                var selectedRows = this.select();
                var selectedDataItems = [];
            }
        });
    }
});