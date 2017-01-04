'use strict';
angular.module('salesPortalApp').directive('talPaging', function () {
    return {
        templateUrl: '/client/app/components/paging/paging.template.html',
        restrict: 'E',
        scope: true,
        controllerAs: 'ctrl',
        bindToController: {
            totalRecords: '=',
            currentPage: '=',
            pageCount: '=',
            onNextPage: '&',
            onPreviousPage: '&'
        },
        controller: 'talPagingController'
    };
});

angular.module('salesPortalApp').controller('talPagingController',
    function () {
        var ctrl = this;

        ctrl.hasPreviousPage = function(){
            return ctrl.currentPage > 1;
        };

        ctrl.hasNextPage = function(){
            return ctrl.currentPage < ctrl.pageCount;
        };
    }
);
