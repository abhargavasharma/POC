'use strict';

angular.module('styleguide')
  .controller('ModalsCtrl', ['$scope', 'ngDialog', function($scope, ngDialog) {
    $scope.ctrl = this;
    $scope.ctrl.increment = function(){
        $scope.data.index ++ ;
    };  
    
    $scope.data = {
      index : 0
    };
    $scope.openModal = function (target, version) {
      ngDialog.open({
        templateUrl: target,
        className: version
      });
    };

    $scope.openModalNotDismissable = function (target, version) {
      ngDialog.open({
        templateUrl: target,
        className: version,
        closeByNavigation: false,
        closeByEscape: false,
        closeByDocument: false
      });
    };

  }]);