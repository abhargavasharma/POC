using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IPolicyAnalyticsIdentifierService
    {
        void SetPolicyAnalyticsIdentifiersForPolicy(IPolicy policy, IAnalyticsIdentifiers analyticsIdentifiers);
    }

    public class PolicyAnalyticsIdentifierService : IPolicyAnalyticsIdentifierService
    {
        private readonly IPolicyAnalyticsIdentifierDtoRepository _policyAnalyticsIdentifierDtoRepository;

        public PolicyAnalyticsIdentifierService(IPolicyAnalyticsIdentifierDtoRepository policyAnalyticsIdentifierDtoRepository)
        {
            _policyAnalyticsIdentifierDtoRepository = policyAnalyticsIdentifierDtoRepository;
        }

        public void SetPolicyAnalyticsIdentifiersForPolicy(IPolicy policy, IAnalyticsIdentifiers analyticsIdentifiers)
        {
            var policyAnalyticsIdentifier = new PolicyAnalyticsIdentifierDto { PolicyId = policy.Id };

            if (analyticsIdentifiers != null)
            {
                policyAnalyticsIdentifier.SitecoreContactId = analyticsIdentifiers.SitecoreContactId;
            }

            _policyAnalyticsIdentifierDtoRepository.InsertPolicyAnalyticsIdentifier(policyAnalyticsIdentifier);
        }
    }
}
