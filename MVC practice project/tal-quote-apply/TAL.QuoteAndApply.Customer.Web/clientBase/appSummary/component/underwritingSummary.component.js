(function(module){
'use strict';
  module.directive('talUnderwritingSummary', function () {
      return {
        templateUrl: '/client/appSummary/component/underwritingSummary.template.html',
        restrict: 'E',
        scope: {},
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talUnderwritingSummaryController'
      };
    });

  function talUnderwritingSummaryController(talCustomerPortalApiService, talUnderwritingService) {
      var ctrl = this;
      ctrl.contextQuestions = [];
    ctrl.loadingPromise = talCustomerPortalApiService.getRisks()
        .then(function(response) {
            ctrl.loadingPromise = talCustomerPortalApiService.getUnderwritingForRisk(response.data[0].id)
                .then(function(response) {
                    ctrl.questions = response.data.questions;
                    talUnderwritingService.buildQuestionsContext(ctrl.questions, ctrl.contextQuestions);
                });
        });
  }

  module.controller('talUnderwritingSummaryController', talUnderwritingSummaryController );
  talUnderwritingSummaryController.$inject = ['talCustomerPortalApiService', 'talUnderwritingService'];

})(angular.module('appSummary'));