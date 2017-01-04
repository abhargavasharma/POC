'use strict';

angular.module('styleguide')
  .controller('ComponentsCtrl', ['$scope', '$timeout', 'toastr', '$q', 'pageSpinnerService', function($scope, $timeout, toastr, $q, pageSpinnerService) {

    $scope.ctrl = this;

    $scope.ctrl.increment = function() {
        $scope.data.index ++ ;
    };

    $scope.data = {
      index : 0
    };

    $scope.startGlobalSpinner = function() {
      pageSpinnerService.start();

      $timeout(function() {
        pageSpinnerService.stop();
      }, 2500);
    };

    $scope.startLocalSpinner = function() {
      $scope.showLocalSpinner = $q.defer();

      $timeout(function() {
        $scope.showLocalSpinner.resolve();
      }, 2500);
    };

    $scope.showToastSuccess = function() {
      toastr.success('Success!', 'Well done.');
    };

    $scope.showToastError = function() {
      toastr.error('Error!', 'How did that happen?!');
    };

    $scope.showToastInfo = function() {
      toastr.info('Info!', 'Wasn\'t that interesting!');
    };

    $scope.showToastWarning = function() {
      toastr.warning('Warning!', 'Pay close attention!');
    };


    // -------- Progress bar ----------------------------------------------------------------
    
    $scope.progressBar1 = $scope.progressBar1 || {
      progress: 50
    };

  }]);