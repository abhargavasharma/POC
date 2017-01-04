(function(module){
  'use strict';

  var uniqueId = 1;

  module.directive('talFigure', function () {
    return {
      templateUrl: '/client/appCustomerPortal/components/figure/figure.template.html',
      restrict: 'E',
      replace: true,
      scope: {
        'value': '=',
        'type': '@',
        'period': '='
      },
      controller: 'talFigureController'
    };
  });

  var talFigureController = function($scope) {
    $scope.id = uniqueId;
    uniqueId += 1;

    // Initialization
    $scope.value = $scope.value || 0;

    $scope.$watch('value', function(newValue) {
      $scope.figValueOld = $scope.figValue;
      $scope.figValue = newValue;
    });
  };

  module.controller('talFigureController', talFigureController);
  talFigureController.$inject = ['$scope'];

})(angular.module('appCustomerPortal'));
