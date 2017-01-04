using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class CoverLoadingDtoMapper : DbItemClassMapper<CoverLoadingDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<CoverLoadingDto> mapper)
        {
            mapper.MapTable("CoverLoading");
            mapper.MapProperty(cover => cover.LoadingType, "LoadingTypeId");
        }
    }
}