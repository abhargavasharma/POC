
(function (module) {
    'use strict';
    module.directive('talSectionMenuItem', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/sectionMenuItem/sectionMenuItem.template.html',
            restrict: 'E',
            transclude: true,
            scope: {
                section: '=',
                hideSections: '=',
                isOpenSectionValid: '=',
                risk: '='
            },
            controller: 'talPolicyRiskWidgetController'
        };
    });

    function talPolicyRiskWidgetController($scope, talPolicyEditSectionsService) {
        $scope.activate = function() {
            talPolicyEditSectionsService.activateSection($scope.section);
        };
    }

    module.controller('talPolicyRiskWidgetController', talPolicyRiskWidgetController);
    talPolicyRiskWidgetController.$inject = ['$scope', 'talPolicyEditSectionsService'];

})(angular.module('salesPortalApp'));