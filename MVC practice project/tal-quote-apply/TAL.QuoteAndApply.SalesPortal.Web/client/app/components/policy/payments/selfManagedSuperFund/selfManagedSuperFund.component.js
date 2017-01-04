(function(module){
    'use strict';
    module.directive('talSelfManagedSuperFund', function () {
        return {
            templateUrl: '/client/app/components/policy/payments/selfManagedSuperFund/selfManagedSuperFund.template.html',
           // templateUrl: '/client/app/components/policy/payments/directDebit/directDebit.template.html',
            restrict: 'E',
            scope: {
                paymentModel: '=',
                submitPayment: '&',
                paymentsChanged: '&',
                readOnly: '='
            },
            controller: 'talSelfManagedSuperFundController'
        };
    });

    function talSelfManagedSuperFundController($scope, $uibModal) {

        $scope.validateForm = function(){
            $scope.submitPayment();
        };

        $scope.setPaymentsChanged = function(){
            $scope.paymentsChanged();
        };

        $scope.editForm = function() {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/client/app/components/policy/payments/modal/confirmEditPaymentDetails.modal.html',
                controller: 'confirmEditPaymentDetailsController',
                size: 'md'
            });

            modalInstance.result.then(function() {
                $scope.paymentModel.isValidForInforce = false;
                $scope.paymentsChanged();
            });
        };

        $scope.policyIsReadonlyOrPaymentDetailsReadOnly = function() {
            return $scope.readOnly || ($scope.paymentModel && $scope.paymentModel.isValidForInforce);
        };
    }

    module.controller('talSelfManagedSuperFundController', talSelfManagedSuperFundController);
    talSelfManagedSuperFundController.$inject = ['$scope', '$uibModal'];

})(angular.module('salesPortalApp'));

