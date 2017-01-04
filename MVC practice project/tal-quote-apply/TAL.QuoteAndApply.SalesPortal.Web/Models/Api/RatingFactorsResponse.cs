using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class RatingFactorsResponse : SalesPortalSummary
    {
        public RatingFactorsRequest RatingFactors { get; }
        public bool IsCompleted { get; set; }

        public RatingFactorsResponse(RatingFactorsRequest ratingFactors, bool isCompleted, 
            UnderwritingCompleteResponse underwritingStatus, RiskPremiumSummaryViewModel riskPremiumSummary, PolicyRiskPlanStatusesResult policyRiskPlanStatusesResult) :
            base(underwritingStatus, riskPremiumSummary, policyRiskPlanStatusesResult)
        {
            RatingFactors = ratingFactors;
            IsCompleted = isCompleted;
        }

        public static RatingFactorsResponse From(RatingFactorsRequest ratingFactors, bool isCompleted,
            SalesPortalSummary salesPortalSummary)
        {
            return new RatingFactorsResponse(ratingFactors, isCompleted, salesPortalSummary.UnderwritingStatus, salesPortalSummary.RiskPremiumSummary, salesPortalSummary.PolicyRiskPlanStatusesResult);
        }
    }
}