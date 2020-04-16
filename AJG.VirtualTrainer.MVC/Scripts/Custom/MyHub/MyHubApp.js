
var MyHubApp = angular.module("MyHubApp", ["ngRoute", "chart.js", "kendo.directives", "rzSlider"]);

MyHubApp.filter('SelectFirst', function () {
    return function (input, scope) {        
        if (!scope.selectedReportName && input && input.length > 0) {
            scope.selectedReportName = input[0];
        }
        return input
    }
});

MyHubApp.controller("MyHubController", function ($scope, $location, $http) {
    $scope.userdetailsloaded = false;
    $scope.ChartOverridden = false;
    $scope.monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    $scope.graphTypes = ["line", "bar"];   

    $scope.HandleResponseMessage = function (isError, message, containerId, singleton) {
        containerId = containerId ? containerId : "";
        if (isError && isError === true) {
            $scope.ShowMessage("<p>Something went wrong: </P>" + message, '', 8000, false, 'danger')
        }        
        else {
            if (message) {
                $scope.ShowMessage(message, containerId, 8000, true, 'success', singleton)
            }
        }
    }
    $scope.RemoveMessages = function (containerId)
    {
        if (containerId) {
            $(containerId + " .alert").remove();
        }
    }
    $scope.ShowMessage = function (message, containerId, delay, autoFadeOut, alertContext, singleton) {
        if (!containerId) { containerId = "#messages"; }
        if (!delay) { delay = 5000; }
        if (autoFadeOut === undefined) { autoFadeOut = true; }
        if (!alertContext) { alertContext = 'info'; }
        // info, warning, danger, success
        if (singleton === true)
        {
            $scope.RemoveMessages(singleton);
        }

        $(containerId).append("<div class='alert alert-" + alertContext + " alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>" + message + "</div>");
        if (autoFadeOut === true) {
            $(containerId + " .alert-" + alertContext).delay(delay).fadeOut("slow", function () {
                $(containerId + " .alert-" + alertContext).remove()
            });
        }
    }
    $scope.LoadSpecificTreeUser = function (UserToFind) {
        $scope.formDisabled = true;        
        $scope.RemoveMessages('#treeMultiUsersFoundDiv');
        $http.get('/MyHub/GetUserUpperHierachy?currentTopOfTreeUser=' + $scope.fullADUserTree.UserDetails.SamAccountName + '&UserToFind=' + UserToFind)
            .then(function (response) {
                $scope.formDisabled = true;
                if (response.data.matchedCount === 0) {
                    $scope.formDisabled = false;
                } else {
                    $scope.LoadSpecificTreeUserData = response.data;
                    if ($scope.LoadSpecificTreeUserData.matchedCount === 1) {
                        $scope.treeItemTotalOffset = 0;
                        $scope.loadTreeManageesRecursive();
                    } else {
                        $scope.formDisabled = false;
                    }
                }
                $scope.HandleResponseMessage(response.data.error, response.data.message, '#treeMultiUsersFoundDiv', true);
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message, '#treeMultiUsersFoundDiv', true);
                $scope.formDisabled = false;
            });
    }
    $scope.loadTreeManageesRecursive = function () {
        var treeview = $("#tree").data("kendoTreeView");
        var selectedTreeItem = treeview.dataSource.get($scope.LoadSpecificTreeUserData.hierachyOfUsers[0]);
        var selectitem = treeview.findByUid(selectedTreeItem.uid);
        var data = treeview.dataItem(selectitem);
        // Managees not yet loaded, load them.
        if (data.HasManagess && data.Managees.length === 0) {
            $http.get('/MyHub/GetADDetails?UserName=' + $scope.LoadSpecificTreeUserData.hierachyOfUsers[0] + '&GetUserManageesOnly=true&uid=' + selectedTreeItem.uid)
            .then(function (response) {
                $scope.formDisabled = true;
                var treeview = $("#tree").data("kendoTreeView");
                var item = treeview.findByUid(response.data.uid);
                var itemData = treeview.dataItem(item);
                if (itemData.Managees.length === 0) {
                    treeview.append(response.data.data, item);
                }
                $scope.SkipLoadingChartData = true;
                $scope.LoadTreeManageeDecision(treeview, item);
            }, function (response) {
                alert("Something went wrong: " + response.data);
                $scope.SkipLoadingChartData = false;
                $scope.formDisabled = false;
            });
        } else {
            $scope.SkipLoadingChartData = true;
            $scope.LoadTreeManageeDecision(treeview, selectitem);
        }
    }
    $scope.LoadTreeManageeDecision = function (treeview, treeItem) {
        if ($scope.LoadSpecificTreeUserData.hierachyOfUsers.length > 1) {
            treeview.select(treeItem);
            // add offset for item to total.
            $scope.treeItemTotalOffset = $scope.treeItemTotalOffset + treeview.select()[0].offsetTop;
            $scope.LoadSpecificTreeUserData.hierachyOfUsers.shift();
            $scope.loadTreeManageesRecursive();
        }
        else {
            treeview.expandTo($scope.LoadSpecificTreeUserData.hierachyOfUsers[0])
            $scope.SkipLoadingChartData = false;
            treeview.select(treeItem);
            // Scroll to the selected item.
            $("#tree").animate({ scrollTop: $scope.treeItemTotalOffset });
            $scope.LoadSpecificTreeUserData.hierachyOfUsers.shift();
            $scope.formDisabled = false;
        }
    }

    $scope.sortChartDatasets = function (caller, order) {   
        if (order === 'descending') {            
            $scope.ChartData.data.datasets = $scope.ChartData.data.datasets.sort(function (a, b) { return (a.data[caller.$index] < b.data[caller.$index]) ? 1 : -1 });
        }
        if (order === 'ascending') {
            $scope.ChartData.data.datasets = $scope.ChartData.data.datasets.sort(function (a, b) { return (a.data[caller.$index] > b.data[caller.$index]) ? 1 : -1 });
        }
    }

    $scope.filterExpression = function (reportName) {
        return ((reportName.System === $scope.selectedSystem.Id) && (reportName.Scope === $scope.selectedReportScope.Id));
    };    
    $scope.reportDDChanged = function (targetChildDDList) {
        if (targetChildDDList === 'SetReportScope') {
            $scope.reportScopes = $scope.selectedSystem.CascadeItems;
            $scope.selectedReportScope = $scope.selectedSystem.CascadeItems[0];
        }
        $scope.reportNames = $scope.selectedReportScope.CascadeItems;
        $scope.selectedReportName = $scope.selectedReportScope.CascadeItems[0];        
    }
    $scope.GetRowTotal = function (rowdata) {
        var total = 0;
        for (var i = 0; i < rowdata.length; i++) {
            var value = rowdata[i];
            total += value;
        }
        return total;
    }
    $scope.LoadValuesForReportDDs = function()
    {
        $http.get('/MyHub/GetValuesForReportDDs')
        .then(function (response) {
            $scope.systems = response.data;
            $scope.selectedSystem = $scope.systems[0];
            $scope.reportScopes = $scope.selectedSystem.CascadeItems;
            $scope.selectedReportScope = $scope.selectedSystem.CascadeItems[0];
            $scope.reportNames = $scope.selectedReportScope.CascadeItems;
            $scope.selectedReportName = $scope.selectedReportScope.CascadeItems[0];
        }, function (response) {
            $scope.content = "Something went wrong";
        });
    }

    $scope.LoadUserForOverrideDD = function()
    {
        $http.get('/MyHub/GetUsersForUserOVerrideDD')
        .then(function (response) {
            $scope.devOverrideUsers = response.data;
            $scope.devSelectedOverrideUser = $scope.devOverrideUsers[0];
            $scope.loadUserDetails();
        }, function (response) {
            $scope.content = "Something went wrong";
        });
    }

    $scope.PageLoad = function()
    {
        $scope.LoadUserForOverrideDD();
        $scope.LoadValuesForReportDDs();
    }

    $scope.PageLoad();
    $scope.GetDateRangeSliderDateStringFromIndex = function (index)
    {
        var now = new Date();
        var date = new Date(now.getFullYear(), now.getMonth(), 1);
        date.setMonth(date.getMonth() - (index));
        var returnString = $scope.monthNames[date.getMonth()] + ' ' + date.getFullYear();
        return $scope.monthNames[date.getMonth()] + ' ' + date.getFullYear();
    }

    $scope.DateRangeSlider = {
        minValue: 1,
        maxValue: 1,
        options: {
            floor: 0,
            ceil: 12,
            step: 1,
            noSwitching: true,
            rightToLeft: true,
            showTicks: true,
            showTicksValues: true,
            translate: function (value) {
                return $scope.GetDateRangeSliderDateStringFromIndex(value);
            },
        }
    };

    GetdDateSelections = function()
    {
        var monthFrom = $scope.GetDateRangeSliderDateStringFromIndex($scope.DateRangeSlider.maxValue).split(" ")[0];
        var yearFrom = $scope.GetDateRangeSliderDateStringFromIndex($scope.DateRangeSlider.maxValue).split(" ")[1];
        var monthTo = $scope.GetDateRangeSliderDateStringFromIndex($scope.DateRangeSlider.minValue).split(" ")[0];
        var yearTo = $scope.GetDateRangeSliderDateStringFromIndex($scope.DateRangeSlider.minValue).split(" ")[1];
        return { monthFrom: monthFrom, yearFrom: yearFrom, monthTo: monthTo, yearTo: yearTo };
    }

    $scope.UpdateReport = function()
    {
        if ($scope.SkipLoadingChartData !== true) {
            $scope.formDisabled = true;
            $scope.loadingReportData = true;
            var dateSelections = GetdDateSelections();
            var user = $scope.treeUserSelection;
            var reportInfo = { System: $scope.selectedSystem.ItemId, Scope: $scope.selectedReportScope.ItemId, Report: $scope.selectedReportName.ItemId, ReportName: $scope.selectedReportName.ItemText };
            var RequestedReportInfo = JSON.stringify(reportInfo);
            var JsonUser = JSON.stringify($scope.fullADUserTree);
        
            $http.post('/MyHub/GetChartData', {
                'RequestedReportInfo': RequestedReportInfo, 'userName': user.UserDetails.SamAccountName, 'monthFrom': dateSelections.monthFrom,
                'yearFrom': dateSelections.yearFrom, 'monthTo': dateSelections.monthTo, 'yearTo': dateSelections.yearTo
            })
            .then(function (response) {            
                $scope.ChartData = response.data.data            
                $scope.ChartType = $scope.ChartOverridden === true ? $scope.ChartType : $scope.ChartData.type;            
                $scope.DisplayReportData();
                $scope.formDisabled = false;
                $scope.HandleResponseMessage(response.data.error, response.data.message)
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            });
        }
    }
    $scope.ChartDataOverridden = function ()
    {
        $scope.ChartOverridden = true;
        $scope.DisplayReportData();
    }
    $scope.DisplayReportData = function()
    {
        if ($scope.ChartData) {           
            $('#masterChart').remove();
            $('#masterChartContainer').append('<canvas id="masterChart" height="120"></canvas>');
            var ctx = document.getElementById('masterChart').getContext('2d');
            // Set the new Config
            $scope.chartConfig = {
                type: $scope.ChartType,
                data:
                    {
                        labels: $scope.ChartData.data.labels,
                        datasets: $scope.ChartData.data.datasets
                    },
                options: {
                    title: $scope.ChartData.options.title,
                    legend: $scope.ChartData.options.legend,
                    scales: {
                        yAxes: $scope.ChartData.options.scales.yAxes,
                        xAxes: $scope.ChartData.options.scales.xAxes
                    }
                }
            }
            var chart = new Chart(ctx, $scope.chartConfig);
        }
        $scope.loadingReportData = false;
    }

    $scope.loadUserDetails = function () {
        $scope.formDisabled = true;
        var user = $scope.devSelectedOverrideUser;
        $http.get('/MyHub/GetADDetails?UserName=' + $scope.devSelectedOverrideUser.UserDetails.SamAccountName + '&GetUserManageesOnly=false')
        .then(function (response) {
            if (!$scope.treeData)
            {
                var treeview = $("#tree").data("kendoTreeView");
                treeview.bind("expand", $scope.treeExpand);
            }

            // If data is null then the user is not in the business hierachy tree.            
            $scope.treeUserSelection = response.data.data;
            $scope.fullADUserTree = response.data.data;
            $scope.treeData = new kendo.data.HierarchicalDataSource({
                data: [response.data.data],
                schema: {
                    model: {
                        id: "id",
                        children: "Managees",
                        hasChildren: "HasManagess"
                    }
                }
            });

            $scope.UpdateReport();
            $scope.userdetailsloaded = true;
            
            //$scope.formDisabled = false;
        }, function (response) {
            $scope.content = "Something went wrong";
            alert("Something went wrong: " + response.data);
            $scope.formDisabled = false;
        });
    }
    $scope.downloadReport = function()
    {
        $scope.formDisabled = true;
        var user = $scope.treeUserSelection;
        var datasets = []
        for (i = 0; i < $scope.ChartData.data.datasets.length; i++) {
            datasets.push({ label: $scope.ChartData.data.datasets[i].label, data: $scope.ChartData.data.datasets[i].data });
        }
        var labels = $scope.ChartData.data.labels;
        var chartTitle = $scope.ChartData.options.title.text;
        var ChartDataJson = JSON.stringify({ datasets: datasets, labels: labels, chartTitle: chartTitle });

        $http.post('/MyHub/CreateChartDownloadFile', { ChartDataJson: ChartDataJson, userName: user.UserDetails.SamAccountName, addRowTotalColumn: $scope.ChartData.calculateRowTotals })
        .then(function (response) {
            var url = JSON.stringify(response.data.filePath);
            if (response.data.error !== true) {
                window.open('/MyHub/DownloadChartFile?docName=' + response.data.docName, '_parent', '');
            }
            $scope.formDisabled = false;
            $scope.HandleResponseMessage(response.data.error, response.data.message);
        }, function (response) {
            var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
            $scope.HandleResponseMessage(true, message);
            $scope.formDisabled = false;
        });
    }
    $scope.treeExpand = function (e)
    {        
        var uid = e.node.dataset.uid;
        var treeview = $("#tree").data("kendoTreeView");
        var selectitem = treeview.findByUid(uid);
        var data = treeview.dataItem(selectitem);
          
        if (data.HasManagess && data.Managees.length === 0) {
            $scope.formDisabled = true;
            $http.get('/MyHub/GetADDetails?UserName=' + data.UserDetails.SamAccountName + '&GetUserManageesOnly=true&uid=' + uid)
            .then(function (response) {
                $scope.formDisabled = true;
                var treeview = $("#tree").data("kendoTreeView");
                var item = treeview.findByUid(response.data.uid);
                var itemData = treeview.dataItem(item);
                if (itemData.Managees.length === 0) {
                    treeview.append(response.data.data, item);
                }
                $scope.formDisabled = false;
            }, function (response) {                
                alert("Something went wrong: " + response.data);
                $scope.formDisabled = false;
            });
        }
    }
});

//angApp.config(function ($routeProvider) {
//    $routeProvider
//        .when("/", {
//            templateUrl: "Projects",
//            controller: "ProjectsController"
//        })
//        .when("/Projects", {
//            templateUrl: "Projects",
//            controller: "ProjectsController"
//        })
//        .when("/SystemUsers", {
//            templateUrl: "SystemUsers",
//            controller: "SystemUsersController"
//        })
//        .when("/SystemLogs", {
//            templateUrl: "SystemLogs",
//            controller: "SystemLogsController"
//        });
//});
