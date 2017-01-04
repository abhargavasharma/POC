(function(module){
    'use strict';
    module.directive('talCard', function () {
        return {
            templateUrl: '/client/appCustomerPortal/components/card/card.template.html',
            restrict: 'E',
            replace: true,
            transclude: true,
            controller: 'cardController'
        };
    });

    function cardController() {
    }

    module.controller('cardController', cardController );
    cardController.$inject = [];

})(angular.module('appCustomerPortal'));
