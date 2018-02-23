schedulersHistoryApp.controller("schedulersHistoryController", function ($timeout,$interval, $scope, $rootScope, crudService, networkService) {
    
    $.fn.editable.defaults.mode = 'inline';
    var vm = this;
    
    $scope.products = [];
    $scope.isLoading = true;

    $scope.updatingInterval = 60000;

    $scope.$on('loadByDates', function (event, data) {
        $scope.isLoading = true;
    });

    $scope.$on('loadedByDates', function (event, data) {
        $scope.isLoading = false;
    });

    $scope.scales = [
        {
            name: "Last week",
            milliseconds: 1000 * 60 * 60 * 24 * 7
        },
        {
            name: "Last month",
            milliseconds: 1000 * 60 * 60 * 24 * 30
        }
    ]

    $scope.selectedScale = {};

    var format = 'MM-DD-YYYY'

    var setDates = function () {
        $('#dateFrom').editable({
            format: 'yyyy-mm-dd hh:ii',
            viewformat: 'dd/mm/yyyy hh:ii',
            validate: function (value) {

                var dateTo = new Date($('#dateTo').editable("getValue").dateTo);
                var dateFrom = new Date(value)

                if (dateFrom >= dateTo) {

                    return 'must be less, then date "To"';
                }
            }
        });
        
        $('#dateTo').editable({
            format: 'yyyy-mm-dd hh:ii',
            viewformat: 'dd/mm/yyyy hh:ii',
            validate: function (value) {
                var dateFrom = new Date($('#dateFrom').editable("getValue").dateFrom);
                var dateTo = new Date(value)
                if (dateTo <= dateFrom) {

                    return 'must be more, then date "From"';
                }
            }
        });


        var now = new Date();
        now.setHours(now.getHours() + 1);
        $('#dateTo').editable('setValue',  now, true);
              
        var from = new Date();
        from.setHours(from.getHours() - 12);
        $('#dateFrom').editable('setValue', from, true);
    }
    
    function formatDate(date) {

        return moment(date).format(format);
    }

    $scope.checkboxIntervalChanged = function () {

        if ($scope.updateByInterval) {
            vm.loadBySelectedDates();
        }
    }

    $scope.refresh = function () {

        setDates();

        crudService.get("Product", "GetAll")
            .then(function(data) {
                $scope.products = data;
                $timeout(function () {

                    vm.loadBySelectedDates();
                },100);
            });
    }
    

    vm.loadBySelectBox = function () {

        var last = new Date();
        last.setTime(last.getTime() - $scope.selectedScale.milliseconds)

        startLoading(last, new Date());
    }

    var intervalId;

    vm.loadBySelectedDates = function () {

        var dateFrom = $('#dateFrom').editable("getValue").dateFrom;
        var dateTo = $('#dateTo').editable("getValue").dateTo;

        startLoading(dateFrom, dateTo);

        if (intervalId) {
            $interval.cancel(intervalId);
        }

        if ($scope.updateByInterval) {

            intervalId = $interval(function () {
                vm.loadBySelectedDates(dateFrom, dateTo);
            }, $scope.updatingInterval);
        }
    }

    var startLoading = function (dateFrom, dateTo) {

        $rootScope.$broadcast('loadByDates', {
            dateFrom: dateFrom,
            dateTo: dateTo
        });
    }

    return vm;
});