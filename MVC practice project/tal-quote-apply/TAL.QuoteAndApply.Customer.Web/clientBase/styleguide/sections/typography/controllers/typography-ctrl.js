'use strict';

angular.module('styleguide')
  .controller('TypographyCtrl', function ($scope) {
		$scope.ctrl = this;
    $scope.ctrl.increment = function(){
        $scope.data.index ++ ;
    };
    $scope.data = {
      index : 0
    };
  });