using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Configuration.Models
{
    public class ConfigurationItemClassMapper : DbItemClassMapper<ConfigurationItem>
    {
        protected override void DefineMappings(ClassMapperWrapper<ConfigurationItem> mapper)
        {
            mapper.MapTable("ProductConfiguration");
            mapper.MapProperty(p => p.BrandId, "ProductBrandId");
        }
    }
}