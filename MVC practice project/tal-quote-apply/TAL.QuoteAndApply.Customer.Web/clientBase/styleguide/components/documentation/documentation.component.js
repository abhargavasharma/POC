(function(module){
  'use strict';

  module.directive('talDocumentation', function () {
    return {
      templateUrl: '/client/styleguide/components/documentation/documentation.template.html',
      restrict: 'E',
      replace: true,
      scope: {
        'index': '=index',
        'data': '=data',
        'path': '=talDocumentationPath',
        'includeInverse': '@',
        'noPadding': '@?'
      },
      controller: 'documentationController',
       link: function (scope) {
          scope.id = scope.index;
       }
    };
  });

  var documentationController = function($scope) {
    $scope.counter = $scope.$parent.ctrl.increment();
    
    $scope.toggleCode = function() {
      $scope.isCodeVisible = !$scope.isCodeVisible;
    };
  };

 

  module.controller('documentationController', documentationController);
  documentationController.$inject = ['$scope'];

})(angular.module('styleguide'));
