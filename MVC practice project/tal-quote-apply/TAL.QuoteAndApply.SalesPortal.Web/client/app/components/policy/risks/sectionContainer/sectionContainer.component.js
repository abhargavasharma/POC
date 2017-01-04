
(function (module) {
    'use strict';
    module.directive('talSectionContainer', ['$document', function ($document) {
        return {
            templateUrl: '/client/app/components/policy/risks/sectionContainer/sectionContainer.template.html',
            restrict: 'E',
            scope: {
                risk: '=',
                section: '=',
                heading: '='
            },
            transclude: true,
            link: function (scope) {
                var bodyElement = angular.element($document[0].body);

                scope.isFullScreen = false;

                scope.toggleFullScreen = function() {
                    scope.isFullScreen = !scope.isFullScreen;
                    if (scope.isFullScreen) {
                        bodyElement.addClass('no-scroll');
                    } else {
                        bodyElement.removeClass('no-scroll');
                    }
                };
            }
        };
    }]);

})(angular.module('salesPortalApp'));