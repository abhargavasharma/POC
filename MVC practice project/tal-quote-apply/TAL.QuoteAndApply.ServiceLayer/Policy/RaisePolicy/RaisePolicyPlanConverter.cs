using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyPlanConverter
    {
        RaisePolicyPlan From(IPlan plan, string brandKey);
    }

    public class RaisePolicyPlanConverter : IRaisePolicyPlanConverter
    {
        public RaisePolicyPlan From(IPlan plan, string brandKey)
        {
            return new RaisePolicyPlan
            {
                Id = plan.Id,
                BrandKey = brandKey,
                RiskId = plan.RiskId,
                CoverAmount = plan.CoverAmount,
                PolicyId = plan.PolicyId,
                Selected = plan.Selected,
                Code = plan.Code,
                LinkedToCpi = plan.LinkedToCpi,
                Premium = plan.Premium,
                MultiCoverDiscount = plan.MultiCoverDiscount,
                MultiPlanDiscount = plan.MultiPlanDiscount,
                MultiPlanDiscountFactor = plan.MultiPlanDiscountFactor,
                WaitingPeriod = plan.WaitingPeriod,
                BenefitPeriod = plan.BenefitPeriod,
                OccupationDefinition = plan.OccupationDefinition,
                PremiumHoliday = plan.PremiumHoliday,
                PremiumType = plan.PremiumType
            };
        }
    }
}