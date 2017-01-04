using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class SuperAnnuationClient : SalesPortalApiClient
    {
        public async Task<IEnumerable<SuperFundSearchResult>> GetSearchSuperannuationByFundName(string fundName)
        {
            var uri = Client.CreateUri($"api/superannuation/search/organisation/{fundName}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get SuperannuationFunds";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<IEnumerable<SuperFundSearchResult>>(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }
    }
}
