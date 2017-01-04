
(function (module) {
    'use strict';
    module.directive('talSuperAnnuationPayment', function () {
        return {
            templateUrl: '/client/app/components/policy/payments/superAnnuation/superAnnuation.template.html',
            restrict: 'E',
            scope: {
                paymentModel: '=',
                submitPayment: '&',
                paymentsChanged: '&',
                readOnly: '='
            },
            controller: 'superAnnuationPaymentController'
        };
    });

    function superAnnuationPaymentController($scope, $uibModal, talSalesPortalApiService, $timeout) {
        $scope.fundSearch = [];
        
        $scope.validateForm = function () {
            $scope.submitPayment();
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
                $scope.paymentModel.taxFileNumber = '';
                $scope.paymentsChanged();
            });
        };

        var fundSearchTimeout;
        var lastSearch = '';
        $scope.fundNameKeydown = function() {
            if (fundSearchTimeout) {
                $timeout.cancel(fundSearchTimeout);
            }

            fundSearchTimeout = $timeout(function() {
                $scope.setPaymentsChanged();
                if ($scope.paymentModel.fundName && lastSearch !== $scope.paymentModel.fundName) {

                    lastSearch = $scope.paymentModel.fundName;

                    talSalesPortalApiService.searchSuperannuationFund($scope.paymentModel.fundName)
                        .then(function(response) {
                            $scope.fundSearch = response.data;
                            
                        })
                        .catch(function (response) {
                            $scope.fundSearch = [];

                            //bad request - most likely because of unsupported characters.
                            //actual problems would be 500.
                            if (response.status === 400) {
                                return;
                            }
                            $scope.superFundPayment.fundName.$error.server = 'The Superfund Search Feature is currently offline. Please try again later';
                        });
                }
            },
            500);
        };

        $scope.selectFundName = function(fund) {
            $scope.paymentModel.fundProduct = fund.fundProduct;
            $scope.paymentModel.fundName = fund.fundName;
            $scope.paymentModel.fundUSI = fund.fundUSI;
            $scope.paymentModel.fundABN = fund.fundABN;

            $scope.setPaymentsChanged();

            $scope.fundSearch = [];
        };

        $scope.setPaymentsChanged = function(){
            $scope.paymentsChanged();
        };

        $scope.policyIsReadonlyOrPaymentDetailsReadOnly = function() {
            return $scope.readOnly || ($scope.paymentModel && $scope.paymentModel.isValidForInforce);
        };
    }

    module.controller('superAnnuationPaymentController', superAnnuationPaymentController);
    superAnnuationPaymentController.$inject = ['$scope', '$uibModal', 'talSalesPortalApiService', '$timeout'];

})(angular.module('salesPortalApp'));

