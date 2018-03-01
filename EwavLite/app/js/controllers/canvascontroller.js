'use strict';

/**
 * Created by shorterd01, usmanm01 on 6/7/2014.
 */


controllerModule.controller('CanvasCtrl', function ($scope, $location, $rootScope, canvasService) {
    var parm = window.location.search.substring(1).split('&')[0];
    var pos = parm.indexOf('=');
    var canvasid = parm.substring(pos + 1);

    // Defaults
    $scope.canvasName = "Canvas Name";
    $scope.datasourceName = "Data Source  Name";
    $scope.recordCount = "Record Count";

    var postParameterCallBackHandler = function (result, status) {
        $scope.canvasName = result['CanvasName'];
        $scope.datasourceName = result['Datasource'];
        $scope.recordCount = "Record Count";
    };

    $scope.getSupportedGadgets = function (gadgets) {
        var outGadgets = [];

        for (var gadget = 0; gadget < gadgets.length; gadget++) {
            var thisGadget = gadgets[gadget];
            var thisGadgetType = thisGadget['@gadgetType'];
            if (EwavConfiguration.supportedGadgets.indexOf(thisGadgetType) > -1) {
                outGadgets.push(gadgets[gadget]);
            }
        }
        return outGadgets;
    };

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

    $rootScope.$on('user-logged-in', function () {
        var parm = $location.$$absUrl.substring(0).split('&')[0];
        var pos = parm.indexOf('=');
        var canvasid = parm.substring(pos + 1);

        //alert('User Logged in requesting data');

        canvasService.getData(canvasid).$promise.then(
            function (result) {
                var allGadgets = result.DashboardCanvas.Gadgets.gadget;
                if (allGadgets) {
                    //  if there are any gadgets    
                    var supportedGadgets;
                    if (!allGadgets.length) {
                        // if there is just one gadget    
                        supportedGadgets = $scope.getSupportedGadgets([allGadgets]);
                    }
                    else {
                        // if there more than one gadget     
                        supportedGadgets = $scope.getSupportedGadgets(allGadgets);
                    }
                }

                var allCharts = result.DashboardCanvas.Gadgets.chart;
                if (allCharts) {
                    //  if there are any charts   
                    var supportedCharts;
                    if (!allCharts.length) {
                        // if there is just one chart  
                        supportedCharts = $scope.getSupportedCharts([allCharts]);
                    }
                    else {
                        // if there is more than one chart
                        supportedCharts = $scope.getSupportedCharts(allCharts);
                    }

                }

                $scope.gadgets = supportedGadgets;
                $scope.charts = supportedCharts;
                $scope.Rules = result.DashboardCanvas.Rules;
                $scope.DataFilters = result.DashboardCanvas.DataFilters;
                $scope.canvasName = result.DashboardCanvas['@CanvasName'];
                $scope.datasourceName = result.DashboardCanvas['@DatasourceName'];
                $scope.recordCount = "Record Count";
                $rootScope.$broadcast('canvas-recieved-event');
                //            $rootScope.$broadcast('canvas-recieved-event-charts');
            },
            function (response) {
                if (response.status == "403") {
                    alert('You are not authenticated in the system.');
                }
                else {
                    alert('Error Reading canvas:' + JSON.stringify(response.status));
                }
            }
        );
    });
});



