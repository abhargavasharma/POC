using System.Web;
using TAL.QuoteAndApply.Infrastructure.Http;

namespace TAL.QuoteAndApply.Infrastructure.Url
{
    public interface ICurrentUrlProvider
    {
        string GetCurrentFullUrl();
        string GetCurrentBaseUrl();
    }

    public class CurrentUrlProvider : ICurrentUrlProvider
    {
        private readonly HttpContextBase _httpContextBase;

        public CurrentUrlProvider(IHttpContextProvider httpContextProvider)
        {
            _httpContextBase = httpContextProvider.GetCurrentContext();
        }

        public string GetCurrentFullUrl()
        {
            return _httpContextBase.Request.Url.AbsoluteUri;
        }

        public string GetCurrentBaseUrl()
        {
            return GetCurrentFullUrl().Replace(_httpContextBase.Request.Url.PathAndQuery, "/");
        }
    }
}
