'use strict';

servicesModule.factory('publishService', function ($http) {


        return {
            login: function (id, password) {
                var currentUrl = globalURL;
                var url = currentUrl + 'api/publish';
                var bigObject = {
                    id: id,
                    password: password
                }
                return $http.post(url, bigObject);

            }
        }
    }
); 