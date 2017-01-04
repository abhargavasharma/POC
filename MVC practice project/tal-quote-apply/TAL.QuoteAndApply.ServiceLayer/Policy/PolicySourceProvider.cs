using System;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicySourceProvider
    {
        PolicySource From(string quoteReference);
        PolicySource From(int riskId);
    }

    public class PolicySourceProvider : IPolicySourceProvider
    {
        private readonly IPolicyService _policyService;
        private readonly IRiskService _riskService;

        public PolicySourceProvider(IPolicyService policyService, IRiskService riskService)
        {
            _policyService = policyService;
            _riskService = riskService;
        }

        public PolicySource From(string quoteReference)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            if (policy == null)
            {
                throw new ApplicationException("Cannot find quote");
            }

            return policy.Source;
        }

        public PolicySource From(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var policy = _policyService.Get(risk.PolicyId);

            if (policy == null)
            {
                throw new ApplicationException("Cannot find quote for risk");
            }

            return policy.Source;
        }
    }
}
