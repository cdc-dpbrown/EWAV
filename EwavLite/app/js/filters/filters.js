'use strict';

/* Filters */

angular.module('myApp.filters', []).filter('interpolate', ['version', function (version) {
    return function (text) {
        return String(text).replace(/\%VERSION\%/mg, version);
    }
}]);

//eventsApp.filter('durations', function() {
//    return function(duration) {
//        switch(duration) {
//            case 1:
//                return "Half Hour";
//            case 2:
//                return "1 Hour";
//            case 3:
//                return "Half Day";
//            case 4:
//                return "Full Day";
//        }
//    }
//})
//
//


//  angular.module('myApp.filters', []).filter('supportedGadgets', function() {
//    return function(tasksSupported, tagsChart ) {
//        return tasksSupported.filter(function(taskSupported) {
//
//            var filtered = []        ;
//
//            for (var i in taskSupported  ) {
//                if (tagsChart.GadgetName.indexOf(taskSupported[i]) != -1) { 
//                     filtered.push( taskSupported[i]  )        ;
//                    // return true;
//                }
//            }
//            return  filtered            ;
//
//        });
//    };
//});
