'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('staging', {
        url: '/staging',
        views: {
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/staging/staging.html');
            }],
            controller: 'StagingCtrl'
          }
        }
    });
   
  }]);
