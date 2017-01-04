using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public interface IApplicationStepContextService
    {
        ApplicationStepContext Get();
    }

    public class ApplicationStepContextService : IApplicationStepContextService
    {
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IQuoteSessionContext _quoteSessionContext;
        private readonly IHttpContextProvider _httpContextProvider;

        public ApplicationStepContextService(IHttpContextProvider httpContextProvider, IPolicyOverviewProvider policyOverviewProvider, IQuoteSessionContext quoteSessionContext)
        {
            _httpContextProvider = httpContextProvider;
            _policyOverviewProvider = policyOverviewProvider;
            _quoteSessionContext = quoteSessionContext;
        }

        public ApplicationStepContext Get()
        {
            return new ApplicationStepContext(_policyOverviewProvider.GetFor(_quoteSessionContext.QuoteSession.QuoteReference), _httpContextProvider.GetCurrentContext());
        }
    }
}