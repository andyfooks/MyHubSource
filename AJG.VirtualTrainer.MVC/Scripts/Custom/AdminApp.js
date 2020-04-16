
var angApp = angular.module("AdminApp", ["ngRoute"]);

angApp.config(function ($routeProvider) {
    $routeProvider
        .when("/", {
            templateUrl: "Projects",
            controller: "ProjectsController"
        })
        .when("/Projects", {
            templateUrl: "Projects",
            controller: "ProjectsController"
        })
        .when("/SystemUsers", {
            templateUrl: "SystemUsers",
            controller: "SystemUsersController"
        })
        .when("/SystemLogs", {
            templateUrl: "SystemLogs",
            controller: "SystemLogsController"
        });
});
