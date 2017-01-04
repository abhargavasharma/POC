using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class SalesPortalSummary
    {
        public UnderwritingCompleteResponse UnderwritingStatus { get; }
        public RiskPremiumSummaryViewModel RiskPremiumSummary { get; }
        public PolicyRiskPlanStatusesResult PolicyRiskPlanStatusesResult { get; }

        public SalesPortalSummary(UnderwritingCompleteResponse underwritingStatus, RiskPremiumSummaryViewModel riskPremiumSummary, PolicyRiskPlanStatusesResult policyRiskPlanStatusesResult)
        {
            UnderwritingStatus = underwritingStatus;
            RiskPremiumSummary = riskPremiumSummary;
            PolicyRiskPlanStatusesResult = policyRiskPlanStatusesResult;
        }
    }
}