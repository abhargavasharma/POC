(function(module) {
    'use strict';
    module.directive('readOnlyElement', function () {
        return {
            restrict: 'EA',
            scope: {
                readOnlyElement: '='
            },
            link: function(scope, el) {
                function toggle(readonly) {
                    el.attr('readonly', readonly);
                    el.attr('disabled', readonly);

                }
                scope.$watch('readOnlyElement', function(val) {
                    if (angular.isDefined(val)) {
                        toggle(val);
                    }
                });
            }
        };
    });
})(angular.module('salesPortalApp'));