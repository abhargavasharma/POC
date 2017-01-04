
(function (module) {
    'use strict';
    module.directive('talRiskUnderwritingSection', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskUnderwritingSection/riskUnderwritingSection.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk: '=',
                quoteReferenceNumber: '=',
                readOnly: '='
            },
            controller: 'talRiskUnderwritingSectionController',
            controllerAs: 'ctrl',
            bindToController: true
        };
    });

    function talRiskUnderwritingSectionController($scope, talSalesPortalApiService, talUnderwritingService, $window, $log, $timeout, talPolicySectionsNotificationService, talPolicyEditSectionsService, SECTION, talLoadingOverlayService, $q) {
        var ctrl = this;
        var isDirty = false;
        var deferInterviewLoaded = null;

        ctrl.mode = 'wizard';
        if (ctrl.readOnly === true) {
            ctrl.mode = 'tree';
        }

        ctrl.section.isModelValid = true; //can always move on from uw section

        ctrl.contextQuestions = [];
        this.currentQuestionIndex = 0;

        var applySectionStates = function (responseData) {
            ctrl.section.isCompleted = false;

            if (responseData.underwritingCompletedForRiskStatus === 'Completed') {
                talPolicyEditSectionsService.clearSectionState(ctrl.section);
                ctrl.section.isCompleted = true;
                talPolicyEditSectionsService.addSectionState(ctrl.section, SECTION.STATUS.VALID);
            }
            else if (responseData.underwritingCompletedForRiskStatus === 'NotCompleted') {
                talPolicyEditSectionsService.clearSectionState(ctrl.section);
                talPolicyEditSectionsService.addSectionState(ctrl.section, SECTION.STATUS.WARNING);
            }
        };

        var handleUnderwritingSyncResponse = function (data) {
            applySectionStates(data.underwritingStatus);
            talPolicySectionsNotificationService.onInsurancePlansPremiumChange(data.riskPremiumSummary);
            talPolicySectionsNotificationService.onPlanStatusChangeEvent(data.policyRiskPlanStatusesResult);
        };

        var syncInterview = function (callBack) {
             var promise = talSalesPortalApiService.getSyncInterview(ctrl.quoteReferenceNumber, ctrl.risk.riskId).then(function (response) {
                handleUnderwritingSyncResponse(response.data);
                if (callBack) {
                    callBack();
                }
            });
            ctrl.loadingPromise = promise;
            return promise;
        };

        var setCurrentQuestion = function () {
            ctrl.currentQuestion = ctrl.underwriting.questions[ctrl.currentQuestionIndex];
            ctrl.endOfQuestions = (ctrl.currentQuestionIndex === ctrl.underwriting.questions.length); //TODO: endOfQuestions is just temp for now for displaying on client, may not be needed eventually
            if (ctrl.endOfQuestions) {
                syncInterview();
            }
        };

        ctrl.setView = function (mode) {
            onDeactivation().then(function() {
                ctrl.mode = mode;
                if (ctrl.mode === 'tree') {
                    isDirty = true;
                    ctrl.talusUiActivation();
                }
                if (ctrl.mode === 'wizard') {
                    onActivation();
                }
            });
        };

        ctrl.goToNextUnanswered = function() {
            ctrl.currentQuestionIndex = _.findIndex(ctrl.underwriting.questions, function(question) {
                return _.every(question.answers, { isSelected: false });
            });
            if (ctrl.currentQuestionIndex === -1) {
                // all questions answered
                ctrl.currentQuestionIndex = ctrl.underwriting.questions.length;
            }
            setCurrentQuestion();
        };

        ctrl.goToBeginning = function () {
            ctrl.currentQuestionIndex = 0;
            setCurrentQuestion();
        };

        var getUnderwritingStatus = function (promise) {
            talSalesPortalApiService.getUnderwritingCompleteStatus(ctrl.quoteReferenceNumber, ctrl.risk.riskId)
                .then(function (response) {
                    applySectionStates(response.data);
                    promise.resolve();
                });
        };

        function onActivation() {

            ctrl.loadingPromise = talSalesPortalApiService.getUnderwritingForRisk(ctrl.quoteReferenceNumber, ctrl.risk.riskId).then(function (response) {
                ctrl.underwriting = response.data;

                talUnderwritingService.buildQuestionsContext(ctrl.underwriting.questions, ctrl.contextQuestions);

                setCurrentQuestion();

                if (ctrl.mode === 'tree') {
                    isDirty = true;
                } else {
                    isDirty = isDirty || false;
                }
            });

            if (ctrl.talusUiActivation) {
                ctrl.talusUiActivation();
            }
        }

        function onDeactivation() {
            var deferred = $q.defer();

            talLoadingOverlayService.showUnderwritingSpinner();
            if (isDirty === true) {
                syncInterview(function() {
                    talLoadingOverlayService.hideUnderwritingSpinner();
                    isDirty = false;
                    deferred.resolve();
                });
            } else {
                talLoadingOverlayService.hideUnderwritingSpinner();
                deferred.resolve();
            }

            return deferred.promise;
        }

        ctrl.section.onActivationEvent = onActivation;
        ctrl.section.onDeactivation = onDeactivation;

        ctrl.syncInterview = syncInterview;

        var filterMatchingQuestions = function (questionsToMatch) {
            var filtered = _.filter(ctrl.underwriting.questions, function (q) {
                return _.some(questionsToMatch, function (cq) {
                    return cq.id === q.id;
                });
            });

            $log.debug('uw - filterMatchingQuestions found: ' + filtered.length + ' out of: ' + questionsToMatch.length);

            return filtered;
        };

        var handleAnswerResponse = function (answerResponse) {
            talUnderwritingService.applyRemovedQuestions(ctrl.underwriting.questions, answerResponse.removedQuestionIds);
            talUnderwritingService.applyChangedQuestionUpdates(ctrl.underwriting.questions, answerResponse.changedQuestions);
            talUnderwritingService.applyAddedQuestions(ctrl.underwriting.questions, answerResponse.addedQuestions);

            ctrl.underwriting.questions = talUnderwritingService.sortQuestions(ctrl.underwriting.categories, ctrl.underwriting.questions);

            //Find the corresponding questions from the main list
            var changedQuestions = filterMatchingQuestions(answerResponse.changedQuestions);
            var addedQuestions = filterMatchingQuestions(answerResponse.addedQuestions);
            var questionsToUpdateContext = _.union(changedQuestions, addedQuestions);

            //Update context for new questions
            talUnderwritingService.buildQuestionsContext(questionsToUpdateContext, ctrl.contextQuestions);

            ctrl.currentQuestion.isAnswered = talUnderwritingService.hasAnySelectedAnswers(ctrl.currentQuestion); //TODO: would never expect this to be false, but should we check and deal with it?

            ctrl.moveToNextQuestion();
        };

        ctrl.currentQuestionHasHelpText = function () {
            if (!ctrl.currentQuestion) {
                return false;
            }

            //todo: use tag
            return ctrl.currentQuestion.helpText;
        };

        ctrl.showHelp = function () {
            if (!ctrl.currentQuestion) {
                return;
            }

            ctrl.currentQuestion.showHelp = true;
        };

        ctrl.getProgress = function () {
            if (!ctrl.underwriting) {
                return 0;
            }

            return ctrl.currentQuestionIndex / ctrl.underwriting.questions.length * 100;
        };

        ctrl.findIndexOfCurrentQuestion = function () {
            return _.findIndex(ctrl.underwriting.questions, { 'id': ctrl.currentQuestion.id });
        };

        ctrl.moveToNextQuestion = function () {
            ctrl.movingBackward = false;

            $timeout(function () {
                var currentQuestionIndex = ctrl.findIndexOfCurrentQuestion();
                ctrl.currentQuestionIndex = currentQuestionIndex + 1;
                setCurrentQuestion();
            });
        };

        ctrl.moveToPreviousQuestion = function () {
            ctrl.movingBackward = true;

            $timeout(function () {
                if (ctrl.currentQuestionIndex > 0) {
                    ctrl.currentQuestionIndex--;
                    setCurrentQuestion();
                }
            });
        };

        ctrl.onQuestionAnswered = function (question) {
            if (ctrl.readOnly) {
                return;
            }

            var selectedAnswers = _.filter(question.answers, { isSelected: true });
            var questionAnswer = {
                questionId: question.id,
                selectedAnswers: selectedAnswers
            };

            ctrl.disableUtilityButtons = true; //Don't want cg-busy to extend outside question area, so other button states are handled this way
            ctrl.loadingPromise = talSalesPortalApiService.answerQuestionForRisk(ctrl.quoteReferenceNumber, ctrl.risk.riskId, questionAnswer).then(function (response) {
                isDirty = true;
                handleAnswerResponse(response.data);
                ctrl.disableUtilityButtons = false;

                $window.scrollTo(0, 0);

            }).catch(function (response) {
                //TODO: handle underwriting errors
                $log.debug('UNDERWRITING ERROR', response);
                ctrl.disableUtilityButtons = false;
            });
        };

        ctrl.showPreviousButton = function () {
            return ctrl.currentQuestionIndex !== 0;
        };

        ctrl.showNextButton = function () {
            var currentQuestionIsAnswered = ctrl.currentQuestion && ctrl.currentQuestion.isAnswered;
            return currentQuestionIsAnswered && ctrl.underwriting && ctrl.currentQuestionIndex < ctrl.underwriting.questions.length;
        };

        ctrl.onProceedClick = function () {
            ctrl.disableUtilityButtons = true; //Don't want cg-busy to extend outside question area, so other button states are handled this way
            //ctrl.loadingPromise = talCustomerPortalApiService.validateUnderwritingForRisk(ctrl.currentRisk.id).then(function (response) {
            //    talAnalyticsService.qualificationSection.trackCompletionOfQuestions();
            //    ctrl.loadingPromise = talNavigationService.handleServerRedirectAction(response.data);
            //}).catch(function (response) {
            //    //TODO: what if we get to the end of underwriting but still errors :(
            //    $log.debug('UNDERWRITING VALIDATION ERROR', response);
            //    ctrl.disableUtilityButtons = false;
            //});
        };

        talPolicySectionsNotificationService.registerUnderwritingStatusChangeEvent(applySectionStates);

        talPolicySectionsNotificationService.registerInsurancePlansChangeEvent(function () {
            deferInterviewLoaded = $q.defer();
            getUnderwritingStatus(deferInterviewLoaded);
        });
    }

    module.controller('talRiskUnderwritingSectionController', talRiskUnderwritingSectionController);
    talRiskUnderwritingSectionController.$inject = ['$scope', 'talSalesPortalApiService', 'talUnderwritingService', '$window', '$log', '$timeout', 'talPolicySectionsNotificationService', 'talPolicyEditSectionsService', 'SECTION', 'talLoadingOverlayService', '$q'];

})(angular.module('salesPortalApp'));

