using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyPremiumSummaryProvider
    {
        PolicyPremiumSummary GetFor(string quoteReferenceNumber);
    }

    public class PolicyPremiumSummaryProvider : IPolicyPremiumSummaryProvider
    {
        private readonly IPolicyWithRisksService _policyWithRisksService;
        private readonly IPolicyPremiumSummaryConverter _policyPremiumSummaryConverter;

        public PolicyPremiumSummaryProvider(IPolicyWithRisksService policyWithRisksService, IPolicyPremiumSummaryConverter policyPremiumSummaryConverter)
        {
            _policyWithRisksService = policyWithRisksService;
            _policyPremiumSummaryConverter = policyPremiumSummaryConverter;
        }

        public PolicyPremiumSummary GetFor(string quoteReferenceNumber)
        {
            var policyWithRisks = _policyWithRisksService.GetFrom(quoteReferenceNumber);
            return _policyPremiumSummaryConverter.CreateFrom(policyWithRisks);
        }
    }
}
