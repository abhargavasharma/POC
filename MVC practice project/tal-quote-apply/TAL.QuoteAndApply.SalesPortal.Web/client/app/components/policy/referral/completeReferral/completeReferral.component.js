'use strict';
angular.module('salesPortalApp').directive('talCompleteReferral', function () {
    return {
        templateUrl: '/client/app/components/policy/referral/completeReferral/completeReferral.template.html',
        restrict: 'E',
        scope: true,
        controllerAs: 'ctrl',
        bindToController: {
            quoteReferenceNumber:'='
        },
        controller: 'talCompleteReferralController'
    };
});

angular.module('salesPortalApp').controller('talCompleteReferralController',
    function($uibModal, talSalesPortalApiService, $window) {
        var ctrl = this;

        function completeReferral(){
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/client/app/components/policy/referral/completeReferral/completeReferral.modal.html',
                controller: 'talCompleteReferralModalController',
                size: 'md'
            });

            modalInstance.result.then(function () {
                ctrl.loadingPromise = talSalesPortalApiService.completeReferral(ctrl.quoteReferenceNumber).then(function(){
                    $window.location.reload();
                }).catch(function (response) {
                    console.log(response);
                });
            });
        }

        ctrl.completeReferral = completeReferral;
    }
);

angular.module('salesPortalApp').controller('talCompleteReferralModalController',
    function($scope, $uibModalInstance) {
        $scope.ok = function() {
            $uibModalInstance.close();
        };
        $scope.cancel = function() {
            $uibModalInstance.dismiss();
        };
    }
);