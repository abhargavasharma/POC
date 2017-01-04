(function (module) {
    'use strict';
    module.directive('talSingleSelectButtonsQuestion', function () {
        return {
            templateUrl     : '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTypes/singleSelectButtonsQuestion/singleSelectButtonsQuestion.template.html',
            restrict        : 'E',
            scope           : {
                question: '=',
                onNext  : '&'
            },
            bindToController: true,
            controllerAs    : 'ctrl',
            controller      : 'talSingleSelectButtonsQuestionController'
        };
    });

    module.controller('talSingleSelectButtonsQuestionController', ['$log', function ($log) {
        $log.debug('talSingleSelectButtonsQuestionController');

        var ctrl = this;

        ctrl.groupName = ctrl.question.name;

        var selectedOption = _.find(ctrl.question.options, {isSelected: true});
        if(selectedOption){
            ctrl.selectedValue = selectedOption.value;
        }

        this.optionChanged = function () {
            _.each(ctrl.question.options, function (option) {
                option.isSelected = option.value === ctrl.selectedValue;
            });

            if(ctrl.question.onAnswered){
                ctrl.question.onAnswered(ctrl.selectedValue);
            }

            ctrl.onNext();
        };
    }]);

})(angular.module('appNeedsAnalysis'));
