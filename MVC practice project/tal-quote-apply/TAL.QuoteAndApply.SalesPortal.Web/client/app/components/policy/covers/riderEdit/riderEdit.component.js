
(function(module){
    'use strict';
    module.directive('talRiderEdit', function () {
        return {
            templateUrl: '/client/app/components/policy/covers/riderEdit/riderEdit.template.html',
            restrict: 'E',
            scope: {
                planRider: '=',
                planIndex: '=',
                updatePlan: '&',
                premiumFrequency: '=',
                premiumType: '=',
                parentPlanDisabled: '=',
                availableDefinitions: '='
            },
            controller: 'talRiderEditController'
        };
    });

    function talRiderEditController($scope) {
        $scope.planRider.planDisabled = ($scope.planRider.disabled || $scope.parentPlanDisabled);

        $scope.planRider.open = $scope.planRider.selected;
        $scope.coverAmountChanged = function(){
            $scope.planRider.selected = true;
            $scope.updatePlan();
        };

        $scope.coverHasChanged = function(){
            $scope.updatePlan();
        };

        $scope.optionHasChanged = function () {
            $scope.updatePlan();
        };

        $scope.riderSelectValidationModel = $scope.planRider.code.toLowerCase();
        $scope.coverAmountValidationModel = $scope.planRider.code.toLowerCase() + 'CoverAmount';

        //todo when rating factors changes make coverReadOnly false
        $scope.coverReadOnly = $scope.planRider.coverAmount;
    }

    module.controller('talRiderEditController', talRiderEditController );
    talRiderEditController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));