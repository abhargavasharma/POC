using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface ISearchByLeadIdService : IQuotesClientsAndProspectSearcher
    {
    }

    public class SearchByLeadIdService : ISearchByLeadIdService
    {
        private readonly IPolicySearchService _policySearchService;
        private readonly ISearchQuoteResultProvider _searchQuoteResultProvider;
        private readonly IGetLeadService _getLeadService;

        public SearchByLeadIdService(IPolicySearchService policySearchService, ISearchQuoteResultProvider searchQuoteResultProvider, IGetLeadService getLeadService)
        {
            _policySearchService = policySearchService;
            _searchQuoteResultProvider = searchQuoteResultProvider;
            _getLeadService = getLeadService;
        }

        public SearchClientsAndProspectsResult Search(SearchQuotesClientsAndProspectsRequest searchRequest)
        {
            if (!searchRequest.LeadId.HasValue)
            {
                return new SearchClientsAndProspectsResult(new List<SearchResult>(), SearchResultType.Leads);
            }

            var policySearchResults = _policySearchService.SearchByLeadId(searchRequest.LeadId.Value);

            if (policySearchResults != null && !policySearchResults.Any())
            {
                var getLead = new List<GetLeadResult>() {_getLeadService.Get(searchRequest.LeadId.Value)};
                if (getLead.Any() && getLead.First() != null)
                {
                    return new SearchClientsAndProspectsResult(
                        getLead.Select(_searchQuoteResultProvider.From).ToList(), SearchResultType.Leads);
                }
            }
            //if there is a quote with the adobe id and it isn't of the same brand as the agent then don't display any results
            if (!policySearchResults.Any() || searchRequest.BrandId != null && policySearchResults.FirstOrDefault().BrandId != searchRequest.BrandId.Value)
            {
                return new SearchClientsAndProspectsResult(new List<SearchResult>(), SearchResultType.Quotes);
            }

            return new SearchClientsAndProspectsResult(policySearchResults.Select(_searchQuoteResultProvider.From).ToList(), SearchResultType.Quotes);
        }
    }
}
