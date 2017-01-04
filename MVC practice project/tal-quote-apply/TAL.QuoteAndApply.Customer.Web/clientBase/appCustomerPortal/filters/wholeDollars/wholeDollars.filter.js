(function(module){
    'use strict';
    module.filter('wholeDollars', ['$filter',function ($filter) {
        return function(input) {
            if(isNaN(input)){
                return input;
            } else {
                return $filter('currency')(input, '$', 0);
            }
        };
    }]);

})(angular.module('appCustomerPortal'));
