'use strict';

controllerModule.controller('PublishCtrl', function ($scope, $location, publishService) {
    $scope.submit = function (UserName, Password) {
        publishService.login(UserName, Password).success(
            function (result) {
                $scope.IsUserLoggedIn = 1;
                //            $scope.canvases = {};
                $scope.Canvases = result;
                $scope.canvas = {};
            }
        ).error(function (data, status) {
            alert('The email or password you entered is incorrect.');
        });

        $scope.selectType = function (canvas) {
            var parm = $location.$$absUrl.substring(0).split('&')[0];
            var pos = parm.lastIndexOf('/');
            var url = parm.substring(0, pos);
            $scope.url = url + "/index.html?canvasid=" + canvas.CanvasId;
        }
    }
});