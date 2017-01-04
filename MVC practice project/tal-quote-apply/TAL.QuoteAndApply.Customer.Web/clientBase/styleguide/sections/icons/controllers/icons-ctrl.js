'use strict';

angular.module('styleguide')
  .controller('IconsCtrl', ['$scope', function ($scope) {
		$scope.ctrl = this;
	    $scope.ctrl.increment = function(){
	        $scope.data.index ++ ;
	    };
	    
			$scope.data = {
	      index : 0
	    };
	    
		$scope.talPEIcons = [
			'tal-logo',
			'cross',
			'tick',
			'plus',
			'minus',
			'arrow-n',
			'arrow-w',
			'arrow-e',
			'arrow-s',
			'caret-n',
			'caret-s',
			'caret-e',
			'caret-w',
			'chat',
			'phone',
			'mail'
		];
  }]);