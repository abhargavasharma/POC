(function (module) {
    'use strict';
    module.directive('talCreditCard', function () {
        return {
            templateUrl: '/client/app/components/policy/payments/creditCard/creditCard.template.html',
            restrict: 'E',
            scope: {
                paymentModel: '=',
                submitPayment: '&',
                expiryDateDefault: '=',
                years: '=',
                months: '=',
                paymentsChanged: '&',
                serverResponse:'=',
                readOnly: '=',
                quoteReferenceNumber: '='
            },
            controller: 'talCreditCardController'
        };
    });

    function talCreditCardController($scope, $uibModal, talSalesPortalApiService) {
        $scope.anAttribute = 'creditCard';

        $scope.validateForm = function () {
            $scope.submitPayment();
        };

        $scope.hasPaymentServerError = function () {
            var r = $scope.serverResponse;

            if (!r) {
                return false;
            }
            if (r.status === 500 && r.statusText === 'CreditCardPayment') {
                return true;
            }
            return false;
        };

        $scope.editForm = function () {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/client/app/components/policy/payments/modal/confirmEditPaymentDetails.modal.html',
                controller: 'confirmEditPaymentDetailsController',
                size: 'md'
            });

            modalInstance.result.then(function () {
                $scope.paymentModel.isValidForInforce = false;
                $scope.paymentModel.cardNumber = null;
                $scope.expiryDateDefault.month = $scope.expiryDateDefault.getMonthDefault();
                $scope.expiryDateDefault.year = $scope.expiryDateDefault.getYearDefault();
                $scope.paymentModel.token = null;
                refreshMonths();
                $scope.paymentsChanged();
            });
        };

        function refreshMonths() {
            for (var j = 0; j < 12; j++) {
                if ($scope.months[j].displayOption === false) {
                    $scope.months[j].displayOption = true;
                }
            }
        }

        $scope.changeYear = function() {

            $scope.setPaymentsChanged();

            var d = new Date();

            var year = d.getFullYear().toString();
            var curYearVal = year.substring(2, 4);
            var month = d.getMonth() + 1;

            if (curYearVal === $scope.paymentModel.expiryYear) {
                //hide expiry month if it is past.
                for (var n = 0; n < 12; n++) {
                    if (month > $scope.months[n].value) {
                        $scope.months[n].displayOption = false;                        
                    } else {
                        break;
                    }
                }

                if (month > $scope.paymentModel.expiryMonth) {
                    $scope.paymentModel.expiryMonth = null;
                }
            } else {
                refreshMonths();
            }
            $scope.setPaymentsChanged();
        };

        function getYears() {
            var years = [];
            var d = new Date();

            for (var i = 0; i < 16; i++) {
                var year = (d.getFullYear() + i).toString();
                var yearVal = year.substring(2, 4);
                years.push({ 'value': yearVal, 'text': year });
            }
            return years;
        }

        function getMonths() {
            var months = [];

            for (var n = 1; n <= 12; n++) {
                var text = (n < 10) ? '0' + n.toString() : n.toString();
                months.push({ 'value': n.toString(), 'text': text, 'displayOption': true });
            }
            return months;
        }
        
        $scope.months = getMonths();
        $scope.years = getYears();

        //init data
        $scope.cardtypes = [];
        talSalesPortalApiService.getAvailableCreditCardTypes($scope.quoteReferenceNumber)
            .then(function (response) {
                $scope.cardtypes = response.data;
            });

        $scope.setPaymentsChanged = function () {
            $scope.paymentsChanged();
        };

        $scope.policyIsReadonlyOrPaymentDetailsReadOnly = function () {
            return $scope.readOnly || ($scope.paymentModel && $scope.paymentModel.isValidForInforce);
        };
    }

    module.controller('talCreditCardController', talCreditCardController);
    talCreditCardController.$inject = ['$scope', '$uibModal', 'talSalesPortalApiService'];

})(angular.module('salesPortalApp'));