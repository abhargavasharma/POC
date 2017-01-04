(function (module) {
    'use strict';
    module.directive('talCurrencyQuestion', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskUnderwritingSection/question/questionTypes/currencyQuestion/currencyQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&',
                readOnly: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talCurrencyQuestionController'
        };
    });

    function controller(talFormModelStateService) {
        var ctrl = this;

        ctrl.annualIncome = ctrl.question.answers[0].text;

        this.onContinueClick = function () {
            talFormModelStateService.updateCleanModelState();

            ctrl.question.answers[0].text = ctrl.annualIncome;

            ctrl.onQuestionAnswered();
        };
    }

    module.controller('talCurrencyQuestionController', controller);
    controller.$inject = ['talFormModelStateService'];

})(angular.module('salesPortalApp'));
