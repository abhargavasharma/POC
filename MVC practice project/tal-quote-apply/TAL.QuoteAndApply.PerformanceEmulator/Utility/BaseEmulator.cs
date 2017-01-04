using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.WebServers;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using PolicyClient = TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients.PolicyClient;

namespace TAL.QuoteAndApply.PerformanceEmulator.Utility
{
    public abstract class BaseEmulator
    {
        protected IWebServer WebServer { get; private set; }
        protected IPerformanceTestTool PerformanceTestTool { get; private set; }
        protected ISalesPortalClientFactory SalesPortalClientFactory { get; private set; }

        protected BaseEmulator(IWebServer webserver, IPerformanceTestTool performanceTestTool)
        {
            WebServer = webserver;
            PerformanceTestTool = performanceTestTool;
            SalesPortalClientFactory = new SalesPortalClientFactory();
        }

        public PolicyClient GetSalesPortalPolicyClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<PolicyClient>(WebServer, PerformanceTestTool);
        }

        public PlanClient GetSalesPortalPlanClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<PlanClient>(WebServer, PerformanceTestTool);
        }

        public PaymentClient GetSalesPortalPaymentClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<PaymentClient>(WebServer, PerformanceTestTool);
        }

        public RiskClient GetSalesPortalRiskClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<RiskClient>(WebServer, PerformanceTestTool);
        }

        public SearchClient GetSalesPortalSearchClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<SearchClient>(WebServer, PerformanceTestTool);
        }

        public UnderwritingClient GetSalesPortalUnderwritingClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<UnderwritingClient>(WebServer, PerformanceTestTool);
        }

        public NoteClient GetSalesPortalNoteClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<NoteClient>(WebServer, PerformanceTestTool);
        }

        public ReferenceClient GetSalesPortalReferenceClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<ReferenceClient>(WebServer, PerformanceTestTool);
        }

        public BeneficiaryClient GetSalesPortalBeneficiaryClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<BeneficiaryClient>(WebServer, PerformanceTestTool);
        }

        public LoginClient GetSalesPortalLoginClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<LoginClient>(WebServer, PerformanceTestTool);
        }

        public DashboardClient GetSalesPortalDasbhoardClient()
        {
            return SalesPortalClientFactory.GetSalesPortalClient<DashboardClient>(WebServer, PerformanceTestTool);
        }
    }
}
