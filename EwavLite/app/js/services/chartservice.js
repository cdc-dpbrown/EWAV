'use strict';

servicesModule.factory('chartService', function ($http) {
        return {
            postChartData: function (chart, Rules, dataFilters) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/chart';

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


//'use strict';

//servicesModule.factory('chartService', function ($http) {
//    return {
//        postChartData: function (chart) {
//            var url = 'http://localhost/EWAV.WebApi/api/chart';

//            return $http.post(url, chart);
//        }

//    }

//}

//);
