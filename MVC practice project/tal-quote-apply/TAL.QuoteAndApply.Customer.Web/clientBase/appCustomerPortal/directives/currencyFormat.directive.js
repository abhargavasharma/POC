(function (module) {
    'use strict';
    module.directive('talCurrencyFormat', [function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            scope : {
                wholeNumbersOnly : '='
            },
            link: function (scope, element, attrs, ngModelController) {

                //TOTALLY RIPPED THIS FROM SALES PORTAL, WHICH WAS RIPPED FROM CUE, WHICH WAS RIPPED FROM COVERBUILDER, WHICH WAS RIPPED FROM SOMEWHERE ELSE.

                // Run formatting on keyup
                var numberWithCommas = function (value, addExtraZero) {
                    if (addExtraZero === undefined) {
                        addExtraZero = false;
                    }
                    value = value.toString();
                    if (!scope.wholeNumbersOnly) {
                        value = value.replace(/[^0-9\.]/g, '');
                    } else {
                        value = value.replace(/[^0-9]/g, '');
                    }
                    var parts = value.split('.');
                    parts[0] = parts[0].replace(/\d{1,3}(?=(\d{3})+(?!\d))/g, '$&,');
                    if (parts[1] && parts[1].length > 2) {
                        parts[1] = parts[1].substring(0, 2);
                    }
                    if (addExtraZero && parts[1] && (parts[1].length === 1)) {
                        parts[1] = parts[1] + '0';
                    }

                    var retVal = parts[0];
                    if (parts.length > 1) {
                        retVal += '.' + parts[1];
                    }
                    return retVal;
                };
                var applyFormatting = function () {
                    var value = element.val();
                    var original = value;
                    if (!value || value.length === 0) {
                        return;
                    }
                    value = numberWithCommas(value);
                    if (value !== original) {
                        element.val(value);
                        element.triggerHandler('input');
                    }
                };

                var keyPress = {};

                element.bind('keydown', function (e) {
                    if (keyPress[e.which]) {
                        e.preventDefault();
                    } else {
                        keyPress[e.which] = true;
                    }
                });
                element.bind('keyup', function (e) {
                    var keycode = e.keyCode;
                    keyPress[e.which] = false;

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
                    if (!value || value.length === 0) {
                        return 0;
                    }

                    var numberIsValid = value && /^[0-9.,]+$/.test(value);
                    ngModelController.$setValidity('format', numberIsValid);

                    value = value.toString();
                    if (!scope.wholeNumbersOnly) {
                        value = value.replace(/[^0-9\.]/g, '');
                    } else {
                        value = value.replace(/[^0-9]/g, '');
                    }
                    return parseFloat(value) || 0;
                });

                ngModelController.$formatters.push(function (value) {
                    if (!value || value.length === 0) {
                        return value;
                    }
                    value = numberWithCommas(value, true);
                    return value;
                });
            }
        };
    }]);
})(angular.module('appCustomerPortal'));