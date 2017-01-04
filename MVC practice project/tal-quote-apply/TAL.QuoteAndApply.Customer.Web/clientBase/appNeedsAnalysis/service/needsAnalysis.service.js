(function (module) {
    'use strict';

    module.service('talNeedsAnalysisService', ['$log', 'talContentService', function ($log, talContentService) {
        $log.debug('talNeedsAnalysisService');
        var service = this;

        this.isParentQuestionSelected = function (questionName) {
            return service.checkQuestionSelectedAnswer(questionName, true);
        };

        this.checkQuestionSelectedAnswer = function (questionName, expectedAnswer) {
            var parentQuestion = _.find(service.questions, {name: questionName});
            if (!parentQuestion) {
                throw 'unable to find question: ' + questionName;
            }
            $log.debug('processing parent question: ' + parentQuestion);

            if (parentQuestion.isAvailable && !parentQuestion.isAvailable()) {
                $log.debug('question not available');
                return false;
            }

            return _.some(parentQuestion.options, {isSelected: true, value: expectedAnswer});
        };

        var isIPAvailable = function () {
            var questionName = 'whatIsIP';
            return service.isParentQuestionSelected(questionName);
        };

        this.getSelectedAnswer = function (question) {
            return _.find(question.options, {isSelected: true});
        };

        this.getSelectedValue = function (question) {
            var selectedAnswer = service.getSelectedAnswer(question);
            if (!selectedAnswer) {
                return null;
            }

            return selectedAnswer.value;
        };

        this.anyAnswerSelected = function (questionsNames) {
            var allSelectedValues = _.map(questionsNames, function (questionName) {
                return service.getSelectedValueByQuestionName(questionName);
            });

            if (_.some(allSelectedValues, function (answer) {
                    return answer === true;
                })) {
                return true;
            }

            if (_.some(allSelectedValues, function (answer) {
                    return answer === false;
                })) {
                return false;
            }

            return null;
        };

        this.getSelectedValueByQuestionName = function (questionName) {
            var question = service.getQuestionByName(questionName);
            if (!question) {
                return null;
            }

            return service.getSelectedValue(question);
        };

        this.getSelectedAnswerByQuestionName = function (questionName) {
            var question = service.getQuestionByName(questionName);
            if (!question) {
                return null;
            }

            return service.getSelectedAnswer(question);
        };

        this.getQuestionByName = function (name) {
            return _.find(service.questions, {name: name});
        };

        this.getIsCoveredAnswer = function (questionName) {
            var question = service.getQuestionByName(questionName);
            if (!question) {
                return null;
            }

            return service.getSelectedValue(question);
        };

        this.getIpProgress = function () {
            var included = service.getSelectedValue(service.getQuestionByName('whatIsIP'));
            if (included === true) {
                return 100;
            }
            if (included === false) {
                return 0;
            }

            var whyAnswers = service.getQuestionByName('whyInsurance').options;
            var whyAnswersResults = 0;
            if (whyAnswers[1]) {
                whyAnswersResults += 60;
            }
            if (whyAnswers[2]) {
                whyAnswersResults += 40;
            }

            return whyAnswersResults;
        };

        this.getLifeProgress = function () {
            var included = service.getSelectedValue(service.getQuestionByName('whatIsLife'));
            if (included === true) {
                return 100;
            }
            if (included === false) {
                return 0;
            }

            var whyAnswers = service.getQuestionByName('whyInsurance').options;
            var whyAnswersResults = 0;

            if (whyAnswers[0]) {
                whyAnswersResults += 50;
            }

            if (whyAnswers[3]) {
                whyAnswersResults += 50;
            }

            return whyAnswersResults;
        };

        this.getTPDProgress = function () {
            var included = service.getSelectedValue(service.getQuestionByName('whatIsTPD'));
            if (included === true) {
                return 100;
            }
            if (included === false) {
                return 0;
            }

            var whyAnswers = service.getQuestionByName('whyInsurance').options;

            var whyAnswersResults = 0;

            if (whyAnswers[1]) {
                whyAnswersResults += 50;
            }

            if (whyAnswers[3]) {
                whyAnswersResults += 50;
            }

            return whyAnswersResults;
        };

        this.getCIProgress = function () {
            var included = service.getSelectedValue(service.getQuestionByName('whatIsCI'));
            if (included === true) {
                return 100;
            }
            if (included === false) {
                return 0;
            }

            var whyAnswers = service.getQuestionByName('whyInsurance').options;

            var whyAnswersResults = 0;

            if (whyAnswers[1]) {
                whyAnswersResults += 50;
            }

            if (whyAnswers[4]) {
                whyAnswersResults += 50;
            }

            return whyAnswersResults;
        };

        this.resetQuestion = function (questionName) {
            var question = service.getQuestionByName(questionName);

            _.each(question.options, function (o) {
                o.isSelected = false;
            });
        };

        this.percentage = {
            ip: 0,
            life: 0,
            tpd: 0,
            ci: 0
        };

        var waitingPeriods = [];
        var benefitPeriods = [];
        var tpdOccupationDefinition = [];

        this.questions = [
            {
                name: 'whyInsurance',
                type: 'whyInsurance',
                title: 'Why do you want insurance?',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/whyInsurance.template.html',
                options: [],
                onAnswered: function () {
                    service.percentage.ip = service.getIpProgress();
                    service.percentage.life = service.getLifeProgress();
                    service.percentage.ci = service.getCIProgress();
                    service.percentage.tpd = service.getTPDProgress();
                }
            },
            {
                name: 'whatIsLife',
                type: 'singleSelectButtons',
                planCode: 'DTH',
                title: 'What is Life Insurance?',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/lifeWhat.template.html',
                options: [
                    {id: 1, label: 'Skip Life Insurance for now', value: false, isSelected: false},
                    {id: 2, label: 'Include Life Insurance in quote', value: true, isSelected: false},
                ],
                onAnswered: function () {
                    service.percentage.life = service.getLifeProgress();
                },
                isAvailable: function () {
                    return !this.isDisabled;
                }
            },
            {
                name: 'whatIsTPD',
                type: 'singleSelectButtons',
                planCode: 'TPS',
                title: 'What is Total Permanent Disability Insurance (TPD)?',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/tpdWhat.template.html',
                options: [
                    {id: 1, label: 'Skip TPD for now', value: false, isSelected: false},
                    {id: 2, label: 'Include TPD in quote', value: true, isSelected: false},
                ],
                onAnswered: function () {
                    service.percentage.tpd = service.getTPDProgress();
                    if (!service.percentage.tpd) {
                        service.resetQuestion('occupationTPD');
                    }
                },
                isAvailable: function () {
                    return !this.isDisabled;
                }
            },
            {
                name: 'occupationTPD',
                type: 'singleSelectButtons',
                title: 'Covering your occupation',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/tpdOccupationType.template.html',
                options: tpdOccupationDefinition,
                isAvailable: function () {
                    if(!service.isParentQuestionSelected('whatIsTPD')){
                        return false;
                    }

                    var enabledOptions = _.filter(tpdOccupationDefinition, {isDisabled: false});

                    if(enabledOptions.length === 0){
                        return false;
                    }

                    if(enabledOptions.length === 1){
                        enabledOptions[0].isSelected = true;
                        return false;
                    }

                    return true;
                }
            },
            {
                name: 'whatIsCI',
                type: 'singleSelectButtons',
                planCode: 'TRS',
                title: 'What is Recovery Insurance?',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/ciWhat.template.html',
                options: [
                    {id: 1, label: 'Skip Recovery Insurance for now', value: false, isSelected: false},
                    {id: 2, label: 'Include Recovery Insurance in quote', value: true, isSelected: false},
                ],
                onAnswered: function () {
                    service.percentage.ci = service.getCIProgress();
                },
                isAvailable: function () {
                    return !this.isDisabled;
                }
            },
            {
                name: 'whatIsIP',
                type: 'singleSelectButtons',
                planCode: 'IP',
                title: 'What is Income Protection?',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/ipWhat.template.html',
                options: [
                    {id: 1, label: 'Skip Income Protection for now', value: false, isSelected: false},
                    {id: 2, label: 'Include Income Protection in quote', value: true, isSelected: false},
                ],
                onAnswered: function () {
                    service.percentage.ip = service.getIpProgress();
                    if (!service.percentage.ip) {
                        service.resetQuestion('ipWaitingPeriod');
                        service.resetQuestion('ipBenefitPeriod');
                    }
                },
                isAvailable: function () {
                    return !this.isDisabled;
                }
            },
            {
                name: 'ipWaitingPeriod',
                type: 'singleSelectRadio',
                title: 'Choose how long your waiting period will be',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/ipWaitingPeriod.template.html',
                options: waitingPeriods,
                isAvailable: isIPAvailable
            },
            {
                name: 'ipBenefitPeriod',
                type: 'singleSelectRadio',
                title: 'Choose how many years of Income Protection you want',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/ipBenefitPeriod.template.html',
                options: benefitPeriods,
                isAvailable: isIPAvailable
            },
            {
                name: 'coverOptions',
                type: 'multiSelectCheckboxes',
                title: 'What would you like to be covered for?',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/coverOptions.template.html',
                options: [
                    {id: 1, label: 'Illnesses (including cancer)', value: 'Illness', isSelected: true},
                    {id: 2, label: 'Accidents and serious injuries', value: 'Accidents', isSelected: true}
                ],
                isAvailable: function () {
                    return service.isParentQuestionSelected('whatIsLife') ||
                        service.isParentQuestionSelected('whatIsTPD') ||
                        service.isParentQuestionSelected('whatIsCI') ||
                        service.isParentQuestionSelected('whatIsIP');
                }
            },
            {
                name: 'sport',
                type: 'singleSelectButtons',
                title: 'Sports Cover',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/sport.template.html',
                options: [
                    {id: 1, label: 'Yes', value: true, isSelected: false},
                    {id: 2, label: 'No', value: false, isSelected: false}
                ],
                isAvailable: function () {
                    return service.checkQuestionSelectedAnswer('coverOptions', 'Accidents');
                }
            },
            {
                name: 'adventureSport',
                type: 'singleSelectButtons',
                title: 'Adventure Sports Cover',
                htmlText: '/client/appNeedsAnalysis/components/needsAnalysis/helpMeChoose/question/questionTemplates/adventureSport.template.html',
                options: [
                    {id: 1, label: 'Yes', value: true, isSelected: false},
                    {id: 2, label: 'No', value: false, isSelected: false},
                ],
                isAvailable: function () {
                    return service.checkQuestionSelectedAnswer('coverOptions', 'Accidents');
                }
            }
        ];

        this.setPlanVariables = function(plans) {

            var coverOptions = talContentService['coverSelection.coverOptions'];

            //TODO: repetitive code below, refactor to some common methods
            var ipPlan = _.find(plans, { planCode: 'IP' });

            //Setup Waiting Period options
            var waitingPeriodVariable = _.find(ipPlan.variables, { code: 'waitingPeriod' });
            var waitingPeriodContent = coverOptions.waitingPeriod.overrideOptions;
            waitingPeriods = _.map(waitingPeriodVariable.options, function (option) {
                var optionContent = _.find(waitingPeriodContent, {value:option.value});
                return {
                    id: option.value,
                    label: optionContent ? optionContent.overrideName || option.name : option.name,
                    value: option.value,
                    isAvailable: option.isAvailable,
                    isSelected: false
                };
            });

            var waitingPeriodsQuestion = _.find(service.questions, {name: 'ipWaitingPeriod'});
            waitingPeriodsQuestion.options = waitingPeriods;

            //Setup Benefit Period options
            var benefitPeriodVariable = _.find(ipPlan.variables, { code: 'benefitPeriod' });
            var benefitPeriodContent = coverOptions.benefitPeriod.overrideOptions;
            benefitPeriods = _.map(benefitPeriodVariable.options, function (option) {
                var optionContent = _.find(benefitPeriodContent, {value:option.value});
                return {
                    id: option.value,
                    label: optionContent ? optionContent.overrideName || option.name : option.name,
                    value: option.value,
                    isAvailable: option.isAvailable,
                    isSelected: false
                };
            });

            var benefitPeriodsQuestion = _.find(service.questions, {name: 'ipBenefitPeriod'});
            benefitPeriodsQuestion.options = benefitPeriods;

            //Setup Occupation Definition options
            var tpdPlan = _.find(plans, { planCode: 'TPS' });
            var occupationVariable = _.find(tpdPlan.variables, { code: 'occupationDefinition' });
            var occupationDefinitionContent = coverOptions.occupationDefinition.overrideOptions;
            tpdOccupationDefinition = _.map(occupationVariable.options, function (option) {
                var optionContent = _.find(occupationDefinitionContent, {value:option.value});
                return {
                    id: option.value,
                    label: optionContent ? optionContent.overrideName || option.name : option.name,
                    value: option.value,
                    isAvailable: option.isAvailable,
                    isDisabled: false,
                    isSelected: false
                };
            });

            var tpdQuestion = _.find(service.questions, {name: 'occupationTPD'});
            tpdQuestion.options = tpdOccupationDefinition;
        };

        this.setOccupationDefinitionAvailability = function (definitionId, isDisabled) {
            var definition = _.find(tpdOccupationDefinition, {id: definitionId});
            if (!definition) {
                throw 'unable to find definition for id: ' + definitionId;
            }

            definition.isDisabled = isDisabled;
        };

        this.updatePlansAvailability = function (plans) {
            _.each(plans, function (plan) {
                service.setPlanAvailability(plan.planCode, !plan.availability.isAvailable);
            });
        };

        this.setPlanAvailability = function (planCode, isDisabled) {
            var question = _.find(service.questions, {planCode: planCode});
            if (!question) {
                throw 'unable to find question for planCode: ' + planCode;
            }

            question.isDisabled = isDisabled;
        };

    }]);

})(angular.module('appNeedsAnalysis'));

