'use strict';

servicesModule.factory('epicurveService', function ($http) {
        return {
            postChartData: function (chart, Rules, dataFilters) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/epicurve';

                var bigObject = {
                    chart: chart,
                    Rules: Rules,
                    DataFilters: dataFilters
                };

                return $http.post(url, bigObject);
            }
        }
    }
);