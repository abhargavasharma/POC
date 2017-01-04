(function(module) {
  'use strict';
  module.directive('talMainQuote', function () {
    return {
      templateUrl: '/client/appCustomerPortal/largeComponents/mainQuote/mainQuote.template.html',
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
      controller: 'mainQuoteController as ctrl',
      bindToController: true
    };
  });


    var mainQuoteController = function($scope, talCustomerPremiumService, EVENT, $log) {

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

        $scope.$on(EVENT.SAVE.TRIGGER_FORCED_SAVE, function ($event, eventData) {
            $log.debug('appSave - save:trigger-forced-save. callBack: ' + JSON.stringify(eventData) + ' : footerShow');

            ctrl.forceShowFooter = true;
        });

        $scope.$on(EVENT.SAVE.HIDE_MOBILE_FOOTER, function () {
            ctrl.forceShowFooter = false;
        });
  };

  module.controller('mainQuoteController', mainQuoteController);
  mainQuoteController.$inject = ['$scope','talCustomerPremiumService', 'EVENT', '$log'];

})(angular.module('appCustomerPortal'));