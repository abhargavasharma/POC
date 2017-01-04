'use strict';

angular.module('appCustomerPortal').directive('talDistributeWidth', function () {
    return {
        restrict: 'A',
        scope:{
            talDistributeWidth: '='
        },
        link: function(scope, elem) {
            var width = 100 / scope.talDistributeWidth;
            elem.css('width', width + '%');
        }
    };
});
