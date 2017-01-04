using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface ISearchByPartyInformationService : IQuotesClientsAndProspectSearcher
    {
    }

    public class SearchByPartyInformationService : ISearchByPartyInformationService
    {
        private readonly IPolicySearchService _policySearchService;
        private readonly ISearchQuoteResultProvider _searchQuoteResultProvider;

        public SearchByPartyInformationService(IPolicySearchService policySearchService, ISearchQuoteResultProvider searchQuoteResultProvider)
        {
            _policySearchService = policySearchService;
            _searchQuoteResultProvider = searchQuoteResultProvider;
        }

        public SearchClientsAndProspectsResult Search(SearchQuotesClientsAndProspectsRequest searchRequest)
        {
            // if you do a search with blank values, you actually get all results
            if (string.IsNullOrEmpty(searchRequest.FirstName)
                && string.IsNullOrEmpty(searchRequest.Surname)
                && string.IsNullOrEmpty(searchRequest.Email)
                && string.IsNullOrEmpty(searchRequest.MobileNumber)
                && string.IsNullOrEmpty(searchRequest.HomeNumber)
                && string.IsNullOrEmpty(searchRequest.ExternalCustomerReference)
                && searchRequest.DateOfBirth == null)
            {
                return new SearchClientsAndProspectsResult(new List<SearchResult>(), SearchResultType.Quotes);
            }

            var policySearchResults = _policySearchService
                .SearchByPolicyOwnerDetails(searchRequest.FirstName,
                    searchRequest.Surname,
                    searchRequest.DateOfBirth,
                    searchRequest.MobileNumber,
                    searchRequest.HomeNumber,
                    searchRequest.Email,
                    searchRequest.ExternalCustomerReference,
                    searchRequest.BrandId);

            return new SearchClientsAndProspectsResult(policySearchResults.Select(_searchQuoteResultProvider.From).ToList(), SearchResultType.Quotes);
        }
    }
}