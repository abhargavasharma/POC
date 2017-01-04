(function(module) {
  'use strict';
  module.directive('talMainNav', function () {
    return {
      templateUrl: '/client/appCustomerPortal/largeComponents/mainNav/mainNav.template.html',
      restrict: 'E',
      replace: true,
      scope: {
        navItems: '=?',
        active: '@?',
        stepIndex: '='
      },
      controller: 'mainNavController'
    };
  });

 
  var mainNavController = function($scope) {
    $scope.navItems = $scope.navItems || [
      {name: 'General Information', url: 'fed-basic-info'},
      {name: 'Select Cover', url: 'fed-select'},
      {name: 'About Me', url: 'fed-qualification'},
      {name: 'Review Cover', url: 'fed-review'},
      {name: 'Purchase Cover', url: 'fed-purchase'}
    ];

    $scope.active = $scope.active || 0;
  };

  module.controller('mainNavController', mainNavController);

})(angular.module('appCustomerPortal'));
