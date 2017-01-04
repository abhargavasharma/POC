'use strict';
angular.module('salesPortalApp').directive('talPremiumSummary', function () {
    return {
        templateUrl: '/client/app/components/policy/risks/premiumSummary/premiumSummary.template.html',
        restrict: 'E',
        scope: true,
        controllerAs: 'ctrl',
        bindToController: {
            quoteReferenceNumber: '=',
            risk: '='
        },
        controller: 'talPremiumSummaryController'
    };
});

angular.module('salesPortalApp').controller('talPremiumSummaryController',
    function(talSalesPortalApiService, talPolicySectionsNotificationService) {
        var ctrl = this;

        var onActivation = function(){
            ctrl.loadingPromise = talSalesPortalApiService.getRiskPremiumSummary(ctrl.quoteReferenceNumber, ctrl.risk.riskId)
                .then(function (response) {
                    ctrl.premiumSummary = response.data;
                });
        };

        var refreshPremiumSummary = function(premiumSummary){
            ctrl.premiumSummary = premiumSummary;
        };

        onActivation();
        talPolicySectionsNotificationService.registerPlansPremiumChangeEvent(refreshPremiumSummary);
    }
);

