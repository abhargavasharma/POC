using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Search.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class SearchClient : SalesPortalApiClient
    {
        public async Task<TResponse> SearchClientsAsync<TResponse>(SearchClientsRequest searchClientsRequest, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/search/clients");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Search Clients");

            var response = await Client.PostAsync<SearchClientsRequest, TResponse>(uri, searchClientsRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Search Clients");

            PerformanceTestTool.LogInformation("API: Search Clients Done");

            return response;
        }

        public async Task<IEnumerable<UnderwritingAnswerSearchResult>> SearchOccupationsAsync(string query, int limit)
        {
            var uri = Client.CreateUri($"api/search/question/occupation?limit={limit}&query={query}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Search Occupations");

            var response = await Client.GetAsync<IEnumerable<UnderwritingAnswerSearchResult>>(uri);

            PerformanceTestTool.EndTransaction("API: Search Occupations");

            PerformanceTestTool.LogInformation("API: Search Occupations Done");

            return response;
        }
    }
}