(function(module){
'use strict';
  module.directive('talMultiSelectAutoCompleteQuestion', function () {
      return {
        templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/multiSelectAutoCompleteQuestion/multiSelectAutoCompleteQuestion.template.html',
        restrict: 'E',
          scope: {
              question:'=',
              onQuestionAnswered:'&'
          },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talMultiSelectAutoCompleteQuestionController'
      };
    });

  function talMultiSelectAutoCompleteQuestionController(talUnderwritingService) {
      var ctrl = this;




      var hasSelectedAnOption = function() {
          return _.some(ctrl.defaultAnswers, {isSelected: true});
      };

      var groupedAnswers = talUnderwritingService.separateDefaultAnswers(ctrl.question);
      //display the other questions after the default questions
      ctrl.defaultAnswers = _.compact(_.concat(groupedAnswers[0], groupedAnswers[1]));

      ctrl.selectedAnswers = _.filter(ctrl.defaultAnswers, {isSelected: true});

      var columnsPerRow = 3;
      ctrl.answersAsRows = _.chunk(ctrl.defaultAnswers, columnsPerRow);

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

  module.controller('talMultiSelectAutoCompleteQuestionController', talMultiSelectAutoCompleteQuestionController );
    talMultiSelectAutoCompleteQuestionController.$inject = ['talUnderwritingService'];

})(angular.module('appQualification'));
