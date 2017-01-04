'use strict';
angular.module('salesPortalApp').directive('talManageInterview', function () {
    return {
        templateUrl: '/client/app/components/manageInterview/manageInterview.template.html',
        restrict: 'E',
        scope: true,
        controllerAs: 'ctrl',
        bindToController: {
            cancelUrl: '@'
        },
        controller: 'talManageInterviewController'
    };
});

angular.module('salesPortalApp').controller('talManageInterviewController',
    function ($scope, talSalesPortalApiService, talFormModelStateService, FORM) {
        var ctrl = this;

       function search() {
            ctrl.loadingPromise = talSalesPortalApiService.getSyncInterviewByInterviewId(ctrl.searchCriteria.interviewId)
                .then(function (results) {
                    ctrl.searchPerformed = true;
                    ctrl.interviewExists = results.data;
                    talFormModelStateService.updateCleanModelState($scope);
                })
                .catch(function (response) {
                    var modelState = response.data;
                    talFormModelStateService.updateModelState(modelState, $scope);
                    ctrl.interviewExists = false;
                });
       }

        function performAction(interviewAction) {
            console.log(interviewAction);
        }

        function init() {
            ctrl.searchCriteria = {};
            ctrl.interviewExists = false;
            ctrl.onSearchInterviewIdClick = search;
            ctrl.searchPerformed = false;
            ctrl.interviewActions = FORM.INTERVIEW.ACTIONS;
            ctrl.performInterviewAction = performAction;
        }

        init();
    }
);
