'use strict';

angular.module('appCustomerPortal').directive('talServerValidate', function (talFormModelStateService) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function(scope, elem, attrs, ngModel) {
            var modelStateKey = attrs.talServerValidate || attrs.ngModel;
            talFormModelStateService.registerNgModel(modelStateKey, ngModel);

            //Register a parser and formatters that will clear any server validation error if user changes value
            ngModel.$parsers.unshift(function(value) {
                ngModel.$setValidity('server', true);
                return value;
            });

            ngModel.$formatters.unshift(function(value) {
                ngModel.$setValidity('server', true);
                return value;
            });
        }
    };
});
