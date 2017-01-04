(function (module) {
    'use strict';
    module.directive('talQuestion', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskUnderwritingSection/question/question.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&',
                readOnly: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talQuestionController'
        };
    });

    function talQuestionController() {
        var ctrl = this; //For if you need to use 'this' within a child scope

        this.isShowingHelp = function () {
            return ctrl.question.showHelp && ctrl.question.helpText;
        };

        this.closeHelp = function () {
            ctrl.question.showHelp = false;
        };

    }

    module.controller('talQuestionController', talQuestionController);
    talQuestionController.$inject = [];

})(angular.module('salesPortalApp'));
