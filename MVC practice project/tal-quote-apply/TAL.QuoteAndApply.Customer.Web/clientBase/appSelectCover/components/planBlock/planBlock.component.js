(function(module){
    'use strict';
    module.directive('talPlanBlock', function () {
        return {
            templateUrl: '/client/appSelectCover/components/planBlock/planBlock.template.html',
            restrict: 'E',
            scope: {
                vm: '=ngModel',
                stateChange: '&',
                loadingPromise: '=',
                attachPlan: '&'
            },
            controller: 'planBlockController as ctrl',
            bindToController: true
        };
    });

    function planBlockController() {
        var ctrl = this;

        ctrl.vm.colSize = (_.filter(ctrl.vm.riders, function (r) { return r.isSelected; }).length + 1) * 4;
        ctrl.vm.hasActiveRiders = _.some(ctrl.vm.riders, function (r) { return r.isSelected; });

    }

    module.controller('planBlockController', planBlockController);
    planBlockController.$inject = ['$scope'];

})(angular.module('appSelectCover'));
