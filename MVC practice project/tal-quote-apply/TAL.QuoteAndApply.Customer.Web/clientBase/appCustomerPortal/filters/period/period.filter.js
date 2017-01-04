(function(module){
  'use strict';


  module.filter('period', function () {
    return function(input, type) {
      if(type === 'review' && input === 'monthly'){
        return 'month';
      } else if(type === 'inline' && input === 'monthly'){
        return 'm';
      } else if(type === 'inline' && input === 'annual'){
        return 'y';
      } else if(type === 'review' && input === 'annual'){
        return 'year';
      }
      
    };
  });

})(angular.module('appCustomerPortal'));
