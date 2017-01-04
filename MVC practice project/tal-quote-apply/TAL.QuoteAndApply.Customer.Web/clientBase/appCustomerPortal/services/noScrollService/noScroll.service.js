(function(module) {
	'use strict';

	/**
	 * Allows for the ability to disable/enable scrolling of the page
	 *
	 * @namespace noscroll
	 * @version 1.0.0
	 * @author Deloitte Digital Australia deloittedigital@deloitte.com.au
	 */

	// TODO: extract window and document as mockable constants

	function noScrollFactory() {
	 	var CLASSES,
			elBody = document.getElementsByTagName('body')[0],
			add,
			remove,
			refresh;

	 	CLASSES = {
	 	  NO_SCROLL: 'no-scroll'
	 	};

	 	/**
	 	 * Stops the page from scrolling
	 	 */
	 	add = function() {
	 	  if (elBody.classList.contains(CLASSES.NO_SCROLL) === false) {
	 	    var scrollTop = window.scrollY;
	 	    elBody.classList.add(CLASSES.NO_SCROLL);
	 	    elBody.style.top = -scrollTop + 'px';
	 	  }
	 	};

	 	/**
	 	 * Allows the page to start scrolling again
	 	 */
	 	remove = function() {
	 	  if (elBody.classList.contains(CLASSES.NO_SCROLL)) {
	 	    var top = parseInt(elBody.style.top, 10);

	 	    elBody.style.top = '';
	 	    elBody.classList.remove(CLASSES.NO_SCROLL);

	 	    window.scrollTo(0, -top);
	 	  }
	 	};

	 	/**
	 	 * On change of the scroll height of the page, update the scroll position
	 	 */
	 	refresh = function() {
	 	  if (elBody.classList.contains(CLASSES.NO_SCROLL)) {
	 	    // If the page has gotten shorter, make sure we aren't scrolled past the footer
	 	    if (elBody.offsetHeight > window.offsetHeight) {
	 	      if (elBody.offsetTop - window.offsetHeight < -elBody.offsetHeight) {
	 	        elBody.style.top = -(elBody.offsetHeight - window.offsetHeight);
	 	      }
	 	    }
	 	  }
	 	};

	 	return {
	 	  add: add,
	 	  remove: remove,
	 	  refresh: refresh
	 	};
	}

	module.factory('noScroll', noScrollFactory);

}(angular.module('appCustomerPortal')));