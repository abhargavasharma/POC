using System.Configuration;
using System.Net;

namespace TAL.QuoteAndApply.Infrastructure.Configuration
{
    public interface IRestProxyProvider
    {
        WebProxy DefaultProxy { get; }
    }

    public class RestProxyConfigurationProvider : IRestProxyProvider
    {
        public WebProxy DefaultProxy => new WebProxy(ConfigurationManager.AppSettings["Rest.DefaultProxyUri"]);
    }
}