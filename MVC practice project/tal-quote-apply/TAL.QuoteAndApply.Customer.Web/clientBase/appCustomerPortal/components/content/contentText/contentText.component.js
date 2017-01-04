(function(module){
'use strict';
  module.directive('talContentText', function () {
      return {
        templateUrl: '/client/appCustomerPortal/components/content/contentText/contentText.template.html',
        restrict: 'E',
        replace: true,
        scope: {
            textReference:'@'
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talContentTextController'
      };
    });

  function talContentTextController(talContentService) {
    this.textContent = talContentService.getCompiledContent(this.textReference);
  }

  module.controller('talContentTextController', talContentTextController );
  talContentTextController.$inject = ['talContentService'];

})(angular.module('appCustomerPortal'));
