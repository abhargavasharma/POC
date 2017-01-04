'use strict';

angular.module('styleguide')
  .controller('StagingCtrl', ['$scope', 'FORM' ,function ($scope, FORM) {
    $scope.ctrl = this;
    $scope.data = {
      index : 0
    };

    $scope.ctrl.increment = function(){
        $scope.data.index ++ ;
    };

    $scope.ctrl.genderOptions = [
        {label:'M', value:'M'},
        {label:'F', value:'F'}
    ];

    $scope.ctrl.smokerOptions = [
        {label:'Y', value:true},
        {label:'N', value:false}
    ];

    $scope.ctrl.titles = FORM.PERSONAL_DETAILS.TITLES;
    $scope.ctrl.states = FORM.ADDRESS.STATES;
  }]);