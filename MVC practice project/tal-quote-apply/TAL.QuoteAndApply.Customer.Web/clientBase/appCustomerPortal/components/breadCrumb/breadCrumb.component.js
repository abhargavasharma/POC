(function(module){
    'use strict';
    module.directive('talBreadCrumb', function () {
        return {
            templateUrl: '/client/appCustomerPortal/components/breadCrumb/breadCrumb.template.html',
            restrict: 'E',
            controller: 'breadCrumbController'
        };
    });

    function breadCrumbController() {
        
    }

    module.controller('breadCrumbController', breadCrumbController );
    breadCrumbController.$inject = [];

})(angular.module('appCustomerPortal'));
