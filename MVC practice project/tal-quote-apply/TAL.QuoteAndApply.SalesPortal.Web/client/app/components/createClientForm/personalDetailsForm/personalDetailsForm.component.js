(function(module){
'use strict';
  module.directive('talPersonalDetailsForm', function () {
      return {
        templateUrl: '/client/app/components/createClientForm/personalDetailsForm/personalDetailsForm.template.html',
        restrict: 'E',
        scope: {
            personalDetails:'='
        },
        controller: 'talPersonalDetailsFormController'
      };
    });

  function talPersonalDetailsFormController($scope, FORM) {
      $scope.titles = FORM.PERSONAL_DETAILS.TITLES;
      $scope.states = FORM.ADDRESS.STATES;
  }

  module.controller('talPersonalDetailsFormController', talPersonalDetailsFormController );
  talPersonalDetailsFormController.$inject = ['$scope', 'FORM'];

})(angular.module('salesPortalApp'));

