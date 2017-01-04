(function (module) {
    'use strict';
    module.directive('talQuestion', function () {
        return {
            templateUrl: '/client/appQualification/components/underwriting/question/question.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talQuestionController'
        };
    });

    function talQuestionController() {
        var ctrl = this; //For if you need to use 'this' within a child scope

        this.isShowingHelp = function () {
            return ctrl.question.showHelp;
        };

        this.closeHelp = function () {
            ctrl.question.showHelp = false;
        };

        this.callHelpPrefix = function() {
            return ctrl.question.helpText ?
                'For further information please call ' :
                'For help with this question please call ';
        };
    }

    module.controller('talQuestionController', talQuestionController);
    talQuestionController.$inject = [];

})(angular.module('appQualification'));
