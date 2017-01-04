'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('fed-purchase', {
        url: '/fed-purchase',
        views: {
          'styleguideFullView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/fedPurchase/fedPurchase.html');
            }],
            controller: 'FedPurchaseCtrl'
          }
        }
    });
   
  }]);
