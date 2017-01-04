'use strict';

angular.module('appCustomerPortal').directive('talQuoteReferenceFormatValidate', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function(scope, elem, attrs, ngModel) {

            var testAndSetValidity = function(value) {
                var formatRegex = /(m|M)\d{9}/;
                var isValidQuoteReferenceFormat = !value || formatRegex.test(value);
                ngModel.$setValidity('quoteReferenceFormat', isValidQuoteReferenceFormat);
            };

            ngModel.$parsers.push(function(value) {
                testAndSetValidity(value);
                return value;
            });
        }
    };
});
