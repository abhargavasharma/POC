using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class CoverDtoClassMapper : DbItemClassMapper<CoverDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<CoverDto> mapper)
        {
            mapper.MapTable("Cover");
            mapper.MapProperty(cover => cover.UnderwritingStatus, "UnderwritingStatusId");
        }
    }
}