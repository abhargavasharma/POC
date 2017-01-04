'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('fed-select', {
        url: '/fed-select',
        views: {
          'styleguideFullView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/fedSelect/fedSelect.html');
            }],
            controller: 'FedSelectCtrl'
          }
        }
    });
   
  }]);
