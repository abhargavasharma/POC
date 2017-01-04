'use strict';
angular.module('salesPortalApp').directive('talClientSearch', function () {
    return {
        templateUrl: '/client/app/components/clientSearch/clientSearch.template.html',
        restrict: 'E',
        scope: {
            role: '@',
            externalRefRequired: '=',
            externalRefLabel:'@'
        },
        controllerAs: 'ctrl',
        bindToController: {
            cancelUrl:'@'
        },
        controller: 'talClientSearchController'
    };
});

angular.module('salesPortalApp').controller('talClientSearchController',
    function ($scope, talSalesPortalApiService, talFormModelStateService, $window, $uibModal) {
        
        var ctrl = this;
        var searchModeQuoteRef = 'QUOTE_REF';
        var searchModeLead = 'LEAD';
        var searchModeParty = 'PARTY';
        ctrl.validForCreateNewClient = $scope.role === 'Agent';
        ctrl.externalRefRequired = $scope.externalRefRequired;
        ctrl.externalRefLabel = $scope.externalRefLabel;
        function getEmptySearchResults(){
            return {
                quotes: []
            };
        }
     
        function search() {
            $scope.searchData = ctrl.searchCriteria;
            ctrl.loadingPromise = talSalesPortalApiService.searchClients(ctrl.searchCriteria)
                .then(function(results){
                    ctrl.searchPerformed = true;
                    ctrl.searchResults = results.data;
                    if(results.data.resultType === 'Leads'){
                        var onlyLeadResult = results.data.quotes[0];
                        $scope.searchData.dateOfBirth = onlyLeadResult.ownerDateOfBirth;
                        $scope.searchData.gender = onlyLeadResult.ownerGender;
                        $scope.searchData.leadId = onlyLeadResult.leadId;
                    }
                    talFormModelStateService.updateCleanModelState($scope);
                })
                .catch(function(response){
                    var modelState = response.data;
                    talFormModelStateService.updateModelState(modelState, $scope);
                    ctrl.searchResults = getEmptySearchResults();
                });
        }

        function searchByQuoteRefernce(){
            resetBySearchMode(searchModeQuoteRef);
            search();
        }

        function searchByLead(){
            resetBySearchMode(searchModeLead);
            search();
        }

        function searchByParty() {
            resetBySearchMode(searchModeParty);
            search();
        }

        function resetBySearchMode(searchMode) {

            if(searchMode === searchModeQuoteRef) {
                ctrl.searchCriteria.searchOnQuoteReference = true;
                ctrl.searchCriteria.searchOnLeadId = false;
                ctrl.searchCriteria.searchOnParty = false;
                ctrl.searchCriteria.leadId = null;
                ctrl.searchCriteria.firstName = null;
                ctrl.searchCriteria.surname = null;
                ctrl.searchCriteria.dateOfBirth = null;
                ctrl.searchCriteria.mobileNumber = null;
                ctrl.searchCriteria.homeNumber = null;
                ctrl.searchCriteria.email = null;
                ctrl.searchCriteria.externalCustomerReference = null;
            }
            else if(searchMode === searchModeLead) {
                ctrl.searchCriteria.searchOnQuoteReference = false;
                ctrl.searchCriteria.searchOnLeadId = true;
                ctrl.searchCriteria.searchOnParty = false;
                ctrl.searchCriteria.quoteReferenceNumber = null;
                ctrl.searchCriteria.firstName = null;
                ctrl.searchCriteria.surname = null;
                ctrl.searchCriteria.dateOfBirth = null;
                ctrl.searchCriteria.mobileNumber = null;
                ctrl.searchCriteria.homeNumber = null;
                ctrl.searchCriteria.email = null;
                ctrl.searchCriteria.externalCustomerReference = null;
            }
            else if (searchMode === searchModeParty) {
                ctrl.searchCriteria.searchOnQuoteReference = false;
                ctrl.searchCriteria.searchOnLeadId = false;
                ctrl.searchCriteria.searchOnParty = true;
                ctrl.searchCriteria.quoteReferenceNumber = null;
                ctrl.searchCriteria.leadId = null;
            }
        }

        function isQuotesResult(){
            return ctrl.searchResults.resultType === 'Quotes';
        }

        function isLeadsResult(){
            return ctrl.searchResults.resultType === 'Leads';
        }

        function hasQuoteResults() {
            if(ctrl.searchResults && ctrl.searchResults.quotes && isQuotesResult()){
                return ctrl.searchResults.quotes.length > 0;
            }
            return false;
        }

      

        function hasLeadResults() {
            if(ctrl.searchResults && ctrl.searchResults.quotes && isLeadsResult()){
                return ctrl.searchResults.quotes.length > 0;
            }
            return false;
        }

        function openQuote(quoteReferenceNumber) {
            $window.location = '/Policy/Edit/' + quoteReferenceNumber;
        }

        function createClient() {
           
            $uibModal.open({
                animation: true,
                templateUrl: '/client/app/components/createClientForm/createClient.modal.html',
                controller: 'createClientModalController',
                size: 'md',
                backdrop: false,
                resolve: {
                    defaultClientData : function() {
                        return $scope.searchData;
                    }
                }
            });

        }

        function init(){
            ctrl.searchCriteria = {
                searchOnQuoteReference: false,
                searchOnLeadId: false
            };
            ctrl.searchResults = getEmptySearchResults();

            ctrl.onSearchQuoteRefClick = searchByQuoteRefernce;
            ctrl.onSearchLeadClick = searchByLead;
            ctrl.onSearchPartyClick = searchByParty;

            ctrl.hasQuoteResults = hasQuoteResults;
            ctrl.hasLeadResults = hasLeadResults;
            ctrl.isQuotesResult = isQuotesResult;
            ctrl.isLeadsResult = isLeadsResult;
            ctrl.openQuote = openQuote;
            ctrl.searchPerformed = false;
            ctrl.onCreateClient = createClient;
        }

        init();
    }
);

