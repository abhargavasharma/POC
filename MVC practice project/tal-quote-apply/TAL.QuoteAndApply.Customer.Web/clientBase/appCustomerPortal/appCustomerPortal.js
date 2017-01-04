'use strict';

angular.module('appCustomerPortal', [
    'cgBusy',
    'ui.utils',
    'ngSanitize',
    'ngAria',
    'countTo',
    'ngDialog',
    'angularRipple',
    'angularSpinners',
    'ngAnimate',
    'toastr',
    'ng-slide-down',
    'angular-intro',
    'ngResource',
    'ngTouch',
    'ui.select',
    'vcRecaptcha',
    'ngCookies'
])

    .value('cgBusyDefaults', {
        delay: 100,
        minDuration: 0,
        templateUrl: '/client/appCustomerPortal/components/loadingIndicator/loadingIndicator.template.html'
    })

    .config(['ngDialogProvider', function (ngDialogProvider) {
        ngDialogProvider.setDefaults({
            showClose: false,
            className: 'modal'
        });
    }])

    .config(['vcRecaptchaServiceProvider', function (vcRecaptchaServiceProvider) {
        vcRecaptchaServiceProvider.setSiteKey('6Lfr0CMTAAAAAN2m1jtNBVTRhhpc7Z-jYHV26Ium');
    }])

    .config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push(function ($q, $injector) {
            return {
                responseError: function (response) {
                    if (response.status === 401) {
                        //handle the case where the user is not authenticated
                        //Have to use $injector at this level to get service
                        $injector.get('talModalService').showTimeoutModal();
                    }
                    if (response.status === 500) {
                        //Have to use $injector at this level to get service
                        $injector.get('toastr').error('', 'An error has occurred');
                    }

                    return $q.reject(response);
                }
            };
        });
    }])

    .config(['$logProvider', 'viewBagConfig', function ($logProvider, viewBagConfig) {
        $logProvider.debugEnabled(viewBagConfig.debugEnabled);
    }])

    .run(['talBrowserService', function (talBrowserService) {
        talBrowserService.detectIe();
    }]);

