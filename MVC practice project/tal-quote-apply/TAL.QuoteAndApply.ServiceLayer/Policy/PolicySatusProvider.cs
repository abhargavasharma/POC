using System;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicySatusProvider
    {
        PolicyStatusParam GetStatus(string quoteReferenceNumber);
    }

    public class PolicySatusProvider : IPolicySatusProvider
    {
        private readonly IPolicyService _policyService;

        public PolicySatusProvider(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        public PolicyStatusParam GetStatus(string quoteReferenceNumber)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            PolicyStatus status;
            Enum.TryParse(policy.Status.ToString(), out status);
            return new PolicyStatusParam()
            {
                QuoteReferenceNumber = quoteReferenceNumber,
                Status = status,
                SaveStatus = policy.SaveStatus,
                Source = policy.Source
            };
        }
    }
}