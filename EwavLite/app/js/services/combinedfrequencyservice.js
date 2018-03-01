'use strict';

servicesModule.factory('combinedFrequencyService', function ($http) {
        return {
            postGadgetData: function (gadget, Rules, dataFilters) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/combinedfrequency';
                var bigObject = {
                    gadget: gadget,
                    Rules: Rules,
                    DataFilters: dataFilters
                }
                return $http.post(url, bigObject);
            }
        }
    }
);