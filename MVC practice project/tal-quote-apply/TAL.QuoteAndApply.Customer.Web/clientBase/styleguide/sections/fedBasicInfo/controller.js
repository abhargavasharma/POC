'use strict';

angular.module('styleguide')
	.controller('FedBasicInfoCtrl', ['$scope', 'FORM' , function ($scope, FORM) {
		$scope.ctrl = this;
    $scope.data = {
      index : 0
    };

    $scope.ctrl.increment = function(){
        $scope.data.index ++ ;
    };

    $scope.ctrl.genderOptions = [
        {label:'Male', value:'Male'},
        {label:'Female', value:'Female'}
    ];

    $scope.ctrl.smokerOptions = [
        {label:'Yes', value:true},
        {label:'No', value:false}
    ];

    $scope.ctrl.titles = FORM.PERSONAL_DETAILS.TITLES;
    $scope.ctrl.states = FORM.ADDRESS.STATES;
	}]);