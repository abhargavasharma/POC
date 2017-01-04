(function(module) {
	'use strict';

	/**
	 * A service for controlling the Page level Spinner.
	 *
	 * As well as showing and hiding the spinner, it also locks the main window scroll.
	 */
	function pageSpinnerService(spinnerService, noScroll) {
	 	this.start = function start() {
	 		noScroll.add();
	 		spinnerService.show('pageSpinner');
	 	};

	 	this.stop = function stop() {
	 		spinnerService.hide('pageSpinner');
	 		noScroll.remove();
	 	};
	}

	pageSpinnerService.$inject = [
		'spinnerService',
		'noScroll'
	];

	module.service('pageSpinnerService', pageSpinnerService);

}(angular.module('appCustomerPortal')));