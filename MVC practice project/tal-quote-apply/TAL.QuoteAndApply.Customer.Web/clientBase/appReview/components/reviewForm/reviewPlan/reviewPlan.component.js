(function(module){
'use strict';
  module.directive('talReviewPlan', function () {
      return {
        templateUrl: '/client/appReview/components/reviewForm/reviewPlan/reviewPlan.template.html',
        restrict: 'E',
        scope: {
            plan:'=',
            questionChoices:'=',
            sharedLoadings:'=',
            onChangeQuestionChoice:'=',
            onPlanChanged:'&',
            onRiderBundleToggle:'=',
            onAttachAsRider:'=',
            paymentFrequency:'=',
            paymentFrequencyPer:'=',
            chatAvailability:'='
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talReviewPlanController'
      };
    });

  function talReviewPlanController($scope, $log, ngDialog, talPlanSelectionService, $window) {
      var ctrl = this;

      function attachApplicableQuestionChoices() {
          ctrl.planQuestionChoices = _.filter(ctrl.questionChoices, function(questionChoice){
              return questionChoice.applicablePlanCodes.indexOf(ctrl.plan.planCode) > -1;
          });
      }

      function attachApplicableLoadings() {
          ctrl.loadings = _.filter(ctrl.sharedLoadings, function(loading){
              return loading.applicablePlanCodes.indexOf(ctrl.plan.planCode) > -1;
          });
      }

      //TODO: using inefficient watch here, could be changed to use a notification service to know when to update
      $scope.$watch('ctrl.sharedLoadings', function(newVal) {
          if (newVal) {
              attachApplicableLoadings();
          }
      });

      //TODO: using inefficient watch here, could be changed to use a notification service to know when to update
      $scope.$watch('ctrl.questionChoices', function(newVal) {
          if (newVal) {
              attachApplicableQuestionChoices();
          }
      });

      //TODO: function used in ng-show so will be pretty expensive :(
      ctrl.hasExclusionsToShow = function() {
          return ctrl.plan.exclusionNames.length || _.some(ctrl.plan.covers, {isSelected: false});
      };

      //TODO: function used in ng-show so will be pretty expensive :(
      ctrl.hasInclusionsToShow = function() {
          return ctrl.loadings.length || _.some(ctrl.plan.covers, {isSelected: true});
      };

      //Chunk variable options into groups of two so can display nicely
      _.forEach(ctrl.plan.variables, function(variable) {
          variable.chunkedVariableOptions = _.chunk(variable.options, 2);
      });

      ctrl.onVariableChanged = function(changedVariable) {
          talPlanSelectionService.updateSelectedVariableTextForVariable(changedVariable);
          $scope.updatingPromise = ctrl.onPlanChanged(); //Promise needs to be on $scope for dialog to track it
      };

      ctrl.getVariableSelectedText = function(variableCode) {
          var variable = _.find(ctrl.plan.variables, {code:variableCode});
          if (variable) {
              return variable.selectedText;
/*
              var selectedOption = variable.find(variable, {selected:true});
              if (selectedOption) {
                  return variable.shortName || variable.name;
              }
*/
          }
      };

      ctrl.showVariableSelectionDialog = function(selectedIndex) {
          ngDialog.open({
              templateUrl: '/client/appReview/components/reviewForm/reviewPlan/modal/variableSelectionModal.template.html',
              controllerAs: 'ctrl',
              scope: $scope,
              data: {
                  plan: ctrl.plan,
                  selectedIndex: selectedIndex,
                  onOptionChanged: ctrl.onVariableChanged,
                  updatingPromise: ctrl.updatingPromise
              }
          });
          $window.scrollTo(0,0);
      };
  }

  module.controller('talReviewPlanController', talReviewPlanController );
    talReviewPlanController.$inject = ['$scope', '$log', 'ngDialog', 'talPlanSelectionService', '$window'];

})(angular.module('appReview'));
