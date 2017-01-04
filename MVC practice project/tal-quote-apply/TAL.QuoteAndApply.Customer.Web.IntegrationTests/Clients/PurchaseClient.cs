using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models.Api;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class PurchaseClient : CustomerPortalApiClient
    {
        public async Task<PurchaseAndPremiumResponse> GetPurchaseOptionsAsync(bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/purchase");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Purchase";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<PurchaseAndPremiumResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<TResponse> PostPurchaseDetailsAsync<TResponse>(int riskId, PurchaseRequest purchaseRequest, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/purchase/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Post Purchase";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<PurchaseRequest, TResponse>(uri, purchaseRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}
