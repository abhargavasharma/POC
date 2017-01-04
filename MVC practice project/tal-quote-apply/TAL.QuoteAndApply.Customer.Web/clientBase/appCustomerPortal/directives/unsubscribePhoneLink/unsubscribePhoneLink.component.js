(function (module) {
    'use strict';
    module.directive('talUnsubscribePhoneLink', function () {
        return {
            restrict: 'E',
            scope: {
                chatAvailability: '='
            },
            transclude: true,
            templateUrl: '/client/appCustomerPortal/directives/unsubscribePhoneLink/unsubscribePhoneLink.template.html',
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talUnsubscribePhoneLinkController'
        };
    });

    function talUnsubscribePhoneLinkController(talContentService) {
        var ctrl = this;
        ctrl.contact = talContentService.getContentByReferenceKey('shared.contact');
    }

    module.controller('talUnsubscribePhoneLinkController', talUnsubscribePhoneLinkController);
    talUnsubscribePhoneLinkController.$inject = ['talContentService'];


}(angular.module('appCustomerPortal')));

