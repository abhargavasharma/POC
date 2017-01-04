(function (module) {
    'use strict';
    module.directive('talSingleSelectIconsQuestion', function () {
        return {
            templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/singleSelectIconsQuestion/singleSelectIconsQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talSingleSelectIconsQuestionController'
        };
    });

    function talSingleSelectIconsQuestionController(talUnderwritingService) {
        var ctrl = this;
        this.showAdditionalAnswers = false;
        this.options = [];
        this.specialOptions = [];


        this.groupName = talUnderwritingService.getGroupName(this.question.id);

        this.isColumnLayout = this.question.answers.length > 3;

        var alreadySelectedAnswer = talUnderwritingService.getSelectedSingleAnswer(this.question);

        if (alreadySelectedAnswer) {
            this.selectedAnswerId = alreadySelectedAnswer.id;

            this.showAdditionalAnswers =
                !talUnderwritingService.isDefaultAnswer(alreadySelectedAnswer) ||
                !talUnderwritingService.isIconAnswer(alreadySelectedAnswer);
        }

        var OPTION_TYPE = {
            SPECIAL: 'SPECIAL',
            ICON: 'ICON',
            NON_ICON: 'NON_ICON'
        };

        var grouped = _.groupBy(this.question.answers, function (answer) {
            if (!talUnderwritingService.isDefaultAnswer(answer)) {
                return OPTION_TYPE.SPECIAL;
            }

            return talUnderwritingService.isIconAnswer(answer) ? OPTION_TYPE.ICON : OPTION_TYPE.NON_ICON;
        });

        ctrl.options = talUnderwritingService.mapAnswersToOptions(_.get(grouped, OPTION_TYPE.ICON));
        ctrl.additionalOptions = talUnderwritingService.mapAnswersToOptions(_.get(grouped, OPTION_TYPE.NON_ICON));
        ctrl.specialOptions = talUnderwritingService.mapAnswersToOptions(_.get(grouped, OPTION_TYPE.SPECIAL));

        this.optionChanged = function () {
            talUnderwritingService.clearAllAnswers(ctrl.question);
            talUnderwritingService.setAnsweredById(ctrl.question, ctrl.selectedAnswerId);

            ctrl.onQuestionAnswered();
        };
    }

    module.controller('talSingleSelectIconsQuestionController', talSingleSelectIconsQuestionController);
    talSingleSelectIconsQuestionController.$inject = ['talUnderwritingService'];

})(angular.module('appQualification'));
