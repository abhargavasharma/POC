// DISCOUNT

(function(module){

    'use strict';
    module.directive('talCoverBreakdown', function () {
        return {
            templateUrl: '/client/appSelectCover/components/coverBreakdown/coverBreakdown.template.html',
            restrict: 'E',
            scope: {
              talCoverBreakdownData: '=',
              period: '=?'  
            },
            transclude:true,
            controller: 'talCoverBreakdownController',
            link: function(scope){
                scope.coverExpanded = false;
            }
        };
    });

    function talCoverBreakdownController($scope) {

        $scope.total = 0;
        var compileFigure = function(){
            var total = 0;
            angular.forEach($scope.talCoverBreakdownData, function(item){
                total += item.value; 
            });
            $scope.total = total;
        };
       $scope.$watch('talCoverBreakdownData', function(){
            compileFigure();
       });
       
    }

    module.controller('talCoverBreakdownController', talCoverBreakdownController);
    talCoverBreakdownController.$inject = ['$scope'];

})(angular.module('appSelectCover'));   
