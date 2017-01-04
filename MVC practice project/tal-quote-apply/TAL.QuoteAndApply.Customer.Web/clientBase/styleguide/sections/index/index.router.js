'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('styleguide', {
        url: '/styleguide',
        views: {
          'styleguideSidebarView': {
            templateProvider: ['$templateCache', function($templateCache) { 
              return $templateCache.get('client/styleguide/sections/index/index.html');
            }],
            controller: 'IntroCtrl'
          },
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/index/readme.html');
            }]
          },
        }
    });
   
  }]);