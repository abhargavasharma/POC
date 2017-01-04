(function (module) {
    'use strict';
    module.directive('talDirectDebit', function () {
        return {
            templateUrl: '/client/appPurchase/components/payments/directDebit/directDebit.template.html',
            restrict: 'E',
            scope: {
                directDebitPayment: '=',
                paymentType: '=',
                directDebitChanged:'='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talDirectDebitController'
        };
    });

    function talDirectDebitController() {
        var ctrl = this;

        ctrl.required = function() {
            return ctrl.paymentType === 'DirectDebit';
        };
    }

    module.controller('talDirectDebitController', talDirectDebitController);
    talDirectDebitController.$inject = [];

})(angular.module('appPurchase'));