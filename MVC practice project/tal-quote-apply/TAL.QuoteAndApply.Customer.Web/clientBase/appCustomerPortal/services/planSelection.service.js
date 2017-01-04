(function(module, ld) {
    'use strict';

    module.service('talPlanSelectionService', function(talContentService) {

        function getSelectedPlans(plans) {
            var selectedPlans = ld.filter(plans, function(p) {
                return p.isSelected;
            });
            var planCodesArr = ld.map(selectedPlans, function(p) {
                return p.planCode;
            });
            return planCodesArr;
        }

        function getSelectedCovers(covers) {
            var selectedCovers = ld.filter(covers, function(c) {
                return c.isSelected;
            });
            var coverCodesArr = ld.map(selectedCovers, function(c) {
                return c.code;
            });
            return coverCodesArr;
        }

        function getPlanOptions(options) {
            var allOptions = ld.map(options, function(o) {
                return {
                    code: o.code,
                    isSelected: o.isSelected
                };
            });
            return allOptions;
        }

        function getRider(rider) {
            var updateRiderRequest = {
                planId: rider.planId,
                planCode: rider.planCode,
                isSelected: rider.isSelected,
                selectedCoverAmount: rider.selectedCoverAmount,
                selectedCovers: getSelectedCovers(rider.covers),
                options: getPlanOptions(rider.options)
            };

            return updateRiderRequest;
        }

        function getRiders(riders) {
            return ld.map(riders, getRider);
        }

        function getVariables(plan) {
            return ld.map(plan.variables, function(variable){
               return {
                   code: variable.code,
                   selectedValue: variable.selectedValue
               };
            });
        }

        function getPlanByCode(plans, planCode) {
            return _.first(_.filter(plans, function(p) {
                return p.planCode === planCode;
            }));
        }

        function getPlanToAttachTo(planCode, plans) {
            var planToAttach = getPlanByCode(plans, planCode);
            return getPlanByCode(plans, planToAttach.attachesTo);
        }

        function update(obj) {
            for (var i = 1; i < arguments.length; i++) {
                for (var prop in arguments[i]) {
                    var val = arguments[i][prop];
                    if (typeof (val) === 'object') { // this also applies to arrays or null!
                        if (val !== null && val.constructor === Array) {
                            //Account for primitive or empty arrays
                            if (val.length === 0 || typeof (val[0]) !== 'object') {
                                obj[prop] = val;
                            }
                        }
                        update(obj[prop], val);
                    } else {
                        obj[prop] = val;
                    }
                }
            }
            return obj;
        }

        function setPlanContent(plans) {
            var planContentLookup = talContentService.getContentByReferenceKey('coverSelection.plans');
            _.each(plans, function(plan) {
                var planContent = planContentLookup[plan.planCode];
                plan.presentation = planContent;
                setCoverContent(plan.covers);
                _.each(plan.riders, function (rider) {
                    var riderContext = planContentLookup[rider.planCode];
                    rider.presentation = riderContext;
                    setCoverContent(rider.covers);
                });
            });
        }

        function setCoverContent(covers) {
            var planContentLookup = talContentService.getContentByReferenceKey('coverSelection.covers');
            _.each(covers, function (cover) {
                var coverContext = planContentLookup[cover.code];
                cover.presentation = coverContext;
            });
        }

        function setSelectedVariableText(variable) {
            var selectedOption = _.find(variable.options, {value:variable.selectedValue});
            if(selectedOption) {
                variable.selectedText = selectedOption.presentation.shortName;
            }
        }

        function setPlanVariableContent(plans) {
            var coverOptionContent = talContentService.getContentByReferenceKey('coverSelection.coverOptions');
            _.forEach(plans, function(plan){
                _.forEach(plan.variables, function(variable){
                    var planOptionContent = coverOptionContent[variable.code];
                    variable.presentation = planOptionContent;

                    //Update any local overrides for variable options
                    _.forEach(variable.options, function(option) {
                        var overrideOption = _.find(variable.presentation.overrideOptions, {value:option.value});
                        option.presentation = overrideOption || {};
                        option.presentation.overrideLabelName = option.presentation.overrideName || option.name;
                        option.presentation.shortName = option.presentation.shortName || option.name;
                    });

                    setSelectedVariableText(variable);
                });
            });
        }

        function updateAnyOwnAvailability(isTpdOwn, isTpdAny) {
            //TODO: eventually any/own availability should come from the server
            var occupationDefinition = talContentService.getContentByReferenceKey('coverSelection.coverOptions').occupationDefinition;

            var ownOccupation = _.find(occupationDefinition.overrideOptions, { value: 'OwnOccupation' });
            ownOccupation.notAvailable = !isTpdOwn;
            var anyOccupation = _.find(occupationDefinition.overrideOptions, { value: 'AnyOccupation' });
            anyOccupation.notAvailable = !isTpdAny;
        }


        this.updateSelectedVariableTextForVariable = function(variable) {
            setSelectedVariableText(variable);
        };

        this.updateSelectedVariableTextForPlans = function(plans) {
            _.forEach(plans, function(plan){
                _.forEach(plan.variables, function(variable){
                    setSelectedVariableText(variable);
                });
            });
        };

        function assignAllCoversForPlan(plan) {
            plan.allCovers = [];
            _.each(plan.covers, function (cover) {
                plan.allCovers.push(cover);
            });
            _.each(plan.riders, function (rider) {
                if (rider.isSelected === true) {
                    _.each(rider.covers, function (cover) {
                        plan.allCovers.push(cover);
                    });
                }
            });
        }

        this.setRelatedRiders = function(plans) {
            //Set 'relatedRiders' for each plan. This is the rider on another plan that would apply if this plan was bundled
            _.forEach(plans, function(plan){
                var relatedRiders = null;

                if (plan.attachesTo) {
                    var attachToPlan = getPlanByCode(plans, plan.attachesTo);
                    relatedRiders = _.filter(attachToPlan.riders, {riderFor: plan.planCode});
                }

                plan.relatedRiders = relatedRiders;
            });
        };

        this.attachAllContent = function(plans, isTpdOwn, isTpdAny) {
            setPlanContent(plans);
            setPlanVariableContent(plans); //TODO: could move this into setPlanContent to cut down on iterations through plans
            updateAnyOwnAvailability(isTpdOwn, isTpdAny);
        };

        this.updatePlanSelection = function(viewModelPlans, updatedPlans) {
            update(viewModelPlans, updatedPlans);
        };

        this.assignAllCoversForPlan = function(plan) {
            assignAllCoversForPlan(plan);
        };

        this.setPaymentFrequencyPer = function(planResponse) {
           planResponse.paymentFrequencyPer = talContentService.paymentFrequencyPer[planResponse.paymentFrequency];
        };

        this.getPlanToAttachTo = function (planCode, plans) {
            return getPlanToAttachTo(planCode, plans);
        };

        this.getPlanByCode = function (planCode, plans) {
            return getPlanByCode(plans, planCode);
        };

        this.buildPlanRequest = function (plans, activePlan) {
            if (!activePlan.isSelected) {
                _.each(activePlan.riders, function (rider) {
                    rider.isSelected = false;
                });
            }

            var updatePlanRequest = {
                planId: activePlan.planId,
                planCode: activePlan.planCode,
                isSelected: activePlan.isSelected,
                premiumType: activePlan.premiumType,
                selectedCoverAmount: activePlan.selectedCoverAmount,
                premiumHoliday: activePlan.premiumHoliday,
                linkedToCpi: activePlan.linkedToCpi,
                selectedPlans: getSelectedPlans(plans),
                selectedCovers: getSelectedCovers(activePlan.covers),
                options: getPlanOptions(activePlan.options),
                riders: getRiders(activePlan.riders),
                variables: getVariables(activePlan),
                waitingPeriod: activePlan.waitingPeriod,
                benefitPeriod: activePlan.benefitPeriod
            };
            return updatePlanRequest;
        };
    });

})(angular.module('appCustomerPortal'), _);