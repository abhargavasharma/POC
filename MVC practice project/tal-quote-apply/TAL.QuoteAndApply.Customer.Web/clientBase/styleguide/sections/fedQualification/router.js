'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('fed-qualification', {
        url: '/fed-qualification',
        views: {
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/fedQualification/fedQualification.html');
            }],
            controller: 'FedQualificationCtrl'
          }
        }
    });
   
  }]);
