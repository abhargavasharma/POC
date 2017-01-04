(function (module) {
    'use strict';
    module.directive('talSelectPlan', function () {
        return {
            templateUrl: '/client/appSelectCover/components/selectPlan/selectPlan.template.html',
            restrict: 'E',
            scope: {
                initMetadata: '@'
            },
            controller: 'selectPlanController as ctrl',
            bindToController: true
        };
    });


    function selectPlanController($log, $scope, $rootScope, $timeout, $window, EVENT,
        talCustomerPortalApiService, talPlanSelectionService, talCustomerPremiumService,
        talContentService, talNavigationService, talFormModelStateService, ngDialog,
        pageSpinnerService, talAnalyticsService, talBrowserService, viewBagConfig, talClientStorageService) {
        var ctrl = this;

        pageSpinnerService.start();

        ctrl.showHelpMeChooseText = talClientStorageService.getGoneViaHelpMeChoose() || viewBagConfig.journeySource === 'CustomerPortalHelpMeChoose';

        ctrl.submissionErrorMessage = '';

        function setPlanTitles(plans) {
            _.each(plans, function (plan) {
                plan.presentation.planTitles = [];
                plan.presentation.planTitles.push(plan.presentation.planTitle);
                _.each(plan.riders, function (rider) {
                    if (rider.isSelected === true) {
                        plan.presentation.planTitles.push('+');
                        plan.presentation.planTitles.push(rider.presentation.planTitle);
                    }
                });
            });
        }


        function setPresentations(plans) {
            _.each(plans, function (plan) {
                plan.presentation = plan.presentation || {};
                //{'is-off':!plan.isSelected}
                plan.presentation.cssClass = 'col-lg-' + ((plan.planWidth + 1) * 3);
            });
            setPlanTitles(ctrl.plans);
        }

        function attachPlanAsRider(planCode) {
            var planToAttachTo = talPlanSelectionService.getPlanToAttachTo(planCode, ctrl.plans);
            ctrl.loadingPromise = talCustomerPortalApiService.attachRiderForRisk(ctrl.risk.id, planCode, planToAttachTo.planCode).then(function (response) {
                talFormModelStateService.updateCleanModelState();
                talPlanSelectionService.updatePlanSelection(ctrl.plans, response.data.plans);
                setPresentations(ctrl.plans);
                ctrl.totalPremium = response.data.totalPremium;
                talCustomerPremiumService.triggerPremiumUpdated(response.data.totalPremium, response.data.paymentFrequency);
            });
        }

        function onLoad() {
            var walkThroughDone = talClientStorageService.getShownWalkthrough();
            if (!walkThroughDone) {
                $timeout(function () {
                    ctrl.intro.startTour();
                    var currentDate = new Date();
                    talClientStorageService.setShownWalkthrough(currentDate);
                });
            }

            if (ctrl.initMetadata && ctrl.initMetadata !== 'null') {
                var metadata = JSON.parse(ctrl.initMetadata);
                if (!metadata.CalculatorResultsUsed) {

                    ctrl.calcResults = metadata.CalculatorResultsJson;
                    ctrl.calcAssumptions = metadata.CalculatorAssumptionsJson;

                    if (metadata.UseResultConfirmationRequired === false) {
                        ctrl.onConfirmUseCalcResults();
                    } else {
                        ctrl.showUseCalcResultsConfirm = true;
                    }
                    
                }
            }

            ctrl.isCoverCalculatorEnabled = viewBagConfig.isCalculatorEnabled;
        }


        ctrl.loadingPromise = talCustomerPortalApiService.getRisks().then(function (response) {
            ctrl.risk = response.data[0]; // TODO: Risk index for which risk?

            $rootScope.$broadcast(EVENT.RISK_ID.UPDATE, ctrl.risk.id);

            ctrl.loadingPromise = talCustomerPortalApiService.getCoverSelectionForRisk(ctrl.risk.id).then(function (response) {
                pageSpinnerService.stop();
                ctrl.plans = response.data.plans;
                talPlanSelectionService.attachAllContent(ctrl.plans, response.data.isOccupationTpdOwn, response.data.isOccupationTpdAny);
                setPresentations(ctrl.plans);
                talPlanSelectionService.setPaymentFrequencyPer(response.data);
                ctrl.paymentFrequencyPer = response.data.paymentFrequencyPer;
                $scope.paymentFrequencyPer = ctrl.paymentFrequencyPer; //Also chuck on scope so dialog has access to it
                ctrl.paymentFrequency = response.data.paymentFrequency;
                ctrl.totalPremium = response.data.totalPremium;
                talCustomerPremiumService.triggerPremiumUpdated(response.data.totalPremium, response.data.paymentFrequency);

                if (_.every(ctrl.plans, { 'isEligibleForPlan': false })) {
                    ctrl.intro.introOptions.steps = [];
                }

                _.forEach(ctrl.plans, function (plan) {
                    plan.isOn = plan.isSelected;
                });

                if (viewBagConfig.saveGatePosition === 'BeforeSelectPlan' &&
                    viewBagConfig.journeySource &&
                    (
                    //if we allow saviving/loading - we require password
                    (viewBagConfig.isQuoteSaveLoadEnabled === true && viewBagConfig.saveStatus && viewBagConfig.saveStatus !== 'passwordCreated') ||
                    //if we dont - we require only personal details to be entered
                    (viewBagConfig.isQuoteSaveLoadEnabled === false && viewBagConfig.saveStatus && viewBagConfig.saveStatus !== 'personalDetails')
                    )) {
                    $timeout($rootScope.$broadcast(EVENT.SAVE.TRIGGER_FORCED_SAVE, { hideOnCallBack: true, callBack: onLoad }));
                } else {
                    onLoad();
                }

                talAnalyticsService.coverSelection.trackAllPlans(ctrl.plans, ctrl.totalPremium, ctrl.paymentFrequency);
            });
        });

        ctrl.stateChange = function (plan, callBackOnSuccess) {
            var updateRequest = talPlanSelectionService.buildPlanRequest(ctrl.plans, plan);
            ctrl.loadingPromise = talCustomerPortalApiService.updateCoverSelectionForRisk(ctrl.risk.id, updateRequest).then(function (response) {
                talFormModelStateService.updateCleanModelState();
                talPlanSelectionService.updatePlanSelection(ctrl.plans, response.data.plans);
                ctrl.paymentFrequency = response.data.paymentFrequency;
                ctrl.totalPremium = response.data.totalPremium;
                talCustomerPremiumService.triggerPremiumUpdated(response.data.totalPremium, response.data.paymentFrequency);

                if (callBackOnSuccess) {
                    callBackOnSuccess();
                }
                talAnalyticsService.coverSelection.trackPlan(plan, ctrl.totalPremium, ctrl.paymentFrequency);
                ctrl.submissionErrorMessage = '';

                _.forEach(ctrl.plans, function (plan) {
                    plan.isOn = plan.isSelected;
                });

            }).catch(function (response) {
                talFormModelStateService.updateModelState(response.data);
            });
        };

        ctrl.forceSaveAndRedirect = function () {
            $rootScope.$broadcast(EVENT.SAVE.TRIGGER_FORCED_SAVE, {
                hideOnCallBack: true, callBack: function () {
                    talCustomerPortalApiService.validateCoversAndProceedForRisk(ctrl.risk.id).then(function (response) {
                        var redirectData = response.data;
                        talNavigationService.handleServerRedirectAction(redirectData);
                    });
                }
            });
        };

        ctrl.validate = function () {
            ctrl.loadingPromise = talCustomerPortalApiService.validateCoversForRisk(ctrl.risk.id).then(function () {
                talCustomerPortalApiService.validateCoversAndProceedForRisk(ctrl.risk.id).then(function (response) {
                    var redirectData = response.data;
                    talNavigationService.handleServerRedirectAction(redirectData);
                });

            }).catch(function (response) {
                if (response.data.minimumSelectionCriteria) {
                    ctrl.submissionErrorMessage = response.data.minimumSelectionCriteria;
                }
            });
        };

        ctrl.attachPlan = function (planCode) {
            var plan = talPlanSelectionService.getPlanByCode(planCode, ctrl.plans);
            plan.attached = false;
            //attachPlanAsRider
            var dialog = ngDialog.open({
                templateUrl: '/client/appSelectCover/components/selectPlan/modal/attachRider.template.html',
                controllerAs: 'ctrl',
                scope: $scope,
                data: plan
            });
            $window.scrollTo(0, 0);

            dialog.closePromise.then(function (data) {
                if (data.value === true) {
                    attachPlanAsRider(planCode);
                }
            });
        };

        ctrl.detachPlan = function (riderCode) {
            ctrl.loadingPromise = talCustomerPortalApiService.detachRiderForRisk(ctrl.risk.id, riderCode).then(function (response) {
                talFormModelStateService.updateCleanModelState();
                talPlanSelectionService.updatePlanSelection(ctrl.plans, response.data.plans);
                setPresentations(ctrl.plans);
                setPlanTitles(ctrl.plans);
                talCustomerPremiumService.triggerPremiumUpdated(response.data.totalPremium, response.data.paymentFrequency);
            });
        };

        function createDialogItem(type, code, title, item) {
            return {
                type: type,
                code: code,
                title: title,
                item: item
            };
        }

        function createPlanOptionTabItems(plan) {
            talPlanSelectionService.assignAllCoversForPlan(plan); //Get all covers (including selected riders)

            //Plan dialog has tabs for cover selection and plan variables. Build those items here...
            var tabItems = [];

            //Create cover items
            _.forEach(plan.allCovers, function (cover) {
                tabItems.push(createDialogItem('cover', cover.code, cover.presentation.shortTitle, cover));
            });

            //Create variable items
            _.forEach(plan.variables, function (variable) {
                if (!variable.presentation.hideOnSelectCover) {
                    tabItems.push(createDialogItem('variable', variable.code, variable.name, variable));
                }
            });

            return tabItems;
        }

        ctrl.selectCovers = function (plan) {
            if ($scope.coverSelectionForm.$invalid) {
                return;
            }

            var dialogTabItems = createPlanOptionTabItems(plan);

            var dialogData = {
                plan: plan,
                tabItems: dialogTabItems
            };

            ngDialog.open({
                templateUrl: '/client/appSelectCover/components/selectPlan/modal/coverSelection.template.html',
                controller: [
                    '$scope', function ($scope) {
                        $scope.onChange = function () {
                            ctrl.stateChange(plan);
                        };
                    }
                ],
                controllerAs: 'ctrl',
                scope: $scope,
                data: dialogData
            });
            $window.scrollTo(0, 0);
        };

        ctrl.displayLearnMoreDialog = function (plan) {
            talPlanSelectionService.assignAllCoversForPlan(plan);
            ngDialog.open({
                templateUrl: '/client/appSelectCover/components/selectPlan/modal/learnMoreModal.template.html',
                controller: [
                    '$scope', function ($scope) {
                        $scope.onChange = function () {
                            ctrl.stateChange(plan);
                        };
                    }
                ],
                controllerAs: 'ctrl',
                scope: $scope,
                data: plan
            });
            $window.scrollTo(0, 0);
        };
        ctrl.intro = {};
        ctrl.intro.shouldAutoStart = false;
        ctrl.intro.introOptions = {
            steps: [
                {
                    element: '.tal-card tal-card-header:not(.is-off) tal-form-switch',
                    intro: '<h3 class="tour-heading h5">Switch cover on/off</h3> <p class="p--small">You can add or remove these insurance options to suit your needs.</p>',
                    position: talBrowserService.isMobileDevice() ? 'bottom-right-aligned' : 'top'
                },
                {
                    element: '.tal-card .tal-cover-product-content:not(.is-off) form-select-textfield',
                    intro: '<h3 class="tour-heading h5">Change how much you\'re insured for</h3> <p class="p--small">You can select how much you want to be insured for.</p>',
                    position: talBrowserService.isMobileDevice() ? 'bottom-right-aligned' : 'top'
                },
                {
                    element: '.tal-card .tal-cover-product-content:not(.is-off) .tal-btn--secondary',
                    intro: '<h3 class="tour-heading h5">Customise your insurance</h3> <p class="p--small">You can personalise the type of cover you receive here.</p>',
                    position: talBrowserService.isMobileDevice() ? 'bottom-right-aligned' : 'top'
                }
            ],
            showStepNumbers: false,
            exitOnOverlayClick: true,
            exitOnEsc: true,
            nextLabel: 'Ok!',
            skipLabel: 'Click here to skip',
            doneLabel: 'Done'
        };

        ctrl.intro.onBeforeChangeEvent = function () {
            angular.element(document.querySelector('.is-clone')).remove();
        };
        ctrl.intro.onAfterChangeEvent = function (targetElement) {
            var elementClone = targetElement.cloneNode(true),
                highlightRegion = document.querySelector('.introjs-tooltipReferenceLayer'),
                appended;

            angular.element(document.querySelector('.is-clone')).remove();

            angular.element(elementClone).addClass('is-faded').addClass('is-clone');
            appended = highlightRegion.appendChild(elementClone);

            $scope.tourIndex = $scope.tourIndex || 0;

            $timeout(function () {
                angular.element(appended).removeClass('is-faded');
                $scope.tourIndex += 1;
            }, $scope.tourIndex === 0 ? 0 : 550);
        };
        ctrl.intro.onExitEvent = function () {
        };
        ctrl.intro.onChangeEvent = function () {
        };
        ctrl.intro.onCompletedEvent = function () {
        };

        ctrl.getResultKey = function (planCode) {
            switch (planCode) {
                case 'TRS': return 'CriticalIllness';
                case 'IP': return 'Income';
                case 'DTH': return 'Life';
                case 'TPS': return 'Tpd';
            }

            throw 'unable to find matching result key for plan code: ' + planCode;
        };

        ctrl.onConfirmUseCalcResults = function () {
            $log.debug('onConfirmUseCalcResults');
            var calcResults = ctrl.calcResults;

            var plansRequest = _.map(ctrl.plans, function (plan) {
                var key = ctrl.getResultKey(plan.planCode);
                var coverAmount = calcResults[key].Value;

                //Life in the calculator has a different structure to get the cover amount
                if (coverAmount === undefined) {
                    coverAmount = calcResults[key].Amount.Value;
                }

                if (isNaN(coverAmount)) {
                    throw 'unable to find cover amount for [' + key + '] plan code: ' + plan.planCode;
                }

                plan.selectedCoverAmount = coverAmount;

                return talPlanSelectionService.buildPlanRequest(ctrl.plans, plan);
            });

            $timeout(function () {
                $scope.$broadcast('cover-amount:calc-results-applied');
            }, 500);

            ctrl.applyCalcResultsPromise = talCustomerPortalApiService.updateMultiCoverSelectionForRisk(ctrl.risk.id, { requests: plansRequest, allowPartialUpdate: true })
                .then(function (response) {
                    talPlanSelectionService.updatePlanSelection(ctrl.plans, response.data.plans);
                })
                .catch(function (response) {
                    talFormModelStateService.updateModelState(response.data);
                })
                .finally(function () {
                    talCustomerPortalApiService.useCalcResultsForRisk(ctrl.risk.id);
                    ctrl.showUseCalcResultsConfirm = false;
                });
        };

        ctrl.onDismissUseCalcResults = function () {
            $log.debug('onDismissUseCalcResults');

            talCustomerPortalApiService.useCalcResultsForRisk(ctrl.risk.id);
            ctrl.showUseCalcResultsConfirm = false;
        };


        this.useCalcResults = function (calcResults) {
            $log.debug('use calc results');

            talCustomerPortalApiService.setCalcResultsForRisk(calcResults, false)
                .then(function () {
                    $window.location.reload();
                });
        };

        $scope.$on(EVENT.CHAT.CHAT_AVAILABILITY, function ($event, chatAvailability) {
            $log.debug('selectPlan - chatAvailability: ' + JSON.stringify(chatAvailability));
            ctrl.chatAvailability = chatAvailability;
        });
       
    }

    module.controller('selectPlanController', selectPlanController);
    selectPlanController.$inject = ['$log', '$scope', '$rootScope', '$timeout', '$window',
        'EVENT', 'talCustomerPortalApiService', 'talPlanSelectionService', 'talCustomerPremiumService',
        'talContentService', 'talNavigationService', 'talFormModelStateService', 'ngDialog', 'pageSpinnerService',
        'talAnalyticsService', 'talBrowserService', 'viewBagConfig', 'talClientStorageService'];

})(angular.module('appSelectCover'));

