'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('intro', {
        url: '/intro',
        views: {
          'styleguideFulView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/intro/intro.html');
            }],
            controller: 'IntroCtrl'
          }
        }
    });
   
  }]);