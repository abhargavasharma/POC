(function(module){
'use strict';
  module.directive('talDeclaration', function () {
      return {
        templateUrl: '/client/appPurchase/components/declaration/declaration.template.html',
        restrict: 'E',
        scope: {
            declarationAgree: '=',
            dncSelection: '='
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talDeclarationController'
      };
  });

  function talDeclarationController(talContentService, $window) {
    var ctrl = this;
    ctrl.links = talContentService.getContentByReferenceKey('shared.links');
    ctrl.dncSelectionText = talContentService.getContentByReferenceKey('consent.dncSelection');
    ctrl.termsAgreeText = talContentService.getContentByReferenceKey('consent.termsAgree');

    ctrl.onUnderwritingSummary = function() {
      $window.open('/summary', '_blank');
    };
  }

  module.controller('talDeclarationController', talDeclarationController );
    talDeclarationController.$inject = ['talContentService', '$window'];

})(angular.module('appPurchase'));
