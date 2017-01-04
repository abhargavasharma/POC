
(function(module){
'use strict';
  module.directive('talDashboardSection', function () {
      return {
        templateUrl: '/client/app/components/dashboard/dashboardSection/dashboardSection.template.html',
        restrict: 'E',
        scope: {
            user: '@',
            role: '@',
            externalRefRequired: '=',
            externalRefLabel: '@'
        },
        controller: 'talDashboardSectionController'
      };
    });

  function talDashboardSectionController($scope) {
      $scope.roles = {
          underwriter: $scope.role === 'underwriter',
          agent: $scope.role === 'agent'
      };
  }

  module.controller('talDashboardSectionController', talDashboardSectionController );
  talDashboardSectionController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

