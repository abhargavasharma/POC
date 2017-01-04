(function(module) {
  'use strict';
  module.directive('talMobileNav', function () {
    return {
      templateUrl: '/client/appCustomerPortal/largeComponents/mobileNav/mobileNav.template.html',
      restrict: 'E',
      replace: true,
      scope: {
        navItems: '=?',
        active: '@?',
        stepIndex: '='
      },
      controller: 'mobileNavController'
    };
  });

 
  var mobileNavController = function($scope) {
    $scope.navItems = $scope.navItems || [
      {name: 'General information', backUrl: ''},
      {name: 'Select Cover', backUrl: ''},
      {name: 'About Me', backUrl: ''},
      {name: 'Review Cover', backUrl: ''},
      {name: 'Purchase', backUrl: ''}
    ];

    $scope.active = $scope.active || 0;

    $scope.url = $scope.navItems[$scope.stepIndex].backUrl || '';
    $scope.name = $scope.navItems[$scope.stepIndex].name || '';
    $scope.current = 1 + parseInt($scope.stepIndex);
    $scope.total = $scope.navItems.length;
  };

  module.controller('mobileNavController', mobileNavController);

})(angular.module('appCustomerPortal'));
