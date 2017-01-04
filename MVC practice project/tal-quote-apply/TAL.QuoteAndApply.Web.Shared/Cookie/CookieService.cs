using System;
using System.Web;
using TAL.QuoteAndApply.Infrastructure.Http;

namespace TAL.QuoteAndApply.Web.Shared.Cookie
{
    public interface ICookieService
    {
        string GetCookieValue(string name);
        void SetCookie(string name, string value, DateTime expires);
        void ClearCookie(string name);
    }

    public class CookieService : ICookieService
    {
        private readonly HttpContextBase _httpContext;

        public CookieService(IHttpContextProvider httpContextProvider)
        {
            _httpContext = httpContextProvider.GetCurrentContext();
        }

        public string GetCookieValue(string name)
        {
            var cookie = _httpContext.Request.Cookies.Get(name);

            if (cookie == null)
                return null;

            return cookie.Value;
        }

        public void SetCookie(string name, string value, DateTime expires)
        {
            var cookie = new HttpCookie(name, value)
            {
                Domain = _httpContext.Request.Url.Host,
                Expires = expires
            };

            var response = _httpContext.Response;
            response.Cookies.Remove(name);
            response.AppendCookie(cookie);
        }

        public void ClearCookie(string name)
        {
            SetCookie(name, null, DateTime.Now.AddDays(-1));
        }
    }
}
