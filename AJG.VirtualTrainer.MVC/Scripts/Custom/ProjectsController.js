angApp.controller("ProjectsController", function ($scope, $location, $http) {
    $scope.maxsize = 5;
    $scope.totalcount = 0;
    $scope.pageIndex = 1;
    $scope.pageSize = 5;

    $http.get('/Admin/ListProjects?pageSize=' + $scope.pageSize + '&pageIndex=' + $scope.pageIndex )
        .then(function (response) {
            $scope.projects = response.data.data;
            $scope.totalcount = response.data.count;
        }, function (response) {
            $scope.content = "Something went wrong";
        });
});