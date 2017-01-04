
(function (module) {
    'use strict';
    module.directive('talRiskPaymentSection', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskPaymentSection/riskPaymentSection.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk: '=',
                quoteReferenceNumber: '=',
                availability: '=',
                readOnly: '='
            },
            controller: 'talRiskPaymentSectionController'
        };
    });


    function talRiskPaymentSectionController($scope, talPolicySectionsNotificationService, talSalesPortalApiService, talFormModelStateService, talPolicyEditSectionsService, PAYMENT, $uibModal) {
        $scope.section.isModelValid = true;
        $scope.paymentChanged = false;
        $scope.emptyPaymentsSet = false;
        $scope.serverErrorResponse = null;
        $scope.modalOpen = false;

        var formSubmitting = false;
        var cvcTokenisation = '***';
        var dateTokenisation = '**';

        //defaults for tal
        $scope.isCreditCardPaymentAvailable = true;
        $scope.isSuperannuationPaymentAvailable = false;
        $scope.isDirectDebitPaymentAvailable = true;
        $scope.isSelfManagementSuperFundPaymentAvailable = false;

        var onActivation = function(){
            $scope.onActivation();
        };        

        function getMonthDefault() {
             return 'mm';
        }

        function getYearDefault() {
            return 'yy';
        }

        //after submitted cc payment, tokenise expiry date
        function tokeniseExpiryDate() {
            $scope.paymentsReturnObj.creditCard.expiryMonth = null;
            $scope.paymentsReturnObj.creditCard.expiryYear = null;
            $scope.paymentsReturnObj.creditCard.cvc = cvcTokenisation;
            $scope.expiryDateDefault.month = dateTokenisation;
            $scope.expiryDateDefault.year = dateTokenisation;
        }

        function resetExpiryDate() {
            return {
                'month': getMonthDefault(),
                'year': getYearDefault(),
                'getMonthDefault': getMonthDefault,
                'getYearDefault': getYearDefault
            };
        }

        function updateUiOnSuccess(response) {
            $scope.section.isModelValid = true;
            talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section, response.data.warnings);
            $scope.paymentsReturnObj = {
                creditCard: response.data.creditCardPayment,
                directDebit: response.data.directDebitPayment,
                superannuation: response.data.superannuationPayment,
                selfManagedSuperFund: response.data.selfManagedSuperFundPayment
            };

            if (response.data.isComplete && response.data.creditCardPayment.token && !formSubmitting && !$scope.serverErrorResponse) {
                tokeniseExpiryDate();
            } else if ($scope.expiryDateDefault.month === dateTokenisation) {
                $scope.expiryDateDefault = resetExpiryDate();
            }

            $scope.paymentsReturnObj.creditCard.type = PAYMENT.TYPE.CREDIT_CARD;
            $scope.paymentsReturnObj.directDebit.type = PAYMENT.TYPE.DIRECT_DEBIT;
            $scope.paymentsReturnObj.superannuation.type = PAYMENT.TYPE.SUPERANNUATION;
            $scope.paymentsReturnObj.selfManagedSuperFund.type = PAYMENT.TYPE.SELFMANAGEDSUPERFUND;
            $scope.section.isCompleted = response.data.isComplete;
            $scope.paymentChanged = false;
        }

        $scope.openOverrideCurrentPaymentWarning = function(paymentSubmitted){
            
            //if any other payment already stored
            if($scope.currentPaymentStatus.isComplete) {
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: '/client/app/components/policy/risks/riskPaymentSection/modal/overrideCurrentPaymentWarning.modal.html',
                    controller: 'paymentModalController',
                    size: 'md'
                });

                modalInstance.result.then(function () {
                    $scope.activePayment = $scope.paymentsReturnObj[paymentSubmitted.type];
                    $scope.activePayment.type = paymentSubmitted.type;
                    talFormModelStateService.updateCleanModelState($scope);
                    $scope.submitPayment(paymentSubmitted);
                });
            }
            //When no payments are currently against the policy
            else { $scope.submitPayment(paymentSubmitted);}
        };

        $scope.submitPaymentOnChange = function (paymentSubmitted) {
            //only set for credit card payment section to control reload ui
            if (paymentSubmitted.type === PAYMENT.TYPE.CREDIT_CARD) {
                $scope.modalOpenAndStopRefresh = true;
            }
            $scope.openOverrideCurrentPaymentWarning(paymentSubmitted);
        };

        $scope.submitPayment = function (paymentSubmitted) {
            formSubmitting = true;

            $scope.loadingPromise = talSalesPortalApiService.updatePayment($scope.quoteReferenceNumber, $scope.risk.riskId, paymentSubmitted, paymentSubmitted.type)
                .then(function (response) {
                    formSubmitting = false;

                    updateUiOnSuccess(response);

                    if (paymentSubmitted.type === PAYMENT.TYPE.CREDIT_CARD) {
                        tokeniseExpiryDate();
                    }

                    $scope.currentPaymentStatus.isComplete = response.data.isComplete;
                    talFormModelStateService.updateModelState(response.data, $scope);
                    if(paymentSubmitted.type !== PAYMENT.TYPE.CREDIT_CARD){
                        $scope.expiryDateDefault = resetExpiryDate();
                    }
		    
                    $scope.serverErrorResponse = null;
                })
                .catch(function (response) {
                    talPolicyEditSectionsService.setSectionStatesOnError($scope.section);
                    $scope.section.isModelValid = false;
                    $scope.section.isCompleted = false;

                    talFormModelStateService.updateModelState(response.data, $scope);
                    
                    $scope.paymentsChanged();
                    $scope.paymentsReturnObj[paymentSubmitted.type] = paymentSubmitted;

                    $scope.serverErrorResponse = response;

                    formSubmitting = false;
                });
        };

        $scope.onActivation = function () {

            //when modal open or server errror response has data, do not update ui
            if (!$scope.modalOpenAndStopRefresh && !$scope.serverErrorResponse) {
                $scope.loadingPromise = talSalesPortalApiService.getPayments($scope.quoteReferenceNumber, $scope.risk.riskId)
                    .then(function(response) {
                        if (!$scope.emptyPaymentsSet) {
                            $scope.activePayment = response.data.directDebitPayment;
                            $scope.currentPaymentStatus = _.cloneDeep(response.data);
                            $scope.emptyPaymentsSet = true;
                        }
                        if (!response.data.isComplete) {
                            $scope.paymentsChanged();
                        }

			            $scope.serverResponse = null;
                        updateUiOnSuccess(response);
                    })
                    .catch(function(response) {
                        talPolicyEditSectionsService.setSectionStatesOnError($scope.section);
                        talFormModelStateService.updateModelState(response.data, $scope);
                        $scope.section.isCompleted = false;
                        $scope.section.isModelValid = false;
                    });
            }

            //reset modal status
            if ($scope.modalOpenAndStopRefresh) {
                $scope.modalOpenAndStopRefresh = false;
            }
        };

        talPolicySectionsNotificationService.registerInitialisationChangeEvent(function() {
            $scope.onActivation();
        });

        $scope.expiryDateDefault = resetExpiryDate();

        $scope.paymentsChanged = function(){
            $scope.paymentChanged = true;
        };

        $scope.checkPaymentChanged = function(paymentTabSelected){
            if($scope.paymentChanged && $scope.activePayment.type !== paymentTabSelected){
                $scope.paymentChanged = false;
                onActivation();
                talFormModelStateService.updateCleanModelState($scope);
            }
        };

        $scope.section.onActivationEvent = onActivation;

        talSalesPortalApiService.getAvailablePaymentOptionsForProduct($scope.quoteReferenceNumber)
            .then(function(response) {
                $scope.isCreditCardPaymentAvailable = response.data.isCreditCardAvailable;
                $scope.isDirectDebitPaymentAvailable = response.data.isDirectDebitAvailable;
                $scope.isSuperannuationPaymentAvailable = response.data.isSuperAvailable;
                $scope.isSelfManagementSuperFundPaymentAvailable = response.data.isSmsfAvailable;
            });
    }

    module.controller('talRiskPaymentSectionController', talRiskPaymentSectionController);
    talRiskPaymentSectionController.$inject = ['$scope', 'talPolicySectionsNotificationService', 'talSalesPortalApiService', 'talFormModelStateService', 'talPolicyEditSectionsService', 'PAYMENT', '$uibModal'];

})(angular.module('salesPortalApp'));

