(function (module) {
    'use strict';
    module.directive('talNeedsAnalysis', function () {
        return {
            templateUrl: '/client/appNeedsAnalysis/components/needsAnalysis/needsAnalysis.template.html',
            restrict: 'E',
            scope: {},
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talNeedsAnalysisController'
        };
    });

    function talNeedsAnalysisController($log, $state, $stateParams, $rootScope, EVENT,
                                        talNeedsAnalysisService, talCustomerPortalApiService, talPlanSelectionService, talNavigationService,
                                        talAnalyticsService, talClientStorageService) {
        $log.debug('needs analysis controller. state: ' + $state.current.name);
        var ctrl = this;

        var ctrlState = 'quote';
        if ($state.current.name === 'help') {
            ctrlState = 'help';
        }

        ctrl.navigation = {
            hasPrevious: false,
            onPrevious: function () {
                $log.debug('previous clicked');
            }
        };

        ctrl.state = ctrlState;

        ctrl.isLastQuestion = false;
        ctrl.movingBackward = false;

        ctrl.percentage = talNeedsAnalysisService.percentage;
        ctrl.questions = talNeedsAnalysisService.questions;

        ctrl.basicInfo = $stateParams.basicInfo;

        if(ctrl.basicInfo){
            talNeedsAnalysisService.setOccupationDefinitionAvailability('AnyOccupation', !ctrl.basicInfo.riskOverview.cover.isOccupationTpdAny);
            talNeedsAnalysisService.setOccupationDefinitionAvailability('OwnOccupation', !ctrl.basicInfo.riskOverview.cover.isOccupationTpdOwn);

            talNeedsAnalysisService.updatePlansAvailability(ctrl.basicInfo.plans);
        } else {
            if ($stateParams.tryContinueExistingQuote) {
                talCustomerPortalApiService.getRisks()
                    .then(function (response) {
                        ctrl.basicInfo = {};
                        $rootScope.$broadcast(EVENT.QUOTE.SHOW_QUOTE_REF_NUMBER);
                        handleRiskResponse(response);
                    })
                    .catch(function () {
                        $log.info('no valid quote in progress');
                        $state.transitionTo('quote');
                    });
            }
        }

        ctrl.getOccupationType = function () {
            return talNeedsAnalysisService.getSelectedValueByQuestionName('occupationTPD');
        };

        ctrl.getIPWaitingPeriod = function () {
            return talNeedsAnalysisService.getSelectedValueByQuestionName('ipWaitingPeriod');
        };

        ctrl.getIPBenefitPeriod = function () {
            return talNeedsAnalysisService.getSelectedValueByQuestionName('ipBenefitPeriod');
        };

        ctrl.getSportAnswerValue = function () {
            return talNeedsAnalysisService.getIsCoveredAnswer('sport');
        };

        ctrl.getAdventureSportAnswerValue = function () {
            return talNeedsAnalysisService.getIsCoveredAnswer('adventureSport');
        };

        ctrl.getAccidentAnswer = function () {
            return talNeedsAnalysisService.checkQuestionSelectedAnswer('coverOptions', 'Accidents');
        };

        ctrl.getIllnessAnswer = function () {
            return talNeedsAnalysisService.checkQuestionSelectedAnswer('coverOptions', 'Illness');
        };

        ctrl.getSportAnswer = function () {
            return ctrl.toCoveredNotCovered(ctrl.getSportAnswerValue());
        };

        ctrl.getAdventureSportAnswer = function () {
            return ctrl.toCoveredNotCovered(ctrl.getAdventureSportAnswerValue());
        };

        ctrl.toCoveredNotCovered = function (selectedValue) {
            if (selectedValue === null) {
                return '';
            }

            return selectedValue === true ? 'Covered' : 'Not covered';
        };

        ctrl.getLifePlan = function () {
            return ctrl.getPlan('DTH');
        };

        ctrl.getPlan = function (planCode) {
            var plan = _.find(ctrl.plans, {planCode: planCode});
            if (!plan) {
                throw 'unable to find plan for: ' + planCode;
            }
            return plan;
        };

        ctrl.getRider = function (plan, riderCode) {
            var riderPlan = _.find(plan.riders, {planCode: riderCode});
            if (!riderPlan) {
                throw 'unable to find rider plan for: ' + riderPlan;
            }
            return riderPlan;
        };

        ctrl.setPlanSelected = function (planCode, planPercentage) {
            var plan = _.find(ctrl.plans, {planCode: planCode});
            plan.isSelected = planPercentage === 100;
            $log.debug('updating: ' + planCode + ' percentage: ' + planPercentage + ' selected: ' + plan.isSelected);
            return plan;
        };

        ctrl.applyPlanUpdate = function (plan) {
            var updateRequest = talPlanSelectionService.buildPlanRequest(ctrl.plans, plan);
            return talCustomerPortalApiService.updateCoverSelectionForRisk(ctrl.basicInfo.riskId, updateRequest)
                .then(function (response) {
                    $log.debug('response success for: ' + plan.planCode);
                    $log.debug(response);

                    talPlanSelectionService.updatePlanSelection(ctrl.plans, response.data.plans);
                })
                .catch(function (error) {
                    $log.debug('response error for: ' + plan.planCode);
                    $log.debug(error);
                });
        };

        ctrl.applyAllPlansUpdate = function (plans) {
            var plansRequest = _.map(plans, function (plan) {
                return talPlanSelectionService.buildPlanRequest(ctrl.plans, plan);
            });

            return talCustomerPortalApiService.updateMultiCoverSelectionForRisk(ctrl.basicInfo.riskId, {requests: plansRequest})
                .then(function (response) {
                    talPlanSelectionService.updatePlanSelection(ctrl.plans, response.data.plans);
                })
                .catch(function (error) {
                    $log.debug(error);
                    throw error;
                });
        };

        ctrl.updatePlan = function (planCode, planPercentage) {
            var plan = ctrl.setPlanSelected(planCode, planPercentage);
            return ctrl.applyPlanUpdate(plan);
        };

        ctrl.updatePlanLife = function () {
            var plan = ctrl.setPlanSelected('DTH', ctrl.percentage.life);
            if (plan.isSelected) {
                var isAccidentSelected = ctrl.getAccidentAnswer();
                ctrl.findCoverAndSelect(plan, 'DTHAC', isAccidentSelected);

                var isIllnessSelected = ctrl.getIllnessAnswer();
                ctrl.findCoverAndSelect(plan, 'DTHIC', isIllnessSelected);

                var isAdventureSportSelected = ctrl.getAdventureSportAnswerValue() === true;
                ctrl.findCoverAndSelect(plan, 'DTHASC', isAdventureSportSelected);
            }

            return plan;
        };

        ctrl.updatePlanCI = function () {
            var plan = ctrl.setPlanSelected('TRS', ctrl.percentage.ci);

            var isAccidentSelected = ctrl.getAccidentAnswer();
            var isIllnessSelected = ctrl.getIllnessAnswer();

            if (plan.isSelected) {
                ctrl.findCoverAndSelect(plan, 'TRSSIC', isIllnessSelected);
                ctrl.findCoverAndSelect(plan, 'TRSCC', isIllnessSelected);
                ctrl.findCoverAndSelect(plan, 'TRSSIN', isAccidentSelected);
            }

            return plan;
        };

        ctrl.updatePlanTPD = function () {
            var plan = ctrl.setPlanSelected('TPS', ctrl.percentage.tpd);

            var isAccidentSelected = ctrl.getAccidentAnswer();
            var isIllnessSelected = ctrl.getIllnessAnswer();
            var isSportSelected = ctrl.getSportAnswerValue() === true;

            if (plan.isSelected) {
                ctrl.findCoverAndSelect(plan, 'TPSAC', isAccidentSelected);
                ctrl.findCoverAndSelect(plan, 'TPSIC', isIllnessSelected);
                ctrl.findCoverAndSelect(plan, 'TPSASC', isSportSelected);

                var selectedOccupationDefinition = talNeedsAnalysisService.getSelectedAnswerByQuestionName('occupationTPD');
                if(selectedOccupationDefinition){
                    _.find(plan.variables, {code: 'occupationDefinition'}).selectedValue = selectedOccupationDefinition.id;
                }

            }


            return plan;
        };

        ctrl.updatePlanIP = function () {
            var plan = ctrl.setPlanSelected('IP', ctrl.percentage.ip);
            if (plan.isSelected) {
                var selectedWaitingPeriod = talNeedsAnalysisService.getSelectedAnswerByQuestionName('ipWaitingPeriod');
                var selectedBenefitPeriod = talNeedsAnalysisService.getSelectedAnswerByQuestionName('ipBenefitPeriod');

                _.find(plan.variables, {code: 'waitingPeriod'}).selectedValue = selectedWaitingPeriod.id;
                _.find(plan.variables, {code: 'benefitPeriod'}).selectedValue = selectedBenefitPeriod.id;

                var isAccidentSelected = ctrl.getAccidentAnswer();
                ctrl.findCoverAndSelect(plan, 'IPSAC', isAccidentSelected);

                var isIllnessSelected = ctrl.getIllnessAnswer();
                ctrl.findCoverAndSelect(plan, 'IPSIC', isIllnessSelected);

                var isSportSelected = ctrl.getSportAnswerValue() === true;
                ctrl.findCoverAndSelect(plan, 'IPSSC', isSportSelected);
            }

            return plan;
        };

        ctrl.findCoverAndSelect = function (plan, coverCode, isSelected) {
            var cover = _.find(plan.covers, {code: coverCode});
            if (!cover) {
                throw 'unable to find cover ' + coverCode;
            }

            cover.isSelected = isSelected;
        };

        ctrl.findRiderCoverAndSelect = function (plan, riderCode, coverCode, isSelected) {
            var rider = _.find(plan.riders, {planCode: riderCode});
            if (!rider) {
                throw 'unable to find rider ' + riderCode;
            }
            ctrl.findCoverAndSelect(rider, coverCode, isSelected);
        };


        ctrl.updateAllPlans = function () {
            var plans = [];

            plans.push(ctrl.updatePlanLife());
            plans.push(ctrl.updatePlanCI());
            plans.push(ctrl.updatePlanTPD());
            plans.push(ctrl.updatePlanIP());

            return plans;
        };

        var handleRiskResponse = function(getRisksResponse) {
            var risk = getRisksResponse.data[0]; // TODO: Risk index for which risk?

            ctrl.basicInfo.riskId = risk.id;

            ctrl.loadingPromise = talCustomerPortalApiService.getReviewForRisk(ctrl.basicInfo.riskId)
                .then(function (response) {
                    ctrl.basicInfo.riskOverview = response.data;

                    $log.debug('5. get plans');
                    return talCustomerPortalApiService.getCoverSelectionForRisk(ctrl.basicInfo.riskId);
                })
                .then(function (response) {
                    ctrl.basicInfo.plans = response.data.plans;
                    talNeedsAnalysisService.setPlanVariables(ctrl.basicInfo.plans);

                    var ipPlan = _.find(ctrl.basicInfo.plans, { planCode: 'IP' });

                    ctrl.basicInfo.variables =
                    {
                        waitingPeriod: _.find(ipPlan.variables, { code: 'waitingPeriod' }),
                        benefitPeriod: _.find(ipPlan.variables, { code: 'benefitPeriod' })
                    };

                    ctrl.basicInfo.availability = {
                        life: _.find(ctrl.basicInfo.plans, { planCode: 'DTH' }).availability.isAvailable,
                        tpd: _.find(ctrl.basicInfo.plans, { planCode: 'TPS' }).availability.isAvailable,
                        ri: _.find(ctrl.basicInfo.plans, { planCode: 'TRS' }).availability.isAvailable,
                        ip: ipPlan.availability.isAvailable
                    };

                    $log.debug('6. redirect to /help');
                    ctrl.state = 'help';
                    $state.go('help', {getQuoteComplete: true, basicInfo: ctrl.basicInfo});
                });

        };


        ctrl.createQuote = function () {
            $log.debug('createQuote');
            $log.debug('1. init quote..');
            return talCustomerPortalApiService.initQuote()
                .then(function () {
                    var basicInfo = ctrl.basicInfo;
                    if (!basicInfo) {
                        throw 'unable to find basic info';
                    }

                    $log.debug('2. creating quote..');
                    return talCustomerPortalApiService.createQuoteForHelpMeChoose(basicInfo);
                })
                .then(function (response) {
                    $log.debug('create quote success: ');
                    $log.debug(response);

                    talAnalyticsService.createQuote.trackNewQuote(ctrl.basicInfo.dateOfBirth, ctrl.basicInfo.gender, 
                        ctrl.basicInfo.isSmoker, ctrl.basicInfo.occupationTitle, ctrl.basicInfo.industryTitle.$$unwrapTrustedValue(),
                        ctrl.basicInfo.annualIncome);

                    var quoteReference = response.data;
                    $rootScope.$broadcast(EVENT.QUOTE.QUOTE_REF_NUMBER, quoteReference);

                    $log.debug('3. getting risk..');
                    return talCustomerPortalApiService.getRisks();
                })
                .then(function (response) {
                    return handleRiskResponse(response);
                })
                .catch(function (response) {
                    $log.debug(response);
                });

        };

        ctrl.modifyPlans = function () {
            $log.debug('modifyPlans');
            $log.debug('1. get cover selection for risk');
            ctrl.plans = ctrl.basicInfo.plans;

            $log.debug('2. update all plans');
            var updatedPlans = ctrl.updateAllPlans();

            $log.debug('3. apply updating all plans');
            return ctrl.applyAllPlansUpdate(updatedPlans)
                .then(function () {
                    $log.debug('4. redirect to select cover');

                    talNavigationService.handleServerRedirectAction({ redirectTo: '/SelectCover' });
                })
                .catch(function (response) {
                    $log.debug(response);
                });
        };

        ctrl.onHelpMeChooseComplete = function () {
            talClientStorageService.setGoneViaHelpMeChoose(true);
            ctrl.questionsComplete = true;

            ctrl.loadingPromise = ctrl.modifyPlans();
        };

        ctrl.onGetQuoteComplete = function (basicInfo) {
            $log.debug('onGetQuoteComplete');
            $log.debug(basicInfo);

            ctrl.basicInfo = basicInfo;
            ctrl.basicInfo.source = 'CustomerPortalHelpMeChoose';

            //navigate to /help
            this.loadingPromise = ctrl.createQuote();
        };

    }

    module.controller('talNeedsAnalysisController', talNeedsAnalysisController);
    talNeedsAnalysisController.$inject = ['$log', '$state', '$stateParams', '$rootScope', 'EVENT',
        'talNeedsAnalysisService', 'talCustomerPortalApiService', 'talPlanSelectionService', 'talNavigationService', 'talAnalyticsService', 'talClientStorageService'];

}(angular.module('appNeedsAnalysis')));