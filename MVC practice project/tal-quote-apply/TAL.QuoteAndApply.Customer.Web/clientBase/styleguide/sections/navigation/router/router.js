'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('navigation', {
        url: '/navigation',
        views: {
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/navigation/navigation.html');
            }],
            controller: 'NavigationCtrl'
          },
          'styleguideSidebarView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/index/index.html');
            }],
          }
        }
    });
   
  }]);
