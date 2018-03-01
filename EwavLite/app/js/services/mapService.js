'use strict';

servicesModule.service('mapService', function ($http) {

    return {
        postGadgetData: function (gadget, Rules, dataFilters) {
            var currentUrl = globalURL;
            var url = currentUrl + 'api/map';

            var bigObject = {
                gadget: gadget,
                Rules: Rules,
                DataFilters: dataFilters
            };
            return $http.post(url, bigObject);
        }
    }

});   