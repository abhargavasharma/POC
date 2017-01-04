
(function(module){
    'use strict';
    module.directive('talRiskRatingFactorsSection', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskRatingFactorsSection/riskRatingFactorsSection.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk: '=',
                quoteReferenceNumber: '=',
                readOnly: '='
            },
            controller: 'talRiskRatingFactorsSectionController'
        };
    });

    function talRiskRatingFactorsSectionController($scope, talSalesPortalApiService, talFormModelStateService,  talPolicySectionsNotificationService, talPolicyEditSectionsService) {
        $scope.isDisabled = false;

        var onActivation = function() {
            $scope.loadingPromise = talSalesPortalApiService.getRiskRatingFactors($scope.quoteReferenceNumber, $scope.risk.riskId).then(function (response) {
                $scope.ratingFactors = response.data.ratingFactors;
                $scope.section.isCompleted = response.data.isCompleted;
                $scope.section.isModelValid = true;
                $scope.$broadcast('form-data-loaded', $scope.ratingFactors);
            });
        };

        var submitForm = function(){
            $scope.ratingFactors.warnings = [];
            $scope.isDisabled = true;
            $scope.updatingPromise = talSalesPortalApiService.updateRiskRatingFactors($scope.quoteReferenceNumber, $scope.risk.riskId, $scope.ratingFactors)
                .then(function(response) {
                    $scope.isDisabled = false;
                    talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section);

                    $scope.ratingFactors = response.data.ratingFactors;
                    $scope.section.isCompleted = response.data.isCompleted;
                    $scope.section.isModelValid = true;

                    talPolicySectionsNotificationService.onInsurancePlansPremiumChange(response.data.riskPremiumSummary);
                    talPolicySectionsNotificationService.onPlanStatusChangeEvent(response.data.policyRiskPlanStatusesResult);
                    talPolicySectionsNotificationService.onUnderwritingStatusChangeEvent(response.data.underwritingStatus);
                })
                .catch(function(response){
                    $scope.isDisabled = false;
                    talPolicyEditSectionsService.setSectionStatesOnError($scope.section);
                    var modelState = response.data;
                    talFormModelStateService.updateModelState(modelState, $scope);
                    $scope.section.isCompleted = false;
                    $scope.section.isModelValid = false;
                    throw response;
                });
            $scope.loadingPromise = $scope.updatingPromise; //Also set loading promise so screen overlay shows
            return $scope.updatingPromise;
        };

        $scope.section.onActivationEvent = onActivation;
        $scope.submitForm = submitForm;

        talPolicySectionsNotificationService.registerInitialisationChangeEvent(function () {
            onActivation();
        });
    }

    module.controller('talRiskRatingFactorsSectionController', talRiskRatingFactorsSectionController );
    talRiskRatingFactorsSectionController.$inject = ['$scope', 'talSalesPortalApiService', 'talFormModelStateService', 'talPolicySectionsNotificationService', 'talPolicyEditSectionsService', 'SECTION'];

})(angular.module('salesPortalApp'));