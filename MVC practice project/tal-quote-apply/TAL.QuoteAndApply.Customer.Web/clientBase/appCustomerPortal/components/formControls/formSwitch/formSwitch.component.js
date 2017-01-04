(function(module){
    'use strict';

    var uniqueId = 0;

    module.directive('talFormSwitch', ['$timeout', function ($timeout) {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/formSwitch/formSwitch.template.html',
            restrict: 'E',
            transclude: true,
            scope: {
                ngModel: '=',
                stateChange: '&'
            },
            require: 'ngModel',
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talFormSwitchController',
            link: function(scope, elem, attrs, ngModelCtrl) {

                scope.id = uniqueId;
                uniqueId += 1;

                scope.toggle = function() {
                    ngModelCtrl.$setViewValue(!ngModelCtrl.$modelValue);
                    ngModelCtrl.$render();
                    $timeout(function() {
                        if (scope.ctrl.stateChange) {
                            scope.ctrl.stateChange();
                        }
                    });
                };

                scope.setOn = function() {
                    if (!scope.isActive) {
                        scope.toggle();
                    }
                };

                scope.setOff = function() {
                    if (scope.isActive) {
                        scope.toggle();
                    }
                };

            	ngModelCtrl.$render = function() {
                    scope.isActive = ngModelCtrl.$modelValue;
                };
            	
            }
        };
    }]);

    function talFormSwitchController() {

    }

    module.controller('talFormSwitchController', talFormSwitchController );
    talFormSwitchController.$inject = [];

})(angular.module('appCustomerPortal'));