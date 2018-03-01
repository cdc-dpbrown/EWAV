'use strict';

servicesModule.factory('frequencyService', function ($http) {
        return {
            postGadgetData: function (gadget, Rules, dataFilters) {
                var currentUrl = globalURL;
                //var url = 'http://localhost/EWAV.WebApi/api/frequency';
                var url = currentUrl + 'api/frequency';

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