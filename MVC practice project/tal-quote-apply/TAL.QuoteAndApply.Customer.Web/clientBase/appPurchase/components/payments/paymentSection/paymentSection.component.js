(function (module) {
    'use strict';
    module.directive('talPaymentSection', function () {
        return {
            templateUrl: '/client/appPurchase/components/payments/paymentSection/paymentSection.template.html',
            restrict: 'E',
            scope: {
                paymentType: '=',
                expiryDateDefault:'=',
                paymentOptions: '=',
                paymentMessage: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talPaymentSectionController'
        };
    });

    function talPaymentSectionController(talCustomerPortalApiService) {
        var ctrl = this;

        ctrl.selectTab = function(paymentType) {
            ctrl.paymentType = paymentType;
        };

        ctrl.isCreditCardPaymentAvailable = false;
        ctrl.isDirectDebitPaymentAvailable = false;
        ctrl.isSuperannuationPaymentAvailable = false;

        ctrl.availablePaymentsLoaded = false;
        
        talCustomerPortalApiService.getAvailablePaymentOptionsForProduct()
            .then(function (response) {
                ctrl.isCreditCardPaymentAvailable = response.data.isCreditCardAvailable;
                ctrl.isDirectDebitPaymentAvailable = response.data.isDirectDebitAvailable;
                ctrl.isSuperannuationPaymentAvailable = response.data.isSuperAvailable;

                if (ctrl.paymentType === 'DirectDebit' && !ctrl.isDirectDebitPaymentAvailable) {
                    ctrl.paymentType = 'CreditCard';
                }

                if (ctrl.paymentType === 'CreditCard' && !ctrl.isCreditCardPaymentAvailable) {
                    ctrl.paymentType = 'DirectDebit';
                }

                ctrl.availablePaymentsLoaded = true;
            });
    }

    module.controller('talPaymentSectionController', talPaymentSectionController);
    talPaymentSectionController.$inject = ['talCustomerPortalApiService'];

})(angular.module('appPurchase'));