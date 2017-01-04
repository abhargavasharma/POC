(function(module){
    'use strict';
    module.directive('talCoverAmount', function () {
        return {
            templateUrl: '/client/appSelectCover/components/coverAmount/coverAmount.template.html',
            restrict: 'E',
            require: 'ngModel',
            scope: {
              options: '='  
            },
            controller: 'talCoverAmountController'
        };
    });

    function talCoverAmountController() {
    }

    module.controller('talCoverAmountController', talCoverAmountController);
    talCoverAmountController.$inject = ['$scope'];

})(angular.module('appSelectCover'));   
