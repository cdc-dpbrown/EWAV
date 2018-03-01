directiveModule.directive('combinedfrequencydirective', function ($locale, $filter) {
    return {
        restrict: 'A',

        link: function ($scope, elm, attrs) {

            $scope.$watch('gadget.combinedfrequencydirective', function () {

                var type = $scope.gadget.type;

                var name = $scope.gadget.typeName;

                var options = {
                    title: name, allowHtml: true, showRowNumber: true
                };
                var gadget = " ";

                var element = "<div class=table-responsive><table class= table CombinedFrequency><tr><th>Value</th><th>Frequency</th><th>Percent</th></tr>";
                var boolResults = $scope.gadget.combinedfrequencydirective.ExtraInfo[0].Value.VarName.toLowerCase();
                var fields = $scope.gadget.combinedfrequencydirective.ExtraInfo[1].Value.VarName;
                var denom = $scope.gadget.combinedfrequencydirective.ExtraInfo[2].Value.VarName;

                if (boolResults == 'false') { // equaivalent to !boolResults as in Ewav
                    denom = denom * fields;
                }

                for (var i = 0; i < $scope.gadget.combinedfrequencydirective.RecordList.length; i++) {
                    var pct = (($scope.gadget.combinedfrequencydirective.RecordList[i].Fields[1].VarName / denom) * 100); //.toFixed(2) + '%';
                    element = element + "<tr><td>" + $scope.gadget.combinedfrequencydirective.RecordList[i].Fields[0].VarName + " </td><td>" + $scope.gadget.combinedfrequencydirective.RecordList[i].Fields[1].VarName + "</td><td> " + $filter('number')(pct, 2) + '%' + "</td></tr>";

                }
                element = element + "</table></div>";

                if ($scope.gadget.showdenominator == "true") {
                    if (boolResults.toLowerCase() == "false") {
                        //element += "<tr><td>Fields are not boolean.</td><td></td><td></td></tr>";
                        //element += "<tr><td>Denominator:</td><td> " + denom * fields + "</td><td></td></tr>";

                        element += "<h4 class=stratheader>Fields are not boolean</h4>";
                        element += "<h4 class=stratheader>Denominator: " + denom + "</h4>";
                    }
                    else {

                        //element += "<tr><td>Fields are boolean.</td><td></td><td></td></tr>";
                        //element += "<tr><td>Denominator:</td><td> " + denom + "</td><td></td></tr>";
                        element += "<h4 class=stratheader>Fields are boolean</h4>";
                        element += "<h4 class=stratheader>Denominator: " + denom + "</h4>";
                    }
                }


                //element = element + "</table>";

                elm[0].innerHTML = element;

            }, true);
        }
    }
});