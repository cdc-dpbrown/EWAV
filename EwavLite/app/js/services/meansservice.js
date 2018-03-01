'use strict';

servicesModule.factory('meansService', function ($http) {
        return {
            postGadgetData: function (gadget, Rules, dataFilters) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/means';

                var bigObject = {
                    gadget: gadget,
                    Rules: Rules,
                    DataFilters: dataFilters
                };
                //using $q promise to async wait for the response. 
                //            var deffered = $q.defer();

                //            $http.post(url, bigObject).then(function (response) {
                //                $timeout(function () { deffered.resolve({ success: true, response: response }) });
                //            }, function (data, status) {
                //                $timeout(function () { alert('Error retrieving means.'); deffered.reject() });
                //            });
                //            
                //            return deffered.promise;

                return $http.post(url, bigObject);
            }
        }
    }
);