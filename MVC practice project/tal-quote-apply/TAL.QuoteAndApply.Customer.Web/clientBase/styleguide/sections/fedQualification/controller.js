'use strict';

angular.module('styleguide')
	.controller('FedQualificationCtrl', ['$scope', '$timeout', function ($scope, $timeout) {
		$scope.questionIndex = 0;

		$scope.questions = [0,1,2,3,4];

		$scope.nextQuestion = function() {
			$scope.reversed = false;
			$timeout(function() {
				$scope.questionIndex = ($scope.questionIndex + 1) % 2;
			});
		};

		$scope.prevQuestion = function() {
			$scope.reversed = true;
			$timeout(function() {
				$scope.questionIndex = ($scope.questionIndex + 1) % 2;
			});
		};
	}]);