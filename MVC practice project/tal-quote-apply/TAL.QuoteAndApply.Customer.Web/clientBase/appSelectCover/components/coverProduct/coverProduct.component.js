// DISCOUNT

(function(module){

    'use strict';
    module.directive('talCoverProduct', function () {
        return {
            restrict: 'E',
            controller: 'talCoverProductController',
            link: function(){
            }
        };
    });

    function talCoverProductController() {
       
       
    }

    module.controller('talCoverProductController', talCoverProductController);
    talCoverProductController.$inject = [];

})(angular.module('appSelectCover'));  


(function(module){
    
    'use strict';
    module.directive('talCoverProductHeader', function () {
        return {
            templateUrl: '/client/appSelectCover/components/coverProduct/coverProductHeader.template.html',
            restrict: 'A',
            scope:{
                isOn:'=productIsOn',
                value: '=productValue',
                stateChange: '&',
                availability: '=',
                paymentFrequencyPer: '='
            },
            transclude:true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talCoverProductHeaderController',
            link: function(){
            }
        };
    });

    function talCoverProductHeaderController() {
       
       
    }

    module.controller('talCoverProductHeaderController', talCoverProductHeaderController);
    talCoverProductHeaderController.$inject = [];

})(angular.module('appSelectCover'));   
