'use strict';

angular.module('appSelectCover', [
    'angular-intro',
    'appCustomerPortal',
    'oc.lazyLoad'
])
   .run(['$ocLazyLoad', 'viewBagConfig', function ($ocLazyLoad, viewBagConfig) {
       if (viewBagConfig.isCalculatorEnabled === true) {
           $ocLazyLoad.inject('coverCalculator');
       }
   }]);
