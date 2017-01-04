using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class PlanDtoClassMapper : DbItemClassMapper<PlanDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PlanDto> mapper)
        {
            mapper.MapTable("Plan");
            mapper.MapProperty(p => p.PremiumType, "PremiumTypeId");
        }
    }
}