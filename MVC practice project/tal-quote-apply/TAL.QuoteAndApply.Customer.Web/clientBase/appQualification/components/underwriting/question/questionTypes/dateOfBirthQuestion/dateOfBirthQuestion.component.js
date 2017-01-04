(function (module) {
    'use strict';
    module.directive('talDateOfBirthQuestion', function () {
        return {
            templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/dateOfBirthQuestion/dateOfBirthQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talDateOfBirthQuestionController'
        };
    });

    function controller(talCustomerPortalApiService, talFormModelStateService) {
        var ctrl = this;

        ctrl.dateOfBirth = ctrl.question.answers[0].text;

        this.onContinueClick = function () {

            var vm = {
                dateOfBirth: ctrl.dateOfBirth
            };
            
            ctrl.loadingPromise = talCustomerPortalApiService.validateAge(vm)
                .then(function () {
                    talFormModelStateService.updateCleanModelState();

                    ctrl.question.answers[0].text = ctrl.dateOfBirth;

                    ctrl.onQuestionAnswered();

                }).catch(function (response) {
                    talFormModelStateService.updateModelState(response.data);
                });
        };
    }

    module.controller('talDateOfBirthQuestionController', controller);
    controller.$inject = ['talCustomerPortalApiService', 'talFormModelStateService'];

})(angular.module('appQualification'));