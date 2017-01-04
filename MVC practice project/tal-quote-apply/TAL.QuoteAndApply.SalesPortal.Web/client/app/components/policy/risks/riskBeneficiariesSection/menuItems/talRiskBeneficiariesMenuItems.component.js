
(function (module) {
    'use strict';
    module.directive('talRiskBeneficiariesMenuItems', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskBeneficiariesSection/menuItems/talRiskBeneficiariesMenuItems.template.html',
            restrict: 'E',
            scope: {
                action: '=',
                index: '=',
                form: '=',
                beneficiaries: '='
            }
        };
    });

    
})(angular.module('salesPortalApp'));

