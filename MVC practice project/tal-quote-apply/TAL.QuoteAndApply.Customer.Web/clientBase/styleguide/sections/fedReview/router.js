'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('fed-review', {
        url: '/fed-review',
        views: {
          'styleguideFullView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/fedReview/fedReview.html');
            }],
            controller: 'FedReviewCtrl'
          }
        }
    });
   
  }]);
