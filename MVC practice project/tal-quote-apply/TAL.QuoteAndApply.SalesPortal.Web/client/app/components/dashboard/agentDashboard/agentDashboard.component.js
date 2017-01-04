(function(module){
'use strict';
  module.directive('talAgentDashboard', function () {
      return {
        templateUrl: '/client/app/components/dashboard/agentDashboard/agentDashboard.template.html',
        restrict: 'E',
        scope: {
            externalRefRequired: '=',
            externalRefLabel: '='
        },
        controller: 'talAgentDashboardController'
      };
    });

  function talAgentDashboardController() {
  }

  module.controller('talAgentDashboardController', talAgentDashboardController );
  talAgentDashboardController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

