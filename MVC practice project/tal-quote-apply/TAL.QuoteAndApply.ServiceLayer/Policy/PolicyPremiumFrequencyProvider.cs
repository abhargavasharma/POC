using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyPremiumFrequencyProvider
    {
        PolicyPremiumFrequencyParam GetPremiumFrequency(string quoteReferenceNumber);
    }

    public class PolicyPremiumFrequencyProvider : IPolicyPremiumFrequencyProvider
    {
        private readonly IPolicyService _policyService;
        private readonly IProductDefinitionProvider _productDefinitionProvider;

        public PolicyPremiumFrequencyProvider(IPolicyService policyService, IProductDefinitionProvider productDefinitionProvider)
        {
            _policyService = policyService;
            _productDefinitionProvider = productDefinitionProvider;
        }

        public PolicyPremiumFrequencyParam GetPremiumFrequency(string quoteReferenceNumber)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            var availableFrequencies = _productDefinitionProvider.GetAvailablePremiumFrequencies(policy.BrandKey);
            return new PolicyPremiumFrequencyParam(quoteReferenceNumber, policy.PremiumFrequency, availableFrequencies);
        }
    }
}
