(function(module) {
	'use strict';

	function CoverCalcResultsController($scope, ngDialog, $log) {
		$scope.useTheseValues = function() {
			$log.debug('useTheseValues');
			ngDialog.closeAll();
		};

		$scope.showAssumptions = function() {
			$log.debug('show assumptions');
			$scope.assumptionsAreVisisble = true;
		};

		$scope.showResults = function() {
			$log.debug('show results');
			$scope.assumptionsAreVisisble = false;
		};
	}

	CoverCalcResultsController.$inject = ['$scope', 'ngDialog', '$log'];

	function CoverCalcQuestionsCtrl($scope, $rootScope, ngDialog, $window) {
		$scope.openCoverCalc = function () {
		  ngDialog.open({
		    templateUrl: 'client/styleguide/sections/coverCalcQuestions/coverCalcQuestions.questions.html',
		    className: ''
		  });
			$window.scrollTop = 0;
		};

		$rootScope.$on('cc-questions-complete', function() {
			ngDialog.closeAll();

			ngDialog.open({
				templateUrl: 'client/styleguide/sections/coverCalcQuestions/coverCalcQuestions.confirm.html',
				controller: CoverCalcResultsController,
				className: '',
				closeByNavigation: false,
				closeByEscape: false,
				closeByDocument: false,
			});
			$window.scrollTop = 0;
		});
	}

	CoverCalcQuestionsCtrl.$inject = ['$scope', '$rootScope', 'ngDialog', '$window'];

	module.controller('CoverCalcQuestionsCtrl', CoverCalcQuestionsCtrl);

}(angular.module('styleguide')));