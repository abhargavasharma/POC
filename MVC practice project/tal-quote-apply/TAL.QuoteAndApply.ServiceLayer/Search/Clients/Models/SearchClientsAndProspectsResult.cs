using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models
{
    public class SearchClientsAndProspectsResult
    {
        public IReadOnlyList<SearchResult> SearchResults { get; }
        public SearchResultType ResultType { get; }

        public SearchClientsAndProspectsResult(IReadOnlyList<SearchResult> searchResults, SearchResultType resultType)
        {
            SearchResults = searchResults;
            ResultType = resultType;
        }
    }

    public enum SearchResultType
    {
        Quotes = 0,
        Leads = 1
    }
}