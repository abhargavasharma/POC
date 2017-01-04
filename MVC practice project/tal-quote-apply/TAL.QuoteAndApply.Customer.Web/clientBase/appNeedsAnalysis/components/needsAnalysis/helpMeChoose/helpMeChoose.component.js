(function (module) {
    'use strict';
    module.directive('talHelpMeChoose', function () {
        return {
            templateUrl     : '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/helpMeChoose.template.html',
            restrict        : 'E',
            scope           : {
                questions: '=',
                navigation: '=',
                onCompleted: '&',
                availability: '='
            },
            bindToController: true,
            controllerAs    : 'ctrl',
            controller      : 'talHelpMeChooseController'
        };
    });

    function controller($log, $timeout, $state, $stateParams, talAnalyticsService, $window) {

        var ctrl = this;

        var index = parseInt($stateParams.index, 10) || 0;

        ctrl.currentQuestionIndex = 0 || index;

        ctrl.updateNavigation= function () {
            ctrl.navigation.hasPrevious = ctrl.currentQuestionIndex > 0;
        };

        ctrl.onNext = function () {
            var currentQuestion = ctrl.questions[ctrl.currentQuestionIndex];

            talAnalyticsService.helpMeChooseSection.trackQuestion(currentQuestion.name, currentQuestion.options);
            
            var nextIndex = ctrl.currentQuestionIndex + 1;
            if(nextIndex >= ctrl.questions.length){
                $log.debug('reached the end');
                if(ctrl.onCompleted){
                    ctrl.onCompleted();
                }
                return;
            }

            ctrl.movingBackward = false;

            $timeout(function () {
                ctrl.currentQuestionIndex++;
                ctrl.updateNavigation();

                var currentQuestion = ctrl.questions[ctrl.currentQuestionIndex];
                if (currentQuestion.isAvailable) {
                    if (!currentQuestion.isAvailable()) {
                        ctrl.onNext();
                    }
                    else {
                        //last question
                        ctrl.isLastQuestion = true;
                        // ctrl.state = 'quote';
                    }
                    return;
                }

                // $state.go('.', {index: ctrl.currentQuestionIndex});
            });
            $window.scrollTo(0, 0);
        };

        ctrl.navigation.onPrevious = function () {
            ctrl.movingBackward = true;

            $timeout(function () {

                ctrl.isLastQuestion = true;
                if (!ctrl.navigation.hasPrevious) {
                    return;
                }

                ctrl.questionsComplete = false;
                ctrl.currentQuestionIndex--;
                ctrl.updateNavigation();

                var currentQuestion = ctrl.questions[ctrl.currentQuestionIndex];
                if (!currentQuestion) {
                    return;
                }

                if (currentQuestion.isAvailable && !currentQuestion.isAvailable()) {
                    ctrl.navigation.onPrevious();
                    return;
                }
            });

            // $state.go('.', {index: ctrl.currentQuestionIndex});
            $window.scrollTo(0, 0);
        };
    }

    module.controller('talHelpMeChooseController', controller);
    controller.$inject = ['$log', '$timeout', '$state', '$stateParams', 'talAnalyticsService', '$window'];

}(angular.module('appNeedsAnalysis')));