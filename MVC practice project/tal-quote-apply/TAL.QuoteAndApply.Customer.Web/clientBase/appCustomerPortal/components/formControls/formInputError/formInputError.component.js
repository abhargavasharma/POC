(function(module){
    'use strict';
    module.directive('talFormInputError', function () {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/formInputError/formInputError.template.html',
            restrict: 'E',
            transclude: true,
            scope: {
                showServerErrorFor: '=',
                showErrorFor: '=',
                showErrorWhen: '@'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talFormInputErrorController'
        };
    });

    function talFormInputErrorController() {
    }

    module.controller('talFormInputErrorController', talFormInputErrorController );
    talFormInputErrorController.$inject = [];

})(angular.module('appCustomerPortal'));
