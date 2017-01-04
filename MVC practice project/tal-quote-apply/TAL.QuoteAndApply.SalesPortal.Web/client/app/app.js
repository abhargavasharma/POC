'use strict';

angular.module('salesPortalApp', [
    'ui.bootstrap.showErrors',
    'cgBusy',
    'ngResource',
    'ngSanitize',
    'ui.bootstrap',
    'ui.utils',
    'ngAnimate',
    'angularSpinners'
]).value('cgBusyDefaults', {
    delay: 0,
    minDuration: 0,
    templateUrl: '/client/app/components/loadingIndicator/loadingIndicator.template.html'
}).run(function () {
    console.log('salesPortalApp started');
}).config(function($httpProvider){
    $httpProvider.interceptors.push(function ($rootScope, $q, $window) {
        return {
            responseError: function (response) {

                switch (response.status) {
                case 403: //session timeout
                    $window.location = ('/home/login?timeout=true');
                    break;
                }

                return $q.reject(response);
            }
        };
    });
});
