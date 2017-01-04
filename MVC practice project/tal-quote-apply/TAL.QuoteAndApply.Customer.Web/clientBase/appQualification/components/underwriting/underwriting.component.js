(function(module){
'use strict';
  module.directive('talUnderwriting', function () {
      return {
        templateUrl: '/client/appQualification/components/underwriting/underwriting.template.html',
        restrict: 'E',
        scope: {},
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talUnderwritingController'
      };
    });

  function talUnderwritingController($timeout, $stateParams, $log, $rootScope, EVENT, talCustomerPortalApiService, talUnderwritingService, talNavigationService, talAnalyticsService, $window, viewBagConfig) {

      var ctrl = this;

      $log.debug($stateParams);

      this.contextQuestions = [];
      this.currentQuestionIndex = 0;

      var setCurrentQuestion = function() {
          ctrl.currentQuestion = ctrl.underwriting.questions[ctrl.currentQuestionIndex];
          ctrl.endOfQuestions = (ctrl.currentQuestionIndex === ctrl.underwriting.questions.length); //TODO: endOfQuestions is just temp for now for displaying on client, may not be needed eventually
      };

      /**
       * Filter matching questions from the main underwriting questions list by id
       * @param questionsToMatch
       * @returns {Array}
       */
      var filterMatchingQuestions = function (questionsToMatch) {
          var filtered= _.filter(ctrl.underwriting.questions, function (q) {
              return _.some(questionsToMatch, function (cq) {
                  return cq.id === q.id;
              });
          });

          $log.debug('uw - filterMatchingQuestions found: ' + filtered.length + ' out of: ' + questionsToMatch.length);

          return filtered;
      };


      var processStateParams = function () {
          var index;
          if($stateParams.questionId){
              index = _.findIndex(ctrl.underwriting.questions, {id: $stateParams.questionId});
              $log.debug('uw - selected question index: ' + index);
              if(index === -1){
                  return false;
              }

              ctrl.currentQuestionIndex = index;
              return true;
          }
          if($stateParams.questionIndex){
              try {
                  index = parseInt($stateParams.questionIndex, 10);
              }
              catch (e){
                  $log.debug(e);
                  return false;
              }

              if(isNaN(index)){
                  return false;
              }

              $log.debug('uw - selected question index: ' + index);
              if(index === -1 || index >= ctrl.underwriting.questions.length){
                  return false;
              }

              ctrl.currentQuestionIndex = index;
              return true;
          }

          return false;
      };

      var handleAnswerResponse = function (answerResponse) {
          talUnderwritingService.applyRemovedQuestions(ctrl.underwriting.questions, answerResponse.removedQuestionIds);
          talUnderwritingService.applyChangedQuestionUpdates(ctrl.underwriting.questions, answerResponse.changedQuestions);
          talUnderwritingService.applyAddedQuestions(ctrl.underwriting.questions, answerResponse.addedQuestions);

          ctrl.underwriting.questions = talUnderwritingService.sortQuestions(ctrl.underwriting.categories, ctrl.underwriting.questions);

          //Find the corresponding questions from the main list
          var changedQuestions = filterMatchingQuestions(answerResponse.changedQuestions);
          var addedQuestions = filterMatchingQuestions(answerResponse.addedQuestions);
          var questionsToUpdateContext = _.union(changedQuestions, addedQuestions);

          //Update context for new questions
          talUnderwritingService.buildQuestionsContext(questionsToUpdateContext, ctrl.contextQuestions);

          ctrl.currentQuestion.isAnswered = talUnderwritingService.hasAnySelectedAnswers(ctrl.currentQuestion); //TODO: would never expect this to be false, but should we check and deal with it?

          ctrl.moveToNextQuestion();
      };

      ctrl.loadingPromise = talCustomerPortalApiService.getRisks().then(function(response) {
          ctrl.risks = response.data;
          ctrl.currentRisk = ctrl.risks[0]; //TODO: when multi risk, can deal with who we are doing underwriting for

          $rootScope.$broadcast(EVENT.RISK_ID.UPDATE, ctrl.currentRisk.id);

          ctrl.loadingPromise = talCustomerPortalApiService.getUnderwritingForRisk(ctrl.currentRisk.id)
              .then(function(response) {
                  ctrl.underwriting = response.data;

                  processStateParams();

                  talUnderwritingService.buildQuestionsContext(ctrl.underwriting.questions, ctrl.contextQuestions);

                  if (viewBagConfig.saveGatePosition === 'BeforeUnderWriting' &&
                    viewBagConfig.journeySource &&
                    (
                      //if we allow saviving/loading - we require password
                    (viewBagConfig.isQuoteSaveLoadEnabled === true && viewBagConfig.saveStatus && viewBagConfig.saveStatus !== 'passwordCreated') ||
                      //if we dont - we require only personal details to be entered
                    (viewBagConfig.isQuoteSaveLoadEnabled === false && viewBagConfig.saveStatus && viewBagConfig.saveStatus !== 'personalDetails')
                    )) {
                      $timeout($rootScope.$broadcast(EVENT.SAVE.TRIGGER_FORCED_SAVE, { hideOnCallBack: true, callBack: setCurrentQuestion }));
                  } else {
                      setCurrentQuestion();
                  }
              });
      });

      this.helpLinkVisible = function () {
          return ctrl.currentQuestion && !ctrl.currentQuestion.showHelp;
      };

      this.showHelp = function () {
          if(!ctrl.currentQuestion){
              return;
          }

          ctrl.currentQuestion.showHelp = true;
      };

      this.getProgress = function () {
          if(!ctrl.underwriting){
              return 0;
          }
                    
          return ctrl.currentQuestionIndex / ctrl.underwriting.questions.length * 100;
      };

      this.findIndexOfCurrentQuestion = function() {
          return _.findIndex(ctrl.underwriting.questions, { 'id': ctrl.currentQuestion.id });
      };

      this.moveToNextQuestion = function() {
          ctrl.movingBackward = false;

          $timeout(function () {
              var currentQuestionIndex = ctrl.findIndexOfCurrentQuestion();
              ctrl.currentQuestionIndex = currentQuestionIndex + 1;
              setCurrentQuestion();
          });
      };

      this.moveToPreviousQuestion = function() {
          ctrl.movingBackward = true;

          $timeout(function () {
              if (ctrl.currentQuestionIndex > 0) {
                  ctrl.currentQuestionIndex--;
                  setCurrentQuestion();
              }
          });
      };

      this.onQuestionAnswered = function(question) {
          var selectedAnswers = _.filter(question.answers, {isSelected: true});
          var questionAnswer = {
              questionId: question.id,
              selectedAnswers: selectedAnswers
          };

          ctrl.disableUtilityButtons = true; //Don't want cg-busy to extend outside question area, so other button states are handled this way
          ctrl.loadingPromise = talCustomerPortalApiService.answerQuestionForRisk(ctrl.currentRisk.id, questionAnswer).then(function(response) {
              handleAnswerResponse(response.data);
              ctrl.disableUtilityButtons = false;

              var answerListFlat = _.join(_.map(questionAnswer.selectedAnswers, 'text'), ';');
              talAnalyticsService.qualificationSection.trackQuestion(question.text, answerListFlat, question.category, question.context || '');

              $window.scrollTo(0, 0);

          }).catch(function(response){
              //TODO: handle underwriting errors
              $log.debug('UNDERWRITING ERROR', response);
              ctrl.disableUtilityButtons = false;
          });
      };

      this.hidePreviousButton = function () {
          return ctrl.currentQuestionIndex === 0;
      };

      this.showNextButton = function() {
          var currentQuestionIsAnswered = ctrl.currentQuestion && ctrl.currentQuestion.isAnswered;
          return currentQuestionIsAnswered && ctrl.underwriting && ctrl.currentQuestionIndex < ctrl.underwriting.questions.length;
      };

      this.onProceedClick = function() {
          ctrl.disableUtilityButtons = true; //Don't want cg-busy to extend outside question area, so other button states are handled this way
          ctrl.loadingPromise = talCustomerPortalApiService.validateUnderwritingForRisk(ctrl.currentRisk.id).then(function (response) {
              talAnalyticsService.qualificationSection.trackCompletionOfQuestions();
              ctrl.loadingPromise = talNavigationService.handleServerRedirectAction(response.data);
          }).catch(function(response){
              //TODO: what if we get to the end of underwriting but still errors :(
              $log.debug('UNDERWRITING VALIDATION ERROR', response);
              ctrl.disableUtilityButtons = false;
          });
      };
  }

  module.controller('talUnderwritingController', talUnderwritingController );
  talUnderwritingController.$inject = ['$timeout', '$stateParams', '$log', '$rootScope', 'EVENT', 'talCustomerPortalApiService', 'talUnderwritingService', 'talNavigationService', 'talAnalyticsService', '$window', 'viewBagConfig'];

})(angular.module('appQualification'));
