using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters
{
    public interface IMaxCoverAmountParamConverter
    {
        MaxCoverAmountParam CreateFrom(PlanStateParam planOptionsParam);
        MaxCoverAmountParam CreateFrom(IRisk risk, IPlan plan, IPlan parentPlan, string brandKey);
    }

    public class MaxCoverAmountParamConverter : IMaxCoverAmountParamConverter
    {
        public MaxCoverAmountParam CreateFrom(PlanStateParam planOptionsParam)
        {
            return new MaxCoverAmountParam(planOptionsParam.PlanCode, planOptionsParam.BrandKey, planOptionsParam.Age, planOptionsParam.Income, planOptionsParam.CoverAmount, planOptionsParam.ParentPlanCoverCap);
        }

        public MaxCoverAmountParam CreateFrom(IRisk risk, IPlan plan, IPlan parentPlan, string brandKey)
        {
            int? parentPlanCoverCap = null;
            if (parentPlan != null)
            {
                parentPlanCoverCap = parentPlan.CoverAmount;
            }

            return new MaxCoverAmountParam(plan.Code, brandKey, risk.DateOfBirth.Age(), risk.AnnualIncome, plan.CoverAmount, parentPlanCoverCap);
        }
    }
}
