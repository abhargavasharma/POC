(function (module) {
    'use strict';

    module.constant('PAYMENT', {
        TYPE: {
            CREDIT_CARD: 'creditCard',
            DIRECT_DEBIT: 'directDebit',
            SUPERANNUATION: 'superannuation',
            SELFMANAGEDSUPERFUND: 'selfManagedSuperFund'
        }
    });

})(angular.module('salesPortalApp'));

