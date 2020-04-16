angApp.controller("SystemLogsController", function ($scope, $location, $http) {    
    LoadAllSystemLogsGrid();
});

var pageVarsPri = {
    vars: {},
    urls: {
    },
    tags: {
        systemLogsGridId: '#systemLogsGrid',
        systemLogErrorMessageId: '#systemLogErrorMessage',
        systemLogStackTraceId: '#systemLogStackTrace'
    }
}

function LoadAllSystemLogsGrid() {
    if ($(pageVarsPri.tags.systemLogsGridId).kendoGrid()) {
        $(pageVarsPri.tags.systemLogsGridId).kendoGrid('destroy').empty();
    }

    $(pageVarsPri.tags.systemLogErrorMessageId).html('');
    $(pageVarsPri.tags.systemLogStackTraceId).html('');

    var dataSource = new kendo.data.DataSource({
        type: "aspnetmvc-ajax",
        transport: {
            read: {
                url: "/Admin/" + 'ListSystemLogs',
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
                    TimeStamp: { type: "date", editable: false, validation: { required: false } },
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
            { field: "MachineName", title: "Machine Name", width: "80px" },
            { field: "UserName", title: "User Name", width: "80px" },
            { field: "TimeStamp", title: "Time Stamp", width: "60px", format: "{0:dd-MMM-yyyy hh:mm}" },
            {
                field: "Level",
                width: "40px",
                headerTemplate: '<span style="" title="0 = Information, 1 = Error">Level (i)</span>'
            },
            { field: "ProjectName", title: "Project Name", width: "120px" },
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
                    url: "/Admin" + "/GetSysemLogStackTrace?logId=" + itemId,
                    success: function (result) {
                        $(pageVarsPri.tags.systemLogStackTraceId).html(result);
                    }
                });
            }
        }
    });
}