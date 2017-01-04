'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('cover-calc-confirm', {
        url: '/cover-calc-confirm',
        views: {
          'styleguideFullView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/coverCalcConfirm/coverCalcConfirm.html');
            }],
            controller: 'CoverCalcConfirmCtrl'
          }
        }
    });
  }]);
