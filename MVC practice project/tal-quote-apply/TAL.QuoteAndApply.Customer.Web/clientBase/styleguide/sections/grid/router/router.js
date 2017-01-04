'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('grid', {
        url: '/grid',
        views: {
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/grid/grid.html');
            }],
            controller: 'GridCtrl'
          },
          'styleguideSidebarView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/index/index.html');
            }],
          }
        }
    });
   
  }]);