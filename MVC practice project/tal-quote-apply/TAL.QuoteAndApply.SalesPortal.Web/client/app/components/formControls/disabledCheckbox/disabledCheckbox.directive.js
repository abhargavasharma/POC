(function (module) {
    'use strict';
    module.directive('talDisabledCheckbox', function () {
        return {
            restrict: 'A',
            scope: {
                talDisabledCheckbox: '=',
                model: '=ngModel'
            },
            require: 'ngModel',
            link: function (scope, elem) {
                scope.$watch('talDisabledCheckbox', function () {

                    var val = scope.talDisabledCheckbox;
                    if (val === true) {
                        // disable and uncheck
                        elem.attr('disabled', 'disabled');
                        scope.model = false;
                    } else if (val === false) {
                        // enable
                        elem.removeAttr('disabled');
                    }
                }, true);

            }

        };
    });

})(angular.module('salesPortalApp'));

