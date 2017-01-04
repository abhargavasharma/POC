(function(module){
'use strict';
  module.directive('talFormError', function () {
      return {
        templateUrl: '/client/app/components/formError/formError.template.html',
        restrict: 'E',
        scope: {
            ngFormModel: '=',
            validationError: '='
        },
        controller: 'talFormErrorController'
      };
    });

  function talFormErrorController() {

  }

  module.controller('talFormErrorController', talFormErrorController );
  talFormErrorController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

