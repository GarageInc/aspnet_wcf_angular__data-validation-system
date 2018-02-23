(function () {
    angular.module('timeframeloader', []).directive('timeframeloaderDirective', function ($timeout, $rootScope, crudService) {

        var nowStartDayDate = new Date();
        nowStartDayDate.setHours(0);
        nowStartDayDate.setMinutes(0);
        nowStartDayDate.setSeconds(0);
        nowStartDayDate.setMilliseconds(0);

        var yesterday = new Date(nowStartDayDate);
        yesterday.setDate(yesterday.getDate() - 1);

        var last7Days = new Date();
        last7Days.setDate(last7Days.getDate() - 7);

        var last30Days = new Date();
        last30Days.setDate(last30Days.getDate() - 30);

        var date = new Date(), y = date.getFullYear(), m = date.getMonth();
        var thisMonthStartDate = new Date(y, m, 1);

        date = new Date();
        y = date.getFullYear();
        m = date.getMonth();

        var lastMonthStartDate = new Date(y, m - 1, 1);
        var lastMonthEndDate = new Date(y, m, 0);

        var scales = [
            {
                name: "Today",
                startDate: nowStartDayDate,
                endDate: new Date()
            },
            {
                name: "Yesterday",
                startDate: yesterday,
                endDate: new Date((new Date(yesterday)).getTime() + 1000 * 60 * 60 * 24 - 1000)
            },
            {
                name: "Last 7 days",
                startDate: last7Days,
                endDate: new Date()
            },
            {
                name: "Last 30 days",
                startDate: last30Days,
                endDate: new Date()
            },
            {
                name: "This Month",
                startDate: thisMonthStartDate,
                endDate: new Date()
            },
            {
                name: "Last Month",
                startDate: lastMonthStartDate,
                endDate: lastMonthEndDate
            }
        ];

        var isFirst = true;

        return {
            restrict: "AE",
            replace: true,
            template: '<div><div><select select2="" style="display:inline-block; margin-left:20px;" ng-options="scale as scale.name for scale in scales" class="form-control input-small select2me" ng-model="selectedScale" ng-change="loadBySelectBox()"><option value="" selected>Timeframe...</option></select></div></div>',
            link: function (scope, e, a) {
                scope.scales = scales;
                scope.selectedScale = {};

                scope.loadBySelectBox = function () {
                    //if (!isFirst) {
                        $rootScope.$broadcast('timeframechanged', scope.selectedScale);
                    //} else {
                    //    isFirst = false;
                    //}
                }
            }
        };
    });
}());

