'use strict';

servicesModule.factory('loginService', function ($http) {


        return {
            login: function (id, password, canvasid) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/login';
                var bigObject = {
                    id: id,
                    password: password,
                    canvasid: canvasid
                }
                return $http.post(url, bigObject);

            }
        }
    }
);

 