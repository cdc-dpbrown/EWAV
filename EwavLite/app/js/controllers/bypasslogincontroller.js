'use strict';

controllerModule.controller('ByPassLoginCtrl', function ($window, $scope, $location, $rootScope,
    byPassLoginService, canvasService) {
    $scope.getParameterByName = function (name, url) {
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

   $scope.submit = function () {

       var parm = $location.$$absUrl.substring(0).split('&')[0];
       var pos = parm.indexOf('=');
       if (pos == -1) {//no parameters passed.
           return;
       }

       var UserId = $scope.getParameterByName("userid");
       var DatasourceId = $scope.getParameterByName("datasourceid");
       var CanvasId = $scope.getParameterByName("canvasId");

       var parm2 = $location.$$absUrl.substring(0).split('&')[1];
       //  if (undefined != parm2) {//not coming from EWE
       if (CanvasId == null) {//not coming from EWE
           // var pos2 = parm2.indexOf('=');
           // var DatasourceId = parm2.substring(pos2 + 1);
           byPassLoginService.validate(UserId, DatasourceId).success(
               function (result) {
                   if (result == "true") {
                       $scope.ByPassLogin = 1;
                       $rootScope.$broadcast('read-canvases-login-bypassed');
                   }
                   else {

                       alert('Error. You do not have access or have no assigned canvas. Please contact the owner of the dashboard to request access.');

                   }
               }
           ).error(function (data, status) {
               alert('Error calling Login Api from bypass Login controller .');
           });

       }
       else {
           $scope.ByPassLogin = 0;
       }

   },
       $scope.selectcanvas = function (canvas) {
           var parm = $location.$$absUrl.substring(0).split('?')[0];
           var pos = parm.indexOf('=');
           $scope.DisplayCanvas = 1;
           $location.$$absUrl = parm + '?canvasid=' + canvas.CanvasGUID;
           $rootScope.$broadcast('load-canvas-login-bypassed');
       },
       $scope.readcanvases = function () {
           var parm = $location.$$absUrl.substring(0).split('&')[0];
           var pos = parm.indexOf('=');
           //  var UserId = parm.substring(pos + 1);


           var UserId = $scope.getParameterByName("userid");
           var DatasourceId = $scope.getParameterByName("datasourceid");
           var CanvasId = $scope.getParameterByName("canvasId");

           var parm2 = $location.$$absUrl.substring(0).split('&')[1];
           var pos2 = parm2.indexOf('=');
           //  var DatasourceId = parm2.substring(pos2 + 1);    

           canvasService.readcanvases(UserId, DatasourceId).success(
               function (result) {
                   $scope.Canvases = result;
                   $scope.canvas = {};
                   if ($scope.Canvases.length == 1) {
                       $scope.selectcanvas($scope.Canvases[0]);
                   }
               }
           ).error(function (data, status) {
               alert('Error in canvasService.readcanvases function ');
           });


       },
       $rootScope.$on('read-canvases-login-bypassed', function (event, args) {
           $scope.readcanvases();
       });


});