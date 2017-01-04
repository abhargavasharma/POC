(function(module){
  'use strict';

  module.directive('talFigureSimple', function () {
    return {
      templateUrl: '/client/appCustomerPortal/components/figureSimple/figureSimple.template.html',
      restrict: 'E',
      scope: {
        'value': '='
      },
      controller: 'talFigureSimpleController'
    };
  });

  var talFigureSimpleController = function($scope) {
    // Initialization
    $scope.value = $scope.value || 0;
    $scope.figValue = $scope.value;
    $scope.figValueOld = $scope.value;

    $scope.$watch('value', function(newValue) {
      $scope.figValueOld = $scope.figValue;
      $scope.figValue = newValue;
    });
  };

  module.controller('talFigureSimpleController', talFigureSimpleController);
  talFigureSimpleController.$inject = ['$scope'];

})(angular.module('appCustomerPortal'));
