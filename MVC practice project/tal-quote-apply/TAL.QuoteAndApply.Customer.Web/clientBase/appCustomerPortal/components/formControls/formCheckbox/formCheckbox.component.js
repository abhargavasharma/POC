(function(module){
    'use strict';
    module.directive('formCheckbox', function () {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/formCheckbox/formCheckbox.template.html',
            restrict: 'E',
            scope: {
                ngModel: '=',
                id:'@talId',
                name:'@talName',
                required:'=talRequired',
                tooltip: '@talTooltip',
                stateChange: '&'
            },
            transclude:true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'formCheckbox',
            require: 'ngModel',
            link: function (scope, elem, attrs, ngModelCtrl) {
                scope.isDisabled = attrs.disabled;
                scope.ctrl.id = attrs.id;
                scope.ctrl.isDisabled = false;
                if(attrs.disabled !== undefined){
                    scope.ctrl.isDisabled = true;

                }
                scope.toggleCheck =  function($event){

                    if(!attrs.disabled) {
                        ngModelCtrl.$setViewValue( !scope.ctrl.ngModel);
                        ngModelCtrl.$setTouched();
                    }

                    if ($event && $event.target && $event.target.tagName !== 'INPUT') {
                        return;
                    }

                    if ($event && $event.srcElement && $event.srcElement.tagName !== 'INPUT') {
                        return;
                    }

                    if (scope.ctrl.stateChange) {
                        scope.ctrl.stateChange();
                    }
                };

                // Add class to indicate the presence of a tooltip
                if (scope.ctrl.tooltip) {
                    angular.element(elem).addClass('has-tooltip');
                }
            }
        };
    });

    function formCheckbox() {
    }

    module.controller('formCheckbox', formCheckbox );
    formCheckbox.$inject = [];

})(angular.module('appCustomerPortal'));
