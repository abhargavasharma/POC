'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('modals', {
        url: '/modals',
        views: {
          'styleguideFullView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/modals/modals.html');
            }],
            controller: 'ModalsCtrl'
          }
        }
    });
   
  }]);
