using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.ApiClients;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public abstract class SalesPortalApiClient
    {
        protected IWebApiClient Client { private set; get; }
        protected IPerformanceTestTool PerformanceTestTool { private set; get; }

        public void AssignDependencies(IWebApiClient client, IPerformanceTestTool performanceTestTool)
        {
            Client = client;
            PerformanceTestTool = performanceTestTool;
        }
    }
}