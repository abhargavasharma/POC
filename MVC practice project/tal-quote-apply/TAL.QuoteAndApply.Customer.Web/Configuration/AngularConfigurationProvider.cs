using System.Configuration;

namespace TAL.QuoteAndApply.Customer.Web.Configuration
{
    public interface IAngularConfigurationProvider
    {
        bool DebugEnabled { get; }
    }

    public class AngularConfigurationProvider : IAngularConfigurationProvider
    {
        public bool DebugEnabled => bool.Parse(ConfigurationManager.AppSettings["Angular.DebugEnabled"]);
    }
}