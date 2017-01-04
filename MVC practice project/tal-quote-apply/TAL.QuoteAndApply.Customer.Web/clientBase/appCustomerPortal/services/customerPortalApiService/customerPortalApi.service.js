'use strict';

angular.module('appCustomerPortal').service('talCustomerPortalApiService', function ($http) {

    /* Put all api calls in here for now, can split out if it gets too big */

    var getCacheBustedUrl = function(originalUrl) {
        //To avoid request being cached, add unique timestamp to end of url
        var timeStamp = new Date().getTime();
        var uniqueUrl = originalUrl + '?_ts=' + timeStamp;
        return uniqueUrl;
    };

    this.initQuote = function () {
        return $http.get(getCacheBustedUrl('/api/policy/init'));
    };

    this.createQuote = function (basicInfo) {
        return $http.post('/api/policy/create', basicInfo);
    };

    this.createQuoteForHelpMeChoose = function (basicInfo) {
        return $http.post('/api/policy/create-for-help-me-choose', basicInfo);
    };

    this.validateQuote = function (basicInfo) {
        return $http.post('/api/policy/validate', basicInfo);
    };

    this.validateIncome = function (incomeVM) {
        return $http.post('/api/policy/validate/income', incomeVM);
    };

    this.validateGeneralInformation = function (vm) {
        return $http.post('/api/policy/validate/generalInformation', vm);
    };

    this.validateAge = function (vm) {
        return $http.post('/api/policy/validate/age', vm);
    };

    this.getRisks = function() {
        return $http.get(getCacheBustedUrl('/api/qualification/risks'));
    };
    
    this.getUnderwritingForRisk = function (riskId) {
        return $http.get(getCacheBustedUrl('/api/qualification/risk/' + riskId));
    };

    this.answerQuestionForRisk = function (riskId, questionAnswer) {
        return $http.post('/api/qualification/risk/' + riskId, questionAnswer);
    };

    this.validateUnderwritingForRisk = function (riskId) {
        return $http.get(getCacheBustedUrl('/api/qualification/risk/' + riskId + '/validate'));
    };

    this.getReviewForRisk = function (riskId) {
        var url = '/api/review/risk/' + riskId;
        return $http.get(getCacheBustedUrl(url));
    };

    this.switchQuestionChoice = function (riskId, questionAnswer) {
        return $http.post('/api/review/risk/' + riskId, questionAnswer);
    };

    this.updatePremiumTypeForRisk = function(riskId, premiumType) {
        var updatePremiumTypeRequest = {
            premiumType: premiumType
        };

        return $http.post('/api/review/risk/' + riskId + '/PremiumType', updatePremiumTypeRequest);
    };

    this.validateReviewForRisk = function(riskId) {
        var url = '/api/review/risk/' + riskId + '/validate';
        return $http.get(getCacheBustedUrl(url));
    };

    this.getCoverSelectionForRisk = function (riskId) {
        return $http.get(getCacheBustedUrl('api/coverselection/risk/' + riskId));
    };

    this.setCalcResultsForRisk = function (data, useResultConfirmationRequired) {
        data.results = JSON.stringify(data.results);
        data.assumptions = JSON.stringify(data.assumptions);
        data.useResultConfirmationRequired = useResultConfirmationRequired;

        return $http.post('api/policy/set-calc-results', data);
    };

    this.useCalcResultsForRisk = function (riskId) {
        return $http.post('api/coverselection/risk/' + riskId + '/use-calc-results');
    };

    this.updateCoverSelectionForRisk = function (riskId, applicationState) {
        return $http.post('api/coverselection/risk/' + riskId, applicationState);
    };

    this.updateMultiCoverSelectionForRisk = function (riskId, applicationState) {
        return $http.post('api/coverselection/risk/' + riskId + '/multi', applicationState);
    };

    this.validateCoversForRisk = function (riskId) {
        return $http.get(getCacheBustedUrl('/api/coverselection/risk/' + riskId + '/validate'));
    };

    this.validateCoversAndProceedForRisk = function (riskId) {
        return $http.get(getCacheBustedUrl('/api/coverselection/risk/' + riskId + '/proceed'));
    };

    this.attachRiderForRisk = function (riskId, planToBecomeRider, planToAttachTo) {
        var attachRiderRequest = {
            planToBecomeRiderCode: planToBecomeRider,
            planToHostRiderCode: planToAttachTo
        };
        return $http.post('/api/coverselection/risk/' + riskId + '/attach', attachRiderRequest);
    };

    this.attachRiderForRiskMulti = function (riskId, attachRequests) {
        var attachRiderRequest = _.map(attachRequests, function (request) {
            return {
                planToBecomeRiderCode: request.riderCode,
                planToHostRiderCode: request.hostCode
            };
        });

        return $http.post('/api/coverselection/risk/' + riskId + '/attach/multi', {requests: attachRiderRequest});
    };

    this.detachRiderForRisk = function (riskId, riderCode) {
        var detachRiderRequest = {
            riderCode: riderCode
        };
        return $http.post('/api/coverselection/risk/' + riskId + '/detach', detachRiderRequest);
    };

    this.initPurchase = function () {
        return $http.get(getCacheBustedUrl('/api/purchase'));
    };

    this.createPurchase = function (createPurchaseRequest) {
        return $http.post('/api/purchase/risk/' + createPurchaseRequest.riskId, createPurchaseRequest);
    };

    this.removeBeneficiary = function (riskId, beneficiaryId) {
        var url = '/api/purchase/risk/' + riskId + '/beneficiaries/' + beneficiaryId;
        return $http.delete(url);
    };

    this.getMaxBeneficiaries = function () {
        var url = '/api/settings/maxBeneficiaries';
        return $http.get(url);
    };

    this.getAvailablePaymentOptionsForProduct = function () {
        var url = '/api/settings/paymentOptions';
        return $http.get(url);
    };

    this.getQuoteRetrievalAvailability = function () {
        var url = '/api/settings/isQuoteRetrivalAvailable';
        return $http.get(url);
    };

    this.getAvailableCreditCardTypes = function () {
        var url = '/api/settings/creditCardTypes';
        return $http.get(url);
    };

    this.getBenefitRelationships = function () {
        var url = '/api/reference/beneficiaryRelationships';
        return $http.get(getCacheBustedUrl(url));
    };

    this.appSaveGatePersonalDetails = function (riskId, appSave) {
        var url = '/api/save/risk/' + riskId;
        return $http.post(url, appSave);
    };

    this.requestCallback = function (data) {
        var url = '/api/chat/callback';
        return $http.post(url, data);
    };

    this.getChatAvailability = function () {
        var url = '/api/chat/available' ;
        return $http.get(getCacheBustedUrl(url));
    };

    this.getChatAvailabilityAndCreateInteraction = function () {
        var url = '/api/chat/availableAndCreateInteraction';
        return $http.get(getCacheBustedUrl(url));
    };

    this.appSaveGatePassword = function (riskId, password) {
        var request = {
            password: password
        };
        var url = '/api/save' + '/risk/' + riskId + '/createLogin';
        return $http.post(url, request);
    };

    this.retrieveQuote = function(retrieveQuoteRequest) {
        var url = '/api/retrieve';
        return $http.post(url, retrieveQuoteRequest);
    };

    this.getContactDetails = function (riskId){
        var url = '/api/save' + '/risk/' + riskId + '/contactDetails' ;
        return $http.get(url);
    };
});