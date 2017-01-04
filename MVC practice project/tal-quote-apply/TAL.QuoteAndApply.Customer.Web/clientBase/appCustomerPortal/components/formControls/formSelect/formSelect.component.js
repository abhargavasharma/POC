(function (module) {
    'use strict';
    module.directive('formSelect', function ($timeout) {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/formSelect/formSelect.template.html',
            restrict: 'E',
            scope: {
                ngModel: '=',
                formSelectItems: '=',
                placeholder: '@',
                id:'@talId',
                name:'@talName',
                isCollection: '@?',
                trackByValue: '@?',
                stateChange: '&'
            },
            transclude: true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'formSelect',
            require: 'ngModel',
            link: function (scope, elem, attrs, ngModelCtrl) {
                scope.ctrl.selectedValue = '';

                if (scope.ctrl.placeholder === undefined) {
                    scope.ctrl.placeholder = 'Please select';
                }
                
                scope.ctrl.onChange = function(){
                    // Remove the class which indicates whether placeholder is shown
                    angular.element(elem).removeClass('is-placeholder');

                    $timeout(function() {
                        scope.ctrl.stateChange();
                    });
                };

                // when model change, update our view (just update the div content)
                ngModelCtrl.$render = function () {
                    if (ngModelCtrl.$modelValue !== undefined && ngModelCtrl.$modelValue !==null) {
                        if (scope.ctrl.trackByValue) {
                            var selectedItem = _.find(scope.ctrl.formSelectItems, {value:ngModelCtrl.$modelValue});
                            if (selectedItem) {
                                scope.ctrl.selectedValue = selectedItem.text;
                            } else {
                                scope.ctrl.selectedValue = undefined;
                            }
                        } else {
                            scope.ctrl.selectedValue = ngModelCtrl.$modelValue;
                        }
                    } else {
                        scope.ctrl.selectedValue = undefined;
                    }
                };

                elem.find('select').on('blur', function () {
                    ngModelCtrl.$setTouched();

                    // If no value has been set, then set the value to an empty string
                    // so validation can kick in.

                    if (ngModelCtrl.$modelValue === undefined) {
                        ngModelCtrl.$setViewValue('');
                    }
                });

                elem.find('select').on('focus', function () {
                    ngModelCtrl.$setUntouched();
                });

                // Add a class to indicate whether placeholder is shown
                if (!scope.ctrl.ngModel) {
                    angular.element(elem).addClass('is-placeholder');
                }

                scope.$watch('ctrl.ngModel', function (n, o) {
                    if (n === o) { return; }
                    angular.element(elem).toggleClass('is-placeholder', !scope.ctrl.ngModel);
                });
            }
        };
    });

    function formSelect() {
    }

    module.controller('formSelect', formSelect);
    formSelect.$inject = [];

})(angular.module('appCustomerPortal'));
