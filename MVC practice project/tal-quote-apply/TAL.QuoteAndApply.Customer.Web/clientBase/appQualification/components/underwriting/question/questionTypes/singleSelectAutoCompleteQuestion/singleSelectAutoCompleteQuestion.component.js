(function (module) {
    'use strict';
    module.directive('talSingleSelectAutoCompleteQuestion', function () {
        return {
            templateUrl: '/client/appQualification/components/underwriting/question/questionTypes/singleSelectAutoCompleteQuestion/singleSelectAutoCompleteQuestion.template.html',
            restrict: 'E',
            scope: {
                question: '=',
                onQuestionAnswered: '&'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talSingleSelectAutoCompleteController'
        };
    });

    function talSingleSelectAutoCompleteController($log, $scope, talUnderwritingService) {
        var ctrl = this;

        ctrl.selectedAnswer = talUnderwritingService.getSelectedSingleAnswer(ctrl.question);

        ctrl.selectedAnswers = [];
        if(ctrl.selectedAnswer){
            ctrl.selectedAnswers.push(ctrl.selectedAnswer);
        }

        this.continueDisabled = function () {
            return !ctrl.selectedAnswers.length;
        };

        this.onContinueClick = function () {
            talUnderwritingService.clearAllAnswers(ctrl.question);

            //mark selected answers
            _.each(ctrl.selectedAnswers, function (selected) {
                var answer = _.find(ctrl.question.answers, {id: selected.id});
                if(!answer){
                    throw 'answer not found for ' + selected.id;
                }

                answer.isSelected = true;
            });

            ctrl.onQuestionAnswered();
        };

        $scope.$watch('ctrl.selectedAnswer', function (newVal, oldVal) {
            if(newVal === oldVal){
                return;
            }

            $log.debug('selectedAnswer changed to: ' + newVal);

            ctrl.selectedAnswers = newVal? [newVal] : [];
        });

        $scope.$watchCollection('ctrl.selectedAnswers', function (newVal, oldVal) {
            if(newVal === oldVal){
                return;
            }

            $log.debug('selectedAnswers changed to: ' + newVal);

            if(newVal.length > 1){
                throw 'unsupported multi options in single select';
            }

            if(newVal.length === 0){
                ctrl.selectedAnswer = null;
            }
            else{
                ctrl.selectedAnswer = newVal[0];
            }
        });


    }

    module.controller('talSingleSelectAutoCompleteController', talSingleSelectAutoCompleteController);
    talSingleSelectAutoCompleteController.$inject = ['$log', '$scope', 'talUnderwritingService'];

})(angular.module('appQualification'));
