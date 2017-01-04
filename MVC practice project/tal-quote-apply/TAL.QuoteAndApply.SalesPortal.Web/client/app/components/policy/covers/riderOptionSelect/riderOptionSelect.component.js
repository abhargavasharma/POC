(function(module){
'use strict';
  module.directive('talRiderOptionSelect', function () {
      return {
        templateUrl: '/client/app/components/policy/covers/riderOptionSelect/riderOptionSelect.template.html',
        restrict: 'E',
          scope: {
              planModel: '=',
              optionModel: '=',
              planIndex: '=',
              optionIndex: '=',
              optionHasChanged: '&'
          },
        controller: 'talRiderOptionSelectController'
      };
    });

  function talRiderOptionSelectController($scope) {
      $scope.optionSelectedValidationModel = $scope.planModel.code.toLowerCase() + $scope.optionModel.name.replace(/ /g, '') + 'Option';

      $scope.optionChanged = function(){
          $scope.planModel.selected = true;
          $scope.optionHasChanged();
      };
  }

  module.controller('talRiderOptionSelectController', talRiderOptionSelectController );
    talRiderOptionSelectController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

