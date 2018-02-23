dashboard.controller("dashboardSidebarController", function ($scope, crudService) {
    var vm = this;

    $scope.sideMenu = [];

    crudService.get("Menu", "Staff").then(function (data) {
        $scope.sideMenu = (data)
    })


    console.log("dashboardSidebarController");

    return vm;
});