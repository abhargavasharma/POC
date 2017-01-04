using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class ReferralClient : SalesPortalApiClient
    {
        public async Task<Dictionary<string, IEnumerable<string>>> CreateReferralReturnErrorAsync(string quoteReference)
        {
            var uri = Client.CreateUri("api/policy/{0}/referral", quoteReference);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Create Referral");

            var response = await Client.PostAsync<Dictionary<string, IEnumerable<string>>, Dictionary<string, IEnumerable<string>>>(uri, default(Dictionary<string, IEnumerable<string>>), false);

            PerformanceTestTool.EndTransaction("API: Create Referral");

            PerformanceTestTool.LogInformation("API: Create Referral");

            return response;
        }

        public async Task<bool> CreateReferralAsync(string quoteReference, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/{0}/referral", quoteReference);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Create Referral");

            var response = await Client.PostAsync(uri, default(string), throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Create Referral");

            PerformanceTestTool.LogInformation("API: Create Referral");

            return response;
        }

        public async Task<bool> CompleteReferralAsync(string quoteReference, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri("api/policy/{0}/referral/complete", quoteReference);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Complete Referral");

            var response = await Client.PostAsync(uri, default(string), throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Complete Referral");

            PerformanceTestTool.LogInformation("API: Complete Referral");

            return response;
        }

        public async Task<TResponse> AssignReferralAsync<TResponse>(string quoteReference, AssignReferralRequest updatePlanRequest, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri("api/policy/{0}/assign", quoteReference);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Assign Referral");

            var response = await Client.PostAsync<AssignReferralRequest, TResponse>(uri, updatePlanRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Assign Referral");

            PerformanceTestTool.LogInformation("API: Assign Referral");

            return response;
        }
    }
}
