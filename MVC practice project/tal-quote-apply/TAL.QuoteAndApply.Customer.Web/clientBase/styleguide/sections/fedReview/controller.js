'use strict';

angular.module('styleguide')
	.controller('FedReviewCtrl', ['$scope', function($scope) {
		$scope.data = {
			isOn:true,
			select : {
				value: 1000000,
				selectItems : [500000, 1000000]
			},
			breakdown: [
				{
					label:'Life Cover',
					value:21
				},
				{
					label:'Permanent Disability Cover',
					value:35
				},
				{
					label:'Critical illness Cover',
					value:10
				},
				{
					label:'Income Protection Cover',
					value:12
				}
			]
		};
	}]);