(function(module){
'use strict';
  module.directive('talSingleSelectQuestion', function () {
      return {
        templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/singleSelectQuestion/singleSelectQuestion.template.html',
        restrict: 'E',
        scope: {
            question:'=',
            onQuestionAnswered:'&'
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

      this.isColumnLayout = this.question.answers.length > 3;
      
      var alreadySelectedAnswer = talUnderwritingService.getSelectedSingleAnswer(this.question);

      if (alreadySelectedAnswer) {
          this.selectedAnswerId = alreadySelectedAnswer.id;
      }

      var threeOptionsOrLess = this.question.answers.length <= 3;

      _.forEach(this.question.answers, function(answer) {

          var option = talUnderwritingService.createOptionForAnswer(answer);

          if(threeOptionsOrLess || answer.answerType === 'Default'){
              ctrl.options.push(option);
          }
          else{
              ctrl.specialOptions.push(option);
          }

      });

      this.optionChanged = function() {
          talUnderwritingService.clearAllAnswers(ctrl.question);
          talUnderwritingService.setAnsweredById(ctrl.question, ctrl.selectedAnswerId);

          ctrl.onQuestionAnswered();
      };
  }

  module.controller('talSingleSelectQuestionController', talSingleSelectQuestionController );
  talSingleSelectQuestionController.$inject = ['talUnderwritingService'];

})(angular.module('appQualification'));
