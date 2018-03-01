directiveModule.directive('gChart', function () {
    return {
        restrict: 'A',

        link: function ($scope, elm, attrs) {

            $scope.$watch('chart.data', function () {
                var type = $scope.chart.type;
                var data;
                if ($scope.chart.data) {
                    var chart = " ";
                    if (type == "Line") {
                        chart = new google.visualization.LineChart(elm[0]);
                    }
                    else if (type == "Bar") {
                        chart = new google.visualization.BarChart(elm[0]);
                    }
                    else if (type == "Column" || type == "StackedColumn") {
                        chart = new google.visualization.ColumnChart(elm[0]);
                    }
                    else if (type == "Pie") {
                        chart = new google.visualization.PieChart(elm[0]);
                    }
                    else if (type == "EpiCurve") {
                        chart = new google.visualization.SteppedAreaChart(elm[0]);
                    }
                    else if (type == "Pareto") {
                        chart = new google.visualization.ComboChart(elm[0]);
                    }
                    else if (type == "Scatter") {
                        chart = new google.visualization.ScatterChart(elm[0]);
                    }


                    var options = {
                        'fontName': 'Segoe UI',
                        'chartArea': {top: 5},
                        'legend': {
                            position: 'bottom',
                            textStyle: {
                                color: '#3f3f3f',
                                fontName: 'Segoe UI',
                                fontSize: 14
                            }
                        },
                        'title': chart.gadgetName,
                        'titleposition': 'out',
                        'titleTextStyle': {
                            color: '#3f3f3f',
                            fontName: 'Segoe UI',
                            fontSize: 22
                        },
                        'axisTitlesPosition': 'out',
                        'hAxis': {
                            slantedText: true,
                            slantedTextAngle: 70,
                            textStyle: {color: '#3f3f3f', fontName: 'Segoe UI'}
                        },
                        'vAxes': {
                            0: {
                                title: $scope.chart.ylbl,
                                targetAxisIndex: 0,
                                minValue: 0,
                                gridlines: {count: -1},
                                textStyle: {
                                    color: '#3f3f3f',
                                    fontName: 'Segoe UI',
                                    fontSize: 10,
                                    italic: false
                                }
                            },
                            1: {
                                targetAxisIndex: 1,
                                minValue: 0,
                                gridlines: {color: 'transparent'},
                                textStyle: {
                                    color: '#3f3f3f',
                                    fontName: 'Segoe UI',
                                    fontSize: 10,
                                    italic: false
                                },
                            }
                        },
                        'pieSliceText': 'value',
                        'allowHtml': true,
                        'showRowNumber': true
                    }
                    if (type == "EpiCurve") {
                        data = $scope.chart.data;
                        options.isStacked = 'true';
                    }
                    else if (type == "StackedColumn") {

                        if ($scope.chart.columnAggregateFunction == "Count %") {

                            var dataSet2, tempdata;
                            var view = new google.visualization.DataView($scope.chart.data);
                            options.vAxes[0].title = "Count %";
                            data = $scope.chart.data;
                            // Calculate the percentages
                            var sum = new Array();
                            dataSet2 = new Array(view.getNumberOfRows());

                            for (row = 0; row < view.getNumberOfRows(); row++) {
                                sum[row] = 0;

                                dataSet2[row] = new Array(view.getNumberOfColumns());

                                for (col = 1; col < view.getNumberOfColumns(); col++) {
                                    sum[row] += parseFloat(view.getValue(row, col));
                                }

                                for (col = 1; col < view.getNumberOfColumns(); col++) {
                                    dataSet2[row][col] = 100 * parseFloat(view.getValue(row, col) / sum[row]).toFixed(2);
                                }
                            }

                            tempdata = google.visualization.arrayToDataTable(dataSet2, true);

                            for (var i = 0; i < tempdata.getNumberOfRows(); i++) {
                                for (var j = 1; j < tempdata.getNumberOfColumns(); j++) {
                                    data.setValue(i, j, tempdata.getValue(i, j));
                                }
                            }
                        }
                        else {
                            data = $scope.chart.data;
                        }

                        options.isStacked = 'true';

                    }
                    else if (type == "Pareto") {
                        options.seriesType = "bars";
                        var paretSeries = {};
                        paretSeries[1] = {
                            type: "line",
                            targetAxisIndex: 1,
                            minValue: 0,
                            lineDashStyle: [4, 4]
                        };
                        options.series = paretSeries;
                        data = $scope.chart.data;
                    }
                    else if (type == "Bar") {
                        options.hAxis = {minValue: 0};
                        data = $scope.chart.data;
                    }
                    else if (type == "Line") {
                        options.pointSize = 5;
                        data = $scope.chart.data;
                    }
                    else if (type == "Scatter") {
                        var lineSeries = {};
                        lineSeries[0] = {lineWidth: 2};
                        options.series = lineSeries;
                        data = $scope.chart.data;
                    }
                    else {
                        data = $scope.chart.data;
                    }
                    options.hAxis.title = $scope.chart.xlbl;

                    chart.draw(data, options);
                }


            }, true);
        }
    }
});

