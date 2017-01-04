(function (module) {
  'use strict';

  	/**
  	 * The directive class.
  	 */
	function animateHeightDirective(bpChangeEvent, $timeout) {

		/**
		 * Attaches the height scaling behavior to tabs.
		 *
		 * @method     setupHeightScaling
		 * @param      {jsLite element}  element  The directive element.
		 */
		function setupHeightScaling(element, listeners) {

			//var elTabs = element[0].querySelector('.tal-tabs');
			var elContent = element[0].querySelector('.tal-tabs__content:not(.ng-scope)');
			var elsRegions= element[0].querySelectorAll('.tal-tabs__pane-inner:not(.ng-scope)');

			if (!angular.element(elContent).scope()) { return; }

			var newHeight = 0;
			if(element[0].hasAttribute('is-payment-section')) {
				elContent.style.opacity = 0; //Added opacity because tabs__content will be visible before animation which doesn't look good - particularly for the purchase page payment section
			}

			$timeout(function() {
				angular.forEach(elsRegions, function(region) {
					if (region.offsetHeight > newHeight) {
						newHeight = region.offsetHeight;
					}
				});

				// Update height, unless it's zero.
				if (newHeight > 0) {
					elContent.style.height = newHeight + 'px';
				}
			}, 250);

			var unsub = angular.element(elContent).scope().$on('tab-selected', function(e, targetIndex) {
				$timeout(function() {
					var newHeight = elsRegions[targetIndex].offsetHeight;
					if(newHeight > 0) {
						elContent.style.height = newHeight + 'px';
						elContent.style.opacity = 1;
					}
				}, 250); // This delay is mirrored by a css animation transition-delay in the less;
			});

			listeners.push(unsub);
		}

		/**
		 * Destroys all traces of the height scaling behavior.
		 *
		 * @method     destroyHeightScaling
		 * @param      {jsLite element}  element  The directive element.
		 */
		function destroyHeightScaling(element, listeners) {
			var elContent = element[0].querySelector('.tal-tabs__content');

			$timeout(function() {
				elContent.style.height = 'auto';
			});

			angular.forEach(listeners, function(unsub) {
				unsub();
			});
		}

		/**
		 * The directive link function.
		 */
		function animateHeightLink(scope, element) {
			var listeners = [];

			bpChangeEvent.on(function(bp) {
				// Post init changes
				if (bp.hasEntered('m')) {
					$timeout(function() {
						setupHeightScaling(element, listeners);
					});
				} else if (bp.hasLeft('m')) {
					$timeout(function() {
						destroyHeightScaling(element, listeners);
						listeners = [];
					});
				}
			});
		}

		/**
		 * The directive controller.
		 */
		// function animateHeightController($scope) {
		// 	// Add window resize to digest cycle
		// 	angular.element($window).bind('resize', function() {
		// 		console.log('resize');
		// 		return $scope.$apply();
		// 	});
		// }

		/**
		 * The directive definition.
		 */
		return {
		  restrict: 'A',
		  require: 'talTabs',
		  link: animateHeightLink,
		  //controller: animateHeightController,
		};
	}

	animateHeightDirective.$inject = ['bpChangeEvent', '$timeout'];

	module.directive('talAnimateHeight', animateHeightDirective);

})(angular.module('appCustomerPortal'));