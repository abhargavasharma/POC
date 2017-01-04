(function(module) {
  'use strict';
  module.directive('talMobileQuote', function () {
    return {
      templateUrl: '/client/appCustomerPortal/largeComponents/mobileQuote/mobileQuote.template.html',
      restrict: 'E',
        scope: {
            hideQuotePremium: '=',
            hideQuoteReference: '=',
            hideSaveButton: '=',
            quoteReferenceNumber: '=',
            discount: '=',
            totalPremium: '=',
            premiumPeriod: '=',
            savedStatus: '='
        },
      replace: true,
      controller: 'mobileQuoteController as ctrl',
      bindToController: true
    };
  });

    var mobileQuoteController = function(talCustomerPremiumService) {
        var ctrl = this;
        // @TODO: Temporary showcase defaults
        ctrl.quoteReferenceNumber = ctrl.quoteReferenceNumber || 'M1234567';
        ctrl.discount = ctrl.discount || 0;
        ctrl.totalPremium = ctrl.totalPremium || 0;
        ctrl.premiumPeriod = ctrl.premiumPeriod || 'monthly';
        // END temp defaults

        talCustomerPremiumService.registerPremiumUpdated(function(updatePremiumObj) {
            //update scope variables here
            ctrl.totalPremium = updatePremiumObj.premium;
            ctrl.discount = updatePremiumObj.discount;
            ctrl.premiumPeriod = updatePremiumObj.paymentFrequency;
        });
  };

  module.controller('mobileQuoteController', mobileQuoteController);
  mobileQuoteController.$inject = ['talCustomerPremiumService'];

})(angular.module('appCustomerPortal'));