(function (module) {
    'use strict';
    module.directive('talWhyInsuranceQuestion', function () {
        return {
            templateUrl     : '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTypes/whyInsuranceQuestion/whyInsuranceQuestion.template.html',
            restrict        : 'E',
            scope           : {
                question: '=',
                onNext: '&',
                availability: '='
            },
            bindToController: true,
            controllerAs    : 'ctrl',
            controller      : 'talWhyInsuranceQuestionController'
        };
    });

    module.controller('talWhyInsuranceQuestionController', ['$log', function ($log) {
        $log.debug('talWhyInsuranceQuestionController');

        var ctrl = this;

        ctrl.isSubmitEnabled = function () {
            return _.some(ctrl.question.options, function (q) {
                return q === true;
            });
        };

        ctrl.onOptionChanged = function () {
            ctrl.question.onAnswered();
        };
    }]);

})(angular.module('appNeedsAnalysis'));
