(function(module) {
	'use strict';

	//@TODO: 
	//		For UNIT testing, make methods public (with _)
	//		Underscore wrapped in angular constant

	/**
	 * This service provides a method, on(), which allows clients to subscribe for
	 * notifications of breakpoint event changes. The breakpoint event comes
	 * equipped with methods for querying matches,
	 * and seeing if we have entered or left a bp range.
	 */
	function bpChangeEventService(enquire) {
		 	/**
			* Descriptions and media queries for all breakpoints.
			*/
			var CONFIG = {
				0: {query: 'screen and (max-width:358px)', min: 0, max: 358 },
				xxs: {query: 'screen and (min-width: 359px) and (max-width:479px)', min: 359, max: 479 },
				xs: {query: 'screen and (min-width: 480px) and (max-width:639px)', min: 480, max: 639 },
				s: {query: 'screen and (min-width: 640px) and (max-width:769px)', min: 640, max: 769 },
				m: {query: 'screen and (min-width: 768px) and (max-width:1023px)', min: 768, max: 1023 },
				l: {query: 'screen and (min-width: 1024px) and (max-width:1243px)', min: 1024, max: 1243 },
				xl: {query: 'screen and (min-width: 1244px) and (max-width:1409px)', min: 1244, max: 1409 },
				xxl: {query: 'screen and (min-width: 1410px)', min: 1411 },
			};

			var KEYS = Object.keys(CONFIG);

			var previousBP;

			angular.forEach(KEYS, function(bp) {
				if (window.matchMedia(CONFIG[bp].query)) {
					previousBP = bp;
				}
			});



			/**
			 * Check if the given bp is in the given bp range.
			 */
			function isIn(bp, bpRange) {
				var satisfiesLower = CONFIG[bpRange.lower].min <= CONFIG[bp].min,
					satisfiesUpper = true;

				if (bpRange.upper) {
					satisfiesUpper = CONFIG[bp].min <= CONFIG[bpRange.upper].min;
				}

				return satisfiesLower && satisfiesUpper;
			}

			/**
			 * Check if we have entered a bp range.
			 *
			 * @method     hasEntered
			 * @param      {<type>}  bp       { description }
			 * @param      {<type>}  bpRange  { description }
			 */
			function hasEntered(bpOld, bpNew, bpRange) {
				if (bpOld) {
					return !isIn(bpOld, bpRange) && isIn(bpNew, bpRange);
				} else {
					return isIn(bpNew, bpRange);
				}
			}

			/**
			 * Check if we have left a bp range.
			 */
			function hasLeft(bpOld, bpNew, bpRange) {
				if (bpOld) {
					return isIn(bpOld, bpRange) && !isIn(bpNew, bpRange);
				} else {
					return !isIn(bpNew, bpRange);
				}
			}

			/**
			 * Take a bp expression of the form 's, m', and return the upper and lower bounds as bp keys.
			 *
			 * @method     parseBPExpression
			 * @param      {string}  bpExpression  A breakpoint expression, such as 's', '0,s', or 's, m'.
			 * @return     {Object}  A validated pair of breakpoints
			 */
			function parseBPExpression(bpExpression) {
				var bounds = bpExpression.replace(/ /g,'').split(',');

				// Validate
				if (KEYS.indexOf(bounds[0]) === -1) {
					console.warn('Invalid expression used with bpChangeEventService.');
					return { lower: 0, upper: null };
				}

				if (bounds[1] && KEYS.indexOf(bounds[1]) === -1) {
					console.warn('Invalid expression used with bpChangeEventService.');
					return { lower: 0, upper: null };
				}

				return { lower: bounds[0], upper: bounds[1] };
			}

			/**
			 * Constructor function for creating new breakpoint change events
			 *
			 * @class
			 * @param      {string}  newBp   The key of breakpoint. e.g. 'xs', or 'l'
			 */
			function BreakpointChangeEvent(newBp, prevBp) {
				this.newBp = newBp;
				this.prevBp = prevBp;

				this.name = newBp;
			}

			BreakpointChangeEvent.prototype.isIn = function(bpRange) {
				return isIn(this.newBp, parseBPExpression(bpRange));
			};

			BreakpointChangeEvent.prototype.hasEntered = function(bpRange) {
				return hasEntered(this.prevBp, this.newBp, parseBPExpression(bpRange));
			};

			BreakpointChangeEvent.prototype.hasLeft = function(bpRange) {
				return hasLeft(this.prevBp, this.newBp, parseBPExpression(bpRange));
			};


			/**
			 * A list of all registered callbacks (subscribers).
			 *
			 * @type       {Array}
			 */
			var subscriberCallbacks = [];

			/**
			 * Register a callback which will fire whenever publishChange is called.
			 *
			 * @method     subscribe
			 * @param      {Function}  callback  The callback to register.
			 * @param      {Function}  initCallback  A callback to fire immediately, if there is
			 * 											init which needs to be carried out.
			 */
			function subscribe(callback) {
				subscriberCallbacks.push(callback);
				callback(new BreakpointChangeEvent(previousBP));
				// @TODO return an unsubscribe function.
			}

			/**
			 * The trigger method for setting off all registered callbacks.
			 *
			 * @method       publishChange
			 */
			function publishChange(e) {
				subscriberCallbacks.forEach(function(thisCallback) {
					thisCallback(e);
				});
			}

			/**
			 * Sets up listeners for all breakpoint change events.
			 * This will only ever run once.
			 * 
			 * @method     addWatchers
			 */
			function addWatchers() {
				if (addWatchers.hasRun) { return; }
				addWatchers.hasRun = true;

				KEYS.forEach(function(key) {
					enquire.register(CONFIG[key].query, {
						match: _.throttle(function() {
							publishChange(new BreakpointChangeEvent(key, previousBP));
							previousBP = key;
						}, 20)
					});
				});
			}

			addWatchers();

		this.on = subscribe;
	}

	bpChangeEventService.$inject = ['enquire'];

	module.service('bpChangeEvent', bpChangeEventService);

}(angular.module('appCustomerPortal')));