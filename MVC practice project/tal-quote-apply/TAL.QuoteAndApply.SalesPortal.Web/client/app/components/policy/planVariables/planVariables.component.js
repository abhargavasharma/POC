
(function(module){
    'use strict';
    module.directive('talPlanVariables', function () {
        return {
        templateUrl: '/client/app/components/policy/planVariables/planVariables.template.html',
        restrict: 'E',
        scope: {
            planDetail: '=',
            updatePlan: '&',
            planIndex: '=',
            riderVariables: '=',
            availableDefinitions: '='
        },
        controller: 'talPlanVariablesController'
        };
    });

  function talPlanVariablesController($scope, FORM) {

      var isFeatureSupported = function(variableName) {

          var matchingVariable = _.find($scope.planDetail.variables, function(v) {
              return v.code === variableName;
          });

          if(matchingVariable) {
              return true;
          }

          return false;
      };

      $scope.submitPlanVariables = function () {
          $scope.planDetail.selected = true;
          $scope.updatePlan();
      };

      $scope.optionHasChanged = function () {
          $scope.updatePlan();
      };

      $scope.occupationDefinitionOptions = $scope.availableDefinitions || FORM.POLICY.OCCUPATION_DEFINITION_OPTIONS;

      $scope.linkedToCpiSupported = isFeatureSupported('linkedToCpi');
      $scope.premiumHolidaySupported = isFeatureSupported('premiumHoliday');
      $scope.premiumTypeSupported = isFeatureSupported('premiumType');
      $scope.waitingPeriodSupported = isFeatureSupported('waitingPeriod');
      $scope.benefitPeriodSupported = isFeatureSupported('benefitPeriod');
      $scope.occupationDefinitionSupported = isFeatureSupported('occupationDefinition') && $scope.occupationDefinitionOptions.length > 0;

      $scope.boolToStr = function(arg) {return arg ? 'Yes' : 'No';};
      $scope.premHolidayOptions = function(arg) {return arg ? 'Activated' : 'No';};

      $scope.waitingPeriods = FORM.POLICY.WAITING_PERIODS;
      $scope.benefitPeriods = FORM.POLICY.BENEFIT_PERIODS;
      
  }

  module.controller('talPlanVariablesController', talPlanVariablesController );
  talPlanVariablesController.$inject = ['$scope', 'FORM'];

})(angular.module('salesPortalApp'));