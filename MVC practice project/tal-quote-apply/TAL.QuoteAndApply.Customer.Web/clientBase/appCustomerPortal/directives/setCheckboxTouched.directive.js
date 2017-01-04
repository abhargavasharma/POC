'use strict';

angular.module('appCustomerPortal').directive('talSetCheckboxTouched', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function(scope, elem, attrs, ngModel) {

            //Register a parser that will set element to touched and therefore displays error if user changes value
            ngModel.$parsers.unshift(function(value) {
                ngModel.$setTouched();
                return value;
            });
        }
    };
});
