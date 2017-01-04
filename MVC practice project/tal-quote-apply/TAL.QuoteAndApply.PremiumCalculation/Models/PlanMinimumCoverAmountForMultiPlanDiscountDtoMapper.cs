using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PlanMinimumCoverAmountForMultiPlanDiscountDtoMapper : DbItemClassMapper<PlanMinimumCoverAmountForMultiPlanDiscountDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PlanMinimumCoverAmountForMultiPlanDiscountDto> mapper)
        {
            mapper.MapTable("PlanMinimumCoverAmountForMultiPlanDiscount");
        }
    }
}