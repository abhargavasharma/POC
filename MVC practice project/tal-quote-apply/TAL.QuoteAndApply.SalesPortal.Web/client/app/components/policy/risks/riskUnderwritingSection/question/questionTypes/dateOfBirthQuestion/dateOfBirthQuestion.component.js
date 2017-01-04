(function (module) {
    'use strict';
    module.directive('talDateOfBirthQuestion', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskUnderwritingSection/question/questionTypes/dateOfBirthQuestion/dateOfBirthQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&',
                readOnly: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talDateOfBirthQuestionController'
        };
    });

    function controller(talFormModelStateService) {
        var ctrl = this;

        ctrl.dateOfBirth = ctrl.question.answers[0].text;

        this.onContinueClick = function () {

            talFormModelStateService.updateCleanModelState();

            ctrl.question.answers[0].text = ctrl.dateOfBirth;

            ctrl.onQuestionAnswered();
        };
    }

    module.controller('talDateOfBirthQuestionController', controller);
    controller.$inject = ['talFormModelStateService'];

})(angular.module('salesPortalApp'));