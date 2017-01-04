(function(module){
    'use strict';

    module.directive('talRatingFactorsForm', function () {
        return {
            templateUrl: '/client/app/components/createClientForm/ratingFactorsForm/ratingFactorsForm.component.template.html',
            restrict: 'E',
            scope: true,
            controllerAs: 'ctrl',
            bindToController: {
                ratingFactors: '='
            },
            controller: 'talRatingFactorsFormController'
        };
    });

    function talRatingFactorsFormController() {
       
    }

    module.controller('talRatingFactorsFormController', talRatingFactorsFormController );

})(angular.module('salesPortalApp'));
