(function(module){
  'use strict';

  /**
   * Updates all the items to use the dollar format exect for custom and select.
   * 
   * e.g. 12.45 -> 12
   * e.g. 10.50 -> 10
   * e.g. 0.20 -> 0
   */
  module.filter('dollarsDropdown', ['$filter',function ($filter) {
    return function(input) {
      if(isNaN(input)){
        return input;
      } else {
        return $filter('currency')(input, '', 0);
      }
    };
  }]);

})(angular.module('appCustomerPortal'));
