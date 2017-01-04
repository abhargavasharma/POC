(function (module) {
    'use strict';
    module.directive('talSingleSelectListQuestion', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskUnderwritingSection/question/questionTypes/singleSelectListQuestion/singleSelectListQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&',
                readOnly: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talSingleSelectListQuestionController'
        };
    });

    function talSingleSelectListQuestionController(talUnderwritingService) {
        var ctrl = this;

        ctrl.selectedAnswer = undefined;
        var answer = talUnderwritingService.getSelectedSingleAnswer(ctrl.question);
        if (answer) {
            ctrl.selectedAnswer = answer.id;
        }

        this.continueDisabled = function () {
            return !this.selectedAnswer;
        };

        this.onAnswerChanged = function() {
            ctrl.question.isAnswered = false;
        };

        this.onContinueClick = function () {
            talUnderwritingService.clearAllAnswers(ctrl.question);
            talUnderwritingService.setAnsweredById(ctrl.question, ctrl.selectedAnswer);
            ctrl.onQuestionAnswered();
        };
    }

    module.controller('talSingleSelectListQuestionController', talSingleSelectListQuestionController);
    talSingleSelectListQuestionController.$inject = ['talUnderwritingService'];

})(angular.module('salesPortalApp'));
