(function(module){
'use strict';


    /*
    TEMPORARY CONTROL TO SHOW UNSUPPORTED QUESTION TYPES, UNTIL WE CAN DEAL WITH ALL OF THEM
     */

  module.directive('talUnsupportedQuestion', function () {
      return {
        templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/unsupportedQuestion/unsupportedQuestion.template.html',
        restrict: 'E',
        scope: {
            question:'=',
            onQuestionAnswered:'&'
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talUnsupportedQuestionController'
      };
    });

  function talUnsupportedQuestionController() {
      var ctrl = this; //For if you need to use 'this' within a child scope
      this.moveToNextQuestion = function() {
          ctrl.onQuestionAnswered();
      };
  }

  module.controller('talUnsupportedQuestionController', talUnsupportedQuestionController );
  talUnsupportedQuestionController.$inject = [];

})(angular.module('appQualification'));
