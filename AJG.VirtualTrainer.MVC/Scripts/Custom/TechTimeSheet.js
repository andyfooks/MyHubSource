
var MyHubApp = angular.module("MyHubApp", ["ngRoute", "chart.js", "kendo.directives", "rzSlider"]);

MyHubApp.filter('SelectFirst', function () {
    return function (input, scope) {        
        if (!scope.selectedReportName && input && input.length > 0) {
            scope.selectedReportName = input[0];
        }
        return input;
    }
});
MyHubApp.directive('ngRightClick', function ($parse) {
    return function (scope, element, attrs) {
        var fn = $parse(attrs.ngRightClick);
        element.bind('contextmenu', function (event) {
            scope.$apply(function () {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
    };
});

MyHubApp.directive('numbersOnly', function () {
    return {
        require: "ngModel",
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text;
                    if (text > 100)
                    {
                        transformedInput = 100;
                    }
                    if (text < 0)
                    {
                        transformedInput = 0;
                    }
                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return 0;
            }            
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});

MyHubApp.controller("TechTimeSheet", function ($scope, $location, $http) {
    $scope.userdetailsloaded = true;
    $scope.formDisabled = false;
    $scope.monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    $scope.totals = [];
    $scope.Locations = ["","WFH","Local Office","Other Office","Off Site"];    
    $scope.showTimeSheetChecked = true;
    $scope.showActions = false;
    $scope.UserStatusDropDown = ["Team", "AllSubordinates"];
    $scope.targetUserStatuses = ["NotSaved","NotSubmitted","NotApproved","Saved","Submitted","Approved", "All"];
    $scope.SelectedAdminAction = "";
    $scope.selectedGroupUseronthlyStatus = "";
    $scope.targetUserStatus = "";
    $scope.EmailTo = "";
    $scope.EmailContent = "";
    $scope.EmailSubject = "";    
    var today = new Date;    
    $scope.bankHolidayConfigYears = [today.getFullYear(), today.getFullYear() + 1];
    $scope.displayTimeSheet = false;

    window.onscroll = function () { myFunction() };
    var header = document.getElementById("TimeSheetHeader");
    var sticky = header.offsetTop;
    function myFunction() {
        if (window.pageYOffset > sticky + 100) {
            header.classList.add("sticky");
        } else {
            header.classList.remove("sticky");
        }
    }

    $scope.bankHolidayYearChange = function ()
    {
        $scope.formDisabled = true;
        $scope.bankHolidayConfig = [];
        $http.get("/TechTimeSheet/GetBankHolidaysForYear?year=" + $scope.selectedBankHolidayYear)
        .then(function (response) {
            $scope.formDisabled = true;
            $scope.bankHolidayForm.$setPristine();
            $scope.bankHolidayConfig = [];
            for (var i = 0; i < response.data.dates.length; i++) {
                $scope.bankHolidayConfig.push(new Date(response.data.dates[i]));                
            }
            if ($scope.bankHolidayConfig && $scope.bankHolidayConfig.length > 0) {
                $scope.monthSelectorOptions = {
                    min: new Date($scope.bankHolidayConfig[0].getFullYear(), 0, 1),
                    max: new Date($scope.bankHolidayConfig[0].getFullYear(), 11, 31)
                };
            }
            $scope.HandleResponseMessage(response.data.error, response.data.message)
            $scope.formDisabled = false;
        }, function (response) {
            $scope.formDisabled = false;            
            var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
        });        
    }
    $scope.saveBankHolidayDates = function()
    {
        $scope.formDisabled = true;
        var json = JSON.stringify($scope.bankHolidayConfig);
        $http.post("/TechTimeSheet/SaveBankHolidaysForYear", { dates: json, year: $scope.selectedBankHolidayYear })
       .then(function (response) {
           $scope.formDisabled = false;
           $scope.bankHolidayForm.$setPristine();
           $scope.HandleResponseMessage(response.data.error, response.data.message)
       }, function (response) {
           $scope.formDisabled = false;
           var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
           $scope.HandleResponseMessage(true, message)
       });
    }    
    $scope.GetEmailRecipientTotal = function (targetField) {
        var count = 0;
        if ($scope.EmailRecipients && $scope.EmailRecipients.length > 0) {            
            for (var i = 0; i < $scope.EmailRecipients.length; i++) {
                if (targetField === "Submitted" && $scope.EmailRecipients[i].Submitted === true) {
                    count++;
                }
                if (targetField === "Approved" && $scope.EmailRecipients[i].Approved === true) {
                    count++;
                }
                if (targetField === "Include" && $scope.EmailRecipients[i].Include === true) {
                    count++;
                }
            }
        }
        return count;
    }
    $scope.GetDDLOptions = function () {
        $scope.formDisabled = true;
        $scope.hideMe("#DDItemEditor");
        $http.get("/TechTimeSheet/GetDDLOptions")
        .then(function (response) {
            $scope.DDLOptions = response.data;
            $scope.formDisabled = false;
            $scope.HandleResponseMessage(response.data.error, response.data.message);
        }, function (response) {
            var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
            $scope.HandleResponseMessage(true, message)
            $scope.formDisabled = false;
        });        
    }
    $scope.AddDDLItem = function (ddlItem, ddlLevel) {
        var template = angular.copy($scope.DDLOptions.template)
        template.DDLItemLevel = ddlLevel;
        ddlItem.push(template);        
    }
    $scope.ShowEmailTemplateEditor = function (action, event) {
        $scope.EmailTemplateAction = action;
        $scope.workingTemplate = angular.copy($scope.SelectedEmailTemplate);
        $scope.EMailTemplateEditWindowTitle = action === "New" ? "Create a new Template" : "Update: '" + $scope.SelectedEmailTemplate.DisplayText + "'";
        $scope.workingTemplate.Id = action === 'New' ? 0 : $scope.SelectedEmailTemplate.Id;
        $scope.EmailTemplateAction = action;
        $scope.UpdateWorkingEmailTemplate();

        var boundingClientRect = event.currentTarget.getBoundingClientRect();
        $scope.showpopup("#saveUpdateEmailTemplate", boundingClientRect);

        DragElement(document.getElementById("saveUpdateEmailTemplate"));
    }
    $scope.UpdateWorkingEmailTemplate = function () {
        if ($scope.workingTemplate) {
            $scope.workingTemplate.UncheckedUsersList = [];
            if ($scope.workingTemplate.IncludeUncheckedUsers === true) {
                if ($scope.EmailRecipients && $scope.EmailRecipients.length > 0) {
                    for (var i = 0; i < $scope.EmailRecipients.length; i++) {
                        if ($scope.EmailRecipients[i].Include !== true) {
                            $scope.workingTemplate.UncheckedUsersList.push($scope.EmailRecipients[i].SamAccountName);
                        }
                    }
                }
            }
        }
    }
    $scope.UpdateEmailTemplate = function (deleteTemplate) {              
        $scope.formDisabled = true;
        $scope.workingTemplate.Body = $scope.EmailContent;
        $scope.workingTemplate.Subject = $scope.EmailSubject;       
        $scope.UpdateWorkingEmailTemplate();

        var json = JSON.stringify($scope.workingTemplate);
        $http.post("/TechTimeSheet/SaveUpdateEmailTemplate", { EmailTemplateJson: json, Delete: deleteTemplate })
        .then(function (response) {
            $scope.hideMe("#saveUpdateEmailTemplate");
            $scope.formDisabled = false;
            $scope.LoadEmailTemplates(response.data.template.Id);
            $scope.HandleResponseMessage(response.data.error, response.data.message)
        }, function (response) {
            var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
            $scope.HandleResponseMessage(true, message)
            $scope.formDisabled = false;
        });                
    }    
    $scope.SetEmailSubAndBody = function() {
        $scope.EmailContent = $scope.SelectedEmailTemplate.Body;
        $scope.EmailSubject = $scope.SelectedEmailTemplate.Subject;
        // set all users to checked, except those without emails.
        if ($scope.EmailRecipients) {
            for (var i = 0; i < $scope.EmailRecipients.length; i++) {
                if ($scope.EmailRecipients[i].Email !== "") {
                    $scope.EmailRecipients[i].Include = true;
                }
            }
        }
        // Now uncheck any specified in list
        if ($scope.SelectedEmailTemplate.IncludeUncheckedUsers === true && $scope.SelectedEmailTemplate.UncheckedUsersList && $scope.SelectedEmailTemplate.UncheckedUsersList.length > 0)
        {
            for (var l = 0; l < $scope.SelectedEmailTemplate.UncheckedUsersList.length; l++) {                
                var filter = $scope.EmailRecipients.filter(function (word) { return word.SamAccountName === $scope.SelectedEmailTemplate.UncheckedUsersList[l] });                
                filter[0].Include = false;                
            }
        }
        $scope.UpdateEmailTo();
    }
    $scope.LoadUserForOverrideDD = function () {
        $http.get('/MyHub/GetUsersForUserOVerrideDD')
        .then(function (response) {
            $scope.devOverrideUsers = response.data;
            $scope.devSelectedOverrideUser = $scope.devOverrideUsers[0];
            $scope.loadUserDetails();
        }, function (response) {
            $scope.content = "Something went wrong" + response;
        });
    }
    $scope.AdminBulkActionChange = function(action) {
        if (action === "SendEmail") {
            $scope.UpdateEmailTo();
        }       
    }    
    $scope.UpdateEmailTo = function () {
        var emailList = "";
        for (var i = 0; i < $scope.EmailRecipients.length; i++) {
            if ($scope.EmailRecipients[i].Email !== null && $scope.EmailRecipients[i].Email !== "" && $scope.EmailRecipients[i].Include === true) {
                emailList += $scope.EmailRecipients[i].Email + '; ';
            }
        }
        emailList = emailList.trim(';');
        $scope.EmailTo = emailList;
    }
    $scope.LoadEmailTemplates = function (selectedTemplateId) {
        $scope.formDisabled = true;
        $http.get("/TechTimeSheet/GetEmailTemplates")
        .then(function (response) {
            $scope.EmailTemplates = response.data.templates;
            for (var i = 0; i < $scope.EmailTemplates.length; i++) {
                if($scope.EmailTemplates[i].Id === selectedTemplateId)
                {
                    $scope.SelectedEmailTemplate = $scope.EmailTemplates[i];
                }
            }            
            $scope.formDisabled = false;
            $scope.HandleResponseMessage(response.data.error, response.data.message)
        }, function (response) {
            var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
            $scope.HandleResponseMessage(true, message)
            $scope.formDisabled = false;
        });
    }
    $scope.saveDDL = function () {
        $scope.formDisabled = true;
        $http.post("/TechTimeSheet/SaveDDL", { ddl: JSON.stringify($scope.DDLOptions.latestCascadeDDL) })
            .then(function (response) {
                $scope.formDisabled = false;
                $scope.HandleResponseMessage(response.data.error, response.data.message)
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            });
    }
    $scope.copyDDItemJsonToClipBoard = function () {
        var json = JSON.stringify(angular.copy($scope.ddItemToEdit));
        copyStringToClipboard(json);        
    }
    $scope.CreateDDItemFromClipBoard = function (ddlList) {
        if (navigator.clipboard != undefined) {
            navigator.clipboard.readText().then(function (clipText) {
                ddlList.push(JSON.parse(clipText))
            });
        } else if (window.clipboardData) {
            var ddItem = window.clipboardData.getData("Text");
            var jsonString = JSON.parse(ddItem);
            ddlList.push(jsonString);
        }
    }
    $scope.copyDDItemCascadeItems = function () {
        $scope.clonedDDItemCascadeItems = angular.copy($scope.ddItemToEdit.CascadeItems);
    }
    $scope.pasteDDItemCascadeItems = function () {
        $scope.ddItemToEdit.CascadeItems = angular.copy($scope.clonedDDItemCascadeItems);
        $scope.clonedDDItemCascadeItems = undefined;
    }
    $scope.DeleteDDItem = function () {
        var index = $scope.ddItemToEditParentArray.indexOf($scope.ddItemToEdit);
        $scope.ddItemToEditParentArray.splice(index, 1);
    }
    $scope.MoveDDItem = function (direction) {
        var index = $scope.ddItemToEditParentArray.indexOf($scope.ddItemToEdit);
        var length = $scope.ddItemToEditParentArray.length;
        if (direction === 'Up' && index > 0) {
            $scope.ddItemToEditParentArray.splice(index, 1);
            $scope.ddItemToEditParentArray.splice(index - 1, 0, $scope.ddItemToEdit);
        }
        if (direction === 'Down' && index < length-1) {
            $scope.ddItemToEditParentArray.splice(index, 1);
            $scope.ddItemToEditParentArray.splice(index + 1, 0, $scope.ddItemToEdit);
        }
    }
    $scope.test = function ($event)
    {
        var boundingClientRect = $event.currentTarget.getBoundingClientRect();
        $scope.showpopup("#DDItemEditor", boundingClientRect);
    }
    $scope.showDDItemEditor = function (ddItem, ddItemParentArray, $event, showCopyDelete) {
        $scope.showDDListCopyPaste = showCopyDelete === undefined ? true : showCopyDelete;       
        $scope.ddItemToEdit = ddItem;
        $scope.ddItemToEditParentArray = ddItemParentArray;
    }
    $scope.showUserDetails = function (itemIndex, $event) {
        $scope.userDetailsHoverItem = $scope.EmailRecipients[itemIndex];
        var boundingClientRect = $event.currentTarget.getBoundingClientRect();
        $scope.showpopup("#showRecipientDetails", boundingClientRect);
    }
    $scope.showpopup = function (id, boundingClientRect) {
        $(id).css({
            position: "fixed",
            top: boundingClientRect.top + "px",
            left: boundingClientRect.left + boundingClientRect.width + "px"
        });
    }
    $scope.GetTimeSheetStatusForManager = function () {
        $scope.formDisabled = true;
        $http.get('/TechTimeSheet/GetTimeSheetStatusForManager?month=' + GetdDateSelections().month + '&year=' + GetdDateSelections().year
            + '&userSamAccountName=' + $scope.treeUserSelection.UserDetails.SamAccountName + '&target=' + $scope.selectedGroupUseronthlyStatus + '&status=' + $scope.targetUserStatus)
       .then(function (response) {
           $scope.ResetAdminForms();
           $scope.EmailRecipients = response.data.data;
           $scope.formDisabled = false;
           $scope.LoadEmailTemplates(0);
           $scope.UpdateEmailTo();
           $scope.HandleResponseMessage(response.data.error, response.data.message)
       }, function (response) {
           var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
           $scope.HandleResponseMessage(true, message)
           $scope.formDisabled = false;
       });
    }
    $scope.LoadLoggedOnUsersPermissions = function() {
        $http.get('/TechTimeSheet/GetCurrentUserWithPermissions')
        .then(function (response) {
            $scope.AuthenticatedUser = response.data;
        }, function (response) {
            $scope.content = "Something went wrong" + response;
        });
    }
    $scope.ddItemFreetextCheckChanged = function() {
        $scope.ddItemToEdit.DisplayFreeTextInputField = $scope.ddItemToEdit.IsFreetext;
        if ($scope.ddItemToEdit.IsFreetext === true) {
            $scope.ddItemToEdit.Name = "FreeText";
        }
    }
    $scope.PageLoad = function () {
        $scope.LoadLoggedOnUsersPermissions();
        $scope.LoadUserForOverrideDD();
    }

    $scope.PageLoad();
    $scope.DeleteArrayItem = function(list, index) {
        list.splice(index, 1);
    }
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
    $scope.RemoveMessages = function (containerId) {
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
        if (singleton === true) {
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
        $scope.formDisabled = true;
        var treeview = $("#tree").data("kendoTreeView");
        var selectedTreeItem = treeview.dataSource.get($scope.LoadSpecificTreeUserData.hierachyOfUsers[0]);
        var selectitem = treeview.findByUid(selectedTreeItem.uid);
        var data = treeview.dataItem(selectitem);
        // Managees not yet loaded, load them.
        if(data.HasManagess && data.Managees.length === 0) {
            $http.get('/MyHub/GetADDetails?UserName=' + $scope.LoadSpecificTreeUserData.hierachyOfUsers[0] + '&GetUserManageesOnly=true&uid=' + selectedTreeItem.uid)
            .then(function (response) {
                $scope.formDisabled = true;
                var treeview = $("#tree").data("kendoTreeView");
                var item = treeview.findByUid(response.data.uid);
                var itemData = treeview.dataItem(item);
                if (itemData.Managees.length === 0) {
                    treeview.append(response.data.data, item);
                }
                $scope.SkipLoadingMonthData = true;
                $scope.LoadTreeManageeDecision(treeview, item);                                               
            }, function (response) {
                alert("Something went wrong: " + response.data);
                $scope.SkipLoadingMonthData = false;
                $scope.formDisabled = false;                
            });
        } else {
            $scope.SkipLoadingMonthData = true;
            $scope.LoadTreeManageeDecision(treeview, selectitem);            
        }
    }
    $scope.LoadTreeManageeDecision = function(treeview, treeItem)
    {        
        if ($scope.LoadSpecificTreeUserData.hierachyOfUsers.length > 1) {
            treeview.select(treeItem);
            // add offset for item to total.
            $scope.treeItemTotalOffset = $scope.treeItemTotalOffset + treeview.select()[0].offsetTop;            
            $scope.LoadSpecificTreeUserData.hierachyOfUsers.shift();
            $scope.loadTreeManageesRecursive();
        }
        else {            
            treeview.expandTo($scope.LoadSpecificTreeUserData.hierachyOfUsers[0])
            $scope.SkipLoadingMonthData = false;
            treeview.select(treeItem);                        
            // Scroll to the selected item.
            $("#tree").animate({ scrollTop: $scope.treeItemTotalOffset });
            $scope.LoadSpecificTreeUserData.hierachyOfUsers.shift();
            $scope.formDisabled = false;
        }
    }
    $scope.loadUserDetails = function() {
        $scope.formDisabled = true;

        $http.get("/MyHub/GetADDetails?UserName=" +$scope.devSelectedOverrideUser.UserDetails.SamAccountName + "&GetUserManageesOnly=false")
        .then(function (response) {
            if (!$scope.treeData) {
                var treeView = $("#tree").data("kendoTreeView");
                treeView.bind("expand", $scope.treeExpand);
            }

            // If data is null then the user is not in the business hierachy tree.            
            $scope.treeUserSelection = response.data.data;
            $scope.fullADUserTree = response.data.data;
            $scope.treeData = new window.kendo.data.HierarchicalDataSource({
                    data: [response.data.data],
                    schema: {
                        model: {
                        id: "id",
                        children: "Managees",
                            hasChildren: "HasManagess"
                }
            }
        });
        $scope.LoadMonthData();
        $scope.userdetailsloaded = true;
        $scope.formDisabled = false;
        }, function (response) {
            $scope.content = "Something went wrong";
            alert("Something went wrong: " +response.data);
            $scope.formDisabled = false;
        });
    }
    $scope.treeExpand = function (e) {        
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

    GetdDateSelections = function () {
        var month = $scope.GetDateRangeSliderDateStringFromIndex($scope.DateRangeSlider.minValue).split(" ")[0];
        var year = $scope.GetDateRangeSliderDateStringFromIndex($scope.DateRangeSlider.minValue).split(" ")[1];
        return { month: month, year: year };
    }

    $scope.GetDateRangeSliderDateStringFromIndex = function (index) {
        var now = new Date();
        var date = new Date(now.getFullYear(), now.getMonth() + 1, 1);
        date.setMonth(date.getMonth() - (index));
        return $scope.monthNames[date.getMonth()] + " " + date.getFullYear();
    }

    $scope.DateRangeSlider = {
        minValue: 1,        
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
            onChange: function () {
                $scope.LoadMonthData();
            }
        }
    };
    $scope.CreateTimeSheetReport = function() {
        $scope.formDisabled = true;
        var settingList = JSON.stringify($scope.TimeSheetReportConfig);
        $http.post("/TechTimeSheet/CreateTimeSheetReport", { TimesheetReport: settingList })
            .then(function (response) {
                if (response.data.error === false) {
                    window.open('/MyHub/DownloadChartFile?docName=' + response.data.docName, '_parent', '');
                }                
                $scope.formDisabled = false;
                $scope.HandleResponseMessage(response.data.error, response.data.message)
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            });
    }
    $scope.SaveAdminSingleFieldSetting = function (value, action) {
        $scope.formDisabled = true;
        var settingList = JSON.stringify(value);
        $http.post("/TechTimeSheet/SaveAdminSingleFieldSetting", { value: settingList, actionEnum: action })
            .then(function (response) {
                $scope.formDisabled = false;
                $scope.HandleResponseMessage(response.data.error, response.data.message);
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            });
    }
    $scope.AddItemAsCloneToList = function (List, ObjectToClone)
    {
        List.push(angular.copy(ObjectToClone));
    }
    $scope.AdminTaskChanged = function(action) {
        $scope.ResetAdminForms();
        $scope.formDisabled = true;
        $http.get("/TechTimeSheet/GetAdminSingleFieldSetting?adminAction=" + action)
            .then(function (response) {                
                if (response.data.option === "TechTimeSheetAccess" || 
                response.data.option === 'LocationRequired' || 
                response.data.option === 'ApprovalRequired' ||
                response.data.option === 'SaveToDB') {
                    $scope.TimeSheetRuleList = response.data.ruleList;
                    $scope.TimeSheetRuleTemplate = response.data.timeSheetRuleTemplate;
                }
                if (response.data.option === "RunReports") {
                    $scope.TimeSheetReportConfig = response.data.TimeSheetReport;
                }
                $scope.formDisabled = false;
                $scope.HandleResponseMessage(response.data.error, response.data.message)
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            });
    }    
    $scope.ResetAdminForms = function ()
    {
        $scope.EmailRecipients = null;
        $scope.EmailTo = "";
        $scope.EmailContent = "";
        $scope.EmailSubject = "";
        $scope.hideMe("#DDItemEditor");
    }
    $scope.AddRow = function()
    {
        var clonedTemplate = JSON.parse(JSON.stringify($scope.MonthData.TimeSheetRowTemplate));
        $scope.MonthData.TimeSheetActviityLogRows.push(clonedTemplate);
    }
    $scope.DeleteRow = function(index)
    {
        if ($scope.MonthData.TimeSheetActviityLogRows.length > 1) {
            var row = $scope.MonthData.TimeSheetActviityLogRows[index];
            if (row !== -1) {
                $scope.MonthData.TimeSheetActviityLogRows.splice(index, 1);
            }
        }
    }
    $scope.getTotal = function (index) {
        var columnTotal = 0;
        for (var i = 0; i < $scope.MonthData.TimeSheetActviityLogRows.length; i++) {
            columnTotal += $scope.MonthData.TimeSheetActviityLogRows[i].dataEntryFields[index] === '' || $scope.MonthData.TimeSheetActviityLogRows[i].dataEntryFields[index] === null ? 0 : $scope.MonthData.TimeSheetActviityLogRows[i].dataEntryFields[index];
        }
        $scope.totals[index] = columnTotal;
        return columnTotal;
    }
    $scope.hideMe = function (ref) {
        $(ref).css({ 'left': -9999 });
    }
    $scope.copyToEndof = function (copyExtent) {
        var itemValue = $scope.inputRightClickData[$scope.focusedItemIndex];
        for (var i = $scope.focusedItemIndex; i < $scope.MonthData.thisMonthsDates.length; i++) {
            if ($scope.MonthData.thisMonthsDates[i].isWorkDay) {
                $scope.inputRightClickData[i] = itemValue;
            }
            else {
                if (copyExtent === "week") {
                    return;
                }
            }
        }        
    }
    $scope.percentEntryFieldIsDisabled = function($index) {
        return !$scope.MonthData.thisMonthsDates[$index].isWorkDay;
    }
    $scope.FormComplete = function ()
    {
       if($scope.timeSheetForm.$invalid === true) { return true; }
        if ($scope.MonthData) {
            for (var i = 0; i < $scope.MonthData.thisMonthsDates.length; i++) {
                if ($scope.MonthData.thisMonthsDates[i].isWorkDay) {
                    if ($scope.totals[i] !== 100) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    $scope.inputRightClick = function (itemIndex, $event, arrayRow)
    {
        $scope.inputRightClickData = arrayRow;
        $scope.focusedItemIndex = itemIndex;
        var boundingClientRect = $event.currentTarget.getBoundingClientRect();
        $scope.showpopup(".info", boundingClientRect);
    }    
    $scope.getLatestTimeSheetDDL = function () {
        $scope.formDisabled = true;
        $http.get("/TechTimeSheet/GetDDLOptions")
            .then(function (response) {
                $scope.MonthData.CascadingDropDownList = response.data.latestCascadeDDL;
                $scope.MonthData.CascadingDropDownListOutOfDate = false;
                $scope.formDisabled = false;
                $scope.HandleResponseMessage(response.data.error, response.data.message ? response.data.message : "Latest DDL options successfully applied to Time Sheet.")
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            });
    }
    $scope.LoadMonthData = function (action) {
        if ($scope.SkipLoadingMonthData !== true) {
            $scope.displayTimeSheet = false;
            $scope.formDisabled = true;
            $http.post('/TechTimeSheet/GetMonthTimeSheetData', {
                month: GetdDateSelections().month,
                year: GetdDateSelections().year,
                userSamAccountName: $scope.treeUserSelection.UserDetails.SamAccountName,
                actionEnum: action
            })
                .then(function (response) {
                    $scope.MonthData = response.data.monthData;
                    $scope.setDropDownSelection($scope.MonthData.SelectedTeamName, "", "team");
                    $scope.ResetAdminForms();
                    $scope.totals = [];
                    //$scope.formDisabled = false;
                    //$scope.displayTimeSheet = true;
                    $scope.HandleResponseMessage(response.data.error, response.data.message)
                }, function (response) {
                    var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                    $scope.HandleResponseMessage(true, message)
                    $scope.formDisabled = false;
                    $scope.displayTimeSheet = true;
                });
        }
    }
    $scope.performMonthDataAction = function(action)
    {        
        $scope.displayTimeSheet = false;
        $scope.formDisabled = true;
        // Clear the cascade lists as we don't want to send this back
        for (var i = 0; i < $scope.MonthData.TimeSheetActviityLogRows.length; i++) {
            $scope.MonthData.TimeSheetActviityLogRows[i].ItActivity = null;
            $scope.MonthData.TimeSheetActviityLogRows[i].WorkItemOrProjectTask = null;
            $scope.MonthData.TimeSheetActviityLogRows[i].BusinessUnit = null;
            $scope.MonthData.Team = null;
        }
        var monthDataJson = JSON.stringify($scope.MonthData);
        $http.post("/TechTimeSheet/MonthDataAction", { monthDataJson: monthDataJson, actionEnum: action })
            .then(function (response) {               
                $scope.MonthData = response.data.data;
                var teamDDL = $scope.MonthData.CascadingDropDownList.filter(function (word) { return word.Name === $scope.MonthData.SelectedTeamName; })[0];
                $scope.MonthData.Team = teamDDL;                       
                $scope.HandleResponseMessage(response.data.error, response.data.message)
                $scope.formDisabled = false;
                $scope.displayTimeSheet = true;
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            }
        );
    }    
    $scope.setDropDownSelection = function (selection, selectedName, target) {
        $scope.formDisabled = true;
        if (target === "team") {
            if ($scope.MonthData.CascadingDropDownList) {
                var teamDDL = $scope.MonthData.CascadingDropDownList.filter(function (word) { return word.Name === selection; })[0];
                $scope.performMonthDataAction('GetDDLForTeam');                
            }
        }
        if (target === "activity") {
            if ($scope.MonthData.Team) {
                selection.ItActivity = $scope.MonthData.Team.CascadeItems.filter(function (word) { return word.Name === selection.SelectedActivityName; })[0];
            }
        }
        if (target === "WorkItem") {
            if (selection.ItActivity) {
                selection.WorkItemOrProjectTask = selection.ItActivity.CascadeItems.filter(function (word) { return word.Name === selection.WorkItemOrProjectTaskName; })[0];
            }
        }
        if (target === "BU") {
            if (selection.WorkItemOrProjectTask) {
                selection.BusinessUnit = selection.WorkItemOrProjectTask.CascadeItems.filter(function (word) { return word.Name === selection.BusinessUnitName; })[0];
            }
        }
        $scope.formDisabled = false;
    }
    $scope.sendEmail = function (to,subject,body) {        
        var link = "mailto:" + to + "&subject=" + escape(subject) + "&body=" + escape(body);
        window.location.href = link;
    }
    $scope.AddToCurrentAdminBulkActionUsers = function () {
        for (var i = 0; i < $scope.EmailRecipients.length; i++) {
            if ($scope.EmailRecipients[i].Email !== null && $scope.EmailRecipients[i].Email !== '' && $scope.EmailRecipients[i].Include === true) {
                var existing = $scope.CurrentAdminBulkActionUsers.filter(function (word) { return word === $scope.EmailRecipients[i].SamAccountName });
                if (existing.length === 0) {
                    $scope.CurrentAdminBulkActionUsers.push($scope.EmailRecipients[i].SamAccountName);
                }
            }
        }
    }
    $scope.SubmitUserTimSheetsToDB = function (userName) {
        $scope.formDisabled = true;        
        var usernames = userName === null || userName === undefined ? [] : [userName];
        $http.post("/TechTimeSheet/SaveUserSheetsToWorkSheetMonthActivityTable", { month: GetdDateSelections().month, year: GetdDateSelections().year, userNames: JSON.stringify(usernames) })
            .then(function (response) {
                $scope.formDisabled = false;
                $scope.HandleResponseMessage(response.data.error, response.data.message)
            }, function (response) {
                var message = "<div><p>" + response.statusText + ": " + response.status + "</p></div>";
                $scope.HandleResponseMessage(true, message)
                $scope.formDisabled = false;
            });
    }
});