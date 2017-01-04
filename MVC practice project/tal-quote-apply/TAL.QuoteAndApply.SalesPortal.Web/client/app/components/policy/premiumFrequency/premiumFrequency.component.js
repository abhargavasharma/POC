(function (module) {
    'use strict';
    module.directive('talPremiumFrequency', function () {
        return {
            templateUrl: '/client/app/components/policy/premiumFrequency/premiumFrequency.template.html',
            restrict: 'E',
            scope: {
                quoteReferenceNumber: '=',
                updatingPromise: '=',
                sectionInError: '=',
                readOnly: '='
            },
            controller: 'talPremiumFrequencyController'
        };
    });

    function talPremiumFrequencyController($scope, FORM, talSalesPortalApiService, talFormModelStateService, talPolicySectionsNotificationService) {
        $scope.premiumFrequencies = [];

        $scope.loadingPromise = talSalesPortalApiService.getPolicyPremiumFrequency($scope.quoteReferenceNumber)
            .then(function(response){
                $scope.premiumFrequencies = response.data.availableFrequencies;
                $scope.premiumFrequency = response.data.premiumFrequency;
            });

        $scope.premiumFrequencyChanged = function() {
            $scope.updatingPromise = talSalesPortalApiService.updatePolicyPremiumFrequency($scope.quoteReferenceNumber, {premiumFrequency: $scope.premiumFrequency})
                .then(function(response){
                    $scope.premiumFrequency = response.data.premiumFrequency;
                    if(!$scope.sectionInError) {
                        talFormModelStateService.updateCleanModelState($scope);
                        talPolicySectionsNotificationService.onPremiumFrequencyChange($scope.premiumFrequency);
                    }
                })
                .catch(function(response){
                    talFormModelStateService.updateModelState(response.data, $scope);
                });
        };
    }

    module.controller('talPremiumFrequencyController', talPremiumFrequencyController);
    talPremiumFrequencyController.$inject = ['$scope', 'FORM', 'talSalesPortalApiService', 'talFormModelStateService', 'talPolicySectionsNotificationService'];

})(angular.module('salesPortalApp'));