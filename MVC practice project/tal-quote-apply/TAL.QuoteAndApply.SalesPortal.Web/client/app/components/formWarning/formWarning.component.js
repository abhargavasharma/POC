(function(module){
'use strict';
  module.directive('talFormWarning', function () {
      return {
        templateUrl: '/client/app/components/formWarning/formWarning.template.html',
        restrict: 'E',
        scope: {
            warnings: '='
        },
        controller: 'talFormWarningController'
      };
    });

  function talFormWarningController($scope) {
      $scope.anAttribute = 'formWarning';
  }

  module.controller('talFormWarningController', talFormWarningController );
  talFormWarningController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

