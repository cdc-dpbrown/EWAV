'use strict';

function getParameterByName(name, url) {
    if (!url) {
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}


controllerModule.controller('LoginCtrl', function ($scope, $location, $rootScope, loginService) {

    $scope.getParameterByName = function(name, url){
        if (!url) {
            url = window.location.href;
        }
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    },

    $scope.submit = function (UserName, Password) {

        var parm = $location.$$absUrl.substring(0).split('&')[0];
        var pos = parm.indexOf('=');
        //   var canvasid = parm.substring(pos + 1);
        var canvasid = $scope.getParameterByName("canvasId");

        loginService.login(UserName, Password, canvasid).success(
            function (result) {
                if (result.UserName != null) {
                    $scope.IsUserLoggedIn = 1;
                    $rootScope.$broadcast('user-logged-in');
                }
                else {

                    if (result == "not-authorized") {
                        alert('You do not have access to this dashboard. Please contact the owner of the dashboard to request access.');
                    }
                    else //if (result = "not-authenticated") {
                        alert('The email or password you entered is incorrect.');
                    //   }

                }
            }
        ).error(function (data, status) {
            alert('Error calling Login Api from Login controller.');
        });


    },
        $rootScope.$on('load-canvas-login-bypassed', function (event, args) {
            $scope.IsUserLoggedIn = 1;
            $rootScope.$broadcast('user-logged-in');
        });


});