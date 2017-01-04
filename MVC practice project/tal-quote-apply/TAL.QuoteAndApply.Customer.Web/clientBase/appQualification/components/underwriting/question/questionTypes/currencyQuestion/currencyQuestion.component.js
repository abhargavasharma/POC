(function (module) {
    'use strict';
    module.directive('talCurrencyQuestion', function () {
        return {
            templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/currencyQuestion/currencyQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talCurrencyQuestionController'
        };
    });

    function controller(talCustomerPortalApiService, talFormModelStateService) {
        var ctrl = this;

        ctrl.annualIncome = ctrl.question.answers[0].text;

        this.onContinueClick = function () {
            var vm = {
                annualIncome: ctrl.annualIncome
            };

            ctrl.loadingPromise = talCustomerPortalApiService.validateIncome(vm)
                .then(function () {
                    talFormModelStateService.updateCleanModelState();

                    ctrl.question.answers[0].text = ctrl.annualIncome;

                    ctrl.onQuestionAnswered();

                }).catch(function (response) {
                    talFormModelStateService.updateModelState(response.data);
                });
        };
    }

    module.controller('talCurrencyQuestionController', controller);
    controller.$inject = ['talCustomerPortalApiService', 'talFormModelStateService'];

})(angular.module('appQualification'));
