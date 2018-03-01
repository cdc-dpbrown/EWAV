'use strict';

servicesModule.factory('linearRegressionService', function ($http) {
        return {
            postGadgetData: function (gadget, Rules, dataFilters) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/LinearRegression';

                var bigObject = {
                    gadget: gadget,
                    Rules: Rules,
                    DataFilters: dataFilters
                };

                return $http.post(url, bigObject);
            }
        }
    }
);