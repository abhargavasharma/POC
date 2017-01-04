'use strict';

angular.module('salesPortalApp').directive('talServerValidate', function (talFormModelStateService) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function(scope, elem, attrs, ngModel) {
            var modelStateKey = attrs.talServerValidate || attrs.ngModel;
            talFormModelStateService.registerNgModel(modelStateKey, ngModel);
            ngModel.requiresEmitUiUpdate = attrs.emitUiUpdate;

            //Register a parser and formatters that will clear any server validation error if user changes value
            ngModel.$parsers.unshift(function(value) {
                ngModel.$setValidity('server', true);
                return value;
            });

            ngModel.$formatters.unshift(function(value) {
                ngModel.$setValidity('server', true);

                if (value && ngModel.requiresEmitUiUpdate) {
                    talFormModelStateService.triggerUiUpdate(scope);
                }
                return value;
            });
        }
    };
});
