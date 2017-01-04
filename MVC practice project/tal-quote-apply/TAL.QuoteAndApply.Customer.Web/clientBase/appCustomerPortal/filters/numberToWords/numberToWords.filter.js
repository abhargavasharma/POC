(function (module) {
    'use strict';

    module.filter('numberToWords', function () {
        var smallNumbers = ['zero', 'one', 'two', 'three', 'four', 'five',
                          'six', 'seven', 'eight', 'nine', 'ten'];
        return function (input) {
            input = parseInt(input, 10);
            if (input < smallNumbers.length) {
                return smallNumbers[input];
            } else {
                return 'not valid';
            }
        };
    });

})(angular.module('appCustomerPortal'));
