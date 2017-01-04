using System;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyProgressProvider
    {
        PolicyProgressParam GetProgress(string quoteReferenceNumber);
    }

    public class PolicyProgressProvider : IPolicyProgressProvider
    {
        private readonly IPolicyService _policyService;

        public PolicyProgressProvider(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        public PolicyProgressParam GetProgress(string quoteReferenceNumber)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            return new PolicyProgressParam()
            {
                QuoteReferenceNumber = quoteReferenceNumber,
                Progress = policy.Progress
            };
        }
    }
}