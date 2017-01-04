'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('questions', {
        url: '/questions',
        views: {
          'styleguideMainView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/questions/questions.html');
            }],
            controller: 'QuestionsCtrl'
          }
        },
          'styleguideSidebarView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/index/index.html');
            }],
          }
    });
   
  }]);
