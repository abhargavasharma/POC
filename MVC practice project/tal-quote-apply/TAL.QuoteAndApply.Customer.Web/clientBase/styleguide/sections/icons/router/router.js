'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('icons', {
        url: '/icons',
        views: {
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/icons/icons.html');
            }],
            controller: 'IconsCtrl'
          },
          'styleguideSidebarView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/index/index.html');
            }],
          }
        }
    });
   
  }]);
