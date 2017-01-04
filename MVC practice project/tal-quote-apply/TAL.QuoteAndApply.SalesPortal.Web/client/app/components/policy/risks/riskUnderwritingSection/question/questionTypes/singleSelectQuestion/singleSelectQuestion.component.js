(function(module){
'use strict';
  module.directive('talSingleSelectQuestion', function () {
      return {
          templateUrl: '/client/app/components/policy/risks/riskUnderwritingSection/question/questionTypes/singleSelectQuestion/singleSelectQuestion.template.html',
        restrict: 'E',
        scope: {
            question:'=',
            onQuestionAnswered: '&',
            readOnly: '='
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talSingleSelectQuestionController'
      };
    });

  function talSingleSelectQuestionController(talUnderwritingService) {
      var ctrl = this;
      this.options = [];
      this.specialOptions = [];

      this.groupName = talUnderwritingService.getGroupName(this.question.id);
     
      var alreadySelectedAnswer = talUnderwritingService.getSelectedSingleAnswer(this.question);

      if (alreadySelectedAnswer) {
          this.selectedAnswerId = alreadySelectedAnswer.id;
      }

      this.answerQuestion = function(answer) {
          talUnderwritingService.clearAllAnswers(ctrl.question);
          answer.isSelected = true;

          ctrl.onQuestionAnswered();
      };
  }

  module.controller('talSingleSelectQuestionController', talSingleSelectQuestionController );
  talSingleSelectQuestionController.$inject = ['talUnderwritingService'];

})(angular.module('salesPortalApp'));
