using System;
using System.Threading.Tasks;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.ApiClients;
using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Models.View;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{

    public class PolicyClient : SalesPortalApiClient
    {
        public async Task<CreateClientRequest> InitAsync()
        {
            var uri = Client.CreateUri("api/policy/init");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Init Policy";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<CreateClientRequest>(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<TResponse> CreateAsync<TResponse>(CreateClientRequest createClientRequest, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri("api/policy/create");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Create Policy";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<CreateClientRequest, TResponse>(uri, createClientRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<PolicyFremiumFrequencyViewModel> GetPolicyPremiumFrequencyAsync(string quoteReferenceNumber)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/premiumfrequency");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Policy Premium Type";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<PolicyFremiumFrequencyViewModel>(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<TResponse> UpdatePolicyPremiumFrequencyAsync<TResponse>(string quoteReferenceNumber, PolicyFremiumFrequencyViewModel policyFremiumFrequencyViewModel)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/premiumfrequency");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Update Policy Premium Type";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<PolicyFremiumFrequencyViewModel, TResponse>(uri, policyFremiumFrequencyViewModel);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<RetrievePolicyViewModel> GetSummaryAsync(string quoteReferenceNumber)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/summary");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Policy Summary";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<RetrievePolicyViewModel>(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<PolicyProgressViewModel> GetPolicyProgressAsync(string quoteReferenceNumber)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/progress");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Policy Progress";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<PolicyProgressViewModel>(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<TResponse> UpdatePolicyProgressAsync<TResponse>(string quoteReferenceNumber, PolicyProgressViewModel policyProgressViewModel)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/progress");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Update Policy Progress";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<PolicyProgressViewModel, TResponse>(uri, policyProgressViewModel);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<RetrievePolicyViewModel> EditAsync(string quoteReferenceNumber, QuoteEditSource quoteEditSource)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/edit/{quoteEditSource.ToString()}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Edit Policy");

            var response = await Client.GetAsync<RetrievePolicyViewModel>(uri);

            PerformanceTestTool.EndTransaction("API: Edit Policy");

            PerformanceTestTool.LogInformation("API: Edit Policy");

            return response;
        }

        public async Task<PolicyOwnerDetailsRequest> GetOwnerDetailsAsync(string quoteReferenceNumber)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/ownerDetails");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Owner Details";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<PolicyOwnerDetailsRequest>(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<TResponse> UpdateOwnerDetailsAsync<TResponse>(string quoteReferenceNumber, PolicyOwnerDetailsRequest policyOwnerDetailsRequest, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/ownerDetails");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Update Owner Details";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<PolicyOwnerDetailsRequest, TResponse>(uri, policyOwnerDetailsRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<TResponse> UpdatePolicyOwnerTypeAsync<TResponse>(string quoteReferenceNumber, string newOwnerType, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/ownerType/{newOwnerType}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Update Policy Owner Type";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<string, TResponse>(uri, newOwnerType, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }
    }
}
