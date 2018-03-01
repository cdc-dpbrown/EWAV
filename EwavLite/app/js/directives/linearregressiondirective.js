directiveModule.directive('linregdirective', function ($locale, $filter) {
    return {
        restrict: 'A',

        link: function ($scope, elm, attrs) {

            $scope.$watch('gadget.linregdirective', function () {

                var type = $scope.gadget.type;

                var name = $scope.gadget.typeName;

                var options = {
                    title: name, allowHtml: true, showRowNumber: true
                };
                var gadget = " ";

                var variableRows = $scope.gadget.linregdirective.Variables;

                var element = "<div class=table-responsive><table class= table LinearRegression><tr><th>Variable</th><th>Coefficient</th><th>Std Error</th><th>F-test</th><th>P-value</th></tr>";
                for (var i = 0; i < variableRows.length; i++) {
                    element += "<tr><td>" + variableRows[i].VariableName + "</td><td>" + variableRows[i].Coefficient + "</td><td>" + variableRows[i].StdError + "</td><td>" + variableRows[i].Ftest + "</td><td>" + variableRows[i].P + "</td></tr>";
                }

                element += "</table></div>";

                var correlationCof = "<div><h4 class=stratheader>Correlation Coefficient: r^2 = " + $scope.gadget.linregdirective.CorrelationCoefficient + "</h4></div><br />";

                element += correlationCof;

                element += "<div><table><tr><th>Source</th><th>df</th><th>Sum of Squares</th><th>Mean Square</th><th>F-statistic</th></tr>";
                element += "<tr><td>Regression</td><td>" + $scope.gadget.linregdirective.RegressionDf + "</td><td>" + $scope.gadget.linregdirective.RegressionSumOfSquares + "</td><td>" + $scope.gadget.linregdirective.RegressionMeanSquare + "</td><td>" + $scope.gadget.linregdirective.RegressionF + "</td></tr>";
                element += "<tr><td>Residuals</td><td>" + $scope.gadget.linregdirective.ResidualsDf + "</td><td>" + $scope.gadget.linregdirective.ResidualsSumOfSquares + "</td><td>" + $scope.gadget.linregdirective.ResidualsMeanSquare + "</td><td></td></tr>";
                element += "<tr><td>Total</td><td>" + $scope.gadget.linregdirective.TotalDf + "</td><td>" + $scope.gadget.linregdirective.TotalSumOfSquares + "</td><td></td><td></td></tr>";
                element += "</table></div>";
                elm[0].innerHTML = element;

            }, true);
        }
    }
});