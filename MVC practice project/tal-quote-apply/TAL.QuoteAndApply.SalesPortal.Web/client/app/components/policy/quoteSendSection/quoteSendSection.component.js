(function(module) {
        'use strict';
        module.directive('talQuoteSend',
            function() {
                return {
                    templateUrl: '/client/app/components/policy/quoteSendSection/quoteSendSection.template.html',
                    restrict: 'E',
                    scope: {
                        risk: '=',
                        section:'=',
                        quoteReferenceNumber: '=',
                        readOnly: '='
                    },
                    controller: 'talQuoteSendController'
                };
            });


        function talQuoteSendController($scope, talSalesPortalApiService) {

            $scope.model = {};
            $scope.emailSuccess = true;
            $scope.emailSent = false;

            $scope.sendQuote = function(){
                $scope.loadingPromise = talSalesPortalApiService.sendCorrespondence($scope.quoteReferenceNumber, $scope.risk.riskId)
                    .then(function (response) {
                        $scope.emailSent = true;
                        $scope.emailSuccess = response.data;
                    })
                    .catch(function () {
                        $scope.emailSuccess = false;
                    });
            };

            $scope.sendQuoteResults = {};

            var onActivation = function () {
                $scope.section.isCompleted = true;
                $scope.section.isModelValid = true;
                $scope.loadingPromise = talSalesPortalApiService.getCorrespondenceSummary($scope.quoteReferenceNumber, $scope.risk.riskId)
                    .then(function (response) {
                        $scope.model = response.data;
                        $scope.model.plans = [];
                        $scope.model.totalPremium = 0;
                        _.each(response.data.planSummaries, function(planSummary){
                            $scope.model.plans.push({name: planSummary.isRider ? planSummary.name.toUpperCase() + ' (Bundled with ' + planSummary.parentName.toUpperCase() + ')' : planSummary.name.toUpperCase(),
                                coverAmount: planSummary.coverAmount, premium: planSummary.premium, type: 'plan', checked: false});
                            $scope.model.totalPremium += planSummary.premium;
                            _.each(planSummary.coverCorrespondenceSummaries, function(coverSummary){
                                $scope.model.plans.push({name: coverSummary.name, coverAmount: 0, premium: 0.0, type: 'cover', checked: coverSummary.selected});
                            });
                        });
                    })
                    .catch(function () {

                    });
            };

            $scope.section.onActivationEvent = onActivation;
    }

    module.controller('talQuoteSendController', talQuoteSendController);
    talQuoteSendController.$inject = ['$scope', 'talSalesPortalApiService'];

})(angular.module('salesPortalApp'));