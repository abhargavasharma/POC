(function(module){
  'use strict';
  module.directive('talProgressBar', function () {
    return {
      templateUrl: '/client/appCustomerPortal/components/progressBar/progressBar.template.html',
      restrict: 'E',
      transclude: true,
      scope: {
        'progress': '=?',
        'total': '@?'
      },
      controller: 'progressBarController'
    };
  });

  /**
   * Clamps the value to be between 0 and 100.
   * @param {number} value The value to clamp.
   * @returns {number}
   */
  var clamp = function clamp(value) {
    return Math.max(0, Math.min(value || 0, 100));
  };

  var progressBarController = function($scope, $element) {
    $element.attr('aria-valuemin', 0);
    $element.attr('aria-valuemax', 100);
    $element.attr('role', 'progressbar');

    $scope.progress = $scope.progress || 0;
    $scope.total = $scope.total || 100;

    $scope.percentage = clamp(100 * $scope.progress / $scope.total);

    $scope.$watch('progress', function() {
      $scope.percentage = clamp(100 * $scope.progress / $scope.total);
      $element.attr('aria-valuenow', $scope.percentage);
    });
  };

  module.controller('progressBarController', progressBarController);

})(angular.module('appCustomerPortal'));
