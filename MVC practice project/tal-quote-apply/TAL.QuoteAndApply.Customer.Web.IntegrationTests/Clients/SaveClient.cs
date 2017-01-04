using System;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models.Api;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class SaveClient : CustomerPortalApiClient
    {
        public async Task<TResponse> SaveDetailsAsync<TResponse>(int riskId, SaveCustomerRequest saveCustomerRequest, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/save/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: SaveGate Save Details";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<SaveCustomerRequest, TResponse>(uri, saveCustomerRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);
            return response;
        }

        public async Task SaveDetailsAsync(int riskId, SaveCustomerRequest saveCustomerRequest, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/save/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: SaveGate Save Details";

            PerformanceTestTool.StartTransaction(tran);

            await Client.PostAsync(uri, saveCustomerRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);            
        }

        public async Task<TResponse> SavePasswordAsync<TResponse>(int riskId, CreateLoginRequest createLoginRequest, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/save/risk/{riskId}/createlogin");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: SaveGate Create Password";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<CreateLoginRequest, TResponse>(uri, createLoginRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task SavePasswordAsync(int riskId, CreateLoginRequest createLoginRequest, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/save/risk/{riskId}/createlogin");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: SaveGate Create Password";
            PerformanceTestTool.StartTransaction(tran);

            await Client.PostAsync(uri, createLoginRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);
        }

        public async Task<TResponse> GetContactDetails<TResponse>(int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/save/risk/{riskId}/contactDetails");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            const string tran = "API: Get Contact Details For Risk";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<TResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}
