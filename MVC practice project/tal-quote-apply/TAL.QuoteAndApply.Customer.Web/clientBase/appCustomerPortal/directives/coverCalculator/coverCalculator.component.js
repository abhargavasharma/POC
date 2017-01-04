(function (module) {
    'use strict';
    module.directive('talCoverCalculator', function () {
        return {
            restrict: 'E',
            scope: {
                startCalcFn: '=',
                useResults: '&'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talCoverCalculatorController'
        };
    });

    function CoverCalcResultsController($scope, ngDialog, $log) {
        $scope.useTheseValues = function (calculatorResultsJson, calculatorAssumptionsJson) {
            var calcResults = {
                results: calculatorResultsJson,
                assumptions: calculatorAssumptionsJson
            };

            $log.debug('useTheseValues');
            if($scope.ctrl.useResults){
                $scope.ctrl.useResults({ calcResults: calcResults});
            }

            ngDialog.closeAll();
        };

        $scope.showAssumptions = function () {
            $scope.assumptionsAreVisisble = true;
        };

        $scope.showResults = function () {
            $scope.assumptionsAreVisisble = false;
        };
    }

    CoverCalcResultsController.$inject = ['$scope', 'ngDialog', '$log'];

    function controller($log, $rootScope, $scope, ngDialog, $window) {
        $log.debug('talCoverCalculatorController');

        this.startCalcFn = function () {
            ngDialog.open({
                templateUrl: '/client/appCustomerPortal/directives/coverCalculator/coverCalculator.questions.template.html',
                className: ''
            });
            $window.scrollTo(0,0);
        };

        $rootScope.$on('cc-questions-complete', function() {
            ngDialog.closeAll();

            ngDialog.open({
                templateUrl: '/client/appCustomerPortal/directives/coverCalculator/coverCalculator.confirm.template.html',
                controller: CoverCalcResultsController,
                scope: $scope,
                className: '',
                closeByNavigation: false,
                closeByEscape: false,
                closeByDocument: false
            });
            $window.scrollTo(0,0);
        });
    }

    module.controller('talCoverCalculatorController', controller);
    controller.$inject = ['$log', '$rootScope', '$scope', 'ngDialog', '$window'];

}(angular.module('appCustomerPortal')));

