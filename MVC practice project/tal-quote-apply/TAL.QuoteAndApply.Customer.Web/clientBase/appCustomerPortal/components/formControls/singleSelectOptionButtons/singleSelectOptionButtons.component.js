(function(module){
    'use strict';
    module.directive('talSingleSelectOptionButtons', function () {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/singleSelectOptionButtons/singleSelectOptionButtons.template.html',
            restrict: 'E',
            scope: {
                groupName: '@',
                options:'=',
                onOptionChanged:'&',
                columnLayout:'=',
                ngModel: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talSingleSelectOptionButtonsController',
            require: 'ngModel',
            link: function(scope, elem, attrs, ngModelCtrl) {

                var validateModelValue = function(modelValue) {
                    if (attrs.required && !modelValue) {
                        ngModelCtrl.$setValidity('required', false);
                    } else {
                        ngModelCtrl.$setValidity('required', true);
                    }
                };

                if (scope.ctrl.columnLayout === true) {

                    var totalColumns = 12;
                    var buttonsPerRow = 4;
                    var col = totalColumns / buttonsPerRow;

                    scope.columnClass = 'col-sm-' + col;

                    scope.ctrl.rows = scope.ctrl.groupOptionsInRows(scope.ctrl.options, buttonsPerRow);
                }

                ngModelCtrl.$formatters.push(function(modelValue){
                    /*
                     The ngModel value passed in needs to be used here as an attribute on an object,
                     otherwise two-way binding breaks if this directive is used inside an ng-transclude
                     */
                    scope.model = {value: modelValue};
                    validateModelValue(modelValue);

                    return modelValue;
                });

                ngModelCtrl.$parsers.push(function(modelValue) {
                    validateModelValue(modelValue);
                    return modelValue;
                });

                scope.setModelValue = function(val) {
                    scope.model.value = val;
                    ngModelCtrl.$setViewValue(scope.model.value);
                    scope.ctrl.onOptionChanged();
                };

            }
        };
    });

    function talSingleSelectOptionButtonsController() {

        this.groupOptionsInRows = function(options, itemsPerRow) {
            var rows = [];
            var lastRow = [];
            
            _.each(options, function (o, i) {
                var isNewRow = i !== 0 && i % itemsPerRow === 0;
                if (isNewRow) {
                    rows.push(lastRow);
                    lastRow = [];
                }

                lastRow.push(o);
            });

            //add the remaining items to rows
            rows.push(lastRow);
            return rows;
        };
    }

    module.controller('talSingleSelectOptionButtonsController', talSingleSelectOptionButtonsController );
    talSingleSelectOptionButtonsController.$inject = [];

})(angular.module('appCustomerPortal'));
