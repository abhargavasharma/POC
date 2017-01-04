(function (module) {
    'use strict';
    module.directive('talSingleSelectRadioQuestion', function () {
        return {
            templateUrl     : '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTypes/singleSelectRadioQuestion/singleSelectRadioQuestion.template.html',
            restrict        : 'E',
            scope           : {
                question: '=',
                onNext  : '&'
            },
            bindToController: true,
            controllerAs    : 'ctrl',
            controller      : 'talSingleSelectRadioQuestionController'
        };
    });

    module.controller('talSingleSelectRadioQuestionController', ['$log', function ($log) {
        $log.debug('talSingleSelectRadioQuestionController');

        var ctrl = this;

        ctrl.groupName = ctrl.question.name;

        ctrl.isContinueEnabled = function () {
            return _.some(ctrl.question.options, {isSelected: true});
        };

        ctrl.optionChanged = function () {
            _.each(ctrl.question.options, function (option) {
                option.isSelected = option.value === ctrl.question.selectedAnswer;
            });

            ctrl.onNext();
        };
    }]);

})(angular.module('appNeedsAnalysis'));
