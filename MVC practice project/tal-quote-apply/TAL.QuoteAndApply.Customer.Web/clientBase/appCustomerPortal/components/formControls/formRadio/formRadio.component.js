(function (module) {
    'use strict';
    module.directive('formRadio', ['$timeout', function ($timeout) {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/formRadio/formRadio.template.html',
            restrict: 'E',
            scope: {
                ngModel: '=',
                id: '@talFormId',
                onOptionChanged: '&',
                onOptionClicked: '&',
                value:'=',
                name: '@',
                disabled: '='
            },
            transclude: true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'formRadio',
            require: 'ngModel',
            link: function (scope) {

                scope.setModelValue = function () {
                    if (scope.ctrl.onOptionChanged) {
                        $timeout(function () {
                            scope.ctrl.onOptionChanged();
                        });
                    }
                };

                scope.onClick = function () {
                    if (scope.ctrl.onOptionClicked) {
                        $timeout(function () {
                            scope.ctrl.onOptionClicked();
                        });
                    }
                };
            }
        };
    }]);

    function formRadio() {
    }

    module.controller('formRadio', formRadio);
    formRadio.$inject = [];

})(angular.module('appCustomerPortal'));
