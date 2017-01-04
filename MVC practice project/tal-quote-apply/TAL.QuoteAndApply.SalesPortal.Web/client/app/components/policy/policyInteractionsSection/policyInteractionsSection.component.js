(function(module) {
        'use strict';
        module.directive('talPolicyInteractions',
            function() {
                return {
                    templateUrl: '/client/app/components/policy/policyInteractionsSection/policyInteractionsSection.template.html',
                    restrict: 'E',
                    scope: {
                        risk: '=',
                        section:'=',
                        quoteReferenceNumber: '=',
                        readOnly: '='
                    },
                    controller: 'talInteractionsController'
                };
            });


        function talInteractionsController($scope, talSalesPortalApiService) {
            $scope.interactionsResults = {};
            var onActivation = function () {
                $scope.section.isCompleted = true;
                $scope.section.isModelValid = true;
                $scope.loadingPromise = talSalesPortalApiService.getInteractions($scope.quoteReferenceNumber)
                .then(function(response) {
                    $scope.interactionsResults = response.data;
                })
                .catch(function() {
                    $scope.interactionsResults = getEmptyInteractionsResults();
                });

            function getEmptyInteractionsResults() {
                return {
                    interactions: []
                    };
                }
            };

            $scope.section.onActivationEvent = onActivation;
    }

    module.controller('talInteractionsController', talInteractionsController);
    talInteractionsController.$inject = ['$scope', 'talSalesPortalApiService'];

})(angular.module('salesPortalApp'));