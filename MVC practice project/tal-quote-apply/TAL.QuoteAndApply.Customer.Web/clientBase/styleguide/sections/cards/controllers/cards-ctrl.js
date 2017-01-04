'use strict';

angular.module('styleguide')
  .controller('CardsCtrl', ['$scope', function($scope) {
    $scope.ctrl = this;
    $scope.ctrl.increment = function(){
        $scope.data.index ++ ;
    };
  	$scope.data = {
     index : 0,
  	 isOn:true,
  	 select : {
  	   value:1000000,
  	   selectItems : [500000, 1000000]
  	 }
  	};
  }]);