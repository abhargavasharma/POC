using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class RiskMarketingStatusDtoClassMapper : DbItemClassMapper<RiskMarketingStatusDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<RiskMarketingStatusDto> mapper)
        {
            mapper.MapTable("RiskMarketingStatus");
        }
    }
}