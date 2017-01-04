using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.ServiceLayer.Search.Models;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class SearchClient : CustomerPortalApiClient
    {
        public async Task<IList<SearchResult>> SearchAsync(string query, bool throwOnFailure = true, bool pause = true)
        {
            var uri = Client.CreateUri($"api/search/question/occupation?limit=30&query={query}");

            if(pause)
                PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Search Question Occupation";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<IList<SearchResult>>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }

    public class SearchResult
    {
        public string ResponseId { get; set; }
        public string Text { get; set; }
        public string HelpText { get; set; }
        public string ParentResponseId { get; set; }
        public string ParentText { get; set; }
        public float Score { get; set; }
    }

}