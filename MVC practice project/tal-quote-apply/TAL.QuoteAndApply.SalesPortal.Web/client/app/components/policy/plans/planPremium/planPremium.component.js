
(function(module){
    'use strict';
    module.directive('talPlanPremium', function () {
        return {
            templateUrl: '/client/app/components/policy/plans/planPremium/planPremium.template.html',
            restrict: 'E',
            scope: {
                premium: '=',
                premiumFrequency: '='
            },
            controller: 'talPlanPremiumController'
        };
    });

    function talPlanEditController($scope) {

        var premiumFrequencyLabels = {
            Monthly: 'month',
            Quarterly: 'quarter',
            HalfYearly: 'half year',
            Yearly: 'year'
        };

        function setPremiumFrequencyLabel(){
            $scope.premiumFrequencyLabel = premiumFrequencyLabels[$scope.premiumFrequency];
        }

        setPremiumFrequencyLabel();
        $scope.$watch('premiumFrequency', setPremiumFrequencyLabel);
    }

    module.controller('talPlanPremiumController', talPlanEditController );
    talPlanEditController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));