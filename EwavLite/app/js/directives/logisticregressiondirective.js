directiveModule.directive('logregdirective', function ($locale, $filter) {
    return {
        restrict: 'A',

        link: function ($scope, elm, attrs) {

            $scope.$watch('gadget.logregdirective', function () {
                var variableRows = $scope.gadget.logregdirective.Variables;

                var type = $scope.gadget.type;

                var name = $scope.gadget.typeName;

                var options = {
                    title: name, allowHtml: true, showRowNumber: true
                };
                var gadget = " ";

                var element = "<div class=table-responsive><table class= table LogisticRegression><tr><th></th><th>Odds Ratio</th><th>95%</th><th>C.I.</th><th>Coefficient</th><th>S.E.</th><th>Z-Statistic</th><th>P-Value</th></tr>";
                for (var i = 0; i < variableRows.length; i++) {
                    var RowOddRatio = variableRows[i].OddsRatio;

                    if (RowOddRatio <= -9999) {
                        RowOddRatio = '*';
                    }

                    var RowNinetyFivePercent = variableRows[i].NinetyFivePercent;

                    if (RowNinetyFivePercent <= -9999) {
                        RowNinetyFivePercent = '*';
                    }

                    var RowCi = variableRows[i].Ci;

                    if (RowCi <= -9999) {
                        RowCi = '*';
                    }

                    if (RowCi > 1.0E12) {
                        RowCi = '>1.0E12';
                    }

                    element += "<tr><td>" + variableRows[i].VariableName + "</td><td>" + RowOddRatio + "</td><td>" + RowNinetyFivePercent + "</td><td>" + RowCi + "</td><td>" + variableRows[i].Coefficient + "</td><td>" + variableRows[i].Se + "</td><td>" + variableRows[i].Z + "</td><td>" + variableRows[i].P + "</td></tr>";
                }

                element += "</table></div>";

                var divText = "<div><h4 class=stratheader>Convergence = " + $scope.gadget.logregdirective.Convergence + "</h4></div>";
                divText += "<div><h4 class=stratheader>Iterations = " + $scope.gadget.logregdirective.Iterations + "</h4></div>";
                divText += "<div><h4 class=stratheader>Final -2*Log-Likelihood = " + $scope.gadget.logregdirective.FinalLikelihood + "</h4></div>";
                divText += "<div><h4 class=stratheader>Cases Included = " + $scope.gadget.logregdirective.CasesIncluded + "</h4></div><br/>";

                element += divText;

                element += "<div><table><tr><th>Test</th><th>Statistic</th><th>D.F.</th><th>P-Value</th></tr>";
                element += "<tr><td>Score</td><td>" + $scope.gadget.logregdirective.ScoreStatistic + "</td><td>" + $scope.gadget.logregdirective.ScoreDF + "</td><td>" + $scope.gadget.logregdirective.ScoreP + "</td></tr>";
                element += "<tr><td>Likelihood Ratio</td><td>" + $scope.gadget.logregdirective.LRStatistic + "</td><td>" + $scope.gadget.logregdirective.LRDF + "</td><td>" + $scope.gadget.logregdirective.LRP + "</td></tr>";
                element += "</table></div>";
                elm[0].innerHTML = element;

            }, true);
        }
    }
});