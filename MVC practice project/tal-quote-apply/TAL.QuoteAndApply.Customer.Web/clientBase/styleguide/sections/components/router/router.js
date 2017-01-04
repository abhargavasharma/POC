'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('components', {
        url: '/components',
        views: {
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/components/components.html');
            }],
            controller: 'ComponentsCtrl'
          },
          'styleguideSidebarView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/index/index.html');
            }],
          }
        }
    });
   
  }]);
