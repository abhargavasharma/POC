using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class CoverExclusionDtoMapper : DbItemClassMapper<CoverExclusionDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<CoverExclusionDto> mapper)
        {
            mapper.MapTable("CoverExclusion");
        }
    }
}