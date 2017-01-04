(function (module) {
    'use strict';
    module.directive('talFooter', function () {
        return {
            templateUrl: '/client/appCustomerPortal/largeComponents/footer/footer.template.html',
            restrict: 'E',
            replace: true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talFooterController'
        };
    });


    var talFooterController = function (talContentService) {
        var ctrl = this;
        ctrl.links = talContentService.getContentByReferenceKey('shared.links');
    };

    module.controller('talFooterController', talFooterController);

})(angular.module('appCustomerPortal'));
