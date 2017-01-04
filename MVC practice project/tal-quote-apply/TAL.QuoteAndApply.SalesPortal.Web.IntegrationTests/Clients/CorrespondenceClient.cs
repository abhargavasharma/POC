using System.Threading.Tasks;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class CorrespondenceClient : SalesPortalApiClient
    {
        public async Task<TResponse> GetCorrespondenceSummaryAsync<TResponse>(string quoteReferenceNumber, int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/correspondence");

            const string tran = "API: Correspondence Summary";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<TResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<bool> SendCorrespondenceAsync(string quoteReferenceNumber, int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/correspond");

            const string tran = "API: Send Correspondence";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<bool>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}
