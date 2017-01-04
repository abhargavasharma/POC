using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.Web.Shared.Cookie;

namespace TAL.QuoteAndApply.Tests.Shared.Mocks
{
    public class MockCookieService : ICookieService
    {
        private static IDictionary<string, string> _cookieCollection = new Dictionary<string, string>();

        public string GetCookieValue(string name)
        {
            if (_cookieCollection.ContainsKey(name))
                return _cookieCollection[name];

            return null;
        }

        public void SetCookie(string name, string value, DateTime expires)
        {
            _cookieCollection[name] = value;
        }

        public void ClearCookie(string name)
        {
            if (_cookieCollection.ContainsKey(name))
                _cookieCollection.Remove(name);
        }
    }
}
