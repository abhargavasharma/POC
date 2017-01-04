'use strict';

angular.module('styleguide')
  .controller('NavigationCtrl', ['$scope', function($scope) {
        $scope.ctrl = this;
        $scope.ctrl.increment = function(){
            $scope.data.index ++ ;
        };
        
        $scope.data = {
          index : 0
        };

        $scope.breakdown = [
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
       ];

       $scope.mainQuote = {};
  }]);