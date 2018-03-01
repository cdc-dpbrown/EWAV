'use strict';

controllerModule.controller('GadgetCtrl', function ($scope, $location, $rootScope, $window,
                                                    canvasService,
                                                    frequencyService,
                                                    combinedFrequencyService,
                                                    linearRegressionService,
                                                    logisticRegressionService,
                                                    meansService,
                                                    mapService) {
    var parm = $location.$$absUrl.substring(0).split('&')[0];
    var pos = parm.indexOf('=');
    var canvasid = parm.substring(pos + 1);
    $scope.ShowGadget = 1;
    $scope.goLegends = false;

    var w = angular.element($window);

    w.bind('resize', function () {
        if ($scope && $scope.gadget && $scope.gadget.mapdirective ) {
            console.log($scope.gadget.mapdirective);
            var tempMapData = $scope.gadget.mapdirective;
            tempMapData[0].Collection[0].Data += ' ';
            $scope.gadget.mapdirective = tempMapData;
            console.log($scope.gadget.mapdirective);
            $scope.$apply();
        }
    });

    $rootScope.$on('canvas-recieved-event', function (event, args) {
        $scope.selectType($scope.gadgets[0], $scope.Rules, $scope.DataFilters);
    });

    $scope.selectType = function (gadget, Rules, dataFilters) {
        $scope.gadget = {};
        $scope.gadget.type = gadget["@gadgetType"];
        $scope.gadget.typeName = gadget.gadgetTitle;
        $scope.gadget.gadgetTitle = gadget.gadgetTitle;
        $scope.gadget.gadgetDescription = gadget.gadgetDescription;
        $scope.ShowLegendButton = "false";//Legend button for Map control


        var postParameterCallBackHandler = function (result) {
            if ($scope.gadget.type == "Ewav.LinearRegression") {
                $scope.gadget.linregdirective = result;
                $scope.ShowGadget = "linregdirective";
            }
            else if ($scope.gadget.type == "Ewav.LogisticRegression") {
                $scope.gadget.logregdirective = result;
                $scope.ShowGadget = "logregdirective";
            }
            else if ($scope.gadget.type == "Ewav.CombinedFrequency") {
                $scope.gadget.combinedfrequencydirective = result;
                $scope.gadget.showdenominator = gadget.showdenominator;
                $scope.ShowGadget = "combinedfrequencydirective";
            }
            else if ($scope.gadget.type == "Ewav.MeansControl") {
                $scope.gadget.meansdirective = result;
                $scope.ShowGadget = "meansdirective";
            }
            else if ($scope.gadget.type == "Ewav.MapControl") {
                $scope.ClusterSize = gadget.ClusterSize;
                $scope.Radius = gadget.Radius;
                $scope.gadget.mapdirective = result;
                $scope.ShowGadget = "mapdirective";
                if (result.length > 1) { //show legend when more than one strata
                    $scope.ShowLegendButton = "true";
                }
                else {
                    $scope.ShowLegendButton = "false";
                }


            }
            else if ($scope.gadget.type == "Ewav.FrequencyControl") {
                $scope.gadget.frequencydirective = result;
                $scope.ShowGadget = "frequencydirective";
            }

            else {
                $scope.gadget.result = result;
                $scope.ShowGadget = 1;
            }

            $scope.gadget.data = new google.visualization.DataTable(result);

        };

        var gadgetType = $scope.gadget.type;
        $scope.ShowGadget = 0;
        if (gadgetType == "Ewav.FrequencyControl") {
            frequencyService.postGadgetData(gadget, Rules, dataFilters).success(postParameterCallBackHandler).error(function (data, status) {
                alert(status);
            });
        }
        else if (gadgetType == "Ewav.CombinedFrequency") {
            combinedFrequencyService.postGadgetData(gadget, Rules, dataFilters).success(postParameterCallBackHandler).error(function (data, status) {
                alert(status);
            });
        }
        else if (gadgetType == "Ewav.LinearRegression") {
            linearRegressionService.postGadgetData(gadget, Rules, dataFilters).success(postParameterCallBackHandler).error(function (data, status) {
                alert(status);
            });
        }

        else if (gadgetType == "Ewav.LogisticRegression") {
            logisticRegressionService.postGadgetData(gadget, Rules, dataFilters).success(postParameterCallBackHandler).error(function (data, status) {
                alert(status);
            });
        }
        else if (gadgetType == "Ewav.MapControl") {
            mapService.postGadgetData(gadget, Rules, dataFilters).success(postParameterCallBackHandler).error(function (data, status) {
                alert(status);
            });
        }
        else if (gadgetType == "Ewav.MeansControl") {
            meansService.postGadgetData(gadget, Rules, dataFilters).success(postParameterCallBackHandler).error(function (data, status) {
                alert(status);
            });
        }
    }
});