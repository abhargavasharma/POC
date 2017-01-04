(function(module){
'use strict';
  module.directive('talFormUpdatingIndicator', function () {
      return {
        templateUrl: '/client/app/components/formUpdatingIndicator/formUpdatingIndicator.template.html',
        restrict: 'E',
        scope: {
            promise: '=',
            errorMessage: '@'
        },
        controller: 'talFormUpdatingIndicatorController'
      };
    });

  function talFormUpdatingIndicatorController($scope, $timeout) {

      $scope.errorMessage = $scope.errorMessage || 'Fields are in error, please correct before proceeding';

      $scope.statuses = {
          NONE: 'none',
          IN_PROGRESS: 'progress',
          SUCCESS: 'success',
          FAILURE: 'failure'
      };

      var successIndicatorTimeout;

      $scope.status = $scope.statuses.NONE;

      $scope.$watch('promise', function(newVal) {
          if (!newVal) {
              return;
          }

          $scope.status = $scope.statuses.IN_PROGRESS;
          $timeout.cancel(successIndicatorTimeout);

          $scope.promise.then(function(){
              $scope.status = $scope.statuses.SUCCESS;

              successIndicatorTimeout = $timeout(function(){
                  $scope.status = $scope.statuses.NONE;
              }, 1300);

          }).catch(function() {
              $scope.status = $scope.statuses.FAILURE;
          });
      });

  }

  module.controller('talFormUpdatingIndicatorController', talFormUpdatingIndicatorController );
  talFormUpdatingIndicatorController.$inject = ['$scope', '$timeout'];

})(angular.module('salesPortalApp'));

