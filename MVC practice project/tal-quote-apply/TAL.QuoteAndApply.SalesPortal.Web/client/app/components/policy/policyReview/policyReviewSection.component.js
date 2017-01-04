
(function (module) {
    'use strict';
    module.directive('talPolicyReviewSection', function () {
        return {
            templateUrl: '/client/app/components/policy/policyReview/policyReviewSection.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk: '=',
                quoteReferenceNumber: '=',
                status: '=',
                validateSectionsAndSubmit: '&',
                inforceAllowed: '&'
            },
            controller: 'talPolicyReviewSectionController'
        };
    });

    function talPolicyReviewSectionController($scope, talPolicySectionsNotificationService, talSalesPortalApiService, FORM) {

        $scope.partyConsentsModel = FORM.CONSENTS.MODEL;
        $scope.partyConsentsSubmitModel = FORM.CONSENTS.SUBMIT_MODEL;
        $scope.partyConsents = [];

        $scope.submit = function(){

            _.each($scope.partyConsentsSubmitModel, function(model){
                model.selected = $scope.partyConsentsModel[model.name];
            });

            $scope.submitModel.consents = _.map(_.filter($scope.partyConsentsSubmitModel, function(n) {
                return n.selected;
            }), 'name');

            $scope.submitModel.expressConsent = $scope.partyConsentsModel.expressConsent;

            talSalesPortalApiService.updatePartyConsent($scope.quoteReferenceNumber, $scope.risk.riskId, $scope.submitModel)
                .then(function () {

                });
        };

        var onActivation = function () {
            $scope.section.isModelValid = true;
            $scope.section.isCompleted = true;

            $scope.disabledForInforce = $scope.status === 'RaisedToPolicyAdminSystem';

            talSalesPortalApiService.getPartyConsent($scope.quoteReferenceNumber, $scope.risk.riskId)
                .then(function (response) {
                    $scope.submitModel = response.data;
                    $scope.partyConsentsModel = FORM.CONSENTS.MODEL;
                    _.each(response.data.consents, function(consent){
                        $scope.partyConsentsModel[consent] = true;
                    });
                });
        };

        talPolicySectionsNotificationService.registerInitialisationChangeEvent(function() {
            onActivation();
        });

        $scope.section.onActivationEvent = onActivation;
    }

    module.controller('talPolicyReviewSectionController', talPolicyReviewSectionController);
    talPolicyReviewSectionController.$inject = ['$scope', 'talPolicySectionsNotificationService', 'talSalesPortalApiService', 'FORM'];

})(angular.module('salesPortalApp'));

