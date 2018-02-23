productsApp.directive("productsDirective", function ($timeout, $rootScope, crudService) {

    var createChart = function (chartName, chartData) {

        if ($(chartName).size() != 1) {
            $timeout(function () {

                createChart(chartName, chartData)
            }, 100);
            return;
        }
        
        var plot = $.plot($(chartName), [{
            data: chartData,
            label: "Issues",
            lines: {
                lineWidth: 1,
            },
            shadowSize: 0

        }], {
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
                borderWidth: 1,
                aboveData: true,
                markings: [{ xaxis: { from: 0, to: 10 }, yaxis: { from: 0, to: 0 }, color: "blue" },
                            { xaxis: { from: 0, to: 0 }, yaxis: { from: 0, to: 15 }, color: "blue" }]
            },
            colors: ["#d12610", "#37b7f3", "#52e136"],
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

        $(chartName).bind("plothover", function (event, pos, item) {
            $("#x").text(pos.x.toFixed(2));
            $("#y").text(pos.y.toFixed(2));

            if (item) {
                if (previousPoint != item.dataIndex) {
                    previousPoint = item.dataIndex;

                    $("#tooltip").remove();
                    var x = item.datapoint[0].toFixed(2),
                        y = item.datapoint[1];

                    var time = new Date();
                    time.setTime(x);
                    showTooltip(item.pageX, item.pageY, (moment(time).format("DD MMMM YYYY")) + ": " + y + " " + item.series.label);
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;
            }
        });
    }

    
    return {
        restrict: "AE",
        replace: true,
        scope: {
            product: '='
        },
        link: function (scope, e, a) {
            var data = scope.product.GraphData;

            var chartData = [];

            for (var key in data) {
                var date = new Date(key);
                chartData.push([date.getTime(), data[key]]);
            }

            createChart("#chart_product_" + scope.product.Product.Id, chartData);
        }
    };
});