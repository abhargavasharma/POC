'use strict';

angular.module('styleguide')
  .controller('GridCtrl', ['$scope', function ($scope) {
		$scope.ctrl = this;
    $scope.ctrl.increment = function(){
        $scope.data.index ++ ;
    };
    
		$scope.data = {
      index : 0
    };
		$scope.colSets = [
			[12],
			[1, 11],
			[2, 10],
			[3, 9],
			[4, 8],
			[5, 7],
			[6, 6],
			[4, 4, 4],
			[3, 3, 3, 3]
		];

		$scope.offsetSet = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
  }]);