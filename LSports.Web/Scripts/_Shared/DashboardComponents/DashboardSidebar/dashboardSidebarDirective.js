dashboard.directive("dash", function () {

    console.log("inside dashboard directive 1")

    return {
        restrict: "AE",
        controller: "dashboardSidebarController",
        constrollerAs: "ctrl",
        link: function (scope, elem, attrs) {
            console.log("inside dashboard directive 2")
        }
    };
});