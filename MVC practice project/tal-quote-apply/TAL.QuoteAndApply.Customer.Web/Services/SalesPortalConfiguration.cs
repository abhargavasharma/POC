using System.Configuration;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface ICustomerPortalConfiguration
    {
        string AdobeTagManagerScriptUrl { get; }
    }

    public class CustomerPortalConfiguration : ICustomerPortalConfiguration
    {
        public string AdobeTagManagerScriptUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CustomerPortal.AdobeTagManagerScriptUrl"];
            }
        }
    }
}