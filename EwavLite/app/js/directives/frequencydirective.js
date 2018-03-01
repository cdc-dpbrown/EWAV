directiveModule.directive('frequencydirective', function ($locale, $filter) {
    return {
        restrict: 'A',

        link: function ($scope, elm, attrs) {

            $scope.$watch('gadget.frequencydirective', function () {
                //                    alert(JSON.stringify($scope.gadget.result));
                //                    console.log(JSON.stringify($scope.gadget.result));

                //  var type = $scope.gadget.type;


                var name = $scope.gadget.typeName;

                var options = {
                    title: name, allowHtml: true, showRowNumber: true
                };
                var gadget = " ";

                var stratas = $scope.gadget.frequencydirective.FreqResult;

                var mainVariable = $scope.gadget.frequencydirective.GadgetParameters.MainVariableName;

                var showconflimits = $scope.gadget.frequencydirective.GadgetParameters.InputVariableList.showconflimits;

                var showcomperc = $scope.gadget.frequencydirective.GadgetParameters.InputVariableList.showcumulativepercent;

                var BigElement = "";

                for (var i = 0; i < stratas.length; i++) {

                    var count = stratas[i].DescriptiveStatisticsList[0].Observations;

                    var element = "<div><h4 class=stratheader>" + stratas[i].FrequencyControlDtoList[0].NameOfDtoList + "</h4></div><div class=table-responsive><table class= table Frequency><tr><th>" + mainVariable + "</th><th>Frequency</th><th>Percent</th>";

                    if (showcomperc == "true") {
                        element += "<th>Cum. Percent</th>";
                    }

                    if (showconflimits == "true") {
                        element += "<th>95% CI Lower</th><th>95% CI Upper</th>";
                    }
                    element += "</tr>";

                    var frequencyDtoList = stratas[i].FrequencyControlDtoList;
                    var accumulatedTotal = 0;
                    var frequencyTotal = 0;
                    for (var j = 0; j < frequencyDtoList.length; j++) {
                        var pct = (frequencyDtoList[j].FrequencyColumn / count) * 100;
                        frequencyTotal = frequencyTotal + parseInt(frequencyDtoList[j].FrequencyColumn);
                        accumulatedTotal = accumulatedTotal + pct;


                        //have to do the following coz parseFloat doesnt take locale into account.
                        var percLower = frequencyDtoList[j].Perc95ClLowerColumn.replace(',', '.');
                        var percUpper = frequencyDtoList[j].Perc95ClUpperColumn.replace(',', '.');
                        element = element + "<tr><td>" + frequencyDtoList[j].FreqVariable + " </td><td>" + frequencyDtoList[j].FrequencyColumn + "</td><td> " + $filter('number')(pct, 2) + '%' + "</td>";
                        if (showcomperc == "true") {
                            element += "<td>" + $filter('number')(accumulatedTotal, 2) + '%' + "</td>";
                        }

                        if (showconflimits == "true") {
                            element += "<td>" + $filter('number')(percLower * 100, 2) + '%' + "</td><td>" + $filter('number')(percUpper * 100, 2) + '%' + "</td>";
                        }
                        element += "</tr>";

                    }
                    element += "<tr><td><strong>TOTAL</strong></td><td><strong>" + frequencyTotal + "</strong></td><td><strong>100%</strong></td>";

                    if (showcomperc == "true") {
                        element += "<td><strong>100%</strong></td>";
                    }

                    if (showconflimits == "true") {
                        element += "<td></td><td></td>";
                    }
                    element += "</tr>";

                    element += "</table></div><BR/><BR/>";
                    BigElement += element;
                }

                elm[0].innerHTML = BigElement;

            }, true);
        }
    }
});