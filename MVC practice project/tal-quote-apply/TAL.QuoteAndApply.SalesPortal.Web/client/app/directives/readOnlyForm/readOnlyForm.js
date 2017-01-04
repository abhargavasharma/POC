(function(module) {
    'use strict';
    module.directive('readOnlyForm', function () {
        return {
            restrict: 'EA',
            scope: {
                readOnlyForm: '='
            },
            link: function(scope, el) {
                function toggle(readonly) {

                    el.find('input').prop('readonly', readonly);
                    el.find('input').prop('disabled', readonly);

                    el.find('textarea').prop('readonly', readonly);
                    el.find('textarea').prop('disabled', readonly);

                    el.find('select').prop('disabled', readonly);
                }

                scope.$watch('readOnlyForm', function(val) {
                    if (angular.isDefined(val)) {
                        toggle(val);
                    }
                });
            }
        };
    });
})(angular.module('salesPortalApp'));