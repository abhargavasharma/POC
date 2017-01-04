(function(module){
    'use strict';
    module.directive('talFullPageLoader', function () {
        return {
            templateUrl: '/client/app/components/loadingIndicator/fullPageLoadingIndicator.template.html',
            restrict: 'E',
            scope: {
                text: '='
            },
            controller: 'talFullPageLoaderController as ctrl',
            bindToController: true
        };
    });

    var talFullPageLoaderController = function() {
        this.text = this.text || 'Loading...';
    };

    module.controller('talFullPageLoaderController', talFullPageLoaderController);
    talFullPageLoaderController.$inject = [];

})(angular.module('salesPortalApp'));

