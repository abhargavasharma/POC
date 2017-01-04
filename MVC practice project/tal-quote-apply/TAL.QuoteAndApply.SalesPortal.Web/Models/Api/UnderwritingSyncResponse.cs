using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class UnderwritingSyncResponse : SalesPortalSummary
    {
        public UnderwritingSyncResponse(UnderwritingCompleteResponse underwritingStatus, RiskPremiumSummaryViewModel riskPremiumSummary, PolicyRiskPlanStatusesResult policyRiskPlanStatusesResult) :
            base(underwritingStatus, riskPremiumSummary, policyRiskPlanStatusesResult)
        {
        }

        public static UnderwritingSyncResponse From(SalesPortalSummary salesPortalSummary)
        {
            return new UnderwritingSyncResponse(salesPortalSummary.UnderwritingStatus, salesPortalSummary.RiskPremiumSummary, salesPortalSummary.PolicyRiskPlanStatusesResult);
        }
    }
}