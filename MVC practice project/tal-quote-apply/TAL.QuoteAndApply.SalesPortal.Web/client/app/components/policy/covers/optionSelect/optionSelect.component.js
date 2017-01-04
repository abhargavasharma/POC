(function(module){
'use strict';
  module.directive('talOptionSelect', function () {
      return {
        templateUrl: '/client/app/components/policy/covers/optionSelect/optionSelect.template.html',
        restrict: 'E',
          scope: {
              planModel: '=',
              optionModel: '=',
              planIndex: '=',
              optionIndex: '=',
              isRider: '=',
              optionHasChanged: '&'
          },
        controller: 'talOptionSelectController'
      };
    });

  function talOptionSelectController($scope) {
      $scope.optionSelectedValidationModel = $scope.planModel.code.toLowerCase() + $scope.optionModel.code;

      $scope.optionChanged = function(){
          $scope.planModel.selected = true;
          $scope.optionHasChanged();
      };

      $scope.boolToStr = function(arg) {return arg ? 'Yes' : 'No';};
  }

  module.controller('talOptionSelectController', talOptionSelectController );
  talOptionSelectController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

