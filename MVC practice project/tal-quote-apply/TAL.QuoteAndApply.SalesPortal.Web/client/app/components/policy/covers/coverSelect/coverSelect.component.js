(function(module){
'use strict';
  module.directive('talCoverSelect', function () {
      return {
          templateUrl: '/client/app/components/policy/covers/coverSelect/coverSelect.template.html',
          restrict: 'E',
          scope: {
              planModel: '=',
              coverModel: '=',
              planIndex: '=',
              coverIndex: '=',
              isRider: '=',
              coverHasChanged: '&'
          },
          controller: 'talCoverSelectController'
      };
    });

  function talCoverSelectController($scope) {
      $scope.coverSelectedValidationModel = $scope.coverModel.code.toLowerCase().replace(/ /g,'');

      $scope.coverChanged = function(addedField){
          $scope.planModel.selected = addedField ? addedField : $scope.planModel.selected;
          $scope.coverHasChanged();
      };
  }

  module.controller('talCoverSelectController', talCoverSelectController);
  talCoverSelectController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

