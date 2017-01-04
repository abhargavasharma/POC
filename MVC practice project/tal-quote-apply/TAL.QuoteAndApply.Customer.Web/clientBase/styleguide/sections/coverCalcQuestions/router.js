'use strict';

angular.module('styleguide')
  .config(['$stateProvider', function($stateProvider){
    
    $stateProvider.state('cover-calc-questions', {
        url: '/cover-calc-questions',
        views: {
          'styleguideFullView': {
            templateProvider: ['$templateCache', function($templateCache){ 
              return $templateCache.get('client/styleguide/sections/coverCalcQuestions/coverCalcQuestions.html');
            }],
            controller: 'CoverCalcQuestionsCtrl'
          }
        }
    });
   
  }]);
