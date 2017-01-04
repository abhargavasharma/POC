'use strict';

angular.module('appNeedsAnalysis', [
        'appCustomerPortal',
        'ui.router'
    ])
    .run(function () {
    })
    .config(function ($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('help', {
                url: '/help/:index',
                params: {
                    getQuoteComplete: null,
                    basicInfo: null,
                    tryContinueExistingQuote: true
                },
                templateUrl: '/client/appNeedsAnalysis/components/needsAnalysis/needsAnalysis.master.template.html'
            })
            .state('quote', {
                url: '/quote/:index',
                params: {
                    helpMeChooseComplete: null,
                    tryContinueExistingQuote: false
                },
                templateUrl: '/client/appNeedsAnalysis/components/needsAnalysis/needsAnalysis.master.template.html'
            });

        $urlRouterProvider.otherwise('/quote/');
    });
