using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface ISearchQuotesClientsAndProspectsService
    {
        SearchClientsAndProspectsResult Search(
            SearchQuotesClientsAndProspectsRequest searchRequest);
    }

    public class SearchQuotesClientsAndProspectsService : ISearchQuotesClientsAndProspectsService
    {
        private readonly IQuotesClientsAndProspectSearcherProvider _quotesClientsAndProspectSearcherProvider;

        public SearchQuotesClientsAndProspectsService(IQuotesClientsAndProspectSearcherProvider quotesClientsAndProspectSearcherProvider)
        {
            _quotesClientsAndProspectSearcherProvider = quotesClientsAndProspectSearcherProvider;
        }

        public SearchClientsAndProspectsResult Search(SearchQuotesClientsAndProspectsRequest searchRequest)
        {
            var searchSvc = _quotesClientsAndProspectSearcherProvider.GetFor(searchRequest);
            return searchSvc.Search(searchRequest);
        }
    }
}
