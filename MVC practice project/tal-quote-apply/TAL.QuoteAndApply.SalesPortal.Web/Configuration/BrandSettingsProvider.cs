using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.UserRoles.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.Configuration
{
    public class BrandSettingsProvider : IBrandSettingsProvider
    {
        public IBrandConfiguration GetForBrand(string brandKey)
        {
            return BrandSettingsConfigurationSection.GetConfig().Brands[brandKey];
        }

        public IEnumerable<IBrandConfiguration> GetAllBrands()
        {
            return BrandSettingsConfigurationSection.GetConfig().Brands.Cast<BrandConfigurationElement>().Where(brand => brand.Enabled);
        }

        public string GetUnderwriterGroup()
        {
            return BrandSettingsConfigurationSection.GetConfig().Brands["TAL"].RoleSettings.UnderwritingGroup;
        }
    }
}