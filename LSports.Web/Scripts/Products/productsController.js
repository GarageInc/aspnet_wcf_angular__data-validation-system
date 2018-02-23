productsApp.controller("productsController", function ($timeout, $scope, $rootScope, crudService, networkService) {

    $.fn.editable.defaults.mode = 'inline';
    var vm = this;
    
    $scope.productsViewModels = [];
    $scope.isLoading = true;

    $scope.$on('loadByDates', function (event, data) {
        $scope.isLoading = true;
    });
    $scope.$on('loadedByDates', function (event, data) {
        $scope.isLoading = false;
    });

    var format = 'MM-DD-YYYY'

    var setDefaultDates = function () {

        var dateFormat = 'DD MMMM YYYY';
        var templateFormat = 'D / MMMM / YYYY';


        $('#dateFrom').editable({
            format: format,
            viewformat: dateFormat,
            template: templateFormat,
            combodate: {
                minYear: 2000,
                maxYear: 2020,
                minuteStep: 1
            },
            validate: function (value) {
                var dateTo = new Date(moment($('#dateTo').editable("getValue").dateTo, 'DD-MM-YYYY')._d);
                var dateFrom = new Date(value._d);
                if (dateFrom >= dateTo) {
                    return 'must be less, then date "To"';
                } else {
                    $timeout(function () {
                        vm.loadBySelectedDates();
                    }, 500);
                }
            }
        });
        
        $('#dateTo').editable({
            format: format,
            viewformat: dateFormat,
            template: templateFormat,
            combodate: {
                minYear: 2000,
                maxYear: 2020,
                minuteStep: 1
            },

            validate: function (value) {
                var dateFrom = new Date(moment($('#dateFrom').editable("getValue").dateTo, 'DD-MM-YYYY')._d);
                var dateTo = new Date(value._d);
                if (dateTo <= dateFrom) {
                    return 'must be more, then date "From"';
                } else {
                    $timeout(function () {
                        vm.loadBySelectedDates();
                    }, 500);
                }
            }
        });

        var now = new Date();
        $('#dateTo').editable('setValue',  formatDate(now), true);
              
        var from = new Date();
        from.setDate(from.getDate() - 7);
        $('#dateFrom').editable('setValue', formatDate(from), true);
    }
    
    function formatDate(date) {

        return moment(date).format(format);
    }

    $scope.refresh = function() {

        setDefaultDates();

        loadProductsData();
    };

    var loadProductsData = function () {

        var dateFrom = $('#dateFrom').editable("getValue").dateFrom;
        var dateTo = $('#dateTo').editable("getValue").dateTo;
        
        if (dateFrom && dateFrom != "" && dateTo && dateTo != "") {
            crudService.get("Product", "List", { from: dateFrom, to: dateTo })
                .then(function(data) {
                    $scope.productsViewModels = data;

                    $rootScope.$broadcast('loadedByDates');
                });
        } else {

            $rootScope.$broadcast('loadedByDates');
            toastr["error"]("Please, select dates for loading!");
        }
    };

    $scope.$on('timeframechanged', function (event, data) {
        
        if (data) {

            $('#dateFrom').editable("setValue", formatDate(data.startDate), true);
            $('#dateTo').editable("setValue", formatDate(data.endDate), true);

            vm.loadBySelectedDates();
        }
    });


    vm.loadBySelectedDates = function () {

        var dateFrom = $('#dateFrom').editable("getValue").dateFrom;
        var dateTo = $('#dateTo').editable("getValue").dateTo;

        $rootScope.$broadcast('loadByDates', {
            dateFrom: dateFrom,
            dateTo: dateTo
        });

        loadProductsData();
    }

    return vm;
});