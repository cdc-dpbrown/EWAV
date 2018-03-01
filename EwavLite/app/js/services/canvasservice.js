'use strict';

servicesModule.factory('canvasService', function ($resource, $http) {


        return {
            getData: function (canvasid) {
                var currentUrl = globalURL;
                var canvasResource = $resource(currentUrl + 'api/canvas?id=:id', {}, {});
                return canvasResource.get({id: canvasid});
            },
            readcanvases: function (userid, datasourceid) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/canvas';
                var bigObject = {
                    id: userid,
                    datasourceid: datasourceid
                }
                return $http.post(url, bigObject);

            }           
        }
    }
); 