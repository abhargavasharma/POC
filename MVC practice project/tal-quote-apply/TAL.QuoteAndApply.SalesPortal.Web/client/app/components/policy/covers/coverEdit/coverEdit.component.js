
(function(module){
    'use strict';
    module.directive('talCoverEdit', function () {
        return {
            templateUrl: '/client/app/components/policy/covers/coverEdit/coverEdit.template.html',
            restrict: 'E',
            scope: {
                planCover: '=',
                planIndex: '=',
                isRider: '=',
                updatePlan: '&',
                premiumFrequency: '=',
                premiumType: '='
            },
            controller: 'talCoverEditController'
        };
    });

    function talCoverEditController($scope) {

        $scope.coverAmountChanged = function(){
            $scope.planCover.selected = true;
            $scope.updatePlan();
        };

        $scope.coverHasChanged = function(){
            $scope.updatePlan();
        };

        $scope.optionHasChanged = function () {
            $scope.updatePlan();
        };

        $scope.coverAmountValidationModel = $scope.planCover.code.toLowerCase() + 'CoverAmount';

        //todo when rating factors changes make coverReadOnly false
        $scope.coverReadOnly = $scope.planCover.coverAmount;
    }

    module.controller('talCoverEditController', talCoverEditController );
    talCoverEditController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));