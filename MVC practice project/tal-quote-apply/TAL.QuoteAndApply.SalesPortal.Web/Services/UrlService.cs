using System.Configuration;
using System.Web;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface IUrlService
    {
        string GetTalusUiUrl(string authToken, string concurrencyToken = null);
    }
    public class UrlService : IUrlService
    {
        private readonly IUnderwritingConfiguration _underwritingConfiguration;

        public UrlService(IUnderwritingConfiguration underwritingConfiguration)
        {
            _underwritingConfiguration = underwritingConfiguration;
        }

        public string GetTalusUiUrl(string authToken, string concurrencyToken = null)
        {
            var etagQueryString = string.Empty;
            if (concurrencyToken != null)
            {
                var encodedConcurrencyToken = HttpUtility.UrlEncode(concurrencyToken);
                etagQueryString =  $"?etag={encodedConcurrencyToken}";
            }
            
            return string.Format("{0}/#/token/{1}{2}", _underwritingConfiguration.TalusUiBaseUrl, authToken, etagQueryString);
        }
    }
}