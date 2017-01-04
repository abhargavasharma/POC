
(function (module) {
    'use strict';
    module.directive('talRiskInsurancePlansSection', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskInsurancePlansSection/riskInsurancePlansSection.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk: '=',
                quoteReferenceNumber: '=',
                validateSectionsAndSubmit: '&',
                readOnly: '='
            },
            controller: 'talRiskInsurancePlansSectionController'
        };
    });

    function talRiskInsurancePlansSectionController($scope, SECTION, talSalesPortalApiService, talFormModelStateService, talPolicySectionsNotificationService, talPolicyEditSectionsService) {

        $scope.planTabClicked = function (plan) {
            if ($scope.activePlan.state !== SECTION.STATUS.ERROR) {
                setPlanActive(plan);
                $scope.submitPlanNow(plan);
            } else {
                setPlanActive(plan);
            }
        };

        $scope.planTabCheckboxClicked = function (plan) {
            setPlanActive(plan);
            $scope.submitPlanNow(plan);
        };

        var setPlanActive = function (newActivePlan) {
            $scope.activePlan = newActivePlan;
           
            _.each($scope.plans, function (plan) {
                if (plan.code === newActivePlan.code) {
                    plan.active = true;
                }
                else {
                    plan.active = false;
                }
            });
        };

        var onActivation = function () {
            $scope.onActivation();
        };

        var setFirstPlanActiveThenSubmit = function () {
            if ($scope.readOnly === true) {
                $scope.section.isModelValid = true;
                $scope.section.isCompleted = true;
                return;
            }
            if (!$scope.activePlan) {
                $scope.activePlan = _.first($scope.plans);
            }
            else {
                var updatedActivePlan = _.find($scope.plans, function (n) {
                    return n.code === $scope.activePlan.code;
                });
                $scope.activePlan = updatedActivePlan;
            }

            $scope.submitPlanNow($scope.activePlan);
        };

        $scope.applyPlanStateStyle = function (plan) {
            var returnCssClasses = '';
            returnCssClasses = returnCssClasses.concat(plan.state);
            returnCssClasses = returnCssClasses.concat(plan.selected ? ' selected' : '');
            return returnCssClasses;
        };

        var setAvailabilityOccupationDefinitions = function (planResponse) {
            $scope.availableDefinitions = [];
            if (planResponse.isOccupationTpdAny === true) {
                $scope.availableDefinitions.push({ value: 'AnyOccupation', name: 'Any Occupation' });
            }
            if (planResponse.isOccupationTpdOwn === true) {
                $scope.availableDefinitions.push({ value: 'OwnOccupation', name: 'Own Occupation' });
            }
        };

        $scope.onActivation = function () {

            if (!talPolicyEditSectionsService.sectionInState($scope.section, SECTION.STATUS.ERROR)) {

                $scope.loadingPromise = talSalesPortalApiService.getPlans($scope.quoteReferenceNumber, $scope.risk.riskId).then(function (response) {

                    _.each(response.data.plans, function (plan) {
                        //work out if everything should be disabled based on plan eligibility and permissions
                        plan.planDisabled = !plan.eligibleForPlan || $scope.readOnly;

                        _.each(plan.riders, function (rider) {
                            rider.disabled = !rider.eligibleForPlan || $scope.readOnly;
                        });
                    });

                    $scope.plans = response.data.plans;
                    setAvailabilityOccupationDefinitions(response.data);

                    setFirstPlanActiveThenSubmit();

                });
            } else {
                setFirstPlanActiveThenSubmit();
            }
        };

        var createClientPlan = function (selectedPlan) {
            var convertedObjArray = [];
            if (selectedPlan.riders) {
                _.each(selectedPlan.riders, function (n) {

                    var rider = createClientPlan(n);

                    //apply defaults from plan to rider:
                    rider.premiumType = selectedPlan.premiumType;

                    convertedObjArray.push(rider);
                });
            }
            return {
                planCode: selectedPlan.code,
                planId: selectedPlan.planId,
                coverAmount: selectedPlan.coverAmount,
                linkedToCpi: selectedPlan.linkedToCpi,
                premiumHoliday: selectedPlan.premiumHoliday,
                occupationDefinition: selectedPlan.occupationDefinition,
                selectedCoverCodes: _.map(_.filter(selectedPlan.covers, function (n) {
                    return n.selected;
                }), 'code'),
                selectedOptionCodes: selectedPlan.options,
                selectedRiders: convertedObjArray,
                premiumType: selectedPlan.premiumType,
                premium: selectedPlan.premium,
                selected: selectedPlan.selected,
                waitingPeriod: selectedPlan.waitingPeriod,
                benefitPeriod: selectedPlan.benefitPeriod
            };
        };

        var updateStatuses = function (riskPlanStatuses) {
            setOverallStatus(riskPlanStatuses.overallPlanStatus);
            _.each(riskPlanStatuses.planValidationStatuses, function (planValidation) {
                var plan = _.find($scope.plans, { 'code': planValidation.planCode });
                setPlanStatus(plan, planValidation.planStatus);
            });
        };

        var setPlanStatus = function (plan, status) {
            plan.state = status.toLowerCase();

            if (plan.state === SECTION.STATUS.ERROR) {
                talPolicyEditSectionsService.addSectionState($scope.section, SECTION.STATUS.ERROR);
            }
            if (plan.state === SECTION.STATUS.WARNING) {
                talPolicyEditSectionsService.addSectionState($scope.section, SECTION.STATUS.WARNING);
                talPolicyEditSectionsService.removeSectionState($scope.section, SECTION.STATUS.VALID);
            }
        };

        var setOverallStatus = function (overallStatus) {
            var overallStatusLower = overallStatus.toLowerCase();
            $scope.section.isModelValid = overallStatusLower !== SECTION.STATUS.ERROR;
            $scope.section.isCompleted = overallStatusLower === SECTION.STATUS.COMPLETED;

            if (overallStatusLower === SECTION.STATUS.WARNING) {
                talPolicyEditSectionsService.addSectionState($scope.section, SECTION.STATUS.WARNING);
            }
        };

        var submitPlanNow = function (selectedPlan) {

            var updatePlanRequest = {
                riskId: $scope.riskId,
                quoteReferenceNumber: $scope.quoteReferenceNumber,
                selectedPlanCodes: _.map(_.filter($scope.plans, function (n) {
                    return n.selected;
                }), 'code'),
                currentActivePlan: createClientPlan(selectedPlan)
            };

            var updatingPromise = talSalesPortalApiService.updatePlan($scope.quoteReferenceNumber, $scope.risk.riskId, selectedPlan.code, updatePlanRequest)
                .then(function (response) {

                    talPolicyEditSectionsService.clearSectionState($scope.section);

                    var activePlan = response.data.currentActivePlan;
                    talFormModelStateService.setCleanModelStateAndApplyWarnings($scope, activePlan.warnings);

                    selectedPlan.planId = activePlan.planId;
                    selectedPlan.premium = activePlan.premium;
                    selectedPlan.premiumIncludingRiders = activePlan.premiumIncludingRiders;

                    selectedPlan.premiumFrequency = activePlan.premiumFrequency;

                    _.each(selectedPlan.riders, function (rider) {
                        var updatedRider = _.find(activePlan.riders, function (n) {
                            return n.code === rider.code;
                        });

                        rider.premium = updatedRider.premium;
                        rider.premiumFrequency = updatedRider.premiumFrequency;
                    });

                    _.each(selectedPlan.covers, function (cover) {
                        var updatedCover = _.find(activePlan.covers, function (n) {
                            return n.code === cover.code;
                        });

                        if (updatedCover) {
                            cover.premium = updatedCover.premium;
                        }
                    });

                    _.each($scope.plans, function (plan) {
                        var apiPlan = _.find(response.data.plans, function (apiPlan) {
                            return apiPlan.planId === plan.planId;
                        });

                        plan.premium = apiPlan.premium;
                        plan.premiumIncludingRiders = apiPlan.premiumIncludingRiders;

                        setPlanStatus(plan, apiPlan.status);
                    });

                    talPolicySectionsNotificationService.onInsurancePlansChange();
                    talPolicySectionsNotificationService.onInsurancePlansPremiumChange(response.data.riskPremiumSummary);

                    setOverallStatus(response.data.overallStatus);

                    talFormModelStateService.updateWarningState(activePlan.warnings);

                    setPlanActive(selectedPlan);
                })
                .catch(function (response) {
                    selectedPlan.state = SECTION.STATUS.ERROR;
                    $scope.section.isCompleted = false;
                    $scope.section.isModelValid = false;

                    talFormModelStateService.updateModelState(response.data, $scope);
                    talPolicyEditSectionsService.clearSectionState($scope.section);
                    talPolicyEditSectionsService.addSectionState($scope.section, SECTION.STATUS.ERROR);

                    throw response;
                });
            $scope.updatingPromise = updatingPromise;
        };


        talPolicySectionsNotificationService.registerInitialisationChangeEvent(function () {
            $scope.onActivation();
        });

        talPolicySectionsNotificationService.registerPremiumFrequencyChangeEvent(function () {
            $scope.onActivation();
        });

        talPolicySectionsNotificationService.registerPlanStatusChangeEvent(updateStatuses);

        $scope.section.onActivationEvent = onActivation;
        $scope.submitPlanNow = submitPlanNow;
    }

    module.controller('talRiskInsurancePlansSectionController', talRiskInsurancePlansSectionController);
    talRiskInsurancePlansSectionController.$inject = ['$scope', 'SECTION', 'talSalesPortalApiService', 'talFormModelStateService', 'talPolicySectionsNotificationService', 'talPolicyEditSectionsService'];

})(angular.module('salesPortalApp'));

