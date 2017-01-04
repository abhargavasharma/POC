'use strict';

angular.module('appBasicInfo', [
    'appCustomerPortal',
    'oc.lazyLoad'
])
    .run(['$ocLazyLoad', 'viewBagConfig', function ($ocLazyLoad, viewBagConfig) {
        if (viewBagConfig.isCalculatorEnabled === true) {
            $ocLazyLoad.inject('coverCalculator');
        }
    }]);