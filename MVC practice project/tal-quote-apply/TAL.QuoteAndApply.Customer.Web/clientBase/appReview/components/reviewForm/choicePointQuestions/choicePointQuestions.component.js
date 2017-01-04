(function (module) {
    'use strict';
    module.directive('talChoicePointQuestions', function () {
        return {
            templateUrl: '/client/appReview/components/reviewForm/choicePointQuestions/choicePointQuestions.template.html',
            restrict: 'E',
            scope: {
                questionChoices: '=',
                paymentFrequency: '=',
                onChangeQuestionChoice: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talChoicePointQuestionsController'
        };
    });

    function talChoicePointQuestionsController(talContentService) {

        this.getPlansAffectedText = function(questionChoice) {
            var plansAffectedText = '';
            var insertAndTextAtIndex = questionChoice.applicablePlanCodes.length - 1;

            _.each(questionChoice.applicablePlanCodes, function(plancode, index) {
                if (plansAffectedText) {
                    plansAffectedText += index === insertAndTextAtIndex ? ' and ' : ', ';
                }
                var planBlockContent = talContentService.planBlockContent[plancode];
                if (planBlockContent) {
                    plansAffectedText += planBlockContent.planTitleRaw || planBlockContent.planTitle;
                }
            });
            return plansAffectedText;
        };
    }

    module.controller('talChoicePointQuestionsController', talChoicePointQuestionsController);
    talChoicePointQuestionsController.$inject = ['talContentService'];

})(angular.module('appReview'));
