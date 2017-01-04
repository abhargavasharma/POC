(function (module) {
    'use strict';
    module.directive('talHeader', function () {
        return {
            templateUrl: '/client/appCustomerPortal/largeComponents/header/header.template.html',
            restrict: 'E',
            replace: true,
            scope: {
                hideQuotePremium: '=',
                hideQuoteReference: '=',
                hideNavigation: '=',
                hideMobileNavigation: '=',
                hideSaveButton:'=',
                hideChatLink:'=',
                quoteReferenceNumber: '@',
                savedStatus: '=',
                totalPremium: '=',
                premiumPeriod:'@',
                stepIndex: '='
            },
            controller: 'headerController as ctrl',
            bindToController: true
        };
    });


    function controller($scope, EVENT) {
        var ctrl = this;
        $scope.$on(EVENT.QUOTE.QUOTE_REF_NUMBER, function ($event, quoteRefNumber) {
            ctrl.quoteReferenceNumber = quoteRefNumber;
        });

        $scope.$on(EVENT.QUOTE.SHOW_QUOTE_REF_NUMBER, function () {
            ctrl.hideQuoteReference = false;
        });
    }

    module.controller('headerController', controller);
    controller.$inject = ['$scope', 'EVENT'];

})(angular.module('appCustomerPortal'));
