(function (module) {
    'use strict';

    function formSelectTextfieldDirective($timeout) {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/formSelectTextfield/formSelectTextfield.template.html',
            restrict: 'E',
            scope: {
                ngModel: '=',
                formSelectItems: '=',
                stateChange: '&',
                placeholder: '@',
                id: '@talId',
                name: '@talName',
                inlineHelpText: '@'
            },
            transclude: true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'formSelectTextfieldController',
            require: 'ngModel',
            link: function (scope, elem, attrs, ngModelCtrl) {

                // ---- Private Methods ------------------------------------------------------------------
                
                // And apply an 'is-empty' class tot he component is the input is empty.
                function toggleEmplyClass() {
                    elem.toggleClass('is-empty', !scope.ctrl.ngModel);
                }

                // Update the input with a new value (of the select menu).
                function updateInputWithNewSelection(option, isInitialLoad) {
                    var newValue = (option === 'custom') ? '' : option;
                    ngModelCtrl.$setViewValue(newValue);
                    toggleEmplyClass();

                    // Allowing a slight delay to trigger the focus state.
                    if (!isInitialLoad) {
                        $timeout(function() {
                            elem[0].querySelector('input').focus();
                        }, 200);
                    }
                }



                // ---- Prepare scope and initialize component ----------------------------------

                // Set defaults
                scope.ctrl.placeholder = scope.placeholder || 'Choose a cover value...';
                scope.ctrl.formSelectItems = scope.ctrl.formSelectItems || [];

                // Cloning to prevent accidental overwriting
                scope.ctrl.formItems = angular.copy(scope.ctrl.formSelectItems);
                scope.ctrl.selection = angular.copy(scope.ctrl.ngModel);

                // Adding the "custom" option used so we can switch to input.
                scope.ctrl.formItems.unshift('custom');

                // Cache the initial value
                scope.ctrl.initValue = angular.copy(scope.ctrl.ngModel);

                // Update component with initial value
                updateInputWithNewSelection(scope.ctrl.initValue, true);

                // Apply is-empty class initially if required
                toggleEmplyClass();


                // ---- Watchers ------------------------------------------------------------------

                // Watch for interaction with the select menu, and if the selected option changes handle this.
                scope.$watch('ctrl.selection', function (n, o) {
                    if (!n || n === o) {  return; }
                    updateInputWithNewSelection(n);
                });

                // Watch input value and add a class to mark then the field is empty
                scope.$watch('ctrl.ngModel', function (n, o) {
                    if (n === o) {  return; }
                    toggleEmplyClass();
                });
            }
        };
    }

    formSelectTextfieldDirective.$inject = ['$timeout'];

    function formSelectTextfieldController() {

        var ctrl = this;

        // ---- Controller Public Methods ------------------------------------------------------------------

        // On value change, fire the registered callback, if any. Currently used on blur.
        ctrl.valueChanged = function () {
            if (ctrl.stateChange) {
                ctrl.stateChange();
            }
        };
    }

    module.controller('formSelectTextfieldController', formSelectTextfieldController);
    module.directive('formSelectTextfield', formSelectTextfieldDirective);

})(angular.module('appCustomerPortal'));
