'use strict';
angular.module('salesPortalApp').directive('talReferToUnderwriter', function () {
    return {
        templateUrl: '/client/app/components/policy/referral/referToUnderwriter/referToUnderwriter.template.html',
        restrict: 'E',
        scope: true,
        controllerAs: 'ctrl',
        bindToController: {
            quoteReferenceNumber:'='
        },
        controller: 'talReferToUnderwriterController'
    };
});

angular.module('salesPortalApp').controller('talReferToUnderwriterController',
    function($uibModal, talSalesPortalApiService, $window) {
        var ctrl = this;

        function createReferral(){
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/client/app/components/policy/referral/referToUnderwriter/referToUnderwriter.modal.html',
                controller: 'talReferToUnderwriterModalController',
                size: 'md'
            });

            modalInstance.result.then(function () {
                ctrl.loadingPromise = talSalesPortalApiService.createReferral(ctrl.quoteReferenceNumber).then(function(){
                    $window.location.reload();
                }).catch(function (response) {
                    console.log(response);
                });
            });
        }

        ctrl.createReferral = createReferral;
    }
);

angular.module('salesPortalApp').controller('talReferToUnderwriterModalController',
    function($scope, $uibModalInstance) {
        $scope.ok = function() {
            $uibModalInstance.close();
        };
        $scope.cancel = function() {
            $uibModalInstance.dismiss();
        };
    }
);