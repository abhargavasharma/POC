using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class ChatClient : CustomerPortalApiClient
    {
        public async Task<TResponse> RequestCallbackAsync<TRequest,TResponse>(TRequest request, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/chat/callback");

            const string tran = "API: Chat Callback";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<TRequest, TResponse>(uri, request, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<ChatAvailabilityResponse> RequestWebchatAsync(bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("/api/chat/availableAndCreateInteraction");

            const string tran = "API: Web Chat Request";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<ChatAvailabilityResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<ChatAvailabilityResponse> GetAvailabilityAsync(bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/chat/available");

            const string tran = "API: Chat Available";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<ChatAvailabilityResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

    }
}
