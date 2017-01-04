(function (module) {
    'use strict';

    module.directive('talHighlight', function () {
        return {
            restrict: 'A',
            scope: {
                talHighlight: '='
            },
            link: function (scope, elem) {
                scope.$watch('talHighlight', function (newValue) {
                    elem.toggleClass('highlight', (newValue !== undefined));
                });
            }
        };
    });
})(angular.module('salesPortalApp'));