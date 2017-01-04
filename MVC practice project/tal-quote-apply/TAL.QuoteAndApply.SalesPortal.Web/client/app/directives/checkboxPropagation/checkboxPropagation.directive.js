'use strict';

angular.module('salesPortalApp').directive('talInputPropagation', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function(scope, element){

            element.bind('click', function(e) {
                // allow checkboxes in tab headings to be checked
                if (element.prop('tagName') === 'INPUT' || element.prop('tagName') === 'SELECT') {
                    e.stopPropagation();
                }
            });
        }
    };
});
