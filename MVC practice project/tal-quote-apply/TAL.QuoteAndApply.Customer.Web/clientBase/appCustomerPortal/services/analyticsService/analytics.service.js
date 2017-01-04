'use strict';

angular.module('appCustomerPortal').service('talAnalyticsService', function ($window, viewBagConfig, $log) {
    $log.debug($window.digitalData.applicationData);
    var defaultPageName = ($window.digitalData && $window.digitalData.page && $window.digitalData.page.pageInfo) ? $window.digitalData.page.pageInfo.pageName : '';
    var defaultPathToPurchase = viewBagConfig.journeySource || 'Unknown';

    var getAge = function (birthdate) {
        var dateParts = birthdate.split('/');
        var ageDifMs = Date.now() - new Date(dateParts[2], (dateParts[1] - 1), dateParts[0]).getTime();
        var ageDate = new Date(ageDifMs); // miliseconds from epoch
        return Math.abs(ageDate.getUTCFullYear() - 1970);
    };

    var sharedTracking = {
        trackSaveQuote: function (event, pageName, pathToPurchase, quoteNumber) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': event,
                    'quoteNumber': quoteNumber,
                    'pageName': pageName,
                    'pathToPurchaseType': pathToPurchase
                });
            }
        },
        trackSaveQuoteButton: function (quoteNumber) {
            return this.trackSaveQuote('saveQuoteIntention', defaultPageName, defaultPathToPurchase, quoteNumber);
        },
        trackSaveQuotePersonalDetailsSubmit: function (quoteNumber) {
            return this.trackSaveQuote('saveQuotePersonalDetails', defaultPageName, defaultPathToPurchase, quoteNumber);
        },
        trackSaveQuotePasswordSubmit: function (quoteNumber) {
            return this.trackSaveQuote('saveQuotePassword', defaultPageName, defaultPathToPurchase, quoteNumber);
        }
    };

    var createQuoteTracking = {
        trackNewQuote : function(dob, gender, smoker, occupation, industry, annualIncome) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'createQuote',
                    'dob': dob,
                    'age': getAge(dob),
                    'gender': gender,
                    'smoker': smoker ? 'Yes': 'No',
                    'occupation': occupation,
                    'industry': industry,
                    'income': annualIncome,
                    'pageName': defaultPageName,
                    'pathToPurchaseType': defaultPathToPurchase
                });
            }
        }
    };

    var coverTracking = {
        
        trackPremiumType: function(event, pageName, pathToPurchase, premiumType) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': event,
                    'premiumType': premiumType,
                    'pageName': pageName,
                    'pathToPurchaseType': pathToPurchase
                });
            }
        },
        trackPlan: function(event, pageName, pathToPurchase, plan, totalPremium, premiumFrequency) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                var analyticsPayload = {
                    'type': 'analytics',
                    'event': event,
                    'planCode': plan.planCode,
                    'isPlanOn': plan.isSelected ? 'Yes' : 'No',
                    'planPrice': plan.premium,
                    'totalPremium': totalPremium,
                    'paymentFrequency': premiumFrequency,
                    'coverAmount': plan.selectedCoverAmount,
                    'pageName': pageName,
                    'inflationProtection': _.find(plan.variables, { code: 'linkedToCpi' }).selectedValue ? 'Yes' : 'No',
                    'pathToPurchaseType': pathToPurchase
                };
                _.each(plan.covers, function (c) {
                    analyticsPayload['cover' + c.code] = c.isSelected ? 'Yes' : 'No';
                });

                $window.digitalData.events.push(analyticsPayload);
            }
        },
        trackPlans: function (event, pageName, pathToPurchase, plans, totalPremium, premiumFrequency) {

            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                var analyticsPayload = {
                    'type': 'analytics',
                    'event': event,
                    'pageName': pageName,
                    'totalPremium': totalPremium,
                    'paymentFrequency': premiumFrequency,
                    'pathToPurchaseType': pathToPurchase
                };
                _.each(plans, function (plan) {
                    analyticsPayload['plan' + plan.planCode] = plan.isSelected ? 'Yes' : 'No';
                    _.each(plan.covers, function (cover) {
                        analyticsPayload['cover' + cover.code] = cover.isSelected ? 'Yes' : 'No';
                    });
                });
                $window.digitalData.events.push(analyticsPayload);
            }
        }

    };

    var coverSelectionTracking = {
        pageName: defaultPageName || 'Select Cover',
        trackPlan: function(plan, totalPremium, premiumFrequency) {
            return coverTracking.trackPlan('updateCover', this.pageName, defaultPathToPurchase, plan, totalPremium, premiumFrequency);
        },
        trackAllPlans: function(plans, totalPremium, premiumFrequency) {
            return coverTracking.trackPlans('loadCover', this.pageName, defaultPathToPurchase, plans, totalPremium, premiumFrequency);
        }
    };

    var reviewCoverTracking = {
        pageName: defaultPageName || 'Review Cover',

        trackPlan: function(plan, totalPremium, premiumFrequency) {
            return coverTracking.trackPlan('reviewUpdateCover', this.pageName, defaultPathToPurchase, plan, totalPremium, premiumFrequency);
        },
        trackPremiumType: function(premiumType) {
            return coverTracking.trackPremiumType('reviewUpdateCover', this.pageName, defaultPathToPurchase, premiumType);
        },
        trackSubmission: function() {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'submitApplication',
                    'pageName': this.pageName,
                    'pathToPurchaseType': defaultPathToPurchase
                });
            }
        },
        trackAllPlans: function (plans, totalPremium, premiumFrequency) {
            return coverTracking.trackPlans('reviewLoadCover', this.pageName, defaultPathToPurchase, plans, totalPremium, premiumFrequency);
        }
    };

    var purchaseTracking = {
        pageName: defaultPageName || 'Purchase',

        trackSubmission: function(nBeneficiaries, paymentType) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'registerWithTalPolicy',
                    'pageName': this.pageName,
                    'pathToPurchaseType': defaultPathToPurchase,
                    'numberOfBeneficiaries' : nBeneficiaries,
                    'paymentType': paymentType
                });
            }
        }
    };

    var needsAnalysisTracking = {
        pageName: defaultPageName || 'Needs Analysis',

        sections : {
            incomeProtection : 'Income Protection',
            lifeInsurance : 'Life Cover',
            recoverInsurance : 'Recovery Insurance',
            disabilityInsurance : 'TPD Cover'
        },

        trackPreviousButtonForLife : function(pageName) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'previousButton',
                    'pathToPurchase': 'Help me Choose',
                    'stepOn': this.sections.lifeInsurance,
                    'pageName': pageName
                });
            }
        },

        trackPreviousButtonForTpd : function(pageName) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'previousButton',
                    'pathToPurchase': 'Help me Choose',
                    'stepOn': this.sections.tpdCover,
                    'pageName': pageName
                });
            }
        },

        trackPreviousButtonForRi : function(pageName) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'previousButton',
                    'pathToPurchase': 'Help me Choose',
                    'stepOn': this.sections.recoverInsurance,
                    'pageName': pageName
                });
            }
        },

        trackPreviousButtonForIp : function(pageName) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'previousButton',
                    'pathToPurchase': 'Help me Choose',
                    'stepOn': this.sections.incomeProtection,
                    'pageName': pageName
                });
            }
        },

        trackWhyDoYouWant : function(isSupportPassAway, isSupportFamilyIllnessInjury, isMaintainIncome, isPayOffDebts, isReceiveLumpSumIllnessInjury) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'confirmNeedsAnalysis',
                    'pathToPurchase': 'Help me Choose',
                    'supportPassaway': isSupportPassAway ? 'Yes' : 'No',
                    'supportIllInjured': isSupportFamilyIllnessInjury ? 'Yes' : 'No',
                    'incomeIllInjured': isMaintainIncome ? 'Yes' : 'No',
                    'payOffDebts': isPayOffDebts ? 'Yes' : 'No',
                    'protectMySelf': isReceiveLumpSumIllnessInjury ? 'Yes' : 'No'
                });
            }
        },

        trackLifeInsuranceInclusions: function (includeInQuote) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': includeInQuote ? 'includeCover' : 'skipCover',
                    'pathToPurchase': 'Help me Choose',
                    'yourNeeds': this.sections.lifeInsurance
                });
            }
        },

        trackTpdInsuranceInclusion: function (includeInQuote) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': includeInQuote ? 'includeCover' : 'skipCover',
                    'pathToPurchase': 'Help me Choose',
                    'yourNeeds': this.sections.disabilityInsurance
                });
            }
        },

        trackRiInclusion: function (includeInQuote) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': includeInQuote ? 'includeCover' : 'skipCover',
                    'pathToPurchase': 'Help me Choose',
                    'yourNeeds': this.sections.recoverInsurance
                });
            }
        },

        trackIpInsuranceInclusion: function (includeInQuote) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': includeInQuote ? 'includeCover' : 'skipCover',
                    'pathToPurchase': 'Help me Choose',
                    'yourNeeds': this.sections.incomeProtection
                });
            }
        },

        trackIpInsuranceCovers: function(isInjuryCover, isIllnessCover, isSportsCover) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'whatIncomeProtectionProductCover',
                    'unableWorkInjury': isInjuryCover,
                    'unableWorkIllness': isIllnessCover,
                    'unableWorkSportsInjury': isSportsCover
                });
            }
        },

         trackLifeInsuranceCovers: function(isAccidentCover, isIllnessCover, isSportsCover) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'whatLifeProductCover',
                    'diesAccident': isAccidentCover,
                    'dieIllness': isIllnessCover,
                    'dieSportAccident': isSportsCover
                });
            }
        },

        trackTpdInsuranceCovers: function(isAccidentCover, isIllnessCover, isSportsCover) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'whatPermanentDisabilityProductCover',
                    'disabledAccident': isAccidentCover,
                    'disabledIllness': isIllnessCover,
                    'disabledSportAccident': isSportsCover
                });
            }
        },

        trackRiCovers: function(isCancerCover, isIllnessCover, isInjuryCover) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'whatRecoveryInsuranceProductCover',
                    'diagnosedCancer': isCancerCover,
                    'sufferSeriousInjury': isInjuryCover,
                    'specifiedIllness': isIllnessCover
                });
            }
        },

        trackWaitingPeriodIpCover: function(weeks) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'waitingPeriodIncomeProtectionCover',
                    'pageName': 'Waiting period for Income Protection',
                    'waitingPeriod': weeks + ' Weeks'
                });
            }
        },

        trackDurantionIpCover: function(years) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'lengthIncomeProtectionCoverage',
                    'pageName': 'Length of Income Protection coverage',
                    'payoutPeriod': years + ' Years'
                });
            }
        },

        trackPersonalInformationOne: function(dob, gender) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'questionOne',
                    'pageName': 'Birth date and gender',
                    'born': dob,
                    'gender': gender
                });
            }
        },

        trackPersonalInformationTwo: function(tobaccoUsage) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'questionTwo',
                    'pageName': 'Smoking or non-smoking',
                    'useTabaccoProducts': tobaccoUsage
                });
            }
        },

        trackPersonalInformationThree: function(industry, occupation, annualIncome) {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'questionThree',
                    'pageName': 'Occupation',
                    'industry': industry,
                    'role': occupation,
                    'annualIncome': annualIncome
                });
            }
        },

        trackQuestion: function(name, optionsArr) {
            if (name === 'whyInsurance') {
                this.trackWhyDoYouWant(optionsArr[0], optionsArr[1], optionsArr[2], optionsArr[3], optionsArr[4]);
            }
        }
    };

    function truncate(str, n){
        return (str.length > n) ? str.substr(0,n-1) : str;
    }

    var qualifificationTracking = {
        pageName: defaultPageName || 'About Me',

        trackQuestion: function (questionText, answerText, category, contextualParentAnswer) {
            var shortQuestionText = truncate(questionText, 50);
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'answerQuestion',
                    'pathToPurchaseType': defaultPathToPurchase,
                    'pageName': this.pageName,
                    'questionText': shortQuestionText,
                    'answerText' : answerText,
                    'category': category,
                    'questionContext': contextualParentAnswer
                });
            }
        },

        trackCompletionOfQuestions: function() {
            if ($window.digitalData && $window.digitalData.events && $window.digitalData.events.push) {
                $window.digitalData.events.push({
                    'type': 'analytics',
                    'event': 'questionsComplete',
                    'pathToPurchaseType': defaultPathToPurchase,
                    'pageName': this.pageName
                });
            }
        }
    };

    this.createQuote = createQuoteTracking;
    this.coverSelection = coverSelectionTracking;
    this.helpMeChooseSection = needsAnalysisTracking;
    this.reviewSection = reviewCoverTracking;
    this.purchaseSection = purchaseTracking;
    this.qualificationSection = qualifificationTracking;
    this.shared = sharedTracking;

});