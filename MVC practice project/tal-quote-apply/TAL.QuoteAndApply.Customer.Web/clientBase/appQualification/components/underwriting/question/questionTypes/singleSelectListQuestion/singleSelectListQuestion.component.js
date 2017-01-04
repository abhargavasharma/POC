(function (module) {
    'use strict';
    module.directive('talSingleSelectListQuestion', function () {
        return {
            templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/singleSelectListQuestion/singleSelectListQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talSingleSelectListQuestionController'
        };
    });

    function talSingleSelectListQuestionController(talUnderwritingService) {
        var ctrl = this;

        ctrl.selectedAnswer = talUnderwritingService.getSelectedSingleAnswer(ctrl.question);

        this.continueDisabled = function () {
            return !this.selectedAnswer;
        };

        this.onAnswerChanged = function() {
            //if changed answer then disable the 'next' button otherwise will not update to server if they click 'next' instead of continue
            //TODO: revisit implementation of the 'next' button, is there a better way?
            ctrl.question.isAnswered = false;
        };

        this.onContinueClick = function () {
            talUnderwritingService.clearAllAnswers(ctrl.question);
            ctrl.selectedAnswer.isSelected = true;
            ctrl.onQuestionAnswered();
        };
    }

    module.controller('talSingleSelectListQuestionController', talSingleSelectListQuestionController);
    talSingleSelectListQuestionController.$inject = ['talUnderwritingService'];

})(angular.module('appQualification'));
