using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class PlanClient : SalesPortalApiClient
    {
        public async Task<RiskPlanDetailReposone> GetPlansAndCoversAsync(string policyNumber, int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/plan/getPlansAndCovers", policyNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get PlansAndCovers");

            var response = await Client.GetAsync<RiskPlanDetailReposone>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Get PlansAndCovers");

            PerformanceTestTool.LogInformation("API: Get PlansAndCovers Done");

            return response;
        }

        public async Task<TResponse> UpdateAsync<TResponse>(string policyNumber, int riskId, PlanUpdateRequest planUpdate, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/plan/edit", policyNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Edit Plan");

            var response = await Client.PostAsync<PlanUpdateRequest, TResponse>(uri, planUpdate, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Edit Plan");

            PerformanceTestTool.LogInformation("API: Edit Plan Done");

            return response;
        }

       
    }
}
