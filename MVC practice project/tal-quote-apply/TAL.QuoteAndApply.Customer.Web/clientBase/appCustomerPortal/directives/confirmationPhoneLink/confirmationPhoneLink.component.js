(function (module) {
    'use strict';
    module.directive('talConfirmationPhoneLink', function () {
        return {
            restrict: 'E',
            scope: {
                chatAvailability: '='
            },
            transclude: true,
            templateUrl: '/client/appCustomerPortal/directives/confirmationPhoneLink/confirmationPhoneLink.template.html',
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talConfirmationPhoneLinkController'
        };
    });

    function talConfirmationPhoneLinkController(talContentService) {
        var ctrl = this;
        ctrl.contact = talContentService.getContentByReferenceKey('shared.contact');
    }

    module.controller('talConfirmationPhoneLinkController', talConfirmationPhoneLinkController);
    talConfirmationPhoneLinkController.$inject = ['talContentService'];


}(angular.module('appCustomerPortal')));

