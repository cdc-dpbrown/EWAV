'use strict';

servicesModule.factory('logisticRegressionService', function ($http) {
        return {
            postGadgetData: function (gadget, Rules, dataFilters) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/LogisticRegression';

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