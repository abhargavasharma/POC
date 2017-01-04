(function(module){
'use strict';
  module.directive('talLoading', function () {
      return {
        templateUrl: '/client/appReview/components/reviewForm/loading/loading.template.html',
        restrict: 'E',
        scope: {
          loading:'=',
          paymentFrequency: '='
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talLoadingController'
      };
    });

  function talLoadingController() {
  }

  module.controller('talLoadingController', talLoadingController );
  talLoadingController.$inject = [];

})(angular.module('appReview'));
