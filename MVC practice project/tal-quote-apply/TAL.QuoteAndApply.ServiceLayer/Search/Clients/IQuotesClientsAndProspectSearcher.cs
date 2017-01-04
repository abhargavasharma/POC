using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface IQuotesClientsAndProspectSearcher
    {
        SearchClientsAndProspectsResult Search(SearchQuotesClientsAndProspectsRequest searchRequest);
    }
}