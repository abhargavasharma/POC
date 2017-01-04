(function (module) {
    'use strict';
    module.directive('talPostcodeFormat', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            scope : {},
            link: function (scope, element, attrs, ngModelController) {

                //Copied from currency format directive and tweaked for postcode

                // Run formatting on keyup
                var numbersOnly = function (value) {
                    value = value.toString();
                    value = value.replace(/[^0-9]/g, '');

                    return value;
                };

                var applyFormatting = function () {
                    var value = element.val();
                    var original = value;
                    if (!value || value.length === 0) {
                        return;
                    }
                    value = numbersOnly(value);
                    if (value !== original) {
                        element.val(value);
                        element.triggerHandler('input');
                    }
                };

                element.bind('keyup', function (e) {
                    var keycode = e.keyCode;

                    var isTextInputKey =
                        (keycode > 47 && keycode < 58) || // number keys
                        keycode === 32 || keycode === 8 || // spacebar or backspace
                        (keycode > 64 && keycode < 91) || // letter keys
                        (keycode > 95 && keycode < 112) || // numpad keys
                        (keycode > 185 && keycode < 193) || // ;=,-./` (in order)
                        (keycode > 218 && keycode < 223);   // [\]' (in order)
                    if (isTextInputKey) {
                        applyFormatting();
                    }
                });


                ngModelController.$parsers.push(function (value) {
                    if (!value || value.length < 4) {
                        ngModelController.$setValidity('format', true); //true if we don't meet min requirements
                        return null;
                    }

                    //Basic postcode validation
                    var validLength = value.length === 4;
                    var numberIsValid = value && /^[0-9]+$/.test(value);
                    var validRange = parseFloat(value) > 199;

                    var validPostcode = numberIsValid && validLength && validRange;

                    ngModelController.$setValidity('format', validPostcode);

                    if (validPostcode) {
                        return value;
                    }

                    return null;

                });

                ngModelController.$formatters.push(function (value) {
                    if (!value || value.length === 0) {
                        return value;
                    }
                    value = numbersOnly(value);
                    return value;
                });
            }
        };
    }]);
})(angular.module('appCustomerPortal'));