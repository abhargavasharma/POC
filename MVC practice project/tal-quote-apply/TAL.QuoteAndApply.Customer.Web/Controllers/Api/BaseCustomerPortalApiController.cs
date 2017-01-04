using System;
using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Web.Shared.Session;
using TAL.QuoteAndApply.Web.Shared.Workflow;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [ApplicationStepValidationFilter]
    public abstract class BaseCustomerPortalApiController : ApiController
    {
        protected readonly IQuoteSessionContext _quoteSessionContext;
        protected readonly IPolicyOverviewProvider _policyOverviewProvider;

        public Lazy<PolicyOverviewResult> PolicyOverview { get; }

        public string QuoteReferenceNumber => _quoteSessionContext.QuoteSession.QuoteReference;

        protected BaseCustomerPortalApiController(IQuoteSessionContext quoteSessionContext, IPolicyOverviewProvider policyOverviewProvider)
        {
            _quoteSessionContext = quoteSessionContext;
            _policyOverviewProvider = policyOverviewProvider;
            PolicyOverview = new Lazy<PolicyOverviewResult>(() => _policyOverviewProvider.GetFor(_quoteSessionContext.QuoteSession.QuoteReference));
        }

        public bool IsRiskValidForApplicationSession(int riskId)
        {
            var policyOverview = _policyOverviewProvider.GetFor(_quoteSessionContext.QuoteSession.QuoteReference);
            return policyOverview.Risks.Any(r => r.RiskId == riskId);
        }
    }
}