(function(module){
'use strict';
  module.directive('talServerFormWarning', function () {
      return {
        templateUrl: '/client/app/components/serverFormWarning/serverFormWarning.template.html',
        restrict: 'E',
        scope: {
            warningKey: '='
        },
        controller: 'talServerFormWarningController'
      };
    });

  function talServerFormWarningController($scope, talFormModelStateService) {

      $scope.warningKeyObj = {
          key: $scope.warningKey,
          message: null
      };

      talFormModelStateService.registerWarningKey($scope.warningKeyObj);

      $scope.anAttribute = 'formWarning';
  }

  module.controller('talServerFormWarningController', talServerFormWarningController );
    talServerFormWarningController.$inject = ['$scope', 'talFormModelStateService'];

})(angular.module('salesPortalApp'));

