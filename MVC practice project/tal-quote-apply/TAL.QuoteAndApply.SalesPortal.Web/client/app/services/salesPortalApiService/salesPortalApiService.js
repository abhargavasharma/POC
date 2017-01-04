'use strict';

angular.module('salesPortalApp').service('talSalesPortalApiService', function ($http) {

    this.initClient = function () {
        return $http.get('/api/policy/init');
    };

    this.createClient = function (createClientRequest) {
        return $http.post('/api/policy/create', createClientRequest);
    };

    this.getPlans = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/plan/getPlansAndCovers';
        return $http.get(url);
    };

    this.updatePlan = function (quoteReferenceNumber, riskId, planId, planRequest) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/plan/edit';
        return $http.post(url, planRequest);
    };

    this.getRiskPersonalDetails = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/lifeInsuredDetails';
        return $http.get(url);
    };

    this.getRiskBeneficiaryDetails = function (quoteReferenceNumber, riskId, beneficiaryId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/beneficiaries/' + beneficiaryId;
        return $http.get(url);
    };

    this.getRiskAllBeneficiaries = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/beneficiaries/';
        return $http.get(url);
    };

    this.validateBeneficiaries = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/beneficiaries/validate';
        return $http.get(url);
    };

    this.removeBeneficiary = function (quoteReferenceNumber, riskId, beneficiaryId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/beneficiaries/' + beneficiaryId;
        return $http.delete(url);
    };

    this.updateBeneficiaryOptions = function (quoteReferenceNumber, riskId, options) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/beneficiaries/options';
        return $http.post(url, options);
    };

    this.getBeneficiaryOptions = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/beneficiaries/options';
        return $http.get(url);
    };

    this.getBenefitRelationships = function () {
        var url = '/api/reference/beneficiaryRelationships';
        return $http.get(url);
    };

    this.getMaxBeneficiaries = function (quoteReferenceNumber) {
        var url = '/api/settings/' + quoteReferenceNumber + '/maxBeneficiaries';
        return $http.get(url);
    };

    this.updateRiskBeneficiariesDetails = function (quoteReferenceNumber, riskId, beneficiaryDetailsRequests) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/beneficiaries/';
        return $http.post(url, beneficiaryDetailsRequests);
    };
    
    this.updateLifeInsuredDetails = function (quoteReferenceNumber, riskId, updateLifeInsuredDetailsRequest) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/lifeInsuredDetails';
        return $http.post(url, updateLifeInsuredDetailsRequest);
    };

    this.getPolicyOwnerDetails = function (quoteReferenceNumber) {
        var url = '/api/policy/' + quoteReferenceNumber + '/ownerDetails';
        return $http.get(url);
    };

    this.updatePolicyOwnerDetails = function (quoteReferenceNumber, updateOwnerDetailsRequest) {
        var url = '/api/policy/' + quoteReferenceNumber + '/ownerDetails';
        return $http.post(url, updateOwnerDetailsRequest);
    };

    this.getUnderwritingCompleteStatus = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/underwriting/status';
        return $http.get(url);
    };

    this.updateKnownQuestionAnswerInUnderwriting = function (quoteReferenceNumber, riskId, quesitonAnswerRequest) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/underwriting/answer';
        return $http.post(url, quesitonAnswerRequest);
    };

    this.getRiskTalusUiUrl = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/underwriting/talusUiUrl';
        return $http.get(url);
    };

    this.getPlanAvailability = function (quoteReferenceNumber, riskId, selectedFeaturesRequest) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/plan/availability';
        return $http.post(url, selectedFeaturesRequest);
    };

    this.getRiskRatingFactors = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/ratingfactors';
        return $http.get(url);
    };

    this.updateRiskRatingFactors = function (quoteReferenceNumber, riskId, updateRatingFactorsRequest) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/ratingfactors';
        return $http.post(url, updateRatingFactorsRequest);
    };

    this.updatePolicyNote = function (quoteReferenceNumber, policyNote) {
        var url = '/api/policy/' + quoteReferenceNumber + '/note';
        return $http.post(url, policyNote);
    };

    this.getPayments = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/payment/risk/' + riskId + '/paymentOptions';
        return $http.get(url);
    };

    this.updatePayment = function (quoteReferenceNumber, riskId, paymentRequest, paymentType) {
        var url = '/api/policy/' + quoteReferenceNumber + '/payment/risk/' + riskId + '/' + paymentType;
        return $http.post(url, paymentRequest);
    };

    this.searchSuperannuationFund = function (fundName) {
        var url = '/api/superannuation/search/organisation/' + fundName;
        return $http.get(url);
    };

    this.getAvailablePaymentOptionsForProduct = function (quoteReferenceNumber) {
        var url = '/api/settings/' + quoteReferenceNumber + '/paymentOptions';
        return $http.get(url);
    };

    this.getAvailableCreditCardTypes = function (quoteReferenceNumber) {
        var url = '/api/settings/' + quoteReferenceNumber + '/creditCards';
        return $http.get(url);
    };

    this.submitPolicy = function (quoteReferenceNumber) {
        var url = '/api/policy/' + quoteReferenceNumber + '/inforce';
        return $http.post(url);
    };

    this.searchClients = function (searchCriteria) {
        var url = '/api/search/clients';
        return $http.post(url, searchCriteria);
    };

    this.getInteractions = function (quoteReferenceNumber) {
        var url = '/api/policy/' + quoteReferenceNumber + '/interactions';
        return $http.get(url);
    };

    this.getNotes = function (quoteReference) {
        var url = '/api/policy/' + quoteReference + '/notes';
        return $http.get(url);
    };

    this.getPolicyPremiumFrequency = function (quoteReferenceNumber) {
        var url = '/api/policy/' + quoteReferenceNumber + '/premiumfrequency';
        return $http.get(url);
    };

    this.updatePolicyPremiumFrequency = function (quoteReferenceNumber, updatePremiumFrequencyRequest) {
        var url = '/api/policy/' + quoteReferenceNumber + '/premiumfrequency';
        return $http.post(url, updatePremiumFrequencyRequest);
    };

    this.getRiskPremiumSummary = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/premium';
        return $http.get(url);
    };
    this.getSyncInterview = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/underwriting/sync';
        return $http.get(url);
    };
    this.getSyncInterviewByInterviewId = function (interviewId) {
        var url = '/api/postsales/manageInterview/' + interviewId + '/find';
        return $http.get(url);
    };
    this.loginRequest = function (loginRequest) {
        var url = '/api/login';
        return $http.post(url, loginRequest);
    };
    this.saveSelectedBrand = function (brandName) {
        var url = '/api/login/brand';
        return $http.post(url, brandName);
    };
    this.getBrandNamesForUser = function(){
        var url = '/api/login/brands';
        return $http.get(url);
    };
    this.createReferral = function (quoteReferenceNumber) {
        var url = '/api/policy/' + quoteReferenceNumber + '/referral';
        return $http.post(url, {});
    };
    this.completeReferral = function (quoteReferenceNumber) {
        var url = '/api/policy/' + quoteReferenceNumber + '/referral/complete';
        return $http.post(url, {});
    };
    this.getUnderwriterReferrals = function () {
        var url = '/api/dashboard/referrals';
        return $http.get(url);
    };
    this.updateAssignedTo = function (quoteReferenceNumber, userObj) {
        var url = '/api/policy/' + quoteReferenceNumber + '/assign';
        return $http.post(url, userObj);
    };
    this.getPartyConsent = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/partyConsent';
        return $http.get(url);
    };
    this.updatePartyConsent = function (quoteReferenceNumber, riskId, updateObj) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/partyConsent';
        return $http.post(url, updateObj);
    };
    this.getAgentQuotes = function () {
        var url = '/api/dashboard/quotes';
        return $http.get(url);
    };
    this.getAgentQuotes = function (getAgentQuotesRequest) {
        var url = '/api/dashboard/quotes';
        return $http.post(url, getAgentQuotesRequest);
    };
    this.getPolicyProgress = function(quoteReferenceNumber) {
        var url = '/api/policy/' + quoteReferenceNumber + '/progress';
        return $http.get(url);
    };
    this.updatePolicyProgress = function (quoteReferenceNumber, updateObj) {
        var url = '/api/policy/' + quoteReferenceNumber + '/progress';
        return $http.post(url, updateObj);
    };
    this.updatePolicyOwnerType = function (quoteReferenceNumber, newOwnerType) {
        var url = '/api/policy/' + quoteReferenceNumber + '/ownerType/' + newOwnerType;
        return $http.post(url);
    };
    this.editPolicy = function (quoteReferenceNumber, editAction) {
        var url = '/api/policy/' + quoteReferenceNumber + '/edit/' + editAction;
        return $http.get(url);
    };
    this.getAvailableOwnerTypes = function (quoteReferenceNumber) {
        var url = '/api/settings/' + quoteReferenceNumber + '/ownerTypes';
        return $http.get(url);
    };
    this.getCorrespondenceSummary = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/correspondence';
        return $http.get(url);
    };
    this.sendCorrespondence = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/correspond';
        return $http.post(url);
    };

    this.getUnderwritingForRisk = function (quoteReferenceNumber, riskId) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/underwriting/interview';
        return $http.get(url);
    };

    this.answerQuestionForRisk = function (quoteReferenceNumber, riskId, questionAnswer) {
        var url = '/api/policy/' + quoteReferenceNumber + '/risk/' + riskId + '/underwriting/question';
        return $http.post(url, questionAnswer);
    };
});