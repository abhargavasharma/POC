(function (module) {
  'use strict';

  	// @TODO: Lodash should be wrapped in an angular constant so that it's injectable.

  	/**
  	 * This directive lets you watch a region of the dom,
  	 * and fire a registered callback (highly throttled) on any subdom change.
  	 * 
  	 * Use sparingly. The only reason this exists is for resizing the tabs component
  	 * when it's used in a modal, and content changes arbitrarily (e.g. an error message is displayed)
  	 */

  	/**
  	 * The directive's link function.
  	 */
  	function link(scope, element) {
  		element[0].addEventListener('DOMSubtreeModified', _.throttle(function() {
  			if (typeof scope.onDomChangeHandler === 'function') {
  				scope.onDomChangeHandler();
  			}
  		}, 1500, {}, false));
  	}

  	/**
  	 * The directive definition.
  	 */
	function onDomChangeDirective() {
		return {
		  restrict: 'A',
		  link: link,
		  scope: {
		  	onDomChangeHandler: '='
		  }
		};
	}

	module.directive('onDomChange', onDomChangeDirective);

})(angular.module('appCustomerPortal'));