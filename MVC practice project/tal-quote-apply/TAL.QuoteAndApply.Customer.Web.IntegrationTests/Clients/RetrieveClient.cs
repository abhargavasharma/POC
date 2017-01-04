using System;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models.Api;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class RetrieveClient : CustomerPortalApiClient
    {
        public async Task<TResponse> RetrieveQuoteAsync<TResponse>(RetrieveQuoteRequest retrieveQuoteRequest)
        {
            var uri = Client.CreateUri($"api/retrieve");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Retrieve Quote";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<RetrieveQuoteRequest, TResponse>(uri, retrieveQuoteRequest, false);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
        public async Task<PolicyReviewResponse> GetReviewForRiskAsync(int riskId, bool throwOnFailure = true, bool pause = true)
        {
            var uri = Client.CreateUri($"api/review/risk/{riskId}");

            if (pause)
                PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Review";
            PerformanceTestTool.StartTransaction(tran);
            
            var response = await Client.GetAsync<PolicyReviewResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}
