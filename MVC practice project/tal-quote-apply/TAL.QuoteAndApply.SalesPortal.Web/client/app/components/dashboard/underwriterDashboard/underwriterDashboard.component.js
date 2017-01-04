(function(module){
'use strict';
  module.directive('talUnderwriterDashboard', function () {
      return {
        templateUrl: '/client/app/components/dashboard/underwriterDashboard/underwriterDashboard.template.html',
        restrict: 'E',
        scope: {},
        controller: 'talUnderwriterDashboardController'
      };
    });

  function talUnderwriterDashboardController() {
  }

  module.controller('talUnderwriterDashboardController', talUnderwriterDashboardController );
  talUnderwriterDashboardController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

