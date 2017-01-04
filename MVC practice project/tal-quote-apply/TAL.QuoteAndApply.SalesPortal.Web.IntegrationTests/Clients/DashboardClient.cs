using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class DashboardClient : SalesPortalApiClient
    {
        public async Task<T> GetReferralsAsync<T>(bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/dashboard/referrals");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get UnderwritingDashboardReferrals";
            PerformanceTestTool.StartTransaction (tran);

            var response = await Client.GetAsync<T>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<TResponse> GetQuotesAsync<TResponse>(AgentDashboardRequest agentDashboardRequest, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/dashboard/quotes");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get AgentDashboardQuotes";
            PerformanceTestTool.StartTransaction(tran);
            
            var response = await Client.PostAsync<AgentDashboardRequest, TResponse>(uri, agentDashboardRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }
    }
}
