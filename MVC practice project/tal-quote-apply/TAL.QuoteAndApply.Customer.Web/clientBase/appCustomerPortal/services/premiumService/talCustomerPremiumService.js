'use strict';

angular.module('appCustomerPortal').service('talCustomerPremiumService', function () {

    var premiumUpdatedCallbackFunctions = [];

    this.registerPremiumUpdated = function(callback) {
        premiumUpdatedCallbackFunctions.push(callback);
    };

    this.triggerPremiumUpdated = function(premium, paymentFrequency, discount) {
        //TODO: pass through discount from calling methods when hooked up to discount
        var premiumModel = {
            premium: premium,
            paymentFrequency: paymentFrequency,
            discount: discount
        };

        _.forEach(premiumUpdatedCallbackFunctions, function(callback){
            callback(premiumModel);
        });
    };

});