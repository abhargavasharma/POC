(function(module){
'use strict';
  module.directive('talReviewForm', function () {
      return {
        templateUrl: '/client/appReview/components/reviewForm/reviewForm.template.html',
        restrict: 'E',
        scope: {},
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talReviewFormController'
      };
    });

  function talReviewFormController($rootScope, EVENT, talCustomerPortalApiService, talNavigationService, talContentService,
                                   talFormModelStateService, talCustomerPremiumService, talPlanSelectionService, pageSpinnerService,
                                   viewBagConfig, $window, talAnalyticsService, $log, $scope) {
      var ctrl = this;
      ctrl.links = talContentService.getContentByReferenceKey('shared.links');
      ctrl.contact = talContentService.getContentByReferenceKey('shared.contact');
      ctrl.submissionErrorMessage = '';
      ctrl.isQuoteSaveLoadEnabled = viewBagConfig.isQuoteSaveLoadEnabled;

      var setSubmitButtonText = function(reviewWorkflowStatus) {
          switch (reviewWorkflowStatus) {
              case 'Accept':
                  ctrl.state = 'accept';
                  break;
              default:
                  ctrl.state = 'refer';
          }

          ctrl.submitButtonText = talContentService.getContentByReferenceKey('review.buttonText.' + ctrl.state);
      };

      var onSuccessfulServerResponse = function(planResponse) {
          ctrl.reviewModel.cover.totalPremium = planResponse.totalPremium;
          talPlanSelectionService.updatePlanSelection(ctrl.reviewModel.cover.premiumTypeOptions, planResponse.premiumTypeOptions);
          talPlanSelectionService.updatePlanSelection(ctrl.reviewModel.cover.plans, planResponse.plans);
          talPlanSelectionService.updateSelectedVariableTextForPlans(ctrl.reviewModel.cover.plans);
          talCustomerPremiumService.triggerPremiumUpdated(ctrl.reviewModel.cover.totalPremium, ctrl.reviewModel.cover.paymentFrequency);
          talFormModelStateService.updateCleanModelState();
      };

      var onFirstLoadSuccessfulServerResponse = function(response) {
          pageSpinnerService.stop();
          ctrl.reviewModel = response.data;
          talPlanSelectionService.setRelatedRiders(ctrl.reviewModel.cover.plans);
          talCustomerPremiumService.triggerPremiumUpdated(ctrl.reviewModel.cover.totalPremium, ctrl.reviewModel.cover.paymentFrequency);
          setSubmitButtonText(ctrl.reviewModel.reviewWorkflowStatus);
          talPlanSelectionService.attachAllContent(ctrl.reviewModel.cover.plans, ctrl.reviewModel.cover.isOccupationTpdOwn, ctrl.reviewModel.cover.isOccupationTpdAny);
          talPlanSelectionService.setPaymentFrequencyPer(ctrl.reviewModel.cover);
      };

      ctrl.loadingPromise = talCustomerPortalApiService.getRisks().then(function(response){
          ctrl.risk = response.data[0]; //Only dealing with single risk at the moment

          $rootScope.$broadcast(EVENT.RISK_ID.UPDATE, ctrl.risk.id);

          ctrl.loadingPromise = talCustomerPortalApiService.getReviewForRisk(ctrl.risk.id).then(function(response){
              onFirstLoadSuccessfulServerResponse(response);
          }).catch(function(){
              //TODO: handle getting error from server on initial page load
          });
      });

      this.onRequestCallback = function () {
          //triggers an event handled in clickToChat.component to display popup for request a callback
          $rootScope.$broadcast(EVENT.CHAT.CALLBACK);
      };

      this.selectPremiumType = function(premiumType) {
          ctrl.loadingPromise = talCustomerPortalApiService.updatePremiumTypeForRisk(ctrl.risk.id, premiumType.premiumType).then(function(response){
              onSuccessfulServerResponse(response.data.cover);
              talAnalyticsService.reviewSection.trackPremiumType(premiumType.premiumTypeName);
          }).catch(function(response){
              talFormModelStateService.updateModelState(response.data);
          });
      };

      this.updatePlan = function(plan) {
          var updateRequest = talPlanSelectionService.buildPlanRequest(ctrl.reviewModel.cover.plans, plan);
          updateRequest.includeReviewInfoInResponse = true;
          ctrl.loadingPromise = talCustomerPortalApiService.updateCoverSelectionForRisk(ctrl.risk.id, updateRequest).then(function (response) {
              onSuccessfulServerResponse(response.data.cover);
              ctrl.reviewModel.sharedLoadings = response.data.sharedLoadings;
              ctrl.reviewModel.questionChoices = response.data.questionChoices;
              setSubmitButtonText(response.data.reviewWorkflowStatus);
              ctrl.submissionErrorMessage = '';
	      talPlanSelectionService.updatePlanSelection(ctrl.reviewModel.questionChoices, response.data.questionChoices);
              talAnalyticsService.reviewSection.trackPlan(plan, response.data.totalPremium, response.data.paymentFrequency);
          }).catch(function (response) {
              talFormModelStateService.updateModelState(response.data);
          });
          return ctrl.loadingPromise;
      };

      this.changeQuestionChoice = function(questionChoice) {
          var questionAnswer = {
              questionId: questionChoice.questionId,
              selectedAnswers: [{id: questionChoice.answerId}]
          };

          ctrl.loadingPromise = talCustomerPortalApiService.switchQuestionChoice(ctrl.risk.id, questionAnswer)
              .then(function(response) {
                  onSuccessfulServerResponse(response.data.cover);
                  ctrl.reviewModel.sharedLoadings = response.data.sharedLoadings;
                  ctrl.reviewModel.questionChoices = response.data.questionChoices;
              })
              .catch(function(response){
                  //TODO: really need to do something about errors
                  $log.debug('error', response);
              });
      };

      this.attachRider = function(rider, planCode) {
          ctrl.loadingPromise = talCustomerPortalApiService.attachRiderForRisk(ctrl.risk.id, rider.riderFor, planCode).then(function (response) {
              onSuccessfulServerResponse(response.data);
          }).catch(function(response){
              talFormModelStateService.updateModelState(response.data);
          });
      };

      function detachRider(riderCode) {
          ctrl.loadingPromise = talCustomerPortalApiService.detachRiderForRisk(ctrl.risk.id, riderCode).then(function(response) {
              onSuccessfulServerResponse(response.data);
          }).catch(function(response){
              talFormModelStateService.updateModelState(response.data);
          });
      }

      ctrl.toggleRiderBundled = function(rider, plan) {
          if (rider.isSelected) {
              ctrl.attachRider(rider, plan.planCode);
          } else {
              detachRider(rider.planCode);
          }
      };
      ctrl.onSaveClick = function() {
          $window.setTimeout(function() {
                  $scope.$apply(function() {
                      ctrl.savedStatus = 'saveClicked';
                  });
              },
              2000);
      };
          
      this.onSubmitClick = function() {
          ctrl.loadingPromise = talCustomerPortalApiService.validateReviewForRisk(ctrl.risk.id).then(function(response) {
              ctrl.loadingPromise = talNavigationService.handleServerRedirectAction(response.data);
          }).catch(function (response) {
              if (response.data.minimumSelectionCriteria) {
                  ctrl.submissionErrorMessage = response.data.minimumSelectionCriteria;
              }
              talFormModelStateService.updateModelState(response.data);
          });
      };

      ctrl.onUnderwritingSummary = function() {
          $window.open('/summary', '_blank');
      };

      $scope.$on(EVENT.CHAT.CHAT_AVAILABILITY, function ($event, chatAvailability) {
          $log.debug('reviewForm - chatAvailability: ' + JSON.stringify(chatAvailability));
          ctrl.chatAvailability = chatAvailability;
      });
  }

  module.controller('talReviewFormController', talReviewFormController );
  talReviewFormController.$inject = ['$rootScope', 'EVENT', 'talCustomerPortalApiService', 'talNavigationService', 'talContentService',
      'talFormModelStateService', 'talCustomerPremiumService', 'talPlanSelectionService', 'pageSpinnerService',
      'viewBagConfig', '$window', 'talAnalyticsService', '$log', '$scope'];

})(angular.module('appReview'));
