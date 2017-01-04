(function (module) {
    'use strict';

    function talPolicyEditSectionsService($timeout, $q, SECTION) {

        var setDefaultSectionStates = function () {
            return {
                error: false,
                warning: false,
                valid: false,
                complete: false
            };
        };

        var createSection = function(heading, id, ignoreValidation){
            return { open: false, heading: heading, isModelValid: false, isCompleted: false, ignoreValidation: !!ignoreValidation, onActivationEvent: null, states: setDefaultSectionStates(), id: id};
        };

        var reviewSection = createSection('Your application is ready to submit');
        var createPolicyEditSections = function () {
            return {
                personalDetails: createSection('Customer Details', 'PersonalDetails'),
                ratingFactors: createSection('Rating Factors', 'RatingFactors'),
                underwriting: createSection('Underwriting', 'Underwriting'),
                insurancePlans: createSection('Cover Selection', 'InsurancePlans'),
                beneficiaries: createSection('Beneficiaries', 'Beneficiaries'),
                payment: createSection('Payment + Billing', 'Payment'),
                noteSection: createSection('Notes', 'NoteSection', true),
                interactionSection: createSection('Interaction History', 'InteractionSection', true),
                sendQuoteSection: createSection('Send To Customer', 'SendQuoteSection', true)
            };
        };

        var setSection = function(section){
            _currentActiveSection = section;
            _currentActiveSection.open = true;

            if(section.onActivationEvent){
                section.onActivationEvent();
            }
        };

        var _risks = null;
        var _currentActiveRisk = null;
        var _currentActiveSection = null;

        var initialise = function(risks) {
            var promise = $q.defer();

            _risks = risks;

            _.each(_risks, function (risk) {
                risk.sections = createPolicyEditSections();
            });

            //for now default to first risk
            //need a function to set active risk when we do multi-risk
            _currentActiveRisk = _risks[0];

            setSectionWhenAllLoaded(_currentActiveRisk.sections.personalDetails, promise);

            return promise;
        };

        //bit of a hack to make sure we only set the active section when all on activation events
        //have been attached by the child components
        var setSectionWhenAllLoaded = function(sectionToActivate, promise) {
            var ready = true;

            _.each(_risks, function (risk) {
                _.each(risk.sections, function (section) {
                    if(!section.onActivationEvent) {
                        ready = false;
                    }
                });
            });

            if(ready) {
                setSection(sectionToActivate);
                promise.resolve();
            }
            else {
                $timeout(function() {
                    setSectionWhenAllLoaded(sectionToActivate, promise);
                }, 500);
            }
        };

        var activateSection = function(section){

            if (_currentActiveSection.isModelValid) {
                if (_currentActiveSection.onDeactivation) {
                    _currentActiveSection.onDeactivation().then(function() {
                        _currentActiveSection.open = false;
                        setSection(section);
                    });
                } else {
                    _currentActiveSection.open = false;
                    setSection(section);
                }
            }
        };

        var applyPersonalDetailsChangeToSelectedRisk = function(change) {
            _currentActiveRisk.firstName = change.personalDetails.firstName;
            _currentActiveRisk.surname = change.personalDetails.surname;
        };

        var addSectionState = function(section, state){
            section.states[state] = true;
        };

        var removeSectionState = function(section, stateToRemove){
            section.states[stateToRemove] = false;
        };

        var setSectionStatesOnError = function(section){
            addSectionState(section, SECTION.STATUS.ERROR);
        };

        var setSectionStatesOnSuccess = function(section, warnings){
            removeSectionState(section, SECTION.STATUS.WARNING);
            removeSectionState(section, SECTION.STATUS.ERROR);
            addSectionState(section, SECTION.STATUS.VALID);
            if(warnings && warnings.length && warnings.length > 0){
                addSectionState(section, SECTION.STATUS.WARNING);
            }
        };
	
	    var clearSectionState = function(section){
            section.states = setDefaultSectionStates();
        };

        var sectionInState = function(section, state){
            return section.states[state];
        };

        return {
            setDefaultSectionStates: setDefaultSectionStates,
            setSectionStatesOnSuccess: setSectionStatesOnSuccess,
            setSectionStatesOnError: setSectionStatesOnError,
            sectionInState: sectionInState,
            removeSectionState: removeSectionState,
            addSectionState: addSectionState,
            initialise : initialise,
            activateSection: activateSection,
            applyPersonalDetailsChangeToSelectedRisk: applyPersonalDetailsChangeToSelectedRisk,
            clearSectionState: clearSectionState,
            reviewSection: reviewSection
        };
    }

    module.factory('talPolicyEditSectionsService', talPolicyEditSectionsService);
    talPolicyEditSectionsService.$inject = ['$timeout', '$q', 'SECTION'];

})(angular.module('salesPortalApp'));