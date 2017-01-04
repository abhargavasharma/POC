'use strict';

angular.module('styleguide')
  .config(['$urlRouterProvider', '$locationProvider', function($urlRouterProvider, $locationProvider){
    // $locationProvider.html5Mode(true);
    if(typeof $locationProvider.hashPrefix !== 'undefined'){
      $locationProvider.hashPrefix('!');
    }
    $urlRouterProvider.otherwise('/');
    

    
  }]);
