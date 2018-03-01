'use strict';

servicesModule.factory('scatterService', function ($http) {
        return {
            postChartData: function (chart, Rules, dataFilters) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/scatter';

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