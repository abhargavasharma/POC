using System;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.ApiClients;
using TAL.Performance.Infrastructure.Core.WebServers;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Tests.Shared.WebIntegration;

namespace TAL.QuoteAndApply.PerformanceEmulator.Utility
{
    public interface ISalesPortalClientFactory
    {
        T GetSalesPortalClient<T>(IWebServer webServer, IPerformanceTestTool performanceTestTool) where T : SalesPortalApiClient;
    }

    public class SalesPortalClientFactory : ISalesPortalClientFactory
    {
        private void AssignClientDependencies(SalesPortalApiClient client, IWebServer webServer, IPerformanceTestTool performanceTestTool)
        {
            client.AssignDependencies(new WebApiClient(webServer, new Json()), performanceTestTool);
        }

        public T GetSalesPortalClient<T>(IWebServer webServer, IPerformanceTestTool performanceTestTool) where T : SalesPortalApiClient
        {
            var retVal = Activator.CreateInstance<T>();
            AssignClientDependencies(retVal, webServer, performanceTestTool);
            return retVal;
        }
    }
}