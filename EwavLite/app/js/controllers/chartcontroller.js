'use strict';

controllerModule.controller('ChartCtrl', function ($scope, $location, $rootScope, $window,
                                                   canvasService,
                                                   chartService,
                                                   epicurveService,
                                                   scatterService) {
    var parm = $location.$$absUrl.substring(0).split('&')[0];
    var pos = parm.indexOf('=');
    var canvasid = parm.substring(pos + 1);
    $scope.ShowChart = 1; // Variable used to show/hide loading... cursor.


    var w = angular.element($window);

    // $scope.$watch(
    //     function () {
    //         return $window.innerWidth;
    //     },
    //     function (value) {
    //         $scope.windowWidth = value;
    //
    //         console.log($scope)        ;
    //         if ($scope.chart.data) {
    //             console.log('chat data change'  )        ;
    //             var tempChartData = $scope.chart.data;
    //             $scope.chart.data = tempChartData;
    //         }
    //     },
    //     true
    // );

    w.bind('resize', function () {

        if ($scope.chart && $scope.chart.data) {
            console.log('chat data change');
            var tempChartData = $scope.chart.data;
            var newVal = tempChartData.getColumnLabel(0);
            newVal += ' ';
            tempChartData.setColumnLabel(0, newVal);
            $scope.chart.data = tempChartData;
        }

        $scope.$apply();
    });


    $scope.getSupportedCharts = function (charts) {
        var outCharts = [];

        for (var chart = 0; chart < charts.length; chart++) {
            var thisChart = charts[chart];
            var thisChartType = thisChart['chartType'];
            if (EwavConfiguration.supportedChartTypes.indexOf(thisChartType) > -1) {
                outCharts.push(charts[chart]);
            }
        }
        return outCharts;
    };


    $scope.getRecordCount = function (result) {
        var recordCount = 0;
        for (var i = 0; i < result.rows.length; i++) {
            for (var j = 1; j < result.rows[i].c.length; j++) {
                if (result.rows[i].c[j].v) {
                    recordCount += parseInt(result.rows[i].c[j].v);
                }

            }

        }
        return recordCount;

    }
    //Following method calculates record count for Pareto and Pie Charts .
    $scope.getPRecordCount = function (result) {

        var recordCount = 0;
        for (var i = 0; i < result.rows.length; i++) {
            recordCount += parseInt(result.rows[i].c[1].v);

        }
        return recordCount;
    }


    $scope.selectChartType = function (chart, Rules, dataFilters, $window) {
        $scope.chart = {};
        $scope.chart.type = chart.chartType;
        $scope.chart.typeName = chart.gadgetName;

        $scope.chart.gadgetTitle = chart.gadgetName;
        $scope.chart.gadgetDescription = chart.gadgetDescription;

        /*    var w = angular.element($window);

         var resizeCheck = function () {
         console.log('resize');
         }



         w.bind('resize', resizeCheck);

         console.log(resizeCheck);

         */
        var postParameterCallBackHandler = function (result, status) {
            $scope.chart.data = new google.visualization.DataTable(result);

            var recordCount;

            if ($scope.chart.type.toLowerCase() == "pie" ||
                $scope.chart.type.toLowerCase() == "pareto") {
                recordCount = $scope.getPRecordCount(result);

            }
            else if ($scope.chart.type.toLowerCase() == "scatter") {
                recordCount = result.rows.length - 2;
            }
            else if ($scope.chart.type.toLowerCase() == "stackedcolumn") {
                recordCount = $scope.getRecordCount(result);
                $scope.chart.columnAggregateFunction = chart.columnAggregateFunction;
            }
            else {
                recordCount = $scope.getRecordCount(result);
            }

            $scope.chart.RecordCountLabel = 'N = ' + recordCount;
            $scope.ShowChart = 1;
        };
        $scope.ShowChart = 0;

        if ($scope.chart.type.toLowerCase() == "bar") {
            $scope.chart.xlbl = chart.yAxisLabel;
            $scope.chart.ylbl = chart.xAxisLabel;
        }
        else if ($scope.chart.type.toLowerCase() == "scatter") {
            $scope.chart.xlbl = chart.xAxisScatterVariable;
            $scope.chart.ylbl = chart.yAxisScatterVariable;
        }
        else {
            $scope.chart.xlbl = chart.xAxisLabel;
            $scope.chart.ylbl = chart.yAxisLabel;
        }


        if ($scope.chart.type.toLowerCase() == "epicurve") {
            epicurveService.postChartData(chart, Rules, dataFilters).success(postParameterCallBackHandler);
        }
        else if ($scope.chart.type.toLowerCase() == "scatter") {
            scatterService.postChartData(chart, Rules, dataFilters).success(postParameterCallBackHandler);
        }
        else {
            chartService.postChartData(chart, Rules, dataFilters).success(postParameterCallBackHandler);
        }

    }


});