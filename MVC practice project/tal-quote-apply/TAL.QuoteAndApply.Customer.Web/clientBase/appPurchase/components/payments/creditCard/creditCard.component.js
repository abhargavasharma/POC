(function (module) {
    'use strict';
    module.directive('talCreditCard', function () {
        return {
            templateUrl: '/client/appPurchase/components/payments/creditCard/creditCard.template.html',
            restrict: 'E',
            scope: {
                expiryDateDefault: '=',
                creditCardPayment: '=',
                creditCardChanged: '=',
                paymentType: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talCreditCardController'
        };
    });

    function talCreditCardController(talCustomerPortalApiService) {
        var ctrl = this;

        function refreshMonths() {
            for (var j = 0; j < 12; j++) {
                if (ctrl.months[j].disabled) {
                    ctrl.months[j].disabled = false;
                }
            }
        }

        ctrl.required = function() {
            return ctrl.paymentType === 'CreditCard';
        };

        ctrl.showServerError = function () {
            return ctrl.creditCardPayment && ctrl.creditCardPayment.error;
        };

        ctrl.cardTypeChange = function () {
            ctrl.creditCardChanged();
        };

        ctrl.setExpiryMonth = function() {
            if (ctrl.selectedExpiryMonthObj) {
                ctrl.creditCardPayment.expiryMonth = ctrl.selectedExpiryMonthObj.value;
            }
        };

        ctrl.changeYear = function () {

            var d = new Date();

            var year = d.getFullYear().toString();
            var curYearVal = year.substring(2, 4);
            var month = d.getMonth() + 1;

            if (curYearVal === ctrl.creditCardPayment.expiryYear) {
                //hide expiry month if it is past.
                for (var n = 0; n < 12; n++) {
                    if (month > ctrl.months[n].value) {
                        ctrl.months[n].disabled = true;
                    } else {
                        break;
                    }
                }

                if (month > ctrl.creditCardPayment.expiryMonth) {
                    ctrl.creditCardPayment.expiryMonth = null;
                }
            } else {
                refreshMonths();
            }
        };

        function getYears() {
            var years = [];
            var d = new Date();

            for (var i = 0; i < 16; i++) {
                var year = (d.getFullYear() + i).toString();
                var yearVal = year.substring(2, 4);
                years.push({ 'value': yearVal, 'text': year });
            }
            return years;
        }

        function getMonths() {
            var months = [];

            for (var n = 1; n <= 12; n++) {
                var text = (n < 10) ? '0' + n.toString() : n.toString();
                months.push({ 'value': n.toString(), 'text': text, 'disabled': false });
            }
            return months;
        }

        //init data
        ctrl.cardtypes = [];
        talCustomerPortalApiService.getAvailableCreditCardTypes()
            .then(function (response) {
                ctrl.cardtypes = response.data;
            });


        ctrl.months = getMonths();
        ctrl.years = getYears();
    }

    module.controller('talCreditCardController', talCreditCardController);
    talCreditCardController.$inject = ['talCustomerPortalApiService'];

})(angular.module('appPurchase'));