arrivalMessagesApp.controller("arrivalMessagesController", function ($timeout, $scope, crudService, $rootScope, dropDownDataSource, smartDropdowns) {

    Array.prototype.unique = function () {
        var o = {}, i, l = this.length, r = [];
        for (i = 0; i < l; i += 1) o[this[i]] = this[i];
        for (i in o) r.push(o[i]);
        return r;
    };

    $.fn.editable.defaults.mode = 'inline';

    var vm = this;

    var arrivalMessagesViewModels = [];
    var productId = $("#productId").val();

    $scope.isLoading = true;
    $scope.isShowingChart = false;

    $scope.offset = 0;
    $scope.counter = 500;
    $scope.multips = [];

    $scope.product = {}

    $scope.drowDownsFilterObject = {}

    $scope.filterObject = {
        Sport: { Id: null, name: null },
        Country: { Id: null, name: null },
        League: { Id: null, name: null },
        Event: { Id: null, name: null },
        Market: { Id: null, name: null },
        Provider: { Id: null, name: null },
        EventStatus: { Id: null, name: null },
    }

    $scope.$on('loading', function (event, data) {

        $scope.isLoading = true;
    });
    $scope.$on('loaded', function (event, data) {

        $scope.isLoading = false;
    });


    $scope.$on('isShowingChart', function (event, data) {
        
        $scope.isShowingChart = true;

        redraw();
    });

    $scope.$on('isShowingGrid', function (event, data) {
        $scope.isShowingChart = false;

        redraw();
    });

    var redraw = function() {
        // console.log("redraw")

        $scope.loadNextDropdownsByType(0).then(function () {

            if ($scope.isShowingChart) {

                loadArrivalMessagesViewModels();
            } else {
                initToolbar();

                initArrivalMessagesDataTable();

                loadArrivalMessagesViewModels();
            }
        });
    }

    $scope.sports = [];
    $scope.countries = [];
    $scope.leaques = [];
    $scope.events = [];
    $scope.markets = [];
    $scope.providers = [];
    $scope.statuses = [];

    $scope.loadNextDropdownsByType = function (filterType) {

        return smartDropdowns.callByType($scope, productId, filterType)
    }


    $scope.refresh = function () {
        setDefaultDates();
        
        crudService.get("Product", "GetProduct", { id: productId }).then(function (data) {

            $scope.product = data;
        });

        redraw();
    }

    
    $scope.showChart = function () {

        $rootScope.$broadcast("isShowingChart");
    }

    $scope.showGrid = function () {
        $rootScope.$broadcast("isShowingGrid");
    }

    var dateFrom = $('#dateFrom');
    var dateTo = $('#dateTo');

    var setDefaultDates = function () {

        dateFrom.editable({
            format: 'yyyy-mm-dd hh:ii',
            viewformat: 'dd/mm/yyyy hh:ii',
            validate: function (value) {
                var dateTo = new Date($('#dateTo').editable("getValue").dateTo);
                var dateFrom = new Date(value);

                if (dateFrom >= dateTo) {

                    return 'must be less, then date "To"';
                } else {

                    $scope.filterObject.StartDate = dateFrom;
                    $scope.filter();
                }
            }
        });

        dateTo.editable({
            format: 'yyyy-mm-dd hh:ii',
            viewformat: 'dd/mm/yyyy hh:ii',
            validate: function (value) {
                var dateFrom = new Date($('#dateFrom').editable("getValue").dateFrom);
                var dateTo = new Date(value);

                if (dateTo <= dateFrom) {

                    return 'must be more, then date "From"';
                } else {

                    $scope.filterObject.EndDate = dateTo;
                }
            }
        });

        var now = new Date();
        dateTo.editable("setValue", now);
        
        var from = new Date();
        from.setHours(0);
        from.setMinutes(0);
        from.setSeconds(0);
        dateFrom.editable("setValue", from);

        $scope.filterObject.StartDate = from;
        $scope.filterObject.EndDate = now;
    }

    $scope.loadByMultips = function ($event) {
        // console.log($event)
        // console.log($scope.offset)
        loadArrivalMessagesViewModels();
    }

    $scope.filter = function () {

        $rootScope.$broadcast('loading');

        crudService.post("ArrivalMessages", "GetCountOfArrivalMessages", { productId: productId, filter: $scope.filterObject }).then(function (count) {

            $scope.multips = [];

            if (count > 0) {
                for (var i = 0; i <= count / $scope.counter; i++) {
                    $scope.multips.push(i);
                }
            }

            loadArrivalMessagesViewModels();
        }).catch(function () {

            toastr["error"]("Can't load count of arrival messages.</br> Too many of them for a certain period of time");
        });
    }

    var loadArrivalMessagesViewModels = function () {
        $rootScope.$broadcast('loading');

        // console.log($scope.isShowingChart);

        if (!$scope.isShowingChart) {

            return crudService.post("ArrivalMessages",
                    "GetArrivalMessages",
                    {
                        productId: productId,
                        filter: $scope.filterObject,
                        offset: $scope.offset * $scope.counter,
                        count: $scope.counter
                    })
                .then(function(data) {

                    $rootScope.$broadcast('loaded');

                    arrivalMessagesViewModels = data;
                    createMessagesTable();
                })
                .catch(function() {

                    toastr["error"]("Can't load arrival messages.</br> Too many of them for a certain period of time");
                });
        } else {

            return crudService.post("ArrivalMessages", "GetDataForChart",
            {
                productId: productId,
                filter: $scope.filterObject
            }).then(function (data) {

                $rootScope.$broadcast('loaded');

                arrivalMessagesViewModels = data;

                setTimeout(function () {
                    createChart();
                }, 0);
            }).catch(function () {

                toastr["error"]("Can't load arrival messages.</br> Too many of them for a certain period of time");
            });
        }
    }


    var createMessagesTable = function () {

        if (!$("#arrival_messages_table").length || !isInited) {

            $timeout(createMessagesTable, 100);
            return;
        }
        
        $("#arrival_messages_table").DataTable().clear().draw();

        var _dataProvider = arrivalMessagesViewModels.map(function (item) {

            return {
                Id: item.ArrivalMessage.Id,
                EventsCount: item.EventsCount,
                LeaguesCount: item.LeaguesCount,
                CountriesCount: item.LocationsCount,
                MarketsCount: item.MarketsCount,

                BetsCount: item.BetsCount,
                OpenBetsCount: item.OpenBetsCount,
                ProvidersCount: item.ProvidersCount,
                SportsCount: item.SportsCount,

                RequestTime: item.ArrivalMessage.CreatedOn,

                Url: item.ArrivalMessage.Url
            }
        })

        $("#arrival_messages_table").DataTable().rows.add(_dataProvider).draw();

        $rootScope.$broadcast('loaded');
    }

    var initToolbar = function () {

        if (!$("#show_stats_chart").length) {

            $timeout(initToolbar, 100);
        } else {

            $("#show_stats_chart").sparkline([9, 10, 9, 10, 10, 11, 12, 10, 10, 11, 11, 12, 11], {
                type: 'line',
                width: '100',
                height: '55',
                lineColor: '#ffb848',//009e11
                //fillColor: "#009e11"
            });
        }
    }

    var isInited = false;
    var initArrivalMessagesDataTable = function () {

        if (!$("#arrival_messages_table").length) {

            $timeout(initArrivalMessagesDataTable, 100);
            return;
        }

        var arrivalMessagesDataTable = $("#arrival_messages_table").DataTable({
            "aaSorting": [[ 10, "desc" ]],
            columns: [
                { "data": "Id" },
                { "data": "SportsCount" },
                { "data": "CountriesCount" },
                { "data": "LeaguesCount" },
                { "data": "EventsCount" },
                { "data": "MarketsCount" },
                { "data": "ProvidersCount" },
                { "data": "BetsCount" },
                { "data": "OpenBetsCount" },
                { "data": "RequestTime" },
                { "data": "Url" },
            ],
            columnDefs: [
                {"targets": 0 },
                {"targets": 1 },
                {"targets": 2 },
                {"targets": 3 },
                {"targets": 4 },

                {"targets": 5 },
                {"targets": 6 },
                {"targets": 7 },
                {"targets": 8 },
                {
                    "targets": 9,
                    "render": function (data, type, row) {

                        var date = getDateFromNotFormat(data);
                        return moment(date).format("DD MMMM YYYY HH:MM:ss");
                    }
                },
                {
                    "targets": 10
                }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                nRow.className = "pointer";

                return;
            }
        });

        $('#arrival_messages_table tbody')
            .on('mouseenter', 'tr', function () {

                if (!arrivalMessagesDataTable.cell(this).index()) {
                    return;
                }
                var rowIdx = arrivalMessagesDataTable.cell(this).index().row;

                $(arrivalMessagesDataTable.cells().nodes()).removeClass('bordered');
                $(arrivalMessagesDataTable.column(rowIdx).nodes()).addClass('bordered');
            });

        $('#arrival_messages_table tbody').on('click', 'a.showing_xml', function () {

            var row = arrivalMessagesDataTable.row($(this).parents('tr'));

            var idMessage = arrivalMessagesDataTable.data()[row.index()].Id;

            var model = arrivalMessagesViewModels.find(function(item){
                return item.ArrivalMessage.Id == idMessage;
            })
            
            showTree(model.ArrivalMessage.XmlMessage);
        });
        
        isInited = true;
    }

    var getDateFromNotFormat = function(not_format) {

        var splicedData = not_format.slice(6, not_format.length - 2);

        var date = new Date();
        date.setTime(splicedData * 1);

        return date;
    }

    var compareDates = function (first, second) {
        return first - second > 0;
    }

    function reverseArr(input) {
        var ret = new Array;
        for (var i = input.length - 1; i >= 0; i--) {
            ret.push(input[i]);
        }
        return ret;
    }

    var createChart = function () {

        var chartId = "#arrivalMessagesChart";

        if ($(chartId).size() != 1) {
            $timeout(function () {

                createChart();
            }, 100);
            return;
        }        

        var dataAccumulator = {};

        // console.log(arrivalMessagesViewModels);
        
        for (key in arrivalMessagesViewModels) {
            dataAccumulator[new Date(key)] = {
                SportsCount: arrivalMessagesViewModels[key].SportsCount,
                CountriesCount: arrivalMessagesViewModels[key].LocationsCount,
                LeaguesCount: arrivalMessagesViewModels[key].LeaguesCount,
                EventsCount: arrivalMessagesViewModels[key].EventsCount,

                MarketsCount: arrivalMessagesViewModels[key].MarketsCount,
                ProvidersCount: arrivalMessagesViewModels[key].ProvidersCount,
                BetsCount: arrivalMessagesViewModels[key].BetsCount,
                OpenBetsCount: arrivalMessagesViewModels[key].BetsCount
            } 
        }
        // console.log(dataAccumulator);
        // arrivalMessagesViewModels = [];

        var getSeries = function (dataAccumulator, name) {
            
            var seriesData = [];
            for (key in dataAccumulator) {
                seriesData.push(dataAccumulator[key][name]);
            }

            return seriesData;
        }

        var xTitles = [];

        for (key in dataAccumulator) {
            xTitles.push((new Date(key)).toLocaleDateString());
        }

        // console.log(xTitles);

        var chartData = [
            {
                data: getSeries(dataAccumulator, "BetsCount"),
                name: "Bets"
            },
            {
                data: getSeries(dataAccumulator, "EventsCount"),
                name: "Events"
            },
            {
                data: getSeries(dataAccumulator, "LeaguesCount"),
                name: "Leagues"
            },
            {
                data: getSeries(dataAccumulator, "MarketsCount"),
                name: "Markets"
            },
            {
                data: getSeries(dataAccumulator, "CountriesCount"),
                name: "Countries"
            }, {
                data: getSeries(dataAccumulator, "ProvidersCount"),
                name: "Providers"
            },
            {
                data: getSeries(dataAccumulator, "SportsCount"),
                name: "Sports"
            }
        ];

        Highcharts.chart('arrivalMessagesChart', {
            chart: {
                type: 'area'
            },
            title: {
                text: ''
            },
            legend: {
                layout: 'horizontal',
                align: 'left',
                verticalAlign: 'top',
                x: 100,
                y: 10,
                floating: true,
                borderWidth: 1,
                backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'
            },
            plotOptions: {
                areaspline: {
                    fillOpacity: 0.5
                }
            },
            xAxis: {
                categories: xTitles,
                tickmarkPlacement: 'false',
                title: {
                    enabled: true
                },
                labels: {
                    formatter: function () {
                        return new Date(this.value).toLocaleDateString();
                    }
                }
            },
            yAxis: {
                title: {
                    text: 'Count'
                },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                }
            },
            tooltip: {
                split: true,
                valueSuffix: ''
            },
            plotOptions: {
                area: {
                    lineColor: '#666666',
                    lineWidth: 1,
                    marker: {
                        lineWidth: 1,
                        lineColor: '#666666'
                    }
                }
            },
            series: chartData
        });

        return;

        for (var index = 0; index < chartData.length; index++) {
            chartData[index].lines = {
                lineWidth: 3,
            },
            chartData[index].shadowSize = 0;
            chartData[index].idx = index;
        }

        togglePlot = function (seriesIdx, notCheckbox) {
            
            if (notCheckbox == false) {
            
                var checkBox = $("#chartLegendBox" + seriesIdx)[0];
                checkBox.checked = !checkBox.checked;
            }

            var someData = arrivalMessagesPlot.getData();
            someData[seriesIdx].lines.show = !someData[seriesIdx].lines.show;

            arrivalMessagesPlot.setData(someData);
            arrivalMessagesPlot.draw();
        }

        var arrivalMessagesPlot = $.plot($(chartId), chartData, {
            legend: {
                show: true,
                labelFormatter: function (label, series) {

                    var cb = '<input class="legendCB" type="checkbox" ';
                    
                    if (series.data.length > 0) {
                        cb += 'checked="true" ';
                    }
                    cb += 'id="chartLegendBox' + series.idx + '" onClick="togglePlot(' + series.idx + ', true); return true;">';
                    ;
                    return cb += '<a href="#" onClick="togglePlot(' + series.idx + ', false); return false;">' + label + '</a>';
                }
            },
            series: {
                lines: {
                    show: true,
                    lineWidth: 3,
                    fill: true,
                    fillColor: {
                        colors: [{
                            opacity: 0.05
                        }, {
                            opacity: 0.01
                        }]
                    }
                },
                points: {
                    show: true,
                    radius: 3,
                    lineWidth: 1
                },
                shadowSize: 2
            },
            grid: {
                hoverable: true,
                clickable: true,
                tickColor: "#eee",
                borderColor: "#eee",
                borderWidth: 0,
                aboveData: true,
                markings: [{ xaxis: { from: 0, to: 10 }, yaxis: { from: 0, to: 0 }, color: "blue" },
                            { xaxis: { from: 0, to: 0 }, yaxis: { from: 0, to: 15 }, color: "blue" }]
            },
            xaxis: {
                ticks: 10,
                tickDecimals: 10,
                tickColor: "red",
                tickLength: 10,

                mode: "time",
                minTickSize: [1, "day"],
                timeformat: "%d %b",
                timezone: "browser"
            },
            yaxis: {
                ticks: 11,
                tickDecimals: 0,
                tickColor: "#eee",
            }
        });


        function showTooltip(x, y, contents) {
            $('<div id="tooltip">' + contents + '</div>').css({
                position: 'absolute',
                display: 'none',
                top: y + 5,
                left: x + 15,
                border: '1px solid #333',
                padding: '4px',
                color: '#fff',
                'border-radius': '3px',
                'background-color': '#333',
                opacity: 0.80
            }).appendTo("body").fadeIn(200);
        }

        var previousPoint = null;

        $(chartId).bind("plothover", function (event, pos, item) {
            
            if (!pos.x || !pos.y) {
                return;
            }

            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));

            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;

                    $("#tooltip").remove();
                    var x = parseInt(item.datapoint[0]),
                        y = parseInt(item.datapoint[1]);

                    var time = new Date();
                    time.setTime(x);
                    showTooltip(item.pageX, item.pageY, item.series.label + " of " + (moment(time).format("DD MMMM YYYY")) + ": " + y);
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    }

    var showTree = function (data) {
                
        if ( !data) {

            toastr["error"]("Don't have xml to show!");
        }
        tree = $("#xmlTree");

        tree.jstree('destroy');
        tree.jstree(data);

        tree.on('loaded.jstree', function () {
            tree.jstree('open_all');
            $("#modal_showing_xml").modal("show");
        });
    }

    return vm;
});