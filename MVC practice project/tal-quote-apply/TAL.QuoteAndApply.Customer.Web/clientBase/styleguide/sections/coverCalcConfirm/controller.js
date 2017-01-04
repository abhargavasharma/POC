'use strict';

angular.module('styleguide')
	.controller('CoverCalcConfirmCtrl', ['$scope', function($scope, $log) {

		$scope.onConfirmMajorNotification = function() {
			$log.debug('Confirm');
		};

		$scope.onDismissMajorNotification = function() {
			$log.debug('Dismiss');
		};
	}]);