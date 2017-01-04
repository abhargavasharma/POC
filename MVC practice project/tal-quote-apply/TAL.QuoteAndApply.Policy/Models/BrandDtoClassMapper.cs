using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class BrandDtoClassMapper : DbItemClassMapper<BrandDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<BrandDto> mapper)
        {
            mapper.MapTable("ProductBrand");
        }
    }
}