using System.Configuration;

namespace TAL.QuoteAndApply.Customer.Web.Configuration
{
    public interface IBrandSettingsProvider
    {
        string BrandKey { get; }
        string BrandCssClass { get; }
    }

    public class BrandSettingsProvider : IBrandSettingsProvider
    {
        public string BrandKey => ConfigurationManager.AppSettings["Application.Brand.Key"];

        public string BrandCssClass => ConfigurationManager.AppSettings["Application.Brand.CssClass"];
    }
}