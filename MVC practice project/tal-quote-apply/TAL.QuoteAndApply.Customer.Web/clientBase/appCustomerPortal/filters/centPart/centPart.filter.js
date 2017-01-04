(function(module){
  'use strict';

  /**
   * Returns just the cent part of a number (as an integer).
   * 
   * e.g. 12.45 -> 45
   * e.g. 10.50 -> 50
   * e.g. 10.00 -> 00
   * 
   * Using numbers (floats) may cause rounding errors,
   * so instead we'll use string manipulation.
   */
  module.filter('centPart', function () {
    return function(input) {
      var cleanValue = input;

      // Convert edge cases:
      // e.g. 2 -> 2.00
      // e,g, 2.4 -> 2.50
      cleanValue += (String(input).indexOf('.') === -1) ? '.00' : '00';

      // return the two digits after the decimal point
      return /\.(\d{2})/.exec(cleanValue)[1];
    };
  });

})(angular.module('appCustomerPortal'));
