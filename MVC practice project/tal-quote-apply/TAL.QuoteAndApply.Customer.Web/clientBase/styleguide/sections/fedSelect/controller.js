'use strict';

angular.module('styleguide')
	.controller('FedSelectCtrl', ['$scope', '$log', function ($scope, $log) {

		$scope.data = {
			index : 0,
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

		$scope.items1 = [
			{
				title: 'Total & Permanent Disability (TPD) Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-3',
			},
			{
				title: 'Life Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-3',
			},
			{
				title: 'Critical Illness (CI) Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-3',
			},
			{
				title: 'Income Protection Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-3',
			}
		];

		$scope.items2 = [
			{
				title: 'Life Cover + Total & Permanent Disability (TPD) Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-6',
			},
			{
				title: 'Critical Illness (CI) Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-3',
			},
			{
				title: 'Income Protection Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-3',
			}
		];

		$scope.items3 = [
			{
				title: 'Life Cover + Total & Permanent Disability (TPD) Cover + Critical Illness (CI) Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-9',
			},
			{
				title: 'Income Protection Cover',
				isOn: true,
				value: 200000,
				premium: 123,
				classCols: 'col-lg-3',
			}
		];

		$scope.onConfirmMajorNotification = function() {
			$log.debug('Confirmed major notification');
		};

		$scope.onDismissMajorNotification = function() {
			$log.debug('Dismissed major notification');
		};
	}]);