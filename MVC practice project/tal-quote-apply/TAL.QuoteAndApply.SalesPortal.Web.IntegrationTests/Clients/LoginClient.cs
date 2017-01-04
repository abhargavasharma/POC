using System;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class LoginClient : SalesPortalApiClient
    {
        public async Task<T> AttemptLoginAsync<T>(LoginRequest loginRequest, bool throwOnFailure)
        {
            var uri = Client.CreateUri("api/login");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Login");

            var response = await Client.PostAsync<LoginRequest, T>(uri, loginRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Login");

            PerformanceTestTool.LogInformation("API: Login");

            return response;
        }

        public async Task<T> GetBrandsAsync<T>(bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/login/brands");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Brands for user");

            var response = await Client.GetAsync<T>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Get Brands for user");

            PerformanceTestTool.LogInformation("API: Get Brands for user");

            return response;
        }

        public async Task SaveSelectedBrand(SaveBrandRequest saveBrandRequest, bool throwOnFailure)
        {
            var uri = Client.CreateUri("api/login/brand");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Save brand on session");

            await Client.PostAsync(uri, saveBrandRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Save brand on session");

            PerformanceTestTool.LogInformation("API: Save brand on session");
        }
    }
}
