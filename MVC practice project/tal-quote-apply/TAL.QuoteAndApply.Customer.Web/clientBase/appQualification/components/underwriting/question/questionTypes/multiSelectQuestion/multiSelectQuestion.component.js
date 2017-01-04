(function(module){
'use strict';
  module.directive('talMultiSelectQuestion', function () {
      return {
        templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/multiSelectQuestion/multiSelectQuestion.template.html',
        restrict: 'E',
          scope: {
              question:'=',
              onQuestionAnswered:'&'
          },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talMultiSelectQuestionController'
      };
    });

  function talMultiSelectQuestionController(talUnderwritingService) {
      var ctrl = this;

      ctrl.selectedAnswers = [];

      var hasSelectedAnOption = function() {
          return _.some(ctrl.defaultAnswers, {isSelected: true});
      };

      var groupedAnswers = talUnderwritingService.separateDefaultAnswers(ctrl.question);
      //display the other questions after the default questions
      ctrl.defaultAnswers = _.compact(_.concat(groupedAnswers[0], groupedAnswers[1]));


      var columnsPerRow = 3;
      ctrl.answersAsRows = _.chunk(ctrl.defaultAnswers, columnsPerRow);

      this.onAnswerStateChanged = function() {
          ctrl.question.isAnswered = false; //if changed answer then disable the 'next' button otherwise will not update to server if they click 'next' instead of continue
      };

      this.isContinueEnabled = function () {
          return hasSelectedAnOption();
      };

      this.onContinueClick = function() {
          if (!hasSelectedAnOption()){
              throw 'no answer selected';
          }

          ctrl.onQuestionAnswered();
      };
  }

  module.controller('talMultiSelectQuestionController', talMultiSelectQuestionController );
  talMultiSelectQuestionController.$inject = ['talUnderwritingService'];

})(angular.module('appQualification'));
