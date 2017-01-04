(function (module) {
  'use strict';

   /**
	* A directive which implements basic 'container queries',
    * for styling the content of a container based on the container's width.
    * Like media queries for individual elements.
	* 
	* It works by attaching classes to the container which say how wide it is.
    * These are used to scope other classes in the css,
    * allowing styling to be specific to container-width.
	*/

	function containerQueryDirective(bpChangeEvent) {
	 	/**
		* Descriptions and media queries for all breakpoints.
		*/
		var CONFIG = {
			xxs: { min: 0, max: 359 },
			xs: { min: 360, max: 480 },
			s: { min: 481, max: 640 },
			m: { min: 641, max: 768 },
			l: { min: 769, max: 1024 },
			xl: { min: 1025, max: 1244 },
			xxl: { min: 1245, max: 1410 },
		};
		
		/**
		 * Functions for measuring the container and updating classes
		 *
		 * @method     getContextClassType
		 * @param      {jqLite element}   element  The container we want to put classes on.
		 * @return     {string}  The context type: xxs, xs, s, m, l, xl, or xxl.
		 */
		function getContextClassType(element, isSelf) {
			var contextType = false;
			
			// TODO: Do we need to hard code the 40px?
			var theWidth = isSelf ? element[0].offsetWidth + 40 : element.parent()[0].offsetWidth + 40;

			angular.forEach(Object.keys(CONFIG), function(key) {
				if (CONFIG[key].min <= theWidth && theWidth < CONFIG[key].max ) {
					contextType = key;
				}
			});

			return contextType;
		}

		/**
		 * Remove all context classes off the given element.
		 *
		 * @method     removeContextClasses
		 * @param      {jqLite element}  element  The target element.
		 */
		function removeContextClasses(element) {
			angular.forEach(Object.keys(CONFIG), function(key) {
				element.removeClass('tal-cq-' + key);
			});
		}

		/**
		 * Add the correct context class to an element.
		 *
		 * @method     addContextClass
		 * @param      {jqLite element}  element  The target element.
		 */
		function addContextClass(element, isSelf) {
			element.addClass('tal-cq-' + getContextClassType(element, isSelf));
		}

		/**
		 * The directive link function.
		 */
		function containerQueryLink(scope, element, $attr) {
			var isSelf = $attr.talCq !== undefined && $attr.talCq === 'self';
			// Add the classes the first time
			addContextClass(element, isSelf);

			bpChangeEvent.on(function() {
				removeContextClasses(element);
				addContextClass(element, isSelf);
			});
		}

		/**
		 * The directive definition.
		 */
		return {
		  restrict: 'A',
		  link: containerQueryLink
		};
	}

	containerQueryDirective.$inject = ['bpChangeEvent'];

	module.directive('talCq', containerQueryDirective);

})(angular.module('appCustomerPortal'));