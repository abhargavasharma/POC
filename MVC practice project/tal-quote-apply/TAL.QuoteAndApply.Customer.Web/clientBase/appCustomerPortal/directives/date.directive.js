'use strict';

angular.module('appCustomerPortal').directive('talDate', function () {
    return {
        restrict: 'A',
        scope:{
        },
        require: 'ngModel',
        link: function(scope, elem, attrs, ngModelCtrl) {

            /*
             The ngModel value passed in needs to be used here as an attribute on an object,
             otherwise two-way binding breaks if this directive is used inside an ng-transclude
             */
            scope.model = {date: undefined};

            var dobDisplayFormat = 'DD/MM/YYYY';
            var dobUiMaskFormat = attrs.talDateMaskFormat || 'DD/MM/YYYY';


            ngModelCtrl.$formatters.push(function(modelValue){

                var momentVal = moment(modelValue, dobDisplayFormat);

                if (!momentVal.isValid()) {
                    return modelValue;
                }

                scope.model.date = momentVal.format(dobDisplayFormat);
                return scope.model.date;
            });

            ngModelCtrl.$parsers.push(function(modelValue) {
                var momentVal = moment(modelValue, dobUiMaskFormat, true); //true = Force strict date checking

                if (momentVal.isValid()) {
                    ngModelCtrl.$setValidity('invalidFormat', true);
                    return momentVal.format(dobDisplayFormat);
                }

                ngModelCtrl.$setValidity('invalidFormat', false);
                return modelValue;
            });

            scope.setModelValue = function(val) {
                scope.model.value = val;
                ngModelCtrl.$setViewValue(scope.model.value);
            };

        }
    };
});
