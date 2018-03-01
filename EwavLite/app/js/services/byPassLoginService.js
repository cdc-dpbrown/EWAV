'use strict';

servicesModule.factory('byPassLoginService', function ($http) {

    return {
        validate: function (userid, datasourceid) {
            var currentUrl = globalURL;
            var url = currentUrl + 'api/login';
            //            var bigObject = {
            //                userid: userid,
            //                datasourceid: datasourceid
            //            }
            return $http.get(url, { params: { userid: userid, datasourceid: datasourceid } });

        }/*,
        readcanvases: function (userid, datasourceid) {
            var currentUrl = globalURL;
            var url = currentUrl + 'api/canvas';
            var bigObject = {
                id: userid,
                datasourceid: datasourceid
            }
            return $http.post(url, bigObject);

        }*/
    }
}
);