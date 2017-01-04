(function(module){
    'use strict';
    module.directive('talPurchaseForm', function () {
        return {
            templateUrl: '/client/appPurchase/components/purchaseForm/purchaseForm.template.html',
            restrict: 'E',
            scope: {},
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talPurchaseFormController'
        };
    });

    function talPurchaseFormController($rootScope, EVENT, talCustomerPortalApiService, talFormModelStateService, talNavigationService, talCustomerPremiumService, talContentService) {
        var ctrl = this;

        ctrl.submissionErrorMessage = talContentService.getCompiledContent('purchase.submissionErrorMessage');
        ctrl.paymentErrorMessage = null;
        ctrl.selectedPaymentType = 'DirectDebit';

        var resetFormState = function () {
            talFormModelStateService.updateCleanModelState();
        };

        ctrl.loadingPromise = talCustomerPortalApiService.initPurchase().then(function (response) {
            ctrl.purchaseModel = response.data.riskPurchaseResponses[0]; //When going to multirisk can determine what to do here.

            talCustomerPremiumService.triggerPremiumUpdated(response.data.totalPremium, response.data.paymentFrequency);

            $rootScope.$broadcast(EVENT.RISK_ID.UPDATE, ctrl.purchaseModel.riskId);

            ctrl.purchaseModel.paymentOptions.resetFormState = resetFormState;

            //for client side validation purpose
            if (ctrl.purchaseModel.paymentOptions.creditCardPayment.cardType && ctrl.purchaseModel.paymentOptions.creditCardPayment.cardType === 'Unknown') {
                ctrl.purchaseModel.paymentOptions.creditCardPayment.cardType = null;
            }
        });

        function getMonthDefault() {
            return 'mm';
        }

        function getYearDefault() {
            return 'yy';
        }

        function resetExpiryDate() {
            return {
                'month': getMonthDefault(),
                'year': getYearDefault(),
                'getMonthDefault': getMonthDefault,
                'getYearDefault': getYearDefault
            };
        }

        function clearError() {
            //clear error
            if (ctrl.purchaseModel.paymentOptions.creditCardPayment && ctrl.purchaseModel.paymentOptions.creditCardPayment.error) {
                delete ctrl.purchaseModel.paymentOptions.creditCardPayment.error;
            }
            ctrl.showSubmissionFailureMessage = false;
        }

        ctrl.submit = function () {
            clearError();
            
            if (ctrl.selectedPaymentType === 'DirectDebit') {
                ctrl.purchaseModel.paymentOptions.isCreditCardSelected = false;
                ctrl.purchaseModel.paymentOptions.isDirectDebitSelected = true;
                ctrl.purchaseModel.paymentOptions.creditCardPayment = null;
            }
            else if (ctrl.selectedPaymentType === 'CreditCard') {
                ctrl.purchaseModel.paymentOptions.isCreditCardSelected = true;
                ctrl.purchaseModel.paymentOptions.isDirectDebitSelected = false;
                ctrl.purchaseModel.paymentOptions.directDebitPayment = null;
            } else {
                ctrl.purchaseModel.paymentOptions.isCreditCardSelected = false;
                ctrl.purchaseModel.paymentOptions.isDirectDebitSelected = false;
            }

            ctrl.purchaseModel.nominateLpr = ctrl.purchaseModel.beneficiaries && ctrl.purchaseModel.beneficiaries.length === 0;
            ctrl.loadingPromise = talCustomerPortalApiService.createPurchase(ctrl.purchaseModel).then(function (response) {
                talNavigationService.handleServerRedirectAction(response.data);
            }).catch(function (response) {
                
                //todo: refactor into formModelState service
                if (response.data.submissionFailure) {
                    ctrl.showSubmissionFailureMessage = true;
                }

                if (response.data.multiplePaymentTypes) {
                    ctrl.paymentErrorMessage = response.data.multiplePaymentTypes[0];
                }
                if (response.statusText === 'CreditCardPayment') {
                    ctrl.purchaseModel.paymentOptions.creditCardPayment.error = response;
                }
                else {
                    talFormModelStateService.updateModelState(response.data);
                }
                $rootScope.$broadcast(EVENT.SUBMIT.ON_ERROR);
            });
        };

        ctrl.expiryDateDefault = resetExpiryDate();
    }

    module.controller('talPurchaseFormController', talPurchaseFormController );
    talPurchaseFormController.$inject = ['$rootScope', 'EVENT', 'talCustomerPortalApiService', 'talFormModelStateService', 'talNavigationService', 'talCustomerPremiumService', 'talContentService'];

})(angular.module('appPurchase'));
