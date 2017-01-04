using System;
using System.Threading.Tasks;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.ApiClients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class RiskClient : SalesPortalApiClient
    {
        public async Task<RatingFactorsResponse> RatingFactorsAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/ratingfactors", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Risk Rating Factors");

            var response = await Client.GetAsync<RatingFactorsResponse>(uri, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Get Risk Rating Factors");

            PerformanceTestTool.LogInformation("API: Get Risk Rating Factors Done");

            return response;
        }

        public async Task<TResponse> RatingFactorsAsync<TResponse>(string quoteReferenceNumber, int riskId, RatingFactorsRequest ratingFactorsRequest)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/ratingfactors", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Post Risk Rating Factors");

            var response = await Client.PostAsync<RatingFactorsRequest, TResponse>(uri, ratingFactorsRequest, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Post Risk Rating Factors");

            PerformanceTestTool.LogInformation("API: Post Risk Rating Factors Done");

            return response;
        }

        public async Task<RiskPremiumSummaryViewModel> GetPremiumAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/premium");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Risk Premium");

            var response = await Client.GetAsync<RiskPremiumSummaryViewModel>(uri);

            PerformanceTestTool.EndTransaction("API: Get Risk Premium");

            PerformanceTestTool.LogInformation("API: Get Risk Premium Done");

            return response;
        }

        public async Task<PartyConsentRequest> PartyConsentAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/partyConsent", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Party Consent");

            var response = await Client.GetAsync<PartyConsentRequest>(uri, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Get Party Consent");

            PerformanceTestTool.LogInformation("API: Get Party Consent Done");

            return response;
        }

        public async Task PartyConsentAsync(string quoteReferenceNumber, int riskId, PartyConsentRequest partyConsentRequest)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/partyConsent", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Post Party Consent");

            await Client.PostAsync(uri, partyConsentRequest, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Post Party Consent");

            PerformanceTestTool.LogInformation("API: Post Party Consent Done");
        }

        public async Task<PersonalDetailsRequest> LifeInsuredPersonalDetailsAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/lifeInsuredDetails", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Life Insured Details");

            var response = await Client.GetAsync<PersonalDetailsRequest>(uri, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Get Life Insured Details");

            PerformanceTestTool.LogInformation("API: Get Life Insured Details");

            return response;
        }

        public async Task<TResponse> LifeInsuredPersonalDetailsAsync<TResponse>(string quoteReferenceNumber, int riskId, LifeInsuredDetailsRequest lifeInsuredDetailsRequest)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/lifeInsuredDetails", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Post Life Insured Details");

            var response = await Client.PostAsync<LifeInsuredDetailsRequest, TResponse>(uri, lifeInsuredDetailsRequest, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Post Life Insured Details");

            PerformanceTestTool.LogInformation("API: Post Life Insured Details");

            return response;
        }
    }
}