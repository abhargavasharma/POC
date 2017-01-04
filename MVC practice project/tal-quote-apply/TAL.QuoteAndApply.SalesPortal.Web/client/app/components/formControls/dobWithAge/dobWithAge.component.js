(function (module) {
    'use strict';
    module.directive('talDobWithAge', function () {
        return {
            templateUrl: '/client/app/components/formControls/dobWithAge/dobWithAge.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                warningsEnabled: '=',
                serverValidate: '@',
                label: '@',
                onChange: '&',
                onBlur: '&',
                inlineSubmit: '&',
                noInlineSubmit: '@',
                readOnly: '='
            },
            require: 'ngModel',
            controller: 'talDobWithAgeController',
            link: function (scope, elem, attrs, ngModel) {

                scope.dataValue = {
                    dob: ''
                };

                scope.updateModel = function () {
                    ngModel.$setViewValue(scope.dataValue.dob);

                    if(scope.onChange){
                        scope.onChange();
                    }
                };

                ngModel.$parsers.push(function (val) {

                    var momentVal = moment(val, scope.dobUiMaskFormat, true); //true = Force strict date checking

                    if (momentVal.isValid()) {
                        ngModel.$setValidity('dobMaskInput', true);
                        return momentVal.format(scope.dobDisplayFormat);
                    }

                    ngModel.$setValidity('dobMaskInput', false);
                    return val;
                });

                ngModel.$formatters.push(function (val) {

                    var momentVal = moment(val, scope.dobDisplayFormat);

                    if (!momentVal.isValid()) {
                        scope.dataValue.dob = '';
                        scope.calculateAge();
                        return val;
                    }

                    scope.dataValue.dob = momentVal.format(scope.dobDisplayFormat);
                    return scope.dataValue.dob;
                });
            }

        };
    });

    function talDobWithAgeController($scope) {
        $scope.dobUiMaskFormat = 'DDMMYYYY'; //This is the format that ui-mask passes back to us
        $scope.dobDisplayFormat = 'DD/MM/YYYY';

        if(!$scope.label) {
            $scope.label = 'Date of Birth';
        }

        $scope.calculateAge = function () {
            var momentDate = moment($scope.dataValue.dob, $scope.dobDisplayFormat);

            if (momentDate.isValid()) {
                var age = moment().diff(momentDate, 'years');
                $scope.ageMessage = age + ' years of age';
            } else {
                $scope.ageMessage = '';
            }

            if ($scope.onBlur) {
                $scope.onBlur();
            }
        };
    }

    module.controller('talDobWithAgeController', talDobWithAgeController);
    talDobWithAgeController.$inject = ['$scope'];

})(angular.module('salesPortalApp'));

