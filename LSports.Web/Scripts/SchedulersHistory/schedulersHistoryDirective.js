schedulersHistoryApp.directive("schedulershistoryDirective", function ($timeout, $rootScope, crudService) {

    var createChart = function(productId, chartName, chartData) {

        if ($("#"+chartName).size() != 1) {
            $timeout(function () {

                createChart(arguments);
            }, 100);
            return;
        }

        var getData = function(data, typeId){
            
            var results = []
            for(var i=0; i<data.length; i++){
                if (typeId == data[i].type) {
                    results.push(data[i].data);
                }
            }

            return results;
        }

        var getByNameAndType = function(name, type, index)
        {
            return {
                idx: index,
                label: name,
                lines: {
                    lineWidth: 1
                },
                shadowSize: 0,
                data: getData(chartData, type)
            }
        }


        // var seriaCreator = getByNameAndType("Events creator", 2, 0);
        // var seriaSelectionTreeItemCreator = getByNameAndType("SelectionTreeItem creator", 6, 1);
        // var processingMessage = getByNameAndType("Processing message", 5, 2);
        // var commonProcessing = getByNameAndType("Common processing", 7, 3);
        var seriaDownloader = chartData["1"];
        var seriaValidation = chartData["3"];
        var seriaMerger = chartData["4"];
        var parsingXml = chartData["8"];

        console.log(seriaDownloader);

        var dataToDisplay = [];
        if (seriaDownloader) {
            dataToDisplay.push({
                data: seriaDownloader.map(function(item) {
                    return {
                        x: new Date(item.date),
                        y: item.timeframe,
                        pointData: item
                    }
                }),
                name: "Downloading"
            });
        }
        
        if (seriaValidation) {
            dataToDisplay.push({
                data: seriaValidation.map(function(item) {
                    return {
                        x: new Date(item.date),
                        y: item.timeframe,
                        pointData: item
                    }
                }),
                name: "Validation"
            });
        }
        
        if (seriaMerger) {
            dataToDisplay.push({
                data: seriaMerger.map(function(item) {
                    return {
                        x: new Date(item.date),
                        y: item.timeframe,
                        pointData: item
                    }
                }),
                name: "Merge"
            });
        }
        
        if (parsingXml) {
            dataToDisplay.push({
                data: parsingXml.map(function(item) {
                    return {
                        x: new Date(item.date),
                        y: item.timeframe,
                        pointData: item
                    }
                }),
                name: "Parse"
            });
        }

        Highcharts.chart(chartName, {
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
                type: 'datetime',
                dateTimeLabelFormats: { // don't display the dummy year
                    millisecond: '%H:%M:%S.%L',
                    second: '%H:%M:%S',
                    minute: '%H:%M',
                    hour: '%H:%M',
                    day: '%e. %b',
                    week: '%e. %b',
                    month: '%b \'%y',
                    year: '%Y'
                },
                title: {
                    text: 'Date'
                },

                labels: {
                    formatter: function () {
//                        return new Date(this.value).toLocaleDateString();
                    }
                }
            },
            yAxis: {
                title: {
                    text: 'Time'
                },
                labels: {
                    formatter: function () {
                        return this.value;
                    }
                },
                min: 0
            },
            tooltip: {
                headerFormat: '<b>{series.name}</b><br>',
                pointFormat: '{point.x:%H:%M:%S}: {point.y:.2f} seconds <br> {point.pointData.additionalInfo} <br> {point.pointData.errorMessage}'
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
            series: dataToDisplay,
        });

        return;

        togglePlot = function (productId, seriesIdx, notCheckbox) {

            console.log(arguments);
            console.log("seriesIdx: " + seriesIdx);

            if (notCheckbox == false) {

                var checkBox = $("#chartLegendBox" + seriesIdx)[0];
                checkBox.checked = !checkBox.checked;
            }

            var someData = plots[productId].getData();
            someData[seriesIdx].lines.show = !someData[seriesIdx].lines.show;
            someData[seriesIdx].points.show = !someData[seriesIdx].points.show;

            plots[productId].setData(someData);
            plots[productId].draw();
        }

        plots[productId] = $.plot($(chartName), [
            //seriaCreator,
            //seriaSelectionTreeItemCreator,
            //processingMessage,
            //commonProcessing,
            seriaDownloader,
            seriaValidation,
            seriaMerger,
            parsingXml//,
            //updatingArrivalMessages
        ], {
            legend: {
                position: "nw",
                show: true,
                labelFormatter: function (label, series) {

                    var cb = '<input class="legendCB" type="checkbox" ';

                    if (series.data.length > 0) {
                        cb += 'checked="true" ';
                    }

                    cb += 'id="chartLegendBox' + series.idx + '" onClick="togglePlot('+productId+',' + series.idx + ', true); return true;">';
                    
                    return cb += '<a href="#" onClick="togglePlot(' + productId + ',' + series.idx + ', false); return false;">' + label + '</a>';
                }
            },
            series: {
                lines: {
                    show: true,
                    lineWidth: 2,
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
                    radius: 2,
                    lineWidth: 1
                },
                shadowSize: 2
            },
            grid: {
                hoverable: true,
                clickable: true,
                tickColor: "#eee",
                borderColor: "#eee",
                borderWidth: 1,
                aboveData: true,
                markings: [{ xaxis: { from: 0, to: 10 }, yaxis: { from: 0, to: 0 }, color: "blue" },
                            { xaxis: { from: 0, to: 0 }, yaxis: { from: 0, to: 15 }, color: "blue" }]
            },
            colors: ["red", "green", "blue", "orange", "black", "purple", "#66CCFF", "#ff9999"],
            xaxis: {
                ticks: 10,
                tickDecimals: 10,
                tickColor: "red",
                tickLength: 10,

                mode: "time",
                minTickSize: [1, "hour"],
                timeformat: "%H:%M </br> %d %b",
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

        $(chartName).bind("plothover", function (event, pos, item) {
            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));

            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;

                    $("#tooltip").remove();
                    var date = item.datapoint[0].toFixed(2),
                        timeframe = parseInt(item.datapoint[1]),
                        isByError = item.datapoint[2],
                        additionalInfo = item.series.data[item.dataIndex][4];

                    
                    var time = new Date();
                    time.setTime(date);

                    var message = (moment(time).format("HH:mm")) + " for:</br> " + timeframe + " seconds"

                    if (additionalInfo && additionalInfo != "") {

                        message += "</br>" + additionalInfo;
                    }

                    if (isByError) {
                        message += "</br>[ERROR]: " + item.series.data[item.dataIndex][3];
                    }


                    showTooltip(item.pageX, item.pageY, message);
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    }

    var isLoading = {
        1: false,
        2: false,
        3: false,
        4: false,
        5: false,
        6: false,
        7: false
    };

    var plots = {
        1: {},
        2: {},
        3: {},
        4: {},
        5: {}
    }

    var load = function (productId, dateFrom, dateTo) {

        if (!dateFrom || !dateTo) {

            toastr["error"]("You are not selected all dates!");
        } else {

            isLoading[productId] = true;
            crudService
                .get("SchedulersHistory", "GetHistoricalDataForProduct", {
                    productId: productId,
                    from: dateFrom,
                    to: dateTo
                })
                .then(function (data) {

                    $rootScope.$broadcast('loadedByDates', {
                        dateFrom: dateFrom,
                        dateTo: dateTo
                    });

                    createChart(productId, "chart_scheduler_history_" + productId, data);

                    isLoading[productId] = false;
                });
        }
    }
    
    return {
        restrict: "AE",
        replace: false,
        scope: {
            product: '='
        },

        template: '<div class="col-md-12 col-sm-12"><div class="portlet" style="margin-bottom: 0px;"><div class="portlet-title"><div class="caption"><i class="icon-share font-red-sunglo hide"></i><span class="caption-subject font-red-sunglo bold uppercase">{{product.Name}}</span></div></div><div class="portlet-body"><div id="chart_scheduler_history_{{product.Id}}" sttyle="height:600px;"></div></div></div></div>',

        link: function (scope, e, a) {

            scope.$on('loadByDates', function (event, data) {

                if (isLoading[scope.product.Id] == false) {
                    load(scope.product.Id, data.dateFrom, data.dateTo);
                }
            });

        }
    };
});