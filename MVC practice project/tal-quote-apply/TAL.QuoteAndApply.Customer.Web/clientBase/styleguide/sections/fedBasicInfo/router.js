'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('fed-basic-info', {
        url: '/fed-basic-info',
        views: {
          'styleguideFullView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/fedBasicInfo/fedBasicInfo.html');
            }],
            controller: 'FedBasicInfoCtrl'
          }
        }
    });
   
  }]);
