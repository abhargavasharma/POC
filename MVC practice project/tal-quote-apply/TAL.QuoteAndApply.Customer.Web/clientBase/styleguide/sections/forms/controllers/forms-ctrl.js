'use strict';
/**
 * @ngdoc controller
 * @name tal.controller:FormsCtrl
 * @description
 * # FormsCtrl
 * Controller of the tal
 */
angular.module('styleguide')
  .controller('FormsCtrl', ['$scope', function ($scope) {
		$scope.ctrl = this;

	$scope.ctrl.increment = function(){
        $scope.data.index ++ ;
    };

	$scope.data = {
      index : 0
    };

	$scope.select = {
		value: null,
		selectItems : [500000, 750000, 1000000, 1250000, 1500000]
	};

	$scope.field = {
		value:'my value'
	};

	$scope.checkbox = {
		value:true
	};

	$scope.dob = {
		value:'22/12/2015'
	};
	$scope.mySwitch = {
		value:true
	};
	$scope.radio = {
		value: null,
		value1: null,
		value2: null,
	};

	$scope.search1 = {};
	$scope.search2 = {};

	$scope.search1.searchCollection = [
		{ title: 'Heart disease' },
		{ title: 'Back pain' },
		{ title: 'Migraines' },
		{ title: 'Arthritis' },
		{ title: 'Cancer' },
		{ title: 'Breast cancer' },
		{ title: 'Dementia' },
		{ title: 'Malaria' },
		{ title: 'Narcoleptic episodes' },
		{ title: 'Seizures' },
		{ title: 'Epilepsy' }
	];

	$scope.search2.searchCollection = [
		{ title: '12kg' },
		{ title: '13kg' },
		{ title: '14kg' },
		{ title: '15kg' },
		{ title: '16kg' },
		{ title: '17kg' },
		{ title: '18kg' },
		{ title: '19kg' },
		{ title: '20kg' },
		{ title: '21kg' },
		{ title: '22kg' },
		{ title: '23kg' },
		{ title: '24kg' },
		{ title: '25kg' },
		{ title: '26kg' },
		{ title: '27kg' },
		{ title: '28kg' },
		{ title: '29kg' },
		{ title: '30kg' },
	];

	$scope.search2.singleOnly = true;

	$scope.search1.values = [];
	$scope.search2.values = [];
  }]);