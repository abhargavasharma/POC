/**
 * A select dropdown which lets you choose between brands/themes for the UI, and updates it in-place.
 */
(function(module){
  'use strict';

  module.directive('sgThemeSelect', function () {
    return {
      templateUrl: '/client/styleguide/components/themeSelect/themeSelect.template.html',
      restrict: 'E',
      scope: {},
      controller: 'themeSelectController',
      controllerAs: 'ctrl',
      bindToController: true
    };
  });

  var themeSelectController = function(styleguideTheme, pageSpinnerService, $timeout) {
    var ctrl = this;

    ctrl.currentTheme = styleguideTheme.getTheme();
    ctrl.allThemes = styleguideTheme.getAllThemes();

    ctrl.updateTheme = function() {

      console.warn('The theme service will need to be updated to use the correct file paths, after the refactor Chris and Kirill performed.');

      pageSpinnerService.start();
      styleguideTheme.setTheme(ctrl.currentTheme);

      $timeout(pageSpinnerService.stop, 800);
    };
  };

  module.controller('themeSelectController', themeSelectController);
  themeSelectController.$inject = ['styleguideTheme', 'pageSpinnerService', '$timeout'];

})(angular.module('styleguide'));
