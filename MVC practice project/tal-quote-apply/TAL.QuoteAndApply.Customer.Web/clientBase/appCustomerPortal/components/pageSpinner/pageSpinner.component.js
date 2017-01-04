(function(module){
  'use strict';
  module.directive('talPageSpinner', function () {
    return {
      templateUrl: '/client/appCustomerPortal/components/pageSpinner/pageSpinner.template.html',
      restrict: 'E',
      controller: 'talPageSpinnerController',
      scope: {
        text: '='
      },
      controllerAs: 'vm',
      bindToController: true,
    };
  });

  var talPageSpinnerController = function() {
      this.text = this.text || 'Configuring quote';
  };

  module.controller('talPageSpinnerController', talPageSpinnerController);
  talPageSpinnerController.$inject = [];


})(angular.module('appCustomerPortal'));
