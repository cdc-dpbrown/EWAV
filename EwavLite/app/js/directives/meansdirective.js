directiveModule.directive('meansdirective', function ($locale, $filter) {
    return {
        restrict: 'A',

        link: function ($scope, elm, attrs) {

            $scope.$watch('gadget.meansdirective', function () {
                //                    alert(JSON.stringify($scope.gadget.result));
                //                    console.log(JSON.stringify($scope.gadget.result));

                var type = $scope.gadget.type;

                var name = $scope.gadget.typeName;

                var options = {
                    title: name, allowHtml: true, showRowNumber: true
                };
                var gadget = " ";


                var stratas;
                var columnNames;
                var crossTab = false;

                var result = $scope.gadget.meansdirective; //.response.data;
                if (result) {

                    if (result.GadgetParameters.CrosstabVariableName.length > 0) {
                        crossTab = true;
                    }

                    if (crossTab) { //crosstab is true
                        stratas = result.FreqResult.CrossTable;
                        //columnNames = $scope.gadget.result.FrequencyResultData.CrossTable[0].ColumnNames;
                    }
                    else {
                        stratas = result.FreqResult;
                    }


                    var mainVariable = result.GadgetParameters.MainVariableName;

                    var BigElement = "";

                    for (var i = 0; i < stratas.length; i++) {
                        var header = '', headerContents;
                        var subStrataLength;
                        var crossTabVar = '';
                        if (crossTab) { //crosstab is true  
                            headerContents = result.FreqResult.FrequencyTable[i].FrequencyControlDtoList[0].NameOfDtoList;
                            columnNames = result.FreqResult.CrossTable[i].ColumnNames;
                            subStrataLength = result.FreqResult.CrossTable[i].DsList.length;
                            crossTabVar = result.GadgetParameters.MainVariableName + '*' + result.GadgetParameters.CrosstabVariableName;
                        }
                        else {
                            headerContents = stratas[i].FrequencyControlDtoList[0].NameOfDtoList;
                            subStrataLength = stratas[i].DescriptiveStatisticsList.length;
                        }

                        if (stratas.length > 1) {
                            header = "<div><h3>" + headerContents + "</h3></div>";
                        }

                        var element = "<div><h4 class=stratheader>" + header + "</h4></div><div class=table-responsive><table class= table Frequency><tr><th>" + crossTabVar + "</th><th>Obs</th><th>Total</th><th>Mean</th><th>Var</th><th>Std Dev</th><th>Min</th><th>25%</th><th>Median</th><th>75%</th><th>Max</th><th>Mode</th></tr>";


                        for (var j = 0; j < subStrataLength; j++) {

                            var count, means;
                            if (crossTab) { //crosstab true

                                count = stratas[i].DsList[j].Observations;

                                means = stratas[i].DsList[j];

                            }
                            else {
                                count = stratas[i].DescriptiveStatisticsList[j].Observations;

                                means = stratas[i].DescriptiveStatisticsList[j];
                            }


                            if (crossTab) {//crosstab is true
                                mainVariable = columnNames[j];
                            }


                            //var frequencyDtoList = stratas[i].FrequencyControlDtoList;
                            //                        var accumulatedTotal = 0;
                            //                        var frequencyTotal = 0;


                            element = element + "<tr><td>" + mainVariable + "</td><td>" + means.Observations + "</td><td>" + means.Sum + " </td><td>" + $filter('number')(means.Mean, 4) + "</td><td> " + $filter('number')(means.Variance, 4) + "</td><td>" + $filter('number')(means.StdDev, 4) + "</td><td>" + $filter('number')(means.Min, 4) + "</td><td>" + $filter('number')(means.Q1, 4) + "</td><td>" + $filter('number')(means.Median, 4) + "</td><td>" + $filter('number')(means.Q3, 4) + "</td><td>" + $filter('number')(means.Max, 4) + "</td><td>" + $filter('number')(means.Mode, 4) + "</td></tr>";
                            //                        element += "<tr><td><strong>TOTAL</strong></td><td><strong>" + frequencyTotal + "</strong></td><td><strong>100%</strong></td><td><strong>100%</strong></td><td></td><td></td></tr>";

                        }

                        element += "</table></div>";

                        var anovaDiv = '';

                        if (crossTab) {//crosstab is true
                            means = stratas[i].DsList[0];
                        }
                        else {
                            means = stratas[i].DescriptiveStatisticsList[0];
                        }


                        if (means && crossTab) {
                            var SsBetween, FStatistic, SsWithin,
                                AnovaPValue, MsWithin, ChiSquare, BartlettPValue,
                                KruskalWallisH, KruskalPValue;

                            if (means.SsBetween == null) {
                                SsBetween = 'NaN';
                            }
                            else {
                                SsBetween = $filter('number')(means.SsBetween, 4);
                            }

                            if (means.MsBetween == null) {
                                MsBetween = 'NaN';
                            }
                            else {
                                MsBetween = $filter('number')(means.MsBetween, 4)
                            }

                            if (means.FStatistic == null) {
                                FStatistic = 'NaN';
                            }
                            else {
                                FStatistic = $filter('number')(means.FStatistic, 4)
                            }

                            if (means.SsWithin == null) {
                                SsWithin = 'NaN';
                            }
                            else {
                                SsWithin = $filter('number')(means.SsWithin, 4)
                            }

                            if (means.MsWithin == null) {
                                MsWithin = 'NaN';
                            }
                            else {
                                MsWithin = $filter('number')(means.MsWithin, 4)
                            }

                            if (means.AnovaPValue == null) {
                                AnovaPValue = 'NaN';
                            }
                            else {
                                AnovaPValue = $filter('number')(means.AnovaPValue, 4)
                            }

                            if (means.ChiSquare == null) {
                                ChiSquare = 'NaN';
                            }
                            else {
                                ChiSquare = $filter('number')(means.ChiSquare, 4)
                            }

                            if (means.BartlettPValue == null) {
                                BartlettPValue = 'NaN';
                            }
                            else {
                                BartlettPValue = $filter('number')(means.BartlettPValue, 4)
                            }

                            if (means.KruskalWallisH == null) {
                                KruskalWallisH = 'NaN';
                            }
                            else {
                                KruskalWallisH = $filter('number')(means.KruskalWallisH, 4)
                            }
                            if (means.KruskalPValue == null) {
                                KruskalPValue = 'NaN';
                            }
                            else {
                                KruskalPValue = $filter('number')(means.KruskalPValue, 4)
                            }
                            anovaDiv += "<div><h4 class=stratheader>ANOVA, a Parametric Test for Inequality of Population Means</h4><p>(For normally distributed data only)</p></div>";
                            anovaDiv += "<div class=table-responsive><table class= table meansstrat><tr><th>Variation</th><th>SS</th><th>dF</th><th>MS</th><th>F-Statistic</th></tr>";
                            anovaDiv += "<tr><td>Between</td><td>" + SsBetween + "</td><td>" + means.DfBetween + "</td><td>" + MsBetween + "</td><td>" + FStatistic + "</td></tr>";
                            anovaDiv += "<tr><td>Within</td><td>" + SsWithin + "</td><td>" + means.DfWithin + "</td><td>" + MsWithin + "</td><td></td></tr>";
                            anovaDiv += "<tr><td>Total</td><td>" + $filter('number')(means.SsBetween + means.SsWithin, 4) + "</td><td>" + (means.DfBetween + means.DfWithin) + "</td><td></td><td></td></tr></table></div>";

                            anovaDiv += "<table class='table meansstrat'><tr><td style='width: 47%;'>P-Value</td><td>" + AnovaPValue + "</td></tr></table>";

                            anovaDiv += "<div><h4 class=stratheader>Bartlestt's Test for Inequality of Population Variances</h4>";

                            anovaDiv += "<table class='table meansstrat'><tr><td style='width: 47%;'>Chi Square</td><td>" + ChiSquare + "</td></tr>";
                            anovaDiv += "<tr><td>Degrees of freedom</td><td>" + means.DfBetween + "</td></tr>";
                            anovaDiv += "<tr><td>P-Value</td><td>" + BartlettPValue + "</td></tr></table>";

                            anovaDiv += "<p style=margin-top:-9px; margin-bottom:15px;>A small p-value (e.g., less than 0.05) suggests that the variances are not homogeneous and that the ANOVA may not be appropriate.</p></div>";
                            anovaDiv += "<div><h4 class=stratheader>Mann-Whitney/Wilcoxon Two-Sample Test (Kruskal-Wallis test for two groups)</h4>";

                            anovaDiv += "<table class='table meansstrat'><tr><td style='width: 47%;'>Kruskal-Wallis H</td><td>" + KruskalWallisH + "</td></tr>";
                            anovaDiv += "<tr><td>Degrees of freedom</td><td>" + means.DfBetween + "</td></tr>";
                            anovaDiv += "<tr><td>P-Value</td><td>" + KruskalPValue + "</td></tr></table></div>";
                            element = element + anovaDiv
                        }

                        BigElement += element;


                    }

                    elm[0].innerHTML = BigElement;
                }
            }, true);
        }
    }
});