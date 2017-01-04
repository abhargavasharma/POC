(function (module) {
    'use strict';
    module.directive('talPhoneNumberLink', function () {
        return {
            restrict: 'E',
            scope: {
                chatAvailability: '='
            },
            transclude: true,
            templateUrl: '/client/appCustomerPortal/directives/phoneNumberLink/phoneNumberLink.template.html',
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talPhoneNumberLinkController'
        };
    });

    function talPhoneNumberLinkController(talContentService) {
        var ctrl = this;
        ctrl.contact = talContentService.getContentByReferenceKey('shared.contact');
    }

    module.controller('talPhoneNumberLinkController', talPhoneNumberLinkController);
    talPhoneNumberLinkController.$inject = ['talContentService'];


}(angular.module('appCustomerPortal')));

