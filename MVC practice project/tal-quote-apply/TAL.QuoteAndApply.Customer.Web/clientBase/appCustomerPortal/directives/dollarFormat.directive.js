(function (module) {
    'use strict';
    module.directive('talDollarFormat', ['$filter',function ($filter) {
        return {
            restrict: 'A',
            require: 'ngModel',
            scope : {
            },
            link: function (scope, element, attrs, ngModelCtrl) {
                ngModelCtrl.$formatters.push(function(data) {

                    var output = $filter('currency')(data, '$', 2);
                    //convert data from model format to view format
                    return output; //converted
                });

                ngModelCtrl.$parsers.push(function(data) {
                    //convert data from view format to model format
                    return parseFloat(data.replace('$','')); //converted
                });
            }
        };
    }]);
})(angular.module('appCustomerPortal'));