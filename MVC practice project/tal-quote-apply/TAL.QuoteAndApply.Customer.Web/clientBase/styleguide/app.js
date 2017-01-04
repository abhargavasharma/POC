'use strict';

var dependancies = [
	'cgBusy',
	'ui.router',
    'ui.utils',
	'appTemplate',
	'appCustomerPortal',
	'appQualification',
	'appSelectCover',
	'appBasicInfo',
	'ngDialog',
	'countTo',
	'hljs',
	'angularRipple',
	'angularSpinners',
	'ngAnimate',
	'toastr',
	'ng-slide-down',
	'ngclipboard',
	'angular-intro',
	'ngTouch',
	'oc.lazyLoad'
];

angular.module('styleguide', dependancies)

.value('cgBusyDefaults', {
    delay: 100,
    minDuration: 0,
    templateUrl: '/client/appCustomerPortal/components/loadingIndicator/loadingIndicator.template.html'
})

.config(['hljsServiceProvider', function (hljsServiceProvider) {
	hljsServiceProvider.setOptions({
		// replace tab with 4 spaces
		tabReplace: '    '
	});
}])

.config(['ngDialogProvider', function(ngDialogProvider) {
	ngDialogProvider.setDefaults({
	    showClose: false,
	    className: 'modal'
	});
}])

.run(['$ocLazyLoad', function ($ocLazyLoad) {
	$ocLazyLoad.load('http://localhost:9000/scripts/scripts.qa.js');
}]);