using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface ISearchByQuoteReferenceNumberService : IQuotesClientsAndProspectSearcher
    { 
    }

    public class SearchByQuoteReferenceNumberService : ISearchByQuoteReferenceNumberService
    {
        private readonly IPolicySearchService _policySearchService;
        private readonly ISearchQuoteResultProvider _searchQuoteResultProvider;

        public SearchByQuoteReferenceNumberService(IPolicySearchService policySearchService, ISearchQuoteResultProvider searchQuoteResultProvider)
        {
            _policySearchService = policySearchService;
            _searchQuoteResultProvider = searchQuoteResultProvider;
        }

        public SearchClientsAndProspectsResult Search(SearchQuotesClientsAndProspectsRequest searchRequest)
        {
            if (string.IsNullOrEmpty(searchRequest.QuoteReferenceNumber))
            {
                return new SearchClientsAndProspectsResult(new List<SearchResult>(), SearchResultType.Quotes);
            }
            var policySearchResults = _policySearchService.SearchByQuoteReference(searchRequest.QuoteReferenceNumber);
            
            if (!policySearchResults.Any() || searchRequest.BrandId != null && policySearchResults.FirstOrDefault().BrandId != searchRequest.BrandId.Value)
            {
                return new SearchClientsAndProspectsResult(new List<SearchResult>(), SearchResultType.Quotes);
            }

            return new SearchClientsAndProspectsResult(policySearchResults.Select(_searchQuoteResultProvider.From).ToList(), SearchResultType.Quotes);
        }
    }
}