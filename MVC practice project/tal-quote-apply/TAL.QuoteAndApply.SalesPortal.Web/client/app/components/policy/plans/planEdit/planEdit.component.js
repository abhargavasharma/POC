
(function(module){
    'use strict';
    module.directive('talPlanEdit', function () {
        return {
            templateUrl: '/client/app/components/policy/plans/planEdit/planEdit.template.html',
            restrict: 'E',
            scope: {
                quoteReferenceNumber: '=',
                planDetail: '=',
                planIndex: '=',
                riskId: '=',
                updatePlan: '&',
                availableDefinitions: '='
            },
            controller: 'talPlanEditController'
        };
    });

    function talPlanEditController() {
    }

    module.controller('talPlanEditController', talPlanEditController );
    talPlanEditController.$inject = ['$scope', 'talSalesPortalApiService', 'talFormModelStateService'];

})(angular.module('salesPortalApp'));