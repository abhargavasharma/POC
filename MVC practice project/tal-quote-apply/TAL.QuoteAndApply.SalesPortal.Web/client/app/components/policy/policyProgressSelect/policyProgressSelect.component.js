
(function(module){
    'use strict';
    module.directive('talPolicyProgressSelect', function () {
        return {
            templateUrl: '/client/app/components/policy/policyProgressSelect/policyProgressSelect.template.html',
            restrict: 'E',
            scope: {
                quoteReferenceNumber: '='
            },
            controller: 'talPolicyProgressSelectController'
        };
    });

    function talPolicyProgressSelectController($scope, talSalesPortalApiService, FORM) {

        $scope.progresses = FORM.POLICY_PROGRESS.STATES;

        $scope.onActivation = function() {

            $scope.loadingPromise = talSalesPortalApiService.getPolicyProgress($scope.quoteReferenceNumber).then(function (response) {
                $scope.policyProgress = response.data;
            });
        };

        var submitForm = function () {

            $scope.loadingPromise = talSalesPortalApiService.updatePolicyProgress($scope.quoteReferenceNumber, $scope.policyProgress)
                .then(function (response) {
                    $scope.policyProgress = response.data;
                });
        };

        $scope.progressUnknown = function(){
            return $scope.policyProgress && $scope.policyProgress.progress === 'Unknown';
        };

        $scope.isActivated = $scope.onActivation();
        $scope.submitForm = submitForm;
    }

    module.controller('talPolicyProgressSelectController', talPolicyProgressSelectController );
    talPolicyProgressSelectController.$inject = ['$scope', 'talSalesPortalApiService', 'FORM'];

})(angular.module('salesPortalApp'));
