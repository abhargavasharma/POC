(function (module) {
    'use strict';

    module.directive('hasError', function () {
        return {
            restrict: 'A',
            scope : {
                hasError: '='
            },
            link: function (scope, elem) {
                scope.$watch('hasError', function(newValue) {
                    elem.toggleClass('has-error', (newValue !== undefined));
                });
            }
        };
    });
})(angular.module('salesPortalApp'));