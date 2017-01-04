(function (module) {
    'use strict';
    module.directive('talPremiumType', function () {
        return {
            templateUrl: '/client/app/components/policy/covers/premiumType/premiumType.template.html',
            restrict: 'E',
            scope: {
                planDetail: '=',
                updatePlan:'&'
            },
            controller: 'talPremiumTypeController'
        };
    });

    function talPremiumTypeController($scope, FORM) {

        $scope.premiumTypes = FORM.COVER_SELECTION.PREMIUM_TYPES;
        $scope.premiumTypeValidationKey = $scope.planDetail.code.toLowerCase() + 'PremiumType';

        $scope.submitPlanPremiumType = function () {
            $scope.planDetail.selected = true;
            $scope.updatePlan();
        };
    }

    module.controller('talPremiumTypeController', talPremiumTypeController);
    talPremiumTypeController.$inject = ['$scope', 'FORM'];

})(angular.module('salesPortalApp'));

