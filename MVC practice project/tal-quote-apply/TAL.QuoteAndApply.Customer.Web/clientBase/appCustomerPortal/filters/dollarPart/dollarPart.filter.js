(function(module){
  'use strict';

  /**
   * Returns just the dollar part of a number (as an integer).
   * 
   * e.g. 12.45 -> 12
   * e.g. 10.50 -> 10
   * e.g. 0.20 -> 0
   */
  module.filter('dollarPart', function () {
    return function(input) {
      return Math.floor(input);
    };
  });

})(angular.module('appCustomerPortal'));
