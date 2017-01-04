using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface ISalesPortalSummaryProvider
    {
        SalesPortalSummary GetFor(string quoteReference, int riskId);
    }

    public class SalesPortalSummaryProvider : ISalesPortalSummaryProvider
    {
        private readonly IRiskUnderwritingService _riskUnderwritingService;
        private readonly IPolicyPremiumSummaryProvider _policyPremiumSummaryProvider;
        private readonly IRiskPremiumSummaryViewModelConverter _riskPremiumSummaryViewModelConverter;
        private readonly IPolicyRiskPlanStatusProvider _policyRiskPlanStatusProvider;

        public SalesPortalSummaryProvider(IPolicyPremiumSummaryProvider policyPremiumSummaryProvider, IRiskPremiumSummaryViewModelConverter riskPremiumSummaryViewModelConverter, IPolicyRiskPlanStatusProvider policyRiskPlanStatusProvider, IRiskUnderwritingService riskUnderwritingService)
        {
            _policyPremiumSummaryProvider = policyPremiumSummaryProvider;
            _riskPremiumSummaryViewModelConverter = riskPremiumSummaryViewModelConverter;
            _policyRiskPlanStatusProvider = policyRiskPlanStatusProvider;
            _riskUnderwritingService = riskUnderwritingService;
        }

        public SalesPortalSummary GetFor(string quoteReference, int riskId)
        {
            var riskUnderwritingStatusResult = _riskUnderwritingService.GetRiskUnderwritingStatus(riskId);
            var uwStatus =
                UnderwritingCompleteResponse.CreateFrom(
                    riskUnderwritingStatusResult.CoverUnderwritingCompleteResults.Count(),
                    riskUnderwritingStatusResult.IsUnderwritingCompleteForRisk);

            //get the premium summary object
            var policyPremiumSummary = _policyPremiumSummaryProvider.GetFor(quoteReference);
            var riskPremium = _riskPremiumSummaryViewModelConverter.CreateFrom(riskId, policyPremiumSummary);

            var plansStatus = _policyRiskPlanStatusProvider.GetFor(quoteReference, riskId);

            return new SalesPortalSummary(uwStatus, riskPremium, plansStatus);
        }
    }
}