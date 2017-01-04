(function (module) {
    'use strict';
    module.directive('talTwoOptionRadio', function ($timeout) {
        return {
            templateUrl: '/client/app/components/formControls/twoOptionRadio/twoOptionRadio.template.html',
            restrict: 'E',
            scope: {
                label: '@',
                serverValidate: '@',
                groupName:'@',
                onChange: '&',
                inlineSubmit: '&',
                noInlineSubmit: '@',
                readOnly: '='
            },
            require: 'ngModel',
            controller: 'talTwoOptionRadioController',
            link: function(scope, elem, attrs, ngModelCtrl) {
                scope.ngModelCtrl = ngModelCtrl;

                ngModelCtrl.$formatters.push(function(modelValue){
                    /*
                        The ngModel value passed in needs to be used here as an attribute on an object,
                        otherwise two-way binding breaks if this directive is used inside an ng-transclude
                    */
                    scope.model = {value: modelValue};
                });

                scope.changeEvent = function() {
                    ngModelCtrl.$setViewValue(scope.model.value);
                    $timeout(function(){
                        if(scope.onChange){
                            scope.onChange();
                        }
                    });
                };
            }
        };
    });

    function talTwoOptionRadioController($scope, $attrs) {
        $scope.option1Label = $attrs.option1Label || 'Yes';
        $scope.option1Value = $attrs.option1Value || true;

        $scope.option2Label = $attrs.option2Label || 'No';
        $scope.option2Value = $attrs.option2Value || false;
    }

    module.controller('talTwoOptionRadioController', talTwoOptionRadioController);
    talTwoOptionRadioController.$inject = ['$scope', '$attrs'];

})(angular.module('salesPortalApp'));

