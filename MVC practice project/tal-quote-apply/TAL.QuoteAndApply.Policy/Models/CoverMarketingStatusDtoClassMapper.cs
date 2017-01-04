using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class CoverMarketingStatusDtoClassMapper : DbItemClassMapper<CoverMarketingStatusDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<CoverMarketingStatusDto> mapper)
        {
            mapper.MapTable("CoverMarketingStatus");
        }
    }
}