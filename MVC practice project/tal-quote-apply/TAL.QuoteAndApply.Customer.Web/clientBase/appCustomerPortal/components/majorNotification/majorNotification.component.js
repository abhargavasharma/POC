(function(module){
	'use strict';

	// ==== Controller ======================================================================

	function majorNotificationController() {
		
	}

	majorNotificationController.$inject = [];
	module.controller('majorNotificationController', majorNotificationController);



	// ==== Directive ======================================================================

	module.directive('talMajorNotification', function () {
		return {
			templateUrl: '/client/appCustomerPortal/components/majorNotification/majorNotification.template.html',
			restrict: 'E',
			controller: majorNotificationController,
			controllerAs: 'vm',
			scope: {
				ctaText: '@',
				onCtaAction: '=',
				onDismiss: '=?',
			},
			bindToController: true,
			transclude: true,
		};
	});

})(angular.module('appCustomerPortal'));
