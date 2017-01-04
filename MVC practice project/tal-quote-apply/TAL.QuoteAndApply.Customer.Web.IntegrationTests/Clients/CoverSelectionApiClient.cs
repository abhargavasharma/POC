using System;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models.Api;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class CoverSelectionApiClient : CustomerPortalApiClient
    {
        public async Task<TResponse> GetPlanForRiskAsync<TResponse>(int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/coverselection/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            const string tran = "API: Get Plan for risk";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<TResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<TResponse> UpdatePlanAsync<TResponse>(int riskId, UpdatePlanRequest updatePlanRequest, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/coverselection/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Update Plan";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<UpdatePlanRequest, TResponse>(uri, updatePlanRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<TResponse> AttachRiderAsync<TResponse>(int riskId, AttachRiderRequest attachRiderRequest,
            bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/coverselection/risk/{riskId}/attach");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Attach Rider";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<AttachRiderRequest, TResponse>(uri, attachRiderRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<TResponse> DetachRiderAsync<TResponse>(int riskId, DetachRiderRequest detachRiderRequest,
            bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/coverselection/risk/{riskId}/detach");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Detach Rider";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<DetachRiderRequest, TResponse>(uri, detachRiderRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}