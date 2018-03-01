    'use strict';


// Declare app level module which depends on filters, and services
var myApp = angular.module('myApp', [
    'ngRoute',
    'myApp.filters',
    'myApp.services',
    'myApp.directives',
    'myApp.controllers',
    // 'myApp.utilities',
    // 'myApp.delayModule',
    'ui.bootstrap'
]).config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/view1', {
        templateUrl: 'partials/partial1.html',
        controller: 'MyCtrl1'
    });
    $routeProvider.when('/', {templateUrl: 'index.html', controller: 'MyCtrl1'});
    $routeProvider.otherwise({redirectTo: '/view1'});
}]);


google.load('visualization', '1', {packages: ['corechart', 'table']});
google.setOnLoadCallback(function () {
    angular.bootstrap(document.body, ['myApp']);
});


var globalURL =      "http://localhost:51110/";


var servicesModule = angular.module('myApp.services', ['ngResource']);


var controllerModule = angular.module('myApp.controllers', []);

var directiveModule = angular.module('myApp.directives', []).directive('appVersion', ['version',
    function (version) {
        return function ($scope, elm, attrs) {
            elm.text(version);
        };
    }
]);
