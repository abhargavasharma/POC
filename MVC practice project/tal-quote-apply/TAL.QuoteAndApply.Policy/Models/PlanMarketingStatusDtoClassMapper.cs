using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class PlanMarketingStatusDtoClassMapper : DbItemClassMapper<PlanMarketingStatusDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PlanMarketingStatusDto> mapper)
        {
            mapper.MapTable("PlanMarketingStatus");
        }
    }
}