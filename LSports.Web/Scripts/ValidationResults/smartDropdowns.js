(function () {
    angular.module('smart-dropdowns', []).service('smartDropdowns', function (crudService) {
        var vm = this;


        vm.scope;
        vm.productId;

        var defaultFilter = {
            Id: null,
            Name: null
        }

        vm.loadSportsDropdowns = function (scope, productId) {

            vm.scope = scope || vm.scope;
            vm.productId = productId || vm.productId;

            return crudService.post("ValidationResults", "GetDropdownValues", { productId: vm.productId, filterType: 1, selectedFilterValue: { Id: null, Name: null } }).then(function (data) {

                vm.scope.sports = data.Sports;
                vm.scope.filterObject.Sport = defaultFilter;
            });
        }

        vm.loadCountriesDropdowns = function (scope, productId) {

            vm.scope = scope || vm.scope;
            vm.productId = productId || vm.productId;

            var filter = vm.scope.filterObject.Sport || defaultFilter;

            return crudService.post("ValidationResults", "GetDropdownValues", { productId: vm.productId, filterType: 2, selectedFilterValue:  filter }).then(function (data) {

                vm.scope.countries = data.Countries;
                vm.scope.filterObject.Country = defaultFilter;
            });
        }

        vm.loadLeaguesDropdowns = function (scope, productId) {

            vm.scope = scope || vm.scope;
            vm.productId = productId || vm.productId;

            var filter = vm.scope.filterObject.Country || defaultFilter;
            return crudService.post("ValidationResults", "GetDropdownValues", { productId: vm.productId, filterType: 3, selectedFilterValue: filter }).then(function (data) {

                vm.scope.leagues = data.Leagues;
                vm.scope.filterObject.League = defaultFilter;
            });
        }

        vm.loadEventsDropdowns = function (scope, productId) {

            vm.scope = scope || vm.scope;
            vm.productId = productId || vm.productId;

            var filter = vm.scope.filterObject.League || defaultFilter;
            return crudService.post("ValidationResults", "GetDropdownValues", { productId: vm.productId, filterType: 4, selectedFilterValue: filter }).then(function (data) {

                vm.scope.events = data.Events;
                vm.scope.filterObject.Event = defaultFilter;
            });
        }

        vm.loadMarketsDropdowns = function (scope, productId) {

            vm.scope = scope || vm.scope;
            vm.productId = productId || vm.productId;

            var filter = vm.scope.filterObject.Event || defaultFilter;
            return crudService.post("ValidationResults", "GetDropdownValues", { productId: vm.productId, filterType: 5, selectedFilterValue: filter}).then(function (data) {

                vm.scope.markets = data.Markets;
              //  vm.scope.filterObject.Country = defaultFilter;
            });
        }

        vm.loadProvidersDropdowns = function (scope, productId) {

            vm.scope = scope || vm.scope;
            vm.productId = productId || vm.productId;

            var filter = vm.scope.filterObject.Event || defaultFilter;
            return crudService.post("ValidationResults", "GetDropdownValues", { productId: vm.productId, filterType: 6, selectedFilterValue: filter }).then(function (data) {

                vm.scope.providers = data.Providers;
               // vm.scope.filterObject.Country = defaultFilter;
            });
        }

        vm.loadStatusesDropdowns = function (scope, productId) {

            vm.scope = scope || vm.scope;
            vm.productId = productId || vm.productId;

            var filter = vm.scope.filterObject.Event || defaultFilter;
            return crudService.post("ValidationResults", "GetDropdownValues", { productId: vm.productId, filterType: 7, selectedFilterValue: filter  }).then(function (data) {

                vm.scope.statuses = data.Statuses;
              //  vm.scope.filterObject.Country = defaultFilter;
            });
        }

        vm.isLoading = false;

        vm.callByType = function (scope, productId, filterType) {

            if (vm.isLoading == true) {
                return $.when(null);
            } else {
                vm.isLoading = true;
            }

            var setIsLoaded = function () {
                vm.isLoading = false;
            }

            if (filterType == 0) {

                return vm.loadSportsDropdowns(scope, productId)
                .then(vm.loadCountriesDropdowns)
                .then(vm.loadLeaguesDropdowns)
                .then(vm.loadEventsDropdowns)
                .then(vm.loadMarketsDropdowns)
                .then(vm.loadProvidersDropdowns)
                .then(vm.loadStatusesDropdowns)
                .then(setIsLoaded);
            } else if (filterType == 1) {

                return vm.loadCountriesDropdowns(scope, productId)
                .then(vm.loadLeaguesDropdowns)
                .then(vm.loadEventsDropdowns)
                .then(vm.loadMarketsDropdowns)
                .then(vm.loadProvidersDropdowns)
                .then(vm.loadStatusesDropdowns)
                .then(setIsLoaded);;
            } else if (filterType == 2) {

                return vm.loadLeaguesDropdowns(scope, productId)
                .then(vm.loadEventsDropdowns)
                .then(vm.loadMarketsDropdowns)
                .then(vm.loadProvidersDropdowns)
                .then(vm.loadStatusesDropdowns)
                .then(setIsLoaded);;
            } else if (filterType == 3) {

                return vm.loadEventsDropdowns(scope, productId)
                .then(vm.loadMarketsDropdowns)
                .then(vm.loadProvidersDropdowns)
                .then(vm.loadStatusesDropdowns)
                .then(setIsLoaded);;
            } else if (filterType == 4) {

                return vm.loadMarketsDropdowns(scope, productId)
                .then(vm.loadProvidersDropdowns)
                .then(vm.loadStatusesDropdowns)
                .then(setIsLoaded);;
            } else if (filterType == 5) {

                setIsLoaded();
//                console.log("nothing to do, it is last dropdown by type: " + filterType)
            } else if (filterType == 6) {


                setIsLoaded();
//                console.log("nothing to do, it is last dropdown by type: " + filterType)
            } else if (filterType == 7) {

                setIsLoaded();
//                console.log("nothing to do, it is last dropdown by type: " + filterType)
            } else {

                console.log("not implemented type: " + filterType);
            }
        }
        return vm;
    });
}());