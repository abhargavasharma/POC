(function (module) {
    'use strict';

    //This service is to handle the notifications between sections so as alleviate the use of $broadcast events
    // and to allow specific updates to sections when there is a dependency on another section
    function talPolicySectionsNotificationService() {

        var initialisationNotificationEvents = [];
        var ratingFactorsNotificationEvents = [];
        var talusUiNotificationEvents = [];
        var personalDetailsNotificationEvents = [];
        var insurancePlansNotificationEvents = [];
        var premiumFrequencyNotificationEvents = [];
        var plansPremiumNotificationEvents = [];
        var policyNotesNotificationEvents = [];
        var planStatusChangeEvents = [];
        var underwritingStatusChangeEvents = [];
        var ownerTypeNotificationEvents = [];

        //To show validation on all sections that can be invalid when not selected
        var onInitialValidation = function() {
            _.each(initialisationNotificationEvents, function(notificationEvent){
                notificationEvent();
            });
        };

        var onTalusUiChange = function() {
            _.each(talusUiNotificationEvents, function(notificationEvent){
                notificationEvent();
            });
        };

        var onRatingFactorsChange = function() {
            _.each(ratingFactorsNotificationEvents, function(notificationEvent){
                notificationEvent();
            });
        };

        var onPersonalDetailsChange = function(personalDetails) {
            _.each(personalDetailsNotificationEvents, function(notificationEvent){
                notificationEvent(personalDetails);
            });
        };

        var onInsurancePlansChange = function(insurancePlans) {
            _.each(insurancePlansNotificationEvents, function(notificationEvent){
                notificationEvent(insurancePlans);
            });
        };

        var onPremiumFrequencyChange = function(premiumFrequency) {
            _.each(premiumFrequencyNotificationEvents, function(notificationEvent){
                notificationEvent(premiumFrequency);
            });
        };

        var onInsurancePlansPremiumChange = function(riskPremiumSummary) {
            _.each(plansPremiumNotificationEvents, function(notificationEvent){
                notificationEvent(riskPremiumSummary);
            });
        };

        var onNoteAddedChange = function () {
            _.each(policyNotesNotificationEvents, function (notificationEvent) {
                notificationEvent();
            });
        };

        var onPlanStatusChangeEvent = function (policyRiskPlanStatusesResult) {
            _.each(planStatusChangeEvents, function (notificationEvent) {
                notificationEvent(policyRiskPlanStatusesResult);
            });
        };

        var onUnderwritingStatusChangeEvent = function (underwritingStatus) {
            _.each(underwritingStatusChangeEvents, function (notificationEvent) {
                notificationEvent(underwritingStatus);
            });
        };

        var onOwnerTypeChangeEvent = function (underwritingStatus) {
            _.each(ownerTypeNotificationEvents, function (notificationEvent) {
                notificationEvent(underwritingStatus);
            });
        };

        var registerTalusUiChangeEvent = function(notificationFunction) {
            talusUiNotificationEvents.push(notificationFunction);
        };

        var registerRatingFactorsChangeEvent = function(notificationFunction) {
            ratingFactorsNotificationEvents.push(notificationFunction);
        };

        var registerPersonalDetailsChangeEvent = function(notificationFunction) {
            personalDetailsNotificationEvents.push(notificationFunction);
        };

        var registerOwnerTypeChangeEvent = function (notificationFunction) {
            ownerTypeNotificationEvents.push(notificationFunction);
        };

        var registerInitialisationChangeEvent = function(notificationFunction) {
            initialisationNotificationEvents.push(notificationFunction);
        };

        var registerInsurancePlansChangeEvent = function(notificationFunction) {
            insurancePlansNotificationEvents.push(notificationFunction);
        };

        var registerPremiumFrequencyChangeEvent = function(notificationFunction) {
            premiumFrequencyNotificationEvents.push(notificationFunction);
        };

        var registerPlansPremiumChangeEvent = function(notificationFunction) {
            plansPremiumNotificationEvents.push(notificationFunction);
        };
        var registerNoteAddedEvent = function (notificationFunction) {
            policyNotesNotificationEvents.push(notificationFunction);
        };

        var registerPlanStatusChangeEvent = function (notificationFunction) {
            planStatusChangeEvents.push(notificationFunction);
        };

        var registerUnderwritingStatusChangeEvent = function (notificationFunction) {
            underwritingStatusChangeEvents.push(notificationFunction);
        };

        return {
            onInitialValidation: onInitialValidation,
            onRatingFactorsChange: onRatingFactorsChange,
            onPersonalDetailsChange: onPersonalDetailsChange,
            onTalusUiChange: onTalusUiChange,
            onInsurancePlansChange: onInsurancePlansChange,
            onPremiumFrequencyChange: onPremiumFrequencyChange,
            onInsurancePlansPremiumChange: onInsurancePlansPremiumChange,
            onNoteAddedChange: onNoteAddedChange,
            onPlanStatusChangeEvent : onPlanStatusChangeEvent,
            onUnderwritingStatusChangeEvent: onUnderwritingStatusChangeEvent,
            onOwnerTypeChangeEvent: onOwnerTypeChangeEvent,

            registerRatingFactorsChangeEvent: registerRatingFactorsChangeEvent,
            registerInitialisationChangeEvent: registerInitialisationChangeEvent,
            registerTalusUiChangeEvent: registerTalusUiChangeEvent,
            registerInsurancePlansChangeEvent: registerInsurancePlansChangeEvent,
            registerPremiumFrequencyChangeEvent: registerPremiumFrequencyChangeEvent,
            registerPlansPremiumChangeEvent: registerPlansPremiumChangeEvent,
            registerNoteAddedEvent: registerNoteAddedEvent,
            registerPlanStatusChangeEvent: registerPlanStatusChangeEvent,
            registerUnderwritingStatusChangeEvent : registerUnderwritingStatusChangeEvent,
            registerPersonalDetailsChangeEvent: registerPersonalDetailsChangeEvent,
            registerOwnerTypeChangeEvent: registerOwnerTypeChangeEvent
        };
    }

    module.factory('talPolicySectionsNotificationService', talPolicySectionsNotificationService);
    talPolicySectionsNotificationService.$inject = [];

})(angular.module('salesPortalApp'));