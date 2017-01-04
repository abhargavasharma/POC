using System.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.Configuration
{
    public class BrandSettingsConfigurationSection
        : ConfigurationSection
    {

        public static BrandSettingsConfigurationSection GetConfig()
        {
            return (BrandSettingsConfigurationSection)System.Configuration.ConfigurationManager.GetSection("brandSettings") ?? new BrandSettingsConfigurationSection();
        }

        [System.Configuration.ConfigurationProperty("brands")]
        [ConfigurationCollection(typeof(BrandConfigurationElement), AddItemName = "brand")]
        public BrandsConfigurationElementCollection Brands
        {
            get
            {
                object o = this["brands"];
                return o as BrandsConfigurationElementCollection;
            }
        }

    }
}